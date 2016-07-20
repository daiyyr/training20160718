<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="creditornotedetails.aspx.cs" Inherits="sapp_sms.creditornotedetails" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Cpayment Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/creditornotedetails.js" type="text/javascript"></script>
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
            <td><b>Element ID:</b></td>
            <td colspan="3">
                <b><asp:Label ID="LabelElementID" runat="server" Text="ID" Visible="False"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Num :</b></td>
            <td>
                <asp:Label ID="LabelNum" runat="server"></asp:Label>
            </td>
            <td><b>Debtor:</b></td>
            <td>
                <asp:Label ID="LabelDebtor" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Bodycorp:</b></td>
            <td>
                <asp:Label ID="LabelBodycorp" runat="server"></asp:Label>
            </td>
            <td><b>Unit :</b></td>
            <td>
                <asp:Label ID="LabelUnit" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>
        
        <tr>
            <td><b>Date :</b></td>
            <td>
                <asp:Label ID="LabelDate" runat="server"></asp:Label>
            </td>

            <td><b>Due</b></td>
            <td>   
                <asp:Label ID="LabelDue" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Description</b></td>
            <td colspan="3">
                <asp:Label ID="LabelDescription" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>

        <tr>
            <td><b>BatchID :</b></td>
            <td>
                <asp:Label ID="LabelBatchID" runat="server"></asp:Label>
            </td>
            <td><b>Net:</b></td>
            <td>
                <asp:Label ID="LabelNet" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>

        <tr>
            <td><b>Tax:</b></td>
            <td>
                <asp:Label ID="LabelTax" runat="server"></asp:Label>
            </td>
            <td><b>Gross:</b></td>
            <td>
                <asp:Label ID="LabelGross" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        
        <tr>
            <td><b>Paid :</b></td>
            <td>
                <asp:Label ID="LabelPaid" runat="server"></asp:Label>
            </td>

            <td><b></b></td>
            <td>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        
        
        <tr>
            <td colspan="4"><b>GL Transaction</b></td>
        </tr>
        <tr>
            <td colspan="4">
                <div>
                    <img src="Images/dot.gif" height="4px" />
                    <cc1:jqGridAdv runat="server" ID="jqGridTrans" colNames="['ID','Chart','Description','Net','Tax','Gross']"
                     colModel="[
                          { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                          { name: 'Chart', index: 'Chart', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Description', index: 'Description', width: 200, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Net', index: 'Net', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Tax', index: 'Tax', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                          { name: 'Gross', index: 'Gross', width: 100, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                      ]"
                     rowNum="25"
                     rowList="[5, 10, 25, 50, 100]"
                     sortname="ID"
                     sortorder="asc"
                     viewrecords="true"
                     width="700"
                     height="300"
                     url="creditornotesedit.aspx/DataGridDataBind"
                     hasID="false"
                     />
                </div>
                <div>
                </div>
            </td>
        </tr>
        </table>  
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title"></div>
            <div>
            </div>
        </div>
    </div>
</asp:Content>
