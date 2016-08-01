using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sapp.Data;
using System.Data.Odbc;
using System.Collections;
using Sapp.Common;
using System.Web;
using System.Data;

namespace Sapp.SMS
{
    public class Cinvoice : IGlTranHost
    {
        #region Consts
        public const int GL_TRAN_TYPE_ID = 2;
        #endregion
        #region Variables
        private string constr;
        private int cinvoice_id;
        private int? cinvoice_order_id;
        private string cinvoice_num;
        private int cinvoice_creditor_id;
        private int cinvoice_bodycorp_id;
        private int? cinvoice_unit_id;
        private bool? cinvoice_unit_admin_fee;
        private int cinvoice_type_id;
        private DateTime cinvoice_date;
        private DateTime? cinvoice_due;
        private DateTime? cinvoice_apply;
        private string cinvoice_description;
        private decimal cinvoice_net;
        private decimal cinvoice_tax;
        private decimal cinvoice_gross;
        private decimal cinvoice_paid;
        private List<GlTransaction<Cinvoice>> gltranList;
        private Odbc odbc;
        #endregion
        public Cinvoice(string constr)
        {
            try
            {
                this.constr = constr;
                gltranList = new List<GlTransaction<Cinvoice>>();
                odbc = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region Properties
        public int CinvoiceId
        {
            get
            {
                return cinvoice_id;
            }
            set
            {
                cinvoice_id = value;
            }
        }

        public int Cinvoice_type_id
        {
            get
            {
                return cinvoice_type_id;
            }
            set
            {
                cinvoice_type_id = value;
            }
        }
        public int? CinvoiceOrderId
        {
            get
            {
                return cinvoice_order_id;
            }
            set
            {
                cinvoice_order_id = value;
            }
        }
        public string CinvoiceNum
        {
            get
            {
                return cinvoice_num;
            }
            set
            {
                cinvoice_num = value;
            }
        }
        public int CinvoiceCreditorId
        {
            get
            {
                return cinvoice_creditor_id;
            }
            set
            {
                cinvoice_creditor_id = value;
            }
        }
        public int CinvoiceBodycorpId
        {
            get
            {
                return cinvoice_bodycorp_id;
            }
            set
            {
                cinvoice_bodycorp_id = value;
            }
        }
        public int? CinvoiceUnitId
        {
            get
            {
                return cinvoice_unit_id;
            }
            set
            {
                cinvoice_unit_id = value;
            }
        }
        public bool? CinvoiceUnitAdminFee
        {
            get
            {
                return cinvoice_unit_admin_fee;
            }
            set
            {
                cinvoice_unit_admin_fee = value;
            }
        }
        public DateTime CinvoiceDate
        {
            get
            {
                return cinvoice_date;
            }
            set
            {
                cinvoice_date = value;
            }
        }
        public DateTime? CinvoiceDue
        {
            get
            {
                return cinvoice_due;
            }
            set
            {
                cinvoice_due = value;
            }
        }
        public DateTime? CinvoiceApply
        {
            get
            {
                return cinvoice_apply;
            }
            set
            {
                cinvoice_apply = value;
            }
        }
        public string CinvoiceDescription
        {
            get
            {
                return cinvoice_description;
            }
            set
            {
                cinvoice_description = value;
            }
        }
        public decimal CinvoiceNet
        {
            get
            {
                return cinvoice_net;
            }
            set
            {
                cinvoice_net = value;
            }
        }
        public decimal CinvoiceTax
        {
            get
            {
                return cinvoice_tax;
            }
            set
            {
                cinvoice_tax = value;
            }
        }
        public decimal CinvoiceGross
        {
            get
            {
                return cinvoice_gross;
            }
            set
            {
                cinvoice_gross = value;
            }
        }
        public decimal CinvoicePaid
        {
            get
            {
                return cinvoice_paid;
            }
            set
            {
                cinvoice_paid = value;
            }
        }
        public List<GlTransaction<Cinvoice>> GltransactionList
        {
            get
            {
                return gltranList;
            }
            set
            {
                gltranList = value;
            }
        }
        #endregion
        #region Standard Functions
        public void LoadData(int cinvoice_id)
        {
            Odbc mydb = null;
            try
            {
                this.cinvoice_id = cinvoice_id;
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                }
                else
                {
                    mydb = odbc;
                }
                string sql = "SELECT * FROM `cinvoices` WHERE `cinvoice_id`=" + cinvoice_id;
                OdbcDataReader dr = mydb.Reader(sql);
                if (dr.Read())
                {
                    #region Initial Properties
                    this.cinvoice_id = Convert.ToInt32(dr["cinvoice_id"]);
                    cinvoice_order_id = DBSafeUtils.DBIntToIntN(dr["cinvoice_order_id"]);
                    cinvoice_num = DBSafeUtils.DBStrToStr(dr["cinvoice_num"]);
                    cinvoice_creditor_id = Convert.ToInt32(dr["cinvoice_creditor_id"]);
                    cinvoice_bodycorp_id = Convert.ToInt32(dr["cinvoice_bodycorp_id"]);
                    cinvoice_unit_id = DBSafeUtils.DBIntToIntN(dr["cinvoice_unit_id"]);
                    cinvoice_unit_admin_fee = dr["cinvoice_unit_admin_fee"].ToString() == "0" ? false : true;
                    cinvoice_date = Convert.ToDateTime(dr["cinvoice_date"]);
                    cinvoice_due = DBSafeUtils.DBDateToDateN(dr["cinvoice_due"]);
                    cinvoice_apply = DBSafeUtils.DBDateToDateN(dr["cinvoice_apply"]);
                    cinvoice_description = DBSafeUtils.DBStrToStr(dr["cinvoice_description"]);
                    cinvoice_net = Convert.ToDecimal(dr["cinvoice_net"]);
                    cinvoice_tax = Convert.ToDecimal(dr["cinvoice_tax"]);
                    cinvoice_gross = Convert.ToDecimal(dr["cinvoice_gross"]);
                    cinvoice_paid = Convert.ToDecimal(dr["cinvoice_paid"]);
                    cinvoice_type_id = Convert.ToInt32(dr["cinvoice_type_id"]);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (odbc == null)
                {
                    if (mydb != null) mydb.Close();
                }
            }
        }
        public Hashtable GetData()
        {
            try
            {
                #region Create hashtable
                Hashtable items = new Hashtable();
                items.Add("cinvoice_order_id", cinvoice_order_id);
                items.Add("cinvoice_num", cinvoice_num);
                items.Add("cinvoice_creditor_id", cinvoice_creditor_id);
                items.Add("cinvoice_bodycorp_id", cinvoice_bodycorp_id);
                items.Add("cinvoice_unit_id", cinvoice_unit_id);
                items.Add("cinvoice_unit_admin_fee", cinvoice_unit_admin_fee);
                items.Add("cinvoice_date", cinvoice_date);
                items.Add("cinvoice_due", cinvoice_due);
                items.Add("cinvoice_apply", cinvoice_apply);
                items.Add("cinvoice_description", cinvoice_description);
                items.Add("cinvoice_net", cinvoice_net);
                items.Add("cinvoice_tax", cinvoice_tax);
                items.Add("cinvoice_gross", cinvoice_gross);
                items.Add("cinvoice_paid", cinvoice_paid);
                #endregion
                return items;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Update(Hashtable items, int cinvoice_id)
        {
            Odbc mydb = null;
            bool isnull = true;
            try
            {
                #region Initial mydb Transaction
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                    this.odbc = mydb;
                    isnull = true;
                }
                else
                {
                    mydb = odbc;
                    isnull = false;
                }
                #endregion
                this.LoadData(cinvoice_id);
                int? _purchorder_id = this.cinvoice_order_id;
                mydb.ExecuteUpdate("cinvoices", items, "WHERE `cinvoice_id`=" + cinvoice_id);
                this.LoadData(cinvoice_id);

                Bodycorp b = new Bodycorp(AdFunction.conn);
                b.LoadData(cinvoice_bodycorp_id);
                if (!b.CheckCloseOff(cinvoice_date))
                {
                    throw new Exception("Invoice before close date");
                }

                #region Update Related Data
                if (this.cinvoice_order_id != _purchorder_id)
                {
                    if (this.cinvoice_order_id != null)
                    {
                        if (_purchorder_id == null)
                        {
                            PurchorderMaster purchorder = new PurchorderMaster(constr);
                            purchorder.SetOdbc(mydb);
                            purchorder.SetAllocatedFlag(this.cinvoice_order_id.Value, true);
                        }
                        else
                        {
                            PurchorderMaster purchorder = new PurchorderMaster(constr);
                            purchorder.SetOdbc(mydb);
                            purchorder.SetAllocatedFlag(this.cinvoice_order_id.Value, true);
                            purchorder.SetAllocatedFlag(_purchorder_id.Value, false);
                        }
                    }
                    else
                    {
                        PurchorderMaster purchorder = new PurchorderMaster(constr);
                        purchorder.SetOdbc(mydb);
                        purchorder.SetAllocatedFlag(_purchorder_id.Value, false);
                    }
                }
                this.LoadTransactions();
                foreach (GlTransaction<Cinvoice> gltran in gltranList)
                {
                    Hashtable gl_items = new Hashtable();
                    gl_items["gl_transaction_type_id"] = GL_TRAN_TYPE_ID;
                    gl_items["gl_transaction_ref"] = this.cinvoice_id.ToString();
                    gl_items["gl_transaction_bodycorp_id"] = this.cinvoice_bodycorp_id.ToString();
                    gl_items["gl_transaction_unit_id"] = DBSafeUtils.IntToSQL(this.cinvoice_unit_id);
                    gl_items["gl_transaction_date"] = DBSafeUtils.DateToSQL(this.cinvoice_date);
                    mydb.ExecuteUpdate("gl_transactions", gl_items, "WHERE `gl_transaction_id`=" + gltran.GlTransactionId);
                }
                int GENERALCREDITOR = 0;
                int GENERALTAX = 0;
                #region Load General Creditor Control and General Tax Chart Account
                ChartMaster chart = new ChartMaster(constr);
                chart.SetOdbc(mydb);
                Sapp.SMS.System system = new System(constr);
                system.SetOdbc(mydb);
                system.LoadData("GENERALCREDITOR");
                chart.LoadData(system.SystemValue);
                GENERALCREDITOR = chart.ChartMasterId;
                system.LoadData("GST Input");
                chart.LoadData(system.SystemValue);
                GENERALTAX = chart.ChartMasterId;
                #endregion
                #region Update Balance Sheet
                string sql = "SELECT `gl_transaction_id` FROM `cinvoice_gls` LEFT JOIN `gl_transactions` ON `cinvoice_gl_gl_id`=`gl_transaction_id` "
                    + "WHERE `cinvoice_gl_cinvoice_id`=" + this.cinvoice_id + " AND `gl_transaction_type_id`=5 AND `gl_transaction_chart_id`=" + GENERALCREDITOR;
                OdbcDataReader dr = mydb.Reader(sql);
                while (dr.Read())
                {
                    Hashtable gl_items = new Hashtable();
                    gl_items.Add("gl_transaction_type_id", 5);
                    gl_items.Add("gl_transaction_ref", this.cinvoice_id.ToString());


                    gl_items.Add("gl_transaction_chart_id", GENERALCREDITOR);
                    gl_items.Add("gl_transaction_bodycorp_id", this.cinvoice_bodycorp_id);
                    gl_items.Add("gl_transaction_unit_id", DBSafeUtils.IntToSQL(this.cinvoice_unit_id));
                    gl_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(this.cinvoice_description));
                    gl_items.Add("gl_transaction_net", cinvoice_gross);
                    gl_items.Add("gl_transaction_tax", 0);
                    gl_items.Add("gl_transaction_gross", 0);
                    gl_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cinvoice_date));
                    mydb.ExecuteUpdate("gl_transactions", gl_items, "WHERE `gl_transaction_id`=" + Convert.ToInt32(dr["gl_transaction_id"]));
                }
                sql = "SELECT `gl_transaction_id` FROM `cinvoice_gls` LEFT JOIN `gl_transactions` ON `cinvoice_gl_gl_id`=`gl_transaction_id` "
                    + "WHERE `cinvoice_gl_cinvoice_id`=" + this.cinvoice_id + " AND `gl_transaction_type_id`=5 AND `gl_transaction_chart_id`=" + GENERALTAX;
                dr = mydb.Reader(sql);
                while (dr.Read())
                {
                    Hashtable gl_items = new Hashtable();
                    gl_items.Add("gl_transaction_type_id", 5);
                    gl_items.Add("gl_transaction_ref", this.cinvoice_id.ToString());
                    gl_items.Add("gl_transaction_chart_id", GENERALTAX);
                    gl_items.Add("gl_transaction_bodycorp_id", this.cinvoice_bodycorp_id);
                    gl_items.Add("gl_transaction_unit_id", DBSafeUtils.IntToSQL(this.cinvoice_unit_id));
                    gl_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(this.cinvoice_description));
                    gl_items.Add("gl_transaction_net", -cinvoice_tax);
                    gl_items.Add("gl_transaction_tax", 0);
                    gl_items.Add("gl_transaction_gross", 0);
                    gl_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cinvoice_date));
                    mydb.ExecuteUpdate("gl_transactions", gl_items, "WHERE `gl_transaction_id`=" + Convert.ToInt32(dr["gl_transaction_id"]));
                }
                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                #region Close mydb Transaction
                if (isnull)
                {
                    if (mydb != null) mydb.Close();
                    odbc = null;
                }
                #endregion
            }
        }
        public void Update(Hashtable items)
        {
            try
            {
                this.Update(items, this.cinvoice_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Add(Hashtable items)
        {
            try
            {
                this.Add(items, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Delete(int cinvoice_id)
        {
            Odbc mydb = null;
            bool isnull = true;
            try
            {
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                    this.odbc = mydb;
                    isnull = true;
                }
                else
                {
                    mydb = odbc;
                    isnull = false;
                }
                mydb.StartTransaction();
                this.LoadData(cinvoice_id);
                #region Check If Any Paid
                string sql = "SELECT * FROM `cinvoice_gls` LEFT JOIN `gl_transactions` ON `cinvoice_gl_gl_id`=`gl_transaction_id` "
                    + "WHERE `cinvoice_gl_cinvoice_id`=" + this.cinvoice_id + " AND `gl_transaction_type_id`=4";
                OdbcDataReader dr = mydb.Reader(sql);
                if (dr.Read())
                {
                    throw new Exception("Error: please delete all related payment first!");
                }
                #endregion
                #region Release Purchorder
                if (this.cinvoice_order_id.HasValue)
                {
                    PurchorderMaster purchorder = new PurchorderMaster(constr);
                    purchorder.SetOdbc(mydb);
                    purchorder.SetAllocatedFlag(this.cinvoice_order_id.Value, false);
                }
                #endregion
                #region Delete Related Gls
                sql = "SELECT * FROM `cinvoice_gls` LEFT JOIN `gl_transactions` ON `cinvoice_gl_gl_id`=`gl_transaction_id` "
                    + "WHERE `cinvoice_gl_cinvoice_id`=" + cinvoice_id;
                dr = mydb.Reader(sql);
                while (dr.Read())
                {
                    CinvoiceGl cinvoicegl = new CinvoiceGl(constr);
                    cinvoicegl.SetOdbc(mydb);
                    cinvoicegl.LoadData(Convert.ToInt32(dr["cinvoice_gl_id"]));
                    cinvoicegl.Delete();
                    GlTransaction<Cinvoice> gltran = new GlTransaction<Cinvoice>(constr);
                    gltran.SetOdbc(mydb);
                    gltran.LoadData(Convert.ToInt32(dr["gl_transaction_id"]));
                    gltran.Drop(mydb);
                }
                #endregion
                sql = "DELETE FROM `cinvoices` WHERE `cinvoice_id`=" + cinvoice_id;
                mydb.NonQuery(sql);
                mydb.Commit();
            }
            catch (Exception ex)
            {
                if (isnull)
                {
                    if (mydb != null) mydb.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (isnull)
                {
                    if (mydb != null) mydb.Close();
                    odbc = null;
                }
            }
        }
        public void Delete()
        {
            try
            {
                this.Delete(this.cinvoice_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Special Functions
        public void LoadData(string cinvoice_num)
        {
            Odbc mydb = null;
            bool isnull = true;
            try
            {
                #region Initial mydb Transaction
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                    this.odbc = mydb;
                    isnull = true;
                }
                else
                {
                    mydb = odbc;
                    isnull = false;
                }
                #endregion
                string sql = "SELECT * FROM `cinvoices` WHERE `cinvoice_num`=" + DBSafeUtils.StrToQuoteSQL(cinvoice_num);
                OdbcDataReader dr = mydb.Reader(sql);
                if (dr.Read())
                {
                    this.LoadData(Convert.ToInt32(dr["cinvoice_id"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                #region Close mydb Transaction
                if (isnull)
                {
                    if (mydb != null) mydb.Close();
                    odbc = null;
                }
                #endregion
            }
        }
        public int Add(Hashtable items, bool isRetLastId)
        {
            Odbc mydb = null;
            bool isnull = true;
            try
            {
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                    this.odbc = mydb;
                    isnull = true;
                }
                else
                {
                    mydb = odbc;
                    isnull = false;
                }
                mydb.StartTransaction();
                mydb.ExecuteInsert("cinvoices", items);
                string sql = "SELECT LAST_INSERT_ID()";
                object id = mydb.ExecuteScalar(sql);
                this.cinvoice_id = Convert.ToInt32(id);
                this.LoadData(this.cinvoice_id);
                if (this.cinvoice_order_id != null)
                {
                    PurchorderMaster purchorder = new PurchorderMaster(constr);
                    purchorder.SetOdbc(mydb);
                    purchorder.SetAllocatedFlag(this.cinvoice_order_id.Value, true);
                }
                #region Add Balance Sheet
                int GENERALCREDITOR = 0;
                int GENERALTAX = 0;
                #region Load General Creditor Control and General Tax Chart Account
                ChartMaster chart = new ChartMaster(constr);
                chart.SetOdbc(mydb);
                Sapp.SMS.System system = new System(constr);
                system.SetOdbc(mydb);
                system.LoadData("GENERALCREDITOR");
                chart.LoadData(system.SystemValue);
                GENERALCREDITOR = chart.ChartMasterId;
                system.LoadData("GST Input");
                chart.LoadData(system.SystemValue);
                GENERALTAX = chart.ChartMasterId;


                Bodycorp b = new Bodycorp(AdFunction.conn);
                b.LoadData(cinvoice_bodycorp_id);
                if (!b.CheckCloseOff(cinvoice_date))
                {
                    throw new Exception("Invoice before close date");
                }

                #endregion
                Hashtable gl_items = new Hashtable();
                int gl_id = 0;
                #region INSERT General Creditor Control
                gl_items.Add("gl_transaction_type_id", 5);
                gl_items.Add("gl_transaction_ref", this.cinvoice_id.ToString());
                gl_items.Add("gl_transaction_ref_type_id", "2");
                gl_items.Add("gl_transaction_chart_id", GENERALCREDITOR);
                gl_items.Add("gl_transaction_bodycorp_id", this.cinvoice_bodycorp_id);
                if (cinvoice_unit_id != null)
                    gl_items.Add("gl_transaction_unit_id", DBSafeUtils.IntToSQL(this.cinvoice_unit_id));
                gl_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(this.cinvoice_description));
                gl_items.Add("gl_transaction_net", 0);
                gl_items.Add("gl_transaction_tax", 0);
                gl_items.Add("gl_transaction_gross", 0);
                gl_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cinvoice_date));
                mydb.ExecuteInsert("gl_transactions", gl_items);
                sql = "SELECT LAST_INSERT_ID()";
                gl_id = Convert.ToInt32(mydb.ExecuteScalar(sql));
                Hashtable cinvoice_gl_items = new Hashtable();
                cinvoice_gl_items.Add("cinvoice_gl_cinvoice_id", this.CinvoiceId);
                cinvoice_gl_items.Add("cinvoice_gl_gl_id", gl_id);
                CinvoiceGl cinvoicegl = new CinvoiceGl(constr);
                cinvoicegl.SetOdbc(mydb);
                cinvoicegl.Add(cinvoice_gl_items, mydb);
                #endregion
                #region INSERT General Tax
                gl_items = new Hashtable();
                gl_items.Add("gl_transaction_type_id", 5);
                gl_items.Add("gl_transaction_ref", this.cinvoice_id.ToString());
                gl_items.Add("gl_transaction_ref_type_id", "2");
                gl_items.Add("gl_transaction_chart_id", GENERALTAX);

                gl_items.Add("gl_transaction_bodycorp_id", this.cinvoice_bodycorp_id);
                if (cinvoice_unit_id != null)
                    gl_items.Add("gl_transaction_unit_id", DBSafeUtils.IntToSQL(this.cinvoice_unit_id));
                gl_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(this.cinvoice_description));
                gl_items.Add("gl_transaction_net", 0);
                gl_items.Add("gl_transaction_tax", 0);
                gl_items.Add("gl_transaction_gross", 0);
                gl_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cinvoice_date));
                mydb.ExecuteInsert("gl_transactions", gl_items);
                sql = "SELECT LAST_INSERT_ID()";
                gl_id = Convert.ToInt32(mydb.ExecuteScalar(sql));
                cinvoice_gl_items = new Hashtable();
                cinvoice_gl_items.Add("cinvoice_gl_cinvoice_id", this.CinvoiceId);
                cinvoice_gl_items.Add("cinvoice_gl_gl_id", gl_id);
                cinvoicegl.Add(cinvoice_gl_items, odbc);
                #endregion
                #endregion
                mydb.Commit();
                return this.cinvoice_id;
            }
            catch (Exception ex)
            {
                if (isnull)
                {
                    if (mydb != null) mydb.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (isnull)
                {
                    if (mydb != null) mydb.Close();
                    odbc = null;
                }
            }
        }
        public string GetGLTranJSON()
        {
            try
            {
                Hashtable items = new Hashtable();
                ArrayList glList = new ArrayList();
                foreach (GlTransaction<Cinvoice> gl_tran in gltranList)
                {
                    Hashtable glitems = new Hashtable();
                    glitems.Add("gl_transaction_id", gl_tran.GlTransactionId.ToString());
                    glitems.Add("gl_transaction_type_id", gl_tran.GLTransactionTypeId.ToString());
                    glitems.Add("gl_transaction_ref_type_id", "2");
                    glitems.Add("gl_transaction_ref", gl_tran.GLTransactionRef);
                    glitems.Add("gl_transaction_chart_id", gl_tran.GlTransactionChartId.ToString());
                    glitems.Add("gl_transaction_description", gl_tran.GlTransactionDescription);
                    glitems.Add("gl_transaction_net", (-gl_tran.GlTransactionNet).ToString());
                    glitems.Add("gl_transaction_tax", (-gl_tran.GlTransactionTax).ToString());
                    glitems.Add("gl_transaction_gross", (-gl_tran.GlTransactionGross).ToString());

                    glList.Add(glitems);
                }
                items.Add("gl_transactions", glList);
                return JSON.JsonEncode(items);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ChartMaster> GetChartMasters()
        {
            Odbc mydb = null;
            try
            {
                #region Initial mydb Standard
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                }
                else
                {
                    mydb = odbc;
                }
                #endregion
                List<ChartMaster> list = new List<ChartMaster>();
                string sql = "SELECT * FROM `chart_master`";
                OdbcDataReader dr = mydb.Reader(sql);
                while (dr.Read())
                {
                    ChartMaster chartMaster = new ChartMaster(constr);
                    chartMaster.SetOdbc(mydb);
                    chartMaster.LoadData(Convert.ToInt32(dr["chart_master_id"]));
                    ChartType charttype = new ChartType(constr);
                    charttype.SetOdbc(mydb);
                    charttype.LoadData(Convert.ToInt32(dr["chart_master_type_id"]));
                    //if chart class = Costs
                    //if (charttype.ChartTypeClassId == 0)
                    //{
                    list.Add(chartMaster);
                    //}
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                #region Close mydb Standard
                if (odbc == null)
                {
                    if (mydb != null) mydb.Close();
                }
                #endregion
            }
        }
        private void UpdateTotal()
        {
            Odbc mydb = null;
            bool isnull = true;
            try
            {
                #region Initial mydb Transaction
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                    this.odbc = mydb;
                    isnull = true;
                }
                else
                {
                    mydb = odbc;
                    isnull = false;
                }
                #endregion
                this.LoadTransactions();
                decimal cinvoice_net = 0;
                decimal cinvoice_tax = 0;
                decimal cinvoice_gross = 0;
                int GENERALCREDITOR = 0;
                int GENERALTAX = 0;
                #region Load General Creditor Control and General Tax Chart Account
                ChartMaster chart = new ChartMaster(constr);
                chart.SetOdbc(mydb);
                Sapp.SMS.System system = new System(constr);
                system.SetOdbc(mydb);
                system.LoadData("GENERALCREDITOR");
                chart.LoadData(system.SystemValue);
                GENERALCREDITOR = chart.ChartMasterId;
                system.LoadData("GST Input");
                chart.LoadData(system.SystemValue);
                GENERALTAX = chart.ChartMasterId;
                #endregion
                #region Update Cinvoice
                foreach (GlTransaction<Cinvoice> gltran in gltranList)
                {
                    cinvoice_net += -gltran.GlTransactionNet;
                    cinvoice_tax += -gltran.GlTransactionTax;
                }
                cinvoice_gross = cinvoice_net + cinvoice_tax;
                Hashtable items = new Hashtable();
                items.Add("cinvoice_net", cinvoice_net);
                items.Add("cinvoice_tax", cinvoice_tax);
                items.Add("cinvoice_gross", cinvoice_gross);
                this.Update(items);
                #endregion
                #region Update Balance Sheet
                string sql = "SELECT `gl_transaction_id` FROM `cinvoice_gls` LEFT JOIN `gl_transactions` ON `cinvoice_gl_gl_id`=`gl_transaction_id` "
                    + "WHERE `cinvoice_gl_cinvoice_id`=" + this.cinvoice_id + " AND `gl_transaction_type_id`=5 AND `gl_transaction_chart_id`=" + GENERALCREDITOR;
                OdbcDataReader dr = mydb.Reader(sql);
                while (dr.Read())
                {
                    Hashtable gl_items = new Hashtable();
                    gl_items.Add("gl_transaction_type_id", 5);
                    gl_items.Add("gl_transaction_ref", this.cinvoice_id.ToString());
                    gl_items.Add("gl_transaction_ref_type_id", "2");
                    gl_items.Add("gl_transaction_chart_id", GENERALCREDITOR);
                    gl_items.Add("gl_transaction_bodycorp_id", this.cinvoice_bodycorp_id);
                    gl_items.Add("gl_transaction_unit_id", DBSafeUtils.IntToSQL(this.cinvoice_unit_id));
                    gl_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(this.cinvoice_description));
                    gl_items.Add("gl_transaction_net", cinvoice_gross);
                    gl_items.Add("gl_transaction_tax", 0);
                    gl_items.Add("gl_transaction_gross", 0);
                    gl_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cinvoice_date));
                    mydb.ExecuteUpdate("gl_transactions", gl_items, "WHERE `gl_transaction_id`=" + Convert.ToInt32(dr["gl_transaction_id"]));
                }
                sql = "SELECT `gl_transaction_id` FROM `cinvoice_gls` LEFT JOIN `gl_transactions` ON `cinvoice_gl_gl_id`=`gl_transaction_id` "
                    + "WHERE `cinvoice_gl_cinvoice_id`=" + this.cinvoice_id + " AND `gl_transaction_type_id`=5 AND `gl_transaction_chart_id`=" + GENERALTAX;
                dr = mydb.Reader(sql);
                while (dr.Read())
                {
                    Hashtable gl_items = new Hashtable();
                    gl_items.Add("gl_transaction_type_id", 5);
                    gl_items.Add("gl_transaction_ref", this.cinvoice_id.ToString());
                    gl_items.Add("gl_transaction_ref_type_id", "2");
                    gl_items.Add("gl_transaction_chart_id", GENERALTAX);
                    gl_items.Add("gl_transaction_bodycorp_id", this.cinvoice_bodycorp_id);
                    gl_items.Add("gl_transaction_unit_id", DBSafeUtils.IntToSQL(this.cinvoice_unit_id));
                    gl_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(this.cinvoice_description));
                    gl_items.Add("gl_transaction_net", -cinvoice_tax);
                    gl_items.Add("gl_transaction_tax", 0);
                    gl_items.Add("gl_transaction_gross", 0);
                    gl_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cinvoice_date));
                    mydb.ExecuteUpdate("gl_transactions", gl_items, "WHERE `gl_transaction_id`=" + Convert.ToInt32(dr["gl_transaction_id"]));
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                #region Close mydb Transaction
                if (isnull)
                {
                    if (mydb != null) mydb.Close();
                    odbc = null;
                }
                #endregion
            }
        }
        public void UpdatePaid()
        {
            Odbc mydb = null;
            try
            {
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                }
                else
                {
                    mydb = odbc;
                }
                this.odbc = mydb;
                string sql = "SELECT * FROM `cinvoice_gls` LEFT JOIN `gl_transactions` ON `cinvoice_gl_gl_id`=`gl_transaction_id` "
                    + "WHERE `cinvoice_gl_cinvoice_id`=" + this.cinvoice_id + " AND `gl_transaction_type_id`=4";
                OdbcDataReader dr = mydb.Reader(sql);
                decimal paid = 0;
                while (dr.Read())
                {
                    paid += Convert.ToDecimal(dr["gl_transaction_net"]);

                }
                //if (cinvoice_type_id == 1)
                //{
                //    if (paid > cinvoice_gross) throw new Exception("Error: invalid paid value!");
                //}
                //else
                //    if (paid < cinvoice_gross) throw new Exception("Error: invalid paid value!");
                Hashtable items = new Hashtable();
                items.Add("cinvoice_paid", paid);
                this.Update(items);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (odbc == null)
                {
                    if (mydb != null) mydb.Close();
                }
            }
        }
        public void SetOdbc(Odbc mydb)
        {
            try
            {
                this.odbc = mydb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region IGlTranHost Functions
        public int AddGlTran(Hashtable items, bool isRetLastId)
        {
            Odbc mydb = null;
            bool isnull = true;
            try
            {
                #region Initial mydb Transaction
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                    this.odbc = mydb;
                    isnull = true;
                }
                else
                {
                    mydb = odbc;
                    isnull = false;
                }
                #endregion
                #region Format
                items.Add("gl_transaction_ref_type_id", "2");
                if (items.ContainsKey("gl_transaction_type_id"))
                    items["gl_transaction_type_id"] = GL_TRAN_TYPE_ID;
                else
                    items.Add("gl_transaction_type_id", GL_TRAN_TYPE_ID);

                if (items.ContainsKey("gl_transaction_ref"))
                    items["gl_transaction_ref"] = DBSafeUtils.StrToQuoteSQL(this.cinvoice_id.ToString());
                else
                    items.Add("gl_transaction_ref", DBSafeUtils.StrToQuoteSQL(this.cinvoice_id.ToString()));

                if (items.ContainsKey("gl_transaction_bodycorp_id"))
                    items["gl_transaction_bodycorp_id"] = this.cinvoice_bodycorp_id.ToString();
                else
                    items.Add("gl_transaction_bodycorp_id", this.cinvoice_bodycorp_id.ToString());

                if (items.ContainsKey("gl_transaction_unit_id"))
                    items["gl_transaction_unit_id"] = DBSafeUtils.IntToSQL(this.cinvoice_unit_id);
                else
                    items.Add("gl_transaction_unit_id", DBSafeUtils.IntToSQL(this.cinvoice_unit_id));

                if (items.ContainsKey("gl_transaction_date"))
                    items["gl_transaction_date"] = DBSafeUtils.DateToSQL(this.cinvoice_date);
                else
                    items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cinvoice_date));

                if (items.ContainsKey("gl_transaction_net"))
                    items["gl_transaction_net"] = DBSafeUtils.DecimalToSQL(-Convert.ToDecimal(items["gl_transaction_net"]));
                if (items.ContainsKey("gl_transaction_tax"))
                    items["gl_transaction_tax"] = DBSafeUtils.DecimalToSQL(-Convert.ToDecimal(items["gl_transaction_tax"]));
                if (items.ContainsKey("gl_transaction_gross"))
                    items["gl_transaction_gross"] = DBSafeUtils.DecimalToSQL(-Convert.ToDecimal(items["gl_transaction_gross"]));
                #endregion
                mydb.ExecuteInsert("gl_transactions", items);
                string sql = "SELECT LAST_INSERT_ID()";
                object id = mydb.ExecuteScalar(sql);
                int gl_transaction_id = Convert.ToInt32(id);
                string gl_transaction_ref = DBSafeUtils.QuoteSQLToStr(items["gl_transaction_ref"].ToString());
                Hashtable cinvoice_gl_items = new Hashtable();
                cinvoice_gl_items.Add("cinvoice_gl_cinvoice_id", this.CinvoiceId);
                cinvoice_gl_items.Add("cinvoice_gl_gl_id", gl_transaction_id);
                CinvoiceGl cinvoicegl = new CinvoiceGl(constr);
                cinvoicegl.SetOdbc(mydb);
                cinvoicegl.Add(cinvoice_gl_items, mydb);
                this.UpdateTotal();
                return gl_transaction_id;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                #region Close mydb Transaction
                if (isnull)
                {
                    if (mydb != null) mydb.Close();
                    odbc = null;
                }
                #endregion
            }

        }
        public void UpdateGlTran(Hashtable items, int gl_transaction_id)
        {
            Odbc mydb = null;
            bool isnull = true;
            try
            {
                #region Initial mydb Transaction
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                    this.odbc = mydb;
                    isnull = true;
                }
                else
                {
                    mydb = odbc;
                    isnull = false;
                }
                #endregion
                #region Format
                if (items.ContainsKey("gl_transaction_type_id"))
                    items["gl_transaction_type_id"] = GL_TRAN_TYPE_ID;
                else
                    items.Add("gl_transaction_type_id", GL_TRAN_TYPE_ID);

                if (items.ContainsKey("gl_transaction_ref"))
                    items["gl_transaction_ref"] = this.cinvoice_id.ToString();
                else
                    items.Add("gl_transaction_ref", this.cinvoice_id.ToString());

                if (items.ContainsKey("gl_transaction_bodycorp_id"))
                    items["gl_transaction_bodycorp_id"] = this.cinvoice_bodycorp_id.ToString();
                else
                    items.Add("gl_transaction_bodycorp_id", this.cinvoice_bodycorp_id.ToString());

                if (items.ContainsKey("gl_transaction_unit_id"))
                    items["gl_transaction_unit_id"] = DBSafeUtils.IntToSQL(this.cinvoice_unit_id);
                else
                    items.Add("gl_transaction_unit_id", DBSafeUtils.IntToSQL(this.cinvoice_unit_id));

                if (items.ContainsKey("gl_transaction_date"))
                    items["gl_transaction_date"] = DBSafeUtils.DateToSQL(this.cinvoice_date);
                else
                    items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cinvoice_date));

                if (items.ContainsKey("gl_transaction_net"))
                {
                    decimal net = decimal.Parse(items["gl_transaction_net"].ToString());

                    items["gl_transaction_net"] = DBSafeUtils.DecimalToSQL(-net);
                }
                if (items.ContainsKey("gl_transaction_tax"))
                {
                    decimal tax = decimal.Parse(items["gl_transaction_tax"].ToString());
                    items["gl_transaction_tax"] = DBSafeUtils.DecimalToSQL(-tax);
                }
                if (items.ContainsKey("gl_transaction_gross"))
                {
                    decimal gross = decimal.Parse(items["gl_transaction_gross"].ToString());
                    items["gl_transaction_gross"] = DBSafeUtils.DecimalToSQL(-gross);
                }
                #endregion

                mydb.ExecuteUpdate("gl_transactions", items, "WHERE `gl_transaction_id`=" + gl_transaction_id);
                this.UpdateTotal();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                #region Close mydb Transaction
                if (isnull)
                {
                    if (mydb != null) mydb.Close();
                    odbc = null;
                }
                #endregion
            }
        }
        public void DeleteGlTran(int gl_transaction_id)
        {
            Odbc mydb = null;
            bool isnull = true;
            try
            {
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                    this.odbc = mydb;
                    isnull = true;
                }
                else
                {
                    mydb = odbc;
                    isnull = false;
                }
                mydb.StartTransaction();
                CinvoiceGl cinvoicegl = new CinvoiceGl(constr);
                cinvoicegl.SetOdbc(mydb);
                cinvoicegl.LoadDataByGLId(gl_transaction_id);
                cinvoicegl.Delete();

                // Update 20/05/2016
                // In case there are 2 records in cinvoice_gls with one gl_transaction_id
                string sql1 = "SELECT cinvoice_gl_gl_id AS gl_id FROM `cinvoice_gls` WHERE `cinvoice_gl_gl_id`=" + gl_transaction_id
                            + "  UNION SELECT cpayment_gl_gl_id AS gl_id FROM cpayment_gls WHERE cpayment_gl_gl_id = " + gl_transaction_id;
                OdbcDataReader dr = mydb.Reader(sql1);
                if (dr.Read() == false)
                {
                    string sql = "DELETE FROM `gl_transactions` WHERE `gl_transaction_id`=" + gl_transaction_id;
                    mydb.NonQuery(sql);
                }
                //string sql = "DELETE FROM `gl_transactions` WHERE `gl_transaction_id`=" + gl_transaction_id;
                //mydb.NonQuery(sql);
                mydb.Commit();

                this.UpdateTotal();
            }
            catch (Exception ex)
            {
                if (isnull)
                {
                    if (mydb != null) mydb.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (isnull)
                {
                    if (mydb != null) mydb.Close();
                    odbc = null;
                }
            }
        }
        public void LoadTransactions()
        {
            Odbc mydb = null;
            try
            {
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                }
                else
                {
                    mydb = odbc;
                }
                string sql = "SELECT * FROM `cinvoice_gls` LEFT JOIN `gl_transactions` ON `cinvoice_gl_gl_id`=`gl_transaction_id` "
                    + "WHERE `cinvoice_gl_cinvoice_id`=" + this.cinvoice_id + " AND `gl_transaction_type_id`=2";
                OdbcDataReader dr = mydb.Reader(sql);
                gltranList = new List<GlTransaction<Cinvoice>>();
                while (dr.Read())
                {
                    GlTransaction<Cinvoice> gltran = new GlTransaction<Cinvoice>(constr);
                    gltran.SetOdbc(mydb);
                    gltran.LoadData(Convert.ToInt32(dr["gl_transaction_id"]));
                    gltranList.Add(gltran);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (odbc == null)
                {
                    if (mydb != null) mydb.Close();
                }
            }
        }


        public DataTable GetPaidDT()
        {
            try
            {
                Odbc odbc = new Odbc(AdFunction.conn);
                DataTable dt = odbc.ReturnTable("SELECT * FROM gl_transactions INNER JOIN cinvoice_gls ON gl_transactions.gl_transaction_id = cinvoice_gls.cinvoice_gl_gl_id where cinvoice_gl_cinvoice_id=" + this.CinvoiceId + " and gl_transaction_type_id=4", "p");
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int Allocate(decimal amount, Odbc odbc)
        {
            try
            {
                Hashtable items = new Hashtable();

                items.Add("gl_transaction_type_id", "4");
                items.Add("gl_transaction_ref_type_id", "4");
                items.Add("gl_transaction_ref", this.cinvoice_id.ToString());

                Sapp.SMS.System system = new System(constr);
                system.SetOdbc(odbc);
                system.LoadData("CPAYMENTCHARTCODE");
                string chart_code = system.SystemValue;
                ChartMaster chart = new ChartMaster(constr);
                chart.SetOdbc(odbc);
                chart.LoadData(chart_code);
                items.Add("gl_transaction_chart_id", chart.ChartMasterId);

                items.Add("gl_transaction_bodycorp_id", this.CinvoiceBodycorpId.ToString());

                items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL("Allocation"));

                items.Add("gl_transaction_gross", "0");

                items.Add("gl_transaction_tax", "0");

                items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.CinvoiceDate));

                items.Add("gl_transaction_net", amount);

                odbc.ExecuteInsert("gl_transactions", items);
                string sql = "SELECT LAST_INSERT_ID()";

                object id = odbc.ExecuteScalar(sql);

                int tran_id = Convert.ToInt32(id);

                this.Allocate(tran_id, odbc);

                return tran_id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Allocate(int tran_id, Odbc odbc)
        {
            try
            {
                #region INSERT InvoiceGl
                Hashtable invoice_gl_items = new Hashtable();
                invoice_gl_items.Add("cinvoice_gl_cinvoice_id", this.cinvoice_id);
                invoice_gl_items.Add("cinvoice_gl_gl_id", tran_id);
                CinvoiceGl invoicegl = new CinvoiceGl(constr);
                invoicegl.Add(invoice_gl_items, odbc);
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UpdateAllocation()
        {
            try
            {
                this.UpdatePaid();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Get_Related_JSON()
        {
            Odbc mydb = null;
            try
            {
                Hashtable items = new Hashtable();
                ArrayList glList = new ArrayList();
                string where = "";
                string cpaywhere = "";
                if (CinvoiceGross > 0)
                {
                    where = "<0";
                    cpaywhere = ">0";
                }
                else
                {
                    where = ">0";
                    cpaywhere = "<0";
                }
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                }
                else
                {
                    mydb = odbc;
                }
                #region CNOTE
                {
                    //CNote
                    DataTable dt = mydb.ReturnTable("SELECT * FROM gl_transactions INNER JOIN cinvoice_gls ON gl_transactions.gl_transaction_id = cinvoice_gls.cinvoice_gl_gl_id INNER JOIN cinvoices ON cinvoice_gls.cinvoice_gl_cinvoice_id = cinvoices.cinvoice_id WHERE (gl_transactions.gl_transaction_type_id = 4) and cinvoice_gl_cinvoice_id=" + CinvoiceId, "t1");

                    DataTable dt2 = mydb.ReturnTable("SELECT * FROM gl_transactions INNER JOIN cinvoice_gls ON gl_transactions.gl_transaction_id = cinvoice_gls.cinvoice_gl_gl_id INNER JOIN cinvoices ON cinvoice_gls.cinvoice_gl_cinvoice_id = cinvoices.cinvoice_id WHERE (gl_transactions.gl_transaction_type_id = 4) and cinvoice_gross" + where, "t2");

                    foreach (DataRow dr in dt.Rows)
                    {
                        string id = "0";
                        string paid = "";
                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            if (dr["gl_transaction_id"].ToString().Equals(dr2["gl_transaction_id"].ToString()))
                            {
                                id = dr2["cinvoice_id"].ToString();
                                paid = dr2["gl_transaction_net"].ToString();
                            }
                        }
                        if (!id.Equals("0"))
                        {
                            Cinvoice r = new Cinvoice(AdFunction.conn);
                            r.LoadData(int.Parse(id));
                            Hashtable glitems = new Hashtable();
                            glitems.Add("gl_transaction_id", r.CinvoiceId);
                            glitems.Add("gl_transaction_type_id", "CInvoice");
                            glitems.Add("gl_transaction_ref", r.CinvoiceNum);
                            glitems.Add("gl_transaction_description", r.CinvoiceDescription);
                            glitems.Add("gl_transaction_date", r.CinvoiceDate.ToString("dd/MM/yyyy"));
                            glitems.Add("gl_transaction_duedate", DBSafeUtils.DBDateToStr(r.CinvoiceDue));
                            if (cinvoice_gross > 0)
                            {
                                glitems.Add("gl_transaction_net", -r.CinvoiceNet);
                                glitems.Add("gl_transaction_tax", -r.CinvoiceTax);
                                glitems.Add("gl_transaction_gross", -r.CinvoiceGross);
                                glitems.Add("gl_transaction_discount", "0");

                                glitems.Add("gl_transaction_due", -r.CinvoiceGross - r.CinvoicePaid);
                            }
                            if (cinvoice_gross < 0)
                            {
                                glitems.Add("gl_transaction_net", r.CinvoiceNet);
                                glitems.Add("gl_transaction_tax", r.CinvoiceTax);
                                glitems.Add("gl_transaction_gross", r.CinvoiceGross);
                                glitems.Add("gl_transaction_discount", "0");

                                glitems.Add("gl_transaction_due", r.CinvoiceGross - r.CinvoicePaid);
                            }
                            glitems.Add("gl_transaction_paid", paid);
                            glList.Add(glitems);
                        }
                    }
                }
                #endregion

                #region CPAY
                {//CPAY
                    DataTable dt = mydb.ReturnTable("SELECT * FROM gl_transactions INNER JOIN cinvoice_gls ON gl_transactions.gl_transaction_id = cinvoice_gls.cinvoice_gl_gl_id INNER JOIN cinvoices ON cinvoice_gls.cinvoice_gl_cinvoice_id = cinvoices.cinvoice_id WHERE (gl_transactions.gl_transaction_type_id = 4) and cinvoice_id =" + CinvoiceId, "t1");

                    DataTable dt2 = mydb.ReturnTable("SELECT * FROM cpayment_gls INNER JOIN gl_transactions ON cpayment_gls.cpayment_gl_gl_id = gl_transactions.gl_transaction_id INNER JOIN cpayments ON cpayment_gls.cpayment_gl_cpayment_id = cpayments.cpayment_id WHERE        (gl_transactions.gl_transaction_ref_type_id = 4)", "t2");

                    foreach (DataRow dr in dt.Rows)
                    {
                        string id = "0";
                        string paid = "";
                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            if (dr["gl_transaction_id"].ToString().Equals(dr2["gl_transaction_id"].ToString()))
                            {
                                id = dr2["cpayment_id"].ToString();
                                paid = dr2["gl_transaction_net"].ToString();
                            }
                        }
                        if (!id.Equals("0"))
                        {
                            CPayment r = new CPayment(AdFunction.conn);
                            r.LoadData(int.Parse(id));
                            decimal gross = r.Cpayment_gross;
                            decimal tax = AdFunction.Rounded(((r.Cpayment_gross / (1 + AdFunction.GSTRate)) * AdFunction.GSTRate));
                            decimal net = gross - tax;
                            Hashtable glitems = new Hashtable();
                            glitems.Add("gl_transaction_id", r.Cpayment_id);
                            glitems.Add("gl_transaction_type_id", "CPAY");
                            glitems.Add("gl_transaction_ref", r.Cpayment_reference);
                            glitems.Add("gl_transaction_description", r.Cpayment_reference);
                            glitems.Add("gl_transaction_date", r.Cpayment_date.ToString("dd/MM/yyyy"));
                            glitems.Add("gl_transaction_duedate", r.Cpayment_date.ToString("dd/MM/yyyy"));
                            if (CinvoiceGross > 0)
                            {
                                glitems.Add("gl_transaction_net", net);
                                glitems.Add("gl_transaction_tax", tax);
                                glitems.Add("gl_transaction_gross", gross);
                                glitems.Add("gl_transaction_due", (r.Cpayment_gross - r.Cpayment_allocated).ToString("f2"));
                                glitems.Add("gl_transaction_paid", paid);
                                glList.Add(glitems);
                            }
                            if (CinvoiceGross < 0)
                            {
                                glitems.Add("gl_transaction_net", -net);
                                glitems.Add("gl_transaction_tax", -tax);
                                glitems.Add("gl_transaction_gross", -gross);
                                glitems.Add("gl_transaction_due", (-r.Cpayment_gross - r.Cpayment_allocated).ToString("f2"));
                                glitems.Add("gl_transaction_paid", paid);
                                glList.Add(glitems);
                            }

                        }
                    }
                }
                #endregion

                #region Journal
                {//Journal
                    string sql = "SELECT * FROM gl_transactions INNER JOIN cinvoice_gls ON gl_transactions.gl_transaction_id = cinvoice_gls.cinvoice_gl_gl_id WHERE  gl_transactions.gl_transaction_type_id=6 and `cinvoice_gl_cinvoice_id`=" + CinvoiceId;
                    DataTable dt = mydb.ReturnTable(sql, "t1");
                    foreach (DataRow dr in dt.Rows)
                    {
                        Hashtable glitems = new Hashtable();
                        glitems.Add("gl_transaction_id", dr["gl_transaction_id"].ToString());
                        glitems.Add("gl_transaction_type_id", "Journal");
                        glitems.Add("gl_transaction_ref", dr["gl_transaction_ref"].ToString());
                        glitems.Add("gl_transaction_description", dr["gl_transaction_description"].ToString());
                        glitems.Add("gl_transaction_date", DBSafeUtils.DBDateToStr(dr["gl_transaction_date"]));
                        glitems.Add("gl_transaction_duedate", DBSafeUtils.DBDateToStr(dr["gl_transaction_date"]));
                        decimal totaltax = AdFunction.Journal_Total_GST(dr["gl_transaction_ref"].ToString());
                        decimal totalGross = AdFunction.Journal_Total_Gross(dr["gl_transaction_ref"].ToString());
                        decimal gross = decimal.Parse(dr["gl_transaction_net"].ToString());
                        decimal tax = AdFunction.Rounded(totaltax / totalGross * gross);
                        decimal net = gross - tax;
                        if (CinvoiceGross > 0)
                        {

                            glitems.Add("gl_transaction_net", -net);
                            glitems.Add("gl_transaction_tax", -tax);
                            glitems.Add("gl_transaction_gross", -gross);

                            decimal paid = AdFunction.Journal_CInv_Paid(cinvoice_id.ToString(), dr["gl_transaction_ref"].ToString());
                            decimal due = -AdFunction.JournalDue(dr["gl_transaction_id"].ToString());

                            glitems.Add("gl_transaction_due", due.ToString("0.00"));
                            glitems.Add("gl_transaction_paid", paid);
                            glList.Add(glitems);
                        }
                        if (CinvoiceGross < 0)
                        {

                            glitems.Add("gl_transaction_net", net);
                            glitems.Add("gl_transaction_tax", tax);
                            glitems.Add("gl_transaction_gross", gross);

                            decimal paid = AdFunction.Journal_CInv_Paid(cinvoice_id.ToString(), dr["gl_transaction_ref"].ToString());
                            decimal due = AdFunction.JournalDue(dr["gl_transaction_id"].ToString());

                            glitems.Add("gl_transaction_due", due.ToString("0.00"));
                            glitems.Add("gl_transaction_paid", paid);
                            glList.Add(glitems);
                        }
                    }
                }
                #endregion



                items.Add("gl_transactions", glList);
                return JSON.JsonEncode(items);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (odbc == null)
                {
                    if (mydb != null) mydb.Close();
                }
            }
        }
        public string Get_Unpaid_JSON()
        {
            Odbc mydb = null;
            try
            {
                Hashtable items = new Hashtable();
                ArrayList glList = new ArrayList();
                DataTable dt = new DataTable();
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                }
                else
                {
                    mydb = odbc;
                }
                string where = "";
                string cpaywhere = "";
                if (CinvoiceGross > 0)
                {
                    where = "<0";
                    cpaywhere = ">0";
                }
                else
                {
                    where = ">0";
                    cpaywhere = "<0";
                }

                #region CNOTE
                {//CINV
                    string sql = "SELECT * FROM `cinvoices` where cinvoice_gross " + where + " and cinvoice_creditor_id=" + CinvoiceCreditorId;

                    dt = mydb.ReturnTable(sql, "r1");

                    foreach (DataRow dr in dt.Rows)
                    {
                        Cinvoice im = new Cinvoice(AdFunction.conn);
                        im.LoadData(int.Parse(dr["cinvoice_id"].ToString()));
                        bool exsit = false;
                        DataTable dt2 = im.GetPaidDT();
                        DataTable dt3 = GetPaidDT();
                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            foreach (DataRow dr3 in dt3.Rows)
                            {
                                if (dr2["gl_transaction_id"].ToString().Equals(dr3["gl_transaction_id"].ToString()))
                                    exsit = true;
                            }
                        }
                        if (!exsit)
                        {
                            Hashtable glitems = new Hashtable();
                            glitems.Add("gl_transaction_id", dr["cinvoice_id"].ToString());
                            glitems.Add("gl_transaction_type_id", "CInvoice");
                            glitems.Add("gl_transaction_ref", dr["cinvoice_num"].ToString());
                            glitems.Add("gl_transaction_description", dr["cinvoice_description"].ToString());
                            glitems.Add("gl_transaction_date", DBSafeUtils.DBDateToStr(dr["cinvoice_date"]));
                            glitems.Add("gl_transaction_duedate", DBSafeUtils.DBDateToStr(dr["cinvoice_due"]));
                            if (CinvoiceGross > 0)
                            {
                                glitems.Add("gl_transaction_net", -decimal.Parse(dr["cinvoice_net"].ToString()));
                                glitems.Add("gl_transaction_tax", -decimal.Parse(dr["cinvoice_tax"].ToString()));
                                glitems.Add("gl_transaction_gross", -decimal.Parse(dr["cinvoice_gross"].ToString()));
                                decimal paid = -Convert.ToDecimal(dr["cinvoice_paid"]);
                                decimal gross = -Convert.ToDecimal(dr["cinvoice_gross"]);
                                decimal due = gross - paid;
                                glitems.Add("gl_transaction_due", due.ToString("0.00"));
                                if (due != 0)
                                    glList.Add(glitems);
                            }
                            else
                            {
                                glitems.Add("gl_transaction_net", decimal.Parse(dr["cinvoice_net"].ToString()));
                                glitems.Add("gl_transaction_tax", decimal.Parse(dr["cinvoice_tax"].ToString()));
                                glitems.Add("gl_transaction_gross", decimal.Parse(dr["cinvoice_gross"].ToString()));
                                decimal paid = Convert.ToDecimal(dr["cinvoice_paid"]);
                                decimal gross = Convert.ToDecimal(dr["cinvoice_gross"]);
                                decimal due = gross - paid;
                                glitems.Add("gl_transaction_due", due.ToString("0.00"));
                                if (due != 0)
                                    glList.Add(glitems);
                            }

                        }
                    }


                }
                #endregion


                #region CPAY
                {
                    string sql = "SELECT * FROM `cpayments` where cpayment_gross" + cpaywhere + " and cpayment_creditor_id=" + CinvoiceCreditorId;

                    dt = mydb.ReturnTable(sql, "r1");
                    foreach (DataRow dr in dt.Rows)
                    {
                        CPayment r = new CPayment(AdFunction.conn);
                        r.LoadData(int.Parse(dr["cpayment_id"].ToString()));
                        bool exsit = false;
                        DataTable dt2 = r.GetPaidDT();
                        DataTable dt3 = GetPaidDT();
                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            foreach (DataRow dr3 in dt3.Rows)
                            {
                                if (dr2["gl_transaction_id"].ToString().Equals(dr3["gl_transaction_id"].ToString()))
                                    exsit = true;
                            }
                        }
                        if (!exsit)
                        {

                            Hashtable glitems = new Hashtable();
                            glitems.Add("gl_transaction_id", r.Cpayment_id);
                            glitems.Add("gl_transaction_type_id", "CPAY");
                            glitems.Add("gl_transaction_ref", r.Cpayment_reference);
                            glitems.Add("gl_transaction_description", r.Cpayment_reference);
                            glitems.Add("gl_transaction_date", r.Cpayment_date.ToString("dd/MM/yyyy"));
                            glitems.Add("gl_transaction_duedate", r.Cpayment_date.ToString("dd/MM/yyyy"));
                            decimal gross = r.Cpayment_gross;
                            decimal tax = AdFunction.Rounded(((r.Cpayment_gross / (1 + AdFunction.GSTRate)) * AdFunction.GSTRate));
                            decimal net = gross - tax;
                            if (CinvoiceGross > 0)
                            {
                                if (r.Cpayment_gross - r.Cpayment_allocated != 0)
                                {
                                    glitems.Add("gl_transaction_net", net);
                                    glitems.Add("gl_transaction_tax", tax);
                                    glitems.Add("gl_transaction_gross", gross);
                                    glitems.Add("gl_transaction_discount", "0.00");
                                    glitems.Add("gl_transaction_due", (r.Cpayment_gross - r.Cpayment_allocated).ToString("f2"));
                                    glitems.Add("gl_transaction_paid", "0.00");
                                    glList.Add(glitems);
                                }
                            }
                            if (CinvoiceGross < 0)
                            {
                                if (-r.Cpayment_gross - r.Cpayment_allocated != 0)
                                {
                                    glitems.Add("gl_transaction_net", -net);
                                    glitems.Add("gl_transaction_tax", -tax);
                                    glitems.Add("gl_transaction_gross", -gross);
                                    glitems.Add("gl_transaction_discount", "0.00");
                                    glitems.Add("gl_transaction_due", (-r.Cpayment_gross - r.Cpayment_allocated).ToString("f2"));
                                    glitems.Add("gl_transaction_paid", "0.00");
                                    glList.Add(glitems);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Journal
                {//Journal
                    string did = AdFunction.GENERALCREDITOR_ChartID;
                    string sql = "SELECT * FROM `gl_transactions` where gl_transaction_id not in " +
                        "(select cinvoice_gl_gl_id from cinvoice_gls where cinvoice_gl_cinvoice_id =" + cinvoice_id + ")" +
                        " and gl_transaction_type_id=6 and gl_transaction_net " + where + " and gl_transaction_chart_id=" + did +
                        " and gl_transaction_creditor_id=" + cinvoice_creditor_id;//modified by dyyr @ 2016 07 28

                    dt = mydb.ReturnTable(sql, "r2");
                    foreach (DataRow dr in dt.Rows)
                    {
                        Hashtable glitems = new Hashtable();
                        glitems.Add("gl_transaction_id", dr["gl_transaction_id"].ToString());
                        glitems.Add("gl_transaction_type_id", "Journal");
                        glitems.Add("gl_transaction_ref", dr["gl_transaction_ref"].ToString());
                        glitems.Add("gl_transaction_description", dr["gl_transaction_description"].ToString());
                        glitems.Add("gl_transaction_date", DBSafeUtils.DBDateToStr(dr["gl_transaction_date"]));
                        glitems.Add("gl_transaction_duedate", DBSafeUtils.DBDateToStr(dr["gl_transaction_date"]));
                        decimal totaltax = AdFunction.Journal_Total_GST(dr["gl_transaction_ref"].ToString());
                        decimal totalGross = AdFunction.Journal_Total_Gross(dr["gl_transaction_ref"].ToString());
                        //decimal totalnet = totalGross - totaltax;
                        decimal gross = decimal.Parse(dr["gl_transaction_net"].ToString());
                        decimal tax = AdFunction.Rounded(totaltax / totalGross * gross);
                        decimal net = gross - tax;
                        if (CinvoiceGross > 0)
                        {
                            glitems.Add("gl_transaction_net", -net);
                            glitems.Add("gl_transaction_tax", -tax);
                            glitems.Add("gl_transaction_gross", -gross);
                            decimal due = -AdFunction.JournalDue(dr["gl_transaction_id"].ToString());
                            if (due != 0)
                            {
                                glitems.Add("gl_transaction_due", due);
                                glList.Add(glitems);
                            }
                        }
                        if (CinvoiceGross < 0)
                        {
                            glitems.Add("gl_transaction_net", net);
                            glitems.Add("gl_transaction_tax", tax);
                            glitems.Add("gl_transaction_gross", gross);
                            decimal due = AdFunction.JournalDue(dr["gl_transaction_id"].ToString());
                            if (due != 0)
                            {
                                glitems.Add("gl_transaction_due", due);
                                glList.Add(glitems);
                            }
                        }
                    }
                }
                #endregion


                items.Add("gl_transactions", glList);
                return JSON.JsonEncode(items);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (odbc == null)
                {
                    if (mydb != null) mydb.Close();
                }
            }
        }
        #endregion
    } 
}

