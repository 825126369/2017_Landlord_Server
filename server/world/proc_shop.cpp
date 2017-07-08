#include "stdafx.h"
#include "proc_shop.h"
#include "game_player.h"
#include "global_sys_mgr.h"
#include "shop_sys.h"
#include "check_apple_task.h"
#include "world_server.h"
using namespace boost;

void initShopPacket()
{
	packetc2w_buy_commodity_factory::regedit_factory();
	packetw2c_buy_commodity_result_factory::regedit_factory();
	packetw2c_check_apple_order_form_factory::regedit_factory();

	packetc2w_ask_recharge_history_factory::regedit_factory();
	packetw2c_recharge_history_result_factory::regedit_factory();

	packetw2c_open_first_gift_factory::regedit_factory();
}

// 购买商品
bool packetc2w_buy_commodity_factory::packet_process(shared_ptr<world_peer> peer, 
													 boost::shared_ptr<game_player> player, 
													 shared_ptr<packetc2w_buy_commodity> msg)
{	
	__ENTER_FUNCTION_CHECK;
	
	SLOG_CRITICAL << "购买商品" << std::endl;
	auto sendmsg = PACKET_CREATE(packetw2c_buy_commodity_result, e_mst_w2c_buy_commodity_result);
	int result = GLOBAL_SYS(ShopSys)->buyCommodity(player.get(), msg->commodityid());
	sendmsg->set_result(result);
	sendmsg->set_commodityid(msg->commodityid());
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 购买商品
bool packetw2c_check_apple_order_form_factory::packet_process(shared_ptr<world_peer> peer, 
													 boost::shared_ptr<game_player> player, 
													 shared_ptr<packetw2c_check_apple_order_form> msg)
{	
	__ENTER_FUNCTION_CHECK;
	SLOG_CRITICAL << "检验苹果订单" << std::endl;
	
	auto task = boost::make_shared<check_apple_task>(world_server::instance().get_io_service());
	task->init_task(  player ,   msg->apple());
	 
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}

// 充值记录
bool packetc2w_ask_recharge_history_factory::packet_process(shared_ptr<world_peer> peer, 
															  boost::shared_ptr<game_player> player, 
															  shared_ptr<packetc2w_ask_recharge_history> msg)
{	
	__ENTER_FUNCTION_CHECK;

	auto& historys = GLOBAL_SYS(ShopSys)->get_charge_historys();
	auto sendmsg = PACKET_CREATE(packetw2c_recharge_history_result, e_mst_w2c_recharge_history_result);
	for (auto it = historys.begin(); it != historys.end(); it++)
	{
		sendmsg->mutable_history_infos()->Reserve(historys.size());
		sendmsg->add_history_infos(*it);
	}
	peer->send_msg_to_client(player->get_sessionid(), sendmsg);
	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}