#include "stdafx.h"

#include <enable_random.h>
#include <net\packet_manager.h>
#include <time_helper.h>

#include "game_db.h"

#include "game_db_log.h"
#include "game_engine.h"
#include "logic_lobby.h"
#include "logic_player.h"
#include "logic_room.h"
#include "logic_table.h"

#include "msg_type_def.pb.h"
#include "pump_type.pb.h"
#include "ShowHand_RoomCFG.h"

using namespace boost;
using namespace game_showhand_protocols;
SHOWHAND_SPACE_USING

const static int g_one_minute = 60;//60 秒

logic_player::logic_player(void):
        m_logic_gold(0),
        m_change_gold(0),
        m_check_sync(0),
        m_check_ticket(0),
        m_cur_tickettime(0)
		,m_room(nullptr)
		,m_table(nullptr)
		,entertable_cd(0.0)
		,prepare_cd(0.0)
		,bet_cd(0.0)
		,bet_max_cd(0.0)
		,or_once_bet(false)
		,clearrubbish_cd(0.0)
		,viewcard_cd(0.0)
		,viewcard_max_cd(0.0)
		,m_kich_rubbish_cd(0.0)
		,rob_continuePlayCount(0)
		,rob_continuePlayCount_max(0)
		,quit_table_rob_cd_max(0.0)
		,robot_auto_quittable_cd(0.0)
		,last_select_table_id(0)
{
	m_player_table_state=player_table_state::e_table_state_none;
	logic_player_db::init_game_object();
}

logic_player::~logic_player(void) 
{

}

bool logic_player::is_offline() 
{
    return m_player->get_state() != e_ps_playing;
}

void logic_player::start_offline_timer() 
{

}

void logic_player::on_change_state(void) 
{

}

void logic_player::on_attribute_change(int atype, int v) {
	if(atype == msg_type_def::e_itd_gold)
		m_logic_gold += v;
}

void logic_player::on_attribute64_change(int atype, GOLD_TYPE v) {
    if (msg_type_def::e_itd_gold == atype)
        m_logic_gold += v;
}

void logic_player::heartbeat( double elapsed ) 
{
	m_check_sync += elapsed;

	if (m_check_sync > g_one_minute) {//1分钟同步1次
		//synchronization_gold();
		m_check_sync = 0;
	}

	if(m_room!=nullptr && m_table==nullptr)//踢出桌子
	{
		if(get_gold()<m_room->get_EnterTableGold())//小于金币踢人
		{
			m_kich_rubbish_cd+=elapsed;
			if(m_kich_rubbish_cd>10)
			{
				if(is_android())
				{
					//std::cout<<"机器人金币不足, 踢出桌子"<<std::endl;
					game_engine::instance().release_robot(this->get_pid());//释放一个机器人
				}
				m_kich_rubbish_cd=0.0;
			}
		}else
		{
			m_kich_rubbish_cd=0.0;
		}
	}else
	{
		m_kich_rubbish_cd=0.0;
	}

	if(m_table!=nullptr && m_table->get_player_count()==1 && is_android())//如果是机器人一个人待在房间里30秒，自动退出房间
	{
		robot_auto_quittable_cd+=elapsed;
		if(quit_table_rob_cd_max==0.0)
		{
			quit_table_rob_cd_max=30.0;
		}
		if(robot_auto_quittable_cd>quit_table_rob_cd_max)
		{
			//std::cout<<"桌子里只有一个机器人, 机器人离开"<<std::endl;
			leave_table();
			robot_auto_quittable_cd=0.0;
			quit_table_rob_cd_max=global_random::instance().rand_int(10,30);
		}
	}else
	{
		robot_auto_quittable_cd=0.0;
	}

	robot_heartbeat(elapsed);
}

uint32_t logic_player::get_pid() {
	return m_player->get_playerid();
}

int logic_player::get_lucky()
{
	return m_player->get_attribute(msg_type_def::e_itd_lucky);
}

