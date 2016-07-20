using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.SMS;
using Sapp.Data;
using Sapp.Common;
using System.Collections;
using System.Configuration;
using System.Text.RegularExpressions;

namespace sapp_sms
{
    public partial class propertymasteredit : System.Web.UI.Page
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

            string property_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["propertyid"] != null) property_id = Request.QueryString["propertyid"];
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
                            AjaxControlUtils.SetupComboBox(ComboBoxBodycorp,
                                "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps` LEFT JOIN `property_master` ON `bodycorp_id`=`property_master_bodycorp_id` WHERE `property_master_id` IS NULL", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `property_type_id` AS `ID`, `property_type_code` AS `Code` FROM `property_types`", "ID", "Code", constr, false);
                            #endregion
                            #region Initial Web Controls
                            TextBoxCode.Text = "";
                            TextBoxName.Text = "";
                            TextBoxTotalSqm.Text = "";
                            TextBoxNumOfUnits.Text = "";
                            TextBoxNotes.Text = "";
                            string qsbodycorpid = CheckedQueryString();
                            if (!string.IsNullOrEmpty(qsbodycorpid))
                            {
                                AjaxControlUtils.ComboBoxSelection(ComboBoxBodycorp, qsbodycorpid, false);
                                ComboBoxBodycorp.Enabled = false;
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
                            PropertyMaster property = new PropertyMaster(constr);
                            property.LoadData(Convert.ToInt32(property_id));
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxBodycorp,
                                "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps` LEFT JOIN `property_master` ON `bodycorp_id`=`property_master_bodycorp_id` WHERE (`property_master_id` IS NULL) OR (`property_master_id`=" + property.PropertyMasterBodycorpId + ")", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `property_type_id` AS `ID`, `property_type_code` AS `Code` FROM `property_types`", "ID", "Code", constr, false);
                            #endregion
                            #region Initial Web Controls
                            
                            AjaxControlUtils.ComboBoxSelection(ComboBoxBodycorp, property.PropertyMasterBodycorpId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxType, property.PropertyMasterTypeId, false);
                            TextBoxCode.Text = property.PropertyMasterCode;
                            TextBoxName.Text = property.PropertyMasterName;
                            TextBoxNumOfUnits.Text = property.PropertyMasterNumOfUnits.ToString();
                            if (property.PropertyMasterTotalsqm.HasValue)
                                TextBoxTotalSqm.Text = property.PropertyMasterTotalsqm.Value.ToString();
                            TextBoxNotes.Text = property.PropertyMasterNotes;
                            #endregion
                            
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
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
        #region WebControl Events
        protected void ImageButtonSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string property_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (!Page.IsValid) return;
                if (Request.QueryString["propertyid"] != null) property_id = Request.QueryString["propertyid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];

                #region Retireve Values

                Hashtable items = new Hashtable();

                items.Add("property_master_code", DBSafeUtils.StrToQuoteSQL(TextBoxCode.Text));
                items.Add("property_master_name", DBSafeUtils.StrToQuoteSQL(TextBoxName.Text));
                items.Add("property_master_bodycorp_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxBodycorp, false));
                items.Add("property_master_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxType, false));
                items.Add("property_master_notes", DBSafeUtils.StrToQuoteSQL(TextBoxNotes.Text));
                items.Add("property_master_begin_date", DBSafeUtils.DateToSQL(TextBoxBeginDate.Text));

                #endregion
                if (mode == "add")
                {
                    #region Save
                    PropertyMaster property = new PropertyMaster(constr);
                    property.Add(items);
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    PropertyMaster property = new PropertyMaster(constr);
                    property.Update(items, Convert.ToInt32(property_id));
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
                Response.Redirect("~/propertymaster.aspx" + NewQueryString("?"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void CustomValidatorBodycorp_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxBodycorp.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
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