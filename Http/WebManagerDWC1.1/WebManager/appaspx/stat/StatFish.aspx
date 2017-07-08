<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonFishlord.master" AutoEventWireup="true" CodeBehind="StatFish.aspx.cs" Inherits="WebManager.appaspx.stat.StatFish" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonFishlord_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonFishlord_Content" runat="server">
     <h2>鱼统计</h2>
     <asp:Button ID="Button1" runat="server" Text="清空鱼表" onclick="onClearFishTable" style="width:125px;height:25px"/>
     <asp:Table ID="m_result" runat="server" CssClass="result">
     </asp:Table>
</asp:Content>
