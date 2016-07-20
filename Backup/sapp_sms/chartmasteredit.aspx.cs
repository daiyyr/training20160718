using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using System.Collections;
using Sapp.Data;
using System.Configuration;
using Sapp.SMS;

namespace sapp_sms
{
    public partial class chartmasteredit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string chartmaster_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["chartmasterid"] != null) chartmaster_id = Request.QueryString["chartmasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                #region Javascript Setup
                Control[] wc = { ComboBoxType, ComboBoxAccount, ComboBoxRechargeTo };
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
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `chart_type_id` AS `ID`, `chart_type_name` AS `Name` FROM `chart_types`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxRechargeTo,
                                "SELECT `chart_master_id` AS `ID`, CONCAT(`chart_master_code`,' | ',`chart_master_name`) AS `Code` FROM `chart_master`", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxAccount, "SELECT `account_id` AS `ID`, `account_code` AS `Code` FROM `accounts`", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxParent, "SELECT `chart_master_id` AS `ID`, `chart_master_name` AS `Code` FROM `chart_master` where chart_master_type_id = 7 and chart_master_id <> 285", "ID", "Code", constr, true);

                            #endregion
                            #region Initial Web Controls
                            TextBoxCode.Text = "";
                            ComboBoxAccount.SelectedIndex = 0;
                            ComboBoxType.SelectedIndex = 0;
                            ComboBoxRechargeTo.SelectedIndex = 0;
                            TextBoxName.Text = "";
                            CheckBoxNotax.Checked = false;
                            CheckBoxInactive.Checked = false;
                            CheckBoxLevyBase.Checked = false;
                            CheckBoxTrust.Checked = false;
                            CheckBoxBankAccount.Checked = false;
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
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `chart_type_id` AS `ID`, `chart_type_name` AS `Name` FROM `chart_types`", "ID", "Name", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxRechargeTo,
                                "SELECT `chart_master_id` AS `ID`, CONCAT(`chart_master_code`,' | ',`chart_master_name`) AS `Code` FROM `chart_master`", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxAccount, "SELECT `account_id` AS `ID`, `account_code` AS `Code` FROM `accounts`", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxParent, "SELECT `chart_master_id` AS `ID`, `chart_master_name` AS `Code` FROM `chart_master` where chart_master_type_id = 7 and chart_master_id <> 285", "ID", "Code", constr, true);

                            #endregion
                            #region Load Web Controls
                            ChartMaster chartmaster = new ChartMaster(constr);
                            chartmaster.LoadData(Convert.ToInt32(chartmaster_id));

                            TextBoxCode.Text = chartmaster.ChartMasterCode;
                            TextBoxName.Text = chartmaster.ChartMasterName;
                            AjaxControlUtils.ComboBoxSelection(ComboBoxAccount, chartmaster.ChartMasterAccountId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxType, chartmaster.ChartMasterTypeId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxRechargeTo, chartmaster.ChartMasterRechargeToId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxParent, chartmaster.ChartMasterParentID, false);
                            CheckBoxNotax.Checked = chartmaster.ChartMasterNotax;
                            CheckBoxInactive.Checked = chartmaster.ChartMasterInactive;
                            CheckBoxLevyBase.Checked = chartmaster.ChartMasterLevyBase;
                            CheckBoxTrust.Checked = chartmaster.ChartMasterTrustAccount;
                            CheckBoxBankAccount.Checked = chartmaster.ChartMasterBankAccount;
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
                string chartmaster_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable items = new Hashtable();
                if (!Page.IsValid) return;
                if (Request.QueryString["chartmasterid"] != null) chartmaster_id = Request.QueryString["chartmasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                #region Retireve Values
                items.Add("chart_master_code", DBSafeUtils.StrToQuoteSQL(TextBoxCode.Text));
                items.Add("chart_master_name", DBSafeUtils.StrToQuoteSQL(TextBoxName.Text));
                items.Add("chart_master_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxType, false));
                items.Add("chart_master_account_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxAccount, true));
                items.Add("chart_master_recharge_to_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxRechargeTo, true));

                items.Add("chart_master_parent_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxParent, true));
                items.Add("chart_master_bank_account", DBSafeUtils.BoolToSQL(CheckBoxBankAccount.Checked));
                items.Add("chart_master_trust_account", DBSafeUtils.BoolToSQL(CheckBoxTrust.Checked));
                items.Add("chart_master_notax", DBSafeUtils.BoolToSQL(CheckBoxNotax.Checked));
                items.Add("chart_master_levy_base", DBSafeUtils.BoolToSQL(CheckBoxLevyBase.Checked));
                items.Add("chart_master_inactive", DBSafeUtils.BoolToSQL(CheckBoxInactive.Checked));
                #endregion
                if (mode == "add")
                {
                    #region Save
                    ChartMaster chartmaster = new ChartMaster(constr);
                    chartmaster.Add(items);
                    #endregion
                    #region Redirect
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    ChartMaster chartmaster = new ChartMaster(constr);
                    chartmaster.Update(items, Convert.ToInt32(chartmaster_id));
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
                Response.Redirect("~/chartmaster.aspx",false);
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