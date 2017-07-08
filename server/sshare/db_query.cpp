#include "stdafx.h"
#include "db_query.h"
#include <enable_hashmap.h>
#include <enable_processinfo.h>
#include <vector>
#include <list>

using namespace boost;
//enable_obj_pool_init(db_buf, boost::null_mutex);

db_queue::db_queue()
	:m_run(true)
	,m_fush(false)
	//,m_thread(boost::bind(&db_queue::log_thread, this))
{
	m_work_grp.create_thread(boost::bind(&db_queue::log_thread, this));
}
db_queue::~db_queue()
{
	m_run = false;
}

void db_queue::push_insert(uint16_t table_type, const mongo::BSONObj& indata)
{
	DBbufPtr dp = db_buf::malloc();
	dp->TableType = table_type;
	dp->DbData = indata.copy();	
	m_logs.push(dp);
}

void db_queue::push_update(uint16_t table_type,  const mongo::BSONObj& qy, const mongo::BSONObj& indata)
{
	DBbufPtr dp = db_buf::malloc();
	dp->TableType = table_type;
	dp->ProcType = 1;
	dp->DbQuery = qy.copy();
	dp->DbData = indata.copy();	
	m_logs.push(dp);
}

void db_queue::push_delete(uint16_t table_type, const mongo::BSONObj& qy)
{
	DBbufPtr dp = db_buf::malloc();
	dp->TableType = table_type;
	dp->ProcType = 2;
	dp->DbQuery = qy.copy();
	m_logs.push(dp);
}

void db_queue::close()
{
	m_fush = true;
	m_work_grp.join_all();

	db_base::close();
}

void db_queue::log_thread()
{	
	ENABLE_MAP<uint16_t, std::vector<mongo::BSONObj>> logmap;
	std::list<DBbufPtr> updates;
	timer t;
	double elapsed=0, total = 0;
	while (m_run)
	{
		t.restart();
		total+=elapsed;
		if((total>5.0 ||m_fush))
		{
			total = 0;

			if(!m_logs.empty())
			{
				DBbufPtr dp;
				while(!m_logs.empty())
				{
					m_logs.pop(dp);

					if(dp->ProcType != 0)
					{
						updates.push_back(dp);
						continue;
					}

					auto it = logmap.find(dp->TableType);
					if(it != logmap.end())
					{
						it->second.push_back(dp->DbData.copy());
					}
					else
					{
						std::vector<mongo::BSONObj> vec;
						auto ret =logmap.insert(std::make_pair(dp->TableType, vec));
						ret.first->second.push_back(dp->DbData.copy());
					}
				}

				for (auto it = logmap.begin(); it != logmap.end(); ++it)
				{
					insert(get_tablename(it->first), it->second);
				}

				for (auto it = updates.begin(); it != updates.end(); ++it)
				{
					dp = (*it);

					if(dp->ProcType == 1)
						update(get_tablename(dp->TableType), dp->DbQuery, dp->DbData);
					else if(dp->ProcType == 2)
						remove(get_tablename(dp->TableType), dp->DbQuery);
				}

				logmap.clear();
				updates.clear();
			}			

			if(m_fush)
				return;
		}
		
		elapsed = t.elapsed();
		if(t.elapsed() < 0.1)
		{
			this_thread::sleep(posix_time::milliseconds(100));
			elapsed = t.elapsed();
		}
	}
}