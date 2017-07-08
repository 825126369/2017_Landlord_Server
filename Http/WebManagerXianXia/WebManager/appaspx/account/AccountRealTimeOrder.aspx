<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountRealTimeOrder.aspx.cs" Inherits="WebManager.appaspx.account.AccountRealTimeOrder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script> 
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#m_time').daterangepicker({ arrows: false });
	    });
	</script>
    <style type="text/css">
        .cSafeWidth td{padding:5px;}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>实时订单记录</h2>

        <table border="0" cellspacing="0" cellpadding="0" width="100%">
        	<tr>
        		<td>时间:</td>
                <td><asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px" ClientIDMode="Static"></asp:TextBox></td>
        	</tr>
            <tr>
        		<td>操作账号:</td>
                <td><asp:TextBox ID="m_opAcc" runat="server" style="width:400px;height:20px" ClientIDMode="Static"></asp:TextBox></td>
        	</tr>
             <tr>
        		<td>目标账号:</td>
                <td><asp:TextBox ID="m_dstAcc" runat="server" style="width:400px;height:20px" ClientIDMode="Static"></asp:TextBox></td>
        	</tr>
            <tr>
                <td></td>
        		<td> <asp:Button ID="Button2" runat="server" Text="查询" CssClass="cButton" onclick="onQueryRecord"/></td>
        	</tr>
        </table>
    </div>

    <asp:Table ID="m_result" runat="server" CssClass="cTable">
    </asp:Table>

    <br />
    <span id="m_page" style="text-align:center;display:block" runat="server"></span>
    <br />
    <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>
</asp:Content>
