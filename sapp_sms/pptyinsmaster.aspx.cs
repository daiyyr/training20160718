using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Sapp.Common;
using System.Configuration;
using Sapp.JQuery;
using Sapp.SMS;

namespace sapp_sms
{
    public partial class pptyinsmaster : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        private string CheckedQueryString()
        {
            if (null != Request.QueryString["propertyid"] && Regex.IsMatch(Request.QueryString["propertyid"], "^[0-9]*$"))
            {
                return Request.QueryString["propertyid"];
            }
            return "";
        }
        private string NewQueryString(string connector)
        {
            string result = CheckedQueryString();
            return string.IsNullOrEmpty(result) ? "" : connector + "propertyid=" + result;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridTable };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                Session["propertyid"] = CheckedQueryString();
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
                if (args[0] == "ImageButtonDelete")
                {
                    ImageButtonDelete_Click(args);
                }
                else if (args[0] == "ImageButtonDetails")
                {
                    ImageButtonDetails_Click(args);
                }
                else if (args[0] == "ImageButtonEdit")
                {
                    ImageButtonEdit_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string sqlfromstr = "FROM (`pptyins_master` left join (`pptyins_types`,`property_master`) on (`pptyins_master_type_id`=`pptyins_type_id`  and `property_master_id`=`pptyins_master_property_id` ))";
                if (null != HttpContext.Current.Session["propertyid"] && !String.IsNullOrEmpty(HttpContext.Current.Session["propertyid"].ToString()))
                {
                    sqlfromstr += " WHERE pptyins_master_property_id=" + HttpContext.Current.Session["propertyid"].ToString();
                }
                string sqlselectstr = "SELECT `ID`, `Policy`, `Type`, `End`, `Cover` from (select  `pptyins_master_id` as `ID`, `pptyins_master_policy_num` AS `Policy`, `pptyins_type_code` as `Type`,"
                    + " DATE_FORMAT(`pptyins_master_end`, '%d/%m/%Y') as `End`, `pptyins_master_cover` AS `Cover` ";

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

        private void ImageButtonDelete_Click(string[] args)
        {
            string pptyinsmaster_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                pptyinsmaster_id = args[1];
                PptyinsMaster pptyinsmaster = new PptyinsMaster(constr);
                pptyinsmaster.Delete(Convert.ToInt32(pptyinsmaster_id));
                Response.BufferOutput = true;
                Response.Redirect("~/pptyinsmaster.aspx" + NewQueryString("?"),false);
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
                Response.BufferOutput = true;
                Response.Redirect("~/pptyinsmasteredit.aspx?mode=add" + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            string pptyinsmaster_id = "";
            try
            {
                pptyinsmaster_id = args[1];

                Response.BufferOutput = true;
                Response.Redirect("~/pptyinsmasteredit.aspx?mode=edit&pptyinsmasterid=" + pptyinsmaster_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        private void ImageButtonDetails_Click(string[] args)
        {
            string pptyinsmaster_id = "";
            try
            {
                pptyinsmaster_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/pptyinsmasterdetails.aspx?pptyinsmasterid=" + pptyinsmaster_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}