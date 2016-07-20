<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="unitmasteredit.aspx.cs" Inherits="sapp_sms.unitmasteredit" %>
<%@ Register TagPrefix="uc" TagName="jqGrid" Src="~/jqGrid.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Unit Master Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/common.js" type="text/javascript"></script>
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/unitmasteredit.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">Save</div>
            <div>
                <asp:ImageButton ID="ImageButtonSave"
                    runat="server" ImageUrl="Images/save.gif" onclick="ImageButtonSave_Click"/>
            </div>
        </div>
        <div  class="button">
        <div class="button-title">Cancel</div>
        <div>
            <asp:ImageButton ID="ImageButtonClose"
                runat="server" ImageUrl="Images/close.gif"  CausesValidation="false"  OnClientClick="history.back();; return false;"
                onclick="ImageButtonClose_Click" />
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
            <td><b>Unit Num(*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxCode" runat="server" ClientIDMode="Static"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxCode"></asp:RequiredFieldValidator>
                </td>
            <td><b>Type (*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxType" runat="server"
                 AutoPostBack="True"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 OnSelectedIndexChanged="ComboBoxType_SelectedIndexChanged"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidatorType" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorType_ServerValidate" ></asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <td><b>Know As:</b></td>
            <td>
                <asp:TextBox ID="TextBoxKnowAs" runat="server" ClientIDMode="Static"></asp:TextBox>
                </td>
            <td></td>
            <td>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Principal:</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxPrincipal" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                   </td>
            <td><b>Property (*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxProperty" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Enabled="false"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                   </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Proprietor(*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxDebtor" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <img alt="" src="images/smallNew.gif" onclick="openNewWindow('debtormasteredit.aspx?mode=add');" /><asp:RequiredFieldValidator 
                    ID="RequiredFieldValidator5" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="ComboBoxDebtor"></asp:RequiredFieldValidator>
                </td>
            <td><b>Size Sqm:</b></td>
            <td>
                <asp:TextBox ID="TextBoxArea" runat="server" ClientIDMode="Static" ></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender TargetControlID="TextBoxArea" ID="FilteredTextBoxExtender3" runat="server" FilterType="Custom, Numbers" ValidChars="."></ajaxToolkit:FilteredTextBoxExtender>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>

        <tr>
            <td><b>Area Type</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxAreaType" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
            </td>
            <td><b>Ownership Interest:</b></td>
            <td>
                <asp:TextBox ID="TextBoxOwnershipInterest" runat="server" ClientIDMode="Static"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender TargetControlID="TextBoxOwnershipInterest" ID="FilteredTextBoxExtender1" runat="server" FilterType="Custom, Numbers" ValidChars="."></ajaxToolkit:FilteredTextBoxExtender>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            
            <td><b>Utility Interest:</b></td>
            <td>
                <asp:TextBox ID="TextBoxUtilityInterest" runat="server" ClientIDMode="Static"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender TargetControlID="TextBoxUtilityInterest" ID="FilteredTextBoxExtender2" runat="server" FilterType="Custom, Numbers" ValidChars="."></ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td><b>Special Scale:</b></td>
            <td>
                <asp:TextBox ID="TextBoxSpecialScale" runat="server" ClientIDMode="Static"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender TargetControlID="TextBoxSpecialScale" ID="FilteredTextBoxExtender4" runat="server" FilterType="Custom, Numbers" ValidChars="."></ajaxToolkit:FilteredTextBoxExtender>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Begin Date (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxBeginDate" runat="server" ClientIDMode="Static"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarExtender3" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxBeginDate"/>   
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxBeginDate"></asp:RequiredFieldValidator>
            </td>
            <td><strong>Inactive Date:</strong></td>
            <td>
                <asp:TextBox ID="InactiveDateT" runat="server" ClientIDMode="Static"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="InactiveDateT_CalendarExtender" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="InactiveDateT"/>   
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            
            <td><b>Committee:</b></td>
            <td>
                <asp:CheckBox ID="CheckBoxCommittee" runat="server" Text=""></asp:CheckBox>
            </td>
            <td></td><td></td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Notes:</b></td>
            <td colspan="3">
                <asp:TextBox ID="TextBoxNotes" runat="server" TextMode="MultiLine" 
                    Width="500px" ClientIDMode="Static"></asp:TextBox>
                   </td>
        </tr>
        <tr>
            <td colspan="4"></td>
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



