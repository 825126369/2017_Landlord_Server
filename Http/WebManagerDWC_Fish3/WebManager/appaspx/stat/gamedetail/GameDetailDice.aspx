<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GameDetailDice.aspx.cs" Inherits="WebManager.appaspx.stat.gamedetail.GameDetailDice" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>骰宝详情</title>
    <link href="../../../style/game_detail.css" rel="stylesheet" type="text/css"/>
</head>
<body>
    <form id="form1" runat="server">
    <div class="cFrame">
        <table>
            <tr>
                <td colspan="2" id="divHead" runat="server"></td>
            </tr>
            <tr>
                <td id="tdPlayerCrocodile" colspan="2" runat="server"></td>
            </tr>
            <tr class="cTrAve">
                <td>骰子</td>
                <td id="tdDiceResult" runat="server">
                </td>
            </tr>
            <tr class="cTrAve">
                <td>结果描述</td>
                <td id="tdDiceDesc" runat="server">
                </td>
            </tr>
        </table>
        <asp:Table ID="tableBet" runat="server"></asp:Table>
    </div>
    </form>
</body>
</html>
