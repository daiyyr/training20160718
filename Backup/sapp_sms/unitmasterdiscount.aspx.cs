using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using Sapp.SMS;
using Sapp.Data;
using Sapp.Common;
using Sapp.JQuery;
using System.Data;

namespace sapp_sms
{
    public partial class unitmasterdiscount : System.Web.UI.Page
    {

        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        public void GetInvoice(string unitid)
        {
            InvoiceLB.Items.Clear();
            DataTable invDT = ReportDT.getTable(constr, "invoice_master");
            invDT = ReportDT.FilterDT(invDT, "invoice_master_unit_id = " + unitid);
            invDT = MergeColumn(invDT, "invoice_master_num", "invoice_master_gross");
            InvoiceLB.DataSource = invDT;
            InvoiceLB.DataTextField = "New";
            InvoiceLB.DataValueField = "invoice_master_id";
            InvoiceLB.DataBind();
        }
        public void GetReceipt(string invID)
        {
            #region Load System Chart ID

            Sapp.SMS.System s = new Sapp.SMS.System(AdFunction.conn);
            ChartMaster ch = new ChartMaster(AdFunction.conn);
            s.LoadData("GST Input");
            ch.LoadData(s.SystemValue);
            string InputGstID = ch.ChartMasterId.ToString();
            s.LoadData("GST Output");
            ch.LoadData(s.SystemValue);
            string OutputGstID = ch.ChartMasterId.ToString();
            s.LoadData("GENERALTAX");
            ch.LoadData(s.SystemValue);
            string gstid = ch.ChartMasterId.ToString();
            s.LoadData("GENERALDEBTOR");
            ch.LoadData(s.SystemValue);
            ch.LoadData(s.SystemValue);
            string proprietorID = ch.ChartMasterId.ToString();
            s.LoadData("GENERALCREDITOR");
            ch.LoadData(s.SystemValue);
            string creditorID = ch.ChartMasterId.ToString();
            s.LoadData("DISCOUNTCHARCODE");
            ch.LoadData(s.SystemValue);
            string discountID = ch.ChartMasterId.ToString();

            #endregion
            ReceiptLB.Items.Clear();
            Odbc odbc = new Odbc(constr);
            string sql = "SELECT   receipts.receipt_ref, invoice_gls.invoice_gl_invoice_id, receipts.receipt_id, receipts.receipt_gross,                  gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_id FROM      receipt_gls, receipts, invoice_gls, gl_transactions WHERE   receipt_gls.receipt_gl_receipt_id = receipts.receipt_id AND                  receipt_gls.receipt_gl_gl_id = invoice_gls.invoice_gl_gl_id AND                  invoice_gls.invoice_gl_gl_id = gl_transactions.gl_transaction_id AND                  (invoice_gls.invoice_gl_invoice_id NOT IN                     (SELECT   invoice_master.invoice_master_id                      FROM      gl_transactions gl_transactions_1, invoice_gls invoice_gls_1, receipt_gls receipt_gls_1,                                       invoice_master                      WHERE   gl_transactions_1.gl_transaction_id = invoice_gls_1.invoice_gl_gl_id AND                                       invoice_gls_1.invoice_gl_gl_id = receipt_gls_1.receipt_gl_gl_id AND                                       invoice_gls_1.invoice_gl_invoice_id = invoice_master.invoice_master_id AND                                       (gl_transactions_1.gl_transaction_type_id = 6))) AND (invoice_gls.invoice_gl_invoice_id = " + invID + ") ";
            DataTable rDT = odbc.ReturnTable(sql, "Temp");
            rDT = MergeColumn(rDT, "receipt_ref", "receipt_gross");
            ReceiptLB.DataSource = rDT;
            ReceiptLB.DataTextField = "New";
            ReceiptLB.DataValueField = "receipt_id";
            ReceiptLB.DataBind();
        }
        public void GetDiscount(string unitid)
        {
            #region Load System Chart ID

            Sapp.SMS.System s = new Sapp.SMS.System(AdFunction.conn);
            ChartMaster ch = new ChartMaster(AdFunction.conn);
            s.LoadData("GST Input");
            ch.LoadData(s.SystemValue);
            string InputGstID = ch.ChartMasterId.ToString();
            s.LoadData("GST Output");
            ch.LoadData(s.SystemValue);
            string OutputGstID = ch.ChartMasterId.ToString();
            s.LoadData("GENERALTAX");
            ch.LoadData(s.SystemValue);
            string gstid = ch.ChartMasterId.ToString();
            s.LoadData("GENERALDEBTOR");
            ch.LoadData(s.SystemValue);
            ch.LoadData(s.SystemValue);
            string proprietorID = ch.ChartMasterId.ToString();
            s.LoadData("GENERALCREDITOR");
            ch.LoadData(s.SystemValue);
            string creditorID = ch.ChartMasterId.ToString();
            s.LoadData("DISCOUNTCHARCODE");
            ch.LoadData(s.SystemValue);
            string discountID = ch.ChartMasterId.ToString();

            #endregion
            DiscountLB.Items.Clear();
            Odbc odbc = new Odbc(constr);
            string sql = "SELECT   gl_transaction_id, gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id,                  gl_transaction_unit_id, gl_transaction_description, gl_transaction_batch_id, gl_transaction_net, gl_transaction_tax,                  gl_transaction_gross, gl_transaction_date FROM      gl_transactions WHERE   (gl_transaction_id NOT IN                     (SELECT   receipt_gl_gl_id                      FROM      receipt_gls)) AND (gl_transaction_id NOT IN                     (SELECT   invoice_gl_gl_id                      FROM      invoice_gls)) AND (gl_transaction_type_id = 6) AND (gl_transaction_chart_id = " + discountID + ") and (gl_transaction_unit_id=" + unitid + ")";

            DataTable dt = odbc.ReturnTable(sql, "Temp");

            DataTable dataDT = dt.Clone();

            foreach (DataRow dr in dt.Rows)
            {
                string r = dr["gl_transaction_ref"].ToString();
                string sql2 = "SELECT   gl_transaction_id, gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id,                  gl_transaction_unit_id, gl_transaction_description, gl_transaction_batch_id, gl_transaction_net, gl_transaction_tax,                  gl_transaction_gross, gl_transaction_date FROM      gl_transactions WHERE   (gl_transaction_id NOT IN                     (SELECT   receipt_gl_gl_id                      FROM      receipt_gls)) AND (gl_transaction_id NOT IN                     (SELECT   invoice_gl_gl_id                      FROM      invoice_gls)) AND (gl_transaction_type_id = 6) AND (gl_transaction_chart_id = " + proprietorID + ") and (gl_transaction_unit_id=" + unitid + ") and gl_transaction_ref='" + r + "'";
                DataTable temp = odbc.ReturnTable(sql2, "Temp");
                if (temp.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in temp.Rows)
                    {
                        dataDT.ImportRow(dr2);
                    }
                }
            }

