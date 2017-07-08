<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountScore.aspx.cs" Inherits="WebManager.appaspx.account.AccountScore" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../Scripts/module/sea.js"></script>
    <script type="text/javascript">
        seajs.use('../../Scripts/account/AccountScorePlayer.js');
    </script>

    <script type="text/javascript">
        function changeSubmitUrl() {
            var url = "/appaspx/account/AccountScore.aspx?action=0";
            var form = document.forms["Form1"];
            form.action = url;
        }
    </script>
    <style type="text/css">
        .cTableSpecAcc td{padding:5px;}
        input[name="__search"]{width:110px;height:30px;}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>代理上分/下分</h2>
        <table class="cTableSpecAcc">
            <tr>
                <td>账号:</td>
                <td><asp:TextBox ID="m_acc" runat="server" style="width:300px;height:20px"></asp:TextBox></td>
            </tr>
            <tr>
                <td>账号类型:</td>
                <td><asp:DropDownList ID="m_type" runat="server" CssClass="cDropDownList" style="width:300px;height:25px"></asp:DropDownList></td>
            </tr>
        </table>
        <input type="submit" name="__search" value="搜索" onclick="javascript:changeSubmitUrl();"/>

        <div>
            <span id="m_levelStr" style="font-size:medium;color:black;font-weight:bold" runat="server"></span>
        </div>        
    </div>
    <div class="container-fluid">
        <asp:Table ID="m_result" runat="server" CssClass="table table-hover table-bordered">
        </asp:Table>
    </div>

    <div class="modal fade" id="_scoreOpConfirmDlg" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="_scoreOpConfirmDlgTitle" ></h4>
                </div>
                <div class="modal-body" id="_scoreOpConfirmDlgContent"></div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default Cancel" data-dismiss="modal">取消</button>
                    <button type="button" class="btn btn-primary Ok" data-dismiss="modal">确定</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
