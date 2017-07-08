#include "StdAfx.h"
#include "BigInteger.h"
//#include <cstring>
#include <string>
#include <algorithm>
#include <assert.h>
#include <enable_random.h>

using namespace std;

const int BigInteger::maxLength = 70;

BigInteger::BigInteger(void)
: dataLength(0), data(0)
{
	data = new uint32_t[maxLength];
	memset(data, 0, maxLength * sizeof(uint32_t));
	dataLength = 1;
}

BigInteger::BigInteger(int64_t value)
{
	data = new uint32_t[maxLength];
	memset(data, 0, maxLength * sizeof(uint32_t));
	int64_t tempVal = value;

	// copy uint8_ts from int64_t to BigInteger without any assumption of
	// the length of the int64_t datatype

	dataLength = 0;
	while (value != 0 && dataLength < maxLength)
	{
		data[dataLength] = (uint32_t)(value & 0xFFFFFFFF);
		value = value >> 32;
		dataLength++;
	}

	if (tempVal > 0)         // overflow check for +ve value
	{
		if (value != 0 || (data[maxLength - 1] & 0x80000000) != 0)
			assert(false);
	}
	else if (tempVal < 0)    // underflow check for -ve value
	{
		if (value != -1 || (data[dataLength - 1] & 0x80000000) == 0)
			assert(false);
	}

	if (dataLength == 0)
		dataLength = 1;
}

BigInteger::BigInteger(uint64_t value)
{
	data = new uint32_t[maxLength];
	memset(data, 0, maxLength * sizeof(uint32_t));
	// copy uint8_ts from uint64_t to BigInteger without any assumption of
	// the length of the uint64_t datatype

	dataLength = 0;
	while (value != 0 && dataLength < maxLength)
	{
		data[dataLength] = (uint32_t)(value & 0xFFFFFFFF);
		value >>= 32;
		dataLength++;
	}

	if (value != 0 || (data[maxLength - 1] & 0x80000000) != 0)
		assert(false);

	if (dataLength == 0)
		dataLength = 1;
}

BigInteger::BigInteger(const BigInteger &bi)
{
	data = new uint32_t[maxLength];
	memset(data, 0, maxLength * sizeof(uint32_t));

	dataLength = bi.dataLength;

	for (int i = 0; i < dataLength; i++)
		data[i] = bi.data[i];
}

BigInteger::~BigInteger(void)
{
	if (data != NULL)
	{
		delete []data;
	}
}

BigInteger::BigInteger(string value, int radix)
{
	BigInteger multiplier((int64_t)1);
	BigInteger result;
	std::transform(value.begin(), value.end(), value.begin(), (int(*)(int))std::toupper);

	int limit = 0;

	if (value[0] == '-')
		limit = 1;

	for (int i = value.size() - 1; i >= limit; i--)
	{
		int posVal = (int)value[i];

		if (posVal >= '0' && posVal <= '9')
			posVal -= '0';
		else if (posVal >= 'A' && posVal <= 'Z')
			posVal = (posVal - 'A') + 10;
		else
			posVal = 9999999;       // arbitrary large


		if (posVal >= radix)
		{
			assert(false);
		}
		else
		{
			if (value[0] == '-')
				posVal = -posVal;

			result = result + (multiplier * BigInteger((int64_t)posVal));

			if ((i - 1) >= limit)
				multiplier = multiplier * BigInteger((int64_t)radix);
		}
	}

	if (value[0] == '-')     // negative values
	{
		if ((result.data[maxLength - 1] & 0x80000000) == 0)
			assert(false);
	}
	else    // positive values
	{
		if ((result.data[maxLength - 1] & 0x80000000) != 0)
			assert(false);
	}

	data = new uint32_t[maxLength];
	memset(data, 0, maxLength * sizeof(uint32_t));
	for (int i = 0; i < result.dataLength; i++)
		data[i] = result.data[i];

	dataLength = result.dataLength;
}

