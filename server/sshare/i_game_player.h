#pragma once
#include <i_game_def.h>

enum e_player_state
{
	e_ps_none = 0, //未初始化
	e_ps_loading, //校验中
	e_ps_playing,	//游戏中
	e_ps_disconnect, //断线
};

class i_game_player
{
public:
	i_game_player();
	virtual ~i_game_player();

	//获取玩家id
	virtual uint32_t get_playerid() = 0;

	//获取玩家属性
	virtual int get_attribute(int atype) = 0;

	//获取玩家64位属性
	virtual GOLD_TYPE get_attribute64(int atype) = 0;

	//获取昵称
	virtual const std::string& get_nickname() = 0;

	//获取自定义头像
	virtual const std::string& get_icon_custom() = 0;

	//是否体验VIP
	virtual bool is_ExperienceVIP() = 0; 

	//修改玩家金币(不要频繁调用)可能操作失败
	virtual bool change_gold(GOLD_TYPE cgold) = 0;
	//修改礼券
	virtual bool change_ticket(int cticket) = 0;
	//修改幸运
	virtual bool change_lucky(int clucky, int64_t tempincome, int64_t totalincome) = 0;
	//获取当前状态
	virtual e_player_state get_state() = 0;
	//改变星星属性
	virtual bool add_starinfo(int addaward, int addstar = 0) = 0;
	//月卡是否有效
	virtual bool check_monthcard() = 0; 

	//是否在保护期
	virtual bool is_protect() = 0;
	//是否机器人
	virtual bool is_robot() = 0;

	virtual bool is_BuyFirstGift() = 0;

	//任务成就改变
	virtual void quest_change(int questid, int count=1, int param = 0) = 0;

	//玩家数据改变记录
	virtual void player_property_log(int at, int changecount, int reason, const std::string& param)=0;

	//广播游戏消息
	virtual void game_broadcast(const std::string& roomname, int infotype, const std::string& strinfo, int money, int moneytype) = 0;

	template<class T>
	int send_msg_to_client(T msg)
	{
		return send_msg_to_client(msg->packet_id(), msg);
	};

	//发送协议到客户端
	virtual int send_msg_to_client(uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg) = 0;

	// 返回多语言
	virtual const std::string* getLan(const std::string& lanKey) = 0;

	// 广播消息
	virtual void gameBroadcast(const std::string& msg) = 0;
public:	
	void set_handler(iGPhandlerPtr phandler);
	iGPhandlerPtr get_handler();
protected:
	

	boost::weak_ptr<i_game_phandler> m_phandler;
};
