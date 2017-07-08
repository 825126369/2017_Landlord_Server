#include "stdafx.h"
#include "logic_player.h"
#include "logic_room.h"
#include "logic_table.h"
#include "game_engine.h"
#include "SHOWHAND_BaseCFG.h"
#include <enable_random.h>
#include "SHOWHAND_RoomCFG.h"
#include "SHOWHAND_RobAICFG.h"
#include "SHOWHAND_RobCFG.h"

using namespace game_showhand_protocols;
SHOWHAND_SPACE_BEGIN
//创建机器人
void logic_room::create_robot()
{
	const static int player_min_count = ShowHand_BaseCFG::GetSingleton()->GetData("PlayerMinCount")->mValue;//30
	if(m_room_players.size()<player_min_count)
	{
		const static int robot_count = ShowHand_BaseCFG::GetSingleton()->GetData("RobMaxCount")->mValue;//15
		if (m_rob_count <= robot_count)
		{
			const static GOLD_TYPE gold_min = ShowHand_RoomCFG::GetSingleton()->GetData(get_room_id())->mRobMinGold;
			const static GOLD_TYPE gold_max = ShowHand_RoomCFG::GetSingleton()->GetData(get_room_id())->mRobMaxGold;
			const static int vip_min = ShowHand_RoomCFG::GetSingleton()->GetData(get_room_id())->mRobMinVip;
			const static int vip_max = ShowHand_RoomCFG::GetSingleton()->GetData(get_room_id())->mRobMaxVip;
			int tag=get_room_id()*10000+0;
			game_engine::instance().request_robot(tag, global_random::instance().rand_int(gold_min,gold_max), global_random::instance().rand_int(vip_min,vip_max));
		}
	}
}

void logic_table::robot_heart(double elapsed)
{
	static bool IsOpenRob=ShowHand_BaseCFG::GetSingleton()->GetData("IsOpenRob")->mValue>0;
	if (IsOpenRob)//创建机器人
	{
		if(get_player_count()==1 && ((m_player_left!=nullptr && !m_player_left->is_android()) || (m_player_right!=nullptr && !m_player_right->is_android())))
		{
			const static double robotEnterTime=5.0;
			m_rob_cd += elapsed;
			if(m_rob_cd > robotEnterTime)
			{
				if(get_player_left()!=nullptr && !get_player_left()->is_android() || get_player_right()!=nullptr && !get_player_right()->is_android())
				{
					logic_player* rob_player=m_room->get_room_Idle_robot();
					const static int robot_count = ShowHand_BaseCFG::GetSingleton()->GetData("RobMaxCount")->mValue;
					if(rob_player==nullptr || (rob_player!=nullptr && rob_player->get_gold()<m_room->get_EnterTableGold()))
					{
						if(m_room->get_rob_count()<=robot_count)
						{
							const static GOLD_TYPE gold_min = ShowHand_RoomCFG::GetSingleton()->GetData(m_room->get_room_id())->mRobMinGold;
							const static GOLD_TYPE gold_max = ShowHand_RoomCFG::GetSingleton()->GetData(m_room->get_room_id())->mRobMaxGold;
							const static int vip_min = ShowHand_RoomCFG::GetSingleton()->GetData(m_room->get_room_id())->mRobMinVip;
							const static int vip_max = ShowHand_RoomCFG::GetSingleton()->GetData(m_room->get_room_id())->mRobMaxVip;

							int tag=m_room->get_room_id()*10000+m_table_Id;
							game_engine::instance().request_robot(tag, global_random::instance().rand_int(gold_min,gold_max), global_random::instance().rand_int(vip_min,vip_max));	
						}
					}else
					{
						rob_player->enter_table(m_table_Id);
					}
				}
				m_rob_cd=0.0;
			}
		}else
		{
			m_rob_cd=0.0;
		}
	}
}


