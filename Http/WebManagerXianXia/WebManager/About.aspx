<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="WebManager.About" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>时间格式说明</title>
    <style type="text/css">
        ul{
           font-family: "微软雅黑","Helvetica Neue", "Lucida Grande", "Segoe UI", Arial, Helvetica, Verdana, sans-serif;
           line-height:2em;
           letter-spacing:5px;
           font-size:16px;
        }
    </style>
</head>
<body>
    <ul>
        <li>基本时间格式，具体到某天： 如 2016/3/20 </li>
        <li>基本时间格式，具体到小时： 如 2016/3/20 15&nbsp;&nbsp; 表示2016年3月20号15点 </li>
        <li>基本时间格式，具体到分钟： 如 2016/3/20 15:30  表示2016年3月20号15点30分 </li>

        <li>查询时可以输入单个日期&nbsp;&nbsp;&nbsp;&nbsp; 如 2016/3/20&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 查询20号当天的数据 </li>
        <li>查询时可以输入上具体到小时单个时间&nbsp;&nbsp; 如 2016/3/20 15  查询20号15点到16点之间的数据</li>
        <li>查询时可以输入一个日期范围 如 2016/3/20 - 2016/3/21  查询20号到21号之间的数据</li>
        <li>可以按照基本格式组合一个范围 如 2016/3/20 15 - 2016/3/21 16:30 查询20号15点到21号16：30之间的数据</li>
    </ul>
</body>
</html>
