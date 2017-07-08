#pragma once
#include "logic_def.h"
#include "logic_core.h"
#include "proc_game_landlord_protocol.h"
#include "LandlordNetHuman_RoomCFG.h"

class logic_table_db :public game_object
{
protected:
	void create_table();
	bool load_table();
public:
	virtual void init_game_object() override;//ע������
	virtual bool store_game_object(bool to_all = false) override;//������������ʵ�ִ˽ӿ�
	Tfield<int16_t>::TFieldPtr		m_db_table_id;			//����id
};

class logic_table :public logic_table_db
{
public:
	logic_table(int);
	~logic_table(void);

	void heartbeat(double elapsed);

	e_server_error_code rob_enter_table(LPlayerPtr player, int deskId);
	e_server_error_code enter_table(LPlayerPtr player);		//���뷿��
	e_server_error_code leave_table(uint32_t playerid);			//�뿪����

	bool orInGame() { return gameState >= e_game_state::e_game_state_startgame; }
	int Find_DeskPos();
	e_game_state get_table_State();
	bool get_orFull();
	bool check_ExistRealPlayer();
	int getTablePlayerCount() { return playerMap.size(); }
	int get_realLandlord_Id() { return realLandlord_Id; }
	std::vector<int>& get_lastplayhand() { return lastPlayhand; }
	int get_lastplayhand_Id() { return lastPlayhand_Id; }
	bool orMustPlayHand(logic_player* player);//�Ƿ�������
	game_landlord_net_human_protocol::card_Info get_rob_playhand(logic_player* player) { return m_cardManager->get_rob_playhand(player); }
	bool judge_rob_orLandlord(logic_player* player) { return m_cardManager->judge_rob_orRobLandlord(player); }

	e_server_error_code playhand(logic_player* player, const game_landlord_net_human_protocol::card_Info& cards);
	e_server_error_code rob_Landlord(logic_player* player,int);

	void do_protobuf_notice_playhand(logic_player* player, const game_landlord_net_human_protocol::card_Info& cards);//֪ͨ ����
	void get_Scene_Info(logic_player* player, game_landlord_net_human_protocol::packetl2c_get_room_scene_info_result* sendmsg);//��ȡ������Ϣ
	void do_protobuf_notice_or_prepare(int pid, bool orPrepare);

	e_server_error_code Set_Room_Info(const std::string& roomName, const std::string& password,const bool orShuffle);
	e_server_error_code kich_player(int deskId);
	e_server_error_code add_rob_player(int deskId);

	std::map<int32_t, LPlayerPtr>& get_playerMap() { return playerMap; }
public:
	int32_t get_table_id() { return m_tableId; }	//����ID
	void set_roomName(std::string name) { roomName = name; }
	std::string get_roomName() { return roomName; }
	void set_roomPassword(std::string name) { roomPassword = name; }
	std::string get_roomPassword() { return roomPassword; }
	bool verifyPassword(std::string password) { return password == roomPassword; }
	bool orNeedPassword() { return roomPassword!="" && roomPassword.c_str()!=nullptr; }
	void set_roomOrShuffle(bool orShuffle) { this->orShuffle = orShuffle; }
	bool get_roomOrShuffle() { return orShuffle; }

	int32_t get_room_creator() { return m_creator; }
private:
	void do_protobuf_notice_start_game();//֪ͨ��ʼ��Ϸ
	void do_protobuf_notice_who_playhand();
	void do_protobuf_notice_robLandlord();//֪ͨ ĳĳ������
	void do_protobuf_notice_robLandlord_result(logic_player* player, int orRob);//֪ͨ�������������Ľ��
	void do_protobuf_notice_winlose(int deskId);//֪ͨ ����
	void do_protobuf_notice_room_enter_leave_info(logic_player* player, int action);
	void do_protobuf_notice_room_creator();
public:
	template<class T>
	int broadcast_msg_to_client(T msg, uint32_t except_id = 0)
	{
		std::vector<uint32_t> pids;
		for (auto it=playerMap.begin(); it != playerMap.end(); it++)
		{
			if (it->second->is_inTable() && it->second->get_pid() != except_id && !it->second->is_robot())
			{
				pids.push_back(it->second->get_pid());
			}
		}
		return game_engine::instance().get_handler()->broadcast_msg_to_client(pids, msg->packet_id(), msg);
	}
private:
	std::map<int32_t, LPlayerPtr> playerMap;
	std::vector<int> deskList;
	e_game_state gameState;
	logic_core* m_cardManager;
	int deskCount;

	double playhand_cdTime;
	double sendcard_cdTime;
	double roblandlord_cdTime;
	double award_cdTime;

	int32_t current_robLandlord_Id;//��ǰ ������ID
	int32_t current_playhand_Id;//��ǰ ����ID
	int32_t realLandlord_Id;

	std::vector<int> lastPlayhand;//һ���غϳ���
	int32_t lastPlayhand_Id;

	int32_t win_deskId;

	//-------------------------�����������---------------------------------------
	int32_t m_tableId;//����Id
	std::string roomName;//������
	std::string roomPassword;//������Կ
	bool orShuffle;//�Ƿ�ϴ��
	int32_t m_score;
	int32_t m_creator;
};