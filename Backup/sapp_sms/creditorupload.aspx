<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="creditorupload.aspx.cs" Inherits="sapp_sms.creditorupload" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Creditor Upload</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
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

        <table class="details">
            <tr>
                <td>
                    <asp:FileUpload ID="FileUpload1" runat="server" />
                    <asp:Button ID="ButtonImport" runat="server" Text="Import" 
                        onclick="ButtonImport_Click" />
                </td>
            </tr>
        </table>

    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Template</div>
            <div>
                <asp:ImageButton ID="ImageButtonTemplate" runat="server" 
                    ImageUrl="~/images/levies.gif" onclick="ImageButtonTemplate_Click"  />
            </div>
        </div>
    </div>
</asp:Content>
