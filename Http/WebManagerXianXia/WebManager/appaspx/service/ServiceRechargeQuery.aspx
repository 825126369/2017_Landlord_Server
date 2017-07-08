<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="ServiceRechargeQuery.aspx.cs" Inherits="WebManager.appaspx.service.ServiceRechargeQuery" %>
<asp:Content ID="Content1" ContentPlaceHolderID="serviceHeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_service_common_m_time').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="service_common" runat="server">
    <h2>充值记录查询</h2>
    <asp:DropDownList ID="m_queryWay" runat="server" style="width:130px;height:30px"></asp:DropDownList>
    <asp:TextBox ID="m_param" runat="server" style="width:300px;height:30px" ></asp:TextBox>
    平台:<asp:DropDownList ID="m_platform" runat="server" style="width:110px;height:30px"></asp:DropDownList>
    充值结果:<asp:DropDownList ID="m_rechargeResult" runat="server" style="width:130px;height:30px"></asp:DropDownList>
    <%--  游戏服务器:<asp:DropDownList ID="m_gameServer" runat="server" style="width:130px;height:30px"></asp:DropDownList> --%>
    充值范围:<asp:TextBox ID="m_range" runat="server" style="width:180px;height:30px" ></asp:TextBox>
    <br/>
    时间:&nbsp;&nbsp;<asp:TextBox ID="m_time" runat="server" style="width:300px;height:30px"></asp:TextBox>
    <asp:Button ID="Button1" runat="server" onclick="onQueryRecharge" Text="查询" style="width:100px;height:30px" />
    <asp:Button ID="Button2" runat="server" onclick="onStatRecharge"  Text="统计" style="width:100px;height:30px" />
    <%--<asp:Button ID="Button3" runat="server" onclick="onExport"  Text="导出Excel" style="width:100px;height:30px" />  --%>
    <%-- <asp:Button ID="Button4" runat="server" onclick="onSameOrder"  Text="统计相同的订单" style="width:100px;height:30px" /> --%>
    <asp:Table ID="m_result" runat="server" CssClass="result">
    </asp:Table>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    <br />
    <br />
    <span id="m_page" style="text-align:center;display:block" runat="server"></span>
    <br />
    <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>
</asp:Content>
