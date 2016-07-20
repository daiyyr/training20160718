using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Configuration;
using System.IO;
using Sapp.Common;
using Sapp.SMS;
using Sapp.JQuery;
using Sapp.Data;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Globalization;
using System.Threading;

using System.Data.OleDb;
using System.Data.Odbc;
using System.Net.Mail;
using System.Net;

namespace sapp_sms
{
    public partial class budgetupload : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
        }


        protected void Button1_Click1(object sender, System.EventArgs e)
        {
            try
            {
                FileUpload1.SaveAs(Server.MapPath("~") + "temp\\4.csv");
                FileInfo f = new FileInfo(Server.MapPath("~") + "temp\\4.csv");
                DataTable dt = CsvDT.CsvToDataTable(Server.MapPath("~") + "temp\\", "4.csv");

                // Add Start 22/04/2016
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = Request.QueryString["bodycorpid"];
                DateTime startdate = Convert.ToDateTime(Request.QueryString["startdate"]);
                Odbc odbc = new Odbc(constr);

                int row_idx = 1;  // Add 14/06/2016
                Hashtable allChartMasterID = new Hashtable();
                foreach (DataRow dr in dt.Rows)
                {
                    row_idx = row_idx + 1;
                    string chartID = dr["ID"].ToString();

                    // Add 16/06/2016
                    if ("".Equals(chartID))
                    {
                        throw new Exception(CsvDT.editErrorMessage(row_idx, "ID", "chart master ID is empty"));
                    }

                    // Add 16/06/2016
                    int chart_id;
                    if (int.TryParse(chartID, out chart_id) == false)
                    {
                        throw new Exception(CsvDT.editErrorMessage(row_idx, "ID", "chart master ID [" + chartID + "] should be intger."));
                    }

                    // Add 16/06/2016
                    ChartMaster cm = new ChartMaster(constr);
                    cm.SetOdbc(odbc);
                    cm.LoadData(chart_id);
                    if (cm.ChartMasterCode == null)
                    {
                        throw new Exception(CsvDT.editErrorMessage(row_idx, "ID", "chart master ID [" + chartID + "] is invalid"));
                    }

                    // Check duplication of chart master ID
                    if (allChartMasterID.ContainsKey(chartID))
                    {
                        //throw new Exception("There are duplicated chart master ID [" + chartID + "], import cann't be finished.");
                        throw new Exception(CsvDT.editErrorMessage(row_idx, "ID", "chart master ID [" + chartID + "] is duplicated"));
                    }
                    else
                    {
                        allChartMasterID.Add(chartID, chartID);
                    }

                    // Check used data
                    string sql = "SELECT COUNT(*) AS summary FROM budget_master "
                               + " WHERE budget_master_bodycorp_id = " + bodycorp_id 
                               + "   AND budget_master_chart_id = " +  chartID
                               + "   AND budget_master_used = 1 "
                               + "   AND budget_master_month BETWEEN " + DBSafeUtils.DateTimeToSQL(startdate) + " AND " + DBSafeUtils.DateTimeToSQL(startdate.AddMonths(12));
                    OdbcDataReader reader = odbc.Reader(sql);
                    while (reader.Read())
                    {
                        if (reader["summary"].ToString().Equals("0") == false)
                        {
                            //throw new Exception("The budget of chart master ID [" + chartID + "] has been used, import cann't be finished.");
                            throw new Exception(CsvDT.editErrorMessage(row_idx, "ID", "the budget of chart master ID [" + chartID + "]  has been used"));
                        }
                    }
                }
                // Add End 22/04/2016

                dt.TableName = "tempbudgets";
                Session["imbudgetmaster"] = dt;
                f.Delete();
                Response.Redirect(Server.UrlDecode(Request.QueryString["burl"]), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        /// <summary>
        /// Download budget csv file template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ImageButtonTemplate_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("Total");
                dt.Columns.Add("Field");
                dt.Columns.Add("Scale");
                dt.Columns.Add("M1");
                dt.Columns.Add("M2");
                dt.Columns.Add("M3");
                dt.Columns.Add("M4");
                dt.Columns.Add("M5");
                dt.Columns.Add("M6");
                dt.Columns.Add("M7");
                dt.Columns.Add("M8");
                dt.Columns.Add("M9");
                dt.Columns.Add("M10");
                dt.Columns.Add("M11");
                dt.Columns.Add("M12");

                CSVIO csvio = new CSVIO();

                string filefolder = Server.MapPath("~/temp");
                string filename = Guid.NewGuid().ToString();
                CsvDT.DataTableToCsv(dt, filefolder + "\\" + filename + ".csv");

                FileInfo fo = new FileInfo(filefolder + "\\" + filename + ".csv");
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fo.Name);
                HttpContext.Current.Response.AddHeader("Content-Length", fo.Length.ToString());
                HttpContext.Current.Response.ContentType = "text/plain";
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.TransmitFile(fo.FullName);
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

    }
}