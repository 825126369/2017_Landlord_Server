<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonFishlord.master" AutoEventWireup="true" CodeBehind="StatFishlordControl.aspx.cs" Inherits="WebManager.appaspx.stat.StatFishlordControl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonFishlord_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonFishlord_Content" runat="server">
    <h2>经典捕鱼参数调整</h2>
    <asp:Table ID="m_expRateTable" runat="server">
    </asp:Table>
    <p>
        期望盈利率:<asp:TextBox ID="m_expRate" runat="server" style="width:100px;height:25px"></asp:TextBox>
    </p>
    <asp:Button ID="Button3" runat="server" Text="修改期望盈利率" onclick="onModifyExpRate" style="width:125px;height:25px"/>
    <asp:Button ID="Button1" runat="server" Text="重置" onclick="onReset" style="width:125px;height:25px"/>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
