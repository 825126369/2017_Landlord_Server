<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountCreateSellerSubAdmin.aspx.cs" Inherits="WebManager.appaspx.account.AccountCreateSellerSubAdmin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../Scripts/jquery-1.8.3.js" type="text/javascript"></script>
    <script src="../../Scripts/account/AccountCreateDealer.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>创建售货亭管理员 子账号</h2>
       
        <p>
            &nbsp;&nbsp;&nbsp;
            账号类型: <asp:DropDownList ID="m_type" runat="server" CssClass="cDropDownList"></asp:DropDownList>
        </p>

        <p>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            密码:<asp:TextBox ID="m_pwd1" runat="server" CssClass="cTextBox" TextMode="Password"></asp:TextBox>

            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                ValidationExpression="^.{6,16}$" ControlToValidate="m_pwd1"></asp:RegularExpressionValidator>

            &nbsp;

            6-16位任意字符
        </p>
        <p>
            &nbsp;确认密码:<asp:TextBox ID="m_pwd2" runat="server" CssClass="cTextBox" TextMode="Password"></asp:TextBox>

            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                ValidationExpression="^.{6,16}$" ControlToValidate="m_pwd2"></asp:RegularExpressionValidator>

        </p>

        <p>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:CompareValidator ID="CompareValidator1" runat="server" 
                ErrorMessage="两次密码不一致" ControlToCompare="m_pwd1" ControlToValidate="m_pwd2" ForeColor="Red" Display="Dynamic">
                </asp:CompareValidator>
        </p>

        <asp:Button ID="Button1" runat="server" Text="创建" CssClass="cButton" onclick="onCreateAccount" />
        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    </div>
</asp:Content>
