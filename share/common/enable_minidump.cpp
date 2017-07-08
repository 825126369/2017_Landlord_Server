#include "stdafx.h"
#include "enable_minidump.h"

#include <boost/filesystem.hpp>
#include <enable_processinfo.h>
#include <enable_string_convert.h>
#include <enable_smart_ptr.h>

using namespace boost;
using namespace google_breakpad;
using namespace std;

static boost::scoped_ptr<google_breakpad::ExceptionHandler> g_excpt;
static enable_minidump_handler* g_handler = nullptr;

bool DumpCallback(const wchar_t* dump_path,
								   const wchar_t* minidump_id,
								   void* context,
								   EXCEPTION_POINTERS* exinfo,
								   MDRawAssertionInfo* assertion,
								   bool succeeded) 
{	
	if(g_handler)
		return g_handler->DumpCallback(dump_path,minidump_id,context,exinfo,assertion,succeeded);

	return succeeded;
}

void enable_minidump::Init(enable_minidump_handler* handler, bool btest)
{	
	wstring fullpath = TEXT("dumps");
	filesystem::path tph(fullpath);
	if(!filesystem::exists(tph))
		filesystem::create_directory(tph);

	//string procname = enable_processinfo::get_processname();
	//if(!procname.empty())
	//{
	//	fullpath = fullpath + TEXT("\\") + enable_string_convert::s2ws(procname);
	//	filesystem::path tph2(fullpath);
	//	if(!filesystem::exists(tph2))
	//		filesystem::create_directory(tph2);
	//	tph = tph2;
	//}

	g_handler = handler;
	g_excpt.reset(
		new ExceptionHandler(
		tph.wstring(),
		NULL,
		&DumpCallback,
		NULL,
		ExceptionHandler::HANDLER_ALL
		));

	//²âÊÔÖ±½Ódump
	if(btest)
		g_excpt->WriteMinidump();

	
}

void enable_minidump::Release()
{
	g_excpt.reset();
}

