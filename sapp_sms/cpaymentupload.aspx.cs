using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Configuration;
using System.IO;
using Sapp.Common;
using Sapp.SMS;
using Sapp.JQuery;
using Sapp.Data;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Globalization;
using System.Threading;

using System.Data.OleDb;
using System.Data.Odbc;
using System.Net.Mail;
using System.Net;

namespace sapp_sms
{
    public partial class paymentupload : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
        }


        protected void Button1_Click1(object sender, System.EventArgs e)
        {
            Odbc mydb = new Odbc(AdFunction.conn);
            mydb.StartTransaction();
            try
            {
                string guid = Guid.NewGuid().ToString();
                FileUpload1.SaveAs(Server.MapPath("~") + "temp\\" + guid + ".csv");
                FileInfo f = new FileInfo(Server.MapPath("~") + "temp\\" + guid + ".csv");
                DataTable dt = CsvDT.CsvToDataTable(Server.MapPath("~") + "temp\\", guid + ".csv");
                DataTable cdt = ReportDT.getTable(AdFunction.conn, "creditor_master");
                int row_idx = 1;  // Add 14/06/2016
                foreach (DataRow dr in dt.Rows)
                {
                    row_idx = row_idx + 1;

                    string cname = dr["Contractor"].ToString();
                    string date = dr["Date"].ToString();
                    string reference = dr["Ref"].ToString();
                    string code = dr["Code"].ToString();
                    string amount = dr["Amount"].ToString();

                    // Add 19/06/2016
                    if ("".Equals(code))
                    {
                        throw new Exception(CsvDT.editErrorMessage(row_idx, "Code", "creditor master code is empty"));
                    }

                    // Add 19/06/2016
                    if ("".Equals(date))
                    {
                        throw new Exception(CsvDT.editErrorMessage(row_idx, "Date", "cpayment date is empty"));
                    }

                    // Add 19/06/2016
                    if ("".Equals(amount))
                    {
                        throw new Exception(CsvDT.editErrorMessage(row_idx, "Amount", "cpayment amount is empty"));
                    }

                    string cid = ReportDT.GetDataByColumn(cdt, "creditor_master_code", code, "creditor_master_id");
                    if (cid.Equals(""))
                        cid = ReportDT.GetDataByColumn(cdt, "creditor_master_code", code + " ", "creditor_master_id");
                    if (cid.Equals(""))
                        cid = ReportDT.GetDataByColumn(cdt, "creditor_master_name", cname, "creditor_master_id");
                    if (cid.Equals(""))
                        cid = ReportDT.GetDataByColumn(cdt, "creditor_master_name", cname + " ", "creditor_master_id");
                    if (cid.Equals(""))
                        throw new Exception(CsvDT.editErrorMessage(row_idx, "Code", "creditor master code [" + code + "] can not be found in DB"));
                        //throw new Exception("Can not find Creditor");
                    if (reference.Equals(""))
                        reference = date;
                    DateTime d = DateTime.Parse(date);
                    CPayment cp = new CPayment(AdFunction.conn);
                    cp.SetOdbc(mydb);
                    cp.Cpayment_reference = "'" + reference + "'";
                    cp.Cpayment_date = d;
                    cp.Cpayment_gross = decimal.Parse(amount);
                    cp.Cpayment_type_id = 6;
                    cp.Cpayment_bodycorp_id = 1;
                    cp.Cpayment_creditor_id = int.Parse(cid);
                    Hashtable hs = cp.GetData();
                    hs["cpayment_date"] = "'" + d.ToString("yyyy-MM-dd") + "'";
                    cp.Add(hs);
                }
                mydb.Commit();
                f.Delete();
                Response.Redirect("~/cpayments.aspx",false);
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }
    }
}