using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.SMS;
using Sapp.Data;
using Sapp.Common;
using System.Data;
namespace sapp_sms
{
    public partial class bankreconciliationReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    #region Initial ComboBox
                    Odbc o = new Odbc(constr);
                    string sql = "SELECT   chart_master.chart_master_id AS ID, chart_master.chart_master_code AS Code,                   chart_master.chart_master_name AS Name, chart_master.chart_master_type_id, chart_types.chart_type_name  FROM      chart_master, chart_types  WHERE   chart_master.chart_master_type_id = chart_types.chart_type_id AND (chart_master.chart_master_bank_account = 1) ";
                    DataTable dt = o.ReturnTable(sql, "c");
                    foreach (DataRow dr in dt.Rows)
                    {
                        ListItem i = new ListItem(dr["Code"].ToString() + " | " + dr["Name"].ToString(), dr["ID"].ToString());
                        ComboBoxAccountCode0.Items.Add(i);
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

 
        public string SetUrl(string reportid, string args)
        {
            string url = "<script type='text/javascript'> window.open('reportviewer.aspx?reportid=" + reportid + "&args=" + args + "','_blank'); </script>";
            return url;
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            string args = ComboBoxAccountCode0.SelectedValue + "|" + StartDate_T.Text + "|" + EndDate_T.Text;
            Response.Write(SetUrl("bankreconciliation", args));
        }
    }
}