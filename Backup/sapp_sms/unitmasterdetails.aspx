<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="unitmasterdetails.aspx.cs" Inherits="sapp_sms.unitmasterdetails" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Unit Master Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery.jqGrid.validation.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/unitmasterdetails.js" type="text/javascript"></script>
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
                <asp:ImageButton ID="ImageButtonClose" runat="server" OnClientClick="history.back();; return false;"
                    ImageUrl="~/images/close.gif" OnClick="ImageButtonClose_Click" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <div>
            <table class="details">
                <tr>
                    <td>
                        <b>Element ID:</b><asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                    <td colspan="3">
                        <b>
                            <asp:Label ID="LabelElementID" runat="server" Text="ID" Visible="False"></asp:Label></b>
                        <asp:HiddenField ID="HiddenUnitId" ClientIDMode="Static" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Unit Num (*):</b><asp:Image ID="Image3" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                    <td>
                        <asp:Label ID="LabelCode" runat="server"></asp:Label>
                    </td>
                    <td>
                        <b>Type (*):</b>
                    </td>
                    <td>
                        <asp:Label ID="LabelType" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Know As:</b><asp:Image ID="Image18" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                    <td>
                        <asp:Label ID="LabelKnowAs" runat="server"></asp:Label>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Image ID="Image4" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Principal:</b><asp:Image ID="Image5" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                    <td>
                        <asp:Label ID="LabelPrincipal" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <b>Bodycorp (*):</b>
                    </td>
                    <td>
                        <asp:Label ID="LabelBodycorp" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Image ID="Image6" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Proprietor:</b><asp:Image ID="Image7" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                    <td>
                        <asp:Label ID="LabelDebtor" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <b>Size Sqm:</b>
                    </td>
                    <td>
                        <asp:Label ID="LabelArea" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Image ID="Image8" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Area Type:</b><asp:Image ID="Image15" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                    <td>
                        <asp:Label ID="LabelAreaType" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <b>Ownership Interest:</b><asp:Image ID="Image9" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                    <td>
                        <asp:Label ID="LabelOwnershipInterest" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Image ID="Image16" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Utility Interest:</b>
                    </td>
                    <td>
                        <asp:Label ID="LabelUtilityInterest" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <b>Special Scale:</b><asp:Image ID="Image11" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                    <td>
                        <asp:Label ID="LabelSpecialScale" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Image ID="Image10" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Committee:</b>
                    </td>
                    <td>
                        <asp:Label ID="LabelCommittee" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <strong>Begining Date:</strong></td>
                    <td>
                        <asp:Label ID="BeginingDateT" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;</td>
                    <td>
                        &nbsp;</td>
                    <td>
                        <strong>Inactive Date:</strong>
                    </td>
                    <td>
                        <asp:Label ID="InactiveDateT" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Image ID="Image12" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Notes:</b>
                    </td>
                    <td colspan="3">
                        <asp:Label ID="LabelNotes" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Image ID="Image14" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <b>Accessory Units:</b>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <div>
                            <cc1:jqGridAdv runat="server" ID="jqGridAccUnit" colNames="['ID','Unit Num(*)', 'Description','Sqm','Area Type','OI','UI','SI']"
                                colModel="[
                    { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', search: false, hidden:true},
                    { name: 'Code', index: 'Code', width: 100, editable:true, editrules:{custom:true, custom_func:ValidateCode},  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'Description', index: 'Description', editable:true,  width: 150, editoptions: { dataEvents: [{type: 'focusout', fn: function(e) {ReplaceDQuote(this);}}]}, editrules:{custom:true, custom_func:StrNoDQuote},  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'AreaSqm', index: 'AreaSqm', editable:true, editrules:{custom:true, custom_func:DecimalNull}, width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'AreaType', index: 'AreaType', width: 100, editable:true, edittype:'select', editoptions:{dataUrl:'unitmasterdetails.aspx/BindAreaTypeSelector'},  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'OI', index: 'OI', editable:true,editrules:{custom:true, custom_func:DecimalNull}, width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'UI', index: 'UI', editable:true,editrules:{custom:true, custom_func:DecimalNull}, width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                    { name: 'SI', index: 'SI', editable:true,editrules:{custom:true, custom_func:DecimalNull}, width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
                ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="Code" sortorder="asc" viewrecords="true"
                                width="750" height="200" url="unitmasterdetails.aspx/AccUnitDataBind" hasID="false"
                                editurl="unitmasterdetails.aspx/AccUnitSaveData" inlineNav="true" />
                        </div>
                        <div style="text-align: left">
                            <asp:Button ID="ButtonDeleteAcc" runat="server" Text="Delete Acc Unit" PostBackUrl="~/unitmasterdetails.aspx"
                                OnClientClick="return ButtonDeleteAcc_ClientClick()" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="PanelActivitySelector" CssClass="PopupCSS" runat="server">
            <div>
                <table class="modaltable">
                    <tr>
                        <td>
                            Start Date:
                        </td>
                        <td>
                            <asp:TextBox ID="TextBoxActivityStart" runat="server" ClientIDMode="Static"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtenderActivityStart" Format="dd/MM/yyyy"
                                CssClass="sappcalendar" TargetControlID="TextBoxActivityStart" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            End Date:
                        </td>
                        <td>
                            <asp:TextBox ID="TextBoxActivityEnd" runat="server" ClientIDMode="Static"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtenderActivityEnd" CssClass="sappcalendar"
                                Format="dd/MM/yyyy" TargetControlID="TextBoxActivityEnd" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Image ID="Image17" runat="server" Height="10px" ImageUrl="~/images/transparent.png" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="ButtonActivityOK" runat="server" Text="OK" />
                            <asp:Button ID="ButtonActivityCancel" runat="server" Text="Cancel" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <ajaxToolkit:ModalPopupExtender ID="MPE2" runat="server" TargetControlID="ImageButtonActivity"
            PopupControlID="PanelActivitySelector" BackgroundCssClass="modalBackground" DropShadow="true"
            OkControlID="ButtonActivityOK" OnOkScript="ButtonActivityOKClick()" CancelControlID="ButtonActivityCancel">
        </ajaxToolkit:ModalPopupExtender>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Activity</div>
            <div>
                <asp:ImageButton ID="ImageButtonActivity" ImageUrl="~/images/activities.gif" runat="server" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Proprietors</div>
            <div>
                <asp:ImageButton ID="ImageButtonDebtor" ImageUrl="~/images/proprietor.gif" runat="server"
                    OnClick="ImageButtonDebtor_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Owner History</div>
            <div>
                <asp:ImageButton ID="ImageButtonOwnership" runat="server" ImageUrl="~/images/menubut_TeamDesk.gif"
                    OnClick="ImageButtonOwnership_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Transfer Ownership</div>
            <div>
                <asp:ImageButton ID="ImageButtonTranOS" runat="server" ImageUrl="~/images/transfer.gif"
                    OnClientClick="ImageButtonTranOS_ClientClick(); return false;" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Unit Journal</div>
            <div>
                <asp:ImageButton ID="ImageButtonUnitJournal" runat="server" ImageUrl="~/images/records.gif"
                    OnClick="ImageButtonUnitJournal_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Unit Statement</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/report.gif"
                    OnClick="ImageButton1_Click" Width="32px" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Discount Allocate</div>
            <div>
                <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/images/edit.gif" Width="32px"
                    OnClick="ImageButton3_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Reset Password</div>
            <div>
                <asp:ImageButton ID="ImageButton2" ImageUrl="~/images/proprietor.gif" runat="server"
                    OnClick="ImageButton2_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Refund</div>
            <div>
                <asp:ImageButton ID="ImageButton4" ImageUrl="~/images/proprietor.gif" runat="server"
                    OnClick="ImageButton4_Click" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Credit Note</div>
            <div>
                <asp:ImageButton ID="ImageButton5" ImageUrl="~/images/proprietor.gif" runat="server"
                    OnClick="ImageButton5_Click" />
            </div>
        </div>
    </div>
</asp:Content>
