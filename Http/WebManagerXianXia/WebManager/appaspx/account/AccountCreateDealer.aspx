<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountCreateDealer.aspx.cs" Inherits="WebManager.appaspx.account.AccountCreateDealer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .cSafeWidth td{padding:5px;}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>创建总代理</h2>
        
        <table>
            <tr>
                <td>总代理名称:</td>
                <td> <asp:TextBox ID="m_accName" runat="server" CssClass="cTextBox"></asp:TextBox> </td>
                <td>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                    ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                    ValidationExpression="^[a-zA-Z]{4,8}$" ControlToValidate="m_accName"></asp:RegularExpressionValidator>

                    <asp:Label ID="Label1" runat="server" Text="4-8位字母" style="margin-left:5px" ></asp:Label>
                </td>
            </tr>
            <tr>
                <td>别名:</td>
                <td>
                     <asp:TextBox ID="m_aliasName" runat="server" CssClass="cTextBox" ></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"  ForeColor="Red" 
                        ControlToValidate="m_aliasName"
                        ErrorMessage="必填项，可以是任意字符"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>密码:</td>
                <td>
                    <asp:TextBox ID="m_pwd1" runat="server" CssClass="cTextBox" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^.{6,16}$" ControlToValidate="m_pwd1"></asp:RegularExpressionValidator>
                    &nbsp;&nbsp;
                    6-16位任意字符
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
                        ValidationExpression="^.{6,16}$" ControlToValidate="m_pwd2"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2">
                    <asp:CompareValidator ID="CompareValidator1" runat="server" 
                        ErrorMessage="两次密码不一致" ControlToCompare="m_pwd1" ControlToValidate="m_pwd2" ForeColor="Red" Display="Dynamic">
                        </asp:CompareValidator>
                </td>
            </tr>
            <tr>
                <td>代理占成:</td>
                <td>
                    <asp:TextBox ID="m_agentRatio" runat="server" CssClass="cTextBox"></asp:TextBox>%
                </td>
                <td>
                    <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="输入错误" 
                        ControlToValidate="m_agentRatio" Display="Dynamic" ForeColor="Red" 
                        MinimumValue="0" MaximumValue="100" Type="Double"></asp:RangeValidator>
                    0-100之间的小数或整数
                </td>
            </tr>
            <tr>
                <td>洗码比:</td>
                <td>
                    <asp:TextBox ID="m_washRatio" runat="server" CssClass="cTextBox"></asp:TextBox>%
                </td>
                <td>
                    <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="输入错误" 
                        ControlToValidate="m_washRatio" Display="Dynamic" ForeColor="Red" 
                        MinimumValue="0" MaximumValue="1.2" Type="Double"></asp:RangeValidator>

                    0-1.2之间的小数
                </td>
            </tr>
            <%--  <tr class="cSubAcc">
                <td>币种:</td>
                <td>
                    <asp:DropDownList ID="m_moneyType" runat="server" CssClass="cDropDownList"></asp:DropDownList>
                </td>
            </tr> --%>
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
