#pragma once
#include "proc_game_landlord_protocol.h"
#include "logic_player.h"

enum cardType
{
	NONE=0,
	SHUANGFEI=1,
	SIZHANGPAI=2,
	SANZHANGPAI=3,
	ERZHANGPAI=4,
	DANZHANGPAI=5,
	SIDAIYI=6,
	SIDAIYIDUI=7,
	SIDAIER=8,
	SIDAIERDUI=9,
	SANDAIYI=10,
	SANDAIYIDUI=11,
	SIZHANGPAISHUNZI=12,
	SIDAIYISHUNZI=13,
	SIDAIYIDUISHUNZI=14,
	SIDAIERSHUNZI=15,
	SIDAIERDUISHUNZI=16,
	SANZHANGPAISHUNZI=17,
	SANDAIYISHUNZI=18,
	SANDAIYIDUISHUNZI=19,
	ERZHANGPAISHUNZI=20,
	DANPAISHUNZI=21,
};

struct LandlordNetHuman_RoomCFGData;
class logic_playhandAI;
class logic_jipaiqi;
class logic_robLandlordAI;
class logic_core
{
	friend class logic_playhandAI;
	friend class logic_jipaiqi;
	friend class logic_robLandlordAI;
public:
	logic_core(const LandlordNetHuman_RoomCFGData*);
	~logic_core();
	void init();
	void send_card();
	void GameFinish();
	std::map<int, std::vector<int>>& get_cards_info() { return m_cardMap; }
	std::vector<int>& get_cards_info(int desk) { return m_cardMap[desk]; }

	int get_openCard() { return mOpenCard; }
	int get_Landlord() { return landlord_id; }
	const std::vector<int>& get_RemainLandlordCards() { return m_RemainlandlordCards; }
	void SureLandlord(int deskId);
	bool check_cards(int deskId, const std::vector<int>& cards_1);
	int compare_card(std::vector<int>& cards_1, std::vector<int>& cards_2);
	void playhand(int deskid,std::vector<int>& cards);
	void playhand(std::vector<int>& desk_cards, std::vector<int>& cards);
	game_landlord_net_human_protocol::card_Info logic_core::get_rob_playhand(logic_player* player);
	bool judge_rob_orRobLandlord(logic_player* player);
	void GM_TEST();
	void TEST_GetPlayHandType(std::vector<int>& sort_cards);
private:
	int rand(int min, int max);
	int take_one_card();
	cardType GetPlayHandType(std::vector<int>& cards);
	int getPoint(cardType type, std::vector<int>& cards);
	bool IS_SHUANGFEI(std::vector<int>& cards);	//���
	bool IS_SIZHANGPAI(std::vector<int>& cards);	//ը��
	bool IS_SANPAI(std::vector<int>& cards);	//������
	bool IS_ERPAI(std::vector<int>& cards);	//������
	bool IS_DANPAI(std::vector<int>& cards);//������
	bool IS_SIDAIYI(std::vector<int>& cards);	
	bool IS_SIDAIYIDUI(std::vector<int>& cards);	
	bool IS_SIDAIER(std::vector<int>& cards);	
	bool IS_SIDAIERDUI(std::vector<int>& cards);
	bool IS_SANDAIYI(std::vector<int>& cards);
	bool IS_SANDAIYIDUI(std::vector<int>& cards);
	bool IS_SIZHANGPAISHUNZI(std::vector<int>& cards);
	bool IS_SIDAIYISHUNZI(std::vector<int>& cards);
	bool IS_SIDAIYIDUISHUNZI(std::vector<int>& cards);
	bool IS_SIDAIERSHUNZI(std::vector<int>& cards);
	bool IS_SIDAIERDUISHUNZI(std::vector<int>& cards);
	bool IS_SANZHANGPAISHUNZI(std::vector<int>& cards);
	bool IS_SANDAIYISHUNZI(std::vector<int>& cards);
	bool IS_SANDAIYIDUISHUNZI(std::vector<int>& cards);
	bool IS_ERPAISHUN(std::vector<int>& cards);	//����˳
	bool IS_DANPAISHUN(std::vector<int>& cards);//����˳

	int FEN_SHUANGFEI(std::vector<int>& cards);	//˫��
	int FEN_SIPAI(std::vector<int>& cards);	//����
	int FEN_SANPAI(std::vector<int>& cards);	//���� 
	int FEN_ERPAI(std::vector<int>& cards);//����
	int FEN_DANPAI(std::vector<int>& cards);	//����˳
private:
	std::vector<int> m_cards;
	int deskCount;//��������м������ڴ�
	int mOpenCard;//ȡ��ǰ������ȷ��˭��ȡ��
	std::map<int, std::vector<int>> m_cardMap;
	std::vector<int> m_RemainlandlordCards;//ʣ�µ�3����
	int landlord_id;

	const LandlordNetHuman_RoomCFGData* m_cfg;
	logic_playhandAI* m_playhandAI;
	logic_robLandlordAI* m_robLandlordAI;
};
