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
    public partial class activityunitdetail : System.Web.UI.Page
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
                else
                {
                    Reverse(type, id);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        public void Reverse(string type, string id)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            Odbc o = new Odbc(constr);
            Odbc mydb = new Odbc(constr);

            mydb.StartTransaction();
            string sql = "";
            try
            {
                #region Journal
                if (type.Equals("Journal"))
                {
                    string temp = "SELECT        gl_transaction_id, gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_unit_id,                           gl_transaction_description, gl_transaction_batch_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross, gl_transaction_date FROM            gl_transactions WHERE        (gl_transaction_id = " + id + ") ";
                    string refe = o.ReturnTable(temp, "gl").Rows[0]["gl_transaction_ref"].ToString();
                    sql = "SELECT *  FROM  gl_transactions  WHERE   (gl_transaction_ref = '" + refe + "') ";
                    DataTable dt = o.ReturnTable(sql, "NewTable");
                    decimal check = ReportDT.SumTotal(dt, "gl_transaction_rec");
                    if (check != 0)
                        throw new Exception("Journal Has Been Reconciled");
                    check += ReportDT.SumTotal(dt, "gl_transaction_rev");
                    if (check != 0)
                        throw new Exception("Journal Has Been Reversed");

                    foreach (DataRow dr in dt.Rows)
                    {

                        Bodycorp b = new Bodycorp(AdFunction.conn);
                        b.LoadData(int.Parse(dr["gl_transaction_bodycorp_id"].ToString()));
                        if (!b.CheckCloseOff(DateTime.Parse(dr["gl_transaction_date"].ToString())))
                        {
                            throw new Exception("Invalid action! Journal before finance close date!");
                        }
                        if (!dr["gl_transaction_rec"].ToString().ToString().Equals("1") && !dr["gl_transaction_rev"].ToString().Equals("1"))
                        {
                            //修改原数据REF
                            //string sql2 = "update gl_transactions set gl_transaction_ref = " + DBSafeUtils.StrToQuoteSQL(dr["gl_transaction_ref"].ToString() + "-REV") + ", gl_transaction_rev=1 where gl_transaction_id=" + dr["gl_transaction_id"].ToString();
                            string sql2 = "update gl_transactions set gl_transaction_rev=1 where gl_transaction_id=" + dr["gl_transaction_id"].ToString();

                            mydb.ReturnTable(sql2, "update");
                            Hashtable items = new Hashtable();
                            items.Add("gl_transaction_ref", DBSafeUtils.StrToQuoteSQL(dr["gl_transaction_ref"].ToString()));
                            items.Add("gl_transaction_chart_id", dr["gl_transaction_chart_id"].ToString());
                            items.Add("gl_transaction_bodycorp_id", dr["gl_transaction_bodycorp_id"].ToString());
                            items.Add("gl_transaction_type_id", "6");
                            string unitid = dr["gl_transaction_unit_id"].ToString();
                            if (!unitid.Equals(""))
                                items.Add("gl_transaction_unit_id", unitid);
                            if (Request.QueryString["date"] == null)
                                items.Add("gl_transaction_date", DBSafeUtils.DateTimeToSQL(dr["gl_transaction_date"].ToString()));
                            else
                                items.Add("gl_transaction_date", DBSafeUtils.DateTimeToSQL(Request.QueryString["date"].ToString()));
                            items.Add("gl_transaction_description", "'Reverse " + dr["gl_transaction_description"].ToString() + "'");
                            items.Add("gl_transaction_net", -decimal.Parse(dr["gl_transaction_net"].ToString()));
                            items.Add("gl_transaction_tax", -decimal.Parse(dr["gl_transaction_tax"].ToString()));
                            items.Add("gl_transaction_gross", -decimal.Parse(dr["gl_transaction_gross"].ToString()));
                            items.Add("gl_transaction_ref_type_id", "6");
                            items.Add("gl_transaction_rev", "1");
                            mydb.ExecuteInsert("gl_transactions", items);//insert each row
                            items.Clear();
                            //DataTable receiptGLDT = ReportDT.getTable(constr, "receipt_gls");
                            DataTable invGLDT = ReportDT.getTable(constr, "invoice_gls");
                            int invid = 0;
                            int.TryParse(ReportDT.GetDataByColumn(invGLDT, "invoice_gl_gl_id", id, "invoice_gl_invoice_id"), out invid);
                            //string recid = ReportDT.GetDataByColumn(receiptGLDT, "receipt_gl_gl_id", id, "receipt_gl_id");
                            string delinvsql = "delete from invoice_gls where invoice_gl_gl_id=" + id;
                            string delrecsql = "delete from receipt_gls where receipt_gl_gl_id=" + id;
                            mydb.ReturnTable(delinvsql, "update");
                            mydb.ReturnTable(delrecsql, "update");
                            InvoiceMaster i = new InvoiceMaster(constr);
                            i.SetOdbc(mydb);
                            if (invid != 0)
                                i.LoadData(invid);
                            if (invid != 0)
                            {
                                i.InvoiceMasterId = invid;
                                i.UpdatePaid();
                            }

                        }
                    }
                }
                #endregion
                #region Invoice
                if (type.Equals("Invoice"))
                {

                    sql = "SELECT   gl_transactions.*, invoice_master.* FROM      gl_transactions, invoice_gls, invoice_master WHERE   gl_transactions.gl_transaction_id = invoice_gls.invoice_gl_gl_id AND                  invoice_gls.invoice_gl_invoice_id = invoice_master.invoice_master_id  and invoice_master_id= " + id;
                    DataTable dt = o.ReturnTable(sql, "NewTable");
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!dr["gl_transaction_ref"].ToString().ToString().Contains("-REV"))
                        {

                            //修改原数据REF
                            string sql2 = "update gl_transactions set gl_transaction_ref = " + DBSafeUtils.StrToQuoteSQL(dr["gl_transaction_ref"].ToString() + "-REV") + " where gl_transaction_id=" + dr["gl_transaction_id"].ToString();
                            mydb.ReturnTable(sql2, "update");
                            Hashtable items = new Hashtable();
                            items.Add("gl_transaction_ref", DBSafeUtils.StrToQuoteSQL(dr["gl_transaction_ref"].ToString() + "-REV"));
                            items.Add("gl_transaction_chart_id", dr["gl_transaction_chart_id"].ToString());
                            items.Add("gl_transaction_bodycorp_id", dr["gl_transaction_bodycorp_id"].ToString());
                            items.Add("gl_transaction_type_id", dr["gl_transaction_type_id"].ToString());
                            string unitid = dr["gl_transaction_unit_id"].ToString();
                            if (!unitid.Equals(""))
                                items.Add("gl_transaction_unit_id", unitid);
                            items.Add("gl_transaction_date", DBSafeUtils.DateTimeToSQL(dr["gl_transaction_date"].ToString()));
                            items.Add("gl_transaction_description", "'Reverse " + dr["gl_transaction_description"].ToString() + "'");
                            items.Add("gl_transaction_net", -decimal.Parse(dr["gl_transaction_net"].ToString()));
                            items.Add("gl_transaction_tax", -decimal.Parse(dr["gl_transaction_tax"].ToString()));
                            items.Add("gl_transaction_gross", -decimal.Parse(dr["gl_transaction_gross"].ToString()));
                            items.Add("gl_transaction_ref_type_id", dr["gl_transaction_ref_type_id"].ToString());
                            mydb.ExecuteInsert("gl_transactions", items);//insert each row

                            string invglsql = "Insert Into invoice_gls values(NULL," + id + "," + AdFunction.GetInsertID(mydb) + ",0)";
                            mydb.ExecuteScalar(invglsql);
                            items.Clear();
                        }
                    }
                    InvoiceMaster i = new InvoiceMaster(constr);
                    i.LoadData(int.Parse(id));
                    i.UpdatePaid();
                    i.UpdateTotal();
                    string num = i.InvoiceMasterNum;
                    num = num + "-REV";
                    string invsql = "update invoice_master set `invoice_master_num`='" + num + "' where invoice_master_id=" + id;
                    mydb.ExecuteScalar(invsql);
                }
                #endregion
                #region CInvoice
                if (type.Equals("Cinvoice"))
                {

                    sql = "SELECT   * FROM      cinvoice_gls, cinvoices, gl_transactions WHERE   cinvoice_gls.cinvoice_gl_cinvoice_id = cinvoices.cinvoice_id AND                  cinvoice_gls.cinvoice_gl_gl_id = gl_transactions.gl_transaction_id   and cinvoice_id= " + id;
                    DataTable dt = o.ReturnTable(sql, "NewTable");
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!dr["gl_transaction_ref"].ToString().ToString().Contains("-REV"))
                        {

                            //修改原数据REF
                            string sql2 = "update gl_transactions set gl_transaction_ref = " + DBSafeUtils.StrToQuoteSQL(dr["gl_transaction_ref"].ToString() + "-REV") + " where gl_transaction_id=" + dr["gl_transaction_id"].ToString();
                            mydb.ReturnTable(sql2, "update");
                            Hashtable items = new Hashtable();
                            items.Add("gl_transaction_ref", DBSafeUtils.StrToQuoteSQL(dr["gl_transaction_ref"].ToString() + "-REV"));
                            items.Add("gl_transaction_chart_id", dr["gl_transaction_chart_id"].ToString());
                            items.Add("gl_transaction_bodycorp_id", dr["gl_transaction_bodycorp_id"].ToString());
                            items.Add("gl_transaction_type_id", dr["gl_transaction_type_id"].ToString());
                            string unitid = dr["gl_transaction_unit_id"].ToString();
                            if (!unitid.Equals(""))
                                items.Add("gl_transaction_unit_id", unitid);
                            items.Add("gl_transaction_date", DBSafeUtils.DateTimeToSQL(dr["gl_transaction_date"].ToString()));
                            items.Add("gl_transaction_description", "'Reverse " + dr["gl_transaction_description"].ToString() + "'");
                            items.Add("gl_transaction_net", -decimal.Parse(dr["gl_transaction_net"].ToString()));
                            items.Add("gl_transaction_tax", -decimal.Parse(dr["gl_transaction_tax"].ToString()));
                            items.Add("gl_transaction_gross", -decimal.Parse(dr["gl_transaction_gross"].ToString()));
                            items.Add("gl_transaction_ref_type_id", dr["gl_transaction_ref_type_id"].ToString());
                            mydb.ExecuteInsert("gl_transactions", items);//insert each row

                            string invglsql = "Insert Into cinvoice_gls values(NULL," + id + "," + AdFunction.GetInsertID(mydb) + ",0)";
                            mydb.ExecuteScalar(invglsql);
                            items.Clear();
                        }
                    }
                    Cinvoice i = new Cinvoice(constr);
                    i.LoadData(int.Parse(id));
                    //i.UpdatePaid();

                    string num = i.CinvoiceNum;
                    num = num + "-REV";
                    string invsql = "update cinvoices set `cinvoice_net`=0, `cinvoice_tax`=0, `cinvoice_gross`=0, `cinvoice_paid`=0 , `cinvoice_num`='" + num + "' where cinvoice_id=" + id;
                    mydb.ExecuteScalar(invsql);
                }
                #endregion
                #region Receipt
                if (type.Equals("Receipt"))
                {
                    sql = "SELECT  * FROM            gl_transactions, receipt_gls, receipts, debtor_master, chart_master, bodycorps, chart_types WHERE        gl_transactions.gl_transaction_id = receipt_gls.receipt_gl_gl_id AND receipt_gls.receipt_gl_receipt_id = receipts.receipt_id AND                           receipts.receipt_debtor_id = debtor_master.debtor_master_id AND gl_transactions.gl_transaction_chart_id = chart_master.chart_master_id AND                           gl_transactions.gl_transaction_bodycorp_id = bodycorps.bodycorp_id AND chart_master.chart_master_type_id = chart_types.chart_type_id AND                           (gl_transactions.gl_transaction_type_id <> 3) AND (gl_transactions.gl_transaction_type_id <> 4)   AND    (gl_transactions.gl_transaction_type_id <> 6) AND                         (receipts.receipt_id = " + id + ") ";
                    DataTable dt = o.ReturnTable(sql, "NewTable");
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!dr["gl_transaction_ref"].ToString().ToString().Contains("-REV"))
                        {

                            //修改原数据REF
                            string sql2 = "update gl_transactions set gl_transaction_ref = " + DBSafeUtils.StrToQuoteSQL(dr["gl_transaction_ref"].ToString() + "-REV") + " where gl_transaction_id=" + dr["gl_transaction_id"].ToString();
                            mydb.ReturnTable(sql2, "update");
                            Hashtable items = new Hashtable();
                            items.Add("gl_transaction_ref", DBSafeUtils.StrToQuoteSQL(dr["gl_transaction_ref"].ToString() + "-REV"));
                            items.Add("gl_transaction_chart_id", dr["gl_transaction_chart_id"].ToString());
                            items.Add("gl_transaction_bodycorp_id", dr["gl_transaction_bodycorp_id"].ToString());
                            items.Add("gl_transaction_type_id", dr["gl_transaction_type_id"].ToString());
                            string unitid = dr["gl_transaction_unit_id"].ToString();
                            if (!unitid.Equals(""))
                                items.Add("gl_transaction_unit_id", unitid);
                            items.Add("gl_transaction_date", DBSafeUtils.DateTimeToSQL(dr["gl_transaction_date"].ToString()));
                            items.Add("gl_transaction_description", "'Reverse " + dr["gl_transaction_description"].ToString() + "'");
                            items.Add("gl_transaction_net", -decimal.Parse(dr["gl_transaction_net"].ToString()));
                            items.Add("gl_transaction_tax", -decimal.Parse(dr["gl_transaction_tax"].ToString()));
                            items.Add("gl_transaction_gross", -decimal.Parse(dr["gl_transaction_gross"].ToString()));
                            items.Add("gl_transaction_ref_type_id", dr["gl_transaction_ref_type_id"].ToString());
                            mydb.ExecuteInsert("gl_transactions", items);//insert each row

                            string invglsql = "Insert Into receipt_gls values(NULL," + id + "," + AdFunction.GetInsertID(mydb) + ",0)";
                            mydb.ExecuteScalar(invglsql);
                            items.Clear();
                        }
                    }

                    Receipt r = new Receipt(constr);
                    r.LoadData(int.Parse(id));

                    string num = r.ReceiptRef;
                    num = num + "-REV";
                    string invsql = "update  receipts set `receipt_ref`='" + num + "', receipt_gross=0 where receipt_id=" + id;
                    mydb.ExecuteScalar(invsql);

                }
                #endregion

                #region Cpayment
                if (type.Equals("Cpayment"))
                {
                    sql = "SELECT   * FROM      gl_transactions, cpayment_gls, cpayments WHERE   gl_transactions.gl_transaction_id = cpayment_gls.cpayment_gl_gl_id AND                  cpayment_gls.cpayment_gl_cpayment_id = cpayments.cpayment_id        and        cpayment_id = " + id;
                    DataTable dt = o.ReturnTable(sql, "NewTable");
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!dr["gl_transaction_ref"].ToString().ToString().Contains("-REV"))
                        {

                            //修改原数据REF
                            string sql2 = "update gl_transactions set gl_transaction_ref = " + DBSafeUtils.StrToQuoteSQL(dr["gl_transaction_ref"].ToString() + "-REV") + " where gl_transaction_id=" + dr["gl_transaction_id"].ToString();
                            mydb.ReturnTable(sql2, "update");
                            Hashtable items = new Hashtable();
                            items.Add("gl_transaction_ref", DBSafeUtils.StrToQuoteSQL(dr["gl_transaction_ref"].ToString() + "-REV"));
                            items.Add("gl_transaction_chart_id", dr["gl_transaction_chart_id"].ToString());
                            items.Add("gl_transaction_bodycorp_id", dr["gl_transaction_bodycorp_id"].ToString());
                            items.Add("gl_transaction_type_id", dr["gl_transaction_type_id"].ToString());
                            string unitid = dr["gl_transaction_unit_id"].ToString();
                            if (!unitid.Equals(""))
                                items.Add("gl_transaction_unit_id", unitid);
                            items.Add("gl_transaction_date", DBSafeUtils.DateTimeToSQL(dr["gl_transaction_date"].ToString()));
                            items.Add("gl_transaction_description", "'Reverse " + dr["gl_transaction_description"].ToString() + "'");
                            items.Add("gl_transaction_net", -decimal.Parse(dr["gl_transaction_net"].ToString()));
                            items.Add("gl_transaction_tax", -decimal.Parse(dr["gl_transaction_tax"].ToString()));
                            items.Add("gl_transaction_gross", -decimal.Parse(dr["gl_transaction_gross"].ToString()));
                            items.Add("gl_transaction_ref_type_id", dr["gl_transaction_ref_type_id"].ToString());
                            mydb.ExecuteInsert("gl_transactions", items);//insert each row

                            string invglsql = "Insert Into cpayment_gls values(NULL," + id + "," + AdFunction.GetInsertID(mydb) + ",0)";
                            mydb.ExecuteScalar(invglsql);
                            items.Clear();
                        }
                    }

                    CPayment r = new CPayment(constr);
                    r.LoadData(int.Parse(id));

                    string num = r.Cpayment_reference;
                    num = num + "-REV";
                    string invsql = "update  cpayments set `cpayment_reference`='" + num + "', cpayment_gross=0 where cpayment_id=" + id;
                    mydb.ExecuteScalar(invsql);
                }
                #endregion

                mydb.Commit();
                GetData(type, id);


            }

            catch (Exception ex)
            {
                mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }

        }
        public void GetData(string type, string id)
        {

            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            Odbc o = new Odbc(constr);
            TypeL.Text = type;
            DataTable dt = new DataTable("DT");
            dt.Columns.Add("Rec");
            dt.Columns.Add("Rev");
            dt.Columns.Add("Code");
            dt.Columns.Add("Name");
            dt.Columns.Add("Type");
            dt.Columns.Add("Date");
            dt.Columns.Add("Ref");
            dt.Columns.Add("BID");
            dt.Columns.Add("Description");
            dt.Columns.Add("Debit");
            dt.Columns.Add("Credit");

            string sql = "";
            if (type.Equals("Invoice"))
            {
                sql = "SELECT    gl_transactions.gl_transaction_rec  As Rec,  gl_transactions.gl_transaction_rev AS Rev,    invoice_master.invoice_master_num, invoice_master.invoice_master_debtor_id, invoice_types.invoice_type_code, invoice_types.invoice_type_name,                           invoice_master.invoice_master_date, invoice_master.invoice_master_net, invoice_master.invoice_master_tax, invoice_master.invoice_master_gross,                           invoice_gls.invoice_gl_paid, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date AS `Date`, gl_transactions.gl_transaction_id, chart_master.chart_master_code AS Code,                           chart_master.chart_master_name AS Name, invoice_master.invoice_master_id, gl_transactions.gl_transaction_ref AS Ref,                           gl_transactions.gl_transaction_batch_id AS BID, gl_transactions.gl_transaction_description AS Description, invoice_master.invoice_master_bodycorp_id,                           bodycorps.bodycorp_code, bodycorps.bodycorp_name, chart_types.chart_type_name AS Type, gl_transactions.gl_transaction_type_id,                           chart_master.chart_master_id FROM            invoice_master, invoice_types, invoice_gls, gl_transactions, chart_master, bodycorps, chart_types WHERE        invoice_master.invoice_master_type_id = invoice_types.invoice_type_id AND invoice_master.invoice_master_id = invoice_gls.invoice_gl_invoice_id AND                           invoice_gls.invoice_gl_gl_id = gl_transactions.gl_transaction_id AND gl_transactions.gl_transaction_chart_id = chart_master.chart_master_id AND                           invoice_master.invoice_master_bodycorp_id = bodycorps.bodycorp_id AND chart_master.chart_master_type_id = chart_types.chart_type_id AND                           (gl_transactions.gl_transaction_type_id <> 3) AND (gl_transactions.gl_transaction_type_id <> 4) AND    (gl_transactions.gl_transaction_type_id <> 6)  and  (gl_transaction_ref_type_id <>3) AND (invoice_master.invoice_master_id =" + id + ")";
            }
            if (type.Equals("InvoiceDetail") || type.Equals("CreditNoteDetail"))
            {
                sql = "SELECT     gl_transactions.gl_transaction_rec  As Rec,  gl_transactions.gl_transaction_rev AS Rev,   invoice_master.invoice_master_num, invoice_master.invoice_master_debtor_id, invoice_types.invoice_type_code, invoice_types.invoice_type_name,                           invoice_master.invoice_master_date, invoice_master.invoice_master_net, invoice_master.invoice_master_tax, invoice_master.invoice_master_gross,                           invoice_gls.invoice_gl_paid, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date AS `Date`, gl_transactions.gl_transaction_id, chart_master.chart_master_code AS Code,                           chart_master.chart_master_name AS Name, invoice_master.invoice_master_id, gl_transactions.gl_transaction_ref AS Ref,                           gl_transactions.gl_transaction_batch_id AS BID, gl_transactions.gl_transaction_description AS Description, invoice_master.invoice_master_bodycorp_id,                           bodycorps.bodycorp_code, bodycorps.bodycorp_name, chart_types.chart_type_name AS Type, gl_transactions.gl_transaction_type_id FROM            invoice_master, invoice_types, invoice_gls, gl_transactions, chart_master, bodycorps, chart_types WHERE        invoice_master.invoice_master_type_id = invoice_types.invoice_type_id AND invoice_master.invoice_master_id = invoice_gls.invoice_gl_invoice_id AND                           invoice_gls.invoice_gl_gl_id = gl_transactions.gl_transaction_id AND gl_transactions.gl_transaction_chart_id = chart_master.chart_master_id AND                           invoice_master.invoice_master_bodycorp_id = bodycorps.bodycorp_id AND chart_master.chart_master_type_id = chart_types.chart_type_id      AND    (gl_transaction_ref_type_id <>3)     and      (invoice_master.invoice_master_id =" + id + ")";
            }
            if (type.Equals("Receipt"))
            {
                sql = "SELECT  gl_transactions.gl_transaction_rec  As Rec,  gl_transactions.gl_transaction_rev AS Rev,      debtor_master.debtor_master_name, debtor_master.debtor_master_name, gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_ref AS Ref,                           gl_transactions.gl_transaction_chart_id, chart_master.chart_master_code AS Code, chart_master.chart_master_name AS Name,                           receipt_notes AS Description, gl_transactions.gl_transaction_batch_id AS BID, gl_transactions.gl_transaction_net, receipts.receipt_id,                           gl_transactions.gl_transaction_date AS `Date`, bodycorps.bodycorp_code, bodycorps.bodycorp_name, chart_types.chart_type_name AS Type,                           chart_master.chart_master_id FROM            gl_transactions, receipt_gls, receipts, debtor_master, chart_master, bodycorps, chart_types WHERE        gl_transactions.gl_transaction_id = receipt_gls.receipt_gl_gl_id AND receipt_gls.receipt_gl_receipt_id = receipts.receipt_id AND                           receipts.receipt_debtor_id = debtor_master.debtor_master_id AND gl_transactions.gl_transaction_chart_id = chart_master.chart_master_id AND                           gl_transactions.gl_transaction_bodycorp_id = bodycorps.bodycorp_id AND chart_master.chart_master_type_id = chart_types.chart_type_id AND                           (gl_transactions.gl_transaction_type_id <> 3) AND (gl_transactions.gl_transaction_type_id <> 4)   AND    (gl_transactions.gl_transaction_type_id <> 6) AND                         (receipts.receipt_id = " + id + ") ";
            }
            if (type.Equals("Journal"))
            {
                string temp = "SELECT * FROM gl_transactions WHERE        (gl_transaction_id = " + id + ") ";
                string refe = o.ReturnTable(temp, "gl").Rows[0]["gl_transaction_ref"].ToString();
                sql = "SELECT   gl_transactions.gl_transaction_rec  As Rec,  gl_transactions.gl_transaction_rev AS Rev,     gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_ref AS Ref, gl_transactions.gl_transaction_chart_id, chart_master.chart_master_code AS Code,                           chart_master.chart_master_name AS Name, gl_transactions.gl_transaction_description AS Description, gl_transactions.gl_transaction_batch_id AS BID,                           gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_date AS `Date`, bodycorps.bodycorp_code, bodycorps.bodycorp_name,                           chart_types.chart_type_name AS Type FROM            chart_master, gl_transactions, bodycorps, chart_types WHERE        chart_master.chart_master_id = gl_transactions.gl_transaction_chart_id AND gl_transactions.gl_transaction_bodycorp_id = bodycorps.bodycorp_id AND    (gl_transactions.gl_transaction_type_id <> 3 AND gl_transactions.gl_transaction_type_id <> 4)  AND                              chart_master.chart_master_type_id = chart_types.chart_type_id AND (gl_transactions.gl_transaction_ref = '" +
                   refe + "')  ";
            }
            if (type.Equals("Cinvoice"))
            {
                sql = "SELECT   gl_transactions.gl_transaction_rec  As Rec,  gl_transactions.gl_transaction_rev AS Rev,     gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_ref AS Ref, gl_transactions.gl_transaction_chart_id, chart_master.chart_master_code AS Code,                           chart_master.chart_master_name AS Name, gl_transactions.gl_transaction_description AS Description, gl_transactions.gl_transaction_batch_id AS BID,                           gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_date AS `Date`, bodycorps.bodycorp_code, bodycorps.bodycorp_name,                           chart_types.chart_type_name AS Type, cinvoices.cinvoice_id FROM            bodycorps, chart_master, gl_transactions, chart_types, cinvoice_gls, cinvoices WHERE        bodycorps.bodycorp_id = cinvoices.cinvoice_bodycorp_id AND chart_master.chart_master_id = gl_transactions.gl_transaction_chart_id AND                           chart_master.chart_master_type_id = chart_types.chart_type_id AND gl_transactions.gl_transaction_id = cinvoice_gls.cinvoice_gl_gl_id AND    (gl_transactions.gl_transaction_type_id <> 3 AND gl_transactions.gl_transaction_type_id <> 4)  AND                              cinvoice_gls.cinvoice_gl_cinvoice_id = cinvoices.cinvoice_id AND (cinvoices.cinvoice_id = " + id + ") ";
            }
            if (type.Equals("Cpayment"))
            {
                sql = "SELECT   gl_transactions.gl_transaction_rec  As Rec,  gl_transactions.gl_transaction_rev AS Rev,   gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_ref AS Ref, gl_transactions.gl_transaction_chart_id, chart_master.chart_master_code AS Code,                           chart_master.chart_master_name AS Name, gl_transactions.gl_transaction_description AS Description, gl_transactions.gl_transaction_batch_id AS BID,                           gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_date AS `Date`, bodycorps.bodycorp_code, bodycorps.bodycorp_name,                           chart_types.chart_type_name AS Type, cpayments.cpayment_id FROM            cpayments, bodycorps, cpayment_gls, chart_master, gl_transactions, chart_types WHERE        cpayments.cpayment_bodycorp_id = bodycorps.bodycorp_id AND cpayments.cpayment_id = cpayment_gls.cpayment_gl_cpayment_id AND                           cpayment_gls.cpayment_gl_gl_id = gl_transactions.gl_transaction_id AND chart_master.chart_master_id = gl_transactions.gl_transaction_chart_id AND   (gl_transactions.gl_transaction_type_id <> 3 AND gl_transactions.gl_transaction_type_id <> 4)  AND                               chart_master.chart_master_type_id = chart_types.chart_type_id AND (cpayments.cpayment_id =" + id + ") ";
            }

            DataTable invoiceDT = o.ReturnTable(sql, "NewTable");
            if (invoiceDT.Rows.Count > 0)
                bodycorpL.Text = invoiceDT.Rows[0]["bodycorp_name"].ToString();

            foreach (DataRow dr in invoiceDT.Rows)
            {
                DataRow newRow = dt.NewRow();
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
                if (n > 0)
                {
                    newRow["Credit"] = n;
                    dt.Rows.Add(newRow);
                }
                if (n < 0)
                {
                    newRow["Debit"] = -n;
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
                string sqlselectstr = "select Code,Name,Date,Ref,Description,Debit,Credit FROM (SELECT *";
                DataTable dt = (DataTable)HttpContext.Current.Session["TempDT"];
                string sqlfromstr = "FROM `" + dt.TableName + "`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                Decimal c = 0;
                Decimal d = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    string c1 = dr["Credit"].ToString();
                    string d1 = dr["Debit"].ToString();
                    if (!c1.Equals(""))
                        c += Decimal.Parse(c1);
                    if (!d1.Equals(""))
                        d += Decimal.Parse(d1);
                }
                Hashtable userdata = new Hashtable();
                userdata.Add("Description", "Total:");
                userdata.Add("Credit", c);
                userdata.Add("Debit", d);
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