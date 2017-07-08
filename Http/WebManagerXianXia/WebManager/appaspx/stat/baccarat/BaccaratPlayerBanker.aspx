<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BaccaratPlayerBanker.aspx.cs" Inherits="WebManager.appaspx.stat.baccarat.BaccaratPlayerBanker" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
   <%-- <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script> --%>
    <script src="../../../Scripts/module/browser.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#__time__').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>上庄查询</h2>
        玩家ID:<asp:TextBox ID="txtPlayerId" runat="server" CssClass="cTextBox"></asp:TextBox>
        时间:<asp:TextBox ID="__time__" runat="server" CssClass="cTextBox" ClientIDMode="Static"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="查询" onclick="onQuery" CssClass="cButton"/>

        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>

        <br />
        <span id="m_page" style="text-align:center;display:block" runat="server"></span>
        <br />
        <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>
        </div>
</asp:Content>
