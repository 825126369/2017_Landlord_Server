#include "stdafx.h"

#define CRYPTOPP_ENABLE_NAMESPACE_WEAK 1

#include "enable_crypto.h"


#include <modes.h>
//#include <osrng.h>
#include <aes.h>
#include <md5.h>
#include <hex.h>
#include <sstream>
#include <string>
#if WIN32
#include <base64.h>
#else
#if CC_TARGET_PLATFORM == CC_PLATFORM_IOS
#include <cryptopp/base64.h>
#else
#include <base64.h>
#endif
#endif

#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/xml_parser.hpp> 
#include <boost/lexical_cast.hpp>

using namespace CryptoPP;
using namespace CryptoPP::Weak1;

static const uint8_t pseudoPrime1[] = {
	(uint8_t)0x85, (uint8_t)0x84, (uint8_t)0x64, (uint8_t)0xFD, (uint8_t)0x70, (uint8_t)0x6A,
	(uint8_t)0x9F, (uint8_t)0xF0, (uint8_t)0x94, (uint8_t)0x0C, (uint8_t)0x3E, (uint8_t)0x2C,
	(uint8_t)0x74, (uint8_t)0x34, (uint8_t)0x05, (uint8_t)0xC9, (uint8_t)0x55, (uint8_t)0xB3,
	(uint8_t)0x85, (uint8_t)0x32, (uint8_t)0x98, (uint8_t)0x71, (uint8_t)0xF9, (uint8_t)0x41,
	(uint8_t)0x21, (uint8_t)0x5F, (uint8_t)0x02, (uint8_t)0x9E, (uint8_t)0xEA, (uint8_t)0x56,
	(uint8_t)0x8D, (uint8_t)0x8C, (uint8_t)0x44, (uint8_t)0xCC, (uint8_t)0xEE, (uint8_t)0xEE,
	(uint8_t)0x3D, (uint8_t)0x2C, (uint8_t)0x9D, (uint8_t)0x2C, (uint8_t)0x12, (uint8_t)0x41,
	(uint8_t)0x1E, (uint8_t)0xF1, (uint8_t)0xC5, (uint8_t)0x32, (uint8_t)0xC3, (uint8_t)0xAA,
	(uint8_t)0x31, (uint8_t)0x4A, (uint8_t)0x52, (uint8_t)0xD8, (uint8_t)0xE8, (uint8_t)0xAF,
	(uint8_t)0x42, (uint8_t)0xF4, (uint8_t)0x72, (uint8_t)0xA1, (uint8_t)0x2A, (uint8_t)0x0D,
	(uint8_t)0x97, (uint8_t)0xB1, (uint8_t)0x31, (uint8_t)0xB3,
};

static const uint8_t pseudoPrime2[] = {
	(uint8_t)0x99, (uint8_t)0x98, (uint8_t)0xCA, (uint8_t)0xB8, (uint8_t)0x5E, (uint8_t)0xD7,
	(uint8_t)0xE5, (uint8_t)0xDC, (uint8_t)0x28, (uint8_t)0x5C, (uint8_t)0x6F, (uint8_t)0x0E,
	(uint8_t)0x15, (uint8_t)0x09, (uint8_t)0x59, (uint8_t)0x6E, (uint8_t)0x84, (uint8_t)0xF3,
	(uint8_t)0x81, (uint8_t)0xCD, (uint8_t)0xDE, (uint8_t)0x42, (uint8_t)0xDC, (uint8_t)0x93,
	(uint8_t)0xC2, (uint8_t)0x7A, (uint8_t)0x62, (uint8_t)0xAC, (uint8_t)0x6C, (uint8_t)0xAF,
	(uint8_t)0xDE, (uint8_t)0x74, (uint8_t)0xE3, (uint8_t)0xCB, (uint8_t)0x60, (uint8_t)0x20,
	(uint8_t)0x38, (uint8_t)0x9C, (uint8_t)0x21, (uint8_t)0xC3, (uint8_t)0xDC, (uint8_t)0xC8,
	(uint8_t)0xA2, (uint8_t)0x4D, (uint8_t)0xC6, (uint8_t)0x2A, (uint8_t)0x35, (uint8_t)0x7F,
	(uint8_t)0xF3, (uint8_t)0xA9, (uint8_t)0xE8, (uint8_t)0x1D, (uint8_t)0x7B, (uint8_t)0x2C,
	(uint8_t)0x78, (uint8_t)0xFA, (uint8_t)0xB8, (uint8_t)0x02, (uint8_t)0x55, (uint8_t)0x80,
	(uint8_t)0x9B, (uint8_t)0xC2, (uint8_t)0xA5, (uint8_t)0xCB,
};

