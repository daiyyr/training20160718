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
    public partial class activity : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridActivity };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                if (Request.QueryString["id"] != null) Session["activity_id"] = Request.QueryString["id"].ToString();
                if (Request.QueryString["start"] != null) Session["activity_start"] = Request.QueryString["start"].ToString();
                if (Request.QueryString["end"] != null) Session["activity_end"] = Request.QueryString["end"].ToString();
                Session["bodycorp_id"] = Request.QueryString["id"].ToString();
                #endregion
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string jqGridActivityDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string id = HttpContext.Current.Session["bodycorp_id"].ToString();
            string start = HttpContext.Current.Session["activity_start"].ToString();
            string end = HttpContext.Current.Session["activity_end"].ToString();
            decimal income = 0;
            decimal expense = 0;
            decimal deposit = 0;
            decimal payment = 0;
            decimal Journal = 0;
            Bodycorp bodycorp = new Bodycorp(constr);
            bodycorp.LoadData(Convert.ToInt32(id));
            DataTable dt = bodycorp.GetActivity(Convert.ToDateTime(start), Convert.ToDateTime(end));
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Income"] != DBNull.Value)
                {
                    if (dr["Income"].ToString() != "") income += Convert.ToDecimal(dr["Income"]);
                }
                if (dr["Expense"] != DBNull.Value)
                {
                    if (dr["Expense"].ToString() != "") expense += Convert.ToDecimal(dr["Expense"]);
                }
                if (dr["Deposit"] != DBNull.Value)
                {
                    if (dr["Deposit"].ToString() != "") deposit += Convert.ToDecimal(dr["Deposit"]);
                }
                if (dr["Payment"] != DBNull.Value)
                {
                    if (dr["Payment"].ToString() != "") payment += Convert.ToDecimal(dr["Payment"]);
                }
                if (!dr["Journal"].ToString().Equals(""))
                {
                    Journal += AdFunction.Rounded(dr["Journal"].ToString());
                }
            }
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`, DATE_FORMAT(`Date`, '%d/%m/%Y'), `Ref`, `Description`, `Income`,`Expense`, `Deposit`, `Payment`,`Journal`,`Type`,`Rec`,`Rev` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            Hashtable userdata = new Hashtable();
            userdata.Add("Description", "Total");
            userdata.Add("Income", income.ToString("0.00"));
            userdata.Add("Expense", expense.ToString("0.00"));
            userdata.Add("Deposit", deposit.ToString("0.00"));
            userdata.Add("Payment", payment.ToString("0.00"));
            userdata.Add("Journal", Journal.ToString("0.00"));
            jqgridObj.SetUserData(userdata);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }
        #endregion

    }
}