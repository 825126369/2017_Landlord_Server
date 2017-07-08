#pragma once
//#pragma execution_character_set("utf-8")

#include <string>
#include <xstring>

class enable_string_convert
{
public:
	//wstring convert string
	static std::wstring s2ws(const std::string &s);
	static std::string ws2s(const std::wstring &ws);

	//w/string convert utf8
	static std::string ws2utf(const std::wstring& ws);
	static std::string s2utf(const std::string& s);

	static std::wstring utf2ws(const std::string& s);
	static std::string utf2s(const std::string& s);
};