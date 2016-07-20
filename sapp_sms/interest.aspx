<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="interest.aspx.cs" Inherits="sapp_sms.interest" %>

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
    <script src="scripts/packages/jquery.dataTables.js" type="text/javascript"></script>
    <style type="text/css">
/*
 *  File:         demo_table.css
 *  CVS:          $Id$
 *  Description:  CSS descriptions for DataTables demo pages
 *  Author:       Allan Jardine
 *  Created:      Tue May 12 06:47:22 BST 2009
 *  Modified:     $Date$ by $Author$
 *  Language:     CSS
 *  Project:      DataTables
 *
 *  Copyright 2009 Allan Jardine. All Rights Reserved.
 *
 * ***************************************************************************
 * DESCRIPTION
 *
 * The styles given here are suitable for the demos that are used with the standard DataTables
 * distribution (see www.datatables.net). You will most likely wish to modify these styles to
 * meet the layout requirements of your site.
 *
 * Common issues:
 *   'full_numbers' pagination - I use an extra selector on the body tag to ensure that there is
 *     no conflict between the two pagination types. If you want to use full_numbers pagination
 *     ensure that you either have "example_alt_pagination" as a body class name, or better yet,
 *     modify that selector.
 *   Note that the path used for Images is relative. All images are by default located in
 *     ../images/ - relative to this CSS file.
 */

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * DataTables features
 */

.dataTables_wrapper {
	position: relative;
	clear: both;
	zoom: 1; /* Feeling sorry for IE */
}

.dataTables_processing {
	position: absolute;
	top: 50%;
	left: 50%;
	width: 250px;
	height: 30px;
	margin-left: -125px;
	margin-top: -15px;
	padding: 14px 0 2px 0;
	border: 1px solid #ddd;
	text-align: center;
	color: #999;
	font-size: 14px;
	background-color: white;
}

.dataTables_length {
	width: 40%;
	float: left;
}

.dataTables_filter {
	width: 50%;
	float: right;
	text-align: right;
}

.dataTables_info {
	width: 60%;
	float: left;
}

.dataTables_paginate {
	float: right;
	text-align: right;
}

/* Pagination nested */
.paginate_disabled_previous, .paginate_enabled_previous,
.paginate_disabled_next, .paginate_enabled_next {
	height: 19px;
	float: left;
	cursor: pointer;
	*cursor: hand;
	color: #111 !important;
}
.paginate_disabled_previous:hover, .paginate_enabled_previous:hover,
.paginate_disabled_next:hover, .paginate_enabled_next:hover {
	text-decoration: none !important;
}
.paginate_disabled_previous:active, .paginate_enabled_previous:active,
.paginate_disabled_next:active, .paginate_enabled_next:active {
	outline: none;
}

.paginate_disabled_previous,
.paginate_disabled_next {
	color: #666 !important;
}
.paginate_disabled_previous, .paginate_enabled_previous {
	padding-left: 23px;
}
.paginate_disabled_next, .paginate_enabled_next {
	padding-right: 23px;
	margin-left: 10px;
}

.paginate_disabled_previous {
	background: url('../images/back_disabled.png') no-repeat top left;
}

.paginate_enabled_previous {
	background: url('../images/back_enabled.png') no-repeat top left;
}
.paginate_enabled_previous:hover {
	background: url('../images/back_enabled_hover.png') no-repeat top left;
}

.paginate_disabled_next {
	background: url('../images/forward_disabled.png') no-repeat top right;
}

.paginate_enabled_next {
	background: url('../images/forward_enabled.png') no-repeat top right;
}
.paginate_enabled_next:hover {
	background: url('../images/forward_enabled_hover.png') no-repeat top right;
}



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * DataTables display
 */
table.display {
	margin: 0 auto;
	clear: both;
	width: 100%;
	
	/* Note Firefox 3.5 and before have a bug with border-collapse
	 * ( https://bugzilla.mozilla.org/show%5Fbug.cgi?id=155955 ) 
	 * border-spacing: 0; is one possible option. Conditional-css.com is
	 * useful for this kind of thing
	 *
	 * Further note IE 6/7 has problems when calculating widths with border width.
	 * It subtracts one px relative to the other browsers from the first column, and
	 * adds one to the end...
	 *
	 * If you want that effect I'd suggest setting a border-top/left on th/td's and 
	 * then filling in the gaps with other borders.
	 */
}

table.display thead th {
	padding: 3px 18px 3px 10px;
	border-bottom: 1px solid black;
	font-weight: bold;
	cursor: pointer;
	* cursor: hand;
}

table.display tfoot th {
	padding: 3px 18px 3px 10px;
	border-top: 1px solid black;
	font-weight: bold;
}

table.display tr.heading2 td {
	border-bottom: 1px solid #aaa;
}

table.display td {
	padding: 3px 10px;
}

table.display td.center {
	text-align: center;
}



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * DataTables sorting
 */

.sorting_asc {
	background: url('../images/sort_asc.png') no-repeat center right;
}

