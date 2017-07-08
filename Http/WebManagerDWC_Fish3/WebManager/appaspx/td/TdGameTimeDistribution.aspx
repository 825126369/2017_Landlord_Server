<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/td/TdCommon.master" AutoEventWireup="true" CodeBehind="TdGameTimeDistribution.aspx.cs" Inherits="WebManager.appaspx.td.TdGameTimeDistribution" %>
<asp:Content ID="Content1" ContentPlaceHolderID="tdHeadContent" runat="server">
    <link rel="stylesheet" type="text/css" media="all" href="../../Scripts/datepicker/daterangepicker.css" />
    <script type="text/javascript" src="http://cdn.bootcss.com/jquery/1.11.1/jquery.min.js"></script>
    <script src="http://cdn.bootcss.com/bootstrap/3.3.0/js/bootstrap.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../Scripts/datepicker/moment.min.js"></script>
    <script type="text/javascript" src="../../Scripts/datepicker/daterangepicker.js"></script>

    <script src="http://cdn.hcharts.cn/highcharts/highcharts.js" type="text/javascript"></script>

    <script src="../../Scripts/module/sea.js" type="text/javascript"></script>
	<script type="text/javascript">
	    $(function () {
	        $('#txtTime').daterangepicker();
	    });

	    seajs.use("../../Scripts/td/TdGameTimeDistribution.js?ver=1");
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="td_common" runat="server">
    <div class="cSafeWidth">
        <h2>平均游戏时长分布</h2>
        <table style="float:left;">
            <tr>
                <td>时间:</td>
                <td><input type="text" style="width:400px;height:20px" id="txtTime"/></td>
                <%-- <td colspan="2">
                    <input type="button" value="查询" style="width:60px;height:30px" id="btnQuery"/>
                </td>--%>
            </tr>
        </table>
        <ul class="SelCard">
            <li class="Active" data="1">活跃玩家</li><li data="2">付费玩家</li><li data="3">新增玩家</li>
        </ul>
        <div class="clear"></div>
    </div>

    <div id="divTemplate" style="display:none">
        <h2 style="text-align:center;background:#ccc;padding:6px;font-size:30px;" id="{0}"></h2>
        <div style="width:800px; height: 300px; margin: 0 auto;display:none;" id="{1}"></div>
        <div style="width:800px; height: 300px; margin: 0 auto;display:none;" id="{2}"></div>
        <div style="width:800px; height: 300px; margin: 0 auto;display:none;" id="{3}"></div>
        <div style="width:800px; height: 300px; margin: 0 auto;display:none;" id="{4}"></div>
        <div style="width:800px; height: 300px; margin: 0 auto;display:none;" id="{5}"></div>
    </div>
    <div id="divContent"></div>
</asp:Content>
