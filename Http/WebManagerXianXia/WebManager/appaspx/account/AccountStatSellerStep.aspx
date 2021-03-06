﻿<%@ Page Title="" Language="C#" MasterPageFile="/appaspx/account/AccountCommonReport.Master" AutoEventWireup="true" CodeBehind="AccountStatSellerStep.aspx.cs" Inherits="WebManager.appaspx.account.AccountStatSellerStep" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AccountCommonReport_HeadContent" runat="server">
   <%-- <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script> --%>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script> 
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#__gmAccountCascadeStaticTime').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="AccountCommonReport_BodyContent" runat="server">
     <div class="cSafeWidth">
        <h2>交易逐级报表</h2>
        
        时间:<asp:TextBox ID="__gmAccountCascadeStaticTime" runat="server" style="width:400px;height:20px" ClientIDMode="Static"></asp:TextBox>

        <p>
            统计类型: <asp:RadioButtonList ID="m_way" runat="server" RepeatDirection="Horizontal"></asp:RadioButtonList>
        </p>

        <asp:Button ID="Button1" runat="server" Text="统计" CssClass="cButton" onclick="onStat"/>

        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>
    </div>
</asp:Content>
