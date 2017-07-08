<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationViewInformHead.aspx.cs" Inherits="WebManager.appaspx.operation.OperationViewInformHead" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
    <script type="text/javascript" src="http://cdn.bootcss.com/jquery/1.11.1/jquery.min.js"></script>
    <script src="../../Scripts/module/sea.js" type="text/javascript"></script>
    <script type="text/javascript">
        seajs.use("../../Scripts/operation/OperationViewInformHead.js?ver=1");
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <div class="cSafeWidth" style="text-align:center;">
        <h2 style="text-align:center;">头像举报</h2>
        <br />
        <input type="button" class="btn btn-primary" value="全选" id="btnSelAll"/>
        <input type="button" class="btn btn-primary" value="取消选择" id="btnCancelSelAll"/>
        <input type="button" class="btn btn-success" value="审核通过选择的头像" id="btnPass"/>
        <asp:Table ID="m_result" runat="server" CssClass="cTable">
        </asp:Table>
       <%--  <asp:Button ID="Button1" runat="server" Text="删除选择" style="width:70px;height:35px;" 
            onclick="onDelPlayer"/> --%>
    </div>
</asp:Content>
