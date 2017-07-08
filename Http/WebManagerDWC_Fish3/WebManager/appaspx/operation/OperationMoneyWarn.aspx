<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationMoneyWarn.aspx.cs" Inherits="WebManager.appaspx.operation.OperationMoneyWarn" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <h2>金币预警</h2>
    货币类型:&nbsp;&nbsp;<asp:DropDownList ID="m_currency" runat="server" style="width:180px;height:30px"></asp:DropDownList>
    <asp:Button ID="Button3" runat="server" onclick="onQuery" Text="查询" style="width:80px;height:30px" />
    <asp:Button ID="Button1" runat="server" onclick="onExport" Text="导出Excel" style="width:100px;height:30px" />
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    <asp:Table ID="m_result" runat="server" CssClass="cTable">
    </asp:Table>
</asp:Content>
