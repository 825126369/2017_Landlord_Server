#pragma once
#include <string>
#include <boost/cstdint.hpp>

class enable_processinfo
{
public:
	static std::string get_processname();

	static uint32_t get_tick_count();
};

