<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationGenGift.aspx.cs" Inherits="WebManager.appaspx.operation.OperationGenGift" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_operation_common_m_deadTime').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <h2>礼包生成</h2>
    <p>礼包内容填写: 道具ID + 空格 + 数量, 若填写多个，以;号相隔</p>
    礼包内容:&nbsp;&nbsp;<asp:TextBox ID="m_content" runat="server" style="width:180px;height:20px"></asp:TextBox>
    <br />
    截止日期:&nbsp;&nbsp;<asp:TextBox ID="m_deadTime" runat="server" style="width:180px;height:20px"></asp:TextBox>
    <br />
    <asp:Button ID="Button3" runat="server" onclick="onAddGift" Text="增加" style="width:100px;height:30px" />
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
