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

namespace sapp_sms
{
    public partial class receipteditM : System.Web.UI.Page
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
                        AjaxControlUtils.SetupComboBox(ComboBoxPaymentType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", constr, false);
                        string sql = "SELECT `ID`,`Num`,`Debtor`,`Bodycorp`,`Unit`, `Date`, `Due`, `Gross`, `Paid`  FROM (SELECT `invoice_master_batch_id`, `invoice_master_id` as `ID`,`invoice_master_num` as `Num`,`debtor_master_name` AS `Debtor`,`bodycorp_code` AS `Bodycorp`,`unit_master_code` AS `Unit`, `invoice_master_date` AS `Date`, `invoice_master_due` AS `Due`, `invoice_master_gross` AS `Gross`, `invoice_master_paid` AS `Paid`  FROM ((((`invoice_master` LEFT JOIN `invoice_types`  ON `invoice_master_type_id`=`invoice_type_id`) LEFT JOIN `debtor_master` ON `debtor_master_id`=`invoice_master_debtor_id`) LEFT JOIN `bodycorps` ON `invoice_master_bodycorp_id`=`bodycorp_id`) LEFT JOIN `unit_master` ON `unit_master_id`=`invoice_master_unit_id`) WHERE invoice_master_gross<>invoice_master_paid and `invoice_master_type_id`=1) as T1 order by `Date`";
                        DataTable dt = odbc.ReturnTable(sql, "t1");
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                        if (GridView1.Rows.Count > 0)
                        {
                            GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
                        }




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
            try
            {
                Odbc o = new Odbc(AdFunction.conn);

                #region check
                decimal amount = decimal.Parse(TextBoxGross.Text);
                decimal total = 0;
                foreach (GridViewRow gv in GridView1.Rows)
                {
                    decimal gross = 0;

                    TextBox tb = (TextBox)gv.Cells[8].FindControl("TextBox1");
                    decimal.TryParse(tb.Text, out gross);
                    total += gross;
                }
                if (total != amount)
                {
                    throw new Exception("Amount not equal");
                }
                #endregion

                foreach (GridViewRow gv in GridView1.Rows)
                {
                    TextBox tb = (TextBox)gv.Cells[8].FindControl("TextBox1");
                    decimal gross = 0;
                    decimal.TryParse(tb.Text, out gross);
                    if (gross != 0)
                    {
                        string iid = tb.Attributes["IID"];
                        string invSQL = "select * from invoice_master where invoice_master_id=" + iid;
                        DataTable invDT = o.ReturnTable(invSQL, "invoice");
                        string debtorID = invDT.Rows[0]["invoice_master_debtor_id"].ToString();
                        string unitID = invDT.Rows[0]["invoice_master_unit_id"].ToString();
                        string bodycorpid = invDT.Rows[0]["invoice_master_bodycorp_id"].ToString();
                        Receipt r = new Receipt(AdFunction.conn);
                        r.SetOdbc(o);
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
                        Hashtable items2 = new Hashtable();
                        items2.Add("invoice_master_id", iid);
                        items2.Add("gl_transaction_net", gross);
                        r.AddGlTran(items2, true);
                    }
                }
                Response.Redirect("~/receipts.aspx", false);
            }
            catch (Exception ex)
            {
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




    }
}