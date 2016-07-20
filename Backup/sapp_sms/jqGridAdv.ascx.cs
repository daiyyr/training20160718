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
    public partial class jqGridAdv : System.Web.UI.UserControl
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
        public string onSelectRow { get; set; }

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
                    "sortorder: \"" + sortorder + "\",";
                        //false
                    if (multiselect == "true") script += "multiselect: \"" + multiselect + "\",";
                        //true
                    script += "viewrecords: " + viewrecords + "," +
                        //700
                    "width: '" + width + "'," +
                        //300
                    "height: '" + height + "',";
                        //function(id){jqGridForms_onSelectRow(id);}
                    if (onSelectRow != "") script += "onSelectRow: " + onSelectRow + ",";
                    script += "});" +
                    "$('#" + datagrid1.ClientID + "').jqGrid('navGrid', '#" + datagrid1pager.ClientID + "', { edit: false, add: false, del: false });" +
                    "});" +
                    "function " + datagrid1.ClientID + "_getProjects(postdata) {" +
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