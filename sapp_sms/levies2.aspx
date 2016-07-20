<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="levies2.aspx.cs" Inherits="sapp_sms.levies2" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Sapp SMS - Levies</title>
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/calendar/Calendar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
    <script src="scripts/packages/jquery.jqGrid.validation.js" type="text/javascript"></script>
    <script src="scripts/levies.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
        <div class="button">
            <div class="button-title">
                Close</div>
            <div>
                <asp:ImageButton ID="ImageButtonClose" runat="server" ImageUrl="Images/close.gif"
                    CausesValidation="false" OnClientClick="window.close(); return false;" />
            </div>
        </div>
    </div>
    <div id="content_middle">
        <div>
            <asp:Image ID="Image1" runat="server" ImageUrl="~/images/loading_small.gif" />
        </div>
        <asp:Timer ID="Timer1" runat="server" Interval="10000" OnTick="Timer1_Tick">
        </asp:Timer>
        <img src="Images/dot.gif" height="4px" />
        <cc1:jqGridAdv runat="server" ID="jqGridLevies" colNames="['ID','Bodycorp','Date','Description','Net']"
            colModel="[
                { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']}},
                { name: 'Bodycorp', index: 'Bodycorp', width: 50, editable:false, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                { name: 'Date', index: 'Date', width: 50, editable:false, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                { name: 'Description', index: 'Description', width: 200, editable:false, align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                { name: 'Net', index : 'Net', width: 50, editable:false,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
            ]" rowNum="25" rowList="[5, 10, 25, 50, 100]" sortname="ID" sortorder="asc" viewrecords="true"
            width="800" height="500" url="levies2.aspx/jqGridLeviesDataBind" hasID="false"
/>
    </div>
    <div id="content_right">
        <div class="button">
            <div class="button-title">
                Invoice</div>
            <div>
                <asp:ImageButton ID="ImageButtonInvoice" PostBackUrl="~/levies.aspx" runat="server"
                    ImageUrl="~/images/invoice.gif" OnClientClick="return ImageButtonInvoice_ClientClick()" />
            </div>
        </div>
    </div>
    <script>
//        $(document).ready(function () {
//            $("tr").click(function () {

//                alert("123");
//            })


//        });
        ////<![CDATA[

        //        $(document).ready(function () {
        //            try {
        //                var lastsel;
        //                var lastselsub;
        //                $('#ContentPlaceHolder1_jqGridLevies_datagrid1').jqGrid({
        //                    datatype: ContentPlaceHolder1_jqGridLevies_datagrid1_getProjects,
        //                    colNames: ['ID', 'Bodycorp', 'Date', 'Description', 'Total'],
        //                    colModel: [
        //                { name: 'ID', index: 'ID', width: 50, editable: false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} },
        //                { name: 'Bodycorp', index: 'Bodycorp', width: 50, editable: false, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']} },
        //                { name: 'Date', index: 'Date', width: 50, editable: false, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']} },
        //          { name: 'Description', index: 'Description', width: 200, editable: false, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']} },
        //              { name: 'Total', index: 'Total', width: 200, editable: false, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']} },
        //            ],
        //                    rowNum: 25,
        //                    rowList: [5, 10, 25, 50, 100],
        //                    sortname: 'ID',
        //                    pager: '#ContentPlaceHolder1_jqGridLevies_datagrid1pager',
        //                    sortorder: "asc",
        //                    viewrecords: true,
        //                    subGrid: true,
        //                    subGridRowExpanded: function (subgrid_id, row_id) {
        //                        $('#HiddenTempSubGridID').val(subgrid_id); $('#HiddenTempRowIDForSub').val(row_id); var subgrid_table_id;
        //                        subgrid_table_id = subgrid_id + '_t';
        //                        var subgrid_pager_id;
        //                        subgrid_pager_id = subgrid_id + '_pgr'
        //                        $('#' + subgrid_id).html("<table id='" + subgrid_table_id + "' class='scroll'></table><div id='" + subgrid_pager_id + "' class='scroll'></div>");
        //                        $("#" + subgrid_table_id).jqGrid({
        //                            datatype: ContentPlaceHolder1_jqGridLevies_subgrid_getProjects, colNames: ['ID', 'Bodycorp', 'Date', 'Description', 'Net'],
        //                            colModel: [
        //                { name: 'ID', index: 'ID', width: 50, editable: false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} },
        //                { name: 'Bodycorp', index: 'Bodycorp', width: 50, editable: false, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']} },
        //                { name: 'Date', index: 'Date', width: 50, editable: false, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']} },
        //                { name: 'Description', index: 'Description', width: 200, editable: false, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']} },
        //                { name: 'Net', index: 'Net', width: 50, editable: false, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']} },
        //            ],
        //                            sortname: '', pager: subgrid_pager_id,
        //                            sortorder: "", viewrecords: true,
        //                            height: '100%',
        //                            width: '600',
        //                            rowNum: 5
        //                            
        //                        });
        //                    },
        //                    width: '800',
        //                    height: '500'
        //                });
        //                $('#ContentPlaceHolder1_jqGridLevies_datagrid1').jqGrid('navGrid', '#ContentPlaceHolder1_jqGridLevies_datagrid1pager', { edit: false, add: false, del: false });
        //            } catch (error) {
        //                alert(error); JGAjaxErrorFunction();
        //            }
        //        });
        //        function ContentPlaceHolder1_jqGridLevies_datagrid1_getProjects(postdata) {
        //            try {
        //                var poststr = JSON.stringify(postdata);
        //                $.ajax({
        //                    error: function (response) { var r = jQuery.parseJSON(response.responseText); alert('ExceptionType: ' + r.ExceptionType + '\r\nMessage:' + r.Message); JGAjaxErrorFunction(); },
        //                    url: 'levies.aspx/jqGridLeviesDataBind',
        //                    data: "{postdata:'" + poststr + "'}",
        //                    dataType: "json",
        //                    type: "POST",
        //                    contentType: "application/json; charset=utf-8",
        //                    success: ContentPlaceHolder1_jqGridLevies_datagrid1_successFunction
        //                });
        //            } catch (error) {
        //                alert(error); JGAjaxErrorFunction();
        //            }
        //        }
        //        function ContentPlaceHolder1_jqGridLevies_datagrid1_successFunction(jsondata) {
        //            var thegrid = jQuery('#ContentPlaceHolder1_jqGridLevies_datagrid1')[0];
        //            thegrid.addJSONData(JSON.parse(jsondata.d));
        //        }
        //        function ContentPlaceHolder1_jqGridLeviesUpdateToServer(rowid, response) {
        //            try {
        //                var dataFromTheRow = jQuery('#ContentPlaceHolder1_jqGridLevies_datagrid1').jqGrid('getRowData', rowid);
        //                var rowValue = JSON.stringify(dataFromTheRow);
        //                $.ajax({
        //                    error: function (response) { var r = jQuery.parseJSON(response.responseText); alert('ExceptionType: ' + r.ExceptionType + '\r\nMessage:' + r.Message); JGAjaxErrorFunction(); },
        //                    url: '',
        //                    data: "{rowValue:'" + rowValue + "'}",
        //                    dataType: "json",
        //                    type: "POST", contentType: "application/json; charset=utf-8",
        //                    success: function () { $('#ContentPlaceHolder1_jqGridLevies_datagrid1').jqGrid('setCaption', '').trigger('reloadGrid'); }
        //                });
        //            } catch (error) {
        //                alert(error); JGAjaxErrorFunction();
        //            }
        //        }
        //        function ContentPlaceHolder1_jqGridLevies_subgrid_getProjects(postdata) {
        //            try {
        //                var poststr = JSON.stringify(postdata);
        //                var selectedID; selectedID = $('#HiddenTempRowIDForSub').val();
        //                var selectedRow; selectedRow = jQuery('#ContentPlaceHolder1_jqGridLevies_datagrid1').jqGrid('getRowData', selectedID);
        //                var rowValue = JSON.stringify(selectedRow);
        //                var poststr = JSON.stringify(postdata);
        //                $.ajax({
        //                    error: function (response) { var r = jQuery.parseJSON(response.responseText); alert('ExceptionType: ' + r.ExceptionType + '\r\nMessage:' + r.Message); JGAjaxErrorFunction(); },
        //                    url: 'levies.aspx/FillSubGrid',
        //                    data: "{postdata:'" + poststr + "', masterRow:'" + rowValue + "'}",
        //                    dataType: "json",
        //                    type: "POST",
        //                    contentType: "application/json; charset=utf-8",
        //                    success: ContentPlaceHolder1_jqGridLevies_subdatagrid1_successFunction
        //                });
        //            } catch (error) {
        //                alert(error);
        //            }
        //        }
        //        function ContentPlaceHolder1_jqGridLevies_subdatagrid1_successFunction(jsondata) {
        //            var gridid = $('#HiddenTempSubGridID').val();
        //            var thegrid = jQuery('#' + gridid + '_t')[0];
        //            thegrid.addJSONData(JSON.parse(jsondata.d));
        //        }
        ////]]>

    </script>
</asp:Content>
