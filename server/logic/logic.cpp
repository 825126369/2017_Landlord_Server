// logic.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include "logic_server.h"

int _tmain(int argc, _TCHAR* argv[])
{
	logic_server::instance().s_init(argc, argv);
	return 0;
}

