#include "stdafx.h"
#include "proc_chat.h"
#include "game_player.h"
#include "chat_sys.h"
#include "global_sys_mgr.h"
#include "pump_sys.h"
#include "pump_type.h"

using namespace boost;

void initChatPacket()
{
	packetc2w_chat_factory::regedit_factory();
	packetw2c_chat_result_factory::regedit_factory();
	packetw2c_notify_factory::regedit_factory();

	packetc2w_player_notify_factory::regedit_factory();
	packetw2c_player_notify_result_factory::regedit_factory();

	packetc2w_player_continuous_send_speaker_factory::regedit_factory();
	packetw2c_player_continuous_send_speaker_result_factory::regedit_factory();
}

// 发送聊天
bool packetc2w_chat_factory::packet_process(shared_ptr<world_peer> peer, 
											boost::shared_ptr<game_player> player, 
											shared_ptr<packetc2w_chat> msg)
{	
	__ENTER_FUNCTION_CHECK;

	GLOBAL_SYS( ChatSys)->gameChat( player.get() , msg->content(),  msg->content().length() ,msg->audio_time()  );

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 玩家发起的通告消息
bool packetc2w_player_notify_factory::packet_process(shared_ptr<world_peer> peer, 
													 boost::shared_ptr<game_player> player, 
													 shared_ptr<packetc2w_player_notify> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_player_notify_result, e_mst_w2c_player_notify_result);
	int result = GLOBAL_SYS(ChatSys)->playerNotify(player.get(), msg->content());
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);
	if(result == msg_type_def::e_rmt_success)
	{
		GLOBAL_SYS(PumpSys)->addGeneralStatLog(stat_player_notify);
	}
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 玩家连续发小喇叭
bool packetc2w_player_continuous_send_speaker_factory::packet_process(shared_ptr<world_peer> peer, 
													 boost::shared_ptr<game_player> player, 
													 shared_ptr<packetc2w_player_continuous_send_speaker> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_player_continuous_send_speaker_result, e_mst_w2c_player_continuous_send_speaker_result);
	int result = GLOBAL_SYS(ChatSys)->playerContinuousSendSpeaker(player.get(), msg->content(), msg->count());
	sendmsg->set_result(result);
	sendmsg->set_count(msg->count());
	if(result == msg_type_def::e_rmt_last_speaker_not_finish)
	{
		auto ptr = GLOBAL_SYS(ChatSys)->getSpeaker(player->PlayerId->get_value());
		if(ptr)
		{
			sendmsg->set_remaincount(ptr->m_remainCount);
		}
	}
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);
	
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

