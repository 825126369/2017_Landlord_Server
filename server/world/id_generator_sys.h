#pragma once
#include "game_sys_def.h"

enum IdType
{
	IdTypeFirst,
	IdTypeNewAccountId = IdTypeFirst,
	IdTypeEnd,
};

// IDÉú³ÉÆ÷
class IdGeneratorSys : public game_sys_base
{
public:
	MAKE_SYS_TYPE(e_gst_id_generator);

	virtual void init_sys_object();
	
	virtual void sys_exit();

	int getCurId(IdType t);
private:
	void _saveId(int idType, int curValue);
private:
	std::map<int, int> m_curIds;
};



