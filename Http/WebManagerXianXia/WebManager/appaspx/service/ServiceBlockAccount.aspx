<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="ServiceBlockAccount.aspx.cs" Inherits="WebManager.appaspx.service.ServiceBlockAccount" %>
<asp:Content ID="Content1" ContentPlaceHolderID="service_common" runat="server">
    <h2>停封账号</h2>
    账号:&nbsp;&nbsp;<asp:TextBox ID="m_acc" runat="server" style="width:220px;height:25px"></asp:TextBox>
    <br />
    备注(可填操作原因):<asp:TextBox ID="m_comment" runat="server" style="width:800px;height:25px"></asp:TextBox>
    <br />
    <asp:Button ID="Button1" runat="server" onclick="onBlockAccount" Text="确定" style="width:133px;height:25px"/>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    <h2>已停封账号列表</h2>
    <asp:Table ID="m_result" runat="server">
    </asp:Table>
    <asp:Button ID="Button2" runat="server" onclick="onUnBlockAccount" Text="解封" style="width:133px;height:25px"/>
</asp:Content>
