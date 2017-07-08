<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationLoginQuery.aspx.cs" Inherits="WebManager.appaspx.operation.OperationLoginQuery" %>
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
    <h2>玩家登陆历史</h2>
    账号:&nbsp;&nbsp;<asp:TextBox ID="m_param" runat="server" style="width:180px;height:20px"></asp:TextBox>
    时间:&nbsp;&nbsp;<asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px"></asp:TextBox>
    &nbsp;
    <asp:Button ID="Button3" runat="server" onclick="onQuery" Text="查询" style="width:60px;height:30px" />
    <br /><br />
    <asp:Table ID="m_result" runat="server" CssClass="result">
    </asp:Table>
    <br />
    <span id="m_page" style="text-align:center;display:block" runat="server"></span>
    <br />
    <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>
</asp:Content>
