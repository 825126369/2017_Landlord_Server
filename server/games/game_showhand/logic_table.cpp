#include "stdafx.h"

#include "logic_table.h"
#include <enable_random.h>
#include <net\packet_manager.h>

#include "game_db.h"
#include "game_engine.h"
#include "i_game_ehandler.h"
#include "logic_lobby.h"
#include "logic_player.h"
#include "logic_room.h"
#include "cardmanager.h"

#include "game_showhand_def.pb.h"
#include "game_showhand_protocol.pb.h"

SHOWHAND_SPACE_USING
using namespace game_showhand_protocols;

logic_table::logic_table(logic_room* room,int tid)
	:m_player_left(nullptr)
	,m_player_right(nullptr)
	,award_time(0.0)
	,takecard_time(0.0)
	,left_kichRubbish_cd(0.0)
	,right_kichRubbish_cd(0.0)
	,left_bet_cout(0)
	,right_bet_cout(0)
	,left_escape_pid(0)
	,right_escape_pid(0)
	,m_rob_cd(0)
{
	m_room=room;
	m_table_Id=tid;
	m_checksave=0.0;
	m_bet_time=0.0;

	Is_left_abandonCard=false;
	Is_right_abandonCard=false;
	min_bet_count=0;
	max_bet_count=0;
	
	once_bet_list.clear();
	Is_left_ShowHands=false;
	Is_right_ShowHands=false;

	left_bet_state=nullptr;
	right_bet_state=nullptr;
	m_game_state=game_showhand_protocols::e_game_state::e_state_game_none;
	m_cardmanager=std::make_shared<cardmanager>();

	currentCardIndex=0;
}

logic_table::~logic_table(void) 
{
	if(left_bet_state!=nullptr)//清楚历史下注状态信息
	{
		delete left_bet_state;
		left_bet_state=nullptr;
	}
	if(right_bet_state!=nullptr)
	{
		delete right_bet_state;
		right_bet_state=nullptr;
	}
}

void logic_table::set_GM_CONTROL_COMMAND(int command)
{
	GM_COMMAND=command;
}

uint32_t logic_table::get_id() 
{
    return m_table_Id;
}

void logic_table::heartbeat(double elapsed) 
{
	if(m_player_left!=nullptr || m_player_right!=nullptr)
	{
		if(m_game_state==e_game_state::e_state_game_none)
		{
			m_game_state=e_game_state::e_state_game_prepare;
		}
	}else
	{
		m_game_state=e_game_state::e_state_game_none;
		return;
	}
	
	if(m_game_state == game_showhand_protocols::e_game_state::e_state_game_prepare)//桌子有人
	{
		if(m_player_left!=nullptr && m_player_left->get_player_table_state()==player_table_state::e_table_state_prepare &&
			m_player_right!=nullptr && m_player_right->get_player_table_state()==player_table_state::e_table_state_prepare)
		{
			m_game_state=e_game_state::e_state_game_start;
			left_kichRubbish_cd=0.0;
			right_kichRubbish_cd=0.0;
		}else
		{
			if(m_player_left!=nullptr)
			{
				if(m_player_left->get_player_table_state()==player_table_state::e_table_state_no_prepare)
				{
					left_kichRubbish_cd+=elapsed;
					if(left_kichRubbish_cd>20.0)
					{
						kich_table_rubbish(m_player_left);
						left_kichRubbish_cd=0.0;
					}
				}else
				{
					left_kichRubbish_cd=0.0;
				}
			}else
			{
				left_kichRubbish_cd=0.0;
			}
			if(m_player_right!=nullptr)
			{
				if(m_player_right->get_player_table_state()==player_table_state::e_table_state_no_prepare)
				{
					right_kichRubbish_cd+=elapsed;
					if(right_kichRubbish_cd>20.0)
					{
						kich_table_rubbish(m_player_right);
						right_kichRubbish_cd=0.0;
					}
				}else
				{
					right_kichRubbish_cd=0.0;
				}
			}else
			{
				right_kichRubbish_cd=0.0;
			}
		}
	}else if(m_game_state==game_showhand_protocols::e_game_state::e_state_game_start)//开始游戏
	{
		start_game_prepare();
		do_protobuf_notice_start_game();//通知开始游戏
		m_game_state=e_game_state::e_state_game_bet;
	}else if(m_game_state==game_showhand_protocols::e_game_state::e_state_game_bet)//开始下注
	{
		m_bet_time+=elapsed;
		if(m_bet_time>21.0)//判断是否超时弃牌
		{
			if(left_bet_state!=nullptr && left_bet_state->state()==1)
			{
				Is_left_abandonCard=true;
			}else if(right_bet_state!=nullptr && right_bet_state->state()==1)
			{
				Is_right_abandonCard=true;
			}else
			{
				assert(false);
			}
			SLOG_CRITICAL<<"超时弃牌:"<<m_bet_time<<std::endl;
			m_game_state=e_game_state::e_state_game_award;
			m_bet_time=0.0;
		}
	}else if(m_game_state==game_showhand_protocols::e_game_state::e_state_game_takecard)//发牌
	{
		if(takecard_time>0.8 && currentCardIndex<4)//隔0.5秒发牌一次
		{
			do_protobuf_notice_sendcard();
			if(Is_left_ShowHands==false || Is_right_ShowHands==false)//如果不是全压，每次只发一次牌
			{
				m_game_state=e_game_state::e_state_game_bet;
			}else//隔5秒发一次牌
			{
				if(currentCardIndex>=4)//五张牌发完就结算奖励
				{
					m_game_state=game_showhand_protocols::e_game_state::e_state_game_award;
				}
			}
			takecard_time=0.0;
		}else
		{
			takecard_time+=elapsed;
		}
	}else if(m_game_state==game_showhand_protocols::e_game_state::e_state_game_award)//发放奖励
	{
		if(award_time>1.0 && m_player_left!=nullptr && m_player_right!=nullptr)
		{
			do_protobuf_notice_award();
			m_game_state = game_showhand_protocols::e_game_state::e_state_game_none;
			award_time=0.0;
		}else
		{
			award_time+=elapsed;
		}
	}

	robot_heart(elapsed);
}

