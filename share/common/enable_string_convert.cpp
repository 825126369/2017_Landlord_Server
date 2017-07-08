#include "stdafx.h"
#include "enable_string_convert.h"
#include <codecvt>

using namespace std;
wstring enable_string_convert::s2ws(const string &s)
{
	setlocale(LC_ALL, "chs"); 
	const char* _Source = s.c_str();
	size_t _Dsize = s.size() + 1;
	wchar_t *_Dest = new wchar_t[_Dsize];
	wmemset(_Dest, 0, _Dsize);
	size_t _ret;
	mbstowcs_s(&_ret, _Dest, _Dsize, _Source, _Dsize);
	wstring result = _Dest;
	delete []_Dest;
	setlocale(LC_ALL, "C");
	return result; 
}

string enable_string_convert::ws2s(const wstring &ws)
{
	string curLocale = setlocale(LC_ALL, NULL);        // curLocale = "C";
	setlocale(LC_ALL, "chs");
	const wchar_t* _Source = ws.c_str();
	size_t _Dsize = 2 * ws.size() + 1;
	char *_Dest = new char[_Dsize];
	memset(_Dest,0,_Dsize);
	size_t _ret;
	wcstombs_s(&_ret, _Dest, _Dsize, _Source, _Dsize);
	string result = _Dest;
	delete []_Dest;
	setlocale(LC_ALL, curLocale.c_str());
	return result;
}

string enable_string_convert::ws2utf(const wstring& ws)
{		
	wstring_convert<codecvt_utf8<wchar_t>> conv;
	return conv.to_bytes(ws.c_str());
}

string enable_string_convert::s2utf(const string& s)
{
	return ws2utf(s2ws(s));
}

wstring enable_string_convert::utf2ws(const string& s)
{
	wstring_convert<codecvt_utf8<wchar_t>> conv;
	return conv.from_bytes(s.c_str());
}

string enable_string_convert::utf2s(const string& s)
{
	return ws2s(utf2ws(s));
}