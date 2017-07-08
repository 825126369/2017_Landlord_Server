<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ModifyLoginPwd.aspx.cs" Inherits="WebManager.appaspx.account.ModifyLoginPwd" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../Scripts/account/AccountModifyLoginPwd.js" type="text/javascript"></script>
    <style type="text/css">
        .cSafeWidth td{padding:5px;}
        .cSafeWidth tr td:first-child{text-align:right;}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>修改登录密码</h2>
        <table>
            <tr>
                <td>原密码:</td>
                <td>
                    <asp:TextBox ID="m_oriPwd" runat="server" CssClass="cTextBox" TextMode="Password" ClientIDMode="Static"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>密码:</td>
                <td>
                    <asp:TextBox ID="m_pwd1" runat="server" CssClass="cTextBox" TextMode="Password" ClientIDMode="Static"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^.{6,16}$" ControlToValidate="m_pwd1"></asp:RegularExpressionValidator>
                    6-16位任意字符
                </td>
            </tr>
            <tr>
                <td>确认密码:</td>
                <td>
                    <asp:TextBox ID="m_pwd2" runat="server" CssClass="cTextBox" TextMode="Password" ClientIDMode="Static"></asp:TextBox>

                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^.{6,16}$" ControlToValidate="m_pwd2"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:CompareValidator ID="CompareValidator1" runat="server" 
                        ErrorMessage="两次密码不一致" ControlToCompare="m_pwd1" ControlToValidate="m_pwd2" ForeColor="Red" Display="Dynamic">
                        </asp:CompareValidator>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Button ID="Button1" runat="server" Text="修改" CssClass="cButton" onclick="onModify" />
                    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
