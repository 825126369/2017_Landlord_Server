#include "stdafx.h"
#include "safe_deposit_box_mgr.h"
#include "game_player.h"
#include "msg_type_def.pb.h"
#include <boost/regex.hpp>
#include "enable_string_convert.h"
#include <enable_crypto.h>
#include "pump_type.pb.h"
#include "global_sys_mgr.h"
#include "mobile_phone_binding_sys.h"
#include "M_BaseInfoCFG.h"
#include "player_log_mgr.h"

static boost::wregex PWD_RULE(L"^[a-zA-Z0-9]{6,8}$");

static std::string DepositAESKEY = "&@*(#kas9081fajk";
static std::string EMPTY_MD5 = "D41D8CD98F00B204E9800998ECF8427E";

void SafeDepositBoxMgr::init_sys_object()
{
	auto ptrPlayer = get_game_player();
	m_password = ptrPlayer->regedit_strfield("safeBoxPwd");	
	m_gold = CONVERT_POINT(Tfield<GOLD_TYPE>, ptrPlayer->regedit_tfield(GOLD_OBJ_TYPE, "safeBoxGold"));
}

int SafeDepositBoxMgr::setPassword(const std::string& pwd1, const std::string& pwd2)
{
	if(!m_password->get_string().empty())
		return msg_type_def::e_rmt_unknow;

	int retCode = _setPassword(pwd1, pwd2);
	if(retCode == msg_type_def::e_rmt_success)
	{
		//get_game_player()->store_game_object();
	}
	return retCode;
}

int SafeDepositBoxMgr::depositGold(GOLD_TYPE gold, const std::string& pwd)
{
	if(gold <= 0)
		return msg_type_def::e_rmt_unknow;

	auto ptrPlayer = get_game_player();
	if(ptrPlayer->Gold->get_value() < gold)
		return msg_type_def::e_rmt_gold_not_enough;

	/*int res = _checkOp(pwd);
	if(res != msg_type_def::e_rmt_success)
		return res;*/

	if(m_gold->get_value() > MAX_MONEY - gold)
	{
		return msg_type_def::e_rmt_beyond_limit;
		//gold = MAX_MONEY - m_gold->get_value();
		//m_gold->set_value(MAX_MONEY);
	}
	else
	{
		m_gold->add_value(gold);
	}

	if(gold == 0)
		return msg_type_def::e_rmt_beyond_limit;

	ptrPlayer->addItem(msg_type_def::e_itd_gold, -gold, type_reason_deposit_safebox);
	static int safeBoxLogMaxCount = M_BaseInfoCFG::GetSingleton()->GetData("safeBoxLogMaxCount")->mValue;
	ptrPlayer->get_sys<PlayerLogMgr>()->addSafeBoxLog(gold, ptrPlayer->get_gold(), safeBoxLogMaxCount);
	//ptrPlayer->store_game_object();

	return msg_type_def::e_rmt_success;
}

int SafeDepositBoxMgr::drawGold(GOLD_TYPE gold, const std::string& pwd)
{
	if(gold <= 0)
		return msg_type_def::e_rmt_unknow;

	auto ptrPlayer = get_game_player();
	if(m_gold->get_value() < gold)
		return msg_type_def::e_rmt_gold_not_enough;

	int res = _checkOp(pwd);
	if(res != msg_type_def::e_rmt_success)
		return res;

	GOLD_TYPE t = ptrPlayer->Gold->get_value();
	if(t > MAX_MONEY - gold)
	{
		return msg_type_def::e_rmt_beyond_limit;
	}

	ptrPlayer->addItem(msg_type_def::e_itd_gold, gold, type_reason_draw_safebox);
	m_gold->add_value(-gold);

	static int safeBoxLogMaxCount = M_BaseInfoCFG::GetSingleton()->GetData("safeBoxLogMaxCount")->mValue;
	ptrPlayer->get_sys<PlayerLogMgr>()->addSafeBoxLog(-gold, ptrPlayer->get_gold(), safeBoxLogMaxCount);
	//ptrPlayer->store_game_object();

	return msg_type_def::e_rmt_success;
}

int SafeDepositBoxMgr::modifyPassword(const std::string& oldPwd, const std::string& pwd1, const std::string& pwd2)
{
	if(m_password->get_string().empty())
		return msg_type_def::e_rmt_need_set_pwd;

	std::string old = enable_crypto_helper::AESDecryptString(oldPwd, DepositAESKEY);
 	if(old != m_password->get_string())
 		return msg_type_def::e_rmt_pwd_error;  // Ô­ÃÜÂë´íÎó

	int retCode = _setPassword(pwd1, pwd2);
	if(retCode == msg_type_def::e_rmt_success)
	{
		//get_game_player()->store_game_object();
	}
	return retCode;
}

int SafeDepositBoxMgr::resetPassword(const std::string& phoneCode, const std::string& pwd1, const std::string& pwd2)
{
	if(m_password->get_string().empty())
		return msg_type_def::e_rmt_need_set_pwd;

	int result = GLOBAL_SYS(MobilePhoneBindingSys)->reqVerifySafeBoxSecurityCode(get_game_player(), phoneCode);
	if(result != msg_type_def::e_rmt_success)
		return result;

	int retCode = _setPassword(pwd1, pwd2);
	if(retCode == msg_type_def::e_rmt_success)
	{
		//get_game_player()->store_game_object();
	}
	return retCode;
}

int SafeDepositBoxMgr::checkPassword(const std::string& pwd)
{
	if(m_password->get_string().empty())
		return msg_type_def::e_rmt_need_set_pwd;

	std::string old = enable_crypto_helper::AESDecryptString(pwd, DepositAESKEY);
	if(old != m_password->get_string())
		return msg_type_def::e_rmt_pwd_error;  // Ô­ÃÜÂë´íÎó

	return msg_type_def::e_rmt_success;
}

bool SafeDepositBoxMgr::isPasswordEmpty()
{
	return m_password->get_string().empty();
}

int SafeDepositBoxMgr::_checkOp(const std::string& pwd)
{
	if(m_password->get_string().empty())
		return msg_type_def::e_rmt_need_set_pwd;

	std::string resPwd = enable_crypto_helper::AESDecryptString(pwd, DepositAESKEY);

	if(resPwd != m_password->get_string())
		return msg_type_def::e_rmt_pwd_error;

	return msg_type_def::e_rmt_success;
}

int SafeDepositBoxMgr::_setPassword(const std::string& pwd1, const std::string& pwd2)
{
	if(pwd1.empty() || pwd2.empty())
		return msg_type_def::e_rmt_format_invalid;

	// ½â³ÉMD5
	std::string resPwd = enable_crypto_helper::AESDecryptString(pwd1, DepositAESKEY);
	std::string resPwd2 = enable_crypto_helper::AESDecryptString(pwd2, DepositAESKEY);
	if(resPwd != resPwd2)
		return msg_type_def::e_rmt_pwd_not_same;
	if(resPwd == EMPTY_MD5)
		return msg_type_def::e_rmt_format_invalid;

	m_password->set_string(resPwd);
	return msg_type_def::e_rmt_success;
}




