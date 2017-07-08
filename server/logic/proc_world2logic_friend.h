#pragma once
#include <net\packet_manager.h>
#include "logic2world_friend.pb.h"
#include "logic2world_robot.pb.h"
#include "server_peer.h"

using namespace logic2world_protocols;

void initWorld2LogicFriend();

// 进入好友房间结果
PACKET_REGEDIT_RECV_LOG(server_peer, packetw2l_enter_friend_room);
PACKET_REGEDIT_SEND(packetl2w_enter_friend_room_result);
PACKET_REGEDIT_SEND(packetl2w_request_robot);