//机器人AI
void logic_player::robot_heartbeat(double elapsed)
{
	if(is_android() && m_room!=nullptr)
	{
		if(m_table==nullptr)
		{
			rob_continuePlayCount=0;
			if(m_room->get_EnterTableGold()<=get_gold())//释放机器人
			{
				if(entertable_cd>10.0)
				{
					int rand= global_random::instance().rand_int(0,100);
					if(rand>40)
					{
						int rob_match=global_random::instance().rand_int(1,10);
						if(rob_match>5)
						{
							int table_id= m_room->rob_match_auto_table();
							if(table_id>0)
							{
								enter_table(table_id);
							}
						}else
						{
							int table_id= m_room->match_auto_table();
							if(table_id>0)
							{
								enter_table(table_id);
							}
						}
					}
					entertable_cd=0.0;
				}else
				{
					entertable_cd+=elapsed;
				}
			}else
			{
				entertable_cd=0.0;
			}
		}else
		{
			if(m_player_table_state==player_table_state::e_table_state_no_prepare)
			{
				or_once_bet=false;
				
				if(prepare_cd>3.0)
				{
					if(rob_continuePlayCount_max==0)
					{
						rob_continuePlayCount_max=global_random::instance().rand_int(2,5);
					}
					if(rob_continuePlayCount>rob_continuePlayCount_max)
					{
						//std::cout<<"机器人连续打完几局，退出桌子："<<rob_continuePlayCount<<std::endl;
						leave_table();
						rob_continuePlayCount=0;
						rob_continuePlayCount_max=global_random::instance().rand_int(2,5);
					}else
					{
						int index=global_random::instance().rand_int(0,100);
						if(index>40)
						{
							prepare_game();
							rob_continuePlayCount++;
						}
					}
					prepare_cd=0.0;
				}else
				{
					prepare_cd+=elapsed;
				}
			}else if(m_table->get_game_state()==e_game_state::e_state_game_bet)
			{
				prepare_cd=0.0;
				if(m_table->get_player_left()==nullptr || m_table->get_player_right()==nullptr)
				{
					SLOG_CRITICAL<<"is in game,but player is null !!!"<<std::endl;
					return;
				}
				p_bet_state* mstate;
				if(get_pid()==m_table->get_player_left()->get_pid())
				{
					mstate=m_table->get_left_betstate();
				}else 
				{
					mstate=m_table->get_right_betstate();
				}

				if(mstate!=nullptr && mstate->state()==1)
				{
					if(bet_max_cd==0.0)
					{
						bet_max_cd=2.0;
					}
					bet_cd+=elapsed;
					msg_bet_info betinfo=msg_bet_info();
					if(bet_cd>bet_max_cd && or_once_bet==false)
					{	
						get_rob_bet(mstate,&betinfo);	
						assert(betinfo.type()!=e_bet_type::e_call_none);
						add_bet(betinfo);
						bet_max_cd=global_random::instance().rand_int(1,4);
						bet_cd=0.0;
						or_once_bet=true;
					}else
					{
	
						if(viewcard_max_cd==0.0)
						{
							viewcard_max_cd=1.0;
						}
						viewcard_cd+=elapsed;
						if(viewcard_cd>viewcard_max_cd)
						{
							int view_card_point=global_random::instance().rand_int(0,100);//看牌随机
							if(view_card_point>50)
							{
								seecard();
								viewcard_max_cd=global_random::instance().rand_int(0,3.0);
							}
							viewcard_cd=0.0;
						}
					}
				}else
				{
					or_once_bet=false;
					viewcard_cd=1.0;
					bet_cd=0.0;
				}
			}else
			{
				or_once_bet=false;
				viewcard_cd=1.0;
				bet_cd=0.0;
			}
		}
	}
}

