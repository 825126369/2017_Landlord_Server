<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="ServiceBlockIP.aspx.cs" Inherits="WebManager.appaspx.service.ServiceBlockIP" %>
<asp:Content ID="Content1" ContentPlaceHolderID="service_common" runat="server">
    <h2>停封IP</h2>
    停封IP地址:&nbsp;&nbsp;<asp:TextBox ID="m_ip" runat="server" style="width:220px;height:25px"></asp:TextBox>
    <asp:Button ID="Button1" runat="server" onclick="onBlockIP" Text="确定" style="width:133px;height:25px"/>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    <h2>已停封IP列表</h2>
    <asp:Table ID="m_result" runat="server">
    </asp:Table>
    <asp:Button ID="Button2" runat="server" onclick="onUnBlockIP" Text="解封" style="width:133px;height:25px"/>
</asp:Content>
