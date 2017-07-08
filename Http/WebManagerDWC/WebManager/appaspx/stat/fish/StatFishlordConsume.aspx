<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonFishlord.master" AutoEventWireup="true" CodeBehind="StatFishlordConsume.aspx.cs" Inherits="WebManager.appaspx.stat.fish.StatFishlordConsume" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonFishlord_HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_stat_common_StatCommonFishlord_Content_m_time').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonFishlord_Content" runat="server">
    <div class="cSafeWidth">
        <h2>捕鱼消耗统计</h2>
        货币类型:<asp:DropDownList ID="moneyType" runat="server" CssClass="cDropDownList"></asp:DropDownList>
        时间:<asp:TextBox ID="m_time" runat="server" CssClass="cTextBox"></asp:TextBox>
        <asp:Button ID="btnStat" runat="server" Text="统计" CssClass="cButton" OnClick="btnStat_Click"/>
    </div>
    <asp:Table ID="tabResult" runat="server" CssClass="cTable"></asp:Table>
</asp:Content>
