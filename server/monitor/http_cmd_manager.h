#pragma once
#include "enable_singleton.h"
#include "enable_queue.h"

/*
		管理通过http传过来的命令
*/
class HttpCmdManager : public enable_singleton<HttpCmdManager>
{
	enum
	{
		// 最大数量
		max_count = 1000,

		// 每帧发送数量
		process_count_each_frame = 20,
	};
public:
	bool addCommand(const std::string& cmd);

	// 发送命令消息
	void heartbeat(double elapsed);
private:
	fast_safe_queue<std::string> m_cmdList;
};


