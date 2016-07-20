<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="ccinvoicedetails.aspx.cs" Inherits="sapp_sms.ccinvoicedetails" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - CCredit Note Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/cinvoicedetails.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Edit</div>
            <div>
                <asp:ImageButton ID="ImageButtonEdit" runat="server" ImageUrl="~/images/edit.gif"
                    OnClick="ImageButtonEdit_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Delete</div>
            <div>
                <asp:ImageButton ID="ImageButtonDelete" runat="server" ImageUrl="~/images/delete.gif"
                    OnClientClick="return confirm_delete();" OnClick="ImageButtonDelete_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" CausesValidation="false" OnClientClick="history.back(); return false;"
                    ImageUrl="~/images/close.gif" OnClick="ImageButtonClose_Click" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
            <tr>
                <td>
                    <b>Cinvoice:</b><asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
                <td colspan="3">
                    <b>
                        <asp:Literal ID="LiteralClientID" runat="server" Text="ID" Visible="false"></asp:Literal></b>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Image ID="Image11" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Num :</b>
                </td>
                <td>
                    <asp:Label ID="LabelNum" runat="server"></asp:Label>
                </td>
                <td>
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Image ID="Image3" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Creditor:</b><asp:Image ID="Image4" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
                <td>
                    <asp:Label ID="LabelCreditor" runat="server"></asp:Label>
                </td>
                <td>
                    <b>Bodycorp:</b>
                </td>
                <td>
                    <asp:Label ID="LabelBodycorp" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Image ID="Image5" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Date :</b><asp:Image ID="Image8" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
                <td>
                    <asp:Label ID="LabelDate" runat="server"></asp:Label>
                </td>
                <td>
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Image ID="Image12" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    <asp:Label ID="LabelPaid" runat="server" Visible="false"></asp:Label>
                    <asp:Label ID="LabelApply" runat="server"></asp:Label>
                    <asp:Label ID="LabelDue" runat="server"></asp:Label>
                    <asp:Label ID="LabelUnit" runat="server"></asp:Label>
                    <asp:Label ID="LabelAdminFee" runat="server" Visible="false"></asp:Label>
                    <asp:Label ID="LabelOrder" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div class="tablecell" style="width: 80px; float: left">
                        <asp:Image ID="Image13" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </div>
                    <div class="tablecell" style="width: 100px; float: left">
                        <asp:Label ID="LabelNet" runat="server" ForeColor="#006600" Visible="false"></asp:Label>
                    </div>
                    <div class="tablecell" style="width: 80px; float: left">
                    </div>
                    <div class="tablecell" style="width: 100px; float: left">
                        <asp:Label ID="LabelTax" runat="server" ForeColor="#006600" Visible="false"></asp:Label>
                    </div>
                    <div class="tablecell" style="width: 80px; float: left">
                    </div>
                    <div class="tablecell" style="width: 100px; float: left">
                        <asp:Label ID="LabelGross" runat="server" ForeColor="#006600" Visible="false"></asp:Label>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Description</b><asp:Image ID="Image17" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
                <td colspan="3">
                    <asp:Label ID="LabelDescription" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Image ID="Image18" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div>
                        <img src="Images/dot.gif" height="4px" />
                        <cc1:jqGridAdv runat="server" ID="jqGridTrans" colNames="['ID','Chart(*)','Description','Net(*)','Tax(*)','Gross(*)']"
                            colModel="[
                          { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                          { name: 'Chart', index: 'Chart', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Description', index: 'Description', width: 200,align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Net', index: 'Net', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Tax', index: 'Tax', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Gross', index: 'Gross', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                      ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
                            width="700" height="300" url="ccinvoicedetails.aspx/DataGridDataBind" hasID="true"
                            idName="cinvoiceid" footerrow="true" userDataOnFooter="true" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Detail</div>
            <div>
                <asp:Image ID="DetailI" runat="server" ImageUrl="~/images/invoice.gif" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Payment</div>
            <div>
                <asp:Image ID="Image16" runat="server" ImageUrl="~/images/invoice.gif" />
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $("#<%=DetailI.ClientID %>").click(function () {
            show(querySt("cinvoiceid"), "Cinvoice");
        });
        $("#<%=Image16.ClientID %>").click(function () {
            Pshow(querySt("cinvoiceid"), "Payment");
        });
        function Pshow(id, type) {
            var url = "activityreciptdetail.aspx?id=" + id + "&type=" + type;
            vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 700px; dialogWidth: 850px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; scroll: No;");
            if (vReturnValue == "refresh") {
                __doPostBack('__Page', 'Refresh|0');
            }
        }
        function QueryString(name) {
            var AllVars = window.location.search.substring(1);
            var Vars = AllVars.split("&");
            for (i = 0; i < Vars.length; i++) {
                var Var = Vars[i].split("=");
                if (Var[0] == name) return Var[1];
            }
            return "";
        }
        function show(id, type) {
            var url = "activityunitdetail.aspx?id=" + id + "&type=" + type;
            vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 700px; dialogWidth: 850px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; scroll: No;");
            if (vReturnValue == "refresh") {
                __doPostBack('__Page', 'Refresh|0');
            }
        }
    </script>
</asp:Content>
