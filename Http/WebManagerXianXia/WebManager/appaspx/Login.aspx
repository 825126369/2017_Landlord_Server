<%@ Page Title="登录" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="WebManager.Account.Login" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="margin:0 auto;width:260px">
    <asp:Login ID="LoginUser" runat="server" TitleText="电玩城登录" UserNameLabelText="用户名:" PasswordLabelText="密码:" LoginButtonText="登录" RememberMeText="下次记住我" EnableViewState="false" RenderOuterTable="false" OnAuthenticate="LoginButton_Click">
    </asp:Login>
    </div>
</asp:Content>
