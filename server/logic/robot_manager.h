#pragma once
#include <enable_hashmap.h>
#include <enable_queue.h>
#include <game_macro.h>

//struct idle_robot
//{
//	int playerid;
//	int vip;
//	int gold;
//	double elapsed;
//};

class robot_manager
{
public:
	robot_manager();
	~robot_manager();

	//请求机器人  返回-1为需要向world请求新机器人
	int request_robot(int tag, GOLD_TYPE needgold, int needvip);

	//释放机器人
	void release_robot(int playerid);
	
	int get_count();
	void inc_robot();
	void heartbeat(double elapsed);

	int pop_tag();
	void set_gameid(int gameid);
private:
	enable_queue<int> m_reqlist;		//请求机器人的数据队列
	int m_gameid;
	void leave_robot(int playerid);
	int m_count;
};