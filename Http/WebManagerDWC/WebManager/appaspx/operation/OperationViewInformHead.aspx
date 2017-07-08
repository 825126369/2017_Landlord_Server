<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationViewInformHead.aspx.cs" Inherits="WebManager.appaspx.operation.OperationViewInformHead" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <div class="cSafeWidth" style="text-align:center;">
        <h2 style="text-align:center;">头像举报</h2>
        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>
        <asp:Button ID="Button1" runat="server" Text="删除选择" style="width:70px;height:35px;" 
            onclick="onDelPlayer"/>
    </div>
</asp:Content>
