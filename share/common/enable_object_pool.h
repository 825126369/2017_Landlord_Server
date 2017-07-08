#pragma once
#include <boost/pool/pool.hpp>
#include <boost/smart_ptr.hpp>
#include <boost/bind.hpp>
#include <boost/thread.hpp>
#include <boost/thread/null_mutex.hpp>

//线程安全 boost::mutex
//非线程使用 boost::null_mutex
template<class obj_type, class M = boost::null_mutex>
class enable_obj_pool
{
	static boost::pool<> m_pool;
	static M m_lock;

public:
	enable_obj_pool(){}
	virtual ~enable_obj_pool(){}

	typedef boost::shared_ptr<obj_type> ObjPtr;

	static boost::shared_ptr<obj_type> malloc()
	{
		boost::lock_guard<M> gurad(m_lock);
		void * mem = m_pool.malloc();
		if(!mem)
			return nullptr;

		obj_type* pobj = new(mem) obj_type();

		return boost::shared_ptr<obj_type>(pobj, boost::bind(&obj_type::free, _1));;
	}

	static void free(obj_type* pobj)
	{
		boost::lock_guard<M> gurad(m_lock);
		pobj->~obj_type();
		m_pool.free(pobj);
	}

	//手动释放未使用的内存
	static void release()
	{
		boost::lock_guard<M> gurad(m_lock);
		m_pool.release_memory();
	}

	static ObjPtr EmptyPtr;
};

#define enable_obj_pool_init(c_type, m_type)
template <class c_type, class m_type>\
boost::pool<> enable_obj_pool<c_type, m_type>::m_pool(sizeof(c_type));\
template <class c_type, class m_type>\
m_type enable_obj_pool<c_type, m_type>::m_lock;\
template <class c_type, class m_type>\
boost::shared_ptr<c_type> enable_obj_pool<c_type, m_type>::EmptyPtr;


#ifndef CONVERT_POINT
#define CONVERT_POINT(dectype, srcptr) boost::static_pointer_cast<dectype>(srcptr)
#endif
