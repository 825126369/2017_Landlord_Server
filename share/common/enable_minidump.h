#pragma once

#include <client/windows/handler/exception_handler.h>

#ifdef WIN32
#ifdef _DEBUG
#pragma comment(lib, "exception_handler-gd.lib")
#pragma comment(lib, "crash_generation_server-gd.lib")
#pragma comment(lib, "crash_generation_client-gd.lib")
#pragma comment(lib, "common-gd.lib")
#else
#pragma comment(lib, "exception_handler.lib")
#pragma comment(lib, "crash_generation_server.lib")
#pragma comment(lib, "crash_generation_client.lib")
#pragma comment(lib, "common.lib")
#endif

#endif

class enable_minidump_handler
{
public:
	/* dmp文件路径
	wstring dump_file = dump_path;
	dump_file += TEXT("\\");
	dump_file += minidump_id;
	dump_file += TEXT(".dmp");*/
	virtual bool DumpCallback(const wchar_t* dump_path,
		const wchar_t* minidump_id,
		void* context,
		EXCEPTION_POINTERS* exinfo,
		MDRawAssertionInfo* assertion,
		bool succeeded){return true;};
};

class enable_minidump
{
public:
	//先写dmp文件 再处理handler
	//btest 测试直接dump 
	static void Init(enable_minidump_handler* handler = nullptr, bool btest = false);
	static void Release();
};

