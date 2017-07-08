<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonCows.master" AutoEventWireup="true" CodeBehind="CowsPlayerBanker.aspx.cs" Inherits="WebManager.appaspx.stat.cows.CowsPlayerBanker" %>

<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonCows_HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_stat_common_StatCommonCows_Content_txtTime').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonCows_Content" runat="server">
    <div class="cSafeWidth">
        <h2>牛牛上庄查询</h2>
        玩家ID:<asp:TextBox ID="txtPlayerId" runat="server" CssClass="cTextBox"></asp:TextBox>
        时间:<asp:TextBox ID="txtTime" runat="server" CssClass="cTextBox"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="查询" onclick="onQuery" CssClass="cButton"/>

        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>

        <br />
        <span id="m_page" style="text-align:center;display:block" runat="server"></span>
        <br />
        <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>
        </div>
</asp:Content>
