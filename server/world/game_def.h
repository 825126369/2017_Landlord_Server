#pragma once
#include <enable_object_pool.h>
#include <enable_hashmap.h>
#include <string>
#include <vector>

#include <game_object.h>
#include <game_object_field.h>
#include <game_object_map.h>
#include <game_object_array.h>

class game_player;

typedef boost::shared_ptr<game_player> GPlayerPtr;

typedef ENABLE_MAP<uint32_t, GPlayerPtr> GPlayerMap;