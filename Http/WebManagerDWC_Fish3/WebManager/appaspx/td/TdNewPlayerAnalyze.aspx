<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/td/TdCommon.master" AutoEventWireup="true" CodeBehind="TdNewPlayerAnalyze.aspx.cs" Inherits="WebManager.appaspx.td.TdNewPlayerAnalyze" %>
<asp:Content ID="Content1" ContentPlaceHolderID="tdHeadContent" runat="server">
    <link rel="stylesheet" type="text/css" media="all" href="../../Scripts/datepicker/daterangepicker.css" />
    <script type="text/javascript" src="http://cdn.bootcss.com/jquery/1.11.1/jquery.min.js"></script>
    <script type="text/javascript" src="../../Scripts/datepicker/moment.min.js"></script>
    <script type="text/javascript" src="../../Scripts/datepicker/daterangepicker.js"></script>
    <script src="../../Scripts/module/sea.js" type="text/javascript"></script>
	<script type="text/javascript">
	    $(function () {
	        $('#txtTime').daterangepicker();
	    });

	    seajs.use("../../Scripts/td/TdNewPlayerAnalyze.js");
	</script>
    <style type="text/css">
        #txtTime{width:400px;height:30px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="td_common" runat="server">
    <div class="cSafeWidth">
        <h2 style="text-align:center;padding:10px;">新进用户分析</h2>
        <table>
            <tr>
                <td>时间:</td>
                <td><input type="text" id="txtTime"/></td>
            </tr>
        </table>
        <ul class="SelCard" style="margin-top:10px;">
            <li class="Active" data="1">炮数成长分布</li><li data="2">金币下注分布</li><li data="3">捕鱼活跃统计</li><li data="5">登录房间次数</li>
        </ul>
    </div>
    <div class="clear"></div>
    <table id="m_result" class="cTable Report"></table>
</asp:Content>
