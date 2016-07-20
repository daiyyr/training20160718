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

namespace sapp_sms
{
    public partial class pptytitleedit : System.Web.UI.Page
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
            string pptytitle_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["pptytitleid"] != null) pptytitle_id = Request.QueryString["pptytitleid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                #region Javascript Setup
                Control[] wc = { ComboBoxProperty, ComboBoxAuthority };
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
                            AjaxControlUtils.SetupComboBox(ComboBoxProperty, "SELECT `property_master_id` AS `ID`, `property_master_code` AS `Code` FROM `property_master`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxAuthority, "SELECT `authority_id` AS `ID`, `authority_code` AS `Code` FROM `authorities`", "ID", "Code", constr, false);
                            #endregion
                            #region Initial Web Controls
                            TextBoxArea.Text = "";
                            TextBoxCtreference.Text = "";
                            TextBoxDplan.Text ="" ;
                            TextBoxLot.Text = "";
                            TextBoxNote.Text = "";
                            TextBoxZone.Text = "";
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
                            AjaxControlUtils.SetupComboBox(ComboBoxProperty, "SELECT `property_master_id` AS `ID`, `property_master_code` AS `Code` FROM `property_master`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxAuthority, "SELECT `authority_id` AS `ID`, `authority_code` AS `Code` FROM `authorities`", "ID", "Code", constr, false);
                            #endregion
                            #region Load Web Controls
                            Pptytitle pptytitle = new Pptytitle(constr);
                            pptytitle.LoadData(Convert.ToInt32(pptytitle_id));

                            AjaxControlUtils.ComboBoxSelection(ComboBoxProperty, pptytitle.PptytitlePropertyId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxAuthority, pptytitle.PptytitleAuthorityId, false);
                            TextBoxArea.Text = pptytitle.PptytitleArea.ToString();
                            TextBoxCtreference.Text = DBSafeUtils.DBStrToStr(pptytitle.PptytitleCtreference);
                            TextBoxDplan.Text = DBSafeUtils.DBStrToStr(pptytitle.PptytitleDplan);
                            TextBoxLot.Text = DBSafeUtils.DBStrToStr(pptytitle.PptytitleLot);
                            TextBoxNote.Text = DBSafeUtils.DBStrToStr(pptytitle.PptytitleNotes);
                            TextBoxZone.Text = DBSafeUtils.IntToSQL(pptytitle.PptytitleZoneId);
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
                string pptytitle_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable items = new Hashtable();
                if (!Page.IsValid) return;
                if (Request.QueryString["pptytitleid"] != null) pptytitle_id = Request.QueryString["pptytitleid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                #region Retireve Values
                items.Add("pptytitle_property_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxProperty, false));
                items.Add("pptytitle_ctreference", DBSafeUtils.StrToQuoteSQL(TextBoxCtreference.Text));
                items.Add("pptytitle_dplan", DBSafeUtils.StrToQuoteSQL(TextBoxDplan.Text));
                items.Add("pptytitle_lot", DBSafeUtils.StrToQuoteSQL(TextBoxLot.Text));
                items.Add("pptytitle_area", DBSafeUtils.DecimalToSQL(TextBoxArea.Text));
                items.Add("pptytitle_authority_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxAuthority,false));
                items.Add("pptytitle_zone_id", DBSafeUtils.IntToSQL(TextBoxZone.Text));
                items.Add("pptytitle_notes", DBSafeUtils.StrToQuoteSQL(TextBoxNote.Text));
                #endregion
                if (mode == "add")
                {
                    #region Save
                    Pptytitle pptytitle = new Pptytitle(constr);
                    pptytitle.Add(items);
                    #endregion
                    #region Redirect
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    Pptytitle pptytitle = new Pptytitle(constr);
                    pptytitle.Update(items, Convert.ToInt32(pptytitle_id));
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
                Response.Redirect("~/pptytitles.aspx" + NewQueryString("?"),false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            #endregion
        }
        #endregion
        #region Validation
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

        protected void CustomValidatorAuthority_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxAuthority.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
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