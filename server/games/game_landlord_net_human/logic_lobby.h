#pragma once
#include "logic_table.h"
#include "i_game_ehandler.h"

class logic_lobby
{
public:
	logic_lobby(void);
	~logic_lobby(void);

	void init_game();					//��Ϸ��ʼ��	//��Ϸ������������
	void release_game();							//ж����Ϸ	//�����������
	void heartbeat( double elapsed );				//��Ϸѭ��

	bool player_enter_game(iGPlayerPtr igplayer);	//��ҽ�����Ϸ
	void player_leave_game(uint32_t playerid);		//����뿪��Ϸ
	int player_join_friend_game(iGPlayerPtr igplayer, uint32_t friendid);	//������ѷ���

	//�����˽��뷿��
	int rob_enter_room(uint32_t pid, uint16_t rid,int32_t deskId);		//����
	void rob_leave_room(uint32_t pid);					//�뿪����
	void response_robot(int32_t playerid, int tag);

	const LTABLE_MAP& get_rooms() const;				//�õ����з�����Ϣ
	LPlayerPtr& get_player(uint32_t pid);			//���������Ϣ
	logic_table* get_room(uint32_t rid);

	int Find_RoomId();//�ҵ�һ�����õķ���ID
	logic_table* AutoMatchRoom();
	e_server_error_code CreateRoom(LPlayerPtr player, packetc2l_create_room* roomInfo);
	void get_all_invite_player_list(int pid, packetl2c_join_player_list_result* sendmsg);
	void get_all_room_list(packetl2c_roomlist_result* sendmsg);

	void do_protobuf_notice_room_change_info(logic_table* table, int action);

private:
	std::vector<int> broadcast_msg_to_client(uint32_t except_id = 0);
private:
	void init_config();				//��������
	void init_protocol();			//����Э��
	void init_room();			//��ʼ������
private:
	bool m_init;					//�Ƿ��ѳ�ʼ��
	int m_max_player;				//�����������

	LTABLE_MAP roomMap;				//���з���
	LPLAYER_MAP playerMap;		//�������
	std::vector<bool> roomIdList;
};
