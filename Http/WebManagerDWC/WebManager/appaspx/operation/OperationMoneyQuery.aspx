<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationMoneyQuery.aspx.cs" Inherits="WebManager.appaspx.operation.OperationMoneyQuery" %>
<asp:Content ID="Content2" ContentPlaceHolderID="operationHeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function ()
	    {
	        $('#MainContent_operation_common_m_time').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="operation_common" runat="server">
    <h2>玩家金币变化</h2>
    <p>日期格式: 2014/7/7-2014/07/7或具体某天2014/7/7</p>
    <p>默认查询所有玩家的金币变化情况</p>
    查询方式:&nbsp;&nbsp;<asp:DropDownList ID="m_queryWay" runat="server" style="width:180px;height:30px"></asp:DropDownList>
    查询参数:&nbsp;&nbsp;<asp:TextBox ID="m_param" runat="server" style="width:180px;height:20px"></asp:TextBox>
    变化范围:&nbsp;&nbsp;<asp:TextBox ID="m_range" runat="server" style="width:180px;height:20px"></asp:TextBox>
    <br />
    变化原因:&nbsp;&nbsp;<asp:DropDownList ID="m_filter" runat="server" style="width:130px;height:30px"></asp:DropDownList>
    所在游戏:&nbsp;&nbsp;<asp:DropDownList ID="m_whichGame" runat="server" style="width:130px;height:30px"></asp:DropDownList>
    属性:&nbsp;&nbsp;<asp:DropDownList ID="m_property" runat="server" style="width:130px;height:30px"></asp:DropDownList>
    时间:&nbsp;&nbsp;<asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px"></asp:TextBox>
    &nbsp;
    <asp:Button ID="Button3" runat="server" onclick="onQuery" Text="查询" style="width:60px;height:30px" />
    <asp:Button ID="Button1" runat="server" onclick="onExport" Text="导出Excel" style="width:100px;height:30px" />
    <br /><br />
    <asp:Table ID="m_result" runat="server" CssClass="cTable">
    </asp:Table>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    <br />
    <span id="m_page" style="text-align:center;display:block" runat="server"></span>
    <br />
    <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>
</asp:Content>
