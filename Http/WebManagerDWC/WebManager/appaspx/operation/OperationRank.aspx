<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationRank.aspx.cs" Inherits="WebManager.appaspx.operation.OperationRank" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_operation_common_m_time').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <h2>金币增长排行</h2>
    时间:<asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px"></asp:TextBox>
    <asp:Button ID="Button3" runat="server" onclick="onSearch" Text="查询" style="width:100px;height:30px" />
   <%--  <asp:Button ID="Button1" runat="server" onclick="onClearRank" Text="清空排行" style="width:100px;height:30px" 
         OnClientClick="return confirm('确认清空排行榜?')"/> --%>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>

    <asp:Table ID="m_result" runat="server" CssClass="result">
    </asp:Table>
</asp:Content>
