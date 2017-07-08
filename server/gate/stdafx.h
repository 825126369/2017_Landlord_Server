// stdafx.h : 标准系统包含文件的包含文件，
// 或是经常使用但不常更改的
// 特定于项目的包含文件
//

#pragma once
//#define _USE_32BIT_TIME_T
#include "targetver.h"
#define WIN32_LEAN_AND_MEAN             //  从 Windows 头文件中排除极少使用的信息
#include <stdio.h>
#include <tchar.h>



// TODO: 在此处引用程序需要的其他头文件
//#include <vld.h>
#include <boost/asio.hpp>
#include <boost/thread.hpp>
#include <boost/bind.hpp>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/xml_parser.hpp> 
#include <boost/lexical_cast.hpp>
#include <boost/noncopyable.hpp>
#include <enable_smart_ptr.h>
#include <boost/cstdint.hpp>
#include <boost/pool/pool.hpp>
#include <boost/property_tree/json_parser.hpp> 
#include <boost/unordered_map.hpp>
#include <boost/function.hpp>
#include <boost/enable_shared_from_this.hpp>
#include <boost/circular_buffer.hpp>
#include <boost/log/common.hpp>
#include <boost/log/sources/logger.hpp>
#include <boost/filesystem.hpp>
#include <boost/timer.hpp>



#include <limits>
#include <string>
#include <xstring>
#include <queue>
#include <list>
#include <exception>
#include <map>
#include <vector>

#include <tchar.h>
#include <com_log.h>

#include <enable_singleton.h>
#include <net/msg_queue.h>
#include <enable_object_manager.h>
#include <enable_singleton.h>
#include <server_manager_handler.h>
#include <net/peer_tcp.h>

#ifdef WIN32
#ifdef _DEBUG
#pragma comment(lib, "sshare-gd.lib")
#pragma comment(lib, "protocol-gd.lib")

#else
#pragma comment(lib, "sshare.lib")
#pragma comment(lib, "protocol.lib")
#endif
#endif