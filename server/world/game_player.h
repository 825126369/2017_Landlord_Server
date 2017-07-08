#pragma once
#include <boost/cstdint.hpp>
#include "game_def.h"
#include <string>
#include <game_sys_mgr.h>

enum e_player_state
{
	e_ps_none = 0, //未初始化
	e_ps_checking, //校验中
	e_ps_playing,	//游戏中
	e_ps_disconnect, //断线
};

class world_peer;
class GiftMap;

class game_player:
	public game_object
	,public enable_obj_pool<game_player>
	,public boost::enable_shared_from_this<game_player>
	,public game_sys_mgr
{
public:
	game_player();
	virtual ~game_player();

	void heartbeat( double elapsed );

	
	void player_logout();
	void on_logout();
	void player_login(uint32_t sessionid,
		const std::string& account, const std::string& token, const std::string& platform, const std::string& login_platform);
	void player_relogin(const std::string& token);

	
	uint32_t get_sessionid();
	void set_sessionid(uint32_t sessionid);
	e_player_state get_state();

	static bool check_token(const std::string& account, const std::string& token, const std::string& sign);

	boost::shared_ptr<world_peer> get_logic();//获取逻辑
	boost::shared_ptr<world_peer> get_gate();//获取网关
	bool check_logic();//检测逻辑服务器连接
	void clear_logic(uint16_t lid = 0);
	template<class T>
	int send_msg_to_client(T msg)//发送协议到客户端
	{
		return send_msg_to_client(msg->packet_id(), msg);
	};

	GOLD_TYPE get_gold();

	void change_property(int type);
	int change_gold(GOLD_TYPE dif_gold, bool needsend = true, bool check = false, int reason = 100);
	void change_ticket(int dif_ticket, bool needsend = true);
	void change_chip(int dif_chip);
	void change_vip(int vip);
	void change_lucky(int dif_lucky);
	bool is_gaming();
	bool join_game(uint16_t gameid,uint16_t serverid);

	// 仅设置玩家所在的游戏ID及所在logic id
	bool setGameIdServerId(uint16_t gameId, uint16_t serverId);

	// 重置游戏ID及Logic id
	bool resetGameIdServerId();

	void leave_game();
	uint16_t get_logicid();
	uint16_t get_gameid();
	void on_joingame(bool blogin = true);
	void init_sys();

	const mongo::BSONObj& get_id_finder();
	bool load_player();
	
	bool loadPlayer(mongo::BSONObj& b);

	//加载机器人
	void load_robot(int playerid);

	bool http_run(const std::string& respose);
	void http_check(bool success, const std::string& respose, bool isrelogin = false);

	// 修改玩家昵称
	int updateNickname(const std::string& newname);

	// 收到礼物
	int addGift(int giftId, GOLD_TYPE count, const std::string& param);

	int addItem(int itemId, GOLD_TYPE count, int reason = 0, const std::string& param = "");

	// 0点重置玩家信息
	void resetPlayerInfo();

	void setFirstGift();
	//增加体验vip时间
	bool addExperienceVIP(int day);
	//int get_channel();

	int get_viplvl();
	bool first_login();
	void check_firstgift(int32_t gold);

	bool is_goldshop();

	bool IsLogin()
	{
		return islogin_success;
	}
protected:
	void player_gold_log(GOLD_TYPE old_value, GOLD_TYPE new_value, int32_t reason);
private:
	uint32_t m_sessionid;
	uint16_t m_logicid;
	uint16_t m_gameid;
	e_player_state m_state;

	std::string m_token;

	time_t m_login_time;
	double m_check_logout;
	double m_check_save;
	mongo::BSONObj m_id_finder;
	void start_check(bool isrelogin = false);

	void reset_logicpeer();
	void reset_gatepeer();
	boost::weak_ptr<world_peer> m_logicpeer;
	boost::weak_ptr<world_peer> m_gatepeer;
	int send_msg_to_client(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg);
	
private:	
	//////////////////////////////////////////////////////////////////////////
	//属性
	virtual void init_game_object();//注册属性
	virtual void to_bson_ex(mongo::BSONObjBuilder& ba);//扩展

	void create_player(bool isRobot = false);

	bool _checkReflush();

	std::string _newAccountName();
	bool islogin_success;

	void heartbeatRobotVip(double elapsed);
	double m_robotVipTime;
public:
	//一次操作执行结束 有数据修改调用一次  无数据修改不要调用
	virtual bool store_game_object(bool to_all = false);

	GStringFieldPtr					Account;			//账号
	GStringFieldPtr					NickName;			//昵称
	GStringFieldPtr					Platform;			//平台
	GStringFieldPtr					LoginPlatform;		//平台
	Tfield<int32_t>::TFieldPtr		PlayerId;			//玩家id
	Tfield<GOLD_TYPE>::TFieldPtr	Gold;			    //金币
	Tfield<int32_t>::TFieldPtr		Ticket;				//钻石
	Tfield<int8_t>::TFieldPtr		Sex;				//性别，0不明，1男，2女
	Tfield<bool>::TFieldPtr			IsRobot;			//是否机器人

	Tfield<int32_t>::TFieldPtr		Chip;				//碎片
	Tfield<int32_t>::TFieldPtr		OnlineTime;			//在线时间
	Tfield<time_t>::TFieldPtr		LogoutTime;			//下线时间
	GStringFieldPtr					IconCustom;			//自定义头像
	Tfield<int32_t>::TFieldPtr		PhotoFrameId;		//当前系统头像ID
	Tfield<time_t>::TFieldPtr		ExperienceVIP;		//体验vip

	// 上传自定义头像的冻结截止时间
	Tfield<time_t>::TFieldPtr		UpLoadCustomHeadFreezeDeadTime;		

	Tfield<int16_t>::TFieldPtr		UpdateIconCount;	//修改头像次数

	GStringFieldPtr					SelfSignature;		//签名
	GMapFieldPtr					m_giftStat;			//收礼物的统计
	boost::shared_ptr<GiftMap>		m_giftStatPtr;
	Tfield<GOLD_TYPE>::TFieldPtr		MaxGold;		//金币最大持有记录
	Tfield<int32_t>::TFieldPtr		MaxTicket;			//礼券最大持有记录

	Tfield<time_t>::TFieldPtr		LastCheckTime;		//最后1次检测刷新的时间
	Tfield<int16_t>::TFieldPtr		OnlineRewardCount;	//在线已领奖次数

	time_t						    LastGameChatTime;   // 上次世界发言时间
	GStringFieldPtr					BindPhone;			// 绑定的手机
	Tfield<int8_t>::TFieldPtr		BindCount;			// 今日绑定手机的次数
	Tfield<int32_t>::TFieldPtr		NewGuildHasFinishStep; // 新手引导已完成步骤

	Tfield<GOLD_TYPE>::TFieldPtr	SendGiftCoinCount;	// 今日赠送礼物的所含金币总和
	Tfield<int8_t>::TFieldPtr		FetchSafeBoxSecurityCodeCount; // 今日获取保险箱验证码次数

	Tfield<int8_t>::TFieldPtr		UpdateNickCount;	// 改名次数

	Tfield<time_t>::TFieldPtr		CreateTime;		//创建时间

	Tfield<int32_t>::TFieldPtr		ChannelID;			//渠道ID
	GStringFieldPtr					OldAcc;			//旧账号

	GMapFieldPtr					CheckMap;			//通用的标识map
	Tfield<time_t>::TFieldPtr		KickEndTime;		//踢下线结束

	Tfield<int32_t>::TFieldPtr		FirstGiftTime;	// 首冲礼包时间-1表示买了
	Tfield<int32_t>::TFieldPtr		WinCount;	//赢次数

	Tfield<int32_t>::TFieldPtr		PlayerType;	//玩家类型

	Tfield<int32_t>::TFieldPtr		Lucky;				//玩家幸运
	Tfield<int64_t>::TFieldPtr		TempIncome;			//玩家临时收益(幸运值产生的收益，次轮幸运值结束，清零)
	Tfield<int64_t>::TFieldPtr		TotalIncome;		//玩家总收益(幸运值产生的收益，不清零)
	Tfield<int32_t>::TFieldPtr		Privilege;			//特权
	GStringFieldPtr					LastIP;
	Tfield<int16_t>::TFieldPtr		LastPort;
	
	std::string MachineCode;
	std::string MachineType;
	int32_t m_channel;
};

