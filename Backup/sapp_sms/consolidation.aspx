<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="consolidation.aspx.cs" Inherits="sapp_sms.consolidation" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Consolidation Edit</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="scripts/packages/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br>
    <asp:Label ID="MessageL" runat="server"></asp:Label>

    <table style="padding: 2px; margin-left: 20px; vertical-align: top;" border="1" 
        cellpadding="0" cellspacing="0">
        <tr bgcolor="#336699">
            <td width="40">No.</td>
            <td width="220">Function</td>
            <td width="280">Description</td>
            <td width="40">No.</td>
            <td width="220">Function</td>
            <td width="280">Description</td>
        </tr>
        <tr valign="top">
            <td>1</td>
            <td align="left"><asp:Button ID="Button8" runat="server" OnClick="Button8_Click" Text="REV Fix" /></td>
            <td align="left"></td>
            <td>2</td>
            <td align="left"><asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Discount Fix" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>3</td>
            <td align="left"><asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Recipt Unit ID" /></td>
            <td align="left"></td>
            <td>4</td>
            <td align="left"><asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="GL Journal Balance" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>5</td>
            <td align="left"><asp:Button ID="Button4" runat="server" OnClick="Button4_Click" Text="Check Invoice Gross" /></td>
            <td align="left"></td>
            <td>6</td>
            <td align="left"><asp:Button ID="Button5" runat="server" OnClick="Button5_Click" Text="Check CInvoice Gross" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>7</td>
            <td align="left"><asp:Button ID="Button6" runat="server" OnClick="Button6_Click" Text="Check Cpayment Gross" /></td>
            <td align="left"></td>
            <td>8</td>
            <td align="left"><asp:Button ID="Button7" runat="server" OnClick="Button7_Click" Text="Check Receipt Gross" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>9</td>
            <td align="left"><asp:Button ID="Button9" runat="server" OnClick="Button9_Click" Text="Fix Discount Amount" /></td>
            <td align="left"></td>
            <td>10</td>
            <td align="left"><asp:Button ID="Button10" runat="server" OnClick="Button10_Click" Text="Check Discount Amount" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>11</td>
            <td align="left"><asp:Button ID="Button11" runat="server" OnClick="Button11_Click" Text="Check CInv Inv" /></td>
            <td align="left"></td>
            <td>12</td>
            <td align="left"><asp:Button ID="Button12" runat="server" OnClick="Button12_Click" Text="Check GL Tax Gross Match" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>13</td>
            <td align="left"><asp:Button ID="Button13" runat="server" OnClick="Button13_Click" Text="Check Cin Num" /></td>
            <td align="left"></td>
            <td>14</td>
            <td align="left"><asp:Button ID="Button14" runat="server" OnClick="Button14_Click" Text="JNL GST CHECK" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>15</td>
            <td align="left"><asp:Button ID="Button15" runat="server" OnClick="Button15_Click" Text="Fix GL Rev Rec" /></td>
            <td align="left"></td>
            <td>16</td>
            <td align="left"><asp:Button ID="Button16" runat="server" OnClick="Button16_Click" Text="Input Output GST" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>17</td>
            <td align="left"><asp:Button ID="Button17" runat="server" OnClick="Button17_Click" Text="Check Inv Allocated" /></td>
            <td align="left"></td>
            <td>18</td>
            <td align="left"><asp:Button ID="Button22" runat="server" OnClick="Button22_Click" Text="Check CInv Allocated" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>19</td>
            <td align="left"><asp:Button ID="Button18" runat="server" OnClick="Button18_Click" Text="Fix Cash Payment Cash Despiot" /></td>
            <td align="left"></td>
            <td>20</td>
            <td align="left"><asp:Button ID="Button19" runat="server" OnClick="Button19_Click" Text="Inv Paid" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>21</td>
            <td align="left"><asp:Button ID="Button21" runat="server" OnClick="Button21_Click" Text="CInv Paid" /></td>
            <td align="left"></td>
            <td>22</td>
            <td align="left"><asp:Button ID="Button20" runat="server" OnClick="Button20_Click" Text="Inv Batch" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>23</td>
            <td align="left"><asp:Button ID="Button23" runat="server" OnClick="Button23_Click" Text="Build Reoport" /></td>
            <td align="left"></td>
            <td>24</td>
            <td align="left"><asp:Button ID="Button24" runat="server" OnClick="Button24_Click" Text="FIX DETOR" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>25</td>
            <td align="left"><asp:Button ID="Button25" runat="server" OnClick="Button25_Click" Text="Opening Balance" /></td>
            <td align="left"></td>
            <td>26</td>
            <td align="left"><asp:Button ID="Button26" runat="server" OnClick="Button26_Click" Text="Fix Detor Code" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>27</td>
            <td align="left"><asp:Button ID="Button27" runat="server" OnClick="Button27_Click" Text="Unit Date" /></td>
            <td align="left"></td>
            <td>28</td>
            <td align="left"><asp:Button ID="Button29" runat="server" OnClick="Button29_Click" Text="Beginning Date" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>29</td>
            <td align="left"><asp:Button ID="Button30" runat="server" OnClick="Button30_Click" Text="Delete Inactive Unit Invoice" /></td>
            <td align="left"></td>
            <td>30</td>
            <td align="left"><asp:Button ID="Button31" runat="server" OnClick="Button31_Click" Text="Change Refund" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>31</td>
            <td align="left"><asp:Button ID="Button32" runat="server" OnClick="Button32_Click" Text="Fix Refund" /></td>
            <td align="left"></td>
            <td>32</td>
            <td align="left"><asp:Button ID="Button33" runat="server" OnClick="Button33_Click" Text="Fix Rec" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>33</td>
            <td align="left"><asp:Button ID="Button34" runat="server" OnClick="Button34_Click" Text="Invoice Import" /></td>
            <td align="left"></td>
            <td>34</td>
            <td align="left"><asp:Button ID="Button28" runat="server" OnClick="Button28_Click" Text="Bulid Detor Code" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>35</td>
            <td align="left"><asp:Button ID="Button35" runat="server" OnClick="Button35_Click" Text="INV CIN Reallocate" /></td>
            <td align="left"></td>
            <td>36</td>
            <td align="left"><asp:Button ID="Button36" runat="server" OnClick="Button36_Click" Text="FIX INV CIN ALLOCATE" /></td>
            <td align="left"></td>
        </tr>
        <tr valign="top">
            <td>37</td>
            <td align="left"><asp:Button ID="Button37" runat="server" OnClick="Button37_Click" Text="GST Remover" /></td>
            <td align="left"><b>Important!</b> <br/>Remove GST records from table gl_transactions, cinvoice_gls, invoices; update GST data in gl_transactions, cinvoice, invoice_master, levies.</td>
            <td>38</td>
            <td align="left"><asp:Button ID="Button38" runat="server" OnClick="Button38_Click" Text="Bodycopr (18) fix cpayment" /></td>
            <td align="left"><b>Important!</b> </td>
        </tr>
    </table>
    
    <br />
    <div style="overflow: auto">
        <asp:GridView ID="GridView1" runat="server" Width="100%" CssClass="details">
        </asp:GridView>
    </div>

</asp:Content>
