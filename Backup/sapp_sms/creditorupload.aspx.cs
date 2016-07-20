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
    public partial class creditorupload : System.Web.UI.Page
    {
        static string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonImport_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = new Guid().ToString() + ".csv";
                FileUpload1.SaveAs(Server.MapPath("~") + "temp\\" + filename);
                UIImport(SetImpotDT(filename));
                FileInfo f = new FileInfo(Server.MapPath("~") + "temp\\" + filename);
                f.Delete();
                Response.Redirect("bodycorps.aspx", false);
            }
            catch (Exception ex)
            {

                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        public DataTable SetImpotDT(string csvfilename)
        {
            try
            {
                CSVIO csvio = new CSVIO();
                csvio.CsvReader(Server.MapPath("~") + "temp\\" + csvfilename);
                DataTable dt = csvio.CsvImport();
                csvio.CsvClose();
                return dt;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }

        public void UIImport(DataTable dt)
        {
            Odbc mydb = null;
            try
            {
                mydb = new Odbc(constr);
                mydb.StartTransaction();
                foreach (DataRow dr in dt.Rows)
                {
                    Hashtable items = new Hashtable();
                    items.Add("creditor_master_code", DBSafeUtils.StrToQuoteSQL(dr["CreditorCode"].ToString()));
                    items.Add("creditor_master_name", DBSafeUtils.StrToQuoteSQL(dr["CreditorName"].ToString()));
                    CreditorMaster creditor = new CreditorMaster(constr);
                    creditor.SetOdbc(mydb);
                    creditor.Add(items);
                    

                }
                mydb.Commit();

                
                foreach (DataRow dr in dt.Rows)
                {
                    mydb.StartTransaction();
                    CreditorMaster creditor = new CreditorMaster(constr);
                    
                    creditor.LoadData(dr["CreditorCode"].ToString());

                    if (dr["Address1"].ToString() != "") InsertComm(dr, creditor, 1, "Address1");
                    if (dr["Address2"].ToString() != "") InsertComm(dr, creditor, 1, "Address2");
                    if (dr["Address3"].ToString() != "") InsertComm(dr, creditor, 1, "Address3");
                    if (dr["Address4"].ToString() != "") InsertComm(dr, creditor, 1, "Address4");
                    if (dr["Address5"].ToString() != "") InsertComm(dr, creditor, 1, "Address5");
                    if (dr["Post1"].ToString() != "") InsertComm(dr, creditor, 2, "Post1");
                    if (dr["Post2"].ToString() != "") InsertComm(dr, creditor, 2, "Post2");
                    if (dr["Post3"].ToString() != "") InsertComm(dr, creditor, 2, "Post3");
                    if (dr["PH_BUS"].ToString() != "") InsertComm(dr, creditor, 3, "PH_BUS");
                    if (dr["PH_RES"].ToString() != "") InsertComm(dr, creditor, 4, "PH_RES");
                    if (dr["MOB"].ToString() != "") InsertComm(dr, creditor, 5, "MOB");
                    if (dr["FAX"].ToString() != "") InsertComm(dr, creditor, 6, "FAX");
                    if (dr["EMAIL"].ToString() != "") InsertComm(dr, creditor, 6, "EMAIL");
                    mydb.Commit();
                }
                
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        private void InsertComm(DataRow dr, CreditorMaster creditor, int type_id, string colname)
        {
            try
            {
                Hashtable comm_items = new Hashtable();

                comm_items.Add("comm_master_type_id", type_id);
                comm_items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr[colname].ToString()));
                comm_items.Add("comm_master_primary", 0);

                creditor.AddComm(comm_items);
            }
            catch (Exception ex)
            {
                throw ex;
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

        protected void ImageButtonTemplate_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("CreditorCode");
                dt.Columns.Add("CreditorName");
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
                CSVIO csvio = new CSVIO();

                string filefolder = Server.MapPath("~/temp");
                string filename = Guid.NewGuid().ToString();
                csvio.CsvWriter(filefolder + "\\" + filename +".csv");
                csvio.CsvExport(dt);
                csvio.CsvClose();
                FileInfo fo = new FileInfo(filefolder + "\\" + filename + ".csv");
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
                HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
    }
}