using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using System.Configuration;
using System.Collections;
using Sapp.SMS;
using Sapp.Data;

namespace sapp_sms
{
    public partial class utilitymasteredit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string utilitymaster_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["utilitymasterid"] != null) utilitymaster_id = Request.QueryString["utilitymasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                #region Javascript Setup
                Control[] wc = { ComboBoxUnit, ComboBoxType };
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
                            AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master`  where unit_master_property_id = " + Request.Cookies["bodycorpid"].Value + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `utility_type_id` AS `ID`, `utility_type_code` AS `Code` FROM `utility_types`", "ID", "Code", constr, false);
                            #endregion
                            #region Initial Web Controls
                            TextBoxDate.Text = "";
                            TextBoxBatch.Text = "";
                            TextBoxReading.Text = "";
                            TextBoxUnitPrice.Text = "";
                            TextBoxNotes.Text = "";
                            
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
                            AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` where unit_master_property_id = " + Request.Cookies["bodycorpid"].Value + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `utility_type_id` AS `ID`, `utility_type_code` AS `Code` FROM `utility_types`", "ID", "Code", constr, false);
                            #endregion
                            #region Initial Web Controls
                            UtilityMaster utilitymaster = new UtilityMaster(constr);
                            utilitymaster.LoadData(Convert.ToInt32(utilitymaster_id));
                            TextBoxDate.Text = utilitymaster.UtilityMasterDate.ToShortDateString();
                            TextBoxBatch.Text = utilitymaster.UtilityMasterBatchId.ToString();
                            TextBoxReading.Text = utilitymaster.UtilityMasterReading.ToString();
                            TextBoxUnitPrice.Text = utilitymaster.UtilityMasterUnitPrice.ToString();
                            TextBoxNotes.Text = utilitymaster.UtilityMasterNotes;
                            AjaxControlUtils.ComboBoxSelection(ComboBoxType, utilitymaster.UtilityMasterTypeId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxUnit, utilitymaster.UtilityMasterUnitId, false);
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
                string utilitymaster_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable items = new Hashtable();
                if (!Page.IsValid) return;
                if (Request.QueryString["utilitymasterid"] != null) utilitymaster_id = Request.QueryString["utilitymasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                #region Retireve Values
                items.Add("utility_master_type_id",AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxType,false));
                items.Add("utility_master_unit_id",AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxUnit,false));
                items.Add("utility_master_reading",DBSafeUtils.IntToSQL(TextBoxReading.Text));
                items.Add("utility_master_date",DBSafeUtils.DateToSQL(TextBoxDate.Text));
                items.Add("utility_master_batch_id",DBSafeUtils.IntToSQL(TextBoxBatch.Text));
                items.Add("utility_master_unit_price",DBSafeUtils.DecimalToSQL(TextBoxUnitPrice.Text));
                items.Add("utility_master_notes",DBSafeUtils.StrToQuoteSQL(TextBoxNotes.Text));
                #endregion
                if (mode == "add")
                {
                    #region Save
                    UtilityMaster utilitymaster = new UtilityMaster(constr);
                    utilitymaster.Add(items);
                    #endregion
                    #region Redirect
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    UtilityMaster utilitymaster = new UtilityMaster(constr);
                    utilitymaster.Update(items, Convert.ToInt32(utilitymaster_id));
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
                Response.Redirect("~/utilitymaster.aspx");
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            #endregion
        }
        #endregion

        protected void CustomValidatorUnit_ServerValidate(object source, ServerValidateEventArgs args)
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
    }
}