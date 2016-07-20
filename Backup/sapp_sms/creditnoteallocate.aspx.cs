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

    public partial class creditnoteallocate : System.Web.UI.Page, IPostBackEventHandler
    {
        public string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        private const string TEMP_TYPE_RELATED = "CNOTEPaid";
        private const string TEMP_TYPE_UNPAID = "CNOTEUnPaid";
        private const string TYPE_ID = "3";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { jqGridRelated, jqGridUnpaid, LabelAllocated };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion

                if (!IsPostBack && Request.QueryString["id"] != null)
                {
                    string id = "";
                    id = Request.QueryString["id"].ToString();

                    #region Initial Web Controls


                    InvoiceMaster im = new InvoiceMaster(AdFunction.conn);
                    im.LoadData(Convert.ToInt32(id));

                    #region Check
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
                    LabelGross.Text = (-im.InvoiceMasterGross).ToString("f2");
                    LabelDate.Text = DBSafeUtils.DBDateToStr(im.InvoiceMasterDate);
                    LabelAllocated.Text = im.InvoiceMasterPaid.ToString("0.00");
                    LabelBodycorp.Text = bodycorp.BodycorpCode;

                    #endregion

                    #region Initial Temp File In Mysql
                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(AdFunction.constr_general);
                    user.LoadData(login);
                    GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, AdFunction.constr_general, constr);
                    gltranTemps.Add(im.Get_CNOTE_Related_JSON());
                    GlTransactionTemps gltranTemps2 = new GlTransactionTemps(user.UserId, TEMP_TYPE_UNPAID, AdFunction.constr_general, constr);
                    gltranTemps2.Add(im.Get_CNOTE_Unpaid_JSON());

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
                    //dr["Discount"] = GetDiscount(dr["InvoiceNum"].ToString());
                    dt.Rows.Add(dr);
                }
            }
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`,`RefType`, `InvoiceNum`, `Description`, `Date`, `DueDate`, `Net`, `Tax`,`Gross`, `Due`, `Paid` FROM (SELECT *";
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

                        decimal paid = Convert.ToDecimal(hdata["Paid"]);
                        if (paid == 0)
                            throw new Exception("Paid error");

                        decimal due = 0;
                        decimal.TryParse(hdata["Due"].ToString(), out due);
                        decimal olddue = 0;
                        decimal.TryParse(glts_related.GLTempList[i].GlTransactionDue, out olddue);
                        decimal oldgross = 0;
                        decimal.TryParse(glts_related.GLTempList[i].GlTransactionGross, out oldgross);
                        decimal odiscount = 0;
                        total_paid = total_paid - ori_paid + paid;

                        if (paid == 0)
                        {
                            throw new Exception("error: paid can not be 0");
                        }
                        if (due - paid + ori_paid + odiscount < 0)
                        {
                            throw new Exception("error: paid and discount is more than due!");
                        }

                        if (glts_related.GLTempList[i].GlTransactionId == hdata["ID"].ToString())
                        {
                            glts_related.GLTempList[i].GlTransactionPaid = hdata["Paid"].ToString();
                            glts_related.GLTempList[i].GlTransactionDue = (due - paid + ori_paid + odiscount).ToString("0.00");
                            glts_related.GLTempList[i].GlTransactionId = glts_related.GLTempList[i].GlTransactionId;
                        }

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
                string iid = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                iid = Request.QueryString["id"];
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);

                InvoiceMaster im = new InvoiceMaster(AdFunction.conn);
                im.LoadData(int.Parse(iid));

                ;

                //Related Temp Trans
                GlTransactionTemps glts_related = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                glts_related.LoadData();

                GlTransactionTemp[] pageList = new GlTransactionTemp[glts_related.GLTempList.Count];


                GlTransactionTemps oldGLTemp = new GlTransactionTemps(user.UserId, "Temp", constr_general, constr);
                string temp = im.Get_CNOTE_Related_JSON();
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
                            Allocation.Allocate(im, invoice, Convert.ToDecimal("0"));
                        }
                        if (type.Equals("Receipt"))
                        {
                            Receipt r = new Receipt(constr);
                            r.LoadData(Convert.ToInt32(id));
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

                                AdFunction.GL_Delete(paidid, o);
                                o.Commit();
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
                            Allocation.Allocate(im, invoice, Convert.ToDecimal(paid));
                        }
                        if (type.Equals("Receipt"))
                        {
                            Receipt r = new Receipt(constr);
                            r.LoadData(Convert.ToInt32(id));
                            Allocation.Allocate(im, r, Convert.ToDecimal(paid));
                        }
                        if (type.Equals("Journal"))
                        {
                            Odbc o = new Odbc(AdFunction.conn);
                            string glinsert = AdFunction.GLInsert("", "", "3", "3", gRef, AdFunction.GENERALDEBTOR_ChartID, im.InvoiceMasterUnitId.Value.ToString(), "", paid, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"), "0", "0");
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
                            Allocation.Allocate(im, invoice, Convert.ToDecimal(paid));
                        }
                        if (type.Equals("Receipt"))
                        {
                            Receipt r = new Receipt(constr);
                            r.LoadData(Convert.ToInt32(id));
                            Allocation.Allocate(im, r, Convert.ToDecimal(paid));
                        }
                        if (type.Equals("Journal"))
                        {
                            string sql = "update gl_transactions set gl_transaction_net=" + paid + " where gl_transaction_id=" + AdFunction.JournalPaid_INV_GLID(iid, gRef);
                            Odbc o = new Odbc(AdFunction.conn);
                            o.ExecuteScalar(sql);
                            im.UpdateAllocation();

                        }
                    }
                }

                Response.Redirect("creditnotemaster.aspx", false);

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


    }
}