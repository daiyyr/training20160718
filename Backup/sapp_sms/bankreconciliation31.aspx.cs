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
    public partial class bankreconciliation31 : System.Web.UI.Page
    {
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
            dt.Columns.Add("Batch", typeof(int));
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
                newdr["CD"] = ReportDT.GetDataByColumn(creditor, "creditor_master_id", dr["cpayment_creditor_id"].ToString(), "creditor_master_code");
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
                newdr["CD"] = ReportDT.GetDataByColumn(creditor, "creditor_master_id", dr["receipt_debtor_id"].ToString(), "creditor_master_code");
                newdr["reconcile_payment"] = "";
                newdr["Batch"] = dr["receipt_recbatchid"].ToString();

                dt.Rows.Add(newdr);
            }

            sql = "SELECT * FROM      gl_transactions WHERE (gl_transaction_rec =1) and  (gl_transaction_type_id = 6) and `gl_transaction_date`>=" + DBSafeUtils.DateToSQL(start) + " and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(end);
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

            sql = "SELECT  * FROM gl_transactions WHERE (gl_transaction_rec =1) and  (gl_transaction_type_id = 7) and `gl_transaction_date`>=" + DBSafeUtils.DateToSQL(start) + " and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(end);
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
            sql = "SELECT  * FROM      gl_transactions WHERE  (gl_transaction_rec =1) and (gl_transaction_type_id = 8) and `gl_transaction_date`>=" + DBSafeUtils.DateToSQL(start) + " and `gl_transaction_date`<=" + DBSafeUtils.DateToSQL(end);
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

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    #region Initial ComboBox
                    Odbc o = new Odbc(constr);
                    string sql = "SELECT   chart_master.chart_master_id AS ID, chart_master.chart_master_code AS Code,                   chart_master.chart_master_name AS Name, chart_master.chart_master_type_id, chart_types.chart_type_name  FROM      chart_master, chart_types  WHERE   chart_master.chart_master_type_id = chart_types.chart_type_id AND (chart_master.chart_master_bank_account = 1) ";
                    DataTable dt = o.ReturnTable(sql, "c");
                    foreach (DataRow dr in dt.Rows)
                    {
                        ListItem i = new ListItem(dr["Code"].ToString() + " | " + dr["Name"].ToString(), dr["ID"].ToString());
                        ComboBoxAccountCode.Items.Add(i);
                    }

                    #endregion
                    #region Initial PDF Button

                    if (Request.QueryString["args"] != null)
                    {
                        LiteralPDF.Visible = true;
                        ImageButtonPDF.Visible = true;

                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }




        protected void ComboBoxAccountCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            GridView1.DataSource = null;
            GridView1.DataBind();
            DataTable datatdt = ReportDT.FilterDT(GetDT(DateTime.Now.AddYears(-100).ToString(), DateTime.Now.AddYears(100).ToString()), "", "Batch desc");
            datatdt = ReportDT.FilterDT(datatdt, "AccountID='" + ComboBoxAccountCode.SelectedValue + "'");
            DataTable dt = new DataTable();

            dt.Columns.Add("Batch", typeof(int));
            dt.Columns.Add("Cutoff");
            if (datatdt.Rows.Count > 0)
            {
                string b = datatdt.Rows[0]["Batch"].ToString();
                int bid = 0;
                if (!b.Equals(""))
                    bid = int.Parse(b);
                for (int i = 1; i <= bid; i++)
                {
                    DataRow dr = dt.NewRow();
                    DataTable tempDT = ReportDT.FilterDT(datatdt, " Batch ='" + i + "'", "reconcile_date");
                    if (tempDT.Rows.Count > 0)
                    {
                        dr["Batch"] = i;
                        string sql = "select * from gl_transactions where gl_transaction_reccutoff is not null and gl_transaction_recbatchid=" + i + " order by gl_transaction_reccutoff";
                        DataTable tempgl = o.ReturnTable(sql, "rr");
                        if (tempgl.Rows.Count > 0)
                            if (!tempgl.Rows[0]["gl_transaction_reccutoff"].ToString().Equals(""))
                            {
                                //dr["Start"] = DateTime.Parse(tempgl.Rows[0]["gl_transaction_recstart"].ToString()).ToString("dd/MM/yyyy");
                                dr["Cutoff"] = DateTime.Parse(tempgl.Rows[0]["gl_transaction_reccutoff"].ToString()).ToString("dd/MM/yyyy");
                            }
                        dt.Rows.Add(dr);
                    }
                }
                dt = ReportDT.FilterDT(dt, "", "Batch desc");
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            LinkButton lb = (LinkButton)sender;
            string bid = lb.Attributes["BID"].ToString();
            string start = DateTime.Now.AddYears(-1000).ToString("yyyy-MM-dd");
            string end = lb.Attributes["End"].ToString();
            if (!end.Equals(""))
                end = DateTime.Parse(end).ToString("yyyy-MM-dd");
            else
                end = DateTime.Now.ToString("yyyy-MM-dd");
            Response.Redirect("bankreconciliation3.aspx?accountid=" + ComboBoxAccountCode.SelectedValue + "&start=" + start + "&end=" + end + "&batch=" + bid, false);
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void Export(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                string account_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                string batch = args[3];

                string filefolder = Server.MapPath("~/temp");

                ReportViewer reportViewer1 = new ReportViewer();

                BReconciliationHistory Report = new BReconciliationHistory(constr, Server.MapPath("templates/BRecoinciliationDetail.rdlc"), reportViewer1);
                Report.SetReportInfo(int.Parse(account_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), batch);

                string filename = Guid.NewGuid().ToString();
                Report.ExportPDF(filefolder + "\\" + filename + ".pdf");
                FileInfo fo = new FileInfo(filefolder + "\\" + filename + ".pdf");
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fo.Name);
                HttpContext.Current.Response.AddHeader("Content-Length", fo.Length.ToString());
                HttpContext.Current.Response.ContentType = "text/plain";
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.TransmitFile(fo.FullName);
                HttpContext.Current.Response.End();



            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }

        }

        protected void ImageButtonPDF_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string args = Request.QueryString["args"];
                Export(args.Split('|'));
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

    }
}