<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountScoreAPIAdmin.aspx.cs" Inherits="WebManager.appaspx.account.AccountScoreAPIAdmin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <script src="../../Scripts/account/AccountScore.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>API管理员上分/下分</h2>
                
        <asp:Table ID="m_result" runat="server" CssClass="cTable cSubLevelTable">
        </asp:Table>

        <asp:TextBox ID="m_curMoney" runat="server" CssClass="cTextBox" style="display:none"></asp:TextBox>
        <asp:TextBox ID="m_isAdmin" runat="server" CssClass="cTextBox" style="display:none"></asp:TextBox>
    </div>
</asp:Content>
