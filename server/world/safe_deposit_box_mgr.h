#pragma once
#include "game_sys_def.h"

class game_player;

// 保险箱
class SafeDepositBoxMgr : public enable_obj_pool<SafeDepositBoxMgr>, public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_safe_deposit_box);

	MAKE_GET_OWNER(game_player);

	virtual void init_sys_object();	

	/*
			设置密码
			只有当密码为空时，才是设置，其他为修改密码
			返回值返回值 e_msg_result_def定义
	*/
	int setPassword(const std::string& pwd1, const std::string& pwd2);

	/*
			存入金币
			返回值返回值 e_msg_result_def定义
	*/
	int depositGold(GOLD_TYPE gold, const std::string& pwd);

	/*
			取出金币
			返回值返回值 e_msg_result_def定义
	*/
	int drawGold(GOLD_TYPE gold, const std::string& pwd);

	/*
			重置保险箱密码
			oldPwd		老密码
			pwd1		输入的新密码1
			pwd2		输入的新密码2
			返回值返回值 e_msg_result_def定义
	*/
	int resetPassword(const std::string& phoneCode, const std::string& pwd1, const std::string& pwd2);

		/*
			验证保险箱密码
			oldPwd		老密码
			pwd1		输入的新密码1
			pwd2		输入的新密码2
			返回值返回值 e_msg_result_def定义
	*/
	int checkPassword(const std::string& pwd);

	/*
			修改保险箱密码
			oldPwd		老密码
			pwd1		输入的新密码1
			pwd2		输入的新密码2
			返回值返回值 e_msg_result_def定义
	*/
	int modifyPassword(const std::string& oldPwd, const std::string& pwd1, const std::string& pwd2);

	/*
			保险箱密码是否为空
	*/
	bool isPasswordEmpty();
private:
	int _checkOp(const std::string& pwd);

	int _setPassword(const std::string& pwd1, const std::string& pwd2);
public:
	// 保险箱密码
	GStringFieldPtr m_password;

	// 存入的金币数量
	Tfield<GOLD_TYPE>::TFieldPtr m_gold;
};

