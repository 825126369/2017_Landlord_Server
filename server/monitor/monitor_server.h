#pragma once
#include "server_base.h"
#include <enable_singleton.h>
#include <net/msg_queue.h>
#include <http/http_server.hpp>

class monitor_peer;
class monitor_server:
	public server_base,
	public enable_singleton<monitor_server>
	,public http::server::request_interface
{
public:
	monitor_server(void);
	~monitor_server(void);

	virtual bool DumpCallback(const wchar_t* dump_path,
		const wchar_t* minidump_id,
		void* context,
		EXCEPTION_POINTERS* exinfo,
		MDRawAssertionInfo* assertion,
		bool succeeded);

	virtual void run();

	virtual boost::shared_ptr<peer_tcp> create_peer();


	//http¥¶¿Ì
	virtual bool handle_request(const std::string& info);
protected:
	virtual bool on_init();
	virtual void on_exit();

private:	
	boost::mutex m_createlock;
	void init_packet();

	boost::shared_ptr<http::server::http_server> m_httpserver;
	boost::mutex io_mutex;
};

