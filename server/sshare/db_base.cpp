#include "stdafx.h"
#include "db_base.h"

#include <mongo/client/init.h>
#include <mongo/client/dbclient_rs.h>
#include <exception>
#include <boost/algorithm/string.hpp>
#include <enable_crypto.h>

using namespace mongo;
using namespace boost;

static bool db_init = false;

static std::string AESKEY = "94@!#(*13&32!@)(";
static std::string AuthDB = "admin";

db_base::db_base(void)
{
	b_auth = false;
	b_init = false;
	db_err = "db not init!";
	//b_thread = false;
	b_useReplicaSet = false;

	if(!db_init)
	{
		client::initialize();
		db_init = true;
	}
}


db_base::~db_base(void)
{
	if(db_init)
	{
		db_init = false;
	}
}

void db_base::set_userpwd(const std::string& user, const std::string& pwd, bool crypto)
{
	if(crypto)
	{
		std::string tmpuser = enable_crypto_helper::Base64Decode(user);
		db_user = enable_crypto_helper::AESDecryptString(tmpuser, AESKEY);
		std::string tmppwd = enable_crypto_helper::Base64Decode(pwd);
		db_pwd = enable_crypto_helper::AESDecryptString(tmppwd, AESKEY);
	}
	else
	{
		db_user = user;
		db_pwd = pwd;
	}

	b_auth = true;
}

void db_base::init_db(std::string& _db_addr, std::string& _db_name)
{
	if(b_init)
		return;

	db_addr = _db_addr;
	db_name_base = _db_name;
	db_name = _db_name + ".";
	db_err = "db_base::init error!";    
	//b_thread = _thread;
	b_init = true;
	init_index();
}

void db_base::init_db_rs(std::string& _db_addrs, std::string& _db_name)
{	
	if(b_init)
		return;

	db_name_base = _db_name;
	db_name = _db_name + ".";
	db_err = "db_base::init error!";    

	std::vector<std::string> vec;
	std::vector<HostAndPort> vec2;
	split(vec, _db_addrs, algorithm::is_space(), token_compress_on);
	for (int i = 0; i<vec.size(); i++)
	{
		vec2.push_back(HostAndPort(vec[i]));
	}
	m_conn_rs.reset(new DBClientReplicaSet(_db_name, vec2));

	b_useReplicaSet = true;
	b_init = true;
	init_index();
}

const std::string& db_base::insert(const std::string& table_name, const BSONObj& bObj)
{
	try
	{
		auto pcon = get_conn();
		if(!pcon) return db_err;
		
		pcon->insert(db_name+table_name, bObj);
		db_err = pcon->getLastError(db_name_base);

	}
	catch (std::exception& ex)
	{
		db_err = ex.what();
		return db_err;
	}

	return db_err;
}

const std::string& db_base::insert(const std::string& table_name, const std::vector<mongo::BSONObj>& bObjs)
{
	try
	{
		auto pcon = get_conn();
		if(!pcon) return db_err;
		
		pcon->insert(db_name+table_name, bObjs);
		db_err = pcon->getLastError(db_name_base);
	
	}
	catch (std::exception& ex)
	{
		db_err = ex.what();
		return db_err;
	}

	return db_err;
}

BSONObj db_base::findone(const std::string& table_name, const BSONObj& bObj, const BSONObj* bField)
{
	try
	{
		auto pcon = get_conn();	
		if(!pcon) return BSONObj();
				
		return pcon->findOne(db_name+table_name, Query(bObj), bField);		
		
	}
	catch (std::exception& ex)
	{
		db_err = ex.what();
	}

	return BSONObj();
}

