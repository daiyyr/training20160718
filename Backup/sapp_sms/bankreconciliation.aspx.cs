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
    public partial class bankreconciliation : System.Web.UI.Page
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
                        ComboBoxAccountCode.Items.Add(i);
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region Validations
        protected void CustomValidatorAccountCode_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxAccountCode.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
        #region WebControl Events
        protected void ImageButtonReconcilication_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string account_id = "";
                string cuttoffdate = "";
                #region Retireve Values
                account_id = ComboBoxAccountCode.SelectedValue;
                cuttoffdate = TextBoxCutOffDate.Text;
                #endregion
                Response.BufferOutput = true;
                Session["cub"] = ClosingBalanceT.Text;
                Session["cb"] = "0";
                Response.Redirect("~/bankreconciliation2.aspx?accountid=" + account_id + "&cutoffdate=" + cuttoffdate + "&enddate=" + cuttoffdate, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("bankreconciliation31.aspx", false);
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("bankreconciliationReport.aspx", false);
        }
    }
}