using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.Odbc;
using System.Collections;
using Sapp.SMS;
using Sapp.Data;
using System.Data;
using System.Collections;

namespace sapp_sms
{
    public partial class home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!IsPostBack)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    Odbc mydb = null;
                    try
                    {
                        mydb = new Odbc(constr);
                        string sql = "SELECT COUNT(*) AS `count`, `property_type_code` FROM `property_master` LEFT JOIN `property_types` ON `property_master_type_id`=`property_type_id` GROUP BY `property_type_id`";
                        OdbcDataReader dr = mydb.Reader(sql);
                        int i = 0;
                        while (dr.Read())
                        {
                            ChartBC.Series["Series1"].Points.AddXY(dr["property_type_code"].ToString(), Convert.ToInt32(dr["count"]));
                            ChartBC.Series["Series1"].Points[i].AxisLabel = Convert.ToInt32(dr["count"]).ToString();
                            ChartBC.Series["Series1"].Points[i].LegendText = dr["property_type_code"].ToString();
                        }
                    }
                    finally
                    {
                        if (mydb != null) mydb.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Odbc mydb = null;
            string PMO_CODE = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                mydb = new Odbc(constr);
                mydb.StartTransaction();
                //string sql = "SELECT * FROM `temp_debtor`";
                //string sql = "SELECT * FROM `temp_unit` INNER JOIN `temp_debtor` ON `PMO_UNIT_CODE`=`PMA_CODE`";
                //string sql = "SELECT * FROM `chart_master` WHERE `chart_master_type_id`=2";
                //string sql = "SELECT * FROM `temp_chart`";
                //string sql = "SELECT * FROM `unit_master` WHERE `unit_master_property_id`=1";
                //string sql = "SELECT * FROM `debtor_master`";
                //string sql = "SELECT * FROM `temp_owner`";
                //string sql = "SELECT * FROM `temp_metropolis`";
                string sql = "SELECT * FROM `temp_unit_ent`";
                //string sql = "SELECT * FROM `unit_master` LEFT JOIN ``";

                ArrayList deleted_id = new ArrayList();
                OdbcDataReader dr = mydb.Reader(sql);
                while (dr.Read())
                {
                    #region Import Debtor
                    //Hashtable items = new Hashtable();
                    //items.Add("debtor_master_code", DBSafeUtils.StrToQuoteSQL(dr["PMO_CODE"].ToString()));
                    //items.Add("debtor_master_name", DBSafeUtils.StrToQuoteSQL(dr["PMO_NAME"].ToString()));
                    //items.Add("debtor_master_type_id", 1);
                    //items.Add("debtor_master_salutation", DBSafeUtils.StrToQuoteSQL(dr["PMO_LETTER"].ToString()));
                    //items.Add("debtor_master_payment_term_id", 1);
                    //items.Add("debtor_master_payment_type_id", 6);
                    //items.Add("debtor_master_print", 1);
                    //items.Add("debtor_master_email", 0);
                    //DebtorMaster debtor = new DebtorMaster(constr);
                    //debtor.SetOdbc(mydb);
                    //debtor.Add(items);
                    #endregion
                    #region Import Comms
                    //DebtorMaster debtor = new DebtorMaster(constr);
                    //debtor.SetOdbc(mydb);
                    //debtor.LoadData(dr["PMO_CODE"].ToString());
                    //if (dr["PMO_PH_BUS"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 3);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["PMO_PH_BUS"].ToString()));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["PMO_PH_RES"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 4);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["PMO_PH_RES"].ToString()));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["PMO_PH_MOB"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 5);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["PMO_PH_MOB"].ToString()));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["PMO_FAX"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 6);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["PMO_FAX"].ToString()));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["PMO_EMAIL"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 7);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["PMO_EMAIL"].ToString()));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["PMO_ADD1"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 1);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["PMO_ADD1"].ToString()));
                    //    items.Add("comm_master_order", debtor.TelOrder(1));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["PMO_ADD2"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 1);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["PMO_ADD2"].ToString()));
                    //    items.Add("comm_master_order", debtor.TelOrder(1));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["PMO_ADD3"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 1);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["PMO_ADD3"].ToString()));
                    //    items.Add("comm_master_order", debtor.TelOrder(1));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["PMO_MAIL1"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 2);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["PMO_MAIL1"].ToString()));
                    //    items.Add("comm_master_order", debtor.TelOrder(2));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["PMO_MAIL2"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 2);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["PMO_MAIL2"].ToString()));
                    //    items.Add("comm_master_order", debtor.TelOrder(2));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["PMO_MAIL3"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 2);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["PMO_MAIL3"].ToString()));
                    //    items.Add("comm_master_order", debtor.TelOrder(2));
                    //    debtor.AddComm(items);
                    //}
                    #endregion
                    #region Import Units
                    //Hashtable items = new Hashtable();
                    //items.Add("unit_master_code", DBSafeUtils.StrToQuoteSQL(dr["PMA_CODE"].ToString()));
                    //items.Add("unit_master_type_id", 1);
                    //items.Add("unit_master_property_id", 1);
                    //DebtorMaster debtor = new DebtorMaster(constr);
                    //debtor.SetOdbc(mydb);
                    //PMO_CODE = dr["PMO_CODE"].ToString();
                    //debtor.LoadData(dr["PMO_CODE"].ToString());
                    //items.Add("unit_master_debtor_id", debtor.DebtorMasterId);
                    //items.Add("unit_master_areatype_id", 4);
                    //items.Add("unit_master_ownership_interest", DBSafeUtils.DecimalToSQL(dr["PMO_SHARE"].ToString()));
                    //items.Add("unit_master_utility_interest", DBSafeUtils.DecimalToSQL(dr["PMO_UTILITY"].ToString()));
                    //PropertyMaster property = new PropertyMaster(constr);
                    //property.SetOdbc(mydb);
                    //property.LoadData(1);
                    //property.AddUnit(items);
                    #endregion
                    #region Import Chart
                    //if (dr["AC_BASE"].ToString() == "1")
                    //{
                    //    ChartMaster chart = new ChartMaster(constr);
                    //    chart.SetOdbc(mydb);
                    //    chart.LoadData(dr["AC_CODE"].ToString());
                    //    Hashtable items = new Hashtable();
                    //    items.Add("chart_master_levy_base", 1);
                    //    chart.Update(items);
                    //}
                    #endregion
                    #region Import Ownership
                    //Hashtable items = new Hashtable();
                    //items.Add("debtor_master_name", DBSafeUtils.StrToQuoteSQL(dr["OLD_NAME"].ToString()));
                    //items.Add("debtor_master_type_id", 1);
                    //items.Add("debtor_master_payment_term_id", 1);
                    //items.Add("debtor_master_payment_type_id", 6);
                    //items.Add("debtor_master_print", 1);
                    //items.Add("debtor_master_email", 0);
                    //DebtorMaster debtor = new DebtorMaster(constr);
                    //debtor.SetOdbc(mydb);
                    //debtor.Add(items);
                    //------------------------------------------------
                    //string debtor_master_name = dr["debtor_master_name"].ToString();
                    //int debtor_master_id = Convert.ToInt32(dr["debtor_master_id"]);

                    //if (!deleted_id.Contains(debtor_master_id))
                    //{
                    //    sql = "SELECT * FROM `debtor_master` WHERE `debtor_master_name`=" + DBSafeUtils.StrToQuoteSQL(debtor_master_name);
                    //    OdbcDataReader dr2 = mydb.Reader(sql);
                    //    while (dr2.Read())
                    //    {
                    //        int debtor_master_id2 = Convert.ToInt32(dr2["debtor_master_id"]);

                    //        if (debtor_master_id != debtor_master_id2)
                    //        {
                    //            sql = "SELECT * FROM `unit_master` WHERE `unit_master_debtor_id`=" + debtor_master_id2;
                    //            OdbcDataReader dr3 = mydb.Reader(sql);
                    //            while (dr3.Read())
                    //            {
                    //                UnitMaster unit = new UnitMaster(constr);
                    //                unit.SetOdbc(mydb);
                    //                unit.LoadData(Convert.ToInt32(dr3["unit_master_id"]));
                    //                Hashtable items = new Hashtable();
                    //                items.Add("unit_master_debtor_id", debtor_master_id);
                    //                unit.Update(items);
                    //            }
                    //            DebtorMaster debtor = new DebtorMaster(constr);
                    //            debtor.SetOdbc(mydb);
                    //            debtor.LoadData(Convert.ToInt32(dr2["debtor_master_id"]));
                    //            PMO_CODE = debtor.DebtorMasterId.ToString();
                    //            debtor.Delete();
                    //            deleted_id.Add(debtor.DebtorMasterId);
                    //        }
                    //    }
                    //}
                    //---------------------------------------------------
                    //string OLD_NAME = dr["OLD_NAME"].ToString();
                    //string OLD_ADD1 = dr["OLD_ADD1"].ToString();
                    //string OLD_ADD2 = dr["OLD_ADD2"].ToString();
                    //string OLD_ADD3 = dr["OLD_ADD3"].ToString();
                    //string OLD_PH_BUS = dr["OLD_PH_BUS"].ToString();
                    //string OLD_PH_RES = dr["OLD_PH_RES"].ToString();
                    //string OLD_PH_MOB = dr["OLD_PH_MOB"].ToString();
                    //string OLD_FAX = dr["OLD_FAX"].ToString();
                    //string OLD_EMAIL = dr["OLD_EMAIL"].ToString();
                    //sql = "SELECT * FROM `debtor_master` WHERE `debtor_master_name`=" + DBSafeUtils.StrToQuoteSQL(OLD_NAME);
                    //OdbcDataReader dr2 = mydb.Reader(sql);
                    //while (dr2.Read())
                    //{
                    //    int debtor_master_id = Convert.ToInt32(dr2["debtor_master_id"]);
                    //    if (!deleted_id.Contains(debtor_master_id))
                    //    {
                    //        DebtorMaster debtor = new DebtorMaster(constr);
                    //        debtor.SetOdbc(mydb);
                    //        debtor.LoadData(debtor_master_id);
                    //        deleted_id.Add(debtor.DebtorMasterId);
                    //        if (dr["OLD_PH_BUS"].ToString() != "")
                    //        {
                    //            Hashtable items = new Hashtable();
                    //            items.Add("comm_master_type_id", 3);
                    //            items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["OLD_PH_BUS"].ToString()));
                    //            debtor.AddComm(items);
                    //        }
                    //        if (dr["OLD_PH_RES"].ToString() != "")
                    //        {
                    //            Hashtable items = new Hashtable();
                    //            items.Add("comm_master_type_id", 4);
                    //            items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["OLD_PH_RES"].ToString()));
                    //            debtor.AddComm(items);
                    //        }
                    //        if (dr["OLD_PH_MOB"].ToString() != "")
                    //        {
                    //            Hashtable items = new Hashtable();
                    //            items.Add("comm_master_type_id", 5);
                    //            items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["OLD_PH_MOB"].ToString()));
                    //            debtor.AddComm(items);
                    //        }
                    //        if (dr["OLD_FAX"].ToString() != "")
                    //        {
                    //            Hashtable items = new Hashtable();
                    //            items.Add("comm_master_type_id", 6);
                    //            items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["OLD_FAX"].ToString()));
                    //            debtor.AddComm(items);
                    //        }
                    //        if (dr["OLD_EMAIL"].ToString() != "")
                    //        {
                    //            Hashtable items = new Hashtable();
                    //            items.Add("comm_master_type_id", 7);
                    //            items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["OLD_EMAIL"].ToString()));
                    //            debtor.AddComm(items);
                    //        }
                    //        if (dr["OLD_ADD1"].ToString() != "")
                    //        {
                    //            Hashtable items = new Hashtable();
                    //            items.Add("comm_master_type_id", 1);
                    //            items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["OLD_ADD1"].ToString()));
                    //            items.Add("comm_master_order", debtor.TelOrder(1));
                    //            debtor.AddComm(items);
                    //        }
                    //        if (dr["OLD_ADD2"].ToString() != "")
                    //        {
                    //            Hashtable items = new Hashtable();
                    //            items.Add("comm_master_type_id", 1);
                    //            items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["OLD_ADD2"].ToString()));
                    //            items.Add("comm_master_order", debtor.TelOrder(1));
                    //            debtor.AddComm(items);
                    //        }
                    //        if (dr["OLD_ADD3"].ToString() != "")
                    //        {
                    //            Hashtable items = new Hashtable();
                    //            items.Add("comm_master_type_id", 1);
                    //            items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["OLD_ADD3"].ToString()));
                    //            items.Add("comm_master_order", debtor.TelOrder(1));
                    //            debtor.AddComm(items);
                    //        }
                    //    }
                    //}
                    //------------------------------------------------------
                    //string OLD_NAME = dr["OLD_NAME"].ToString();
                    //string OLD_UNIT_CODE = dr["OLD_UNIT_CODE"].ToString() ;
                    //string OLD_LOADED = dr["OLD_LOADED"].ToString();
                    //string OLD_FINISHED = dr["OLD_FINISHED"].ToString();
                    //DateTime start = Convert.ToDateTime(OLD_LOADED);
                    //DateTime end = Convert.ToDateTime(OLD_FINISHED);
                    //sql = "SELECT * FROM `debtor_master` WHERE `debtor_master_name`=" + DBSafeUtils.StrToQuoteSQL(OLD_NAME);
                    //OdbcDataReader dr2 = mydb.Reader(sql);
                    //if (dr2.Read())
                    //{
                    //    DebtorMaster debtor = new DebtorMaster(constr);
                    //    debtor.SetOdbc(mydb);
                    //    debtor.LoadData(Convert.ToInt32(dr2["debtor_master_id"]));
                    //    if (OLD_UNIT_CODE != "")
                    //    {
                    //        sql = "SELECT * FROM `unit_master` WHERE `unit_master_code`=" + DBSafeUtils.StrToQuoteSQL(OLD_UNIT_CODE);
                    //        OdbcDataReader dr3 = mydb.Reader(sql);
                    //        if (dr3.Read())
                    //        {
                    //            int unit_id = Convert.ToInt32(dr3["unit_master_id"]);
                    //            Ownership ownership = new Ownership(constr);
                    //            ownership.SetOdbc(mydb);
                    //            Hashtable items = new Hashtable();
                    //            items.Add("ownership_unit_id", unit_id);
                    //            items.Add("ownership_debtor_id", debtor.DebtorMasterId);
                    //            items.Add("ownership_start", DBSafeUtils.DateToSQL(start));
                    //            items.Add("ownership_end", DBSafeUtils.DateToSQL(end));
                    //            ownership.Add(items);
                    //        }
                    //    }
                    //}
                    #endregion
                    #region Import Metropolis
                    //-----------------Debtor--------------------------
                    //Hashtable items = new Hashtable();
                    //items.Add("debtor_master_code", DBSafeUtils.StrToQuoteSQL(dr["ID"].ToString()));
                    //items.Add("debtor_master_name", DBSafeUtils.StrToQuoteSQL(dr["Name"].ToString()));
                    //items.Add("debtor_master_type_id", 1);
                    //items.Add("debtor_master_payment_term_id", 1);
                    //items.Add("debtor_master_payment_type_id", 6);
                    //items.Add("debtor_master_print", 1);
                    //items.Add("debtor_master_email", 0);
                    //DebtorMaster debtor = new DebtorMaster(constr);
                    //debtor.SetOdbc(mydb);
                    //debtor.Add(items);
                    //-----------------Comms----------------------------
                    //DebtorMaster debtor = new DebtorMaster(constr);
                    //debtor.SetOdbc(mydb);
                    //debtor.LoadData(dr["ID"].ToString());
                    //if (dr["Tel"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 3);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["Tel"].ToString()));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["Tel2"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 3);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["Tel2"].ToString()));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["Tel3"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 3);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["Tel3"].ToString()));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["Tel4"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 3);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["Tel4"].ToString()));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["Mobile"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 5);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["Mobile"].ToString()));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["Email"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 7);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["Email"].ToString()));
                    //    debtor.AddComm(items);
                    //}
                    //if (dr["Address"].ToString() != "")
                    //{
                    //    Hashtable items = new Hashtable();
                    //    items.Add("comm_master_type_id", 1);
                    //    items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(dr["Address"].ToString()));
                    //    items.Add("comm_master_order", debtor.TelOrder(1));
                    //    debtor.AddComm(items);
                    //}
                    //-------------------Units--------------------
                    //Hashtable items = new Hashtable();
                    //items.Add("unit_master_code", DBSafeUtils.StrToQuoteSQL(dr["Unit"].ToString()));
                    //items.Add("unit_master_type_id", 1);
                    //items.Add("unit_master_property_id", 1);
                    //DebtorMaster debtor = new DebtorMaster(constr);
                    //debtor.SetOdbc(mydb);
                    //PMO_CODE = dr["ID"].ToString();
                    //debtor.LoadData(dr["ID"].ToString());
                    //items.Add("unit_master_debtor_id", debtor.DebtorMasterId);
                    //items.Add("unit_master_areatype_id", 4);
                    //items.Add("unit_master_ownership_interest", 0.24);
                    //items.Add("unit_master_utility_interest", 0.24);
                    //PropertyMaster property = new PropertyMaster(constr);
                    //property.SetOdbc(mydb);
                    //property.LoadData(1);
                    //property.AddUnit(items);
                    #endregion
                    #region Import Metropolis Unit Interest
                    //------------------Import Principle Unit------------
                    //string unit_code = dr["CODE"].ToString();
                    //sql = "UPDATE `unit_master` SET `unit_master_ownership_interest`=" + DBSafeUtils.DecimalToSQL(dr["TO 100%"].ToString())
                    //    + ", `unit_master_utility_interest`=" + DBSafeUtils.DecimalToSQL(dr["TO 100%"].ToString()) + " WHERE `unit_master_code` LIKE '%" + unit_code + "%'";
                    //mydb.NonQuery(sql);
                    //------------------Import Accessory Unit------------
                    string unit_code = dr["CODE"].ToString();
                    string notes = "";

                    for (int i = 1; i < 10; i++)
                    {
                        if (dr["AU" + i] != DBNull.Value)
                        {
                            if (dr["AU" + i].ToString() != "")
                            {
                                notes += dr["AU" + i].ToString() + ", ";
                            }
                        }
                    }
                    if (notes != "") notes = notes.Substring(0, notes.Length - 2);
                    sql = "UPDATE `unit_master` SET `unit_master_notes`=" + DBSafeUtils.StrToQuoteSQL(notes)
                        + " WHERE `unit_master_code` LIKE '%" + unit_code + "%'";
                    mydb.NonQuery(sql);

                    #endregion
                }
                mydb.Commit();
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Response.Write(ex.ToString() + "| count:" + PMO_CODE);
            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        #region WebControl Events
        protected void ImageButtonBodycorp_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/bodycorps.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void ImageButtonProperty_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/propertymaster.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonReports_Click(object sender, ImageClickEventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string sql = "SELECT  invoice_master . * FROM invoice_master WHERE ( invoice_master_date = '2013-6-17' )";
            string sql2 = "SELECT        gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_ref, gl_transactions.gl_transaction_chart_id,                           gl_transactions.gl_transaction_bodycorp_id, gl_transactions.gl_transaction_unit_id, gl_transactions.gl_transaction_description,                           gl_transactions.gl_transaction_batch_id, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date, invoice_master.invoice_master_id FROM            invoice_master, invoice_gls, gl_transactions WHERE        invoice_master.invoice_master_id = invoice_gls.invoice_gl_invoice_id AND invoice_gls.invoice_gl_gl_id = gl_transactions.gl_transaction_id ";
            string idsql = "SELECT LAST_INSERT_ID()";
            Odbc o = new Odbc(constr);
            DataTable invDT = o.ReturnTable(sql, "Inv");
            DataTable glDT = o.ReturnTable(sql2, "gl");
            o.StartTransaction();
            InvoiceMaster im = new InvoiceMaster(constr);
            im.SetOdbc(o);
            foreach (DataRow invdr in invDT.Rows)
            {
                if (invdr["invoice_master_description"].Equals("Special Maintenance  Levy O/S"))
                {
                    string id = invdr["invoice_master_id"].ToString();
                    Hashtable items = new Hashtable();
                    items.Add("invoice_master_unit_id", DBSafeUtils.IntToSQL(invdr["invoice_master_unit_id"].ToString()));
                    items.Add("invoice_master_debtor_id", invdr["invoice_master_debtor_id"].ToString());
                    items.Add("invoice_master_bodycorp_id", invdr["invoice_master_bodycorp_id"].ToString());
                    items.Add("invoice_master_type_id", "2");
                    items.Add("invoice_master_num", DBSafeUtils.StrToQuoteSQL(im.GetNextCreditNumber(-1)));
                    items.Add("invoice_master_date", DBSafeUtils.DateTimeToSQL("2013-10-17"));
                    items.Add("invoice_master_due", DBSafeUtils.DateTimeToSQL("2013-10-17"));
                    items.Add("invoice_master_description", DBSafeUtils.StrToQuoteSQL(invdr["invoice_master_description"].ToString()));
                    o.ExecuteInsert("invoice_master", items);
                    object newInvID = o.ExecuteScalar(idsql);
                    DataTable gldt = ReportDT.FilterDT(glDT, "invoice_master_id=" + id);
                    foreach (DataRow gldr in gldt.Rows)
                    {
                        Hashtable glItems = new Hashtable();
                        string chartid = gldr["gl_transaction_chart_id"].ToString();
                        decimal net = -decimal.Parse(gldr["gl_transaction_net"].ToString());
                        decimal Tax = -decimal.Parse(gldr["gl_transaction_tax"].ToString());
                        decimal gross = -decimal.Parse(gldr["gl_transaction_gross"].ToString());
                        glItems.Add("gl_transaction_type_id", gldr["gl_transaction_type_id"].ToString());
                        glItems.Add("gl_transaction_ref", DBSafeUtils.StrToQuoteSQL(gldr["gl_transaction_ref"].ToString()));
                        glItems.Add("gl_transaction_chart_id", chartid);
                        glItems.Add("gl_transaction_bodycorp_id", gldr["gl_transaction_bodycorp_id"].ToString());
                        glItems.Add("gl_transaction_unit_id", DBSafeUtils.IntToSQL(gldr["gl_transaction_unit_id"].ToString()));
                        glItems.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(gldr["gl_transaction_description"].ToString()));
                        glItems.Add("gl_transaction_net", DBSafeUtils.DecimalToSQL(net));
                        glItems.Add("gl_transaction_tax", DBSafeUtils.DecimalToSQL(Tax));
                        glItems.Add("gl_transaction_gross", DBSafeUtils.DecimalToSQL(gross));
                        glItems.Add("gl_transaction_date", DBSafeUtils.DateTimeToSQL("2013-10-17"));
                        o.ExecuteInsert("gl_transactions", glItems);
                        object glid = o.ExecuteScalar(idsql);
                        Hashtable invgl = new Hashtable();
                        invgl.Add("invoice_gl_invoice_id", newInvID);
                        invgl.Add("invoice_gl_gl_id", glid);
                        invgl.Add("invoice_gl_paid", "0");
                        o.ExecuteInsert("invoice_gls", invgl);
                    }
                    im.LoadData(int.Parse(newInvID.ToString()));
                    im.UpdatePaid();
                    im.UpdateTotal();
                }


            }
            o.Commit();

            Response.Redirect("creditnotemaster.aspx",false);
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            FixGL();
        }
        //public void FixGL2()
        //{
        //    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //    Odbc mydb = new Odbc(constr);

        //    string recsql = "SELECT        gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_ref, gl_transactions.gl_transaction_chart_id,                           gl_transactions.gl_transaction_bodycorp_id, gl_transactions.gl_transaction_unit_id, gl_transactions.gl_transaction_description,                           gl_transactions.gl_transaction_batch_id, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date, unit_master.unit_master_code, unit_master.unit_master_id, receipts.receipt_notes, receipts.receipt_ref,                           debtor_master.debtor_master_code, debtor_master.debtor_master_name FROM            receipt_gls, gl_transactions, receipts, unit_master, debtor_master WHERE        receipt_gls.receipt_gl_gl_id = gl_transactions.gl_transaction_id AND receipt_gls.receipt_gl_receipt_id = receipts.receipt_id AND                           receipts.receipt_unit_id = unit_master.unit_master_id AND receipts.receipt_debtor_id = debtor_master.debtor_master_id ";

        //    string cinvsql = "SELECT        gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_ref, gl_transactions.gl_transaction_chart_id,                           gl_transactions.gl_transaction_bodycorp_id, gl_transactions.gl_transaction_unit_id, gl_transactions.gl_transaction_description,                           gl_transactions.gl_transaction_batch_id, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date, cinvoices.cinvoice_num, cinvoices.cinvoice_order_id, cinvoices.cinvoice_description, creditor_master.creditor_master_code,                           creditor_master.creditor_master_name FROM            gl_transactions, cinvoice_gls, cinvoices, creditor_master WHERE        gl_transactions.gl_transaction_id = cinvoice_gls.cinvoice_gl_gl_id AND cinvoice_gls.cinvoice_gl_cinvoice_id = cinvoices.cinvoice_id AND                           cinvoices.cinvoice_creditor_id = creditor_master.creditor_master_id ";

        //    string cpayment = "SELECT        gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_ref, gl_transactions.gl_transaction_chart_id,                           gl_transactions.gl_transaction_bodycorp_id, gl_transactions.gl_transaction_unit_id, gl_transactions.gl_transaction_description,                           gl_transactions.gl_transaction_batch_id, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date, creditor_master.creditor_master_code, creditor_master.creditor_master_name, cpayments.cpayment_reference,                           cpayments.cpayment_date FROM            gl_transactions, cpayment_gls, creditor_master, cpayments WHERE        gl_transactions.gl_transaction_id = cpayment_gls.cpayment_gl_gl_id AND cpayment_gls.cpayment_gl_cpayment_id = cpayments.cpayment_id AND                           creditor_master.creditor_master_id = cpayments.cpayment_creditor_id ";

        //    string invsql = "SELECT        gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_ref, gl_transactions.gl_transaction_chart_id,                           gl_transactions.gl_transaction_bodycorp_id, gl_transactions.gl_transaction_unit_id, gl_transactions.gl_transaction_description,                           gl_transactions.gl_transaction_batch_id, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date, unit_master.unit_master_code, unit_master.unit_master_id, invoice_master.invoice_master_description,                           invoice_master.invoice_master_date, invoice_master.invoice_master_due, debtor_master.debtor_master_code, debtor_master.debtor_master_name FROM            gl_transactions, invoice_gls, invoice_master, unit_master, debtor_master WHERE        gl_transactions.gl_transaction_id = invoice_gls.invoice_gl_gl_id AND invoice_gls.invoice_gl_invoice_id = invoice_master.invoice_master_id AND                           invoice_master.invoice_master_unit_id = unit_master.unit_master_id AND invoice_master.invoice_master_debtor_id = debtor_master.debtor_master_id ";
        //    DataTable RecDT = mydb.ReturnTable(recsql, "temp");
        //    DataTable InvDT = mydb.ReturnTable(invsql, "temp");
        //    DataTable CPayDT = mydb.ReturnTable(cpayment, "temp");
        //    DataTable CInvDT = mydb.ReturnTable(cinvsql, "temp");
        //    DataTable glDT = ReportDT.getTable(constr, "gl_transactions");

        //    for (int i = 0; i < glDT.Rows.Count; i++)
        //    {
        //        int gid = int.Parse(glDT.Rows[i]["gl_transaction_id"].ToString());
        //        int unitid = int.Parse(glDT.Rows[i]["gl_transaction_id"].ToString());
        //        string typeid = glDT.Rows[i]["gl_transaction_type_id"].ToString();
        //        string description = glDT.Rows[i]["gl_transaction_description"].ToString();
        //        string chartid = glDT.Rows[i]["gl_transaction_chart_id"].ToString();
        //        //if (typeid.Equals("1"))
        //        //{
        //        //    updateGl2(gid.ToString(), description, chartid);
        //        //}
        //    }
        //}
        //protected void FixGL1()
        //{
        //    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //    Odbc mydb = new Odbc(constr);

        //    string recsql = "SELECT        gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_ref, gl_transactions.gl_transaction_chart_id,                           gl_transactions.gl_transaction_bodycorp_id, gl_transactions.gl_transaction_unit_id, gl_transactions.gl_transaction_description,                           gl_transactions.gl_transaction_batch_id, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date, unit_master.unit_master_code, unit_master.unit_master_id, receipts.receipt_notes, receipts.receipt_ref,                           debtor_master.debtor_master_code, debtor_master.debtor_master_name FROM            receipt_gls, gl_transactions, receipts, unit_master, debtor_master WHERE        receipt_gls.receipt_gl_gl_id = gl_transactions.gl_transaction_id AND receipt_gls.receipt_gl_receipt_id = receipts.receipt_id AND                           receipts.receipt_unit_id = unit_master.unit_master_id AND receipts.receipt_debtor_id = debtor_master.debtor_master_id ";

        //    string cinvsql = "SELECT        gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_ref, gl_transactions.gl_transaction_chart_id,                           gl_transactions.gl_transaction_bodycorp_id, gl_transactions.gl_transaction_unit_id, gl_transactions.gl_transaction_description,                           gl_transactions.gl_transaction_batch_id, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date, cinvoices.cinvoice_num, cinvoices.cinvoice_order_id, cinvoices.cinvoice_description, creditor_master.creditor_master_code,                           creditor_master.creditor_master_name FROM            gl_transactions, cinvoice_gls, cinvoices, creditor_master WHERE        gl_transactions.gl_transaction_id = cinvoice_gls.cinvoice_gl_gl_id AND cinvoice_gls.cinvoice_gl_cinvoice_id = cinvoices.cinvoice_id AND                           cinvoices.cinvoice_creditor_id = creditor_master.creditor_master_id ";

        //    string cpayment = "SELECT        gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_ref, gl_transactions.gl_transaction_chart_id,                           gl_transactions.gl_transaction_bodycorp_id, gl_transactions.gl_transaction_unit_id, gl_transactions.gl_transaction_description,                           gl_transactions.gl_transaction_batch_id, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date, creditor_master.creditor_master_code, creditor_master.creditor_master_name, cpayments.cpayment_reference,                           cpayments.cpayment_date FROM            gl_transactions, cpayment_gls, creditor_master, cpayments WHERE        gl_transactions.gl_transaction_id = cpayment_gls.cpayment_gl_gl_id AND cpayment_gls.cpayment_gl_cpayment_id = cpayments.cpayment_id AND                           creditor_master.creditor_master_id = cpayments.cpayment_creditor_id ";

        //    string invsql = "SELECT        gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_ref, gl_transactions.gl_transaction_chart_id,                           gl_transactions.gl_transaction_bodycorp_id, gl_transactions.gl_transaction_unit_id, gl_transactions.gl_transaction_description,                           gl_transactions.gl_transaction_batch_id, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date, unit_master.unit_master_code, unit_master.unit_master_id, invoice_master.invoice_master_description,                           invoice_master.invoice_master_date, invoice_master.invoice_master_due, debtor_master.debtor_master_code, debtor_master.debtor_master_name FROM            gl_transactions, invoice_gls, invoice_master, unit_master, debtor_master WHERE        gl_transactions.gl_transaction_id = invoice_gls.invoice_gl_gl_id AND invoice_gls.invoice_gl_invoice_id = invoice_master.invoice_master_id AND                           invoice_master.invoice_master_unit_id = unit_master.unit_master_id AND invoice_master.invoice_master_debtor_id = debtor_master.debtor_master_id ";
        //    DataTable RecDT = mydb.ReturnTable(recsql, "temp");
        //    DataTable InvDT = mydb.ReturnTable(invsql, "temp");
        //    DataTable CPayDT = mydb.ReturnTable(cpayment, "temp");
        //    DataTable CInvDT = mydb.ReturnTable(cinvsql, "temp");
        //    DataTable glDT = ReportDT.getTable(constr, "gl_transactions");
        //    if (glDT.Columns.Contains("gl_transaction_ref_type_id"))
        //        for (int i = 0; i < glDT.Rows.Count; i++)
        //        {
        //            int gid = int.Parse(glDT.Rows[i]["gl_transaction_id"].ToString());
        //            int unitid = int.Parse(glDT.Rows[i]["gl_transaction_id"].ToString());
        //            string typeid = glDT.Rows[i]["gl_transaction_type_id"].ToString();
        //            string reftypeid = glDT.Rows[i]["gl_transaction_ref_type_id"].ToString();
        //            if (!typeid.Equals("5"))
        //            {
        //                int before2ID = 0;
        //                int beforeID = 0;
        //                int after2ID = 0;
        //                int afterID = 0;
        //                if (i > 2 && i < glDT.Rows.Count - 2)
        //                {
        //                    before2ID = i - 2;
        //                    beforeID = i - 1;
        //                    after2ID = i + 2;
        //                    afterID = i + 1;
        //                }
        //                decimal b2net = decimal.Parse(glDT.Rows[before2ID]["gl_transaction_net"].ToString());
        //                decimal bnet = decimal.Parse(glDT.Rows[beforeID]["gl_transaction_net"].ToString());
        //                decimal anet = decimal.Parse(glDT.Rows[afterID]["gl_transaction_net"].ToString());
        //                decimal a2net = decimal.Parse(glDT.Rows[after2ID]["gl_transaction_net"].ToString());
        //                decimal net = decimal.Parse(glDT.Rows[i]["gl_transaction_net"].ToString());
        //                int id2 = 0;
        //                int id1 = 0;
        //                if (b2net + bnet + net == 0)
        //                {
        //                    string checkREF = glDT.Rows[beforeID]["gl_transaction_ref"].ToString();
        //                    string checkREF2 = glDT.Rows[before2ID]["gl_transaction_ref"].ToString();
        //                    string checkunit = glDT.Rows[beforeID]["gl_transaction_ref"].ToString();
        //                    string checkunit2 = glDT.Rows[before2ID]["gl_transaction_ref"].ToString();
        //                    if (checkREF.Equals(checkREF2))
        //                    {
        //                        if (checkunit.Equals(checkunit2))
        //                        {
        //                            id2 = before2ID;
        //                            id1 = beforeID;
        //                        }
        //                    }
        //                }
        //                else if (anet + a2net + net == 0)
        //                {
        //                    string checkREF = glDT.Rows[afterID]["gl_transaction_ref"].ToString();
        //                    string checkREF2 = glDT.Rows[after2ID]["gl_transaction_ref"].ToString();
        //                    string checkunit = glDT.Rows[afterID]["gl_transaction_ref"].ToString();
        //                    string checkunit2 = glDT.Rows[after2ID]["gl_transaction_ref"].ToString();
        //                    if (checkREF.Equals(checkREF2))
        //                    {
        //                        if (checkunit.Equals(checkunit2))
        //                        {
        //                            id2 = after2ID;
        //                            id1 = afterID;
        //                        }
        //                    }
        //                }

        //                else if (b2net + bnet + net == net)
        //                {
        //                    id2 = before2ID;
        //                    id1 = beforeID;
        //                }

        //                else if (anet + a2net + net == net)
        //                {
        //                    id2 = after2ID;
        //                    id1 = afterID;
        //                }
        //                updateGl(gid.ToString(), typeid);
        //                if (id2 != 0)
        //                {
        //                    string gid2 = glDT.Rows[id2]["gl_transaction_id"].ToString();
        //                    updateGl(gid2, typeid);
        //                }
        //                if (id1 != 0)
        //                {
        //                    string gid1 = glDT.Rows[id1]["gl_transaction_id"].ToString();
        //                    updateGl(gid1, typeid);
        //                }
        //            }
        //        }
        //}
        public void FixGL()
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            Odbc mydb = new Odbc(constr);
            string recsql = "SELECT        gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_ref, gl_transactions.gl_transaction_chart_id,                           gl_transactions.gl_transaction_bodycorp_id, gl_transactions.gl_transaction_unit_id, gl_transactions.gl_transaction_description,                           gl_transactions.gl_transaction_batch_id, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date, unit_master.unit_master_code, unit_master.unit_master_id, receipts.receipt_notes, receipts.receipt_ref,                           debtor_master.debtor_master_code, debtor_master.debtor_master_name FROM            receipt_gls, gl_transactions, receipts, unit_master, debtor_master WHERE        receipt_gls.receipt_gl_gl_id = gl_transactions.gl_transaction_id AND receipt_gls.receipt_gl_receipt_id = receipts.receipt_id AND                           receipts.receipt_unit_id = unit_master.unit_master_id AND receipts.receipt_debtor_id = debtor_master.debtor_master_id ";
            string cinvsql = "SELECT        gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_ref, gl_transactions.gl_transaction_chart_id,                           gl_transactions.gl_transaction_bodycorp_id, gl_transactions.gl_transaction_unit_id, gl_transactions.gl_transaction_description,                           gl_transactions.gl_transaction_batch_id, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date, cinvoices.cinvoice_num, cinvoices.cinvoice_order_id, cinvoices.cinvoice_description, creditor_master.creditor_master_code,                           creditor_master.creditor_master_name FROM            gl_transactions, cinvoice_gls, cinvoices, creditor_master WHERE        gl_transactions.gl_transaction_id = cinvoice_gls.cinvoice_gl_gl_id AND cinvoice_gls.cinvoice_gl_cinvoice_id = cinvoices.cinvoice_id AND                           cinvoices.cinvoice_creditor_id = creditor_master.creditor_master_id ";
            string cpayment = "SELECT        gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_ref, gl_transactions.gl_transaction_chart_id,                           gl_transactions.gl_transaction_bodycorp_id, gl_transactions.gl_transaction_unit_id, gl_transactions.gl_transaction_description,                           gl_transactions.gl_transaction_batch_id, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date, creditor_master.creditor_master_code, creditor_master.creditor_master_name, cpayments.cpayment_reference,                           cpayments.cpayment_date FROM            gl_transactions, cpayment_gls, creditor_master, cpayments WHERE        gl_transactions.gl_transaction_id = cpayment_gls.cpayment_gl_gl_id AND cpayment_gls.cpayment_gl_cpayment_id = cpayments.cpayment_id AND                           creditor_master.creditor_master_id = cpayments.cpayment_creditor_id ";
            string invsql = "SELECT        gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_ref, gl_transactions.gl_transaction_chart_id,                           gl_transactions.gl_transaction_bodycorp_id, gl_transactions.gl_transaction_unit_id, gl_transactions.gl_transaction_description,                           gl_transactions.gl_transaction_batch_id, gl_transactions.gl_transaction_net, gl_transactions.gl_transaction_tax, gl_transactions.gl_transaction_gross,                           gl_transactions.gl_transaction_date, unit_master.unit_master_code, unit_master.unit_master_id, invoice_master.invoice_master_description,                           invoice_master.invoice_master_date, invoice_master.invoice_master_due, debtor_master.debtor_master_code, debtor_master.debtor_master_name FROM            gl_transactions, invoice_gls, invoice_master, unit_master, debtor_master WHERE        gl_transactions.gl_transaction_id = invoice_gls.invoice_gl_gl_id AND invoice_gls.invoice_gl_invoice_id = invoice_master.invoice_master_id AND                           invoice_master.invoice_master_unit_id = unit_master.unit_master_id AND invoice_master.invoice_master_debtor_id = debtor_master.debtor_master_id ";
            DataTable RecDT = mydb.ReturnTable(recsql, "temp");
            DataTable InvDT = mydb.ReturnTable(invsql, "temp");
            DataTable CPayDT = mydb.ReturnTable(cpayment, "temp");
            DataTable CInvDT = mydb.ReturnTable(cinvsql, "temp");
            DataTable glDT = ReportDT.getTable(constr, "gl_transactions");

            foreach (DataRow dr in glDT.Rows)
            {
                string gid = dr["gl_transaction_id"].ToString();
                string typeid = dr["gl_transaction_type_id"].ToString();
                if (typeid.Equals("1"))
                {
                    updateGl(gid, "1");
                }
                if (typeid.Equals("2"))
                {
                    updateGl(gid, "2");
                }

                if (typeid.Equals("3"))
                {
                    updateGl(gid, "3");
                }

                if (typeid.Equals("4"))
                {
                    updateGl(gid, "4");
                }
                if (typeid.Equals("6"))
                {
                    updateGl(gid, "6");
                }
                if (!ReportDT.GetDataByColumn(RecDT, "gl_transaction_id", gid, "gl_transaction_id").Equals(""))
                {
                    if (typeid.Equals("5"))
                    {
                        updateGl(gid, "3");
                    }
                }
                if (!ReportDT.GetDataByColumn(InvDT, "gl_transaction_id", gid, "gl_transaction_id").Equals(""))
                {
                    if (typeid.Equals("5"))
                    {
                        updateGl(gid, "1");
                    }
                }
                if (!ReportDT.GetDataByColumn(CPayDT, "gl_transaction_id", gid, "gl_transaction_id").Equals(""))
                {
                    if (typeid.Equals("5"))
                    {
                        updateGl(gid, "4");
                    }

                }
                if (!ReportDT.GetDataByColumn(CInvDT, "gl_transaction_id", gid, "gl_transaction_id").Equals(""))
                {
                    if (typeid.Equals("5"))
                    {
                        updateGl(gid, "2");
                    }
                }
            }


        }
        public void updateGl(string id, string typeid)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            Odbc o = new Odbc(constr);
            string sql = "UPDATE gl_transactions SET gl_transaction_ref_type_id = " + typeid + " where  gl_transaction_id =" + id;
            o.ReturnTable(sql, "s");
        }
        //public void updateGl2(string id, string description, string oldchartid)
        //{
        //    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //    Odbc o = new Odbc(constr);
        //    string chartid = "0";
        //    if (description.Equals("2nd Instal Op. Levy O/S"))
        //    {
        //        chartid = "391";

        //    }
        //    if (description.Equals("Special Maintenance  Levy O/S"))
        //    {
        //        chartid = "371";
        //    }
        //    if (description.Equals("Special Levy 1st Instalment"))
        //    {
        //        chartid = "371";
        //    }
        //    if (description.Equals("Special Levy 1st Installment"))
        //    {
        //        chartid = "371";
        //    }
        //    if (!chartid.Equals("0"))
        //    {
        //        if (oldchartid.Equals("285"))
        //        {
        //            string sql = "UPDATE gl_transactions SET gl_transaction_chart_id =  " + chartid + " where  gl_transaction_id =" + id;
        //            o.ReturnTable(sql, "s");
        //        }
        //    }
        //}

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            Odbc o = new Odbc(constr);
            string sql = "UPDATE gl_transactions SET gl_transaction_description =  'Special Levy 1st Instalment' where  gl_transaction_description = 'Special Levy 1st Installment'";
            o.ReturnTable(sql, "s");
            sql = "UPDATE invoice_master SET invoice_master_description =  'Special Levy 1st Instalment' where  invoice_master_description = 'Special Levy 1st Installment'";
            o.ReturnTable(sql, "s");
        }

        protected void ImageButton3_Click(object sender, ImageClickEventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            o.StartTransaction();
            try
            {
                string sql = "SELECT  gl_transactions.gl_transaction_id FROM      invoice_master, invoice_gls, gl_transactions WHERE   invoice_master.invoice_master_id = invoice_gls.invoice_gl_invoice_id AND    invoice_gls.invoice_gl_gl_id = gl_transactions.gl_transaction_id AND (invoice_master.invoice_master_type_id = 2) and invoice_master_date = '2013-10-17'";
                string updategl = "update gl_transactions set gl_transaction_date = '2013-6-20' where gl_transaction_id =";
                DataTable dt = o.ReturnTable(sql, "gl");
                foreach (DataRow dr in dt.Rows)
                {
                    string gid = dr[0].ToString();
                    string upgl = updategl + gid;
                    o.ExecuteScalar(upgl);
                }
                string upinv = "update invoice_master set invoice_master_date ='2013-6-20', invoice_master_due='2013-6-20' where invoice_master_type_id=2";
                o.ExecuteScalar(upinv);
                o.Commit();
            }

            catch (Exception ex)
            {
                o.Rollback();
                throw ex;
            }
        }

    }
}