using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using Sapp.Common;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.SMS;
using System.Text.RegularExpressions;

namespace sapp_sms
{
    public partial class accountedit : System.Web.UI.Page
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
            string account_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["accountid"] != null) account_id = Request.QueryString["accountid"];
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
                            AjaxControlUtils.SetupComboBox(ComboBoxChart, "SELECT `chart_master_id` AS `ID`, `chart_master_code` AS `Code` FROM `chart_master` where chart_master_bank_account=1", "ID", "Code", constr, false);
                            #endregion
                            #region Initial Web Controls
                            TextBoxNum.Text = "";
                            TextBoxCode.Text = "";
                            TextBoxName.Text = "";
                            TextBoxBank.Text = "";
                            TextBoxBranch.Text = "";
                            TextBoxSwift.Text = "";
                            TextBoxDescription.Text = "";
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
                            AjaxControlUtils.SetupComboBox(ComboBoxChart, "SELECT `chart_master_id` AS `ID`, `chart_master_code` AS `Code` FROM `chart_master` where chart_master_bank_account=1", "ID", "Code", constr, false);
                            #endregion
                            #region Load Web Controls
                            Account account = new Account(constr);
                            account.LoadData(Convert.ToInt32(account_id));
                            TextBoxNum.Text = account.AccountNum;
                            TextBoxCode.Text = account.AccountCode;
                            TextBoxName.Text = account.AccountName;
                            TextBoxBank.Text = account.AccountBank;
                            TextBoxBranch.Text = account.AccountBranch;
                            TextBoxSwift.Text = account.AccountSwift;
                            TextBoxDescription.Text = account.AccountDescription;
                            AjaxControlUtils.ComboBoxSelection(ComboBoxChart, account.AccountId, false);
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
                string account_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (!Page.IsValid) return;
                if (Request.QueryString["accountid"] != null) account_id = Request.QueryString["accountid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];

                #region Retireve Values
                Hashtable items = new Hashtable();
                items.Add("account_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxChart, true));
                items.Add("account_num", DBSafeUtils.StrToQuoteSQL(TextBoxNum.Text));
                items.Add("account_code", DBSafeUtils.StrToQuoteSQL(TextBoxCode.Text));
                items.Add("account_name", DBSafeUtils.StrToQuoteSQL(TextBoxName.Text));
                items.Add("account_bank", DBSafeUtils.StrToQuoteSQL(TextBoxBank.Text));
                items.Add("account_branch", DBSafeUtils.StrToQuoteSQL(TextBoxBranch.Text));
                items.Add("account_swift", DBSafeUtils.StrToQuoteSQL(TextBoxSwift.Text));
                items.Add("account_description", DBSafeUtils.StrToQuoteSQL(TextBoxDescription.Text));
                #endregion
                if (mode == "add")
                {
                    #region Save
                    Account account = new Account(constr);
                    account.Add(items);
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    Account account = new Account(constr);
                    account.Update(items, Convert.ToInt32(account_id));
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
                Response.Redirect("~/accounts.aspx" + NewQueryString("?"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
        #region Validations
        protected void CustomValidatorChart_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxChart.SelectedIndex >= -1) args.IsValid = true; // -1 unselect, 0 null selected
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
