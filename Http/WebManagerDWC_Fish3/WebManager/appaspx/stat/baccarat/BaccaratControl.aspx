<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonBaccarat.master" AutoEventWireup="true" CodeBehind="BaccaratControl.aspx.cs" Inherits="WebManager.appaspx.stat.baccarat.BaccaratControl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonBaccarat_Content" runat="server">
    <div class="cSafeWidth">
        <h2>百家乐参数调整</h2>
        <asp:Table ID="m_expRateTable" runat="server" CssClass="cTable">
        </asp:Table>
        <asp:Button ID="Button1" runat="server" Text="重置" onclick="onReset" style="width:125px;height:25px"/>
        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    </div>
</asp:Content>
