<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountPlayerOp.aspx.cs" Inherits="WebManager.appaspx.account.AccountPlayerOp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../Scripts/account/AccountScore.js" type="text/javascript"></script>
    <script src="../../Scripts/account/AccountPlayerOp.js" type="text/javascript"></script>

    <style type="text/css">
        .cOp{ margin-top:20px;}
        .cOp input{width:110px;height:30px;padding:5px;margin-right:20px;font-size:12px;}
        .cOp1{border:1px solid black;padding:5px;margin-top:2px;}
        .cOp1 td{padding:2px;}
        .cOpenClose{width:40px;height:20px;font-size:10px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>会员快捷操作</h2>
        <%-- <input id="btnOpenClose1" type="button" value="切换" class="cOpenClose" /> --%>
        <div class="cOp1 cDiv1">
            <table>
                <tr>
                    <td>玩家账号:</td>
                    <td><asp:TextBox ID="m_acc" runat="server" CssClass="cTextBox"></asp:TextBox></td>
                    <td>分数:</td>
                    <td><asp:TextBox ID="m_score" runat="server" CssClass="cTextBox"></asp:TextBox></td>
                    <td><asp:Button ID="Button7" runat="server" Text="上分" onclick="onAddScore" 
                        OnClientClick="return confirmScoreInfo(true, 'player')" />
                        <asp:Button ID="Button8" runat="server" Text="下分" onclick="onDecScore" 
                            OnClientClick="return confirmScoreInfo(false, 'player')"/>
                        <span id="m_scoreRes" style="font-size:medium;color:red" runat="server"></span>
                    </td>
                </tr>
            </table>
            <div class="cOp">
                <asp:Button ID="Button1" runat="server" Text="查询信息" onclick="onQueryPlayerInfo" />
                <asp:Button ID="Button2" runat="server" Text="踢出玩家" onclick="onKickPlayer" />
                <asp:Button ID="Button3" runat="server" Text="解锁玩家" onclick="onUnlockPlayer" />
                <asp:Button ID="Button4" runat="server" Text="清理登录失败次数" onclick="onClearLoginFailed" />
                <asp:Button ID="Button5" runat="server" Text="停封账号" onclick="onBlockAcc" />
                <asp:Button ID="Button6" runat="server" Text="解封账号" onclick="onUnBlockAcc" />
                <asp:Button ID="Button10" runat="server" Text="影响盈利率" onclick="onAffectRate" />
                <asp:Button ID="Button11" runat="server" Text="不影响盈利率" onclick="onUnAffectRate" />
            </div>
            <asp:Table ID="m_result" runat="server" CssClass="cTable">
            </asp:Table>
            <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
        </div>

       <%-- <div><input id="btnOpenClose2" type="button" value="切换" class="cOpenClose"  /></div>--%>
        <div class="cOp1 cDiv2">
            <h3>更改密码</h3>
            <table border="0" cellspacing="0" cellpadding="0" width="100%">
            	<tr>
            		<td>玩家账号:</td>
                    <td><asp:TextBox ID="m_acc1" runat="server" CssClass="cTextBox"></asp:TextBox></td>
            	</tr>
                <tr>
            		<td>新密码:</td>
                    <td><asp:TextBox ID="m_pwd1" runat="server" CssClass="cTextBox"></asp:TextBox></td>

                     <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^[0-9a-zA-Z]{6,14}$" ControlToValidate="m_pwd1"></asp:RegularExpressionValidator>
            	</tr>
            </table>
            <asp:Button ID="Button9" runat="server" Text="重置密码" onclick="onResetPlayerPwd" CssClass="cButton" />
            <span id="m_res1" style="font-size:medium;color:red" runat="server"></span>
        </div>

        <asp:TextBox ID="m_curMoney" runat="server" CssClass="cTextBox" style="display:none"></asp:TextBox>
        <asp:TextBox ID="m_isAdmin" runat="server" CssClass="cTextBox" style="display:none"></asp:TextBox>
    </div>
</asp:Content>
