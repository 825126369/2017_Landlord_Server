#include "stdafx.h"
#include "name_valid.h"
#include "enable_string_convert.h"
#include <boost/regex.hpp>
#include "msg_type_def.pb.h"

using namespace boost;

static wregex StrReg(L"^[^\\\\~!@#&:;,='<>/\\?\\.\\+\\*\\$\\^\\|\"\\[\\]\\s\\{\\}]+$");
static wregex StrRegAllDigit(L"^\\s*\\d+\\s*$");

int NameValid::isValid(const std::string& str, int errorRet)
{
	std::wstring strName = enable_string_convert::utf2ws(str);

	if(!regex_match(strName, StrReg))
		return errorRet;

	// 全部为数字
	if(regex_match(strName, StrRegAllDigit))
		return errorRet;

	return msg_type_def::e_rmt_success;
}

