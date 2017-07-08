<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GameDetailBaccarat.aspx.cs" Inherits="WebManager.appaspx.stat.gamedetail.GameDetailBaccarat" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>百家乐详情</title>
    <link href="../../../style/game_detail.css" rel="stylesheet" type="text/css"/>
</head>
<body>
    <form id="form1" runat="server">
    <div class="cFrame">
        <table>
            <tr>
                <td colspan="3" id="divHead" runat="Server"></td>
            </tr>
            <tr>
                <td id="tdPlayer" runat="Server"></td>
                <td id="tdIsBanker" runat="Server" colspan="2"> </td>
            </tr>
            <tr>
                <td>
                    <h3>庄家牌型</h3>
                    <div id="divBankerCard" runat="Server"></div>
                </td>
                <td colspan="2">
                    <h3>闲家牌型</h3>
                    <div id="divPlayerCard" runat="Server"></div>
                </td>
            </tr>
            <tr>
                <td>押注区域</td><td>押注</td><td>得奖</td>
            </tr>
            <tr>
                <td>庄</td><td id="tdZhuangBet" runat="server"></td><td id="tdZhuangWin" runat="Server"></td>
            </tr>
            <tr>
                <td>闲</td><td id="tdXianBet" runat="server" ></td><td id="tdXianWin" runat="server"></td>
            </tr>
            <tr>
                <td>和</td><td id="tdHeBet" runat="server"></td><td id="tdHeWin" runat="server"></td>
            </tr>
            <tr>
                <td>庄对</td><td id="tdZhuangDuiBet" runat="server"></td><td id="tdZhuangDuiWin" runat="server"></td>
            </tr>
            <tr>
                <td>闲对</td><td id="tdXianDuiBet" runat="server"></td><td id="tdXianDuiWin" runat="server"></td>
            </tr>
            <tr>
                <td>合计</td><td id="tdSumBet" runat="server"></td><td id="tdSumWin" runat="server"></td>
            </tr>
            <tr id="trTotalIncome" runat="server">
                <td>总收益</td><td id="tdTotalIncome" runat="server" colspan="2"></td>
            </tr>
            <tr id="trServiceCharge" runat="server">
                <td>手续费</td><td id="tdServiceChargeRatio" runat="server"></td><td id="tdServiceCharge" runat="server"></td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
