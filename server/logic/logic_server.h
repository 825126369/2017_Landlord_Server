#pragma once
#include "server_base.h"
#include <enable_singleton.h>
#include <net/msg_queue.h>

class i_game_engine;
class logic_server:
	public server_base,
	public enable_singleton<logic_server>
{
public:
	logic_server(void);
	~logic_server(void);

	virtual bool DumpCallback(const wchar_t* dump_path,
		const wchar_t* minidump_id,
		void* context,
		EXCEPTION_POINTERS* exinfo,
		MDRawAssertionInfo* assertion,
		bool succeeded);

	virtual void run();

	virtual boost::shared_ptr<peer_tcp> create_peer();

	boost::shared_ptr<peer_tcp> create_peer(uint16_t remote_type);

	//uint16_t get_groupid();

	void init_game_engine();

protected:
	virtual bool on_init();
	virtual void on_exit();

private:	
	boost::mutex m_createlock;
	void init_packet();
	bool init_config();
	bool init_db();
	void connect_monitor();	
	i_game_engine* m_engine;
};

