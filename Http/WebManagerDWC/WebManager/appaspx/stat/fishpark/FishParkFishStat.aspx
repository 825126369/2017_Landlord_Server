<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonFishPark.master" AutoEventWireup="true" CodeBehind="FishParkFishStat.aspx.cs" Inherits="WebManager.appaspx.stat.fishpark.FishParkFishStat" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonFishPark_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonFishPark_Content" runat="server">
    <div class="cSafeWidth">
        <h2>鳄鱼公园鱼的统计</h2>
        房间:&nbsp;&nbsp;<asp:DropDownList ID="m_room" runat="server" class="cDropDownList"></asp:DropDownList>
        <asp:Button ID="Button2" runat="server" Text="查询" onclick="onQuery" class="cButton"/>
        <asp:Button ID="Button1" runat="server" Text="清空鱼表" onclick="onClearFishTable" class="cButton"/>

        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>
    </div>
</asp:Content>
