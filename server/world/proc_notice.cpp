#include "stdafx.h"
#include "proc_notice.h"
#include "game_player.h"
#include "global_sys_mgr.h"
#include "notice_sys.h"

using namespace boost;

void initNoticePacket()
{
	packetc2w_req_notice_factory::regedit_factory();
	packetw2c_req_notice_result_factory::regedit_factory();
}

// ÇëÇó¹«¸æ
bool packetc2w_req_notice_factory::packet_process(shared_ptr<world_peer> peer, 
												  boost::shared_ptr<game_player> player, 
												  shared_ptr<packetc2w_req_notice> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto sendmsg = PACKET_CREATE(packetw2c_req_notice_result, e_mst_w2c_req_notice_result);
	time_t Last = msg->lasttime();
	std::vector<stNoticeInfo> noticeList;
	GLOBAL_SYS(NoticeSys)->getNotice(noticeList, msg->lasttime(), &Last);
	sendmsg->set_lasttime(Last);
	if(!noticeList.empty())
	{
		auto pList = sendmsg->mutable_infolist();
		pList->Reserve(noticeList.size());
		for(auto it = noticeList.begin(); it != noticeList.end(); ++it)
		{
			auto ptr = pList->Add();
			ptr->set_gentime(it->m_genTime);
			ptr->set_title(it->m_title);
			ptr->set_content(it->m_content);
			ptr->set_order(it->m_order);
		}
	}
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
