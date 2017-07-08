using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO.Compression;
using System.Text;
using System.IO;


public class ZipHelper
{
    /// <summary>  
    /// 将传入字符串以GZip算法压缩后，返回Base64编码字符  
    /// </summary>  
    /// <param name="rawString">需要压缩的字符串</param>  
    /// <returns>压缩后的Base64编码的字符串</returns>  
    public static string GZipCompressString(string rawString)
    {
        if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
        {
            return "";
        }
        else
        {
            byte[] rawData = Encoding.Default.GetBytes(rawString.ToString());
            byte[] zippedData = Compress(rawData);
            return (string)(Convert.ToBase64String(zippedData));
        }

    }
    /// <summary>  
    /// GZip压缩  
    /// </summary>  
    /// <param name="rawData"></param>  
    /// <returns></returns>  
    public static byte[] Compress(byte[] rawData)
    {
        MemoryStream ms = new MemoryStream(rawData);
        MemoryStream outBuffer = new MemoryStream();
        GZipStream compressedzipStream = new GZipStream(outBuffer, CompressionMode.Compress);
        ms.CopyTo(compressedzipStream);
        compressedzipStream.Close();
        return outBuffer.ToArray();
    }
    /// <summary>  
    /// 将传入的二进制字符串资料以GZip算法解压缩  
    /// </summary>  
    /// <param name="zippedString">经GZip压缩后的二进制字符串</param>  
    /// <returns>原始未压缩字符串</returns>  
    public static string GZipDecompressString(string zippedString)
    {
        if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
        {
            return "";
        }
        else
        {
            byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
            return (string)(Encoding.Default.GetString(Decompress(zippedData)));
        }
    }
    /// <summary>  
    /// ZIP解压  
    /// </summary>  
    /// <param name="zippedData"></param>  
    /// <returns></returns>  
    public static byte[] Decompress(byte[] zippedData)
    {
        MemoryStream ms = new MemoryStream(zippedData);
        GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
        MemoryStream outBuffer = new MemoryStream();
        compressedzipStream.CopyTo(outBuffer);       
        compressedzipStream.Close();
        return outBuffer.ToArray();
    }
}  
