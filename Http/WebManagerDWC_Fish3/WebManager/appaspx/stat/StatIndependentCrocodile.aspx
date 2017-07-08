<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonCrocodile.master" AutoEventWireup="true" CodeBehind="StatIndependentCrocodile.aspx.cs" Inherits="WebManager.appaspx.stat.StatIndependentCrocodile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonCrocodile_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonCrocodile_Content" runat="server">
    <h2>独立数据-鳄鱼</h2>    
    <asp:Button ID="Button1" runat="server" onclick="onStat" Text="统计" style="width:100px;height:30px" />
    <asp:Table ID="m_result" runat="server" CssClass="result">
    </asp:Table>
</asp:Content>
