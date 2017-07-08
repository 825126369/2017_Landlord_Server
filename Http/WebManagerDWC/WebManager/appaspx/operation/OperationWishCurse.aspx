<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationWishCurse.aspx.cs" Inherits="WebManager.appaspx.operation.OperationWishCurse" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
    <script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="../../Scripts/operation/OperationWishCurse.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <div class="cSafeWidth">
        <h2 style="text-align:center;"> GM祝福诅咒</h2>
        <table border="0" cellspacing="0" cellpadding="0" class="inputForm" width="50%">
            <tr>
        		<td class="tdLeft">操作类型:</td>
                <td>
                    <asp:DropDownList ID="m_opType" runat="server" style="width:150px;"></asp:DropDownList>
                </td>
        	</tr>
        	<tr>
        		<td class="tdLeft">祝福or诅咒:</td>
                <td>
                    <asp:DropDownList ID="m_type" runat="server" style="width:150px;"></asp:DropDownList>
                </td>
        	</tr>
            <tr>
        		<td class="tdLeft">玩家ID:</td>
                <td>
                    <asp:TextBox ID="m_playerId" runat="server"></asp:TextBox>
                </td>
        	</tr>
            <tr class="tdAlt">
        		<td class="tdLeft">升降命中率:</td>
                <td>
                    <asp:TextBox ID="m_rate" runat="server"></asp:TextBox>
                </td>
        	</tr>
            <tr>
        		<td class="tdLeft">所在游戏:</td>
                <td>
                    <asp:DropDownList ID="m_game" runat="server" style="width:150px;"></asp:DropDownList>
                </td>
        	</tr>
            <tr>
        		<td colspan="2" class="tdButton">
                    <asp:Button ID="Button1" runat="server" Text="确定" onclick="onAddWishCurse"/>
        		</td>
        	</tr>
            <tr>
        		<td colspan="2" style="text-align:center;">
                    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
        		</td>
        	</tr>
        </table>
    </div>
</asp:Content>
