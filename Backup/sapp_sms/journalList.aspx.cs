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
    public partial class journalList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridActivity };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                if (!IsPostBack)
                    if (Request.QueryString["s"] != null)
                    {
                        string v = Request.QueryString["s"].ToString();
                        Session["Select"] = v;
                        DropDownList1.SelectedValue = v;
                    }
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
            decimal income = 0;
            decimal expense = 0;
            decimal deposit = 0;
            decimal payment = 0;
            decimal Journal = 0;
            Bodycorp bodycorp = new Bodycorp(constr);
            bodycorp.LoadData(Convert.ToInt32(AdFunction.BodyCorpID));
            DataTable dt = bodycorp.GetActivity(DateTime.Now.AddYears(-100), DateTime.Now.AddYears(100));
            dt = ReportDT.FilterDT(dt, "Type='Journal'");
            string select = AdFunction.GetSessionString("Select");

            if (select.Equals("JNL"))
                dt = ReportDT.FilterDT(dt, "TypeID='6'");
            if (select.Equals("CD"))
                dt = ReportDT.FilterDT(dt, "TypeID='7'");
            if (select.Equals("CP"))
                dt = ReportDT.FilterDT(dt, "TypeID='8'");
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
            string sqlselectstr = "SELECT `ID`, DATE_FORMAT(`Date`, '%d/%m/%Y'), `Ref`, `Description`, `Journal`,`Type`,`Rec`,`Rev` FROM (SELECT *";
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

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            if (DropDownList1.SelectedValue.Equals("JNL"))
                Response.Redirect("journalsedit.aspx?mode=bc");
            if (DropDownList1.SelectedValue.Equals("CD"))
                Response.Redirect("cashdeposit.aspx");
            if (DropDownList1.SelectedValue.Equals("CP"))
                Response.Redirect("cashpayment.aspx");
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["Select"] = DropDownList1.SelectedValue;
        }

        protected void ImageButton4_Click(object sender, ImageClickEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("BodycorpCode");
            dt.Columns.Add("UnitNum");
            dt.Columns.Add("ChartCode");
            dt.Columns.Add("DR");
            dt.Columns.Add("CR");
            dt.Columns.Add("Date");
            dt.Columns.Add("Description");

            CsvDT.DataTableToCsv(dt, Server.MapPath("~/Temp/JulTemplate.csv"));
            Response.Redirect("~/Temp/JulTemplate.csv");
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/journalupload.aspx");
        }
        //public string QuString(string text)
        //{
        //    return "'" + text + "'";
        //}
        //public string GLInsert(Odbc o, string bid, string oldrefernce, string oldid, string typeid, string reftyprid, string reference, string chartid, string unitid, string description, string net, string date, string createdate, string rev, string rec, string tax = "0", string gross = "0", string creditorID = "null")
        //{
        //    string gl_insert = "insert into gl_transactions (`gl_transaction_oldref`,`gl_trasaction_oldid`,`gl_transaction_type_id`,`gl_transaction_ref_type_id`,`gl_transaction_ref`,`gl_transaction_chart_id`,`gl_transaction_bodycorp_id`,`gl_transaction_unit_id`,`gl_transaction_description`,`gl_transaction_net`,`gl_transaction_tax`,`gl_transaction_gross`,`gl_transaction_date`,`gl_transaction_createdate`,`gl_transaction_rev`,`gl_transaction_rec`,`gl_transaction_creditor_id`)";
        //    gl_insert += " values (" + QuString(oldrefernce) + "," + QuString(oldid) + "," + typeid + "," + reftyprid + ",'" + reference + "'," + chartid + "," + bid + "," + unitid + ",'" + description + "'," + net + "," + tax + "," + gross + ",'" + date + "','" + createdate + "'," + rev + "," + rec + "," + creditorID + ")";
        //    o.ExecuteScalar(gl_insert);
        //    return newDB.ExecuteScalar("SELECT LAST_INSERT_ID()").ToString();
        //}

        //protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        //{
        //    string jnlNum = Journal.GetNextNumber();
        //    Odbc o = new Odbc(AdFunction.conn);
        //    DataTable dt = CsvDT.CsvToDataTable();
        //}

    }
}