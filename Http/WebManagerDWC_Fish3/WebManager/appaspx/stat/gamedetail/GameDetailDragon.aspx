<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GameDetailDragon.aspx.cs" Inherits="WebManager.appaspx.stat.gamedetail.GameDetailDragon" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>五龙详情</title>
    <link href="../../../style/game_detail.css" rel="stylesheet" type="text/css"/>
</head>
<body>
    <div class="cFrame">
        <table>
            <tr>
                <td colspan="2" id="divHead" runat="server"></td>
            </tr>
            <tr>
                <td id="tdPlayer" colspan="2" runat="server"></td>
            </tr>
            <tr>
                <td colspan="2">开牌结果</td>
            </tr>
            <tr class="cTrAve">
                <td>是否为Free game</td>
                <td runat="server" id="tdIsFreeGame"></td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="cDragResult">
                        <div id="divResult1" runat="server"></div>
                        <div id="divResult2" runat="server"></div>
                        <div id="divResult3" runat="server"></div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>押注金额</td>
                <td runat="server" id="tdBetMoney"></td>
            </tr>
        </table>
        <asp:Table ID="tableBet" runat="server"></asp:Table>
    </div>
</body>
</html>
