<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonCalfRoping.master" AutoEventWireup="true" CodeBehind="CalfRopingIndependent.aspx.cs" Inherits="WebManager.appaspx.stat.calfroping.CalfRopingIndependent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonCalfRoping_Content" runat="server">
    <div class="cSafeWidth">
        <h2>独立数据-套牛</h2>    
        <asp:Button ID="Button1" runat="server" onclick="onStat" Text="统计" style="width:100px;height:30px" />
        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>
    </div>
</asp:Content>