bool logic_player::change_lucky(int addgold)
{
	if(addgold<=0)
	{
		return false;
	}
	if(!Is_Luckly_Valid(addgold))
	{
		TempIncome += addgold;
		TotalIncome +=addgold;
	}
	static const ShowHand_RoomStockCFGData* m_StockCFG=ShowHand_RoomStockCFG::GetSingleton()->GetData(m_room->get_room_id());
	int v=addgold/m_StockCFG->mLuckyGoldRate;

	if(get_lucky()==0)
	{
		return false;
	}

	int before_luckly=get_lucky();
	Luck_add_value=0;
	if(get_lucky()>0)
	{
		Luck_add_value=-v;
	}else if(get_lucky()<0)
	{
		Luck_add_value=v;
	}

	int Lucky=0;
	if(abs(get_lucky())>=abs(Luck_add_value))
	{
		Lucky=get_lucky()+Luck_add_value;
	}else
	{
		TempIncome =0;
		Lucky=0;
	}
	bool result=m_player->change_lucky(Lucky, TempIncome, TotalIncome);
	if(result==true)
	{
		TempIncome += addgold;
		TotalIncome += addgold;
		db_log::instance().pumpPlayerLucky(this,before_luckly);
		do_protobuf_notice_Luck_Info();
		Luck_add_value=0;
	}
	return result;
}

bool logic_player::Is_Luckly_Valid(int addgold)
{
	bool luckyValid = true;
	int64_t lucky_value = get_lucky();
	if (lucky_value != 0)
	{
		static const ShowHand_RoomStockCFGData* m_StockCFG=ShowHand_RoomStockCFG::GetSingleton()->GetData(m_room->get_room_id());
		int64_t fish_score = addgold;			
		int64_t temp = lucky_value * m_StockCFG->mLuckyGoldRate;//总共的幸运值金币数
		if((temp + m_StockCFG->mLuckyEx) <= (TempIncome+fish_score))
		{
			luckyValid = false;
		}
	}else
	{
		luckyValid=false;
	}
	return luckyValid;
}
bool logic_player::is_GM_CONTROL()
{
	return m_player->get_attribute(msg_type_def::e_itd_privilege)> 0;
}

GOLD_TYPE logic_player::get_gold() {
	return m_logic_gold;
}

int logic_player::get_ticket() {
	return m_player->get_attribute(msg_type_def::e_itd_ticket);
}

int16_t logic_player::get_viplvl() {
	return m_player->get_attribute(msg_type_def::e_itd_vip);
}

bool logic_player::is_ExperienceVIP()
{
	return m_player->is_ExperienceVIP();
}

bool logic_player::change_gold(GOLD_TYPE v, bool needbc) {
	if (-v <= m_logic_gold) {
		if (m_logic_gold > MAX_MONEY - v)
			v = MAX_MONEY - m_logic_gold;

		m_change_gold += v;
		m_logic_gold += v;

        if (needbc)
            return synchronization_gold();

		return true;
	}

    SLOG_ERROR
        << "error gold value[" 
        << v
        << "], more than player logic_gold["
        << m_logic_gold
        << "], player id["
        << m_player->get_playerid()
        << "]";

	return false;
}

bool logic_player::synchronization_gold() {
    if (! m_change_gold)
        return false;

    bool res = m_player->change_gold(m_change_gold);

    db_log::instance().property_log(this,
        game_engine::instance().get_gameid(),
        msg_type_def::e_item_type_def::e_itd_gold,
        m_change_gold,
        PropertyReasonType::type_showhand_synchronization
    );

    m_change_gold = 0;

    return res;
}

bool logic_player::change_ticket(int v, bool needbc) 
{
	return true;
}

const std::string& logic_player::get_nickname() {
	return m_player->get_nickname();
}

int logic_player::get_sex() {
	return m_player->get_attribute(msg_type_def::e_itd_sex);
}

int logic_player::get_photo_frame() {
	return m_player->get_attribute(msg_type_def::e_itd_photoframe);
}

const std::string& logic_player::get_icon_custom() {
	return m_player->get_icon_custom();
}

void logic_player::release() {
  
	leave_room();
	m_player.reset();
}
//------------------------------------------------------------------------


int logic_player::get_player_type() {
    return 0;//这里需要修改
}

bool logic_player::is_inRoom()
{
	return m_room != nullptr;
}
bool logic_player::is_inTable()
{
	return m_table != nullptr;
}
logic_room* logic_player::get_room()
{
	return m_room;
}
logic_table* logic_player::get_table()
{
	return m_table;
}


bool logic_player::is_android() {
    return m_player->is_robot();
}

//------------------------------------------------------------------------

int logic_player::get_player_world_state() 
{
	if(m_table != nullptr)
		return 3;
	else if(m_room != nullptr)
		return 2;
	return 1;
}



int logic_player:: get_player_table_state()
{
	return m_player_table_state;
}

void logic_player::set_player_table_state(player_table_state state)
{
	m_player_table_state=state;
}

