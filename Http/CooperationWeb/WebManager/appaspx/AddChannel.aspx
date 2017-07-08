<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddChannel.aspx.cs" Inherits="WebManager.appaspx.AddChannel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="masterMainContent" runat="server">
    <div class="cSafeWidth">
        <h2>添加渠道</h2>
        <div style="border:1px solid black;padding:5px;">
            <div>渠道编号:</div>
            <div>
                <asp:TextBox ID="txtChannelNo" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                    ErrorMessage="渠道编号不能为空" ControlToValidate="txtChannelNo" ForeColor="Red" 
                    ValidationGroup="channelGroup"></asp:RequiredFieldValidator>
            </div>

            <div>渠道名称:</div>
            <div>
                <asp:TextBox ID="txtChannelName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                    ErrorMessage="渠道名称不能为空" ControlToValidate="txtChannelName" ForeColor="Red" 
                    ValidationGroup="channelGroup"></asp:RequiredFieldValidator>
            </div>
            <asp:Button ID="Button1" runat="server" Text="添加"  CssClass="cButton" OnClick="Button1_Click" ValidationGroup="channelGroup"/>
            <span id="m_opRes" style="font-size:medium;color:red" runat="server"></span>
        </div>

        <div style="border:1px solid black;padding:5px;margin-top:5px;">
            当前所有渠道:
            <asp:Table ID="tabChannel" runat="server" CssClass="cTable"></asp:Table>
            <asp:Button ID="btnDel" runat="server" Text="删除"  CssClass="cButton" OnClick="btnDel_Click" />
        </div>
    </div>
</asp:Content>
