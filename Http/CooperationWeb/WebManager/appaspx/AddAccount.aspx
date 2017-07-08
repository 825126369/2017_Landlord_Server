<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddAccount.aspx.cs" Inherits="WebManager.appaspx.AddAccount" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../style/default.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="masterMainContent" runat="server">
     <div class="cSafeWidth">
        <h2>添加账号</h2>
        <div>账号名:</div>
         
        <asp:TextBox ID="txtAccount" runat="server" style="width:200px;height:20px"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
             ErrorMessage="账号不能为空" ForeColor="Red" ControlToValidate="txtAccount"></asp:RequiredFieldValidator>

        <div>密码:</div>
        <asp:TextBox ID="txtPwd" runat="server" style="width:200px;height:20px" TextMode="Password"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
             ErrorMessage="密码不能为空" ForeColor="Red" ControlToValidate="txtPwd" Display="Dynamic"></asp:RequiredFieldValidator>

        <div>确认密码:</div>
        <asp:TextBox ID="txtPwdRep" runat="server" style="width:200px;height:20px" TextMode="Password"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
             ErrorMessage="密码不能为空" ForeColor="Red" ControlToValidate="txtPwdRep" Display="Dynamic"></asp:RequiredFieldValidator>

         <asp:CompareValidator ID="CompareValidator1" runat="server" 
             ErrorMessage="两次密码不一致" ControlToCompare="txtPwd" ControlToValidate="txtPwdRep" ForeColor="Red" Display="Dynamic">
         </asp:CompareValidator>

        <div>
            <asp:Button ID="btnAdd" runat="server" Text="添加"  CssClass="cButton" OnClick="btnAdd_Click" />
        </div>
        <span id="m_opRes" style="font-size:medium;color:red" runat="server"></span>
    </div>
</asp:Content>
