<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="ServiceBindPhone.aspx.cs" Inherits="WebManager.appaspx.service.ServiceBindPhone" %>
<asp:Content ID="Content1" ContentPlaceHolderID="serviceHeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="service_common" runat="server">
     <h2>绑定手机</h2>
     账号:<asp:TextBox ID="m_account" runat="server" style="width:240px;height:20px"></asp:TextBox>
     <br /><br />
     手机号:<asp:TextBox ID="m_phone" runat="server" style="width:240px;height:20px"></asp:TextBox>
     <br /><br />
     <asp:Button ID="Button3" runat="server" onclick="onModify" Text="绑定" style="width:60px;height:30px" />
     <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
