<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="AddAccount.aspx.cs" Inherits="WebManager.appaspx.AddAccount" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="http://cdn.hcharts.cn/jquery/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="../Scripts/module/sea.js"></script>
    
    <script type="text/javascript">
        seajs.use("../Scripts/AddAccount.js");
	</script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="cSafeWidth">
        <h2>添加账号</h2>
        <div>
            <table>
                <tr>
                    <td>账号:</td>
                    <td><asp:TextBox ID="m_accountName" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>密码:</td>
                    <td><asp:TextBox ID="m_key1" runat="server" TextMode="Password"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>确认密码:</td>
                    <td><asp:TextBox ID="m_key2" runat="server" TextMode="Password"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>账号类型:</td>
                    <td>
                         <asp:DropDownList ID="m_type" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="Button5" runat="server" Height="40px" Text="提交" Width="126px" 
                            onclick="onAddAccount" />
                    </td>
                    <td>
                        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
                    </td>
                </tr>
            </table>
        </div>

        <div>
            <h2>当前账号列表</h2>
            <br />
            <asp:Table ID="m_curAccount" runat="server" class="cTable">
            </asp:Table>
        </div>
    </div>
</asp:Content>
