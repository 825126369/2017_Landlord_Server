#include "stdafx.h"
#include "mobile_phone_binding_sys.h"
#include "send_code_task.h"
#include "world_server.h"
#include "msg_type_def.pb.h"
#include "game_player.h"
#include "enable_random.h"
#include "M_BaseInfoCFG.h"
#include "time_helper.h"
#include "game_db.h"
#include "game_sys_recharge.h"

using namespace boost;

const char* g_codeType[] = {"0", "1", "2"};

enum CodeType
{
	// 手机绑定类型
	codeTypeMobilePhoneBind,

	// 保险箱验证码
	codeTypeSafeBox,

	//手机解除绑定
	codeTypeMobilePhoneRelive,
};

MobilePhoneBindingSys::MobilePhoneBindingSys() //: m_run(true),
	//m_work(boost::bind(&MobilePhoneBindingSys::_sendCode, this))
{
	m_maxBindCount = M_BaseInfoCFG::GetSingleton()->GetData("bindCount")->mValue;
	m_expiryDate = M_BaseInfoCFG::GetSingleton()->GetData("bindCodeExpiryDate")->mValue * 60;
}

void MobilePhoneBindingSys::sys_update(double delta)
{
	time_t curTime = time_helper::instance().get_cur_time();
	for(auto it = m_user.begin(); it != m_user.end(); ++it)
	{
		time_t delta = curTime - it->second.m_genTime;
		if(delta >= m_expiryDate)
		{
			m_user.erase(it);
			break;
		}
	}

	for(auto it = m_safeBoxUser.begin(); it != m_safeBoxUser.end(); ++it)
	{
		time_t delta = curTime - it->second.m_genTime;
		if(delta >= m_expiryDate)
		{
			m_safeBoxUser.erase(it);
			break;
		}
	}
}

void MobilePhoneBindingSys::sys_exit()
{
	//m_run = false;
}

int MobilePhoneBindingSys::reqBindPhone(game_player* player, const std::string& phone, time_t curTime)
{
	if(player == nullptr || phone == "")
		return msg_type_def::e_rmt_unknow;
		
	if(player->get_viplvl() <=0)
		return msg_type_def::e_rmt_vip_under;

	if(_isBindPhone(player, phone))
		return msg_type_def::e_rmt_has_bind_phone;

	if (!_bindAccountLimit(phone))
		return msg_type_def::e_rmt_has_bind_phone;

	bool isRunOutCount = m_maxBindCount <= player->BindCount->get_value();
	if(isRunOutCount)
		return msg_type_def::e_rmt_chat_too_often;

	auto it = m_user.find(player->PlayerId->get_value());
	if (it != m_user.end())
	{
		return msg_type_def::e_rmt_chat_too_often;
	}

	stUserBindInfo bindInfo;

	bindInfo.m_genTime = curTime;
	bindInfo.m_idCode = _genIdentifyingCode();
	bindInfo.m_phone = phone;
	bindInfo.m_codeType = codeTypeMobilePhoneBind;
	m_user[player->PlayerId->get_value()] = bindInfo;

	player->BindCount->add_value(1);

	//player->store_game_object();
	//m_sendQue.push(bindInfo);
	_send(bindInfo);

	return msg_type_def::e_rmt_success;
}

int MobilePhoneBindingSys::reqVerifyCode(game_player* player, const std::string& code, time_t curTime)
{
	if(player == nullptr)
		return msg_type_def::e_rmt_unknow;

	auto it = m_user.find(player->PlayerId->get_value());
	if (it == m_user.end())
	{
		return msg_type_def::e_rmt_code_error;
	}

	if(it->second.m_idCode != code)
		return msg_type_def::e_rmt_code_error;

	if(_isBindPhone(player, it->second.m_phone))
		return msg_type_def::e_rmt_has_bind_phone;

	if (!_bindAccountLimit(it->second.m_phone))
		return msg_type_def::e_rmt_has_bind_phone;

	player->BindPhone->set_string(it->second.m_phone);
	m_user.erase(it);
	//player->store_game_object();

	_sendphone(player);

	return msg_type_def::e_rmt_success;
}

int MobilePhoneBindingSys::getSafeBoxSecurityCode(game_player* player, time_t curTime)
{
	if(player == nullptr)
		return msg_type_def::e_rmt_unknow;

	if(player->BindPhone->get_string().empty())
		return msg_type_def::e_rmt_not_bind_phone;

	bool isRunOutCount = m_maxBindCount <= player->FetchSafeBoxSecurityCodeCount->get_value();
	if(isRunOutCount)
		return msg_type_def::e_rmt_chat_too_often;

	auto it = m_safeBoxUser.find(player->PlayerId->get_value());
	if (it != m_safeBoxUser.end())
	{
		return msg_type_def::e_rmt_chat_too_often;
	}

	stUserBindInfo bindInfo;

	bindInfo.m_genTime = curTime;
	bindInfo.m_idCode = _genIdentifyingCode();
	bindInfo.m_phone = player->BindPhone->get_string();
	bindInfo.m_codeType = codeTypeSafeBox;
	m_safeBoxUser[player->PlayerId->get_value()] = bindInfo;

	player->FetchSafeBoxSecurityCodeCount->add_value(1);

	//player->store_game_object();
	//m_sendQue.push(bindInfo);
	_send(bindInfo);
	return msg_type_def::e_rmt_success;
}

