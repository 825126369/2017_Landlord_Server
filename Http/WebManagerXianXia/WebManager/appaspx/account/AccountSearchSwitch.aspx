<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountSearchSwitch.aspx.cs" Inherits="WebManager.appaspx.account.AccountSearchSwitch" %>

<%@ Register Src="~/ascx/CommonSearchPanel.ascx" TagName="CommonSearchPanel" TagPrefix="myctrl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
   <%--  <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script> --%>
    <script src="../../Scripts/module/browser.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script> 
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
    <script src="../../Scripts/module/sea.js"></script>
	<script type="text/javascript">
	    $(function () {
	        $('#__gmAccountCascadeStaticTime').daterangepicker({ arrows: false });
	    });
	    seajs.use("../../Scripts/account/AccountProperty.js");
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div  class="cSafeWidth">
        <h2>下线管理</h2>
        <myctrl:CommonSearchPanel ID="m_searchCond" runat="server"></myctrl:CommonSearchPanel>

        <asp:Button ID="Button1" runat="server" Text="搜索" CssClass="cButton" onclick="onQueryMember"/>
        <div>
            <span id="m_levelStr" style="font-size:medium;color:black;font-weight:bold" runat="server"></span>
        </div>
    </div>
    <div class="container-fluid">
        <asp:Table ID="m_result" runat="server" CssClass="table table-hover table-bordered">
        </asp:Table>
    </div>

    <div id="divModifyName" class="PopDialog">
        <div class="container form-horizontal PopDialogBg">
            <h2 style="text-align:center;padding:10px;margin-bottom:10px;background:#ccc;cursor:pointer;" class="close">关闭</h2>
            <h3 style="text-align:center;padding:10px;margin-bottom:10px;">修改别名</h3>
            <div class="form-group">
                <label for="account" class="col-sm-2 control-label">账号:</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="txtAccount" disabled="disabled" /> 
                </div>
            </div> 
            <div class="form-group">
                <label for="account" class="col-sm-2 control-label">别名:</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="txtNewName" /> 
                </div>
            </div> 
            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                     <input type="button" value="提交修改" class="btn btn-primary form-control" id="btnModifyName" />
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                    <span id="opRes" />
                </div>
            </div>
        </div>
     </div>
</asp:Content>
