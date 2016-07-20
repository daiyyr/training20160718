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
using System.IO;
using System.Data;
using System.Collections;
namespace sapp_sms
{
    public partial class chartmasterImport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }



        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                FileUpload1.SaveAs(Server.MapPath("~") + "temp\\3.csv");
                DataTable dt = CsvDT.CsvToDataTable(Server.MapPath("~") + "temp\\", "3.csv");
                Odbc odbc = new Odbc(AdFunction.conn);
                DataTable ctypeDT = odbc.ReturnTable("select * from `chart_types`", "temp");

                foreach (DataRow dr in dt.Rows)
                {
                    string typeid = ReportDT.GetDataByColumn(ctypeDT, "chart_type_name", dr["Type"].ToString(), "chart_type_id");
                    ChartMaster cm = new ChartMaster(AdFunction.conn);
                    Hashtable ht = new System.Collections.Hashtable();
                    ht["chart_master_code"] = "'" + dr["Code"].ToString().Replace("'", "''") + "'";
                    ht["chart_master_name"] = "'" + dr["Name"].ToString().Replace("'", "''") + "'";
                    ht["chart_master_type_id"] = typeid;
                    cm.Add(ht);
                }

                FileInfo f = new FileInfo(Server.MapPath("~") + "temp\\3.csv");
                f.Delete();
                Response.Redirect("bodycorps.aspx", false);
            }
            catch (Exception ex)
            {

                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


    }
}
