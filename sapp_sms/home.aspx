<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="sapp_sms.home" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Home</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery.jqGrid.validation.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Button ID="Button1" runat="server" Text="Button" onclick="Button1_Click" Visible="true"/>

    <div id="content_left">
        <div  class="button">
        <div class="button-title">Bodycorp</div>
        <div>
            <asp:ImageButton ID="ImageButtonBodycorp"
                    runat="server" ImageUrl="~/images/bodycorp.gif" onclick="ImageButtonBodycorp_Click" 
                     />
        </div>
        </div>
        <div  class="button">
        <div class="button-title">Property</div>
        <div>
            <asp:ImageButton ID="ImageButtonProperty"
                    runat="server" ImageUrl="~/images/property.gif" onclick="ImageButtonProperty_Click" 
                     />
        </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details">
                 <tr>
                    <td></td>
                    <td><img src="Images/dot.gif" height="4px", width="4px" /></td>
                    <td><img src="Images/dot.gif" height="4px", width="4px" /></td>
                    <td></td>
                    <td></td>
                 </tr>
                 <tr>
                    <td></td>
                    <td>
                        <asp:Chart ID="ChartBC" runat="server">
                            <Titles>
                                <asp:Title Text="Bodycorp"></asp:Title>
                            </Titles>
                            <Series>
                                <asp:Series Name="Series1" ChartType="Pie" Legend="Legend1">
                                    
                                </asp:Series>
                            </Series>
                            <ChartAreas>
                                <asp:ChartArea Name="ChartArea1">
                                </asp:ChartArea>
                            </ChartAreas>
                         <Legends>
                            <asp:Legend Name="Legend1"></asp:Legend>
                         </Legends>
                        </asp:Chart>
                    </td>
                    <td></td>
                    <td><img src="Images/dot.gif" height="250px", width="250px" /></td>
                    <td></td>
                 </tr>
                 <tr>
                    <td></td>
                    <td><img src="Images/dot.gif" height="4px", width="4px" /></td>
                    <td><img src="Images/dot.gif" height="4px", width="4px" /></td>
                    <td></td>
                    <td></td>
                 </tr>
                 <tr>
                    <td></td>
                    <td><img src="Images/dot.gif" height="250px", width="250px" /></td>
                     <td></td>
                    <td><img src="Images/dot.gif" height="250px", width="250px" /></td>
                    <td></td>
                 </tr>   
                 <tr>
                    <td></td>
                    <td><img src="Images/dot.gif" height="4px", width="4px" /></td>
                    <td><img src="Images/dot.gif" height="4px", width="4px" /></td>
                    <td></td>
                    <td></td>
                 </tr>
        </table>
        
    </div>
    <div id="content_right">
        <div  class="button">
                    <div class="button-title">Make Credit Note</div>
            <div>
                <asp:ImageButton ID="ImageButtonReports" ImageUrl="~/images/proprietor.gif" runat="server"                    onclick="ImageButtonReports_Click" />
            </div>
    </div>
            <div  class="button">
                    <div class="button-title">GL Fix</div>
            <div>
                <asp:ImageButton ID="ImageButton1" ImageUrl="~/images/proprietor.gif" 
                    runat="server"                    onclick="ImageButton1_Click" />
            </div>
    </div>
                <div  class="button">
                    <div class="button-title">GL Fix Description</div>
            <div>
                <asp:ImageButton ID="ImageButton2" ImageUrl="~/images/proprietor.gif" 
                    runat="server" onclick="ImageButton2_Click"/>
            </div>
    </div>
                    <div  class="button">
                    <div class="button-title">Credit note fix</div>
            <div>
                <asp:ImageButton ID="ImageButton3" ImageUrl="~/images/proprietor.gif" 
                    runat="server" onclick="ImageButton3_Click" />
            </div>
    </div>
</asp:Content>
