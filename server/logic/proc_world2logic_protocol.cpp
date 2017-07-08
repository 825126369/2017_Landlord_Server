#include "stdafx.h"
#include "proc_world2logic_protocol.h"
#include "game_player_mgr.h"
#include "game_player.h"
#include "logic_peer.h"
#include "game_check.h"
#include "backstage_manager.h"
#include "game_manager.h"
#include <i_game_engine.h>
#include "i_game_phandler.h"

using namespace boost;

void init_world2logic_protocol()
{
	//logic->world
	packetl2w_game_ready_factory::regedit_factory();
	//world->logic
	packetw2l_player_login_factory::regedit_factory();
	packetl2w_player_login_result_factory::regedit_factory();
	packetw2l_player_logout_factory::regedit_factory();
	packetl2w_player_logout_result_factory::regedit_factory();
	packetw2l_change_player_property_factory::regedit_factory();
	packetl2w_player_property_stat_factory::regedit_factory();
	packetw2l_accept_gift_factory::regedit_factory();
	packetl2w_game_broadcast_factory::regedit_factory();
	packetl2w_player_star_change_factory::regedit_factory();
	packetl2w_player_quest_change_factory::regedit_factory();
}

//玩家连接
bool packetw2l_player_login_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packetw2l_player_login> msg)
{	
	__ENTER_FUNCTION_CHECK;
	auto p = game_player_mgr::instance().find_playerbyid(msg->account_info().aid());

	auto sendmsg = PACKET_CREATE(packetl2w_player_login_result, e_mst_l2w_player_login_result);	
	sendmsg->set_playerid(msg->account_info().aid());

	if(p!=nullptr)
	{
		game_player_mgr::instance().reset_player(p, msg->sessionid());
		p->set_state(e_ps_playing);
		sendmsg->set_result(msg_type_def::e_rmt_success);
	}
	else
	{		
		auto ainfo = msg->account_info();
		p = game_player::malloc();
		//p->Account = ainfo.account();
		p->PlayerID = ainfo.aid();
		p->Gold = ainfo.gold();
		p->NickName = ainfo.nickname();
		p->VIPLevel = ainfo.viplvl();
		p->Ticket = ainfo.ticket();
		p->PhotoFrame = ainfo.curphotoframeid();
		p->Sex = ainfo.sex();
		p->IconCustom = ainfo.icon_custom();
		p->ExperienceVIP = ainfo.experience_vip();
		p->CreateTime = ainfo.create_time();
		p->set_sessionid(msg->sessionid());		
		p->MonthCard = ainfo.monthcard_time();
		p->IsBuyFirstGift = ainfo.isbuyfirstgift();
		p->Privilege = ainfo.privilege();

		auto ainfoex = msg->account_info_ex();
		p->IsRobot = ainfoex.is_robot();
		p->Lucky = ainfoex.lucky();
		p->TempIncome = ainfoex.temp_income();
		p->TotalIncome = ainfoex.total_income();
		
		auto eng = game_manager::instance().get_game_engine();
		if(eng != nullptr)
		{
			if(eng->player_enter_game(p))
			{

				p->set_state(e_ps_playing);
				game_player_mgr::instance().add_player(p);				
				sendmsg->set_result(msg_type_def::e_rmt_success);

				//机器人需要另外通知
				p->reset_robot_life();				
				
			}
		}		
	}
	peer->send_msg(sendmsg);
	
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

//玩家离开
bool packetw2l_player_logout_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packetw2l_player_logout> msg)
{	
	__ENTER_FUNCTION_CHECK;
	game_player_mgr::instance().del_player(msg->playerid());
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 玩家属性变化
bool packetw2l_change_player_property_factory::packet_process(shared_ptr<server_peer> peer, shared_ptr<packetw2l_change_player_property> msg)
{	
	__ENTER_FUNCTION_CHECK;
	auto p = game_player_mgr::instance().find_playerbyid(msg->playerid());
	if(p)
	{
		if(msg->has_change_info())
		{
			auto cinfo = msg->change_info();
			if(cinfo.has_gold())
			{
				p->world_attribute64_change(msg_type_def::e_itd_gold, cinfo.gold());
			}
			if(cinfo.has_ticket())
			{
				p->world_attribute_change(msg_type_def::e_itd_ticket, cinfo.ticket());
			}
			if(cinfo.has_viplvl())
			{
				p->world_attribute_change(msg_type_def::e_itd_vip, cinfo.viplvl());
			}
			if(cinfo.has_icon_custom())
			{
				p->IconCustom = cinfo.icon_custom();			
			}
			if(cinfo.has_sex())
			{
				p->world_attribute_change(msg_type_def::e_itd_sex, cinfo.sex());
			}
			if(cinfo.has_curphotoframeid())
			{
				p->world_attribute_change(msg_type_def::e_itd_photoframe, cinfo.curphotoframeid());
			}
			if(cinfo.has_nickname())
			{
				p->NickName = cinfo.nickname();
			}
			if(cinfo.has_experience_vip())
			{
				p->ExperienceVIP = cinfo.experience_vip();
			}
			if(cinfo.has_monthcard_time())
			{
				p->world_attribute_change(msg_type_def::e_itd_monthcard, cinfo.monthcard_time());	
			}
			if (cinfo.has_isbuyfirstgift())
			{
				p->world_attribute_change(msg_type_def::e_itd_firstgift, cinfo.isbuyfirstgift());
			}

		}
	
		if(msg->has_change_info_ex())
		{
			auto cinfoex = msg->change_info_ex();
			if (cinfoex.has_lucky())
			{
				p->world_attribute_change(msg_type_def::e_itd_lucky, cinfoex.lucky());
			}
		}	
	}

	__LEAVE_FUNCTION_CHECK
	return !EX_CHECK;
}

bool packetw2l_accept_gift_factory::packet_process(shared_ptr<server_peer> peer,
												   shared_ptr<packetw2l_accept_gift> msg)
{	
	__ENTER_FUNCTION_CHECK;
	
	auto pReceiver = game_player_mgr::instance().find_playerbyid(msg->receiverid());
	if(pReceiver)
	{
		pReceiver->get_handler()->onAcceptGift(msg->receiverid(), msg->giftid());
		return true;
	}

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
