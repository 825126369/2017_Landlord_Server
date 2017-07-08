<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GameStandingBook.aspx.cs" Inherits="WebManager.appaspx.GameStandingBook" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
         .cTable tr:hover td{background:#ddd;}
         .cTable td{padding:20px;font-size:18px;}
         .Search td{padding:6px;}
         #divDetail{width:100%;position:absolute;left:0;top:0;background:rgba(0,0,0,0.5);
                    min-height:800px;display:none;
         }
         #divDetail a{background:#ddd;padding:10px;font-size:30px;width:100%;display:block;}
         #dateTitle{padding:20px;background:#fff;text-align:center;font-size:30px;}
    </style>
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
    <script src="../Scripts/module/sea.js" type="text/javascript"></script>
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_m_time').daterangepicker({ arrows: false });
	    });

	    seajs.use("../Scripts/GameStandingBook.js");
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <div class="cSafeWidth">
        <h2>游戏台账</h2>
        <table class="Search">
            <tr>
                <td>时间:</td>
                <td><asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px"></asp:TextBox></td>
                <%-- <td><asp:Button ID="Button1" runat="server" onclick="onExport" Text="导出Excel" style="width:100px;height:30px" /></td>--%>
            </tr>
            <tr>
                <td>属性:</td>
                <td><asp:DropDownList ID="m_property" runat="server" style="width:130px;height:30px"></asp:DropDownList></td>
            </tr>
            <tr>
                <td><input type="button" value="查询" style="width:60px;height:30px" id="btnQuery" /></td>
                <%-- <td><asp:Button ID="Button3" runat="server" onclick="onQuery" Text="查询" style="width:60px;height:30px" /></td>--%>
            </tr>
            <tr>
                <td colspan="3"><span id="m_res" style="font-size:medium;color:red" runat="server"></span></td>
            </tr>
        </table>
    </div>
    <asp:Table ID="m_result" runat="server" CssClass="cTable"></asp:Table>
    <br />
    <span id="m_page" style="text-align:center;display:block" runat="server"></span>
    <br />
    <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>

    <div id="divDetail">
        <a style="text-align:center;" href="#" >点击关闭</a>

        <div>
            <h2 id="dateTitle"></h2>
            <table id="tableHead" class="cTable"></table>
            <table id="tableContent" class="cTable"></table>
        </div>

        <a style="text-align:center;" href="#" >点击关闭</a>
    </div>
</asp:Content>
