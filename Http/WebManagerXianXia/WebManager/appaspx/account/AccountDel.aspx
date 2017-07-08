<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountDel.aspx.cs" Inherits="WebManager.appaspx.AccountDel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../style/del.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/account/AccountDel.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cDel">
        <table>
            <tr>
                <td colspan="3"><h2>删除账号</h2></td>
            </tr>
            <tr>
                <td>删除代理号及其子代理，及所有相关的会员</td>
                <td><input type="text" id="txtAgency" name="txtAgency"/></td>
                <td><input type="button" id="btnDelAgency" name="btnDelAgency" value="删除"/></td>
                <td><span id="txtAgencyMsg"></span></td>
            </tr>
            <tr>
                <td>删除某个会员</td>
                <td><input type="text" id="txtPlayer" name="txtPlayer"/></td>
                <td><input type="button" id="btnDelPlayer" name="btnDelPlayer" value="删除"/></td>
                <td><span id="txtPlayerMsg"></span></td>
            </tr>
        </table>
    </div>
</asp:Content>
