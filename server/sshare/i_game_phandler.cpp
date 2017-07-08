#include "stdafx.h"
#include "i_game_phandler.h"

i_game_phandler::i_game_phandler()
{
}

i_game_phandler::~i_game_phandler()
{
	m_player.reset();
}

void i_game_phandler::set_player(iGPlayerPtr player)
{
	m_player = player;
}