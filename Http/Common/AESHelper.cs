using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;


public class AESHelper
{
    //默认密钥向量   
    private static byte[] _key1 = { 0x12, 0x3c, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x66, 0x10, 0xCD, 0xEF };

    /// <summary>  
    /// AES加密算法  
    /// </summary>  
    /// <param name="plainText">明文字符串</param>  
    /// <param name="strKey">密钥</param>  
    /// <returns>返回加密后的密文字节数组</returns>  
    public static string AESEncrypt(string plainText, string strKey)
    {
        byte[] encrypted;
        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.Default.GetBytes(strKey);
            aesAlg.IV = _key1;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {

                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }


        // Return the encrypted bytes from the memory stream.
        return Convert.ToBase64String(encrypted);
    }

    /// <summary>  
    /// AES解密  
    /// </summary>  
    /// <param name="cipherText">密文字节数组</param>  
    /// <param name="strKey">密钥</param>  
    /// <returns>返回解密后的字符串</returns>  
    public static string AESDecrypt(string cipherText, string strKey)
    {
        string plaintext = null;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.Default.GetBytes(strKey);
            aesAlg.IV = _key1;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting                         stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

        }

        return plaintext;
    }


    public static string MD5Encrypt(string strPwd)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] data = Encoding.Default.GetBytes(strPwd);//将字符编码为一个字节序列 
        byte[] md5data = md5.ComputeHash(data);//计算data字节数组的哈希值 
        md5.Clear();
        string str = "";
        for (int i = 0; i < md5data.Length; i++)
        {
            str += md5data[i].ToString("X").PadLeft(2, '0');
        }
        return str;
    } 
} 

