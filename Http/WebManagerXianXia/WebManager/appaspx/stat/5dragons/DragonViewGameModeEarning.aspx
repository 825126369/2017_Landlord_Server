<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DragonViewGameModeEarning.aspx.cs" Inherits="WebManager.appaspx.stat._5dragons.DragonViewGameModeEarning" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        td.left{width:100px;}
        table.tableForm td{padding-top:5px; padding-bottom:5px;}
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>五龙具体盈利情况</h2>
        <table border="0" cellspacing="0" cellpadding="0" width="100%" class="tableForm">
        	<tr>
        		<td class="left">房间:</td>
                <td>
                    <asp:DropDownList ID="m_room" runat="server" CssClass="cDropDownList"></asp:DropDownList>
                </td>
        	</tr>
            <tr>
        		<td class="left">桌子:</td>
                <td>
                    <asp:TextBox ID="m_desk" runat="server"></asp:TextBox>
                </td>
        	</tr>
            <tr>
        		<td>
                    <asp:Button ID="Button1" runat="server" Text="查看" OnClick="onViewGameMode" /></td>
        	</tr>
        </table>
        <asp:Table ID="m_gameMode" runat="server" CssClass="cTable">
        </asp:Table>
    </div>
</asp:Content>
