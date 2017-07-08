using System;
using System.IO;
using System.Text;
using System.Web;

class LOGW
{
    public static void Info(string message)
    {
        write(message);
    }

    public static void Info(string message, params object[] args)
    {
        write(message, args);
    }

    private static void write(string msg)
    {
        string file = getfileName();
        string time = DateTime.Now.ToString(ConstDef.DATE_TIME24);
        StreamWriter sw = new StreamWriter(file, true, Encoding.Default);
        sw.WriteLine(time + " " + msg);
        sw.Close();
    }

    private static void write(string msg, params object[] args)
    {
        string file = getfileName();
        string time = DateTime.Now.ToString(ConstDef.DATE_TIME24);
        StreamWriter sw = new StreamWriter(file, true, Encoding.Default);
        sw.WriteLine(time + " " + msg, args);
        sw.Close();
    }

    private static string getfileName()
    {
        DateTime dt = DateTime.Now;
        string f = Convert.ToString(dt.Year) + "_" + Convert.ToString(dt.Month) + "_" + Convert.ToString(dt.Day) + "_" + dt.Hour + ".txt";

        string dir = HttpRuntime.BinDirectory + "..\\log\\";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        f = dir + f;
        return f;
    }
}
