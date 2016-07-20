<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="unitmaster.aspx.cs" Inherits="sapp_sms.unitmaster" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Unit Master</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/unitmaster.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
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
                <asp:ImageButton ID="ImageButtonDetails" runat="server" ImageUrl="Images/detail.gif"
                    PostBackUrl="~/unitmaster.aspx" OnClientClick="return ImageButtonDetails_ClientClick()" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Edit</div>
            <div>
                <asp:ImageButton ID="ImageButtonEdit" runat="server" ImageUrl="Images/edit.gif" PostBackUrl="~/unitmaster.aspx"
                    OnClientClick="return ImageButtonEdit_ClientClick()" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Delete</div>
            <div>
                <asp:ImageButton ID="ImageButtonDelete" runat="server" ImageUrl="Images/delete.gif"
                    PostBackUrl="~/unitmaster.aspx" OnClientClick="return ImageButtonDelete_ClientClick()" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Delete All</div>
            <div>
                <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="Images/delete.gif" OnClientClick="return confirm('Are you sure you want to delete the item?');"
                    OnClick="ImageButton3_Click" />
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
            <cc1:jqGridAdv runat="server" ID="jqGridTable" colNames="['ID','Unit Num','Type','Description','Proprietor','Area Sqm','Area Type','OI','UI','Balance']"
                colModel="[
                  { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                  { name: 'Code', index: 'Code', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Type', index: 'Type', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Description', index: 'Description', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Proprietor', index: 'Proprietor', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'AreaSqm', index: 'AreaSqm', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'AreaType', index: 'AreaType', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'OI', index: 'OI', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'UI', index: 'UI', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                                    { name: 'Balance', index: 'Balance', width: 100,  align: 'left', search: true,  hidden:true,  searchoptions: { sopt: ['cn', 'nc']}}

              ]" rowNum="10000" rowList="[]" sortname="Code" sortorder="asc" viewrecords="true"
                width="800" height="500" url="unitmaster.aspx/DataGridDataBind" hasID="true"
                idName="propertyid" footerrow="true" userDataOnFooter="true" />
            <div style="text-align: right; background-color: White; visibility: hidden;">
                <b>Total</b>&nbsp;&nbsp;&nbsp; <b>OI:</b>&nbsp;<asp:Label ID="LabelTotalOI" runat="server"
                    ForeColor="Blue"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <b>UI:</b>&nbsp;<asp:Label
                        ID="LabelTotalUI" runat="server" ForeColor="Blue"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Balance:</b><asp:Label
                            ID="LabelBalance" runat="server" ForeColor="Blue"></asp:Label>
            </div>
        </div>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Setup Wizard</div>
            <div>
                <asp:ImageButton ID="ImageButtonSetupWizard" runat="server" ImageUrl="Images/unitplan.gif"
                    OnClick="ImageButtonSetupWizard_Click" Enabled="false" />
            </div>
        </div>
                <div class="button">
            <div class="button-title">
                Reports</div>
            <div>
                <asp:ImageButton ID="ImageButton4" ImageUrl="~/images/proprietor.gif" runat="server"
                    OnClientClick="ImageButtonReports_ClientClick(); return false;" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                P<span lang="EN-US" style="font-size:10.5pt;mso-bidi-font-size:
11.0pt;font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;;mso-ascii-theme-font:minor-latin;
mso-fareast-font-family:ËÎÌו;mso-fareast-theme-font:minor-fareast;mso-hansi-theme-font:
minor-latin;mso-bidi-font-family:&quot;Times New Roman&quot;;mso-bidi-theme-font:minor-bidi;
mso-ansi-language:EN-US;mso-fareast-language:ZH-CN;mso-bidi-language:AR-SA">roprietor </span></div>
            <div>
                <asp:ImageButton ID="ImageButtonReports" ImageUrl="~/images/proprietor.gif" runat="server"
                    OnClientClick="P_Click(); return false;" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Export</div>
            <div>
                <asp:ImageButton ID="ImageButtonExport" runat="server" ImageUrl="~/images/export.gif"
                    OnClick="ImageButtonExport_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Import</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/import.gif"
                    OnClick="ImageButton1_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Import Ownership</div>
            <div>
                <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/images/transfer.gif"
                    OnClick="ImageButton6_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Make Receipt</div>
            <div>
                <asp:ImageButton ID="ImageButtonEdit0" runat="server" ImageUrl="Images/edit.gif"
                    OnClientClick="MakePaymentClick()" />
                <script>
                    function show(id) {
                        var url = "invoicemasterUnit.aspx?unitid=" + id;
                        vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 800px; dialogWidth: 1524px; edge: Raised; center: Yes;" +
                    "help: No; resizable: Yes; status: No; scroll: Yes;");
                        if (vReturnValue == "refresh") {
                            __doPostBack('__Page', 'Refresh|0');
                        }
                    }
                    function MakePaymentClick() {

                        var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
                        var rowKeys = grid.getGridParam("selrow");
                        if (rowKeys.length > 0) {
                            var id = grid.getCell(rowKeys, 'ID');
                            show(id);
                        }
                        else {
                            alert("Please select a row first!");
                            return false;
                        }
                        return false;
                    }
                    function P_Click() {
                        var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
                        var rowKey = grid.getGridParam("selrow");

                        if (rowKey) {
                            var ID = grid.getCell(rowKey, 'ID');
                            __doPostBack('__Page', 'P|' + ID);
                            return false;
                        }
                        else {
                            alert("Please select a row first!");
                            return false;
                        }

                        return false;
                    }
                </script>
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Inactive</div>
            <div>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="Images/unitplan.gif"
                    OnClick="ImageButton2_Click" />
            </div>
        </div>
    </div>
</asp:Content>
