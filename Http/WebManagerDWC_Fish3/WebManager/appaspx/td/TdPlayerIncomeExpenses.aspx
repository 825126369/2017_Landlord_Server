<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/td/TdCommon.master" AutoEventWireup="true" CodeBehind="TdPlayerIncomeExpenses.aspx.cs" Inherits="WebManager.appaspx.td.TdPlayerIncomeExpenses" %>
<asp:Content ID="Content1" ContentPlaceHolderID="tdHeadContent" runat="server">
    <style type="text/css">
         .cTable tr:hover td{background:#ddd;}
         .Search td{padding:6px;}
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
        <h2>活跃玩家收支统计</h2>
        <table class="Search">
            <tr>
                <td>时间:</td>
                <td><asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px"></asp:TextBox></td>
                <%-- <td><asp:Button ID="Button1" runat="server" onclick="onExport" Text="导出Excel" style="width:100px;height:30px" /></td>--%>
            </tr>
            <tr>
                <td>属性:</td>
                <td><asp:DropDownList ID="m_property" runat="server" style="width:130px;height:30px"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>玩家:</td>
                <td><asp:DropDownList ID="m_player" runat="server" style="width:130px;height:30px"></asp:DropDownList></td>
            </tr>
            <tr>
                <td><asp:Button ID="Button3" runat="server" onclick="onQuery" Text="查询" style="width:60px;height:30px" /></td>
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
