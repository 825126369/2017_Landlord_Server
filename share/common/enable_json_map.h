#pragma once
#include <enable_hashmap.h>
#include <string>
#include <boost/property_tree/json_parser.hpp>  

class enable_json_helper
{
public:
	static bool jsonmap_to_str(const ENABLE_MAP<std::string, std::string>& json_map, std::string& retstr)
	{
		using namespace boost::property_tree;

		try
		{			
			ptree    jsonparser;  
			for (auto itr = json_map.begin(); itr!=json_map.end(); ++itr)
			{
				jsonparser.put<std::string>(itr->first, itr->second); 
			}

			std::stringstream  jsonstream;  
			json_parser::write_json<ptree>(jsonstream, jsonparser);
			retstr = jsonstream.str();
			return true;
		}
		catch (json_parser::json_parser_error& error)
		{
			retstr = "";
		}	
		return false;
	}

	static bool str_to_json_map(std::string& json_str, ENABLE_MAP<std::string, std::string>& json_map)
	{
		using namespace boost::property_tree;
		
		try
		{
			json_map.clear();
			jsonstr_replace(json_str);

			ptree    jsonparser;  
			std::stringstream  jsonstream(json_str);  
			json_parser::read_json<ptree>(jsonstream, jsonparser);		
			
			for (auto itr = jsonparser.begin(); itr!=jsonparser.end(); ++itr)
			{
				json_map.insert(std::make_pair(itr->first, itr->second.data()));
			}	
			return true;
		}
		catch (json_parser::json_parser_error& error)
		{
			json_map.clear();
		}		
		return false;
	}

	static void  jsonstr_replace( std::string &strBig)  
	{  
		enable_json_helper::string_replace(strBig,"\n","");
		enable_json_helper::string_replace(strBig,"\\n","");
		enable_json_helper::string_replace(strBig,"\r\n","");
		//enable_json_helper::string_replace(strBig," ","");
	}  

	static void  string_replace( std::string &strBig, const std::string &strsrc, const std::string &strdst )  
	{  
		std::string::size_type pos = 0;  
		std::string::size_type srclen = strsrc.size();  
		std::string::size_type dstlen = strdst.size();  

		while( (pos=strBig.find(strsrc, pos)) != std::string::npos )  
		{  
			strBig.replace( pos, srclen, strdst );  
			pos += dstlen;  
		}  
	}  
};