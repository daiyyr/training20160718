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
using Sapp.General;
using System.IO;
using System.Data.Odbc;
namespace sapp_sms
{
    public partial class consolidation : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {



            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
            {

                string sql = "SELECT   receipt_gls.receipt_gl_gl_id, receipts.* FROM      receipts, receipt_gls WHERE   receipts.receipt_id = receipt_gls.receipt_gl_receipt_id";
                DataTable dt = mydb.ReturnTable(sql, "temp");
                mydb.StartTransaction();
                foreach (DataRow dr in dt.Rows)
                {
                    string unitid = dr["receipt_unit_id"].ToString();
                    string gid = dr["receipt_gl_gl_id"].ToString();
                    string updatesql = "update gl_transactions set gl_transaction_unit_id =" + unitid + " where gl_transaction_id=" + gid;
                    mydb.ExecuteScalar(updatesql);
                }
                mydb.Commit();
                MessageL.Text = "Recipt Unit ID Fixed";
            }
            catch (Exception ex)
            {

                mydb.Rollback();
                MessageL.Text = (ex.Message);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
            {
                string sql = "SELECT  * FROM      gl_transactions, invoice_gls, receipt_gls, invoice_master, receipts WHERE   gl_transactions.gl_transaction_id = invoice_gls.invoice_gl_gl_id AND                 invoice_gls.invoice_gl_gl_id = receipt_gls.receipt_gl_gl_id AND        invoice_gls.invoice_gl_invoice_id = invoice_master.invoice_master_id AND        receipt_gls.receipt_gl_receipt_id = receipts.receipt_id AND (gl_transactions.gl_transaction_type_id = 6)";
                DataTable disDT = mydb.ReturnTable(sql, "temp");
                string paysql = "SELECT   * FROM      gl_transactions, receipt_gls, invoice_gls, invoice_master, receipts WHERE   gl_transactions.gl_transaction_id = receipt_gls.receipt_gl_gl_id AND   gl_transactions.gl_transaction_id = invoice_gls.invoice_gl_gl_id AND                 invoice_gls.invoice_gl_invoice_id = invoice_master.invoice_master_id AND                 receipt_gls.receipt_gl_receipt_id = receipts.receipt_id";
                DataTable RecPayDT = mydb.ReturnTable(paysql, "pay");

                RecPayDT.Columns.Add("Discount");
                DataTable binddt = RecPayDT.Clone();
                foreach (DataRow pdr in RecPayDT.Rows)
                {
                    string pinvID = pdr["invoice_master_id"].ToString();
                    decimal allowcated = AdFunction.Rounded(pdr["gl_transaction_net"].ToString(), 2);
                    decimal invtotal = AdFunction.Rounded(pdr["invoice_master_gross"].ToString(), 2);
                    foreach (DataRow dr in disDT.Rows)
                    {
                        string invID = dr["invoice_master_id"].ToString();
                        if (pinvID.Equals(invID))
                        {
                            decimal discount = AdFunction.Rounded(dr["gl_transaction_net"].ToString(), 2);
                            pdr["Discount"] = discount.ToString();
                            if (allowcated + discount > invtotal)
                            {
                                binddt.ImportRow(pdr);
                            }
                        }
                    }
                }
                GridView1.DataSource = binddt;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                mydb.Rollback();
                mydb.Close();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
            {
                string sql = "SELECT   * FROM      gl_transactions WHERE   (gl_transaction_type_id = 6) GROUP BY gl_transaction_ref";

                DataTable dt = mydb.ReturnTable(sql, "temp");
                DataTable bindDT = dt.Clone();
                foreach (DataRow dr in dt.Rows)
                {
                    string reference = dr["gl_transaction_ref"].ToString();
                    string sql2 = "select sum(`gl_transaction_net`) from gl_transactions where (gl_transaction_type_id = 6) and gl_transaction_ref= " + DBSafeUtils.StrToQuoteSQL(reference);
                    DataTable sumdt = mydb.ReturnTable(sql2, "sum");
                    decimal sum = decimal.Parse(sumdt.Rows[0][0].ToString());
                    if (sum != 0)
                        bindDT.ImportRow(dr);
                }
                GridView1.DataSource = bindDT;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
            {
                string isql = "SELECT * FROM      invoice_master, invoice_gls, gl_transactions WHERE   invoice_master.invoice_master_id = invoice_gls.invoice_gl_invoice_id AND                  invoice_gls.invoice_gl_gl_id = gl_transactions.gl_transaction_id ";
                DataTable invDT = mydb.ReturnTable(isql, "t1");

                DataTable tempDT = new DataTable();
                tempDT.Columns.Add("ID");
                tempDT.Columns.Add("Type");


                foreach (DataRow dr in invDT.Rows)
                {

                    string id = dr["invoice_master_id"].ToString();
                    decimal gross = AdFunction.Rounded(dr["invoice_master_gross"].ToString(), 2);
                    string tempsql = isql.Replace("*", " Sum(`gl_transaction_net`) ") + " and invoice_master_id=" + id;
                    decimal t = AdFunction.Rounded(mydb.ReturnTable(tempsql, "t").Rows[0][0].ToString(), 2);
                    if (gross != t)
                    {
                        DataRow newdr = tempDT.NewRow();
                        newdr["ID"] = id;
                        newdr["Type"] = "Invoice";
                    }
                }



                GridView1.DataSource = tempDT;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
            {
                string cisql = "SELECT   * FROM      cinvoice_gls, cinvoices, gl_transactions WHERE   cinvoice_gls.cinvoice_gl_cinvoice_id = cinvoices.cinvoice_id AND                  cinvoice_gls.cinvoice_gl_gl_id = gl_transactions.gl_transaction_id ";
                DataTable cinvDT = mydb.ReturnTable(cisql, "t2");
                DataTable tempDT = new DataTable();
                tempDT.Columns.Add("ID");
                tempDT.Columns.Add("Type");

                foreach (DataRow dr in cinvDT.Rows)
                {

                    string id = dr["cinvoice_id"].ToString();
                    decimal gross = AdFunction.Rounded(dr["cinvoice_gross"].ToString(), 2);
                    string tempsql = cisql.Replace("*", " Sum(`gl_transaction_net`) ") + " and cinvoice_id=" + id;
                    decimal t = AdFunction.Rounded(mydb.ReturnTable(tempsql, "t").Rows[0][0].ToString(), 2);
                    if (gross != t)
                    {
                        DataRow newdr = tempDT.NewRow();
                        newdr["ID"] = id;
                        newdr["Type"] = "Cinvoice";
                    }
                }
                GridView1.DataSource = tempDT;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
            {
                string cpsql = "SELECT  * FROM      cpayment_gls, cpayments, gl_transactions WHERE   cpayment_gls.cpayment_gl_cpayment_id = cpayments.cpayment_id AND                  cpayment_gls.cpayment_gl_gl_id = gl_transactions.gl_transaction_id ";
                DataTable cpayDT = mydb.ReturnTable(cpsql, "t3");
                DataTable tempDT = new DataTable();
                tempDT.Columns.Add("ID");
                tempDT.Columns.Add("Type");
                tempDT.Columns.Add("Description");

                foreach (DataRow dr in cpayDT.Rows)
                {

                    string id = dr["cpayment_id"].ToString();
                    decimal gross = AdFunction.Rounded(dr["cpayment_gross"].ToString(), 2);
                    string tempsql = cpsql.Replace("*", " Sum(`gl_transaction_net`) ") + " and cpayment_id=" + id;
                    decimal t = AdFunction.Rounded(mydb.ReturnTable(tempsql, "t").Rows[0][0].ToString(), 2);
                    if (gross != t)
                    {
                        DataRow newdr = tempDT.NewRow();
                        newdr["ID"] = id;
                        newdr["Type"] = "Cpayment";
                    }
                }
                string sql = "select * from cpayments where`cpayment_gross`<`cpayment_allocated`";
                DataTable temp = mydb.ReturnTable(sql, "t4");
                foreach (DataRow dr in temp.Rows)
                {

                    string id = dr["receipt_id"].ToString();
                    decimal gross = AdFunction.Rounded(dr["cpayment_gross"].ToString(), 2);
                    decimal allow = AdFunction.Rounded(dr["cpayment_allocated"].ToString(), 2);
                    if (gross < allow)
                    {
                        DataRow newdr = tempDT.NewRow();
                        newdr["ID"] = id;
                        newdr["Type"] = "Cpayment";
                        newdr["Description"] = "Allocated<Gross";
                    }
                }
                GridView1.DataSource = tempDT;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        protected void Button7_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
            {
                string recsql = "SELECT   * FROM      receipt_gls, receipts, gl_transactions WHERE   receipt_gls.receipt_gl_receipt_id = receipts.receipt_id AND                  receipt_gls.receipt_gl_gl_id = gl_transactions.gl_transaction_id ";
                DataTable recDT = mydb.ReturnTable(recsql, "t4");
                DataTable tempDT = new DataTable();
                tempDT.Columns.Add("ID");
                tempDT.Columns.Add("Type");
                tempDT.Columns.Add("Description");

                foreach (DataRow dr in recDT.Rows)
                {

                    string id = dr["receipt_id"].ToString();
                    decimal gross = AdFunction.Rounded(dr["receipt_gross"].ToString(), 2);
                    string tempsql = recsql.Replace("*", " Sum(`gl_transaction_net`) ") + " and receipt_id=" + id;
                    decimal t = AdFunction.Rounded(mydb.ReturnTable(tempsql, "t").Rows[0][0].ToString(), 2);
                    if (gross != t)
                    {
                        DataRow newdr = tempDT.NewRow();
                        newdr["ID"] = id;
                        newdr["Type"] = "Receipt";
                        newdr["Description"] = "GL Problem";
                    }
                }

                string sql = "select * from receipts where`receipt_gross`<`receipt_allocated`";
                DataTable temp = mydb.ReturnTable(sql, "t4");
                foreach (DataRow dr in temp.Rows)
                {

                    string id = dr["receipt_id"].ToString();
                    decimal gross = AdFunction.Rounded(dr["receipt_gross"].ToString(), 2);
                    decimal allow = AdFunction.Rounded(dr["receipt_allocated"].ToString(), 2);
                    if (gross < allow)
                    {
                        DataRow newdr = tempDT.NewRow();
                        newdr["ID"] = id;
                        newdr["Type"] = "Receipt";
                        newdr["Description"] = "Allocated<Gross";
                    }
                }

                GridView1.DataSource = tempDT;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        protected void Button8_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
            {
                string sql = "SELECT   * FROM  gl_transactions where gl_transaction_ref like '%REV%'";
                DataTable DT = mydb.ReturnTable(sql, "t4");
                foreach (DataRow dr in DT.Rows)
                {
                    string refe = dr["gl_transaction_ref"].ToString();
                    string id = dr["gl_transaction_id"].ToString();
                    if (refe.Contains("REV"))
                    {
                        string update = " update gl_transactions set gl_transaction_rev=1,gl_transaction_ref='" + refe.Replace("-REV", "") + "' where gl_transaction_id=" + id;
                        mydb.ExecuteScalar(update);
                    }
                    if (refe.Contains("REC"))
                    {
                        string update = " update gl_transactions set gl_transaction_rec=1,gl_transaction_ref='" + refe.Replace("-REC", "") + "' where gl_transaction_id=" + id;
                        mydb.ExecuteScalar(update);
                    }
                }
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        protected void Button9_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
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
                s.LoadData("GENERALCREDITOR");
                ch.LoadData(s.SystemValue);
                string creditorID = ch.ChartMasterId.ToString();
                s.LoadData("DISCOUNTCHARCODE");
                ch.LoadData(s.SystemValue);
                string discountID = ch.ChartMasterId.ToString();

                #endregion

                string sql = "SELECT   * FROM  gl_transactions where gl_transaction_type_id =6 and 	 gl_transaction_chart_id =" + discountID + " and gl_transaction_date >='2014-01-01'";
                DataTable DT = mydb.ReturnTable(sql, "t");
                foreach (DataRow dr in DT.Rows)
                {
                    string refernce = dr["gl_transaction_ref"].ToString();
                    string sql2 = "SELECT   * FROM  gl_transactions where gl_transaction_type_id =6 and 	 gl_transaction_ref ='" + refernce + "'";
                    DataTable dt2 = mydb.ReturnTable(sql2, "t2");
                    if (dt2.Rows.Count == 3)
                    {

                        string grossid = ReportDT.GetDataByColumn(dt2, "gl_transaction_chart_id", AdFunction.GENERALDEBTOR_ChartID, "gl_transaction_id");

                        string netid = ReportDT.GetDataByColumn(dt2, "gl_transaction_chart_id", discountID, "gl_transaction_id");


                        string taxid = ReportDT.GetDataByColumn(dt2, "gl_transaction_chart_id", gstid, "gl_transaction_id");

                        if (!grossid.Equals("") && !taxid.Equals("") && !netid.Equals(""))
                        {
                            double gross = double.Parse(ReportDT.GetDataByColumn(dt2, "gl_transaction_chart_id", AdFunction.GENERALDEBTOR_ChartID, "gl_transaction_net"));
                            double tax = gross / 115 * 15;
                            double net = gross - tax;
                            net = -net;
                            tax = -tax;

                            string updateNetSQL = "UPDATE gl_transactions set gl_transaction_net=" + net.ToString("f2") + " where  gl_transaction_id =" + netid;
                            string updateTaxSQL = "UPDATE gl_transactions set gl_transaction_net=" + tax.ToString("f2") + " where  gl_transaction_id =" + taxid;
                            mydb.ExecuteScalar(updateNetSQL);
                            mydb.ExecuteScalar(updateTaxSQL);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        protected void Button10_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
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
                s.LoadData("GENERALCREDITOR");
                ch.LoadData(s.SystemValue);
                string creditorID = ch.ChartMasterId.ToString();
                s.LoadData("DISCOUNTCHARCODE");
                ch.LoadData(s.SystemValue);
                string discountID = ch.ChartMasterId.ToString();

                #endregion

                string sql = "SELECT   * FROM  gl_transactions where gl_transaction_type_id =6 and 	 gl_transaction_chart_id =" + discountID;
                DataTable DT = mydb.ReturnTable(sql, "t");
                DataTable checkdt = new DataTable();
                checkdt.Columns.Add("REF");
                checkdt.Columns.Add("OLDNET");
                checkdt.Columns.Add("NET");
                checkdt.Columns.Add("OLDTAX");
                checkdt.Columns.Add("TAX");
                foreach (DataRow dr in DT.Rows)
                {
                    string refernce = dr["gl_transaction_ref"].ToString();
                    string sql2 = "SELECT   * FROM  gl_transactions where gl_transaction_type_id =6 and 	 gl_transaction_ref ='" + refernce + "'";
                    DataTable dt2 = mydb.ReturnTable(sql2, "t2");
                    if (dt2.Rows.Count == 3)
                    {

                        string grossid = ReportDT.GetDataByColumn(dt2, "gl_transaction_chart_id", AdFunction.GENERALDEBTOR_ChartID, "gl_transaction_id");

                        string netid = ReportDT.GetDataByColumn(dt2, "gl_transaction_chart_id", discountID, "gl_transaction_id");


                        string taxid = ReportDT.GetDataByColumn(dt2, "gl_transaction_chart_id", gstid, "gl_transaction_id");
                        if (!grossid.Equals("") && !taxid.Equals("") && !netid.Equals(""))
                        {
                            double oldtax = double.Parse(ReportDT.GetDataByColumn(dt2, "gl_transaction_chart_id", gstid, "gl_transaction_net"));
                            double oldnet = double.Parse(ReportDT.GetDataByColumn(dt2, "gl_transaction_chart_id", discountID, "gl_transaction_net"));

                            double gross = double.Parse(ReportDT.GetDataByColumn(dt2, "gl_transaction_chart_id", AdFunction.GENERALDEBTOR_ChartID, "gl_transaction_net"));
                            double tax = gross / 115 * 15;
                            double net = gross - tax;
                            net = -net;
                            tax = -tax;
                            if (!oldnet.ToString("f2").Equals(net.ToString("f2")) || !oldtax.ToString("f2").Equals(tax.ToString("f2")))
                            {
                                DataRow newdr = checkdt.NewRow();
                                newdr["REF"] = refernce;
                                newdr["OLDNET"] = oldnet.ToString("f2");
                                newdr["NET"] = net.ToString("f2");
                                newdr["OLDTAX"] = oldtax.ToString("f2");
                                newdr["TAX"] = tax.ToString("f2");
                                checkdt.Rows.Add(newdr);
                            }
                        }
                    }
                }
                GridView1.DataSource = checkdt;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        protected void Button11_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
            {
                string sql = "SELECT * FROM `cinvoices` WHERE `cinvoice_unit_id` IS NOT NULL";
                DataTable cinvDT = mydb.ReturnTable(sql, "t");
                DataTable invDT = mydb.ReturnTable("select * from invoice_master", "t");
                DataTable checkDT = cinvDT.Clone();
                foreach (DataRow dr in cinvDT.Rows)
                {
                    string cunitid = dr["cinvoice_unit_id"].ToString();
                    string camount = dr["cinvoice_gross"].ToString();
                    DataTable tempInvDT = ReportDT.FilterDT(invDT, "invoice_master_unit_id = " + cunitid);
                    bool check = false;
                    foreach (DataRow dr2 in tempInvDT.Rows)
                    {
                        string amount = dr2["invoice_master_gross"].ToString();
                        if (amount.Equals(camount))
                        {
                            check = true;
                        }
                    }
                    if (!check)
                    {
                        checkDT.ImportRow(dr);
                    }
                }
                GridView1.DataSource = checkDT;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        protected void Button12_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);

            try
            {
                string sql = "SELECT * FROM `gl_transactions` WHERE `gl_transaction_tax` >0";
                DataTable GLDT = mydb.ReturnTable(sql, "t");
                DataTable resultDT = GLDT.Clone();
                foreach (DataRow dr in GLDT.Rows)
                {
                    double tax = double.Parse(dr["gl_transaction_tax"].ToString());
                    double gross = double.Parse(dr["gl_transaction_gross"].ToString());
                    double check = (gross / 1.15 * 0.15);
                    check = double.Parse(check.ToString("f2"));
                    if (check != tax)
                    {
                        if (check != (tax + 0.01))
                            if (check != (tax - 0.01))
                                resultDT.ImportRow(dr);
                    }
                }
                GridView1.DataSource = resultDT;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        protected void Button13_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
            {
                string sql = "SELECT * FROM `cinvoices`";
                DataTable cinvDT = mydb.ReturnTable(sql, "t");
                DataTable checkDT = cinvDT.Clone();
                foreach (DataRow dr in cinvDT.Rows)
                {
                    string cumn = dr["cinvoice_num"].ToString();
                    string sql2 = "SELECT * FROM `cinvoices` where cinvoice_num='" + cumn + "'";
                    DataTable temp = mydb.ReturnTable(sql2, "123");
                    if (temp.Rows.Count > 1)
                    {
                        bool check = true;
                        foreach (DataRow dr2 in checkDT.Rows)
                        {
                            if (dr2["cinvoice_num"].Equals(cumn))
                            {
                                check = false;
                            }
                        }
                        if (check)
                            checkDT.ImportRow(dr);


                    }
                }
                GridView1.DataSource = checkDT;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        protected void Button14_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
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

                s.LoadData("GENERALCREDITOR");
                ch.LoadData(s.SystemValue);
                string creditorID = ch.ChartMasterId.ToString();
                s.LoadData("DISCOUNTCHARCODE");
                ch.LoadData(s.SystemValue);
                string discountID = ch.ChartMasterId.ToString();

                #endregion
                string sql = "SELECT   unit_master.unit_master_code, gl_transactions.* FROM      gl_transactions, unit_master WHERE   gl_transactions.gl_transaction_unit_id = unit_master.unit_master_id AND (gl_transactions.gl_transaction_ref_type_id = 6)";
                DataTable DT = mydb.ReturnTable(sql, "t");
                DataTable checkDT = DT.Clone();
                foreach (DataRow dr in DT.Rows)
                {
                    string refe = dr["gl_transaction_ref"].ToString();
                    string sql2 = "SELECT * FROM `gl_transactions` where gl_transaction_ref='" + refe + "'";
                    DataTable tempDT = mydb.ReturnTable(sql2, "t");
                    double net = 0;
                    double tax = 0;
                    double gross = 0;
                    foreach (DataRow dr2 in tempDT.Rows)
                    {
                        if (dr2["gl_transaction_chart_id"].ToString().Equals(gstid))
                        {
                            tax += double.Parse(dr2["gl_transaction_net"].ToString());
                        }
                    }
                    if (tax > 0)
                    {
                        foreach (DataRow dr2 in tempDT.Rows)
                        {
                            double tempnet = double.Parse(dr2["gl_transaction_net"].ToString());
                            if (tempnet < 0)
                                gross += double.Parse(dr2["gl_transaction_net"].ToString());
                        }
                        gross = -gross;
                    }
                    if (tax < 0)
                    {
                        foreach (DataRow dr2 in tempDT.Rows)
                        {
                            double tempnet = double.Parse(dr2["gl_transaction_net"].ToString());
                            if (tempnet > 0)
                                gross += double.Parse(dr2["gl_transaction_net"].ToString());
                        }
                        tax = -tax;
                    }
                    double temp = gross / 1.15 * 0.15;
                    temp = double.Parse(temp.ToString("f2"));
                    if (temp != tax)
                        if (temp + 0.01 != tax)
                            if (temp - 0.01 != tax)
                                if (ReportDT.GetDataByColumn(checkDT, "gl_transaction_ref", refe, "gl_transaction_ref").ToString().Equals(""))
                                    checkDT.ImportRow(dr);
                }
                GridView1.DataSource = checkDT;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        protected void Button15_Click(object sender, EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            try
            {
                string sql = "SELECT * FROM `gl_transactions` where gl_transaction_type_id=6  and (gl_transaction_rev <>0 or  gl_transaction_rec <>0)  group by gl_transaction_ref";
                DataTable glDT = mydb.ReturnTable(sql, "t");
                DataTable checkDT = new DataTable();
                checkDT.Columns.Add("Reference");
                checkDT.Columns.Add("Rev");
                checkDT.Columns.Add("Rec");

                foreach (DataRow dr in glDT.Rows)
                {
                    string refe = dr["gl_transaction_ref"].ToString();
                    string sql2 = "SELECT * FROM `gl_transactions` where gl_transaction_ref='" + refe + "' ";
                    DataTable temp = mydb.ReturnTable(sql2, "123");
                    if (temp.Rows.Count > 0)
                    {
                        foreach (DataRow dr2 in temp.Rows)
                        {
                            string rec = dr2["gl_transaction_rev"].ToString();
                            string rev = dr2["gl_transaction_rec"].ToString();
                            string baid = dr2["gl_transaction_recbatchid"].ToString();
                            string revsql = "";
                            string recsql = "";
                            string bsql = "";
                            //string batchid = dr2["gl_transaction_recbatchid"].ToString();
                            if (rev.Equals("1"))
                                revsql = "update gl_transactions set gl_transaction_rev=1 where gl_transaction_ref ='" + refe + "'";
                            if (rec.Equals("1"))
                                recsql = "update gl_transactions set gl_transaction_rec=1 where gl_transaction_ref ='" + refe + "'";
                            if (!baid.Equals(""))
                                bsql = "update gl_transactions set gl_transaction_recbatchid=" + baid + " where gl_transaction_ref ='" + refe + "'";
                            if (!revsql.Equals(""))
                            {
                                mydb.ExecuteScalar(revsql);
                                DataRow newrow = checkDT.NewRow();
                                newrow["Reference"] = refe;
                                newrow["Rev"] = "Changed";
                                checkDT.Rows.Add(newrow);
                            }
                            if (!recsql.Equals(""))
                            {
                                mydb.ExecuteScalar(recsql);
                                DataRow newrow = checkDT.NewRow();
                                newrow["Reference"] = refe;
                                newrow["Rec"] = "Changed";
                                checkDT.Rows.Add(newrow);
                            }
                            if (!bsql.Equals(""))
                            {
                                mydb.ExecuteScalar(bsql);
                                DataRow newrow = checkDT.NewRow();
                                newrow["Reference"] = refe;
                                newrow["Rec"] = "Batch";
                                checkDT.Rows.Add(newrow);
                            }
                        }
                    }
                }
                GridView1.DataSource = checkDT;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        protected void Button16_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            try
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
                string proprietorID = ch.ChartMasterId.ToString();
                s.LoadData("GENERALCREDITOR");
                ch.LoadData(s.SystemValue);
                string creditorID = ch.ChartMasterId.ToString();
                s.LoadData("DISCOUNTCHARCODE");
                ch.LoadData(s.SystemValue);
                string discountID = ch.ChartMasterId.ToString();

                #endregion


                string sql = "select * from gl_transactions where gl_transaction_chart_id=" + gstid + " and gl_transaction_date>= '2013-12-01'";


                o.StartTransaction();
                DataTable dt = o.ReturnTable(sql, "1");


                foreach (DataRow dr in dt.Rows)
                {
                    string uid = "";
                    string gid = dr["gl_transaction_id"].ToString();
                    string rt = dr["gl_transaction_ref_type_id"].ToString();
                    if (rt.Equals("1"))//Invoice
                    {
                        uid = OutputGstID.ToString();
                    }
                    else if (rt.Equals("2"))//Cinv
                    {
                        uid = InputGstID.ToString();
                    }
                    else if (rt.Equals("6"))
                    {
                        if (dr["gl_transaction_description"].ToString().Equals("Discount Offered"))
                        {
                            uid = OutputGstID.ToString();
                        }
                    }
                    if (!uid.Equals("") && !gid.Equals(""))
                    {
                        string updatesql = "update gl_transactions set gl_transaction_chart_id=" + uid + " where gl_transaction_id=" + gid;
                        o.ExecuteScalar(updatesql);
                    }
                }
                o.Commit();
            }
            catch (Exception ex)
            {
                o.Rollback();
                throw ex;
            }
        }

        protected void Button17_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            DataTable resultDT = new DataTable();

            resultDT.Columns.Add("InvID");
            resultDT.Columns.Add("Description");
            resultDT.Columns.Add("Date");
            {
                DataTable dt = o.ReturnTable("SELECT  * FROM      gl_transactions, receipt_gls, receipts, invoice_gls, invoice_master WHERE   gl_transactions.gl_transaction_id = receipt_gls.receipt_gl_gl_id AND                  receipt_gls.receipt_gl_receipt_id = receipts.receipt_id AND                  gl_transactions.gl_transaction_id = invoice_gls.invoice_gl_gl_id AND                  invoice_gls.invoice_gl_invoice_id = invoice_master.invoice_master_id AND (gl_transactions.gl_transaction_type_id = 3)  ", "t1");

                foreach (DataRow glt in dt.Rows)
                {
                    string rid = glt["receipt_id"].ToString();
                    string iid = glt["invoice_master_id"].ToString();
                    string date = glt["invoice_master_date"].ToString();
                    int c = 0;
                    foreach (DataRow glt2 in dt.Rows)
                    {
                        string rid2 = glt2["receipt_id"].ToString();
                        string iid2 = glt2["invoice_master_id"].ToString();
                        if (rid.Equals(rid2) && iid.Equals(iid2))
                            c++;
                    }
                    if (c > 1)
                    {
                        DataRow newdr = resultDT.NewRow();
                        newdr["InvID"] = iid;
                        newdr["Description"] = "Doublie Allocated";
                        newdr["Date"] = date;
                        resultDT.Rows.Add(newdr);
                    }
                }
            }
            DataTable InvDT = o.ReturnTable("select * from invoice_master", "t1");
            foreach (DataRow invDR in InvDT.Rows)
            {
                string invID = invDR["invoice_master_id"].ToString();
                string date = invDR["invoice_master_date"].ToString();
                string sql = "SELECT   gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_net,                  receipt_gls.receipt_gl_receipt_id, invoice_master.invoice_master_id, invoice_master.invoice_master_num FROM      gl_transactions, invoice_gls, receipt_gls, invoice_master WHERE   gl_transactions.gl_transaction_id = invoice_gls.invoice_gl_gl_id AND                  invoice_gls.invoice_gl_gl_id = receipt_gls.receipt_gl_gl_id AND                  invoice_gls.invoice_gl_invoice_id = invoice_master.invoice_master_id AND (gl_transactions.gl_transaction_type_id = 6)                  AND (invoice_master_id = " + invID + ")";
                DataTable discountDT = o.ReturnTable(sql, "DiscountDT");
                decimal disc = 0;
                if (discountDT.Rows.Count > 0)
                    decimal.TryParse(discountDT.Rows[0]["gl_transaction_net"].ToString(), out disc);
                decimal gross = decimal.Parse(invDR["invoice_master_gross"].ToString());

                decimal paid = 0;
                string paidsql = "SELECT * FROM invoice_gls, gl_transactions WHERE invoice_gls.invoice_gl_gl_id = gl_transactions.gl_transaction_id AND (gl_transactions.gl_transaction_description = 'Receipt') and invoice_gl_invoice_id = " + invID;
                DataTable paidDT = o.ReturnTable(paidsql, "Paid");
                if (paidDT.Rows.Count > 0)
                    decimal.TryParse(paidDT.Rows[0]["gl_transaction_net"].ToString(), out paid);

                if ((paid + disc) > gross)
                {
                    DataRow newdr = resultDT.NewRow();
                    newdr["InvID"] = invID;
                    newdr["Description"] = "Discount + Paid > Gross";
                    newdr["Date"] = date;
                    resultDT.Rows.Add(newdr);
                }


            }
            GridView1.DataSource = resultDT;
            GridView1.DataBind();
        }

        protected void Button18_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            try
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
                s.LoadData("GENERALCREDITOR");
                ch.LoadData(s.SystemValue);
                string creditorID = ch.ChartMasterId.ToString();
                s.LoadData("DISCOUNTCHARCODE");
                ch.LoadData(s.SystemValue);
                string discountID = ch.ChartMasterId.ToString();

                #endregion

                Sapp.Data.Odbc newOdbc = new Sapp.Data.Odbc(AdFunction.conn);
                DataTable accountDT = o.ReturnTable("select * from bodycorps where bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value, "t1");
                string accountid = accountDT.Rows[0]["bodycorp_account_id"].ToString();

                DataTable dt = o.ReturnTable("SELECT  * FROM      gl_transactions where (gl_transaction_type_id=7 or gl_transaction_type_id=8) ", "t1");

                foreach (DataRow glt in dt.Rows)
                {
                    string refe = glt["gl_transaction_ref"].ToString();
                    string type = glt["gl_transaction_type_id"].ToString();
                    string gid = glt["gl_transaction_id"].ToString();
                    string date = glt["gl_transaction_date"].ToString();
                    string sql2 = "SELECT  * FROM      gl_transactions where (gl_transaction_type_id=7 or gl_transaction_type_id=8) and gl_transaction_ref='" + refe + "'";

                    DataTable dt2 = o.ReturnTable(sql2, "t1");
                    if (dt2.Rows.Count == 3)
                    {
                        decimal gross = 0;
                        string grossgid = "";
                        decimal tax = 0;
                        string taxgid = "";
                        decimal net = 0;
                        string netgid = "";
                        foreach (DataRow dr in dt2.Rows)
                        {

                            string chart = dr["gl_transaction_chart_id"].ToString();
                            if (chart.Equals(accountid))
                            {
                                grossgid = dr["gl_transaction_id"].ToString();
                                gross = decimal.Parse(dr["gl_transaction_net"].ToString());
                            }
                            else if (chart.Equals(gstid))
                            {
                                taxgid = dr["gl_transaction_id"].ToString();
                            }
                            else
                            {
                                netgid = dr["gl_transaction_id"].ToString();
                            }
                        }

                        tax = gross * 15 / 115;
                        net = gross - tax;


                        string ut = "update gl_transactions set gl_transaction_net=" + -tax + " where gl_transaction_id=" + taxgid;
                        o.ExecuteScalar(ut);

                        string nt = "update gl_transactions set gl_transaction_net=" + -net + " where gl_transaction_id=" + netgid;
                        o.ExecuteScalar(nt);
                    }
                }

                DataTable dtr = o.ReturnTable("SELECT  * FROM      gl_transactions where (gl_transaction_type_id=7 or gl_transaction_type_id=8) group by `gl_transaction_ref`", "t3");
                foreach (DataRow dr in dtr.Rows)
                {
                    string refe = dr["gl_transaction_ref"].ToString();
                    string type = dr["gl_transaction_type_id"].ToString();
                    string date = dr["gl_transaction_date"].ToString();
                    string num = "";
                    Sapp.SMS.System system = new Sapp.SMS.System(AdFunction.conn);
                    system.SetOdbc(o);
                    string sql = "";
                    if (type.Equals("7"))
                    {



                        num = system.GetNextNumber("CASHDEPOSITPREFIX", "CASHDEPOSITPILOT", "CASHDEPOSITDIGIT", "gl_transactions", "gl_transaction_ref");
                        sql = "update gl_transactions set gl_transaction_ref='" + num + "', gl_transaction_ref_type_id=" + type + ", gl_transaction_createdate='" + date + "' where gl_transaction_ref='" + refe + "'";


                    }
                    else
                    {
                        num = system.GetNextNumber("CASHPAYMENTPREFIX", "CASHPAYMENTPILOT", "CASHPAYMENTDIGIT", "gl_transactions", "gl_transaction_ref");
                        sql = "update gl_transactions set gl_transaction_ref='" + num + "',gl_transaction_description='" + refe + "', gl_transaction_ref_type_id=" + type + ", gl_transaction_createdate='" + date + "' where gl_transaction_ref='" + refe + "'";
                    }
                    o.ExecuteScalar(sql);
                }
                GridView1.DataSource = dt;
                GridView1.DataBind();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void Button19_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            DataTable invDT = o.ReturnTable("select * from invoice_master", "g1");
            foreach (DataRow dr in invDT.Rows)
            {
                try
                {
                    InvoiceMaster im = new InvoiceMaster(AdFunction.conn);
                    im.LoadData(int.Parse(dr["invoice_master_id"].ToString()));
                    im.UpdatePaid();
                }
                catch (Exception ex)
                {
                    MessageL.Text += ex.Message + "INV ID:" + dr["invoice_master_id"].ToString() + "<BR>";
                }
            }


        }

        protected void Button20_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            {
                DataTable invDT = o.ReturnTable("select * from invoice_master GROUP BY  invoice_master_date", "g1");

                foreach (DataRow dr in invDT.Rows)
                {
                    DataTable invBatch = o.ReturnTable("select * from invoice_master order by  invoice_master_batch_id desc", "g2");
                    int batch = 0;
                    int.TryParse(invBatch.Rows[0]["invoice_master_batch_id"].ToString(), out batch);
                    batch++;

                    string date = DateTime.Parse(dr["invoice_master_date"].ToString()).ToString("yyyy/MM/dd");
                    string sql = "select * from invoice_master where invoice_master_date='" + date + "' order by invoice_master_num";
                    DataTable temp = o.ReturnTable(sql, "t1");
                    foreach (DataRow dr2 in temp.Rows)
                    {
                        decimal gross = decimal.Parse(dr2["invoice_master_gross"].ToString());
                        string num = dr2["invoice_master_num"].ToString();
                        if (num.Contains("OB"))
                        {
                            if (num.Contains("-"))
                            {
                                int bb = int.Parse(num.Substring(num.IndexOf("-"), num.Length - num.IndexOf("-")));
                                o.ExecuteScalar("update invoice_master set invoice_master_batch_id=" + (batch + bb).ToString() + " where invoice_master_id=" + dr2["invoice_master_id"].ToString());
                            }
                            else
                                o.ExecuteScalar("update invoice_master set invoice_master_batch_id=" + batch + " where invoice_master_id=" + dr2["invoice_master_id"].ToString());
                        }
                        else if (num.Contains("SPL"))
                        {
                            o.ExecuteScalar("update invoice_master set invoice_master_batch_id=" + (batch + 3).ToString() + " where invoice_master_id=" + dr2["invoice_master_id"].ToString());
                        }
                        else if (num.Contains("OP"))
                        {
                            o.ExecuteScalar("update invoice_master set invoice_master_batch_id=" + (batch + 4).ToString() + " where invoice_master_id=" + dr2["invoice_master_id"].ToString());
                        }
                        else if (gross < 0)
                        {
                            o.ExecuteScalar("update invoice_master set invoice_master_batch_id=" + (batch + 5).ToString() + " where invoice_master_id=" + dr2["invoice_master_id"].ToString());
                        }
                    }
                }
            }
            {
                DataTable invDT = o.ReturnTable("select * from invoice_master where invoice_master_batch_id is null GROUP BY  invoice_master_date", "g1");

                foreach (DataRow dr in invDT.Rows)
                {
                    DataTable invBatch = o.ReturnTable("select * from invoice_master order by  invoice_master_batch_id desc", "g2");
                    int batch = 0;
                    int.TryParse(invBatch.Rows[0]["invoice_master_batch_id"].ToString(), out batch);
                    batch++;

                    string date = DateTime.Parse(dr["invoice_master_date"].ToString()).ToString("yyyy/MM/dd");
                    string sql = "select * from invoice_master where invoice_master_batch_id is null and invoice_master_date='" + date + "' order by invoice_master_num";
                    DataTable temp = o.ReturnTable(sql, "t1");
                    foreach (DataRow dr2 in temp.Rows)
                    {
                        decimal gross = decimal.Parse(dr2["invoice_master_gross"].ToString());
                        string num = dr2["invoice_master_num"].ToString();

                        o.ExecuteScalar("update invoice_master set invoice_master_batch_id=" + (batch).ToString() + " where invoice_master_id=" + dr2["invoice_master_id"].ToString());

                    }
                }
            }

        }

        protected void Button21_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            DataTable cinvDT = o.ReturnTable("select * from cinvoices", "g1");
            foreach (DataRow dr in cinvDT.Rows)
            {
                try
                {
                    Cinvoice im = new Cinvoice(AdFunction.conn);
                    im.SetOdbc(o);
                    im.LoadData(int.Parse(dr["cinvoice_id"].ToString()));
                    im.UpdatePaid();
                }
                catch (Exception ex)
                {
                    MessageL.Text += ex.Message + "CINV ID:" + dr["cinvoice_id"].ToString() + "<BR>";
                }
            }
        }

        protected void Button22_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            DataTable resultDT = new DataTable();

            resultDT.Columns.Add("InvID");
            resultDT.Columns.Add("Description");
            resultDT.Columns.Add("Date");
            DataTable dt = o.ReturnTable("SELECT  * FROM            gl_transactions, cpayments, cpayment_gls, cinvoice_gls, cinvoices WHERE        gl_transactions.gl_transaction_id = cpayment_gls.cpayment_gl_gl_id AND cpayments.cpayment_id = cpayment_gls.cpayment_gl_cpayment_id AND                          gl_transactions.gl_transaction_id = cinvoice_gls.cinvoice_gl_gl_id AND cinvoice_gls.cinvoice_gl_cinvoice_id = cinvoices.cinvoice_id", "t1");
            foreach (DataRow glt in dt.Rows)
            {
                string rid = glt["cpayment_id"].ToString();
                string iid = glt["cinvoice_id"].ToString();
                string date = glt["cinvoice_date"].ToString();
                int c = 0;
                decimal paid = 0;
                decimal gross = decimal.Parse(glt["cinvoice_gross"].ToString());
                decimal cinvpaid = decimal.Parse(glt["cinvoice_paid"].ToString());
                foreach (DataRow glt2 in dt.Rows)
                {
                    string rid2 = glt2["cpayment_id"].ToString();
                    string iid2 = glt2["cinvoice_id"].ToString();
                    if (rid.Equals(rid2) && iid.Equals(iid2))
                    {
                        c++;
                        decimal g = decimal.Parse(glt2["gl_transaction_net"].ToString());
                        paid += g;
                    }
                }
                if (c > 1)
                {
                    DataRow newdr = resultDT.NewRow();
                    newdr["InvID"] = iid;
                    newdr["Description"] = "Doublie Allocated";
                    newdr["Date"] = date;
                    resultDT.Rows.Add(newdr);
                }
                if (paid != cinvpaid)
                {
                    DataRow newdr = resultDT.NewRow();
                    newdr["InvID"] = iid;
                    newdr["Description"] = "Allocated Error";
                    newdr["Date"] = date;
                    resultDT.Rows.Add(newdr);
                }

            }
            GridView1.DataSource = resultDT;
            GridView1.DataBind();
        }

        protected void Button23_Click(object sender, EventArgs e)
        {
            string bid = Request.Cookies["bodycorpid"].Value;
            string filepath = Server.MapPath("~/ReportData/") + bid + "\\";
            Bodycorp bc = new Bodycorp(AdFunction.conn);
            bc.LoadData(int.Parse(bid));

            DateTime endDate = bc.Bodycorp_Close_Off;
            if (endDate.Year > 1000)
            {
                DateTime startDate = endDate.AddYears(-100);
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                ReportProprietorAgedDetail rep = new ReportProprietorAgedDetail(AdFunction.conn, "", new Microsoft.Reporting.WebForms.ReportViewer());
                rep.SetReportInfo(bc.BodycorpId, startDate, endDate);
                DataSet ds = new DataSet("ReportProprietorAgedDetail");
                ds.Tables.Add(rep.Creditor);
                ds.WriteXml(filepath + "ReportProprietorAgedDetail.xml", XmlWriteMode.WriteSchema);
            }
        }

        protected void Button24_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            DataTable dt = o.ReturnTable("select * from debtor_master where debtor_master_code not like '%-%'", "t1");
            foreach (DataRow dr in dt.Rows)
            {
                string code = dr["debtor_master_code"].ToString();
                DataTable dt2 = o.ReturnTable("select * from debtor_master where debtor_master_code like '" + code + "-%'", "t2");
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    string id = dt2.Rows[i]["debtor_master_id"].ToString();
                    string newcode = code + "-" + (i + 1);
                    string update = "update debtor_master set debtor_master_code='" + newcode + "' where debtor_master_id=" + id;
                    o.ExecuteScalar(update);
                }

            }
        }
        public void GLInsert(string oldrefernce, string oldid, string typeid, string reftyprid, string reference, string chartid, string unitid, string description, string net, string date, string createdate, string rev, string rec, string tax = "0", string gross = "0", string creditorID = "null")
        {
            Odbc o = new Odbc(AdFunction.conn);
            string bid = HttpContext.Current.Request.Cookies["bodycorpid"].Value;
            string gl_insert = "insert into gl_transactions (`gl_transaction_oldref`,`gl_trasaction_oldid`,`gl_transaction_type_id`,`gl_transaction_ref_type_id`,`gl_transaction_ref`,`gl_transaction_chart_id`,`gl_transaction_bodycorp_id`,`gl_transaction_unit_id`,`gl_transaction_description`,`gl_transaction_net`,`gl_transaction_tax`,`gl_transaction_gross`,`gl_transaction_date`,`gl_transaction_createdate`,`gl_transaction_rev`,`gl_transaction_rec`,`gl_transaction_creditor_id`)";
            gl_insert += " values (" + QuString(oldrefernce) + "," + QuString(oldid) + "," + typeid + "," + reftyprid + ",'" + reference + "'," + chartid + "," + bid + "," + unitid + ",'" + description + "'," + net + "," + tax + "," + gross + ",'" + date + "','" + createdate + "'," + rev + "," + rec + "," + creditorID + ")";
            o.ExecuteScalar(gl_insert);
            //return o.ExecuteScalar(idsql).ToString();
        }

        public string QuString(string text)
        {
            return "'" + text + "'";
        }


        protected void Button25_Click(object sender, EventArgs e)
        {
            DataTable Creditor = new DataTable();
            Creditor.Columns.Add("Name");
            Creditor.Columns.Add("Unit");
            Creditor.Columns.Add("Code");

            Creditor.Columns.Add("Num");
            Creditor.Columns.Add("Date");
            Creditor.Columns.Add("Invoice", Type.GetType("System.Decimal"));
            Creditor.Columns.Add("Receipt", Type.GetType("System.Decimal"));
            Creditor.Columns.Add("Balance", Type.GetType("System.Decimal"));
            Creditor.Columns.Add("DueDate");
            Creditor.Columns.Add("Description");
            Odbc o = new Odbc(AdFunction.conn);
            string bid = HttpContext.Current.Request.Cookies["bodycorpid"].Value;
            DataTable unitmaster = o.ReturnTable("SELECT        * FROM            bodycorps INNER JOIN                          property_master ON bodycorps.bodycorp_id = property_master.property_master_bodycorp_id INNER JOIN                          unit_master ON property_master.property_master_id = unit_master.unit_master_property_id INNER JOIN                          debtor_master ON unit_master.unit_master_debtor_id = debtor_master.debtor_master_id and bodycorp_id=" + bid, "t1");
            Bodycorp bc = new Bodycorp(AdFunction.conn);
            bc.LoadData(int.Parse(bid));
            foreach (DataRow udr in unitmaster.Rows)
            {
                UnitMaster unit = new UnitMaster(AdFunction.conn);
                unit.LoadData(Convert.ToInt32(udr["unit_master_id"].ToString()));
                DataTable temp = unit.GetActivity(bc.Bodycorp_Close_Off.AddYears(-100), bc.Bodycorp_Close_Off);
                DataTable dt = ReportDT.FilterDT(temp, "DueDate <= #" + bc.Bodycorp_Close_Off.ToString("yyyy-MM-dd") + "# or InvDate is null");
                foreach (DataRow dr in dt.Rows)
                {

                    DataRow newRow = Creditor.NewRow();
                    newRow["Name"] = udr["debtor_master_name"].ToString();
                    newRow["Code"] = udr["debtor_master_code"].ToString();
                    newRow["Unit"] = udr["unit_master_code"].ToString();
                    //RecRow["Name"] = newRow["Name"];
                    //RecRow["Code"] = newRow["Code"];
                    //RecRow["Unit"] = newRow["Unit"];
                    decimal inv = 0;
                    decimal.TryParse(dr["Invoice"].ToString(), out inv);
                    decimal rec = 0;
                    decimal.TryParse(dr["Receipt"].ToString(), out rec);
                    decimal balnace = 0;
                    decimal.TryParse(dr["Balance"].ToString(), out balnace);

                    newRow["Invoice"] = inv;
                    newRow["Receipt"] = rec;
                    newRow["Balance"] = balnace;
                    newRow["Date"] = ReportDT.CDateTime(dr["InvDate"].ToString());
                    newRow["DueDate"] = ReportDT.CDateTime(dr["DueDate"].ToString()); ;
                    newRow["Description"] = dr["Ref"].ToString() + "  " + dr["Description"].ToString();
                    Creditor.Rows.Add(newRow);

                }
                decimal invTotal = ReportDT.SumTotal(Creditor, "Invoice");
                decimal recTotal = ReportDT.SumTotal(Creditor, "Receipt");
                decimal balanceTotal = invTotal - recTotal;

                GLInsert("", "", "9", "9", "Invoice", "278", unit.UnitMasterId.ToString(), "Invoice Opening Balance", invTotal.ToString(), bc.Bodycorp_Close_Off.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"), "0", "0");
                GLInsert("", "", "9", "9", "Receipt", "278", unit.UnitMasterId.ToString(), "Receipt Opening Balance", recTotal.ToString(), bc.Bodycorp_Close_Off.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"), "0", "0");
                GLInsert("", "", "9", "9", "Balance", "278", unit.UnitMasterId.ToString(), "Opening Balance", balanceTotal.ToString(), bc.Bodycorp_Close_Off.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"), "0", "0");


            }
        }

        protected void Button26_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            string sql = "SELECT        unit_master.*, bodycorps.bodycorp_code FROM            unit_master INNER JOIN                          property_master ON unit_master.unit_master_property_id = property_master.property_master_id INNER JOIN                          bodycorps ON property_master.property_master_bodycorp_id = bodycorps.bodycorp_id";
            DataTable dt = o.ReturnTable(sql, "u1");
            foreach (DataRow dr in dt.Rows)
            {
                string bodycode = dr["bodycorp_code"].ToString();
                string dcode = dr["unit_master_code"].ToString();
                string id = dr["unit_master_id"].ToString();
                string updatesql = "update unit_master set unit_master_debtor_code='" + bodycode + "/" + dcode + "' where unit_master_id=" + id;
                o.ExecuteScalar(updatesql);
            }
        }

        protected void Button27_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            string sql = "SELECT ownerships.ownership_start, ownerships.ownership_end, unit_master . * FROM unit_master INNER JOIN ownerships ON unit_master.unit_master_id = ownerships.ownership_unit_id GROUP BY unit_master_id ORDER BY  ownership_start desc ";
            DataTable dt = o.ReturnTable(sql, "u1");
            foreach (DataRow dr in dt.Rows)
            {
                string end = DateTime.Parse(dr["ownership_end"].ToString()).ToString("yyyy-MM-dd");
                string uid = dr["unit_master_id"].ToString();
                string updatesql = "update unit_master set unit_master_begin_date='" + end + "' where unit_master_id=" + uid;
                o.ExecuteScalar(updatesql);

            }
        }

        protected void Button28_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            string sql = "select * from debtor_master";
            DataTable dt = o.ReturnTable(sql, "t");
            foreach (DataRow dr in dt.Rows)
            {
                o.ExecuteScalar("update debtor_master set debtor_master_code='" + Guid.NewGuid().ToString() + "' where debtor_master_id=" + dr["debtor_master_id"].ToString());
            }
            foreach (DataRow dr in dt.Rows)
            {
                string name = dr["debtor_master_name"].ToString();
                string code = "";
                char[] s = name.ToCharArray();
                code = s[0].ToString();
                for (int i = 1; i < s.Length; i++)
                {
                    if (s[i - 1].ToString().Equals(" "))
                    {
                        code += s[i].ToString();
                    }
                }
                code = code + "-" + dr["debtor_master_id"].ToString();

                string update = "update debtor_master set debtor_master_code='" + code + "' where debtor_master_id=" + dr["debtor_master_id"].ToString();
                o.ExecuteScalar(update);
            }
        }

        protected void Button29_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            string sql = "SELECT  * FROM unit_master INNER JOIN ownerships ON unit_master.unit_master_id = ownerships.ownership_unit_id where unit_master_begin_date = ownership_end";
            DataTable dt = o.ReturnTable(sql, "t");
            foreach (DataRow dr in dt.Rows)
            {
                string id = dr["ownership_id"].ToString();
                DateTime bdate = DateTime.Parse(dr["unit_master_begin_date"].ToString());

                string update = "update ownerships set ownership_end='" + bdate.AddDays(-1).ToString("yyyy-MM-dd") + "' where ownership_id=" + id;
                o.ExecuteScalar(update);

            }
        }

        protected void Button30_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            string sql = "SELECT       * FROM            invoice_master INNER JOIN   unit_master ON invoice_master.invoice_master_unit_id = unit_master.unit_master_id where unit_master_type_id=5 or unit_master_inactive_date is not null";
            DataTable dt = o.ReturnTable(sql, "t1");
            foreach (DataRow dr in dt.Rows)
            {
                string invID = dr["invoice_master_id"].ToString();
                InvoiceMaster im = new InvoiceMaster(AdFunction.conn);
                im.LoadData(int.Parse(invID));
                im.Delete();
            }
        }

        protected void Button31_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            o.ExecuteScalar("update `receipts` set `receipt_gross` = -`receipt_gross` where `receipt_type_id`=2");
        }

        protected void Button32_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            DataTable dt = o.ReturnTable("select * from receipts where `receipt_type_id`=2", "t1");
            foreach (DataRow dr in dt.Rows)
            {
                string id = dr["receipt_id"].ToString();
                Receipt r = new Receipt(AdFunction.conn);
                r.LoadData(int.Parse(id));
                Hashtable h = new Hashtable();
                h.Add("receipt_gross", r.ReceiptGross);
                r.Update(h);
            }



        }

        protected void Button33_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            string sql = "update gl_transactions set gl_transaction_recbatchid=21 where gl_transaction_recbatchid=239; ";
            o.ExecuteScalar(sql);
            sql = "update receipts set receipt_recbatchid=21 where receipt_recbatchid=239; "; o.ExecuteScalar(sql);
            sql = "update gl_transactions set gl_transaction_recbatchid=21 where gl_transaction_recbatchid=240;"; o.ExecuteScalar(sql);
            sql = "update receipts  set receipt_recbatchid=21 where receipt_recbatchid=240; "; o.ExecuteScalar(sql);
            sql = "update gl_transactions set gl_transaction_recbatchid=35 where gl_transaction_recbatchid=878;"; o.ExecuteScalar(sql);
            sql = "update receipts  set receipt_recbatchid=35 where receipt_recbatchid=878;"; o.ExecuteScalar(sql);
            sql = "update gl_transactions set gl_transaction_recbatchid=41 where gl_transaction_recbatchid=884;"; o.ExecuteScalar(sql);
            sql = "update receipts  set receipt_recbatchid=41 where receipt_recbatchid=884; "; o.ExecuteScalar(sql);
            sql = "update gl_transactions set gl_transaction_recbatchid=50 where gl_transaction_recbatchid=885;"; o.ExecuteScalar(sql);
            sql = "update receipts  set receipt_recbatchid=50 where receipt_recbatchid=885; "; o.ExecuteScalar(sql);
            sql = "update gl_transactions set gl_transaction_recbatchid=41 where gl_transaction_recbatchid=899;"; o.ExecuteScalar(sql);
            sql = "update receipts  set receipt_recbatchid=41 where receipt_recbatchid=899;"; o.ExecuteScalar(sql);
            sql = "update gl_transactions set gl_transaction_recbatchid=54 where gl_transaction_recbatchid=1455;"; o.ExecuteScalar(sql);
            sql = "update receipts  set receipt_recbatchid=54 where receipt_recbatchid=1455;"; o.ExecuteScalar(sql);
            sql = "update gl_transactions set gl_transaction_recbatchid=54 where gl_transaction_recbatchid=1457;"; o.ExecuteScalar(sql);
            sql = "update receipts  set receipt_recbatchid=54 where receipt_recbatchid=1457;"; o.ExecuteScalar(sql);
            sql = "update gl_transactions set gl_transaction_recbatchid=57 where gl_transaction_recbatchid=1462;"; o.ExecuteScalar(sql);
            sql = "update receipts  set receipt_recbatchid=57 where receipt_recbatchid=1462;"; o.ExecuteScalar(sql);
            o.ExecuteScalar(sql);
        }

        protected void Button34_Click(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            DataTable dt = o.ReturnTable("select * from invoice_master", "t1");
            foreach (DataRow dr in dt.Rows)
            {
                string id = dr["invoice_master_id"].ToString();
                if (id.Equals("7851"))
                {
                }
                string bid = dr["invoice_master_bodycorp_id"].ToString();
                string unitid = dr["invoice_master_unit_id"].ToString();
                if (!unitid.Equals(""))
                {
                    UnitMaster u = new UnitMaster(AdFunction.conn);
                    u.LoadData(int.Parse(unitid));

                    DataTable unitDT = o.ReturnTable("SELECT        * FROM            unit_master INNER JOIN                          property_master ON unit_master.unit_master_property_id = property_master.property_master_id INNER JOIN                          bodycorps ON  property_master.property_master_bodycorp_id = bodycorps.bodycorp_id where  bodycorps.bodycorp_id=" + bid, "t1");
                    string unit_master_id = ReportDT.GetDataByColumn(unitDT, "unit_master_code", u.UnitMasterCode, "unit_master_id");
                    u.LoadData(int.Parse(unit_master_id));

                    string update = "update invoice_master set invoice_master_debtor_id=" + u.UnitMasterDebtorId + ", invoice_master_unit_id=" + unit_master_id + " where invoice_master_id=" + id;
                    o.ExecuteScalar(update);
                }

            }

        }

        protected void Button35_Click(object sender, EventArgs e)
        {
            DataTable dt2 = new DataTable();
            dt2.Columns.Add("ID");
            dt2.Columns.Add("Num");

            dt2.Columns.Add("Type");
            Odbc o = new Odbc(AdFunction.conn);
            {
                DataTable dt = o.ReturnTable("select * from invoice_master", "t1");
                foreach (DataRow dr in dt.Rows)
                {
                    string id = dr["invoice_master_id"].ToString();
                    InvoiceMaster im = new InvoiceMaster(AdFunction.conn);
                    im.LoadData(int.Parse(id));
                    string sql = "SELECT sum(`gl_transaction_net`) FROM `invoice_gls` LEFT JOIN `gl_transactions` ON `invoice_gl_gl_id`=`gl_transaction_id` WHERE gl_transaction_type_id=3 and (`invoice_gl_invoice_id`=" + im.InvoiceMasterId + ")";
                    Hashtable items = new Hashtable();
                    decimal al = 0;
                    decimal.TryParse(o.ReturnTable(sql, "s").Rows[0][0].ToString(), out al);
                    if (System.Math.Abs(al) != im.InvoiceMasterPaid)
                    {
                        DataRow newdr = dt2.NewRow();
                        newdr["ID"] = im.InvoiceMasterId;
                        newdr["Type"] = "INV";
                        newdr["Num"] = im.InvoiceMasterNum;
                        dt2.Rows.Add(newdr);
                    }
                    //im.UpdatePaid();
                }
            }
            {
                DataTable dt = o.ReturnTable("select * from cinvoices", "t1");
                foreach (DataRow dr in dt.Rows)
                {
                    string id = dr["cinvoice_id"].ToString();
                    Cinvoice im = new Cinvoice(AdFunction.conn);
                    im.LoadData(int.Parse(id));
                    string sql = "SELECT sum(`gl_transaction_net`) FROM `cinvoice_gls` LEFT JOIN `gl_transactions` ON `cinvoice_gl_gl_id`=`gl_transaction_id` "
                   + "WHERE `cinvoice_gl_cinvoice_id`=" + im.CinvoiceId + " AND `gl_transaction_type_id`=4";

                    decimal al = 0;
                    decimal.TryParse(o.ReturnTable(sql, "s").Rows[0][0].ToString(), out al);


                    if (System.Math.Abs(al) != im.CinvoicePaid)
                    {

                        DataRow newdr = dt2.NewRow();
                        newdr["ID"] = im.CinvoiceId;
                        newdr["Type"] = "CINV";
                        newdr["Num"] = im.CinvoiceNum;
                        dt2.Rows.Add(newdr);
                    }
                    //im.UpdatePaid();
                }
            }
            GridView1.DataSource = dt2;
            GridView1.DataBind();
        }

        protected void Button36_Click(object sender, EventArgs e)
        {

            Odbc o = new Odbc(AdFunction.conn);
            {
                DataTable dt = o.ReturnTable("select * from invoice_master", "t1");
                foreach (DataRow dr in dt.Rows)
                {
                    string id = dr["invoice_master_id"].ToString();
                    InvoiceMaster im = new InvoiceMaster(AdFunction.conn);
                    im.LoadData(int.Parse(id));
                    string sql = "SELECT sum(`gl_transaction_net`) FROM `invoice_gls` LEFT JOIN `gl_transactions` ON `invoice_gl_gl_id`=`gl_transaction_id` WHERE gl_transaction_type_id=3 and (`invoice_gl_invoice_id`=" + im.InvoiceMasterId + ")";
                    Hashtable items = new Hashtable();
                    decimal al = 0;
                    decimal.TryParse(o.ReturnTable(sql, "s").Rows[0][0].ToString(), out al);
                    if (System.Math.Abs(al) != im.InvoiceMasterPaid)
                    {
                        im.UpdatePaid();
                    }

                }
            }
            {
                DataTable dt = o.ReturnTable("select * from cinvoices", "t1");
                foreach (DataRow dr in dt.Rows)
                {
                    string id = dr["cinvoice_id"].ToString();
                    Cinvoice im = new Cinvoice(AdFunction.conn);
                    im.LoadData(int.Parse(id));
                    string sql = "SELECT sum(`gl_transaction_net`) FROM `cinvoice_gls` LEFT JOIN `gl_transactions` ON `cinvoice_gl_gl_id`=`gl_transaction_id` "
                   + "WHERE `cinvoice_gl_cinvoice_id`=" + im.CinvoiceId + " AND `gl_transaction_type_id`=4";

                    decimal al = 0;
                    decimal.TryParse(o.ReturnTable(sql, "s").Rows[0][0].ToString(), out al);


                    if (System.Math.Abs(al) != im.CinvoicePaid)
                    {
                        im.UpdatePaid();
                        //DataRow newdr = dt2.NewRow();
                        //newdr["ID"] = im.CinvoiceId;
                        //newdr["Type"] = "CINV";
                        //newdr["Num"] = im.CinvoiceNum;
                        //dt2.Rows.Add(newdr);
                    }

                }
            }
        }

        /// <summary>
        /// Clear all GTS records for one body corp.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">EventArgs</param>
        protected void Button37_Click(object sender, EventArgs e)
        {
            // DB connection string
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            // Bodycorp_ID
            String bodycorp_id = HttpContext.Current.Request.Cookies["bodycorpid"].Value;

            Odbc mydb = null;
            try
            {
                mydb = new Odbc(constr);

                #region Load General Creditor Control and General Tax Chart Account
                ChartMaster chart = new ChartMaster(constr);
                Sapp.SMS.System system = new Sapp.SMS.System(constr);

                system.LoadData("GENERALDEBTOR");
                chart.LoadData(system.SystemValue.Split('|')[0]);
                int GENERAL_DEBTOR = chart.ChartMasterId;   // 865

                system.LoadData("GENERALCREDITOR");
                chart.LoadData(system.SystemValue);
                int GENERAL_CREDITOR = chart.ChartMasterId;  // 867

                system.LoadData("GST Output");
                chart.LoadData(system.SystemValue);
                int GENERAL_TAX = chart.ChartMasterId;      // 868

                // system folder
                system.LoadData("FILEFOLDER");
                string filefolder = system.SystemValue;
                #endregion

                #region Get targets
                // Get all target Invoice
                string sql1 = "SELECT invoice_master_id, invoice_master_tax FROM invoice_master WHERE invoice_master_bodycorp_id = " + bodycorp_id + " AND invoice_master_tax <> 0";
                // Get all target Cinvoice
                string sql2 = "SELECT cinvoice_id, cinvoice_tax FROM cinvoices WHERE cinvoice_bodycorp_id = " + bodycorp_id + " AND cinvoice_tax <> 0";
                // Get all Levies
                string sql3 = "SELECT levy_id, levy_tax FROM levies WHERE levy_bodycorp_id = " + bodycorp_id + " AND levy_tax <> 0";
                DataTable invoice_target = mydb.ReturnTable(sql1, "invoice_target");
                DataTable cinvoice_target = mydb.ReturnTable(sql2, "cinvoice_target");
                DataTable levy_target = mydb.ReturnTable(sql3, "levy_target");
                #endregion

                mydb.StartTransaction();

                #region Delete GTS records
                // Delete cinvoice_gls records
                String sql4 = "DELETE FROM cinvoice_gls "
                            + "WHERE cinvoice_gl_gl_id "
                            + "  IN (SELECT gl_transaction_id FROM gl_transactions "
                            + "      WHERE gl_transaction_type_id = 5 "                 // System transaction
                            + "        AND gl_transaction_chart_id = " + GENERAL_TAX    // GST Control
                            + "        AND gl_transaction_bodycorp_id = " + bodycorp_id + ")";
                mydb.NonQuery(sql4);

                // Delete invoice_gls records
                String sql5 = "DELETE FROM invoice_gls "
                            + "WHERE invoice_gl_gl_id "
                            + "  IN (SELECT gl_transaction_id FROM gl_transactions "
                            + "      WHERE gl_transaction_type_id = 5 "                 // System transaction
                            + "        AND gl_transaction_chart_id = " + GENERAL_TAX    // GST Control
                            + "        AND gl_transaction_bodycorp_id = " + bodycorp_id + ")";
                mydb.NonQuery(sql5);

                // Delete gl_transactions records
                String sql6 = "DELETE FROM gl_transactions "
                            + "WHERE gl_transaction_type_id = 5 "                       // System transaction
                            + "  AND gl_transaction_chart_id = " + GENERAL_TAX          // GST Control
                            + "  AND gl_transaction_bodycorp_id = " + bodycorp_id;
                mydb.NonQuery(sql6);
                #endregion

                #region Update tax & gross
                // invoice_master
                String sql7 = "UPDATE invoice_master "
                            + "SET invoice_master_tax = 0, invoice_master_net = invoice_master_gross "
                            + "  WHERE invoice_master_bodycorp_id = " + bodycorp_id
                            + "    AND invoice_master_tax <> 0 ";
                mydb.NonQuery(sql7);

                // cinvoices
                String sql8 = "UPDATE cinvoices "
                            + "SET cinvoice_tax = 0, cinvoice_net = cinvoice_gross"
                            + "  WHERE cinvoice_bodycorp_id = " + bodycorp_id
                            + "    AND cinvoice_tax <> 0 ";
                mydb.NonQuery(sql8);

                // levies
                String sql9 = "UPDATE levies "
                            + "SET levy_tax = 0, levy_net = levy_gross "
                            + "WHERE levy_bodycorp_id = " + bodycorp_id
                            + "  AND levy_tax <> 0 ";
                mydb.NonQuery(sql9);

                // gl_transactions (Cinvoice & Invoice)
                String sql10 = "UPDATE gl_transactions "
                            + "SET gl_transaction_tax = 0, gl_transaction_net = gl_transaction_gross "
                            + "WHERE gl_transaction_bodycorp_id = " + bodycorp_id
                            + "  AND gl_transaction_tax <> 0 "
                            + "  AND gl_transaction_type_id IN (1, 2) "
                            + "  AND gl_transaction_chart_id NOT IN (" + GENERAL_DEBTOR + ", " + GENERAL_CREDITOR + ", " + GENERAL_TAX + ")";
                mydb.NonQuery(sql10);

                // gl_transactions (Creditor Control & Debtor Control)
                String sql11 = "UPDATE gl_transactions AS T0, "
                             + "     (SELECT gl_transaction_ref, SUM(gl_transaction_net) AS new_net "
                             + "     FROM gl_transactions "
                             + "     WHERE gl_transaction_bodycorp_id = " + bodycorp_id
                             + "       AND gl_transaction_type_id IN (1, 2) "
                             + "       AND gl_transaction_chart_id NOT IN (" + GENERAL_DEBTOR + ", " + GENERAL_CREDITOR + ", " + GENERAL_TAX + ") "
                             + "     GROUP By (gl_transaction_ref)) AS T1 "
                             + "SET T0.gl_transaction_net = -T1.new_net "
                             + "WHERE T0.gl_transaction_ref = T1.gl_transaction_ref "
                             + "  AND T0.gl_transaction_bodycorp_id = " + bodycorp_id
                             + "  AND T0.gl_transaction_type_id = 5 "
                             + "  AND T0.gl_transaction_chart_id IN (" + GENERAL_DEBTOR + ", " + GENERAL_CREDITOR + ")";
                mydb.NonQuery(sql11);
                #endregion

                mydb.Commit();

                #region Output CSV file
                // New csv file
                string path = filefolder + "\\GST_remover_" + DateTime.Now.ToString("ddMMyyyy_HHmmssfff") + ".csv";      // In case of mulity request in one minute
                FileStream fs = new FileStream(path, FileMode.Create);
                StreamWriter sw = new StreamWriter(new BufferedStream(fs), System.Text.Encoding.Default);
                DataTableToCsv(invoice_target, sw);
                DataTableToCsv(cinvoice_target, sw);
                DataTableToCsv(levy_target, sw);
                sw.Close();
                fs.Close();

                // Response to the export request
                FileInfo file = new FileInfo(path);
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
                HttpContext.Current.Response.ContentType = "text/plain";
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.TransmitFile(file.FullName);
                HttpContext.Current.Response.End();
                #endregion
            }
            catch (Exception ex)
            {
                if (mydb != null)
                {
                    mydb.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (mydb != null) 
                {
                    mydb.Close();
                }
            }
        }

        /// <summary>
        /// Output CSV file with multi-dataset
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="sw">StreamWriter</param>
        private void DataTableToCsv(DataTable table, StreamWriter sw)
        {
            string title = "";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                title += table.Columns[i].ColumnName + ",";
            }
            title = title.Substring(0, title.Length - 1) + "\r\n";
            sw.Write(title);

            foreach (DataRow row in table.Rows)
            {
                string line = "";
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    line += "\"" + row[i].ToString().Replace("\"", "\"\"") + "\",";
                }
                line = line.Substring(0, line.Length - 1) + "\r\n";
                sw.Write(line);
            }
            sw.WriteLine();
            sw.WriteLine();
        }

        /// <summary>
        /// Fix GTS records for one body corp 18.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">EventArgs</param>
        protected void Button38_Click(object sender, EventArgs e)
        {
            // DB connection string
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            // Bodycorp_ID
            // String bodycorp_id = HttpContext.Current.Request.Cookies["bodycorpid"].Value;

            Odbc mydb = null;
            try
            {
                mydb = new Odbc(constr);
                mydb.StartTransaction();

                // cinvoice master
                String sql1 = " UPDATE "
                            + "   gl_transactions AS T0, "
                            + "   ( SELECT Ta.gl_transaction_id, Tb.gl_transaction_net"
                            + "     FROM "
                            + "         (SELECT  T1.cpayment_id, T1.cpayment_bodycorp_id, T1.cpayment_gross, "
                            + "             T2.cpayment_gl_gl_id, T2.cpayment_gl_paid, T3.gl_transaction_id, "
                            + "             T3.gl_transaction_ref, T3.gl_transaction_ref_type_id, T3.gl_transaction_net, T3.gl_transaction_gross, T3.gl_transaction_chart_id "
                            + "         FROM   cpayments T1, cpayment_gls T2, gl_transactions T3 "
                            + "         WHERE T1.cpayment_id = T2.cpayment_gl_cpayment_id "
                            + "          AND T2.cpayment_gl_gl_id = T3.gl_transaction_id "
                            + "          AND T1.cpayment_bodycorp_id = 18 "
                            + "          AND T3.gl_transaction_type_id = 5 "
                            + "          AND T3.gl_transaction_chart_id = 279 )  AS Ta, "

                            + "         (SELECT  T1.cpayment_id, T1.cpayment_bodycorp_id, T1.cpayment_gross, "
                            + "             T2.cpayment_gl_gl_id, T2.cpayment_gl_paid, T3.gl_transaction_id, "
                            + "             T3.gl_transaction_ref, T3.gl_transaction_ref_type_id, T3.gl_transaction_net, T3.gl_transaction_gross, T3.gl_transaction_chart_id "
                            + "         FROM   cpayments T1, cpayment_gls T2, gl_transactions T3 "
                            + "         WHERE T1.cpayment_id = T2.cpayment_gl_cpayment_id "
                            + "          AND T2.cpayment_gl_gl_id = T3.gl_transaction_id "
                            + "          AND T1.cpayment_bodycorp_id = 18 "
                            + "          AND T3.gl_transaction_type_id = 5 "
                            + "          AND T3.gl_transaction_chart_id = 1190 ) AS Tb "

                            + "    WHERE  Ta.gl_transaction_ref = Tb.gl_transaction_ref "
                            + "     AND Ta.cpayment_id = Tb.cpayment_id "
                            + "     AND Ta.gl_transaction_id <> Tb.gl_transaction_id "
                            + "     AND Ta.gl_transaction_net <> -Tb.gl_transaction_net "
                            + "  ) AS Tx "
                            + " SET T0.gl_transaction_net = -Tx.gl_transaction_net "
                            + " WHERE "
                            + "    T0.gl_transaction_id = Tx.gl_transaction_id ";

                mydb.NonQuery(sql1);

                mydb.Commit();
            }
            catch (Exception ex)
            {
                if (mydb != null)
                {
                    mydb.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (mydb != null)
                {
                    mydb.Close();
                }
            }
        }

    }

}