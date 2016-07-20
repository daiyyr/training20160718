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
    public partial class pptymtgedit : System.Web.UI.Page
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
            string pptymtg_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["pptymtgid"] != null) pptymtg_id = Request.QueryString["pptymtgid"];
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
                            AjaxControlUtils.SetupComboBox(ComboBoxProperty, "SELECT `property_master_id` AS `ID`, `property_master_code` AS `Code` FROM `property_master`", "ID", "Code", constr, false);
                            #endregion
                            #region Initial Web Controls
                            TextBoxDate.Text = "";
                            TextBoxExpiry.Text = "";
                            TextBoxMortgagor.Text = "";
                            TextBoxPayment.Text = "";
                            TextBoxPrinciple.Text = "";
                            TextBoxRate.Text = "";
                            TextBoxTerm.Text = "";
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
                            #endregion
                            #region Load Web Controls
                            Pptymtg pptymtg = new Pptymtg(constr);
                            pptymtg.LoadData(Convert.ToInt32(pptymtg_id));

                            AjaxControlUtils.ComboBoxSelection(ComboBoxProperty, pptymtg.PptymtgPropertyId, false);
                            TextBoxDate.Text = DBSafeUtils.DBDateToStr(pptymtg.PptymtgDate);
                            TextBoxExpiry.Text = DBSafeUtils.DBDateToStr(pptymtg.PptymtgExpiry);
                            TextBoxMortgagor.Text = pptymtg.PptymtgMortgagor;
                            TextBoxPayment.Text = pptymtg.PptymtgPayment.ToString();
                            TextBoxPrinciple.Text = pptymtg.PptymtgPrincipal.ToString();
                            TextBoxRate.Text = pptymtg.PptymtgRate.ToString();
                            TextBoxTerm.Text = pptymtg.PptymtgTerms;
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
                    OldEndHF.Value = TextBoxExpiry.Text;
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
                string pptymtg_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable items = new Hashtable();
                if (!Page.IsValid) return;
                if (Request.QueryString["pptymtgid"] != null) pptymtg_id = Request.QueryString["pptymtgid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                #region Retireve Values
                items.Add("pptymtg_property_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxProperty, false));
                items.Add("pptymtg_mortgagor", DBSafeUtils.StrToQuoteSQL(TextBoxMortgagor.Text));
                items.Add("pptymtg_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                items.Add("pptymtg_principal", DBSafeUtils.DecimalToSQL(TextBoxPrinciple.Text));
                items.Add("pptymtg_rate", DBSafeUtils.DecimalToSQL(TextBoxRate.Text));
                items.Add("pptymtg_payment", DBSafeUtils.DecimalToSQL(TextBoxPayment.Text));
                items.Add("pptymtg_expiry", DBSafeUtils.DateToSQL(TextBoxExpiry.Text));
                items.Add("pptymtg_terms", DBSafeUtils.StrToQuoteSQL(TextBoxTerm.Text));
                #endregion
                if (mode == "add")
                {
                    #region Save
                    Pptymtg pptymtg = new Pptymtg(constr);
                    pptymtg.Add(items);
                    #endregion
                    //#region Redirect
                    //Response.BufferOutput = true;
                    //Response.Redirect("goback.aspx", false);
                    ////#endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    Pptymtg pptymtg = new Pptymtg(constr);
                    pptymtg.Update(items, Convert.ToInt32(pptymtg_id));
                    #endregion
                    //#region Redirect
                    //Response.BufferOutput = true;
                    //Response.Redirect("goback.aspx", false);
                    //#endregion
                }
                if (!TextBoxExpiry.Text.Equals(""))
                {
                    if (!TextBoxExpiry.Text.Equals(OldEndHF.Value))
                        Response.Redirect("noticemessage.aspx?propertyid=" + Request.QueryString["propertyid"] + "&date=" + Server.UrlEncode(TextBoxExpiry.Text) + "&title=" + Server.UrlEncode("Mortgages Expiry"), false);
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
                Response.Redirect("~/pptymtgs.aspx" + NewQueryString("?"),false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            #endregion
        }
        #endregion
    }
}