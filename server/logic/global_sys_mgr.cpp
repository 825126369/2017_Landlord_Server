#include "stdafx.h"
#include "global_sys_mgr.h"
#include "count_sys.h"

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
	regedit_sys(make_shared<CountSys>());

	init_sys_object();
}

