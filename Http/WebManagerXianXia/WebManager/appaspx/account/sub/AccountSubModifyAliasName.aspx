<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountSubModifyAliasName.aspx.cs" Inherits="WebManager.appaspx.account.sub.AccountSubModifyAliasName" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .cSafeWidth td{padding:5px;}
        .tdLeft{width:100px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>修改别名</h2>
        <table border="0" cellspacing="0" cellpadding="0" width="100%">
        	<tr>
        		<td class="tdLeft">账号:</td>
                <td>
                    <asp:TextBox ID="m_acc" runat="server" style="width:200px;height:20px"></asp:TextBox>
                </td>
        	</tr>

            <tr>
        		<td>别名:</td>
                <td>
                    <asp:TextBox ID="m_aliasName" runat="server" style="width:200px;height:20px"></asp:TextBox>
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
