using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.Common;
using Sapp.SMS;
using System.Collections;
using Sapp.Data;
using System.Text.RegularExpressions;
using System.Data;
namespace sapp_sms
{
    public partial class ProprietorNew : System.Web.UI.Page
    {
        private string CheckedQueryString()
        {
            if (null != Request.Cookies["bodycorpid"].Value && Regex.IsMatch(Request.Cookies["bodycorpid"].Value, "^[0-9]*$"))
            {
                return Request.Cookies["bodycorpid"].Value;
            }
            return "";
        }
        private string NewQueryString(string connector)
        {
            string result = CheckedQueryString();
            return string.IsNullOrEmpty(result) ? "" : connector + "bodycorpid=" + result;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string debtor_id = "";
            string mode = "";
            string uid = "";
            try
            {

                if (Request.QueryString["unitmasterid"] != null)
                {
                    uid = Request.QueryString["unitmasterid"];
                    string did = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "unit_master"), "unit_master_id", uid, "unit_master_debtor_id");
                    DataTable dDT = ReportDT.getTable(constr, "debtor_master");
                    string dcode = ReportDT.GetDataByColumn(dDT, "debtor_master_id", did, "debtor_master_code");
                    // Update 27/04/2016
                    /*
                    string code = "";
                    for (int i = 1; i < 100; i++)
                    {
                        code = dcode + "-" + i;
                        if (ReportDT.GetDataByColumn(dDT, "debtor_master_code", code, "debtor_master_id").Equals(""))
                        {
                            break;
                        }
                    }
                    TextBoxCode.Text = code;
                    */
                }
                if (Request.QueryString["debtorid"] != null) debtor_id = Request.QueryString["debtorid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                if (!IsPostBack)
                {
                    if (mode == "add")
                    {
                        #region Load Page
                        try
                        {
                            TextBoxCode.Text = Guid.NewGuid().ToString();
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxPaymentTerm, "SELECT `payment_term_id` AS `ID`, `payment_term_code` AS `Code` FROM `Payment_terms`", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxPaymentType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `debtor_type_id` AS `ID`, `debtor_type_code` AS `Code` FROM `Debtor_types`", "ID", "Code", constr, false);
                            #endregion
                            #region Initial Web Controls
                            CheckBoxEmail.Checked = false;
                            CheckBoxPrint.Checked = false;
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
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxPaymentTerm, "SELECT `payment_term_id` AS `ID`, `payment_term_code` AS `Code` FROM `Payment_terms`", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxPaymentType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `debtor_type_id` AS `ID`, `debtor_type_code` AS `Code` FROM `Debtor_types`", "ID", "Code", constr, false);
                            #endregion

                            #region Load Web Controls
                            DebtorMaster debtor = new DebtorMaster(constr);
                            debtor.LoadData(Convert.ToInt32(debtor_id));
                            AjaxControlUtils.ComboBoxSelection(ComboBoxPaymentTerm, debtor.DebtorMasterPaymentTermId, true);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxPaymentType, debtor.DebtorMasterPaymentTypeId, true);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxType, debtor.DebtorMasterTypeId, false);
                            TextBoxCode.Text = debtor.DebtorMasterCode;
                            TextBoxName.Text = debtor.DebtorMasterName;
                            TextBoxNotes.Text = debtor.DebtorMasterNotes;
                            TextBoxSalutation.Text = debtor.DebtorMasterSalutation;
                            CheckBoxEmail.Checked = debtor.DebtorMasterEmail ?? false;
                            CheckBoxPrint.Checked = debtor.DebtorMasterPrint ?? false;
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
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
                string debtor_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (!Page.IsValid) return;
                if (Request.QueryString["debtorid"] != null) debtor_id = Request.QueryString["debtorid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];

                #region Retireve Values

                Hashtable items = new Hashtable();

                items.Add("debtor_master_code", DBSafeUtils.StrToQuoteSQL(TextBoxCode.Text));
                items.Add("debtor_master_name", DBSafeUtils.StrToQuoteSQL(TextBoxName.Text));

                items.Add("debtor_master_payment_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxPaymentType, true));
                items.Add("debtor_master_payment_term_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxPaymentTerm, true));
                items.Add("debtor_master_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxType, false));
                items.Add("debtor_master_bodycorp_id", Request.Cookies["bodycorpid"].Value);
                items.Add("debtor_master_salutation", DBSafeUtils.StrToQuoteSQL(TextBoxSalutation.Text));
                items.Add("debtor_master_email", DBSafeUtils.BoolToSQL(CheckBoxEmail.Checked));
                items.Add("debtor_master_print", DBSafeUtils.BoolToSQL(CheckBoxPrint.Checked));
                items.Add("debtor_master_notes", DBSafeUtils.StrToQuoteSQL(TextBoxNotes.Text));

                #endregion
                if (mode == "add")
                {
                    #region Save
                    DebtorMaster debtor = new DebtorMaster(constr);
                    debtor.Add(items);

                    // Add Start 28/04/2016
                    string debtorCode = null;
                    string[] proprietorName = TextBoxName.Text.Split(' ');
                    for (int i = 0; i < proprietorName.Length; i++)
                    {
                        debtorCode = debtorCode + proprietorName[i].Substring(0, 1).ToUpper();
                    }
                    int new_debtor_id = debtor.GetIDByCode(TextBoxCode.Text);
                    debtorCode = debtorCode + "-" + new_debtor_id;

                    items = new Hashtable();
                    items.Add("debtor_master_code", DBSafeUtils.StrToQuoteSQL(debtorCode));
                    debtor.Update(items, new_debtor_id);
                    // Add End 28/04/2016

                    Response.BufferOutput = true;
                    string id = Request.QueryString["unitmasterid"].ToString();
                    Response.Redirect("ownershiptransfer.aspx?unitmasterid=" + id + "&debtormasterid=" + debtorCode, false);    // Update 28/04/2016
                    #endregion
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/ownershiptransfer.aspx?unitmasterid=" + Request.QueryString["unitmasterid"].ToString(), false);    // Update 28/04/2016
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion


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

        protected void CustomValidatorPaymentTerm_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxPaymentTerm.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
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
                if (ComboBoxPaymentType.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
    }
}