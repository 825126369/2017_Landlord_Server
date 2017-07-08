#include "stdafx.h"
#include "bag_mgr.h"
#include "bag_def.h"
#include "game_db.h"
#include "game_player.h"
#include "game_sys_recharge.h"

void BagMgr::init_game_object()
{
	m_playerId = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "player_id"));
	m_items = regedit_mapfield("items", BagMap::malloc());
	m_itemsPtr = m_items->get_Tmap<BagMap>();
}

void BagMgr::init_sys_object()
{
	init_game_object();
}

bool BagMgr::sys_load()
{
	if(get_game_player() == NULL)
	{
		SLOG_CRITICAL << "BagMgr::sys_load没有设置玩家实例";
		return false;
	}

	bool res = false;
	mongo::BSONObj b = db_player::instance().findone(DB_PLAYER_BAG, get_game_player()->get_id_finder());
	if(b.isEmpty())
	{
		m_playerId->set_value(get_game_player()->PlayerId->get_value());
		db_player::instance().update(DB_PLAYER_BAG, get_game_player()->get_id_finder(),BSON("$set" << to_bson(true)));
		res = true;
	}
	else
	{
		res = from_bson(b);
	}

	m_itemsPtr->setPlayerId(m_playerId->get_value());
	return res;
}

bool BagMgr::addItem(int id, int count)
{
	if(count == 0)
		return false;

	if(count <= 0)
	{
		return removeItem(id, -count);
	}

	auto pItem = m_itemsPtr->find_Tobj<GameItem>(id);
	if(pItem != nullptr)
	{
		pItem->m_itemCount->add_value(count);
		m_itemsPtr->db_update(pItem);
	}
	else
	{
		auto tmpItem = GameItem::malloc();
		tmpItem->m_itemId->set_value(id);
		tmpItem->m_itemCount->set_value(count);
		m_itemsPtr->put_obj(tmpItem);
		m_itemsPtr->db_add(tmpItem);
	}

	return true;
}

bool BagMgr::removeItem(int id, int count)
{
	if(count <= 0)
		return false;

	auto pItem = m_itemsPtr->find_Tobj<GameItem>(id);
	if(pItem == nullptr)
		return false;

	pItem->m_itemCount->add_value(-count);
	int tmp = pItem->m_itemCount->get_value();
	if(tmp <= 0)
	{
		m_itemsPtr->del_obj(id);
		m_itemsPtr->db_del(pItem);
	}
	else
	{
		m_itemsPtr->db_update(pItem);
	}

	return true;
}

bool BagMgr::removeItem(int id)
{
	auto pItem = m_itemsPtr->find_Tobj<GameItem>(id);
	if(pItem == nullptr)
		return false;

	m_itemsPtr->del_obj(id);
	m_itemsPtr->db_del(pItem);
	return true;
}

bool BagMgr::empty()
{
	return m_itemsPtr->get_obj_count() == 0;
}

GameItem* BagMgr::getItem(int id)
{
	auto pItem = m_itemsPtr->find_Tobj<GameItem>(id);
	if(pItem == nullptr)
		return NULL;

	return pItem.get();
}

int BagMgr::getItemCount(int id)
{
	auto pItem = getItem(id);
	if(pItem != nullptr)
		return pItem->m_itemCount->get_value();

	return 0;
}

BagMap* BagMgr::getBagMap()
{
	return m_itemsPtr.get();
}

void BagMgr::doActivity()
{
	auto player = get_game_player();
	if(player == nullptr)
		return;

	int val = player->get_sys<game_sys_recharge>()->Recharged->get_value();
	doActivity(val);
}

void BagMgr::doActivity(int rechargeVal)
{
	auto player = get_game_player();
	if(player == nullptr)
		return;

	if(rechargeVal > 0)
	{
		int count = getItemCount(50115);
		if(count == 0)
		{
			addItem(50115, 1);
		}
	}
}




















