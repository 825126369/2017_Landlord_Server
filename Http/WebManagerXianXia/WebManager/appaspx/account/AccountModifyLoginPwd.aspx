<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountModifyLoginPwd.aspx.cs" Inherits="WebManager.appaspx.account.AccountModifyLoginPwd" %>
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
                <td>修改内容:</td>
                <td>
                    <input type="radio" value="0" name="content" runat="server" id="opPwd" checked="true" clientidmode="Static"/>
                    <label for="opPwd">修改登录密码</label>
                    <input type="radio" value="1" name="content" runat="server" id="opVer" clientidmode="Static"/>
                    <label for="opVer">修改4位固定验证码</label>
                </td>
            </tr>
            <tr>
                <td>账号选择:</td>
                <td>
                    <asp:DropDownList ID="m_accList" runat="server" CssClass="cDropDownList"></asp:DropDownList>
                </td>
            </tr>
            <tr class="cOriPwd">
                <td>原密码:</td>
                <td>
                    <asp:TextBox ID="m_oriPwd" runat="server" CssClass="cTextBox" TextMode="Password" ClientIDMode="Static"></asp:TextBox>
                </td>
            </tr>
            <tr class="cOpLoginPwd">
                <td>密码:</td>
                <td>
                    <asp:TextBox ID="m_pwd1" runat="server" CssClass="cTextBox" TextMode="Password" ClientIDMode="Static"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^.{6,16}$" ControlToValidate="m_pwd1"></asp:RegularExpressionValidator>
                    6-16位任意字符
                </td>
            </tr>
            <tr class="cOpLoginPwd">
                <td>确认密码:</td>
                <td>
                    <asp:TextBox ID="m_pwd2" runat="server" CssClass="cTextBox" TextMode="Password" ClientIDMode="Static"></asp:TextBox>

                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^.{6,16}$" ControlToValidate="m_pwd2"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr class="cOpLoginPwd">
                <td>
                </td>
                <td>
                    <asp:CompareValidator ID="CompareValidator1" runat="server" 
                        ErrorMessage="两次密码不一致" ControlToCompare="m_pwd1" ControlToValidate="m_pwd2" ForeColor="Red" Display="Dynamic">
                        </asp:CompareValidator>
                </td>
            </tr>
            
            <%---------------------- 验证码的修改----------------------%>
            <tr class="cOpVerCode">
                <td>密码:</td>
                <td>
                    <asp:TextBox ID="m_verCode1" runat="server" CssClass="cTextBox" TextMode="Password" ClientIDMode="Static"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^[0-9]{4,4}$" ControlToValidate="m_verCode1"></asp:RegularExpressionValidator>
                    4位固定数字
                </td>
            </tr>
            <tr class="cOpVerCode">
                <td>确认密码:</td>
                <td>
                    <asp:TextBox ID="m_verCode2" runat="server" CssClass="cTextBox" TextMode="Password" ClientIDMode="Static"></asp:TextBox>

                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^[0-9]{4,4}$" ControlToValidate="m_verCode2"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr class="cOpVerCode">
                <td>
                </td>
                <td>
                    <asp:CompareValidator ID="CompareValidator2" runat="server" 
                        ErrorMessage="两次密码不一致" ControlToCompare="m_verCode1" ControlToValidate="m_verCode2" 
                        ForeColor="Red" Display="Dynamic">
                        </asp:CompareValidator>
                </td>
            </tr>
             <%---------------------- 验证码的修改 end----------------------%>


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
