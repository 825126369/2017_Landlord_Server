<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationWeekChampionSetting.aspx.cs" Inherits="WebManager.appaspx.operation.OperationWeekChampionSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
    <style type="text/css">
        .container{width:960px;padding:2px;margin:0 auto;}
        .left, .right{border:1px dashed black;padding:4px;width:440px;margin-top:6px;}
        .left{float:left;}
        .right{float:right;}
        .container input[type=text]{width:200px;height:20px;}
        .container input[type=button]{width:64px;height:30px;}
    </style>
    <script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="../../Scripts/operation/OperationWeekChampionSetting.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <div class="container">
        <h2 style="text-align:center;">大奖赛冠军设置</h2>
        <div class="left">
            <h3>安全账号</h3>
            <table>
                <tr>
                    <td>玩家ID:</td>
                    <td><input type="text" id="playerId" name="playerId"/></td>
                </tr>
                <tr>
                    <td>
                        <input type="button" id="btnAdd" value="增加"/>
                        <input type="button" id="btnRemove" value="删除"/>
                    </td>
                </tr>
            </table>
            <asp:Table ID="m_result" runat="server" CssClass="cTable"></asp:Table>
        </div>

        <div class="right">
            <h3>修改成绩</h3>
            <table>
                <tr>
                    <td>安全玩家ID:</td>
                    <td><input type="text" id="scorePlayerId" name="scorePlayerId"/></td>
                </tr>
                <tr>
                    <td>分数:</td>
                    <td><input type="text" id="score" name="score"/></td>
                </tr>
                <tr>
                     <td>
                        <input type="button" id="btnModify" value="修改"/>
                    </td>
                </tr>
            </table>
        </div>

        <div class="clear"></div>
    </div>
</asp:Content>
