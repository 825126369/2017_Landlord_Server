<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonShcd.master" AutoEventWireup="true" CodeBehind="ShcdResult.aspx.cs" Inherits="WebManager.appaspx.stat.shcd.ShcdResult" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonShcd_Content" runat="server">
    <div class="cSafeWidth">
        <h2>黑红梅方结果控制</h2>
        <table>
            <tr>
                <td>开奖结果:</td>
                <td><asp:DropDownList ID="m_result" runat="server" class="cDropDownList"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="Button2" runat="server" Text="设置结果" onclick="onSetResult" CssClass="cButton"/>
                </td>
                <td>                    
                    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
                </td>
            </tr>
        </table>

        <div style="border:1px solid black;margin-top:10px;padding:5px;">
            <h2>结果列表</h2>
            <asp:Table ID="m_allResult" runat="server" CssClass="cTable">
            </asp:Table>
        </div>
    </div>
</asp:Content>
