using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Configuration;
using Sapp.Common;
using Sapp.SMS;
using Sapp.Data;
using System.Collections;

namespace sapp_sms
{
    public partial class pptycntrmasteredit : System.Web.UI.Page
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
            string pptycntrmaster_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["pptycntrmasterid"] != null) pptycntrmaster_id = Request.QueryString["pptycntrmasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                #region Javascript Setup
                Control[] wc = { ComboBoxService, ComboBoxCreditor, ComboBoxProperty };
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
                            AjaxControlUtils.SetupComboBox(ComboBoxService, "SELECT `pptycntr_service_id` AS `ID`, `pptycntr_service_name` AS `Name` FROM `pptycntr_services`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxCreditor, "SELECT `creditor_master_id` AS `ID`, `creditor_master_name` AS `Name` FROM `creditor_master`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxProperty, "SELECT `property_master_id` AS `ID`, `property_master_name` AS `Name` FROM `property_master`", "ID", "Name", constr, false);
                            #endregion
                            #region Initial Web Controls
                            CheckBoxInactive.Checked = false;
                            TextBoxNotes.Text = "";
                            TextBoxExpiry.Text = "";

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
                            AjaxControlUtils.SetupComboBox(ComboBoxService, "SELECT `pptycntr_service_id` AS `ID`, `pptycntr_service_name` AS `Name` FROM `pptycntr_services`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxCreditor, "SELECT `creditor_master_id` AS `ID`, `creditor_master_name` AS `Name` FROM `creditor_master`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxProperty, "SELECT `property_master_id` AS `ID`, `property_master_name` AS `Name` FROM `property_master`", "ID", "Name", constr, false);
                            #endregion
                            #region Load Web Controls

                            PptycntrMaster pptycntrmaster = new PptycntrMaster(constr);
                            pptycntrmaster.LoadData(Convert.ToInt32(pptycntrmaster_id));

                            AjaxControlUtils.ComboBoxSelection(ComboBoxCreditor, pptycntrmaster.PptycntrMasterCreditorId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxService, pptycntrmaster.PptycntrMasterServiceId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxProperty, pptycntrmaster.PptycntrMasterPropertyId, false);

                            CheckBoxInactive.Checked = pptycntrmaster.PptycntrInactive;
                            TextBoxExpiry.Text = DBSafeUtils.DBDateToStr(pptycntrmaster.PptycntrExpiry);
                            TextBoxNotes.Text = pptycntrmaster.PptycntrNotes;
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
                string pptycntrmaster_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable items = new Hashtable();
                if (!Page.IsValid) return;
                if (Request.QueryString["pptycntrmasterid"] != null) pptycntrmaster_id = Request.QueryString["pptycntrmasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                #region Retireve Values
                items.Add("pptycntr_master_service_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxService, false));
                items.Add("pptycntr_master_creditor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxCreditor, false));
                items.Add("pptycntr_master_property_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxProperty, false));
                items.Add("pptycntr_expiry", DBSafeUtils.DateToSQL(TextBoxExpiry.Text));
                items.Add("pptycntr_inactive", DBSafeUtils.BoolToSQL(CheckBoxInactive.Checked));
                items.Add("pptycntr_notes", DBSafeUtils.StrToQuoteSQL(TextBoxNotes.Text));
                #endregion
                if (mode == "add")
                {
                    #region Save
                    PptycntrMaster pptycntrmaster = new PptycntrMaster(constr);
                    pptycntrmaster.Add(items);
                    #endregion
                    //#region Redirect
                    //Response.BufferOutput = true;
                    //Response.Redirect("goback.aspx", false);
                    //#endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    PptycntrMaster pptycntrmaster = new PptycntrMaster(constr);
                    pptycntrmaster.Update(items, Convert.ToInt32(pptycntrmaster_id));
                    #endregion
                    //#region Redirect
                    //Response.BufferOutput = true;
                    //Response.Redirect("goback.aspx", false);
                    ////#endregion
                }
                if (!TextBoxExpiry.Text.Equals(""))
                {
                    if (!TextBoxExpiry.Text.Equals(OldEndHF.Value))
                        Response.Redirect("noticemessage.aspx?propertyid=" + Request.QueryString["propertyid"] + "&date=" + Server.UrlEncode(TextBoxExpiry.Text) + "&title=" + Server.UrlEncode("Contractor Expiry"), false);
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
                Response.Redirect("~/pptycntrmaster.aspx" + NewQueryString("?"),false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            #endregion
        }
        #endregion
        #region Validatio

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



        protected void CustomValidatorService_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxService.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
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

        #endregion
    }
}