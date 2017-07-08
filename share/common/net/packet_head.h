#pragma once
#include <boost/cstdint.hpp>
#include <enable_smart_ptr.h>

static const int PACKET_HEAD_SIZE = 12;

struct packet_head
{
	uint32_t tick_time;
	uint16_t packet_id;
	uint16_t packet_size;
	char head_mark[4];
};

class packet_head_s
{
public:
	packet_head_s(){}
	void init(uint16_t pid, uint16_t psize);

	virtual void to_array(char* buf, int offset = 0);	
	virtual void parse_from(const char* buf, int offset = 0);
	virtual void buffer_decryption(char* buf, int len);

	bool check_head();
	uint16_t get_id();
	uint16_t get_size();
	char* get_checkmark();
protected:
	packet_head m_head;
};

class packet_head_c :public packet_head_s
{
public:
	packet_head_c(){}

	virtual void to_array(char* buf, int offset = 0);	
	virtual void parse_from(const char* buf, int offset = 0);
	virtual void buffer_decryption(char* buf, int len);
};