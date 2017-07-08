
#pragma once
#include <net/msg_queue.h>
typedef fast_mutli_queue<peer_event> peer_event_queue;

class server_manager_handler
{
public:
	server_manager_handler(){};
	virtual ~server_manager_handler()
	{
		event_queue.clear();
	}
	void push_event(int _id, e_peer_event _event)
	{
		event_queue.push(peer_event(_id, _event));
	}

	virtual void exec_event() =0;

protected:
	peer_event_queue* get_event_list()
	{
		return &event_queue;
	}

private:	
	peer_event_queue event_queue;
};

class session_helper
{
public:
	static uint32_t get_sessionid(uint16_t gateid, uint16_t peerid)
	{
		uint32_t ret =  gateid <<16;
		return ret + peerid;
	};

	static uint16_t get_gateid(uint32_t sessionid)
	{
		return sessionid>>16;
	};

	static uint16_t get_peerid(uint32_t sessionid)
	{
		return sessionid&0x0000ffff;
	};
};