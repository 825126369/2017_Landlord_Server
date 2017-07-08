#pragma once
//#include <enable_hashmap.h>
//#include <string>

#include <server_base.pb.h>
#include <enable_smart_ptr.h>

typedef boost::shared_ptr<server_protocols::server_info> server_info_define;

typedef ENABLE_MAP<uint16_t, server_info_define> SINFO_MAP;
//struct server_info_define
//{
//public:
//	server_info_define()
//	{
//		server_port = 0;
//		server_type = 0;
//	}
//	//uint16_t server_id; //monitorÉÏµÄid
//	uint16_t server_type;	
//	uint16_t server_port; //port = id
//	std::string server_ip;
//	ENABLE_MAP<std::string, std::string> attributes_map;
//};

//class client_peer_helper
//{
//public:
//	
//};