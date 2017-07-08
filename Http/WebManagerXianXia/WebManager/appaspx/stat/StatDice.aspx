<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StatDice.aspx.cs" Inherits="WebManager.appaspx.stat.StatDice" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>欢乐骰宝盈利率</h2>
    <asp:Table ID="m_expRateTable" runat="server" CssClass="cTable">
    </asp:Table>
    <asp:Button ID="Button1" runat="server" Text="重置" onclick="onReset" style="width:125px;height:25px"/>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
