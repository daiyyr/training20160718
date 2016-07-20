<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="purchordermasterdetails.aspx.cs" Inherits="sapp_sms.purchordermasterdetails" %>
<%@ Register TagPrefix="uc" TagName="jqGrid" Src="~/jqGrid.ascx" %>
<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Purch Order Master Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/purchordermasterdetails.js" type="text/javascript"></script>

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
            <td colspan="2"><b>Element ID:</b><asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td colspan="6">
                <b><asp:Label ID="LabelElementID" runat="server" Text="ID" Visible="False"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="8"><asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        
        <tr>
            <td colspan="2"><b>Num :</b><asp:Image ID="Image3" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td colspan="2"><asp:Label ID="LabelNum" runat="server" ></asp:Label></td>
            <td colspan="2"><b>Type :</b></td>
            <td colspan="2">
                <asp:Label ID="LabelType" runat="server" ></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="8"><asp:Image ID="Image4" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            <td colspan="2"><b>Creditor:</b><asp:Image ID="Image5" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td colspan="2">
                <asp:Label ID="LabelCreditor" runat="server" ></asp:Label>
            </td>
            <td colspan="2"><b>Bodycorp:</b></td>
            <td colspan="2">
                <asp:Label ID="LabelBodycorp" runat="server" ></asp:Label>
            </td>
        </tr>
        <tr>
        <td colspan="8"><asp:Image ID="Image6" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            <td colspan="2"><b>Unit :</b><asp:Image ID="Image7" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td colspan="2">
                <asp:Label ID="LabelUnit" runat="server" ></asp:Label>
            </td>
            <td colspan="2"><b>Date :</b></td>
            <td colspan="2">
                <asp:Label ID="LabelDate" runat="server" ></asp:Label></td>
        </tr>
        <tr>
            <td colspan="8"><asp:Image ID="Image8" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        

        <tr>
            <td colspan="2"><b>Allocated :</b><asp:Image ID="Image9" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td colspan="2">
                <asp:Label ID="LabelAllocated" runat="server" ></asp:Label>
            </td>
            <td colspan="2"><b style="display:none;">Approve :</b></td>
            <td colspan="2">
                <asp:Label ID="LabelApproval" runat="server" Visible="false" ></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="8"><asp:Image ID="Image10" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>

        <tr>
            <%--<td colspan="4">
                <div></div>
            </td>--%>
            <td><b>Net :</b><asp:Image ID="Image11" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td colspan="2">
                <asp:Label ID="LabelNet" runat="server" ForeColor="#006600" ></asp:Label>
            </td>
            <td><b>Tax :</b></td>
            <td>
                <asp:Label ID="LabelTax" runat="server" ForeColor="#006600" ></asp:Label>
            </td>
            <td colspan="2"><b>Gross :</b></td>
            <td>
                <asp:Label ID="LabelGross" runat="server" ForeColor="#006600" ></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="8"><asp:Image ID="Image12" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            <td><b>Note :</b></td>
            <td colspan="7"><asp:Label ID="LabelNote" runat="server" ></asp:Label></td>
        </tr>
        
        <tr>
            <td colspan="8"><b>Purchase Order Transaction</b></td>
        </tr>
        <tr>
            <td colspan="8">
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
                     height="500"
                     url="purchordermasterdetails.aspx/DataGridDataBind"
                     hasID="false"
                     inlineNav="false"
                     />
                </div>
            </td>
        </tr>
           
        </table>  
    </div>
    <div id="content_right">
        <div class="button">
        <div class="button-title">Invoice</div>
        <div>
            <asp:ImageButton ID="ImageButtonInvoice"
                runat="server" ImageUrl="~/images/invoice.gif" 
                onclick="ImageButtonInvoice_Click" />
        </div>
    </div>
    </div>
</asp:Content>
