<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonFishlord.master" AutoEventWireup="true" CodeBehind="StatFishlordControl.aspx.cs" Inherits="WebManager.appaspx.stat.StatFishlordControl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonFishlord_HeadContent" runat="server">
    <style type="text/css">
        .Boss{margin-top:20px;border:1px solid black;padding:10px;}
    </style>
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#m_resetMidTime').daterangepicker({ arrows: false });
	        $('#m_resetHighTime').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonFishlord_Content" runat="server">
    <div class="cSafeWidth">
        <h2>经典捕鱼参数调整</h2>
        <asp:Table ID="m_expRateTable" runat="server" CssClass="cTable">
        </asp:Table>
        <p>
            期望盈利率:<asp:TextBox ID="m_expRate" runat="server" style="width:100px;height:25px"></asp:TextBox>
        </p>
        <asp:Button ID="Button3" runat="server" Text="修改期望盈利率" onclick="onModifyExpRate" style="width:125px;height:25px"/>
        <asp:Button ID="Button1" runat="server" Text="重置" onclick="onReset" style="width:125px;height:25px"/>
        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>

        <div class="Boss">
            输入中级场重置日期:<asp:TextBox ID="m_resetMidTime" runat="server" style="width:300px;height:25px" ClientIDMode="Static"></asp:TextBox>
            <br />
            输入高级场重置日期:<asp:TextBox ID="m_resetHighTime" runat="server" style="width:300px;height:25px" ClientIDMode="Static"></asp:TextBox>
            <asp:Button ID="Button2" runat="server" Text="查询" onclick="onBoss" style="width:125px;height:25px" />
            <asp:Table ID="m_bossTable1" runat="server" CssClass="cTable"></asp:Table>
        </div>
    </div>
</asp:Content>
