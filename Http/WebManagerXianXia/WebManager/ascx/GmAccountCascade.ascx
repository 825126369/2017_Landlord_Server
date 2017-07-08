<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GmAccountCascade.ascx.cs" Inherits="WebManager.ascx.GmAccountCascade" %>

<div>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

     时间:&nbsp;&nbsp;<asp:TextBox ID="__gmAccountCascadeStaticTime" runat="server" style="width:400px;height:20px" ClientIDMode="Static"></asp:TextBox>

     <p>
         &nbsp;账号:<asp:TextBox ID="m_acc" runat="server" CssClass="cTextBox"></asp:TextBox>
     </p>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            经销商:<asp:DropDownList ID="m_dealer" runat="server" CssClass="cDropDownList" AutoPostBack="True" 
            OnSelectedIndexChanged="onDealerChanged"></asp:DropDownList>

            经销商管理员:<asp:DropDownList ID="m_dealerAdmin" runat="server" CssClass="cDropDownList" AutoPostBack="True"
                OnSelectedIndexChanged="onDealerAdminChanged"></asp:DropDownList>

            <p>
            售货亭:<asp:DropDownList ID="m_seller" runat="server" CssClass="cDropDownList" AutoPostBack="True"
                OnSelectedIndexChanged="onSellerChanged"></asp:DropDownList>

            售货亭管理员:<asp:DropDownList ID="m_sellerAdmin" runat="server" CssClass="cDropDownList"></asp:DropDownList>
            </p>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>