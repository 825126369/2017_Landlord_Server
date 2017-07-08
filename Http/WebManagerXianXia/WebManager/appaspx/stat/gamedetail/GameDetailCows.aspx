<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GameDetailCows.aspx.cs" Inherits="WebManager.appaspx.stat.gamedetail.GameDetailCows" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>牛牛详情</title>
    <link href="../../../style/game_detail.css" rel="stylesheet" type="text/css"/>
</head>
<body>
    <form id="form1" runat="server">
    <div class="cFrame">
        <table>
            <tr>
                <td colspan="3" id="divHead" runat="server"></td>
            </tr>
            <tr>
                <td id="tdPlayer" runat="server"></td>
                <td id="tdIsBanker" runat="server" colspan="2"> </td>
            </tr>
            <tr>
                <td>
                    <h3>庄家牌型</h3>
                    <div id="divBankerCard" runat="server"></div>
                </td>
                <td colspan="2" id="tdBankerCardType" runat="server">
                </td>
            </tr>
            <tr>
                <td>
                    <h3>东牌型</h3>
                    <div id="divEastCard" runat="server"></div>
                </td>
                <td colspan="2" id="tdEastCardType" runat="server">
                </td>
            </tr>
            <tr>
                <td>
                    <h3>南牌型</h3>
                    <div id="divSouthCard" runat="server"></div>
                </td>
                <td colspan="2" id="tdSouthCardType" runat="server">
                </td>
            </tr>
            <tr>
                <td>
                    <h3>西牌型</h3>
                    <div id="divWestCard" runat="server"></div>
                </td>
                <td colspan="2" id="tdWestCardType" runat="server">
                </td>
            </tr>
            <tr>
                <td>
                    <h3>北牌型</h3>
                    <div id="divNorthCard" runat="server"></div>
                </td>
                <td colspan="2" id="tdNorthCardType" runat="server">
                </td>
            </tr>
            <tr>
                <td>押注区域</td><td>押注</td><td>得奖</td>
            </tr>
            <tr>
                <td>东</td><td id="tdEastBet" runat="server"></td><td id="tdEastWin" runat="Server"></td>
            </tr>
            <tr>
                <td>南</td><td id="tdSouthBet" runat="server" ></td><td id="tdSouthWin" runat="server"></td>
            </tr>
            <tr>
                <td>西</td><td id="tdWestBet" runat="server"></td><td id="tdWestWin" runat="server"></td>
            </tr>
            <tr>
                <td>北</td><td id="tdNorthBet" runat="server"></td><td id="tdNorthWin" runat="server"></td>
            </tr>
            <tr>
                <td>合计</td><td id="tdSumBet" runat="server"></td><td id="tdSumWin" runat="server"></td>
            </tr>
            <tr id="trServiceCharge" runat="server">
                <td>手续费</td><td id="tdServiceChargeRatio" runat="server"></td><td id="tdServiceCharge" runat="server"></td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
