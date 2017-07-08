#pragma once
#include "game_object_def.h"

class game_object_handler
{
public:
	virtual bool db_add(GObjPtr obj) {return false;};
	virtual bool db_update(GObjPtr obj) {return false;};
	virtual bool db_del(GObjPtr obj){return false;};

	virtual bool db_add(game_object* obj) {return false;};
	virtual bool db_update(game_object* obj) {return false;};
	virtual bool db_del(game_object* obj){return false;};

	virtual void set_update(bool bupdate = true){};
};