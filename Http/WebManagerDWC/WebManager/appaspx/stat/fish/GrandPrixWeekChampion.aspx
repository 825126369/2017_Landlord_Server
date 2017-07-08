<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonFishlord.master" AutoEventWireup="true" CodeBehind="GrandPrixWeekChampion.aspx.cs" Inherits="WebManager.appaspx.stat.fish.GrandPrixWeekChampion" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonFishlord_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonFishlord_Content" runat="server">
    <div class="cSafeWidth">
        <h2 style="text-align:center;">大奖赛周冠军</h2>
        <asp:Table ID="m_result" runat="server" class="cTable">
        </asp:Table>
    </div>
</asp:Content>
