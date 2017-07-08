<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginAccount.aspx.cs" Inherits="WebManager.appaspx.LoginAccount" %>

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
                   <img alt="" src="/data/image/icon.png" height="44"/> 管理系统登录
                </td>
            </tr>
            <tr>
                <td colspan="2" class="cTdAcc">
                    <input id="m_acc" type="text"  maxlength="30" autofocus="autofocus" autocomplete="off" placeholder="请输入账号" runat="Server"/>
                </td>
            </tr>
            <tr>
                <td class="cTdVerCode">
                    <input id="m_ver" type="text" maxlength="6" runat="server" autocomplete="off" placeholder="请输入验证码"/>
                </td>
                <td class="cTdImgCode">
                    <asp:Image ID="Image1" runat="server" alt="" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <%-- <input id="Submit1" type="submit" value="提交" runat="Server" onclick="onLogin" /> --%>
                    <asp:Button ID="Button1" runat="server" Text="提交" onclick="onLogin"/>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
