#pragma once
#ifdef _DEBUG
#include <map>
#define ENABLE_MAP std::map
#include <set>
#define ENABLE_SET std::set
#else
//效率比map高很多 但是无法查看数据 并且无序
#include <boost/unordered_map.hpp>
#define ENABLE_MAP boost::unordered_map
#include <boost/unordered_set.hpp>
#define ENABLE_SET boost::unordered_set
#endif

