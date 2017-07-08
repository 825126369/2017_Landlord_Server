#include <stdafx.h>
#include <benefits_mgr.h>
#include <global_sys_mgr.h>
#include <enable_object_pool.h>

#include "game_player.h"
#include <M_BaseInfoCFG.h>

void BenefitsMgr::init_sys_object()
{
	m_collected = CONVERT_POINT(Tfield<int32_t>, get_game_player()->regedit_tfield(e_got_int32, "hasReceiveAlmsCount"));
}

void BenefitsMgr::sys_time_update()
{
	m_collected->set_value(0);
}

int BenefitsMgr::collected() const
{
	return m_collected->get_value();
}

void BenefitsMgr::store()
{
	m_collected->set_update(true);
}