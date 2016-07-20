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
    public partial class cpaymentallocate : System.Web.UI.Page, IPostBackEventHandler
    {
        private const string TEMP_TYPE_RELATED = "cpaymentallocate1";
        private const string TEMP_TYPE_UNPAID = "cpaymentallocate2";
        private const string TYPE_ID = "4";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { jqGridRelated, jqGridUnpaid, LabelAllocated };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion

                if (!IsPostBack)
                {

                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                    string cpayment_id = "";
                    cpayment_id = Request.QueryString["cpaymentid"];
                    CPayment cpayment = new CPayment(constr);
                    cpayment.LoadData(Convert.ToInt32(cpayment_id));
                    #region Initial Web Controls
                    LabelCpaymentID.Text = cpayment_id.ToString();

                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(cpayment.Cpayment_bodycorp_id);
                    if (!bodycorp.CheckCloseOff(cpayment.Cpayment_date))
                    {
                        throw new Exception("Payment before close date");

                    }

                    LabelBodycorp.Text = bodycorp.BodycorpCode;
                    CreditorMaster creditor = new CreditorMaster(constr);
                    creditor.LoadData(cpayment.Cpayment_creditor_id);
                    LabelCreditor.Text = creditor.CreditorMasterCode;
                    LabelReference.Text = cpayment.Cpayment_reference;

                    PaymentType paytype = new PaymentType(constr);
                    paytype.LoadData(cpayment.Cpayment_type_id);
                    LabelType.Text = paytype.PaymentTypeCode;

                    if (cpayment.Cpayment_gross > 0)
                        LabelGross.Text = cpayment.Cpayment_gross.ToString();
                    else
                        LabelGross.Text = (-cpayment.Cpayment_gross).ToString();
                    LabelDate.Text = cpayment.Cpayment_date.ToShortDateString();
                    LabelAllocated.Text = cpayment.Cpayment_allocated.ToString("0.00");
                    LabelAccountL.Text = creditor.CreditorMasterBankAc;
                    #endregion
                    #region Initial Temp File In Mysql
                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(constr_general);
                    user.LoadData(login);
                    GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                    gltranTemps.Add(cpayment.Get_CINV_Paid());
                    GlTransactionTemps gltranTemps2 = new GlTransactionTemps(user.UserId, TEMP_TYPE_UNPAID, constr_general, constr);
                    gltranTemps2.Add(cpayment.Get_CINV_UnPaid());
                    #endregion
                }

                Session["cpayment_gross"] = LabelGross.Text;
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
            dt.Columns.Add("CinvoiceNum");
            dt.Columns.Add("Description");
            dt.Columns.Add("Date");
            dt.Columns.Add("DueDate");
            dt.Columns.Add("Net");
            dt.Columns.Add("Tax");
            dt.Columns.Add("Gross");
            dt.Columns.Add("Due");
            dt.Columns.Add("Paid");
            foreach (GlTransactionTemp gltran in gltranTemps.GLTempList)
            {
                Hashtable items = gltran.GetData();
                DataRow dr = dt.NewRow();
                if (items["gl_transaction_id"].ToString()[0] != 'd')
                {
                    dr["ID"] = items["gl_transaction_id"].ToString();

                    dr["CinvoiceNum"] = items["gl_transaction_ref"].ToString();
                    dr["Description"] = items["gl_transaction_description"].ToString();
                    dr["Date"] = items["gl_transaction_date"].ToString();
                    dr["DueDate"] = items["gl_transaction_duedate"].ToString();
                    dr["Net"] = items["gl_transaction_net"].ToString();
                    dr["Tax"] = items["gl_transaction_tax"].ToString();
                    dr["Gross"] = items["gl_transaction_gross"].ToString();
                    dr["Due"] = items["gl_transaction_due"].ToString();
                    dr["Paid"] = items["gl_transaction_paid"].ToString();
                    dt.Rows.Add(dr);
                }
            }
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`, `CinvoiceNum`, `Description`, `Date`, `DueDate`, `Net`, `Tax`,`Gross`, `Due`, `Paid` FROM (SELECT *";
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
            dt.Columns.Add("CinvoiceNum");
            dt.Columns.Add("Description");
            dt.Columns.Add("Date");
            dt.Columns.Add("DueDate");
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
                    dr["CinvoiceNum"] = items["gl_transaction_ref"].ToString();
                    dr["Description"] = items["gl_transaction_description"].ToString();
                    dr["Date"] = items["gl_transaction_date"].ToString();
                    dr["DueDate"] = items["gl_transaction_duedate"].ToString();
                    dr["Net"] = items["gl_transaction_net"].ToString();
                    dr["Tax"] = items["gl_transaction_tax"].ToString();
                    dr["Gross"] = items["gl_transaction_gross"].ToString();
                    dr["Due"] = items["gl_transaction_due"].ToString();
                    dt.Rows.Add(dr);
                }
            }
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`, `CinvoiceNum`, `Description`, `Date`, `DueDate`, `Net`, `Tax`,`Gross`, `Due` FROM (SELECT *";
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
                    if (glts_related.GLTempList[i].GlTransactionId == hdata["ID"].ToString())
                    {
                        decimal ori_paid = Convert.ToDecimal(glts_related.GLTempList[i].GlTransactionPaid);
                        decimal paid = Convert.ToDecimal(hdata["Paid"]);
                        decimal due = Convert.ToDecimal(hdata["Due"]);
                        if (paid > ori_paid)
                        {
                            if ((paid - ori_paid) > due)
                                throw new Exception("error: paid is more than overdue amount!");
                        }
                        glts_related.GLTempList[i].GlTransactionPaid = hdata["Paid"].ToString();
                        glts_related.GLTempList[i].GlTransactionDue = (due - paid + ori_paid).ToString("0.00");

                        glts_related.GLTempList[i].GlTransactionId = glts_related.GLTempList[i].GlTransactionId;

                    }
                    total_paid += Convert.ToDecimal(glts_related.GLTempList[i].GlTransactionPaid);
                }
                decimal cpayment_gross = Convert.ToDecimal(HttpContext.Current.Session["cpayment_gross"]);
                if (total_paid > cpayment_gross)
                    throw new Exception("error: paid is more than total Cpayment!");
                #endregion
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

            GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
            gltranTemps.LoadData();


            foreach (GlTransactionTemp gltran in gltranTemps.GLTempList)
            {
                allocate += Convert.ToDecimal(gltran.GlTransactionPaid);
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
                            decimal recGross = decimal.Parse(LabelGross.Text);
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
                receipt_id = Request.QueryString["cpaymentid"];
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);

                CPayment receipt = new CPayment(AdFunction.conn);
                receipt.LoadData(int.Parse(receipt_id));

                ;

                //Related Temp Trans
                GlTransactionTemps glts_related = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                glts_related.LoadData();

                GlTransactionTemp[] pageList = new GlTransactionTemp[glts_related.GLTempList.Count];


                GlTransactionTemps oldGLTemp = new GlTransactionTemps(user.UserId, "Temp", constr_general, constr);
                string temp = receipt.Get_CINV_Paid();
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
                        if (type.Equals("CINV"))
                        {
                            Cinvoice invoice = new Cinvoice(constr);
                            invoice.LoadData(Convert.ToInt32(id));
                            Allocation.Allocate(receipt, invoice, Convert.ToDecimal("0"));

                        }
                        if (type.Equals("CNote"))
                        {
                            CPayment r = new CPayment(constr);
                            r.LoadData(Convert.ToInt32(id));
                            Allocation.Allocate(receipt, r, Convert.ToDecimal("0"));
                        }
                        if (type.Equals("Journal"))
                        {
                            Odbc o = new Odbc(AdFunction.conn);
                            o.StartTransaction();
                            try
                            {
                                string paidid = AdFunction.Journal_CPayment_Paid_GLID(receipt_id, gRef);
                                AdFunction.GLS_CPayment_Delete(receipt_id, gRef, id);
                                string dsql = "delete from cpayment_gls where cpayment_gl_cpayment_id=" + receipt_id + " and cpayment_gl_gl_id=" + paidid;
                                o.ExecuteScalar(dsql);
                                dsql = "delete from cpayment_gls where cpayment_gl_cpayment_id=" + receipt_id + " and cpayment_gl_gl_id=" + id;
                                o.ExecuteScalar(dsql);
                                AdFunction.GL_Delete(paidid, o);
                                o.Commit();
                                receipt.UpdateAllocation();

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
                        if (type.Equals("CINV"))
                        {
                            Cinvoice invoice = new Cinvoice(constr);
                            invoice.LoadData(Convert.ToInt32(id));
                            Allocation.Allocate(receipt, invoice, Convert.ToDecimal(paid));

                        }
                        if (type.Equals("CNote"))
                        {
                            CPayment r = new CPayment(constr);
                            r.LoadData(Convert.ToInt32(id));
                            Allocation.Allocate(receipt, r, Convert.ToDecimal(paid));
                        }
                        if (type.Equals("Journal"))
                        {
                            Odbc o = new Odbc(AdFunction.conn);
                            string glinsert = AdFunction.GLInsert("", "", "4", "4", gRef, AdFunction.GENERALCREDITOR_ChartID, "null", "Allocation", paid, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"), "0", "0");
                            o.ExecuteScalar(glinsert);
                            string gid = AdFunction.GetInsertID(o);
                            o.ExecuteScalar(AdFunction.GLSInsert(id, gid));

                            Hashtable receipt_gl_items = new Hashtable();
                            receipt_gl_items.Add("cpayment_gl_cpayment_id", receipt_id);
                            receipt_gl_items.Add("cpayment_gl_gl_id", gid);
                            CpaymentGl receiptgl = new CpaymentGl(constr);
                            receiptgl.Add(receipt_gl_items);
                            Hashtable receipt_gl_items2 = new Hashtable();
                            receipt_gl_items2.Add("cpayment_gl_cpayment_id", receipt_id);
                            receipt_gl_items2.Add("cpayment_gl_gl_id", id);
                            receiptgl.Add(receipt_gl_items2);
                            receipt.UpdateAllocation();
                        }
                    }
                    else
                    {//Update
                        if (type.Equals("CINV"))
                        {
                            Cinvoice invoice = new Cinvoice(constr);
                            invoice.LoadData(Convert.ToInt32(id));
                            Allocation.Allocate(receipt, invoice, Convert.ToDecimal(paid));

                        }
                        if (type.Equals("CNOTE"))
                        {
                            CPayment r = new CPayment(constr);
                            r.LoadData(Convert.ToInt32(id));
                            Allocation.Allocate(receipt, r, Convert.ToDecimal(paid));
                        }
                        if (type.Equals("Journal"))
                        {
                            string sql = "update gl_transactions set gl_transaction_net=" + paid + " where gl_transaction_id=" + AdFunction.Journal_CPayment_Paid_GLID(receipt_id, gRef);
                            Odbc o = new Odbc(AdFunction.conn);
                            o.ExecuteScalar(sql);
                            receipt.UpdateAllocation();

                        }
                    }
                }

                Response.Redirect("cpayments.aspx", false);

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}