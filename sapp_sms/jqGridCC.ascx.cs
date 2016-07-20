using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.Common;
using Sapp.Data;
using Sapp.JQuery;

namespace sapp_sms
{
    public partial class jqGridCC : System.Web.UI.UserControl
    {
        public string colNames { get; set; }
        public string colModel { get; set; }
        public string rowNum { get; set; }
        public string rowList { get; set; }
        public string sortname { get; set; }
        public string sortorder { get; set; }
        public string viewrecords { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string url { get; set; }
        public string hasID { get; set; }
        public string idName { get; set; }
        public string multiselect { get; set; }
        public string editurl { get; set; }

        #region Setting For Detail Grid
        public string onSelectRowUrl { get; set; }
        public string dcolNames { get; set; }
        public string dcolModel { get; set; }
        public string drowNum { get; set; }
        public string drowList { get; set; }
        public string dsortName { get; set; }
        public string dsortOrder { get; set; }
        public string dwidth { get; set; }
        public string dheight { get; set; }
        public string dmultiSelect { get; set; }
        public string deditUrl { get; set; }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            string script = "";
            try
            {
                if (hasID == "true")
                {
                    script += "function " + datagrid1.ClientID + "_querySt(ji) {" +
                            "hu = window.location.search.substring(1);" +
                            "gy = hu.split(\"&\");" +
                            "for (i = 0; i < gy.length; i++) {" +
                            "ft = gy[i].split(\"=\");" +
                            "if (ft[0] == ji) {" +
                            "return ft[1];" +
                            "}" +
                            "}" +
                            "}";
                }

                script += "$(document).ready(function () {" +
                "var lastsel; var lastselsub;" +
                "$('#" + datagrid1.ClientID + "').jqGrid({" +
                "datatype: " + datagrid1.ClientID + "_getProjects," +
                    //['ID', 'Title', 'Status', 'Start', 'Deadline', 'Category']
                "colNames: " + colNames + "," +
                #region colModel sample
                    //[{ name: 'ID', index: 'ID', width: 50, align: 'left', sorttype: \"int\", search: true, searchoptions: { sopt: ['eq', 'ne', 'cn', 'nc']} }," +
                    //"{ name: 'Title', index: 'Title', width: 100, align: 'left', search: false }," +
                    //"{ name: 'Status', index: 'Status', width: 50, align: 'left', search: false }," +
                    //"{ name: 'Start', index: 'Start', width: 100, align: 'left', search: false }," +
                    //"{ name: 'Deadline', index: 'Deadline', width: 100, align: 'left', search: false }," +
                    //"{ name: 'Category', index: 'Category', width: 50, align: 'left', search: false }" +
                    //"]
                #endregion
                "colModel: " + colModel + "," +
                    //25
                "rowNum: " + rowNum + "," +
                    //[5, 10, 25, 50, 100]
                "rowList: " + rowList + "," +
                    //ID
                "sortname: '" + sortname + "'," +
                "pager: '#" + datagrid1pager.ClientID + "'," +
                    //asc
                "sortorder: \"" + sortorder + "\"," +
                    //true
                "viewrecords: " + viewrecords + ",";
                //true
                if (multiselect == "true")
                {
                    script += "multiselect:true,";
                }
                // inline editing
                if (editurl != "" && editurl != null)
                {
                    // save the selected id into global varible (lastsel)
                    script += "onSelectRow: function(id){" +
                                                "if(id && id!==lastsel){" +
                                                    "jQuery('#" + datagrid1.ClientID + "').restoreRow(lastsel);" +
                                                    "lastsel=id;" +
                                                "}" +
                                            "},";
                    // set the editurl atttribute 
                    script += "editurl:'" + editurl + "',";
                }
                // Master Details
                if (onSelectRowUrl != "" && onSelectRowUrl != null)
                {
                    script += "onSelectRow: function(ids){" +
                                                "if(ids == null) { ids=0;}" +
                                                "jQuery('#" + list10_d.ClientID + "').jqGrid('setCaption','') .trigger('reloadGrid');" +
                                            "},";
                }

                //700
                script += "width: '" + width + "'," +
                    //300
                "height: '" + height + "'" +
                "});" +
                "$('#" + datagrid1.ClientID + "').jqGrid('navGrid', '#" + datagrid1pager.ClientID + "', { edit: false, add: false, del: false });";

                if (editurl != "" && editurl != null)
                {
                    script += "$('#" + datagrid1.ClientID + "').jqGrid('inlineNav','#" + datagrid1pager.ClientID + "',{edittext:'Edit',editParams:{key: true,aftersavefunc:UpdateToServer}});";
                }
                if (onSelectRowUrl != "" && onSelectRowUrl != null)
                {
                    script += "$('#" + list10_d.ClientID + "').jqGrid({ " +
                                    "height: " + dheight + ", " +
                                    "width: " + dwidth + ", " +
                                    "url:'" + onSelectRowUrl + "', " +
                                    "datatype: " + list10_d.ClientID + "_getProjects," +
                                    "colNames:" + dcolNames + ", " +
                                    "colModel:" + dcolModel + ", " +
                                     "rowNum:" + drowNum + ", " +
                                     "rowList:" + drowList + ", " +
                                     "pager: '#" + pager10_d.ClientID + "'," +
                                     "sortname: '" + dsortName + "', " +
                                     "viewrecords: true, " +
                                     "sortorder: '" + dsortOrder + "', " +
                                     "multiselect: " + dmultiSelect +", "+
                                     "onSelectRow: function(id){" +
                                                "if(id && id!==lastselsub){" +
                                                    "jQuery('#" + list10_d.ClientID + "').restoreRow(lastselsub);" +
                                                    "lastselsub=id;" +
                                                "}" +
                                            "}," +
                                     "editurl:'" + deditUrl + "'" +
                               "}).navGrid('#" + pager10_d.ClientID + "',{add:false,edit:false,del:false}); ";
                    script += "$('#" + list10_d.ClientID + "').jqGrid('inlineNav','#" + pager10_d.ClientID + "',{edittext:'Edit',editParams:{key: true,aftersavefunc:" + list10_d.ClientID + "_UpdateSubToServer}});";
                }

                script += "});";

                #region Get Data For Master Grid
                script += "function " + datagrid1.ClientID + "_getProjects(postdata) {" +
                "var poststr = JSON.stringify(postdata);" +
                "$.ajax({" +
                    //jqGrid.aspx/DataGrid1DataBind
                "url: '" + url + "',";

                if (hasID == "false")
                {
                    script += "data: \"{postdata:'\" + poststr + \"'}\"," +
                    "dataType: \"json\"," +
                    "type: \"POST\"," +
                    "contentType: \"application/json; charset=utf-8\"," +
                    "success: " + datagrid1.ClientID + "_successFunction" +
                    "});" +
                    "}" +
                    "function " + datagrid1.ClientID + "_successFunction(jsondata) {" +
                    "var thegrid = jQuery('#" + datagrid1.ClientID + "')[0];" +
                    "thegrid.addJSONData(JSON.parse(jsondata.d));" +
                    "}";
                }
                else if (hasID == "true")
                {
                    script += "data: \"{postdata:'\" + poststr + \"', " + idName + ":'\" + " + datagrid1.ClientID + "_querySt('" + idName + "') + \"'}\"," +
                    "dataType: \"json\"," +
                    "type: \"POST\"," +
                    "contentType: \"application/json; charset=utf-8\"," +
                    "success: " + datagrid1.ClientID + "_successFunction" +
                    "});" +
                    "}" +
                    "function " + datagrid1.ClientID + "_successFunction(jsondata) {" +
                    "var thegrid = jQuery('#" + datagrid1.ClientID + "')[0];" +
                    "thegrid.addJSONData(JSON.parse(jsondata.d));" +
                    "}";
                }
                #endregion

                #region Get Data For Details Grid
                // Get Data For Grid Details
                script += "function " + list10_d.ClientID + "_getProjects(postdata) {" +
                            "var poststr = JSON.stringify(postdata);" +
                            "var selectedID;　selectedID=$('#" + datagrid1.ClientID + "').getGridParam('selrow');" +
                            "var selectedRow; selectedRow= jQuery('#" + datagrid1.ClientID + "').jqGrid ('getRowData', selectedID);" +
                            "var rowValue=JSON.stringify(selectedRow);" +
                            "$.ajax({" +
                    //jqGrid.aspx/DataGrid1DataBind
                            "url: '" + onSelectRowUrl + "',";
                script += "data: \"{postdata:'\" + poststr + \"', selectedRow:'\" + rowValue + \"'}\",";
                script += "dataType: \"json\"," +
                "type: \"POST\"," +
                "contentType: \"application/json; charset=utf-8\"," +
                "success: " + list10_d.ClientID + "_successFunction" +
                "});" +
                "}" +
            "function " + list10_d.ClientID + "_successFunction(jsondata) {" +
                "var thegrid = jQuery('#" + list10_d.ClientID + "')[0];" +
                "thegrid.addJSONData(JSON.parse(jsondata.d));" +
                "}";
                #endregion

                #region SaveData For Master Grid
                script += "function UpdateToServer(rowid, response){" +
                          "var dataFromTheRow = jQuery('#" + datagrid1.ClientID + "').jqGrid ('getRowData', rowid);" +
                          "var rowValue=JSON.stringify(dataFromTheRow);" +
                          "$.ajax({" +
                          "url: '" + editurl + "'," +
                          "data: \"{dataFromTheRow:'\" + rowValue + \"'}\"," +
                          "dataType: \"json\"," +
                          "type: \"POST\"," +
                          "contentType: \"application/json; charset=utf-8\"," +
                          "success: function(){$('#" + datagrid1.ClientID + "').jqGrid('setCaption','').trigger('reloadGrid');}" +
                          "});" +
                    "}";
                #endregion

                #region Save Data For Details Grid
                script += "function " + list10_d.ClientID + "_UpdateSubToServer(rowid, response){" +
                            "var dataFromTheRow = jQuery('#" + list10_d.ClientID + "').jqGrid ('getRowData', rowid);" +
                            "var rowValue=JSON.stringify(dataFromTheRow); " +
                            "var masterID;　masterID=$('#" + datagrid1.ClientID + "').getGridParam('selrow');" +
                            "var masterRow; masterRow= jQuery('#" + datagrid1.ClientID + "').jqGrid ('getRowData', masterID);" +
                            "var masterRowValue=JSON.stringify(masterRow);" +
                            "$.ajax({" +
                            "url: '" + deditUrl + "'," +
                            "beforeSend: function (request){request.setRequestHeader('IsAsyc','1');},"+
                            "data: \"{dataFromTheRow:'\" + rowValue + \"', dataFromMasterRow:'\" + masterRowValue + \"'}\"," +
                            "dataType: \"json\"," +
                            "type: \"POST\"," +
                            "contentType: \"application/json; charset=utf-8\"," +
                            "success: function(){$('#" + list10_d.ClientID + "').jqGrid('setCaption','').trigger('reloadGrid');}" +
                            "});" +
                      "}";
                #endregion

                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(GetType(), datagrid1.ClientID, script, true);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
    }
}