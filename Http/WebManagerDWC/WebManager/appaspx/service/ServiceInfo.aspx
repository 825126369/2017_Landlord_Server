<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="ServiceInfo.aspx.cs" Inherits="WebManager.appaspx.service.ServiceInfo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="service_common" runat="server">
    <h2>增加客服信息</h2>
    <p>默认信息为所有平台公用</p>
    <p>客户信息增加规则，信息之间以#相隔，各小段信息间以,相隔</p>
    <p>如:3,QQ:,8899#2,微信,9999</p>
    <p>前缀 1:表示微博  2：表示微信  3：表示QQ     4：表示客服电话</p>
    平台:<asp:DropDownList ID="m_platform" runat="server" style="width:180px;height:30px"></asp:DropDownList>
    <br /><br />
    描述:<asp:TextBox ID="m_desc" runat="server" style="width:900px;height:20px"></asp:TextBox>
    <br /><br />
    <asp:Button ID="Button3" runat="server" onclick="onCommit" Text="提交" style="width:60px;height:30px" />
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    
    <h2>现有客服信息</h2>
    <asp:Table ID="m_curHelp" runat="server">
    </asp:Table>
    <asp:Button ID="Button1" runat="server" onclick="onDelInfo" Text="删除" style="width:60px;height:30px" />
</asp:Content>