//得到机器人的下注金额
void logic_player::get_rob_bet(p_bet_state* mstate,game_showhand_protocols::msg_bet_info* betinfo1)
{
	e_bet_type bet_type=get_rob_bet_type_1(mstate);//随机一个机器人可以下注的类型
	assert(bet_type!=e_bet_type::e_call_none);
	/*if(m_table->get_player_left()->is_android()!=m_table->get_player_right()->is_android())
	{
		std::cout<<"机器人"<<bet_type<<std::endl;
	}*/
	msg_bet_info& betinfo=*betinfo1;
	if(bet_type==e_bet_type::e_call_common_add_1)
	{
		betinfo.set_type(e_bet_type::e_call_common_add_1);
		betinfo.set_bet_count(m_table->get_min_bet_gold());
	}else if(bet_type==e_bet_type::e_call_common_add_2)
	{
		betinfo.set_type(e_bet_type::e_call_common_add_2);
		betinfo.set_bet_count(m_table->get_min_bet_gold()*2);
	}else if(bet_type==e_bet_type::e_call_common_add_3)
	{
		betinfo.set_type(e_bet_type::e_call_common_add_3);
		betinfo.set_bet_count(m_table->get_min_bet_gold()*3);
	}else if(bet_type==e_bet_type::e_call_common_follow)
	{
		if(m_table->get_or_have_hasuo())
		{
			betinfo.set_type(e_bet_type::e_call_common_follow);
			betinfo.set_bet_count(0);
		}else
		{
			betinfo.set_type(e_bet_type::e_call_common_follow);
			betinfo.set_bet_count(mstate->last_betinfo().bet_count());
		}
	}else if(bet_type==e_bet_type::e_call_showhand)
	{
		betinfo.set_type(e_bet_type::e_call_showhand);
		betinfo.set_bet_count(0);
	}else if(bet_type==e_bet_type::e_call_abandon)
	{
		betinfo.set_type(e_bet_type::e_call_abandon);
		betinfo.set_bet_count(0);
	}
}

void logic_player::get_rob_cheat_bet(p_bet_state* mstate,game_showhand_protocols::msg_bet_info* betinfo1)
{
	int size=mstate->bet_type_list().size();
	int index=global_random::instance().rand_int(0,size-1);
	e_bet_type bet_type=(e_bet_type)mstate->bet_type_list().Get(index);//随机一个机器人可以下注的类型

	msg_bet_info& betinfo=*betinfo1;
	if(bet_type==e_bet_type::e_call_common_add_1)
	{
		betinfo.set_type(e_bet_type::e_call_common_add_1);
		betinfo.set_bet_count(m_table->get_min_bet_gold());
	}else if(bet_type==e_bet_type::e_call_common_add_2)
	{
		betinfo.set_type(e_bet_type::e_call_common_add_2);
		betinfo.set_bet_count(m_table->get_min_bet_gold()*2);
	}else if(bet_type==e_bet_type::e_call_common_add_3)
	{
		betinfo.set_type(e_bet_type::e_call_common_add_3);
		betinfo.set_bet_count(m_table->get_min_bet_gold()*3);
	}else if(bet_type==e_bet_type::e_call_common_follow)
	{
		if(m_table->get_or_have_hasuo())
		{
			betinfo.set_type(e_bet_type::e_call_common_follow);
			betinfo.set_bet_count(0);
		}else
		{
			betinfo.CopyFrom(mstate->last_betinfo());
		}
	}else if(bet_type==e_bet_type::e_call_showhand)
	{
		betinfo.set_type(e_bet_type::e_call_showhand);
		betinfo.set_bet_count(0);
	}else if(bet_type==e_bet_type::e_call_abandon)
	{
		betinfo.set_type(e_bet_type::e_call_abandon);
		betinfo.set_bet_count(0);
	}
}

bool logic_player::judge_rob_win_lose(int currentCardIndex)
{
	if(m_player_table_state==player_table_state::e_table_state_in_game)
	{
		if(m_table->get_player_left()->get_pid()==this->get_pid())
		{
			return m_table->get_cardmanager()->get_left_win_rate()[currentCardIndex-1];
		}else
		{
			return m_table->get_cardmanager()->get_right_win_rate()[currentCardIndex-1];
		}
	}else
	{
		assert(false);
		return false;
	}
}

