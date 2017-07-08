#include "stdafx.h"
#include "i_game_player.h"

i_game_player::i_game_player()
{
}

i_game_player::~i_game_player()
{
}

void i_game_player::set_handler(iGPhandlerPtr phandler)
{
	m_phandler = boost::weak_ptr<i_game_phandler>(phandler);
}

iGPhandlerPtr i_game_player::get_handler()
{
	return m_phandler.lock();
}