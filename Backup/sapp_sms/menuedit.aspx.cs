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
    public partial class menuedit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string menu_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["menuid"] != null) menu_id = Request.QueryString["menuid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                #region Javascript Setup
                Control[] wc = { ComboBoxParent };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    if (mode == "add" || mode == "addsub")
                    {
                        #region Load Page
                        try
                        {
                            string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxParent, "SELECT `menu_id` AS `ID`, `menu_name` AS `Name` FROM `menus`", "ID", "Name", constr, true);
                            #endregion
                            #region Initial Web Controls
                            if (mode == "add")
                            {
                                if (menu_id == "")
                                    AjaxControlUtils.ComboBoxSelection(ComboBoxParent, null, true);
                                else
                                {
                                    Sapp.General.Menu menu = new Sapp.General.Menu(constr);
                                    menu.LoadData(Convert.ToInt32(menu_id));
                                    AjaxControlUtils.ComboBoxSelection(ComboBoxParent, menu.MenuParentId, true);
                                }
                            }
                            else
                                AjaxControlUtils.ComboBoxSelection(ComboBoxParent, Convert.ToInt32(menu_id), true);
                            TextBoxName.Text = "";
                            TextBoxDir.Text = "";
                            TextBoxModuleId.Text = "";
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
                            AjaxControlUtils.SetupComboBox(ComboBoxParent, "SELECT `menu_id` AS `ID`, `menu_name` AS `Name` FROM `menus`", "ID", "Name", constr, true);
                            #endregion
                            #region Load Web Controls
                            Sapp.General.Menu menu = new Sapp.General.Menu(constr);
                            menu.LoadData(Convert.ToInt32(menu_id));
                            LabelMenuID.Text = menu.MenuId.ToString();
                            AjaxControlUtils.ComboBoxSelection(ComboBoxParent, menu.MenuParentId, true);
                            TextBoxName.Text = menu.MenuName;
                            TextBoxDir.Text = menu.MenuDir;
                            TextBoxModuleId.Text = menu.MenuModuleId.ToString();
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
        protected void CustomValidatorParent_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxParent.SelectedIndex > -1) args.IsValid = true; // -1 unselect, 0 null selected
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
                string menu_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (!Page.IsValid) return;
                if (Request.QueryString["menuid"] != null) menu_id = Request.QueryString["menuid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];

                #region Retireve Values
                Hashtable items = new Hashtable();
                items.Add("menu_parent_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxParent, true));
                items.Add("menu_name", DBSafeUtils.StrToQuoteSQL(TextBoxName.Text));
                items.Add("menu_dir", DBSafeUtils.StrToQuoteSQL(TextBoxDir.Text));
                items.Add("menu_module_id", Convert.ToInt32(TextBoxModuleId.Text));
                #endregion
                if (mode == "add" || mode == "addsub")
                {
                    #region Save
                    Sapp.General.Menu menu = new Sapp.General.Menu(constr);
                    menu.Add(items);
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    Sapp.General.Menu menu = new Sapp.General.Menu(constr);
                    menu.Update(items, Convert.ToInt32(menu_id));
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
                Response.Redirect("~/menus.aspx?menuid=1", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}