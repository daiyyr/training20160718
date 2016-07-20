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
using System.Text.RegularExpressions;


namespace sapp_sms
{
    public partial class accounts : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        private string CheckedQueryString()
        {
            if (null != Request.Cookies["bodycorpid"].Value && Regex.IsMatch(Request.Cookies["bodycorpid"].Value, "^[0-9]*$"))
            {
                return Request.Cookies["bodycorpid"].Value;
            }
            return "";
        }
        private string NewQueryString(string connector)
        {
            string result = CheckedQueryString();
            return string.IsNullOrEmpty(result) ? "" : connector + "bodycorpid=" + result;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridAccounts };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if ("XMLHttpRequest" != Request.Headers["X-Requested-With"])
                {
                    Session["bodycorpid"] = CheckedQueryString();
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
                if (args[0] == "ImageButtonEdit")
                {
                    ImageButtonEdit_Click(args);
                }
                else if (args[0] == "ImageButtonDelete")
                {
                    ImageButtonDelete_Click(args);
                }
                else if (args[0] == "ImageButtonDetails")
                {
                    ImageButtonDetails_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridAccountsDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string sqlfromstr = "FROM (`accounts`)";
                //if (null != HttpContext.Current.Session["bodycorpid"] && !String.IsNullOrEmpty(HttpContext.Current.Session["bodycorpid"].ToString()))
                //{
                //    sqlfromstr += "where account_id in (select bodycorp_account_id from bodycorps where bodycorp_id=" + HttpContext.Current.Session["bodycorpid"].ToString() + ")";
                //}
                string sqlselectstr = "SELECT `ID`, `Code`, `Name`, `Number` FROM (SELECT `account_id` AS `ID`, `account_code` AS `Code`, `account_name` AS `Name`, `account_num` AS `Number`";
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
                Response.BufferOutput = true;
                Response.Redirect("~/accountedit.aspx?mode=add" + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            string account_id = "";
            try
            {
                account_id = args[1];
                
                Response.BufferOutput = true;
                Response.Redirect("~/accountedit.aspx?mode=edit&accountid=" + account_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonDelete_Click(string[] args)
        {
            string account_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                account_id = args[1];
                Account account = new Account(constr);
                account.Delete(Convert.ToInt32(account_id));
                Response.BufferOutput = true;
                Response.Redirect("~/accounts.aspx" + NewQueryString("?"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonDetails_Click(string[] args)
        {
            string account_id = "";
            try
            {
                account_id = args[1];
                Session["accountid"] = "1";
                Response.BufferOutput = true;
                Response.Redirect("~/accountdetails.aspx?accountid=" + account_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}