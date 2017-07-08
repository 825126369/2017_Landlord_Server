<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountWinLoseReport.aspx.cs" Inherits="WebManager.appaspx.account.AccountWinLoseReport" %>
<%@Register TagPrefix="DateSpan" TagName="WithTime" Src="~/ascx/CommonDateSpan.ascx"%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
   <%--  <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script> 

    <script type="text/javascript" src="/Scripts/DateRange/js/jquery.js"></script> --%>
    <script src="../../../Scripts/module/browser.js"></script> 

	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery.datetimepicker.full.js"></script>

	<link rel="stylesheet" href="/Scripts/DateRange/css/jquery.datetimepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />

    <style type="text/css">
        .cSafeWidth td{padding:5px;}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>输赢报表</h2>

        <table border="0" cellspacing="0" cellpadding="0" width="100%">
        	<tr style="height:20px" >
        		<td>日期:</td>
                <td>
                    <DateSpan:WithTime runat="server"  ID="searchDateSpan" ClientIDMode="Static" />
                </td>
        	</tr>
                <asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px" ClientIDMode="Static" Visible="False"></asp:TextBox>
            <tr style="height:20px" >
        		<td>会员账号:</td><td><asp:TextBox ID="m_acc" runat="server" style="width:400px;height:20px" ClientIDMode="Static"></asp:TextBox></td>
        	</tr>
            <tr style="height:20px" >
        		<td> <asp:Button ID="Button2" runat="server" Text="查询" CssClass="cButton" onclick="onQuery"/></td>
        	</tr>
        </table>

        <div style="padding-top:10px;">
            <span id="m_levelStr" style="font-size:medium;color:black;font-weight:bold" runat="server"></span>
        </div>

        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>

        <br />
        <div style="padding-top:10px;">
            <span id="m_info" style="font-size:medium;color:black;font-weight:bold" runat="server"></span>
        </div>
        <asp:Table ID="m_detailResult" runat="server" CssClass="cTable">
        </asp:Table>
    </div>
</asp:Content>
