<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="ServiceBenefits.aspx.cs" Inherits="WebManager.appaspx.service.ServiceBenefits" %>
<asp:Content ID="Content1" ContentPlaceHolderID="service_common" runat="server">
    <h2>发放福利</h2>
    <p>发放金币，钻石，礼券时，可以为负，此时是减玩家的金币，钻石，礼券</p>
    <p>发放道具格式：道具ID + 空格 + 道具数量</p>
    <p>给指定用户发放福利时，需要输入用户ID，其他方式不用</p>
    <p>发放道具时，可输入列表，如 75 1;95 1;115 1 表示发海盗炮，海盗子弹，海弹渔网 各1个</p>
    <p>76 1;96 1;116 1 表示发土豪金炮台，土豪金子弹，土豪金渔网各1个</p>
    平台:&nbsp;&nbsp;<asp:DropDownList ID="m_platform" runat="server" style="width:200px;height:30px"></asp:DropDownList>
    <br /><br />
    发放目标:&nbsp;&nbsp;<asp:DropDownList ID="m_target" runat="server" style="width:200px;height:30px"></asp:DropDownList>
    <br /><br />
    福利类型:&nbsp;&nbsp;<asp:DropDownList ID="m_type" runat="server" style="width:200px;height:30px"></asp:DropDownList>
    <br /><br />
    发放参数:&nbsp;&nbsp;<asp:TextBox ID="m_param" runat="server" style="width:219px;height:20px"></asp:TextBox>
    <br /><br />
    用户ID:&nbsp;&nbsp;<asp:TextBox ID="m_playerId" runat="server" style="width:800px;height:20px"></asp:TextBox>
    <%--asp:Button ID="Button2" runat="server" onclick="onPaste" Text="粘贴" style="width:133px;height:25px"--%>
    <br /><br />
    给所有用户发放福利时，可以指定等级条件。当给VIP用户发放福利时，这里表示VIP等级， 1:白银VIP 2：黄金VIP, 3:钻石VIP 不填时，默认>=1
    <br />
    发放条件，等级>=&nbsp;&nbsp;<asp:TextBox ID="m_level" runat="server" style="width:240px;height:20px"></asp:TextBox>
    <br /><br />
    <asp:Button ID="Button1" runat="server" onclick="onGrant" Text="确定" style="width:133px;height:25px"/>
    <br />
    <br />
    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
</asp:Content>
