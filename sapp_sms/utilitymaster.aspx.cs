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

namespace sapp_sms
{
    public partial class utilitymaster : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridUtilityMaster };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
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
        public static string DataGridUtilityMasterDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string sqlfromstr = "FROM (`utility_master` left join `utility_types` on `utility_master_type_id`=`utility_type_id`)";
                string sqlselectstr = "SELECT `ID`,`Type`, `UnitPrice` from (select `utility_master_id` as `ID` , `utility_type_code` as `Type`, `utility_master_unit_price` as `UnitPrice` ";
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
                Response.Redirect("~/utilitymasteredit.aspx?mode=add", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            string utilitymaster_id = "";
            try
            {
                utilitymaster_id = args[1];

                Response.BufferOutput = true;
                Response.Redirect("~/utilitymasteredit.aspx?mode=edit&utilitymasterid=" + utilitymaster_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        private void ImageButtonDetails_Click(string[] args)
        {
            string utilitymaster_id = "";
            try
            {
                utilitymaster_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/utilitymasterdetails.aspx?utilitymasterid=" + utilitymaster_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void ImageButtonDelete_Click(string[] args)
        {
            string utility_master_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                utility_master_id = args[1];
                UtilityMaster utilitymaster = new UtilityMaster(constr);
                utilitymaster.Delete(Convert.ToInt32(utility_master_id));
                Response.BufferOutput = true;
                Response.Redirect("~/utilitymaster.aspx",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}
