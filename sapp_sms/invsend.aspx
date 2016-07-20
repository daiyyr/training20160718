<%@ Page Title="" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true"
    CodeBehind="invsend.aspx.cs" Inherits="sapp_sms.invsend" %>

<%@ Register Assembly="jqGridAdv" Namespace="jqGridAdv" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/sub.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/json2-min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_left">
    </div>
    <div id="content_middle">
        <div>
            <img src="Images/dot.gif" height="4px" />
            <cc1:jqGridAdv runat="server" ID="jqGridTable" colNames="['ID','Unit Num','Type','Description','Proprietor','Area Sqm','Area Type','OI','UI']"
                colModel="[
                  { name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
                  { name: 'Code', index: 'Code', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Type', index: 'Type', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Description', index: 'Description', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'Proprietor', index: 'Proprietor', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'AreaSqm', index: 'AreaSqm', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'AreaType', index: 'AreaType', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'OI', index: 'OI', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}},
                  { name: 'UI', index: 'UI', width: 100,  align: 'left', search: true,  searchoptions: { sopt: ['cn', 'nc']}}
              ]" rowNum="10000" rowList="[]" sortname="Code" sortorder="asc" viewrecords="true"
                width="800" height="500" url="invsend.aspx/DataGridDataBind" hasID="true" idName="propertyid"
                footerrow="true" userDataOnFooter="true" multiselect="true" />
            <div style="text-align: right; background-color: White; visibility: hidden;">
                <b>Total</b>&nbsp;&nbsp;&nbsp; <b>OI:</b>&nbsp;<asp:Label ID="LabelTotalOI" runat="server"
                    ForeColor="Blue"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <b>UI:</b>&nbsp;<asp:Label
                        ID="LabelTotalUI" runat="server" ForeColor="Blue"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </div>
        </div>
        <input id="Button1" type="button" value="Select" /><asp:Button ID="SendB" runat="server"
            Text="Send Email" PostBackUrl="~/privileges.aspx" OnClientClick="return SendClick()"
          />
        <script>
            function select() {
                var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
                $.ajax({
                    type: "Post",
                    contentType: "application/json;utf-8",
                    url: "invsend.aspx/Select",
                    data: "",
                    dataType: "json",
                    success: function (result) {
                        var ids = result.d.split('|');
                        var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
                        var rowIds = grid.jqGrid('getDataIDs');
                        for (i = 0; i < rowIds.length; i++) {
                            var unitcode = grid.getCell(rowIds[i], 'Code')
                            $.each(ids, function (index) {
                                if (ids[index] == unitcode)
                                    grid.jqGrid('setSelection', i+1);
                            });
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(XMLHttpRequest.status);
                        alert(XMLHttpRequest.readyState);
                        alert(textStatus);
                    }
                });
                alert("You Have Seleted The Primary Email");
            }
            $(document).ready(function () {
                $("#Button1").click(function () {
                    select();
                });
            });
            function SendClick() {
                var grid = jQuery("#" + GetClientId("jqGridTable") + "_datagrid1");
                var rowKeys = grid.getGridParam("selarrrow");

                if (rowKeys.length > 0) {
                    var IDs = "{\"IDs\":[";
                    for (i = 0; i < rowKeys.length; i++) {
                        if (i != (rowKeys.length - 1)) IDs += "\"" + grid.getCell(rowKeys[i], 'ID') + "\", ";
                        else IDs += "\"" + grid.getCell(rowKeys[i], 'ID') + "\"";
                    }
                    IDs += "]}";
                    __doPostBack('__Page', 'Send|' + IDs);
                    return false;
                }
                else {
                    alert("Please select a row first!");
                    return false;
                }

                return false;
            }
        </script>
    </div>
    <div id="content_right">
    </div>
</asp:Content>
