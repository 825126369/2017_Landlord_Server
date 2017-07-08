<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationPlayerOp.aspx.cs" Inherits="WebManager.appaspx.operation.OperationPlayerOp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <div class="cSafeWidth">
        <h2>玩家相关操作</h2>
        <div class="cOp1 cDiv1">
            玩家ID:<asp:TextBox ID="m_playerId" runat="server" CssClass="cTextBox"></asp:TextBox>
            <div class="cOp">
                <asp:Button ID="Button2" runat="server" Text="踢出玩家" onclick="onKickPlayer" />
                <asp:Button ID="Button1" runat="server" Text="给所有玩家添加新任务" onclick="onAddNewTask" />
            </div>
            <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
        </div>
    </div>
</asp:Content>