void logic_table::initTablePlayer(bool left)
{
	if(left)
	{
		if(left_bet_state!=nullptr)
		{
			delete left_bet_state;
			left_bet_state=nullptr;
		}
		left_kichRubbish_cd=0.0;
		left_escape_pid=0;
		m_player_left->set_player_table_state(player_table_state::e_table_state_no_prepare);	
	}else
	{
		if(right_bet_state!=nullptr)
		{
			delete right_bet_state;
			right_bet_state=nullptr;
		}
		right_kichRubbish_cd=0.0;
		right_escape_pid=0;
		m_player_right->set_player_table_state(player_table_state::e_table_state_no_prepare);
	}
}

int logic_table::enter_table(logic_player* player) 
{
	if(m_room->get_EnterTableGold()>player->get_gold())//小于进入桌子最小金币数
	{
		return msg_type_def::e_rmt_gold_not_enough;
	}
	if(m_player_left==nullptr)
	{
		m_player_left=player;
		initTablePlayer(true);
	}else if(m_player_right==nullptr)
	{
		m_player_right=player;
		initTablePlayer(false);
	}else
	{
		return msg_type_def::e_rmt_fail;
	}
    return msg_type_def::e_rmt_success;
}

int logic_table::quit_table(logic_player* player) 
{
	if (m_player_left!= nullptr && m_player_left->get_pid() == player->get_pid())
	{
		if(m_game_state>1)
		{
			if(m_game_state!=e_game_state::e_state_game_award)//进行游戏的时候逃跑，直接开奖
			{
				left_escape_pid=player->get_pid();
			}
			ImmediatelyOpenAward();
		}
		m_player_left = nullptr;
	}else if(m_player_right!= nullptr && m_player_right->get_pid() == player->get_pid())
	{
		if(m_game_state>1)
		{
			if(m_game_state!=e_game_state::e_state_game_award)//进行游戏的时候逃跑，直接开奖
			{
				right_escape_pid=player->get_pid();
			}
			ImmediatelyOpenAward();
		}
		m_player_right = nullptr;
	}else
	{
		return msg_type_def::e_msg_result_def::e_rmt_fail;
	}
	return msg_type_def::e_msg_result_def::e_rmt_success;
}

//玩家逃跑时，立即开奖
void logic_table::ImmediatelyOpenAward()
{
	award_time=0.0;
	do_protobuf_notice_award();
	m_game_state = game_showhand_protocols::e_game_state::e_state_game_none;
}

GOLD_TYPE logic_table::get_rob_income()//收入
{
	return m_rob_income;
}

GOLD_TYPE logic_table::get_rob_outcome()//支出
{
	return m_rob_outcome;
}

PCARDMANAGER logic_table::get_cardmanager()
{
	return m_cardmanager;
}


int logic_table::get_player_count() 
{
	int count=0;
	if(m_player_left!=nullptr)
	{
		count++;
	}
	if(m_player_right!=nullptr)
	{
		count++;
	}
	return count;
}

bool logic_table::get_or_have_hasuo()
{
	return	Is_left_ShowHands || Is_right_ShowHands;
}

bool logic_table::is_empty()
{
	return m_player_left==nullptr && m_player_right==nullptr;
}

logic_player* logic_table::get_player_left()
{
	return m_player_left;
}

logic_player* logic_table::get_player_right()
{
	return m_player_right;
}

p_bet_state* logic_table::get_left_betstate()//得到左边玩家下注状态
{
	return left_bet_state;
}
p_bet_state* logic_table::get_right_betstate()//得到右边玩家下注状态
{
	return right_bet_state;
}

GOLD_TYPE logic_table::get_betcount(int32_t pid)//得到左边玩家总下注
{
	if(pid==m_player_left->get_pid())
	{
		return left_bet_cout;
	}else if(pid==m_player_right->get_pid())
	{
		return right_bet_cout;
	}else
	{
		assert(false);
		return 0;
	}
}

//获取下注底分
void logic_table::set_bet_gold_count() 
{
    //获取下注底分
	if (m_player_left==nullptr || m_player_right==nullptr)
        return;

	GOLD_TYPE p_one_g = m_player_left->get_gold();
	GOLD_TYPE p_two_g = m_player_right->get_gold();

    GOLD_TYPE lUserScore = p_one_g > p_two_g ? p_two_g : p_one_g;

   LONGLONG lCellScore = 0;
	if (lUserScore < 1000)
		lCellScore = 4;
	else if (lUserScore < 10000)
		lCellScore = 32;
	else if (lUserScore < 100000)
		lCellScore = 128;
	else if (lUserScore < 400000)
		lCellScore = 1024;
	else if (lUserScore < 800000)
		lCellScore = 4096;
	else if (lUserScore < 1600000)
		lCellScore = 16384;
	else if (lUserScore < 3200000)
		lCellScore = 32768;
	else if (lUserScore < 6400000)
		lCellScore = 65536;
	else if (lUserScore < 12800000)
		lCellScore = 131072;
	else if (lUserScore < 28000000)
        lCellScore = 262144;
	else if (lUserScore < 128000000)
		lCellScore = 1000000;
	else
		lCellScore = 2000000;
	
	min_bet_count=lCellScore;

	max_bet_count=lCellScore*4;
}

GOLD_TYPE logic_table::get_min_bet_gold()
{
	return min_bet_count;
}

