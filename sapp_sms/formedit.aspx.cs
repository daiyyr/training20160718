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
    public partial class formedit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string form_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["formid"] != null) form_id = Request.QueryString["formid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                if (!IsPostBack)
                {
                    if (mode == "add")
                    {
                        #region Load Page
                        try
                        {
                            string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                            #region Initial Web Controls
                            TextBoxName.Text = "";
                            TextBoxFileName.Text = "";
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
                            #region Load Web Controls
                            Form form = new Sapp.General.Form(constr);
                            form.LoadData(Convert.ToInt32(form_id));
                            TextBoxName.Text = form.FormName;
                            TextBoxFileName.Text = form.FormFilename;
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
                string form_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (!Page.IsValid) return;
                if (Request.QueryString["formid"] != null) form_id = Request.QueryString["formid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];

                #region Retireve Values
                Hashtable items = new Hashtable();
                items.Add("form_name", DBSafeUtils.StrToQuoteSQL(TextBoxName.Text));
                items.Add("form_filename", DBSafeUtils.StrToQuoteSQL(TextBoxFileName.Text));
                #endregion
                if (mode == "add")
                {
                    #region Save
                    Form form = new Sapp.General.Form(constr);
                    form.Add(items);
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    Form form = new Sapp.General.Form(constr);
                    form.Update(items, Convert.ToInt32(form_id));
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
                Response.Redirect("~/forms.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}