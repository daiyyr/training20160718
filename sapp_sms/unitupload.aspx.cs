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
    public partial class unitupload : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
        }


        protected void Button1_Click1(object sender, System.EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn); o.StartTransaction();
            string name = "";
            string id = "";
            string d = "";
            try
            {

                FileUpload1.SaveAs(Server.MapPath("~") + "temp\\1.csv");

                {
                    int row_idx = 1;  // Add 19/06/2016

                    DataTable dt = CsvDT.CsvToDataTable(Server.MapPath("~") + "temp\\", "1.csv");
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_idx = row_idx + 1;

                        // Add 19/06/2016
                        if ("".Equals(dr["DebtorName"].ToString()))
                        {
                            throw new Exception(CsvDT.editErrorMessage(row_idx, "DebtorName", "debtor master name is empty"));
                        }

                        // Add 19/06/2016
                        if ("".Equals(dr["Code"].ToString()))
                        {
                            throw new Exception(CSVIO.editErrorMessage(row_idx, "Code", "unit master code is empty"));
                        }
                        UnitMaster um = new UnitMaster(AdFunction.conn);
                        um.SetOdbc(o);
                        um.LoadData(DBSafeUtils.StrToQuoteSQL(dr["Code"].ToString()));
                        if (um.UnitMasterId > 0)
                        {
                            throw new Exception(CsvDT.editErrorMessage(row_idx, "Code", "unit master master code [" + dr["Code"].ToString() + "] is duplicated"));
                        }

                        // Add Start 19/06/2016
                        if ("".Equals(dr["DebtorCode"].ToString()) == false)
                        {
                            DebtorMaster dm = new DebtorMaster(AdFunction.conn);
                            dm.SetOdbc(o);
                            dm.LoadData(dr["DebtorCode"].ToString());
                            if (dm.DebtorMasterId == 0)
                            {
                                throw new Exception(CsvDT.editErrorMessage(row_idx, "DebtorCode", "debtor master code [" + dr["DebtorCode"].ToString() + "] could not be found in DB"));
                            }
                        }

                        string did = "";
                        string dname = dr["DebtorName"].ToString().Replace("'", "''");
                        DataTable ddt = o.ReturnTable("select * from debtor_master where debtor_master_name='" + dname + "'", "t1");
                        if (ddt.Rows.Count == 0)
                        {
                            Hashtable ht = new Hashtable();
                            ht.Add("debtor_master_type_id", "1");
                            ht.Add("debtor_master_code", "'" + Guid.NewGuid().ToString() + "'");
                            ht.Add("debtor_master_name", "'" + dname + "'");
                            ht.Add("debtor_master_bodycorp_id", Request.Cookies["bodycorpid"].Value.ToString());
                            ht.Add("debtor_master_payment_term_id", "1");
                            ht.Add("debtor_master_payment_type_id", "6");
                            ht.Add("debtor_master_print", "0");
                            ht.Add("debtor_master_email", "0");
                            o.ExecuteInsert("debtor_master", ht);

                            string idsql = "SELECT LAST_INSERT_ID()";
                            string gid = Convert.ToInt32(o.ExecuteScalar(idsql)).ToString();
                            did = gid;
                            InsertComm(o, gid, "Address1", "1", dr["Address1"].ToString(), "1");
                            InsertComm(o, gid, "Address2", "1", dr["Address2"].ToString(), "2");
                            InsertComm(o, gid, "Address3", "1", dr["Address3"].ToString(), "3");
                            InsertComm(o, gid, "Address4", "1", dr["Address4"].ToString(), "4");
                            InsertComm(o, gid, "Address5", "1", dr["Address5"].ToString(), "5");
                            InsertComm(o, gid, "Post1", "2", dr["Post1"].ToString(), "1");
                            InsertComm(o, gid, "Post2", "2", dr["Post2"].ToString(), "2");
                            InsertComm(o, gid, "Post3", "2", dr["Post3"].ToString(), "3");
                            InsertComm(o, gid, "PH_BUS", "3", dr["PH_BUS"].ToString(), "3");
                            InsertComm(o, gid, "PH_RES", "3", dr["PH_RES"].ToString(), "4");
                            InsertComm(o, gid, "MOB", "5", dr["MOB"].ToString(), "5");
                            InsertComm(o, gid, "FAX", "6", dr["FAX"].ToString(), "6");
                            InsertComm(o, gid, "EMAIL", "7", dr["EMAIL"].ToString(), "7");
                        }
                        else
                            did = ddt.Rows[0][0].ToString();
                        Hashtable uht = new Hashtable();
                        uht.Add("unit_master_code", "'" + dr["Code"].ToString() + "'");
                        uht.Add("unit_master_debtor_code", "'" + dr["DebtorCode"].ToString() + "'");
                        uht.Add("unit_master_type_id", "1");
                        uht.Add("unit_master_property_id", Request.QueryString["propertyid"].ToString());
                        uht.Add("unit_master_debtor_id", did);
                        uht.Add("unit_master_areatype_id", "1");
                        uht.Add("unit_master_ownership_interest", dr["OI"].ToString());
                        uht.Add("unit_master_utility_interest", dr["UI"].ToString());
                        uht.Add("unit_master_notes", "'" + dr["Description"].ToString() + "'");
                        uht.Add("unit_master_begin_date", "'0000-00-00'");

                        o.ExecuteInsert("unit_master", uht);
                    }
                    FileInfo f = new FileInfo(Server.MapPath("~") + "temp\\1.csv");
                    f.Delete();
                    o.Commit();
                }
                {
                    string sql = "select * from debtor_master";
                    DataTable dt = o.ReturnTable(sql, "t");
                    foreach (DataRow dr in dt.Rows)
                    {
                        o.ExecuteScalar("update debtor_master set debtor_master_code='" + Guid.NewGuid().ToString() + "' where debtor_master_id=" + dr["debtor_master_id"].ToString());
                    }
                    foreach (DataRow dr in dt.Rows)
                    {
                        id = dr["debtor_master_id"].ToString();
                        name = dr["debtor_master_name"].ToString();
                        string code = "";

                        char[] s = name.ToCharArray();
                        if (s.Length > 0)
                        {
                            code = s[0].ToString();
                            for (int i = 1; i < s.Length; i++)
                            {
                                if (s[i - 1].ToString().Equals(" "))
                                {
                                    code += s[i].ToString();
                                }
                            }
                            code = code + "-" + dr["debtor_master_id"].ToString();

                            string update = "update debtor_master set debtor_master_code='" + code + "' where debtor_master_id=" + dr["debtor_master_id"].ToString();
                            o.ExecuteScalar(update);
                        }
                    }
                }
                Response.Redirect("bodycorps.aspx", false);
            }
            catch (Exception ex)
            {
                o.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString() + " debtor : " + name + " row:" + id; HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        public void InsertComm(Odbc o, string debtorID, string col, string typeid, string data, string order = "")
        {
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

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Code");
            dt.Columns.Add("DebtorCode");
            dt.Columns.Add("Type");
            dt.Columns.Add("Description");
            dt.Columns.Add("AreaSqm");
            dt.Columns.Add("AreaType");
            dt.Columns.Add("OI");
            dt.Columns.Add("UI");
            dt.Columns.Add("DebtorName");
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
            CsvDT.DataTableToCsv(dt, Server.MapPath("~/Temp/UnitMasterTemplate.csv"));
            Response.Redirect("~/Temp/UnitMasterTemplate.csv");
        }
    }
}