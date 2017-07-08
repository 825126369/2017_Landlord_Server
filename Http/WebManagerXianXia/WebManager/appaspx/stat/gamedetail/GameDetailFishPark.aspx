<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GameDetailFishPark.aspx.cs" Inherits="WebManager.appaspx.stat.gamedetail.GameDetailFishPark" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>捕鱼详情</title>
    <link href="../../../style/game_detail.css" rel="stylesheet" type="text/css"/>
</head>
<body>
    <form id="form1" runat="server">
    <div class="cFrame">
        <table>
            <tr>
                <td id="divHead" runat="server"></td>
            </tr>
            <tr>
                <td id="tdPlayerCrocodile" runat="server"></td>
            </tr>
            <tr>
                <td id="tdAbandonedbullets" runat="server"></td>
            </tr>
        </table>
        <asp:Table ID="tableFish" runat="server"></asp:Table>
    </div>
    </form>
</body>
</html>
