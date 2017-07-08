<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonFishlord.master" AutoEventWireup="true" CodeBehind="GrandPrixMatchDay.aspx.cs" Inherits="WebManager.appaspx.stat.fish.GrandPrixMatchDay" %>
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
    <style type="text/css">
        td{padding:2px;}
        td input[type=submit]{width:100px;height:30px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonFishlord_Content" runat="server">
    <div class="cSafeWidth">
        <h2 style="text-align:center;">大奖赛名次</h2>
        <table>
            <tr>
                <td>时间:</td>
                <td><asp:TextBox ID="m_time" runat="server" CssClass="cTextBox" ></asp:TextBox></td>
            </tr>
            <tr>
                <td>玩家ID:</td>           
                <td><asp:TextBox ID="m_playerId" runat="server" CssClass="cTextBox"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnQuery" runat="server" Text="查询" CssClass="cButton" OnClick="onQueryPlayer"/>
                </td>
                <td>
                    <asp:Button ID="btnQueryTop100" runat="server" Text="查询前100名" CssClass="cButton" OnClick="onQueryTop100"/>
                </td>
            </tr>
        </table>

        <asp:Table ID="m_result" runat="server" CssClass="cTable"></asp:Table>
    </div>
</asp:Content>
