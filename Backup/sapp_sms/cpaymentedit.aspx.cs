using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.General;
using Sapp.Common;
using Sapp.SMS;

namespace sapp_sms
{
    public partial class cpaymentedit : System.Web.UI.Page
    {
        private const string TEMP_TYPE_RELATED = "cpaymentedit1";
        private const string TEMP_TYPE_UNPAID = "cpaymentedit2";
        private const string TYPE_ID = "4";
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Javascript setup
            Control[] wc = { LabelGross };
            JSUtils.RenderJSArrayWithCliendIds(Page, wc);
            #endregion
            string cpayment_id = "";
            string mode = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (Request.QueryString["cpaymentid"] != null) cpayment_id = Request.QueryString["cpaymentid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                if (!IsPostBack)
                {
                    Session["gobackstep"] = 1;
                    if (mode == "add")
                    {
                        #region Load Page
                        try
                        {
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxCreditor, "SELECT `creditor_master_id` AS `ID`, `creditor_master_code` AS `Code` FROM `creditor_master`", "ID", "Code", constr, false);
                            #endregion
                            #region Initial Web Controls
                            TextBoxReference.Text = "";
                            TextBoxDate.Text = DateTime.Now.ToShortDateString();
                            TextBoxGross.Text = "0";
                            //LabelGross.Text = "0";
                            #endregion
                            #region Initial Temp File In Mysql

                            if (Request.QueryString["cinvoiceid"] != null)
                            {
                                string cinvID = Request.QueryString["cinvoiceid"];
                                Cinvoice c = new Cinvoice(constr);
                                c.LoadData(int.Parse(cinvID));
                                TextBoxGross.Text = (c.CinvoiceGross - c.CinvoicePaid).ToString();
                                TextBoxReference.Text = c.CinvoiceNum;
                                TextBoxDate.Text = c.CinvoiceDate.ToString("dd/MM/yyyy");
                                ComboBoxBodycorp.SelectedValue = c.CinvoiceBodycorpId.ToString();
                                ComboBoxBodycorp.Enabled = false;
                                ComboBoxCreditor.SelectedValue = c.CinvoiceCreditorId.ToString();
                                ComboBoxCreditor.Enabled = false;


                            }
                            //CPayment cpayment = new CPayment(constr);
                            //string login = HttpContext.Current.User.Identity.Name;
                            //Sapp.General.User user = new User(constr_general);
                            //user.LoadData(login);
                            ////Create Two Empty Temp
                            //GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                            //gltranTemps.Add(cpayment.GetGLTranJSON());
                            //GlTransactionTemps gltranTemps2 = new GlTransactionTemps(user.UserId, TEMP_TYPE_UNPAID, constr_general, constr);
                            //gltranTemps2.Add(cpayment.GetGLTranJSON());
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
                    if (mode == "edit")
                    {
                        #region Load Page
                        try
                        {
                            CPayment cpayment = new CPayment(constr);
                            cpayment.LoadData(Convert.ToInt32(cpayment_id));
                            #region  Check Rec

                            //string cksql = "Select * FROM `cpayments` WHERE `cpayment_id`=" + cpayment_id;

                            //DataTable dt = AdFunction.odbc.ReturnTable(cksql, "check");
                            //if (dt.Rows.Count > 0)
                            #region Check
                            if (cpayment.CpaymentReconciled)
                            {
                                ImageButtonSave.Visible = false;
                                SaveL.Text = "Reconciled Payment";
                            }


                            Bodycorp b = new Bodycorp(constr);
                            b.LoadData(cpayment.Cpayment_bodycorp_id);
                            if (!b.CheckCloseOff(cpayment.Cpayment_date))
                            {
                                SaveL.Text = "Payment before close date";
                                ImageButtonSave.Visible = false;
                                throw new Exception("Payment before close date");

                            }
                            #endregion
                            #endregion
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxCreditor, "SELECT `creditor_master_id` AS `ID`, `creditor_master_code` AS `Code` FROM `creditor_master`", "ID", "Code", constr, false);
                            #endregion
                            #region Load Web Controls
                            TextBoxReference.Text = cpayment.Cpayment_reference;
                            TextBoxDate.Text = cpayment.Cpayment_date.ToShortDateString();
                            TextBoxGross.Text = cpayment.Cpayment_gross.ToString();
                            LabelGross.Text = cpayment.Cpayment_gross.ToString();
                            AjaxControlUtils.ComboBoxSelection(ComboBoxBodycorp, cpayment.Cpayment_bodycorp_id, true);
                            ComboBoxBodycorp.Enabled = false;
                            AjaxControlUtils.ComboBoxSelection(ComboBoxType, cpayment.Cpayment_type_id, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxCreditor, cpayment.Cpayment_creditor_id, false);
                            ComboBoxCreditor.Enabled = false;
                            #endregion
                            #region Initial Temp File In Mysql
                            //string login = HttpContext.Current.User.Identity.Name;
                            //Sapp.General.User user = new User(constr_general);
                            //user.LoadData(login);
                            //GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                            //gltranTemps.Add(cpayment.GetRelatedCInvGLJSON(cpayment.Cpayment_id));
                            //GlTransactionTemps gltranTemps2 = new GlTransactionTemps(user.UserId, TEMP_TYPE_UNPAID, constr_general, constr);
                            //gltranTemps.Add(cpayment.GetUnpaidCInvGLJSON(cpayment.Cpayment_id, cpayment.Cpayment_bodycorp_id));
                            #endregion


                            if (cpayment.Cpayment_reference.Contains("REV"))
                                ImageButtonSave.Visible = false;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
                    if (Request.Cookies["bodycorpid"] != null)
                    {
                        ComboBoxBodycorp.SelectedValue = Request.Cookies["bodycorpid"].Value;
                        ComboBoxBodycorp.Enabled = false;
                    }
                }
                if (null != Request.QueryString["creditorid"])
                {
                    ComboBoxCreditor.SelectedValue = Request.QueryString["creditorid"];
                    ComboBoxCreditor.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridDataBindRelatedTrans(string postdata)
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
            dt.Columns.Add("Chart");
            dt.Columns.Add("Description");
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
                    if (!String.IsNullOrEmpty(items["gl_transaction_ref"].ToString()))
                    {
                        Cinvoice cinvoice = new Cinvoice(constr);
                        cinvoice.LoadData(Convert.ToInt32(items["gl_transaction_ref"]));
                        dr["CinvoiceNum"] = cinvoice.CinvoiceNum;
                    }
                    else
                        dr["CinvoiceNum"] = "";
                    if (!String.IsNullOrEmpty(items["gl_transaction_chart_id"].ToString()))
                    {
                        ChartMaster chartMaster = new ChartMaster(constr);
                        chartMaster.LoadData(Convert.ToInt32(items["gl_transaction_chart_id"]));
                        dr["Chart"] = chartMaster.ChartMasterCode;
                    }
                    else
                        dr["Chart"] = "";
                    dr["Description"] = items["gl_transaction_description"].ToString();
                    dr["Net"] = items["gl_transaction_net"].ToString();
                    dr["Tax"] = items["gl_transaction_tax"].ToString();
                    dr["Gross"] = items["gl_transaction_gross"].ToString();
                    dr["Due"] = items["gl_transaction_due"].ToString();
                    dr["Paid"] = items["gl_transaction_paid"].ToString();
                    dt.Rows.Add(dr);
                }
            }
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`, `CinvoiceNum`, `Chart`, `Description`, `Net`, `Tax`,`Gross`, `Due`, `Paid` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }
        [System.Web.Services.WebMethod]
        public static string DataGridDataBindUnpaidTrans(string postdata)
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
            dt.Columns.Add("Chart");
            dt.Columns.Add("Description");
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
                    if (!String.IsNullOrEmpty(items["gl_transaction_ref"].ToString()))
                    {
                        Cinvoice cinvoice = new Cinvoice(constr);
                        cinvoice.LoadData(Convert.ToInt32(items["gl_transaction_ref"]));
                        dr["CinvoiceNum"] = cinvoice.CinvoiceNum;
                    }
                    else
                        dr["CinvoiceNum"] = "";
                    if (!String.IsNullOrEmpty(items["gl_transaction_chart_id"].ToString()))
                    {
                        ChartMaster chartMaster = new ChartMaster(constr);
                        chartMaster.LoadData(Convert.ToInt32(items["gl_transaction_chart_id"]));
                        dr["Chart"] = chartMaster.ChartMasterCode;
                    }
                    else
                        dr["Chart"] = "";
                    dr["Description"] = items["gl_transaction_description"].ToString();
                    dr["Net"] = items["gl_transaction_net"].ToString();
                    dr["Tax"] = items["gl_transaction_tax"].ToString();
                    dr["Gross"] = items["gl_transaction_gross"].ToString();
                    dr["Due"] = items["gl_transaction_due"].ToString();
                    dt.Rows.Add(dr);
                }
            }
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`, `CinvoiceNum`, `Chart`, `Description`, `Net`, `Tax`,`Gross`, `Due` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }
        [System.Web.Services.WebMethod]
        public static string SaveDataFromGrid(string rowValue)
        {
            //string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            //Object c = JSON.JsonDecode(rowValue);
            //Hashtable hdata = (Hashtable)c;
            //string session_id = HttpContext.Current.Session.SessionID;
            //CPayment cpayment = new CPayment(constr);
            //cpayment.ImportJSON(session_id, "Related");
            //GlTransaction gltran = new GlTransaction(constr);
            //string selectedIndex = hdata["ID"].ToString();
            //if (selectedIndex != "")// Only Updating Paid Allowed
            //{
            //    CpaymentGl cpaygl= cpayment.CpaymentGlList[Int32.Parse(selectedIndex) - 1] as CpaymentGl ;
            //    cpaygl.LoadData(cpaygl.CpaymentGlId);
            //    gltran.LoadData(cpaygl.Cpayment_gl_gl_id);
            //    decimal due = 0;
            //    if (null != HttpContext.Current.Session["cpaymentid"])
            //    {
            //        string cpaymentid = HttpContext.Current.Session["cpaymentid"].ToString();
            //        due = gltran.GetOutstandingAmountExcludeCpayment(Int32.Parse(cpaymentid));
            //    }
            //    else
            //    {
            //        due = gltran.GetOutstandingAmount();
            //    }
            //    decimal paid = Convert.ToDecimal(hdata["Paid"].ToString());
            //    if (paid > due) paid = due;
            //    cpaygl.CpaymentGlPaid = paid;
            //}
            //cpayment.ExportJSON(session_id, "Related");
            return "dd";
        }
        #endregion
        #region WebControl Events
        protected void ImageButtonSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                #region Load System Chart ID

                Sapp.SMS.System s = new Sapp.SMS.System(AdFunction.conn);
                ChartMaster ch = new ChartMaster(AdFunction.conn);
                s.LoadData("GST Input");
                ch.LoadData(s.SystemValue);
                string InputGstID = ch.ChartMasterId.ToString();
                s.LoadData("GST Output");
                ch.LoadData(s.SystemValue);
                string OutputGstID = ch.ChartMasterId.ToString();
                s.LoadData("GENERALTAX");
                ch.LoadData(s.SystemValue);
                string gstid = ch.ChartMasterId.ToString();
                s.LoadData("GENERALDEBTOR");
                ch.LoadData(s.SystemValue);
                ch.LoadData(s.SystemValue);
                string proprietorID = ch.ChartMasterId.ToString();
                s.LoadData("GENERALCREDITOR");
                ch.LoadData(s.SystemValue);
                string creditorID = ch.ChartMasterId.ToString();
                s.LoadData("DISCOUNTCHARCODE");
                ch.LoadData(s.SystemValue);
                string discountID = ch.ChartMasterId.ToString();

                #endregion
                string cpaymentid = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                //Check
                Bodycorp b = new Bodycorp(constr);
                b.LoadData(ComboBoxBodycorp.SelectedValue);
                if (!b.CheckCloseOff(DateTime.Parse(TextBoxDate.Text)))
                {
                    throw new Exception("Payment before close date");
                }

                if (Request.QueryString["cpaymentid"] != null) cpaymentid = Request.QueryString["cpaymentid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                if (!Page.IsValid) return;
                Hashtable items = new Hashtable();
                #region Retireve Values
                items.Add("cpayment_bodycorp_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxBodycorp, false));
                items.Add("cpayment_creditor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxCreditor, false));
                items.Add("cpayment_reference", DBSafeUtils.StrToQuoteSQL(TextBoxReference.Text));
                items.Add("cpayment_ctype_id", "1");
                items.Add("cpayment_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxType, false));
                items.Add("cpayment_gross", DBSafeUtils.DecimalToSQL(TextBoxGross.Text));
                items.Add("cpayment_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                #endregion
                if (mode == "add")
                {
                    #region Add


                    CPayment cpayment = new CPayment(constr);
                    cpayment.Add(items);
                    #endregion
                    #region Redirect
                    Response.BufferOutput = true;
                    Response.Redirect("cpaymentallocate.aspx?cpaymentid=" + cpayment.Cpayment_id, false);
                    #endregion

                    if (Request.QueryString["cinvoiceid"] != null)
                    {
                        Hashtable allowitems = new Hashtable();
                        string invoice_id = Request.QueryString["cinvoiceid"].ToString();
                        allowitems.Add("cinvoice_id", invoice_id);
                        allowitems.Add("gl_transaction_description", "CPayment");
                        allowitems.Add("gl_transaction_tax", "0");
                        allowitems.Add("gl_transaction_date", "'" + TextBoxDate.Text + "'");
                        allowitems.Add("gl_transaction_type_id", "4");
                        allowitems.Add("gl_transaction_gross", "0");
                        allowitems.Add("gl_transaction_chart_id", creditorID);
                        allowitems.Add("gl_transaction_ref", invoice_id);
                        allowitems.Add("gl_transaction_bodycorp_id", ComboBoxBodycorp.SelectedValue);
                        allowitems.Add("gl_transaction_net", TextBoxGross.Text);
                        cpayment.AddGlTran(allowitems, true);
                        Cinvoice c = new Cinvoice(constr);
                        c.LoadData(int.Parse(invoice_id));
                        c.UpdatePaid();

                    }
                }
                else if (mode == "edit")
                {
                    #region Edit
                    CPayment cpayment = new CPayment(AdFunction.conn);
                    cpayment.LoadData(Convert.ToInt32(cpaymentid));
                    b.LoadData(cpayment.Cpayment_bodycorp_id);
                    if (!b.CheckCloseOff(cpayment.Cpayment_date))
                    {
                        throw new Exception("Payement before close date");

                    }
                    else if (cpayment.CpaymentReconciled)
                    {
                        throw new Exception("Payement Reconciled");
                    }

                    cpayment.Update(items, Convert.ToInt32(cpaymentid));
                    #endregion
                    #region Redirect
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
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
        #endregion
        #region Validations

        protected void CustomValidatorCreditor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxCreditor.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void CustomValidatorType_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxType.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}