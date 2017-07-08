<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountAgencyRight.aspx.cs" Inherits="WebManager.appaspx.account.sub.AccountAgencyRight" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>下线代理权限设定</h2>
        <table border="0" cellspacing="0" cellpadding="0" width="100%">
        	<tr>
        		<td>账号:</td>
                <td>
                    <asp:TextBox ID="m_acc" runat="server"></asp:TextBox>
                </td>
        	</tr>

            <tr>
        		<td>权限:</td>
                <td>
                    <asp:CheckBoxList ID="m_right" runat="server" RepeatDirection="Horizontal"></asp:CheckBoxList>
                </td>
        	</tr>

            <tr>
        		<td>
                    <asp:Button ID="Button1" runat="server" Text="确定修改" onclick="onModifyRight"/>
        		</td>
                <td><span id="m_res" style="font-size:medium;color:red" runat="server"></span></td>
        	</tr>
        </table>
    </div>
</asp:Content>
