#include "stdafx.h"
#include "game_player.h"
#include "proc_mail.h"
#include "game_player.h"
#include "global_sys_mgr.h"
#include "mail_sys.h"
#include "pump_sys.h"
#include "pump_type.h"
#include "game_player_mgr.h"
#include "proc_logic2world_protocol.h"
#include "player_log_mgr.h"
#include "M_BaseInfoCFG.h"

using namespace boost;

void initMailPacket()
{
	packetc2w_get_mails_factory::regedit_factory();
	packetw2c_get_mails_result_factory::regedit_factory();

	packetc2w_recv_mail_gifts_factory::regedit_factory();
	packetw2c_recv_mail_gifts_result_factory::regedit_factory();

	packetc2w_send_mail_factory::regedit_factory();
	packetw2c_send_mail_result_factory::regedit_factory();

	packetc2w_remove_mail_factory::regedit_factory();
	packetw2c_remove_mail_result_factory::regedit_factory();

	packetw2c_accept_gift_notify_factory::regedit_factory();

	packetc2w_req_send_mail_log_factory::regedit_factory();
	packetw2c_req_send_mail_log_result_factory::regedit_factory();

	packetc2w_remove_mail_log_factory::regedit_factory();
	packetw2c_remove_mail_log_result_factory::regedit_factory();
}

// 获取邮件列表请求
bool packetc2w_get_mails_factory::packet_process(shared_ptr<world_peer> peer, 
												 shared_ptr<game_player> player,
												 shared_ptr<packetc2w_get_mails> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_get_mails_result, e_mst_w2c_get_mails_result);
	std::vector<stMail> mails;
	time_t Last = msg->lasttime();
	GLOBAL_SYS(MailSys)->getMail(player.get(), mails, msg->lasttime(), &Last);
	sendmsg->set_lasttime(Last);
	auto pmails = sendmsg->mutable_mails();
	pmails->Reserve(mails.size());

	for(auto it = mails.begin(); it != mails.end(); ++it)
	{
		auto th = pmails->Add();
		th->set_mailid(it->m_mailId);
		th->set_time(it->m_time);
		th->set_title(it->m_title);
		th->set_sender(it->m_sender);
		th->set_content(it->m_content);
		th->set_isrecvive(it->m_isRecvive);
		th->set_senderid(it->m_senderId);

		auto pitem = th->mutable_gifts();
		pitem->Reserve(it->m_items.size());
		for(int i = 0; i < it->m_items.size(); i++)
		{
			auto ti = pitem->Add();
			ti->set_giftid(it->m_items[i].m_giftId);
			ti->set_count(it->m_items[i].m_count);
		}
	}
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);
	
	__LEAVE_FUNCTION_CHECK
	return !EX_CHECK;
}

// 领取邮件中的道具请求
bool packetc2w_recv_mail_gifts_factory::packet_process(shared_ptr<world_peer> peer, 
													   shared_ptr<game_player> player,
													   shared_ptr<packetc2w_recv_mail_gifts> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_recv_mail_gifts_result, e_mst_w2c_recv_mail_gifts_result);
	int res = GLOBAL_SYS(MailSys)->receiveGift(player.get(), msg->mailid());
	sendmsg->set_mailid(msg->mailid());
	sendmsg->set_result(res);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
	return !EX_CHECK;
}

