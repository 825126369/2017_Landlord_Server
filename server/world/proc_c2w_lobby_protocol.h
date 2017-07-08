#pragma once
#include <net\packet_manager.h>
#include <client2world_protocol.pb.h>

#include "world_peer.h"

using namespace client2world_protocols;
class game_player;

void initLobbyProtocol();

//gate <-> world
PACKET_REGEDIT_RECVGATE_SID_LOG(world_peer, packetc2w_player_connect);
PACKET_REGEDIT_SEND(packetw2c_player_connect_result);

PACKET_REGEDIT_RECVGATE(world_peer, packet_c2w_timesync, game_player);
PACKET_REGEDIT_SEND(packet_w2c_timesync_result);

PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_ask_login, game_player);
PACKET_REGEDIT_SEND(packetw2c_ask_login_result);

PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_enter_game, game_player);
PACKET_REGEDIT_SEND(packetw2c_enter_game_result);

PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_leave_game, game_player);
PACKET_REGEDIT_SEND(packetw2c_leave_game_result);

PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_gm_command, game_player);
PACKET_REGEDIT_SEND(packetw2c_gm_command_result);

PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_ask_check_payment, game_player);
PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_ask_test_payment, game_player);
PACKET_REGEDIT_SEND(packetw2c_ask_check_payment_result);
PACKET_REGEDIT_SEND(packetw2c_ask_check_payment_result2);

PACKET_REGEDIT_SEND(packetw2c_player_kick);


PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_get_gamelist, game_player);
PACKET_REGEDIT_SEND(packetw2c_get_gamelist_result);


PACKET_REGEDIT_RECVGATE(world_peer, packetc2w_first_login, game_player);
PACKET_REGEDIT_SEND(packetw2c_first_login_result);


