<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="creditnotedetails.aspx.cs" Inherits="sapp_sms.creditnotedetails" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Credit Note Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div id="content_left">
    <div class="button">
        <div class="button-title">Edit</div>
        <div>
            <asp:ImageButton ID="ImageButtonEdit"
                runat="server" ImageUrl="~/images/edit.gif" 
                onclick="ImageButtonEdit_Click" />
        </div>
    </div>
    <div class="button">
        <div class="button-title">Delete</div>
        <div>
            <asp:ImageButton ID="ImageButtonDelete"
                runat="server" ImageUrl="~/images/delete.gif"
                OnClientClick="return confirm_delete();" 
                onclick="ImageButtonDelete_Click" />
        </div>
    </div>
    <div class="button">
        <div class="button-title">Close</div>
        <div>
            <asp:ImageButton ID="ImageButtonClose" runat="server"  OnClientClick="history.back(); return false;"
                ImageUrl="~/images/close.gif" onclick="ImageButtonClose_Click" />
        </div>
    </div>
    </div>
    <div id="content_middle">
        <table class="details">
           <tr>
            <td><b>Credite Note ID:</b><asp:Image ID="Image11" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td colspan="3">
                <b><asp:Label ID="LabelElementID" runat="server" Text="ID"  Visible="False"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>

        <tr>
            <td><b>Num :</b><asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td><asp:Label ID="LabelNum" runat="server" ClientIDMode="Static"></asp:Label>
            </td>
            <td><b>Debtor:</b></td>
            <td>
                <asp:Label ID="LabelDebtor" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image3" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            <td><b>Bodycorp:</b><asp:Image ID="Image4" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td>
                <asp:Label ID="LabelBodycorp" runat="server"></asp:Label>
            </td>
            <td><b>Unit :</b></td>
            <td>
                <asp:Label ID="LabelUnit" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
        <td colspan="4"><asp:Image ID="Image5" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            <td><b>Date :</b><asp:Image ID="Image6" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td>
                <asp:Label ID="LabelDate" runat="server"></asp:Label>
            </td>

            <td><b>Due</b></td>
            <td><asp:Label ID="LabelDue" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image7" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            <td><b>Description</b><asp:Image ID="Image8" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td colspan="3"><asp:Label ID="LabelDescription" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image9" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            <td colspan="4">
                <div>
                    <img src="Images/dot.gif" height="4px" />
                    <cc1:jqGridAdv runat="server" ID="jqGridTrans" colNames="['ID','Chart','Description','Net','Tax','Gross']"
                     colModel="[
                          { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                          { name: 'Chart', index: 'Chart', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Description', index: 'Description', width: 100,align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Net', index: 'Net', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Tax', index: 'Tax', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Gross', index: 'Gross', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                      ]"
                     rowNum="25"
                     rowList="[5, 10, 25, 50, 100]"
                     sortname="ID"
                     sortorder="asc"
                     viewrecords="true"
                     width="700"
                     height="300"
                     url="creditnotedetails.aspx/DataGridDataBind"
                     hasID="true"
                     idName="invoicemasterid"
                     footerrow="true"
                     userDataOnFooter="true"
                     />
                </div>
                <div>
                    <input type="hidden" id="HiddenNet" value="98" />
                    <input type="hidden" id="HiddenNetOld" value="" />
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div style="width:400px; float:left;">&nbsp;
                    <asp:ImageButton ID="ImageButton1"
                        runat="server" ImageUrl="~/images/delete.gif" PostBackUrl="~/invoicemasteredit.aspx"
                        OnClientClick="return ImageButtonDelete_ClientClick()"/>
                </div>
                <div style="width:100px; float:left">
                    <asp:Label ID="LabelNet" runat="server" ForeColor="#006600" Visible="false"></asp:Label>
                </div>
                <div style="width:100px; float:left">
                    <asp:Label ID="LabelTax" runat="server" ForeColor="#006600" Visible="false"></asp:Label>
                </div>
                <div style="width:100px; float:left">
                    <asp:Label ID="LabelGross" runat="server" ForeColor="#006600" Visible="false"></asp:Label>
                </div>
            </td>
        </tr>
        </table> 
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Export</div>
            <div>
                <asp:ImageButton ID="ImageButtonExport" runat="server" ImageUrl="~/images/export_pdf.gif"
                    OnClick="ImageButtonExport_Click" />
            </div>
            <div class="button-title">
                Detail</div>
            <div>
                <asp:ImageButton ID="DetailB" runat="server" ImageUrl="Images/close.gif" CausesValidation="false"
                    OnClientClick="return DetailClick()" />
            </div>
            <div class="button-title">
                Refund</div>
            <div>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="Images/close.gif" CausesValidation="false"
                    OnClientClick="return RDetailClick()" />
            </div>
        </div>
    </div>
     <script>
         function Rshow(id, type) {
             var url = "activityreciptdetail.aspx?id=" + id + "&type=Refund";
             vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 700px; dialogWidth: 850px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; scroll: No;");
             if (vReturnValue == "refresh") {
                 __doPostBack('__Page', 'Refresh|0');
             }
         }
         function RDetailClick() {
             var id = getUrlParam("invoicemasterid");
             Rshow(id, 'Refund');
         }
         function show(id, type) {
             var url = "activityunitdetail.aspx?id=" + id + "&type=CreditNoteDetail";
             vReturnValue = window.showModalDialog(url, "#1", "dialogHeight: 700px; dialogWidth: 850px; edge: Raised; center: Yes;" +
                    "help: No; resizable: No; status: No; scroll: No;");
             if (vReturnValue == "refresh") {
                 __doPostBack('__Page', 'Refresh|0');
             }
         }
         function getUrlParam(name) {

             var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");

             var r = window.location.search.substr(1).match(reg);

             if (r != null) return unescape(r[2]); return null;

         }
         function DetailClick() {
             var id = getUrlParam("invoicemasterid");
             show(id, 'Refund');
         }
    </script>
    </asp:Content>
