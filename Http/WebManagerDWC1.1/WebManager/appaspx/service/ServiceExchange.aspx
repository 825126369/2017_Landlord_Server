<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="ServiceExchange.aspx.cs" Inherits="WebManager.appaspx.service.ServiceExchange" %>
<asp:Content ID="Content1" ContentPlaceHolderID="serviceHeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="service_common" runat="server">
    <h2>兑换管理</h2>
    类型:&nbsp;&nbsp;<asp:DropDownList ID="m_filter" runat="server" style="width:180px;height:30px"></asp:DropDownList>
    玩家ID:&nbsp;&nbsp;<asp:TextBox ID="m_param" runat="server" style="width:180px;height:20px"></asp:TextBox>
    &nbsp;<asp:Button ID="Button3" runat="server" onclick="onSearch" Text="查询" style="width:60px;height:30px" />
    <asp:Table ID="GiftTable" runat="server" CssClass="result">
    </asp:Table>
    <br />
    <asp:Button ID="m_btnActive" runat="server" onclick="onActivateGift" Text="激活" style="width:144px;height:53px"/>
    <span id="m_page" style="text-align:center;display:block" runat="server"></span>
    <br />
    <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>
</asp:Content>
