<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommon.master" AutoEventWireup="true" CodeBehind="StatGeneralPlayerLost.aspx.cs" Inherits="WebManager.appaspx.stat.StatGeneralPlayerLost" %>

<asp:Content ID="Content1" ContentPlaceHolderID="stat_comm_HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_stat_common_m_time').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="stat_common" runat="server">
    <div class="cSafeWidth">
        <h2>玩家流失查询</h2>
        <div>
            账号创建时间:<br />
            <asp:TextBox ID="m_time" runat="server" CssClass="cTextBox"></asp:TextBox>
        </div>

        <div>
            小于金币:<br />
            <asp:TextBox ID="m_gold" runat="server" CssClass="cTextBox"></asp:TextBox>
        </div>

        <asp:Button ID="btnQuery" runat="server" Text="查询" CssClass="cButton" OnClick="btnQuery_Click" />
        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
        
        <div runat="Server" id="divResult"></div>
    </div>
</asp:Content>
