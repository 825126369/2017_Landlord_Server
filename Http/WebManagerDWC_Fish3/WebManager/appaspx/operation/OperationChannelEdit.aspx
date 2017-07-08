<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationChannelEdit.aspx.cs" Inherits="WebManager.appaspx.operation.OperationChannelEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
    <style type="text/css">
        .cLeft{float:left;
               border:1px solid orange;
               padding:5px;}

        .cRight{float:right; width:400px;border:1px solid gray;margin-left:10px;padding:5px;}
        .crl{clear:both;}
        .cChannel{width:1500px;margin:0px auto;overflow:auto;
        }

        .cLeft table{border:1px solid black;border-collapse:collapse;
                        background-color:#FFE4C4;
        }
        .cLeft td{padding:5px;width:200px;border:1px solid black;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <h2>渠道测试编辑</h2>
    <div class="cChannel">
      <%--   <div class="cLeft">
            <p>勾选要连接到测试服务器的渠道</p>
            <asp:Button ID="Button3" runat="server" Text="添加选中的测试渠道" OnClick="addTestChannel" />
            <asp:TreeView ID="trvAllChannel" runat="server" ShowCheckBoxes="All"></asp:TreeView>
            <asp:Button ID="Button1" runat="server" Text="添加选中的测试渠道" OnClick="addTestChannel" />
        </div> --%>

        <div class="cLeft">
            <h2>从这里选择要测式哪些渠道</h2>

            <h3>自定义渠道</h3>
            <asp:Button ID="Button1" runat="server" Text="添加选中的测试渠道" OnClick="addTestChannelSelf" />
            <asp:CheckBoxList ID="m_selfDefChannel" runat="server" RepeatColumns="5" RepeatDirection="Horizontal"></asp:CheckBoxList>

            <h3>内置渠道</h3>
            <asp:Button ID="Button3" runat="server" Text="添加选中的测试渠道" OnClick="addTestChannelBuilt" />
            <asp:CheckBoxList ID="m_builtInChanngel" runat="server" RepeatColumns="5" RepeatDirection="Horizontal"></asp:CheckBoxList>
        </div>

        <div class="cRight">
            <h1>当前连到测试服务器的渠道</h1>
            <asp:TreeView ID="testChannelList" runat="server" ShowCheckBoxes="All"></asp:TreeView>
            <asp:Button ID="Button2" runat="server" Text="删除" OnClick="delTestChannel" />
        </div>
        <div class="crl"></div>
    </div>
</asp:Content>