//得到下注的数量
GOLD_TYPE logic_table::get_bet_cout(const int32_t& pid,const msg_bet_info& betinfo)
{
	GOLD_TYPE m_once_gold_count=0;
	if(betinfo.type()==game_showhand_protocols::e_bet_type::e_call_common_add_1)
	{
		if(once_bet_list.size()==0)
		{
			m_once_gold_count =min_bet_count*1;
		}else
		{
			m_once_gold_count =once_bet_list[once_bet_list.size()-1]+min_bet_count*1;
		}
	}
	else if(betinfo.type()==game_showhand_protocols::e_bet_type::e_call_common_add_2)
	{
		if(once_bet_list.size()==0)
		{
			m_once_gold_count =min_bet_count*2;
		}else
		{
			m_once_gold_count =once_bet_list[once_bet_list.size()-1]+min_bet_count*2;
		}
	}
	else if(betinfo.type()==game_showhand_protocols::e_bet_type::e_call_common_add_3)
	{
		if(once_bet_list.size()==0)
		{
			m_once_gold_count =min_bet_count*3;
		}else 
		{
			m_once_gold_count =once_bet_list[once_bet_list.size()-1]+min_bet_count*3;
		}
	}
	else if(betinfo.type()==game_showhand_protocols::e_bet_type::e_call_common_follow)//跟注
	{
		if(once_bet_list.size()==0)
		{
			m_once_gold_count=0;
		}else
		{
			if(!Is_left_ShowHands && !Is_right_ShowHands)
			{
				m_once_gold_count=once_bet_list[once_bet_list.size()-1];
			}else
			{
				if(m_player_left->get_pid()==pid)
				{
					m_once_gold_count=m_player_left->get_gold();
				}else
				{
					m_once_gold_count=m_player_right->get_gold();
				}
			}
		}
	}else if(betinfo.type()==game_showhand_protocols::e_bet_type::e_call_showhand)//全压(有多少压多少)
	{
		if(m_player_left->get_pid()==pid)
		{
			m_once_gold_count=m_player_left->get_gold();
		}else
		{
			m_once_gold_count=m_player_right->get_gold();
		}
	}

	if(m_player_left->get_pid()==pid)
	{
		left_bet_cout+=m_once_gold_count;
	}else
	{
		right_bet_cout+=m_once_gold_count;
	}
	once_bet_list.push_back(m_once_gold_count);
	/*std::cout<<"第几次下注："<<once_bet_list.size()<<std::endl;
	std::cout<<"下注类型："<<betinfo.type()<<std::endl;
	std::cout<<"下注金额："<<m_once_gold_count<<std::endl;*/
	once_betinfo_list.push_back(betinfo);//断线重连用的
	return m_once_gold_count;
}

//得到游戏状态
game_showhand_protocols::e_game_state logic_table::get_game_state()
{
	return m_game_state;
}

