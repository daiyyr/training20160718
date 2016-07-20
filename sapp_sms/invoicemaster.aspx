<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="invoicemaster.aspx.cs" Inherits="sapp_sms.invoicemaster" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Invoice Master</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="Scripts/invoicemaster.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                All Invoice</div>
            <div>
                <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="Images/new.gif" OnClick="ImageButton3_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Add</div>
            <div>
                <asp:ImageButton ID="ImageButtonAdd" runat="server" ImageUrl="Images/new.gif" OnClick="ImageButtonAdd_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Details</div>
            <div>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="Images/edit.gif" OnClientClick="return BatchClick()" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Delete</div>
            <div>
                <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/images/delete.gif"
                    OnClientClick="return DeleteClick();" />
            </div>
        </div>
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
        <div>
            <img src="Images/dot.gif" height="4px" />
            <cc1:jqGridAdv runat="server" ID="jqGridTable" colNames="['ID', 'Num','Bodycorp','InvCount', 'Date', 'Gross', 'Paid','Balance','Type']"
                colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                 { name: 'Num', index: 'Num', width: 100, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
             
                 { name: 'Bodycorp', index: 'Bodycorp', width: 150, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'InvCount', index: 'InvCount', width: 150, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                 { name: 'Date', index: 'Date', width: 150, align: 'left', search: true, formatter: 'date', formatoptions:{srcformat: 'd/m/Y'}},
                 { name: 'Gross', index: 'Gross', width: 150, align: 'right', search: false},
                 { name: 'Paid', index: 'Paid', width: 150, align: 'right', search: false},
                        { name: 'Balance', index: 'Balance', width: 150, align: 'right', search: false},
                                         { name: 'Type', index: 'Type', width: 150, align: 'right', search: false}
                 ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="Date" sortorder="desc"
                viewrecords="true" width="700" height="500" url="invoicemaster.aspx/DataGridDataBind"
                hasID="false" />
        </div>
    </div>
    <div id="content_right">
        <%--        <div class="button">
            <div class="button-title">
                Export</div>
            <div>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/export_pdf.gif"
                    OnClick="ImageButton2_Click" />
            </div>
        </div>--%>
        <div class="button">
            <div class="button-title">
                Import</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/export_pdf.gif"
                    OnClick="ImageButton1_Click" />
            </div>
            <div class="button-title">
                Export</div>
            <div>
                <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/images/export_pdf.gif"
                    OnClientClick="return ExportClick();" />
            </div>
            <div class="button-title">
                Template</div>
            <div>
                <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/images/history.gif"
                    CausesValidation="false" OnClick="ImageButton4_Click" />
            </div>
        </div>
    </div>
    <script>
        function ExportClick() {

            var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
            var rowKey = grid.getGridParam("selrow");

            if (rowKey) {
                var ID = grid.getCell(rowKey, 'ID');
                __doPostBack('__Page', 'Export|' + ID);
                return false;
            }
            else {
                alert("Please select a row first!");
                return false;
            }

        }
        function DeleteClick() {
            if (confirm("Are you sure you want to delete the item?") == true) {
                var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
                var rowKey = grid.getGridParam("selrow");

                if (rowKey) {
                    var ID = grid.getCell(rowKey, 'ID');
                    __doPostBack('__Page', 'Delete|' + ID);
                    return false;
                }
                else {
                    alert("Please select a row first!");
                    return false;
                }
            }
        }


        function BatchClick() {
            var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
            var rowKey = grid.getGridParam("selrow");

            if (rowKey) {
                var ID = grid.getCell(rowKey, 'ID');
                __doPostBack('__Page', 'Batch|' + ID);
                return false;
            }
            else {
                alert("Please select a row first!");
                return false;
            }

            return false;
        }
    </script>
</asp:Content>
