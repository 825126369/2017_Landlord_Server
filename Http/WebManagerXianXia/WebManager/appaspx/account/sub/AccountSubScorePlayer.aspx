<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountSubScorePlayer.aspx.cs" Inherits="WebManager.appaspx.account.sub.AccountSubScorePlayer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../../Scripts/module/sea.js"></script>
    <script src="../../../Scripts/module/jquery.min.js"></script>
    <script src="../../../Scripts/module/bootstrap.min.js"></script>
    <script type="text/javascript">
        seajs.use('../../../Scripts/account/AccountScorePlayer.js');
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>会员上分/下分</h2>
                
        <div>
            <span id="m_levelStr" style="font-size:medium;color:black;font-weight:bold" runat="server"></span>
        </div>
        <asp:TextBox ID="m_curMoney" runat="server" CssClass="cTextBox" style="display:none"></asp:TextBox>
        <asp:TextBox ID="m_isAdmin" runat="server" CssClass="cTextBox" style="display:none"></asp:TextBox>
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
