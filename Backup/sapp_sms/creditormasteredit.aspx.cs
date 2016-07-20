using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using System.Configuration;
using Sapp.SMS;
using System.Collections;
using Sapp.Data;

namespace sapp_sms
{
    public partial class creditormasteredit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string creditor_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["creditorid"] != null) creditor_id = Request.QueryString["creditorid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                if (!IsPostBack)
                {
                    if (mode == "add")
                    {
                        #region Load Page
                        try
                        {
                            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxPaymentTerm, "SELECT `payment_term_id` AS `ID`, `payment_term_code` AS `Code` FROM `Payment_terms`", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxPaymentType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", constr, true);
                            #endregion
                            #region Initial Web Controls
                            TextBoxBankAC.Text = "";
                            TextBoxCode.Text = "";
                            TextBoxGST.Text = "";
                            TextBoxName.Text = "";
                            TextBoxNotes.Text = "";
                            TextBoxPayee.Text = "";
                            TextBoxref.Text = "";
                            TextBoxSalutation.Text = "";
                            CheckBoxNotax.Checked = false;
                            TextBoxCntrType.Text = "";
                            TextBoxSrvArea.Text = "";
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
                            AjaxControlUtils.SetupComboBox(ComboBoxPaymentTerm, "SELECT `payment_term_id` AS `ID`, `payment_term_code` AS `Code` FROM `Payment_terms`", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxPaymentType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", constr, true);
                            #endregion

                            #region Load Web Controls
                            CreditorMaster creditor = new CreditorMaster(constr);
                            creditor.LoadData(Convert.ToInt32(creditor_id));
                            AjaxControlUtils.ComboBoxSelection(ComboBoxPaymentTerm, creditor.CreditorMasterPaymentTermId, true);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxPaymentType, creditor.CreditorMasterPaymentTypeId, true);
                            TextBoxBankAC.Text = creditor.CreditorMasterBankAc;
                            TextBoxCode.Text = creditor.CreditorMasterCode;
                            TextBoxGST.Text = creditor.CreditorMasterGst;
                            TextBoxName.Text = creditor.CreditorMasterName;
                            TextBoxNotes.Text = creditor.CreditorMasterNotes;
                            TextBoxPayee.Text = creditor.CreditorMasterPayee;
                            TextBoxref.Text = creditor.CreditorMasterRef;
                            TextBoxSalutation.Text = creditor.CreditorMasterSalutation;
                            CheckBoxNotax.Checked = creditor.CreditorMasterNotax;
                            TextBoxCntrType.Text = creditor.CreditorMasterCntrtype;
                            TextBoxSrvArea.Text = creditor.CreditorMasterSrvarea;
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
                string creditor_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (!Page.IsValid) return;
                if (Request.QueryString["creditorid"] != null) creditor_id = Request.QueryString["creditorid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];

                #region Retireve Values
                
                Hashtable items = new Hashtable();

                items.Add("creditor_master_code", DBSafeUtils.StrToQuoteSQL(TextBoxCode.Text));
                items.Add("creditor_master_name", DBSafeUtils.StrToQuoteSQL(TextBoxName.Text));
                items.Add("creditor_master_gst", DBSafeUtils.StrToQuoteSQL(TextBoxGST.Text));
                items.Add("creditor_master_payee", DBSafeUtils.StrToQuoteSQL(TextBoxPayee.Text));

                items.Add("creditor_master_payment_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxPaymentType, true));
                items.Add("creditor_master_payment_term_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxPaymentTerm, true));

                items.Add("creditor_master_ref", DBSafeUtils.StrToQuoteSQL(TextBoxref.Text));
                items.Add("creditor_master_bank_ac", DBSafeUtils.StrToQuoteSQL(TextBoxBankAC.Text));
                items.Add("creditor_master_salutation", DBSafeUtils.StrToQuoteSQL(TextBoxSalutation.Text));
                items.Add("creditor_master_notax", DBSafeUtils.BoolToSQL(CheckBoxNotax.Checked));
                items.Add("creditor_master_notes", DBSafeUtils.StrToQuoteSQL(TextBoxNotes.Text));
                items.Add("creditor_master_cntrtype", DBSafeUtils.StrToQuoteSQL(TextBoxCntrType.Text));
                items.Add("creditor_master_srvarea", DBSafeUtils.StrToQuoteSQL(TextBoxSrvArea.Text));

                #endregion
                if (mode == "add")
                {
                    #region Save
                    CreditorMaster creditor = new CreditorMaster(constr);
                    creditor.Add(items);
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    CreditorMaster creditor = new CreditorMaster(constr);
                    creditor.Update(items, Convert.ToInt32(creditor_id));
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
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
                Response.Redirect("~/creditormaster.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

    }
}