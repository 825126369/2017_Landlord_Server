#pragma once
#include "game_object_container.h"

// 礼物
struct stGift
{
	// 礼物ID
	int m_giftId;
	// 数量
	GOLD_TYPE m_count;
	bool m_receive;

	stGift()
	{
		m_giftId = -1;
		m_count = 0;
		m_receive = false;
	}
	stGift(int id, GOLD_TYPE count)
	{
		m_giftId = id;
		m_count = count;
		m_receive = false;
	}
};

// 礼物信息
struct GiftInfo : public game_object, public enable_obj_pool<GiftInfo>
{
	// 礼物ID
	Tfield<int32_t>::TFieldPtr m_giftId;

	// 收到的数量
	Tfield<int32_t>::TFieldPtr m_count;

	GiftInfo();

	// 注册属性
	virtual void init_game_object();

	virtual uint32_t get_id();
};

class GiftMap : public game_object_container, public enable_obj_pool<GiftMap>
{
public:
	virtual const std::string& get_cells_name();		

	virtual const std::string& get_index_name();

	virtual uint32_t get_index_id();

	virtual GObjPtr create_game_object(uint32_t object_id);

	virtual const std::string& get_container_name();

	virtual bool is_load();		

	virtual db_base* get_db();

	virtual const mongo::BSONObj& get_id_finder();

	virtual const std::string& get_id_name();

	void attachPlayer(int playerId);
private:
	mongo::BSONObj m_idFinder;

	int m_playerId;
};
