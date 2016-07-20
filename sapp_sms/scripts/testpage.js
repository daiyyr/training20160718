//$(function () {
//    $("#demo1").jstree({
//        "json_data": {
//            "data": [
//				{
//				    "data": "A node",
//				    "metadata": { id: 23 },
//				    "children": ["Child 1", "A Child 2"]
//				},
//				{
//				    "attr": { "id": "li.node.id1" },
//				    "data": {
//				        "title": "Long format demo",
//				        "attr": { "href": "#" }
//				    }
//				}
//			]
//        },
//        "plugins": ["themes", "json_data", "ui"]
//    }).bind("select_node.jstree", function (e, data) { alert(data.rslt.obj.data("id")); });
//});

//$(function () {
    //    $("#demo1").jstree({
    //        "json_data": {
    //            "ajax": {
    //                "url": "testpage.aspx/DataTreeDataBind",
    //                "data": function (n) {
    //                        return {postdata:'something'};
    //                    }
    //                }
    //            }
    //        },
    //        "plugins":["themes", "json_data", "ui"]
    //    }).bind("select_node.jstree", function (e, data) { alert(data.rslt.obj.data("id")); });
//    $("#demo1").jstree({
//        "json_data": {
//            "ajax": {
//                "url": 'testpage.aspx/DataTreeDataBind',
//                "data": function (n) { return {postdata:'2'}; }
//            }
//        },
//        "plugins": ["themes", "json_data"]
//    });

//    $.ajax({
//        url: 'testpage.aspx/DataTreeDataBind',
//        data: "{postdata:'1'}",
//        dataType: "json",
//        type: "POST",
//        contentType: "application/json;charset=utf-8",
//        success: jqGridAccounts_datagrid1_successFunction
//    });

//});
$(document).ready(function () {
    'use strict';
    var mydata = [
                    { id: "1", invdate: "2007-10-01", name: "test", note: "note", amount: "200.00", tax: "10.00", closed: true, ship_via: "TN", total: "210.00" },
                    { id: "2", invdate: "2007-10-02", name: "test2", note: "note2", amount: "300.00", tax: "20.00", closed: false, ship_via: "FE", total: "320.00" },
                    { id: "3", invdate: "2007-09-01", name: "test3", note: "note3", amount: "400.00", tax: "30.00", closed: false, ship_via: "FE", total: "430.00" },
                    { id: "4", invdate: "2007-10-04", name: "test4", note: "note4", amount: "200.00", tax: "10.00", closed: true, ship_via: "TN", total: "210.00" },
                    { id: "5", invdate: "2007-10-31", name: "test5", note: "note5", amount: "300.00", tax: "20.00", closed: false, ship_via: "FE", total: "320.00" },
                    { id: "6", invdate: "2007-09-06", name: "test6", note: "note6", amount: "400.00", tax: "30.00", closed: false, ship_via: "FE", total: "430.00" },
                    { id: "7", invdate: "2007-10-04", name: "test7", note: "note7", amount: "200.00", tax: "10.00", closed: true, ship_via: "TN", total: "210.00" },
                    { id: "8", invdate: "2007-10-03", name: "test8", note: "note8", amount: "300.00", tax: "20.00", closed: false, ship_via: "FE", total: "320.00" },
                    { id: "9", invdate: "2007-09-01", name: "test9", note: "note9", amount: "400.00", tax: "30.00", closed: false, ship_via: "TN", total: "430.00" },
                    { id: "10", invdate: "2007-09-08", name: "test10", note: "note10", amount: "500.00", tax: "30.00", closed: true, ship_via: "TN", total: "530.00" },
                    { id: "11", invdate: "2007-09-08", name: "test11", note: "note11", amount: "500.00", tax: "30.00", closed: false, ship_via: "FE", total: "530.00" },
                    { id: "12", invdate: "2007-09-10", name: "test12", note: "note12", amount: "500.00", tax: "30.00", closed: false, ship_via: "FE", total: "530.00" }
                ],
                grid = $("#list"),
                gid = $.jgrid.jqID(grid[0].id),
                numberTemplate = { formatter: 'number', align: 'right', sorttype: 'number',
                    searchoptions: { sopt: ['eq', 'ne', 'lt', 'le', 'gt', 'ge', 'nu', 'nn', 'in', 'ni']}
                };

    grid.jqGrid({
        datatype: 'local',
        data: mydata,
        colNames: [/*'Inv No', */'Date', 'Client', 'Amount', 'Tax', 'Total', 'Closed', 'Shipped via', 'Notes'],
        colModel: [
        //{ name: 'id', index: 'id', width: 70, align: 'center', sorttype: 'int', searchoptions: { sopt: ['eq', 'ne']} },
                    {name: 'invdate', index: 'invdate', width: 75, align: 'center', sorttype: 'date',
                    formatter: 'date', formatoptions: { newformat: 'd-M-Y' }, datefmt: 'd-M-Y',
                    searchoptions: { sopt: ['eq', 'ne']}
                },
                    { name: 'name', index: 'name', width: 65, editrules: { required: true} },
                    { name: 'amount', index: 'amount', width: 65, template: numberTemplate },
                    { name: 'tax', index: 'tax', width: 52, template: numberTemplate },
                    { name: 'total', index: 'total', width: 60, template: numberTemplate },
                    { name: 'closed', index: 'closed', width: 67, align: 'center', formatter: 'checkbox',
                        edittype: 'checkbox', editoptions: { value: 'Yes:No', defaultValue: 'Yes' },
                        stype: 'select', searchoptions: { sopt: ['eq', 'ne'], value: ':Any;true:Yes;false:No'}
                    },
                    { name: 'ship_via', index: 'ship_via', width: 95, align: 'center', formatter: 'select',
                        edittype: 'select', editoptions: { value: 'FE:FedEx;TN:TNT;IN:Intim', defaultValue: 'IN' },
                        stype: 'select', searchoptions: { sopt: ['eq', 'ne'], value: ':Any;FE:FedEx;TN:TNT;IN:IN'}
                    },
                    { name: 'note', index: 'note', width: 60, sortable: false, edittype: 'textarea' }
                ],
        rowNum: 10,
        rowList: [5, 10, 20],
        pager: '#pager',
        gridview: true,
        rownumbers: true,
        autoencode: true,
        ignoreCase: true,
        sortname: 'invdate',
        viewrecords: true,
        sortorder: 'desc',
        caption: 'How to show/hide the navigator buttons dynamically',
        height: '100%'
    });
    grid.jqGrid('navGrid', '#pager', { view: true });
    grid.jqGrid(
                'navButtonAdd',
                '#pager',
                {
                    caption: "",
                    buttonicon: "ui-icon-calculator",
                    title: "choose columns",
                    onClickButton: function () {
                        grid.jqGrid('columnChooser', {
                            done: function (perm) {
                                if (perm) {
                                    grid.jqGrid("remapColumns", perm, true);
                                    //alert("The column chooser closed with 'OK' button");
                                } //else {
                                //  alert("The column chooser closed with 'Cancel' button");
                                //}
                            }
                        });
                    }
                }
            );
                //var $td = $('#add_' + gid);
                //$td.hide();
});