using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

public struct SqlCmdStr
{
    public const string SQL_ADD_GAME_LOG = " INSERT into {0} (acc,enterMoney,exitMoney,genTime)" +
                                           " VALUES ('{1}',{2},{3},'{4}' )";
}

public struct CC
{
    public static string MYSQL_IP = WebConfigurationManager.AppSettings["mysql"];
}

