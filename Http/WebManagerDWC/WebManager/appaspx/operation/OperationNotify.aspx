<%@ Page Title="" Language="C#" MasterPageFile="~/appaspx/operation/OperationCommon.master" AutoEventWireup="true" CodeBehind="OperationNotify.aspx.cs" Inherits="WebManager.appaspx.operation.OperationNotify" %>
<asp:Content ID="Content1" ContentPlaceHolderID="operationHeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="operation_common" runat="server">
    <h2>运营公告</h2>
    <div class="SplitBorder">
        <p>
            id： <asp:TextBox ID="m_noticeId" runat="server" style="width:360px" ></asp:TextBox>
            id为空时，表示增加公告，不为空表示修改公告
        </p>
    
        <p>
            标题： <asp:TextBox ID="m_title" runat="server" style="width:360px" ></asp:TextBox>
        </p>
    
        <p>
            内容：<asp:TextBox ID="m_content" runat="server"
                TextMode="MultiLine" Wrap="False" style="height:294px;width:370px"></asp:TextBox>
        </p>
         
        <p>
           显示天数： <asp:TextBox ID="m_day" runat="server" style="width:340px" ></asp:TextBox>
        </p>

        <p>
           排序字段： <asp:TextBox ID="m_order" runat="server" style="width:340px" ></asp:TextBox>数字越小，越排前面，默认值为0
        </p>

        <p>
           备注(可填写本次操作原因)： <asp:TextBox ID="m_comment" runat="server" style="width:340px" ></asp:TextBox>
        </p>

        <p>
            <asp:Button ID="Button1" runat="server" onclick="onPublishNotice" 
                Text="发布公告" style="width:134px;height:28px" />
        </p>
        <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
    </div>

    <div class="SplitBorder">
        <asp:Table ID="m_result" runat="server" CssClass="result">
        </asp:Table>
        <asp:Button ID="Button2" runat="server" onclick="onCancelNotice" Text="撤消" style="width:100px;height:28px" />
    </div>
</asp:Content>
