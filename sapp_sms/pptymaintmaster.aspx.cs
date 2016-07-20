using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.SMS;
using System.Configuration;
using System.Collections;
using System.Text.RegularExpressions;

namespace sapp_sms
{
    public partial class pptymaintmaster : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
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
        public static string DataGridPptymaintDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string sqlfromstr = "FROM (`pptymaint_master` left join `pptymaint_types`  on `pptymaint_master_type_id`=`pptymaint_type_id` left join `creditor_master`  on `pptymaint_master_creditor_id`=`creditor_master_id` left join `unit_master`      on `pptymaint_master_unit_id`=`unit_master_id`  left join `property_master`  on `property_master_id`=`pptymaint_master_property_id` left join `freqtypes`        on `pptymaint_master_freqtype_id`=`freqtype_id`) ";
                if (null != HttpContext.Current.Session["propertyid"] && !String.IsNullOrEmpty(HttpContext.Current.Session["propertyid"].ToString()))
                {
                    sqlfromstr += " WHERE pptymaint_master_property_id=" + HttpContext.Current.Session["propertyid"].ToString();
                }
                string sqlselectstr = "SELECT `ID`,`Type`,`Freq`,`NextDue`,`Contractor`,`Comply`,`Area`,`Property`,`Status` from (select  `pptymaint_master_id` as `ID`,`pptymaint_type_code` as `Type`,`pptymaint_master_freq` as `Freq`, DATE_FORMAT(`pptymaint_master_next_due`, '%d/%m/%Y') as `NextDue`,`creditor_master_code` as `Contractor`,REPLACE(REPLACE(`pptymaint_master_compliance`, '0', 'False'), '1', 'True') as `Comply`, `unit_master_code` as `Area`, `property_master_code` as `Property`,`freqtype_code` as `Status` ";
                
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
            string pptymaintmaster_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                pptymaintmaster_id = args[1];
                PptymaintMaster pptymaintmaster = new PptymaintMaster(constr);
                pptymaintmaster.Delete(Convert.ToInt32(pptymaintmaster_id));
                Response.BufferOutput = true;
                Response.Redirect("~/pptymaintmaster.aspx"+ NewQueryString("?"),false);
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
                Response.Redirect("~/pptymaintmasteredit.aspx?mode=add" + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            string pptymaintmaster_id = "";
            try
            {
                pptymaintmaster_id = args[1];

                Response.BufferOutput = true;
                Response.Redirect("~/pptymaintmasteredit.aspx?mode=edit&pptymaintmasterid=" + pptymaintmaster_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        private void ImageButtonDetails_Click(string[] args)
        {
            string pptymaintmaster_id = "";
            try
            {
                pptymaintmaster_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/pptymaintmasterdetails.aspx?pptymaintmasterid=" + pptymaintmaster_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}
