using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using Sapp.General;
using Sapp.Common;
using Sapp.Data;
using Sapp.SMS;
using Sapp.JQuery;
using System.Data.Odbc;

namespace sapp_sms
{

    public partial class invoiceallocate : System.Web.UI.Page, IPostBackEventHandler
    {
        public string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        private const string TEMP_TYPE_RELATED = "invPaid";
        private const string TEMP_TYPE_UNPAID = "invUnPaid";
        private const string TYPE_ID = "3";
        private static bool DataModified = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { jqGridRelated, jqGridUnpaid, LabelAllocated, LabelGross };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion

                //modified by dyyr @2016 07 24
                DataModified = false;
                Bodycorp body = new Bodycorp(AdFunction.conn);
                body.LoadData(Convert.ToInt32(Request.Cookies["bodycorpid"].Value));
                if (body.BodycorpDiscount)
                {
                    HiddenOfferDiscount.Value = "true";
                }
                else
                {
                    HiddenOfferDiscount.Value = "false";
                }

                if (!IsPostBack && Request.QueryString["id"] != null)
                {
                    string id = "";
                    id = Request.QueryString["id"].ToString();
                    HttpContext.Current.Session["Invoice_ID"] = id;
                    HttpContext.Current.Session["DiscountDT"] = null;

                    #region Initial Web Controls


                    InvoiceMaster im = new InvoiceMaster(AdFunction.conn);
                    im.LoadData(Convert.ToInt32(id));

                    #region Check Close Off Date
                    {
                        Bodycorp b = new Bodycorp(constr);
                        b.LoadData(im.InvoiceMasterBodycorpId);
                        if (!b.CheckCloseOff(im.InvoiceMasterDate))
                        {
                            throw new Exception("Before close date");

                        }
                    }
                    #endregion

                    DebtorMaster debtor = new DebtorMaster(constr);
                    debtor.LoadData(im.InvoiceMasterDebtorId);
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(im.InvoiceMasterBodycorpId);
                    if (im.InvoiceMasterUnitId != null)
                    {
                        UnitMaster unit = new UnitMaster(constr);
                        unit.LoadData(im.InvoiceMasterUnitId.Value);
                        LabelUnit.Text = unit.UnitMasterCode;
                    }
                    else
                        LabelUnit.Text = "";
                    LabelNum.Text = im.InvoiceMasterNum;
                    LabelNote.Text = im.InvoiceMasterDescription;
                    LabelDue.Text = DBSafeUtils.DBDateToStr(im.InvoiceMasterDue);
                    LabelGross.Text = im.InvoiceMasterGross.ToString("f2");
                    LabelDate.Text = DBSafeUtils.DBDateToStr(im.InvoiceMasterDate);
                    LabelAllocated.Text = im.InvoiceMasterPaid.ToString("0.00");
                    LabelBodycorp.Text = bodycorp.BodycorpCode;

                    #endregion

                    #region Initial Temp File In Mysql
                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(AdFunction.constr_general);
                    user.LoadData(login);
                    GlTransactionTemps gltranTemps;

                    GlTransactionTemps gltranTemps2;

                    string related_json = null;     // Add 26/05/2016
                    if (im.InvoiceMasterGross > 0)
                    {
                        LabelGross.Text = im.InvoiceMasterGross.ToString("f2");
                        Page.Title = "Sapp SMS - Invoice Allocate";
                        gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, AdFunction.constr_general, constr);
                        related_json = im.Get_Inv_Related_JSON();       // Add 26/05/2016
                        gltranTemps.Add(related_json);
                        gltranTemps2 = new GlTransactionTemps(user.UserId, TEMP_TYPE_UNPAID, AdFunction.constr_general, constr);
                        gltranTemps2.Add(im.Get_Inv_Unpaid_JSON());

                    }
                    else
                    {
                        LabelGross.Text = (-im.InvoiceMasterGross).ToString("f2");
                        Page.Title = "Sapp SMS - CredteNote Allocate";
                        gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, AdFunction.constr_general, constr);
                        related_json = im.Get_CNOTE_Related_JSON();       // Add 26/05/2016
                        gltranTemps.Add(related_json);
                        gltranTemps2 = new GlTransactionTemps(user.UserId, TEMP_TYPE_UNPAID, AdFunction.constr_general, constr);
                        gltranTemps2.Add(im.Get_CNOTE_Unpaid_JSON());

                    }

                    LabelAllocated.Text = GetAllocate();    // Update 15/03/2016 Initialize 'Discount' for display

                    // Load system value of Discount
                    Sapp.SMS.System sys = new Sapp.SMS.System(AdFunction.conn);
                    sys.LoadData("MAXDISCOUNT");
                    HiddenMaxDiscount.Value = sys.SystemValue;

                    // Add 26/05/2016 Check is there a receipt allocated to the invoice
                    Hashtable items = (Hashtable)JSON.JsonDecode(related_json);
                    ArrayList glList = (ArrayList)items["gl_transactions"];

                    HiddenHasReceipt.Value = "false";
                    foreach (Hashtable gltran in glList)
                    {
                        if ("Receipt".Equals(gltran["gl_transaction_type_id"].ToString()))
                        {
                            HiddenHasReceipt.Value = "true";
                        }
                    }

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
                if (args[0] == "ImageButtonDown")
                {
                    ImageButtonDown_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string jqGridRelatedDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);
            GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
            gltranTemps.LoadData();

            DataTable dt = new DataTable("gl_temp_transactions");
            dt.Columns.Add("ID");
            dt.Columns.Add("RefType");
            dt.Columns.Add("InvoiceNum");
            dt.Columns.Add("Description");
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("DueDate", typeof(DateTime));
            dt.Columns.Add("Net");
            dt.Columns.Add("Tax");
            dt.Columns.Add("Gross");
            dt.Columns.Add("Due");
            dt.Columns.Add("Paid");
            dt.Columns.Add("Discount");     // Add 'Discount' column 15/03/2016

