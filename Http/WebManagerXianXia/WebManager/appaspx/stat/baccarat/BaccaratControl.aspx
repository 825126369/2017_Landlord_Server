<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BaccaratControl.aspx.cs" Inherits="WebManager.appaspx.stat.baccarat.BaccaratControl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>百家乐参数调整</h2>
    <asp:Table ID="m_expRateTable" runat="server" CssClass="cTable">
    </asp:Table>
    <asp:Button ID="Button1" runat="server" Text="重置" onclick="onReset" style="width:125px;height:25px"/>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
