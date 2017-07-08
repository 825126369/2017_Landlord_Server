#pragma once
#include <limits>
#include <boost/atomic.hpp>
#include <enable_queue.h>

//无锁线程安全的id管理
template<typename T>
class enable_id_generate
{
public:
	enable_id_generate()
	{
		clear();
	}
	~enable_id_generate()
	{
		clear();
	}

	void clear()
	{
		id_queue.clear();
		m_cur_id = std::numeric_limits<T>::min();
		m_cur_id++;//最小值为无效值
	}

	T generate_id()
	{
		if(id_queue.empty())
		{			
			return m_cur_id++;
		}
		else
		{
			T id;
			id_queue.safe_pop(id);
			return id;
		}
	}

	void release_id(T id)
	{
		id_queue.safe_push(id);
	}
private:
	boost::atomic<T> m_cur_id;
	enable_safe_queue<T> id_queue; 
};