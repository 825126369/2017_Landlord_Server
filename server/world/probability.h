#pragma once

// 概率计算
class Probability
{
	typedef std::pair<int, int> RANGE;
public:
	/*
			初始化概率列表
	*/
	bool initByProbabilityList(std::vector<int>& pList);

	/*
			返回区间代号
	*/
	int getRandRange(int randNum);

	int getRandRange();
private:
	std::vector<RANGE> m_rangeList;
	
	int m_total;
};

