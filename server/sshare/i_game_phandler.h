#pragma once
#include <i_game_def.h>



//服务器通知玩家逻辑接口
class i_game_phandler
{
public:
	i_game_phandler();
	virtual ~i_game_phandler();

	//属性改变(货币类型都是变化量)
	virtual void on_attribute_change(int atype, int v = 0) = 0;
	virtual void on_attribute64_change(int atype, GOLD_TYPE v = 0) = 0;

	//玩家状态改变
	virtual void on_change_state() = 0;

	void set_player(iGPlayerPtr player);

	/*
			通知玩家收到礼物
			senderId	发送者玩家id
			receiverId	接收者id
			giftId		礼物ID
	*/
	virtual void onAcceptGift(int receiverId, int giftId){}

	i_game_player* getIGamePlayer(){ return m_player.get(); }
protected:
	iGPlayerPtr m_player;
};

