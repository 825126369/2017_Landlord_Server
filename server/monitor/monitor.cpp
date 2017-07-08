// monitor.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include "monitor_server.h"
//#include <iostream>
int _tmain(int argc, _TCHAR* argv[])
{	
	monitor_server::instance().s_init(argc, argv);

	return 0;
}

