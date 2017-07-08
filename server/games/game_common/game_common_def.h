#pragma once
#include <string>

// 百家乐游戏总局数，一直递增
static const std::string TOTAL_COUNT_BACCARAT = "totalCountBaccarat";
// 百家乐游戏当天计数，０点清空
static const std::string DAY_COUNT_BACCARAT = "dayCountBaccarat";

// 鳄鱼大亨游戏总局数，一直递增
static const std::string TOTAL_COUNT_CROCODILE = "totalCountCrocodile";

// 骰宝游戏总局数，一直递增
static const std::string TOTAL_COUNT_DICE = "totalCountDice";

// 牛牛游戏总局数，一直递增
static const std::string TOTAL_COUNT_COWS = "totalCountCows";

// 黑红梅方游戏总局数，一直递增
static const std::string TOTAL_COUNT_SHCD = "totalCountShcd";
// 黑红梅方游戏当天计数，０点清空
static const std::string DAY_COUNT_SHCD = "dayCountShcd";

enum GameId
{
	game_baccracat = 5,             // 百家乐

	game_shcd = 10,                 // 黑红梅方
};

