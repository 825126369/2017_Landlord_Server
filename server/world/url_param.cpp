#include "stdafx.h"
#include "url_param.h"
#include <boost/algorithm/string.hpp>
using namespace boost;

bool UrlParam::parse(const std::string& param)
{
	std::vector<std::string> vecKeyVal;
	std::vector<std::string> vecStr;
	split(vecStr, param, boost::is_any_of("&"), token_compress_on);

	int count = (int)vecStr.size();
	for(int i = 0; i < count; i++)
	{
		vecKeyVal.clear();
		split(vecKeyVal, vecStr[i], boost::is_any_of("="), token_compress_on);

		if(vecKeyVal.size() == 2)
		{
			boost::trim(vecKeyVal[0]);
			boost::trim(vecKeyVal[1]);

			m_param[ vecKeyVal[0] ] = vecKeyVal[1];
		}
	}
	return true;
}

void UrlParam::reset()
{
	m_param.clear();
}

int UrlParam::getIntValue(const std::string& key, int defValue)
{
	auto it = m_param.find(key);
	if(it != m_param.end())
	{
		return atoi(it->second.c_str());
	}

	return defValue;
}

const std::string& UrlParam::getStringValue(const std::string& key, const std::string& defValue)
{
	auto it = m_param.find(key);
	if(it != m_param.end())
	{
		return it->second;
	}

	return defValue;
}


