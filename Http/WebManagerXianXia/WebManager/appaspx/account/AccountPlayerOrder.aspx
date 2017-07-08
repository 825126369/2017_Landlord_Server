<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountPlayerOrder.aspx.cs" Inherits="WebManager.appaspx.account.AccountPlayerOrder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .cTableMgr td{padding:2px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>订单管理</h2>
        <table class="cTableMgr">
            <tr>
                <td>订单状态:</td>
                <td> <asp:DropDownList ID="m_orderState" runat="server" CssClass="cDropDownList"></asp:DropDownList> </td>
                <td>
                    <asp:CheckBox ID="m_forwardOrder" runat="server" AutoPostBack="True" OnCheckedChanged="onAutoForward" Text="自动转发订单到上级" />
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Button ID="Button1" runat="server" Text="查询" CssClass="cButton"/>
                </td>
            </tr>
        </table>
        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>
    </div>
</asp:Content>
