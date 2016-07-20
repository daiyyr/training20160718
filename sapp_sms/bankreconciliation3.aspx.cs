using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Data.Odbc;
using Sapp.General;
using Sapp.Common;
using Sapp.Data;
using Sapp.SMS;
using Sapp.JQuery;

namespace sapp_sms
{
    public partial class bankreconciliation3 : System.Web.UI.Page
    {
        private const string TEMP_TYPE_RECONCILED = "bankreconciliation2_1";
        private const string TEMP_TYPE_UNRECONCILED = "bankreconciliation2_2";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string account_id = Request.QueryString["accountid"];
                if (account_id_HF.Value.Equals(""))
                    account_id_HF.Value = account_id;
                else
                    account_id = account_id_HF.Value;
                string start = Request.QueryString["start"];
                string end = Request.QueryString["end"];
                string batch = Request.QueryString["batch"];

                if (!IsPostBack)
                {


                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;

                    #region Initial Temp File In Mysql
                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(constr_general);
                    user.LoadData(login);
                    #region Get Reconciled JSON
                    ReconcileTemps reconciled = new ReconcileTemps(user.UserId, TEMP_TYPE_RECONCILED, constr_general, constr);
                    Hashtable reconciled_items = new Hashtable();
                    ArrayList reconciled_list = new ArrayList();
                    reconciled_items.Add("reconciles", reconciled_list);
                    #endregion
                    reconciled.Add(JSON.JsonEncode(reconciled_items));
                    ReconcileTemps unreconciled = new ReconcileTemps(user.UserId, TEMP_TYPE_UNRECONCILED, constr_general, constr);
                    Odbc mydb = null;
                    Hashtable items = new Hashtable();
                    try
                    {

                        Odbc o = new Odbc(AdFunction.conn);
                        decimal openbalance = 0;

                        string startsql = "select * from gl_transactions where (gl_transaction_chart_id = " + account_id + ") and gl_transaction_recbatchid =" + batch + " order by gl_transaction_date";
                        DataTable startDT = o.ReturnTable(startsql, "t1");
                        if (startDT.Rows.Count > 0)
                        {
                            start = (DateTime.Parse(startDT.Rows[0]["gl_transaction_date"].ToString())).ToString("yyyy-MM-dd");

                        }

                        string opsql = "SELECT sum(`cpayment_gross`) FROM `cpayments` WHERE `cpayment_bodycorp_id` IN (SELECT `bodycorp_id` FROM `bodycorps` WHERE `bodycorp_account_id`=" + account_id + ") AND `cpayment_date`<" + DBSafeUtils.DateToSQL(start) + " AND `cpayment_reconciled`=1 ";
                        openbalance -= AdFunction.Rounded(o.ReturnTable(opsql, "sum").Rows[0][0].ToString());
                        opsql = "SELECT sum(`receipt_gross`) FROM `receipts` WHERE `receipt_bodycorp_id` IN (SELECT `bodycorp_id` FROM `bodycorps` WHERE `bodycorp_account_id`=" + account_id + ") AND `receipt_date`<" + DBSafeUtils.DateToSQL(start) + " AND `receipt_reconciled`=1";
                        openbalance += AdFunction.Rounded(o.ReturnTable(opsql, "sum").Rows[0][0].ToString());
                        opsql = "SELECT sum(`gl_transaction_net`) FROM      gl_transactions WHERE  (gl_transaction_rec =1) and (gl_transaction_type_id = 7) AND (gl_transaction_chart_id = " + account_id + ") and `gl_transaction_date`<" + DBSafeUtils.DateToSQL(start);
                        openbalance -= AdFunction.Rounded(o.ReturnTable(opsql, "sum").Rows[0][0].ToString());
                        opsql = "SELECT sum(`gl_transaction_net`) FROM      gl_transactions WHERE (gl_transaction_rec =1) and   (gl_transaction_type_id = 8) AND (gl_transaction_chart_id = " + account_id + ") and `gl_transaction_date`<" + DBSafeUtils.DateToSQL(start);
                        openbalance -= AdFunction.Rounded(o.ReturnTable(opsql, "sum").Rows[0][0].ToString());
                        opsql = "SELECT sum(`gl_transaction_net`) FROM      gl_transactions WHERE (gl_transaction_rec =1) and   (gl_transaction_type_id = 6) AND (gl_transaction_chart_id = " + account_id + ") and `gl_transaction_date`<" + DBSafeUtils.DateToSQL(start);
                        openbalance -= AdFunction.Rounded(o.ReturnTable(opsql, "sum").Rows[0][0].ToString());

                        OpenBalanceL.Text = openbalance.ToString();
                        Session["OpeningBalance"] = openbalance;

                        DataTable creditor = ReportDT.getTable(constr, "creditor_master");
                        DataTable debort = ReportDT.getTable(constr, "debtor_master");
                        mydb = new Odbc(constr);
                        ArrayList list = new ArrayList();
                        string sql = "SELECT * FROM `cpayments` WHERE `cpayment_bodycorp_id` IN (SELECT `bodycorp_id` FROM `bodycorps` WHERE `bodycorp_account_id`=" + account_id + ") AND `cpayment_date`>=" + DBSafeUtils.DateToSQL(start) + "and `cpayment_date`<=" + DBSafeUtils.DateToSQL(end) + "  AND `cpayment_reconciled`=1 And cpayment_recbatchid=" + batch;
                        OdbcDataReader dr = mydb.Reader(sql);
                        while (dr.Read())
                        {
                            Hashtable list_items = new Hashtable();
                            list_items.Add("reconcile_id", "p" + dr["cpayment_id"].ToString());
                            list_items.Add("reconcile_date", DBSafeUtils.DBDateToStr(dr["cpayment_date"]));
                            list_items.Add("reconcile_ref", dr["cpayment_reference"].ToString());
                            list_items.Add("reconcile_description", "");
                            list_items.Add("reconcile_deposit", "");
                            list_items.Add("CD", ReportDT.GetDataByColumn(creditor, "creditor_master_id", dr["cpayment_creditor_id"].ToString(), "creditor_master_code"));
                            list_items.Add("reconcile_payment", Convert.ToDecimal(dr["cpayment_gross"]).ToString("0.00"));
                            list.Add(list_items);
                        }
                        sql = "SELECT * FROM `receipts` WHERE `receipt_bodycorp_id` IN (SELECT `bodycorp_id` FROM `bodycorps` WHERE `bodycorp_account_id`=" + account_id + ") AND `receipt_date`>=" + DBSafeUtils.DateToSQL(start) + " AND `receipt_date`<=" + DBSafeUtils.DateToSQL(end) + "  AND `receipt_reconciled`=1 and receipt_recbatchid=" + batch;
                        dr = mydb.Reader(sql);
                        while (dr.Read())
                        {
                            Hashtable list_items = new Hashtable();
                            list_items.Add("reconcile_id", "d" + dr["receipt_id"].ToString());
                            list_items.Add("reconcile_date", DBSafeUtils.DBDateToStr(dr["receipt_date"]));
                            list_items.Add("reconcile_ref", dr["receipt_ref"].ToString());
                            list_items.Add("reconcile_description", dr["receipt_notes"].ToString());
                            list_items.Add("reconcile_deposit", Convert.ToDecimal(dr["receipt_gross"]).ToString("0.00"));
                            list_items.Add("CD", ReportDT.GetDataByColumn(debort, "debtor_master_id", dr["receipt_debtor_id"].ToString(), "debtor_master_name"));
                            list_items.Add("reconcile_payment", "");
                            list.Add(list_items);
                        }

                        sql = "SELECT * FROM      gl_transactions WHERE (gl_transaction_rec =1) and  (gl_transaction_type_id = 6) AND (gl_transaction_chart_id = " + account_id + ") and `gl_transaction_date`>=" + DBSafeUtils.DateToSQL(start) + " and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(end) + " and gl_transaction_recbatchid =" + batch; ;
                        dr = mydb.Reader(sql);
                        while (dr.Read())
                        {
                            Hashtable list_items = new Hashtable();
                            list_items.Add("reconcile_id", "j" + dr["gl_transaction_id"].ToString());
                            list_items.Add("CD", "");
                            list_items.Add("reconcile_date", DBSafeUtils.DBDateToStr(dr["gl_transaction_date"]));
                            list_items.Add("reconcile_ref", dr["gl_transaction_ref"].ToString());
                            list_items.Add("reconcile_description", dr["gl_transaction_description"].ToString());
                            decimal n = AdFunction.Rounded(dr["gl_transaction_net"].ToString());
                            if (n < 0)
                                list_items.Add("reconcile_deposit", (-n).ToString());
                            else
                                list_items.Add("reconcile_payment", n.ToString());
                            list.Add(list_items);
                        }
                        sql = "SELECT  * FROM      gl_transactions WHERE (gl_transaction_rec =1) and  (gl_transaction_type_id = 7) AND (gl_transaction_chart_id = " + account_id + ") and `gl_transaction_date`>=" + DBSafeUtils.DateToSQL(start) + " and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(end) + " and gl_transaction_recbatchid =" + batch; ;
                        dr = mydb.Reader(sql);
                        while (dr.Read())
                        {
                            Hashtable list_items = new Hashtable();
                            list_items.Add("reconcile_id", "j" + dr["gl_transaction_id"].ToString());
                            list_items.Add("reconcile_date", DBSafeUtils.DBDateToStr(dr["gl_transaction_date"]));
                            list_items.Add("CD", "");
                            list_items.Add("reconcile_ref", dr["gl_transaction_ref"].ToString());
                            list_items.Add("reconcile_description", dr["gl_transaction_description"].ToString());
                            decimal n = AdFunction.Rounded(dr["gl_transaction_net"].ToString());
                            if (n < 0)
                                list_items.Add("reconcile_deposit", (-n).ToString());
                            else
                                list_items.Add("reconcile_payment", n.ToString());
                            list.Add(list_items);
                        }
                        sql = "SELECT  * FROM      gl_transactions WHERE  (gl_transaction_rec =1) and (gl_transaction_type_id = 8) AND (gl_transaction_chart_id = " + account_id + ") and `gl_transaction_date`>=" + DBSafeUtils.DateToSQL(start) + " and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(end) + " and gl_transaction_recbatchid =" + batch; ;
                        dr = mydb.Reader(sql);
                        while (dr.Read())
                        {
                            Hashtable list_items = new Hashtable();
                            list_items.Add("reconcile_id", "j" + dr["gl_transaction_id"].ToString());
                            list_items.Add("CD", "");
                            list_items.Add("reconcile_date", DBSafeUtils.DBDateToStr(dr["gl_transaction_date"]));
                            list_items.Add("reconcile_description", dr["gl_transaction_description"].ToString());
                            decimal n = AdFunction.Rounded(dr["gl_transaction_net"].ToString());
                            if (n < 0)
                                list_items.Add("reconcile_deposit", (-n).ToString());
                            else
                                list_items.Add("reconcile_payment", n.ToString());
                            list.Add(list_items);
                        }
                        items.Add("reconciles", list);
                        decimal ld = 0;
                        decimal lp = 0;
                        for (int i = 0; i < list.Count; i++)
                        {
                            Hashtable ht = (Hashtable)list[i];
                            if (ht.Contains("reconcile_deposit"))
                                ld += AdFunction.Rounded(ht["reconcile_deposit"].ToString());
                            if (ht.Contains("reconcile_payment"))
                                lp += AdFunction.Rounded(ht["reconcile_payment"].ToString());
                        }
                        CBL.Text = (ld - lp + openbalance).ToString();

                        string args = account_id_HF.Value + "|" + start + "|" + end + "|" + batch;
                        string url = "reportviewer.aspx?reportid=BankReconciliationDetail&args=" + args;
                        HyperLink1.NavigateUrl = url;
                    }
                    finally
                    {
                        if (mydb != null) mydb.Close();
                    }
                    unreconciled.Add(JSON.JsonEncode(items));
                    #endregion
                }




            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid

