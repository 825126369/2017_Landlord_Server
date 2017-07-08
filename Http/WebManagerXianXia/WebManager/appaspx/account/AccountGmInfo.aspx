<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountGmInfo.aspx.cs" Inherits="WebManager.appaspx.account.AccountGmInfo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .cSafeWidth td{font-size:14px;}
        .cSafeWidth table{border-collapse:collapse;}
        div.cSpliter{border:1px solid black; margin-top:2px;padding:5px;}
        .cTdLabel{width:200px;text-align:left;}
        .cTdMiddle{width:300px;}
        .cDetailInfo td{padding:2px;}
        .cDetailInfo table input{width:300px;height:20px;}
        #btnGameOn,#btnRight{margin:10px auto;width:60px;height:30px;}

        #apiLimit td{padding:8px;border-bottom:1px solid black;text-align:center;
                     width:200px;
        }
        #apiLimit tr:hover td{background:#f1f1f1;}
        #apiLimit td input[type=text]{width:100%;height:30px;}
        #apiLimit td input[type=button]{width:80%;height:40px;}
        #apiLimit span{font-size:16px;padding:10px;color:blue;cursor:pointer;}
    </style>
    <script src="../../Scripts/account/AccountGmInfo.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2 style="padding-bottom:20px;" runat="server" id="hSelfTitle">个人信息</h2>

        <div class="cSpliter">
            <h4>注册信息</h4>

            <table border="0" cellspacing="0" cellpadding="0" width="100%">
            	<tr>
            		<td class="cTdLabel"> 账号: </td>
                    <td class="cTdMiddle"> <asp:Label ID="m_acc" runat="server" Text="" /> </td>
                    <td>
                        <asp:HyperLink ID="linkModifyPwd" runat="server">修改密码</asp:HyperLink>
                    </td>
            	</tr>
                <tr>
            		<td>账号类型: </td>
                    <td> <asp:Label ID="m_accType" runat="server" Text="" /> </td>
            	</tr>
                <tr>
            		<td> 注册日期: </td>
                    <td> <asp:Label ID="m_createTime" runat="server" Text="" /> </td>
            	</tr>

                <tr>
            		<td> 上级账号: </td>
                    <td> <asp:Label ID="m_owner" runat="server" Text=""  /> </td>
            	</tr>
                
                <tr>
            		<td> ID号: </td>
                    <td> <asp:Label ID="m_id" runat="server" Text=""  /> </td>
            	</tr>
                
                <tr>
            		<td>别名: </td>
                    <td> <asp:Label ID="m_aliasName" runat="server" Text=""  /> </td>
                    <td>
                        <asp:HyperLink ID="linkModifyAliasName" runat="server">修改别名</asp:HyperLink>
                    </td>
            	</tr>

                <tr id="trDevKey" runat="server">
            		<td> 接入密钥: </td>
                    <td> <asp:Label ID="m_devKey" runat="server" Text=""  /> </td>
            	</tr>
                <tr id="trApiPostfix" runat="server">
            		<td> API后缀: </td>
                    <td> <asp:Label ID="m_postfix" runat="server" Text=""  /> </td>
            	</tr>
                 <tr id="trApiHome" runat="server">
            		<td> API首页: </td>
                    <td> <asp:Label ID="m_apiHome" runat="server" Text=""  /> </td>
                     <td>
                        <asp:HyperLink ID="linkModifyHome" runat="server">修改首页</asp:HyperLink>
                    </td>
            	</tr>
            </table>
        </div>

        <div class="cSpliter">
            <h4>余额信息</h4>
            <table border="0" cellspacing="0" cellpadding="0" width="100%">
                <tr>
            		<td class="cTdLabel"> 余额: </td>
                    <td> <asp:Label ID="m_remainMoney" runat="server" Text="" /> </td>
            	</tr>
            </table>

           <%--  <p> 总存款:<asp:Label ID="Label10" runat="server" Text=""></asp:Label> </p> --%>
        </div>

        <div class="cSpliter">
            <h4>账户信息</h4>
             <table border="0" cellspacing="0" cellpadding="0" width="100%">
            	<tr>
            		<td class="cTdLabel"> 状态: </td>
                    <td> <asp:Label ID="m_state" runat="server" Text="" /> </td>
            	</tr>
            </table>
        </div>

        <div runat="server" id="apiSetting">
            <div class="cSpliter" runat="server" id="gameOn">
                <h2>开放游戏</h2>
                请勾选需要开放的游戏:<br />
                <div id="gameList" runat="server"></div>
                <input type="button" value="提交" id="btnGameOn"/>
            </div>
            <div class="cSpliter" runat="server" id="apiLimit" clientidmode="Static">
                <h2>API可设置的下注上限</h2>
                <table>
                    <tr><td>游戏</td><td>当前可设置上限<span>刷新</span></td><td>新的可设置上限</td><td></td></tr>
                    <tr>
                        <td>骰宝</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" gameId="3" roomId="1"/></td>
                    </tr>
                    <tr>
                        <td>百家乐</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" gameId="5" roomId="1"/></td>
                    </tr>
                    <tr>
                        <td>万人牛牛</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" gameId="4" roomId="1"/></td>
                    </tr>
                    <tr>
                        <td>黑红梅方</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" gameId="10" roomId="1"/></td>
                    </tr>
                    <tr>
                        <td>鳄鱼大亨</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" gameId="2" roomId="1"/></td>
                    </tr>
                    <tr>
                        <td>捕鱼初级场</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" gameId="9" roomId="1"/></td>
                    </tr>
                    <tr>
                        <td>捕鱼中级场</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" gameId="9" roomId="2"/></td>
                    </tr>
                    <tr>
                        <td>捕鱼高级场</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" gameId="9" roomId="3"/></td>
                    </tr>
                    <tr>
                        <td>捕鱼VIP专场</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" gameId="9" roomId="4"/></td>
                    </tr>
                </table>
            </div>
        </div>

        <div class="cSpliter" id="awModify">
            <h2>代理占成洗码比修改</h2>
            <table>
                <tr>
                <td>代理占成:</td>
                <td>
                    <asp:TextBox ID="m_agentRatio" runat="server" CssClass="cTextBox"></asp:TextBox>%
                </td>
                <td><input type="button" value="提交修改" vid="#MainContent_m_agentRatio" op="2"/></td>
                <td>
                    <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="输入错误" 
                        ControlToValidate="m_agentRatio" Display="Dynamic" ForeColor="Red" 
                        MinimumValue="0" MaximumValue="100" Type="Double"></asp:RangeValidator>
                    0-100之间的小数或整数
                </td>
            </tr>
            <tr>
                <td>洗码比:</td>
                <td>
                    <asp:TextBox ID="m_washRatio" runat="server" CssClass="cTextBox"></asp:TextBox>%
                </td>
                <td><input type="button" value="提交修改" vid="#MainContent_m_washRatio" op="3"/></td>
                <td>
                    <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="输入错误" 
                        ControlToValidate="m_washRatio" Display="Dynamic" ForeColor="Red" 
                        MinimumValue="0" MaximumValue="1.2" Type="Double"></asp:RangeValidator>

                    0-1.2之间的小数
                </td>
            </tr>
            </table>
        </div>
        <div class="cSpliter" runat="server" id="rightOp">
            <h2>权限分配</h2>
            勾选权限:<br />
            <div id="rightGroup" runat="server"></div>
            <input type="button" value="提交" id="btnRight"/>
        </div>
    </div>
    
</asp:Content>
