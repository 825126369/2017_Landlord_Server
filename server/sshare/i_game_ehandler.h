#pragma once
#include <i_game_def.h>
#include <vector>
class db_base;

//游戏引擎回调到服务器接口
class i_game_ehandler
{
public:
	i_game_ehandler();
	virtual ~i_game_ehandler();

	//游戏引擎初始化完成（仅成功调用）
	virtual void on_init_engine(uint16_t game_id, const std::string& game_ver) = 0;

	//服务器关闭 游戏引擎退出
	virtual void on_exit_engine() = 0;

	//请求机器人 请求的机器人不一定及时返回  
	//要求的vip 要求的gold  自定义标志tag
	virtual void request_robot(int tag, GOLD_TYPE needgold, int needvip = 0) = 0;
	
	//当不需要使用机器人时 只要退出到房间选择然后调用此函数
	virtual void release_robot(int playerid) = 0;

	virtual void release_player(uint32_t pid) = 0;

	//获取当前机器人数
	virtual int get_robot_count() = 0;

	template<class T>
	int broadcast_msg_to_client(std::vector<uint32_t>& pids,T msg)
	{
		return broadcast_msg_to_client(pids, msg->packet_id(), msg);
	};

	//发送协议到客户端
	virtual int broadcast_msg_to_client(std::vector<uint32_t>& pids, uint16_t packet_id, boost::shared_ptr<google::protobuf::Message> msg) = 0;

	//发送协议列表
	virtual int broadcast_msglist_to_client(std::vector<uint32_t>& pids, std::vector<msg_packet_one>& msglist) = 0;

	virtual bool setGameDb(db_base *dgGame) = 0;

	// 返回当前的局数id
	virtual int64_t getCurId(const std::string& key) = 0;
};

