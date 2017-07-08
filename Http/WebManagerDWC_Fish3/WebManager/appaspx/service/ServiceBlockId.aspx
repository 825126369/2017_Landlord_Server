<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="ServiceBlockId.aspx.cs" Inherits="WebManager.appaspx.service.ServiceBlockId" %>
<asp:Content ID="Content1" ContentPlaceHolderID="serviceHeadContent" runat="server">
   <%--  <link href="../../style/page.css" rel="stylesheet" />
    <script src="../../Scripts/tool/jquery-1.8.3.js" type="text/javascript"></script>
    <script src="../../Scripts/tool/jqPaginator.min.js" type="text/javascript"></script>
    <script src="../../Scripts/service/ServiceBlockId.js" type="text/javascript"></script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="service_common" runat="server">
    <h2>停封玩家ID</h2>
    ID:&nbsp;&nbsp;<asp:TextBox ID="m_playerId" runat="server" style="width:220px;height:25px"></asp:TextBox>
    <br />
    备注(可填操作原因):<asp:TextBox ID="m_comment" runat="server" style="width:800px;height:25px"></asp:TextBox>
    <br />
    <asp:Button ID="Button1" runat="server" onclick="onBlockPlayerId" Text="确定" style="width:133px;height:25px"/>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    <h2>已停封玩家ID列表</h2>
    <asp:Table ID="m_result" runat="server">
    </asp:Table>
    <asp:Button ID="Button2" runat="server" onclick="onUnBlockPlayerId" Text="解封" style="width:133px;height:25px"/>

    <div>
        <ul class="pagination" id="pagination0"></ul>  
    </div>
</asp:Content>
