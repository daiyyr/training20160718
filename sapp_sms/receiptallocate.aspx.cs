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

namespace sapp_sms
{

    public partial class receiptallocate : System.Web.UI.Page, IPostBackEventHandler
    {
        public string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        private const string TEMP_TYPE_RELATED = "receiptallocate1";
        private const string TEMP_TYPE_UNPAID = "receiptallocate2";
        private const string TYPE_ID = "3";
        private static string StaticReciptID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { jqGridRelated, jqGridUnpaid, LabelAllocated };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion

                if (!IsPostBack && Request.QueryString["receiptid"] != null)
                {

                    Session["DiscountDT"] = null;
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                    string receipt_id = "";
                    receipt_id = Request.QueryString["receiptid"].ToString();
                    if (Request.QueryString["mu"] != null)
                    {
                        Panel1.Visible = true;
                        SaveL.Visible = false;
                        ImageButtonSave.Visible = false;
                        DataSet ds = new DataSet();
                        ds.ReadXml(Server.MapPath("~/temp/MulReceipt.xml"));
                        DataTable dt = ds.Tables[0];
                        decimal total = 0;
                        decimal allocate = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            Receipt r = new Receipt(AdFunction.conn);
                            r.LoadData(int.Parse(dr["ID"].ToString()));
                            dr["Unit"] = dr["Unit"].ToString() + " - " + r.ReceiptAllocated;
                            total += r.ReceiptGross;
                            allocate += r.ReceiptAllocated;
                        }
                        if (total == allocate)
                        {
                            ImageButton2.Visible = true;
                            Label2.Visible = true;
                        }
                        DropDownList2.DataSource = dt;
                        DropDownList2.DataTextField = "Unit";
                        DropDownList2.DataValueField = "ID";
                        DropDownList2.DataBind();
                        DropDownList2.SelectedValue = receipt_id;
                    }


                    StaticReciptID = receipt_id;
                    Receipt receipt = new Receipt(constr);
                    receipt.LoadData(Convert.ToInt32(receipt_id));


                    #region Check
                    {
                        Bodycorp b = new Bodycorp(constr);
                        b.LoadData(receipt.ReceiptBodycorpId);
                        if (!b.CheckCloseOff(receipt.ReceiptDate))
                        {
                            SaveL.Text = "Receipt before close date";
                            ImageButtonSave.Visible = false;
                            throw new Exception("Receipt before close date");

                        }
                        if (receipt.ReceiptGross < 0)
                        {
                            Response.Redirect("refundallocate.aspx?receiptid=" + receipt_id, false);
                        }
                    }


                    #endregion

                    #region Initial Web Controls
                    LabelRef.Text = receipt.ReceiptRef;
                    LabelNote.Text = receipt.ReceiptNotes;
                    DebtorMaster debtor = new DebtorMaster(constr);
                    debtor.LoadData(receipt.ReceiptDebtorId);
                    LabelDebtor.Text = debtor.DebtorMasterName;
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(receipt.ReceiptBodycorpId);
                    LabelBodycorp.Text = bodycorp.BodycorpCode;
                    if (receipt.ReceiptUnitId != null)
                    {
                        UnitMaster unit = new UnitMaster(constr);
                        unit.LoadData(receipt.ReceiptUnitId.Value);
                        LabelUnit.Text = unit.UnitMasterCode;
                    }
                    else
                        LabelUnit.Text = "";
                    PaymentType paymenttype = new PaymentType(constr);
                    paymenttype.LoadData(receipt.ReceiptPaymentTypeId);
                    LabelPaymentType.Text = paymenttype.PaymentTypeCode;
                    Account account = new Account(constr);
                    account.LoadData(receipt.ReceiptAccountId);
                    LabelAccount.Text = account.AccountCode;
                    LabelGross.Text = receipt.ReceiptGross.ToString("0.00");
                    LabelDate.Text = DBSafeUtils.DBDateToStr(receipt.ReceiptDate);
                    LabelAllocated.Text = receipt.ReceiptAllocated.ToString("0.00");
                    #endregion
                    #region Initial Temp File In Mysql
                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(constr_general);
                    user.LoadData(login);
                    GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                    gltranTemps.Add(receipt.GetRelatedCInvGLJSON());
                    GlTransactionTemps gltranTemps2 = new GlTransactionTemps(user.UserId, TEMP_TYPE_UNPAID, constr_general, constr);
                    gltranTemps2.Add(receipt.GetUnpaidCInvGLJSON());
                    Session["receipt_gross"] = LabelGross.Text;

