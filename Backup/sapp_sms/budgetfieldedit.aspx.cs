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

namespace sapp_sms
{
    public partial class budgetfieldedit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string mode = "";
            string targetID="";
            string bodycorpID = "";
            try
            {
                if (Request.QueryString["targetID"] != null) targetID = Request.QueryString["targetID"];
                if (Request.Cookies["bodycorpid"].Value != null) bodycorpID = Request.Cookies["bodycorpid"].Value;
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                #region Javascript Setup
                Control[] wc = { ComboBoxBodycorp };
                #endregion
                if (!IsPostBack)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    #region Initial ComboBox
                    AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, false);
                    #endregion
                    #region Initial Web Controls
                    AjaxControlUtils.ComboBoxSelection(ComboBoxBodycorp, bodycorpID, false);
                    ComboBoxBodycorp.Enabled = false;
                    TextBoxName.Text = "";
                    TextBoxStart.Text = "";
                    TextBoxEnd.Text = "";
                    TextBoxOrder.Text = "";
                    #endregion
                    if (mode == "insert")
                    {
                        #region Load Page
                        try
                        {
                            #region Initial Web Controls
                            BudgetField buf = new BudgetField(constr);
                            TextBoxRate.Text = "0";
                            TextBoxRate.Visible = false;
                            LabelRate.Visible = false;
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
                    if (mode == "split")
                    {
                        #region Load Page
                        try
                        {
                            BudgetField targetField = new BudgetField(constr);
                            targetField.LoadData(Int32.Parse(targetID));
                            #region Initial Web Controls
                            if(targetField.Budget_field_start.HasValue)
                                TextBoxStart.Text = targetField.Budget_field_start.Value.ToString("dd/MM/yyyy");
                            if(targetField.Budget_field_end.HasValue)
                                TextBoxEnd.Text = targetField.Budget_field_end.Value.ToString("dd/MM/yyyy");
                            TextBoxOrder.Text = (targetField.Budget_field_order + 1).ToString();
                            TextBoxRate.Text = "0";
                            TextBoxRate.Visible = true;
                            LabelRate.Visible = true;
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
            string targetID = "";
            string mode = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (Request.QueryString["targetID"] != null) targetID = Request.QueryString["targetID"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                BudgetMaster targetBudget = new BudgetMaster(constr);
                Hashtable items = new Hashtable();
                if (!Page.IsValid) return;
                if (!string.IsNullOrEmpty(TextBoxStart.Text))
                {
                    DateTime start=DateTime.Parse(TextBoxStart.Text);
                    DateTime newstart = new DateTime(start.Year, start.Month, 1);
                    TextBoxStart.Text = newstart.ToString("dd/MM/yyyy");
                }
                if (!string.IsNullOrEmpty(TextBoxEnd.Text))
                {
                    DateTime end = DateTime.Parse(TextBoxEnd.Text);
                    DateTime newend = new DateTime(end.Year, end.Month, 1).AddMonths(1).AddDays(-1);
                    TextBoxEnd.Text = newend.ToString("dd/MM/yyyy");
                }
                #region Retireve Values
                items.Add("budget_field_name", DBSafeUtils.StrToQuoteSQL(TextBoxName.Text));
                items.Add("budget_field_bodycorp_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxBodycorp, false));
                items.Add("budget_field_start", DBSafeUtils.DateToSQL(TextBoxStart.Text));
                items.Add("budget_field_end", DBSafeUtils.DateToSQL(TextBoxEnd.Text));
                items.Add("budget_field_order", DBSafeUtils.IntToSQL(TextBoxOrder.Text));
                #endregion
                if (mode == "insert")
                {
                    #region Save
                    BudgetField budgetfield = new BudgetField(constr);
                    budgetfield.Add(items);
                    #endregion
                    #region Redirect
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "split")
                {
                    decimal rate = Decimal.Parse(TextBoxRate.Text);
                    if (rate > 1 || rate < 0) return;
                    #region Save
                    BudgetField budgetfield = new BudgetField(constr);
                    budgetfield.Add(items);
                    //int newFieldID = budgetfield.GetLatestID();
                    //BudgetMasters budgetmasters = new BudgetMasters(constr);
                    //budgetmasters.Split(Int32.Parse(targetID), newFieldID, rate);
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
        }
        #endregion
    }
}