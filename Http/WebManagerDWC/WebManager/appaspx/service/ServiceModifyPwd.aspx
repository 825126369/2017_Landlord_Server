<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="ServiceModifyPwd.aspx.cs" Inherits="WebManager.appaspx.service.ServiceModifyPwd" %>
<asp:Content ID="Content1" ContentPlaceHolderID="service_common" runat="server">
    <div class="cSafeWidth">
     <h2>重置密码</h2>
     类型:<asp:DropDownList ID="m_pwdType" runat="server" style="width:130px;height:30px">
         <asp:ListItem Value="0">登录密码</asp:ListItem>
         <asp:ListItem Value="1">保险箱密码</asp:ListItem>
     </asp:DropDownList>
     <br /><br />
     玩家ID:<asp:TextBox ID="m_account" runat="server" style="width:240px;height:20px"></asp:TextBox>
     <br /><br />
     新密码:<asp:TextBox ID="m_newPwd" runat="server" style="width:240px;height:20px"></asp:TextBox>
      <%-- 手机号:<asp:TextBox ID="m_phone" runat="server" style="width:240px;height:20px"></asp:TextBox> --%>
     <br /><br />
     <asp:Button ID="Button3" runat="server" onclick="onModify" Text="重置" style="width:60px;height:30px" />
     <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
     </div>
</asp:Content>
