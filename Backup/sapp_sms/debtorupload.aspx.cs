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
    public partial class debtorupload : System.Web.UI.Page
    {
        static string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        Odbc o = new Odbc(constr);
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        protected void Button1_Click1(object sender, System.EventArgs e)
        {
            try
            {
                FileUpload1.SaveAs(Server.MapPath("~") + "temp\\2.csv");
                UIImport(SetImpotDT());
                FileInfo f = new FileInfo(Server.MapPath("~") + "temp\\2.csv");
                f.Delete();
                Response.Redirect("bodycorps.aspx", false);
            }
            catch (Exception ex)
            {

                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        public DataTable SetImpotDT()
        {
            DataTable dt = CsvDT.CsvToDataTable(Server.MapPath("~") + "temp\\", "2.csv");
            dt.Columns.Remove("Address1");
            dt.Columns.Remove("Address2");
            dt.Columns.Remove("Address3");
            dt.Columns.Remove("Address4");
            dt.Columns.Remove("Address5");
            dt.Columns.Remove("Post1");
            dt.Columns.Remove("Post2");
            dt.Columns.Remove("Post3");
            dt.Columns.Remove("PH_BUS");
            dt.Columns.Remove("PH_RES");
            dt.Columns.Remove("MOB");
            dt.Columns.Remove("FAX");
            dt.Columns.Remove("EMAIL");
            return dt;
        }
        public DataTable SetCommDT()
        {
            DataTable dt = CsvDT.CsvToDataTable(Server.MapPath("~") + "temp\\", "2.csv");
            return dt;
        }
        public string GetIDbyeName(string name, string col, string Table)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string result = "";
            Odbc o = new Odbc(constr);
            string sql = "select * from `" + Table + "` where `" + col + "` = '" + name.Replace("'", "''") + "'";
            DataTable dt = o.ReturnTable(sql, "type");
            if (dt.Rows.Count > 0)
                result = dt.Rows[0][0].ToString();
            return result;
        }
        public void UIImport(DataTable dt)
        {
            o.StartTransaction();
            try
            {
                string sql = "select * from `debtor_master`";

                DataTable old = o.ReturnTable(sql, "oldDE");
                foreach (DataRow newdr in dt.Rows)
                {
                    Hashtable ht = MakeHsahTable(newdr);
                    ht.Remove("debtor_master_id");
                    ht.Add("debtor_master_type_id", "1");
                    ht.Add("debtor_master_bodycorp_id", Request.Cookies["bodycorpid"].Value.ToString());
                    ht.Add("debtor_master_payment_term_id", "1");
                    ht.Add("debtor_master_payment_type_id", "6");
                    ht.Add("debtor_master_print", "0");
                    ht.Add("debtor_master_email", "0");
                    o.ExecuteInsert("debtor_master", ht);

                    string idsql = "SELECT LAST_INSERT_ID()";
                    string gid = Convert.ToInt32(o.ExecuteScalar(idsql)).ToString();
                    string dcode = newdr["debtor_master_code"].ToString();
                    InsertComm(dcode, gid, "Address1", "1", "1");
                    InsertComm(dcode, gid, "Address2", "1", "2");
                    InsertComm(dcode, gid, "Address3", "1", "3");
                    InsertComm(dcode, gid, "Address4", "1", "4");
                    InsertComm(dcode, gid, "Address5", "1", "5");
                    InsertComm(dcode, gid, "Post1", "2", "1");
                    InsertComm(dcode, gid, "Post2", "2", "2");
                    InsertComm(dcode, gid, "Post3", "2", "3");
                    InsertComm(dcode, gid, "PH_BUS", "3");
                    InsertComm(dcode, gid, "PH_RES", "4");
                    InsertComm(dcode, gid, "MOB", "5");
                    InsertComm(dcode, gid, "FAX", "6");
                    InsertComm(dcode, gid, "EMAIL", "7");
                }
                o.Commit();
            }
            catch (Exception ex)
            {
                if (o != null) o.Rollback();
                throw ex;
            }
            finally
            {
                if (o != null) o.Close();
            }
        }

        public void InsertComm(string debtorcode, string debtorID, string col, string typeid, string order = "")
        {
            DataTable commdt = SetCommDT();
            string data = ReportDT.GetDataByColumn(commdt, "debtor_master_code", debtorcode, col);
            if (!data.Equals(""))
            {

                string idsql = "SELECT LAST_INSERT_ID()";
                Hashtable hs = new Hashtable();
                hs.Add("comm_master_data", "'" + data.Replace("'", "''") + "'");
                hs.Add("comm_master_type_id", typeid);
                if (!order.Equals(""))
                    hs.Add("comm_master_order", order);
                o.ExecuteInsert("comm_master", hs);
                string id = Convert.ToInt32(o.ExecuteScalar(idsql)).ToString();
                Hashtable hs2 = new Hashtable();
                hs2.Add("debtor_comm_debtor_id", debtorID);
                hs2.Add("debtor_comm_comm_id", id);
                o.ExecuteInsert("debtor_comms", hs2);
            }
        }

        public Hashtable MakeHsahTable(DataRow dr)
        {
            Hashtable ht = new Hashtable();
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                string cname = dr.Table.Columns[i].ColumnName;
                string v = dr[cname].ToString();
                if (!v.Equals("Null"))
                    ht.Add(cname, "'" + v.Replace("'", "''") + "'");
            }
            return ht;
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("debtor_master_code");
            dt.Columns.Add("debtor_master_name");
            dt.Columns.Add("Address1");
            dt.Columns.Add("Address2");
            dt.Columns.Add("Address3");
            dt.Columns.Add("Address4");
            dt.Columns.Add("Address5");
            dt.Columns.Add("Post1");
            dt.Columns.Add("Post2");
            dt.Columns.Add("Post3");
            dt.Columns.Add("PH_BUS");
            dt.Columns.Add("PH_RES");
            dt.Columns.Add("MOB");
            dt.Columns.Add("FAX");
            dt.Columns.Add("EMAIL");
            CsvDT.DataTableToCsv(dt, Server.MapPath("~/Temp/DebtorTemplate.csv"));
            Response.Redirect("~/Temp/DebtorTemplate.csv");



        }
    }
}