GOLD_TYPE logic_player::getbetCount()
{
	if(m_table!=nullptr)
	{
		return m_table->get_betcount(get_pid());
	}else
	{
		SLOG_CRITICAL<<"bet count is null"<<std::endl;
		return 0;
	}
}

/*msg_bet_info logic_player::getlastbetCount()
{
	return bet_list[bet_list.size()-1].bet_count();
}*/

//-------------------------逻辑------------------------------------------------
void logic_player::enter_game(logic_lobby* lobby)
{
	m_logic_gold = m_player->get_attribute64(msg_type_def::e_itd_gold);
	TempIncome = m_player->get_attribute64(msg_type_def::e_itd_tempincome);
	//std::cout<<"幸运值暂时收入："<<TempIncome<<std::endl;
	TotalIncome = m_player->get_attribute64(msg_type_def::e_itd_totalincome);
	//std::cout<<"幸运值总收入："<<TempIncome<<std::endl;
	m_db_player_id->set_value(get_pid());//设置数据库的iD
	load_player();
}


int logic_player::enter_room(int roomid) 
{
	auto room_it = game_engine::instance().get_lobby().get_room(roomid);
	auto player_it=game_engine::instance().get_lobby().get_player(get_pid());
	if (room_it==nullptr)
        return msg_type_def::e_rmt_room_notopen;

	int res =room_it->enter_room(player_it);//玩家进入房间
    if ((msg_type_def::e_msg_result_def)res == msg_type_def::e_rmt_success) 
	{
		m_room = room_it.get();
		m_room->do_protobuf_notice_Stock_Info(this);
		do_protobuf_notice_Luck_Info();
    }
    return res;
}

int logic_player::leave_room() 
{	
	if (m_room == nullptr)
		return msg_type_def::e_rmt_fail;

	leave_table();

	int res=m_room->leave_room(this);
	if(res==msg_type_def::e_rmt_success)
	{
		m_room = nullptr;
	}
	return res;
}

int logic_player::enter_table(int table_id) 
{
	if(m_room==nullptr)
	{
		return msg_type_def::e_rmt_fail;
	}
	if(table_id<=0 || m_table!=nullptr)
	{
		return msg_type_def::e_rmt_fail;
	}

	auto table_it=m_room->get_table(table_id);

	if(table_it!=nullptr)
	{
		int res=table_it->enter_table(this);
		if(res==msg_type_def::e_rmt_success)
		{
			m_table = table_it.get();
			set_player_table_state(player_table_state::e_table_state_no_prepare);
			m_room->do_protobuf_notice_enter_table(this);
			m_table->do_protobuf_notice_enter_table(this);
		}
		return res;
	}else
	{
		SLOG_CRITICAL <<"桌子ID非法:"<<table_id<<std::endl;
		return msg_type_def::e_rmt_fail;
	}
}

int logic_player::leave_table() 
{
	if(m_table==nullptr)
	{
		return msg_type_def::e_rmt_fail;
	}

	int res= m_table->quit_table(this);

	if(res==msg_type_def::e_rmt_success)
	{	
		set_player_table_state(player_table_state::e_table_state_none);
		m_table->do_protobuf_notice_leave_table(get_pid());
		m_room->do_protobuf_notice_leave_table(m_table->get_id(),get_pid());
		m_table = nullptr;
	}
	store_game_object();
	synchronization_gold();//同步金币
	return res;
}

int logic_player::prepare_game() 
{
	msg_type_def::e_msg_result_def result=msg_type_def::e_rmt_success;
	if(get_gold()<m_room->get_EnterTableGold())//小于满足桌子条件金币数量
	{
		result=msg_type_def::e_rmt_gold_not_enough; 
	}
	if(m_table == nullptr || m_player_table_state==player_table_state::e_table_state_prepare)
	{
		SLOG_CRITICAL<<"玩家准备错误： 玩家状态："<<m_player_table_state<<std::endl;
		result=msg_type_def::e_msg_result_def::e_rmt_fail; 
	}
	if(result==msg_type_def::e_rmt_success)
	{
		m_player_table_state=player_table_state::e_table_state_prepare;
	}
	/*if(m_table!=nullptr && m_table->get_player_left()!=nullptr)
	{
		std::cout<<(get_pid()==m_table->get_player_left()->get_pid()?"左边玩家准备":"右边玩家准备")<<":"<<(result==1?"成功":"失败")<<std::endl;
	}else
	{
		std::cout<<"右边玩家准备:"<<":"<<(result==1?"成功":"失败")<<std::endl;
	}*/
	auto sendmsg = PACKET_CREATE(packetl2c_player_ready_result, e_mst_l2c_player_ready_result);
	sendmsg->set_result((msg_type_def::e_msg_result_def)result);
	if(result==msg_type_def::e_rmt_success)
	{
		sendmsg->set_player_id(get_pid());

		if(get_table()!=nullptr && get_table()->get_player_left()!=nullptr)
		{
			get_table()->get_player_left()->send_msg_to_client(sendmsg);
		}
		if(get_table()!=nullptr && get_table()->get_player_right()!=nullptr)
		{
			get_table()->get_player_right()->send_msg_to_client(sendmsg);
		}

		m_room->do_protobuf_notice_table_player_state(m_table);
	}else
	{
		send_msg_to_client(sendmsg);
	}

	return 0;
}
int logic_player::set_min_bet(GOLD_TYPE count)
{
	change_gold(-count);
	//std::cout<<"本次底注："<<count<<std::endl;
	return 0;
}

