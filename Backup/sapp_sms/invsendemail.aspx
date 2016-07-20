<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="invsendemail.aspx.cs" Inherits="sapp_sms.invsendemail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/unitmaster.js" type="text/javascript"></script>
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" CausesValidation="false" 
                    ImageUrl="Images/close.gif" OnClientClick="history.back(); return false;" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <div>
            <table class="details">
                <tr>
                    <td>
                        Date Between:
                    </td>
                    <td>
                        <asp:TextBox ID="IStartT" runat="server"></asp:TextBox>
                        <ajaxToolkit:CalendarExtender ID="IStartT_CalendarExtender" runat="server" Enabled="True"
                            TargetControlID="IStartT" CssClass="sappcalendar" Format="dd/MM/yyyy">
                        </ajaxToolkit:CalendarExtender>
                        To<asp:TextBox ID="IEndT" runat="server"></asp:TextBox>
                        <ajaxToolkit:CalendarExtender ID="IEndT_CalendarExtender" runat="server" Enabled="True"
                            TargetControlID="IEndT" CssClass="sappcalendar" Format="dd/MM/yyyy">
                        </ajaxToolkit:CalendarExtender>
                    </td>
                    <td>
                        &nbsp; Send Inv:<asp:CheckBox ID="InvCheckBox" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Statement Date:
                    </td>
                    <td>
                        <asp:TextBox ID="SEDateT" runat="server"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" Enabled="True"
                            TargetControlID="SEDateT" CssClass="sappcalendar" Format="dd/MM/yyyy">
                        </ajaxToolkit:CalendarExtender>
                    </td>
                    <td>
                        &nbsp; Send Statement:<asp:CheckBox ID="StatementCheckBox" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Notes:
                    </td>
                    <td>
                        <asp:FileUpload ID="FileUpload1" runat="server" />
                        <asp:Button ID="UploadB" runat="server" OnClick="UploadB_Click" Text="Upload" />
                        <asp:GridView ID="GridView1" CssClass="details" runat="server" 
                            AutoGenerateColumns="False" Width="100%">
                            <Columns>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:Button ID="DeleteB" CommandName='<%# Bind("FileName") %>' runat="server" OnClick="DeleteB_Click"
                                            Text="Delete" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="FileName" HeaderText="File" />
                                <asp:TemplateField>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("FileName") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("FileName") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                    <td>
                        &nbsp; Send Notes:<asp:CheckBox ID="NotesCheckBox" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        &nbsp;
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        Email Subject:<asp:TextBox ID="SubT" runat="server" Width="600px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        Email Body:
                    </td>
                </tr>
                <tr>
                    <td colspan="3" width="600">
                        <asp:TextBox ID="BodyT" runat="server" Height="300px" Width="100%" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        &nbsp;
                    </td>
                    <td>
                        &nbsp;
                        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Send" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="content_right">
        </div>
        <div>
        </div>
</asp:Content>
