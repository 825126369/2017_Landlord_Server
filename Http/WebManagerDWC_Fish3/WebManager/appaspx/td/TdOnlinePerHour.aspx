<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/td/TdCommon.master" AutoEventWireup="true" CodeBehind="TdOnlinePerHour.aspx.cs" Inherits="WebManager.appaspx.td.TdOnlinePerHour" %>
<asp:Content ID="Content1" ContentPlaceHolderID="tdHeadContent" runat="server">
    <script type="text/javascript" src="http://cdn.hcharts.cn/jquery/jquery-1.8.3.min.js"></script>

	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />

    <script src="http://cdn.hcharts.cn/highcharts/highcharts.js" type="text/javascript"></script>

    <script src="../../Scripts/module/sea.js" type="text/javascript"></script>
	<script type="text/javascript">
	    $(function () {
	        $('#m_time').daterangepicker({ arrows: false });
	    });

	    seajs.use("../../Scripts/td/TdOnlinePerHour.js?ver=1");
	</script>
    <style type="text/css">
        .cSafeWidth li{list-style:none;float:left;width:80px;height:30px;line-height:30px;font-size:16px;text-align:center;
                       background:#aaa;margin-left:10px;color:#000;border-radius:5px;border:1px solid #000;
        }
        #optionGame{float:left;}
        .cSafeWidth li:hover{background:orange}
        .cSafeWidth .Active{background:orange}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="td_common" runat="server">
    <div class="cSafeWidth" style="width:1000px;">
        <h2>实时在线</h2>

        <table style="float:left;">
            <tr>
                <td>时间:</td>
                <td><input id="m_time" style="width:300px;height:20px" /></td>
                <td colspan="2">
                    <input type="button" id="onQuery" value="查询" style="width:60px;height:30px" />
                </td>
            </tr>
        </table>
        <ul id="optionGame">
            <li class="Active" gameId="0">总体</li><li class="" gameId="1">捕鱼</li><li gameId="2">鳄鱼大亨</li><li gameId="4">牛牛</li><li gameId="6">五龙</li><li gameId="10">黑红梅方</li>
        </ul>
        <div class="clear"></div>
    </div>

    <table id="m_result" class="cTable"></table>
    
    <div id="divTemplate" style="display:none;">
        <div id="{0}" style="max-width:1200px;min-height:400px; margin:10px auto;border:1px solid #000;border-radius:10px;padding:10px;"></div>
    </div>
    
    <div id="divContent" style="padding:10px;"></div>
</asp:Content>
