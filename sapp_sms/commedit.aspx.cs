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

namespace sapp_sms
{
    public partial class commedit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                string comm_master_id = "";
                string mode = "";
                string fktype = "";
                if (Request.QueryString["commid"] != null) comm_master_id = Request.QueryString["commid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                if (Request.QueryString["fktype"] != null) fktype = Request.QueryString["fktype"];
                #region Javascript Setup
                Control[] wc = { ComboBoxType };
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
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `comm_type_id` AS `ID`, `comm_type_code` AS `Name` FROM `comm_types`", "ID", "Name", constr, false);
                            #endregion
                            #region Initial Web Controls
                            TextBoxData.Text = "";
                            CheckBoxPrimary.Checked = false;
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
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `comm_type_id` AS `ID`, `comm_type_code` AS `Name` FROM `comm_types`", "ID", "Name", constr, false);
                            #endregion
                            #region Load Web Controls
                            if (fktype == "account")
                            {
                                Comm<Account> comm = new Comm<Account>(constr);
                                comm.LoadData(Convert.ToInt32(comm_master_id));
                                TextBoxData.Text = comm.CommMasterData;
                                CheckBoxPrimary.Checked = comm.CommMasterPrimary;
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
                string comm_master_id = "";
                string mode = "";
                string fk_id = "";
                string fktype = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable items = new Hashtable();
                if (!Page.IsValid) return;
                if (Request.QueryString["commid"] != null) comm_master_id = Request.QueryString["commid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                if (Request.QueryString["fkid"] != null) fk_id = Request.QueryString["fkid"];
                if (Request.QueryString["fktype"] != null) fktype = Request.QueryString["fktype"];
                #region Retireve Values
                items.Add("comm_master_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxType, false));
                items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(TextBoxData.Text));
                items.Add("comm_master_primary", DBSafeUtils.BoolToSQL(CheckBoxPrimary.Checked));
                #endregion
                if (mode == "add")
                {
                    #region Save
                    switch (Convert.ToInt32(items["comm_master_type_id"]))
                    {
                        case 1:
                            if (fktype == "account")
                            {
                                Account account = new Account(constr);
                                account.LoadData(Convert.ToInt32(fk_id));
                                Comm<Account> comm = new Comm<Account>(constr);
                                comm.Add(items, account);
                            }
                            break;
                        default:
                            throw new Exception("Error: not a proper fktype!");
                            break;
                    }

                    #endregion
                    #region Redirect
                    if (fktype == "account")
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("~/accountdetails.aspx?accountid=" + fk_id, false);
                    }
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    if (fktype == "account")
                    {
                        Account account = new Account(constr);
                        account.LoadData(Convert.ToInt32(fk_id));
                        Comm<Account> comm = new Comm<Account>(constr);
                        comm.Update(items, Convert.ToInt32(comm_master_id));
                    }
                    #endregion
                    #region Redirect
                    if (fktype == "account")
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("~/accountdetails.aspx?accountid=" + fk_id, false);
                    }
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
                string retpage = "";
                if (Request.QueryString["retpage"] != null) retpage = Request.QueryString["retpage"];
                Response.BufferOutput = true;
                Response.Redirect(retpage, false);
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
                if (ComboBoxType.SelectedIndex >= 0) args.IsValid = true; // -1 unselect
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