//设置下注状态
void logic_table::set_bet_state(p_bet_state* bet_state,logic_player* player,int32_t player_id,const msg_bet_info* betinfo)
{
	bool isleft=player->get_pid()==m_player_left->get_pid();//是否是左边玩家
	bet_state->set_player_id(player->get_pid());
	if(betinfo==nullptr && player_id==0)
	{
		if(m_game_state==e_game_state::e_state_game_takecard || m_game_state==e_game_state::e_state_game_start)//发牌和开始的时候上一次下注信息是null的
		{
			if(Is_left_ShowHands && Is_right_ShowHands)//全压后发牌
			{
				if(currentCardIndex>=4)
				{
					bet_state->set_state(4);//发奖
				}else
				{
					bet_state->set_state(3);//等牌
				}
			}else//发牌
			{
				if(m_cardmanager->get_who_first_bet_list()[currentCardIndex-1])//是否左边边先下注
				{
					if(isleft)
					{
						bet_state->set_state(1);//将要下注
					}else
					{
						bet_state->set_state(2);//等待下注
					}
				}else
				{
					if(isleft)
					{
						bet_state->set_state(2);
					}else
					{
						bet_state->set_state(1);
					}
				}
				if(bet_state->state()==1)
				{
					bet_state->add_bet_type_list(e_bet_type::e_call_abandon);
					bet_state->add_bet_type_list(e_bet_type::e_call_common_add_1);
					bet_state->add_bet_type_list(e_bet_type::e_call_common_add_2);
					bet_state->add_bet_type_list(e_bet_type::e_call_common_add_3);
					bet_state->add_bet_type_list(e_bet_type::e_call_common_follow);
					if(currentCardIndex>=2)
					{
						bet_state->add_bet_type_list(e_bet_type::e_call_showhand);//哈索
					}
				}
			}
		}
	}else
	{
		bool isleftBet=m_player_left->get_pid()!=player_id;//左边玩家要下注，还是右边玩家要下注
		if(betinfo->type()==e_bet_type::e_call_common_add_1 || betinfo->type()==e_bet_type::e_call_common_add_2 || betinfo->type()==e_bet_type::e_call_common_add_3)
		{	
			if(isleftBet && isleft)//左边要下注,且现在要设置左边玩家的状态
			{
				bet_state->set_state(1);
			}else if(!isleftBet && !isleft)//右边玩家要下注，且是设置右边玩家状态
			{
				bet_state->set_state(1);
			}else
			{
				bet_state->set_state(2);
			}
			if(bet_state->state()==1)
			{
				bet_state->add_bet_type_list(e_bet_type::e_call_abandon);
				if(once_bet_list.size()<2)//0：第一次，1：第二次，2：第三次
				{
					bet_state->add_bet_type_list(e_bet_type::e_call_common_add_1);
					bet_state->add_bet_type_list(e_bet_type::e_call_common_add_2);
					bet_state->add_bet_type_list(e_bet_type::e_call_common_add_3);
					if(currentCardIndex>=2)//第三张牌
					{
						bet_state->add_bet_type_list(e_bet_type::e_call_showhand);//哈索
					}
				}
				bet_state->add_bet_type_list(e_bet_type::e_call_common_follow);
			}
		}else if(betinfo->type()==e_bet_type::e_call_common_follow)//第一次跟注或等待发牌
		{
			if(once_bet_list.size()==1)
			{
				if(isleftBet && isleft)//左边要下注,且现在要设置左边玩家的状态
				{
					bet_state->set_state(1);
				}else if(!isleftBet && !isleft)//右边玩家要下注，且是设置右边玩家状态
				{
					bet_state->set_state(1);
				}else
				{
					bet_state->set_state(2);
				}
				if(bet_state->state()==1)
				{
					bet_state->add_bet_type_list(e_bet_type::e_call_abandon);
					if(once_bet_list.size()<2)//0：第一次，1：第二次，2：第三次
					{
						bet_state->add_bet_type_list(e_bet_type::e_call_common_add_1);
						bet_state->add_bet_type_list(e_bet_type::e_call_common_add_2);
						bet_state->add_bet_type_list(e_bet_type::e_call_common_add_3);
						if(currentCardIndex>=2)//第三张牌
						{
							bet_state->add_bet_type_list(e_bet_type::e_call_showhand);//哈索
						}
					}
					bet_state->add_bet_type_list(e_bet_type::e_call_common_follow);
				}
			}else
			{
				bet_state ->set_state(3);
			}
		}else if(betinfo->type()==e_bet_type::e_call_showhand)//全压
		{
			if(Is_left_ShowHands && Is_right_ShowHands)
			{
				if(currentCardIndex<4)
				{
					bet_state ->set_state(3);
				}else
				{
					bet_state ->set_state(4);
				}
			}else
			{			
				if(Is_left_ShowHands==true)
				{
					if(isleft)
					{
						bet_state->set_state(2);
					}
					else
					{
						bet_state->set_state(1);
					}
				}else
				{
					if(isleft)
					{
						bet_state->set_state(1);
					}else
					{
						bet_state->set_state(2);
					}
				}
				if(bet_state->state()==1)
				{
					bet_state->add_bet_type_list(e_bet_type::e_call_abandon);//放弃
					bet_state->add_bet_type_list(e_bet_type::e_call_common_follow);//跟牌	
				}
			}
		}
		else if(betinfo->type()==e_bet_type::e_call_abandon)//弃牌
		{
			bet_state->set_state(4);
		}

		if(player->get_pid()!=player_id)
		{
			bet_state->mutable_last_betinfo()->set_type(betinfo->type());//设置下注信息
			if(once_bet_list.size()>0)
			{
				bet_state->mutable_last_betinfo()->set_bet_count(once_bet_list[once_bet_list.size()-1]);//设置下注信息
			}else
			{
				SLOG_CRITICAL<<"下注类型:全压时：下注的历史信息被清除"<<std::endl;
			}
		}
	}

	if(isleft)//设置玩家状态，缓存信息
	{
		if(left_bet_state!=nullptr)
		{
			delete left_bet_state;
			left_bet_state=nullptr;
		}
		left_bet_state=new p_bet_state();
		left_bet_state->CopyFrom(*bet_state);
	}else
	{
		if(right_bet_state!=nullptr)
		{
			delete right_bet_state;
			right_bet_state=nullptr;
		}
		right_bet_state=new p_bet_state();
		right_bet_state->CopyFrom(*bet_state);
	}

	/*if(isleft)
	{
		std::cout<<"----------------左边玩家 可以下注的状态:-------------------------"<<bet_state->state()<<std::endl;
		for(auto& var : bet_state->bet_type_list())
		{
			std::cout<<"下注类型"<<var<<std::endl;
		}
	}else
	{
		std::cout<<"-----------------右边玩家 可以下注的状态-------------------------"<<bet_state->state()<<std::endl;
		for(auto& var : bet_state->bet_type_list())
		{
			std::cout<<"下注类型"<<var<<std::endl;
		}
	}*/
}
//---------------------------------------游戏状态-----------------------------------------------------

//游戏开始准备
void logic_table::start_game_prepare()
{
	set_bet_gold_count();//设置最小下注
	left_bet_cout=min_bet_count;//总下注初始化
	right_bet_cout=min_bet_count;
	m_player_left->set_min_bet(min_bet_count);
	m_player_right->set_min_bet(min_bet_count);

	currentCardIndex=0;//发牌索引初始化为0

	m_player_left->set_player_table_state(player_table_state::e_table_state_in_game);//设置玩家在桌子中的状态
	m_player_right->set_player_table_state(player_table_state::e_table_state_in_game);

	Is_left_abandonCard=false;//弃牌初始化
	Is_right_abandonCard=false;

	Is_left_ShowHands=false;//哈所初始化
	Is_right_ShowHands=false;

	once_bet_list.clear();//依次记录所有的下注数量
	m_bet_time=0.0;
	award_time=0.0;

	if(left_bet_state!=nullptr)//清楚历史下注状态信息
	{
		delete left_bet_state;
		left_bet_state=nullptr;
	}
	if(right_bet_state!=nullptr)
	{
		delete right_bet_state;
		right_bet_state=nullptr;
	}

	m_room->do_protobuf_notice_table_player_state(this);//广播桌子内玩家状态信息

	if(m_player_left->is_android()!=m_player_right->is_android())
	{
		adjust_earn_rate();
	}else
	{
		m_cardmanager->sendcard();//提前把牌发完
	}

	do_protobuf_notice_gm_private_Info();
}

