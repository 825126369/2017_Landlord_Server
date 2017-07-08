<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/td/TdCommon.master" AutoEventWireup="true" CodeBehind="TdDragonBallDaily.aspx.cs" Inherits="WebManager.appaspx.td.TdDragonBallDaily" %>
<asp:Content ID="Content1" ContentPlaceHolderID="tdHeadContent" runat="server">
    <style type="text/css">
         .cTable tr:hover td{background:#ddd;}
    </style>
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_td_common_m_time').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="td_common" runat="server">
    <div class="cSafeWidth">
        <h2>每日龙珠</h2>
        <table>
            <tr>
                <td>时间:</td>
                <td><asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px"></asp:TextBox></td>
            </tr>
            <tr>
                <td>每龙珠价值:</td>
                <td><asp:TextBox ID="m_eachValue" runat="server" style="width:400px;height:20px"></asp:TextBox></td>
                <td>小数，默认值1</td>
            </tr>
            <tr>
                <td>渠道折价:</td>
                <td><asp:TextBox ID="m_discount" runat="server" style="width:400px;height:20px"></asp:TextBox></td>
                <td>小数，默认值1</td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="Button3" runat="server" onclick="onQuery" Text="查询" style="width:60px;height:30px" />
                    <asp:Button ID="Button1" runat="server" onclick="onExport" Text="导出Excel" style="width:100px;height:30px" />
                </td>
            </tr>
            <tr>
                <td colspan="3"><span id="m_res" style="font-size:medium;color:red" runat="server"></span></td>
            </tr>
        </table>
    </div>
    <asp:Table ID="m_result" runat="server" CssClass="cTable"></asp:Table>
    <br />
    <span id="m_page" style="text-align:center;display:block" runat="server"></span>
    <br />
    <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>
</asp:Content>
