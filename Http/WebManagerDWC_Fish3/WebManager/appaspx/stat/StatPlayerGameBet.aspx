<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommon.master" AutoEventWireup="true" CodeBehind="StatPlayerGameBet.aspx.cs" Inherits="WebManager.appaspx.stat.StatPlayerGameBet" %>
<asp:Content ID="Content1" ContentPlaceHolderID="stat_comm_HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
    <script src="../../Scripts/module/sea.js" type="text/javascript"></script>
	<script type="text/javascript">
	    $(function () {
	        $('#txtTime').daterangepicker({ arrows: false });
	    });
	    seajs.use("../../Scripts/stat/PlayerGameBet.js");
    </script>
    <style type="text/css">
         .cTable tr:hover td{background:#ddd;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="stat_common" runat="server">
    <div class="cSafeWidth">
        <h2 style="text-align:center;">用户下注情况统计</h2>
        <table class="SearchCondition">
            <tr>
                <td>游戏:</td>
                <td><asp:DropDownList ID="m_gameList" runat="server"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>玩家ID:</td>
                <td><input type="text" id="txtPlayerId"/></td>
            </tr>
            <tr>
                <td>时间:</td>
                <td><input type="text" id="txtTime"/></td>
                <td><input type="button" value="查询" id="btnQuery"/></td>
            </tr>
        </table>
    </div>

    <table class="cTable" id="resultTable">

    </table>
</asp:Content>
