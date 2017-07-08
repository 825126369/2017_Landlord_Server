<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationGenGiftCode.aspx.cs" Inherits="WebManager.appaspx.operation.OperationGenGiftCode" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/gift_code.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <h2>礼包码生成</h2>
    <input id="m_codeInfo" type='hidden' runat='server' value="" />
    <input id="m_clientInfo" type='hidden' runat='server' value="" />

    <asp:Table ID="m_result" runat="server" CssClass="result">
    </asp:Table>
    <input type="button" id="BtnGenGiftCode" value="生成礼包码" runat="server" onclick="submitGiftCodeParam()" />

    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
