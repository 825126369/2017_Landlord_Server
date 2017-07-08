<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StatTotalConsume.aspx.cs" Inherits="WebManager.appaspx.stat.StatTotalConsume" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
   <%-- <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script> --%>
    <script src="../../Scripts/module/browser.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(document).ready(function ()
	    {
	        $('#__time__').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>消耗总计</h2>
    货币类型:<asp:DropDownList ID="m_currencyType" runat="server" style="width:200px;height:30px">
        <asp:ListItem Value="1">金币</asp:ListItem>
    <asp:ListItem Value="2">礼券</asp:ListItem>
    </asp:DropDownList>
    
    变化类型:<asp:DropDownList ID="m_changeType" runat="server" style="width:200px;height:30px"></asp:DropDownList>

    时间区间:<asp:TextBox ID="__time__" runat="server" style="width:300px;" ClientIDMode="Static"></asp:TextBox>
    <asp:Button ID="Button3" runat="server" onclick="onStat" Text="统计" style="width:60px;height:30px" />

    <asp:Table ID="m_result" runat="server" CssClass="result">
    </asp:Table>
</asp:Content>
