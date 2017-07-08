#pragma once
#include <boost/noncopyable.hpp>
#include <enable_smart_ptr.h>

#ifdef WIN32
#include <boost/thread.hpp>
#else
#include <mutex>
#include <thread>
#endif


template<class T>
class enable_singleton : private boost::noncopyable
{
public:
	static T& instance()
	{
		#ifdef WIN32
		boost::call_once(init, flag);
#else
		std::call_once(flag, init);
#endif
		return *t;
	}

	static void init()
	{
		t.reset(new T());
	}

	static void release()
	{
		t.reset();
	}
protected:
	enable_singleton(){}
	~enable_singleton(){}

private:
	static boost::scoped_ptr<T> t;
	#ifdef WIN32
	static boost::once_flag flag;
#else
	static std::once_flag flag;
#endif
};

template <class T> boost::scoped_ptr<T> enable_singleton<T>::t(nullptr);
#ifdef WIN32
template <class T> boost::once_flag enable_singleton<T>::flag = BOOST_ONCE_INIT;
#else
template <class T> std::once_flag enable_singleton<T>::flag;
#endif