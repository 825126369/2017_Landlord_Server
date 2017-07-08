<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StatOldEarningsRate.aspx.cs" Inherits="WebManager.appaspx.stat.StatOldEarningsRate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>重置前盈利率</h2>    
    <p>
        游戏:<asp:DropDownList ID="m_game" runat="server" style="width:130px;height:30px"></asp:DropDownList>
    </p>
    <asp:Button ID="Button1" runat="server" onclick="onQuery" Text="查询" style="width:100px;height:30px" />
    <asp:Table ID="m_result" runat="server" CssClass="cTable">
    </asp:Table>
</asp:Content>
