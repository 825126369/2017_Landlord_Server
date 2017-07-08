<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommon.master" AutoEventWireup="true" CodeBehind="StatLobby.aspx.cs" Inherits="WebManager.appaspx.stat.StatLobby" %>
<asp:Content ID="Content1" ContentPlaceHolderID="stat_comm_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="stat_common" runat="server">
     <h2>独立数据-大厅</h2>
     统计类型<asp:DropDownList ID="m_statWay" runat="server" style="width:130px;height:30px"></asp:DropDownList>
     <asp:Button ID="Button1" runat="server" onclick="onStat" Text="统计" style="width:100px;height:30px" />
    <asp:Table ID="m_result" runat="server" CssClass="result">
    </asp:Table>
</asp:Content>
