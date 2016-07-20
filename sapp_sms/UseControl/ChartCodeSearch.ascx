<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChartCodeSearch.ascx.cs" Inherits="sapp_sms.UseControl.ChartCodeSearch" %>
<asp:DropDownList ID="DropDownList1" runat="server">
    <asp:ListItem>Chart Name</asp:ListItem>
</asp:DropDownList>
<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
<asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Search" />
<br />
<asp:Label ID="Label1" runat="server"></asp:Label>

