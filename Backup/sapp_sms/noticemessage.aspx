<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="noticemessage.aspx.cs" Inherits="sapp_sms.noticemessage" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Notice</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery.cookie.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/purchordertypes.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" ImageUrl="Images/close.gif"
                    CausesValidation="false" OnClientClick="history.back(); return false;" />
            </div>
        </div>
    </div>
    <div id="content_middle">

    <table class="details" width="100%">
        <tr>
            <td>
                Title:</td>
            <td>
                <asp:TextBox ID="TextBox1" runat="server" Width="100%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td width="120">
                Description:</td>
            <td>
                <asp:TextBox ID="TextBox2" runat="server" Height="300px" Width="100%" 
                    TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td style="text-align: right">
                <asp:Button ID="Button1" runat="server" Text="Send" onclick="Button1_Click" />
                <asp:Button ID="Button2" runat="server" onclick="Button2_Click" Text="Cancel" />
            </td>
        </tr>
    </table>

        <asp:HiddenField ID="Date_HF" runat="server" />
        <asp:HiddenField ID="PID_HF" runat="server" />

    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
            </div>
            <div>
            </div>
        </div>
    </div>
</asp:Content>
