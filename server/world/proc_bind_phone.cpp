#include "stdafx.h"
#include "proc_bind_phone.h"
#include "game_player.h"
#include "global_sys_mgr.h"
#include "mobile_phone_binding_sys.h"
#include "time_helper.h"
using namespace boost;

void initBindPhonePacket()
{
	packetc2w_req_bind_phone_factory::regedit_factory();
	packetw2c_req_bind_phone_result_factory::regedit_factory();
	packetc2w_req_verify_code_factory::regedit_factory();
	packetw2c_req_verify_code_result_factory::regedit_factory();
	packetc2w_req_relieve_phone_factory::regedit_factory();
	packetw2c_req_relieve_phone_result_factory::regedit_factory();
	packetc2w_req_relieve_verify_factory::regedit_factory();
	packetw2c_req_relieve_verify_result_factory::regedit_factory();
}

// 请求绑定手机
bool packetc2w_req_bind_phone_factory::packet_process(shared_ptr<world_peer> peer, 
													  boost::shared_ptr<game_player> player, 
													  shared_ptr<packetc2w_req_bind_phone> msg)
{	
	__ENTER_FUNCTION_CHECK;
	
	auto sendmsg = PACKET_CREATE(packetw2c_req_bind_phone_result, e_mst_w2c_req_bind_phone_result);
	int result = GLOBAL_SYS(MobilePhoneBindingSys)->reqBindPhone(player.get(), msg->phone(), time_helper::instance().get_cur_time());
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 请求验证
bool packetc2w_req_verify_code_factory::packet_process(shared_ptr<world_peer> peer, 
													  boost::shared_ptr<game_player> player, 
													  shared_ptr<packetc2w_req_verify_code> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_verify_code_result, e_mst_w2c_req_verify_code_result);
	int result = GLOBAL_SYS(MobilePhoneBindingSys)->reqVerifyCode(player.get(), msg->code(), time_helper::instance().get_cur_time());
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}


// 请求验证码
bool packetc2w_req_relieve_verify_factory::packet_process(shared_ptr<world_peer> peer, 
													  boost::shared_ptr<game_player> player, 
													  shared_ptr<packetc2w_req_relieve_verify> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_relieve_verify_result, e_mst_w2c_req_relieve_verify_result);
	int result = GLOBAL_SYS(MobilePhoneBindingSys)->reqReliveVerify(player.get());
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 请求解除
bool packetc2w_req_relieve_phone_factory::packet_process(shared_ptr<world_peer> peer, 
													   boost::shared_ptr<game_player> player, 
													   shared_ptr<packetc2w_req_relieve_phone> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_relieve_phone_result, e_mst_w2c_req_relieve_phone_result);
	int result = GLOBAL_SYS(MobilePhoneBindingSys)->reqRelivePhone(player.get(), msg->code());
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}