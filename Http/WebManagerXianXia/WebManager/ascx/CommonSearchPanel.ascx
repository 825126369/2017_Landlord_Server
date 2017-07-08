<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommonSearchPanel.ascx.cs" Inherits="WebManager.ascx.CommonSearchPanel" %>
<style type="text/css">
    .commonSearch td{padding:5px;}
    .commonSearch td input[type="text"]
    {
        width:300px;height:25px;
    }
</style>
<div>
    <table class="commonSearch">
        <tr>
            <td>时间:</td>
            <td>
                <asp:TextBox ID="__gmAccountCascadeStaticTime" runat="server" ClientIDMode="Static"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>创建者:</td>
            <td>
                <asp:TextBox ID="m_creator" runat="server" CssClass="cTextBox"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>账号:</td>
            <td>
                <asp:TextBox ID="m_acc" runat="server" CssClass="cTextBox"></asp:TextBox>
            </td>
        </tr>
        <tr id="tdViewLevel" runat="server">
            <td>查看层次:</td>
            <td>
                 <asp:RadioButtonList ID="m_way" runat="server" RepeatDirection="Horizontal"></asp:RadioButtonList>
            </td>
        </tr>
    </table>
</div>