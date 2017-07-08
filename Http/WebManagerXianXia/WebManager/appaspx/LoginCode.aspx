<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginCode.aspx.cs" Inherits="WebManager.appaspx.LoginCode" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<%-- <meta http-equiv="refresh" content="60;url=<%= ResolveUrl("~/appaspx/LoginAccount.aspx") %> " /> --%>
    <title></title>
    <link href="../style/login.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.8.3.js" type="text/javascript"></script>
    <script src="../Scripts/logincode.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="cLogin">
        <div class="countDown">
            <span id="idCountDown">60</span>秒内不输入将会自动跳转到登录页面
        </div>
        <table>
            <tr>
                <td colspan="2" class="cTdInfo">
                    请输入双重认证码
                </td>
            </tr>
            <tr>
                <td colspan="2" class="cTdAcc">
                    <input id="m_acc" type="text" maxlength="4" autofocus="autofocus" autocomplete="off" placeholder="请输入4位固定验证码" runat="server"/>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="" ControlToValidate="m_acc"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage=""
                        ValidationExpression="^[0-9]{4,4}$" ControlToValidate="m_acc" ValidationGroup="groupCommit"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td class="cTdVerCode">
                    <asp:Button ID="Button1" runat="server" Text="提交" onclick="onLogin" ValidationGroup="groupCommit" />
                </td>
                <td class="cTdVerCode">
                    <asp:Button ID="Button2" runat="server" Text="返回" onclick="onReturn" ValidationGroup="groupCommit1" />
                </td>
            </tr>
            <tr>
                <td colspan="2" class="cErrInfo" runat="server" id="tdError"></td>
            </tr>
        </table>

        <input type="hidden" id="idInputCountDown" name="inputCountDown" runat="server" clientidmode="Static" value="60" />    
    </div>
    </form>
</body>
</html>
