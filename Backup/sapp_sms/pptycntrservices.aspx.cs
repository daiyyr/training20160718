using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using System.Configuration;
using Sapp.JQuery;
using System.Collections;
using Sapp.SMS;
using Sapp.Data;

namespace sapp_sms
{
    public partial class pptycntrservices : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridPptycntrservices };
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
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridPaymentTypesDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string sqlfromstr = "FROM (`pptycntr_services`)";
                string sqlselectstr = "SELECT `ID`,`Code`,`Name`,`Description` from (select `pptycntr_service_id` AS `ID`,`pptycntr_service_code` AS `Code`,`pptycntr_service_name` AS `Name`,`pptycntr_service_description` AS `Description` ";
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
            Hashtable hdata = (Hashtable)c;
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string line_id = hdata["ID"].ToString();
            PptycntrService pptycntrservice = new PptycntrService(constr);
            Hashtable items = new Hashtable();
            items.Add("pptycntr_service_code", DBSafeUtils.StrToQuoteSQL(hdata["Code"].ToString()));
            items.Add("pptycntr_service_name", DBSafeUtils.StrToQuoteSQL(hdata["Name"].ToString()));
            items.Add("pptycntr_service_description", DBSafeUtils.StrToQuoteSQL(hdata["Description"].ToString()));
            if (line_id == "")
            {
                pptycntrservice.Add(items);
            }
            else
            {
                pptycntrservice.Update(items, Convert.ToInt32(line_id));
            }
            return "dd";
        }
        #endregion

        #region WebControl Events

        private void ImageButtonDelete_Click(string[] args)
        {
            string pptycntr_service_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                pptycntr_service_id = args[1];
                PptycntrService pptycntrservice = new PptycntrService(constr);
                pptycntrservice.Delete(Convert.ToInt32(pptycntr_service_id));
                Response.BufferOutput = true;
                Response.Redirect("~/pptycntrservices.aspx",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}