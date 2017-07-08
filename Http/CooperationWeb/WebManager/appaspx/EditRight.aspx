<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditRight.aspx.cs" Inherits="WebManager.appaspx.EditRight" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="masterMainContent" runat="server">
    <div class="cSafeWidth">
        <h2>编辑账号</h2>
        <asp:Table ID="tabRight" runat="server" CssClass="cTable"></asp:Table>

        <asp:Button ID="btnCommit" runat="server" Text="提交" OnClick="btnCommit_Click" CssClass="cButton"/>
        <asp:Button ID="btnDelAccount" runat="server" Text="删除账号" CssClass="cButton" OnClick="btnDelAccount_Click" />
        <span id="m_opRes" style="font-size:medium;color:red" runat="server"></span>
    </div>
</asp:Content>
