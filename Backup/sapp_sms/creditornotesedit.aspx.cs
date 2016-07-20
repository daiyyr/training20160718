using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Configuration;
using Sapp.SMS;
using Sapp.JQuery;
using System.IO;
using Sapp.Data;
using Sapp.Common;

namespace sapp_sms
{
    public partial class creditornotesedit : System.Web.UI.Page, IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Javascript setup
            Control[] wc = { jqGridTrans, TextBoxNet, TextBoxTax, TextBoxGross };
            JSUtils.RenderJSArrayWithCliendIds(Page, wc);
            #endregion
            string cinvoice_id = "";
            string mode = "";

            try
            {
                if (Request.QueryString["invoicemasterid"] != null) cinvoice_id = Request.QueryString["invoicemasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                #region Javascript Setup
                #endregion
                if (!IsPostBack)
                {
                    if (mode == "add")
                    {
                        #region Load Page
                        try
                        {
                            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxDebtor, "SELECT `debtor_master_id` AS `ID`, `debtor_master_name` AS `Code` FROM `debtor_master`", "ID", "Code", constr, false);

                            AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` LEFT JOIN `property_master` ON property_master_id=unit_master_property_id where property_master_bodycorp_id = " + Request.Cookies["bodycorpid"].Value + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Code", constr, true);
                            #endregion

                            #region Initial Web Controls
                            TextBoxDescription.Text = "";
                            InvoiceMaster i = new InvoiceMaster(constr);
                            TextBoxNum.Text = i.GetNextInvoiceNumber();
                            TextBoxDue.Text = "";
                            TextBoxBatchID.Text = "";
                            TextBoxPaid.Text = "";
                            TextBoxGross.Text = "";
                            TextBoxNet.Text = "";
                            TextBoxTax.Text = "";
                            TextBoxDate.Text = "";
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
                    if (mode == "edit")
                    {
                        #region Load Page
                        try
                        {
                            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxDebtor, "SELECT `debtor_master_id` AS `ID`, `debtor_master_name` AS `Code` FROM `debtor_master`", "ID", "Code", constr, false);

                            AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` LEFT JOIN `property_master` ON property_master_id=unit_master_property_id where property_master_bodycorp_id = " + Request.Cookies["bodycorpid"].Value + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Code", constr, true);
                            #endregion

                            #region Initial Web Controls
                            InvoiceMaster invoice = new InvoiceMaster(constr);
                            invoice.LoadData(Convert.ToInt32(cinvoice_id));
                            TextBoxDescription.Text = invoice.InvoiceMasterDescription;
                            TextBoxNum.Text = invoice.InvoiceMasterNum;
                            if (invoice.InvoiceMasterDue.HasValue)
                                TextBoxDue.Text = invoice.InvoiceMasterDue.Value.ToShortDateString();
                            TextBoxBatchID.Text = invoice.InvoiceMasterBatchId.ToString();
                            TextBoxDate.Text = invoice.InvoiceMasterDate.ToShortDateString();
                            TextBoxGross.Text = invoice.InvoiceMasterGross.ToString();
                            TextBoxNet.Text = invoice.InvoiceMasterNet.ToString();
                            TextBoxTax.Text = invoice.InvoiceMasterTax.ToString();
                            TextBoxDate.Text = invoice.InvoiceMasterDate.ToShortDateString();
                            TextBoxPaid.Text = invoice.InvoiceMasterPaid.ToString();

                            AjaxControlUtils.ComboBoxSelection(ComboBoxDebtor, invoice.InvoiceMasterDebtorId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxBodycorp, invoice.InvoiceMasterBodycorpId, true);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxUnit, invoice.InvoiceMasterUnitId, true);
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
                    if ("XMLHttpRequest" != Request.Headers["X-Requested-With"])
                    {
                        Session["invoiceid"] = Request.QueryString["invoicemasterid"];
                    }
                    if (Request.Cookies["bodycorpid"] != null)
                    {
                        ComboBoxBodycorp.SelectedValue = Request.Cookies["bodycorpid"].Value;
                        ComboBoxBodycorp.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            try
            {
                string[] args = eventArgument.Split('|');
                if (args[0] == "ImageButtonCommand")
                {
                    ImageButtonCommand_Click(args);
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
            try
            {
                string invoice_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable items = new Hashtable();
                if (!Page.IsValid) return;
                if (Request.QueryString["invoicemasterid"] != null) invoice_id = Request.QueryString["invoicemasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");

                decimal sumNet = 0;

                #region Retireve Values

                items.Add("invoice_master_type_id", 2);
                items.Add("invoice_master_debtor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxDebtor, false));
                items.Add("invoice_master_unit_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxUnit, true));
                items.Add("invoice_master_bodycorp_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxBodycorp, true));
                items.Add("invoice_master_num", DBSafeUtils.StrToQuoteSQL(TextBoxNum.Text));
                items.Add("invoice_master_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                items.Add("invoice_master_due", DBSafeUtils.DateToSQL(TextBoxDue.Text));
                items.Add("invoice_master_net", DBSafeUtils.DecimalToSQL(sumNet));
                items.Add("invoice_master_tax", DBSafeUtils.DecimalToSQL(sumNet * (decimal)0.15));
                items.Add("invoice_master_gross", DBSafeUtils.DecimalToSQL(sumNet * (decimal)1.15));
                items.Add("invoice_master_paid", DBSafeUtils.DecimalToSQL(TextBoxPaid.Text));
                items.Add("invoice_master_batch_id", DBSafeUtils.IntToSQL(TextBoxBatchID.Text));
                items.Add("invoice_master_description", DBSafeUtils.StrToQuoteSQL(TextBoxDescription.Text));

                #endregion
                if (mode == "add")
                {
                    #region Save
                    InvoiceMaster invoice = new InvoiceMaster(constr);
                    invoice.Add(items);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    InvoiceMaster invoice = new InvoiceMaster(constr);
                    invoice.Update(items, Convert.ToInt32(invoice_id));
                    #endregion
                }

                #region Redirect
                Response.BufferOutput = true;
                Response.Redirect("goback.aspx", false);
                #endregion
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            #region Redirect
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/creditornotes.aspx",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            #endregion
        }
        #endregion

        #region Validations
        protected void CustomValidatorDebtor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxDebtor.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #endregion


        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string sqlfromstr = "FROM( `gl_transactions` left join `invoice_gls` on `gl_transaction_id`=`invoice_gl_gl_id` left join `chart_master` on `chart_master_id`=`gl_transaction_chart_id`) where `gl_transaction_gross`>`invoice_gl_paid` or `invoice_gl_paid` is null";
                string sqlselectstr = "SELECT `ID`,`Chart`,`Description`,`Net`,`Tax`,`Gross` FROM (SELECT `gl_transaction_id` AS `ID`, `Chart_master_code` AS `Chart`, `gl_transaction_description` AS `Description`,`gl_transaction_net` AS `Net`,`gl_transaction_tax` AS `Tax`,`gl_transaction_gross` AS `Gross`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        #endregion

        private void ImageButtonCommand_Click(string[] args)
        {
            try
            {
                string[] glidList = args[1].Split(',');
                foreach (string s in glidList)
                {
                    LabelSelectedRows.Text += s + "--";
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

    }
}