int MobilePhoneBindingSys::reqVerifySafeBoxSecurityCode(game_player* player, const std::string& code)
{
	if(player == nullptr)
		return msg_type_def::e_rmt_unknow;

	auto it = m_safeBoxUser.find(player->PlayerId->get_value());
	if (it == m_safeBoxUser.end())
	{
		return msg_type_def::e_rmt_code_error;
	}

	if(it->second.m_idCode != code)
		return msg_type_def::e_rmt_code_error;

	m_safeBoxUser.erase(it);

	return msg_type_def::e_rmt_success;
}

#ifdef _DEBUG

std::string MobilePhoneBindingSys::getCode(int playerId)
{
	auto it = m_user.find(playerId);
	if(it != m_user.end())
		return it->second.m_idCode;
	return "";
}

#endif

//void MobilePhoneBindingSys::_sendCode()
//{
//	stUserBindInfo info;
//	while(m_run)
//	{
//		while(!m_sendQue.empty())
//		{
//			m_sendQue.pop(info);
//			_send(info);
//		}
//		this_thread::sleep(posix_time::seconds(10));
//	}
//}

void MobilePhoneBindingSys::_send(stUserBindInfo& userInfo)
{
	auto task = boost::make_shared<SendCode>(world_server::instance().get_io_service());
	msg_request mr;
	mr.uri = world_server::instance().get_cfg().get<std::string>("http_check");
	mr.spath = "/CheckCode.aspx?phone=" + userInfo.m_phone 
		+ "&code=" + userInfo.m_idCode
		+ "&type=" + g_codeType[userInfo.m_codeType];
	task->post_request(mr);
}

void MobilePhoneBindingSys::_sendphone(game_player* player)
{
	auto task = boost::make_shared<SendCode>(world_server::instance().get_io_service());
	msg_request mr;
	mr.uri = world_server::instance().get_cfg().get<std::string>("http_check");
	mr.spath = "/BindPhone.aspx?phone=" + player->BindPhone->get_string()
		+ "&acc=" + player->Account->get_string()
		+ "&platform=" + player->LoginPlatform->get_string();

	task->post_request(mr);
}

static mongo::BSONObj g_retField = BSON("player_id" << 1);

bool MobilePhoneBindingSys::_isBindPhone(game_player* player, const std::string& phone)
{
	if(!player->BindPhone->get_string().empty())
		return true;

	return false;
}

bool MobilePhoneBindingSys::_bindAccountLimit(const std::string& phone)
{
	mongo::BSONObj cond = BSON("bindPhone" << phone);
	std::vector<mongo::BSONObj> vec;
	db_player::instance().find(vec, DB_PLAYER_INFO, cond, &g_retField);
	return vec.size() < 5;
}

std::string MobilePhoneBindingSys::_genIdentifyingCode()
{
	static char buf[7] = {0};
	int i = 0;
	for (i = 0; i < 6; i++)
	{
		buf[i] = global_random::instance().rand_int(0, 9) + '0';
	}
	return std::string(buf);
}


//////////////////////////////////////////////////////////////////////////
//请求解除验证码
int MobilePhoneBindingSys::reqReliveVerify(game_player* player)
{
	if(player == nullptr)
		return msg_type_def::e_rmt_unknow;

	if(player->BindPhone->get_string().empty())
		return msg_type_def::e_rmt_not_bind_phone;

	stUserBindInfo bindInfo;

	bindInfo.m_genTime = time_helper::instance().get_cur_time();
	bindInfo.m_idCode = _genIdentifyingCode();
	bindInfo.m_phone = player->BindPhone->get_string();
	bindInfo.m_codeType = codeTypeMobilePhoneRelive;
	m_user[player->PlayerId->get_value()] = bindInfo;

	player->BindCount->add_value(1);

	_send(bindInfo);
	return msg_type_def::e_rmt_success;
}
//请求解除手机
int MobilePhoneBindingSys::reqRelivePhone(game_player* player, const std::string& code)
{
	if(player == nullptr)
		return msg_type_def::e_rmt_unknow;

	auto it = m_user.find(player->PlayerId->get_value());
	if (it == m_user.end())
	{
		return msg_type_def::e_rmt_code_error;
	}

	if(it->second.m_idCode != code)
		return msg_type_def::e_rmt_code_error;

	if(player->BindPhone->get_string().empty())
		return msg_type_def::e_rmt_not_bind_phone;

	player->BindPhone->set_string(it->second.m_phone);
	m_user.erase(it);
	//player->store_game_object();

	_sendphone(player);

	return msg_type_def::e_rmt_success;
}