enable_crypto::enable_crypto()
{
	BigInteger bi_p(pseudoPrime1, 64);
	BigInteger bi_q(pseudoPrime2, 64);
	m_pq = (bi_p-1)*(bi_q-1);
	m_n = bi_p * bi_q;
}

void enable_crypto::init(int keysize)
{
	m_e = m_pq.genCoPrime(keysize);
	m_d = m_e.modInverse(m_pq);
}


enable_crypto::~enable_crypto(void)
{
}


std::string enable_crypto::getModulus()
{
	std::string ret = m_e.ToHexString();

	return enable_crypto_helper::Base64Encode(reinterpret_cast<const uint8_t*>(ret.c_str()), ret.length());
}

void enable_crypto::setModulus(const std::string& str)
{
	std::string recovered = enable_crypto_helper::Base64Decode(str);
	m_e = BigInteger(recovered, 16);
}

std::string enable_crypto::RSAEncryptStr(const uint8_t* indata, uint32_t inlen)  
{  
	std::string cipher = enable_crypto_helper::str_to_hex(indata,inlen);
	BigInteger b_data(cipher, 16);
	b_data = b_data.modPow(m_e, m_n);
	cipher = b_data.ToHexString();

	return enable_crypto_helper::Base64Encode(reinterpret_cast<const uint8_t*>(cipher.c_str()), cipher.length());
}  

std::string enable_crypto::RSADecryptStr(const std::string& str)  
{  
	std::string recovered = enable_crypto_helper::Base64Decode(str);
	BigInteger b_data(recovered, 16);
	b_data = b_data.modPow(m_d, m_n);

	return enable_crypto_helper::hex_to_str(b_data.ToHexString());
}




std::string enable_crypto_helper::CalMD5(const uint8_t* indata, uint32_t inlen)
{
	static MD5 m;
	uint8_t bmd5[MD5::DIGESTSIZE] = {0};
	m.CalculateDigest(bmd5, indata, inlen);
	std::string result;
	HexEncoder enc(new StringSink(result));  
	enc.Put(bmd5, MD5::DIGESTSIZE);
	enc.MessageEnd();
	return result;
}


static const uint8_t iv[AES::BLOCKSIZE]={ 0x12, 0x3c, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x66, 0x10, 0xCD, 0xEF };
std::string enable_crypto_helper::AESEncryptString(const uint8_t* indata, uint32_t inlen, const std::string& passPhrase) // AES加密字符串函数
{
	std::string outstr;	

	try
	{
		CBC_Mode<AES>::Encryption  Encryptor1(reinterpret_cast<const uint8_t*>(passPhrase.c_str()), passPhrase.length(), iv);   
		StringSource(   indata,inlen,  
			true,  
			new StreamTransformationFilter( Encryptor1,
			new Base64Encoder(new StringSink( outstr )),  
			BlockPaddingSchemeDef::BlockPaddingScheme::PKCS_PADDING,  
			true)  
			);  
	}
	catch (...)
	{
	}

	return outstr;
}