            foreach (GlTransactionTemp gltran in gltranTemps.GLTempList)
            {
                Hashtable items = gltran.GetData();
                DataRow dr = dt.NewRow();
                if (items["gl_transaction_id"].ToString()[0] != 'd'
                        && isSystemDiscount(items["gl_transaction_ref"].ToString()) == false // Update 15/03/2016 Add 'Discount' column
                        && !(items["gl_transaction_description"].ToString().Contains("Discount Offered")) //modified by dyyr @2016 07 24
                    ) 
                {
                    dr["ID"] = items["gl_transaction_id"].ToString();
                    dr["RefType"] = items["gl_transaction_type_id"].ToString();
                    dr["InvoiceNum"] = items["gl_transaction_ref"].ToString();
                    dr["Description"] = items["gl_transaction_description"].ToString();
                    dr["Date"] = items["gl_transaction_date"].ToString();
                    if (!items["gl_transaction_duedate"].ToString().Equals("Null") && !items["gl_transaction_duedate"].ToString().Equals(""))
                        dr["DueDate"] = items["gl_transaction_duedate"].ToString();
                    dr["Net"] = items["gl_transaction_net"].ToString();
                    dr["Tax"] = items["gl_transaction_tax"].ToString();
                    dr["Gross"] = items["gl_transaction_gross"].ToString();
                    
                    dr["Paid"] = items["gl_transaction_paid"].ToString();


                    //modified by dyyr @2016 07 23
                    if (HttpContext.Current.Session["Invoice_ID"] != null)
                    {
                        dr["Discount"] = GetDiscount(dr["InvoiceNum"].ToString());      // Update 15/03/2016 Add 'Discount' column
                    }
                    else
                    {
                        dr["Discount"] = "0";
                    }
                    if (DataModified)
                    {
                        dr["Due"] = items["gl_transaction_due"].ToString();
                    }
                    else
                    {
                        decimal due = 0, discount = 0;
                        decimal.TryParse(items["gl_transaction_due"].ToString(), out due);
                        decimal.TryParse(dr["Discount"].ToString(), out discount);
                        dr["Due"] = (due - discount).ToString();
                    }
                    //modified end

                    
                    dt.Rows.Add(dr);
                }
            }

            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`,`RefType`, `InvoiceNum`, `Description`, `Date`, `DueDate`, `Net`, `Tax`,`Gross`, `Due`, `Paid`, `Discount` FROM (SELECT *";  // Update 15/03/2016 Add 'Discount' column
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }

        #region discount
        public static void SettingDiscount(string discount, string receipt_ref)
        {
            DataTable dt = new DataTable();
            if (discount.Equals(""))
                discount = "0";
            if (HttpContext.Current.Session["DiscountDT"] == null)
                GetDiscountDT(HttpContext.Current.Session["Invoice_ID"].ToString());
            if (HttpContext.Current.Session["DiscountDT"] != null)
            {
                dt = (DataTable)HttpContext.Current.Session["DiscountDT"];
                bool checkupdate = false;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["receipt_ref"].ToString().Equals(receipt_ref))
                    {
                        dr["gl_transaction_net"] = discount;
                        checkupdate = true;
                    }
                }
                if (!checkupdate)
                {
                    if (!receipt_ref.Contains("JNL"))
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        DataRow dr = dt.NewRow();
                        dr["receipt_ref"] = receipt_ref;
                        DataTable invDT = ReportDT.getTable(constr, "invoice_master");
                        string invoice_master_num = ReportDT.GetDataByColumn(invDT, "invoice_master_id", HttpContext.Current.Session["Invoice_ID"].ToString(), "invoice_master_num");
                        dr["invoice_master_num"] = invoice_master_num;
                        dr["invoice_master_id"] = HttpContext.Current.Session["Invoice_ID"].ToString();
                        dr["gl_transaction_net"] = discount;
                        DataTable receptDT = ReportDT.getTable(constr, "receipts");
                        string receipt_id = ReportDT.GetDataByColumn(receptDT, "receipt_ref", receipt_ref, "receipt_id");
                        dr["receipt_gl_receipt_id"] = receipt_id;
                        dt.Rows.Add(dr);
                    }
                }
            }


            HttpContext.Current.Session["DiscountDT"] = dt;
        }

        /// <summary>
        /// Get discount amount by receipt id
        /// </summary>
        /// <param name="receipt_id">receipt id</param>
        /// <returns>discount amoun</returns>
        public static string GetDiscount(string receipt_ref)
        {
            string r = null;
            DataTable dt = new DataTable();
            if (HttpContext.Current.Session["DiscountDT"] != null)
            {
                dt = (DataTable)HttpContext.Current.Session["DiscountDT"];
            }
            else
            {
                GetDiscountDT(HttpContext.Current.Session["Invoice_ID"].ToString());
                dt = (DataTable)HttpContext.Current.Session["DiscountDT"];
            }
            r = ReportDT.GetDataByColumn(dt, "receipt_ref", receipt_ref, "gl_transaction_net");

            if ("".Equals(r))
            {
                r = "0.00";
            }

            return r;
        }

        /// <summary>
        /// check the exist of system discount
        /// </summary>
        /// <param name="reference">reference (journal id)</param>
        private static bool isSystemDiscount(string reference)
        {
            Odbc odbc = null;
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            try
            {
                // Load DB value of Discount
                Sapp.SMS.System sys = new Sapp.SMS.System(AdFunction.conn);
                ChartMaster cm = new ChartMaster(AdFunction.conn);

                sys.LoadData("GENERALDEBTOR");
                cm.LoadData(sys.SystemValue);
                string chart_id_debtor = cm.ChartMasterId.ToString();

                sys.LoadData("GENERALLEVY");
                cm.LoadData(sys.SystemValue);
                string chart_id_general_Levy = cm.ChartMasterId.ToString();

                sys.LoadData("DISCOUNTCHARCODE");
                cm.LoadData(sys.SystemValue);
                string chart_id_discount = cm.ChartMasterId.ToString();

                string sql = " SELECT gl_transaction_id, gl_transaction_net "
                           + "  FROM gl_transactions "
                           + "  WHERE gl_transaction_type_id = 6 "
                           + "    AND gl_transaction_ref = '" + reference + "' "
                           + "    AND gl_transaction_chart_id IN (" + chart_id_general_Levy + ", " + chart_id_discount + ") ";

                odbc = new Odbc(constr);
                OdbcDataReader reader = odbc.Reader(sql);

                if (reader.Read())
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (odbc != null)
                {
                    odbc.Close();
                }
            }
            
        }

        /// <summary>
        /// Get discount amount and set to session
        /// </summary>
        /// <param name="reciptID">receipt id</param>
        public static void GetDiscountDT(string invoice_id)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string sql = " SELECT gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_net, ";
            sql = sql + "        receipt_gls.receipt_gl_receipt_id, invoice_master.invoice_master_id, invoice_master.invoice_master_num, receipts.receipt_ref ";
            sql = sql + "  FROM gl_transactions, invoice_gls, receipt_gls, invoice_master, receipts";
            sql = sql + "  WHERE gl_transactions.gl_transaction_id = invoice_gls.invoice_gl_gl_id ";
            sql = sql + "    AND invoice_gls.invoice_gl_gl_id = receipt_gls.receipt_gl_gl_id ";
            sql = sql + "    AND invoice_gls.invoice_gl_invoice_id = invoice_master.invoice_master_id ";
            sql = sql + "    AND receipts.receipt_id = receipt_gls.receipt_gl_receipt_id ";


            sql = sql + "    AND (gl_transactions.gl_transaction_type_id = 6) ";


            //sql = sql + "    AND (receipt_gls.receipt_gl_receipt_id = " + reciptID + ")";
            sql = sql + "    AND (invoice_gls.invoice_gl_invoice_id = " + invoice_id + ")";

            Odbc o = new Odbc(constr);
            DataTable dt = o.ReturnTable(sql, "DiscountDT");
            HttpContext.Current.Session["DiscountDT"] = dt;
        }

        public void UpDateDiscount(string receipt_ref)
        {
            Odbc o = new Odbc(constr);
            DataTable dt = (DataTable)Session["DiscountDT"];
            string grossid = ReportDT.GetDataByColumn(dt, "receipt_ref", receipt_ref, "gl_transaction_id");
            if (!grossid.Equals(""))
            {
                int Grossglid = int.Parse(grossid);
                string invID = ReportDT.GetDataByColumn(dt, "receipt_ref", receipt_ref, "invoice_master_id");
                string g = ReportDT.GetDataByColumn(dt, "receipt_ref", receipt_ref, "gl_transaction_net");
                if (g.Equals(""))
                    g = "0";
                if (g.Equals("0") || g.Equals("0.00"))
                {
                    DeleteDiscount(receipt_ref);
                }
                else
                {
                    decimal gross = decimal.Parse(g);
                    //string gltaxid = (Grossglid - 1).ToString();
                    //string glnetid = (Grossglid - 2).ToString();

                    string glgrossid = Grossglid.ToString();


                    DataTable sysDT = ReportDT.getTable(constr, "system");
                    string taxcode = ReportDT.GetDataByColumn(sysDT, "system_code", "GST Output", "system_value");
                    string discountcode = ReportDT.GetDataByColumn(sysDT, "system_code", "DISCOUNTCHARCODE", "system_value");
                    DataTable chartDT = ReportDT.getTable(constr, "chart_master");
                    string did = AdFunction.GENERALDEBTOR_ChartID;
                    string taxtid = ReportDT.GetDataByColumn(chartDT, "chart_master_code", taxcode, "chart_master_id");
                    string disid = ReportDT.GetDataByColumn(chartDT, "chart_master_code", discountcode, "chart_master_id");
                    string reference = Journal.GetNextNumber();
                    Receipt receipt = new Receipt(constr);

                    //dyyr modified @20160723
                    //string receipt_id = Request.QueryString["receiptid"];
                    string receipt_id = ReportDT.GetDataByColumn(dt, "receipt_ref", receipt_ref, "receipt_gl_receipt_id");
                    


                    receipt.LoadData(Convert.ToInt32(receipt_id));
                    decimal gst = decimal.Parse(ReportDT.GetDataByColumn(sysDT, "system_code", "GST", "system_value"));
                    decimal tax = gross / 115 * 15;
                    tax = AdFunction.Rounded(tax.ToString());
                    decimal net = gross - tax;
                    tax = -tax;
                    net = -net;


                    string sql = "select * from gl_transactions where gl_transaction_id=" + glgrossid;

                    DataTable DT1 = o.ReturnTable(sql, "D1");
                    string dt1Ref = "";
                    if (DT1.Rows.Count == 1)
                    {
                        dt1Ref = DT1.Rows[0]["gl_transaction_ref"].ToString();
                    }
                    string sql2 = "select * from gl_transactions where gl_transaction_ref='" + dt1Ref + "'";

                    DataTable DT2 = o.ReturnTable(sql2, "D2");

                    string gltaxid = ReportDT.GetDataByColumn(DT2, "gl_transaction_chart_id", taxtid, "gl_transaction_id");
                    string glnetid = ReportDT.GetDataByColumn(DT2, "gl_transaction_chart_id", disid, "gl_transaction_id");

                    if (gltaxid.Equals("") || glnetid.Equals(""))
                    {
                        throw new Exception("Discount Journal Error");

                    }




                    string tsql = "Update gl_transactions set gl_transaction_net=" + tax + " where gl_transaction_id=" + gltaxid;
                    string nsql = "Update gl_transactions set gl_transaction_net=" + net + " where gl_transaction_id=" + glnetid;
                    string gsql = "Update gl_transactions set gl_transaction_net=" + gross + " where gl_transaction_id=" + glgrossid;
                    o.ExecuteScalar(tsql);
                    o.ExecuteScalar(nsql);
                    o.ExecuteScalar(gsql);
                }
                //UpdatePaid(-gross, invID);
            }
            else
            {
                InsertDiscount(receipt_ref);
            }
        }
        public void InsertDiscount(string receipt_ref)
        {
            if (Session["DiscountDT"] != null)
            {
                if (!receipt_ref.Equals(""))
                {
                    // Add 7/6/2016 Load rate of Discount
                    Sapp.SMS.System sys = new Sapp.SMS.System(AdFunction.conn);
                    sys.LoadData("GST");
                    decimal gst_rate = decimal.Parse(sys.SystemValue);

                    Bodycorp body = new Bodycorp(AdFunction.conn);
                    body.LoadData(Convert.ToInt32(Request.Cookies["bodycorpid"].Value));

                    //dyyr modified @ 2016 07 23
                    //DataTable dtDiscount = (DataTable)Session["DiscountDT"];
                    //DataTable dt = ReportDT.FilterDT(dtDiscount, "receipt_ref=" + receipt_ref);
                    DataTable dt = (DataTable)Session["DiscountDT"];
              //      foreach (DataRow dr in dt.Rows)
              //      {}

                    string discountValue = ReportDT.GetDataByColumn(dt, "receipt_ref", receipt_ref, "gl_transaction_net");
                    if (discountValue.Equals(""))
                    {
                        discountValue = "0";
                    }
                    decimal dv = decimal.Parse(discountValue);
                    if (dv != 0)
                    {
                        DataTable sysDT = ReportDT.getTable(constr, "system");
                        string taxcode = ReportDT.GetDataByColumn(sysDT, "system_code", "GST Output", "system_value");
                        string discountcode = ReportDT.GetDataByColumn(sysDT, "system_code", "DISCOUNTCHARCODE", "system_value");
                        DataTable chartDT = ReportDT.getTable(constr, "chart_master");
                        string did = AdFunction.GENERALDEBTOR_ChartID;
                        string taxtid = ReportDT.GetDataByColumn(chartDT, "chart_master_code", taxcode, "chart_master_id");
                        string disid = ReportDT.GetDataByColumn(chartDT, "chart_master_code", discountcode, "chart_master_id");
                        string reference = Journal.GetNextNumber();
                        Receipt receipt = new Receipt(constr);

                        //dyyr modified @20160723
                        string receipt_id = ReportDT.GetDataByColumn(dt, "receipt_ref", receipt_ref, "receipt_gl_receipt_id");

                        receipt.LoadData(Convert.ToInt32(receipt_id));
                        decimal gst = decimal.Parse(ReportDT.GetDataByColumn(sysDT, "system_code", "GST", "system_value"));
                        decimal discountv = decimal.Parse(discountValue);
                        decimal gross = discountv;

                        // Update 7/6/2016
                        //decimal tax = gross / 115 * 15;
                        decimal tax = 0m;
                        if (body.BodycorpNoGST == false)
                        {
                            tax = gross * gst_rate / (1.0m + gst_rate);
                            tax = AdFunction.Rounded(tax.ToString());
                        }

                        decimal net = discountv - tax;

                        //dyyr modified @2016 07 23
                    //       DataTable reglDT = ReportDT.getTable(constr, "gl_transactions");
                    //       reglDT = ReportDT.FilterDT(reglDT, "gl_transaction_type_id=6 and gl_transaction_chart_id=" + disid);
                        string invID = ReportDT.GetDataByColumn(dt, "receipt_ref", receipt_ref, "invoice_master_id");

                        InsertDiscount(reference, receipt.ReceiptBodycorpId, net, tax, gross, taxtid, did, disid, invID, receipt_id);
                    }
                    
                }

            }
        }

        public void InsertDiscount(string reference, int bodycorpid, decimal net, decimal tax, decimal gross, string tid, string nid, string gid, string InvID, string reid)
        {
            if (Session["DiscountDT"] != null)
            {
                Odbc o = new Odbc(constr);
                DataTable recepitDT = ReportDT.getTable(constr, "receipts");

              //dyyr modified @20160723
              //string unitid = ReportDT.GetDataByColumn(recepitDT, "receipt_id", Request.QueryString["receiptid"].ToString(), "receipt_unit_id");
                string unitid = ReportDT.GetDataByColumn(recepitDT, "receipt_id", reid, "receipt_unit_id");
                
                DataTable chartDT = ReportDT.getTable(constr, "chart_master");
                string login = System.Web.HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new Sapp.General.User(Sapp.SMS.AdFunction.constr_general);
                user.LoadData(login);
                string glsqlnet = "INSERT INTO gl_transactions (gl_transaction_ref_type_id,gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross, gl_transaction_date,gl_transaction_description,gl_transaction_unit_id,gl_transaction_createdate,gl_transaction_user_id)";
                glsqlnet += " VALUES   (6,6,'" + reference + "'," + gid + "," + bodycorpid + "," + -net + ",0,0," + DBSafeUtils.DateTimeToSQL(LabelDate.Text) + ",'" + "Discount Offered" + "'," + unitid + ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," + user.UserId.ToString() + ")";
                string glsqltax = "INSERT INTO gl_transactions (gl_transaction_ref_type_id,gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross, gl_transaction_date,gl_transaction_description,gl_transaction_unit_id,gl_transaction_createdate,gl_transaction_user_id)";
                glsqltax += " VALUES   (6,6,'" + reference + "'," + tid + "," + bodycorpid + "," + -tax + ",0,0," + DBSafeUtils.DateTimeToSQL(LabelDate.Text) + ",'" + "Discount Offered" + "'," + unitid + ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," + user.UserId.ToString() + ")";
                string glsqlgross = "INSERT INTO gl_transactions (gl_transaction_ref_type_id,gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_net, gl_transaction_tax, gl_transaction_gross, gl_transaction_date,gl_transaction_description,gl_transaction_unit_id,gl_transaction_createdate,gl_transaction_user_id)";
                glsqlgross += " VALUES   (6,6,'" + reference + "'," + nid + "," + bodycorpid + "," + gross + ",0,0," + DBSafeUtils.DateTimeToSQL(LabelDate.Text) + ",'" + "Discount Offered" + "'," + unitid + ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," + user.UserId.ToString() + ")";


                string idsql = "SELECT LAST_INSERT_ID()";
                o.ReturnTable(glsqlnet, "NetDT");
                string glnetID = Convert.ToInt32(o.ExecuteScalar(idsql)).ToString();
                o.ReturnTable(glsqltax, "NetDT");
                string gltaxID = Convert.ToInt32(o.ExecuteScalar(idsql)).ToString();
                o.ReturnTable(glsqlgross, "NetDT");
                string glgrossID = Convert.ToInt32(o.ExecuteScalar(idsql)).ToString();
                string recGL3 = " INSERT INTO receipt_gls (receipt_gl_receipt_id, receipt_gl_gl_id, receipt_gl_paid) VALUES   (" + reid + "," + glgrossID + ",0)";
                o.ExecuteScalar(recGL3);
                DataTable invoice = ReportDT.getTable(constr, "invoice_master");
                string invGL3 = "INSERT INTO invoice_gls (invoice_gl_invoice_id, invoice_gl_gl_id, invoice_gl_paid) VALUES   (" + InvID + "," + glgrossID + ",0)";
                o.ExecuteScalar(invGL3);
                //UpdatePaid(gross, InvID);
            }
        }
        public void DeleteDiscount(string receipt_ref)
        {
            Odbc o = new Odbc(constr);
            DataTable dt = (DataTable)Session["DiscountDT"];
            string gid = ReportDT.GetDataByColumn(dt, "receipt_ref", receipt_ref, "gl_transaction_id");
            if (!gid.Equals(""))
            {
                int glid = int.Parse(gid);
                string glgrossid = glid.ToString();

                //modified by dyyr @2016 07 23
                //string referencr = ReportDT.GetDataByColumn(ReportDT.getTable(AdFunction.conn, "gl_transactions"), "gl_transaction_id", gid, "gl_transaction_ref");
                string sql = "select gl_transaction_ref from gl_transactions where gl_transaction_id=" + gid;
                DataTable DT1 = o.ReturnTable(sql, "D1");
                string referencr = "";
                if (DT1.Rows.Count > 0)
                {
                    referencr = DT1.Rows[0].ToString();
                }

                string rsql = "delete FROM  receipt_gls where receipt_gl_gl_id =" + glgrossid;
                string isql = "delete FROM  invoice_gls where invoice_gl_gl_id =" + glgrossid;
                string tsql = "delete FROM  gl_transactions where gl_transaction_ref='" + referencr + "'";
                o.ExecuteScalar(rsql);
                o.ExecuteScalar(isql);
                o.ExecuteScalar(tsql);
            }
        }

        #endregion discount

        [System.Web.Services.WebMethod]
        public static string jqGridUnpaidDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);
            GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_UNPAID, constr_general, constr);
            gltranTemps.LoadData();

            DataTable dt = new DataTable("gl_temp_transactions");
            dt.Columns.Add("ID");
            dt.Columns.Add("RefType");
            dt.Columns.Add("InvoiceNum");
            dt.Columns.Add("Description");
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("DueDate", typeof(DateTime));
            dt.Columns.Add("Net");
            dt.Columns.Add("Tax");
            dt.Columns.Add("Gross");
            dt.Columns.Add("Due");
            foreach (GlTransactionTemp gltran in gltranTemps.GLTempList)
            {
                Hashtable items = gltran.GetData();
                DataRow dr = dt.NewRow();
                if (items["gl_transaction_id"].ToString()[0] != 'd'
                    && isSystemDiscount(items["gl_transaction_ref"].ToString()) == false)   // Update 25/05/2016
                    //&& items["gl_transaction_type_id"].ToString() != "Journal")      // Update 15/03/2016 Add 'Discount' column (Hide discount row)
                {
                    dr["ID"] = items["gl_transaction_id"].ToString();
                    dr["RefType"] = items["gl_transaction_type_id"].ToString();
                    dr["InvoiceNum"] = items["gl_transaction_ref"].ToString();
                    dr["Description"] = items["gl_transaction_description"].ToString();
                    dr["Date"] = items["gl_transaction_date"].ToString();
                    if (!items["gl_transaction_duedate"].ToString().Equals("Null"))
                        dr["DueDate"] = items["gl_transaction_duedate"].ToString();
                    dr["Net"] = items["gl_transaction_net"].ToString();
                    dr["Tax"] = items["gl_transaction_tax"].ToString();
                    dr["Gross"] = items["gl_transaction_gross"].ToString();
                    dr["Due"] = items["gl_transaction_due"].ToString();
                    dt.Rows.Add(dr);
                }
            }
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`,`RefType`, `InvoiceNum`, `Description`, `Date`, `DueDate`, `Net`, `Tax`,`Gross`, `Due` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }
        [System.Web.Services.WebMethod]
        public static string SaveDataFromGrid(string rowValue)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);
            Object c = JSON.JsonDecode(rowValue);
            Hashtable hdata = (Hashtable)c;
            string line_id = hdata["ID"].ToString();
            GlTransactionTemps glts_related = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
            glts_related.LoadData();
            Hashtable line = new Hashtable();
            if (line_id == "")
            {
                throw new Exception("Error: no insert allowed!");
            }
            else
            {
                #region UPDATE
                decimal total_paid = 0;

                for (int i = 0; i < glts_related.GLTempList.Count; i++)
                {
                    decimal temp = 0;
                    decimal.TryParse(glts_related.GLTempList[i].GlTransactionPaid, out temp);
                    total_paid += temp;
                    if (hdata["ID"].ToString().Equals(glts_related.GLTempList[i].GlTransactionId))
                    {

                        decimal temp2 = 0;
                        decimal.TryParse(glts_related.GLTempList[i].GlTransactionPaid, out temp2);
                        decimal ori_paid = temp2;
                        decimal paid = Convert.ToDecimal(hdata["Paid"]);

                        string d = hdata["Discount"].ToString();
                        if (d.Equals(""))
                            d = "0";
                        decimal discount = decimal.Parse(d);
                        if (discount < 0)
                            throw new Exception("Discount must over 0!");
                        decimal due = 0;
                        decimal.TryParse(hdata["Due"].ToString(), out due);
                        decimal olddue = 0;
                        decimal.TryParse(glts_related.GLTempList[i].GlTransactionDue, out olddue);
                        decimal oldgross = 0;
                        decimal.TryParse(glts_related.GLTempList[i].GlTransactionGross, out oldgross);
                        decimal odiscount = 0;
                        
                        //dyyr
                        decimal.TryParse(GetDiscount(hdata["InvoiceNum"].ToString()), out odiscount);

                        
                        // Update 15/03/2016 Update allocation check
                        //if (paid == 0)
                        if (paid <= 0 || paid > oldgross)
                        {
                            throw new Exception("error: Paid can no be less than 0 or larger than gross.");
                        }

                        total_paid = total_paid - ori_paid + paid;

                        if (due - paid + ori_paid + odiscount - discount < 0)
                        {
                            throw new Exception("error: paid and discount is more than due!");
                        }

                        if (glts_related.GLTempList[i].GlTransactionId == hdata["ID"].ToString())
                        {
                            glts_related.GLTempList[i].GlTransactionPaid = hdata["Paid"].ToString();
                            glts_related.GLTempList[i].GlTransactionDue = (due - paid + ori_paid + odiscount - discount).ToString("0.00");
                            glts_related.GLTempList[i].GlTransactionId = glts_related.GLTempList[i].GlTransactionId;
                        }
                        SettingDiscount(hdata["Discount"].ToString(), hdata["InvoiceNum"].ToString());
                        DataModified = true;
                    }

                #endregion
                }


            }
            glts_related.UpdateTemp();

            return "dd";
        }
        [System.Web.Services.WebMethod]
        public static string GetAllocate()
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            decimal allocate = 0;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);

            GlTransactionTemps glts_related = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
            glts_related.LoadData();

            foreach (GlTransactionTemp gltran in glts_related.GLTempList)
            {
                decimal paid = 0;
                decimal.TryParse(gltran.GlTransactionPaid, out paid);

                allocate += paid;
            }

            return allocate.ToString("0.00");
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
                //Related Temp Trans
                GlTransactionTemps glts_related = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                glts_related.LoadData();
                //Unpaid Temp Trans
                GlTransactionTemps glts_unpaid = new GlTransactionTemps(user.UserId, TEMP_TYPE_UNPAID, constr_general, constr);
                glts_unpaid.LoadData();

                foreach (string glt_id in args[1].Split(','))
                {
                    for (int i = 0; i < glts_unpaid.GLTempList.Count; i++)
                    {
                        if (glts_unpaid.GLTempList[i].GlTransactionId == glt_id)
                        {
                            GlTransactionTemp glt = new GlTransactionTemp(constr);
                            GlTransactionTemp glitems = glts_unpaid.GLTempList[i];

                            // get discount in case of receipt
                            string discount_str = "0.0";
                            if ("Receipt".Equals(glitems.GLTransactionTypeId))
                            {
                                discount_str = GetDiscount(glitems.GLTransactionRef);
                            }

                            decimal due = 0;
                            decimal.TryParse(glitems.GlTransactionDue, out due);
                            decimal recGross = decimal.Parse(LabelGross.Text);
                            decimal recAllocate = 0;
                            decimal.TryParse(GetAllocate(), out recAllocate);
                            decimal discount = 0;
                            decimal.TryParse(discount_str, out discount);
                            decimal paid = 0;
                            if ((recGross - recAllocate - discount) >= due)
                                paid = due;
                            else
                                paid = (recGross - recAllocate - discount);
                            glitems.GlTransactionPaid = paid.ToString("f2");
                            glitems.GlTransactionDue = (due - paid).ToString("f2");

                            glts_related.GLTempList.Add(glitems); //Remove from temp data
                            glts_unpaid.GLTempList.RemoveAt(i);
                            // Move discount record (if exist) at the same time
                            moveDiscountToRelate(glts_related.GLTempList, glts_unpaid.GLTempList, glitems);     // Update 15/03/2016 Add 'Discount' column (Hide discount row)

                            glts_related.UpdateTemp();
                            glts_unpaid.UpdateTemp();
                        }
                    }
                }

                glts_related.UpdateTemp();
                glts_unpaid.UpdateTemp();
                LabelAllocated.Text = GetAllocate();

                // Add 26/05/2016
                HiddenHasReceipt.Value = "false";
                for (int i = 0; i < glts_related.GLTempList.Count; i++)
                {
                    if ("Receipt".Equals(glts_related.GLTempList[i].GLTransactionTypeId))
                    {
                        HiddenHasReceipt.Value = "true";
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        /// <summary>
        /// Add discount recount to related transaction list and remove it from outsanding transaction list.
        /// </summary>
        /// <param name="relatedList">related transaction list</param>
        /// <param name="unpaidList">outsanding transaction list</param>
        /// <param name="glitems">payment (receipt) transaction</param>
        private string moveDiscountToRelate(List<GlTransactionTemp> relatedList, List<GlTransactionTemp> unpaidList, GlTransactionTemp glitems)
        {
            string discount_gl_id = getDiscountGLidByReceiptId(glitems.GlTransactionId);
            if (discount_gl_id != null)
            {
                for (int i = 0; i < unpaidList.Count; i++)
                {
                    if (unpaidList[i].GlTransactionId == discount_gl_id)
                    {
                        relatedList.Add(unpaidList[i]);
                        string discount = unpaidList[i].GlTransactionNet;
                        unpaidList.RemoveAt(i);
                        return discount;
                    }
                }
            }

            return "0.0";
        }

        /// <summary>
        /// Get discount transaction id by recept id of payment
        /// </summary>
        /// <param name="receipt_id">recept id</param>
        /// <returns>discount transaction id</returns>
        private string getDiscountGLidByReceiptId(string receipt_id)
        {
            Odbc mydb = null;

            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string dbsql = " SELECT T1.gl_transaction_id FROM gl_transactions as T1, receipt_gls as T2 ";
                dbsql = dbsql + " WHERE T1.gl_transaction_type_id = '6' AND T1.gl_transaction_id = T2.receipt_gl_gl_id AND T2.receipt_gl_receipt_id = \"" + receipt_id + "\"";

                mydb = new Odbc(constr);
                OdbcDataReader reader = mydb.Reader(dbsql);

                if (reader.Read())
                {
                    return DBSafeUtils.DBStrToStr(reader["gl_transaction_id"]);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (mydb != null)
                {
                    mydb.Close();
                }
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
                GlTransactionTemps glts_related = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                glts_related.LoadData();
                GlTransactionTemps glts_unpaid = new GlTransactionTemps(user.UserId, TEMP_TYPE_UNPAID, constr_general, constr);
                glts_unpaid.LoadData();

                string glt_id = args[1];
                for (int i = 0; i < glts_related.GLTempList.Count; i++)
                {
                    if (glts_related.GLTempList[i].GlTransactionId == glt_id)
                    {


                        GlTransactionTemp glt = new GlTransactionTemp(constr);
                        GlTransactionTemp glitems = glts_related.GLTempList[i];
                        decimal due = 0;
                        decimal.TryParse(glitems.GlTransactionDue, out due);
                        decimal paid = 0;
                        decimal.TryParse(glitems.GlTransactionPaid, out paid);
                        glitems.GlTransactionDue = (due + paid).ToString("f2");
                        glts_unpaid.GLTempList.Add(glitems);
                        glts_related.GLTempList.RemoveAt(i); //Remove from temp data

                        // Move discount record (if exist) at the same time
                        moveDiscountToOutstanding(glts_related.GLTempList, glts_unpaid.GLTempList, glitems);        // Update 15/03/2016 Add 'Discount' column (Hide discount row)
                    }
                }
                glts_related.UpdateTemp();
                glts_unpaid.UpdateTemp();
                LabelAllocated.Text = GetAllocate();

                // Add 26/05/2016
                HiddenHasReceipt.Value = "false";
                for (int i = 0; i < glts_related.GLTempList.Count; i++)
                {
                    if ("Receipt".Equals(glts_related.GLTempList[i].GLTransactionTypeId))
                    {
                        HiddenHasReceipt.Value = "true";
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        /// <summary>
        /// Remove discount recount from related transaction list and add it to outsanding transaction list.
        /// </summary>
        /// <param name="relatedList">related transaction list</param>
        /// <param name="unpaidList">outsanding transaction list</param>
        /// <param name="glitems">payment (receipt) transaction</param>
        private void moveDiscountToOutstanding(List<GlTransactionTemp> relatedList, List<GlTransactionTemp> unpaidList, GlTransactionTemp glitems)
        {
            string discount_gl_id = getDiscountGLidByReceiptId(glitems.GlTransactionId);
            if (discount_gl_id != null)
            {
                for (int i = 0; i < relatedList.Count; i++)
                {
                    if (relatedList[i].GlTransactionId == discount_gl_id)
                    {
                        unpaidList.Add(relatedList[i]);       // remove discount completely
                        relatedList.RemoveAt(i);
                    }
                }
            }
        }

        protected void ImageButtonSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string iid = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                iid = Request.QueryString["id"];
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);

                InvoiceMaster im = new InvoiceMaster(AdFunction.conn);
                im.LoadData(int.Parse(iid));

                //Related Temp Trans
                GlTransactionTemps glts_related = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                glts_related.LoadData();

                GlTransactionTemp[] pageList = new GlTransactionTemp[glts_related.GLTempList.Count];


                GlTransactionTemps oldGLTemp = new GlTransactionTemps(user.UserId, "Temp", constr_general, constr);
                string temp = im.Get_Inv_Related_JSON();
                oldGLTemp.Add(temp);
                oldGLTemp.LoadData();

                glts_related.GLTempList.CopyTo(pageList);

                //dyyr modified @2016 07 24
                Bodycorp body = new Bodycorp(AdFunction.conn);
                body.LoadData(Convert.ToInt32(Request.Cookies["bodycorpid"].Value));
                

                // Add 20/05/2016
           //     string offerDiscount = HiddenOfferDiscount.Value; 

                for (int i = 0; i < oldGLTemp.GLTempList.Count; i++)
                {
                    bool exist = false;
                    string id = oldGLTemp.GLTempList[i].GlTransactionId;
                    string paid = oldGLTemp.GLTempList[i].GlTransactionPaid;
                    string gRef = oldGLTemp.GLTempList[i].GLTransactionRef;
                    string type = oldGLTemp.GLTempList[i].GLTransactionTypeId;

                    for (int j = 0; j < pageList.Length; j++)
                    {
                        if (id.Equals(pageList[j].GlTransactionId))
                            exist = true;
                    }
                    if (!exist)
                    {//Delete 
                        if (type.Equals("Invoice"))
                        {
                            InvoiceMaster invoice = new InvoiceMaster(constr);
                            invoice.LoadData(Convert.ToInt32(id));
                            if (body.BodycorpDiscount)
                            {
                                DeleteDiscount(invoice.InvoiceMasterNum);
                            }
                            Allocation.Allocate(im, invoice, Convert.ToDecimal("0"));
                        }
                        if (type.Equals("Receipt"))
                        {
                            Receipt r = new Receipt(constr);
                            r.LoadData(Convert.ToInt32(id));
                            if (body.BodycorpDiscount)
                            {
                                DeleteDiscount(im.InvoiceMasterNum);
                          //      DeleteDiscount(r.InvoiceMasterNum);   dyyr
                            }
                            Allocation.Allocate(im, r, Convert.ToDecimal("0"));
                        }
                        if (type.Equals("Journal"))
                        {
                            Odbc o = new Odbc(AdFunction.conn);
                            o.StartTransaction();
                            try
                            {
                                string paidid = AdFunction.JournalPaid_INV_GLID(iid, gRef);
                                AdFunction.GLS_Inv_Delete(iid, gRef, id, o);
                                string dsql = "delete from invoice_gls where invoice_gl_invoice_id=" + iid + " and invoice_gl_gl_id=" + paidid;
                                o.ExecuteScalar(dsql);
                                dsql = "delete from invoice_gls where invoice_gl_invoice_id=" + iid + " and invoice_gl_gl_id=" + id;
                                o.ExecuteScalar(dsql);

                                // Update Start 15/03/2016 Add 'Discount' column (delete discount records)
                                string rsql = "DELETE FROM receipt_gls WHERE receipt_gl_gl_id =" + id;
                                o.ExecuteScalar(rsql);

                                string tsql = "DELETE FROM gl_transactions WHERE gl_transaction_ref='" + gRef + "'";
                                o.ExecuteScalar(tsql);
                                // Update End 15/03/2016 Add 'Discount' column

                                AdFunction.GL_Delete(paidid, o);
                                o.Commit(); // Update End 15/03/2016 Fix bug

                                im.UpdateAllocation();
                            }
                            catch (Exception ex)
                            {

                                o.Rollback();
                                throw ex;
                            }
                        }
                    }
                }

                // Add 23/05/2016
                // Offer discount
     //           if ("true".Equals(HiddenOfferDiscount.Value))  

                

       //         if (body.BodycorpDiscount)
       //         {
       //             processDiscount(im, pageList);
       //         }

                for (int i = 0; i < pageList.Length; i++)
                {
                    bool exist = false;
                    string id = pageList[i].GlTransactionId;
                    string paid = pageList[i].GlTransactionPaid;
                    string gRef = pageList[i].GLTransactionRef;
                    string type = pageList[i].GLTransactionTypeId;

                    for (int j = 0; j < oldGLTemp.GLTempList.Count; j++)
                    {
                        if (pageList[i].GlTransactionId.Equals(oldGLTemp.GLTempList[j].GlTransactionId))
                        {
                            exist = true;
                        }
                    }

                    //test what the discoutDT is in session
                    DataTable dt = (DataTable)Session["DiscountDT"];

                    if (!exist)
                    {//INSERT
                        if (type.Equals("Invoice"))
                        {
                            InvoiceMaster invoice = new InvoiceMaster(constr);
                            invoice.LoadData(Convert.ToInt32(id));
                            if (body.BodycorpDiscount)
                            {
                                InsertDiscount(gRef);
                            }
                            Allocation.Allocate(im, invoice, Convert.ToDecimal(paid));
                        }
                        if (type.Equals("Receipt"))
                        {
                            Receipt r = new Receipt(constr);
                            r.LoadData(Convert.ToInt32(id));
                            if (body.BodycorpDiscount)
                            {
                                InsertDiscount(gRef);
                            }
                            Allocation.Allocate(im, r, Convert.ToDecimal(paid));
                        }
                        if (type.Equals("Journal"))
                        {
                            Odbc o = new Odbc(AdFunction.conn);

                            string unit_id = null;
                            if (im.InvoiceMasterUnitId == null)
                            {
                                unit_id = "null";
                            }
                            else
                            {
                                unit_id = im.InvoiceMasterUnitId.ToString();
                            }

                            string glinsert = AdFunction.GLInsert("", "", "3", "3", gRef, AdFunction.GENERALDEBTOR_ChartID, unit_id, "", paid, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"), "0", "0");
                            o.ExecuteScalar(glinsert);
                            string gid = AdFunction.GetInsertID(o);
                            o.ExecuteScalar(AdFunction.GLSInsert(id, gid));
                            Hashtable receipt_gl_items = new Hashtable();
                            receipt_gl_items.Add("invoice_gl_invoice_id", iid);
                            receipt_gl_items.Add("invoice_gl_gl_id", gid);
                            InvoiceGl receiptgl = new InvoiceGl(constr);
                            receiptgl.Add(receipt_gl_items);
                            Hashtable receipt_gl_items2 = new Hashtable();
                            receipt_gl_items2.Add("invoice_gl_invoice_id", iid);
                            receipt_gl_items2.Add("invoice_gl_gl_id", id);
                            receiptgl.Add(receipt_gl_items2);
                            im.UpdateAllocation();
                        }
                    }
                    else
                    {//Update
                        if (type.Equals("Invoice"))
                        {
                            InvoiceMaster invoice = new InvoiceMaster(constr);
                            invoice.LoadData(Convert.ToInt32(id));
                            if (body.BodycorpDiscount)
                            {
                                UpDateDiscount(gRef);
                            }
                            Allocation.Allocate(im, invoice, Convert.ToDecimal(paid));
                        }
                        if (type.Equals("Receipt"))
                        {
                            Receipt r = new Receipt(constr);
                            r.LoadData(Convert.ToInt32(id));
                            if (body.BodycorpDiscount)
                            {
                                //dyyr
                                UpDateDiscount(gRef);
                            }
                            Allocation.Allocate(im, r, Convert.ToDecimal(paid));
                        }
                        if (type.Equals("Journal"))
                        {
                            //modified by dyyr @2016 07 23
                            //string sql = "update gl_transactions set gl_transaction_net=" + paid + " where gl_transaction_id=" + AdFunction.JournalPaid_INV_GLID(iid, gRef);
                            string journalId = AdFunction.JournalPaid_INV_GLID(iid, gRef);
                            if (journalId != "")
                            {
                                string sql = "update gl_transactions set gl_transaction_net=" + paid + " where gl_transaction_id=" + journalId;
                                Odbc o = new Odbc(AdFunction.conn);
                                o.ExecuteScalar(sql);
                                im.UpdateAllocation();
                            }
                            //modify end
                        }
                    }
                }

                Response.Redirect("invoicemaster.aspx", false);

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/invoicemaster.aspx", false);
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            ImageButtonSave_Click(null, null);
        }

        /// <summary>
        /// Add journal transaction records and invoive_gls record for discount
        /// </summary>
        /// <param name="im">invoice master</param>
        /// <param name="transactionList">current transaction list</param>
        private void processDiscount(InvoiceMaster im, GlTransactionTemp[] transactionList)
        {
            // DB connection string
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            Odbc mydb = null;
            string receipt_id = null;

            try
            {
                mydb = new Odbc(constr);

                // Load DB value of Discount
                Sapp.SMS.System sys = new Sapp.SMS.System(AdFunction.conn);
                ChartMaster cm = new ChartMaster(AdFunction.conn);

                sys.LoadData("GENERALLEVY");
                cm.LoadData(sys.SystemValue);
                string chart_id_general_Levy = cm.ChartMasterId.ToString();

                sys.LoadData("DISCOUNTCHARCODE");
                cm.LoadData(sys.SystemValue);
                string chart_id_discount = cm.ChartMasterId.ToString();

                sys.LoadData("GST Output");
                cm.LoadData(sys.SystemValue);
                string chart_id_gst = cm.ChartMasterId.ToString();

                sys.LoadData("GENERALDEBTOR");
                cm.LoadData(sys.SystemValue);
                string chart_id_debtor = cm.ChartMasterId.ToString();

                sys.LoadData("GST");
                decimal gst_rate = decimal.Parse(sys.SystemValue);

                // Add 7/6/2016
                Bodycorp body = new Bodycorp(AdFunction.conn);
                body.LoadData(Convert.ToInt32(Request.Cookies["bodycorpid"].Value));

                decimal allocated = 0;
                GlTransactionTemp discount_transaction = null;
                DataTable dt = null;

                for (int i = 0; i < transactionList.Length; i++)
                {
                    // caculate paid
                    decimal paid = 0;
                    decimal.TryParse(transactionList[i].GlTransactionPaid, out paid);
                    allocated = allocated + paid;

                    if ("Journal".Equals(transactionList[i].GLTransactionTypeId)) 
                    {
                        // Check the exist of discount journal
                        string discount_sql = " SELECT * FROM gl_transactions WHERE gl_transaction_ref = '" + transactionList[i].GLTransactionRef + "' "
                                            + "   AND gl_transaction_chart_id IN ( " + chart_id_general_Levy + ", " + chart_id_discount + " )";
                        OdbcDataReader reader = mydb.Reader(discount_sql);
                        if (reader.Read())
                        {
                            discount_transaction = transactionList[i];

                            // Get related (3) transactions
                            string gl_sql = " SELECT * FROM gl_transactions WHERE gl_transaction_ref = '" + transactionList[i].GLTransactionRef + "' ";
                            dt = mydb.ReturnTable(gl_sql, "discount_temp");
                        }
                    } else if ("Receipt".Equals(transactionList[i].GLTransactionTypeId))
                    {
                        // get last receipt id
                        receipt_id = transactionList[i].GlTransactionId;
                    }
                }

                decimal diff = im.InvoiceMasterGross - allocated;
                mydb.StartTransaction();

                if (discount_transaction != null)
                {
                    decimal old_paid = 0;
                    decimal.TryParse(discount_transaction.GlTransactionPaid, out old_paid);
                    decimal new_paid = old_paid + diff;

                    // Update 7/6/2016
                    decimal new_tax = 0m;
                    if (body.BodycorpNoGST == false)
                    {
                        new_tax = new_paid * gst_rate / (1.0m + gst_rate);
                        new_tax = AdFunction.Rounded(new_tax.ToString());
                    }
                    decimal new_net = new_paid - new_tax;

                    // Update allocated discount (gross)
                    discount_transaction.GlTransactionPaid = new_paid.ToString("f2");

                    // related update (update DB)
                    foreach (DataRow dr in dt.Rows)
                    {
                        // Proprietor Debtor Control Account
                        if (dr["gl_transaction_chart_id"].ToString().Equals(chart_id_discount) || dr["gl_transaction_chart_id"].ToString().Equals(chart_id_general_Levy))
                        {
                            string debtor_sql = " UPDATE gl_transactions SET gl_transaction_net=" + (-new_net) + " WHERE gl_transaction_id = " + dr["gl_transaction_id"].ToString();
                            mydb.ExecuteScalar(debtor_sql);
                        }

                        // GST Control Account
                        if (dr["gl_transaction_chart_id"].ToString().Equals(chart_id_gst))
                        {
                            string tax_sql = " UPDATE gl_transactions SET gl_transaction_net=" + (-new_tax) + " WHERE gl_transaction_id = " + dr["gl_transaction_id"].ToString();
                            mydb.ExecuteScalar(tax_sql);
                        }
                    }
                }
                else if (receipt_id != null)
                {
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(int.Parse(AdFunction.BodyCorpID));
                    string chart_id = null;

                    if (bodycorp.BodycorpDiscount == true)
                    {
                        chart_id = chart_id_discount;
                    }
                    else
                    {
                        chart_id = chart_id_general_Levy;
                    }

                    // Add new journal
                    string login = System.Web.HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new Sapp.General.User(Sapp.SMS.AdFunction.constr_general);
                    user.LoadData(login);

                    string journal_id = Journal.GetNextNumber();

                    // Update 7/6/2016
                    decimal new_tax = 0m;
                    if (body.BodycorpNoGST == false)
                    {
                        new_tax = diff * gst_rate / (1.0m + gst_rate);
                        new_tax = AdFunction.Rounded(new_tax.ToString());
                    }
                    decimal new_net = diff - new_tax;

                    string unit_id = null;
                    if (im.InvoiceMasterUnitId == null)
                    {
                        unit_id = "null";
                    }
                    else
                    {
                        unit_id = im.InvoiceMasterUnitId.ToString();
                    }

                    string glsqlnet = "INSERT INTO gl_transactions (gl_transaction_type_id, gl_transaction_ref, gl_transaction_ref_type_id, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_unit_id, "
                                    + "    gl_transaction_description, gl_transaction_net, gl_transaction_tax, gl_transaction_gross, "
                                    + "    gl_transaction_date, gl_transaction_createdate, gl_transaction_user_id) "
                                    + "  VALUES (6, '" + journal_id + "', 6, " + chart_id + ", " + AdFunction.BodyCorpID + ", " + unit_id + ", "
                                    + "    'Discount Offered', " + (-new_net) + ", 0, 0, "
                                    + "    '" + DateTime.Now.ToString("yyyy-MM-dd") + "', '" + DateTime.Now.ToString("yyyy-MM-dd") + "', " + user.UserId.ToString() + ")";

                    string glsqltax = "INSERT INTO gl_transactions (gl_transaction_type_id, gl_transaction_ref, gl_transaction_ref_type_id, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_unit_id, "
                                    + "    gl_transaction_description, gl_transaction_net, gl_transaction_tax, gl_transaction_gross, "
                                    + "    gl_transaction_date, gl_transaction_createdate, gl_transaction_user_id) "
                                    + "  VALUES (6, '" + journal_id + "', 6, " + chart_id_gst + ", " + AdFunction.BodyCorpID + ", " + unit_id + ", "
                                    + "    'Discount Offered', " + (-new_tax) + ", 0, 0, "
                                    + "    '" + DateTime.Now.ToString("yyyy-MM-dd") + "', '" + DateTime.Now.ToString("yyyy-MM-dd") + "', " + user.UserId.ToString() + ")";

                    string glsqlgross = "INSERT INTO gl_transactions (gl_transaction_type_id, gl_transaction_ref, gl_transaction_ref_type_id, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_unit_id, "
                                      + "    gl_transaction_description, gl_transaction_net, gl_transaction_tax, gl_transaction_gross, "
                                      + "    gl_transaction_date, gl_transaction_createdate, gl_transaction_user_id) "
                                      + "  VALUES (6, '" + journal_id + "', 6, " + chart_id_debtor + ", " + AdFunction.BodyCorpID + ", " + unit_id + ", "
                                      + "    'Discount Offered', " + (diff) + ", 0, 0, "
                                      + "    '" + DateTime.Now.ToString("yyyy-MM-dd") + "', '" + DateTime.Now.ToString("yyyy-MM-dd") + "', " + user.UserId.ToString() + ")";


                    mydb.ReturnTable(glsqlnet, "NetDT");

                    mydb.ReturnTable(glsqltax, "TaxDT");

                    mydb.ReturnTable(glsqlgross, "GrossDT");

                    string id_sql = "SELECT LAST_INSERT_ID()";
                    string gl_gross_id = Convert.ToInt32(mydb.ExecuteScalar(id_sql)).ToString();

                    string rec_gl_sql = " INSERT INTO receipt_gls (receipt_gl_receipt_id, receipt_gl_gl_id, receipt_gl_paid) VALUES   (" + receipt_id + ", " + gl_gross_id + ", 0)";
                    mydb.ExecuteScalar(rec_gl_sql);

                    //DataTable invoice = ReportDT.getTable(constr, "invoice_master");
                    string inv_gl_sql = " INSERT INTO invoice_gls (invoice_gl_invoice_id, invoice_gl_gl_id, invoice_gl_paid) VALUES (" + im.InvoiceMasterId + ", " + gl_gross_id + ", 0) ";
                    mydb.ExecuteScalar(inv_gl_sql);
                }

                mydb.Commit();
            }
            catch (Exception ex)
            {
                if (mydb != null)
                {
                    mydb.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (mydb != null)
                {
                    mydb.Close();
                }
            }
        }

     }
}