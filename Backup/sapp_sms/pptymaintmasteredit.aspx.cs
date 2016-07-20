using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.Common;
using Sapp.SMS;
using Sapp.Data;
using System.Collections;
using System.Text.RegularExpressions;

namespace sapp_sms
{
    public partial class pptymaintmasteredit : System.Web.UI.Page
    {
        private string CheckedQueryString()
        {
            if (null != Request.QueryString["propertyid"] && Regex.IsMatch(Request.QueryString["propertyid"], "^[0-9]*$"))
            {
                return Request.QueryString["propertyid"];
            }
            return "";
        }
        private string NewQueryString(string connector)
        {
            string result = CheckedQueryString();
            return string.IsNullOrEmpty(result) ? "" : connector + "propertyid=" + result;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string pptymaintmaster_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["pptymaintmasterid"] != null) pptymaintmaster_id = Request.QueryString["pptymaintmasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                #region Javascript Setup
                Control[] wc = { ComboBoxType, ComboBoxCredit, ComboBoxFreqType, ComboBoxProperty, ComboBoxUnit };
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
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `pptymaint_type_id` AS `ID`, `pptymaint_type_name` AS `Name` FROM `pptymaint_types`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxCredit, "SELECT `creditor_master_id` AS `ID`, `creditor_master_name` AS `Name` FROM `creditor_master`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxFreqType, "SELECT `freqtype_id` AS `ID`, `freqtype_name` AS `Name` FROM `freqtypes`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxProperty, "SELECT `property_master_id` AS `ID`, `property_master_name` AS `Name` FROM `property_master`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Name` FROM `unit_master`" + " where unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Name", constr, true);
                            #endregion
                            #region Initial Web Controls
                            CheckBoxCompliance.Checked = false;
                            TextBoxDue.Text = "";
                            TextBoxNextDue.Text = "";
                            TextBoxNotes.Text = "";

                            string qsbodycorpid = CheckedQueryString();
                            if (!string.IsNullOrEmpty(qsbodycorpid))
                            {
                                AjaxControlUtils.ComboBoxSelection(ComboBoxProperty, qsbodycorpid, false);
                                ComboBoxProperty.Enabled = false;
                            }
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
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `pptymaint_type_id` AS `ID`, `pptymaint_type_name` AS `Name` FROM `pptymaint_types`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxCredit, "SELECT `creditor_master_id` AS `ID`, `creditor_master_name` AS `Name` FROM `creditor_master`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxFreqType, "SELECT `freqtype_id` AS `ID`, `freqtype_name` AS `Name` FROM `freqtypes`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxProperty, "SELECT `property_master_id` AS `ID`, `property_master_name` AS `Name` FROM `property_master`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Name` FROM `unit_master`" + " where unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Name", constr, true);
                            #endregion
                            #region Load Web Controls

                            PptymaintMaster pptymaintmaster = new PptymaintMaster(constr);
                            pptymaintmaster.LoadData(Convert.ToInt32(pptymaintmaster_id));

                            AjaxControlUtils.ComboBoxSelection(ComboBoxCredit, pptymaintmaster.PptymaintMasterCreditorId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxType, pptymaintmaster.PptymaintMasterTypeId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxFreqType, pptymaintmaster.PptymaintMasterFreqtypeId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxProperty, pptymaintmaster.PptymaintMasterPropertyId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxUnit, pptymaintmaster.PptymaintMasterUnitId, true);

                            CheckBoxCompliance.Checked = pptymaintmaster.PptymaintMasterCompliance;
                            TextBoxDue.Text = DBSafeUtils.DBDateToStr(pptymaintmaster.PptymaintMasterDue);
                            TextBoxNextDue.Text = DBSafeUtils.DBDateToStr(pptymaintmaster.PptymaintMasterNextDue);
                            TextBoxFreq.Text = pptymaintmaster.PptymaintMasterFreq.ToString();
                            TextBoxNotes.Text = pptymaintmaster.PptymaintMasterNotes;
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
                    OldEndHF.Value = TextBoxDue.Text;
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
                string pptymaintmaster_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable items = new Hashtable();
                if (!Page.IsValid) return;
                if (Request.QueryString["pptymaintmasterid"] != null) pptymaintmaster_id = Request.QueryString["pptymaintmasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                #region Retireve Values
                items.Add("pptymaint_master_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxType, false));
                items.Add("pptymaint_master_property_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxProperty, false));
                items.Add("pptymaint_master_creditor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxCredit, false));
                items.Add("pptymaint_master_unit_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxUnit, true));
                items.Add("pptymaint_master_freqtype_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxFreqType, false));
                items.Add("pptymaint_master_compliance", DBSafeUtils.BoolToSQL(CheckBoxCompliance.Checked));
                items.Add("pptymaint_master_notes", DBSafeUtils.StrToQuoteSQL(TextBoxNotes.Text));
                items.Add("pptymaint_master_due", DBSafeUtils.DateToSQL(TextBoxDue.Text));
                items.Add("pptymaint_master_next_due", DBSafeUtils.DateToSQL(TextBoxNextDue.Text));
                items.Add("pptymaint_master_freq", DBSafeUtils.DBStrToStr(TextBoxFreq.Text));
                #endregion
                if (mode == "add")
                {
                    #region Save
                    PptymaintMaster pptymaintmaster = new PptymaintMaster(constr);
                    pptymaintmaster.Add(items);
                    #endregion
                    //#region Redirect
                    //Response.BufferOutput = true;
                    //Response.Redirect("goback.aspx", false);
                    //#endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    PptymaintMaster pptymaintmaster = new PptymaintMaster(constr);
                    pptymaintmaster.Update(items, Convert.ToInt32(pptymaintmaster_id));
                    #endregion
                    //#region Redirect
                    //Response.BufferOutput = true;
                    //Response.Redirect("goback.aspx", false);
                    //#endregion
                }
                if (!TextBoxDue.Text.Equals(""))
                {
                    if (!TextBoxDue.Text.Equals(OldEndHF.Value))
                        Response.Redirect("noticemessage.aspx?propertyid=" + Request.QueryString["propertyid"] + "&date=" + Server.UrlEncode(TextBoxDue.Text) + "&title=" + Server.UrlEncode("Maintenance Due"), false);
                }
                else
                {
                    Response.Redirect("goback.aspx", false);
                }
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
                Response.Redirect("~/pptymaintmaster.aspx" + NewQueryString("?"),false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            #endregion
        }
        #endregion
        #region Validation



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

        protected void CustomValidatorProperty_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxProperty.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void CustomValidatorCredit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxCredit.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void CustomValidatorFreq_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxFreqType.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
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