void logic_table::adjust_earn_rate()
{
	static const ShowHand_RoomStockCFGData* m_StockCFG=ShowHand_RoomStockCFG::GetSingleton()->GetData(m_room->get_room_id());
	int index_Stock=0;
	for(int i=0;i<m_StockCFG->mStock.size();i++)
	{
		if(m_room->TotalStock->get_value()>=static_cast<GOLD_TYPE>(m_StockCFG->mStock[i]))
		{
			index_Stock=i;
		}else
		{
			break;
		}
	}
	std::cout<<"当前库存档位:"<<index_Stock<<","<<m_StockCFG->mStock[index_Stock]<<std::endl;
	std::cout<<"当前库存:"<<m_room->TotalStock->get_value()<<std::endl;
	std::cout<<"当前抽水收益:"<<m_room->TotalProfit->get_value()<<std::endl;

	std::cout<<"当前左边玩家幸运值:"<<m_player_left->get_lucky()<<std::endl;
	std::cout<<"当前右边玩家幸运值:"<<m_player_right->get_lucky()<<std::endl;

	if(GM_COMMAND>0)
	{
		//m_cardmanager->gm_exchange_card();
		if(GM_COMMAND==1)
		{
			m_cardmanager->send_cheat_card(true);
		}else if(GM_COMMAND==2)
		{
			m_cardmanager->send_cheat_card(false);
		}
		GM_COMMAND=0;
	}else if(m_player_left->get_lucky()>0 || m_player_right->get_lucky()>0)
	{
		double tmpodd=50.0;
		tmpodd *= (1.0+m_StockCFG->mLuckyIncBuff);
		int rand_1=global_random::instance().rand_int(0, 100);
		if(rand_1<tmpodd)
		{
			if(m_player_left->get_lucky()>0)
			{
				m_cardmanager->send_cheat_card(true);
			}else
			{
				m_cardmanager->send_cheat_card(false);
			}
		}else
		{
			m_cardmanager->sendcard();
		}
		std::cout<<"幸运值 加成BUFF"<<std::endl;
	}else if(m_player_left->get_lucky()<0 || m_player_right->get_lucky()<0)
	{
		double tmpodd=50.0;
		tmpodd *= (1.0+m_StockCFG->mLuckyDecBuff);
		int rand_1=global_random::instance().rand_int(0, 100);
		if(rand_1<tmpodd)
		{
			if(m_player_left->get_lucky()<0)
			{
				m_cardmanager->send_cheat_card(true);
			}else
			{
				m_cardmanager->send_cheat_card(false);
			}
		}else
		{
			m_cardmanager->sendcard();
		}
		std::cout<<"幸运值 减益BUFF"<<std::endl;
	}
	else
	{
		 bool or_Cheat_FANGFEN=false;
		 bool or_Cheat_SHOUFEN=false;
          
		 if(m_room->TotalStock->get_value()<m_StockCFG->mStock[0])
		 {
			std::cout<<"突破低分"<<std::endl;
			or_Cheat_SHOUFEN=true;
		 }else
		 {
			 int rand_1=global_random::instance().rand_int(0, 100);
			 if(rand_1<= m_StockCFG->mScoreId[index_Stock])
			 {
				or_Cheat_FANGFEN=true;
			 }
		 }

		 if(or_Cheat_SHOUFEN)//收分
		 {
			if(m_player_left->is_android()!=m_player_right->is_android())
			{
				if(m_player_left->is_android())
				{
					m_cardmanager->send_cheat_card(true);
				}else
				{
					m_cardmanager->send_cheat_card(false);
				}
				std::cout<<"------------收分--------"<<std::endl;
			}
		 }else if(or_Cheat_FANGFEN)//放分
		 {
			if(m_player_left->is_android()!=m_player_right->is_android())
			{
				if(m_player_left->is_android())
				{
					m_cardmanager->send_cheat_card(false);
				}else
				{
					m_cardmanager->send_cheat_card(true);
				}
				std::cout<<"-------放分--------"<<std::endl;
			}
		 }
		 else
		 {
			 m_cardmanager->sendcard();  
			 std::cout<<"正常发牌"<<std::endl;
		 }
	}
}

//----------------------------------------------------------协议------------------------------------------------

//玩家准备(这个没错)
/*void logic_table::do_protobuf_player_ready(int pid) 
{
	auto sendmsg = PACKET_CREATE(packetl2c_player_ready_result, e_mst_l2c_player_ready_result);
	sendmsg->set_player_id(pid);

	if(m_player_left!=nullptr)
	{
		m_player_left->send_msg_to_client(sendmsg);
		std::cout<<"左边玩家准备"<<std::endl;
	}
	if(m_player_right!=nullptr)
	{
		m_player_right->send_msg_to_client(sendmsg);
		std::cout<<"右边玩家准备"<<std::endl;
	}
}*/

//通知 玩家开始游戏
void logic_table::do_protobuf_notice_start_game() 
{
	currentCardIndex=1;
	m_bet_time=0.0;

	auto sendmsg = PACKET_CREATE(packetl2c_notice_start_game_message, e_mst_l2c_notice_start_game_message);
	sendmsg->set_minbetcount(min_bet_count);//设置最小押注数

	const std::vector<int> right_cards= m_cardmanager->get_right_cards();
	const std::vector<int> left_cards= m_cardmanager->get_left_cards();

	m_player_left->copy_player_info(sendmsg->mutable_player_left());
	auto player_left_state= sendmsg->add_bet_state();//押注状态
	set_bet_state(player_left_state,m_player_left,0,nullptr);

	m_player_right->copy_player_info(sendmsg->mutable_player_right());
	auto player_right_state= sendmsg->add_bet_state();//押注状态
	set_bet_state(player_right_state,m_player_right,0,nullptr);
	
	bool sendleft=true;
	if(sendleft)
	{
		sendmsg->mutable_player_left()->clear_cards();
		sendmsg->mutable_player_left()->add_cards(left_cards[0]);//发牌
		sendmsg->mutable_player_left()->add_cards(left_cards[1]);

		sendmsg->mutable_player_right()->clear_cards();
		sendmsg->mutable_player_right()->add_cards(0);//发牌
		sendmsg->mutable_player_right()->add_cards(right_cards[1]);

		m_player_left->send_msg_to_client(sendmsg);
		sendleft=false;
	}
	if(sendleft==false)
	{
		sendmsg->mutable_player_left()->clear_cards();
		sendmsg->mutable_player_left()->add_cards(0);//发牌
		sendmsg->mutable_player_left()->add_cards(left_cards[1]);

		sendmsg->mutable_player_right()->clear_cards();
		sendmsg->mutable_player_right()->add_cards(right_cards[0]);//发牌
		sendmsg->mutable_player_right()->add_cards(right_cards[1]);
		m_player_right->send_msg_to_client(sendmsg);
	}

	//std::cout<<"开始游戏：发牌----------------------"<<player_left_state->state()<<":"<<player_right_state->state()<<std::endl;
}

