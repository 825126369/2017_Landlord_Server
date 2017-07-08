<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonCows.master" AutoEventWireup="true" CodeBehind="StatIndependentCows.aspx.cs" Inherits="WebManager.appaspx.stat.cows.StatIndependentCows" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonCows_HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonCows_Content" runat="server">
    <div class="cSafeWidth">
        <h2>独立数据-牛牛</h2>    
        <asp:Button ID="Button1" runat="server" onclick="onStat" Text="统计" style="width:100px;height:30px" />
        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>
    </div>
</asp:Content>
