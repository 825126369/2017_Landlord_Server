#include "stdafx.h"
#include "world_server.h"
#include "proc_server_packet.h"
#include "proc_world_packet.h"
#include "proc_c2w_lobby_protocol.h"
#include "proc_logic2world_protocol.h"
#include "proc_dial_lottery.h"
#include "proc_player_property.h"
#include "proc_mail.h"
#include "proc_rank.h"
#include "proc_chat.h"
#include "proc_exchange.h"
#include "proc_shop.h"
#include "proc_friend.h"
#include "proc_online_reward.h"
#include "proc_safe_deposit_box.h"
#include "proc_logic2world_friend.h"
#include "proc_benefits.h"
#include "proc_notice.h"
#include "proc_bind_phone.h"
#include "proc_daily_box_lottery.h"
#include "proc_logic2world_robot.h"
#include "proc_activity.h"
#include "proc_star_lottery.h"
#include "proc_player_quest.h"

void world_server::init_packet()
{
	__ENTER_FUNCTION;
	//救济金
	initBenefitsPacket();

	//gate和world间通信,包括了用户登录,支付,和gm命令
	initLobbyProtocol();

	//聊天功能
	initChatPacket();

	//logic和world通信
	initLogic2WorldPacket();

	//转盘抽奖
	initDialLotteryPacket();

	//玩家属性的一些操作
	initPlayerPropertyPacket();

	//montior和world间的通信
	initServerPacket();

	//邮件
	initMailPacket();

	//排名系统
	initRankPacket();

	//兑换
	initExchangePacket();

	//商店
	initShopPacket();

	//好友
	initFriendPacket();

	//在线奖励
	initOnlineRewardPacket();

	//保险箱
	initSafeDepositBoxPacket();

	//login和world通信来进入好友的房间
	initLogic2WorldFriend();

	//服务器间的通信,包括所有服务器
	initWorldPacket();

	initNoticePacket();

	initBindPhonePacket();

	initDailyBoxLotteryPacket();

	initLogic2WorldRobot();

	initActivityPacket();

	initStarLotteryPacket();

	initQuestPacket();
	__LEAVE_FUNCTION
}