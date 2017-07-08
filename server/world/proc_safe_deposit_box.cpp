#include "stdafx.h"
#include "proc_safe_deposit_box.h"
#include "game_player.h"
#include "safe_deposit_box_mgr.h"
#include "pump_type.h"
#include "pump_sys.h"
#include "global_sys_mgr.h"
#include "mobile_phone_binding_sys.h"
#include "time_helper.h"

using namespace boost;

void initSafeDepositBoxPacket()
{
	packetc2w_set_password_factory::regedit_factory();
	packetw2c_set_password_result_factory::regedit_factory();

	packetc2w_modify_password_factory::regedit_factory();
	packetw2c_modify_password_result_factory::regedit_factory();

	packetc2w_reset_password_factory::regedit_factory();
	packetw2c_reset_password_result_factory::regedit_factory();

	packetc2w_check_password_factory::regedit_factory();
	packetw2c_check_password_result_factory::regedit_factory();

	packetc2w_deposit_gold_factory::regedit_factory();
	packetw2c_deposit_gold_result_factory::regedit_factory();

	packetc2w_draw_gold_factory::regedit_factory();
	packetw2c_draw_gold_result_factory::regedit_factory();

	packetc2w_get_safe_box_security_code_factory::regedit_factory();
	packetw2c_get_safe_box_security_code_result_factory::regedit_factory();
}

