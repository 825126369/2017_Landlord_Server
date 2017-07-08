#pragma once
#include <boost/cstdint.hpp>
#include <enable_smart_ptr.h>

enum e_peer_state
{	
	e_ps_disconnected = 0,
	e_ps_disconnecting,
	e_ps_connecting,
	e_ps_connected,	
	e_ps_accepting,
	//e_ps_accepted,
};

enum e_peer_event
{
	e_pe_disconnected = 0,
	e_pe_connected,	
	e_pe_connectfail,
	e_pe_accepted,
	e_pe_acceptfail,
	e_pe_recved,
	//e_pe_timeout,

	e_pe_only_remove,
};