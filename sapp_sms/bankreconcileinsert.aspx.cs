using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Text;
using Sapp.Common;
using Sapp.SMS;
using Sapp.JQuery;
using System.Data;
using Sapp.Data;
using Sapp.SMS;
using Sapp.General;
using System.Data.Odbc;
namespace sapp_sms
{
    public partial class bankreconcileinsert : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["bankreconcileinsert_unitselected"] = null;
                Session["bankreconcileinsert_debtorselected"] = null;

            }
        }
        #region WebControl Events
        protected void ButtonInsert_Click(object sender, EventArgs e)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string account_id = "";
                if (Request.QueryString["accountid"] != null)
                    account_id = Request.QueryString["accountid"].ToString();
                if (account_id != "")
                {
                    if (RadioButtonListInsert.SelectedValue == "Receipt")
                    {

                        #region Initial ComboBox

                        AjaxControlUtils.SetupComboBox(ComboBoxDebtor, AdFunction.DebtorComboxDT(), "debtor_master_id", "debtor_master_name", false);
                        AjaxControlUtils.SetupComboBox(ComboBoxUnit, AdFunction.UnitComboxDT(), "unit_master_id", "unit_master_code", false);
                        AjaxControlUtils.SetupComboBox(ComboBoxPaymentType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", constr, false);

                        #endregion
                        #region Initial Web Controls
                        TextBoxRef.Text = "";
                        Receipt receipt = new Receipt(constr);
                        TextBoxRef.Text = receipt.GetNextReceiptNumber();
                        TextBoxGross.Text = "";
                        TextBoxDate.Text = "";
                        #endregion
                        PanelStart.Visible = false;
                        PanelReceipt.Visible = true;
                        PanelPayment.Visible = false;
                    }
                    if (RadioButtonListInsert.SelectedValue == "Payment")
                    {
                        #region Initial ComboBox
                        AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", constr, false);
                        AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, true);
                        AjaxControlUtils.SetupComboBox(ComboBoxCreditor, "SELECT `creditor_master_id` AS `ID`, `creditor_master_code` AS `Code` FROM `creditor_master`", "ID", "Code", constr, false);
                        #endregion
                        #region Initial Web Controls
                        TextBoxReference.Text = "";
                        TextBoxDate.Text = DateTime.Now.ToShortDateString();
                        TextBoxGross.Text = "0";
                        #endregion
                        PanelStart.Visible = false;
                        PanelReceipt.Visible = false;
                        PanelPayment.Visible = true;
                    }
                    if (RadioButtonListInsert.SelectedValue == "Cash Deposit")
                    {
                        Receipt receipt = new Receipt(constr);
                        TextBoxCashDepositRef.Text = receipt.GetNextReceiptNumber();
                        PanelStart.Visible = false;
                        PanelCashDeposit.Visible = true;
                        Odbc o = new Odbc(constr);
                        AjaxControlUtils.SetupComboBox(ComboBoxCashDepositBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, false);
                        string sql = "SELECT `chart_master_id` AS `ID`, `chart_master_code` AS `Code`, chart_master_name as `Name` FROM `chart_master` WHERE   (chart_master_type_id = 2) ";
                        DataTable dt = o.ReturnTable(sql, "c");
                        foreach (DataRow dr in dt.Rows)
                        {
                            ListItem i = new ListItem(dr["Code"].ToString() + " | " + dr["Name"].ToString(), dr["ID"].ToString());
                            ChartCombobox.Items.Add(i);
                        }

                    }
                    if (RadioButtonListInsert.SelectedValue == "Cash Payment")
                    {
                        PanelStart.Visible = false;
                        PanelCashCashPayment.Visible = true;
                        Odbc o = new Odbc(constr);
                        AjaxControlUtils.SetupComboBox(ComboBoxCashPaymentBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, false);
                        string sql = "SELECT `chart_master_id` AS `ID`, `chart_master_code` AS `Code`, chart_master_name as `Name` FROM `chart_master`WHERE   (chart_master_type_id = 2)  ";
                        DataTable dt = o.ReturnTable(sql, "c");
                        foreach (DataRow dr in dt.Rows)
                        {
                            ListItem i = new ListItem(dr["Code"].ToString() + " | " + dr["Name"].ToString(), dr["ID"].ToString());
                            ChartCashPaymentCombobox.Items.Add(i);
                        }

                    }

                }

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
                RequiredFieldValidatorRef.Enabled = true;
                CustomValidatorDebtor.Enabled = true;
                CustomValidatorUnit.Enabled = true;
                CustomValidatorPaymentType.Enabled = true;
                CustomValidatorCreditor.Enabled = false;
                CustomValidatorType.Enabled = false;
                RequiredFieldValidator1.Enabled = false;
            }
            else
            {
                LiteralChequeNum.Visible = false;
                TextBoxChequeNum.Visible = false;
                LiteralBank.Visible = false;
                TextBoxBank.Visible = false;
                LiteralBranch.Visible = false;
                TextBoxBranch.Visible = false;

                RequiredFieldValidatorRef.Enabled = false;
                CustomValidatorDebtor.Enabled = false;
                CustomValidatorUnit.Enabled = false;
                CustomValidatorPaymentType.Enabled = false;

                CustomValidatorCreditor.Enabled = true;
                CustomValidatorType.Enabled = true;
                RequiredFieldValidator1.Enabled = true;
            }
        }

        protected void ComboBoxDebtor_SelectedIndexChanged(object sender, EventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string pid = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "property_master"), "property_master_bodycorp_id", ComboBoxBodycorp.SelectedValue, "property_master_id");
            AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` WHERE `unit_master_debtor_id`=" + ComboBoxDebtor.SelectedValue + " and unit_master_inactive_date is null and unit_master_type_id <>5 order by LENGTH(Code), Code", "ID", "Code", constr, false);

        }

        protected void ComboBoxUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ComboBoxUnit.SelectedValue.Equals("null"))
            {
                Odbc o = new Odbc(AdFunction.conn);
                DataTable dt = o.ReturnTable("SELECT * FROM `unit_master` where unit_master_id=" + ComboBoxUnit.SelectedValue, "y1");
                string did = dt.Rows[0]["unit_master_debtor_id"].ToString();
                ComboBoxDebtor.SelectedValue = did;
            }
        }



        protected void ButtonReceipt_Click(object sender, EventArgs e)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                #region Retireve Values
                Hashtable items = new Hashtable();
                items.Add("receipt_ref", DBSafeUtils.StrToQuoteSQL(TextBoxRef.Text));
                items.Add("receipt_debtor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxDebtor, false));


                int? unit_id = AjaxControlUtils.ComboBoxSelectedValue(ComboBoxUnit, true);

                items.Add("receipt_bodycorp_id", AdFunction.BodyCorpID);
                items.Add("receipt_unit_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxUnit, true));
                items.Add("receipt_payment_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxPaymentType, false));
                Bodycorp bodycorp = new Bodycorp(constr);
                bodycorp.LoadData(int.Parse(AdFunction.BodyCorpID));
                items.Add("receipt_account_id", bodycorp.BodycorpAccountId);
                items.Add("receipt_notes", "'" + TextBoxNotes.Text + "'");
                items.Add("receipt_gross", Convert.ToDecimal(TextBoxGross.Text));
                items.Add("receipt_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                if (TextBoxChequeNum.Visible == true)
                {
                    items.Add("receipt_notes", DBSafeUtils.StrToQuoteSQL("Cheque Number: " + TextBoxChequeNum.Text + "; Bank: " + TextBoxBank.Text + "; Branch: " + TextBoxBranch.Text));
                }
                #endregion
                #region Add
                Receipt receipt = new Receipt(constr);
                receipt.Add(items);
                #endregion
                #region Close
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.returnValue = 'refresh'; window.close();", true);
                #endregion
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ButtonPayment_Click(object sender, EventArgs e)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (!Page.IsValid) return;
                Hashtable items = new Hashtable();
                #region Retireve Values
                items.Add("cpayment_bodycorp_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxBodycorp, false));
                items.Add("cpayment_creditor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxCreditor, false));
                items.Add("cpayment_reference", DBSafeUtils.StrToQuoteSQL(TextBoxReference.Text));
                items.Add("cpayment_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxType, false));
                items.Add("cpayment_gross", DBSafeUtils.DecimalToSQL(TextBoxPymtGross.Text));
                items.Add("cpayment_date", DBSafeUtils.DateToSQL(TextBoxPymtDate.Text));
                #endregion
                #region Add
                CPayment cpayment = new CPayment(constr);
                cpayment.Add(items);
                #endregion
                #region Close
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.returnValue = 'refresh'; window.close();", true);
                #endregion

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
        #region Validations
        protected void CustomValidatorDebtor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxDebtor.SelectedIndex > -1) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
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
        protected void CustomValidatorUnit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxDebtor.SelectedIndex > -1) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void CustomValidatorCreditor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxCreditor.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void CustomValidatorType_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxType.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void ButtonCashDeposit_Click(object sender, EventArgs e)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                decimal taxr = decimal.Parse(ReportDT.GetDataByColumn(ReportDT.getTable(constr, "system"), "system_code", "GST", "system_value"));
                Odbc o = new Odbc(constr);
                string bid = ComboBoxCashDepositBodycorp.SelectedValue;
                string date = DBSafeUtils.DateTimeToSQL(TextBoxCashDepositDate.Text);
                decimal amount = decimal.Parse(TextBoxCashDepositGross.Text);
                decimal gst = amount * taxr;
                decimal net = amount - gst;
                string chartgstCode = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "system"), "system_code", "GENERALTAX", "system_value");
                string chartgstID = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "chart_master"), "chart_master_code", chartgstCode, "chart_master_id");
                string reference = TextBoxCashDepositRef.Text;
                string accountid = Request.QueryString["accountid"].ToString();
                string sql = "INSERT INTO gl_transactions                          (gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross,                           gl_transaction_date) VALUES        (7, '" + reference + "', " + accountid + ", " + bid + ", " + amount + ", 0, 0, " + date + ")";
                string gstsql = "INSERT INTO gl_transactions                          (gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross,                           gl_transaction_date) VALUES        (7, '" + reference + "', " + chartgstID + ", " + bid + ", " + -gst + ", 0, 0, " + date + ")";
                string charsql = "INSERT INTO gl_transactions                          (gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross,                           gl_transaction_date) VALUES        (7, '" + reference + "', " + ChartCombobox.SelectedValue + ", " + bid + ", " + -net + ", 0, 0, " + date + ")";
                o.ReturnTable(sql, "amount");
                o.ReturnTable(gstsql, "gst");
                o.ReturnTable(charsql, "amount");
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.returnValue = 'refresh'; window.close();", true);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }

        }

        protected void ButtonCashPayment_Click(object sender, EventArgs e)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                decimal taxr = decimal.Parse(ReportDT.GetDataByColumn(ReportDT.getTable(constr, "system"), "system_code", "GST", "system_value"));
                Odbc o = new Odbc(constr);
                string bid = ComboBoxCashPaymentBodycorp.SelectedValue;
                string date = DBSafeUtils.DateTimeToSQL(TextBoxCashPaymentDate.Text);
                decimal amount = decimal.Parse(TextBoxCashPaymentGross.Text);
                decimal gst = amount * taxr;
                decimal net = amount - gst;
                string chartgstCode = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "system"), "system_code", "GENERALTAX", "system_value");
                string chartgstID = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "chart_master"), "chart_master_code", chartgstCode, "chart_master_id");
                string reference = TextBoxCashPaymentRef.Text;
                string accountid = Request.QueryString["accountid"].ToString();
                string sql = "INSERT INTO gl_transactions                          (gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross,                           gl_transaction_date) VALUES        (8, '" + reference + "', " + accountid + ", " + bid + ", " + -amount + ", 0, 0, " + date + ")";
                string gstsql = "INSERT INTO gl_transactions                          (gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross,                           gl_transaction_date) VALUES        (8, '" + reference + "', " + chartgstID + ", " + bid + ", " + gst + ", 0 , 0, " + date + ")";
                string charsql = "INSERT INTO gl_transactions                          (gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross,                           gl_transaction_date) VALUES        (8, '" + reference + "', " + ChartCashPaymentCombobox.SelectedValue + ", " + bid + ", " + net + ", 0, 0, " + date + ")";
                o.ReturnTable(sql, "amount");
                o.ReturnTable(gstsql, "gst");
                o.ReturnTable(charsql, "amount");
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.returnValue = 'refresh'; window.close();", true);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }



        [System.Web.Services.WebMethod]
        public static string jqGridUnpaidDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);
            GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, "1", constr_general, constr);
            gltranTemps.LoadData();

            DataTable dt = new DataTable("gl_temp_transactions");
            dt.Columns.Add("ID");
            dt.Columns.Add("CinvoiceNum");
            dt.Columns.Add("Description");
            dt.Columns.Add("Date");
            dt.Columns.Add("DueDate");
            dt.Columns.Add("Net");
            dt.Columns.Add("Tax");
            dt.Columns.Add("Gross");
            dt.Columns.Add("Due");
            foreach (GlTransactionTemp gltran in gltranTemps.GLTempList)
            {
                Hashtable items = gltran.GetData();
                DataRow dr = dt.NewRow();
                if (items["gl_transaction_id"].ToString()[0] != 'd')
                {
                    dr["ID"] = items["gl_transaction_id"].ToString();
                    dr["CinvoiceNum"] = items["gl_transaction_ref"].ToString();
                    dr["Description"] = items["gl_transaction_description"].ToString();
                    dr["Date"] = items["gl_transaction_date"].ToString();
                    dr["DueDate"] = items["gl_transaction_duedate"].ToString();
                    dr["Net"] = items["gl_transaction_net"].ToString();
                    dr["Tax"] = items["gl_transaction_tax"].ToString();
                    dr["Gross"] = items["gl_transaction_gross"].ToString();
                    dr["Due"] = items["gl_transaction_due"].ToString();
                    dt.Rows.Add(dr);
                }
            }
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`, `CinvoiceNum`, `Description`, `Date`, `DueDate`, `Net`, `Tax`,`Gross`, `Due` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }

        protected void ComboBoxCreditor_SelectedIndexChanged(object sender, EventArgs e)
        {
            Odbc odbc = new Odbc(AdFunction.conn);
            string sql = "SELECT * FROM `cinvoices` "
                           + "WHERE (`cinvoice_gross` > `cinvoice_paid`)  "
    + "AND (`cinvoice_bodycorp_id`=" + Request.Cookies["bodycorpid"].Value + ") and cinvoice_creditor_id=" + ComboBoxCreditor.SelectedValue;
            DataTable dt = odbc.ReturnTable(sql, "f1");
            DataRow dr = dt.NewRow();
            dr["cinvoice_num"] = "Total";
            dr["cinvoice_gross"] = ReportDT.SumTotal(dt, "cinvoice_gross");
            dr["cinvoice_paid"] = ReportDT.SumTotal(dt, "cinvoice_paid");
            dt.Rows.Add(dr);
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }






    }
}