int logic_player::add_bet(const game_showhand_protocols::msg_bet_info& betinfo) 
{
	if(betinfo.bet_count()>m_logic_gold)
	{
		//std::cout<<"下注金币不足"<<std::endl;
		return msg_type_def::e_rmt_gold_not_enough;
	}
	GOLD_TYPE m_once_bet_cout=0;
	m_once_bet_cout=m_table->get_bet_cout(get_pid(),betinfo);
	if(m_once_bet_cout>m_logic_gold)
	{
		//std::cout<<"下注金币不足"<<std::endl;
		return msg_type_def::e_rmt_gold_not_enough;
	}
	change_gold(-m_once_bet_cout);

	m_table->do_protobuf_notice_bet(get_pid(),betinfo);//通知下注
	return msg_type_def::e_rmt_success;
}

void logic_player::seecard()
{
	if(m_table != nullptr)
		m_table->do_protobuf_notice_view_card(get_pid());
}


void logic_player::copy_player_info(game_showhand_protocols::msg_player_info* player_info)
{
	player_info->set_player_id(get_pid());
	player_info->set_player_nickname(get_nickname());
	player_info->set_player_head_frame(get_photo_frame());
	player_info->set_player_head_custom(get_icon_custom());
	player_info->set_player_gold(get_gold());
	player_info->set_player_vip_lv(get_viplvl());
	player_info->set_state(get_player_table_state());

	if(m_table!=nullptr)
	{
		if(m_table->get_player_left()!=nullptr && m_table->get_player_left()->get_pid()==get_pid())
		{
			player_info->set_is_left(true);
		}else if(m_table->get_player_right()!=nullptr && m_table->get_player_right()->get_pid()==get_pid())
		{
			player_info->set_is_left(false);
		}else
		{
			assert(false);
		}
	}
}

void logic_player::do_protobuf_notice_Luck_Info()
{
	if(!is_android() && is_GM_CONTROL())
	{
		auto sendmsg = PACKET_CREATE(packetl2c_notice_gm_luck_info,e_mst_l2c_notice_gm_luck_info);
		sendmsg->set_total_luck(get_lucky());
		sendmsg->set_add_luck(Luck_add_value);
		this->send_msg_to_client(sendmsg);
	}
}



//--------------------------------------数据库-----------------------------------------------------------
bool logic_player_db::load_player()
{
	mongo::BSONObj b = db_game::instance().findone(DB_SHOWHAND_PLAYER, BSON("player_id" << m_db_player_id->get_value()));
	if(b.isEmpty())
		return false;

	return from_bson(b);
}

void logic_player_db::init_game_object()
{	
	m_db_player_id = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "player_id"));
	m_db_once_win_maxgold = CONVERT_POINT(Tfield<GOLD_TYPE>, regedit_tfield(e_got_int64, "OnceWinMaxGold"));
	m_db_win_count = CONVERT_POINT(Tfield<int32_t>, regedit_tfield(e_got_int32, "win_count"));
}

bool logic_player_db::store_game_object(bool to_all)
{
	if(!has_update())
		return true;
	
	auto err = db_game::instance().update(DB_SHOWHAND_PLAYER, BSON("player_id" << m_db_player_id->get_value()), BSON("$set"<<to_bson(to_all)));
	if(!err.empty())
	{
		SLOG_ERROR << "logic_player::store_game_object :" <<err;
		return false;
	}
	return true;
}


//--------------------------------------------------Protobuf-------------------------------------------------
//玩家下注结果返回
 void do_protobuf_player_bet(int pid)
 {

 }