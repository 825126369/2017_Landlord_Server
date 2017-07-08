<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/td/TdCommon.master" AutoEventWireup="true" CodeBehind="TdRechargePerHour.aspx.cs" Inherits="WebManager.appaspx.td.TdRechargePerHour" %>
<asp:Content ID="Content1" ContentPlaceHolderID="tdHeadContent" runat="server">
    <script type="text/javascript" src="http://cdn.hcharts.cn/jquery/jquery-1.8.3.min.js"></script>

	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />

    <script src="http://cdn.hcharts.cn/highcharts/highcharts.js" type="text/javascript"></script>

    <script src="../../Scripts/module/sea.js" type="text/javascript"></script>
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_td_common_m_time').daterangepicker({ arrows: false });
	    });

	    seajs.use("../../Scripts/td/TdRechargePerHour.js");
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="td_common" runat="server">
    <div class="cSafeWidth">
        <h2>实时付费</h2>

        <table>
            <tr>
                <td>时间:</td>
                <td><asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px"></asp:TextBox></td>
                <td colspan="2">
                    <asp:Button ID="Button3" runat="server" onclick="onQuery" Text="查询" style="width:60px;height:30px" />
                </td>
            </tr>
        </table>
    </div>

    <asp:Table ID="m_result" runat="server" CssClass="cTable"></asp:Table>
    
    <div id="container" style="min-width: 310px; height: 400px; margin: 0 auto"></div>
</asp:Content>
