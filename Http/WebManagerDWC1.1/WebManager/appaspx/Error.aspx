<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="WebManager.appaspx.Error" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
     <h2>
        错误
    </h2>
    <p runat="server" ID="ErrorInfo" style="font-size:medium;color:red"></p>

</asp:Content>
