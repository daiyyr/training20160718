<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="pptymaintmasteredit.aspx.cs" Inherits="sapp_sms.pptymaintmasteredit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Pptymaint Master Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
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
            runat="server" ImageUrl="Images/close.gif"  CausesValidation="false"  OnClientClick="history.back(); return false;"
            onclick="ImageButtonClose_Click" />
    </div>
    </div>
</div>
    <div id="content_middle">
    <table class="details">
        <tr>
            <td><b>Element ID:</b></td>
            <td colspan="3">
                <b><asp:Label ID="LabelElementID" runat="server" Text="ID"  Visible="False"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>

        <tr>
            
            <td><b>Type (*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxType" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidatorType" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorType_ServerValidate" ></asp:CustomValidator>
            </td>
            <td><b>Property (*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxProperty" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidator1" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorProperty_ServerValidate" ></asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        <tr>
            
            <td><b>Contractor (*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxCredit" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidator2" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorCredit_ServerValidate" ></asp:CustomValidator>
            </td>
            <td><b>Unit:</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxUnit" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>

            </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>
        <tr>
            <td><b>Compliance (*):</b></td>
            <td>
                <asp:CheckBox ID="CheckBoxCompliance" runat="server" />
            </td>
            <td></td><td></td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>

        <tr>
            <td><b>Due (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxDue" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarDue" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxDue"/>   
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorDue" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxDue"></asp:RequiredFieldValidator>   
                <asp:HiddenField ID="OldEndHF" runat="server" />
            </td>
            <td><b>Next Due (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxNextDue" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarExtenderNextDue" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxNextDue"/>  
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorNextDue" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxNextDue"></asp:RequiredFieldValidator>   
            </td>

        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>
        <tr>
            
            <td><b>Freq (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxFreq" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="TextBoxFreq" FilterType="Numbers"></ajaxToolkit:FilteredTextBoxExtender>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorFreq" runat="server" ErrorMessage="!" ForeColor="Red"
                   ControlToValidate="TextBoxFreq"></asp:RequiredFieldValidator>
            </td>
            <td><b>Freq Type (*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxFreqType" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidator3" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorFreq_ServerValidate" ></asp:CustomValidator>
            </td>

        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        

        <tr>
            <td><b>Notes:</b></td>
            <td colspan="3">
                <asp:TextBox ID="TextBoxNotes" TextMode="MultiLine" Width="500" runat="server" />
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

