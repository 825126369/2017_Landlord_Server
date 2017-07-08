<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/td/TdCommon.master" AutoEventWireup="true" CodeBehind="TdLose.aspx.cs" Inherits="WebManager.appaspx.td.TdLose" %>
<asp:Content ID="Content1" ContentPlaceHolderID="tdHeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="td_common" runat="server">
    <div class="cSafeWidth">
        <h2>大R流失</h2>

        <table>
            <tr>
                <td colspan="2">
                    <asp:Button ID="Button3" runat="server" onclick="onQuery" Text="查询" style="width:60px;height:30px" />
                </td>
            </tr>
        </table>

        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>
    </div>
</asp:Content>
