#pragma once
#include <enable_hashmap.h>
#include <boost/thread/null_mutex.hpp>
#include <enable_smart_ptr.h>


template <class T, class M = boost::null_mutex>
class enable_object_manager
{
public:
	enable_object_manager(void){}
	virtual ~enable_object_manager(void)
	{
		clear();
	}

	virtual bool add_obj(int obj_id, boost::shared_ptr<T> obj)
	{
		boost::lock_guard<M> gurad(m_lock);

		auto it = obj_map.insert(std::make_pair(obj_id, obj));

		return it.second;
	}

	virtual boost::shared_ptr<T> find_objr(int obj_id)
	{
		auto it = obj_map.find(obj_id);
		if(it != obj_map.end())
			return it->second;

		return nullptr;
	}

	virtual bool remove_obj(int obj_id)
	{
		boost::lock_guard<M> gurad(m_lock);
		auto it = obj_map.find(obj_id);
		if(it != obj_map.end())
		{
			obj_map.erase(it);
			return true;
		}
		return false;
	}

	virtual void clear()
	{
		obj_map.clear();
	}

	int get_count()
	{
		return obj_map.size();
	}

	const ENABLE_MAP<int, boost::shared_ptr<T>>& get_map()
	{
		return obj_map;
	}
protected:
	M m_lock;
	ENABLE_MAP<int, boost::shared_ptr<T>> obj_map;
};

