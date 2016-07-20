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
    public partial class ccrefundlocate : System.Web.UI.Page, IPostBackEventHandler
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


                    LabelGross.Text = (-cpayment.Cpayment_gross).ToString();
                    LabelDate.Text = cpayment.Cpayment_date.ToShortDateString();
                    LabelAllocated.Text = (-cpayment.Cpayment_allocated).ToString("0.00");
                    #endregion
                    #region Initial Temp File In Mysql
                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(constr_general);
                    user.LoadData(login);
                    GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                    //gltranTemps.Add(cpayment.CGetRelatedCInvGLJSON());
                    GlTransactionTemps gltranTemps2 = new GlTransactionTemps(user.UserId, TEMP_TYPE_UNPAID, constr_general, constr);
                    //gltranTemps2.Add(cpayment.CGetUnpaidCInvGLJSON());
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
                    dr["Net"] = AdFunction.Rounded(items["gl_transaction_net"].ToString());
                    dr["Tax"] = AdFunction.Rounded(items["gl_transaction_tax"].ToString());
                    dr["Gross"] = AdFunction.Rounded(items["gl_transaction_gross"].ToString());
                    dr["Due"] = AdFunction.Rounded(items["gl_transaction_due"].ToString());
                    dr["Paid"] = AdFunction.Rounded(items["gl_transaction_paid"].ToString());
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
                        if ((glts_related.GLTempList[i].GlTransactionId[0] != 'i') && (glts_related.GLTempList[i].GlTransactionId[0] != 'u'))
                        {
                            glts_related.GLTempList[i].GlTransactionId = "u" + glts_related.GLTempList[i].GlTransactionId;
                        }
                    }
                    total_paid += Convert.ToDecimal(glts_related.GLTempList[i].GlTransactionPaid);
                }
                decimal cpayment_gross = Convert.ToDecimal(HttpContext.Current.Session["cpayment_gross"]);
                if (total_paid > cpayment_gross)
                    throw new Exception("error: paid is more than total Cpayment!");
                #endregion
            }
            glts_related.UpdateTemp();
            GetAllocate();
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
                string[] invidList = args[1].Split(',');
                decimal cp_gross = Convert.ToDecimal(LabelGross.Text);
                decimal cp_allocated = Convert.ToDecimal(LabelAllocated.Text);
                decimal cp_avail = cp_gross - cp_allocated;
                foreach (string inv_id in invidList)
                {
                    Cinvoice cinvoice = new Cinvoice(constr);
                    cinvoice.LoadData(Convert.ToInt32(inv_id));
                    Hashtable glitems = new Hashtable();
                    glitems.Add("gl_transaction_id", "i" + inv_id);
                    glitems.Add("gl_transaction_ref_type_id", "4");
                    glitems.Add("gl_transaction_ref", JSON.StrToDQuoteJson(cinvoice.CinvoiceNum));
                    glitems.Add("gl_transaction_description", JSON.StrToDQuoteJson(cinvoice.CinvoiceDescription));
                    glitems.Add("gl_transaction_date", DBSafeUtils.DBDateToStr(cinvoice.CinvoiceDate));
                    glitems.Add("gl_transaction_duedate", DBSafeUtils.DBDateToStr(cinvoice.CinvoiceDue));
                    glitems.Add("gl_transaction_net", (-cinvoice.CinvoiceNet).ToString("0.00"));
                    glitems.Add("gl_transaction_tax", (-cinvoice.CinvoiceTax).ToString("0.00"));
                    glitems.Add("gl_transaction_gross", (-cinvoice.CinvoiceGross).ToString("0.00"));
                    decimal paid = -cinvoice.CinvoicePaid;
                    decimal gross = -cinvoice.CinvoiceGross;
                    decimal due = gross - paid;

                    if (cp_avail >= due)
                    {
                        glitems.Add("gl_transaction_paid", due.ToString("0.00"));
                        cp_avail -= due;
                        due = 0;
                    }
                    else
                    {
                        glitems.Add("gl_transaction_paid", cp_avail.ToString("0.00"));
                        due -= cp_avail;
                        cp_avail = 0;
                    }
                    glitems.Add("gl_transaction_due", due.ToString("0.00"));
                    #region Add Gl tran to Related Temps
                    GlTransactionTemp glt = new GlTransactionTemp(constr);
                    glt.LoadData(glitems);
                    glts_related.GLTempList.Add(glt);
                    glts_related.UpdateTemp();
                    #endregion
                    #region Remove Gl tran from Unpaid Temps
                    for (int i = 0; i < glts_unpaid.GLTempList.Count; i++)
                    {
                        if (glts_unpaid.GLTempList[i].GlTransactionId == inv_id)
                        {
                            glts_unpaid.GLTempList.RemoveAt(i);
                            break;
                        }
                    }
                    glts_unpaid.UpdateTemp();
                    #endregion
                }
                LabelAllocated.Text = (cp_gross - cp_avail).ToString("0.00");
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
                //Related Temp Trans
                GlTransactionTemps glts_related = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                glts_related.LoadData();
                //Unpaid Temp Trans
                GlTransactionTemps glts_unpaid = new GlTransactionTemps(user.UserId, TEMP_TYPE_UNPAID, constr_general, constr);
                glts_unpaid.LoadData();

                decimal cp_gross = Convert.ToDecimal(LabelGross.Text);
                decimal cp_allocated = Convert.ToDecimal(LabelAllocated.Text);
                decimal cp_avail = cp_gross - cp_allocated;
                decimal gl_due = 0;
                decimal gl_paid = 0;

                string glt_id = args[1];
                string gl_id = "";
                Cinvoice cinvoice = new Cinvoice(constr);
                CinvoiceGl cinvoicegl = new CinvoiceGl(constr);
                #region Update Gl from Related Temps
                for (int i = 0; i < glts_related.GLTempList.Count; i++)
                {
                    if (glts_related.GLTempList[i].GlTransactionId == glt_id)
                    {
                        gl_due = Convert.ToDecimal(glts_related.GLTempList[i].GlTransactionDue);
                        gl_paid = Convert.ToDecimal(glts_related.GLTempList[i].GlTransactionPaid);
                        gl_due += gl_paid;
                        cp_allocated -= gl_paid;
                        switch (glt_id[0])
                        {
                            case 'i':
                                #region INSERT
                                glts_related.GLTempList.RemoveAt(i); //Remove from temp data
                                #endregion
                                break;
                            case 'u':
                                #region UPDATE
                                glts_related.GLTempList[i].GlTransactionId = 'd' + glt_id.Substring(1); //Mark as DELETED
                                #endregion
                                break;
                            default:
                                #region DEFAULT
                                glts_related.GLTempList[i].GlTransactionId = 'd' + glt_id; //Mark as DELETED
                                #endregion
                                break;
                        }
                        break;
                    }
                }
                glts_related.UpdateTemp();
                #endregion
                LabelAllocated.Text = cp_allocated.ToString("0.00");
                Hashtable glitems = new Hashtable();

                switch (glt_id[0])
                {
                    case 'i':
                        #region INSERT
                        string inv_id = glt_id.Substring(1);
                        cinvoice.LoadData(Convert.ToInt32(inv_id));
                        break;
                        #endregion
                    case 'u':
                        #region UPDATE
                        gl_id = glt_id.Substring(1);
                        cinvoicegl.LoadDataByGLId(Convert.ToInt32(gl_id));
                        cinvoice.LoadData(cinvoicegl.Cinvoice_gl_cinvoice_id);
                        break;
                        #endregion
                    default:
                        #region Default
                        gl_id = glt_id;
                        cinvoicegl.LoadDataByGLId(Convert.ToInt32(gl_id));
                        cinvoice.LoadData(cinvoicegl.Cinvoice_gl_cinvoice_id);
                        break;
                        #endregion
                }

                glitems.Add("gl_transaction_id", cinvoice.CinvoiceId.ToString());
                glitems.Add("gl_transaction_ref", JSON.StrToDQuoteJson(cinvoice.CinvoiceNum));
                glitems.Add("gl_transaction_description", JSON.StrToDQuoteJson(cinvoice.CinvoiceDescription));
                glitems.Add("gl_transaction_date", DBSafeUtils.DBDateToStr(cinvoice.CinvoiceDate));
                glitems.Add("gl_transaction_duedate", DBSafeUtils.DBDateToStr(cinvoice.CinvoiceDue));
                glitems.Add("gl_transaction_net", (-cinvoice.CinvoiceNet).ToString("0.00"));
                glitems.Add("gl_transaction_tax", (-cinvoice.CinvoiceTax).ToString("0.00"));
                glitems.Add("gl_transaction_gross", (-cinvoice.CinvoiceGross).ToString("0.00"));
                decimal paid = cinvoice.CinvoicePaid;
                decimal gross = cinvoice.CinvoiceGross;
                decimal due = gross - paid;
                glitems.Add("gl_transaction_due", gl_due.ToString("0.00"));
                #region Add Gl to Unpaid Temps
                GlTransactionTemp glt = new GlTransactionTemp(constr);
                glt.LoadData(glitems);
                glts_unpaid.GLTempList.Add(glt);
                glts_unpaid.UpdateTemp();
                #endregion

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
                string cpayment_id = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                cpayment_id = Request.QueryString["cpaymentid"];
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                //Related Temp Trans
                GlTransactionTemps glts_related = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                glts_related.LoadData();
                //Unpaid Temp Trans
                GlTransactionTemps glts_unpaid = new GlTransactionTemps(user.UserId, TEMP_TYPE_UNPAID, constr_general, constr);
                glts_unpaid.LoadData();

                foreach (GlTransactionTemp glt in glts_related.GLTempList)
                {
                    string invoice_id = "";
                    string gl_id = "";
                    CPayment cpayment = new CPayment(constr);
                    cpayment.LoadData(Convert.ToInt32(cpayment_id));
                    Hashtable items = new Hashtable();
                    switch (glt.GlTransactionId[0])
                    {
                        case 'i':
                            #region INSERT
                            invoice_id = glt.GlTransactionId.Substring(1);
                            items.Add("cinvoice_id", Convert.ToInt32(invoice_id));
                            items.Add("gl_transaction_net", -Convert.ToDecimal(glt.GlTransactionPaid));
                            cpayment.AddGlTran(items, true);
                            #endregion
                            break;
                        case 'u':
                            #region UPDATE
                            gl_id = glt.GlTransactionId.Substring(1);
                            CinvoiceGl cinvoicegl = new CinvoiceGl(constr);
                            cinvoicegl.LoadDataByGLId(Convert.ToInt32(gl_id));
                            invoice_id = cinvoicegl.Cinvoice_gl_cinvoice_id.ToString();
                            items.Add("cinvoice_id", Convert.ToInt32(invoice_id));
                            items.Add("gl_transaction_net", -Convert.ToDecimal(glt.GlTransactionPaid));
                            cpayment.UpdateGlTran(items, Convert.ToInt32(gl_id));
                            #endregion
                            break;
                        case 'd':
                            #region DELETE
                            gl_id = glt.GlTransactionId.Substring(1);
                            cpayment.DeleteGlTran(Convert.ToInt32(gl_id));
                            #endregion
                            break;
                        default:
                            break;
                    }
                }
                #region Redirect
                Response.BufferOutput = true;
                Response.Redirect("goback2.aspx", false);
                #endregion
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}