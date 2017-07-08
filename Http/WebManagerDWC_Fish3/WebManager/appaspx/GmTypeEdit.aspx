<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GmTypeEdit.aspx.cs" Inherits="WebManager.appaspx.GmTypeEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="http://cdn.hcharts.cn/jquery/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="../Scripts/module/sea.js"></script>
    
    <script type="text/javascript">
        seajs.use("../Scripts/GmTypeEdit.js");
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2 style="text-align:center;">GM账号类型编辑</h2>
        <table>
            <tr>
                <td>账号类型名称:</td>
                <td><input type="text" id="gmType"/></td>
                <td><input type="button" id="btnAdd" value="添加"/></td>
            </tr>
        </table>
        
        <h4>现有账号类型列表:</h4>
        <table id="gmList" class="cTable"></table>
    </div>
</asp:Content>
