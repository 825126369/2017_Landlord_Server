<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="LogViewer.aspx.cs" Inherits="WebManager.appaspx.LogViewer" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
     <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_m_time').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>查看日志</h2>
    操作类型：<asp:DropDownList ID="opType" runat="server"></asp:DropDownList>
    操作时间：<asp:TextBox ID="m_time" runat="server" style="width:400px;"></asp:TextBox>
    <asp:Button ID="Button1" runat="server" Text="查询" style="width:80px;height:30px" OnClick="onSearchLog" />
    <asp:Button ID="Button2" runat="server" Text="删除所有日志" style="width:100px;height:30px" OnClick="onDelAllLog"
         OnClientClick="return confirm('确认删除所有日志?')" />

    <asp:Table ID="LogTable" runat="server" CssClass="cTable">
    </asp:Table>
    <br />
    <br />
    <span id="m_page" style="text-align:center;display:block" runat="server"></span>
    <br />
    <br />
    <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>
</asp:Content>
