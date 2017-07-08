using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class RSAHelper
{
    static byte[] pseudoPrime1 = {
                         (byte)0x85, (byte)0x84, (byte)0x64, (byte)0xFD, (byte)0x70, (byte)0x6A,
                        (byte)0x9F, (byte)0xF0, (byte)0x94, (byte)0x0C, (byte)0x3E, (byte)0x2C,
                        (byte)0x74, (byte)0x34, (byte)0x05, (byte)0xC9, (byte)0x55, (byte)0xB3,
                        (byte)0x85, (byte)0x32, (byte)0x98, (byte)0x71, (byte)0xF9, (byte)0x41,
                        (byte)0x21, (byte)0x5F, (byte)0x02, (byte)0x9E, (byte)0xEA, (byte)0x56,
                        (byte)0x8D, (byte)0x8C, (byte)0x44, (byte)0xCC, (byte)0xEE, (byte)0xEE,
                        (byte)0x3D, (byte)0x2C, (byte)0x9D, (byte)0x2C, (byte)0x12, (byte)0x41,
                        (byte)0x1E, (byte)0xF1, (byte)0xC5, (byte)0x32, (byte)0xC3, (byte)0xAA,
                        (byte)0x31, (byte)0x4A, (byte)0x52, (byte)0xD8, (byte)0xE8, (byte)0xAF,
                        (byte)0x42, (byte)0xF4, (byte)0x72, (byte)0xA1, (byte)0x2A, (byte)0x0D,
                        (byte)0x97, (byte)0xB1, (byte)0x31, (byte)0xB3,
                };

    static byte[] pseudoPrime2 = {
                         (byte)0x99, (byte)0x98, (byte)0xCA, (byte)0xB8, (byte)0x5E, (byte)0xD7,
                        (byte)0xE5, (byte)0xDC, (byte)0x28, (byte)0x5C, (byte)0x6F, (byte)0x0E,
                        (byte)0x15, (byte)0x09, (byte)0x59, (byte)0x6E, (byte)0x84, (byte)0xF3,
                        (byte)0x81, (byte)0xCD, (byte)0xDE, (byte)0x42, (byte)0xDC, (byte)0x93,
                        (byte)0xC2, (byte)0x7A, (byte)0x62, (byte)0xAC, (byte)0x6C, (byte)0xAF,
                        (byte)0xDE, (byte)0x74, (byte)0xE3, (byte)0xCB, (byte)0x60, (byte)0x20,
                        (byte)0x38, (byte)0x9C, (byte)0x21, (byte)0xC3, (byte)0xDC, (byte)0xC8,
                        (byte)0xA2, (byte)0x4D, (byte)0xC6, (byte)0x2A, (byte)0x35, (byte)0x7F,
                        (byte)0xF3, (byte)0xA9, (byte)0xE8, (byte)0x1D, (byte)0x7B, (byte)0x2C,
                        (byte)0x78, (byte)0xFA, (byte)0xB8, (byte)0x02, (byte)0x55, (byte)0x80,
                        (byte)0x9B, (byte)0xC2, (byte)0xA5, (byte)0xCB,
                };

    public RSAHelper()
    {       
        BigInteger bi_p = new BigInteger(pseudoPrime1, 64);
        BigInteger bi_q = new BigInteger(pseudoPrime2, 64);
        m_pq = (bi_p - 1) * (bi_q - 1);
        m_n = bi_p * bi_q;        
    }

    BigInteger m_n;
    BigInteger m_e;
    BigInteger m_d;
    BigInteger m_pq;

    public void init(int keysize = 512)
    {
        Random rand = new Random();
        m_e = m_pq.genCoPrime(keysize, rand);
        m_d = m_e.modInverse(m_pq);
    }

    public string RSAEncryptStr(string str)
    {
        string temp = StrToHex(str);
        BigInteger b_data = new BigInteger(temp, 16);
        b_data = b_data.modPow(m_e, m_n);
        return Convert.ToBase64String(Encoding.Default.GetBytes(b_data.ToHexString()));
    }

    public string RSADecryptStr(string str)
    {
        string temp = Encoding.Default.GetString(Convert.FromBase64String(str));
        BigInteger b_data = new BigInteger(temp, 16);
        b_data = b_data.modPow(m_d, m_n);
        return HexToStr(b_data.ToHexString());
    }

    public string getModulus()
    {
        return Convert.ToBase64String(Encoding.Default.GetBytes(m_e.ToHexString()));
    }

    public void setModulus(string str)
    {
        string temp = Encoding.Default.GetString(Convert.FromBase64String(str));
        m_e = new BigInteger(temp, 16);
    }

    public static string StrToHex(string value)
    {
        byte[] bs = System.Text.Encoding.Default.GetBytes(value);
        string temp = "";
        for (int i = 0; i < bs.Length; i++)
        {
            temp += Convert.ToString(bs[i], 16);
        }
        return temp;
    }

    public static string HexToStr(string value)
    {
        string result = string.Empty;
        if (value.Length % 2 != 0)
            value = "0" + value;

        byte[] arrByte = new byte[value.Length / 2];
        int index = 0;
        for (int i = 0; i < value.Length; i += 2)
        {
            arrByte[index++] = Convert.ToByte(value.Substring(i, 2), 16);   
        }
        result = System.Text.Encoding.Default.GetString(arrByte);

        return result;
    }
}

