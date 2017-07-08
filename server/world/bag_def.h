#pragma once
#include "game_object.h"
#include "game_object_container.h"

// µ¿æﬂ
class GameItem : public game_object, public enable_obj_pool<GameItem>
{
public:
	GameItem();
	virtual ~GameItem();
	virtual void init_game_object();//◊¢≤· Ù–‘
	virtual uint32_t get_id();
public:
	Tfield<int32_t>::TFieldPtr m_itemId;
	Tfield<int32_t>::TFieldPtr m_itemCount;
};

class BagMap : public game_object_container, public enable_obj_pool<BagMap>
{
public:
	BagMap();

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
