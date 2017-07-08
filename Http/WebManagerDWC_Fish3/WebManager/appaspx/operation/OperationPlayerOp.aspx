<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationPlayerOp.aspx.cs" Inherits="WebManager.appaspx.operation.OperationPlayerOp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
    <script type="text/javascript" src="http://cdn.bootcss.com/jquery/1.11.1/jquery.min.js"></script>
    <script src="http://cdn.bootcss.com/bootstrap/3.3.0/js/bootstrap.min.js" type="text/javascript"></script>

    <script src="../../Scripts/operation/OperationPlayerOp.js?ver=1" type="text/javascript"></script>
    <style type="text/css">
        .cOp1 table{border-collapse:collapse}
        .cOp1 td{padding:10px;width:200px;border:1px solid black;text-align:center;
                 font-size:16px;
                 background:#ccf;
                 color:#000;
                 font-weight:bold;
        }
        .cOp1 input[type=text]{padding:1px;width:90%;height:20px;}
        .cOp1 input[type=button]{padding:10px;width:80px;height:40px;line-height:10px;
        }
        .cOp1 input.SingleOp{width:160px;}
        #logFish li{list-style:none;font-size:14px;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <div class="cSafeWidth">
        <h2 style="text-align:center;">玩家相关操作</h2>
        <div class="cOp1 cDiv1">
            <table>
                <tr>
                    <td>操作内容</td>
                    <td>玩家ID</td>
                    <td>数值</td>
                    <td></td>
                </tr>
                <tr>
                    <td>VIP经验</td>
                    <td><input type="text" id="playerExp"/></td>
                    <td><input type="text" id="valueExp"/></td>
                    <td>
                        <input type="button" id="btnAddExp" value="增加" player="playerExp" val="valueExp" opType="add" playerProp="vip" class="SingleOp" />
                    </td>
                </tr>
                <tr>
                    <td>金币</td>
                    <td><input type="text" id="playerGold"/></td>
                    <td><input type="text" id="valueGold"/></td>
                    <td>
                        <input type="button" id="btnAddGold" value="增加" player="playerGold" val="valueGold" opType="add" playerProp="gold"/>
                        <input type="button" id="btnDecGold" value="减少" player="playerGold" val="valueGold" opType="dec" playerProp="gold"/>
                    </td>
                </tr>

                <tr>
                    <td>钻石</td>
                    <td><input type="text" id="playerGem"/></td>
                    <td><input type="text" id="valueGem"/></td>
                    <td>
                        <input type="button" id="Button3" value="增加" player="playerGem" val="valueGem" opType="add" playerProp="gem"/>
                        <input type="button" id="Button4" value="减少" player="playerGem" val="valueGem" opType="dec" playerProp="gem"/>
                    </td>
                </tr>

                <tr>
                    <td>龙珠</td>
                    <td><input type="text" id="playerDragonBall"/></td>
                    <td><input type="text" id="valueDragonBall"/></td>
                    <td>
                        <input type="button" id="Button5" value="增加" player="playerDragonBall" val="valueDragonBall" opType="add"  playerProp="dragonBall"/>
                        <input type="button" id="Button6" value="减少" player="playerDragonBall" val="valueDragonBall" opType="dec"  playerProp="dragonBall"/>
                    </td>
                </tr>
                <tr>
                    <td>踢出</td>
                    <td><input type="text" id="playerKick"/></td>
                    <td></td>
                    <td>
                        <input type="button" id="Button7" value="踢出玩家" player="playerKick" opType="kick" playerProp=""/>
                    </td>
                </tr>
                <tr>
                    <td>新任务</td>
                    <td></td>
                    <td></td>
                    <td>
                        <input type="button" id="Button8" value="给所有玩家添加新任务"
                            class="SingleOp" opType="task" playerProp=""/>
                    </td>
                </tr>
                <tr>
                    <td>捕鱼LOG开关</td>
                    <td><input type="text" id="playerLog"/></td>
                    <td></td>
                    <td>
                        <input type="button" id="Button1" value="打开捕鱼log" 
                            class="SingleOp" player="playerLog" opType="logFish" playerProp="open"/>
                        <input type="button" id="Button2" value="关闭捕鱼log" 
                            class="SingleOp" player="playerLog" opType="logFish" playerProp="close"/>
                    </td>
                </tr>
                <tr>
                    <td>龙珠转出限制</td>
                    <td><input type="text" id="playerLimitSendDb"/></td>
                    <td></td>
                    <td>
                        <input type="button" id="Button9" value="限制转出" 
                            class="SingleOp" player="playerLimitSendDb" opType="LimitSendDb" playerProp="close"/>
                        <input type="button" id="Button10" value="取消限制" 
                            class="SingleOp" player="playerLimitSendDb" opType="LimitSendDb" playerProp="open"/>
                    </td>
                </tr>
            </table>
        </div>
        <div id="logFish" class="cOp1">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-lg-6">
                        <h3>当前打开捕鱼Log的玩家ID列表</h3>
                        <input id="logFishRefresh" type="button" value="刷新列表"/>
                        <ul></ul>
                    </div>
                    <div class="col-lg-6">
                        <h3>当前限制转出龙珠的玩家ID列表</h3>
                        <input id="logLimitDbRefresh" type="button" value="刷新列表"/>
                        <ul></ul>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
