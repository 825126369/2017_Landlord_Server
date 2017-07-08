using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using Newtonsoft.Json;

/// <summary>
/// 解析JSON，仿Javascript风格
/// </summary>
public static class JsonHelper
{

    public static T ParseFromStr<T>(string jsonString)
    {
        return JsonConvert.DeserializeObject<T>(jsonString);
    }

    public static string ConvertToStr(object jsonObject)
    {
        return JsonConvert.SerializeObject(jsonObject);
    }
}

