#pragma once
#include <net\packet_manager.h>
#include "logic2world_friend.pb.h"
#include "world_peer.h"

using namespace logic2world_protocols;

void initLogic2WorldFriend();

// 进入好友房间结果
PACKET_REGEDIT_RECV(world_peer, packetl2w_enter_friend_room_result);
PACKET_REGEDIT_SEND(packetw2l_enter_friend_room);
