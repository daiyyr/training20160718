<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testpage.aspx.cs" Inherits="sapp_sms.testpage" %>
<%@ Register TagPrefix="uc" TagName="jqGrid" Src="~/jqGrid.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.11.custom.css" rel="stylesheet" type="text/css" />
    <link href="styles/tabs/Tabs.css" rel="stylesheet" type="text/css" />
    <script src="scripts/packages/jquery-1.5.2.min.js" type="text/javascript"></script>
    <script src="Scripts/packages/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/packages/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="scripts/testpage.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <%--<div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <img src="Images/dot.gif" height="4px" />
        <uc:jqGrid runat="server" ID="jqGridAccounts" colNames="['ID', 'Code', 'Name', 'Number']"
            colModel="[{ name: 'ID', index: 'ID', width: 50,editable:false, align: 'left', sorttype: 'int', search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} , hidden:true},
            { name: 'Code', index: 'Code', width: 100, align: 'left', search: true, searchoptions: { sopt: ['cn', 'nc']}},
            { name: 'Name', index: 'Name', width: 150, align: 'left', search: false},
            { name: 'Number', index: 'Number', width: 150, align: 'left', search: false}
            ]"
            rowNum="25"
            rowList="[5, 10, 25, 50, 100]"
            sortname="ID"
            sortorder="asc"
            viewrecords="true"
            width="700"
            height="500"
            url="testpage.aspx/DataTreeDataBind"
            hasID="false"
            />
        <ajaxToolkit:ComboBox ID="ComboBox1" runat="server">
        </ajaxToolkit:ComboBox>
    </div>--%>
    <%--<div>
        <asp:TreeView ID="TreeView1" runat="server">
        </asp:TreeView>
        
        <asp:LinkButton ID="LinkButtonShow" runat="server" Text="Show Modal Popup" ></asp:LinkButton>
        <input type="button" value="Show" runat="server" id="btnShow" />
        <asp:Panel ID="PanelContainer" runat="server">
            <asp:Label ID="Label1" runat="server" Text="ddd"></asp:Label>
        </asp:Panel>
        <ajaxToolkit:ModalPopupExtender ID="MPE" runat="server"
            TargetControlID="btnShow"
            PopupControlID="PanelContainer"
            DropShadow="true" 
            OkControlID="OkButton" 
            OnOkScript="onOk()"
            CancelControlID="CancelButton" 
            PopupDragHandleControlID="Panel3" >
        </ajaxToolkit:ModalPopupExtender>
    </div>--%>
    <%--<div>
        <asp:FileUpload ID="FileUploadDataFile" runat="server" size="50px" />
        <asp:Button ID="ButtonUpload" runat="server" Text="Upload" 
            CausesValidation="false" onclick="ButtonUpload_Click" />
        <asp:Button ID="ButtonCSV" runat="server" Text="CSV" 
            CausesValidation="false" onclick="ButtonCSV_Click" />
        <asp:Button ID="ButtonDown" runat="server" Text="CSV down" 
            CausesValidation="false" onclick="ButtonDown_Click"  />
    </div>--%>
    <div style="clear:left">
        <table id="list"><tr><td /></tr></table>
        <div id="pager"></div>
    </div>
    </form>
</body>
</html>
