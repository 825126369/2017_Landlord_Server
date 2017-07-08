#ifndef XK_Random_H
#define XK_Random_H

#include <stdio.h>  
#include <time.h>  
#include <iostream>
#include <math.h>
#include <string>
#include <cassert>

class xk_Random
{
public:
	xk_Random(uint64_t seed)
	{
		rand_seed = seed;
	}

	uint64_t rand(uint64_t min, uint64_t max)
	{
		assert(max>min && min>=0);

		uint64_t A = 0x5DEECE66D;
		uint64_t C = 0xB;
		uint64_t M = ((uint64_t)1 << 48);
		rand_seed = (rand_seed * A + C) % M;

		uint64_t bb = max - min;
		uint64_t value = rand_seed%bb + min;
		return value;
	}

	std::string rand_str(std::string data, int count)
	{
		std::string retstr;
		for (uint32_t i = 0; i<count; i++)
		{
			retstr += data[rand(0, data.length())];
		}
		return retstr;
	}
private:
	uint64_t rand_seed;
};

#endif