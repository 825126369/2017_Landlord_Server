<%@ Page Language="C#" MasterPageFile="~/appaspx/service/ServiceCommon.master" AutoEventWireup="true" CodeBehind="Notice.aspx.cs" Inherits="WebManager.appaspx.Notice" %>

<%-- <asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content> --%>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="service_common">
     <h2>
        发布公告
    </h2>
    <br />
     <asp:Panel ID="Panel1" runat="server" Height="500px" Width="749px" style="height:auto">
        <h3>增加新公告</h3>

         <p>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:Label ID="Label1" runat="server" Text="标题："></asp:Label>
             &nbsp;
             <asp:TextBox ID="m_title" runat="server" Width="357px" style="width:357px" ></asp:TextBox>
         </p>
         <p>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Label ID="Label2" runat="server" Text="内容："></asp:Label>
             &nbsp;
             <asp:TextBox ID="m_content" runat="server" Width="356px" Height="294px" 
                 TextMode="MultiLine" Wrap="False" style="height:294px;width:356px"></asp:TextBox>
         </p>
         
         <p>
             &nbsp;&nbsp;
             <asp:Label ID="Label3" runat="server" Text="显示天数："></asp:Label>
             &nbsp;
             <asp:TextBox ID="m_day" runat="server" Width="354px" style="width:354px" ></asp:TextBox>
         </p>
         <p>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:Button ID="Button1" runat="server" Height="28px" onclick="onPublishNotice" 
                 Text="发布公告" Width="134px" />
         </p>
         <span id="m_res" style="font-size:medium;color:red" runat="server"></span>
        <h3>现有的系统公告</h3>
        <br />

        <asp:Table ID="NoticeTable" runat="server">
        </asp:Table>
         &nbsp;&nbsp;&nbsp;
         <asp:Button ID="Button2" runat="server" Height="33px" onclick="onSelectAll" 
             Text="全选" Width="78px" />
         &nbsp;&nbsp;&nbsp;&nbsp;
         <asp:Button ID="Button3" runat="server" Height="31px" onclick="onDelete" 
             style="margin-top: 0px" Text="删除" Width="86px" />
         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
         <asp:Button ID="Button4" runat="server" Height="31px" onclick="onActivate" 
             Text="激活" Width="87px" />
         &nbsp;&nbsp;&nbsp;
         <asp:Button ID="Button5" runat="server" Height="29px" onclick="onHideAllNotice" 
             Text="全部不显示" Width="91px" />
     </asp:Panel>
</asp:Content>
