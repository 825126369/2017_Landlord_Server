<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountScoreOpLog.aspx.cs" Inherits="WebManager.appaspx.account.AccountScoreOpLog" %>
<%@Register TagPrefix="DateSpan" TagName="WithTime" Src="~/ascx/CommonDateSpan.ascx"%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
   <%--  <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script> --%>
    <script src="../../Scripts/module/browser.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script> 
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery.datetimepicker.full.js"></script>

	<link rel="stylesheet" href="/Scripts/DateRange/css/jquery.datetimepicker.css" type="text/css" />
   	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />

    <%-- <script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />

	<script type="text/javascript">
	    $(function () {
	        $('#m_time').daterangepicker({ arrows: false });
	    });
	</script> --%>
    <style type="text/css">
        .cSafeWidth td{padding:5px;}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>点数记录</h2>

        <table border="0" cellspacing="0" cellpadding="0">
        	<tr>
        		<td style="width:100px;">时间:</td>
                 <td><DateSpan:WithTime runat="server"  ID="searchDateSpan" ClientIDMode="Static" /></td>

                <td style="width:410px;"><asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px" Visible="false" ClientIDMode="Static"></asp:TextBox></td>
                <%-- <td> 
                    <asp:Button ID="Button1" runat="server" Text="删除记录" CssClass="cButton" onclick="onDelRecord"
                        OnClientClick="return confirm('删除后记录不可恢复,是否确定删除?')" />
                    删除操作 以时间 ，操作账号，目标账号作为条件,<br />删除某个时间点之前的记录，删除后记录不可恢复。
                </td>--%>
        	</tr>
            <tr>
                <td>操作结果:</td>
                <td>
                    <asp:DropDownList ID="m_orderResult" runat="server" class="form-control"></asp:DropDownList>
                </td>
            </tr>
            <tr>
        		<td>操作账号:</td>
                <td><asp:TextBox ID="m_opAcc" runat="server" style="width:400px;height:20px" ClientIDMode="Static"></asp:TextBox></td>
        	</tr>
             <tr>
        		<td>目标账号:</td>
                <td><asp:TextBox ID="m_dstAcc" runat="server" style="width:400px;height:20px" ClientIDMode="Static"></asp:TextBox></td>
                 <td> 
                    <asp:Button ID="Button2" runat="server" Text="查询" CssClass="btn btn-primary" onclick="onQueryRecord"/>
                 </td>
                 <td>
                    <asp:Button ID="Button1" runat="server" Text="导出Excel" CssClass="btn btn-success" onclick="onExport" style="margin-left:20px;"/>
        		 </td>
        	</tr>
        </table>
    </div>

    <asp:Table ID="m_result" runat="server" CssClass="table table-hover table-bordered" ViewStateMode="Disabled">
    </asp:Table>

    <br />
    <span id="m_page" style="text-align:center;display:block" runat="server"></span>
    <br />
    <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>
</asp:Content>