e_bet_type logic_player::get_rob_bet_type(p_bet_state* mstate)//得到机器人的类型
{
	const int bet_percent_add_1= 25;//普通下注
	const int bet_percent_add_2= 15;//普通下注	
	const int bet_percent_add_3= 10;//普通下注

	const int bet_percent_follow= 30;//跟注			
	const int bet_percent_abandon= 5;//弃牌
	const int bet_percent_showhand= 5;//梭哈

	int size=mstate->bet_type_list().size();
	int index=global_random::instance().rand_int(0,size-1);
	e_bet_type bet_type=(e_bet_type)mstate->bet_type_list().Get(index);//随机一个机器人可以下注的类型

	int size_percent=0;
	for(auto& bettype : mstate->bet_type_list())
	{
		if(bettype==e_bet_type::e_call_common_add_1)
		{
			size_percent+=bet_percent_add_1;
		}else if(bettype==e_bet_type::e_call_common_add_2)
		{
			size_percent+=bet_percent_add_2;
		}else if(bettype==e_bet_type::e_call_common_add_3)
		{
			size_percent+=bet_percent_add_3;
		}
		else if(bettype==e_bet_type::e_call_common_follow)
		{
			size_percent+=bet_percent_follow;
		}
		else if(bettype==e_bet_type::e_call_abandon)
		{
			size_percent+=bet_percent_abandon;
		}
		else if(bettype==e_bet_type::e_call_showhand)
		{
			size_percent+=bet_percent_showhand;
		}
	}
	int percent_index=global_random::instance().rand_int(0,size_percent);
	size_percent =0;
	for(auto& bettype : mstate->bet_type_list())
	{
		if(bettype==e_bet_type::e_call_common_add_1)
		{
			size_percent+=bet_percent_add_1;
		}else if(bettype==e_bet_type::e_call_common_add_2)
		{
			size_percent+=bet_percent_add_2;
		}else if(bettype==e_bet_type::e_call_common_add_3)
		{
			size_percent+=bet_percent_add_3;
		}
		else if(bettype==e_bet_type::e_call_common_follow)
		{
			size_percent+=bet_percent_follow;
		}
		else if(bettype==e_bet_type::e_call_abandon)
		{
			size_percent+=bet_percent_abandon;
		}
		else if(bettype==e_bet_type::e_call_showhand)
		{
			size_percent+=bet_percent_showhand;
		}

		if(percent_index<=size_percent)
		{
			bet_type=(e_bet_type)bettype;
			break;
		}
	}
	return bet_type;
}

