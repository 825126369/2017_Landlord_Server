#include "stdafx.h"
#include "logic_server.h"
#include "M_GameCFG.h"
#include "M_MultiLanguageCFG.h"
#include "M_BaseInfoCFG.h"

bool logic_server::init_config()
{
	__ENTER_FUNCTION_CHECK;
	
	M_GameCFG::GetSingleton()->Load();
	M_MultiLanguageCFG::GetSingleton()->Load();
	M_BaseInfoCFG::GetSingleton()->Load();

	__LEAVE_FUNCTION_CHECK
		return !EX_CHECK;
}
