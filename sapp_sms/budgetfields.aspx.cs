using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using System.Configuration;
using Sapp.JQuery;
using System.Collections;
using Sapp.SMS;
using Sapp.Data;

namespace sapp_sms
{
    public partial class budgetfields : System.Web.UI.Page,IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridBudgetFields };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                Session["bodycorpid"] = Request.Cookies["bodycorpid"].Value;
                #endregion
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        public void RaisePostBackEvent(string eventArgument)
        {
            try
            {
                string[] args = eventArgument.Split('|');
                if (args[0] == "ImageButtonDelete")
                {
                    ImageButtonDelete_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata, string bodycorpid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string sqlfromstr = "FROM (((`budget_fields` LEFT JOIN `bodycorps` ON `bodycorp_id`=`budget_field_bodycorp_id`) LEFT JOIN `chart_master` ON `budget_field_chart_id`=`chart_master_id`) "
                    + "LEFT JOIN `scale_types` ON `budget_field_scale_id`=`scale_type_id`)  WHERE `budget_field_bodycorp_id`=" + bodycorpid;
                string sqlselectstr = "SELECT `ID`,`Name`,`GlCode`, `Scale`, `Start`,`End`,`Order` "
                    + "FROM (SELECT `budget_field_id` AS `ID`,`budget_field_name` AS `Name`, `chart_master_code` AS `GlCode`, `scale_type_code` AS `Scale`, DATE_FORMAT(`budget_field_start`, '%d/%m/%Y') as `Start`, DATE_FORMAT(`budget_field_end`, '%d/%m/%Y') as `End`,  `budget_field_order` as `Order` ";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        [System.Web.Services.WebMethod]
        public static string SaveDataFromGrid(string rowValue)
        {
            string dataFromTheRow = rowValue;
            Object c = JSON.JsonDecode(dataFromTheRow);
            Hashtable hdata = (Hashtable)c;
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string line_id = hdata["ID"].ToString();
            string bodycorpid = HttpContext.Current.Session["bodycorpid"].ToString();
            BudgetField budgetfield = new BudgetField(constr);
            Hashtable items = new Hashtable();
            items.Add("budget_field_name", DBSafeUtils.StrToQuoteSQL(hdata["Name"].ToString()));
            items.Add("budget_field_bodycorp_id", DBSafeUtils.IntToSQL(bodycorpid));
            string chart_code = hdata["GlCode"].ToString();
            ChartMaster chart = new ChartMaster(constr);
            chart.LoadData(chart_code);
            items.Add("budget_field_chart_id", DBSafeUtils.IntToSQL(chart.ChartMasterId));
            string scale_code = hdata["Scale"].ToString();
            ScaleType scale = new ScaleType(constr);
            scale.LoadData(scale_code);
            items.Add("budget_field_scale_id", DBSafeUtils.IntToSQL(scale.ScaleTypeId));
            items.Add("budget_field_start", DBSafeUtils.DateToSQL(hdata["Start"].ToString()));
            items.Add("budget_field_end", DBSafeUtils.DateToSQL(hdata["End"].ToString()));
            items.Add("budget_field_order", DBSafeUtils.IntToSQL(0));
            if (line_id == "")
            {
                budgetfield.Add(items);
            }
            else
            {
                budgetfield.Update(items, Convert.ToInt32(line_id));
            }
            return "dd";
        }
        [System.Web.Services.WebMethod]
        public static string BindGlCodeSelector()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                ChartMaster cm = new ChartMaster(constr);
                List<ChartMaster> chartList = cm.GetChartMasterList();
                foreach (ChartMaster chart in chartList)
                {
                    html += "<option value='" + chart.ChartMasterId + "'>" + chart.ChartMasterCode + "</option>";
                }
                html += "</select>";
                return html;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        [System.Web.Services.WebMethod]
        public static string BindScaleSelector()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                ScaleType scaletypes = new ScaleType(constr);
                List<ScaleType> list = scaletypes.GetScaleTypeList();
                foreach (ScaleType scaletype in list)
                {
                    html += "<option value='" + scaletype.ScaleTypeId + "'>" + scaletype.ScaleTypeCode + "</option>";
                }
                html += "</select>";
                return html;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        #endregion
        #region WebControl Events
        private void ImageButtonDelete_Click(string[] args)
        {
            string budgetfield_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                budgetfield_id = args[1];
                BudgetField budgetfield = new BudgetField(constr);
                budgetfield.Delete(Convert.ToInt32(budgetfield_id));
                Response.BufferOutput = true;
                Response.Redirect("~/budgetfields.aspx",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ImageButtonChartBase_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                #region Create Budget Field Based on Chart of Account Levy Base
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = Request.Cookies["bodycorpid"].Value;
                ChartMaster cm = new ChartMaster(constr);
                List<ChartMaster> chartList = cm.GetChartMasterList();
                foreach (ChartMaster chart in chartList)
                {
                    if (chart.ChartMasterLevyBase)
                    {
                        Hashtable bf_items = new Hashtable();
                        bf_items.Add("budget_field_name", DBSafeUtils.StrToQuoteSQL(chart.ChartMasterName));
                        bf_items.Add("budget_field_chart_id", chart.ChartMasterId);
                        bf_items.Add("budget_field_bodycorp_id", bodycorp_id);
                        bf_items.Add("budget_field_scale_id", 1);
                        BudgetField budgetfield = new BudgetField(constr);
                        budgetfield.Add(bf_items);
                    }
                }
                
                #endregion
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        
    }
}