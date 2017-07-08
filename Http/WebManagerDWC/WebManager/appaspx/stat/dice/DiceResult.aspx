<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonDice.Master" AutoEventWireup="true" CodeBehind="DiceResult.aspx.cs" Inherits="WebManager.appaspx.stat.dice.DiceResult" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonDice_HeadContent" runat="server">
    <style type="text/css">
        .cSafeWidth td{padding:5px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonDice_Content" runat="server">
    <div class="cSafeWidth">
        <h2>骰宝结果控制</h2>
        <table>
            <tr>
                <td>点数1:</td>
                <td><asp:DropDownList ID="m_result1" runat="server" class="cDropDownList"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>点数2:</td>
                <td><asp:DropDownList ID="m_result2" runat="server" class="cDropDownList"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>点数2:</td>
                <td><asp:DropDownList ID="m_result3" runat="server" class="cDropDownList"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="Button2" runat="server" Text="设置结果" onclick="onSetResult" CssClass="cButton"/>
                    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
