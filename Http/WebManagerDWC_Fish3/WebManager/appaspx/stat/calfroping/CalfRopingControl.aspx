<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonCalfRoping.master" AutoEventWireup="true" CodeBehind="CalfRopingControl.aspx.cs" Inherits="WebManager.appaspx.stat.calfroping.CalfRopingControl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonCalfRoping_Content" runat="server">
    <div class="cSafeWidth">
        <h2>套牛参数调整</h2>
        <asp:Table ID="m_expRateTable" runat="server" CssClass="cTable">
        </asp:Table>

        期望盈利率:<asp:TextBox ID="txtExpRate" runat="server" CssClass="cTextBox"></asp:TextBox>
        <br /><br />
        <asp:Button ID="Button2" runat="server" Text="修改" onclick="onModifyExpRate" CssClass="cButton"/>
        <asp:Button ID="Button1" runat="server" Text="重置" onclick="onReset" CssClass="cButton"/>
        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    </div>
</asp:Content>