        [System.Web.Services.WebMethod]

        public static string jqGridUnreonciledDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);
            ReconcileTemps unreconciles = new ReconcileTemps(user.UserId, TEMP_TYPE_UNRECONCILED, constr_general, constr);
            unreconciles.LoadData();


            DataTable dt = new DataTable("reconcile");
            dt.Columns.Add("ID");
            dt.Columns.Add(new DataColumn("Date", typeof(System.DateTime)));
            dt.Columns.Add("CD");
            dt.Columns.Add("Ref");
            dt.Columns.Add("Description");
            dt.Columns.Add("Deposit");
            dt.Columns.Add("Payment");
            //dt.Columns.Add("Balance");
            //decimal open = (decimal)HttpContext.Current.Session["OpeningBalance"];
            foreach (ReconcileTemp reconcile in unreconciles.ReconcileList)
            {
                Hashtable items = reconcile.GetData();
                DataRow dr = dt.NewRow();
                dr["ID"] = items["reconcile_id"].ToString();
                dr["Date"] = items["reconcile_date"].ToString();
                dr["CD"] = items["CD"].ToString();
                dr["Ref"] = items["reconcile_ref"].ToString();
                dr["Description"] = items["reconcile_description"].ToString();
                dr["Deposit"] = items["reconcile_deposit"].ToString();
                dr["Payment"] = items["reconcile_payment"].ToString();

                dt.Rows.Add(dr);
            }
            //Hashtable s = (Hashtable)JSON.JsonDecode(postdata);
            //string sort = s["sidx"].ToString() + " " + s["sord"].ToString();
            //dt = ReportDT.FilterDT(dt, "", sort);
            //foreach (DataRow dr in dt.Rows)
            //{
            //    dr["Balance"] = open + AdFunction.Rounded(dr["Deposit"].ToString()) - AdFunction.Rounded(dr["Payment"].ToString());
            //    open = AdFunction.Rounded(dr["Balance"].ToString());
            //}

            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`, `Date`,`CD`,`Ref`, `Description`, `Deposit`, `Payment` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            Hashtable userdata = new Hashtable();
            userdata.Add("Description", "Total:");
            userdata.Add("Deposit", ReportDT.SumTotal(dt, "Deposit"));
            userdata.Add("Payment", ReportDT.SumTotal(dt, "Payment"));
            jqgridObj.SetUserData(userdata);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }
        #endregion

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];
            string batch = Request.QueryString["batch"];
            string args = account_id_HF.Value + "|" + start + "|" + end + "|" + batch;

            Response.Write(SetUrl("BankReconciliationDetail", args));
        }
        public string SetUrl(string reportid, string args)
        {
            string url = "<script type='text/javascript'> window.open('reportviewer.aspx?reportid=" + reportid + "&args=" + args + "','_blank'); </script>";
            return url;
        }



    }
}