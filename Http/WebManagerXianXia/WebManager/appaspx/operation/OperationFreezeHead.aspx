<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationFreezeHead.aspx.cs" Inherits="WebManager.appaspx.operation.OperationFreezeHead" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <h2>冻结头像</h2>
    玩家ID:&nbsp;&nbsp;<asp:TextBox ID="m_playerId" runat="server" style="width:180px;height:20px"></asp:TextBox>
    <asp:Button ID="Button1" runat="server" onclick="onViewHead" Text="查看头像" style="width:100px;height:30px" />
    <br />
    冻结天数(默认7天):&nbsp;&nbsp;<asp:TextBox ID="m_freezeDays" runat="server" style="width:180px;height:20px"></asp:TextBox>
    <br />
    <asp:Button ID="Button3" runat="server" onclick="onFreeze" Text="冻结" style="width:100px;height:30px" />
    <div>
        <asp:Image ID="m_headImg" runat="server" />
    </div>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
