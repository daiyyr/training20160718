using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using Sapp.Common;
using Sapp.JQuery;
using Sapp.SMS;
using Sapp.Data;
using Sapp.General;

namespace sapp_sms
{
    public partial class ownerships : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Javascript setup
            Control[] wc = { jqGridOwnerships };
            JSUtils.RenderJSArrayWithCliendIds(Page, wc);
            #endregion
            if (!IsPostBack)
            {
                if (Request.QueryString["unitid"] != null) Session["unitid"] = Request.QueryString["unitid"].ToString();
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
        public static string jqGridOwnershipsDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string unit_id = HttpContext.Current.Session["unitid"].ToString();
                string sqlfromstr = "FROM (`ownerships` LEFT JOIN `debtor_master` ON `ownership_debtor_id`=`debtor_master_id`) WHERE `ownership_unit_id`=" + unit_id;
                string sqlselectstr = "SELECT `ID`,`Debtor` ,`Start`, `End`, `Notes` FROM (SELECT `debtor_master_id` AS `ID`, `debtor_master_name` AS `Debtor`,"
                    + " `ownership_start` AS `Start`, `ownership_end` AS `End`, `ownership_notes` AS `Notes`";
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
        public static string BindDebtorSelector()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                DebtorMaster deb = new DebtorMaster(constr);
                List<DebtorMaster> deblist = deb.GetDebtorList();
                foreach (DebtorMaster debtor in deblist)
                {
                    html += "<option value='" + debtor.DebtorMasterId.ToString() + "'>" + debtor.DebtorMasterName + "</option>";
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
        public static string SaveDataFromGrid(string rowValue)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string unit_id = HttpContext.Current.Session["unitid"].ToString();
            Object c = JSON.JsonDecode(rowValue);
            Hashtable hdata = (Hashtable)c;
            string line_id = hdata["ID"].ToString();
            Ownership ownership = new Ownership(constr);
            Hashtable items = new Hashtable();
            items.Add("ownership_unit_id", unit_id);
            string[] _split = hdata["Debtor"].ToString().Split('|');
            string debtor_code = _split[0].Substring(0, _split[0].Length - 1);
            DebtorMaster debtor = new DebtorMaster(constr);
            debtor.LoadData(debtor_code);
            items.Add("ownership_debtor_id", debtor.DebtorMasterId);
            items.Add("ownership_start", DBSafeUtils.DateToSQL(hdata["Start"]));
            items.Add("ownership_end", DBSafeUtils.DateToSQL(hdata["End"]));
            items.Add("ownership_notes", DBSafeUtils.StrToQuoteSQL(hdata["Notes"].ToString()));
            if (line_id == "")
            {
                ownership.Add(items);
            }
            else
            {
                ownership.Update(items, Convert.ToInt32(line_id));
            }
            return "dd";
        }
        #endregion

        #region WebControl Events

        private void ImageButtonDelete_Click(string[] args)
        {
            string ownership_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                ownership_id = args[1];
                Ownership ownership = new Ownership(constr);
                ownership.Delete(Convert.ToInt32(ownership_id));
                Response.BufferOutput = true;
                Response.Redirect("~/goback.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}