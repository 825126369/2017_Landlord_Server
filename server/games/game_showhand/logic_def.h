#pragma once

#define SHOWHAND_SPACE_BEGIN namespace showhand_space {
#define SHOWHAND_SPACE_END	}
#define SHOWHAND_SPACE_USING using namespace showhand_space;

#include <enable_smart_ptr.h>
#include <enable_object_pool.h>
#include <enable_hashmap.h>

#include <game_object.h>
#include <game_object_field.h>
#include <game_object_map.h>
#include <game_object_array.h>
#include <game_object_container.h>

#include <server_log.h>

SHOWHAND_SPACE_BEGIN

class logic_room;
class logic_table;
class logic_player;
class logic_lobby;

typedef boost::shared_ptr<logic_room> LRoomPtr;
typedef boost::shared_ptr<logic_table> LTablePtr;
typedef boost::shared_ptr<logic_player> LPlayerPtr;

typedef std::map<uint16_t, LRoomPtr> LROOM_MAP;
typedef std::map<uint16_t, LTablePtr> LTABLE_MAP;
typedef ENABLE_MAP<uint32_t, LPlayerPtr> LPLAYER_MAP;

#define SAFE_DELETE(v) if(v != nullptr){delete v; v = nullptr;}

SHOWHAND_SPACE_END
