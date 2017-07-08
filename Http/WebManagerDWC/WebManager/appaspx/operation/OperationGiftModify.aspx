<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationGiftModify.aspx.cs" Inherits="WebManager.appaspx.operation.OperationGiftModify" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/gift_modify.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    修改礼包:&nbsp;&nbsp;<asp:DropDownList ID="m_queryWay" runat="server" style="width:180px;height:30px"></asp:DropDownList>
    &nbsp;&nbsp;<asp:Button ID="Button3" runat="server" onclick="onQuery" Text="查询" style="width:60px;height:30px" />

    <asp:Table ID="m_result" runat="server" CssClass="result">
    </asp:Table>
    <input type="button" id="BtnModifyGift" value="确认修改" runat="server" onclick="submitGiftModifyParam()" />

    <input id="m_modifyInfo" type='hidden' runat='server' value="" />
    <input id="m_clientInfo" type='hidden' runat='server' value="" />
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
