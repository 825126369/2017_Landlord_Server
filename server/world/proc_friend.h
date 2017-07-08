#pragma once
#include <net\packet_manager.h>
#include "client2world_friend.pb.h"
#include "world_peer.h"

using namespace client2world_protocols;
class game_player;

void initFriendPacket();

// 添加好友
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_add_friend, game_player);
// 添加好友结果
PACKET_REGEDIT_SEND(packetw2c_add_friend_result);

// 移除好友
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_remove_friend, game_player);
// 移除好友结果
PACKET_REGEDIT_SEND(packetw2c_remove_friend_result);

// 请求好友列表
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_friend_list, game_player);
// 请求好友列表结果
PACKET_REGEDIT_SEND(packetw2c_req_friend_list_result);

// 搜索好友
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_search_friend, game_player);
// 搜索好友结果
PACKET_REGEDIT_SEND(packetw2c_search_friend_result);

// 进入好友房间
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_enter_friend_room, game_player);
// 进入好友房间结果
PACKET_REGEDIT_SEND(packetw2c_enter_friend_room_result);


// 获取好友所在游戏id
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_get_friend_gameid, game_player);
// 返回好友游戏id
PACKET_REGEDIT_SEND(packetw2c_get_friend_gameid_result);

