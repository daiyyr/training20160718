﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Data.Odbc;
using Sapp.SMS;
using Sapp.Common;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.General;
using Sapp.SMS;
namespace sapp_sms
{
    public partial class cashpayment : System.Web.UI.Page
    {
        private const string temp_type = "tempbudgetmaster";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Sapp.SMS.System system = new Sapp.SMS.System(AdFunction.conn);

                TextBoxCashPaymentRef.Text = system.GetNextNumber("CASHPAYMENTPREFIX", "CASHPAYMENTPILOT", "CASHPAYMENTDIGIT", "gl_transactions", "gl_transaction_ref");
                Odbc o = new Odbc(AdFunction.conn);
                AjaxControlUtils.SetupComboBox(ComboBoxCashPaymentBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", AdFunction.conn, false);
                string sql = "SELECT `chart_master_id` AS `ID`, `chart_master_code` AS `Code`, chart_master_name as `Name` FROM `chart_master`   order by Code";
                DataTable dt = o.ReturnTable(sql, "c");
                foreach (DataRow dr in dt.Rows)
                {
                    ListItem i = new ListItem(dr["Code"].ToString() + " | " + dr["Name"].ToString(), dr["ID"].ToString());
                    ChartCashPaymentCombobox.Items.Add(i);
                }
                if (Request.Cookies["bodycorpid"] != null)
                {
                    ComboBoxCashPaymentBodycorp.SelectedValue = Request.Cookies["bodycorpid"].Value;
                    ComboBoxCashPaymentBodycorp.Enabled = false;
                }
            }
        }





        protected void ImageButtonSave_Click2(object sender, ImageClickEventArgs e)
        {
            try
            {
                string login = System.Web.HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new Sapp.General.User(Sapp.SMS.AdFunction.constr_general);
                user.LoadData(login);
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                decimal taxr = decimal.Parse(ReportDT.GetDataByColumn(ReportDT.getTable(constr, "system"), "system_code", "GST", "system_value"));
                Odbc o = new Odbc(constr);
                string bid = ComboBoxCashPaymentBodycorp.SelectedValue;
                string date = DBSafeUtils.DateTimeToSQL(TextBoxCashPaymentDate.Text);
                Bodycorp b = new Sapp.SMS.Bodycorp(constr);
                b.LoadData(int.Parse(bid));
                decimal amount = decimal.Parse(TextBoxCashPaymentGross.Text);
                if (GSTCK.Checked)
                {
                    decimal gst = amount / (1 + taxr) * taxr;
                    decimal net = amount - gst;
                    string chartgstCode = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "system"), "system_code", "GENERALTAX", "system_value");
                    string chartgstID = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "chart_master"), "chart_master_code", chartgstCode, "chart_master_id");
                    string reference = TextBoxCashPaymentRef.Text;
                    string accountid = b.BodycorpAccountId.ToString();
                    string sql = "INSERT INTO gl_transactions (gl_transaction_type_id, gl_transaction_ref,gl_transaction_ref_type_id, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross, gl_transaction_date,gl_transaction_createdate,gl_transaction_user_id,gl_transaction_description) VALUES       (8, '" + reference + "',8, " + accountid + ", " + bid + ", " + amount + ", 0, 0, " + date + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + user.UserId + ",'" + DescriptionT.Text + "')";
                    string gstsql = "INSERT INTO gl_transactions (gl_transaction_type_id, gl_transaction_ref,gl_transaction_ref_type_id, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross, gl_transaction_date,gl_transaction_createdate,gl_transaction_user_id,gl_transaction_description) VALUES        (8, '" + reference + "',8, " + chartgstID + ", " + bid + ", " + -gst + ", 0 , 0, " + date + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + user.UserId + ",'" + DescriptionT.Text + "')";
                    string charsql = "INSERT INTO gl_transactions (gl_transaction_type_id, gl_transaction_ref,gl_transaction_ref_type_id, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross, gl_transaction_date,gl_transaction_createdate,gl_transaction_user_id,gl_transaction_description) VALUES        (8, '" + reference + "', 8," + ChartCashPaymentCombobox.SelectedValue + ", " + bid + ", " + -net + ", 0, 0, " + date + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + user.UserId + ",'" + DescriptionT.Text + "')";
                    o.ReturnTable(sql, "amount");
                    o.ReturnTable(gstsql, "gst");
                    o.ReturnTable(charsql, "amount");
                }
                else
                {
                    //decimal gst = amount * taxr;
                    decimal net = amount;
                    string chartgstCode = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "system"), "system_code", "GENERALTAX", "system_value");
                    string chartgstID = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "chart_master"), "chart_master_code", chartgstCode, "chart_master_id");
                    string reference = TextBoxCashPaymentRef.Text;
                    string accountid = b.BodycorpAccountId.ToString();
                    string sql = "INSERT INTO gl_transactions (gl_transaction_type_id, gl_transaction_ref,gl_transaction_ref_type_id, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross, gl_transaction_date,gl_transaction_createdate,gl_transaction_user_id,gl_transaction_description) VALUES       (8, '" + reference + "',8, " + accountid + ", " + bid + ", " + amount + ", 0, 0, " + date + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + user.UserId + ",'" + DescriptionT.Text + "')";
                    //string gstsql = "INSERT INTO gl_transactions                          (gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross,                           gl_transaction_date) VALUES        (8, '" + reference + "', " + chartgstID + ", " + bid + ", " + gst + ", 0 , 0, " + date + ")";
                    string charsql = "INSERT INTO gl_transactions (gl_transaction_type_id, gl_transaction_ref,gl_transaction_ref_type_id, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross, gl_transaction_date,gl_transaction_createdate,gl_transaction_user_id,gl_transaction_description) VALUES (8, '" + reference + "',8, " + ChartCashPaymentCombobox.SelectedValue + ", " + bid + ", " + -net + ", 0, 0, " + date + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + user.UserId + ",'" + DescriptionT.Text + "')";
                    o.ReturnTable(sql, "amount");
                    //o.ReturnTable(gstsql, "gst");
                    o.ReturnTable(charsql, "amount");
                }
                Response.Redirect("goback.aspx", false);
                //this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.returnValue = 'refresh'; window.close();", true);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


    }

}