// 发送邮件
bool packetc2w_send_mail_factory::packet_process(shared_ptr<world_peer> peer, 
												 shared_ptr<game_player> player,
												 shared_ptr<packetc2w_send_mail> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_send_mail_result, e_mst_w2c_send_mail_result);
	int result = 0;
	static int SendGiftLimit = M_BaseInfoCFG::GetSingleton()->GetData("SendGiftLimit")->mValue;

	if(msg->gifts_size() > 0 && player->get_gold() >= SendGiftLimit)
	{
		std::vector<stGift> giftList;
		for(int i = 0; i < msg->gifts_size(); i++)
		{
			auto gt = msg->mutable_gifts(i);
			giftList.push_back(stGift(gt->giftid(), gt->count()));
		}

		std::string mailId = "";
		result = GLOBAL_SYS(MailSys)->sendGift(player.get(), msg->title(), msg->toplayerid(), 0, giftList, &mailId);
		if(result == msg_type_def::e_rmt_success)
		{
			//*sendmsg->mutable_gifts() = *msg->mutable_gifts();
			sendmsg->set_mailtype(1);

			auto pumpSys = GLOBAL_SYS(PumpSys);
			pumpSys->sendGiftLog(giftList);

			static int sendGiftLogMaxCount = M_BaseInfoCFG::GetSingleton()->GetData("sendGiftLogMaxCount")->mValue;
			player->get_sys<PlayerLogMgr>()->addSendGiftLog(msg->toplayerid(), 
				giftList[0].m_giftId, 
				giftList[0].m_count, 
				mailId,
				true,
				sendGiftLogMaxCount);

			shared_ptr<world_peer> peerLogic;

			auto targetPlayer = game_player_mgr::instance().findPlayerById(msg->toplayerid());
			if(targetPlayer && targetPlayer->is_gaming()) // 接收者在游戏内
			{
				if(targetPlayer->check_logic())
				{
					peerLogic = targetPlayer->get_logic();

					auto notifyMsg = PACKET_CREATE(packetw2l_accept_gift, e_mst_w2l_accept_gift);
					notifyMsg->set_senderid(player->PlayerId->get_value());
					notifyMsg->set_receiverid(msg->toplayerid());
					notifyMsg->set_giftid(giftList[0].m_giftId);
					peerLogic->send_msg(notifyMsg);
				}
			}
		}
	}
	else
	{
		//std::string mailId = "";
		//result = GLOBAL_SYS(MailSys)->sendMail(msg->title(),
		//	player->NickName->get_string(), 
		//	msg->content(), 
		//	player->PlayerId->get_value(),
		//	msg->toplayerid(), 
		//	0, nullptr, &mailId);
		//
		//sendmsg->set_mailtype(0);
		//sendmsg->set_mailid(mailId);
		//
		//static int sendMailLogMaxCount = M_BaseInfoCFG::GetSingleton()->GetData("sendMailLogMaxCount")->mValue;
		//player->get_sys<PlayerLogMgr>()->addSendMailLog(msg->toplayerid(), 
		//	msg->title(), 
		//	msg->content(), 
		//	mailId, 
		//	sendMailLogMaxCount);
		result = msg_type_def::e_rmt_fail;
	}
	sendmsg->set_result(result);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 删除某个邮件
bool packetc2w_remove_mail_factory::packet_process(shared_ptr<world_peer> peer, 
												   shared_ptr<game_player> player,
												   shared_ptr<packetc2w_remove_mail> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_remove_mail_result, e_mst_w2c_remove_mail_result);
	int res = GLOBAL_SYS(MailSys)->removeMail(player.get(), msg->mailid());
	sendmsg->set_mailid(msg->mailid());
	sendmsg->set_result(res);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 请求发送邮件日志
bool packetc2w_req_send_mail_log_factory::packet_process(shared_ptr<world_peer> peer, 
														 boost::shared_ptr<game_player> player, 
														 shared_ptr<packetc2w_req_send_mail_log> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_send_mail_log_result, e_mst_w2c_req_send_mail_log_result);
	auto mgr = player->get_sys<PlayerLogMgr>();
	std::deque<stSendMailLogInfo> logList;
	time_t Last = 0;
	mgr->getSendMailLog(msg->lasttime(), Last, logList);
	if(!logList.empty())
	{
		auto pLogList = sendmsg->mutable_loglist();
		pLogList->Reserve(logList.size());
		for(auto it = logList.begin(); it != logList.end(); ++it)
		{
			auto pLog = pLogList->Add();
			pLog->set_sendtime(it->m_sendTime);
			pLog->set_firendid(it->m_friendId);
			pLog->set_content(it->m_content);
			pLog->set_mailid(it->m_mailId);
			pLog->set_title(it->m_title);
			pLog->set_friendnickname(it->m_friendNickName);
		}
	}
	sendmsg->set_lasttime(Last);
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 删除某个邮件日志
bool packetc2w_remove_mail_log_factory::packet_process(shared_ptr<world_peer> peer, 
													   boost::shared_ptr<game_player> player, 
													   shared_ptr<packetc2w_remove_mail_log> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_remove_mail_log_result, e_mst_w2c_remove_mail_log_result);
	auto mgr = player->get_sys<PlayerLogMgr>();
	int result = mgr->delSendMailLog(msg->mailid());
	sendmsg->set_result(result);
	sendmsg->set_mailid(msg->mailid());
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
