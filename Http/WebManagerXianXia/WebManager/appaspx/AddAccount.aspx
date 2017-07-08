<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="AddAccount.aspx.cs" Inherits="WebManager.appaspx.AddAccount" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        添加账号
    </h2>
    <asp:Panel ID="Panel1" runat="server" Height="285px" Width="807px" style="height:auto">
        <asp:Label ID="Label2" runat="server" Text="账号："></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="m_accountName" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="Label3" runat="server" Text="密码："></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="m_key1" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <asp:Label ID="Label5" runat="server" Text="确认密码："></asp:Label>
        <asp:TextBox ID="m_key2" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="Label4" runat="server" Text="人员类型："></asp:Label>
        <asp:DropDownList ID="m_type" runat="server">
        </asp:DropDownList>
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" Height="56px" Text="提交" Width="126px" 
            onclick="onAddAccount" />
        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
        <h2>
            当前账号列表
        </h2>
        <br />
        <asp:Table ID="m_curAccount" runat="server">
        </asp:Table>
        <br />
        <asp:Button ID="Button2" runat="server" Height="56px" Text="修改" Width="126px" onclick="onModifyGMType" />
        <asp:Button ID="Button3" runat="server" Height="56px" Text="删除账号" Width="126px" onclick="onDelAccount" />
    </asp:Panel>
</asp:Content>
