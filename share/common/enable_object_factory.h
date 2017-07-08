#pragma once
#include <enable_singleton.h>
#include <enable_hashmap.h>
#include <enable_smart_ptr.h>
#include <exception>
#include <boost/lexical_cast.hpp>


class factory_base
{
public:
	factory_base(){}
	virtual ~factory_base(){}

	virtual boost::shared_ptr<void> create()=0;

	virtual bool is_from_gate(){return false;}

	virtual bool use_sessionid(){return false;}

	virtual bool packet_process(boost::shared_ptr<void> peer, boost::shared_ptr<void> msg) {return false;};

	virtual bool packet_process(boost::shared_ptr<void> peer, uint32_t sessionid, boost::shared_ptr<void> msg) {return false;};

	virtual bool packet_process(boost::shared_ptr<void> peer, boost::shared_ptr<void> player, boost::shared_ptr<void> msg) {return false;};

	virtual bool packet_process(uint32_t player_id, boost::shared_ptr<void> msg) {return false;};
};

template <class T>
class object_factory_handler : public factory_base
{
public:
	object_factory_handler(){}
	virtual ~object_factory_handler(){}

	virtual boost::shared_ptr<void> create()
	{
		return boost::make_shared<T>();
	}
};

class enable_object_factory
{
public:
	enable_object_factory(void):b_release(false)
	{}
	virtual ~enable_object_factory(void)
	{
		release();
	}

	void release()
	{
		value_map.clear();
		b_release = true;
	}

	void regedit_object(int id, boost::shared_ptr<factory_base> obj)
	{
		auto ret = value_map.insert(std::make_pair(id, obj));
		if(!ret.second)
		{
			//std::string err = "regedit_object error id:" + boost::lexical_cast<std::string>(id);
			throw new std::exception();	
		}
	}

	boost::shared_ptr<void> create(int id)
	{
		if(b_release) 
			return nullptr;

		auto it = value_map.find(id);
		if(it != value_map.end())
			return it->second->create();
		return nullptr;
	}

	boost::shared_ptr<factory_base> get_factroy(int id)
	{
		if(b_release) 
			return nullptr;

		auto it = value_map.find(id);
		if(it != value_map.end())
			return it->second;

		return nullptr;
	}

	const ENABLE_MAP<int, boost::shared_ptr<factory_base>>& get_map()
	{
		return value_map;
	}
private:
	ENABLE_MAP<int, boost::shared_ptr<factory_base>> value_map;
	bool b_release;
};

