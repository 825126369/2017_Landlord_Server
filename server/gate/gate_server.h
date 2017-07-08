#pragma once
#include "server_base.h"
#include <enable_singleton.h>
#include <net/msg_queue.h>

class gate_server:
	public server_base,
	public enable_singleton<gate_server>
{
public:
	gate_server(void);
	~gate_server(void);

	virtual bool DumpCallback(const wchar_t* dump_path,
		const wchar_t* minidump_id,
		void* context,
		EXCEPTION_POINTERS* exinfo,
		MDRawAssertionInfo* assertion,
		bool succeeded);

	virtual void run();

	virtual boost::shared_ptr<peer_tcp> create_peer();

	boost::shared_ptr<peer_tcp> create_peer(uint16_t remote_type);
protected:
	virtual bool on_init();
	virtual void on_exit();

private:	
	boost::mutex m_createlock;
	void init_packet();

	void connect_monitor();	
};

