<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ServiceExcelDownLoad.aspx.cs" Inherits="WebManager.appaspx.service.ExcelDownLoad" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   <h2>EXCEL下载</h2>
   <p>Excel表格数据在服务器保存7天后将删除，请及时下载备份。</p>
   <p>采用目标另存为可以下载到完整文档</p>
   <asp:Table ID="m_result" runat="server" CssClass="result">
   </asp:Table>
</asp:Content>
