using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.SMS;
using System.Configuration;
using System.Collections;
using System.Data;
namespace sapp_sms
{
    public partial class chartmaster : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridChartMaster };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
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
                else if (args[0] == "ImageButtonDetails")
                {
                    ImageButtonDetails_Click(args);
                }
                else if (args[0] == "ImageButtonEdit")
                {
                    ImageButtonEdit_Click(args);
                }


            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridChartMasterDataBind(string postdata)
        {
            try
            {
                Odbc odbc = new Odbc(AdFunction.conn);
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                DataTable dt = odbc.ReturnTable("SELECT   chart_master.chart_master_id AS ID, chart_master.chart_master_code AS Code,                 chart_master.chart_master_name AS Name, chart_master_1.chart_master_name AS Parent,                 chart_types.chart_type_name AS Type FROM      chart_types, { oj chart_master LEFT OUTER JOIN                 chart_master chart_master_1 ON chart_master.chart_master_parent_id = chart_master_1.chart_master_id } WHERE   chart_master.chart_master_type_id = chart_types.chart_type_id", "temp");
                //string sqlfromstr = "FROM (`chart_master` LEFT JOIN `chart_types` ON `chart_master_type_id`=`chart_type_id`)";
                //string sqlselectstr = "SELECT `ID`, `Code`, `Name`, `Type` FROM (SELECT `chart_master_id` AS `ID`, `chart_master_code` AS `Code`, `chart_master_name` AS `Name`, `chart_type_name` AS `Type` ";
                string sqlfromstr = "FROM `" + dt.TableName + "`";
                string sqlselectstr = "SELECT `ID`, `Code`, `Name`, `Type`,`Parent` FROM (SELECT * ";

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

        #region WebControl Events

        private void ImageButtonDelete_Click(string[] args)
        {
            string chart_master_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                chart_master_id = args[1];
                ChartMaster chartmaster = new ChartMaster(constr);
                chartmaster.Delete(Convert.ToInt32(chart_master_id));
                Response.BufferOutput = true;
                Response.Redirect("~/chartmaster.aspx",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/chartmasteredit.aspx?mode=add", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            string chartmaster_id = "";
            try
            {
                chartmaster_id = args[1];

                Response.BufferOutput = true;
                Response.Redirect("~/chartmasteredit.aspx?mode=edit&chartmasterid=" + chartmaster_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        private void ImageButtonDetails_Click(string[] args)
        {
            string chartmaster_id = "";
            try
            {
                chartmaster_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/chartmasterdetails.aspx?chartmasterid=" + chartmaster_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/chartmasterImport.aspx", false);
        }
    }
}