.sorting_desc {
	background: url('../images/sort_desc.png') no-repeat center right;
}

.sorting {
	background: url('../images/sort_both.png') no-repeat center right;
}

.sorting_asc_disabled {
	background: url('../images/sort_asc_disabled.png') no-repeat center right;
}

.sorting_desc_disabled {
	background: url('../images/sort_desc_disabled.png') no-repeat center right;
}
 
table.display thead th:active,
table.display thead td:active {
	outline: none;
}




/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * DataTables row classes
 */
table.display tr.odd.gradeA {
	background-color: #ddffdd;
}

table.display tr.even.gradeA {
	background-color: #eeffee;
}

table.display tr.odd.gradeC {
	background-color: #ddddff;
}

table.display tr.even.gradeC {
	background-color: #eeeeff;
}

table.display tr.odd.gradeX {
	background-color: #ffdddd;
}

table.display tr.even.gradeX {
	background-color: #ffeeee;
}

table.display tr.odd.gradeU {
	background-color: #ddd;
}

table.display tr.even.gradeU {
	background-color: #eee;
}


tr.odd {
	background-color: #E2E4FF;
}

tr.even {
	background-color: white;
}





/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Misc
 */
.dataTables_scroll {
	clear: both;
}

.dataTables_scrollBody {
	*margin-top: -1px;
	-webkit-overflow-scrolling: touch;
}

.top, .bottom {
	padding: 15px;
	background-color: #F5F5F5;
	border: 1px solid #CCCCCC;
}

.top .dataTables_info {
	float: none;
}

.clear {
	clear: both;
}

.dataTables_empty {
	text-align: center;
}

tfoot input {
	margin: 0.5em 0;
	width: 100%;
	color: #444;
}

tfoot input.search_init {
	color: #999;
}

td.group {
	background-color: #d1cfd0;
	border-bottom: 2px solid #A19B9E;
	border-top: 2px solid #A19B9E;
}

td.details {
	background-color: #d1cfd0;
	border: 2px solid #A19B9E;
}


.example_alt_pagination div.dataTables_info {
	width: 40%;
}

.paging_full_numbers {
	width: 400px;
	height: 22px;
	line-height: 22px;
}

.paging_full_numbers a:active {
	outline: none
}

.paging_full_numbers a:hover {
	text-decoration: none;
}

.paging_full_numbers a.paginate_button,
 	.paging_full_numbers a.paginate_active {
	border: 1px solid #aaa;
	-webkit-border-radius: 5px;
	-moz-border-radius: 5px;
	padding: 2px 5px;
	margin: 0 3px;
	cursor: pointer;
	*cursor: hand;
	color: #333 !important;
}

.paging_full_numbers a.paginate_button {
	background-color: #ddd;
}

.paging_full_numbers a.paginate_button:hover {
	background-color: #ccc;
	text-decoration: none !important;
}

.paging_full_numbers a.paginate_active {
	background-color: #99B3FF;
}

table.display tr.even.row_selected td {
	background-color: #B0BED9;
}

table.display tr.odd.row_selected td {
	background-color: #9FAFD1;
}


/*
 * Sorting classes for columns
 */
/* For the standard odd/even */
tr.odd td.sorting_1 {
	background-color: #D3D6FF;
}

tr.odd td.sorting_2 {
	background-color: #DADCFF;
}

tr.odd td.sorting_3 {
	background-color: #E0E2FF;
}

tr.even td.sorting_1 {
	background-color: #EAEBFF;
}

tr.even td.sorting_2 {
	background-color: #F2F3FF;
}

tr.even td.sorting_3 {
	background-color: #F9F9FF;
}


/* For the Conditional-CSS grading rows */
/*
 	Colour calculations (based off the main row colours)
  Level 1:
		dd > c4
		ee > d5
	Level 2:
	  dd > d1
	  ee > e2
 */
tr.odd.gradeA td.sorting_1 {
	background-color: #c4ffc4;
}

tr.odd.gradeA td.sorting_2 {
	background-color: #d1ffd1;
}

tr.odd.gradeA td.sorting_3 {
	background-color: #d1ffd1;
}

tr.even.gradeA td.sorting_1 {
	background-color: #d5ffd5;
}

tr.even.gradeA td.sorting_2 {
	background-color: #e2ffe2;
}

tr.even.gradeA td.sorting_3 {
	background-color: #e2ffe2;
}

tr.odd.gradeC td.sorting_1 {
	background-color: #c4c4ff;
}

tr.odd.gradeC td.sorting_2 {
	background-color: #d1d1ff;
}

tr.odd.gradeC td.sorting_3 {
	background-color: #d1d1ff;
}

tr.even.gradeC td.sorting_1 {
	background-color: #d5d5ff;
}

tr.even.gradeC td.sorting_2 {
	background-color: #e2e2ff;
}

tr.even.gradeC td.sorting_3 {
	background-color: #e2e2ff;
}

tr.odd.gradeX td.sorting_1 {
	background-color: #ffc4c4;
}

