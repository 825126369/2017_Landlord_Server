<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountStatPlayer.aspx.cs" Inherits="WebManager.appaspx.account.AccountStatPlayer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  <%--  <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>  --%>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_m_time').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>玩家进出游戏统计</h2>
        
        时间:&nbsp;&nbsp;<asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px"></asp:TextBox>

        <div>
            币种: <asp:DropDownList ID="m_moneyType" runat="server" CssClass="cDropDownList"></asp:DropDownList>
        </div>

        <div>
            统计类型: <asp:RadioButtonList ID="m_way" runat="server" RepeatDirection="Horizontal"></asp:RadioButtonList>
        </div>

        <asp:Button ID="Button1" runat="server" Text="统计" CssClass="cButton" onclick="onStat"/>

        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>
    </div>
</asp:Content>
