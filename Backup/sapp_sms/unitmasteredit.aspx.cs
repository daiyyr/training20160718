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
using System.Data;

namespace sapp_sms
{
    public partial class unitmasteredit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string unitmaster_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["unitmasterid"] != null) unitmaster_id = Request.QueryString["unitmasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                #region Javascript Setup
                Control[] wc = { ComboBoxProperty, ComboBoxDebtor, ComboBoxPrincipal, ComboBoxType };
                #endregion
                if (!IsPostBack)
                {
                    if (mode == "add")
                    {
                        #region Load Page
                        try
                        {
                            string propertyid = Request.QueryString["propertyid"];
                            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxProperty, "SELECT `property_master_id` AS `ID`, `property_master_code` AS `Code` FROM `property_master`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxDebtor, "SELECT `debtor_master_id` AS `ID`, `debtor_master_name` AS `Code` FROM `debtor_master` Where `debtor_master_type_id`=1 order by debtor_master_name", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxPrincipal, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` LEFT JOIN `unit_types` ON `unit_master_type_id`=`unit_type_id` WHERE `unit_type_code`='PRINCIPAL' AND unit_master_property_id = " + propertyid + " ORDER BY IF(LEFT(Code, 1) REGEXP '[0-9]', CONVERT(Code, UNSIGNED), 2147483648), RIGHT(Code, LENGTH(Code) - LENGTH(CONVERT(Code, UNSIGNED))) ", "ID", "Code", constr, true);  // Update 08/04/2016
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `unit_type_id` AS `ID`, `unit_type_code` AS `Code` FROM `unit_types`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxAreaType, "SELECT `unit_areatype_id` AS `ID`, `unit_areatype_code` AS `Code` FROM `unit_areatypes`", "ID", "Code", constr, true);
                            #endregion
                            #region Initial Web Controls
                            TextBoxArea.Text = "";
                            TextBoxCode.Text = "";
                            TextBoxKnowAs.Text = "";
                            TextBoxNotes.Text = "";
                            TextBoxOwnershipInterest.Text = "";
                            TextBoxSpecialScale.Text = "";
                            TextBoxUtilityInterest.Text = "";
                            CheckBoxCommittee.Checked = false;
                            AjaxControlUtils.ComboBoxSelection(ComboBoxProperty, Convert.ToInt32(propertyid), false);
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
                            string unitmasterid = Request.QueryString["unitmasterid"];  // Add 08/04/2016
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxProperty, "SELECT `property_master_id` AS `ID`, `property_master_code` AS `Code` FROM `property_master`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxDebtor, "SELECT `debtor_master_id` AS `ID`, `debtor_master_name` AS `Code` FROM `debtor_master` Where `debtor_master_type_id`=1 order by debtor_master_name", "ID", "Code", constr, true);
                            //AjaxControlUtils.SetupComboBox(ComboBoxPrincipal, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master`", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxPrincipal, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` WHERE unit_master_property_id IN (SELECT unit_master_property_id FROM unit_master WHERE unit_master_id = " + unitmasterid + " ) ORDER BY IF(LEFT(Code, 1) REGEXP '[0-9]', CONVERT(Code, UNSIGNED), 2147483648), RIGHT(Code, LENGTH(Code) - LENGTH(CONVERT(Code, UNSIGNED))) ", "ID", "Code", constr, true);  // Update 08/04/2016
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `unit_type_id` AS `ID`, `unit_type_code` AS `Code` FROM `unit_types`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxAreaType, "SELECT `unit_areatype_id` AS `ID`, `unit_areatype_code` AS `Code` FROM `unit_areatypes`", "ID", "Code", constr, true);
                            #endregion
                            #region Load Web Controls
                            UnitMaster unitmaster = new UnitMaster(constr);
                            unitmaster.LoadData(Convert.ToInt32(unitmaster_id));

                            AjaxControlUtils.ComboBoxSelection(ComboBoxProperty, unitmaster.UnitMasterPropertyId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxDebtor, unitmaster.UnitMasterDebtorId, true);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxPrincipal, unitmaster.UnitMasterPrincipalId, true);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxType, unitmaster.UnitMasterTypeId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxAreaType, unitmaster.UnitMasterAreatypeId, true);
                            if (unitmaster.UnitMasterInactiveDate != null)
                                InactiveDateT.Text = DateTime.Parse(unitmaster.UnitMasterInactiveDate.ToString()).ToString("dd/MM/yyyy");
                            TextBoxArea.Text = unitmaster.UnitMasterArea.ToString();
                            TextBoxCode.Text = DBSafeUtils.DBStrToStr(unitmaster.UnitMasterCode);
                            TextBoxKnowAs.Text = DBSafeUtils.DBStrToStr(unitmaster.UnitMasterKnowAs);
                            TextBoxNotes.Text = DBSafeUtils.DBStrToStr(unitmaster.UnitMasterNotes);
                            TextBoxOwnershipInterest.Text = unitmaster.UnitMasterOwnershipInterest.ToString();
                            TextBoxSpecialScale.Text = unitmaster.UnitMasterSpecialScale.ToString();
                            TextBoxUtilityInterest.Text = unitmaster.UnitMasterUtilityInterest.ToString();
                            CheckBoxCommittee.Checked = unitmaster.UnitMasterCommittee ?? false;
                            TextBoxBeginDate.Text = unitmaster.UnitMasterBeginDate.ToString("dd/MM/yyyy");
                            if (unitmaster.UnitMasterInactiveDate != null)
                            {

                                InactiveDateT.Text = DateTime.Parse(unitmaster.UnitMasterInactiveDate.ToString()).ToString("dd/MM/yyyy");
                            }

                            // Add 08/04/2016
                            if (unitmaster.UnitMasterTypeId == 1)
                            {
                                ComboBoxPrincipal.SelectedIndex = -1;
                                ComboBoxPrincipal.Enabled = false;
                            }
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
                string unitmaster_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable items = new Hashtable();
                if (!Page.IsValid) return;
                if (Request.QueryString["unitmasterid"] != null) unitmaster_id = Request.QueryString["unitmasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                #region Retireve Values


                items.Add("unit_master_code", DBSafeUtils.StrToQuoteSQL(TextBoxCode.Text));
                items.Add("unit_master_knowas", DBSafeUtils.StrToQuoteSQL(TextBoxKnowAs.Text));
                items.Add("unit_master_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxType, false));
                items.Add("unit_master_principal_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxPrincipal, true));
                items.Add("unit_master_property_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxProperty, false));
                items.Add("unit_master_debtor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxDebtor, true));
                items.Add("unit_master_areatype_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxAreaType, true));
                items.Add("unit_master_area", DBSafeUtils.DecimalToSQL(TextBoxArea.Text));
                items.Add("unit_master_ownership_interest", DBSafeUtils.DecimalToSQL(TextBoxOwnershipInterest.Text));
                items.Add("unit_master_utility_interest", DBSafeUtils.DecimalToSQL(TextBoxUtilityInterest.Text));
                items.Add("unit_master_special_scale", DBSafeUtils.DecimalToSQL(TextBoxSpecialScale.Text));
                items.Add("unit_master_committee", DBSafeUtils.BoolToSQL(CheckBoxCommittee.Checked));
                items.Add("unit_master_notes", DBSafeUtils.StrToQuoteSQL(TextBoxNotes.Text));
                if (!InactiveDateT.Text.Equals(""))
                    items.Add("unit_master_inactive_date", DBSafeUtils.StrToQuoteSQL(DateTime.Parse(InactiveDateT.Text).ToString("yyyy-MM-dd")));
                else
                    items.Add("unit_master_inactive_date", "null");
                items.Add("unit_master_begin_date", DBSafeUtils.StrToQuoteSQL(DateTime.Parse(TextBoxBeginDate.Text).ToString("yyyy-MM-dd")));
                {
                    string sql = "select * from ownerships where ownership_unit_id=" + unitmaster_id + " order by ownership_end desc";
                    Odbc o = new Odbc(AdFunction.conn);
                    DataTable dt = o.ReturnTable(sql, "t1");

                    //foreach (DataRow dr in dt.Rows)
                    //{
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        string id = dr["ownership_id"].ToString();
                        DateTime start = DateTime.Parse(dr["ownership_start"].ToString());
                        DateTime end = DateTime.Parse(TextBoxBeginDate.Text).AddDays(-1);
                        if (start > end)
                        {
                            new Exception("Ownership start date error");
                        }
                        string ends = end.ToString("yyyy-MM-dd");
                        string upsql = "update ownerships set ownership_end='" + ends + "' where ownership_id=" + id;
                        o.ExecuteScalar(upsql);
                    }
                    //}
                }
                #endregion
                if (mode == "add")
                {
                    #region Save
                    UnitMaster unitmaster = new UnitMaster(constr);
                    unitmaster.Add(items);
                    #endregion
                    #region Redirect
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    UnitMaster unitmaster = new UnitMaster(constr);
                    unitmaster.Update(items, Convert.ToInt32(unitmaster_id));
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
                Response.Redirect("goback.aspx", false);
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

        protected void ComboBoxType_SelectedIndexChanged(object source, EventArgs args)
        {
            try
            {
                if (ComboBoxType.SelectedValue.Equals("1"))
                {
                    // Principal
                    ComboBoxPrincipal.SelectedIndex = -1;
                    ComboBoxPrincipal.Enabled = false;
                }
                else
                {
                    // NOT Principal
                    ComboBoxPrincipal.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #endregion
    }
}