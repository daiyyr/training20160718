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
    public partial class pptyinstypes : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridTypes };
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
        public static string DataGridDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string sqlfromstr = "FROM (`pptyins_types`)";
                string sqlselectstr = "SELECT `ID`,`Code`,`Name`,`Description` from (select `pptyins_type_id` AS `ID`,`pptyins_type_code` AS `Code`,`pptyins_type_name` AS `Name`,`pptyins_type_description` AS `Description` ";
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
            PptyinsType pptyinstype = new PptyinsType(constr);
            Hashtable items = new Hashtable();
            items.Add("pptyins_type_code", DBSafeUtils.StrToQuoteSQL(hdata["Code"].ToString()));
            items.Add("pptyins_type_name", DBSafeUtils.StrToQuoteSQL(hdata["Name"].ToString()));
            items.Add("pptyins_type_description", DBSafeUtils.StrToQuoteSQL(hdata["Description"].ToString()));
            if (line_id == "")
            {
                pptyinstype.Add(items);
            }
            else
            {
                pptyinstype.Update(items, Convert.ToInt32(line_id));
            }
            return "dd";
        }
        #endregion

        #region WebControl Events

        private void ImageButtonDelete_Click(string[] args)
        {
            string pptyinstype_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                pptyinstype_id = args[1];
                PptyinsType pptyinstype = new PptyinsType(constr);
                pptyinstype.Delete(Convert.ToInt32(pptyinstype_id));
                Response.BufferOutput = true;
                Response.Redirect("~/pptyinstypes.aspx",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}