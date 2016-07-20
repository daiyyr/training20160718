//070713
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Sapp.Common;
using Sapp.SMS;
using Sapp.JQuery;
using Sapp.Data;
using System.Data;
using System.Diagnostics;

namespace sapp_sms
{
    public partial class receipteditMR : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string bodycorpid = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Odbc odbc = new Odbc(AdFunction.conn);

                Page.Title = "Sapp SMS - Receipt Edit Multi Unit";
                if (Request.Cookies["bodycorpid"].Value != null) bodycorpid = Request.Cookies["bodycorpid"].Value;
                if (!IsPostBack)
                {


                    #region Load Page
                    try
                    {
                        if (!File.Exists(Server.MapPath("~/temp/MulReceipt.xml")))
                        {
                            ImageButton1.Enabled = false;
                        }
                        AjaxControlUtils.SetupComboBox(ComboBoxPaymentType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", constr, false);
                        DataTable dt = AdFunction.UnitComboxDT(false);
                        dt.Columns.Add("OutStanding");
                        DataTable template = new DataTable();
                        template.Columns.Add("Bodycorp");
                        template.Columns.Add("Unit");
                        template.Columns.Add("Amount");
                        template.Columns.Add("AA");
                        template.Columns.Add("Type");
                        template.Columns.Add("Date");
                        template.Columns.Add("Note");

                        foreach (DataRow dr in dt.Rows)
                        {
                            //DataRow tdr = template.NewRow();
                            //tdr["Bodycorp"] = dr["bodycorp_code"].ToString();
                            //tdr["Unit"] = dr["unit_master_code"].ToString();
                            //template.Rows.Add(tdr);
                            string unitid = dr["unit_master_id"].ToString();

                            string sql = "select sum(invoice_master_gross)-sum(invoice_master_paid) from invoice_master where invoice_master_unit_id=" + unitid;
                            dr["OutStanding"] = odbc.ReturnTable(sql, "r1").Rows[0][0].ToString();
                            if (dr["OutStanding"].ToString().Equals(""))
                            {
                                dr["OutStanding"] = "0";
                            }
                        }
                        CsvDT.DataTableToCsv(template, Server.MapPath("~/Temp/RMTemplate.csv"));
                        //GridView1.DataSource = dt;
                        //GridView1.DataBind();
                        //if (GridView1.Rows.Count > 0)
                        //{
                        //    GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
                        //    GridView1.FooterRow.TableSection = TableRowSection.TableFooter;
                        //}




                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    #endregion
                }



            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }

        }

