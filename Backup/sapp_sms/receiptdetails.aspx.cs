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
    public partial class receiptdetails : System.Web.UI.Page
    {
        private static string StaticReciptID = "";
        private const string TEMP_TYPE = "receiptdetails";
        private static string ReciptID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { jqGridTrans };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
        
                #endregion
                if (!IsPostBack)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                    string receipt_id = Request.QueryString["receiptid"];
                    Receipt receipt = new Receipt(constr);
                    ReciptID = receipt_id;
                    StaticReciptID = receipt_id;
                    receipt.LoadData(Convert.ToInt32(receipt_id));
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
         
                    GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
                    gltranTemps.Add(receipt.GetRelatedCInvGLJSON());
                    #endregion
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebControl Events
        protected void ImageButtonEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string receipt_id = "";
                receipt_id = Request.QueryString["receiptid"];
                Response.BufferOutput = true;
                Response.Redirect("~/receiptedit.aspx?receiptid=" + Server.UrlEncode(receipt_id) + "&mode=edit",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string receipt_id = "";
                if (Request.QueryString["receiptid"] != null) receipt_id = Request.QueryString["receiptid"];
                if (receipt_id != "")
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    Receipt receipt = new Receipt(constr);
                    receipt.LoadData(Convert.ToInt32(receipt_id));
                    receipt.Delete();
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
        }
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
        #endregion

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);
            GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
            gltranTemps.LoadData();

            DataTable dt = new DataTable("gl_temp_transactions");
            dt.Columns.Add("ID");
            dt.Columns.Add("InvoiceNum");
            dt.Columns.Add("Description");
            dt.Columns.Add("Date");
            dt.Columns.Add("DueDate");
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
                    dr["InvoiceNum"] = items["gl_transaction_ref"].ToString();
                    dr["Description"] = items["gl_transaction_description"].ToString();
                    dr["Date"] = items["gl_transaction_date"].ToString();
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
            string sqlselectstr = "SELECT `ID`, `InvoiceNum`, `Description`, `Date`, `DueDate`, `Net`, `Tax`,`Gross`, `Due`, `Paid`,`Discount` FROM (SELECT * ";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }
        #endregion

    }
}