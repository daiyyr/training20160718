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
using Microsoft.Reporting.WebForms;
using System.IO;


namespace sapp_sms
{
    public partial class bankreconciliation2 : System.Web.UI.Page, IPostBackEventHandler
    {
        private const string TEMP_TYPE_RECONCILED = "bankreconciliation2_1";
        private const string TEMP_TYPE_UNRECONCILED = "bankreconciliation2_2";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { jqGridReonciled, jqGridUnreconciled };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                string account_id = Request.QueryString["accountid"];
                if (account_id_HF.Value.Equals(""))
                    account_id_HF.Value = account_id;
                else
                    account_id = account_id_HF.Value;
                string cutoffdate = Request.QueryString["cutoffdate"];
                if (cutoffdate_HF.Value.Equals(""))
                    cutoffdate_HF.Value = cutoffdate;
                else
                    cutoffdate = cutoffdate_HF.Value;

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
                        string opsql = "SELECT sum(`cpayment_gross`) FROM `cpayments` WHERE `cpayment_bodycorp_id` IN (SELECT `bodycorp_id` FROM `bodycorps` WHERE `bodycorp_account_id`=" + account_id + ") AND `cpayment_date`<=" + DBSafeUtils.DateToSQL(cutoffdate) + " AND `cpayment_reconciled`=1";
                        openbalance -= AdFunction.Rounded(o.ReturnTable(opsql, "sum").Rows[0][0].ToString());

                        opsql = "SELECT sum(`receipt_gross`) FROM `receipts` WHERE `receipt_bodycorp_id` IN (SELECT `bodycorp_id` FROM `bodycorps` WHERE `bodycorp_account_id`=" + account_id + ") AND `receipt_date`<=" + DBSafeUtils.DateToSQL(cutoffdate) + " AND `receipt_reconciled`=1";
                        openbalance += AdFunction.Rounded(o.ReturnTable(opsql, "sum").Rows[0][0].ToString());
                        opsql = "SELECT sum(`gl_transaction_net`) FROM      gl_transactions WHERE  (gl_transaction_rec =1) and (gl_transaction_type_id = 7) AND (gl_transaction_chart_id = " + account_id + ") and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(cutoffdate);
                        openbalance -= AdFunction.Rounded(o.ReturnTable(opsql, "sum").Rows[0][0].ToString());
                        opsql = "SELECT sum(`gl_transaction_net`) FROM      gl_transactions WHERE (gl_transaction_rec =1) and   (gl_transaction_type_id = 8) AND (gl_transaction_chart_id = " + account_id + ") and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(cutoffdate);
                        openbalance -= AdFunction.Rounded(o.ReturnTable(opsql, "sum").Rows[0][0].ToString());
                        opsql = "SELECT sum(`gl_transaction_net`) FROM      gl_transactions WHERE (gl_transaction_rec =1) and   (gl_transaction_type_id = 6) AND (gl_transaction_chart_id = " + account_id + ") and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(cutoffdate);
                        openbalance -= AdFunction.Rounded(o.ReturnTable(opsql, "sum").Rows[0][0].ToString());

                        OpenBalanceL.Text = openbalance.ToString();
                        Session["OpeningBalance"] = openbalance;



                        CBL.Text = (openbalance).ToString();
                        CUBL.Text = Session["cub"].ToString();

