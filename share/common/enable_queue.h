#pragma once
#pragma warning(disable:4996)

#include <boost/thread/mutex.hpp>
//#define USE_LIST

#ifndef USE_LIST
#include <queue>
template<class T>
class enable_queue : public std::queue<T>
{
public:
	void clear()
	{
		std::queue<T>::c.clear();
	}

	bool pop(T& v)
	{
		v = std::queue<T>::front();
		std::queue<T>::pop();
		return true;
	}
};

template<class T>
class enable_safe_queue : public std::queue<T>
{
public:
	void clear()
	{
		boost::lock_guard<boost::mutex> lock(m_mutex);
		std::queue<T>::c.clear();
	}

	bool safe_pop(T& v)
	{
		boost::lock_guard<boost::mutex> lock(m_mutex);
		if (!std::queue<T>::empty())
		{
			this->pop(v);
			return true;
		}
		return false;
	}

	void safe_push(T& v)
	{
		boost::lock_guard<boost::mutex> lock(m_mutex);
		std::queue<T>::push(v);
	}

	void safe_swap(enable_queue<T> q)
	{
		boost::lock_guard<boost::mutex> lock(m_mutex);
		swap(q);
	}
protected:
	bool pop(T& v)
	{
		v = std::queue<T>::front();
		std::queue<T>::pop();
		return true;
	}
private:
	boost::mutex m_mutex;
};

#else
#include <list>

template<class T>
class enable_queue : public std::list<T>
{
public:	
	bool pop(T& v)
	{
		v = front();
		erase(begin());
		return true;
	}

	void push(T& v)
	{
		push_back(v);
	}
};

template<class T>
class enable_safe_queue : public std::queue<T>
{
public:
	bool pop(T& v)
	{
		v = front();
		erase(begin());
		return true;
	}

	void push(T& v)
	{
		push_back(v);
	}

	void safe_pop(T& v)
	{
		boost::lock_guard<boost::mutex> lock(m_mutex);
		pop(v);
	}

	void safe_push(T& v)
	{
		boost::lock_guard<boost::mutex> lock(m_mutex);
		push(v);
	}

	void safe_swap(enable_safe_queue<T> q)
	{
		boost::lock_guard<boost::mutex> lock(m_mutex);
		swap(q);
	}
private:
	boost::mutex m_mutex;
};

#endif



#include <boost/lockfree/queue.hpp> 
#include <boost/lockfree/spsc_queue.hpp> 
#include <boost/atomic.hpp>
//无锁线程安全队列

template<class T>
class fast_safe_queue : public boost::lockfree::spsc_queue<T>
{
public:
	//queue必须先定义队列数量 过大会初始化很慢
	fast_safe_queue(uint16_t maxqueue = std::numeric_limits<int16_t>::max())
		:boost::lockfree::spsc_queue<T>(maxqueue)
	{

	}
	~fast_safe_queue()
	{
		//clear();
	}


	//必须在停止使用后调用 否则无法完全清理掉
	void clear()
	{
		T temp;
		while(!boost::lockfree::spsc_queue<T>::empty())
		{
			if(!boost::lockfree::spsc_queue<T>::pop(temp))
				break;
		}
	}
};

template<class T>
class fast_count_queue : public boost::lockfree::spsc_queue<T>
{
public:
	//queue必须先定义队列数量 过大会初始化很慢
	fast_count_queue(uint16_t maxqueue = std::numeric_limits<int16_t>::max())
		:boost::lockfree::spsc_queue<T>(maxqueue),m_count(0)
	{

	}
	~fast_count_queue()
	{
		//clear();
	}

	//必须在停止使用后调用 否则无法完全清理掉
	void clear()
	{
		T temp;
		while(!boost::lockfree::spsc_queue<T>::empty())
		{
			if(!safe_pop(temp))
				break;
		}
	}

	bool safe_pop(T& v)
	{
		if(boost::lockfree::spsc_queue<T>::pop(v))
		{
			m_count--;
			return true;
		}
		return false;
	}

	bool safe_push(const T& v)
	{
		if(boost::lockfree::spsc_queue<T>::push(v))
		{
			m_count++;
			return true;
		}
		return false;
	}

	int size()
	{
		return m_count;
	}

private:
	boost::atomic_int m_count; 
};

//释放资源时可能出问题
template<class T>
class fast_mutli_queue : public boost::lockfree::queue<T>
{
public:
	//queue必须先定义队列数量 过大会初始化很慢
	fast_mutli_queue(uint16_t maxqueue = std::numeric_limits<int16_t>::max())
		:boost::lockfree::queue<T>(maxqueue)
	{

	}
	~fast_mutli_queue()
	{
		//clear();
	}


	//必须在停止使用后调用 否则无法完全清理掉
	void clear()
	{
		T temp;
		while(!boost::lockfree::queue<T>::empty())
		{
			if(!boost::lockfree::queue<T>::pop(temp))
				break;
		}
	}
};