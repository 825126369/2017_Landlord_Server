#include "stdafx.h"
#include "i_game_engine.h"

i_game_engine::i_game_engine()
	:m_ehandler(nullptr)
{
}

i_game_engine::~i_game_engine()
{
}

void i_game_engine::set_handler(i_game_ehandler* ehandler)
{
	m_ehandler = ehandler;
}
i_game_ehandler* i_game_engine::get_handler()
{
	return m_ehandler;
}