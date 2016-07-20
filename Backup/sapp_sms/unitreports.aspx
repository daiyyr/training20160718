<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="unitreports.aspx.cs" Inherits="sapp_sms.unitreports" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Unit Report Selection</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/unitreports.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server"   OnClientClick="window.close(); return false;"
                    ImageUrl="~/images/close.gif"/>
            </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
            <tr>
                <td colspan="4">
                    <asp:RadioButtonList ID="RadioButtonList01" runat="server">
                        <asp:ListItem selected="true">Unit List Report</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
        </table>   
    </div>
    <div id="content_right">
        <div class="button">
        <div class="button-title">OK</div>
        <div>
            <asp:ImageButton ID="ImageButtonSubmit" ImageUrl="~/images/submit.gif" 
                runat="server" onclick="ImageButtonSubmit_Click"/>
        </div>
        </div>
    </div>
</asp:Content>
