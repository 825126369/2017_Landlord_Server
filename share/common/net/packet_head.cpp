#include "stdafx.h"
#include "packet_head.h"
#include <enable_processinfo.h>

const char checkmark[4] = {'$','3','&','@'};
const int keys_len = 12;
const unsigned char ed_keys[keys_len] = {0x12,0x33,0xa8,0x5c,0x6b,0x86,0x05,0x01,0xff,0xf3,0x5e,0xec/*0x77,0x9a,0x81,0x32,*/};


//////////////////////////////////////////////////////////////////////////

void packet_head_s::init(uint16_t pid, uint16_t psize)
{
	m_head.tick_time = enable_processinfo::get_tick_count();
	m_head.packet_id = pid;
	m_head.packet_size = psize;
	memcpy(m_head.head_mark, checkmark, sizeof(checkmark));
}

void packet_head_s::to_array(char* buf, int offset)
{
	memcpy(buf+offset, &m_head, PACKET_HEAD_SIZE);
}

void packet_head_s::parse_from(const char* buf, int offset)
{	
	memcpy(&m_head, buf+offset, PACKET_HEAD_SIZE);
}

bool packet_head_s::check_head()
{
	if (memcmp(&m_head.head_mark, checkmark, sizeof(checkmark)) == 0)
	{
		return true;
	}
	else
	{
		/*SLOG_CRITICAL << "包头：id:" << m_head.packet_id << std::endl;
		SLOG_CRITICAL << "包头：checkmark:" << m_head.head_mark << std::endl;*/
		return false;
	}
}

uint16_t packet_head_s::get_id()
{
	return m_head.packet_id;
}
uint16_t packet_head_s::get_size()
{
	return m_head.packet_size;
}
char* packet_head_s::get_checkmark()
{
	return m_head.head_mark;
}
void packet_head_s::buffer_decryption(char* buf, int len)
{

}

//////////////////////////////////////////////////////////////////////////

void packet_head_c::to_array(char* buf, int offset)
{
	packet_head_s::to_array(buf, offset);
	int j= 0;
	for (int i = 4;i<keys_len;i++,j++)
	{
		if(j>3) j=0;

		buf[i] ^= buf[j];
	}

	for (int i = 0;i<keys_len;i++)
	{
		buf[i] ^= ed_keys[i];
	}
}

void packet_head_c::parse_from(const char* buf, int offset)
{	
	packet_head_s::parse_from(buf, offset);
	char* pt = (char*)&m_head;
	for (int i = 0;i<keys_len;i++)
	{
		pt[i] ^= ed_keys[i];
	}

	int j = 0;
	for (int i = 4;i<keys_len;i++,j++)
	{
		if (j > 3)
		{
			j = 0;
		}
		pt[i] ^= pt[j];
	}
}

void packet_head_c::buffer_decryption(char* buf, int len)
{
	int j= 0;
	char* pt = (char*)&m_head;
	for (int i = 0;i<len;i++,j++)
	{
		if (j > 3)
		{
			j = 0;
		}
		buf[i] ^= pt[j];
	}
}
