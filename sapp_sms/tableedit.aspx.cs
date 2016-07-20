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
    public partial class tableedit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string table_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["tableid"] != null) table_id = Request.QueryString["tableid"];
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
                            TextBoxDBName.Text = "";
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
                            string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                            #region Load Web Controls
                            Sapp.General.Table table = new Sapp.General.Table(constr);
                            table.LoadData(Convert.ToInt32(table_id));
                            TextBoxName.Text = table.TableName;
                            TextBoxDBName.Text = table.TableDBName;
                            TextBoxDescription.Text = table.TableDescription;
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
                string table_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (!Page.IsValid) return;
                if (Request.QueryString["tableid"] != null) table_id = Request.QueryString["tableid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];

                #region Retireve Values
                Hashtable items = new Hashtable();
                items.Add("table_name", DBSafeUtils.StrToQuoteSQL(TextBoxName.Text));
                items.Add("table_dbname", DBSafeUtils.StrToQuoteSQL(TextBoxDBName.Text));
                items.Add("table_description", DBSafeUtils.StrToQuoteSQL(TextBoxDescription.Text));
                #endregion
                if (mode == "add")
                {
                    #region Save
                    Sapp.General.Table table = new Sapp.General.Table(constr);
                    table.Add(items);
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    Sapp.General.Table table = new Sapp.General.Table(constr);
                    table.Update(items, Convert.ToInt32(table_id));
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
                Response.Redirect("~/tables.aspx",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}