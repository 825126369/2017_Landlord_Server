#pragma once
#include <net\packet_manager.h>
#include "server_peer.h"
#include <logic2world_protocol.pb.h>


using namespace logic2world_protocols;

void init_world2logic_protocol();

//logic -> world
PACKET_REGEDIT_SEND(packetl2w_game_ready);
PACKET_REGEDIT_SEND(packetl2w_player_login_result);
PACKET_REGEDIT_SEND(packetl2w_player_logout_result);
PACKET_REGEDIT_SEND(packetl2w_game_broadcast);

//world -> logic
PACKET_REGEDIT_RECV_LOG(server_peer, packetw2l_player_login);
PACKET_REGEDIT_RECV_LOG(server_peer, packetw2l_player_logout);

// 玩家属性变化
PACKET_REGEDIT_RECV_LOG(server_peer, packetw2l_change_player_property);

// 统计金币，礼券变化的协议, logic->world
PACKET_REGEDIT_SEND(packetl2w_player_property_stat);

// 接收到礼物
PACKET_REGEDIT_RECV_LOG(server_peer, packetw2l_accept_gift);

PACKET_REGEDIT_SEND(packetl2w_player_star_change);
PACKET_REGEDIT_SEND(packetl2w_player_quest_change);