        #region WebControl Events
        protected void ImageButtonNext_Click(object sender, ImageClickEventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            try
            {

                o.StartTransaction();
                #region check
                decimal amount = decimal.Parse(TextBoxGross.Text);
                decimal total = 0;
                foreach (GridViewRow gv in GridView1.Rows)
                {
                    decimal gross = 0;

                    TextBox tb = (TextBox)gv.Cells[4].FindControl("TextBox1");
                    decimal.TryParse(tb.Text, out gross);
                    total += gross;
                }
                if (total != amount)
                {
                    throw new Exception("Amount not equal");
                }
                #endregion

                //Odbc o = new Odbc(AdFunction.conn);
                string firstID = "";
                DataTable nextreceipt = new DataTable("MuReceipt");
                nextreceipt.Columns.Add("ID");
                nextreceipt.Columns.Add("Ref");
                nextreceipt.Columns.Add("Unit");
                nextreceipt.Columns.Add("Allocated");
                foreach (GridViewRow gv in GridView1.Rows)
                {
                    TextBox tb = (TextBox)gv.Cells[4].FindControl("TextBox1");
                    CheckBox cb = (CheckBox)gv.Cells[5].FindControl("CheckBox1");
                    decimal gross = 0;
                    decimal.TryParse(tb.Text, out gross);
                    if (gross != 0)
                    {
                        string uid = tb.Attributes["IID"];
                        string invSQL = "select * from invoice_master where invoice_master_gross>invoice_master_paid and invoice_master_type_id=1 and invoice_master_unit_id=" + uid;
                        DataTable invDT = o.ReturnTable(invSQL, "invoice");

                        string unitID = uid;
                        string bodycorpid = invDT.Rows[0]["invoice_master_bodycorp_id"].ToString();
                        string debtorID = invDT.Rows[0]["invoice_master_debtor_id"].ToString();
                        Receipt r = new Receipt(AdFunction.conn);

                        string rref = r.GetNextReceiptNumber();


                        Hashtable items = new Hashtable();
                        items.Add("receipt_ref", DBSafeUtils.StrToQuoteSQL(rref));
                        items.Add("receipt_debtor_id", debtorID);
                        items.Add("receipt_bodycorp_id", bodycorpid);
                        items.Add("receipt_unit_id", unitID);
                        items.Add("receipt_payment_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxPaymentType, false));
                        Bodycorp bodycorp = new Bodycorp(AdFunction.conn);
                        bodycorp.SetOdbc(o);
                        bodycorp.LoadData(int.Parse(bodycorpid));
                        items.Add("receipt_account_id", bodycorp.BodycorpAccountId);
                        items.Add("receipt_gross", gross);
                        items.Add("receipt_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                        items.Add("receipt_notes", DBSafeUtils.StrToQuoteSQL(TextBoxNotes.Text));
                        r.Add(items);
                        if (firstID.Equals(""))
                            firstID = r.ReceiptId.ToString();
                        DataRow dr = nextreceipt.NewRow();
                        dr["ID"] = r.ReceiptId.ToString();
                        dr["Ref"] = r.ReceiptRef;
                        dr["Unit"] = o.ReturnTable("select * from unit_master where unit_master_id=" + uid, "u1").Rows[0]["unit_master_code"].ToString() + " - " + r.ReceiptRef + " - " + r.ReceiptGross;
                        dr["Allocated"] = "No";
                        nextreceipt.Rows.Add(dr);
                        if (cb.Checked)
                            foreach (DataRow invDR in invDT.Rows)
                            {
                                string invid = invDR["invoice_master_id"].ToString();
                                Hashtable items2 = new Hashtable();
                                items2.Add("invoice_master_id", invid);
                                decimal paid = decimal.Parse(invDR["invoice_master_gross"].ToString()) - decimal.Parse(invDR["invoice_master_paid"].ToString());
                                if (gross > 0)
                                {
                                    if (gross >= paid)
                                    {
                                        gross -= paid;
                                        items2.Add("gl_transaction_net", paid);
                                    }
                                    else
                                    {
                                        items2.Add("gl_transaction_net", gross);
                                        gross = 0;
                                    }
                                    r.AddGlTran(items2, true);
                                }

                            }
                    }
                }
                //Session["NextReceipt"] = nextreceipt;
                DataSet ds = new DataSet("mureceipt");
                ds.Tables.Add(nextreceipt);
                ds.WriteXml(Server.MapPath("~/temp/MulReceipt.xml"));
                o.Commit();
                Response.Redirect("~/receiptallocate.aspx?mu=yes&receiptid=" + firstID, false);
            }
            catch (Exception ex)
            {
                o.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ComboBoxPaymentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxPaymentType.SelectedItem.Text == "CHEQUE")
            {
                LiteralChequeNum.Visible = true;
                TextBoxChequeNum.Visible = true;
                LiteralBank.Visible = true;
                TextBoxBank.Visible = true;
                LiteralBranch.Visible = true;
                TextBoxBranch.Visible = true;
            }
            else
            {
                LiteralChequeNum.Visible = false;
                TextBoxChequeNum.Visible = false;
                LiteralBank.Visible = false;
                TextBoxBank.Visible = false;
                LiteralBranch.Visible = false;
                TextBoxBranch.Visible = false;
            }
        }
        #endregion
        #region Validations

        protected void CustomValidatorPaymentType_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxPaymentType.SelectedIndex > -1) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #endregion

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                ds.ReadXml(Server.MapPath("~/temp/MulReceipt.xml"));
                dt = ds.Tables[0];
                string fristID = dt.Rows[0]["ID"].ToString();
                Response.Redirect("~/receiptallocate.aspx?mu=yes&receiptid=" + fristID, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void UploadB_Click(object sender, EventArgs e)
        {

            Odbc o = new Odbc(AdFunction.conn);
            o.StartTransaction();
            try
            {

                FileUpload1.SaveAs(Server.MapPath("~/Temp/RM.csv"));
                DataTable dt = CsvDT.CsvToDataTable(Server.MapPath("~/Temp/"), "RM.csv");
                string firstID = "";
                DataTable nextreceipt = new DataTable("MuReceipt");
                nextreceipt.Columns.Add("ID");
                nextreceipt.Columns.Add("Ref");
                nextreceipt.Columns.Add("Unit");
                nextreceipt.Columns.Add("Allocated");
                foreach (DataRow cdr in dt.Rows)
                {
                    string Bodycorp = cdr["Bodycorp"].ToString();
                    string Unit = cdr["Unit"].ToString();
                    string Amount = cdr["Amount"].ToString().Replace("$", "");
                    string AA = cdr["AA"].ToString();
                    decimal check = 0;
                    decimal.TryParse(Amount, out check);

                    if (check != 0)
                    {
                        DateTime date = DateTime.Parse(cdr["Date"].ToString());
                        string Note = cdr["Note"].ToString();
                        string type = cdr["Type"].ToString();

                        DataTable unitDT = AdFunction.UnitComboxDT(false, false);
                        unitDT = ReportDT.FilterDT(unitDT, "bodycorp_code='" + Bodycorp + "' and unit_master_code='" + Unit + "'");
                        if (unitDT.Rows.Count > 0)
                        {
                            string uid = unitDT.Rows[0][0].ToString();
                            string bodycorpid = unitDT.Rows[0]["bodycorp_id"].ToString();
                            string debtorID = unitDT.Rows[0]["unit_master_debtor_id"].ToString();
                            decimal gross = 0;
                            decimal.TryParse(Amount, out gross);
                            if (gross != 0)
                            {
                                //Stopwatch stopwatch = new Stopwatch();
                                //stopwatch.Start();
                                string invSQL = "select * from invoice_master where invoice_master_gross>invoice_master_paid and invoice_master_type_id=1 and invoice_master_unit_id=" + uid;
                                DataTable invDT = o.ReturnTable(invSQL, "invoice");

                                string unitID = uid;


                                Receipt r = new Receipt(AdFunction.conn);
                                r.SetOdbc(o);

                                string rref = r.GetNextReceiptNumber();


                                Hashtable items = new Hashtable();
                                items.Add("receipt_ref", DBSafeUtils.StrToQuoteSQL(rref));
                                items.Add("receipt_debtor_id", debtorID);
                                items.Add("receipt_bodycorp_id", bodycorpid);
                                items.Add("receipt_unit_id", unitID);
                                string typeid = "1";
                                if (type.ToUpper().Equals("CHEQUE"))
                                    type = "5";
                                if (type.ToUpper().Equals("CASH"))
                                    type = "6";
                                items.Add("receipt_payment_type_id", typeid);
                                Bodycorp bodycorp = new Bodycorp(AdFunction.conn);
                                //bodycorp.SetOdbc(o);
                                bodycorp.LoadData(int.Parse(bodycorpid));
                                items.Add("receipt_account_id", bodycorp.BodycorpAccountId);
                                items.Add("receipt_gross", gross);
                                items.Add("receipt_date", DBSafeUtils.DateToSQL(date.ToString("yyyy-MM-dd")));
                                items.Add("receipt_notes", DBSafeUtils.StrToQuoteSQL(Note));
                                r.Add(items);
                                if (firstID.Equals(""))
                                    firstID = r.ReceiptId.ToString();
                                DataRow dr = nextreceipt.NewRow();
                                dr["ID"] = r.ReceiptId.ToString();
                                dr["Ref"] = r.ReceiptRef;
                                dr["Unit"] = o.ReturnTable("select * from unit_master where unit_master_id=" + uid, "u1").Rows[0]["unit_master_code"].ToString() + " - " + r.ReceiptRef + " - " + r.ReceiptGross;
                                dr["Allocated"] = "No";
                                nextreceipt.Rows.Add(dr);
                                if (AA.Equals("Yes") || AA.Equals("y") || AA.Equals("Y"))
                                    if (invDT.Rows.Count > 0)
                                        foreach (DataRow invDR in invDT.Rows)
                                        {
                                            string invid = invDR["invoice_master_id"].ToString();
                                            Hashtable items2 = new Hashtable();
                                            items2.Add("invoice_master_id", invid);
                                            decimal paid = decimal.Parse(invDR["invoice_master_gross"].ToString()) - decimal.Parse(invDR["invoice_master_paid"].ToString());
                                            if (gross > 0)
                                            {
                                                if (gross >= paid)
                                                {
                                                    gross -= paid;
                                                    items2.Add("gl_transaction_net", paid);
                                                }
                                                else
                                                {
                                                    items2.Add("gl_transaction_net", gross);
                                                    gross = 0;
                                                }
                                                r.AddGlTran(items2, true);
                                            }
                                        }
                                //TimeSpan timespan = stopwatch.Elapsed;
                                //double seconds = timespan.TotalSeconds;
                                //Response.Write(seconds.ToString() + "<br>");

                            }
                        }
                        else
                        {
                            throw new Exception("Unit not found: Bodycorp" + Bodycorp + " Unit:" + Unit);
                        }
                    }


                }
                DataSet ds = new DataSet("mureceipt");
                ds.Tables.Add(nextreceipt);
                ds.WriteXml(Server.MapPath("~/temp/MulReceipt.xml"));
                o.Commit();
                Response.Redirect("~/receiptallocate.aspx?mu=yes&receiptid=" + firstID, false);
            }
            catch (Exception ex)
            {
                o.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButton3_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/Temp/RMTemplate.csv");
        }




    }
}