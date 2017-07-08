<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationSpeaker.aspx.cs" Inherits="WebManager.appaspx.operation.OperationSpeaker" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/DateRange/js/jquery-1.3.1.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/jquery-ui-1.7.1.custom.min.js"></script>
	<script type="text/javascript" src="/Scripts/DateRange/js/daterangepicker.jQuery.js"></script>
	<link rel="stylesheet" href="/Scripts/DateRange/css/ui.daterangepicker.css" type="text/css" />
	<link rel="stylesheet" href="/Scripts/DateRange/css/redmond/jquery-ui-1.7.1.custom.css" type="text/css" title="ui-theme" />
	<script type="text/javascript">
	    $(function () {
	        $('#MainContent_operation_common_txtSendTime').daterangepicker({ arrows: false });
	    });
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <div class="cSafeWidth">
        <h2>通告消息</h2>
        这里的通告消息，将出现在下面所示的通告栏
        <div>
            <img alt="" src="../../image/notify.png" width="400" height="100"/>
        </div>

        <div>
            显示内容:<asp:TextBox ID="txtContent" runat="server" CssClass="cTextBox"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                ErrorMessage="不可为空" ForeColor="Red" ControlToValidate="txtContent" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>
        
        <div>
            发送时间:<asp:TextBox ID="txtSendTime" runat="server" CssClass="cTextBox"></asp:TextBox>
           
            <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
                ErrorMessage="时间格式非法" ForeColor="Red" ControlToValidate="txtSendTime" Display="Dynamic"
                ValidationExpression="^\s*(\d{4})/(\d{1,2})/(\d{1,2})\s+(\d{1,2}):(\d{1,2})$">
            </asp:RegularExpressionValidator>

             可预设什么时候发到通告栏。不填立即发送。完整格式:2015/12/29 12:30
        </div>

        <div>
            重复次数:<asp:TextBox ID="txtRepCount" runat="server" CssClass="cTextBox"></asp:TextBox>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                ErrorMessage="只能输入数字" ForeColor="Red" ControlToValidate="txtRepCount" Display="Dynamic" 
                ValidationExpression="\d*"></asp:RegularExpressionValidator>
            默认为1
        </div>

        <div>
        <%--     重复间隔:<asp:TextBox ID="txtInterval" runat="server" CssClass="cTextBox"></asp:TextBox>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                ErrorMessage="只能输入数字" ForeColor="Red" ControlToValidate="txtInterval" Display="Dynamic" 
                ValidationExpression="\d*"></asp:RegularExpressionValidator>
            秒, 默认为1 --%>
        </div>

        <div>
            <asp:Button ID="btnSend" runat="server" Text="发送" CssClass="cButton" OnClick="btnSend_Click" />
            <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
        </div>
    </div>
</asp:Content>
