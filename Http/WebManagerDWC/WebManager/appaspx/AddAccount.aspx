<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="AddAccount.aspx.cs" Inherits="WebManager.appaspx.AddAccount" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="http://cdn.bootcss.com/jquery/1.11.1/jquery.min.js"></script>
    <script src="http://cdn.bootcss.com/bootstrap/3.3.0/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="../Scripts/mselect/js/bootstrap-multiselect.js"></script>
    <link href="../Scripts/mselect/css/bootstrap-multiselect.css" rel="stylesheet" />
    <script src="../Scripts/module/sea.js" type="text/javascript"></script>
    <script type="text/javascript">
        seajs.use("../Scripts/AddAccount.js?ver=1");
    </script>
    <style type="text/css">
        .table{table-layout:fixed;}
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <%-- <h2>
        添加账号
    </h2>
    <asp:Panel ID="Panel1" runat="server" Height="285px" Width="807px" style="height:auto">
        <asp:Label ID="Label2" runat="server" Text="账号："></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="m_accountName" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="Label3" runat="server" Text="密码："></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="m_key1" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <asp:Label ID="Label5" runat="server" Text="确认密码："></asp:Label>
        <asp:TextBox ID="m_key2" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="Label4" runat="server" Text="人员类型："></asp:Label>
        <asp:DropDownList ID="m_type" runat="server">
        </asp:DropDownList>
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" Height="56px" Text="提交" Width="126px" 
            onclick="onAddAccount" />
        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
        <br />
        <asp:Button ID="Button2" runat="server" Height="56px" Text="修改" Width="126px" onclick="onModifyGMType" />
        <asp:Button ID="Button3" runat="server" Height="56px" Text="删除账号" Width="126px" onclick="onDelAccount" />
    </asp:Panel>--%>

    <div class="container form-horizontal">
        <h2 style="padding:20px;text-align:center;">添加账号</h2>
        <div class="form-group">
            <label for="account" class="col-sm-2 control-label">账号：</label>
            <div class="col-sm-10">
              <asp:TextBox ID="m_accountName" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label for="account" class="col-sm-2 control-label">密码：</label>
            <div class="col-sm-10">
                <asp:TextBox ID="m_key1" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label for="account" class="col-sm-2 control-label">确认密码：</label>
            <div class="col-sm-10">
                <asp:TextBox ID="m_key2" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label for="account" class="col-sm-2 control-label">人员类型：</label>
            <div class="col-sm-10">
                <asp:DropDownList ID="m_type" runat="server" CssClass="form-control"></asp:DropDownList>
            </div>
        </div>
        <div class="form-group">
            <div class="col-sm-offset-2 col-sm-10">
                <asp:Button ID="Button1" runat="server" Text="提交"
                onclick="onAddAccount" CssClass="btn btn-primary form-control" />
            </div>
        </div>
        <div class="form-group">
            <div class="col-sm-offset-2 col-sm-10">
                <span id="m_res" runat="server" class="control-label"></span>
            </div>
        </div>
    </div>

    <div class="container-fluid">
        <h2 style="padding:20px;text-align:center;">当前账号列表</h2>
        <br />
        <asp:Table ID="m_curAccount" runat="server" CssClass="table table-hover table-bordered">
        </asp:Table>
    </div>
</asp:Content>