            dataDT = MergeColumn(dataDT, "gl_transaction_ref", "gl_transaction_net");
            DiscountLB.DataSource = dataDT;
            DiscountLB.DataTextField = "New";
            DiscountLB.DataValueField = "gl_transaction_id";
            DiscountLB.DataBind();
        }
        public DataTable MergeColumn(DataTable dt, string column, string coulmn2)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                DataTable RDt = dt.Copy();
                RDt.Columns.Add("New");
                foreach (DataRow dr in RDt.Rows)
                {
                    string v = dr[column].ToString();
                    string v2 = dr[coulmn2].ToString();
                    dr["New"] = v + " Gross:" + v2;
                }
                return RDt;
            }
            return null;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string unitid = Request.QueryString["unitid"];
                DataTable dt = ReportDT.getTable(constr, "unit_master");
                UnitNameL.Text = ReportDT.GetDataByColumn(dt, "unit_master_id", unitid, "unit_master_code");
                GetInvoice(unitid);
                GetDiscount(unitid);
            }
        }

        protected void InvoiceLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetReceipt(InvoiceLB.SelectedValue);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (DiscountLB.SelectedIndex > -1)
                if (InvoiceLB.SelectedIndex > -1)
                    if (ReceiptLB.SelectedIndex > -1)
                    {
                        string unitid = Request.QueryString["unitid"];
                        string gid = DiscountLB.SelectedValue;
                        string invid = InvoiceLB.SelectedValue;
                        string rid = ReceiptLB.SelectedValue;
                        Odbc odbc = new Odbc(constr);
                        Odbc o = new Odbc(constr);
                        string recGL3 = " INSERT INTO receipt_gls (receipt_gl_receipt_id, receipt_gl_gl_id, receipt_gl_paid) VALUES   (" + rid + "," + gid + ",0)";
                        o.ExecuteScalar(recGL3);
                        //2014-010-17 gl date = receipt date
                        Receipt r = new Receipt(constr);
                        r.LoadData(int.Parse(rid));
                        string glsql = "select * from gl_transactions where gl_transaction_id=" + gid;
                        string reference = o.ReturnTable(glsql, "disgl").Rows[0]["gl_transaction_ref"].ToString();
                        string updatesql = "update gl_transactions set gl_transaction_date=" + DBSafeUtils.DateTimeToSQL(r.ReceiptDate) + " where gl_transaction_ref=" + "'" + reference + "'";
                        o.ExecuteScalar(updatesql);
                        //end
                        DataTable invoice = ReportDT.getTable(constr, "invoice_master");
                        string invGL3 = "INSERT INTO invoice_gls (invoice_gl_invoice_id, invoice_gl_gl_id, invoice_gl_paid) VALUES   (" + invid + "," + gid + ",0)";
                        o.ExecuteScalar(invGL3);
                        GetDiscount(unitid);
                        InvoiceMaster i = new InvoiceMaster(constr);
                        i.InvoiceMasterId = int.Parse(invid);
                        i.LoadData(i.InvoiceMasterId);
                        i.UpdatePaid();

                    }
        }

        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/unitmaster.aspx?bodycorpid=" + Request.Cookies["bodycorpid"].Value + "&unitid=" + Request.QueryString["unitid"], false);
        }

    }
}