e_bet_type logic_player::get_rob_bet_type_1(p_bet_state* mstate)//得到机器人的下注类型
{
	int index=global_random::instance().rand_int(0,100);
	if(index<10)
	{
		return get_rob_bet_type(mstate);
	}else
	{
		bool or_use_add_1=false;
		bool or_use_add_2=false;
		bool or_use_add_3=false;
		bool or_use_add_follow=false;
		bool or_use_add_abandon=false;
		bool or_use_add_showhand=false;

		for(auto& bettype : mstate->bet_type_list())
		{
		if(bettype==e_bet_type::e_call_common_add_1)
		{
			or_use_add_1=true;
		}else if(bettype==e_bet_type::e_call_common_add_2)
		{
			or_use_add_2=true;
		}else if(bettype==e_bet_type::e_call_common_add_3)
		{
			or_use_add_3=true;
		}
		else if(bettype==e_bet_type::e_call_common_follow)
		{
			or_use_add_follow=true;
		}
		else if(bettype==e_bet_type::e_call_abandon)
		{
			or_use_add_abandon=true;
		}
		else if(bettype==e_bet_type::e_call_showhand)
		{
			or_use_add_showhand=true;
		}
		}

		int CardCount=m_table->get_currentCardIndex()+1;
		bool orWin=judge_rob_win_lose(m_table->get_currentCardIndex());//在下注的时候做手脚
		/*if(m_table->get_player_left()->is_android()!=m_table->get_player_right()->is_android())
		{
			std::cout<<"机器人想我的牌大不大？ "<<(orWin?"---大----":"----小----")<<std::endl;
		}*/
		
		if(mstate->has_last_betinfo())
		{
			e_bet_type bettype=mstate->last_betinfo().type();	
			if(bettype==e_bet_type::e_call_common_add_1 || bettype==e_bet_type::e_call_common_add_2 || bettype==e_bet_type::e_call_common_add_3 || bettype==e_bet_type::e_call_common_follow)
			{
				if(orWin && or_use_add_1 && or_use_add_2 && or_use_add_3 && or_use_add_follow)
				{
					int index=global_random::instance().rand_int(1,4);
					if(index==1)
					{
						return e_bet_type::e_call_common_add_1;
					}else if(index==2)
					{
						return e_bet_type::e_call_common_add_2;
					}else if(index==3)
					{
						return e_bet_type::e_call_common_add_3;
					}else if(index==4)
					{
						return e_bet_type::e_call_common_follow;
					}
				}else if(orWin && or_use_add_follow)
				{
					return e_bet_type::e_call_common_follow;
				}
				else
				{
					if(CardCount==4)
					{
						int index=global_random::instance().rand_int(1,2);
						if(index==1)
						{
							return e_bet_type::e_call_abandon;
						}
					}else if(CardCount==5)
					{
						return e_bet_type::e_call_abandon;
					}
					return e_bet_type::e_call_common_follow;
				}
			}
			else if(bettype==e_bet_type::e_call_common_follow)//只可能是发牌后玩家第一次下注
			{
				assert(m_table->get_once_bet_size()==1);
				if(orWin && or_use_add_follow)
				{
					return e_bet_type::e_call_common_follow;
				}
			}
			else if(bettype==e_bet_type::e_call_showhand)
			{
				if(orWin)
				{
					int index=global_random::instance().rand_int(1,5);	
					if(index==1 || index==2)
					{
						return e_bet_type::e_call_abandon;
					}else
					{
						return e_bet_type::e_call_common_follow;
					}
				}else
				{
					int index=global_random::instance().rand_int(1,5);	
					if(index==1 || index==2 || index==3 ||index ==4)
					{
						return e_bet_type::e_call_abandon;
					}else
					{
						return e_bet_type::e_call_common_follow;
					}
				}
			}
			else
			{
				assert(false);
				return e_bet_type::e_call_none;
			}
		}else
		{
			//std::cout<<"机器人第一次发牌"<<std::endl;
			if(orWin && (or_use_add_1 || or_use_add_2 || or_use_add_3 || or_use_add_follow))
			{
				int index=global_random::instance().rand_int(1,3);
				if(index==1)
				{
					return e_bet_type::e_call_common_add_1;
				}else if(index==2)
				{
					return e_bet_type::e_call_common_add_2;
				}else if(index==3)
				{
					return e_bet_type::e_call_common_add_3;
				}
			}else
			{
				if(CardCount==4)
				{
					int index=global_random::instance().rand_int(1,2);
					if(index==1)
					{
						return e_bet_type::e_call_abandon;
					}
				}else if(CardCount==5)
				{
					return e_bet_type::e_call_abandon;
				}

				int index=global_random::instance().rand_int(1,10);
				if(index>=1 && index<=4)
				{
					return e_bet_type::e_call_common_follow;
				}else if(index>=5 && index<=7)
				{
					return e_bet_type::e_call_common_add_1;
				}else if(index>=8|| index<=9)
				{
					return e_bet_type::e_call_common_add_2;
				}else if(index==10)
				{
					return e_bet_type::e_call_common_add_3;
				}
			}
			return e_bet_type::e_call_none;
		}
	}
	assert(false);
	return e_bet_type::e_call_none;
}
SHOWHAND_SPACE_END