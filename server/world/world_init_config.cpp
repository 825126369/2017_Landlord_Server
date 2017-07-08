#include "stdafx.h"
#include "world_server.h"
#include "M_BaseInfoCFG.h"
#include "M_RechangeCFG.h"
#include "M_VIPProfitCFG.h"
#include "M_DialLotteryCFG.h"
#include "M_GiftCFG.h"
#include "M_MultiLanguageCFG.h"
#include "M_ItemCFG.h"
#include "M_ExchangeCFG.h"
#include "M_CommodityCFG.h"
#include "M_GiftCFG.h"
#include "M_GiftRewardCFG.h"
#include "M_DailyLoginCFG.h"
#include "M_ActivityCFG.h"
#include "M_RechangeExCFG.h"
#include "M_RobotNameCFG.h"
#include "M_OnlineRewardCFG.h"
#include "M_QuestCFG.h"
#include "M_GameCFG.h"
#include "M_RechangeLotteryCFG.h"
#include "M_StarLotteryCFG.h"
#include "M_FreeLotteryCFG.h"
#include "M_ShopCFG.h"

bool world_server::init_config()
{
	__ENTER_FUNCTION_CHECK;
	M_BaseInfoCFG::GetSingleton()->Load();
	M_RechangeCFG::GetSingleton()->Load();
	M_VIPProfitCFG::GetSingleton()->Load();
	// ת�̳齱
	M_DialLotteryCFG::GetSingleton()->Load();
	M_MultiLanguageCFG::GetSingleton()->Load();
	M_ItemCFG::GetSingleton()->Load();
	M_ExchangeCFG::GetSingleton()->Load();
	M_CommodityCFG::GetSingleton()->Load();
	M_GiftCFG::GetSingleton()->Load();
	M_GiftRewardCFG::GetSingleton()->Load();
	M_DailyLoginCFG::GetSingleton()->Load();
	M_ActivityCFG::GetSingleton()->Load();
	M_RechangeExCFG::GetSingleton()->Load();
	M_RobotNameCFG::GetSingleton()->Load();
	M_OnlineRewardCFG::GetSingleton()->Load();
	M_QuestCFG::GetSingleton()->Load();
	M_ShopCFG::GetSingleton()->Load();

	M_GameCFG::GetSingleton()->Load();
	M_RechangeLotteryCFG::GetSingleton()->Load();
	M_StarLotteryCFG::GetSingleton()->Load();
	M_FreeLotteryCFG::GetSingleton()->Load();

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