BigInteger::BigInteger(const uint8_t* inData, int inLen)
{
	dataLength = inLen >> 2;

	int leftOver = inLen & 0x3;
	if (leftOver != 0)         // length not multiples of 4
		dataLength++;


	if (dataLength > maxLength)
		assert(false);

	data = new uint32_t[maxLength];
	memset(data, 0, maxLength * sizeof(uint32_t));

	for (int i = inLen - 1, j = 0; i >= 3; i -= 4, j++)
	{
		data[j] = (uint32_t)((inData[i - 3] << 24) + (inData[i - 2] << 16) +
			(inData[i - 1] << 8) + inData[i]);
	}

	if (leftOver == 1)
		data[dataLength - 1] = (uint32_t)inData[0];
	else if (leftOver == 2)
		data[dataLength - 1] = (uint32_t)((inData[0] << 8) + inData[1]);
	else if (leftOver == 3)
		data[dataLength - 1] = (uint32_t)((inData[0] << 16) + (inData[1] << 8) + inData[2]);


	while (dataLength > 1 && data[dataLength - 1] == 0)
		dataLength--;
}

BigInteger::BigInteger(const uint32_t* inData, int inLen)
{
	dataLength = inLen;

	if (dataLength > maxLength)
		assert(false);

	data = new uint32_t[maxLength];
	memset(data, 0, maxLength * sizeof(maxLength));

	for (int i = dataLength - 1, j = 0; i >= 0; i--, j++)
		data[j] = inData[i];

	while (dataLength > 1 && data[dataLength - 1] == 0)
		dataLength--;
}