                        DataTable creditor = ReportDT.getTable(constr, "creditor_master");
                        DataTable debort = ReportDT.getTable(constr, "debtor_master");
                        mydb = new Odbc(constr);
                        ArrayList list = new ArrayList();
                        string sql = "SELECT * FROM `cpayments` WHERE `cpayment_bodycorp_id` IN (SELECT `bodycorp_id` FROM `bodycorps` WHERE `bodycorp_account_id`=" + account_id + ") AND `cpayment_date`<=" + DBSafeUtils.DateToSQL(cutoffdate) + " AND `cpayment_reconciled`=0";
                        OdbcDataReader dr = mydb.Reader(sql);
                        while (dr.Read())
                        {
                            Hashtable list_items = new Hashtable();
                            list_items.Add("reconcile_id", "p" + dr["cpayment_id"].ToString());
                            list_items.Add("reconcile_date", DBSafeUtils.DBDateToStr(dr["cpayment_date"]));
                            list_items.Add("reconcile_ref", dr["cpayment_reference"].ToString());
                            list_items.Add("reconcile_description", "");
                            list_items.Add("reconcile_deposit", "");
                            list_items.Add("CD", ReportDT.GetDataByColumn(creditor, "creditor_master_id", dr["cpayment_creditor_id"].ToString(), "creditor_master_name"));
                            list_items.Add("reconcile_payment", Convert.ToDecimal(dr["cpayment_gross"]).ToString("0.00"));
                            list.Add(list_items);
                        }
                        sql = "SELECT * FROM `receipts` WHERE `receipt_bodycorp_id` IN (SELECT `bodycorp_id` FROM `bodycorps` WHERE `bodycorp_account_id`=" + account_id + ") AND `receipt_date`<=" + DBSafeUtils.DateToSQL(cutoffdate) + " AND `receipt_reconciled`=0";
                        dr = mydb.Reader(sql);
                        while (dr.Read())
                        {
                            Hashtable list_items = new Hashtable();
                            list_items.Add("reconcile_id", "d" + dr["receipt_id"].ToString());
                            list_items.Add("reconcile_date", DBSafeUtils.DBDateToStr(dr["receipt_date"]));
                            list_items.Add("reconcile_ref", dr["receipt_ref"].ToString());
                            list_items.Add("reconcile_description", dr["receipt_notes"].ToString());
                            decimal g = Convert.ToDecimal(dr["receipt_gross"].ToString());
                            if (g > 0)
                                list_items.Add("reconcile_deposit", g.ToString());
                            else
                                list_items.Add("reconcile_payment", (-g).ToString());
                            list_items.Add("CD", ReportDT.GetDataByColumn(debort, "debtor_master_id", dr["receipt_debtor_id"].ToString(), "debtor_master_name"));

                            list.Add(list_items);
                        }

                        // Update 10/06/2016 Add 'gl_transaction_rev=0'
                        sql = "SELECT   gl_transaction_id, gl_transaction_type_id, gl_transaction_ref, gl_transaction_ref_type_id, gl_transaction_chart_id,                  gl_transaction_bodycorp_id, gl_transaction_unit_id, gl_transaction_description, gl_transaction_batch_id,                  gl_transaction_net, gl_transaction_tax, gl_transaction_gross, gl_transaction_date FROM      gl_transactions WHERE (gl_transaction_rec =0 AND gl_transaction_rev = 0) and  (gl_transaction_type_id = 6) AND (gl_transaction_chart_id = " + account_id + ") and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(cutoffdate);
                        dr = mydb.Reader(sql);
                        while (dr.Read())
                        {
                            Hashtable list_items = new Hashtable();
                            list_items.Add("reconcile_id", "j" + dr["gl_transaction_id"].ToString());
                            list_items.Add("CD", "");
                            list_items.Add("reconcile_date", DBSafeUtils.DBDateToStr(dr["gl_transaction_date"]));
                            list_items.Add("reconcile_ref", dr["gl_transaction_ref"].ToString());
                            list_items.Add("reconcile_description", "");
                            decimal n = AdFunction.Rounded(dr["gl_transaction_net"].ToString());
                            if (n < 0)
                                list_items.Add("reconcile_deposit", (-n).ToString());
                            else
                                list_items.Add("reconcile_payment", n.ToString());
                            list.Add(list_items);
                        }

