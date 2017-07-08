#include "stdafx.h"
#include "game_player.h"
#include <game_sys_mgr.h>
#include "game_sys_recharge.h"
#include "dial_lottery_mgr.h"
#include "bag_mgr.h"
#include "friend_mgr.h"
#include "online_reward_mgr.h"
#include "safe_deposit_box_mgr.h"
#include "benefits_mgr.h"
#include "player_log_mgr.h"
#include "daily_box_lottery_mgr.h"
#include "operation_activity_mgr.h"
#include "game_quest_mgr.h"
#include "star_lottery_mgr.h"

void game_player::init_sys()
{
	regedit_sys(game_sys_recharge::malloc());
	regedit_sys(DialLotteryMgr::malloc());
	regedit_sys(BagMgr::malloc());
	regedit_sys(FriendMgr::malloc());
	regedit_sys(OnlineRewardMgr::malloc());
	regedit_sys(SafeDepositBoxMgr::malloc());
	regedit_sys(BenefitsMgr::malloc());
	regedit_sys(PlayerLogMgr::malloc());
	regedit_sys(DailyBoxLotteryMgr::malloc());
	regedit_sys(OperationActivityMgr::malloc());
	regedit_sys(game_quest_mgr::malloc());
	regedit_sys(StarLotteryMgr::malloc());

	init_sys_object();
}