tr.odd.gradeX td.sorting_2 {
	background-color: #ffd1d1;
}

tr.odd.gradeX td.sorting_3 {
	background-color: #ffd1d1;
}

tr.even.gradeX td.sorting_1 {
	background-color: #ffd5d5;
}

tr.even.gradeX td.sorting_2 {
	background-color: #ffe2e2;
}

tr.even.gradeX td.sorting_3 {
	background-color: #ffe2e2;
}

tr.odd.gradeU td.sorting_1 {
	background-color: #c4c4c4;
}

tr.odd.gradeU td.sorting_2 {
	background-color: #d1d1d1;
}

tr.odd.gradeU td.sorting_3 {
	background-color: #d1d1d1;
}

tr.even.gradeU td.sorting_1 {
	background-color: #d5d5d5;
}

tr.even.gradeU td.sorting_2 {
	background-color: #e2e2e2;
}

tr.even.gradeU td.sorting_3 {
	background-color: #e2e2e2;
}


/*
 * Row highlighting example
 */
.ex_highlight #example tbody tr.even:hover, #example tbody tr.even td.highlighted {
	background-color: #ECFFB3;
}

.ex_highlight #example tbody tr.odd:hover, #example tbody tr.odd td.highlighted {
	background-color: #E6FF99;
}

.ex_highlight_row #example tr.even:hover {
	background-color: #ECFFB3;
}

.ex_highlight_row #example tr.even:hover td.sorting_1 {
	background-color: #DDFF75;
}

.ex_highlight_row #example tr.even:hover td.sorting_2 {
	background-color: #E7FF9E;
}

.ex_highlight_row #example tr.even:hover td.sorting_3 {
	background-color: #E2FF89;
}

.ex_highlight_row #example tr.odd:hover {
	background-color: #E6FF99;
}

.ex_highlight_row #example tr.odd:hover td.sorting_1 {
	background-color: #D6FF5C;
}

.ex_highlight_row #example tr.odd:hover td.sorting_2 {
	background-color: #E0FF84;
}

.ex_highlight_row #example tr.odd:hover td.sorting_3 {
	background-color: #DBFF70;
}


/*
 * KeyTable
 */
table.KeyTable td {
	border: 3px solid transparent;
}

table.KeyTable td.focus {
	border: 3px solid #3366FF;
}

table.display tr.gradeA {
	background-color: #eeffee;
}

table.display tr.gradeC {
	background-color: #ddddff;
}

table.display tr.gradeX {
	background-color: #ffdddd;
}

table.display tr.gradeU {
	background-color: #ddd;
}

div.box {
	height: 100px;
	padding: 10px;
	overflow: auto;
	border: 1px solid #8080FF;
	background-color: #E5E5FF;
}

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" CausesValidation="false" ImageUrl="Images/close.gif"
                    OnClientClick="history.back(); return false;" />
            </div>
        </div>
        <div class="button">
            <div class="button-title">
                Submit</div>
            <div>
                <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="false" ImageUrl="~/images/submit.gif"
                    OnClick="ImageButton1_Click" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <table class="details" style="width: 100%">
            <tr>
                <td>
                    Start Date:
                </td>
                <td>
                    <asp:TextBox ID="StartDateT" runat="server" ReadOnly="True"></asp:TextBox>
                </td>
                <td>
                    Date:
                    <td>
                        <asp:TextBox ID="EndDateT" runat="server"></asp:TextBox>
                        <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtenderActivityStart" Format="dd/MM/yyyy"
                            CssClass="sappcalendar" TargetControlID="EndDateT" />
                    </td>
                    <td>
                        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click1" Text="Submit" />
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/images/loading_small.gif" Visible="False" />
                    </td>
            </tr>
        </table>
        <asp:Timer ID="Timer1" runat="server" Interval="5000" OnTick="Timer1_Tick" Enabled="False">
        </asp:Timer>
        <div style="text-align: left;">
            <asp:GridView ID="GridView1" runat="server" CssClass="details" AutoGenerateColumns="False"
                Width="100%">
                <Columns>
                    <asp:BoundField DataField="Unit" HeaderText="Unit" />
                    <asp:BoundField DataField="Amount" HeaderText="Amount" />
                    <asp:BoundField DataField="Interest" HeaderText="Interest" />
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:CheckBox ID="AllCheck" runat="server" AutoPostBack="true" runat="server" Checked="true"
                                OnCheckedChanged="CheckBox2_CheckedChanged" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="CheckBox1" runat="server" Checked="true" ToolTip='<%# Bind("UnitID") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <script>
        $("#ContentPlaceHolder1_GridView1").DataTable({ "bStateSave": true,
            "sPaginationType": "full_numbers", "sScrollY": "500px",
            "aLengthMenu": [[30, 50, 100, -1], [30, 50, 100, "All"]],
            "iDisplayLength": 30
        });
    </script>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Export</div>
            <div>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/export_pdf.gif"
                    OnClick="ImageButton2_Click" />
            </div>
        </div>
    </div>
</asp:Content>
