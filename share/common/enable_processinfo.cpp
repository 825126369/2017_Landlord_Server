#include "stdafx.h"
#include "enable_processinfo.h"

#ifdef WIN32
#include <windows.h>
#else
#include <limits.h>
#include <stdio.h>
#include <string.h>
#include <unistd.h>
#include <time.h>
#endif

using namespace std;
string enable_processinfo::get_processname()
{
	string procname;
	char szFileFullPath[256]={0};
#ifdef WIN32	
	GetModuleFileNameA(NULL,static_cast<LPSTR>(szFileFullPath),255);
	procname = szFileFullPath;
	size_t npos = procname.find_last_of('\\');
	if(npos == string::npos)
		return procname;
	procname = procname.erase(0, npos+1);
	npos = procname.find_last_of('.');
	if(npos == string::npos)
		return procname;
	procname = procname.erase(npos);
#else
	char* path_end;
	if(readlink("/proc/self/exe", szFileFullPath,255) <=0)
		return procname;
	path_end = strrchr(szFileFullPath,  '/');
	if(path_end == NULL)
		return procname;
	++path_end;
	procname = path_end;	
#endif
	return procname;
}

uint32_t enable_processinfo::get_tick_count()
{
#ifdef WIN32
	return GetTickCount();
#else	
    struct timeval tv;
    gettimeofday(&tv, nullptr);
    return tv.tv_sec + tv.tv_usec / 1000;
	//struct timespec ts;

	//clock_gettime(CLOCK_MONOTONIC, &ts);

	//return (ts.tv_sec * 1000 + ts.tv_nsec / 1000000);
#endif
}