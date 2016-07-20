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
    public partial class unittypes : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridUnitTypes };
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
        public static string DataGridUnitTypesDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string sqlfromstr = "FROM (`unit_types`)";
                string sqlselectstr = "SELECT `ID`,`Code`,`Name`,`Description` from (select `unit_type_id` AS `ID`,`unit_type_code` AS `Code`,`unit_type_name` AS `Name`,`unit_type_description` AS `Description` ";
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
            Sapp.SMS.UnitType unittype = new Sapp.SMS.UnitType(constr);
            Hashtable items=new Hashtable();
            items.Add("unit_type_code", DBSafeUtils.StrToQuoteSQL(hdata["Code"].ToString()));
            items.Add("unit_type_name", DBSafeUtils.StrToQuoteSQL(hdata["Name"].ToString()));
            items.Add("unit_type_description", DBSafeUtils.StrToQuoteSQL(hdata["Description"].ToString()));
            if (line_id == "")
            {
                unittype.Add(items);
            }
            else
            {
                unittype.Update(items, Convert.ToInt32(line_id));
            }
            return "dd";
        }
        #endregion

        #region WebControl Events

        private void ImageButtonDelete_Click(string[] args)
        {
            string unit_type_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                unit_type_id = args[1];
                Sapp.SMS.UnitType unittype = new Sapp.SMS.UnitType(constr);
                unittype.Delete(Convert.ToInt32(unit_type_id));
                Response.BufferOutput = true;
                Response.Redirect("~/unittypes.aspx",false);
            }
            catch (System.Data.Odbc.OdbcException ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}
