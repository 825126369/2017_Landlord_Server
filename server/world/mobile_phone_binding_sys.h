#pragma once
#include "game_sys_def.h"
struct stUserBindInfo;

// 手机绑定系统
class MobilePhoneBindingSys : public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_mobile_phone_binding);

	MobilePhoneBindingSys();

	virtual void sys_update(double delta);

	virtual void sys_exit();

	/*
			请求手机绑定
	*/
	int reqBindPhone(game_player* player, const std::string& phone, time_t curTime);

	/*
			请求手机绑定码验证
	*/
	int reqVerifyCode(game_player* player, const std::string& code, time_t curTime);

	//请求解除验证码
	int reqReliveVerify(game_player* player);
	//请求解除手机
	int reqRelivePhone(game_player* player, const std::string& code);

	/*
			获取修改保险箱密码的验证码
	*/
	int getSafeBoxSecurityCode(game_player* player, time_t curTime);

	/*
			请求验证保险箱码
	*/
	int reqVerifySafeBoxSecurityCode(game_player* player, const std::string& code);

#ifdef _DEBUG
	std::string getCode(int playerId);
#endif
private:
	// 发送验证码
	//void _sendCode();

	void _sendphone(game_player* player);

	void _send(stUserBindInfo& userInfo);

	bool _isBindPhone(game_player* player, const std::string& phone);

	//已经绑定过5个账号
	bool _bindAccountLimit(const std::string& phone);

	// 生成6位随机数字验证码
	std::string _genIdentifyingCode();
private:
	// 是否运行线程
	//bool m_run;
	// 发送验证码码队列
	//fast_safe_queue<stUserBindInfo> m_sendQue;
	// 工作线程
	//boost::thread m_work;

	ENABLE_MAP<int, stUserBindInfo> m_user;

	ENABLE_MAP<int, stUserBindInfo> m_safeBoxUser;

	// 一天最多绑定次数
	int m_maxBindCount;
	// 验证码的有效期
	int m_expiryDate;
};

// 绑定相关信息
struct stUserBindInfo
{
	// 验证码
	std::string m_idCode;

	// 验证码生成时间
	time_t m_genTime;

	// 返回码
	int m_retCode;

	// 手机号
	std::string m_phone;

	int m_codeType;
};

