#pragma once
#include <boost/circular_buffer.hpp>
#include <limits>

typedef boost::circular_buffer_space_optimized<char> buff_type;

class buffer_base
{
public:
	//maxlen ±ØÐë´óÓÚ blocklen
	buffer_base(uint32_t blocklen, uint32_t maxlen = std::numeric_limits<uint32_t>::max());
	virtual ~buffer_base();

	uint32_t get_size();
	virtual void clearup();

	void reset_size(uint32_t blocklen, uint32_t maxlen);
protected:
	buff_type m_buf;
	uint32_t m_maxlen;
	uint32_t m_release;

	bool check_len(uint32_t len);
};

class send_buffer:public buffer_base
{
public:
	send_buffer(uint32_t blocklen = 2048, uint32_t maxlen = 2048*16);
	virtual ~send_buffer();

	bool malloc_buf(uint32_t len);
	bool add_data(const char* buf, uint32_t len);
	void release_buf(uint32_t len);

	char* get_sendbuf();
	uint32_t get_sendlen();
};

class recv_buffer:public buffer_base
{
public:
	recv_buffer(uint32_t blocklen = 4096, uint32_t maxlen = 4096*12);
	virtual ~recv_buffer();

	bool malloc_buf();
	char* get_data(char* buf, uint32_t len);
	void release_buf(uint32_t len);
	void fix_offset(uint32_t len);
	char* get_head();
	char* get_recvbuf();
	uint32_t get_recvlen();
	uint32_t get_offset();
	virtual void clearup();
private:
	uint32_t m_offset;
	uint32_t m_left;
};