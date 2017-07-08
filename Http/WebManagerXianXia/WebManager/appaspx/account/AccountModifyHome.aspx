<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountModifyHome.aspx.cs" Inherits="WebManager.appaspx.account.AccountModifyHome" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>修改首页</h2>
        
        <table>
            <tr>
        		<td>账号:</td>
                <td>
                    <asp:TextBox ID="m_acc" runat="server" CssClass="cTextBox"></asp:TextBox>
                </td>
        	</tr>
            <tr>
                <td>首页:</td>
                <td>
                     <asp:TextBox ID="m_home" runat="server" CssClass="cTextBox" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="Button1" runat="server" Text="修改" CssClass="cButton" onclick="onModifyHome" />
                    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
