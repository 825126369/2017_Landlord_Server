<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationMaintenanceNotice.aspx.cs" Inherits="WebManager.appaspx.operation.OperationMaintenanceNotice" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <h2>维护公告</h2>
    当前状态:<p id="m_curState" runat="server"></p>
    维护信息:<asp:TextBox ID="m_info" runat="server" style="width:800px;height:20px"></asp:TextBox>
    <br />
    <asp:Button ID="Button3" runat="server" onclick="onOk" Text="确定维护" style="width:80px;height:30px" />
    <asp:Button ID="Button1" runat="server" onclick="onCancel" Text="撤消维护" style="width:80px;height:30px" />
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
