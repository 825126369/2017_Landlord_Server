#pragma once

#include <string>
#include <BigInteger.h>

#ifdef WIN32
#ifdef _DEBUG
#pragma comment(lib, "cryptlib-gd.lib")

#else
#pragma comment(lib, "cryptlib.lib")
#endif
#endif

class enable_crypto
{
public:
	enable_crypto(void);
	~enable_crypto(void);
	void init(int keysize = 512);

	std::string RSAEncryptStr(const uint8_t* indata, uint32_t inlen);
	std::string RSADecryptStr(const std::string& str);

	std::string getModulus();	
	void setModulus(const std::string& str);	
private:
	BigInteger m_e;
	BigInteger m_n;
	BigInteger m_d;
	BigInteger m_pq;
};


class enable_crypto_helper
{
public:
	static std::string CalMD5(const uint8_t* indata, uint32_t inlen);
	static std::string AESEncryptString(const uint8_t* indata, uint32_t inlen, const std::string& passPhrase);
	static std::string AESDecryptString(const std::string& str, const std::string& passPhrase);

	static std::string UrlEncode(const std::string& sIn);
	static std::string UrlDecode(const std::string& sIn);

	static std::string Base64Decode(const std::string& str);
	static std::string Base64Encode(const uint8_t* indata, uint32_t inlen);

	static std::string str_to_hex(const uint8_t* indata, uint32_t inlen);
	static std::string hex_to_str(std::string vec);
};