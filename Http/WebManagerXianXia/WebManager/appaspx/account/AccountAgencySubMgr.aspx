<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountAgencySubMgr.aspx.cs" Inherits="WebManager.appaspx.account.AccountAgencySubMgr" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <div class="cSafeWidth">
        <h2 style="padding-bottom:20px;">子账号管理</h2>
    </div>
    <div class="container-fluid">
        <asp:Table ID="m_result" runat="server" CssClass="table table-hover table-bordered">
        </asp:Table>
    </div>
</asp:Content>
