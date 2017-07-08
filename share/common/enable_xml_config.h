#pragma once
#include <enable_hashmap.h>
#include <string>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/xml_parser.hpp> 
#include <boost/lexical_cast.hpp>

class enable_xml_config
{
public:
	enable_xml_config():b_init(false)
	{
	}
	bool init(const std::string& filename)
	{		
		boost::property_tree::ptree root;
		try
		{
			boost::property_tree::xml_parser::read_xml(filename, pt, boost::property_tree::xml_parser::trim_whitespace);
			root = pt.get_child("root");
			filepath = filename;
		}
		catch (...)
		{
			return false;
		}
		value_map.clear();
		for (auto itr = root.begin(); itr!=root.end(); ++itr)
		{
			value_map.insert(std::make_pair(itr->first, itr->second.data()));
		}
		b_init = true;
		return b_init;
	}

	bool check(const std::string& skey) {
		auto it = value_map.find(skey);
		if(it == value_map.end())
			return false;
		return true;
	}

	template<class T>
	T get(const std::string& skey) {
		try
		{
			if(check(skey))
				return boost::lexical_cast<T>(value_map[skey]);	
		}
		catch (...)
		{
		}
		return T();
	}

	template<class T>
	void add(const std::string& key, T  v)
	{
		value_map.insert(std::make_pair(key, boost::lexical_cast<std::string>(v)));
	}

	void add(const std::string& key, const std::string&  v)
	{
		value_map.insert(std::make_pair(key, v));
	}

	template<class T>
	T get_ex(const std::string& skey, T def_val = T()) {
		try
		{
			if(check(skey))
				return boost::lexical_cast<T>(value_map[skey]);	
		}
		catch (...)
		{
		}
		return def_val;
	}

	bool save()
	{
		if(!b_init)
			return false;

		try
		{
			boost::property_tree::ptree& root = pt.get_child("root");
			for (auto itr = root.begin(); itr!=root.end(); ++itr)
			{
				auto it = value_map.find(itr->first);
				if (it != value_map.end())
				{
					itr->second.data() = it->second;
				}
			}
			static boost::property_tree::xml_writer_settings<std::string> settings('\t', 1); 
			boost::property_tree::xml_parser::write_xml(filepath, pt,std::locale(), settings);
		}
		catch (...)
		{
			return false;
		}

		return true;
	}
private:
	ENABLE_MAP<std::string, std::string> value_map;
	boost::property_tree::ptree pt;
	std::string filepath;
	bool b_init;
};