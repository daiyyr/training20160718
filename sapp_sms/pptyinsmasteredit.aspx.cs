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
    public partial class pptyinsmasteredit : System.Web.UI.Page
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
            string pptyinsmaster_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["pptyinsmasterid"] != null) pptyinsmaster_id = Request.QueryString["pptyinsmasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                #region Javascript Setup
                Control[] wc = { ComboBoxType, ComboBoxBroker, ComboBoxUnderWriter, ComboBoxProperty };
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
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `pptyins_type_id` AS `ID`, `pptyins_type_name` AS `Name` FROM `pptyins_types`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxProperty, "SELECT `property_master_id` AS `ID`, `property_master_name` AS `Name` FROM `property_master`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxBroker, "SELECT `contact_master_id` AS `ID`, `contact_master_name` AS `Name` FROM `contact_master` WHERE `contact_master_type_id`=10", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxUnderWriter, "SELECT `contact_master_id` AS `ID`, `contact_master_name` AS `Name` FROM `contact_master` WHERE `contact_master_type_id`=11", "ID", "Name", constr, false);
                            #endregion
                            #region Initial Web Controls

                            TextBoxPolicyNum.Text = "";
                            TextBoxPlacement.Text = "";
                            TextBoxStart.Text = "";
                            TextBoxEnd.Text = "";
                            TextBoxPremium.Text = "";
                            TextBoxExcess.Text = "";
                            TextBoxInsvt.Text = "";
                            TextBoxGST.Text = "";
                            TextBoxCover.Text = "";
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
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `pptyins_type_id` AS `ID`, `pptyins_type_name` AS `Name` FROM `pptyins_types`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxProperty, "SELECT `property_master_id` AS `ID`, `property_master_name` AS `Name` FROM `property_master`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxBroker, "SELECT `contact_master_id` AS `ID`, `contact_master_name` AS `Name` FROM `contact_master` WHERE `contact_master_type_id`=10", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxUnderWriter, "SELECT `contact_master_id` AS `ID`, `contact_master_name` AS `Name` FROM `contact_master` WHERE `contact_master_type_id`=11", "ID", "Name", constr, false);
                            #endregion
                            #region Load Web Controls

                            PptyinsMaster pptyinsmaster = new PptyinsMaster(constr);
                            pptyinsmaster.LoadData(Convert.ToInt32(pptyinsmaster_id));

                            AjaxControlUtils.ComboBoxSelection(ComboBoxType, pptyinsmaster.PptyinsMasterTypeId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxProperty, pptyinsmaster.PptyinsMasterPropertyId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxBroker, pptyinsmaster.PptyinsMasterBrokerId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxUnderWriter, pptyinsmaster.PptyinsMasterUnderwriterId, false);

                            TextBoxPolicyNum.Text = pptyinsmaster.PptyinsMasterPolicyNum;
                            TextBoxPlacement.Text = pptyinsmaster.PptyinsMasterPlacement;
                            TextBoxStart.Text = DBSafeUtils.DBDateToStr(pptyinsmaster.PptyinsMasterStart);
                            TextBoxEnd.Text = DBSafeUtils.DBDateToStr(pptyinsmaster.PptyinsMasterEnd);
                            TextBoxPremium.Text = pptyinsmaster.PptyinsMasterPremium.ToString();
                            TextBoxExcess.Text = pptyinsmaster.PptyinsMasterExcess.ToString();
                            TextBoxInsvt.Text = pptyinsmaster.PptyinsMasterInsvt.ToString();
                            TextBoxGST.Text = pptyinsmaster.PptyinsMasterGst;
                            TextBoxCover.Text = pptyinsmaster.PptyinsMasterCover;
                            TextBoxNotes.Text = pptyinsmaster.PptyinsMasterNotes;

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
                    OldEndHF.Value = TextBoxEnd.Text;
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
                string pptyinsmaster_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable items = new Hashtable();
                if (!Page.IsValid) return;
                if (Request.QueryString["pptyinsmasterid"] != null) pptyinsmaster_id = Request.QueryString["pptyinsmasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                #region Retireve Values


                items.Add("pptyins_master_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxType, false));
                items.Add("pptyins_master_property_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxProperty, false));
                items.Add("pptyins_master_policy_num", DBSafeUtils.StrToQuoteSQL(TextBoxPolicyNum.Text));
                items.Add("pptyins_master_broker_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxBroker, false));
                items.Add("pptyins_master_underwriter_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxUnderWriter, false));
                items.Add("pptyins_master_placement", DBSafeUtils.StrToQuoteSQL(TextBoxPlacement.Text));
                items.Add("pptyins_master_start", DBSafeUtils.DateToSQL(TextBoxStart.Text));
                items.Add("pptyins_master_end", DBSafeUtils.DateToSQL(TextBoxEnd.Text));
                items.Add("pptyins_master_premium", DBSafeUtils.DecimalToSQL(TextBoxPremium.Text));
                items.Add("pptyins_master_excess", DBSafeUtils.DecimalToSQL(TextBoxExcess.Text));
                items.Add("pptyins_master_insvt", DBSafeUtils.DecimalToSQL(TextBoxInsvt.Text));
                items.Add("pptyins_master_cover", DBSafeUtils.StrToQuoteSQL(TextBoxCover.Text));
                items.Add("pptyins_master_gst", DBSafeUtils.StrToQuoteSQL(TextBoxGST.Text));
                items.Add("pptyins_master_notes", DBSafeUtils.StrToQuoteSQL(TextBoxNotes.Text));

                #endregion

                if (mode == "add")
                {
                    #region Save
                    PptyinsMaster pptyinsmaster = new PptyinsMaster(constr);
                    pptyinsmaster.Add(items);
                    #endregion
                    #region Redirect
                    Response.BufferOutput = true;
                    //Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    PptyinsMaster pptyinsmaster = new PptyinsMaster(constr);
                    pptyinsmaster.Update(items, Convert.ToInt32(pptyinsmaster_id));
                    #endregion
                    #region Redirect
                    Response.BufferOutput = true;
                    //Response.Redirect("goback.aspx", false);
                    #endregion
                }
                if (!TextBoxEnd.Text.Equals(""))
                {
                    if (!TextBoxEnd.Text.Equals(OldEndHF.Value))
                        Response.Redirect("noticemessage.aspx?propertyid=" + Request.QueryString["propertyid"] + "&date=" + Server.UrlEncode(TextBoxEnd.Text) + "&title=" + Server.UrlEncode("Insurance Ends"), false);
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
                Response.Redirect("~/pptyinsmaster.aspx" + NewQueryString("?"),false);
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

        protected void CustomValidatorUnderWriter_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxUnderWriter.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void CustomValidatorBroker_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxBroker.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
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