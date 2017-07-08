<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationRechargePoint.aspx.cs" Inherits="WebManager.appaspx.operation.OperationRechargePoint" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_operation_common_m_time').daterangepicker({ arrows: false });
	    });
	</script>
    <style type="text/css">
        .rechargePoint{padding-left:10px;}
        .rechargePoint table:first-child td{padding:2px;}
         .cTable tr:hover td{background:#ddd;}
         .cTable td{font-size:12px;font-weight:bold;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <h2>付费点统计</h2>
    <div class="rechargePoint">
        <table>
            <tr>
                <td>时间:<asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px"></asp:TextBox></td>
            </tr>
            <tr>
                <td>渠道:<asp:DropDownList ID="m_channel" runat="server" style="width:130px;height:30px"></asp:DropDownList></td>
            </tr>
            <tr>
                <td><asp:Button ID="Button3" runat="server" onclick="onQuery" Text="查询" style="width:60px;height:30px" /></td>
            </tr>
        </table>

        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>
    </div>
</asp:Content>
