<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountMaxBetLimit.aspx.cs" Inherits="WebManager.appaspx.account.AccountMaxBetLimit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .cSafeWidth
        {
            width:1000px;
        }
        .cSafeWidth div{display:none;}
        .cSafeWidth table{border-collapse:collapse;}
        .cSafeWidth td{padding:8px;border-bottom:1px solid black;text-align:center;font-size:14px;width:200px;
                       font-size:16px;font-weight:bold;
                       word-break: break-all; word-wrap:break-word;
        }
        .cSafeWidth div[gameId='9'] td:nth-of-type(2){width:600px;}
        .cSafeWidth tr:hover td{background:#f1f1f1;}
        .cSafeWidth td input[type=text]{width:100%;height:30px;}
        .cSafeWidth td input[type=button]{width:80%;height:40px;}
        .cSafeWidth li{float:left;margin-right:10px;background:#f1f1f1;font-size:16px;font-weight:bold;
                       line-height:30px;height:30px;text-align:center;padding:10px;width:120px;cursor:pointer;
                       color:gray;
        }
        .cSafeWidth li.active{color:#000;background:#aaa;border:2px solid orange;}
        .cSafeWidth a{font-size:16px;background:#aaa;display:block;padding:10px;}
    </style>
    <script src="../../Scripts/account/AccountMaxBetLimit.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth" id="divBetLimitOp">
        <ul>
            <li class="active" gameId="3">骰宝</li><li gameId="5">百家乐</li><li gameId="4">万人牛牛</li><li  gameId="10">黑红梅方</li><li gameId="2">鳄鱼大亨</li><li gameId="9">捕鱼</li>
        </ul>
        <p class="clear"></p>

        <div style="display:block;" gameId="3">
            <a href="#">刷新数据到游戏服务器</a>
            <table>
                <tr><td>下注区域</td><td>当前限制</td><td>新的限制</td><td></td></tr>
                <tr><td>大</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="0" /></td></tr>
                <tr><td>豹子1</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="1" /></td></tr>
                <tr><td>豹子2</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="2"/></td></tr>
                <tr><td>豹子3</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="3"/></td></tr>
                <tr><td>豹子4</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="4"/></td></tr>
                <tr><td>豹子5</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="5"/></td></tr>
                <tr><td>豹子6</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="6"/></td></tr>
                <tr><td>任意豹子</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="7"/></td></tr>
                <tr><td>小</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="8"/></td></tr>
                <tr><td>点数4</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="9"/></td></tr>
                <tr><td>点数5</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="10"/></td></tr>
                <tr><td>点数6</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="11"/></td></tr>
                <tr><td>点数7</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="12"/></td></tr>
                <tr><td>点数8</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="13"/></td></tr>
                <tr><td>点数9</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="14"/></td></tr>
                <tr><td>点数10</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="15"/></td></tr>
                <tr><td>点数11</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="16"/></td></tr>
                <tr><td>点数12</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="17"/></td></tr>
                <tr><td>点数13</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="18"/></td></tr>
                <tr><td>点数14</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="19"/></td></tr>
                <tr><td>点数15</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="20"/></td></tr>
                <tr><td>点数16</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="21"/></td></tr>
                <tr><td>点数17</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="22"/></td></tr>
                <tr><td>1出现个数</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="23"/></td></tr>
                <tr><td>2出现个数</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="24"/></td></tr>
                <tr><td>3出现个数</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="25"/></td></tr>
                <tr><td>4出现个数</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="26"/></td></tr>
                <tr><td>5出现个数</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="27"/></td></tr>
                <tr><td>6出现个数</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改"area="28"/></td></tr>
            </table>
        </div>
        
        <div gameId="5">
            <a href="#">刷新数据到游戏服务器</a>
            <table>
                <tr><td>下注区域</td><td>当前限制</td><td>新的限制</td><td></td></tr>
                <tr><td>和</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="0" /></td></tr>
                <tr><td>闲</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="1"/></td></tr>
                <tr><td>闲对</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="2"/></td></tr>
                <tr><td>庄对</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="3" /></td></tr>
                <tr><td>庄</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="4"/></td></tr>
            </table>
        </div>

        <div gameId="4">
            <a href="#">刷新数据到游戏服务器</a>
             <table>
                <tr><td>下注区域</td><td>当前限制</td><td>新的限制</td><td></td></tr>
                <tr><td>东</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="0"/></td></tr>
                <tr><td>南</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="1"/></td></tr>
                <tr><td>西</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="2"/></td></tr>
                <tr><td>北</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="3"/></td></tr>
            </table>
        </div>

        <div gameId="10">
            <a href="#">刷新数据到游戏服务器</a>
            <table>
                <tr><td>下注区域</td><td>当前限制</td><td>新的限制</td><td></td></tr>
                <tr><td>黑桃</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="0"/></td></tr>
                <tr><td>红心</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="1" /></td></tr>
                <tr><td>梅花</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="2" /></td></tr>
                <tr><td>方块</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="3" /></td></tr>
                <tr><td>大小王</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="4"/></td></tr>
            </table>
        </div>

        <div gameId="2">
            <a href="#">刷新数据到游戏服务器</a>
            <table>
                <tr><td>下注区域</td><td>当前限制</td><td>新的限制</td><td></td></tr>
                <tr><td>所有区域</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="0"/></td></tr>
            </table>
        </div>

        <div gameId="9">
            <a href="#">刷新数据到游戏服务器</a>
            <table>
                <tr><td>房间</td><td>当前倍率列表</td><td>新的倍率列表</td><td></td></tr>
                <tr><td>初级场</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="1"/></td></tr>
                <tr><td>中级场</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="2"/></td></tr>
                <tr><td>高级场</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="3"/></td></tr>
                <tr><td>VIP专场</td><td></td><td><input type="text"/></td><td><input type="button" value="提交修改" area="4"/></td></tr>
            </table>
        </div>
    </div>
</asp:Content>
