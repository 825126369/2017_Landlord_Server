#pragma once
#include <boost/cstdint.hpp>
#include <boost/smart_ptr.hpp>
#include <enable_object_manager.h>
#include <enable_singleton.h>

template <class T>
class packet_factory : public object_factory_handler<T>
{
public:
	bool packet_process()
};


class peer_manager:
	public enable_object_manager<>
{

};