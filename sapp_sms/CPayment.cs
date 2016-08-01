//070713
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using Sapp.Data;
using Sapp.Common;


namespace Sapp.SMS
{
    public class CPayment : IGlTranHost
    {
        #region Consts
        public const int GL_TRAN_TYPE_ID = 4;
        #endregion
        #region Variables
        private string constr;
        private int cpayment_id;
        private int cpayment_bodycorp_id;
        private int cpayment_creditor_id;
        private string cpayment_reference;
        private int cpayment_type_id;
        private decimal cpayment_gross;
        private DateTime cpayment_date;
        private decimal cpayment_allocated;
        private int cpayment_ctype_id;
        private bool cpayment_reconciled;
        private List<GlTransaction<CPayment>> gltranList;
        private Odbc odbc = null;
        #endregion
        public CPayment(string constr)
        {
            try
            {
                this.constr = constr;
                gltranList = new List<GlTransaction<CPayment>>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region Properties

        public int Cpayment_id
        {
            get { return cpayment_id; }
            set { cpayment_id = value; }
        }

        public int Cpayment_bodycorp_id
        {
            get { return cpayment_bodycorp_id; }
            set { cpayment_bodycorp_id = value; }
        }

        public int Cpayment_creditor_id
        {
            get { return cpayment_creditor_id; }
            set { cpayment_creditor_id = value; }
        }
        public int Cpayment_ctype_id
        {
            get { return cpayment_ctype_id; }
            set { cpayment_ctype_id = value; }
        }

        public string Cpayment_reference
        {
            get { return cpayment_reference; }
            set { cpayment_reference = value; }
        }

        public int Cpayment_type_id
        {
            get { return cpayment_type_id; }
            set { cpayment_type_id = value; }
        }

        public decimal Cpayment_gross
        {
            get { return cpayment_gross; }
            set { cpayment_gross = value; }
        }

        public DateTime Cpayment_date
        {
            get { return cpayment_date; }
            set { cpayment_date = value; }
        }


        public decimal Cpayment_allocated
        {
            get { return cpayment_allocated; }
            set { cpayment_allocated = value; }
        }

        public bool CpaymentReconciled
        {
            get { return cpayment_reconciled; }
            set { cpayment_reconciled = value; }
        }

        public List<GlTransaction<CPayment>> GltransactionList
        {
            get { return gltranList; }
            set { gltranList = value; }
        }
        #endregion
        #region Standard Functions
        public void LoadData(int cpayment_id)
        {
            Odbc mydb = null;
            try
            {
                this.cpayment_id = cpayment_id;
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                }
                else
                {
                    mydb = odbc;
                }
                string sql = "SELECT * FROM `cpayments` WHERE `cpayment_id`=" + cpayment_id;
                OdbcDataReader dr = mydb.Reader(sql);
                if (dr.Read())
                {
                    #region Initial Properties
                    cpayment_bodycorp_id = Convert.ToInt32(dr["cpayment_bodycorp_id"]);
                    cpayment_creditor_id = Convert.ToInt32(dr["cpayment_creditor_id"]);
                    cpayment_reference = dr["cpayment_reference"].ToString();
                    cpayment_type_id = Convert.ToInt32(dr["cpayment_type_id"]);
                    cpayment_gross = Convert.ToDecimal(dr["cpayment_gross"]);
                    cpayment_date = Convert.ToDateTime(dr["cpayment_date"]);
                    cpayment_allocated = Convert.ToDecimal(dr["cpayment_allocated"]);
                    cpayment_reconciled = DBSafeUtils.DBBoolToBool(dr["cpayment_reconciled"]);
                    cpayment_ctype_id = Convert.ToInt32(dr["cpayment_ctype_id"]);
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
                items.Add("cpayment_bodycorp_id", cpayment_bodycorp_id);
                items.Add("cpayment_creditor_id", cpayment_creditor_id);
                items.Add("cpayment_reference", cpayment_reference);
                items.Add("cpayment_type_id", cpayment_type_id);
                items.Add("cpayment_gross", cpayment_gross);
                items.Add("cpayment_date", cpayment_date);
                items.Add("cpayment_ctype_id", Cpayment_ctype_id);
                items.Add("cpayment_allocated", cpayment_allocated);
                items.Add("cpayment_reconciled", cpayment_reconciled);
                #endregion
                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Update(Hashtable items, int cpayment_id, string batch = "", string start = "", string end = "")
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
                if (!batch.Equals(""))
                    items.Add("cpayment_recbatchid", batch);

                mydb.ExecuteUpdate("cpayments", items, "WHERE `cpayment_id`=" + cpayment_id);
                this.LoadData(cpayment_id);
                int GENERALCREDITOR = 0;
                int CHARTOFACCOUNT = 0;
                #region Load General Creditor Control and Chart of Account
                ChartMaster chart = new ChartMaster(constr);
                Sapp.SMS.System system = new System(constr);
                system.LoadData("GENERALCREDITOR");
                chart.LoadData(system.SystemValue);
                GENERALCREDITOR = chart.ChartMasterId;
                string sql = "SELECT * FROM `bodycorps` WHERE `bodycorp_id`=" + this.cpayment_bodycorp_id;
                OdbcDataReader dr = mydb.Reader(sql);
                if (dr.Read())
                {
                    CHARTOFACCOUNT = Convert.ToInt32(dr["bodycorp_account_id"]);
                }
                if (CHARTOFACCOUNT == 0) throw new Exception("Error: account is missing!");
                #endregion
                #region Update Balance Sheet
                sql = "SELECT `gl_transaction_id` FROM `cpayment_gls` LEFT JOIN `gl_transactions` ON `cpayment_gl_gl_id`=`gl_transaction_id` "
                    + "WHERE `cpayment_gl_cpayment_id`=" + this.cpayment_id + " AND `gl_transaction_type_id`=5 AND `gl_transaction_chart_id`=" + GENERALCREDITOR;
                dr = mydb.Reader(sql);
                while (dr.Read())
                {
                    Hashtable gl_items = new Hashtable();
                    gl_items.Add("gl_transaction_type_id", 5);
                    gl_items.Add("gl_transaction_ref", this.cpayment_id.ToString());
                    gl_items.Add("gl_transaction_ref_type_id", "4");
                    gl_items.Add("gl_transaction_chart_id", GENERALCREDITOR);
                    gl_items.Add("gl_transaction_bodycorp_id", this.cpayment_bodycorp_id);
                    gl_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(cpayment_reference));
                    gl_items.Add("gl_transaction_net", (-cpayment_gross).ToString("0.00"));
                    gl_items.Add("gl_transaction_tax", 0);
                    gl_items.Add("gl_transaction_gross", 0);
                    if (!batch.Equals(""))
                    {
                        gl_items.Add("gl_transaction_rec", 1);
                        gl_items.Add("gl_transaction_recbatchid", batch);
                        if (!end.Equals(""))
                            items.Add("gl_transaction_reccutoff", end);
                    }
                    gl_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cpayment_date));
                    mydb.ExecuteUpdate("gl_transactions", gl_items, "WHERE `gl_transaction_id`=" + Convert.ToInt32(dr["gl_transaction_id"]));
                }
                sql = "SELECT `gl_transaction_id` FROM `cpayment_gls` LEFT JOIN `gl_transactions` ON `cpayment_gl_gl_id`=`gl_transaction_id` "
                    + "WHERE `cpayment_gl_cpayment_id`=" + this.cpayment_id + " AND `gl_transaction_type_id`=5 AND `gl_transaction_chart_id`=" + CHARTOFACCOUNT;
                dr = mydb.Reader(sql);
                while (dr.Read())
                {
                    Hashtable gl_items = new Hashtable();
                    gl_items.Add("gl_transaction_type_id", 5);
                    gl_items.Add("gl_transaction_ref", this.cpayment_id.ToString());
                    gl_items.Add("gl_transaction_ref_type_id", "4");
                    gl_items.Add("gl_transaction_chart_id", CHARTOFACCOUNT);
                    gl_items.Add("gl_transaction_bodycorp_id", this.cpayment_bodycorp_id);
                    gl_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(cpayment_reference));
                    gl_items.Add("gl_transaction_net", (cpayment_gross).ToString("0.00"));
                    gl_items.Add("gl_transaction_tax", 0);
                    gl_items.Add("gl_transaction_gross", 0);
                    if (!batch.Equals(""))
                    {
                        gl_items.Add("gl_transaction_rec", 1);
                        gl_items.Add("gl_transaction_recbatchid", batch);
                    }
                    //if (!start.Equals(""))
                    //{
                    //    gl_items.Add("gl_transaction_recstart", "'" + start + "'");
                    //}
                    if (!end.Equals(""))
                    {
                        gl_items.Add("gl_transaction_reccutoff", "'" + end + "'");

                    }
                    gl_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cpayment_date));
                    mydb.ExecuteUpdate("gl_transactions", gl_items, "WHERE `gl_transaction_id`=" + Convert.ToInt32(dr["gl_transaction_id"]));
                }
                #endregion
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
                    odbc = null;
                    if (mydb != null) mydb.Close();
                }
            }
        }
        public void Update(Hashtable items, string batch = "", string start = "", string end = "")
        {
            try
            {
                this.Update(items, this.cpayment_id, batch, start, end);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Add(Hashtable items)
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
                mydb.ExecuteInsert("cpayments", items);
                string sql = "SELECT LAST_INSERT_ID()";
                object id = mydb.ExecuteScalar(sql);
                this.cpayment_id = Convert.ToInt32(id);
                this.LoadData(this.cpayment_id);
                int CHARTOFACCOUNT = 0;
                Bodycorp bodycorp = new Bodycorp(constr);
                bodycorp.LoadData(this.cpayment_bodycorp_id);
                CHARTOFACCOUNT = bodycorp.BodycorpAccountId;
                #region Add Balance Sheet
                int GENERALCREDITOR = 0;
                #region Load General Creditor Control and General Tax Chart Account
                ChartMaster chart = new ChartMaster(constr);
                Sapp.SMS.System system = new System(constr);
                system.LoadData("GENERALCREDITOR");
                chart.LoadData(system.SystemValue);
                GENERALCREDITOR = chart.ChartMasterId;
                #endregion
                Hashtable gl_items = new Hashtable();
                int gl_id = 0;
                #region INSERT General Creditor Control
                gl_items.Add("gl_transaction_type_id", 5);
                gl_items.Add("gl_transaction_ref", this.cpayment_id.ToString());
                gl_items.Add("gl_transaction_ref_type_id", "4");
                gl_items.Add("gl_transaction_chart_id", GENERALCREDITOR);
                gl_items.Add("gl_transaction_bodycorp_id", this.cpayment_bodycorp_id);
                gl_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(cpayment_reference));
                gl_items.Add("gl_transaction_net", (-cpayment_gross).ToString("0.00"));
                gl_items.Add("gl_transaction_tax", 0);
                gl_items.Add("gl_transaction_gross", 0);
                gl_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cpayment_date));
                mydb.ExecuteInsert("gl_transactions", gl_items);
                sql = "SELECT LAST_INSERT_ID()";
                gl_id = Convert.ToInt32(mydb.ExecuteScalar(sql));
                Hashtable cpayment_gl_items = new Hashtable();
                cpayment_gl_items.Add("cpayment_gl_cpayment_id", this.cpayment_id);
                cpayment_gl_items.Add("cpayment_gl_gl_id", gl_id);
                CpaymentGl cpaymentgl = new CpaymentGl(constr);
                cpaymentgl.Add(cpayment_gl_items, mydb);
                #endregion
                #region INSERT Chart of Account
                gl_items = new Hashtable();
                gl_items.Add("gl_transaction_type_id", 5);
                gl_items.Add("gl_transaction_ref", this.cpayment_id.ToString());
                gl_items.Add("gl_transaction_ref_type_id", "4");
                gl_items.Add("gl_transaction_chart_id", CHARTOFACCOUNT);
                gl_items.Add("gl_transaction_bodycorp_id", this.cpayment_bodycorp_id);
                gl_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(cpayment_reference));
                gl_items.Add("gl_transaction_net", (cpayment_gross).ToString("0.00"));
                gl_items.Add("gl_transaction_tax", 0);
                gl_items.Add("gl_transaction_gross", 0);
                gl_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cpayment_date));
                mydb.ExecuteInsert("gl_transactions", gl_items);
                sql = "SELECT LAST_INSERT_ID()";
                gl_id = Convert.ToInt32(mydb.ExecuteScalar(sql));
                cpayment_gl_items = new Hashtable();
                cpayment_gl_items.Add("cpayment_gl_cpayment_id", this.cpayment_id);
                cpayment_gl_items.Add("cpayment_gl_gl_id", gl_id);
                cpaymentgl = new CpaymentGl(constr);
                cpaymentgl.Add(cpayment_gl_items, mydb);
                #endregion
                #endregion
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
                    odbc = null;
                    if (mydb != null) mydb.Close();
                }
            }
        }
        public void Delete(int cpayment_id)
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
                #region  Check Rec

                string cksql = "Select * FROM `cpayments` WHERE `cpayment_id`=" + cpayment_id;

                DataTable dt = mydb.ReturnTable(cksql, "check");
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["cpayment_reconciled"].ToString().Equals("1"))
                    {
                        throw new Exception("Reconciled Payment");
                    }
                }


                #endregion
                #region Delete All Related Trans
                string sql = "SELECT * FROM `cpayment_gls` LEFT JOIN `gl_transactions` ON `cpayment_gl_gl_id`=`gl_transaction_id` WHERE `cpayment_gl_cpayment_id`=" + cpayment_id;
                OdbcDataReader dr = mydb.Reader(sql);
                while (dr.Read())
                {
                    CpaymentGl cpaymentgl = new CpaymentGl(constr);
                    cpaymentgl.LoadDataByGLId(Convert.ToInt32(dr["gl_transaction_id"]));
                    cpaymentgl.Delete(mydb);
                    if (Convert.ToInt32(dr["gl_transaction_type_id"]) == 4)
                    {
                        CinvoiceGl cinvoicegl = new CinvoiceGl(constr);
                        cinvoicegl.SetOdbc(mydb);
                        cinvoicegl.LoadDataByGLId(Convert.ToInt32(dr["gl_transaction_id"]));
                        cinvoicegl.Delete();
                        Cinvoice cinvoice = new Cinvoice(constr);
                        cinvoice.SetOdbc(mydb);
                        cinvoice.LoadData(cinvoicegl.Cinvoice_gl_cinvoice_id);
                        cinvoice.UpdatePaid();
                    }
                    GlTransaction<CPayment> tran = new GlTransaction<CPayment>(constr);
                    tran.Drop(Convert.ToInt32(dr["gl_transaction_id"]), mydb);
                }
                #endregion
                sql = "DELETE FROM `cpayments` WHERE `cpayment_id`=" + cpayment_id;
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
                    odbc = null;
                    if (mydb != null) mydb.Close();
                }
            }
        }
        public void Delete()
        {
            try
            {
                this.Delete(this.cpayment_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Special Function
        public string GetGLTranJSON()
        {
            try
            {
                Hashtable items = new Hashtable();
                ArrayList glList = new ArrayList();
                foreach (GlTransaction<CPayment> gl_tran in gltranList)
                {
                    Hashtable glitems = new Hashtable();
                    glitems.Add("gl_transaction_id", gl_tran.GlTransactionId.ToString());
                    glitems.Add("gl_transaction_ref_type_id", "4");
                    glitems.Add("gl_transaction_type_id", gl_tran.GLTransactionTypeId.ToString());
                    glitems.Add("gl_transaction_ref", gl_tran.GLTransactionRef);
                    glitems.Add("gl_transaction_chart_id", gl_tran.GlTransactionChartId.ToString());
                    glitems.Add("gl_transaction_description", gl_tran.GlTransactionDescription);
                    glitems.Add("gl_transaction_net", gl_tran.GlTransactionNet.ToString());
                    glitems.Add("gl_transaction_tax", gl_tran.GlTransactionTax.ToString());
                    glitems.Add("gl_transaction_gross", gl_tran.GlTransactionGross.ToString());
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



        public string Get_CINV_Paid()
        {
            Odbc mydb = null;
            try
            {
                Hashtable items = new Hashtable();
                ArrayList glList = new ArrayList();
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                }
                else
                {
                    mydb = odbc;
                }

                string where = "";
                string CINVwhere = "";
                if (Cpayment_gross > 0)
                {
                    where = "<0";
                    CINVwhere = ">0";
                }
                else
                {
                    where = ">0";
                    CINVwhere = "<0";
                }

                #region CINV
                {
                    string sql = "SELECT * FROM (((`cpayment_gls` LEFT JOIN `gl_transactions` ON `cpayment_gl_gl_id`=`gl_transaction_id`) "
                        + "LEFT JOIN `cinvoice_gls` ON `cpayment_gl_gl_id`=`cinvoice_gl_gl_id`) RIGHT JOIN `cinvoices` ON `cinvoice_gl_cinvoice_id`=`cinvoice_id`) "
                        + "WHERE `cpayment_gl_cpayment_id`=" + this.cpayment_id;
                    OdbcDataReader dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        Hashtable glitems = new Hashtable();
                        glitems.Add("gl_transaction_id", dr["cinvoice_id"].ToString());
                        glitems.Add("gl_transaction_type_id", "CINV");
                        glitems.Add("gl_transaction_ref", dr["cinvoice_num"].ToString());
                        glitems.Add("gl_transaction_description", dr["cinvoice_description"].ToString());
                        glitems.Add("gl_transaction_date", DBSafeUtils.DBDateToStr(dr["cinvoice_date"]));
                        glitems.Add("gl_transaction_duedate", DBSafeUtils.DBDateToStr(dr["cinvoice_due"]));
                        glitems.Add("gl_transaction_net", dr["cinvoice_net"].ToString());
                        glitems.Add("gl_transaction_tax", dr["cinvoice_tax"].ToString());
                        glitems.Add("gl_transaction_gross", dr["cinvoice_gross"].ToString());
                        decimal paid = Convert.ToDecimal(dr["cinvoice_paid"]);
                        decimal gross = Convert.ToDecimal(dr["cinvoice_gross"]);
                        decimal due = gross - paid;
                        glitems.Add("gl_transaction_due", due.ToString("0.00"));
                        glitems.Add("gl_transaction_paid", Convert.ToDecimal(dr["gl_transaction_net"]).ToString("0.00"));
                        glList.Add(glitems);
                    }
                }
                #endregion
                {//Journal
                    string sql = "SELECT * FROM gl_transactions INNER JOIN cpayment_gls ON gl_transactions.gl_transaction_id = cpayment_gls.cpayment_gl_gl_id WHERE  gl_transactions.gl_transaction_type_id=6 and gl_transaction_description <>'Discount Offered' and  `cpayment_gl_cpayment_id`=" + this.Cpayment_id;
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
                        //decimal totalnet = totalGross - totaltax;

                        decimal gross = -decimal.Parse(dr["gl_transaction_net"].ToString());
                        decimal tax = AdFunction.Rounded(totaltax / totalGross * gross);
                        decimal net = gross - tax;


                        glitems.Add("gl_transaction_net", net);
                        glitems.Add("gl_transaction_tax", tax);
                        glitems.Add("gl_transaction_gross", gross);

                        decimal paid = AdFunction.JournalCPayPaid(Cpayment_id.ToString(), dr["gl_transaction_ref"].ToString());
                        decimal due = -AdFunction.JournalDue(dr["gl_transaction_id"].ToString());

                        glitems.Add("gl_transaction_due", due.ToString("0.00"));
                        glitems.Add("gl_transaction_paid", paid);
                        glList.Add(glitems);
                    }
                }
                { //CNote
                    //cpayments
                    //    cpayment_gls
                    //    cpayment_gl_cpayment_id
                    //        cpayment_gl_gl_id
                    string sql = "SELECT * FROM cpayment_gls INNER JOIN gl_transactions ON cpayment_gls.cpayment_gl_gl_id = gl_transactions.gl_transaction_id INNER JOIN cpayments ON cpayment_gls.cpayment_gl_cpayment_id = cpayments.cpayment_id WHERE        (gl_transactions.gl_transaction_ref_type_id = 4) and `cpayment_gl_cpayment_id`=" + this.Cpayment_id;
                    DataTable dt = mydb.ReturnTable(sql, "t1");
                    sql = "SELECT * FROM cpayment_gls INNER JOIN gl_transactions ON cpayment_gls.cpayment_gl_gl_id = gl_transactions.gl_transaction_id INNER JOIN cpayments ON cpayment_gls.cpayment_gl_cpayment_id = cpayments.cpayment_id WHERE        (gl_transactions.gl_transaction_ref_type_id = 4) and cpayment_gross" + where;
                    DataTable dt2 = mydb.ReturnTable(sql, "t2");

                    foreach (DataRow dr in dt.Rows)
                    {
                        string RefundID = "0";
                        string paid = "";
                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            if (dr["gl_transaction_id"].ToString().Equals(dr2["gl_transaction_id"].ToString()))
                            {
                                RefundID = dr2["cpayment_id"].ToString();
                                paid = dr2["gl_transaction_net"].ToString();
                            }
                        }
                        if (!RefundID.Equals("0"))
                        {
                            CPayment r = new CPayment(AdFunction.conn);
                            r.LoadData(int.Parse(RefundID));

                            Hashtable glitems = new Hashtable();
                            glitems.Add("gl_transaction_id", r.Cpayment_id);
                            glitems.Add("gl_transaction_type_id", "CNote");
                            glitems.Add("gl_transaction_ref", r.Cpayment_reference);
                            glitems.Add("gl_transaction_description", r.Cpayment_reference);
                            glitems.Add("gl_transaction_date", r.Cpayment_date.ToString("dd/MM/yyyy"));
                            glitems.Add("gl_transaction_duedate", r.Cpayment_date.ToString("dd/MM/yyyy"));
                            if (Cpayment_gross > 0)
                            {
                                decimal gross = -r.Cpayment_gross;
                                decimal net = AdFunction.Rounded(gross / (1 + AdFunction.GSTRate));
                                decimal tax = gross - net;
                                glitems.Add("gl_transaction_net", net);
                                glitems.Add("gl_transaction_tax", tax);
                                glitems.Add("gl_transaction_gross", gross);
                                glitems.Add("gl_transaction_discount", "0");

                                glitems.Add("gl_transaction_due", -r.Cpayment_gross - r.Cpayment_allocated);
                                glitems.Add("gl_transaction_paid", paid);
                            }
                            else
                            {
                                decimal gross = r.Cpayment_gross;

                                decimal net = AdFunction.Rounded(gross / (1 + AdFunction.GSTRate));
                                decimal tax = gross - net;
                                glitems.Add("gl_transaction_net", net);
                                glitems.Add("gl_transaction_tax", tax);
                                glitems.Add("gl_transaction_gross", gross);
                                glitems.Add("gl_transaction_discount", "0");

                                glitems.Add("gl_transaction_due", r.Cpayment_gross - r.Cpayment_allocated);
                                glitems.Add("gl_transaction_paid", paid);
                            }
                            glList.Add(glitems);
                        }
                    }
                }
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
        public string Get_CINV_UnPaid()
        {
            Odbc mydb = null;
            try
            {
                Hashtable items = new Hashtable();
                ArrayList glList = new ArrayList();
                if (odbc == null)
                {
                    mydb = new Odbc(constr);
                }
                else
                {
                    mydb = odbc;
                }
                string where = "";
                string CINVwhere = "";
                if (Cpayment_gross > 0)
                {
                    where = "<0";
                    CINVwhere = ">0";
                }
                else
                {
                    where = ">0";
                    CINVwhere = "<0";
                }

                #region CINV
                {
                    string sql = "SELECT * FROM `cinvoices` "
                        + "WHERE (`cinvoice_id` NOT IN (SELECT `cinvoice_gl_cinvoice_id` FROM (`cinvoice_gls` LEFT JOIN `cpayment_gls` ON `cinvoice_gl_gl_id`=`cpayment_gl_gl_id`) "
                        + "WHERE `cpayment_gl_cpayment_id`=" + this.cpayment_id + ")) AND (`cinvoice_gross` " + CINVwhere + ") and (`cinvoice_gross` > `cinvoice_paid`) AND (`cinvoice_creditor_id`=" + this.cpayment_creditor_id + ") "
                        + "AND (`cinvoice_bodycorp_id`=" + this.cpayment_bodycorp_id + ")";
                    OdbcDataReader dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        Hashtable glitems = new Hashtable();
                        glitems.Add("gl_transaction_id", dr["cinvoice_id"].ToString());
                        glitems.Add("gl_transaction_ref", dr["cinvoice_num"].ToString());
                        glitems.Add("gl_transaction_type_id", "CINV");
                        glitems.Add("gl_transaction_description", dr["cinvoice_description"].ToString());
                        glitems.Add("gl_transaction_date", DBSafeUtils.DBDateToStr(dr["cinvoice_date"]));
                        glitems.Add("gl_transaction_duedate", DBSafeUtils.DBDateToStr(dr["cinvoice_due"]));
                        if (Cpayment_gross > 0)
                        {
                            glitems.Add("gl_transaction_net", dr["cinvoice_net"].ToString());
                            glitems.Add("gl_transaction_tax", dr["cinvoice_tax"].ToString());
                            glitems.Add("gl_transaction_gross", dr["cinvoice_gross"].ToString());
                            decimal paid = Convert.ToDecimal(dr["cinvoice_paid"]);
                            decimal gross = Convert.ToDecimal(dr["cinvoice_gross"]);
                            decimal due = gross - paid;
                            glitems.Add("gl_transaction_due", due.ToString("0.00"));
                            glList.Add(glitems);
                        }
                        else if (Cpayment_gross < 0)
                        {
                            glitems.Add("gl_transaction_net", -decimal.Parse(dr["cinvoice_net"].ToString()));
                            glitems.Add("gl_transaction_tax", -decimal.Parse(dr["cinvoice_tax"].ToString()));
                            glitems.Add("gl_transaction_gross", -decimal.Parse(dr["cinvoice_gross"].ToString()));
                            decimal paid = Convert.ToDecimal(dr["cinvoice_paid"]);
                            decimal gross = Convert.ToDecimal(dr["cinvoice_gross"]);
                            decimal due = gross - paid;
                            glitems.Add("gl_transaction_due", due.ToString("0.00"));
                            glList.Add(glitems);
                        }

                    }
                }
                #endregion
                #region JNL
                {//Journal
                    string cid = AdFunction.GENERALCREDITOR_ChartID;

                    // Update 27/04/2016
                    //string sql = "SELECT * FROM `gl_transactions` where gl_transaction_id not in (select cpayment_gl_gl_id from cpayment_gls where cpayment_gl_cpayment_id =" + Cpayment_id + ") and  gl_transaction_type_id=6 and gl_transaction_net " + where + " and gl_transaction_chart_id=" + cid;
                    string sql = "SELECT * FROM `gl_transactions` where gl_transaction_bodycorp_id = " + this.cpayment_bodycorp_id + 
                                 " AND gl_transaction_id not in "+
                                 "(select cpayment_gl_gl_id from cpayment_gls where cpayment_gl_cpayment_id =" + Cpayment_id + ")"+
                                 " and gl_transaction_type_id=6 and gl_transaction_net " + where + " and gl_transaction_chart_id=" + cid +
                                 " and gl_transaction_creditor_id=" + cpayment_creditor_id;
                    DataTable dt = mydb.ReturnTable(sql, "r2");
                    foreach (DataRow dr in dt.Rows)
                    {
                        Hashtable glitems = new Hashtable();
                        glitems.Add("gl_transaction_id", dr["gl_transaction_id"].ToString());
                        glitems.Add("gl_transaction_type_id", "Journal");
                        glitems.Add("gl_transaction_ref", dr["gl_transaction_ref"].ToString());
                        glitems.Add("gl_transaction_description", dr["gl_transaction_description"].ToString());
                        glitems.Add("gl_transaction_date", DBSafeUtils.DBDateToStr(dr["gl_transaction_date"]));
                        glitems.Add("gl_transaction_duedate", DBSafeUtils.DBDateToStr(dr["gl_transaction_date"]));
                        decimal due = 0;
                        if (Cpayment_gross > 0)
                        {
                            glitems.Add("gl_transaction_net", -decimal.Parse(dr["gl_transaction_net"].ToString()));
                            glitems.Add("gl_transaction_tax", -decimal.Parse(dr["gl_transaction_tax"].ToString()));
                            glitems.Add("gl_transaction_gross", -decimal.Parse(dr["gl_transaction_net"].ToString()));
                            due = -AdFunction.JournalDue(dr["gl_transaction_id"].ToString());
                        }
                        else if (Cpayment_gross < 0)
                        {
                            glitems.Add("gl_transaction_net", decimal.Parse(dr["gl_transaction_net"].ToString()));
                            glitems.Add("gl_transaction_tax", decimal.Parse(dr["gl_transaction_tax"].ToString()));
                            glitems.Add("gl_transaction_gross", decimal.Parse(dr["gl_transaction_net"].ToString()));
                            due = AdFunction.JournalDue(dr["gl_transaction_id"].ToString());
                        }

                        if (due != 0)
                        {
                            glitems.Add("gl_transaction_due", due);
                            glList.Add(glitems);
                        }
                    }
                }
                {//CNOTE

                    string sql = "SELECT * from cpayments where cpayment_gross " + where + " and cpayment_creditor_id=" + Cpayment_creditor_id;
                    DataTable dt = mydb.ReturnTable(sql, "t2");

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
                            glitems.Add("gl_transaction_type_id", "CNote");
                            glitems.Add("gl_transaction_ref", r.Cpayment_reference);
                            glitems.Add("gl_transaction_description", r.Cpayment_reference);
                            glitems.Add("gl_transaction_date", r.Cpayment_date.ToString("dd/MM/yyyy"));
                            glitems.Add("gl_transaction_duedate", r.Cpayment_date.ToString("dd/MM/yyyy"));
                            if (Cpayment_gross > 0)
                                if (r.Cpayment_gross - r.Cpayment_allocated < 0)
                                {
                                    glitems.Add("gl_transaction_net", -r.Cpayment_gross);
                                    glitems.Add("gl_transaction_tax", "0");
                                    glitems.Add("gl_transaction_gross", -r.Cpayment_gross);
                                    glitems.Add("gl_transaction_discount", "0");
                                    glitems.Add("gl_transaction_due", -r.Cpayment_gross - r.Cpayment_allocated);
                                    glitems.Add("gl_transaction_paid", r.Cpayment_allocated);
                                }
                            if (Cpayment_gross < 0)
                            {
                                if (r.Cpayment_gross - r.Cpayment_allocated > 0)
                                {
                                    glitems.Add("gl_transaction_net", r.Cpayment_gross);
                                    glitems.Add("gl_transaction_tax", "0");
                                    glitems.Add("gl_transaction_gross", r.Cpayment_gross);
                                    glitems.Add("gl_transaction_discount", "0");
                                    glitems.Add("gl_transaction_due", r.Cpayment_gross - r.Cpayment_allocated);
                                    glitems.Add("gl_transaction_paid", r.Cpayment_allocated);
                                }
                            }
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
        private void UpdateAllocated()
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
                string sql = "SELECT sum(`gl_transaction_net`) FROM `cpayment_gls` LEFT JOIN `gl_transactions` ON `cpayment_gl_gl_id`=`gl_transaction_id` WHERE (`gl_transaction_type_id`=4) AND (`cpayment_gl_cpayment_id`=" + this.cpayment_id + ")";
                Hashtable items = new Hashtable();
                decimal al = 0;
                decimal.TryParse(mydb.ReturnTable(sql, "s").Rows[0][0].ToString(), out al);

                items.Add("cpayment_allocated", al);
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
                #region Format
                int cinvoice_id = Convert.ToInt32(items["cinvoice_id"]);
                items.Remove("cinvoice_id");
                items.Add("gl_transaction_ref_type_id", "4");
                if (items.ContainsKey("gl_transaction_type_id"))
                    items["gl_transaction_type_id"] = GL_TRAN_TYPE_ID;
                else
                    items.Add("gl_transaction_type_id", GL_TRAN_TYPE_ID);

                if (items.ContainsKey("gl_transaction_ref"))
                    items["gl_transaction_ref"] = this.cpayment_id.ToString();
                else
                    items.Add("gl_transaction_ref", this.cpayment_id.ToString());

                if (items.ContainsKey("gl_transaction_chart_id"))
                {
                    Sapp.SMS.System system = new System(constr);
                    system.LoadData("CPAYMENTCHARTCODE");
                    string chart_code = system.SystemValue;
                    ChartMaster chart = new ChartMaster(constr);
                    chart.LoadData(chart_code);
                    items["gl_transaction_chart_id"] = chart.ChartMasterId;
                }
                else
                {
                    Sapp.SMS.System system = new System(constr);
                    system.LoadData("CPAYMENTCHARTCODE");
                    string chart_code = system.SystemValue;
                    ChartMaster chart = new ChartMaster(constr);
                    chart.LoadData(chart_code);
                    items.Add("gl_transaction_chart_id", chart.ChartMasterId);
                }

                if (items.ContainsKey("gl_transaction_bodycorp_id"))
                {
                    items["gl_transaction_bodycorp_id"] = this.cpayment_bodycorp_id.ToString();
                }
                else
                    items.Add("gl_transaction_bodycorp_id", this.cpayment_bodycorp_id.ToString());

                if (items.ContainsKey("gl_transaction_unit_id"))
                    items["gl_transaction_unit_id"] = DBSafeUtils.IntToSQL("");
                else
                    items.Add("gl_transaction_unit_id", DBSafeUtils.IntToSQL(""));

                if (items.ContainsKey("gl_transaction_description"))
                    items["gl_transaction_description"] = DBSafeUtils.StrToQuoteSQL("CPayment");
                else
                    items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL("CPayment"));

                if (items.ContainsKey("gl_transaction_gross"))
                    items["gl_transaction_gross"] = "0";
                else
                    items.Add("gl_transaction_gross", "0");

                if (items.ContainsKey("gl_transaction_tax"))
                    items["gl_transaction_tax"] = "0";
                else
                    items.Add("gl_transaction_tax", "0");


                if (items.ContainsKey("gl_transaction_date"))
                    items["gl_transaction_date"] = DBSafeUtils.DateToSQL(this.cpayment_date);
                else
                    items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cpayment_date));
                #endregion
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
                mydb.ExecuteInsert("gl_transactions", items);
                string sql = "SELECT LAST_INSERT_ID()";
                object id = mydb.ExecuteScalar(sql);
                int gl_transaction_id = Convert.ToInt32(id);
                #region INSERT CpaymentGl
                Hashtable cpayment_gl_items = new Hashtable();
                cpayment_gl_items.Add("cpayment_gl_cpayment_id", this.cpayment_id);
                cpayment_gl_items.Add("cpayment_gl_gl_id", gl_transaction_id);
                CpaymentGl cpaymentgl = new CpaymentGl(constr);
                cpaymentgl.Add(cpayment_gl_items, mydb);
                #endregion
                #region INSERT CinvoiceGl
                Hashtable cinvoice_gl_items = new Hashtable();
                cinvoice_gl_items.Add("cinvoice_gl_cinvoice_id", cinvoice_id);
                cinvoice_gl_items.Add("cinvoice_gl_gl_id", gl_transaction_id);
                CinvoiceGl cinvoicegl = new CinvoiceGl(constr);
                cinvoicegl.SetOdbc(mydb);
                cinvoicegl.Add(cinvoice_gl_items, mydb);
                Cinvoice cinvoice = new Cinvoice(constr);
                cinvoice.LoadData(cinvoice_id);
                #endregion
                mydb.Commit();
                this.UpdateAllocated();
                cinvoice.UpdatePaid();
                return gl_transaction_id;
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
                    odbc = null;
                    if (mydb != null) mydb.Close();
                }
            }
        }
        public void UpdateGlTran(Hashtable items, int gl_transaction_id)
        {
            Odbc mydb = null;
            bool isnull = true;
            try
            {
                #region Format
                int cinvoice_id = Convert.ToInt32(items["cinvoice_id"]);
                items.Remove("cinvoice_id");

                if (items.ContainsKey("gl_transaction_type_id"))
                    items["gl_transaction_type_id"] = GL_TRAN_TYPE_ID;
                else
                    items.Add("gl_transaction_type_id", GL_TRAN_TYPE_ID);

                if (items.ContainsKey("gl_transaction_ref"))
                    items["gl_transaction_ref"] = this.cpayment_id.ToString();
                else
                    items.Add("gl_transaction_ref", this.cpayment_id.ToString());

                if (items.ContainsKey("gl_transaction_chart_id"))
                {
                    Sapp.SMS.System system = new System(constr);
                    system.LoadData("CPAYMENTCHARTCODE");
                    string chart_code = system.SystemValue;
                    ChartMaster chart = new ChartMaster(constr);
                    chart.LoadData(chart_code);
                    items["gl_transaction_chart_id"] = chart.ChartMasterId;
                }
                else
                {
                    Sapp.SMS.System system = new System(constr);
                    system.LoadData("CPAYMENTCHARTCODE");
                    string chart_code = system.SystemValue;
                    ChartMaster chart = new ChartMaster(constr);
                    chart.LoadData(chart_code);
                    items.Add("gl_transaction_chart_id", chart.ChartMasterId);
                }

                if (items.ContainsKey("gl_transaction_bodycorp_id"))
                {
                    items["gl_transaction_bodycorp_id"] = this.cpayment_bodycorp_id.ToString();
                }
                else
                    items.Add("gl_transaction_bodycorp_id", this.cpayment_bodycorp_id.ToString());

                if (items.ContainsKey("gl_transaction_unit_id"))
                    items["gl_transaction_unit_id"] = DBSafeUtils.IntToSQL("");
                else
                    items.Add("gl_transaction_unit_id", DBSafeUtils.IntToSQL(""));

                if (items.ContainsKey("gl_transaction_description"))
                    items["gl_transaction_description"] = DBSafeUtils.StrToQuoteSQL("CPayment");
                else
                    items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL("CPayment"));

                if (items.ContainsKey("gl_transaction_gross"))
                    items["gl_transaction_gross"] = "0";
                else
                    items.Add("gl_transaction_gross", "0");

                if (items.ContainsKey("gl_transaction_tax"))
                    items["gl_transaction_tax"] = "0";
                else
                    items.Add("gl_transaction_tax", "0");



                if (items.ContainsKey("gl_transaction_date"))
                    items["gl_transaction_date"] = DBSafeUtils.DateToSQL(this.cpayment_date);
                else
                    items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cpayment_date));
                #endregion
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
                mydb.ExecuteUpdate("gl_transactions", items, "WHERE `gl_transaction_id`=" + gl_transaction_id);
                mydb.Commit();
                Cinvoice cinvoice = new Cinvoice(constr);
                cinvoice.LoadData(cinvoice_id);
                this.UpdateAllocated();
                cinvoice.UpdatePaid();
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
                    odbc = null;
                    if (mydb != null) mydb.Close();
                }
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
                CpaymentGl cpaymentgl = new CpaymentGl(constr);
                cpaymentgl.LoadDataByGLId(gl_transaction_id);
                cpaymentgl.Delete(mydb);

                //CinvoiceGl cinvoicegl = new CinvoiceGl(constr);
                //cinvoicegl.SetOdbc(mydb);
                //cinvoicegl.LoadDataByGLId(gl_transaction_id);
                //int cinvoice_id = cinvoicegl.Cinvoice_gl_cinvoice_id;
                //cinvoicegl.Delete();

                //string sql = "DELETE FROM `gl_transactions` WHERE `gl_transaction_id`=" + gl_transaction_id;
                //mydb.NonQuery(sql);

                mydb.Commit();
                this.UpdateAllocated();
                //Cinvoice cinvoice = new Cinvoice(constr);
                //cinvoice.LoadData(cinvoice_id);
                //cinvoice.UpdatePaid();

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
                    odbc = null;
                    if (mydb != null) mydb.Close();
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
                string sql = "SELECT * FROM `cpayment_gls` LEFT JOIN `gl_transactions` ON `cpayment_gl_gl_id`=`gl_transaction_id` WHERE `cpayment_gl_cpayment_id`=" + this.cpayment_id;
                OdbcDataReader dr = mydb.Reader(sql);
                while (dr.Read())
                {
                    GlTransaction<CPayment> gltran = new GlTransaction<CPayment>(constr);
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
                DataTable dt = odbc.ReturnTable("SELECT * FROM gl_transactions INNER JOIN cpayment_gls ON gl_transactions.gl_transaction_id = cpayment_gls.cpayment_gl_gl_id where cpayment_gl_cpayment_id=" + this.Cpayment_id + " and gl_transaction_type_id=4", "p");
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
                items.Add("gl_transaction_type_id", GL_TRAN_TYPE_ID);
                items.Add("gl_transaction_ref_type_id", "4");
                items.Add("gl_transaction_ref", this.Cpayment_id.ToString());

                Sapp.SMS.System system = new System(constr);
                system.SetOdbc(odbc);
                system.LoadData("CPAYMENTCHARTCODE");
                string chart_code = system.SystemValue;
                ChartMaster chart = new ChartMaster(constr);
                chart.SetOdbc(odbc);
                chart.LoadData(chart_code);
                items.Add("gl_transaction_chart_id", chart.ChartMasterId);

                items.Add("gl_transaction_bodycorp_id", this.cpayment_bodycorp_id.ToString());

                items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL("Allocation"));

                items.Add("gl_transaction_gross", "0");

                items.Add("gl_transaction_tax", "0");

                items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(this.cpayment_date));

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
                Hashtable items = new Hashtable();
                items.Add("cpayment_gl_cpayment_id", this.Cpayment_id);
                items.Add("cpayment_gl_gl_id", tran_id);
                CpaymentGl gl = new CpaymentGl(constr);
                gl.Add(items, odbc);
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
                this.UpdateAllocated();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
