<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommon.master" AutoEventWireup="true" CodeBehind="StatStarLottery.aspx.cs" Inherits="WebManager.appaspx.stat.StatStarLottery" %>
<asp:Content ID="Content1" ContentPlaceHolderID="stat_comm_HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_stat_common_m_time').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="stat_common" runat="server">
     <div class="cSafeWidth">
        <h2>星星抽奖</h2>
        时间:<asp:TextBox ID="m_time" runat="server" CssClass="cTextBox" ></asp:TextBox>
        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>
        <asp:Button ID="Button1" runat="server" Text="查询" onclick="onQuery" style="width:100px;height:25px"/>
    </div>
</asp:Content>
