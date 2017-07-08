<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserLogin.aspx.cs" Inherits="WebManager.appaspx.UserLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="../style/default.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div style="width:450px;margin:100px auto" class="cLogin">
            <asp:Table ID="tabLogin" runat="server">
                <asp:TableRow>
                    <asp:TableCell CssClass="cTabContentL">用户名:</asp:TableCell>

                    <asp:TableCell CssClass="cTabContentR">
                        <asp:TextBox ID="txtAccount" runat="server" style="width:200px;height:20px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                            ErrorMessage="账号不能为空" ControlToValidate="txtAccount" ForeColor="Red"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell CssClass="cTabContentL">密码:</asp:TableCell>
                    <asp:TableCell CssClass="cTabContentR">
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" style="width:200px;height:20px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                ErrorMessage="密码不能为空" ControlToValidate="txtPassword" ForeColor="Red"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table> 
                
            <div class="cButton">
                <asp:Button ID="Button1" runat="server" Text="登录" OnClick="Button1_Click" Height="39px" Width="73px"  />
            </div>
            <br />
            <span id="m_opRes" style="font-size:medium;color:red" runat="server"></span>
        </div>
    </div>
    </form>
</body>
</html>
