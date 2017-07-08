<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/td/TdCommon.master" AutoEventWireup="true" CodeBehind="TdFirstRechargePointDistribution.aspx.cs" Inherits="WebManager.appaspx.td.TdFirstRechargePointDistribution" %>
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

	    seajs.use("../../Scripts/td/TdFirstRechargePointDistribution.js");
	</script>
    <style type="text/css">
        #divTemplate>div{padding:20px;max-width:1200px;height:400px;border:2px solid #000;border-radius:10px;
                         margin-top:10px;margin-left:auto;margin-right:auto;
        }
        .cSafeWidth li{list-style:none;float:left;width:100px;height:30px;line-height:30px;font-size:16px;text-align:center;
                       background:#aaa;margin-left:10px;color:#000;border-radius:5px;border:1px solid #000;
        }
        #playerType{float:left;}
        .cSafeWidth li:hover{background:orange}
        .cSafeWidth .Active{background:orange}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="td_common" runat="server">
    <div class="cSafeWidth">
        <h2>首付计费点分布</h2>
        <table style="float:left;">
            <tr>
                <td>时间:</td>
                <td><input type="text" style="width:400px;height:20px" id="txtTime"/></td>
                <td colspan="2">
                    <input type="button" value="查询" style="width:60px;height:30px" id="btnQuery"/>
                </td>
            </tr>
        </table>
        <ul id="playerType">
            <li class="Active" pType="3">新增玩家</li><li pType="1">活跃玩家</li>
        </ul>
        <div class="clear"></div>
    </div>

    <div id="divTemplate" style="display:none">
        <h2 style="text-align:center;background:#ccc;padding:6px;font-size:30px;" id="{0}"></h2>
        <div style="width:800px; min-height: 300px; margin: 0 auto;display:none;" id="{1}"></div>
    </div>
    <div id="divContent"></div>
</asp:Content>
