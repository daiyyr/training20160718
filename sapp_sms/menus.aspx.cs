using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;
using System.Configuration;
using System.Collections;
using Sapp.Common;
using Sapp.Data;
using Sapp.JQuery;

namespace sapp_sms
{
    public partial class menus : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string menu_id = "";
                if (Request.QueryString["menuid"] != null) menu_id = Request.QueryString["menuid"];
                #region Javascript setup
                Control[] wc = { jqGridMenus };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                    #region Initial TreeView
                    Sapp.General.Menus menusObj = new Sapp.General.Menus(constr);
                    menusObj.LoadData();
                    TreeViewMenus.Nodes.Add(menusObj.GetTreeNode());
                    TreeNode tn = TreeViewMenus.Nodes[0];
                    TreeViewNodeSelected(tn, menu_id);
                    #endregion
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
                if (args[0] == "ButtonSubEdit")
                {
                    ButtonSubEdit_Click(args);
                }
                else if (args[0] == "ButtonSubDelete")
                {
                    ButtonSubDelete_Click(args);
                }
                else if (args[0] == "ButtonMoveUp")
                {
                    ButtonMoveUp_Click(args);
                }
                else if (args[0] == "ButtonMoveDown")
                {
                    ButtonMoveDown_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethod For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridMenusDataBind(string postdata, string menuid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string sqlfromstr = "FROM `menus` WHERE `menu_parent_id`=" + menuid;
                string sqlselectstr = "SELECT `ID`, `Name`, `Dir`, `ModuleId`, `Order` FROM (SELECT `menu_id` AS `ID`, `menu_name` AS `Name`, `menu_dir` AS `Dir`,"
                    + " `menu_module_id` AS `ModuleId`, `menu_order` AS `Order`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        #endregion

        #region WebControl Events

        protected void ImageButtonAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string menu_id = "";
                if (Request.QueryString["menuid"] != null) menu_id = Request.QueryString["menuid"];
                Response.BufferOutput = true;
                Response.Redirect("~/menuedit.aspx?mode=add&menuid=" + menu_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ButtonSubAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string menu_id = "";
                if (Request.QueryString["menuid"] != null) menu_id = Request.QueryString["menuid"];
                Response.BufferOutput = true;
                Response.Redirect("~/menuedit.aspx?mode=addsub&menuid=" + menu_id,false);
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
                string menu_id = "";
                if (Request.QueryString["menuid"] != null) menu_id = Request.QueryString["menuid"];
                Response.BufferOutput = true;
                Response.Redirect("~/menuedit.aspx?mode=edit&menuid=" + menu_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ButtonSubEdit_Click(string[] args)
        {
            string menu_id = "";
            try
            {
                menu_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/menuedit.aspx?mode=edit&menuid=" + menu_id, false);
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
                string menu_id = "";
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (Request.QueryString["menuid"] != null) menu_id = Request.QueryString["menuid"];
                Sapp.Telco.Menu menu = new Sapp.Telco.Menu(constr);
                menu.Delete(Convert.ToInt32(menu_id));
                Response.BufferOutput = true;
                Response.Redirect("~/menus.aspx?menuid=1",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ButtonSubDelete_Click(string[] args)
        {
            string menu_id = "";
            try
            {
                menu_id = args[1];
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                Sapp.General.Menu menu = new Sapp.General.Menu(constr);
                menu.Delete(Convert.ToInt32(menu_id));
                Response.BufferOutput = true;
                Response.Redirect("~/menus.aspx?menuid=1", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        public void TreeViewMenus_SelectedNodeChanged(object sender, EventArgs e)
        {
            try
            {
                if (TreeViewMenus.SelectedNode.Text != "Roots")
                {
                    Hashtable items = (Hashtable)JSON.JsonDecode(TreeViewMenus.SelectedValue);
                    string menu_id = items["menu_id"].ToString();
                    Response.BufferOutput = true;
                    Response.Redirect("~/menus.aspx?menuid=" + menu_id, false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void TreeViewNodeSelected(TreeNode tn, string menu_id)
        {
            try
            {
                if (tn.Text != "Roots")
                {
                    Hashtable items = (Hashtable)JSON.JsonDecode(tn.Value);
                    if (menu_id == items["menu_id"].ToString())
                    {
                        tn.Selected = true;
                        return;
                    }
                }
                if (tn.ChildNodes.Count > 0)
                {
                    for (int i = 0; i < tn.ChildNodes.Count; i++)
                    {
                        TreeViewNodeSelected(tn.ChildNodes[i], menu_id);
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ButtonMoveUp_Click(string[] args)
        {
            string menu_id = "";
            try
            {
                menu_id = args[1];
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                Sapp.General.Menu menu = new Sapp.General.Menu(constr);
                menu.LoadData(Convert.ToInt32(menu_id));
                menu.MoveUp();
                Response.BufferOutput = true;
                Response.Redirect("~/menus.aspx?menuid=" + Request.QueryString["menuid"],false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ButtonMoveDown_Click(string[] args)
        {
            string menu_id = "";
            try
            {
                menu_id = args[1];
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                Sapp.General.Menu menu = new Sapp.General.Menu(constr);
                menu.LoadData(Convert.ToInt32(menu_id));
                menu.MoveDown();
                Response.BufferOutput = true;
                Response.Redirect("~/menus.aspx?menuid=" + Request.QueryString["menuid"],false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonMoveUp_Click(object sender, ImageClickEventArgs e)
        {
            string menu_id = "";
            try
            {
                menu_id = Request.QueryString["menuid"];
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                Sapp.General.Menu menu = new Sapp.General.Menu(constr);
                menu.LoadData(Convert.ToInt32(menu_id));
                menu.MoveUp();
                Response.BufferOutput = true;
                Response.Redirect("~/menus.aspx?menuid=" + menu_id,false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonMoveDown_Click(object sender, ImageClickEventArgs e)
        {
            string menu_id = "";
            try
            {
                menu_id = Request.QueryString["menuid"];
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                Sapp.General.Menu menu = new Sapp.General.Menu(constr);
                menu.LoadData(Convert.ToInt32(menu_id));
                menu.MoveDown();
                Response.BufferOutput = true;
                Response.Redirect("~/menus.aspx?menuid=" + menu_id,false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        

        


    }
}