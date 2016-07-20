using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Collections;
using Sapp.Common;
using Sapp.Data;
using Sapp.SMS;
using Sapp.JQuery;

namespace sapp_sms
{
    public partial class activityunit : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridActivityUnit };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                if (Request.QueryString["id"] != null) Session["activityunit_id"] = Request.QueryString["id"].ToString();
                if (Request.QueryString["start"] != null) Session["activityunit_start"] = Request.QueryString["start"].ToString();
                if (Request.QueryString["end"] != null) Session["activityunit_end"] = Request.QueryString["end"].ToString();
                UnitMaster u = new UnitMaster(AdFunction.conn);
                u.LoadData(int.Parse(Request.QueryString["id"].ToString()));
                Label1.Text = u.UnitMasterCode;
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
                if (args[0] == "Detail")
                {

                    Detail(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        public void Detail(string[] args)
        {
            try
            {
                if (Session["TempActivityUnitDT"] == null)
                    jqGridAUDataBind("");
                string jsonStr = args[1];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable jsonObj = (Hashtable)JSON.JsonDecode(jsonStr);
                ArrayList ids = (ArrayList)jsonObj["IDs"];
                DataTable dt = (DataTable)Session["TempActivityUnitDT"];
                for (int i = 0; i < ids.Count; i++)
                {
                    string t = ReportDT.GetDataByColumn(dt, "ID", ids[i].ToString(), "Invoice");
                    if (!t.Equals(""))
                        Response.Write("<script>window.showModalDialog(WebFrom1?id=" + ids[i] + "&type=Invoice, \"#1\", \"dialogHeight: 500px; dialogWidth: 650px; edge: Raised; center: Yes;help: No; resizable: No; status: No; scroll: No;\");\")</script>");
                    //Response.Redirect("~/activityunitdetail.aspx?id=" + ids[i] + "&type=Invoice");
                    else
                        Response.Redirect("~/activityunitdetail.aspx?id=" + ids[i] + "&type=Recepit", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string jqGridAUDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string id = HttpContext.Current.Session["activityunit_id"].ToString();
            string start = HttpContext.Current.Session["activityunit_start"].ToString();
            string end = HttpContext.Current.Session["activityunit_end"].ToString();
            //decimal invoice = 0;
            //decimal receipt = 0;
            //decimal balance = 0;

            UnitMaster unit = new UnitMaster(constr);
            unit.LoadData(Convert.ToInt32(id));
            DataTable dt = unit.GetActivity(Convert.ToDateTime(start), Convert.ToDateTime(end));
            //dt = dt.DefaultView.ToTable();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    if (dr["Invoice"] != DBNull.Value)
            //    {
            //        if (dr["Invoice"].ToString() != "") invoice += Convert.ToDecimal(dr["Invoice"]);
            //    }
            //    if (dr["Receipt"] != DBNull.Value)
            //    {
            //        if (dr["Receipt"].ToString() != "") receipt += Convert.ToDecimal(dr["Receipt"]);
            //    }
            //    if (dr["Balance"] != DBNull.Value)
            //    {
            //        if (dr["Balance"].ToString() != "") balance += Convert.ToDecimal(dr["Balance"]);
            //        dr["Balance"] = balance.ToString("0.00");
            //    }
            //    else
            //    {
            //        dr["Balance"] = balance.ToString("0.00");
            //    }
            //}
            HttpContext.Current.Session["AUDT"] = dt;
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`, `InvDate`, `DueDate`, `Ref`, `Description`, `Invoice`,`Receipt`, `Balance`,`Journal`,`Type`,`Rec`,`Rev` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            Hashtable userdata = new Hashtable();
            userdata.Add("Description", "Total");
            userdata.Add("Invoice", ReportDT.SumTotal(dt, "Invoice"));
            userdata.Add("Receipt", ReportDT.SumTotal(dt, "Receipt"));
            userdata.Add("Balance", ReportDT.SumTotal(dt, "Invoice") - ReportDT.SumTotal(dt, "Receipt"));
            jqgridObj.SetUserData(userdata);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }
        #endregion

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            DataTable dt = new DataTable();
            string path = "~/temp/ActivityUnit.csv";
            string fpath = Server.MapPath(path);
            if (Session["AUDT"] != null)
            {
                dt = (DataTable)Session["AUDT"];
            }
            else
            {
                return;
            }
            CsvDT.DataTableToCsv(dt, fpath);
            Response.Redirect(path, false);
        }

        protected void DetailB_Click(object sender, ImageClickEventArgs e)
        {

        }
    }
}