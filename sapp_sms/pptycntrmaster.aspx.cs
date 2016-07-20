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
    public partial class pptycntrmaster : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
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
                Control[] wc = { jqGridMaster };
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
                string sqlfromstr = "FROM (`pptycntr_master` left join (`pptycntr_services`,`creditor_master`, `property_master`) on (`pptycntr_master_service_id`=`pptycntr_service_id` and `pptycntr_master_creditor_id`=`creditor_master_id` and `pptycntr_master_property_id`=`property_master_id` ))";
                if (null != HttpContext.Current.Session["propertyid"] && !String.IsNullOrEmpty(HttpContext.Current.Session["propertyid"].ToString()))
                {
                    sqlfromstr += " WHERE pptycntr_master_property_id=" + HttpContext.Current.Session["propertyid"].ToString();
                }
                string sqlselectstr = "SELECT `ID`,`Service`,`Creditor`,`Property`,`Expiry` from (select  `pptycntr_master_id` as `ID`,`pptycntr_service_code` as `Service`,`creditor_master_code` as `Creditor`, `Property_Master_Code` as `Property`, DATE_FORMAT(`pptycntr_expiry`, '%d/%m/%Y') as `Expiry`";

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
            string pptycntrmaster_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                pptycntrmaster_id = args[1];
                PptycntrMaster pptymaintmaster = new PptycntrMaster(constr);
                pptymaintmaster.Delete(Convert.ToInt32(pptycntrmaster_id));
                Response.BufferOutput = true;
                Response.Redirect("~/pptycntrmaster.aspx" + NewQueryString("?"),false);
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
                Response.Redirect("~/pptycntrmasteredit.aspx?mode=add" + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            string pptycntrmaster_id = "";
            try
            {
                pptycntrmaster_id = args[1];

                Response.BufferOutput = true;
                Response.Redirect("~/pptycntrmasteredit.aspx?mode=edit&pptycntrmasterid=" + pptycntrmaster_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        private void ImageButtonDetails_Click(string[] args)
        {
            string pptycntrmaster_id = "";
            try
            {
                pptycntrmaster_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/pptycntrmasterdetails.aspx?pptycntrmasterid=" + pptycntrmaster_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}