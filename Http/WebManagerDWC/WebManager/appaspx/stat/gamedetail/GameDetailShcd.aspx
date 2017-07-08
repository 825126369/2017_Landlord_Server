<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GameDetailShcd.aspx.cs" Inherits="WebManager.appaspx.stat.gamedetail.GameDetailShcd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>黑红梅方详情</title>
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
                <td id="tdPlayer" colspan="2" runat="server"></td>
            </tr>
            <tr class="cTrAve">
                <td>开牌结果</td>
                <td><div id="divNormalResult" runat="server"></div></td>
            </tr>
        </table>
        <asp:Table ID="tableBet" runat="server"></asp:Table>
    </div>
    </form>
</body>
</html>
