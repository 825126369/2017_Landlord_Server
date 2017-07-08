#pragma once

#include <db_base.h>
#include <enable_queue.h>
#include <enable_object_pool.h>
#include <string>
#include <boost/thread.hpp>
#include <boost/atomic.hpp>


//////////////////////////////////////////////////////////////////////////
class db_buf:public enable_obj_pool<db_buf, boost::mutex>
{
public:
	db_buf():TableType(0),ProcType(0)
	{

	}
	uint16_t TableType;
	int ProcType;
	mongo::BSONObj DbQuery;
	mongo::BSONObj DbData;
};

typedef boost::shared_ptr<db_buf> DBbufPtr;

class db_queue : public db_base
{
public:
	db_queue();
	~db_queue();

	void push_insert(uint16_t table_type, const mongo::BSONObj& indata);
	void push_update(uint16_t table_type,  const mongo::BSONObj& qy, const mongo::BSONObj& indata);
	void push_delete(uint16_t table_type, const mongo::BSONObj& qy);
	virtual const std::string& get_tablename(uint16_t table_type) = 0;
	virtual void close();
private:
	void log_thread();
	bool m_run;
	boost::atomic_bool m_fush;
	fast_safe_queue<DBbufPtr> m_logs;
	//boost::thread m_thread;
	boost::thread_group m_work_grp;
};