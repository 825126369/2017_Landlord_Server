#pragma once
#include "game_object.h"
#include "game_object_container.h"
#include "gift_def.h"

// 好友信息
class FriendItem : public game_object, public enable_obj_pool<FriendItem>
{
public:
	FriendItem();
	
	virtual void init_game_object();
	
	virtual uint32_t get_id();
public:
	// 好友ID
	Tfield<int32_t>::TFieldPtr m_friendId;
};

class FriendMap : public game_object_container, public enable_obj_pool<FriendMap>
{
public:
	FriendMap();

	virtual const std::string& get_cells_name();		

	virtual const std::string& get_index_name();

	virtual uint32_t get_index_id();

	virtual GObjPtr create_game_object(uint32_t object_id);

	virtual const std::string& get_container_name();

	virtual bool is_load();		

	virtual db_base* get_db();

	virtual const mongo::BSONObj& get_id_finder();

	virtual const std::string& get_id_name();

	void setPlayerId(int playerId);
private:
	int m_playerId;
	mongo::BSONObj m_idFinder;
};

struct stGift;

struct stFriendInfo
{
	// 玩家ID
	int m_friendId;
	
	// 是否在线
	bool m_isOnLine;

	// 性别
	int m_sex;

	// VIP等级
	int m_vipLevel;

	std::string m_nickName;

	//int m_headId;

	std::string m_iconCustom;

	int m_photoFrameId;

	// 签名
	std::string m_selfSignature;

	// 礼物列表
	std::vector<stGift> m_giftList;

	// 赠送礼物金币数量
	GOLD_TYPE m_sendGiftCoinCount;

	// 金币
	GOLD_TYPE m_gold;

	// 经典捕鱼等级
	int m_fishLevel;
};

