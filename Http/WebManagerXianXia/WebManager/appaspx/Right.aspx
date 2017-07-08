<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Right.aspx.cs" Inherits="WebManager.appaspx.Right" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>编辑人员权限</h2>
    <asp:Panel ID="Panel1" runat="server" Height="131px" Width="808px" style="height:auto" >
        <asp:Table ID="m_right" runat="server">
        </asp:Table>
        <asp:Button ID="Button1" runat="server" onclick="onCommitRight" Text="提交" style="width:98px;height:40px" />
        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    </asp:Panel>
</asp:Content>
