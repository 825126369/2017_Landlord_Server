<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OperationMoneyQuery.aspx.cs" Inherits="WebManager.appaspx.operation.OperationMoneyQuery" %>
<%@Register TagPrefix="DateSpan" TagName="WithTime" Src="~/ascx/CommonDateSpan.ascx"%>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">

   <%--  <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>--%>
    <script src="../../Scripts/module/browser.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery.datetimepicker.full.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/jquery.datetimepicker.css" type="text/css" />
    <%-- <script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" /> --%>
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />

    <%-- <script type="text/javascript">
	    $(function ()
	    {
	        $('#__time__').daterangepicker({ arrows: false });
	    });
	</script> --%>

    <style type="text/css">
        .cSafeWidth td{padding:5px;}
        .cSafeWidth select{
            width:180px;height:30px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div  class="cSafeWidth">
    <h2>客户交易</h2>
    <table>
        <tr>
            <td>
                查询方式:
            </td>
            <td>
                <asp:DropDownList ID="m_queryWay" runat="server"></asp:DropDownList>
                查询参数:
                <asp:TextBox ID="m_param" runat="server" style="width:180px;height:20px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>所在游戏:</td>
            <td>
                <asp:DropDownList ID="m_whichGame" runat="server"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>变化原因:</td>
            <td>
                <asp:DropDownList ID="m_filter" runat="server" style="width:180px;height:30px"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>时间:</td>
            <td>
                <DateSpan:WithTime runat="server"  ID="searchDateSpan" />

            </td>
                <asp:TextBox ID="__time__" runat="server" style="width:400px;height:20px" ClientIDMode="Static" Visible="false"></asp:TextBox>
            <td>
                <asp:Button ID="Button3" runat="server" onclick="onQuery" Text="查询" class="btn btn-primary" />
            </td>
            <td>
                <asp:Button ID="Button1" runat="server" onclick="onExport" Text="导出Excel" class="btn btn-success" />
            </td>
        </tr>
    </table>

    <%-- 变化范围:&nbsp;&nbsp;<asp:TextBox ID="m_range" runat="server" style="width:180px;height:20px"></asp:TextBox>--%>
    <br />
   <%--变化原因:&nbsp;&nbsp;<asp:DropDownList ID="m_filter" runat="server" style="width:130px;height:30px"></asp:DropDownList>--%>
    <%-- 属性:&nbsp;&nbsp;<asp:DropDownList ID="m_property" runat="server" style="width:130px;height:30px"></asp:DropDownList>--%>
    </div>
  
    <div class="container-fluid">
        <asp:Table ID="m_result" runat="server" CssClass="table table-hover table-bordered">
        </asp:Table>

        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
        <br />
        <span id="m_page" style="text-align:center;display:block" runat="server"></span>
        <br />
        <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>
    </div>
</asp:Content>
