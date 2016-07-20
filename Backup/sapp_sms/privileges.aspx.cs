using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data.Odbc;
using Sapp.Common;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.General;

namespace sapp_sms
{
    public partial class privileges : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string privilege_id = "";
                if (Request.QueryString["privilegeid"] != null) privilege_id = Request.QueryString["privilegeid"];
                #region Javascript setup
                Control[] wc = { jqGridForms, jqGridIncludedForms, jqGridMenus, jqGridIncludedMenus };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                    #region Initial ComboBox
                    AjaxControlUtils.SetupComboBox(ComboBoxPrivileges, "SELECT `privilege_id` AS `ID`, `privilege_name` AS `Name` FROM `privileges`", "ID", "Name", constr, false);
                    #endregion
                    AjaxControlUtils.ComboBoxSelection(ComboBoxPrivileges, Convert.ToInt32(privilege_id), false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            try
            {
                string[] args = eventArgument.Split('|');
                if (args[0] == "ImageButtonMoveLeft")
                {
                    ImageButtonMoveLeft_Click(args);
                }
                else if (args[0] == "ImageButtonMoveRight")
                {
                    ImageButtonMoveRight_Click(args);
                }
                else if (args[0] == "ImageButtonMenuLeft")
                {
                    ImageButtonMenuLeft_Click(args);
                }
                else if (args[0] == "ImageButtonMenuRight")
                {
                    ImageButtonMenuRight_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethod For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridFormsDataBind(string postdata, string privilegeid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (privilegeid == "") throw new Exception("Error: missing privilegeid!");
                string sqlfromstr = "FROM `forms` WHERE (`form_id` NOT IN (SELECT `for_x_pri_form_id` FROM `for_x_pri` WHERE `for_x_pri_privilege_id`=" + privilegeid + "))";
                string sqlselectstr = "SELECT `ID`, `Name`, `FileName` FROM(SELECT `form_id` AS `ID`, `form_name` AS `Name`, `form_filename` AS `FileName`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return "{}";
        }

        [System.Web.Services.WebMethod]
        public static string DataGridIncludedFormsDataBind(string postdata, string privilegeid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (privilegeid == "") throw new Exception("Error: missing privilegeid!");
                string sqlfromstr = "FROM `forms` WHERE (`form_id` IN (SELECT `for_x_pri_form_id` FROM `for_x_pri` WHERE `for_x_pri_privilege_id`=" + privilegeid + "))";
                string sqlselectstr = "SELECT `ID`, `Name`, `FileName` FROM(SELECT `form_id` AS `ID`, `form_name` AS `Name`, `form_filename` AS `FileName`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return "{}";
        }

        [System.Web.Services.WebMethod]
        public static string DataGridMenusDataBind(string postdata, string privilegeid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (privilegeid == "") throw new Exception("Error: missing privilegeid!");
                string sqlfromstr = "FROM `menus` AS t1 LEFT JOIN `menus` AS t2 ON t1.`menu_parent_id`=t2.`menu_id` WHERE (t1.`menu_id` NOT IN (SELECT `menu_x_pri_menu_id` FROM `menu_x_pri` WHERE `menu_x_pri_privilege_id`=" + privilegeid + "))";
                string sqlselectstr = "SELECT `ID`, `Name`, `Parent` FROM(SELECT t1.`menu_id` AS `ID`, t1.`menu_name` AS `Name`, t2.`menu_name` AS `Parent`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return "{}";
        }

        [System.Web.Services.WebMethod]
        public static string DataGridIncludedMenusDataBind(string postdata, string privilegeid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (privilegeid == "") throw new Exception("Error: missing privilegeid!");
                string sqlfromstr = "FROM `menus` AS t1 LEFT JOIN `menus` AS t2 ON t1.`menu_parent_id`=t2.`menu_id` WHERE (t1.`menu_id` IN (SELECT `menu_x_pri_menu_id` FROM `menu_x_pri` WHERE `menu_x_pri_privilege_id`=" + privilegeid + "))";
                string sqlselectstr = "SELECT `ID`, `Name`, `Parent` FROM(SELECT t1.`menu_id` AS `ID`, t1.`menu_name` AS `Name`, t2.`menu_name` AS `Parent`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return "{}";
        }
        #endregion

        #region WebControl Events
        protected void ComboBoxPrivileges_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string privilege_id = ComboBoxPrivileges.SelectedValue;
                Response.BufferOutput = true;
                Response.Redirect("~/privileges.aspx?privilegeid=" + privilege_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonMoveLeft_Click(string[] args)
        {
            try
            {
                string jsonStr = args[1];
                string privilege_id = Request.QueryString["privilegeid"];
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;

                Hashtable jsonObj = (Hashtable)JSON.JsonDecode(jsonStr);
                ArrayList form_ids = (ArrayList)jsonObj["IDs"];
                foreach (string form_id in form_ids)
                {
                    FormXPrivilege forxpri = new FormXPrivilege(constr);
                    forxpri.Delete(Convert.ToInt32(form_id), Convert.ToInt32(privilege_id));
                }
                Response.BufferOutput = true;
                Response.Redirect("~/privileges.aspx?privilegeid=" + privilege_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonMoveRight_Click(string[] args)
        {
            try
            {
                string jsonStr = args[1];
                string privilege_id = Request.QueryString["privilegeid"];
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                Hashtable jsonObj = (Hashtable)JSON.JsonDecode(jsonStr);
                ArrayList form_ids = (ArrayList)jsonObj["IDs"];
                foreach (string form_id in form_ids)
                {
                    Hashtable items = new Hashtable();
                    items.Add("for_x_pri_form_id", Convert.ToInt32(form_id));
                    items.Add("for_x_pri_privilege_id", Convert.ToInt32(privilege_id));
                    FormXPrivilege forxpri = new FormXPrivilege(constr);
                    forxpri.Add(items);
                }
                Response.BufferOutput = true;
                Response.Redirect("~/privileges.aspx?privilegeid=" + privilege_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonMenuLeft_Click(string[] args)
        {
            try
            {
                string jsonStr = args[1];
                string privilege_id = Request.QueryString["privilegeid"];
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;

                Hashtable jsonObj = (Hashtable)JSON.JsonDecode(jsonStr);
                ArrayList menu_ids = (ArrayList)jsonObj["IDs"];
                foreach (string menu_id in menu_ids)
                {
                    MenuXPrivilege menuxpri = new MenuXPrivilege(constr);
                    menuxpri.Delete(Convert.ToInt32(menu_id), Convert.ToInt32(privilege_id));
                }
                Response.BufferOutput = true;
                Response.Redirect("~/privileges.aspx?privilegeid=" + privilege_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonMenuRight_Click(string[] args)
        {
            try
            {
                string jsonStr = args[1];
                string privilege_id = Request.QueryString["privilegeid"];
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;

                Hashtable jsonObj = (Hashtable)JSON.JsonDecode(jsonStr);
                ArrayList menu_ids = (ArrayList)jsonObj["IDs"];
                foreach (string menu_id in menu_ids)
                {
                    Hashtable items = new Hashtable();
                    items.Add("menu_x_pri_menu_id", Convert.ToInt32(menu_id));
                    items.Add("menu_x_pri_privilege_id", Convert.ToInt32(privilege_id));
                    MenuXPrivilege menuxpri = new MenuXPrivilege(constr);
                    menuxpri.Add(items);
                }
                Response.BufferOutput = true;
                Response.Redirect("~/privileges.aspx?privilegeid=" + privilege_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string privilege_id = ComboBoxPrivileges.SelectedValue;
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                Privilege privilege = new Privilege(constr);
                privilege.Delete(Convert.ToInt32(privilege_id));
                Response.BufferOutput = true;
                Response.Redirect("~/privileges.aspx?privilegeid=1",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string privilege_id = ComboBoxPrivileges.SelectedValue;
                Response.BufferOutput = true;
                Response.Redirect("~/privilegeedit.aspx?mode=add&privilegeid=" + privilege_id,false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string privilege_id = ComboBoxPrivileges.SelectedValue;
                Response.BufferOutput = true;
                Response.Redirect("~/privilegeedit.aspx?mode=edit&privilegeid=" + privilege_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}