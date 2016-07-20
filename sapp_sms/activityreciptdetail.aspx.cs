using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using Sapp.Common;
using System.Configuration;
using Sapp.Data;
using Sapp.SMS;
using Sapp.JQuery;
namespace sapp_sms
{
    public partial class activityreciptdetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string id = HttpContext.Current.Request.QueryString["id"];
                string type = HttpContext.Current.Request.QueryString["type"];
                string op = "";
                if (HttpContext.Current.Request.QueryString["op"] != null)
                    op = HttpContext.Current.Request.QueryString["op"];
                if (!op.Equals("reverse"))
                {
                    GetData(type, id);
                }

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        public void GetData(string type, string id)
        {

            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            Odbc o = new Odbc(constr);
            TypeL.Text = type;
            DataTable dt = new DataTable("DT");
            dt.Columns.Add("Num");
            dt.Columns.Add("Rec");
            dt.Columns.Add("Rev");
            dt.Columns.Add("Code");
            dt.Columns.Add("Name");
            dt.Columns.Add("Type");
            dt.Columns.Add("Date");
            dt.Columns.Add("Ref");
            dt.Columns.Add("BID");
            dt.Columns.Add("Description");
            dt.Columns.Add("Amount");

            string sql = "";
            if (type.Equals("Receipt"))
            {

                sql = "SELECT  receipt_ref AS Num,  gl_transactions.gl_transaction_rec AS Rec, gl_transactions.gl_transaction_rev AS Rev,                  gl_transactions.gl_transaction_ref AS Ref, chart_master.chart_master_code AS Code,                  chart_master.chart_master_name AS Name, receipt_notes AS Description,                  gl_transactions.gl_transaction_batch_id AS BID, gl_transactions.gl_transaction_net, receipts.receipt_id,                  gl_transactions.gl_transaction_date AS `Date`, chart_types.chart_type_name AS Type,                  invoice_gls.invoice_gl_invoice_id, bodycorps.bodycorp_name FROM      gl_transactions, receipt_gls, receipts, debtor_master, chart_master, chart_types, invoice_gls, bodycorps WHERE   gl_transactions.gl_transaction_id = receipt_gls.receipt_gl_gl_id AND                  receipt_gls.receipt_gl_receipt_id = receipts.receipt_id AND                  receipts.receipt_debtor_id = debtor_master.debtor_master_id AND                  gl_transactions.gl_transaction_chart_id = chart_master.chart_master_id AND                  chart_master.chart_master_type_id = chart_types.chart_type_id AND                  receipt_gls.receipt_gl_gl_id = invoice_gls.invoice_gl_gl_id AND                  gl_transactions.gl_transaction_bodycorp_id = bodycorps.bodycorp_id   and invoice_gl_invoice_id=" + id;
            }
            if (type.Equals("Refund"))
            {

                sql = "SELECT  receipt_ref AS Num,  gl_transactions.gl_transaction_rec AS Rec, gl_transactions.gl_transaction_rev AS Rev,                  gl_transactions.gl_transaction_ref AS Ref, chart_master.chart_master_code AS Code,                  chart_master.chart_master_name AS Name, receipt_notes AS Description,                  gl_transactions.gl_transaction_batch_id AS BID,gl_transactions.gl_transaction_net, receipts.receipt_id,                  gl_transactions.gl_transaction_date AS `Date`, chart_types.chart_type_name AS Type,                  invoice_gls.invoice_gl_invoice_id, bodycorps.bodycorp_name FROM      gl_transactions, receipt_gls, receipts, debtor_master, chart_master, chart_types, invoice_gls, bodycorps WHERE   gl_transactions.gl_transaction_id = receipt_gls.receipt_gl_gl_id AND                  receipt_gls.receipt_gl_receipt_id = receipts.receipt_id AND                  receipts.receipt_debtor_id = debtor_master.debtor_master_id AND                  gl_transactions.gl_transaction_chart_id = chart_master.chart_master_id AND                  chart_master.chart_master_type_id = chart_types.chart_type_id AND                  receipt_gls.receipt_gl_gl_id = invoice_gls.invoice_gl_gl_id AND                  gl_transactions.gl_transaction_bodycorp_id = bodycorps.bodycorp_id   and invoice_gl_invoice_id=" + id;
            }
            if (type.Equals("Payment"))
            {

                sql = "SELECT cpayment_reference as Num,  gl_transactions.gl_transaction_rec AS Rec, gl_transactions.gl_transaction_rev AS Rev,                  gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_ref AS Ref, gl_transactions.gl_transaction_chart_id,                  chart_master.chart_master_code AS Code, chart_master.chart_master_name AS Name,                  gl_transactions.gl_transaction_description AS Description, gl_transactions.gl_transaction_batch_id AS BID,                  gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_date AS `Date`, bodycorps.bodycorp_code,                  bodycorps.bodycorp_name, chart_types.chart_type_name AS Type, cpayments.cpayment_id,                  cinvoice_gls.cinvoice_gl_cinvoice_id FROM      cpayments, cpayment_gls, gl_transactions, chart_master, chart_types, cinvoice_gls, bodycorps WHERE   cpayments.cpayment_id = cpayment_gls.cpayment_gl_cpayment_id AND                  cpayment_gls.cpayment_gl_gl_id = gl_transactions.gl_transaction_id AND                  gl_transactions.gl_transaction_chart_id = chart_master.chart_master_id AND                  chart_master.chart_master_type_id = chart_types.chart_type_id AND                  cpayment_gls.cpayment_gl_gl_id = cinvoice_gls.cinvoice_gl_gl_id AND                  gl_transactions.gl_transaction_bodycorp_id = bodycorps.bodycorp_id and cinvoice_gl_cinvoice_id= " + id;
            }

            DataTable invoiceDT = o.ReturnTable(sql, "NewTable");

            if (invoiceDT.Rows.Count > 0)
                bodycorpL.Text = invoiceDT.Rows[0]["bodycorp_name"].ToString();

            foreach (DataRow dr in invoiceDT.Rows)
            {
                DataRow newRow = dt.NewRow();
                newRow["Num"] = dr["Num"];
                newRow["Rec"] = dr["Rec"];
                newRow["Rev"] = dr["Rev"];
                newRow["Code"] = dr["Code"];
                newRow["Name"] = dr["Name"];
                newRow["Type"] = dr["Type"];
                newRow["Date"] = DateTime.Parse(dr["Date"].ToString()).ToString("dd/MM/yyyy");
                newRow["Ref"] = dr["Ref"];
                newRow["BID"] = dr["BID"];
                newRow["Description"] = dr["Description"];
                decimal n = decimal.Parse(dr["gl_transaction_net"].ToString());
                if (type.Equals("Refund"))
                    n = -n;
                if (n > 0)
                {
                    newRow["Amount"] = n;
                    dt.Rows.Add(newRow);
                }

            }
            dt.DefaultView.Sort = "Code";
            Session.Add("TempDT", dt.DefaultView.ToTable());
        }
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string sqlselectstr = "select Num,Code,Name,Date,Ref,Description,Amount FROM (SELECT *";
                DataTable dt = (DataTable)HttpContext.Current.Session["TempDT"];
                string sqlfromstr = "FROM `" + dt.TableName + "`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                Decimal c = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    string c1 = dr["Amount"].ToString();
                    if (!c1.Equals(""))
                        c += Decimal.Parse(c1);
                }
                Hashtable userdata = new Hashtable();
                userdata.Add("Description", "Total:");
                userdata.Add("Amount", c);
                jqgridObj.SetUserData(userdata);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
    }
}