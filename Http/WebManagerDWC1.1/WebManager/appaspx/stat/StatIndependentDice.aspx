<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonDice.master" AutoEventWireup="true" CodeBehind="StatIndependentDice.aspx.cs" Inherits="WebManager.appaspx.stat.StatIndependentDice" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonDice_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonDice_Content" runat="server">
    <h2>独立数据-骰宝</h2>    
    <asp:Button ID="Button1" runat="server" onclick="onStat" Text="统计" style="width:100px;height:30px" />
    <asp:Table ID="m_result" runat="server" CssClass="result">
    </asp:Table>
</asp:Content>
