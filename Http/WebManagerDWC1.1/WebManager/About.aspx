<%@ Page Title="关于我们" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="WebManager.About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        关于
    </h2>
    <ol>
        <li>可以选择菜单项所标识的IP地址，切换要操作的数据</li>
        <li>若选择的IP地址未开机，会出现切换缓慢的情况</li>
        <li>默认要操作的数据库地址是: 192.168.1.12</li>
    </ol>
</asp:Content>
