#pragma once
#include <string>

// 名称的合法性判定
class NameValid
{
public:
	/*
			str是否合法，输入编码为utf-8
			errorRet	非法时的返回值，
			成功返回e_rmt_success
	*/
	static int isValid(const std::string& str, int errorRet);
};

