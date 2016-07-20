<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="pptyinsmasterdetails.aspx.cs" Inherits="sapp_sms.pptyinsmasterdetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Property Contractor Details</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="scripts/pptyinsmasterdetails.js" type="text/javascript"></script>
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
            <td><b>Element ID:</b><asp:Image ID="Image1" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
            <td colspan="3">
                <b><asp:Label ID="LabelElementID" runat="server" Text="ID"  Visible="False"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image2" runat="server" Height="10px" ImageUrl="~/images/transparent.png" /></td>
        </tr>

        <tr>
            
            <td><b>Type :<asp:Image ID="Image3" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></b></td>
            <td>
                <asp:Label ID="LabelType" runat="server"></asp:Label>
                </td>
            <td><b>Property :</b></td>
            <td>
                <asp:Label ID="LabelProperty" runat="server"></asp:Label>
                </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image4" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></td>
        </tr>
        <tr>
            
            <td><b>Policy Num :<asp:Image ID="Image5" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></b></td>
            <td>
                <asp:Label ID="LabelPolicyNum" runat="server"></asp:Label>
                </td>
            <td><b>Broker :</b></td>
            <td>
                <asp:Label ID="LabelBroker" runat="server"></asp:Label>
                </td>
        </tr>
        <tr>
        <td colspan="4"><asp:Image ID="Image6" runat="server" Height="10px" 
                ImageUrl="~/images/transparent.png" /></td>
        </tr>

        <tr>
            
            <td><b>Under Writer :<asp:Image ID="Image7" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></b></td>
            <td>
                <asp:Label ID="LabelUnderWriter" runat="server"></asp:Label>
            </td>
            <td><b>Placement:</b></td>
            <td>
                <asp:Label ID="LabelPlacement" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
        <td colspan="4"><asp:Image ID="Image8" runat="server" Height="10px" 
                ImageUrl="~/images/transparent.png" /></td>
        </tr>


        <tr>
            <td><b>Start:<asp:Image ID="Image9" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></b></td>
            <td>
                <asp:Label ID="LabelStart" runat="server"></asp:Label>
            </td>
            <td><b>End:</b></td>
            <td>
                <asp:Label ID="LabelEnd" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image10" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></td>
        </tr>
        


        <tr>
            
            <td><b>Premium:<asp:Image ID="Image11" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></b></td>
            <td>
                <asp:Label ID="LabelPremium" runat="server"></asp:Label>
            </td>
            <td><b>Excess:</b></td>
            <td>
                <asp:Label ID="LabelExcess" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
        <td colspan="4"><asp:Image ID="Image12" runat="server" Height="10px" 
                ImageUrl="~/images/transparent.png" /></td>
        </tr>

        <tr>
            
            <td><b>Insvt:<asp:Image ID="Image13" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></b></td>
            <td>
                <asp:Label ID="LabelInsvt" runat="server"></asp:Label>
            </td>
            <td><b>Cover:</b></td>
            <td>
                <asp:Label runat="server" ID="LabelCover"></asp:Label>
            </td>
        </tr>
        <tr>
        <td colspan="4"><asp:Image ID="Image14" runat="server" Height="10px" 
                ImageUrl="~/images/transparent.png" /></td>
        </tr>

        <tr>
            
            <td><b>GST:<asp:Image ID="Image15" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></b></td>
            <td>
                <asp:Label runat="server" ID="LabelGST"></asp:Label>
            </td>
            <td><b></b></td>
            <td>
                
            </td>
        </tr>
        <tr>
        <td colspan="4"><asp:Image ID="Image16" runat="server" Height="10px" 
                ImageUrl="~/images/transparent.png" /></td>
        </tr>





        <tr>
            <td ><b>Notes:<asp:Image ID="Image17" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></b></td>
            <td colspan="3">
                <asp:Label ID="LabelNotes" runat="server" TextMode="MultiLine" 
                    Width="500px" /></td>
        </tr>
        <tr>
            <td colspan="4"><asp:Image ID="Image18" runat="server" Height="10px" 
                    ImageUrl="~/images/transparent.png" /></td>
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