void logic_table::copy_table_info(game_showhand_protocols::msg_table_info* table_info)
{
	table_info->set_tableid(get_id());

	if(m_player_left!=nullptr)
	{
		m_player_left->copy_player_info(table_info->mutable_player_left());			
	}

	if(m_player_right !=nullptr)
	{
		m_player_right->copy_player_info(table_info->mutable_player_right());
	}
}

//看底牌返回信息
void logic_table::do_protobuf_notice_view_card(int whoSeeCardId)
{
	auto sendmsg = PACKET_CREATE(packetl2c_view_card_result, e_mst_l2c_view_card_result);
	sendmsg->set_player_id(whoSeeCardId);
	if(m_player_left!=nullptr && m_player_left->get_pid()!=whoSeeCardId)
	{
		m_player_left->send_msg_to_client(sendmsg);
	}else if(m_player_right!=nullptr)
	{
		m_player_right->send_msg_to_client(sendmsg);
	}
}

//通知 玩家下注
void logic_table::do_protobuf_notice_bet(int32_t playerId, const msg_bet_info& betinfo) 
{
	m_bet_time=0.0;
	//设置下注后的状态
	if(betinfo.type()==e_bet_type::e_call_common_follow)//是否跟牌
	{
		if(once_bet_list.size()>1)
		{
			if(Is_left_ShowHands || Is_right_ShowHands)
			{
				Is_left_ShowHands=true;
				Is_right_ShowHands=true;
			}
			if(currentCardIndex==4)
			{
				m_game_state=e_game_state::e_state_game_award;
			}else
			{
				m_game_state=e_game_state::e_state_game_takecard;
			}
		}
	}
	else if(betinfo.type()==e_bet_type::e_call_abandon)//是否弃牌
	{
		if(m_player_left->get_pid()==playerId)
		{
			Is_left_abandonCard=true;
		}else
		{
			Is_right_abandonCard=true;
		}

		m_game_state=e_game_state::e_state_game_award;
	}
	else if(currentCardIndex>=2 && betinfo.type()==e_bet_type::e_call_showhand)//全压
	{
		if(m_player_left->get_pid()==playerId)
		{
			Is_left_ShowHands=true;
		}else
		{
			Is_right_ShowHands=true;
		}
		if(Is_left_ShowHands && Is_right_ShowHands)
		{
			if(currentCardIndex<4)
			{
				m_game_state=e_game_state::e_state_game_takecard;
			}else
			{
				m_game_state=e_game_state::e_state_game_award;
			}
		}
	}

	//发送下注信息
	auto sendmsg = PACKET_CREATE(packetl2c_notice_bet, e_mst_l2c_notice_start_bet_message);

	auto player_left_state= sendmsg->add_bet_state();
	set_bet_state(player_left_state,m_player_left,playerId,&betinfo);

	auto player_right_state= sendmsg->add_bet_state();
	set_bet_state(player_right_state,m_player_right,playerId,&betinfo);

	broadcast_msg_to_client(sendmsg);

	//std::cout<<"下注----------------------"<<player_left_state->state()<<":"<<player_right_state->state()<<std::endl;
}

//通知 玩家发牌
void logic_table::do_protobuf_notice_sendcard() 
{
	once_bet_list.clear();
	m_bet_time=0.0;
	currentCardIndex++;

	auto sendmsg = PACKET_CREATE(packetl2c_notice_sendcard_message, e_mst_l2c_notice_sendcard_message);

	const std::vector<int> right_cards= m_cardmanager->get_right_cards();
	const std::vector<int> left_cards= m_cardmanager->get_left_cards();

	sendmsg->set_player_left(m_cardmanager->get_left_cards()[currentCardIndex]);
	sendmsg->set_player_right(m_cardmanager->get_right_cards()[currentCardIndex]);

	auto player_left_state= sendmsg->add_bet_state();
	set_bet_state(player_left_state,m_player_left,0,nullptr);


	auto player_right_state= sendmsg->add_bet_state();
	set_bet_state(player_right_state,m_player_right,0,nullptr);

	broadcast_msg_to_client(sendmsg);

	//std::cout<<"发牌----------------------"<<player_left_state->state()<<":"<<player_right_state->state()<<std::endl;
}

