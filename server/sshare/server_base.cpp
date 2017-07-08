#include "stdafx.h"
#include "server_base.h"
#include <boost/program_options.hpp>
#include <string>
#include "com_log.h"
#include <net/peer_tcp_server.h>
#include <net/peer_handler.h>
#include "enable_processinfo.h"
//#include <net/peer_tcp.h>
#include <google/protobuf/stubs/common.h>

using namespace boost;
using namespace boost::asio::ip;
using namespace boost::program_options;
using namespace std;

#ifdef WIN32
boost::function0<void> before_interrupt_quit_func2;
BOOL WINAPI console_ctrl_handler2(DWORD ctrl_type)
{
	switch (ctrl_type)
	{
	case CTRL_C_EVENT:
	case CTRL_BREAK_EVENT:
	case CTRL_CLOSE_EVENT:
	case CTRL_SHUTDOWN_EVENT:
		before_interrupt_quit_func2();
		return TRUE;
	default:
		return FALSE;
	}
}
#else
#include <signal.h>
#endif

server_base::server_base(void)
	:b_run(true)
	,m_ncount(1)
	,m_serverid(0)
	,m_signals(m_io_service)
	,m_groupid(0)
	,b_closing(false)
{	
	GOOGLE_PROTOBUF_VERIFY_VERSION;

	
	

#ifdef WIN32
	before_interrupt_quit_func2 = boost::bind(&server_base::s_exit, this);
	SetConsoleCtrlHandler(console_ctrl_handler2, TRUE);
#else
	m_signals.add(SIGINT);
	m_signals.add(SIGTERM);
#if defined(SIGQUIT)
	m_signals.add(SIGQUIT);
#endif // defined(SIGQUIT)

	m_signals.async_wait(boost::bind(&server_base::s_exit, this));
#endif

}


server_base::~server_base(void)
{
	google::protobuf::ShutdownProtobufLibrary();
}

bool server_base::s_init(int argc, _TCHAR* argv[])
{
	//解析命令
	options_description opts;
	opts.add_options()("cfg", value<string>(), "");
	opts.add_options()("title", value<string>(), "");

	variables_map vm;
	store(parse_command_line(argc, argv, opts), vm);

	//初始化dump
	if(vm.count("dump_test") > 0)		
		enable_minidump::Init(this, true);
	else
		enable_minidump::Init(this);

	string filename;
	if(vm.count("cfg") > 0)
	{
		//加载配置
		filename = vm["cfg"].as<string>();
		if(!xml_cfg.init(filename))
		{
			SLOG_ALERT << "init xml error: "<<filename;			
			return false;
		}

		// 读取游戏服务器ID
		if(xml_cfg.check("server_id"))
		{			
			m_groupid = xml_cfg.get<uint16_t>("server_id");
		}
	}
	else
	{
		SLOG_ALERT << "can't find cfg in cmdline";

		filename = enable_processinfo::get_processname() + ".xml";
		if(!xml_cfg.init(filename))
		{
			SLOG_ALERT << "init xml error: "<<filename;			
			return false;
		}
	}
#ifdef WIN32
	if(vm.count("title") > 0)
	{
		std::string title = vm["title"].as<string>();
		SetConsoleTitleA(title.c_str());
	}
#endif
	//初始化log
	{

		size_t npos = filename.find_last_of('\\');
		if(npos != std::string::npos)
			filename = filename.erase(0, npos+1);
		npos = filename.find_last_of('.');
		if(npos != std::string::npos)
			filename = filename.erase(npos);
		com_log::InitLog(filename);

		if(vm.count("loglvl") > 0)
		{
			int loglvl = vm["loglvl"].as<int>();
			if(loglvl>=slog_emergency && loglvl<=slog_debug)
				com_log::SetLevel((severity_levels)loglvl);
		}
	}

	SLOG_INFO << " server starting..." ;

	//初始化socket
	if(!xml_cfg.check("out_port"))
		return false;

	m_serverid = xml_cfg.get<uint16_t>("out_port");

	uint16_t type = xml_cfg.get<uint16_t>("server_type");
	if(type == 1)//ipv6
	{
		tcp_server.reset(new peer_v6_server(m_io_service, m_serverid, boost::bind(&server_base::post_accept, this)));
	}
	else if(type == 2)//ipv4+ipv6    使用同一端口
	{
		tcp_server.reset(new peer_v4v6_server(m_io_service, m_serverid, boost::bind(&server_base::post_accept, this)));
	}
	else//ipv4
	{
		tcp_server.reset(new peer_v4_server(m_io_service, m_serverid, boost::bind(&server_base::post_accept, this)));
	}

	

	int theardcount = 1;
	if(xml_cfg.check("mutli_thread"))
	{
		theardcount = xml_cfg.get<int>("mutli_thread");
		if(theardcount < 2)
			theardcount = boost::thread::hardware_concurrency();
	}
	//theardcount = boost::thread::hardware_concurrency();
	std::cout << "当前线程 数：" << theardcount << std::endl;
	for (int i = 0;i<theardcount;i++)
	{
		work_grp.create_thread(boost::bind(&server_base::io_run, this));
		if(tcp_server->check_accept_v4(true))
			tcp_server->accept_v4(create_peer(), true);	

		if (tcp_server->check_accept_v6(true))
			tcp_server->accept_v6(create_peer(), true);
	}

	if(!on_init())
	{
		SLOG_ALERT<< "on_init error!";
		return false;
	}

	return s_run();
}
static bool IO_RUN = true;
bool server_base::s_run()
{
	__ENTER_FUNCTION
		SLOG_INFO << " server runing...";
	run();

	on_exit();
	com_log::flush();
	m_io_service.stop();
	m_timer_service.stop();
	IO_RUN = false;
	boost::this_thread::sleep(posix_time::milliseconds(300));

	__LEAVE_FUNCTION
		return true;
}


