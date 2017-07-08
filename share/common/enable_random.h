#pragma once
#include <boost/random.hpp>
#include <enable_singleton.h>
#include <boost/thread.hpp>
#include <string>

template<class T = boost::rand48>
class enable_random
{
public:
	enable_random(int seed)
		:m_rand(seed)
	{

	}

	std::string rand_str(const std::string& data, uint32_t count)
	{
		int maxrange = data.length();
		if(maxrange<0)
			maxrange = 0;

		boost::uniform_smallint<> ui(0, maxrange);
		std::string retstr;
		for (uint32_t i =0 ;i<count; i++)
		{
			retstr += data[ui(m_rand)];
		}
		return retstr;
	}

	int rand_100()
	{
		static boost::uniform_smallint<> ui(0,100);
		return ui(m_rand);
	}

	int rand_1w()
	{
		static boost::uniform_int<> ui(0,10000);
		return ui(m_rand);
	}

	int rand_int(int min, int max)
	{
		boost::uniform_int<> ui(min,max);
		return ui(m_rand);
	}

	double rand_01()
	{
		static boost::uniform_01<> ui;
		return ui(m_rand);
	}

	double rand_double(double min, double max)
	{
		boost::uniform_real<> ui(min, max);
		return ui(m_rand);
	}

private:
	T m_rand;
};

//全局随机
class global_random
	:public enable_singleton<global_random>
{
public:
	std::string rand_str(const std::string& data, uint32_t count)
	{
		return get_gen().rand_str(data, count);
	}

	int rand_100()
	{
		return get_gen().rand_100();
	}

	int rand_1w()
	{		
		return get_gen().rand_1w();
	}

	// 随机一个数，范围[min, max]
	int rand_int(int min, int max)
	{
		return get_gen().rand_int(min, max);
	}

	double rand_01()
	{
		return get_gen().rand_01();
	}

	double rand_double(double min, double max)
	{
		return get_gen().rand_double(min, max);
	}

private:
	boost::thread_specific_ptr<enable_random<boost::mt19937>> m_rand;

	enable_random<boost::mt19937>& get_gen()
	{
		enable_random<boost::mt19937> *pRng = m_rand.get();
		if (pRng) return *pRng;

		m_rand.reset(new enable_random<boost::mt19937>((int)std::time(nullptr)));
		return *m_rand.get();
	}
};
