#pragma once
#include "logic_def.h"
#include <i_game_phandler.h>
#include <vector>
#include "proc_game_landlord_protocol.h"
#include "i_game_player.h"
#include "msg_type_def.pb.h"

using namespace game_landlord_rank_protocol;

class logic_lobby;
class logic_player_db:public game_object
{
public:
	bool load_player();
	virtual void init_game_object() override;//注册属性
	virtual bool store_game_object(bool to_all = false) override;//非数组对象必须实现此接口
public:
	Tfield<int32_t>::TFieldPtr m_player_id;
};

class logic_player: 
	public enable_obj_pool<logic_player>
	,public i_game_phandler
	,public logic_player_db
{
public:
	logic_player(void);
	virtual ~logic_player(void);

	void heartbeat( double elapsed );

	//-----------------------------------------------------------------------
	//从服务器通知逻辑的接口
	virtual void on_attribute_change(int atype, int v) override;			//金钱改变

	virtual void on_change_state() override;					//玩家掉线

	virtual void on_attribute64_change(int atype, GOLD_TYPE v = 0) override;

	//------------------------------------------------------------------------
	bool is_robot();

	const std::string& get_nickname();
	const std::string& get_icon_custom();
	const uint32_t get_head_frame_id();
	const int16_t get_player_sex();
	uint32_t get_pid();
	uint16_t get_viplvl();
	GOLD_TYPE get_gold();
	int get_ticket();
	bool change_gold(GOLD_TYPE v);
	bool change_ticket(int count,int reason = -1);

	int get_deskId() { return deskId; }
	void set_deskId(int id) { deskId = id; }
	void set_table(logic_table* table) { m_table = table; }
	logic_table* get_table() { return m_table; }

	void enter_game(logic_lobby* lobby);		//进入游戏

	e_server_error_code enter_room(logic_room* room);			//进入房间
	e_server_error_code leave_room();
	e_server_error_code enter_table();
	e_server_error_code leave_table();							//离开桌子
	e_server_error_code start_match();
	int get_wait_time();

	e_server_error_code robLandlord(int);// 抢地主
	e_server_error_code playhand(const game_landlord_rank_protocol::card_Info& cards);// 出牌
	e_server_error_code SetOrHang(bool orHang);//托管
	void do_protobuf_send_SceneInfo();

	logic_room* get_room(){return m_room;}
	bool is_inTable() { return m_table != nullptr; }
	bool is_inRoom() { return m_room != nullptr; }
	e_player_state get_game_state();
	e_player_game_state get_player_game_state() { return player_state; }
	void set_player_game_state(e_player_game_state state);
	void release();								//退出整个游戏客户端

	void sycn_gold();
	void onAcceptGift(int receiverId, int giftId);

	void add_star_lottery_info(int32_t award,int32_t star = 0);

	void quest_change(int quest_type,int count,int param = 0);
public:
	void bc_game_msg(int money, const std::string& sinfo, int mtype = 1);

	template<class T>
	int send_msg_to_client(T msg)
	{
		return m_player->send_msg_to_client(msg);
	};
private:
	logic_lobby* m_lobby;
	logic_room* m_room;
	logic_table* m_table;
	int deskId;

	double m_checksave;
	bool is_first_save;

	bool orHang;//是否托管

	GOLD_TYPE m_logic_gold;
	GOLD_TYPE m_change_gold;

	e_player_game_state player_state;
	e_player_state m_player_online_state;

	double rob_match_cd;
	double rob_match_max_cd;
	double rob_robLandlord_cd;
	double rob_robLandlord_max_cd;
	double rob_playhand_cd;
	double rob_playhand_max_cd;
};