#include "stdafx.h"
#include "global_sys_mgr.h"
#include "dial_lottery_sys.h"
#include "mail_sys.h"
#include "game_rank_sys.h"
#include "chat_sys.h"
#include "exchange_sys.h"
#include "shop_sys.h"
#include "id_generator_sys.h"
#include "pump_sys.h"
#include "notice_sys.h"
#include "mobile_phone_binding_sys.h"
#include "gm_sys.h"
#include "daily_box_lottery_sys.h"
#include "operation_activity_sys.h"
#include "robots_sys.h"
#include "star_lottery_sys.h"
#include "order_sys.h"

using namespace boost;
global_sys_mgr::global_sys_mgr()
{
	init_sys();
}

global_sys_mgr::~global_sys_mgr()
{
}

void global_sys_mgr::init_sys()
{
	regedit_sys(make_shared<DialLotterySys>());
	regedit_sys(make_shared<MailSys>());
	regedit_sys(make_shared<GameRankSys>());
	regedit_sys(make_shared<ChatSys>());
	regedit_sys(make_shared<ExchangeSys>());
	regedit_sys(make_shared<ShopSys>());
	regedit_sys(make_shared<IdGeneratorSys>());
	regedit_sys(make_shared<PumpSys>());
	regedit_sys(make_shared<NoticeSys>());
	regedit_sys(make_shared<MobilePhoneBindingSys>());
	regedit_sys(make_shared<GmSys>());
	regedit_sys(make_shared<DailyBoxLotterySys>());
	regedit_sys(make_shared<OperationActivitySys>());
	regedit_sys(make_shared<RobotsSys>());
	regedit_sys(make_shared<StarLotterySys>());
	regedit_sys(make_shared<OrderSys>());

	init_sys_object();
}

