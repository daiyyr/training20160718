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
    public partial class owershipupload : System.Web.UI.Page
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
                FileUpload1.SaveAs(Server.MapPath("~") + "temp\\owership.csv");

                UIImport(SetImpotDT());
                Response.Redirect("bodycorps.aspx", false);

            }
            catch (Exception ex)
            {

                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        public DataTable SetImpotDT()
        {
            DataTable dt = CsvDT.CsvToDataTable(Server.MapPath("~") + "temp\\", "owership.csv");

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
            DataTable dt = CsvDT.CsvToDataTable(Server.MapPath("~") + "temp\\", "owership.csv");
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

        public void Transfer(string oldDebtorID, string newDebtorID, string startDate)
        {
            try
            {
                string unitsql = "select * from unit_master where unit_master_debtor_id=" + oldDebtorID;

                DataTable unitDT = o.ReturnTable(unitsql, "t33");
                if (unitDT.Rows.Count > 0)
                {
                    string unit_id = unitDT.Rows[0]["unit_master_id"].ToString();
                    UnitMaster unit = new UnitMaster(constr);
                    unit.SetOdbc(o);
                    unit.LoadData(Convert.ToInt32(unit_id));


                    DebtorMaster olddm = new DebtorMaster(constr);
                    olddm.SetOdbc(o);
                    olddm.LoadData(int.Parse(oldDebtorID));
                    DebtorMaster newdm = new DebtorMaster(constr);
                    newdm.SetOdbc(o);
                    newdm.LoadData(int.Parse(newDebtorID));
                    string nowCODE = newdm.DebtorMasterCode;
                    string oldCODE = olddm.DebtorMasterCode;
                    newdm.DebtorMasterCode = oldCODE;
                    olddm.DebtorMasterCode = nowCODE;



                    string tempCODE = Guid.NewGuid().ToString();

                    Hashtable ditems = new Hashtable();
                    ditems.Add("debtor_master_code", "'" + oldCODE + "'");
                    Hashtable titems = new Hashtable();
                    titems.Add("debtor_master_code", "'" + tempCODE + "'");
                    Hashtable oditems = new Hashtable();
                    oditems.Add("debtor_master_code", "'" + nowCODE + "'");


                    newdm.Update(titems);
                    olddm.Update(oditems);
                    newdm.Update(ditems);
                    DateTime start_date = Convert.ToDateTime(startDate);
                    DateTime last_end = new DateTime();

                    #region Get Last OS Last End Date

                    string sql = "SELECT MAX(`ownership_end`) FROM `ownerships` WHERE `ownership_unit_id`=" + unit_id;
                    object id = o.ExecuteScalar(sql);
                    if (id != DBNull.Value)
                        last_end = Convert.ToDateTime(id);
                    else
                        last_end = unit.UnitMasterBeginDate;
                    #endregion
                    #region Create New OS for Current
                    Hashtable items = new Hashtable();
                    items.Add("ownership_unit_id", unit_id);
                    items.Add("ownership_debtor_id", unit.UnitMasterDebtorId);
                    items.Add("ownership_start", DBSafeUtils.DateToSQL(last_end.AddDays(1)));
                    items.Add("ownership_end", DBSafeUtils.DateToSQL(start_date.AddDays(-1)));
                    Ownership os = new Ownership(constr);
                    os.SetOdbc(o);
                    os.Add(items);
                    #endregion
                    #region Change OS
                    items = new Hashtable();
                    items.Add("unit_master_debtor_id", newDebtorID);
                    unit.Update(items);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                if (o != null) o.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
        }



        public void UIImport(DataTable dt)
        {
            o.StartTransaction();
            try
            {
                {
                    string sql = "select * from `debtor_master`";

                    DataTable old = o.ReturnTable(sql, "oldDE");
                    foreach (DataRow newdr in dt.Rows)
                    {
                        string dsql = "select * from debtor_master where debtor_master_code='" + newdr["debtor_master_code"].ToString() + "'";
                        DataTable dDT = ReportDT.getTable(constr, "debtor_master");
                        string dcode = newdr["debtor_master_code"].ToString();
                        string code = "";
                        for (int i = 1; i < 100; i++)
                        {
                            code = dcode + "-" + i;
                            if (ReportDT.GetDataByColumn(dDT, "debtor_master_code", code, "debtor_master_id").Equals(""))
                            {
                                break;
                            }
                        }
                        dcode = code;

                        Hashtable ht = MakeHsahTable(newdr);
                        ht.Remove("debtor_master_id");
                        ht.Remove("Start");
                        ht["debtor_master_code"] = "'" + dcode + "'";
                        ht.Add("debtor_master_payment_term_id", "1");
                        ht.Add("debtor_master_payment_type_id", "6");
                        ht.Add("debtor_master_bodycorp_id", Request.Cookies["bodycorpid"].Value);
                        ht.Add("debtor_master_type_id", "1");
                        ht.Add("debtor_master_print", "0");
                        ht.Add("debtor_master_email", "0");
                        o.ExecuteInsert("debtor_master", ht);

                        string idsql = "SELECT LAST_INSERT_ID()";
                        string gid = Convert.ToInt32(o.ExecuteScalar(idsql)).ToString();


                        InsertComm(newdr["debtor_master_code"].ToString(), gid, "Address1", "1", "1");
                        InsertComm(newdr["debtor_master_code"].ToString(), gid, "Address2", "1", "2");
                        InsertComm(newdr["debtor_master_code"].ToString(), gid, "Address3", "1", "3");
                        InsertComm(newdr["debtor_master_code"].ToString(), gid, "Address4", "1", "4");
                        InsertComm(newdr["debtor_master_code"].ToString(), gid, "Address5", "1", "5");
                        InsertComm(newdr["debtor_master_code"].ToString(), gid, "Post1", "2", "1");
                        InsertComm(newdr["debtor_master_code"].ToString(), gid, "Post2", "2", "2");
                        InsertComm(newdr["debtor_master_code"].ToString(), gid, "Post3", "2", "3");
                        InsertComm(newdr["debtor_master_code"].ToString(), gid, "PH_BUS", "3");
                        InsertComm(newdr["debtor_master_code"].ToString(), gid, "PH_RES", "4");
                        InsertComm(newdr["debtor_master_code"].ToString(), gid, "MOB", "5");
                        InsertComm(newdr["debtor_master_code"].ToString(), gid, "FAX", "6");
                        InsertComm(newdr["debtor_master_code"].ToString(), gid, "EMAIL", "7");

                        DebtorMaster dm = new DebtorMaster(AdFunction.conn);
                        dm.SetOdbc(o);
                        dm.LoadData(newdr["debtor_master_code"].ToString());

                        string oldID = dm.DebtorMasterId.ToString();
                        string startdate = newdr["Start"].ToString();
                        if (!oldID.Equals("0"))
                        {
                            Transfer(oldID, gid, startdate);
                        }
                        else
                        {
                            throw new Exception("Debtor code not match. Code:" + newdr["debtor_master_code"].ToString());
                        }
                    }
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

    }
}