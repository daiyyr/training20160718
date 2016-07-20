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
using Sapp.General;

namespace sapp_sms
{
    public partial class useredit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string user_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["userid"] != null) user_id = Request.QueryString["userid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                #region Javascript Setup
                Control[] wc = { ComboBoxPrivilege };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    if (mode == "add")
                    {
                        #region Load Page
                        try
                        {
                            string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxPrivilege, "SELECT `privilege_id` AS `ID`, `privilege_name` AS `Name` FROM `privileges`", "ID", "Name", constr, false);
                            #endregion
                            #region Initial Web Controls
                            TextBoxLogin.Text = "";
                            TextBoxName.Text = "";
                            TextBoxPassword.Text = "";
                            ComboBoxPrivilege.SelectedIndex = 0;
                            TextBoxEmail.Text = "";
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
                            string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxPrivilege, "SELECT `privilege_id` AS `ID`, `privilege_name` AS `Name` FROM `privileges`", "ID", "Name", constr, false);
                            #endregion
                            #region Load Web Controls
                            Sapp.General.User user = new User(constr);
                            user.LoadData(Convert.ToInt32(user_id));
                            TextBoxLogin.Text = user.UserLogin;
                            TextBoxName.Text = user.UserName;
                            TextBoxPassword.Text = user.UserPassword;
                            AjaxControlUtils.ComboBoxSelection(ComboBoxPrivilege, user.UserPrivilegeId, false);
                            TextBoxEmail.Text = user.UserEmail;
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

        #region Validation
        protected void CustomValidatorPrivilege_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxPrivilege.SelectedIndex > -1) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        #region WebControl Events
        protected void ImageButtonSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string user_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (!Page.IsValid) return;
                if (Request.QueryString["userid"] != null) user_id = Request.QueryString["userid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];

                #region Retireve Values
                Hashtable items = new Hashtable();
                items.Add("user_login", DBSafeUtils.StrToQuoteSQL(TextBoxLogin.Text));
                items.Add("user_name", DBSafeUtils.StrToQuoteSQL(TextBoxName.Text));
                if(TextBoxPassword.Text != "") items.Add("user_password", DBSafeUtils.StrToQuoteSQL(TextBoxPassword.Text));
                items.Add("user_privilege_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxPrivilege, false));
                items.Add("user_email", DBSafeUtils.StrToQuoteSQL(TextBoxEmail.Text));
                #endregion
                if (mode == "add")
                {
                    #region Save
                    User user = new Sapp.General.User(constr);
                    user.Add(items);
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    User user = new Sapp.General.User(constr);
                    user.Update(items, Convert.ToInt32(user_id));
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
                Response.Redirect("~/users.aspx",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}