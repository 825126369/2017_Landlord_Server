#pragma once
#include "enable_hashmap.h"

// url²ÎÊı½âÎö
class UrlParam
{
public:
	bool parse(const std::string& param);

	void reset();

	int getIntValue(const std::string& key, int defValue = 0);

	const std::string& getStringValue(const std::string& key, const std::string& defValue = "");
private:
	ENABLE_MAP<std::string, std::string> m_param;
};








