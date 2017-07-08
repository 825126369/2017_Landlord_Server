#include "stdafx.h"
#include "notice_sys.h"
#include "msg_type_def.pb.h"
#include "game_db.h"
#include "time_helper.h"
#include "enable_string_convert.h"
#include <boost/tokenizer.hpp>     
#include <boost/algorithm/string.hpp> 

static std::string OPERATION_NOTIFY = "optionNotify";

static std::string SERVICE_INFO = "serviceInfo";

static const time_t REMOVE_DELTA = 6 * 3600;

void NoticeSys::init_sys_object()
{
	m_lastRemove = 0;
	m_elapsedTime = 0.0;
}

void NoticeSys::sys_update(double delta)
{
	m_elapsedTime += delta;
	if(m_elapsedTime > 30.0)
	{
		m_elapsedTime = 0.0;

		std::vector<mongo::BSONObj> noticeList;
		mongo::BSONObj cond = BSON("plat" << mongo::NE << "!!");
		db_player::instance().find(noticeList, SERVICE_INFO, cond);

		for(int i = 0; i < (int)noticeList.size(); i++)
		{
			std::string plat = noticeList[i].getStringField("plat");
			std::string info = noticeList[i].getStringField("info");

			_parseServiceInfo(plat, info);
		}
	}
}

int NoticeSys::getNotice(std::vector<stNoticeInfo>& res, time_t minT, time_t* Last)
{
	time_t curT = time_helper::instance().get_cur_time();
	bool del = REMOVE_DELTA < curT - m_lastRemove;
	if(del)
	{
		mongo::BSONObj delCond = BSON("deadTime" << mongo::LT << mongo::Date_t(curT * 1000));
		db_player::instance().remove(OPERATION_NOTIFY, delCond, false);
		m_lastRemove = curT;
	}

	mongo::BSONObj condition = BSON("genTime" << mongo::GT << mongo::Date_t(minT * 1000));
	std::vector<mongo::BSONObj> noticeList;
	db_player::instance().find(noticeList, OPERATION_NOTIFY, condition);
	time_t maxT = 0;
	for(int i = 0; i < noticeList.size(); i++)
	{
		stNoticeInfo m;
		mongo::BSONObj& b = noticeList[i];
		m.m_genTime   = b.getField("genTime").Date().toTimeT();
		m.m_title  = b.getStringField("title");
		m.m_content = b.getStringField("content");
		if(m.m_genTime > maxT)
		{
			maxT = m.m_genTime;
		}

		if(b.hasField("order"))
		{
			m.m_order = b.getIntField("order");
		}
		else
		{
			m.m_order = 0;
		}
		res.push_back(m);
	}

	if(Last && noticeList.size() > 0)
	{
		*Last = maxT;
	}
	return msg_type_def::e_rmt_success;
}

bool NoticeSys::getServiceInfo(const std::string& plat, std::vector<stServiceInfo>* & info)
{
	auto it = m_serviceInfo.find(plat);
	if(it != m_serviceInfo.end())
	{
		info = &it->second;
		return true;
	}

	it = m_serviceInfo.find("default");
	if(it != m_serviceInfo.end())
	{
		info = &it->second;
		return true;
	}

	return false;
}

void NoticeSys::_parseServiceInfo(std::string& plat, std::string& info)
{
	std::vector<stServiceInfo>* pInfo = &m_serviceInfo[plat];
	pInfo->clear();

	std::vector<std::string> group;       
	boost::split(group, info, boost::is_any_of("#"));  

	std::vector<std::string> vecInfos;

	for(int i = 0; i < (int)group.size(); i++)
	{
		boost::split(vecInfos, group[i], boost::is_any_of(","));  

		if(vecInfos.size() == 3)
		{
			stServiceInfo tmp;
			tmp.m_infoType = atoi(vecInfos[0].c_str());
			tmp.m_key = vecInfos[1];
			tmp.m_value = vecInfos[2];
			pInfo->push_back(tmp);
		}
	}
}


