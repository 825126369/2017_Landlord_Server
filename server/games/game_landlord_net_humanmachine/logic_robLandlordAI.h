#pragma once
#include <vector>
#include "logic_core.h"
#include "logic_playhandAI.h"
#include "logic_player.h"
#include "logic_table.h"

class logic_robLandlordAI
{
public:
	logic_robLandlordAI(logic_core*);
	bool orRobLandlord(int deskId);
private:
	logic_core* m_cardmanager;
};
