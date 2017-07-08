<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="ServiceMail.aspx.cs" Inherits="WebManager.appaspx.service.ServiceMail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="serviceHeadContent" runat="server">
     <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_service_common_m_logOutTime').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="service_common" runat="server">
    <h2>发邮件</h2>
    <p>目标玩家ID，可以输入多个，以空格相隔</p>
    <p>道具列表输入格式: 道具ID + 空格 + 道具数量。若需要输入多个，以;号相隔。如 10001 1;10002 2;10003 3</p>
    发放方式:&nbsp;&nbsp;<asp:DropDownList ID="m_target" runat="server" style="width:200px;height:30px"></asp:DropDownList>
    &nbsp;&nbsp;<asp:CheckBox ID="m_chk" runat="server" Text="缓存邮件，以待检查" style="width:200px;height:30px"></asp:CheckBox>
    <br /><br />
    标题：<asp:TextBox ID="m_title" runat="server" style="width:300px;height:20px"></asp:TextBox>
    <br />
    发送者：<asp:TextBox ID="m_sender" runat="server" style="width:300px;height:20px;margin-top:10px"></asp:TextBox>
    <br />
    邮件内容：<asp:TextBox ID="m_content" runat="server" style="width:300px;height:20px;margin-top:10px"></asp:TextBox>
    <br />
    目标玩家ID：<asp:TextBox ID="m_toPlayer" runat="server" style="width:800px;height:20px;margin-top:10px"></asp:TextBox>
    <br />
    道具列表：<asp:TextBox ID="m_itemList" runat="server" style="width:800px;height:20px;margin-top:10px"></asp:TextBox>
    <br />
    有效时间(天，默认7天)：<asp:TextBox ID="m_validDay" runat="server" style="width:300px;height:20px;margin-top:10px"></asp:TextBox>
    <br />
    全服发放条件--下线时间区间：<asp:TextBox ID="m_logOutTime" runat="server" style="width:350px;height:20px;margin-top:10px"></asp:TextBox>
    <br />
    全服发放条件--VIP等级区间(如3 10)：<asp:TextBox ID="m_vipLevel" runat="server" style="width:300px;height:20px;margin-top:10px"></asp:TextBox> 
    <br />
    <asp:Button ID="btnSend" runat="server" onclick="onSendMail" Text="发送邮件" style="width:133px;height:25px;margin-top:10px" />
    <br />
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
