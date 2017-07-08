<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginPwd.aspx.cs" Inherits="WebManager.appaspx.LoginPwd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="../style/login.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="cLogin">
        <table>
            <tr>
                <td colspan="2" class="cTdInfo">
                   <img alt="" src="/data/image/icon.png" height="44"/>管理系统登录
                </td>
            </tr>
            <tr>
                <td colspan="2" class="cTdAcc">
                    <input id="m_acc" type="password" autofocus="autofocus" autocomplete="off" placeholder="请输入密码" runat="server"/>
                </td>
            </tr>
            <tr>
                <td class="cTdVerCode">
                    <asp:Button ID="Button1" runat="server" Text="提交" onclick="onLogin"/>
                </td>
                <td class="cTdVerCode">
                    <asp:Button ID="Button2" runat="server" Text="返回" onclick="onReturn"/>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
