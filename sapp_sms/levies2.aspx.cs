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
    public partial class levies2 : System.Web.UI.Page, IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridLevies };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                if (Request.QueryString["bid"] != null)
                    Session["bid"] = Request.QueryString["bid"].ToString();

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
                if (args[0] == "ImageButtonInvoice")
                {
                    ImageButtonInvoice_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string jqGridLeviesDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Odbc mydb = new Odbc(constr);
                string id = "";
                if (HttpContext.Current.Session["bid"] != null) id = HttpContext.Current.Session["bid"].ToString();
                string sql = "SELECT `levy_id` AS `ID` , `bodycorp_code` AS `Bodycorp` , DATE_FORMAT( `levy_date` , '%d/%m/%Y' ) AS `Date` , `levy_description` AS `Description` , `levy_net` AS `Net` FROM (`levies` LEFT JOIN `bodycorps` ON `levy_bodycorp_id` = `bodycorp_id`) where levy_batch_id=" + id;
                DataTable dt = mydb.ReturnTable(sql, "tempdt");
                string sqlselectstr = "SELECT `ID`, `Bodycorp`, `Date`, `Description`,`Net` FROM (SELECT * ";
                string sqlfromstr = "FROM `" + dt.TableName + "`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return "{}";
        }

        #endregion

        #region WebControl Events
        private void ImageButtonInvoice_Click(string[] args)
        {
            try
            {
                string levy_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/invoicemasterOLD.aspx?levyid=" + levy_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (Session["LevyFinish"] != null)
            {
                if (!(bool)Session["LevyFinish"])
                {
                    Timer1.Enabled = true;
                    Image1.Visible = true;
                }
                else
                {
                    Timer1.Enabled = false;
                    Image1.Visible = false;
                }
            }
            else
            {
                Timer1.Enabled = false;
                Image1.Visible = false;
            }
        }
    }
}