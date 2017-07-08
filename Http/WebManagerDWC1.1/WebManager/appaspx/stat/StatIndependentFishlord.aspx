<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonFishlord.master" AutoEventWireup="true" CodeBehind="StatIndependentFishlord.aspx.cs" Inherits="WebManager.appaspx.stat.StatIndependentFishlord" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonFishlord_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonFishlord_Content" runat="server">
    <h2>独立数据-捕鱼</h2>    
    <asp:Button ID="Button1" runat="server" onclick="onStat" Text="统计" style="width:100px;height:30px" />
    <asp:Table ID="m_result" runat="server" CssClass="result">
    </asp:Table>
</asp:Content>
