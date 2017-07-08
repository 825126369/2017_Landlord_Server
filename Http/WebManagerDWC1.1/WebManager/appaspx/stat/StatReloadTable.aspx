<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommon.master" AutoEventWireup="true" CodeBehind="StatReloadTable.aspx.cs" Inherits="WebManager.appaspx.stat.StatReloadTable" %>
<asp:Content ID="Content1" ContentPlaceHolderID="stat_comm_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="stat_common" runat="server">
    <h2>重新加载表格</h2>    
    表格:<asp:DropDownList ID="m_table" runat="server" style="width:130px;height:30px"></asp:DropDownList>
    <asp:Button ID="Button1" runat="server" onclick="onLoad" Text="加载" style="width:100px;height:30px" />
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
