using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using Sapp.Common;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.SMS;
using Sapp.General;

namespace sapp_sms
{
    public partial class bodycorpmanagers : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Javascript setup
            Control[] wc = { jqGridManagers };
            JSUtils.RenderJSArrayWithCliendIds(Page, wc);
            #endregion
            if (Request.Cookies["bodycorpid"].Value != null)
                Session["bodycorpid"] = Request.Cookies["bodycorpid"].Value;
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            try
            {
                string[] args = eventArgument.Split('|');
                if (args[0] == "ImageButtonDelete")
                {
                    ImageButtonDelete_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string jqGridManagersDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = HttpContext.Current.Session["bodycorpid"].ToString();
                string sqlfromstr = "FROM (`bodycorp_managers` LEFT JOIN `sapp_general`.`users` ON `bodycorp_manager_user_id`=`user_id`) WHERE (`bodycorp_manager_bodycorp_id`=" + bodycorp_id + ")";
                string sqlselectstr = "SELECT `ID`,`Login`, `Name` ,`Type` FROM (SELECT `bodycorp_manager_id` AS `ID`, `user_login` AS `Login`, `user_name` AS `Name`, `bodycorp_manager_type` AS `Type`";
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
        [System.Web.Services.WebMethod]
        public static string SaveDataFromGrid(string rowValue)
        {
            Object c = JSON.JsonDecode(rowValue);
            string bodycorp_id = HttpContext.Current.Session["bodycorpid"].ToString();
            Hashtable hdata = (Hashtable)c;
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string line_id = hdata["ID"].ToString();
            Sapp.General.User user = new Sapp.General.User(constr_general);
            string user_login = hdata["Login"].ToString();
            user.LoadData(user_login);
            Hashtable items = new Hashtable();
            items.Add("bodycorp_manager_bodycorp_id", Convert.ToInt32(bodycorp_id));
            items.Add("bodycorp_manager_user_id", user.UserId);
            items.Add("bodycorp_manager_type", DBSafeUtils.StrToQuoteSQL(hdata["Type"].ToString()));
            BodycorpManager bm = new BodycorpManager(constr);
            if (line_id == "")
            {
                bm.Add(items);
            }
            else
            {
                bm.Update(items, Convert.ToInt32(line_id));
            }
            return "dd";
        }

        [System.Web.Services.WebMethod]
        public static string BindLoginSelector()
        {
            try
            {
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string html = "<select>";
                Sapp.General.User user = new Sapp.General.User(constr_general);
                List<User> userList = user.GetUserList();
                foreach (User _usr in userList)
                {
                    html += "<option value='" + _usr.UserId.ToString() + "'>" + _usr.UserLogin + "</option>";
                }
                html += "</select>";
                return html;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }

        [System.Web.Services.WebMethod]
        public static string BindTypeSelector()
        {
            try
            {
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string html = "<select>";
                ArrayList typeList = new ArrayList();
                typeList.Add("READ");
                typeList.Add("WRITE");
                foreach (string type in typeList)
                {
                    html += "<option value='" + type + "'>" + type + "</option>";
                }
                html += "</select>";
                return html;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        #endregion

        #region WebControl Events

        private void ImageButtonDelete_Click(string[] args)
        {
            string bm_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                bm_id = args[1];
                BodycorpManager bm = new BodycorpManager(constr);
                bm.Delete(Convert.ToInt32(bm_id)); 
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}