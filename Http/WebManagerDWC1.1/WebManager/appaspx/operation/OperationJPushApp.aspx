<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationJPushApp.aspx.cs" Inherits="WebManager.appaspx.operation.OperationJPushApp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <h2>极光推送应用配置</h2>
    平台:<asp:DropDownList ID="m_platform" runat="server" style="width:110px;height:30px"></asp:DropDownList>
    <br />
    应用名称：<asp:TextBox ID="m_appName" runat="server" style="width:300px;height:20px;margin-top:10px"></asp:TextBox>
    <br />
    AppKey：<asp:TextBox ID="m_appKey" runat="server" style="width:300px;height:20px;margin-top:10px"></asp:TextBox>
    <br />
    APISecret：<asp:TextBox ID="m_apiSecret" runat="server" style="width:300px;height:20px;margin-top:10px"></asp:TextBox>
    <br />
    <asp:Button ID="add" runat="server" onclick="onAddApp" Text="添加" style="width:133px;height:25px;margin-top:10px" />
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>

    <h2>当前已配置的应用</h2>
    <asp:Table ID="m_result" runat="server">
    </asp:Table>
    <asp:Button ID="Button2" runat="server" onclick="onDelApp" Text="删除" style="width:133px;height:25px"/>
</asp:Content>
