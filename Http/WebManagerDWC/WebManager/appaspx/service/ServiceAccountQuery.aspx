<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="ServiceAccountQuery.aspx.cs" Inherits="WebManager.appaspx.service.ServiceAccountQuery" %>
<asp:Content ID="Content2" ContentPlaceHolderID="serviceHeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_service_common_m_regTime').daterangepicker({ arrows: false });
	    });
	</script>
    <style type="text/css">
        .cSafeWidth td{padding:6px;}
        .cSafeWidth input[type=text]{width:200px;height:30px;}
        .cSafeWidth input[type=submit]{width:60px;height:30px;}
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="service_common" runat="server">
    <div class="cSafeWidth">
        <h2>账号查询</h2>
        <div>
            <table>
                <tr>
                    <td>查询方式:</td>
                    <td>
                        <asp:DropDownList ID="m_queryWay" runat="server" style="width:180px;height:30px"></asp:DropDownList>
                        参数:<asp:TextBox ID="m_param" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>注册时间：</td>
                    <td>
                        <asp:TextBox ID="m_regTime" runat="server"></asp:TextBox>
                        <asp:Button ID="Button1" runat="server" onclick="onQueryAccount" Text="查询"/>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <asp:Table ID="m_result" runat="server" CssClass="cTable">
    </asp:Table>
    <%-- <asp:Button ID="BtnBlockAcc" runat="server" onclick="onBlockAcc" Text="停封账号" style="width:133px;height:25px" visible="false" /> --%>
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    <br />
    <br />
    <span id="m_page" style="text-align:center;display:block" runat="server"></span>
    <br />
    <span id="m_foot" style="font-size:x-large;text-align:center;display:block" runat="server"></span>
</asp:Content>
