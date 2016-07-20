using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using System.Configuration;
using Sapp.SMS;
using Sapp.Data;
using System.Collections;
using System.Text.RegularExpressions;

namespace sapp_sms
{
    public partial class pptyvtmasteredit : System.Web.UI.Page
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
            string pptyvtmaster_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["pptyvtmasterid"] != null) pptyvtmaster_id = Request.QueryString["pptyvtmasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                #region Javascript Setup
                Control[] wc = { ComboBoxProperty };
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
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `pptyvt_type_id` AS `ID`, `pptyvt_type_code` AS `Code` FROM `pptyvt_types`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxProperty, "SELECT `property_master_id` AS `ID`, `property_master_code` AS `Code` FROM `property_master`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxValuer, "SELECT `contact_master_id` AS `ID`, `contact_master_name` AS `Name` FROM `contact_master`", "ID", "Name", constr, false);
                            #endregion
                            #region Initial Web Controls
                            TextBoxDate.Text = "";
                            TextBoxDemolition.Text = "";
                            TextBoxFee.Text = "";
                            TextBoxGST.Text = "";
                            TextBoxInflation.Text = "";
                            TextBoxReplacement.Text = "";
                            TextBoxReinstatement.Text = "";
                            TextBoxRef.Text = "";
                            TextBoxNotes.Text = "";

                            string propertyid = CheckedQueryString();
                            if (!string.IsNullOrEmpty(propertyid))
                            {
                                AjaxControlUtils.ComboBoxSelection(ComboBoxProperty, propertyid, false);
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
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `pptyvt_type_id` AS `ID`, `pptyvt_type_code` AS `Code` FROM `pptyvt_types`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxProperty, "SELECT `property_master_id` AS `ID`, `property_master_code` AS `Code` FROM `property_master`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxValuer, "SELECT `contact_master_id` AS `ID`, `contact_master_name` AS `Name` FROM `contact_master`", "ID", "Name", constr, false);
                            #endregion
                            #region Load Web Controls
                            PptyvtMaster pptyvtmaster = new PptyvtMaster(constr);
                            pptyvtmaster.LoadData(Convert.ToInt32(pptyvtmaster_id));

                            AjaxControlUtils.ComboBoxSelection(ComboBoxType, pptyvtmaster.PptyvtMasterTypeId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxProperty, pptyvtmaster.PptyvtMasterPropertyId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxValuer, pptyvtmaster.PptyvtMasterValuerId, false);
                            if (pptyvtmaster.PptyvtMasterDate.HasValue)
                            {
                                TextBoxDate.Text = pptyvtmaster.PptyvtMasterDate.Value.ToShortDateString();
                            }
                            TextBoxDemolition.Text =pptyvtmaster.PptyvtMasterDemolition.ToString();
                            TextBoxFee.Text = pptyvtmaster.PptyvtMasterFee.ToString();
                            TextBoxGST.Text = pptyvtmaster.PptyvtMasterGst;
                            TextBoxInflation.Text = pptyvtmaster.PptyvtMasterInflation.ToString();
                            TextBoxReplacement.Text = pptyvtmaster.PptyvtMasterReplacement.ToString();
                            TextBoxReinstatement.Text = pptyvtmaster.PptyvtMasterReinstatement.ToString();
                            TextBoxRef.Text = pptyvtmaster.PptyvtMasterRef.ToString();
                            TextBoxNotes.Text = DBSafeUtils.DBStrToStr(pptyvtmaster.PptyvtMasterNotes);
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
                string pptyvtmaster_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable items = new Hashtable();
                if (!Page.IsValid) return;
                if (Request.QueryString["pptyvtmasterid"] != null) pptyvtmaster_id = Request.QueryString["pptyvtmasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                #region Retireve Values
                items.Add("pptyvt_master_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxType, false));
                items.Add("pptyvt_master_property_id",AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxProperty,false));
                items.Add("pptyvt_master_date",DBSafeUtils.DateToSQL(TextBoxDate.Text));
                items.Add("pptyvt_master_ref",DBSafeUtils.StrToQuoteSQL(TextBoxRef.Text));
                items.Add("pptyvt_master_valuer_id",AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxValuer,false));
                items.Add("pptyvt_master_reinstatement",DBSafeUtils.DecimalToSQL(TextBoxReinstatement.Text));
                items.Add("pptyvt_master_inflation",DBSafeUtils.DecimalToSQL(TextBoxInflation.Text));
                items.Add("pptyvt_master_demolition",DBSafeUtils.DecimalToSQL(TextBoxDemolition.Text));
                items.Add("pptyvt_master_replacement",DBSafeUtils.DecimalToSQL(TextBoxReplacement.Text));
                items.Add("pptyvt_master_fee",DBSafeUtils.DecimalToSQL(TextBoxFee.Text));
                items.Add("pptyvt_master_gst", DBSafeUtils.StrToQuoteSQL(TextBoxGST.Text));
                items.Add("pptyvt_master_notes", DBSafeUtils.StrToQuoteSQL(TextBoxNotes.Text));

                #endregion
                if (mode == "add")
                {
                    #region Save
                    PptyvtMaster pptyvtmaster = new PptyvtMaster(constr);
                    pptyvtmaster.Add(items);
                    #endregion
                    #region Redirect
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    PptyvtMaster pptyvtmaster = new PptyvtMaster(constr);
                    pptyvtmaster.Update(items, Convert.ToInt32(pptyvtmaster_id));
                    #endregion
                    #region Redirect
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
            #region Redirect
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/pptyvtmaster.aspx" + NewQueryString("?"),false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            #endregion
        }
        #endregion

        protected void CustomValidatorValuer_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxValuer.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
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
    }
}