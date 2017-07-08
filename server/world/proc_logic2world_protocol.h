#pragma once
#include <net\packet_manager.h>
#include <logic2world_protocol.pb.h>
#include "world_peer.h"

using namespace logic2world_protocols;

void initLogic2WorldPacket();

//logic <- world
PACKET_REGEDIT_SEND(packetw2l_player_login);
PACKET_REGEDIT_SEND(packetw2l_player_logout);


PACKET_REGEDIT_RECV(world_peer, packetw2l_change_player_property);

//logic -> world
PACKET_REGEDIT_RECV(world_peer, packetl2w_game_ready);
PACKET_REGEDIT_RECV(world_peer, packetl2w_player_login_result);
PACKET_REGEDIT_RECV(world_peer, packetl2w_player_logout_result);
PACKET_REGEDIT_RECV(world_peer, packetl2w_game_broadcast);

// 金币，礼券的统计
PACKET_REGEDIT_RECV(world_peer, packetl2w_player_property_stat);

// 通知接收到礼物
PACKET_REGEDIT_SEND(packetw2l_accept_gift);

//任务成就改变
PACKET_REGEDIT_RECV(world_peer, packetl2w_player_quest_change);

//星星属性改变
PACKET_REGEDIT_RECV(world_peer, packetl2w_player_star_change);


