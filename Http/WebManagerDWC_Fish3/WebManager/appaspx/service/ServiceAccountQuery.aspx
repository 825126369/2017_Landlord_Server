<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="ServiceAccountQuery.aspx.cs" Inherits="WebManager.appaspx.service.ServiceAccountQuery" %>
<asp:Content ID="Content1" ContentPlaceHolderID="service_common" runat="server">
    <h2>账号查询</h2>
    <asp:DropDownList ID="m_queryWay" runat="server" style="width:180px;height:30px">
    </asp:DropDownList>
    <asp:TextBox ID="m_param" runat="server" style="width:300px;height:20px"></asp:TextBox>
    <asp:Button ID="Button1" runat="server" onclick="onQueryAccount" Text="查询" style="width:133px;height:25px" />
    <asp:Table ID="m_result" runat="server">
    </asp:Table>
    <%-- <asp:Button ID="BtnBlockAcc" runat="server" onclick="onBlockAcc" Text="停封账号" style="width:133px;height:25px" visible="false" /> --%>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    <br />
    <br />
    <span id="m_page" style="text-align:center;display:block" runat="server"></span>
    <br />
    <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>
</asp:Content>
