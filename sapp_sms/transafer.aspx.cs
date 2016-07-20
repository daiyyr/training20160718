using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.General;
using Sapp.Common;
using Sapp.SMS;

namespace sapp_sms
{
    public partial class transafer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    Odbc o = new Odbc(AdFunction.conn);
                    string sql = "SELECT   chart_master.chart_master_id AS ID, chart_master.chart_master_code AS Code,                   chart_master.chart_master_name AS Name, chart_master.chart_master_type_id, chart_types.chart_type_name  FROM      chart_master, chart_types  WHERE   chart_master.chart_master_type_id = chart_types.chart_type_id AND (chart_master.chart_master_bank_account = 1) ";
                    DataTable dt = o.ReturnTable(sql, "c");
                    foreach (DataRow dr in dt.Rows)
                    {
                        ListItem i = new ListItem(dr["Code"].ToString() + " | " + dr["Name"].ToString(), dr["ID"].ToString());
                        ComboBoxFromAccount.Items.Add(i);
                        ComboBoxToAccount.Items.Add(i);
                    }
                    TextBoxNum.Text = Journal.GetNextNumber();



                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebControl Events
        protected void ImageButtonSave_Click(object sender, ImageClickEventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
            {
                mydb.StartTransaction();
                Hashtable gl_items = new Hashtable();
                gl_items.Add("gl_transaction_type_id", 6);
                gl_items.Add("gl_transaction_ref", DBSafeUtils.StrToQuoteSQL(TextBoxNum.Text));
                gl_items.Add("gl_transaction_ref_type_id", 6);
                gl_items.Add("gl_transaction_chart_id", ComboBoxToAccount.SelectedValue);
                gl_items.Add("gl_transaction_bodycorp_id", Request.Cookies["bodycorpid"].Value);
                gl_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(TextBoxDescription.Text));
                gl_items.Add("gl_transaction_net", -decimal.Parse(TextBoxAmount.Text));
                gl_items.Add("gl_transaction_tax", 0);
                gl_items.Add("gl_transaction_gross", 0);
                gl_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                mydb.ExecuteInsert("gl_transactions", gl_items);
                gl_items["gl_transaction_chart_id"] = ComboBoxFromAccount.SelectedValue;
                gl_items["gl_transaction_net"] = decimal.Parse(TextBoxAmount.Text);
                mydb.ExecuteInsert("gl_transactions", gl_items);

                mydb.Commit();
                Response.Redirect(Request.Url.ToString(), false);
            }

            catch (Exception ex)
            {
                mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #endregion


    }
}