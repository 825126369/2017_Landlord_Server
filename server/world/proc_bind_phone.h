#pragma once
#include <net\packet_manager.h>
#include "client2world_bind_phone.pb.h"
#include "world_peer.h"

using namespace client2world_protocols;
class game_player;

void initBindPhonePacket();

// 请求绑定手机
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_bind_phone, game_player);
// 请求绑定手机结果
PACKET_REGEDIT_SEND(packetw2c_req_bind_phone_result);

// 请求验证
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_verify_code, game_player);
// 请求验证结果
PACKET_REGEDIT_SEND(packetw2c_req_verify_code_result);


// 请求解除手机
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_relieve_phone, game_player);
PACKET_REGEDIT_SEND(packetw2c_req_relieve_phone_result);

// 请求解除验证
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_req_relieve_verify, game_player);
PACKET_REGEDIT_SEND(packetw2c_req_relieve_verify_result);