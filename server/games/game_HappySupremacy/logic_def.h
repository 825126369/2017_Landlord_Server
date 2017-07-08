#pragma once

#include <enable_smart_ptr.h>
#include <enable_object_pool.h>
#include <enable_hashmap.h>

#include <game_object.h>
#include <game_object_field.h>
#include <game_object_map.h>
#include <game_object_array.h>


class logic_room;
class logic_player;
class logic_lobby;

typedef boost::shared_ptr<logic_room> LRoomPtr;
typedef boost::shared_ptr<logic_player> LPlayerPtr;

typedef std::map<uint16_t, LRoomPtr> LROOM_MAP;
typedef std::map<uint32_t, LPlayerPtr> LPLAYER_MAP;

#define SAFE_DELETE(v) if(v != nullptr){delete v; v = nullptr;}

//0：对门:1：顺们，2：倒门,3:顺对门（角）,4:倒对门（角）,5:顺倒门（桥，横门）
static const int MAX_BET_COUNT = 6;//押注种类数
//每人发2张牌
const int EVERYONE_CARDCOUNT=2;
//发牌数量为8
const int SENDCARD_COUNT=8;
//牌的数量为32
const int CARD_MAX_COUNT =32;

#ifndef _DEBUG
#define NDEBUG
#endif