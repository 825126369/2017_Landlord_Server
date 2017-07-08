#pragma once
#include "game_sys_def.h"

class game_player;
class FriendMap;
struct stFriendInfo;

// 好友管理
class FriendMgr : public enable_obj_pool<FriendMgr>, public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_friend);

	MAKE_GET_OWNER(game_player);

	virtual void init_sys_object();	

	virtual bool sys_load();

	/*
			添加好友
			返回值返回值 e_msg_result_def定义
	*/
	int addFriend(int friendId);

	/*
			移除好友
			返回值返回值 e_msg_result_def定义
	*/
	int removeFriend(int friendId);

	/*
			获取好友列表
	*/
	void getFriendList(std::vector<stFriendInfo>& friendList);

	/*
			返回好友信息
			friendId		好友ID
			返回true成功，false失败
	*/
	bool getFriendInfo(int friendId, stFriendInfo& info);

	/*
			是否存在某好友
	*/
	bool hasFriend(int friendId);

	/*
			搜索玩家
			playerId		玩家ID，不一定为好友
			返回值返回值 e_msg_result_def定义
	*/
	int searchPlayer(int playerId, stFriendInfo& info);
private:
	bool _getFriendInfo(int friendId, stFriendInfo& info);
private:
	// 好友
	GMapFieldPtr m_friends;

	boost::shared_ptr<FriendMap> m_friendsPtr;
};

