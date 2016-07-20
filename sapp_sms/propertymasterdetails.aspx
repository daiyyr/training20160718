    <%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="propertymasterdetails.aspx.cs" Inherits="sapp_sms.propertymasterdetails" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<%@ Register Src="jqGridCC.ascx" TagName="jqGridCC" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Property Master Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/propertymasterdetails.js" type="text/javascript"></script>
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
                    OnClick="ImageButtonDelete_Click" />
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
        <div id="tabs">
            <ul>
                <li><a href="#tabs-1">General</a></li>
                <li><a href="#tabs-2">Comms</a></li>
                <li><a href="#tabs-3">Contact</a></li>
            </ul>
            <div id="tabs-1">
                                <table class="details">
                        <tr>
                            <td>
                                <b>Property Master:<asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </b>
                            </td>
                            <td colspan="3">
                                <b>
                                    <asp:Label ID="LabelID" runat="server" Text="ID" Visible="False"></asp:Label></b>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <b>
                                    <asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </b>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>Code:<asp:Image ID="Image3" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </b>
                            </td>
                            <td>
                                <asp:Label ID="LabelCode" runat="server" CssClass="sapplabel"></asp:Label>
                            </td>
                            <td>
                                <b>Name:</b>
                            </td>
                            <td>
                                <asp:Label ID="LabelName" runat="server" CssClass="sapplabel"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <b>
                                    <asp:Image ID="Image4" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </b>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>Bodycorp:<asp:Image ID="Image5" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </b>
                            </td>
                            <td>
                                <asp:Label ID="LabelBodycorp" runat="server" CssClass="sapplabel"></asp:Label>
                            </td>
                            <td>
                                <b>Type:</b>
                            </td>
                            <td>
                                <asp:Label ID="LabelType" runat="server" CssClass="sapplabel"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <b>
                                    <asp:Image ID="Image6" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </b>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>TotalSqm:<asp:Image ID="Image7" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </b>
                            </td>
                            <td>
                                <asp:Label ID="LabelTotalSqm" runat="server" CssClass="sapplabel"></asp:Label>
                            </td>
                            <td>
                                <b>Num of Units</b>
                            </td>
                            <td>
                                <asp:Label ID="LabelNumOfUnits" runat="server" CssClass="sapplabel"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <b>
                                    <asp:Image ID="Image8" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </b>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>Begin Date::<asp:Image ID="Image12" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </b>
                            </td>
                            <td>
                                <asp:Label ID="LabelBeginDate" runat="server" CssClass="sapplabel"></asp:Label>
                            </td>
                            <td>
                                <b></b>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <b>
                                    <asp:Image ID="Image11" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </b>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>Notes:<asp:Image ID="Image9" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </b>
                            </td>
                            <td colspan="3">
                                <asp:Label ID="LabelNotes" runat="server" TextMode="MultiLine" Width="500px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <b>
                                    <asp:Image ID="Image10" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                                </b>
                            </td>
                        </tr>
                    </table>
            </div>
            <div id="tabs-2">
                                <div>
                        <cc1:jqGridAdv runat="server" ID="jqGridComms" colNames="['ID', 'Type(*)', 'Details(*)', 'Primary', 'Order']"
                            colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                     { name: 'Type', index: 'Type', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'propertymasterdetails.aspx/DataGridCommsTypeSelect'}, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                     { name: 'Details', index: 'Details', width: 200, editable:true,editoptions: { 'maxlength': 100 } ,editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: false},
                     { name: 'Primary', index: 'Primary', width: 50, editable:true, edittype:'checkbox', editoptions:{value:'Yes:No'}, align: 'left', search: false},
                     { name: 'Order', index: 'Order', width: 50, align: 'left', search: false, hidden: true}
                     ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="Order" sortorder="asc" viewrecords="true"
                            width="700" height="500" url="propertymasterdetails.aspx/DataGridCommsDataBind"
                            hasID="true" idName="propertyid" inlineNav="true" editurl="propertymasterdetails.aspx/DataGridCommsSave"
                            contentPlaceHolder="ContentPlaceHolder1" />
                    </div>
                    <div align="left">
                        <asp:Button ID="ButtonDeleteComm" runat="server" Text="Delete" OnClientClick="return ButtonDeleteComm_ClientClick()" />
                    </div>
            </div>
            <div id="tabs-3">
                                <img src="Images/dot.gif" height="4px" />
                    <uc1:jqGridCC runat="server" ID="jqGridContacts" colNames="['ID','Type', 'Name(*)','Notes']"
                        colModel="[{ name: 'ID', index: 'ID', width: 50, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']}, hidden:true},
                    { name: 'Type', index: 'Type', width: 100,editable:true, edittype:'select', editoptions:{dataUrl:'propertymasterdetails.aspx/DataGridContactTypeSelect'}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'Name', index: 'Name', width: 100,editable:true, editoptions: { 'maxlength': 100 },editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'Notes', index: 'Notes', width: 100,editable:true, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                    ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
                        width="700" height="200" url="propertymasterdetails.aspx/DataGridContactDataBind"
                        hasID="false" editurl="propertymasterdetails.aspx/SaveDataFromContactGrid" onSelectRowUrl="propertymasterdetails.aspx/DataGridContactSubDataBind"
                        dcolNames="['ID','Type','Details(*)','Primary','Order']" dcolModel="[
                        { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                        { name: 'Type', index: 'Type', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'propertymasterdetails.aspx/DataGridContactCommsTypeSelect'}, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
                        { name: 'Details', index: 'Details', width: 200, editable:true,editoptions: { 'maxlength': 100 },editrules:{custom:true, custom_func:ValidateRowData}, align: 'left', search: false},
                        { name: 'Primary', index: 'Primary', width: 50, editable:true, edittype:'checkbox', editoptions:{value:'Yes:No'},  align: 'left', search: false},
                        { name: 'Order', index: 'Order', width: 50, align: 'left', search: false, hidden: true}
                     ]" drowNum="25" drowList="[5, 10, 25, 50, 100]" dsortName="ID" dsortOrder="asc"
                        dwidth="700" dheight="200" dmultiSelect="false" deditUrl="propertymasterdetails.aspx/SaveDataFromContactCommGrid" />
                    <div align="left">
                        <asp:Button ID="ButtonContact" runat="server" Text="DeleteContact" OnClientClick="return ButtonDeleteContact_ClientClick()" />
                        <asp:Button ID="ButtonContactComm" runat="server" Text="DeleteContactComm" OnClientClick="return ButtonDeleteContactComm_ClientClick()" />
                    </div>
            </div>
        </div>

    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Unit Plan</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/unitplan.gif"
                    OnClick="ImageButtonUnitplan_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Maintenance</div>
            <div>
                <asp:ImageButton ID="ImageButtonMaintenance" runat="server" ImageUrl="Images/Maintenance.gif"
                    OnClick="ImageButtonMaintenance_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Valuations</div>
            <div>
                <asp:ImageButton ID="ImageButtonValuations" runat="server" ImageUrl="Images/valuation.gif"
                    OnClick="ImageButtonValuations_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Titles</div>
            <div>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="Images/Title.gif" OnClick="ImageButtonTitles_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Mortgages</div>
            <div>
                <asp:ImageButton ID="ImageButtonMortgages" runat="server" ImageUrl="Images/mortgage.gif"
                    OnClick="ImageButtonMortgages_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Contractor</div>
            <div>
                <asp:ImageButton ID="ImageButtonContractor" runat="server" ImageUrl="Images/contractor.gif"
                    OnClick="ImageButtonContractor_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Insurance</div>
            <div>
                <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="Images/insurance.gif"
                    OnClick="ImageButtonInsurance_Click" />
            </div>
        </div>
    </div>
</asp:Content>
