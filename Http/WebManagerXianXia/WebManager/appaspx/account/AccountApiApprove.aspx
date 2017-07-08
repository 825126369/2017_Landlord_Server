<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountApiApprove.aspx.cs" Inherits="WebManager.appaspx.account.AccountApiApprove" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../Scripts/module/sea.js"></script>
    <script type="text/javascript">
        seajs.use("../../Scripts/account/AccountApiApprove.js");
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <h2 style="padding-bottom:20px;">API审批</h2>
        <asp:Table ID="m_result" runat="server" CssClass="table table-hover table-bordered">
        </asp:Table>
        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    </div>
</asp:Content>
