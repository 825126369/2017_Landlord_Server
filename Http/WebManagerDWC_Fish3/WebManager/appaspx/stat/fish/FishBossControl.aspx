<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonFishlord.master" AutoEventWireup="true" CodeBehind="FishBossControl.aspx.cs" Inherits="WebManager.appaspx.stat.fish.FishBossControl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonFishlord_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonFishlord_Content" runat="server">
    <div class="cSafeWidth">
        <h2>经典捕鱼BOSS控制</h2>
        <asp:Table ID="m_room" runat="server" CssClass="cTable">
        </asp:Table>
        <p>
            BOSS最大数量:<asp:TextBox ID="m_maxCount" runat="server" style="width:100px;height:25px"></asp:TextBox>
            BOSS出现概率:<asp:TextBox ID="m_rand" runat="server" style="width:100px;height:25px"></asp:TextBox>这是个万分值
        </p>
        <asp:Button ID="Button3" runat="server" Text="修改" onclick="onModify" style="width:125px;height:25px"/>
        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    </div>
</asp:Content>
