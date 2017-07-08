// gate.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include "gate_server.h"

int _tmain(int argc, _TCHAR* argv[])
{
	gate_server::instance().s_init(argc, argv);
	return 0;
}

