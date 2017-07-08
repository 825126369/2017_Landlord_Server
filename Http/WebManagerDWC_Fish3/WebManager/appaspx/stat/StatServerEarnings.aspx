<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommon.master" AutoEventWireup="true" CodeBehind="StatServerEarnings.aspx.cs" Inherits="WebManager.appaspx.stat.StatServerEarnings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="stat_comm_HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
    <script src="http://cdn.hcharts.cn/highcharts/highcharts.js" type="text/javascript"></script>
    <script src="../../Scripts/module/sea.js" type="text/javascript"></script>
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_stat_common_m_time').daterangepicker({ arrows: false });
	    });
	    seajs.use("../../Scripts/stat/StatServerEarnings.js");
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="stat_common" runat="server">
    <div class="cSafeWidth">
        <h2>游戏金币流动统计</h2>
        游戏:<asp:DropDownList ID="m_game" runat="server" style="width:130px;height:30px"></asp:DropDownList>
        时间:<asp:TextBox ID="m_time" runat="server" style="width:300px;height:30px"></asp:TextBox>
        <%-- <asp:Button ID="Button1" runat="server" onclick="onStat" Text="统计" style="width:100px;height:30px" />--%>
        <input type="button" id="statGame" value="统计" style="width:100px;height:30px" />
    </div>

    <div id="chartActive" style="max-width:1200px;min-height:400px; margin:10px auto;border:1px solid #000;border-radius:10px;padding:10px;display:none"></div>
    <div id="chartEarnValue" style="max-width:1200px;min-height:400px; margin:10px auto;border:1px solid #000;border-radius:10px;padding:10px;display:none"></div>

    <asp:Table ID="m_result" runat="server" CssClass="cTable">
    </asp:Table>
</asp:Content>
