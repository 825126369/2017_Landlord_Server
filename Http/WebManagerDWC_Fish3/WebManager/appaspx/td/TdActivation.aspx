<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/td/TdCommon.master" AutoEventWireup="true" CodeBehind="TdActivation.aspx.cs" Inherits="WebManager.appaspx.td.TdActivation" %>
<asp:Content ID="Content2" ContentPlaceHolderID="tdHeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_td_common_m_time').daterangepicker({ arrows: false });
	    });
	</script>

    <style type="text/css">
        .cSafeWidth td{padding:5px;}
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="td_common" runat="server">
    <div class="cSafeWidth">
        <h2>留存相关</h2>

        <table>
            <tr>
                <td>时间:</td>
                <td><asp:TextBox ID="m_time" runat="server" style="width:400px;height:20px"></asp:TextBox></td>
            </tr>
            <tr>
                <td>渠道:</td>
                <td><asp:DropDownList ID="m_channel" runat="server" style="width:130px;height:30px"></asp:DropDownList></td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="Button3" runat="server" onclick="onQuery" Text="查询" style="width:60px;height:30px" />
                </td>
            </tr>
        </table>
    </div>
    <asp:Table ID="m_result" runat="server" CssClass="cTable"></asp:Table>
</asp:Content>
