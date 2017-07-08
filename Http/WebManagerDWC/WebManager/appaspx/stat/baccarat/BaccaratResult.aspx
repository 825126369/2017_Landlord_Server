<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonBaccarat.master" AutoEventWireup="true" CodeBehind="BaccaratResult.aspx.cs" Inherits="WebManager.appaspx.stat.baccarat.BaccaratResult" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonBaccarat_HeadContent" runat="server">
    <style type="text/css">
        .cSafeWidth td{padding:5px;}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonBaccarat_Content" runat="server">
     <div class="cSafeWidth">
        <h2>百家乐结果控制</h2>
        <table>
            <tr>
                <td>开奖结果:</td>
                <td><asp:DropDownList ID="m_result" runat="server" class="cDropDownList"></asp:DropDownList></td>
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
