<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountCreateDealerSubAdmin.aspx.cs" Inherits="WebManager.appaspx.account.AccountCreateDealerSubAdmin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../Scripts/account/AccountCreateDealerSubAdmin.js" type="text/javascript"></script>
    <style type="text/css">
        .cSafeWidth td{padding:5px;}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="cSafeWidth">
        <h2>创建下级代理，API账号 </h2>
        <table>
            <tr>
                <td>账号类型:</td>
                <td>
                    <asp:DropDownList ID="m_type" runat="server" CssClass="cDropDownList"></asp:DropDownList>
                </td>
            </tr>
            <tr class="cAgency cSub">
                <td>名称:</td>
                <td>
                    <asp:Label ID="m_prefix" runat="server" Text=""></asp:Label>
                    <asp:TextBox ID="m_accName" runat="server" CssClass="cTextBox"></asp:TextBox>
                </td>
                <td>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
                    ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                    ValidationExpression="^[a-zA-Z0-9]{3,10}$" ControlToValidate="m_accName"></asp:RegularExpressionValidator>
                    <asp:Label ID="Label1" runat="server" Text="3-10位字母数字组合" style="margin-left:5px" ></asp:Label>
                </td>
            </tr>
            <tr class="cAPI">
                <td>名称:</td>
                <td>
                    <asp:TextBox ID="m_accName1" runat="server" CssClass="cTextBox"></asp:TextBox>
                </td>
                <td>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^[a-zA-Z0-9]{3,10}$" ControlToValidate="m_accName1"></asp:RegularExpressionValidator>
                    <asp:Label ID="Label2" runat="server" Text="3-10位字母数字组合" style="margin-left:5px" ></asp:Label>
                </td>
            </tr>
            <tr class="cAPI">
                <td>API前缀:</td>
                <td>
                    <asp:TextBox ID="m_apiPrefix" runat="server" CssClass="cTextBox" ClientIDMode="Static">
                    </asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                        ErrorMessage="必填项" Display="Dynamic" ControlToValidate="m_apiPrefix" ForeColor="Red">
                    </asp:RequiredFieldValidator>

                    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^[a-zA-Z0-9]{2,10}$" ControlToValidate="m_apiPrefix"></asp:RegularExpressionValidator>
                    <asp:Label ID="Label4" runat="server" Text="2-10位字母数字组合。" style="margin-left:5px" ></asp:Label>
                    前缀不能为空，创建后需要管理员审批，一经通过，前缀不可更改。
                </td>
            </tr>
            <tr>
                <td>别名:</td>
                <td>
                    <asp:TextBox ID="m_aliasName" runat="server" CssClass="cTextBox" ></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"  ForeColor="Red" 
                        ControlToValidate="m_aliasName"
                        ErrorMessage="必填项，可以是任意字符"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>密码:</td>
                <td>
                    <asp:TextBox ID="m_pwd1" runat="server" CssClass="cTextBox" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                     <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^.{6,16}$" ControlToValidate="m_pwd1"></asp:RegularExpressionValidator>
                    &nbsp;&nbsp;
                    6-16位任意字符
                </td>
            </tr>
            <tr>
                <td>确认密码:</td>
                <td>
                    <asp:TextBox ID="m_pwd2" runat="server" CssClass="cTextBox" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                        ErrorMessage="格式非法" Display="Dynamic" ForeColor="Red"
                        ValidationExpression="^.{6,16}$" ControlToValidate="m_pwd2"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                     <asp:CompareValidator ID="CompareValidator1" runat="server" 
                        ErrorMessage="两次密码不一致" ControlToCompare="m_pwd1" ControlToValidate="m_pwd2" ForeColor="Red" Display="Dynamic">
                        </asp:CompareValidator>
                </td>
            </tr>
            <tr>
                <td>代理占成:</td>
                <td>
                    <asp:TextBox ID="m_agentRatio" runat="server" CssClass="cTextBox"></asp:TextBox>%
                </td>
                <td>
                    <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="输入错误" 
                        ControlToValidate="m_agentRatio" Display="Dynamic" ForeColor="Red" 
                        MinimumValue="0" MaximumValue="1" Type="Double"></asp:RangeValidator>

                    0-100之间的小数或整数，但不能超出自身的代理占成
                </td>
            </tr>
            <tr>
                <td>洗码比:</td>
                <td>
                    <asp:TextBox ID="m_washRatio" runat="server" CssClass="cTextBox"></asp:TextBox>%
                </td>
                <td>
                   <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="输入错误" 
                        ControlToValidate="m_washRatio" Display="Dynamic" ForeColor="Red" 
                        MinimumValue="0" MaximumValue="1.2" Type="Double"></asp:RangeValidator>

                    0-1.2之间的小数，但不能超出自身的洗码比
                </td>
            </tr>
            <tr class="cAgency">
                <td> 权限: </td>
                <td>
                     <asp:CheckBoxList ID="m_right" runat="server" RepeatDirection="Horizontal"></asp:CheckBoxList>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Button ID="Button1" runat="server" Text="创建" CssClass="cButton" onclick="onCreateAccount" />
                    <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
