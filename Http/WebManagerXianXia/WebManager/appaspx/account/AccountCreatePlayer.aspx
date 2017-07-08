<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountCreatePlayer.aspx.cs" Inherits="WebManager.appaspx.account.AccountCreatePlayer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .cSafeWidth td{padding:5px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>新增会员</h2>
        <table>
            <tr>
                <td>账号名称:</td>
                <td>
                    <asp:Label ID="m_prefix" runat="server" Text=""></asp:Label>
                    <asp:TextBox ID="m_accName" runat="server" CssClass="cTextBox"></asp:TextBox>
                </td>
                <td>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^[0-9a-zA-Z]{6,30}$" ControlToValidate="m_accName"></asp:RegularExpressionValidator>
                    &nbsp;
                    <asp:Label ID="Label1" runat="server" Text="6-30位字母或数字"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>别名:</td>
                <td>
                    <asp:TextBox ID="m_aliasName" runat="server" CssClass="cTextBox" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>密码:</td>
                <td>
                    <asp:TextBox ID="m_pwd1" runat="server" CssClass="cTextBox" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^[\S]{6,20}$" ControlToValidate="m_pwd1"></asp:RegularExpressionValidator>
                    &nbsp;
                    <asp:Label ID="Label2" runat="server" Text="6-20位任意字符"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>确认密码:</td>
                <td>
                    <asp:TextBox ID="m_pwd2" runat="server" CssClass="cTextBox" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^[\S]{6,20}$" ControlToValidate="m_pwd2"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:CompareValidator ID="CompareValidator1" runat="server" 
                        ErrorMessage="两次密码不一致" ControlToCompare="m_pwd1" ControlToValidate="m_pwd2" ForeColor="Red" Display="Dynamic">
                        </asp:CompareValidator>
                </td>
            </tr>
            <tr>
                <td>洗码比:</td>
                <td>
                   <%-- <asp:TextBox ID="m_washRatio" runat="server" CssClass="cTextBox"></asp:TextBox>  --%>
                    <asp:CheckBox ID="m_hasWashRation" runat="server" Text="给该会员分配洗码比" />
                </td>
               <%-- <td>
                     <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="输入错误" 
                        ControlToValidate="m_washRatio" Display="Dynamic" ForeColor="Red" 
                        MinimumValue="0" MaximumValue="1" Type="Double"></asp:RangeValidator>

                    0-1之间的小数，但不能超出自身的洗码比
                </td> --%>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Button ID="Button1" runat="server" Text="创建" CssClass="cButton" onclick="onCreateAccount" />
                    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