                        sql = "SELECT   gl_transaction_id, gl_transaction_type_id, gl_transaction_ref, gl_transaction_ref_type_id, gl_transaction_chart_id,                  gl_transaction_bodycorp_id, gl_transaction_unit_id, gl_transaction_description, gl_transaction_batch_id,                  gl_transaction_net, gl_transaction_tax, gl_transaction_gross, gl_transaction_date FROM      gl_transactions WHERE (gl_transaction_rec =0) and  (gl_transaction_type_id = 7) AND (gl_transaction_chart_id = " + account_id + ") and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(cutoffdate);
                        dr = mydb.Reader(sql);
                        while (dr.Read())
                        {
                            Hashtable list_items = new Hashtable();
                            list_items.Add("reconcile_id", "j" + dr["gl_transaction_id"].ToString());
                            list_items.Add("reconcile_date", DBSafeUtils.DBDateToStr(dr["gl_transaction_date"]));
                            list_items.Add("CD", "");
                            list_items.Add("reconcile_ref", dr["gl_transaction_ref"].ToString());
                            list_items.Add("reconcile_description", "");
                            decimal n = AdFunction.Rounded(dr["gl_transaction_net"].ToString());
                            if (n < 0)
                                list_items.Add("reconcile_deposit", (-n).ToString());
                            else
                                list_items.Add("reconcile_payment", n.ToString());
                            list.Add(list_items);
                        }
                        sql = "SELECT   gl_transaction_id, gl_transaction_type_id, gl_transaction_ref, gl_transaction_ref_type_id, gl_transaction_chart_id,                  gl_transaction_bodycorp_id, gl_transaction_unit_id, gl_transaction_description, gl_transaction_batch_id,                  gl_transaction_net, gl_transaction_tax, gl_transaction_gross, gl_transaction_date FROM      gl_transactions WHERE  (gl_transaction_rec =0) and (gl_transaction_type_id = 8) AND (gl_transaction_chart_id = " + account_id + ") and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(cutoffdate);
                        dr = mydb.Reader(sql);
                        while (dr.Read())
                        {
                            Hashtable list_items = new Hashtable();
                            list_items.Add("reconcile_id", "j" + dr["gl_transaction_id"].ToString());
                            list_items.Add("CD", "");
                            list_items.Add("reconcile_date", DBSafeUtils.DBDateToStr(dr["gl_transaction_date"]));
                            list_items.Add("reconcile_description", dr["gl_transaction_ref"].ToString());
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
                        ABL.Text = (ld - lp + openbalance).ToString();
                        UCBL.Text = (ld - lp).ToString();
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
        public void RaisePostBackEvent(string eventArgument)
        {
            try
            {
                string[] args = eventArgument.Split('|');
                if (args[0] == "ImageButtonUp")
                {
                    ImageButtonUp_Click(args);
                }
                else if (args[0] == "ImageButtonDown")
                {
                    ImageButtonDown_Click(args);
                }
                else if (args[0] == "Refresh")
                {
                    string account_id = account_id_HF.Value;
                    string cutoffdate = cutoffdate_HF.Value;
                    #region Redirect
                    //Response.BufferOutput = true;
                    //Response.Redirect("bankreconciliation2.aspx?accountid=" + account_id + "&cutoffdate=" + cutoffdate + "&status=refresh", false);
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
        public static string jqGridReonciledDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);
            ReconcileTemps unreconciles = new ReconcileTemps(user.UserId, TEMP_TYPE_RECONCILED, constr_general, constr);
            unreconciles.LoadData();


            DataTable dt = new DataTable("reconcile");
            dt.Columns.Add("ID");
            dt.Columns.Add(new DataColumn("Date", typeof(System.DateTime)));
            dt.Columns.Add("Ref");
            dt.Columns.Add("Description");
            dt.Columns.Add("Deposit");
            dt.Columns.Add("Payment");
            foreach (ReconcileTemp reconcile in unreconciles.ReconcileList)
            {
                Hashtable items = reconcile.GetData();
                DataRow dr = dt.NewRow();
                dr["ID"] = items["reconcile_id"].ToString();
                dr["Date"] = items["reconcile_date"].ToString();
                dr["Ref"] = items["reconcile_ref"].ToString();
                dr["Description"] = items["reconcile_description"].ToString();
                dr["Deposit"] = items["reconcile_deposit"].ToString();
                dr["Payment"] = items["reconcile_payment"].ToString();

                dt.Rows.Add(dr);
            }
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`, `Date`,`Ref`, `Description`, `Deposit`, `Payment` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            Hashtable userdata = new Hashtable();
            userdata.Add("Description", "Total:");
            userdata.Add("Deposit", ReportDT.SumTotal(dt, "Deposit"));
            userdata.Add("Payment", ReportDT.SumTotal(dt, "Payment"));
            HttpContext.Current.Session["cbdeposit"] = userdata["Deposit"].ToString();
            HttpContext.Current.Session["cbpayment"] = userdata["Payment"].ToString();
            jqgridObj.SetUserData(userdata);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;

        }
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
        #region WebControl Events
        private void ImageButtonUp_Click(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                //Load Reconciled Temp Trans
                ReconcileTemps reconciled = new ReconcileTemps(user.UserId, TEMP_TYPE_RECONCILED, constr_general, constr);
                reconciled.LoadData();
                //Load Unreconciled Temp Trans
                ReconcileTemps unreconciled = new ReconcileTemps(user.UserId, TEMP_TYPE_UNRECONCILED, constr_general, constr);
                unreconciled.LoadData();

                string[] idList = args[1].Split(',');
                ArrayList reconciles = new ArrayList();
                decimal d = 0;
                decimal p = 0;
                foreach (string id in idList)
                {

                    foreach (ReconcileTemp reconcile in unreconciled.ReconcileList)
                    {

                        if (id == reconcile.ReconcileId)
                        {
                            reconciles.Add(reconcile);
                            p += AdFunction.Rounded(reconcile.ReconcilePayment);
                            d += AdFunction.Rounded(reconcile.ReconcileDeposit);
                        }
                    }
                }
                foreach (ReconcileTemp reconcile in reconciles)
                {
                    unreconciled.ReconcileList.Remove(reconcile);
                    reconciled.ReconcileList.Add(reconcile);
                }
                unreconciled.UpdateTemp();
                reconciled.UpdateTemp();


                HttpContext.Current.Session["cbdeposit"] = d;
                HttpContext.Current.Session["cbpayment"] = p;
                UCBL.Text = (AdFunction.Rounded(UCBL.Text) - d + p).ToString();
                CBL.Text = (AdFunction.Rounded(CBL.Text) + d - p).ToString();

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void ImageButtonDown_Click(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                //Load Reconciled Temp Trans
                ReconcileTemps reconciled = new ReconcileTemps(user.UserId, TEMP_TYPE_RECONCILED, constr_general, constr);
                reconciled.LoadData();
                //Load Unreconciled Temp Trans
                ReconcileTemps unreconciled = new ReconcileTemps(user.UserId, TEMP_TYPE_UNRECONCILED, constr_general, constr);
                unreconciled.LoadData();

                string[] idList = args[1].Split(',');
                ArrayList reconciles = new ArrayList();
                decimal d = 0;
                decimal p = 0;
                foreach (string id in idList)
                {
                    foreach (ReconcileTemp reconcile in reconciled.ReconcileList)
                    {
                        if (id == reconcile.ReconcileId)
                        {
                            reconciles.Add(reconcile);

                        }
                    }
                }
                foreach (ReconcileTemp reconcile in reconciles)
                {
                    unreconciled.ReconcileList.Add(reconcile);
                    reconciled.ReconcileList.Remove(reconcile);
                    p += AdFunction.Rounded(reconcile.ReconcilePayment);
                    d += AdFunction.Rounded(reconcile.ReconcileDeposit);
                }
                unreconciled.UpdateTemp();
                reconciled.UpdateTemp();
                p = -p;
                d = -d;
                HttpContext.Current.Session["cbdeposit"] = d;
                HttpContext.Current.Session["cbpayment"] = p;
                UCBL.Text = (AdFunction.Rounded(UCBL.Text) - d + p).ToString();
                CBL.Text = (AdFunction.Rounded(CBL.Text) + d - p).ToString();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ImageButtonSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (AdFunction.Rounded(CUBL.Text) == AdFunction.Rounded(CBL.Text))
                {
                    string b = "";

                    //DataTable datatdt = ReportDT.FilterDT(GetDT(DateTime.Now.AddYears(-100).ToString(), DateTime.Now.AddYears(100).ToString()), "", "Batch desc");
                    //if (datatdt.Rows.Count > 0)
                    //{
                    //    b = datatdt.Rows[0]["Batch"].ToString();
                    //}
                    //if (!b.Equals(""))
                    //{
                    //    b = (int.Parse(b) + 1).ToString();
                    //}
                    //else
                    //    b = "1";
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(constr_general);
                    user.LoadData(login);
                    ReconcileTemps reconciled = new ReconcileTemps(user.UserId, TEMP_TYPE_RECONCILED, constr_general, constr);
                    reconciled.LoadData();
                    //string start = DateTime.Parse(Request.QueryString["startdate"].ToString()).ToString("yyyy-MM-dd");
                    string end = DateTime.Parse(Request.QueryString["enddate"].ToString()).ToString("yyyy-MM-dd");
                    string start = "";
                    string batch =  reconciled.Submit(start,end);
                   
                   Odbc o=new Odbc(AdFunction.conn);
                   string startsql = "select * from gl_transactions where (gl_transaction_chart_id = " + account_id_HF.Value + ") and gl_transaction_recbatchid =" + batch + " order by gl_transaction_date";
                   DataTable startDT = o.ReturnTable(startsql, "t1");
                   if (startDT.Rows.Count > 0)
                   {
                       start = (DateTime.Parse(startDT.Rows[0]["gl_transaction_date"].ToString())).ToString("yyyy-MM-dd");

                   }

                    string args = account_id_HF.Value + "|" + start + "|" + end + "|" + batch;

                    Response.Redirect("bankreconciliation31.aspx?args=" + args, false);
                }
            }
            catch (Exception ex)
            {
               HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        public DataTable GetDT(string start, string end)
        {
            Odbc o = new Odbc(AdFunction.conn);
            DataTable creditor = ReportDT.getTable(AdFunction.conn, "creditor_master");
            DataTable debort = ReportDT.getTable(AdFunction.conn, "debtor_master");
            Odbc mydb = new Odbc(AdFunction.conn);
            ArrayList list = new ArrayList();
            string sql = "SELECT * FROM `cpayments` WHERE `cpayment_bodycorp_id` IN (SELECT `bodycorp_id` FROM `bodycorps`) AND `cpayment_date`>=" + DBSafeUtils.DateToSQL(start) + "and `cpayment_date`<=" + DBSafeUtils.DateToSQL(end) + "  AND `cpayment_reconciled`=1";
            OdbcDataReader dr = mydb.Reader(sql);
            DataTable bdt = ReportDT.getTable(AdFunction.conn, "bodycorps");
            DataTable dt = new DataTable();
            dt.Columns.Add("Type");
            dt.Columns.Add("AccountID");
            dt.Columns.Add("reconcile_id");
            dt.Columns.Add("reconcile_date", typeof(DateTime));
            dt.Columns.Add("reconcile_ref");
            dt.Columns.Add("reconcile_description");
            dt.Columns.Add("reconcile_deposit");
            dt.Columns.Add("CD");
            dt.Columns.Add("reconcile_payment");
            dt.Columns.Add("Batch");
            while (dr.Read())
            {
                DataRow newdr = dt.NewRow();
                Hashtable list_items = new Hashtable();
                newdr["Type"] = "CPayment";
                newdr["AccountID"] = ReportDT.GetDataByColumn(bdt, "bodycorp_id", dr["cpayment_bodycorp_id"].ToString(), "bodycorp_account_id");
                newdr["reconcile_id"] = dr["cpayment_id"].ToString();
                newdr["reconcile_date"] = dr["cpayment_date"].ToString();
                newdr["reconcile_ref"] = dr["cpayment_reference"].ToString();
                newdr["reconcile_description"] = "";
                newdr["reconcile_deposit"] = "";
                newdr["CD"] = ReportDT.GetDataByColumn(creditor, "creditor_master_id", dr["cpayment_creditor_id"].ToString(), "creditor_master_name");
                newdr["reconcile_payment"] = dr["cpayment_gross"].ToString();
                newdr["Batch"] = dr["cpayment_recbatchid"].ToString();
                dt.Rows.Add(newdr);
            }
            sql = "SELECT * FROM `receipts` WHERE `receipt_bodycorp_id` IN (SELECT `bodycorp_id` FROM `bodycorps`) AND `receipt_date`>=" + DBSafeUtils.DateToSQL(start) + " AND `receipt_date`<=" + DBSafeUtils.DateToSQL(end) + "  AND `receipt_reconciled`=1";
            dr = mydb.Reader(sql);
            while (dr.Read())
            {
                DataRow newdr = dt.NewRow();
                Hashtable list_items = new Hashtable();
                newdr["Type"] = "Recipt";
                newdr["AccountID"] = ReportDT.GetDataByColumn(bdt, "bodycorp_id", dr["receipt_bodycorp_id"].ToString(), "bodycorp_account_id");
                newdr["reconcile_id"] = dr["receipt_id"].ToString();
                newdr["reconcile_date"] = dr["receipt_date"].ToString();
                newdr["reconcile_ref"] = dr["receipt_ref"].ToString();
                newdr["reconcile_description"] = dr["receipt_notes"].ToString();
                newdr["reconcile_deposit"] = dr["receipt_gross"].ToString();
                newdr["CD"] = ReportDT.GetDataByColumn(creditor, "creditor_master_id", dr["receipt_debtor_id"].ToString(), "creditor_master_name");
                newdr["reconcile_payment"] = "";
                newdr["Batch"] = dr["receipt_recbatchid"].ToString();

                dt.Rows.Add(newdr);
            }

            sql = "SELECT * FROM      gl_transactions WHERE (gl_transaction_rec = 1) and  (gl_transaction_type_id = 6) and `gl_transaction_date`>=" + DBSafeUtils.DateToSQL(start) + " and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(end);
            dr = mydb.Reader(sql);
            while (dr.Read())
            {

                DataRow newdr = dt.NewRow();
                newdr["Type"] = "GL";
                newdr["AccountID"] = dr["gl_transaction_chart_id"].ToString();
                newdr["reconcile_id"] = dr["gl_transaction_id"].ToString();
                newdr["reconcile_date"] = dr["gl_transaction_date"].ToString();
                newdr["reconcile_ref"] = dr["gl_transaction_ref"].ToString();
                newdr["reconcile_description"] = "";
                newdr["CD"] = "";
                newdr["Batch"] = dr["gl_transaction_recbatchid"].ToString();


                decimal n = AdFunction.Rounded(dr["gl_transaction_net"].ToString());
                if (n < 0)
                    newdr["reconcile_deposit"] = (-n).ToString();

                else
                    newdr["reconcile_payment"] = n.ToString();

                dt.Rows.Add(newdr);
            }

            sql = "SELECT  * FROM      gl_transactions WHERE (gl_transaction_rec = 1) and  (gl_transaction_type_id = 7) and `gl_transaction_date`>=" + DBSafeUtils.DateToSQL(start) + " and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(end);
            dr = mydb.Reader(sql);
            while (dr.Read())
            {
                DataRow newdr = dt.NewRow();
                newdr["Type"] = "GL";
                newdr["AccountID"] = dr["gl_transaction_chart_id"].ToString();
                newdr["reconcile_id"] = dr["gl_transaction_id"].ToString();
                newdr["reconcile_date"] = dr["gl_transaction_date"].ToString();
                newdr["reconcile_ref"] = dr["gl_transaction_ref"].ToString();
                newdr["reconcile_description"] = "";
                newdr["CD"] = "";
                newdr["Batch"] = dr["gl_transaction_recbatchid"].ToString();
                decimal n = AdFunction.Rounded(dr["gl_transaction_net"].ToString());
                if (n < 0)
                    newdr["reconcile_deposit"] = (-n).ToString();

                else
                    newdr["reconcile_payment"] = n.ToString();

                dt.Rows.Add(newdr);
            }
            sql = "SELECT  * FROM      gl_transactions WHERE  (gl_transaction_rec = 1) and (gl_transaction_type_id = 8) and `gl_transaction_date`>=" + DBSafeUtils.DateToSQL(start) + " and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(end);
            dr = mydb.Reader(sql);
            while (dr.Read())
            {
                DataRow newdr = dt.NewRow();
                newdr["Type"] = "Recipt";
                newdr["AccountID"] = dr["gl_transaction_chart_id"].ToString();
                newdr["reconcile_id"] = dr["gl_transaction_id"].ToString();
                newdr["reconcile_date"] = dr["gl_transaction_date"].ToString();
                newdr["reconcile_ref"] = dr["gl_transaction_ref"].ToString();
                newdr["reconcile_description"] = "";
                newdr["CD"] = "";
                newdr["Batch"] = dr["gl_transaction_recbatchid"].ToString();
                decimal n = AdFunction.Rounded(dr["gl_transaction_net"].ToString());
                if (n < 0)
                    newdr["reconcile_deposit"] = (-n).ToString();

                else
                    newdr["reconcile_payment"] = n.ToString();
                dt.Rows.Add(newdr);
            }
            return dt;
        }
        #endregion


    }
}