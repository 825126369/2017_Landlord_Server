<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationRecharge.aspx.cs" Inherits="WebManager.appaspx.operation.OperationRecharge" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <h2>后台充值</h2>
    <p>
        充值金额:&nbsp;&nbsp;<asp:DropDownList ID="m_rechargeRMB" runat="server" style="width:180px;height:30px"></asp:DropDownList>
    </p>
    <p>
        玩家ID:&nbsp;&nbsp;<asp:TextBox ID="m_playerId" runat="server" style="width:180px;height:20px"></asp:TextBox>
    </p>
    
    <asp:Button ID="Button1" runat="server" onclick="onRecharge" Text="充值" style="width:100px;height:30px" />
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
