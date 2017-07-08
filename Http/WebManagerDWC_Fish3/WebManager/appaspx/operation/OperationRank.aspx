<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationRank.aspx.cs" Inherits="WebManager.appaspx.operation.OperationRank" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
    <script src="../../Scripts/module/sea.js" type="text/javascript"></script>
	<script type="text/javascript">
	    $(function () {
	        $('#time').daterangepicker({ arrows: false });
	    });
        seajs.use("../../Scripts/operation/OperationRank.js");
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <div class="cSafeWidth">
        <h2 style="text-align:center;padding-bottom:10px;">排行榜</h2>
        <table class="SearchCondition">
            <tr>
                <td>排行方式:</td>
                <td>
                    <select id="rankWay">
                        <option value="0">增长</option>
                        <option value="1">净增长</option>
                    </select>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                     <select id="rankSel">
                        <option value="0">金币榜</option>
                        <option value="1">钻石榜</option>
                        <option value="2">龙珠榜</option>
                        <option value="3">碎片榜</option>
                    </select>
                </td>
            </tr>
            <tr>
                <td>时间:</td>
                <td><input type="text" id="time"/></td>
                <td><input type="button"  id="btnQuery" value="查询"/></td>
            </tr>
        </table>
    </div>
  <%--   时间:<asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px"></asp:TextBox>
    <asp:Button ID="Button3" runat="server" onclick="onSearch" Text="查询" style="width:100px;height:30px" /> --%>
   <%--  <asp:Button ID="Button1" runat="server" onclick="onClearRank" Text="清空排行" style="width:100px;height:30px" 
         OnClientClick="return confirm('确认清空排行榜?')"/> --%>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>

    <div id="resultDiv"></div>
    <asp:Table ID="m_result" runat="server" CssClass="result">
    </asp:Table>
</asp:Content>