void server_base::s_exit()
{
	SLOG_INFO << " server exiting..." ;
	tcp_server->close();
	boost::this_thread::sleep(posix_time::milliseconds(300));


	b_run = false;
	work_grp.join_all();
}

void server_base::close()
{
	b_closing = true;
}

void server_base::io_run()
{
	__ENTER_FUNCTION
	int cout = 0;
	do 
	{
		if(!IO_RUN)
			return;

		cout = m_io_service.poll();
		//std::cout << "io run: count:" << cout << std::endl;
		if (cout == 0)
		{
			boost::this_thread::sleep(posix_time::milliseconds(1));
		}
	} while (cout >= 0);

	__LEAVE_FUNCTION
}

void server_base::run_timer()
{
	__ENTER_FUNCTION

		int cout = 0;
	do 
	{
		if(!IO_RUN)
			return;

		cout = m_timer_service.poll();

	} while (cout > 0);

	__LEAVE_FUNCTION
}

void server_base::post_accept()
{
	__ENTER_FUNCTION

	if(tcp_server->check_accept_v4())
		tcp_server->accept_v4(create_peer());	

	if(tcp_server->check_accept_v6())
		tcp_server->accept_v6(create_peer());	

	__LEAVE_FUNCTION
}

bool server_base::is_runing()
{
	if(b_closing)
	{
		//tcp_server->close();
		//this_thread::sleep(posix_time::milliseconds(50));
		//on_exit();
		//com_log::flush();
		//IO_RUN = false;
		//m_io_service.stop();
		//m_timer_service.stop();
		//this_thread::sleep(posix_time::milliseconds(50));
		////std::exit(0);

		com_log::flush();
		boost::this_thread::sleep(posix_time::milliseconds(200));
#ifdef WIN32
		ExitProcess(0);
#else
		std::exit(0);
#endif

		b_run = b_closing = false;		 
	}



	return b_run;
}

uint16_t server_base::generate_id()
{
	m_ncount++;
	return id_queue.generate_id();
}

void server_base::push_id(uint16_t peer_id)
{
	m_ncount--;
	id_queue.release_id(peer_id);
}

uint16_t server_base::get_peer_count()
{
	return m_ncount;
}

#include "server_timer.h"
void server_base::add_server_timer(boost::function0<void> func, int s)
{
	if(s<0)
		return;
	auto tt = boost::make_shared<server_timer>(m_timer_service);
	tt->init(func, s);
}

uint16_t server_base::get_groupid()
{
	return m_groupid;
}
void server_base::set_groutid(uint16_t v)
{
	m_groupid = v;
}