BigInteger BigInteger::operator *( BigInteger bi2)
{
	BigInteger bi1(*this);
	int lastPos = maxLength - 1;
	bool bi1Neg = false, bi2Neg = false;

	// take the absolute value of the inputs
	try
	{
		if ((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
		{
			bi1Neg = true; 
			bi1 = -bi1;
		}
		if ((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
		{
			bi2Neg = true; bi2 = -bi2;
		}
	}
	catch (...) { }

	BigInteger result;

	// multiply the absolute values
	try
	{
		for (int i = 0; i < bi1.dataLength; i++)
		{
			if (bi1.data[i] == 0) continue;

			uint64_t mcarry = 0;
			for (int j = 0, k = i; j < bi2.dataLength; j++, k++)
			{
				// k = i + j
				uint64_t val = ((uint64_t)bi1.data[i] * (uint64_t)bi2.data[j]) +
					(uint64_t)result.data[k] + mcarry;

				result.data[k] = (uint64_t)(val & 0xFFFFFFFF);
				mcarry = (val >> 32);
			}

			if (mcarry != 0)
				result.data[i + bi2.dataLength] = (uint32_t)mcarry;
		}
	}
	catch (...)
	{
		assert(false);
	}


	result.dataLength = bi1.dataLength + bi2.dataLength;
	if (result.dataLength > maxLength)
		result.dataLength = maxLength;

	while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
		result.dataLength--;

	// overflow check (result is -ve)
	if ((result.data[lastPos] & 0x80000000) != 0)
	{
		if (bi1Neg != bi2Neg && result.data[lastPos] == 0x80000000)    // different sign
		{
			// handle the special case where multiplication produces
			// a max negative number in 2's complement.

			if (result.dataLength == 1)
				return result;
			else
			{
				bool isMaxNeg = true;
				for (int i = 0; i < result.dataLength - 1 && isMaxNeg; i++)
				{
					if (result.data[i] != 0)
						isMaxNeg = false;
				}

				if (isMaxNeg)
					return result;
			}
		}

		assert(false);
	}

	// if input has different signs, then result is -ve
	if (bi1Neg != bi2Neg)
		return -result;

	return result;
}
BigInteger BigInteger::operator =(const BigInteger &bi2)
{
	if (&bi2 == this)
	{
		return *this;
	}
	if (data != NULL)
	{
		delete []data;
		data = NULL;
	}
	data = new uint32_t[maxLength];
	memset(data, 0, maxLength * sizeof(uint32_t));

	dataLength = bi2.dataLength;

	for (int i = 0; i < dataLength; i++)
		data[i] = bi2.data[i];
	return *this;
}

BigInteger BigInteger::operator +(const BigInteger &bi2)
{
	BigInteger result;

	result.dataLength = (this->dataLength > bi2.dataLength) ? this->dataLength : bi2.dataLength;

	int64_t carry = 0;
	for (int i = 0; i < result.dataLength; i++)
	{
		int64_t sum = (int64_t)this->data[i] + (int64_t)bi2.data[i] + carry;
		carry = sum >> 32;
		result.data[i] = (uint32_t)(sum & 0xFFFFFFFF);
	}

	if (carry != 0 && result.dataLength < maxLength)
	{
		result.data[result.dataLength] = (uint32_t)(carry);
		result.dataLength++;
	}

	while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
		result.dataLength--;


	// overflow check
	int lastPos = maxLength - 1;
	if ((this->data[lastPos] & 0x80000000) == (bi2.data[lastPos] & 0x80000000) &&
		(result.data[lastPos] & 0x80000000) != (this->data[lastPos] & 0x80000000))
	{
		assert(false);
	}

	return result;
}

BigInteger BigInteger::operator -()
{
	// handle neg of zero separately since it'll cause an overflow
	// if we proceed.

	if (this->dataLength == 1 && this->data[0] == 0)
		return BigInteger();

	BigInteger result(*this);

	// 1's complement
	for (int i = 0; i < maxLength; i++)
		result.data[i] = (uint32_t)(~(this->data[i]));

	// add one to result of 1's complement
	int64_t val, carry = 1;
	int index = 0;

	while (carry != 0 && index < maxLength)
	{
		val = (int64_t)(result.data[index]);
		val++;

		result.data[index] = (uint32_t)(val & 0xFFFFFFFF);
		carry = val >> 32;

		index++;
	}

	if ((this->data[maxLength - 1] & 0x80000000) == (result.data[maxLength - 1] & 0x80000000))
		assert(false&&"Overflow in negation.\n");

	result.dataLength = maxLength;

	while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
		result.dataLength--;
	return result;
}

BigInteger BigInteger::modPow( BigInteger exp,  BigInteger n)
{
	if ((exp.data[maxLength - 1] & 0x80000000) != 0)	
		assert(false&&"Positive exponents only.");	

	BigInteger resultNum((int64_t)1);
	BigInteger tempNum;
	bool thisNegative = false;

	if ((this->data[maxLength - 1] & 0x80000000) != 0)   // negative this
	{
		tempNum = -(*this) % n;
		thisNegative = true;
	}
	else
		tempNum = (*this) % n;  // ensures (tempNum * tempNum) < b^(2k)

	if ((n.data[maxLength - 1] & 0x80000000) != 0)   // negative n
		n = -n;

	// calculate constant = b^(2k) / m
	BigInteger constant;

	int i = n.dataLength << 1;
	constant.data[i] = 0x00000001;
	constant.dataLength = i + 1;

	constant = constant / n;
	int totalBits = exp.bitCount();
	int count = 0;

	// perform squaring and multiply exponentiation
	for (int pos = 0; pos < exp.dataLength; pos++)
	{
		uint32_t mask = 0x01;
		//Console.WriteLine("pos = " + pos);

		for (int index = 0; index < 32; index++)
		{
			if ((exp.data[pos] & mask) != 0)
				resultNum = BarrettReduction(resultNum * tempNum, n, constant);

			mask <<= 1;

			tempNum = BarrettReduction(tempNum * tempNum, n, constant);


			if (tempNum.dataLength == 1 && tempNum.data[0] == 1)
			{
				if (thisNegative && (exp.data[0] & 0x1) != 0)    //odd exp
					return -resultNum;
				return resultNum;
			}
			count++;
			if (count == totalBits)
				break;
		}
	}

	if (thisNegative && (exp.data[0] & 0x1) != 0)    //odd exp
		return -resultNum;

	return resultNum;
}

int BigInteger::bitCount()
{
	while (dataLength > 1 && data[dataLength - 1] == 0)
		dataLength--;

	uint32_t value = data[dataLength - 1];
	uint32_t mask = 0x80000000;
	int bits = 32;

	while (bits > 0 && (value & mask) == 0)
	{
		bits--;
		mask >>= 1;
	}
	bits += ((dataLength - 1) << 5);

	return bits;
}

BigInteger BigInteger::BarrettReduction(const BigInteger& x,const  BigInteger& n,const  BigInteger& constant)
{
	int k = n.dataLength,
		kPlusOne = k + 1,
		kMinusOne = k - 1;

	BigInteger q1;

	// q1 = x / b^(k-1)
	for (int i = kMinusOne, j = 0; i < x.dataLength; i++, j++)
		q1.data[j] = x.data[i];
	q1.dataLength = x.dataLength - kMinusOne;
	if (q1.dataLength <= 0)
		q1.dataLength = 1;


	BigInteger q2 = q1 * constant;
	BigInteger q3;

	// q3 = q2 / b^(k+1)
	for (int i = kPlusOne, j = 0; i < q2.dataLength; i++, j++)
		q3.data[j] = q2.data[i];
	q3.dataLength = q2.dataLength - kPlusOne;
	if (q3.dataLength <= 0)
		q3.dataLength = 1;


	// r1 = x mod b^(k+1)
	// i.e. keep the lowest (k+1) words
	BigInteger r1;
	int lengthToCopy = (x.dataLength > kPlusOne) ? kPlusOne : x.dataLength;
	for (int i = 0; i < lengthToCopy; i++)
		r1.data[i] = x.data[i];
	r1.dataLength = lengthToCopy;


	// r2 = (q3 * n) mod b^(k+1)
	// partial multiplication of q3 and n

	BigInteger r2;
	for (int i = 0; i < q3.dataLength; i++)
	{
		if (q3.data[i] == 0) continue;

		uint64_t mcarry = 0;
		int t = i;
		for (int j = 0; j < n.dataLength && t < kPlusOne; j++, t++)
		{
			// t = i + j
			uint64_t val = ((uint64_t)q3.data[i] * (uint64_t)n.data[j]) +
				(uint64_t)r2.data[t] + mcarry;

			r2.data[t] = (uint32_t)(val & 0xFFFFFFFF);
			mcarry = (val >> 32);
		}

		if (t < kPlusOne)
			r2.data[t] = (uint32_t)mcarry;
	}
	r2.dataLength = kPlusOne;
	while (r2.dataLength > 1 && r2.data[r2.dataLength - 1] == 0)
		r2.dataLength--;

	r1 -= r2;
	if ((r1.data[maxLength - 1] & 0x80000000) != 0)        // negative
	{
		BigInteger val;
		val.data[kPlusOne] = 0x00000001;
		val.dataLength = kPlusOne + 1;
		r1 += val;
	}

	while (r1 >= n)
		r1 -= n;

	return r1;
}

bool BigInteger::operator >(const BigInteger& bi2)
{
	int pos = maxLength - 1;
	BigInteger bi1(*this);

	// bi1 is negative, bi2 is positive
	if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
		return false;

	// bi1 is positive, bi2 is negative
	else if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
		return true;

	// same sign
	int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;
	for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--) ;

	if (pos >= 0)
	{
		if (bi1.data[pos] > bi2.data[pos])
			return true;
		return false;
	}
	return false;
}

bool BigInteger::operator ==(const BigInteger& bi2)
{
	if (this->dataLength != bi2.dataLength)
		return false;

	for (int i = 0; i < this->dataLength; i++)
	{
		if (this->data[i] != bi2.data[i])
			return false;
	}
	return true;
}

BigInteger BigInteger::operator %( BigInteger bi2)
{
	BigInteger bi1(*this);
	BigInteger quotient;
	BigInteger remainder(bi1);

	int lastPos = maxLength - 1;
	bool dividendNeg = false;

	if ((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
	{
		bi1 = -bi1;
		dividendNeg = true;
	}
	if ((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
		bi2 = -bi2;

	if (bi1 < bi2)
	{
		return remainder;
	}

	else
	{
		if (bi2.dataLength == 1)
			singleByteDivide(bi1, bi2, quotient, remainder);
		else
			multiByteDivide(bi1, bi2, quotient, remainder);

		if (dividendNeg)
			return -remainder;

		return remainder;
	}
}

void BigInteger::singleByteDivide( BigInteger bi1,  BigInteger bi2,
					  BigInteger &outQuotient, BigInteger &outRemainder)
{
	uint32_t result[maxLength];
	memset(result, 0, sizeof(uint32_t) * maxLength);
	int resultPos = 0;

	// copy dividend to reminder
	for (int i = 0; i < maxLength; i++)
		outRemainder.data[i] = bi1.data[i];
	outRemainder.dataLength = bi1.dataLength;

	while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0)
		outRemainder.dataLength--;

	uint64_t divisor = (uint64_t)bi2.data[0];
	int pos = outRemainder.dataLength - 1;
	uint64_t dividend = (uint64_t)outRemainder.data[pos];

	//Console.WriteLine("divisor = " + divisor + " dividend = " + dividend);
	//Console.WriteLine("divisor = " + bi2 + "\ndividend = " + bi1);

	if (dividend >= divisor)
	{
		uint64_t quotient = dividend / divisor;
		result[resultPos++] = (uint64_t)quotient;

		outRemainder.data[pos] = (uint64_t)(dividend % divisor);
	}
	pos--;

	while (pos >= 0)
	{
		//Console.WriteLine(pos);

		dividend = ((uint64_t)outRemainder.data[pos + 1] << 32) + (uint64_t)outRemainder.data[pos];
		uint64_t quotient = dividend / divisor;
		result[resultPos++] = (uint32_t)quotient;

		outRemainder.data[pos + 1] = 0;
		outRemainder.data[pos--] = (uint32_t)(dividend % divisor);
		//Console.WriteLine(">>>> " + bi1);
	}

	outQuotient.dataLength = resultPos;
	int j = 0;
	for (int i = outQuotient.dataLength - 1; i >= 0; i--, j++)
		outQuotient.data[j] = result[i];
	for (; j < maxLength; j++)
		outQuotient.data[j] = 0;

	while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0)
		outQuotient.dataLength--;

	if (outQuotient.dataLength == 0)
		outQuotient.dataLength = 1;

	while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0)
		outRemainder.dataLength--;
}

void BigInteger::multiByteDivide( BigInteger bi1, BigInteger bi2,
					 BigInteger &outQuotient, BigInteger &outRemainder)
{
	uint32_t result[maxLength];
	memset(result, 0, sizeof(uint32_t) * maxLength);
	int remainderLen = bi1.dataLength + 1;
	uint32_t *remainder = new uint32_t[remainderLen];
	memset(remainder, 0, sizeof(uint32_t) * remainderLen);

	uint32_t mask = 0x80000000;
	uint32_t val = bi2.data[bi2.dataLength - 1];
	int shift = 0, resultPos = 0;

	while (mask != 0 && (val & mask) == 0)
	{
		shift++; mask >>= 1;
	}

	for (int i = 0; i < bi1.dataLength; i++)
		remainder[i] = bi1.data[i];
	this->shiftLeft(remainder, remainderLen, shift);
	bi2 = bi2 << shift;

	int j = remainderLen - bi2.dataLength;
	int pos = remainderLen - 1;

	uint64_t firstDivisorByte = bi2.data[bi2.dataLength - 1];
	uint64_t secondDivisorByte = bi2.data[bi2.dataLength - 2];

	int divisorLen = bi2.dataLength + 1;
	uint32_t *dividendPart = new uint32_t[divisorLen];
	memset(dividendPart, 0, sizeof(uint32_t) * divisorLen);

	while (j > 0)
	{
		uint64_t dividend = ((uint64_t)remainder[pos] << 32) + (uint64_t)remainder[pos - 1];

		uint64_t q_hat = dividend / firstDivisorByte;
		uint64_t r_hat = dividend % firstDivisorByte;

		bool done = false;
		while (!done)
		{
			done = true;

			if (q_hat == 0x100000000 ||
				(q_hat * secondDivisorByte) > ((r_hat << 32) + remainder[pos - 2]))
			{
				q_hat--;
				r_hat += firstDivisorByte;

				if (r_hat < 0x100000000)
					done = false;
			}
		}

		for (int h = 0; h < divisorLen; h++)
			dividendPart[h] = remainder[pos - h];

		BigInteger kk(dividendPart, divisorLen);
		BigInteger ss = bi2 * BigInteger((int64_t)q_hat);

		while (ss > kk)
		{
			q_hat--;
			ss -= bi2;
		}
		BigInteger yy = kk - ss;

		for (int h = 0; h < divisorLen; h++)
			remainder[pos - h] = yy.data[bi2.dataLength - h];

		result[resultPos++] = (uint32_t)q_hat;

		pos--;
		j--;
	}

	outQuotient.dataLength = resultPos;
	int y = 0;
	for (int x = outQuotient.dataLength - 1; x >= 0; x--, y++)
		outQuotient.data[y] = result[x];
	for (; y < maxLength; y++)
		outQuotient.data[y] = 0;

	while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0)
		outQuotient.dataLength--;

	if (outQuotient.dataLength == 0)
		outQuotient.dataLength = 1;

	outRemainder.dataLength = this->shiftRight(remainder, remainderLen, shift);

	for (y = 0; y < outRemainder.dataLength; y++)
		outRemainder.data[y] = remainder[y];
	for (; y < maxLength; y++)
		outRemainder.data[y] = 0;

	delete []remainder;
	delete []dividendPart;
}

int BigInteger::shiftRight(uint32_t buffer[], int bufferLen,int shiftVal)
{
	int shiftAmount = 32;
	int invShift = 0;
	int bufLen = bufferLen;

	while (bufLen > 1 && buffer[bufLen - 1] == 0)
		bufLen--;

	for (int count = shiftVal; count > 0; )
	{
		if (count < shiftAmount)
		{
			shiftAmount = count;
			invShift = 32 - shiftAmount;
		}

		uint64_t carry = 0;
		for (int i = bufLen - 1; i >= 0; i--)
		{
			uint64_t val = ((uint64_t)buffer[i]) >> shiftAmount;
			val |= carry;

			carry = ((uint64_t)buffer[i]) << invShift;
			buffer[i] = (uint32_t)(val);
		}

		count -= shiftAmount;
	}

	while (bufLen > 1 && buffer[bufLen - 1] == 0)
		bufLen--;

	return bufLen;
}

BigInteger BigInteger::operator <<(int shiftVal)
{
	BigInteger result(*this);
	result.dataLength = shiftLeft(result.data, maxLength, shiftVal);

	return result;
}

int BigInteger::shiftLeft(uint32_t buffer[], int bufferLen, int shiftVal)
{
	int shiftAmount = 32;
	int bufLen = bufferLen;

	while (bufLen > 1 && buffer[bufLen - 1] == 0)
		bufLen--;

	for (int count = shiftVal; count > 0; )
	{
		if (count < shiftAmount)
			shiftAmount = count;

		uint64_t carry = 0;
		for (int i = 0; i < bufLen; i++)
		{
			uint64_t val = ((uint64_t)buffer[i]) << shiftAmount;
			val |= carry;

			buffer[i] = (uint32_t)(val & 0xFFFFFFFF);
			carry = val >> 32;
		}

		if (carry != 0)
		{
			if (bufLen + 1 <= bufferLen)
			{
				buffer[bufLen] = (uint32_t)carry;
				bufLen++;
			}
		}
		count -= shiftAmount;
	}
	return bufLen;
}

bool BigInteger::operator <(const BigInteger& bi2)
{
	BigInteger bi1(*this);
	int pos = maxLength - 1;

	// bi1 is negative, bi2 is positive
	if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
		return true;

	// bi1 is positive, bi2 is negative
	else if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
		return false;

	// same sign
	int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;
	for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--) ;

	if (pos >= 0)
	{
		if (bi1.data[pos] < bi2.data[pos])
			return true;
		return false;
	}
	return false;
}

BigInteger BigInteger::operator +=(const BigInteger& bi2)
{
	*this = *this + bi2;
	return *this;
}

BigInteger BigInteger::operator /( BigInteger bi2)
{
	BigInteger bi1(*this);
	BigInteger quotient;
	BigInteger remainder;

	int lastPos = maxLength - 1;
	bool divisorNeg = false, dividendNeg = false;

	if ((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
	{
		bi1 = -bi1;
		dividendNeg = true;
	}
	if ((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
	{
		bi2 = -bi2;
		divisorNeg = true;
	}

	if (bi1 < bi2)
	{
		return quotient;
	}

	else
	{
		if (bi2.dataLength == 1)
			singleByteDivide(bi1, bi2, quotient, remainder);
		else
			multiByteDivide(bi1, bi2, quotient, remainder);

		if (dividendNeg != divisorNeg)
			return -quotient;

		return quotient;
	}
}

BigInteger BigInteger::operator -=(const BigInteger& bi2)
{
	*this = *this - bi2;
	return *this;
}

BigInteger BigInteger::operator -(const BigInteger& bi2)
{
	BigInteger bi1(*this);
	BigInteger result;

	result.dataLength = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

	int64_t carryIn = 0;
	for (int i = 0; i < result.dataLength; i++)
	{
		int64_t diff;

		diff = (int64_t)bi1.data[i] - (int64_t)bi2.data[i] - carryIn;
		result.data[i] = (uint32_t)(diff & 0xFFFFFFFF);

		if (diff < 0)
			carryIn = 1;
		else
			carryIn = 0;
	}

	// roll over to negative
	if (carryIn != 0)
	{
		for (int i = result.dataLength; i < maxLength; i++)
			result.data[i] = 0xFFFFFFFF;
		result.dataLength = maxLength;
	}

	// fixed in v1.03 to give correct datalength for a - (-b)
	while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
		result.dataLength--;

	// overflow check

	int lastPos = maxLength - 1;
	if ((bi1.data[lastPos] & 0x80000000) != (bi2.data[lastPos] & 0x80000000) &&
		(result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
	{
		assert(false);
	}

	return result;
}

BigInteger BigInteger::operator -(int n)
{
	BigInteger bi2((int64_t)n);
	return (*this)-bi2;
}


string BigInteger::DecToHex(uint32_t value, const string& format)
{
	string HexStr;
	int a[100]; 
	int i = 0; 
	int m = 0;
	int mod = 0; 
	char hex[16]={'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};
	while(value > 0) 
	{ 
		mod = value % 16; 
		a[i++] = mod; 
		value = value/16; 

	} 

	for(i = i - 1; i >= 0; i--)
	{ 
		m=a[i];
		HexStr.push_back(hex[m]);
	} 

	while (format == string("X8") && HexStr.size() < 8)
	{
		HexStr = "0" + HexStr;
	}

	return HexStr;
}

string BigInteger::ToHexString()
{
	string result = DecToHex(data[dataLength - 1], string("X"));

	for (int i = dataLength - 2; i >= 0; i--)
	{
		result += DecToHex(data[i], string("X8"));
	}

	return result;
}

BigInteger BigInteger::genCoPrime(int bits)
{
	bool done = false;
	BigInteger result;

	int i = 0;
	while(!done)
	{
		result.genRandomBits(bits);
		i++;
		// gcd test
		BigInteger g = result.gcd(*this);
		if(g.dataLength == 1 && g.data[0] == 1)
			done = true;
	}

	return result;
}

BigInteger BigInteger::gcd(BigInteger bi)
{
	BigInteger x;
	BigInteger y;

	if((data[maxLength-1] & 0x80000000) != 0)     // negative
		x = -(*this);
	else
		x = (*this);

	if((bi.data[maxLength-1] & 0x80000000) != 0)     // negative
		y = -bi;
	else
		y = bi;

	BigInteger g = y;

	while(x.dataLength > 1 || (x.dataLength == 1 && x.data[0] != 0))
	{
		g = x;
		x = y % x;
		y = g;
	}

	return g;
}

void BigInteger::genRandomBits(int bits)
{
	int dwords = bits >> 5;
	int remBits = bits & 0x1F;

	if(remBits != 0)
		dwords++;

	if(dwords > maxLength)
		assert(false&&"Number of required bits > maxLength.");

	for(int i = 0; i < dwords; i++)
		data[i] = (uint32_t)(global_random::instance().rand_01() * 0x100000000);

	//test
	//data[0] = 2975304701;
	//data[1] = 2135302146;
	//data[2] = 2740359507;
	//data[3] = 3077254231;
	//data[4] = 3062544525;
	//data[5] = 649999918;
	//data[6] = 2727774185;
	//data[7] = 2911333499;
	//data[8] = 370452102;
	//data[9] = 2538281931;
	//data[10] = 3087643749;
	//data[11] = 474661970;
	//data[12] = 2037983128;
	//data[13] = 3885114261;
	//data[14] = 2302314343;
	//data[15] = 3548398705;

	for(int i = dwords; i < maxLength; i++)
		data[i] = 0;

	if(remBits != 0)
	{
		uint32_t mask = (uint32_t)(0x01 << (remBits-1));
		data[dwords-1] |= mask;

		mask = (uint32_t)(0xFFFFFFFF >> (32 - remBits));
		data[dwords-1] &= mask;
	}
	else
		data[dwords-1] |= 0x80000000;

	dataLength = dwords;

	if(dataLength == 0)
		dataLength = 1;
}

BigInteger BigInteger::modInverse(const BigInteger& modulus)
{
        BigInteger p[] = { (int64_t)0, (int64_t)1 };
        BigInteger q[2];    // quotients
        BigInteger r[] = { (int64_t)0, (int64_t)0 };             // remainders

        int step = 0;

        BigInteger a = modulus;
        BigInteger b = *this;

        while(b.dataLength > 1 || (b.dataLength == 1 && b.data[0] != 0))
        {
                BigInteger quotient;
                BigInteger remainder;

                if(step > 1)
                {
                        BigInteger pval = (p[0] - (p[1] * q[0])) % modulus;
                        p[0] = p[1];
                        p[1] = pval;
                }

                if(b.dataLength == 1)
                        singleByteDivide(a, b, quotient, remainder);
                else
                        multiByteDivide(a, b, quotient, remainder);

                /*
                Console.WriteLine(quotient.dataLength);
                Console.WriteLine("{0} = {1}({2}) + {3}  p = {4}", a.ToString(10),
                                    b.ToString(10), quotient.ToString(10), remainder.ToString(10),
                                    p[1].ToString(10));
                */

                q[0] = q[1];
                r[0] = r[1];
                q[1] = quotient; r[1] = remainder;

                a = b;
                b = remainder;

                step++;
        }

        if(r[0].dataLength > 1 || (r[0].dataLength == 1 && r[0].data[0] != 1))
                assert(false &&"No inverse!");

        BigInteger result = ((p[0] - (p[1] * q[0])) % modulus);

        if((result.data[maxLength - 1] & 0x80000000) != 0)
                result += modulus;  // get the least positive modulus

        return result;
}