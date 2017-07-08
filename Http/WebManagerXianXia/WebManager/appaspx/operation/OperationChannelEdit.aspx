<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationChannelEdit.aspx.cs" Inherits="WebManager.appaspx.operation.OperationChannelEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
    <style type="text/css">
        .cLeft{float:left;width:300px;border:1px solid gray;margin-left:5px;padding:5px;}
        .cRight{float:left; width:300px;border:1px solid gray;margin-left:10px;padding:5px;}
        .crl{clear:both;}
        .cChannel{width:700px;margin:0px auto;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <h2>渠道测试编辑</h2>
    <div class="cChannel">
        <div class="cLeft">
            <p>勾选要连接到测试服务器的渠道</p>
            <asp:Button ID="Button3" runat="server" Text="添加选中的测试渠道" OnClick="addTestChannel" />
            <asp:TreeView ID="trvAllChannel" runat="server" ShowCheckBoxes="All"></asp:TreeView>
            <asp:Button ID="Button1" runat="server" Text="添加选中的测试渠道" OnClick="addTestChannel" />
        </div>

        <div class="cRight">
            <p>当前连到测试服务的渠道</p>
            <asp:TreeView ID="testChannelList" runat="server" ShowCheckBoxes="All"></asp:TreeView>
            <asp:Button ID="Button2" runat="server" Text="删除" OnClick="delTestChannel" />
        </div>
    </div>
</asp:Content>