                    #endregion
                }
                LabelAllocated.Text = GetAllocate();
            }
            catch (Exception ex)
            {

                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region Discount
        public static void SettingDiscount(string discount, string invcode)
        {
            DataTable dt = new DataTable();
            if (discount.Equals(""))
                discount = "0";
            if (HttpContext.Current.Session["DiscountDT"] == null)
                GetDiscountDT(StaticReciptID);
            if (HttpContext.Current.Session["DiscountDT"] != null)
            {
                dt = (DataTable)HttpContext.Current.Session["DiscountDT"];
                bool checkupdate = false;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["invoice_master_num"].ToString().Equals(invcode))
                    {
                        dr["gl_transaction_net"] = discount;
                        checkupdate = true;
                    }
                }
                if (!checkupdate)
                {
                    if (!invcode.Contains("JNL"))
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        DataRow dr = dt.NewRow();
                        dr["invoice_master_num"] = invcode;
                        DataTable invDT = ReportDT.getTable(constr, "invoice_master");
                        string invID = ReportDT.GetDataByColumn(invDT, "invoice_master_num", invcode, "invoice_master_id");
                        dr["invoice_master_id"] = invID;
                        dr["gl_transaction_net"] = discount;
                        dr["receipt_gl_receipt_id"] = StaticReciptID;
                        dt.Rows.Add(dr);
                    }
                }
            }


            HttpContext.Current.Session["DiscountDT"] = dt;
        }
        public static string GetDiscount(string invcode)
        {
            string r = "";
            DataTable dt = new DataTable();
            if (HttpContext.Current.Session["DiscountDT"] != null)
            {
                dt = (DataTable)HttpContext.Current.Session["DiscountDT"];
            }
            else
            {
                GetDiscountDT(StaticReciptID);
                dt = (DataTable)HttpContext.Current.Session["DiscountDT"];
            }
            r = ReportDT.GetDataByColumn(dt, "invoice_master_num", invcode, "gl_transaction_net");
            return r;
        }
        public static void GetDiscountDT(string reciptID)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string sql = "SELECT   gl_transactions.gl_transaction_id, gl_transactions.gl_transaction_type_id, gl_transactions.gl_transaction_net,                  receipt_gls.receipt_gl_receipt_id, invoice_master.invoice_master_id, invoice_master.invoice_master_num FROM      gl_transactions, invoice_gls, receipt_gls, invoice_master WHERE   gl_transactions.gl_transaction_id = invoice_gls.invoice_gl_gl_id AND                  invoice_gls.invoice_gl_gl_id = receipt_gls.receipt_gl_gl_id AND                  invoice_gls.invoice_gl_invoice_id = invoice_master.invoice_master_id AND (gl_transactions.gl_transaction_type_id = 6)                  AND (receipt_gls.receipt_gl_receipt_id = " + reciptID + ")";
            Odbc o = new Odbc(constr);
            DataTable dt = o.ReturnTable(sql, "DiscountDT");
            HttpContext.Current.Session["DiscountDT"] = dt;
        }

        public void UpDateDiscount(string Invcode)
        {
            Odbc o = new Odbc(constr);
            DataTable dt = (DataTable)Session["DiscountDT"];
            string grossid = ReportDT.GetDataByColumn(dt, "invoice_master_num", Invcode, "gl_transaction_id");
            if (!grossid.Equals(""))
            {
                int Grossglid = int.Parse(grossid);
                string invID = ReportDT.GetDataByColumn(dt, "invoice_master_num", Invcode, "invoice_master_id");
                string g = ReportDT.GetDataByColumn(dt, "invoice_master_num", Invcode, "gl_transaction_net");
                if (g.Equals(""))
                    g = "0";
                if (g.Equals("0") || g.Equals("0.00"))
                {
                    DeleteDiscount(Invcode);
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
                    string receipt_id = Request.QueryString["receiptid"];
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
                string invid = ReportDT.GetDataByColumn(dt, "invoice_master_num", Invcode, "invoice_master_id");
                InsertDiscount(invid);
            }
        }
        public void InsertDiscount(string invID)
        {
            if (Session["DiscountDT"] != null)
            {
                if (!invID.Equals(""))
                {
                    // Add 7/6/2016 Load rate of Discount
                    Sapp.SMS.System sys = new Sapp.SMS.System(AdFunction.conn);
                    sys.LoadData("GST");
                    decimal gst_rate = decimal.Parse(sys.SystemValue);

                    Bodycorp body = new Bodycorp(AdFunction.conn);
                    body.LoadData(Convert.ToInt32(Request.Cookies["bodycorpid"].Value));

                    DataTable dtDiscount = (DataTable)Session["DiscountDT"];
                    DataTable dt = ReportDT.FilterDT(dtDiscount, "invoice_master_id=" + invID);
                    foreach (DataRow dr in dt.Rows)
                    {
                        string discountValue = dr["gl_transaction_net"].ToString();
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
                            string receipt_id = Request.QueryString["receiptid"];
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
                            DataTable reglDT = ReportDT.getTable(constr, "gl_transactions");
                            reglDT = ReportDT.FilterDT(reglDT, "gl_transaction_type_id=6 and gl_transaction_chart_id=" + disid);

                            InsertDiscount(reference, receipt.ReceiptBodycorpId, net, tax, gross, taxtid, did, disid, invID, receipt_id);
                        }
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
                string unitid = ReportDT.GetDataByColumn(recepitDT, "receipt_id", Request.QueryString["receiptid"].ToString(), "receipt_unit_id");
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
        public void DeleteDiscount(string Invcode)
        {
            Odbc o = new Odbc(constr);
            DataTable dt = (DataTable)Session["DiscountDT"];
            string gid = ReportDT.GetDataByColumn(dt, "invoice_master_num", Invcode, "gl_transaction_id");
            if (!gid.Equals(""))
            {
                int glid = int.Parse(gid);
                string glgrossid = glid.ToString();
                string referencr = ReportDT.GetDataByColumn(ReportDT.getTable(AdFunction.conn, "gl_transactions"), "gl_transaction_id", gid, "gl_transaction_ref");
                string rsql = "delete FROM  receipt_gls where receipt_gl_gl_id =" + glgrossid;
                string isql = "delete FROM  invoice_gls where invoice_gl_gl_id =" + glgrossid;
                string tsql = "delete FROM  gl_transactions where gl_transaction_ref='" + referencr + "'";
                o.ExecuteScalar(rsql);
                o.ExecuteScalar(isql);
                o.ExecuteScalar(tsql);
            }
        }
        #endregion
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
            dt.Columns.Add("Discount");
            foreach (GlTransactionTemp gltran in gltranTemps.GLTempList)
            {
                Hashtable items = gltran.GetData();
                DataRow dr = dt.NewRow();
                if (items["gl_transaction_id"].ToString()[0] != 'd')
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
                    dr["Due"] = items["gl_transaction_due"].ToString();
                    dr["Paid"] = items["gl_transaction_paid"].ToString();
                    dr["Discount"] = GetDiscount(dr["InvoiceNum"].ToString());
                    dt.Rows.Add(dr);
                }
            }
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`,`RefType`, `InvoiceNum`, `Description`, `Date`, `DueDate`, `Net`, `Tax`,`Gross`, `Due`, `Paid`,`Discount` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }
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
                if (items["gl_transaction_id"].ToString()[0] != 'd')
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
                        string d = hdata["Discount"].ToString();
                        if (d.Equals(""))
                            d = "0";

                        decimal paid = Convert.ToDecimal(hdata["Paid"]);
                        if (paid < 0)
                            throw new Exception("Paid must over 0!");
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
                        total_paid = total_paid - ori_paid + paid;
                        decimal.TryParse(GetDiscount(hdata["InvoiceNum"].ToString()), out odiscount);
                        if (paid == 0)
                        {
                            throw new Exception("error: paid can not be 0");
                        }
                        if (due - paid + ori_paid + odiscount - discount < 0)
                        {
                            throw new Exception("error: paid and discount is more than due!");
                        }

                        if (glts_related.GLTempList[i].GlTransactionId == hdata["ID"].ToString())
                        {
                            glts_related.GLTempList[i].GlTransactionPaid = hdata["Paid"].ToString();
                            glts_related.GLTempList[i].GlTransactionDue = (due - paid + ori_paid + odiscount - discount).ToString("0.00");
                            //glts_related.GLTempList[i].GlTransactionDue = (oldgross - paid - discount).ToString("0.00");
                            glts_related.GLTempList[i].GlTransactionId = glts_related.GLTempList[i].GlTransactionId;
                        }

                        SettingDiscount(hdata["Discount"].ToString(), hdata["InvoiceNum"].ToString());
                    }

                #endregion
                }
                decimal receipt_gross = Convert.ToDecimal(HttpContext.Current.Session["receipt_gross"]);
                if (total_paid > receipt_gross)
                    throw new Exception("error: paid is more than total receipt Gross!");

            }
            glts_related.UpdateTemp();
            //SettingDiscount(hdata["Discount"].ToString(), hdata["InvoiceNum"].ToString());

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

            GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
            gltranTemps.LoadData();

            foreach (GlTransactionTemp gltran in gltranTemps.GLTempList)
            {
                decimal temp = 0;
                decimal.TryParse(gltran.GlTransactionPaid, out temp);
                allocate += temp;
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

                            decimal due = 0;
                            decimal.TryParse(glitems.GlTransactionDue, out due);
                            string receipt_id = Request.QueryString["receiptid"].ToString();
                            Receipt receipt = new Receipt(constr);
                            receipt.LoadData(Convert.ToInt32(receipt_id));
                            decimal recGross = receipt.ReceiptGross;
                            decimal recAllocate = 0;
                            decimal.TryParse(GetAllocate(), out recAllocate);
                            decimal paid = 0;
                            if ((recGross - recAllocate) >= due)
                                paid = due;
                            else
                                paid = (recGross - recAllocate);
                            glitems.GlTransactionPaid = paid.ToString("f2");
                            glitems.GlTransactionDue = (due - paid).ToString("f2");
                            glts_related.GLTempList.Add(glitems); //Remove from temp data
                            glts_unpaid.GLTempList.RemoveAt(i);
                            glts_related.UpdateTemp();
                            glts_unpaid.UpdateTemp();
                        }
                    }
                }
                glts_related.UpdateTemp();
                glts_unpaid.UpdateTemp();
                LabelAllocated.Text = GetAllocate();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
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

                    }
                }
                glts_related.UpdateTemp();
                glts_unpaid.UpdateTemp();
                LabelAllocated.Text = GetAllocate();


            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        protected void ImageButtonSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string receipt_id = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                receipt_id = Request.QueryString["receiptid"];
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);

                Receipt receipt = new Receipt(AdFunction.conn);
                receipt.LoadData(int.Parse(receipt_id));

                ;

                //Related Temp Trans
                GlTransactionTemps glts_related = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                glts_related.LoadData();

                GlTransactionTemp[] pageList = new GlTransactionTemp[glts_related.GLTempList.Count];


                GlTransactionTemps oldGLTemp = new GlTransactionTemps(user.UserId, "Temp", constr_general, constr);
                string temp = receipt.GetRelatedCInvGLJSON();
                oldGLTemp.Add(temp);
                oldGLTemp.LoadData();

                glts_related.GLTempList.CopyTo(pageList);

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
                            DeleteDiscount(invoice.InvoiceMasterNum);
                            Allocation.Allocate(receipt, invoice, Convert.ToDecimal("0"));

                        }
                        if (type.Equals("Refund"))
                        {
                            Receipt r = new Receipt(constr);
                            r.LoadData(Convert.ToInt32(id));
                            Allocation.Allocate(receipt, r, Convert.ToDecimal("0"));
                        }
                        if (type.Equals("Journal"))
                        {
                            Odbc o = new Odbc(AdFunction.conn);
                            o.StartTransaction();
                            try
                            {
                                string paidid = AdFunction.Journal_Receipt_Paid_GLID(receipt_id, gRef);
                                AdFunction.GLS_Receipt_Delete(receipt_id, gRef, id);
                                string dsql = "delete from receipt_gls where receipt_gl_receipt_id=" + receipt_id + " and receipt_gl_gl_id=" + paidid;
                                o.ExecuteScalar(dsql);
                                dsql = "delete from receipt_gls where receipt_gl_receipt_id=" + receipt_id + " and receipt_gl_gl_id=" + id;
                                o.ExecuteScalar(dsql);
                                AdFunction.GL_Delete(paidid, o);
                                receipt.UpdateAllocated();

                            }
                            catch (Exception ex)
                            {

                                o.Rollback();
                                throw ex;
                            }
                        }
                    }
                }

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

                    if (!exist)
                    {//INSERT
                        if (type.Equals("Invoice"))
                        {
                            InvoiceMaster invoice = new InvoiceMaster(constr);
                            invoice.LoadData(Convert.ToInt32(id));
                            InsertDiscount(id);
                            Allocation.Allocate(receipt, invoice, Convert.ToDecimal(paid));
                        }
                        if (type.Equals("Refund"))
                        {
                            Receipt r = new Receipt(constr);
                            r.LoadData(Convert.ToInt32(id));
                            Allocation.Allocate(receipt, r, Convert.ToDecimal(paid));
                        }
                        if (type.Equals("Journal"))
                        {
                            Odbc o = new Odbc(AdFunction.conn);
                            string glinsert = AdFunction.GLInsert("", "", "3", "3", gRef, AdFunction.GENERALDEBTOR_ChartID, receipt.ReceiptUnitId.ToString(), "", paid, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"), "0", "0");
                            o.ExecuteScalar(glinsert);
                            string gid = AdFunction.GetInsertID(o);
                            o.ExecuteScalar(AdFunction.GLSInsert(id, gid));
                            Hashtable receipt_gl_items = new Hashtable();
                            receipt_gl_items.Add("receipt_gl_receipt_id", receipt_id);
                            receipt_gl_items.Add("receipt_gl_gl_id", gid);
                            ReceiptGl receiptgl = new ReceiptGl(constr);
                            receiptgl.Add(receipt_gl_items);
                            Hashtable receipt_gl_items2 = new Hashtable();
                            receipt_gl_items2.Add("receipt_gl_receipt_id", receipt_id);
                            receipt_gl_items2.Add("receipt_gl_gl_id", id);
                            receiptgl.Add(receipt_gl_items2);
                            receipt.UpdateAllocated();
                        }
                    }
                    else
                    {//Update
                        if (type.Equals("Invoice"))
                        {
                            InvoiceMaster invoice = new InvoiceMaster(constr);
                            invoice.LoadData(Convert.ToInt32(id));
                            UpDateDiscount(invoice.InvoiceMasterNum);
                            Allocation.Allocate(receipt, invoice, Convert.ToDecimal(paid));

                        }
                        if (type.Equals("Refund"))
                        {
                            Receipt r = new Receipt(constr);
                            r.LoadData(Convert.ToInt32(id));
                            Allocation.Allocate(receipt, r, Convert.ToDecimal(paid));
                        }
                        if (type.Equals("Journal"))
                        {
                            string sql = "update gl_transactions set gl_transaction_net=" + paid + " where gl_transaction_id=" + AdFunction.Journal_Receipt_Paid_GLID(receipt_id, gRef);
                            Odbc o = new Odbc(AdFunction.conn);
                            o.ExecuteScalar(sql);
                            receipt.UpdateAllocated();

                        }
                    }
                }

                if (Request.QueryString["mu"] == null)
                    Response.Redirect("receipts.aspx", false);
                else
                {
                    int select = DropDownList2.SelectedIndex;
                    if (select + 1 < DropDownList2.Items.Count)
                        DropDownList2.SelectedIndex += 1;
                    Response.Redirect("receiptallocate.aspx?mu=yes&receiptid=" + DropDownList2.SelectedValue, false);
                }
            } 
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/receipts.aspx", false);
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            ImageButtonSave_Click(null, null);
        }

        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string id = DropDownList2.SelectedValue;
            Response.Redirect("~/receiptallocate.aspx?mu=yes&receiptid=" + id, false);
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            System.IO.File.Delete(Server.MapPath("~/temp/MulReceipt.xml"));
            Response.Redirect("~/receipts.aspx");
        }
    }
}