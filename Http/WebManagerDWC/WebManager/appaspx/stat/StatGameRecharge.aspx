<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/stat/StatCommon.master" AutoEventWireup="true" CodeBehind="StatGameRecharge.aspx.cs" Inherits="WebManager.appaspx.stat.StatGameRecharge" %>
<asp:Content ID="Content1" ContentPlaceHolderID="stat_comm_HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(document).ready(function ()
	    {
	        $('#MainContent_stat_common_m_time').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="stat_common" runat="server">
    <div class="container form-horizontal">
        <h2 style="text-align:center;padding:20px;">游戏每天收入统计</h2>
        <div class="form-group">
            <label for="account" class="col-sm-2 control-label">时间区间：</label>
            <div class="col-sm-10">
              <asp:TextBox ID="m_time" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <div class="col-sm-offset-2 col-sm-10">
                <asp:Button ID="Button3" runat="server" onclick="onStat" Text="统计" class="btn btn-primary form-control" />
            </div>
        </div>

        <asp:Table ID="m_result" runat="server" CssClass="table table-hover table-bordered">
        </asp:Table>
    </div>
</asp:Content>
