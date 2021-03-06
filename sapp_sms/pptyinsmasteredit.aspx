﻿<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="pptyinsmasteredit.aspx.cs" Inherits="sapp_sms.pptyinsmasteredit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Pptyins Master Edit</title>
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
            
            <td><b>Policy Num (*):</b></td>
            <td>
                <asp:TextBox ID="TextBoxPolicyNum" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBoxPolicyNum"
                    ErrorMessage="!" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
            <td><b>Broker (*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxBroker" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidatorBroker" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorBroker_ServerValidate" ></asp:CustomValidator>
            </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>

        <tr>
            
            <td><b>Under Writer (*):</b></td>
            <td>
                <ajaxToolkit:ComboBox ID="ComboBoxUnderWriter" runat="server"
                 AutoPostBack="False"
                 DropDownStyle="DropDownList"
                 AutoCompleteMode="SuggestAppend"
                 CaseSensitive="False"
                 ItemInsertLocation="Append"
                 Width="150px">
                </ajaxToolkit:ComboBox>
                <asp:CustomValidator ID="CustomValidatorUnderWriter" runat="server" ForeColor="Red"
                ErrorMessage="!" onservervalidate="CustomValidatorUnderWriter_ServerValidate" ></asp:CustomValidator>
            </td>
            <td><b>Placement:</b></td>
            <td>
                <asp:TextBox ID="TextBoxPlacement" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>


        <tr>
            <td><b>Start:</b></td>
            <td>
                <asp:TextBox ID="TextBoxStart" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarExtenderStart" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxStart"/>  
            </td>
            <td><b>End:</b></td>
            <td>
                <asp:TextBox ID="TextBoxEnd" runat="server"></asp:TextBox>
                <ajaxToolkit:CalendarExtender runat="server"
                    ID="CalendarExtender1" CssClass="sappcalendar"
                    Format="dd/MM/yyyy"
                    TargetControlID="TextBoxEnd"/>  
                <asp:HiddenField ID="OldEndHF" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
        


        <tr>
            
            <td><b>Premium:</b></td>
            <td>
                <asp:TextBox ID="TextBoxPremium" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender TargetControlID="TextBoxPremium" 
                    ID="FilteredTextBoxExtender3" runat="server" FilterType="Custom, Numbers" ValidChars="." ></ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td><b>Excess:</b></td>
            <td>
                <asp:TextBox ID="TextBoxExcess" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender TargetControlID="TextBoxExcess" 
                    ID="FilteredTextBoxExtender1" runat="server" FilterType="Custom, Numbers" ValidChars="." ></ajaxToolkit:FilteredTextBoxExtender>
            </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>

        <tr>
            
            <td><b>Insvt:</b></td>
            <td>
                <asp:TextBox ID="TextBoxInsvt" runat="server"></asp:TextBox>
                <ajaxToolkit:FilteredTextBoxExtender TargetControlID="TextBoxInsvt" 
                    ID="FilteredTextBoxExtenderInsvt" runat="server" FilterType="Custom, Numbers" ValidChars="." ></ajaxToolkit:FilteredTextBoxExtender>
            </td>
            <td><b>Cover:</b></td>
            <td>
                <asp:TextBox runat="server" ID="TextBoxCover"></asp:TextBox>
            </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>

        <tr>
            
            <td><b>GST:</b></td>
            <td>
                <asp:TextBox runat="server" ID="TextBoxGST"></asp:TextBox>
            </td>
            <td><b></b></td>
            <td>
                
            </td>
        </tr>
        <tr>
        <td colspan="4"></td>
        </tr>





        <tr>
            <td ><b>Notes:</b></td>
            <td colspan="3">
                <asp:TextBox ID="TextBoxNotes" runat="server" TextMode="MultiLine" 
                    Width="500px" /></td>
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


