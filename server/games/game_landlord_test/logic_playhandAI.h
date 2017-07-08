#pragma once
#include "proc_game_landlord_protocol.h"
#include <vector>
#include "logic_core.h"
#include "logic_player.h"
#include "logic_table.h"

class cardTypeInfo
{
public:
	cardType type;
	std::vector<int> cards;
};

class combineTypeInfo
{
public:
	std::vector<cardTypeInfo> mCardTypeList;
};

class logic_playhandAI
{
	friend class logic_robLandlordAI;
public:
	void InitAI();
	logic_playhandAI(logic_core* cardmanager);
	game_landlord_net_humanmachine_protocol::card_Info get_rob_playhand(logic_player* player);
	void Init_Card_Type(int deskId);
	void Init_Card_Type(std::vector<int>& sort_cards_1);
	void Init_Card_Type_1(int deskId);
	std::vector<int> get_Card_Type_List(std::vector<int>& sort_cards_2);
	std::vector<int> get_Card_Type_List_1(int);
	std::vector<int> get_Card_Type_List_1(std::vector<cardType>& exceptType,bool orReverse=false);
	int IS_JIZHANGPAI(std::vector<int>& cards, int index);
	bool IS_SHUANGFEI(std::vector<int>& cards, int index);
	int IS_DANZHANGPAISHUNZI(std::vector<int>& cards, int index);
	int IS_ERZHANGPAISHUNZI(std::vector<int>& cards, int index);
	int IS_SANZHANGPAISHUNZI(std::vector<int>& cards, int index);
	int IS_SIZHANGPAISHUNZI(std::vector<int>& cards, int index);

	std::vector<int> Get_Playhand_DANPAI(int point);//���ƣ�����
	std::vector<int> Get_PlayHand_DANPAISHUNZI(int point, int count);//���ƣ�����˳��
	std::vector<int> Get_PlayHand_DUIPAI(int point);//���ƣ�����
	std::vector<int> Get_PlayHand_DUIPAISHUNZI(int point, int count);//���ƣ�����˳��
	std::vector<int> Get_PlayHand_SANTIAO(int point);//���ƣ�����
	std::vector<int> Get_PlayHand_SANDAIYI(int point);//���ƣ�����1
	std::vector<int> Get_PlayHand_SANDAIYIDUI(int point);//���ƣ�����1��
	std::vector<int> Get_PlayHand_SANSHUN(int point, int count);//���ƣ���˳
	std::vector<int> Get_PlayHand_SANDAIYISHUNZI(int point, int count);//���ƣ�����һ ˳��
	std::vector<int> Get_PlayHand_SANDAIYIDUISHUNZI(int point, int count);//���ƣ�����һ�� ˳��
	std::vector<int> Get_PlayHand_SITIAO(int point);//���ƣ�����
	std::vector<int> Get_PlayHand_SIDAIYI(int point);//���ƣ�����1
	std::vector<int> Get_PlayHand_SIDAIYIDUI(int point);//���ƣ�����1��
	std::vector<int> Get_PlayHand_SIDAIER(int point);//���ƣ�����2
	std::vector<int> Get_PlayHand_SIDAIERDUI(int point);//���ƣ�����2��
	std::vector<int> Get_PlayHand_SITIAOSHUNZI(int point, int count);//���ƣ����� ˳��
	std::vector<int> Get_PlayHand_SIDAIYISHUNZI(int point, int count);//���ƣ�����1 ˳��
	std::vector<int> Get_PlayHand_SIDAIYIDUISHUNZI(int point, int count);//���ƣ�����1�� ˳��
	std::vector<int> Get_PlayHand_SIDAIERSHUNZI(int point, int count);//���ƣ�����2 ˳��
	std::vector<int> Get_PlayHand_SIDAIERDUISHUNZI(int point, int count);//���ƣ�����2�� ˳��
	std::vector<int> Get_PlayHand_SHUANGFEI();//���ƣ�˫��

	std::vector<int> Get_Playhand_DANPAI_1(int count);//�õ� ���ĵ���
	std::vector<int> Get_Playhand_DUIPAI_1(int count);//�õ� ���Ķ���
private:
	void playhand(std::vector<int>& m_desk_cards, std::vector<int>& cards);
	bool orCamp(int deskId, int lastdeskId);
	bool NextOrCamp(int deskId);
	bool orRenege(int deskId, int lastdeskId);
	int get_enemy_min_cards(int deskId);
	int get_friend_cards(int deskId);
private:
	std::vector<cardTypeInfo> mCardTypeList;
	std::vector<cardTypeInfo> mCardTypeList_1;
	std::vector<combineTypeInfo> mCombineTypeList;
	logic_core* m_cardmanager;

	int currentwhoplayHandId;
	bool orrenege;//�Ƿ����
	bool orcamp;//�ϼҳ��Ƶģ��Ƿ�ͬһ��Ӫ
	std::map<int,int> currentContinueNotPlayHandCountMap;
};