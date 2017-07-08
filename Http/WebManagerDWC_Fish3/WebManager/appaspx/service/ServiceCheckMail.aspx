<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="ServiceCheckMail.aspx.cs" Inherits="WebManager.appaspx.service.ServiceCheckMail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="serviceHeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="service_common" runat="server">
    <h2>邮件检测</h2>
    <asp:Table ID="m_result" runat="server">
    </asp:Table>
    <asp:Button ID="Button2" runat="server" onclick="onDelMail" Text="删除" style="width:133px;height:25px"/>
    <asp:Button ID="Button1" runat="server" onclick="onSendMail" Text="发送" style="width:133px;height:25px"/>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
