using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Collections;
using Sapp.Common;
using Sapp.Data;
using Sapp.SMS;
using Sapp.JQuery;
using SystemAlias = System;
using System.Text;
using System.Data.Odbc;
using Microsoft.Reporting.WebForms;
namespace sapp_sms
{
    public partial class activitybc : System.Web.UI.Page
    {
        private DataTable activityDT = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridActivity };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                string bid = "";
                string start = "";
                string end = "";
                if (Request.QueryString["id"] != null)
                {
                    Session["activity_id"] = Request.QueryString["id"].ToString();
                    bid = Request.QueryString["id"].ToString();
                }
                if (Request.QueryString["start"] != null)
                {
                    Session["activity_start"] = Request.QueryString["start"].ToString();
                    start = Request.QueryString["start"].ToString();
                }
                if (Request.QueryString["end"] != null)
                {
                    Session["activity_end"] = Request.QueryString["end"].ToString();
                    end = Request.QueryString["end"].ToString();
                }
                DataTable dt = GetDTData(bid, start, end);
                Session.Add("TempAct", dt);
                #endregion
                activityDT.Columns.Add("chart_code");
                activityDT.Columns.Add("chart_name");
                activityDT.Columns.Add("date");
                activityDT.Columns.Add("reference");
                activityDT.Columns.Add("description");
                activityDT.Columns.Add("debit");
                activityDT.Columns.Add("credit");
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        public DataTable GetDTData(string bodycorp_id, string start, string end)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                ActivityReport activityReport = new ActivityReport(constr, HttpContext.Current.Server.MapPath("templates/activityreport.rdlc"), new ReportViewer());
                activityReport.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start), Convert.ToDateTime(end));
                return activityReport.activityDT;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string JsonForJqgrid(DataTable dt, int pageSize, int totalRecords, int page)
        {
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"total\":" + totalPages + ",\"page\":" + page + ",\"records\":" + (totalRecords));
            bool First = true;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (First)
                {
                    jsonBuilder.Append(",\"rows\":[");
                    First = false;
                }
                jsonBuilder.Append("{\"i\":" + (i) + ",\"cell\":[");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("]},");
            }
            if (!First)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("]");
            }
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }
        public static string DataTableToJson(DataTable dt, string JsonName)
        {
            try
            {
                if (dt == null) { return "DataTable Is Null ,So I Can't Do It To Json!"; }
                string josn = "\"" + JsonName + "\":["; string temp = ""; for (int j = 0; j < dt.Rows.Count; j++)
                {
                    temp = temp + "{"; for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        temp += "\"" + dt.Columns[i].ColumnName.ToLower() + "\":\"" + dt.Rows[j][i] + "\"";
                        if (i != dt.Columns.Count - 1) { temp = temp + ","; }
                    } if (j == dt.Rows.Count - 1) { temp = temp + "}"; } else { temp = temp + "},"; }
                } josn = josn + temp + "]"; return josn;
            }
            catch (Exception ex)
            {
                return "Codeing is Error----" + ex.ToString();
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string jqGridActivityDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            Odbc o = new Odbc(constr);
            string bid = HttpContext.Current.Session["activity_id"].ToString();
            string start = HttpContext.Current.Session["activity_start"].ToString();
            string end = HttpContext.Current.Session["activity_end"].ToString();
            string sql = "SELECT  `gl_transaction_id`, `gl_transaction_batch_id`, `chart_master_code`, `chart_master_name`, DATE_FORMAT(`gl_transaction_date`, '%d %b %y') AS `date`, `gl_transaction_description`, `gl_transaction_batch_id`, `gl_transaction_ref`, SUM(`gl_transaction_net`) AS `net` FROM `gl_transactions` LEFT JOIN `chart_master` ON `gl_transaction_chart_id`=`chart_master_id` WHERE  `gl_transaction_bodycorp_id`=" + bid + " AND (`gl_transaction_date` BETWEEN " + DBSafeUtils.DateTimeToSQL(start) + " AND " + DBSafeUtils.DateTimeToSQL(end) + ") AND (`gl_transaction_type_id`!=3) AND (`gl_transaction_type_id`!=4) GROUP BY `chart_master_code` Having Count(*) > 1";
            DataTable dt = o.ReturnTable(sql, "temp");
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `chart_master_code`, `chart_master_name` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }
        [System.Web.Services.WebMethod]
        public static string FillSubGrid(string postdata, string masterRow)
        {
            try
            {
                Object c = JSON.JsonDecode(masterRow);
                Hashtable hdata = (Hashtable)c;
                string code = hdata["chart_code"].ToString();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                DataTable dt = (DataTable)HttpContext.Current.Session["TempAct"];
                dt.TableName = "Temp";
                dt = ReportDT.FilterDT(dt, "chart_code =" + code);
                string sqlfromstr = "FROM `" + dt.TableName + "`";
                string sqlselectstr = "SELECT `chart_code`, `date`, `reference`, `description`,`debit`, `credit` FROM (SELECT *";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        #endregion
    }
}