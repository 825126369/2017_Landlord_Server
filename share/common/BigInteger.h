#pragma once
#include <boost/cstdint.hpp>
#include <string>

class BigInteger
{
public:
	BigInteger(void);
	BigInteger(int64_t value);
	BigInteger(uint64_t value);
	BigInteger(const BigInteger &bi);
	BigInteger(std::string value, int radix);
	BigInteger(const uint8_t* inData, int inLen);
	BigInteger(const uint32_t* inData, int inLen);
	BigInteger operator =(const BigInteger &bi2);
	BigInteger operator +(const BigInteger &bi2);
	BigInteger operator -();
	BigInteger modPow( BigInteger exp,  BigInteger n);
	int bitCount();
	BigInteger BarrettReduction(const BigInteger& x,const  BigInteger& n,const  BigInteger& constant);
	bool operator >=(const BigInteger& bi2)
	{
		return ((*this) == bi2 || (*this) > bi2);
	}
	bool operator >(const BigInteger& bi2);
	bool operator ==(const BigInteger& bi2);
	BigInteger operator %( BigInteger bi2);
	void singleByteDivide( BigInteger bi1,  BigInteger bi2,
		 BigInteger &outQuotient,  BigInteger &outRemainder);
	void multiByteDivide( BigInteger bi1,  BigInteger bi2,
		 BigInteger &outQuotient,  BigInteger &outRemainder);
	int shiftRight(uint32_t buffer[], int bufLen, int shiftVal);
	BigInteger operator <<(int shiftVal);
	int shiftLeft(uint32_t buffer[], int bufLen, int shiftVal);
	bool operator <(const BigInteger& bi2);
	BigInteger operator +=(const BigInteger& bi2);
	BigInteger operator /( BigInteger bi2);
	BigInteger operator -=(const BigInteger& bi2);
	BigInteger operator -(const BigInteger& bi2);
	BigInteger operator -(int n);
	std::string DecToHex(unsigned int value,const std::string& format);
	std::string ToHexString();

	BigInteger genCoPrime(int bits);
	BigInteger gcd(BigInteger bi);
	void genRandomBits(int bits);
	BigInteger modInverse(const BigInteger& modulus);
public:
	~BigInteger(void);
public:
	BigInteger operator *( BigInteger bi2);
private:

public:
	int dataLength;
		// number of actual chars used
private:
	static const int maxLength;
		// maximum length of the BigInteger in uint (4 uint8_ts)
		// change this to suit the required level of precision.
	unsigned int *data;
		// stores uint8_ts from the Big Integer
};
