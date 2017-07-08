#pragma once
#include <net\packet_manager.h>
#include "client2world_chat.pb.h"
#include "world_peer.h"

using namespace client2world_protocols;
class game_player;

void initChatPacket();

// 发送聊天
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_chat, game_player);
// 发送聊天结果
PACKET_REGEDIT_SEND(packetw2c_chat_result);

// 通告
PACKET_REGEDIT_SEND(packetw2c_notify);

// 玩家发起的通告消息
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_player_notify, game_player);
// 玩家发起的通告消息
PACKET_REGEDIT_SEND(packetw2c_player_notify_result);

// 玩家连续发小喇叭
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_player_continuous_send_speaker, game_player);
// 玩家连续发小喇叭消息
PACKET_REGEDIT_SEND(packetw2c_player_continuous_send_speaker_result);
