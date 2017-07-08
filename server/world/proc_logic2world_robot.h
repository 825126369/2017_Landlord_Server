#pragma once
#include <net\packet_manager.h>
#include "logic2world_robot.pb.h"
#include "world_peer.h"

using namespace logic2world_protocols;

void initLogic2WorldRobot();


PACKET_REGEDIT_RECV(world_peer, packetl2w_request_robot);
