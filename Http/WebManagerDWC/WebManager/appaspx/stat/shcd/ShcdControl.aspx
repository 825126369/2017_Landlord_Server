<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommonShcd.master" AutoEventWireup="true" CodeBehind="ShcdControl.aspx.cs" Inherits="WebManager.appaspx.stat.shcd.ShcdControl" %>
<asp:Content ID="Content2" ContentPlaceHolderID="StatCommonShcd_HeadContent" runat="server">
    <script src="../../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="../../../Scripts/module/sea.js" type="text/javascript"></script>
    <script type="text/javascript">
        seajs.use("../../../Scripts/stat/ShcdControl.js");
    </script>
   
    <style type="text/css">
        .cSafeWidth input[type=button], input[type=submit]{width:120px;height:30px;margin-left:2px;}
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="StatCommonShcd_Content" runat="server">
    <div class="cSafeWidth">
        <h2>黑红梅方参数调整</h2>
        <asp:Table ID="m_expRateTable" runat="server" CssClass="cTable">
        </asp:Table>

        期望盈利率:<asp:TextBox ID="txtExpRate" runat="server" CssClass="cTextBox"></asp:TextBox>
        <select id="level">
            <option value="0">自动控制</option>
            <option value="1">天堂</option>
            <option value="2">普通</option>
            <option value="3">困难</option>
            <option value="4">超难</option>
            <option value="5">最难</option>
        </select>
        <br /><br />
        <asp:Button ID="Button2" runat="server" Text="修改期望盈利率" onclick="onModifyExpRate" CssClass="cButton"/>
        <input type="button" value="修改难度" id="btnModifyLevel" class="cButton"/>
        <asp:Button ID="Button1" runat="server" Text="重置" onclick="onReset" CssClass="cButton"/>
        <input type="button" value="修改大小王个数" id="btnModifyJokerCount" />
        <input type="button" value="设置作弊局数" id="btnCheat" />
        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    </div>
</asp:Content>
