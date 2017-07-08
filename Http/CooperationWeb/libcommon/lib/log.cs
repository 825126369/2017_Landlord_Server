using System;
using System.Runtime.InteropServices;

public class LOG
{
    public static void Info(object message)
    {
        string str = DateTime.Now.ToString();
        System.Console.ForegroundColor = ConsoleColor.White;
        System.Console.WriteLine(str + " " + (string)message);
    }

    public static void Info(string format, params object[] args)
    {
        string str = DateTime.Now.ToString();
        System.Console.ForegroundColor = ConsoleColor.White;
        System.Console.WriteLine(str +" " + format, args);
    }

    public static void Error(object message)
    {
        string str = DateTime.Now.ToString();
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine(str + " " + (string)message);
    }

    public static void Error(string format, params object[] args)
    {
        string str = DateTime.Now.ToString();
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine(str + " " + format, args);
    }

    public static void Warn(object message)
    {
        string str = DateTime.Now.ToString();
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine(str + " " + (string)message);
    }

    public static void Warn(string format, params object[] args)
    {
        string str = DateTime.Now.ToString();
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine(str + " " + format, args);
    }

    public static void InfoOnlyConsole(object message)
    {
        string str = DateTime.Now.ToString();
        System.Console.ForegroundColor = ConsoleColor.White;
        System.Console.WriteLine(str + " " + (string)message);
    }

    public static void InfoOnlyConsole(string format, params object[] args)
    {
        string str = DateTime.Now.ToString();
        System.Console.ForegroundColor = ConsoleColor.White;
        System.Console.WriteLine(str + " " + format, args);
    }

    public static void ErrorOnlyConsole(object message)
    {
        string str = DateTime.Now.ToString();
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine(str + " " + (string)message);
    }

    public static void ErrorOnlyConsole(string format, params object[] args)
    {
        string str = DateTime.Now.ToString();
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine(str + " " + format, args);
    }

    public static void WarnOnlyConsole(object message)
    {
        string str = DateTime.Now.ToString();
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine(str + " " + (string)message);
    }

    public static void WarnOnlyConsole(string format, params object[] args)
    {
        string str = DateTime.Now.ToString();
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine(str + " " + format, args);
    }
}






