<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonFishlord.master" AutoEventWireup="true" CodeBehind="StatFish.aspx.cs" Inherits="WebManager.appaspx.stat.StatFish" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonFishlord_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonFishlord_Content" runat="server">
    <div class="cSafeWidth">
        <h2>鱼统计</h2>
        房间:&nbsp;&nbsp;<asp:DropDownList ID="m_room" runat="server" class="cDropDownList"></asp:DropDownList>
        <asp:Button ID="Button2" runat="server" Text="查询" onclick="onQuery" class="cButton"/>
        <asp:Button ID="Button1" runat="server" Text="清空鱼表" onclick="onClearFishTable" class="cButton"/>

        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>
    </div>
</asp:Content>
