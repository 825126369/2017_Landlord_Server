using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Configuration;
using System.Net;
using System.IO;
using System.Collections.Specialized;

public class HttpPost
{

    /// <summary>
    /// 以Post 形式提交数据到 uri
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="files"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static byte[] Post(Uri uri, IEnumerable<UploadFile> files = null, NameValueCollection values = null)
    {
        string boundary = "-----------" + DateTime.Now.Ticks.ToString("x");
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.ContentType = "multipart/form-data; boundary=" + boundary;
        request.Method = "POST";
        request.KeepAlive = true;
        request.Credentials = CredentialCache.DefaultCredentials;
        MemoryStream stream = new MemoryStream();
        byte[] line = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
        byte[] endline = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
        //提交文本字段
        if (values != null)
        {
            string format = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
            foreach (string key in values.Keys)
            {
                string s = string.Format(format, key, values[key]);
                byte[] data = Encoding.UTF8.GetBytes(s);
                stream.Write(data, 0, data.Length);
            }

            stream.Write(line, 0, line.Length);

        }

        //提交文件
        if (files != null)
        {
            string fformat = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n Content-Type: application/octet-stream\r\n\r\n";

            foreach (UploadFile file in files)
            {
                string s = string.Format(fformat, file.Name, file.Filename);
                byte[] data = Encoding.UTF8.GetBytes(s);
                stream.Write(line, 0, line.Length);
                stream.Write(data, 0, data.Length);
                stream.Write(file.Data, 0, file.Data.Length);
            }

            stream.Write(endline, 0, endline.Length);
        }

        request.ContentLength = stream.Length;
        Stream requestStream = request.GetRequestStream();
        stream.Position = 0L;
        stream.CopyTo(requestStream);
        stream.Close();
        requestStream.Close();

        using (var response = request.GetResponse())
        using (var responseStream = response.GetResponseStream())
        using (var mstream = new MemoryStream())
        {
            responseStream.CopyTo(mstream);
            return mstream.ToArray();
        }
    }
    //private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
    public static byte[] Get(Uri uri, bool usegzip = false)
    {
        byte[] bytes = null;
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";

            if (usegzip)
                request.Headers.Add("Accept-Encoding", "gzip");

            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();

            List<byte> lb = new List<byte>();
            MemoryStream ms = new MemoryStream();
            dataStream.CopyTo(ms);
            bytes = ms.ToArray();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message + uri);
        }
        return bytes;
    }

    public static byte[] PostWebRequest(string postUrl)
    {
        byte[] bytes = null;
        try
        {
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
            webReq.Method = "POST";
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.ContentLength = 0;
            
            HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
            Stream dataStream = response.GetResponseStream();
            MemoryStream ms = new MemoryStream();
            using (var mstream = new MemoryStream())
            {
                dataStream.CopyTo(mstream);
                bytes = mstream.ToArray();
                return bytes;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + postUrl);
        }
        return bytes;
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    public class UploadFile
    {
        public UploadFile()
        {
            ContentType = "application/octet-stream";
        }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }
}