#pragma once

#include "logic_core.h"
#include "logic_playhandAI.h"

class  logic_jipaiqi
{
public:
	 logic_jipaiqi();
	~ logic_jipaiqi();
private:
	std::map<cardType, std::vector<cardTypeInfo>> mMap;
//	std::map<int,int> 
};