void db_base::find(std::vector<BSONObj>& vec, const std::string& table_name, const BSONObj& bObj, const BSONObj* bField,
				   int toReturn, int toSkip, const BSONObj* bSort)
{
	try
	{
		auto pcon = get_conn();
		if(!pcon) return;
		
		Query q(bObj);
		if(bSort)
		{
			q = q.sort(*bSort);
		}

		if(toReturn == 0)
		{
			toReturn = pcon->count(db_name+table_name, bObj);
			if(toReturn == 0)
				return;
		}

		vec.reserve(toReturn);
		pcon->findN(vec, db_name+table_name, q, toReturn, toSkip, bField);		
		
	}
	catch (std::exception& ex)
	{
		db_err = ex.what();
	}	
}

const std::string& db_base::update(const std::string& table_name, const BSONObj& bObj, const BSONObj& bUp, bool upsert, bool multi)
{
	try
	{
		auto pcon = get_conn();
		if(!pcon) return db_err;
		
		pcon->update(db_name+table_name, Query(bObj), bUp, upsert, multi);
		db_err = pcon->getLastError(db_name_base);

	}
	catch (std::exception& ex)
	{
		db_err = ex.what();
		return db_err;
	}

	return db_err;
}

const std::string& db_base::remove(const std::string& table_name, const BSONObj& bObj, bool justone)
{
	try
	{
		auto pcon = get_conn();
		if(!pcon) return db_err;
	
		pcon->remove(db_name+table_name, Query(bObj), justone);
		db_err = pcon->getLastError(db_name_base);

	}
	catch (std::exception& ex)
	{
		db_err = ex.what();
		return db_err;
	}

	return db_err;
}

bool db_base::ensure_index(const std::string& table_name, const BSONObj& bKey, const std::string& index_name, bool unique)
{
	try
	{
		auto pcon = get_conn();
		if(!pcon) return false;
		
		pcon->createIndex(db_name+table_name, bKey);
		return true;		
	}
	catch (std::exception& ex)
	{
		db_err = ex.what();
		BOOST_ASSERT_MSG(false, "please start mongodb!!!\n");
	}	

	return false;
}

int64_t db_base::get_count(const std::string& table_name, const mongo::BSONObj& bObj)
{
	try
	{
		auto pcon = get_conn();
		if(!pcon) return -1;
		
		return pcon->count(db_name+table_name, bObj);
		
	}
	catch (std::exception& ex)
	{
		db_err = ex.what();
	}

	return -1;
}

// 删除表格table_name
const std::string& db_base::clearTable(const std::string& table_name)
{
	try
	{
		auto pcon = get_conn();
		if(!pcon) return db_err;
		
		pcon->dropCollection(db_name + table_name);
		db_err = pcon->getLastError(db_name_base);
		
	}
	catch (std::exception& ex)
	{
		db_err = ex.what();
	}

	return db_err;
}

bool db_base::exists(const std::string& table_name)
{
	try
	{
		auto pcon = get_conn();		
		if(!pcon) return false;

		return pcon->exists(db_name+table_name);		
	}
	catch (std::exception& ex)
	{
		db_err = ex.what();
	}

	return false;
}

mongo::DBClientBase* db_base::get_conn()
{
	if(!b_init)
		return nullptr;

	if(b_useReplicaSet)
	{
		if(m_conn_rs == nullptr)
			return nullptr;

		return (mongo::DBClientBase*)&(m_conn_rs->masterConn());
	}	

	if(m_conn == nullptr)
	{
		m_conn.reset(new DBClientConnection(true));		

		m_conn->connect(db_addr);

		if(b_auth)//登录验证
		{
			try
			{
				m_conn->setWireVersions(2,2);//2以上需要驱动SSL支持
				if(!m_conn->auth(AuthDB, db_user, db_pwd, db_err))
				{
					m_conn->reset();
					return nullptr;
				}

			}
			catch (std::exception& ex)
			{
				m_conn->reset();
				db_err = ex.what();
				return nullptr;
			}
		}
	}

	return m_conn.get();
}

const std::string& db_base::get_last_error()
{
	return db_err;
}

void db_base::close()
{
	if(b_init)
	{
		b_init = false;
		m_conn.reset();
		db_err = "db:server_closing";
		client::shutdown();
	}	
}