//通知 玩家赢奖励
void logic_table::do_protobuf_notice_award()
{
	if(m_player_left!=nullptr)
	{
		m_player_left->set_player_table_state(player_table_state::e_table_state_no_prepare);
	}
	if(m_player_right!=nullptr)
	{
		m_player_right->set_player_table_state(player_table_state::e_table_state_no_prepare);
	}
	left_kichRubbish_cd=0.0;
	right_kichRubbish_cd=0.0;
	if(left_bet_state!=nullptr)
	{
		delete left_bet_state;
		left_bet_state=nullptr;
	}
	if(right_bet_state!=nullptr)
	{
		delete right_bet_state;
		right_bet_state=nullptr;
	}

	//计算奖励
	GOLD_TYPE gold_left_wincount=0;
	GOLD_TYPE gold_right_wincount=0;
	
	GOLD_TYPE winCount=left_bet_cout+right_bet_cout;
	if(Is_left_abandonCard || left_escape_pid>0)
	{
		gold_right_wincount=winCount;
	}else if(Is_right_abandonCard || right_escape_pid>0)
	{
		gold_left_wincount=winCount;
	}
	else
	{
		bool orleftwin= m_cardmanager->get_left_result();
		if(orleftwin)
		{
			gold_left_wincount=winCount;
		}else
		{
			gold_right_wincount=winCount;
		}
	}

		logic_player* left_player=nullptr;
		if(m_player_left!=nullptr)
		{
			left_player=m_player_left;
		}		
		else if(left_escape_pid>0)
		{
			LPlayerPtr left_player1=game_engine::instance().get_lobby().get_player(left_escape_pid);
			if(left_player1.get()!=nullptr)
			{
				left_player=left_player1.get();
			}
		}

		logic_player* right_player=nullptr; 
		if(m_player_right!=nullptr)
		{
			right_player=m_player_right;
		}else if(right_escape_pid>0)
		{
			LPlayerPtr right_player1=game_engine::instance().get_lobby().get_player(right_escape_pid);
			if(right_player1!=nullptr)
			{
				right_player=right_player1.get();
			}
		}

		assert(left_player!=nullptr && right_player!=nullptr);

		left_player->change_gold(gold_left_wincount,true);
		right_player->change_gold(gold_right_wincount,true);
	

		if(left_player->is_android() != right_player->is_android())//一个是机器人，一个是玩家的时候才计算机器人盈利率
		{
			if(left_player->is_android())//如果是左边玩家
			{
				m_rob_outcome+=left_bet_cout;
				m_rob_income+=gold_left_wincount;

				m_room->add_rob_income(gold_left_wincount);
				m_room->add_rob_outcome(left_bet_cout);
			}else
			{
				if(left_player->get_lucky()==0 && right_player->get_lucky()==0)
				{
					m_room->add_Stock_income(left_bet_cout,gold_left_wincount);
				}else
				{
					if(left_player->get_lucky()!=0 && gold_left_wincount>0)
					{
						left_player->change_lucky(gold_left_wincount);					
					}
				}
			}

			if(right_player->is_android())//如果是右边玩家
			{
				m_rob_outcome+=right_bet_cout;
				m_rob_income+=gold_right_wincount;

				m_room->add_rob_income(gold_right_wincount);
				m_room->add_rob_outcome(right_bet_cout);
			}else
			{
				if(left_player->get_lucky()==0 && right_player->get_lucky()==0)
				{
					m_room->add_Stock_income(right_bet_cout,gold_right_wincount);
				}else
				{
					if(right_player->get_lucky()!=0 && gold_right_wincount>0)
					{
						right_player->change_lucky(gold_right_wincount);						
					}
				}
			}
			m_room->set_rob_earn_rate();
		}

	m_room->do_protobuf_notice_Stock_Info();//同步库存
		

	//发送协议
	auto sendmsg = PACKET_CREATE(packetl2c_notice_award_message , e_mst_l2c_notice_start_award_message);

	sendmsg->mutable_player_left()->set_player_id(left_player->get_pid());
	sendmsg->mutable_player_left()->set_synctotalgoldcount(left_player->get_gold());
	sendmsg->mutable_player_left()->set_card(m_cardmanager->get_left_cards()[0]);
	sendmsg->mutable_player_left()->set_wingoldcount(gold_left_wincount);
	int combine_id=m_cardmanager->get_cards_type(m_cardmanager->get_left_cards());
	sendmsg->mutable_player_left()->set_combinecards_id(combine_id);

	sendmsg->mutable_player_right()->set_player_id(right_player->get_pid());
	sendmsg->mutable_player_right()->set_synctotalgoldcount(right_player ->get_gold());
	sendmsg->mutable_player_right()->set_card(m_cardmanager->get_right_cards()[0]);
	sendmsg->mutable_player_right()->set_wingoldcount(gold_right_wincount);
	combine_id=m_cardmanager->get_cards_type(m_cardmanager->get_right_cards());
	sendmsg->mutable_player_right()->set_combinecards_id(combine_id);

	broadcast_msg_to_client(sendmsg);

	left_escape_pid=0;
	right_escape_pid=0;

	m_room->do_protobuf_notice_table_player_state(this);//广播桌子内玩家状态信息

	//std::cout<<"左边玩家赢了："<<gold_left_wincount<<std::endl;
	//std::cout<<"右边玩家赢了："<<gold_right_wincount<<std::endl;
	//std::cout<<"开奖----------------------------------"<<std::endl;
}

//通知 桌子里其他人 进入
void logic_table::do_protobuf_notice_enter_table(logic_player* player) 
{
	auto sendmsg = PACKET_CREATE(game_showhand_protocols::packetl2c_notice_join_table , game_showhand_protocols::e_mst_l2c_notice_join_table_message);
	sendmsg->set_table_id(m_table_Id);
	player->copy_player_info(sendmsg->mutable_player_info());
	broadcast_msg_to_client(sendmsg, player->get_pid());

	//std::cout<<"通知 桌子 有人进入桌子："<<player->get_pid()<<std::endl;
}

