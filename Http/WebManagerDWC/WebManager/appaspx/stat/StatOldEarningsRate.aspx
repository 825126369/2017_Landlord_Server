<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommon.master" AutoEventWireup="true" CodeBehind="StatOldEarningsRate.aspx.cs" Inherits="WebManager.appaspx.stat.StatOldEarningsRate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="stat_comm_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="stat_common" runat="server">
    <h2>重置前盈利率</h2>    
    <p>
        游戏:<asp:DropDownList ID="m_game" runat="server" style="width:130px;height:30px"></asp:DropDownList>
    </p>
    <asp:Button ID="Button1" runat="server" onclick="onQuery" Text="查询" style="width:100px;height:30px" />
    <asp:Table ID="m_result" runat="server" CssClass="result">
    </asp:Table>
</asp:Content>
