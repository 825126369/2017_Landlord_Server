<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DailyRecharge.aspx.cs" Inherits="WebManager.appaspx.DailyRecharge" %>

<%@ Register Src="~/ascx/ViewResult.ascx" TagName="ViewResult" TagPrefix="myctrl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../ascx/ViewResult.css" rel="stylesheet" />

    <script type="text/javascript" src="/script/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/script/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/script/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/script/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/script/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#masterMainContent_txtTime').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="masterMainContent" runat="server">
    <div class="cSafeWidth">
        <h2>每日充值总额</h2>
        时间:<asp:TextBox ID="txtTime" runat="server" style="width:400px;height:20px"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" onclick="onQuery" Text="查询" style="width:100px;height:30px" />
        <span id="m_opRes" style="font-size:medium;color:red" runat="server"></span>
    
        <myctrl:ViewResult ID="viewResult" runat="server"></myctrl:ViewResult>
    </div>
</asp:Content>