//通知 桌子里其他人 离开
void logic_table::do_protobuf_notice_leave_table(int pid) 
{
	auto sendmsg = PACKET_CREATE(game_showhand_protocols::packetl2c_notice_leave_table , game_showhand_protocols::e_mst_l2c_notice_leave_table_message);
	sendmsg->set_table_id(m_table_Id);
	sendmsg->set_player_id(pid);
	broadcast_msg_to_client(sendmsg, pid);

	//std::cout<<"通知 桌子 有人离开桌子："<<pid<<std::endl;
}

//断线重连
void logic_table::do_protobuf_get_table_scene_info(logic_player* player)
{
	auto sendmsg = PACKET_CREATE(packetl2c_get_table_scene_result, e_mst_l2c_get_table_scene_info);
	sendmsg->set_minbetcount(min_bet_count);
	copy_table_info(sendmsg->mutable_table_info());
		if(m_player_left!=nullptr)
		{			
			if(m_player_left->get_player_table_state()==player_table_state::e_table_state_in_game)
			{
				sendmsg->set_left_betcount(left_bet_cout);
				if(left_bet_state!=nullptr)
				{
					sendmsg->add_bet_state()->CopyFrom(*left_bet_state);
					if(left_bet_state->state()==1 || left_bet_state->state()==2)
					{
						sendmsg->set_bet_time(m_bet_time-1);
					}
				}
				if(player->get_pid()==m_player_left->get_pid())
				{
					for(int i=0;i<=currentCardIndex;i++)
					{
						sendmsg->mutable_table_info()->mutable_player_left()->add_cards(m_cardmanager->get_left_cards()[i]);
						if(i>0)
						{
							sendmsg->mutable_table_info()->mutable_player_right()->add_cards(m_cardmanager->get_right_cards()[i]);
						}else
						{
							sendmsg->mutable_table_info()->mutable_player_right()->add_cards(0);
						}
					}
				}
			}
		}
		if(m_player_right!=nullptr)
		{
			if(m_player_right->get_player_table_state()==player_table_state::e_table_state_in_game)
			{
				sendmsg->set_right_betcount(right_bet_cout);
				if(player->get_pid()==m_player_right->get_pid())
				{
					if(right_bet_state!=nullptr)
					{
						sendmsg->add_bet_state()->CopyFrom(*right_bet_state);
						if(right_bet_state->state()==1 || right_bet_state->state()==2)
						{
							sendmsg->set_bet_time(m_bet_time-1);
						}
					}
					for(int i=0;i<=currentCardIndex;i++)
					{
						sendmsg->mutable_table_info()->mutable_player_right()->add_cards(m_cardmanager->get_right_cards()[i]);
						if(i>0)
						{
							sendmsg->mutable_table_info()->mutable_player_left()->add_cards(m_cardmanager->get_left_cards()[i]);
						}else
						{
							sendmsg->mutable_table_info()->mutable_player_left()->add_cards(0);
						}
					}
				}		
			}
		}
	player->send_msg_to_client(sendmsg);

	std::cout<<"断线重连 桌子信息："<<player->get_pid()<<std::endl;
}
//踢出桌子
void logic_table::kich_table_rubbish(logic_player* lcplayer) 
{
    auto sendmsg = PACKET_CREATE(packet_l2c_quit_desk_result, e_mst_l2c_quit_desk_result);
	msg_type_def::e_msg_result_def result = msg_type_def::e_rmt_fail;
	if(lcplayer->is_inRoom() && lcplayer->is_inTable())
	{
		 result=(msg_type_def::e_msg_result_def)lcplayer->leave_table();
		 sendmsg->set_room_id(lcplayer->get_room()->get_room_id());
	}else
	{
		 result = msg_type_def::e_rmt_fail;
	}

	lcplayer->get_room()->copy_tablelist(sendmsg->mutable_table_list());
    sendmsg->set_result(result);
    lcplayer->send_msg_to_client(sendmsg);

	//std::cout<<"踢出玩家："<<lcplayer->get_pid()<<std::endl;
}

//踢出房间,废弃的
/*void logic_table::kich_room_rubbish(logic_player* lcplayer)
{
	if(lcplayer!=nullptr && lcplayer->is_android())
	{
		game_engine::instance().release_robot(lcplayer->get_pid());//清除垃圾机器人
		m_room->set_and_decrease_count();
	}

	auto sendmsg = PACKET_CREATE(packetl2c_quit_game_room_result, e_mst_l2c_quit_game_room_result);
	if(lcplayer!=nullptr)
	{
		int result= lcplayer->leave_room();
		if(result == 1)
		{
			LROOM_MAP rooms = game_engine::instance().get_lobby().get_rooms();
			sendmsg->mutable_room_ids()->Reserve(rooms.size());
			for (LROOM_MAP::iterator it = rooms.begin(); it != rooms.end(); ++it) 
			{
				sendmsg->add_room_ids(it->first);
			}
		}
		sendmsg->set_result((msg_type_def::e_msg_result_def)result);
	}else
	{
		sendmsg->set_result(msg_type_def::e_msg_result_def::e_rmt_fail);
	}
    lcplayer->send_msg_to_client(sendmsg);

	std::cout<<"踢出房间："<<lcplayer->get_pid()<<std::endl;
}*/

void logic_table::do_protobuf_notice_gm_private_Info()
{
	auto sendmsg = PACKET_CREATE(packetl2c_notice_gm_all_card_info,e_mst_l2c_notice_gm_all_cards_info);
	std::vector<int> left_cards=m_cardmanager->get_left_cards();
	std::vector<int> right_cards=m_cardmanager->get_right_cards();
	std::for_each(left_cards.begin(),left_cards.end(),[&](int x){sendmsg->add_left_cards(x);});
	std::for_each(right_cards.begin(),right_cards.end(),[&](int x){sendmsg->add_right_cards(x);});
	sendmsg->set_orleft(m_cardmanager->get_left_result());
	broadcast_msg_to_client(sendmsg);
}