// 设置保险箱密码
bool packetc2w_set_password_factory::packet_process(shared_ptr<world_peer> peer, 
													boost::shared_ptr<game_player> player, 
													shared_ptr<packetc2w_set_password> msg)
{	
	__ENTER_FUNCTION_CHECK;
	if(player->is_gaming())
	{
		SLOG_ERROR << boost::format("玩家[id=%1%, nick=%2%, acc=%3%]在游戏内设置保险箱密码")
			% player->PlayerId->get_value() % player->NickName->get_string() % player->Account->get_string();
		return !EX_CHECK;
	}

	auto sendmsg = PACKET_CREATE(packetw2c_set_password_result, e_mst_w2c_set_password_result);
	int result = player->get_sys<SafeDepositBoxMgr>()->setPassword(msg->pwd1(), msg->pwd2());
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 重置保险箱密码
bool packetc2w_reset_password_factory::packet_process(shared_ptr<world_peer> peer, 
													   boost::shared_ptr<game_player> player, 
													   shared_ptr<packetc2w_reset_password> msg)
{	
	__ENTER_FUNCTION_CHECK;
	if(player->is_gaming())
	{
		SLOG_ERROR << boost::format("玩家[id=%1%, nick=%2%, acc=%3%]在游戏内修改保险箱密码")
			% player->PlayerId->get_value() % player->NickName->get_string() % player->Account->get_string();
		return !EX_CHECK;
	}

	auto sendmsg = PACKET_CREATE(packetw2c_reset_password_result, e_mst_w2c_reset_password_result);
	int result = player->get_sys<SafeDepositBoxMgr>()->resetPassword(msg->safecode(), msg->pwd1(), msg->pwd2());
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 修改保险箱密码
bool packetc2w_modify_password_factory::packet_process(shared_ptr<world_peer> peer, 
													boost::shared_ptr<game_player> player, 
													shared_ptr<packetc2w_modify_password> msg)
{	
	__ENTER_FUNCTION_CHECK;
	if(player->is_gaming())
	{
		SLOG_ERROR << boost::format("玩家[id=%1%, nick=%2%, acc=%3%]在游戏内修改保险箱密码")
			% player->PlayerId->get_value() % player->NickName->get_string() % player->Account->get_string();
		return !EX_CHECK;
	}

	auto sendmsg = PACKET_CREATE(packetw2c_modify_password_result, e_mst_w2c_modify_password_result);
	int result = player->get_sys<SafeDepositBoxMgr>()->modifyPassword(msg->old_pwd(), msg->new_pwd1(), msg->new_pwd2());
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 存入金币
bool packetc2w_deposit_gold_factory::packet_process(shared_ptr<world_peer> peer, 
													boost::shared_ptr<game_player> player, 
													shared_ptr<packetc2w_deposit_gold> msg)
{	
	__ENTER_FUNCTION_CHECK;
	if(player->is_gaming())
	{
		SLOG_ERROR << boost::format("玩家[id=%1%, nick=%2%, acc=%3%]在游戏内将金币存入保险箱")
			% player->PlayerId->get_value() % player->NickName->get_string() % player->Account->get_string();
		return !EX_CHECK;
	}

	auto sendmsg = PACKET_CREATE(packetw2c_deposit_gold_result, e_mst_w2c_deposit_gold_result);
	int result = player->get_sys<SafeDepositBoxMgr>()->depositGold(msg->gold(), msg->pwd());
	sendmsg->set_result(result);
	sendmsg->set_gold(msg->gold());
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);
	if(result == msg_type_def::e_rmt_success)
	{
		GLOBAL_SYS(PumpSys)->addGeneralStatLog(stat_safe_box_deposit);
	}
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 取出金币
bool packetc2w_draw_gold_factory::packet_process(shared_ptr<world_peer> peer, 
												 boost::shared_ptr<game_player> player, 
												 shared_ptr<packetc2w_draw_gold> msg)
{	
	__ENTER_FUNCTION_CHECK;

	if(player->is_gaming())
	{
		SLOG_ERROR << boost::format("玩家[id=%1%, nick=%2%, acc=%3%]在游戏内从保险箱取出金币")
			% player->PlayerId->get_value() % player->NickName->get_string() % player->Account->get_string();
		return !EX_CHECK;
	}

	auto sendmsg = PACKET_CREATE(packetw2c_draw_gold_result, e_mst_w2c_draw_gold_result);
	
	int result = player->get_sys<SafeDepositBoxMgr>()->drawGold(msg->gold(), msg->pwd());
	sendmsg->set_result(result);
	sendmsg->set_gold(msg->gold());
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);
	if(result == msg_type_def::e_rmt_success)
	{
		GLOBAL_SYS(PumpSys)->addGeneralStatLog(stat_safe_box_draw);
	}
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 获取保险箱的验证码
bool packetc2w_get_safe_box_security_code_factory::packet_process(shared_ptr<world_peer> peer, 
																  boost::shared_ptr<game_player> player, 
																  shared_ptr<packetc2w_get_safe_box_security_code> msg)
{	
	__ENTER_FUNCTION_CHECK;

	if(player->is_gaming())
	{
		SLOG_ERROR << boost::format("玩家[id=%1%, nick=%2%, acc=%3%]在游戏内获取保险箱的验证码")
			% player->PlayerId->get_value() % player->NickName->get_string() % player->Account->get_string();
		return !EX_CHECK;
	}

	auto sendmsg = PACKET_CREATE(packetw2c_get_safe_box_security_code_result, e_mst_w2c_get_safe_box_security_code_result);
	int result = GLOBAL_SYS(MobilePhoneBindingSys)->getSafeBoxSecurityCode(player.get(), time_helper::instance().get_cur_time());
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);
	
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 验证保险箱密码
bool packetc2w_check_password_factory::packet_process(shared_ptr<world_peer> peer, 
													  boost::shared_ptr<game_player> player, 
													  shared_ptr<packetc2w_check_password> msg)
{	
	__ENTER_FUNCTION_CHECK;
	if(player->is_gaming())
	{
		SLOG_ERROR << boost::format("玩家[id=%1%, nick=%2%, acc=%3%]在游戏内修改保险箱密码")
			% player->PlayerId->get_value() % player->NickName->get_string() % player->Account->get_string();
		return !EX_CHECK;
	}

	auto sendmsg = PACKET_CREATE(packetw2c_check_password_result, e_mst_w2c_check_password_result);
	int result = player->get_sys<SafeDepositBoxMgr>()->checkPassword(msg->pwd());
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}