std::string enable_crypto_helper::AESDecryptString(const std::string& str, const std::string& passPhrase)// AES解密字符串函数
{
	std::string outstr;
	
	try
	{
		CBC_Mode<AES>::Decryption Decryptor1(reinterpret_cast<const uint8_t*>(passPhrase.c_str()), passPhrase.length(), iv);
		StringSource( reinterpret_cast<const uint8_t*>(str.c_str()), str.length(),   
			true,  
			new Base64Decoder(new StreamTransformationFilter( Decryptor1,  
			new StringSink( outstr ),  
			BlockPaddingSchemeDef::BlockPaddingScheme::PKCS_PADDING,  
			true))
			);  
	}
	catch (...)
	{
	}	

	return outstr;
}


std::string enable_crypto_helper::Base64Encode(const uint8_t* indata, uint32_t inlen)
{
	std::string outstr;
	StringSource(indata, inlen, true,
		new Base64Encoder(
		new StringSink(outstr)));
	return outstr;
}
std::string enable_crypto_helper::Base64Decode(const std::string& str)
{
	std::string outstr;
	StringSource(reinterpret_cast<const uint8_t*>(str.c_str()), str.length(), true,
		new Base64Decoder(
		new StringSink(outstr)));
	return outstr;
}

inline uint8_t toHex(const uint8_t &x)  
{  
	return x > 9 ? x -10 + 'a': x + '0';  
}  

inline uint8_t fromHex(const uint8_t &x)  
{  
	return isdigit(x) ? x-'0' : x-'a'+10;  
}  

std::string enable_crypto_helper::UrlEncode(const std::string &sIn)  
{  
	std::string sOut;  
	for( uint32_t ix = 0; ix < sIn.size(); ix++ )  
	{        
		uint8_t buf[4];  
		memset( buf, 0, 4 );  
		if( isalnum( (uint8_t)sIn[ix] ) )  
		{        
			buf[0] = sIn[ix];  
		}  
		//else if ( isspace( (uint8_t)sIn[ix] ) ) //貌似把空格编码成%20或者+都可以  
		//{  
		//    buf[0] = '+';  
		//}  
		else  
		{  
			buf[0] = '%';  
			buf[1] = toHex( (uint8_t)sIn[ix] >> 4 );  
			buf[2] = toHex( (uint8_t)sIn[ix] % 16);  
		}  
		sOut += (char *)buf;  
	}  
	return sOut;  
};  

uint8_t convectLower(uint8_t c)
{
	return tolower(c);
}

std::string enable_crypto_helper::UrlDecode(const std::string &sIn)  
{  
	std::string sOut;  
	for( uint32_t ix = 0; ix < sIn.size(); ix++ )  
	{  
		uint8_t ch = 0;  
		if(sIn[ix]=='%')  
		{  
			ch = (fromHex(convectLower(sIn[ix+1]))<<4);  
			ch |= fromHex(convectLower(sIn[ix+2]));  
			ix += 2;  
		}  
		else if(sIn[ix] == '+')  
		{  
			ch = ' ';  
		}  
		else  
		{  
			ch = sIn[ix];  
		}  
		sOut += (char)ch;  
	}  
	return sOut;  
}  


std::string  enable_crypto_helper::str_to_hex(const uint8_t* indata, uint32_t inlen)
{
	std::stringstream  ss;
	for(int i=0;i<(int)inlen; i++) 
	{
		ss << std::hex << std::setw(2) <<std::setfill('0') << (int)indata[i];
	}
	return ss.str();
}

std::string  enable_crypto_helper::hex_to_str( std::string str)
{
	int ns = str.length();
	if(ns%2 != 0)
	{
		str = "0"+str;
		ns++;
	}
	
	std::stringstream ss; 	
	std::stringstream oss; 
	int temp = 0;
	for (int i =0;i<ns;i+=2) {
		ss << std::dec << str.substr(i, 2);
		ss >> std::hex >> temp;
		ss.clear();
		oss << (uint8_t)temp;
	}

	return oss.str();

}