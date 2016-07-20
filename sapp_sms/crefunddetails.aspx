<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="crefunddetails.aspx.cs" Inherits="sapp_sms.credunfdetails" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - CRefund Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/cpaymentdetails.js" type="text/javascript"></script>
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
                <asp:ImageButton ID="ImageButtonClose" runat="server" OnClientClick="history.back(); return false;"
                    ImageUrl="~/images/close.gif" OnClick="ImageButtonClose_Click" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
            <tr>
                <td>
                    <b>CRefund:<asp:Image ID="Image11" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b>
                </td>
                <td>
                    <b>
                        <asp:Label ID="LabelCpaymentID" runat="server" Text="ID" Visible="false"></asp:Label></b>
                </td>
                <td>
                    <b>Date :</b>
                </td>
                <td>
                    <asp:Label ID="LabelDate" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Image ID="Image12" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Reference:<asp:Image ID="Image15" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b>
                </td>
                <td>
                    <asp:Label ID="LabelReference" runat="server"></asp:Label>
                </td>
                <td>
                    <b>Type:</b>
                </td>
                <td>
                    <asp:Label ID="LabelType" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Bodycorp:<asp:Image ID="Image13" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b>
                </td>
                <td>
                    <asp:Label ID="LabelBodycorp" runat="server"></asp:Label>
                </td>
                <td>
                    <b>Creditor:</b>
                </td>
                <td>
                    <asp:Label ID="LabelCreditor" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Image ID="Image14" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Gross :<asp:Image ID="Image17" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></b>
                </td>
                <td>
                    <asp:Label ID="LabelGross" runat="server"></asp:Label>
                </td>
                <td>
                    <b>Allocated:<asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                </td>
                <td>
                    <asp:Label ID="LabelAllocated" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div>
                        <img src="Images/dot.gif" height="4px" />
                        <cc1:jqGridAdv runat="server" ID="jqGridRelated" colNames="['ID','Cinvoice Num','Description','Date', 'DueDate', 'Net','Tax','Gross','Due','Paid']"
                            colModel="[
                        { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                        { name: 'CinvoiceNum', index: 'CinvoiceNum', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Description', index: 'Description', width: 200, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Date', index: 'Date', width: 100, align: 'left', search: false},
                        { name: 'DueDate', index: 'DueDate', width: 100, align: 'left', search: false},
                        { name: 'Net', index: 'Net', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Tax', index: 'Tax', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Gross', index: 'Gross', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Due', index: 'Due', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Paid', index: 'Paid', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                    ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="CinvoiceNum" sortorder="asc"
                            viewrecords="true" width="700" height="200" url="crefunddetails.aspx/DataGridDataBind"
                            hasID="false" />
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
    </div>
    <script type="text/javascript">
        $("#<%=DetailI.ClientID %>").click(function () {
            show(querySt("cpaymentid"), "Cpayment");
        });
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
