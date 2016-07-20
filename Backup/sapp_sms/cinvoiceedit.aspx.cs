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
using System.Data.Odbc;
using System.Web.Script.Serialization;

namespace sapp_sms
{
    public partial class cinvoiceedit : System.Web.UI.Page, IPostBackEventHandler
    {
        private const string TEMP_TYPE = "cinvoiceedit";
        private const string TYPE_ID = "2";
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Javascript setup
            Control[] wc = { jqGridTrans, LabelNet, LabelTax, LabelGross, CheckBoxUnitAdminFee, TextBoxAdminFee, TextBoxDate, TextBoxApply };
            JSUtils.RenderJSArrayWithCliendIds(Page, wc);
            #endregion
            string cinvoice_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["cinvoiceid"] != null)
                {
                    cinvoice_id = Request.QueryString["cinvoiceid"];
                    Session["cinvoiceid"] = Request.QueryString["cinvoiceid"];
                }
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                if (!IsPostBack)
                {
                    Session["gobackstep"] = 1;
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;

                    // Get chart master data as JSon string. 01/04/2016
                    SetChartCodeNameJSonstring();

                    // Link to GST registered. 01/04/2016
                    SetGSTRegister();

                    if (mode == "add")
                    {
                        #region Load Page
                        try
                        {
                            if (!IsPostBack)
                            {
                                #region Initial ComboBox
                                AjaxControlUtils.SetupComboBox(ComboBoxOrder, "SELECT `purchorder_master_id` AS `ID`, `purchorder_master_num` AS `Code` FROM `purchorder_master` WHERE `purchorder_master_allocated`=0", "ID", "Code", constr, false);
                                AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, false);
                                string pid = "";
                                if (ComboBoxBodycorp.Items.Count > 0 && ComboBoxBodycorp.SelectedIndex < 0)
                                {
                                    ComboBoxBodycorp.SelectedIndex = 0;
                                    pid = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "property_master"), "property_master_bodycorp_id", ComboBoxBodycorp.SelectedValue, "property_master_id");
                                }

                                // Update 15/04/2016
                                string sql = "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master`, `property_master` WHERE unit_master_property_id = property_master_id ";
                                if (!pid.Equals(""))
                                    sql += "  AND property_master_bodycorp_id=" + pid;
                                AjaxControlUtils.SetupComboBox(ComboBoxUnit, sql + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Code", constr, true);

                                AjaxControlUtils.SetupComboBox(ComboBoxCreditor, "SELECT `creditor_master_id` AS `ID`, `creditor_master_code` AS `Code` FROM `creditor_master`", "ID", "Code", constr, false);
                                if (null != Request.QueryString["creditorid"])
                                {
                                    ComboBoxCreditor.SelectedValue = Request.QueryString["creditorid"];
                                    ComboBoxCreditor.Enabled = false;
                                }
                                #endregion
                                #region Initial Web Controls
                                TextBoxDescription.Text = "";
                                TextBoxNum.Text = "";
                                TextBoxDue.Text = "";
                                TextBoxApply.Text = "";
                                //TextBoxPaid.Text = "";
                                LabelGross.Text = "0.00";
                                LabelNet.Text = "0.00";
                                LabelTax.Text = "0.00";
                                TextBoxNum.Text = "";
                                TextBoxDate.Text = "";
                                #endregion
                                #region Initial Temp File In Mysql
                                Cinvoice cinvoice = new Cinvoice(constr);
                                string login = HttpContext.Current.User.Identity.Name;
                                Sapp.General.User user = new User(constr_general);
                                user.LoadData(login);
                                GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
                                gltranTemps.Add(cinvoice.GetGLTranJSON());
                                InvoiceMaster invoice = new InvoiceMaster(constr);
                                GlTransactionTemps gltranTemps2 = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
                                gltranTemps2.Add(invoice.GetGLTranJSON());
                                
                                // Fix bug for ComboBoxUnit selected value. 17/03/2016
                                if (Request.Cookies["bodycorpid"] != null)
                                {
                                    ComboBoxBodycorp.SelectedValue = Request.Cookies["bodycorpid"].Value;
                                    ComboBoxBodycorp.Enabled = false;
                                    ComboBoxBodycorp_SelectedIndexChanged(null, null);
                                }
                                
                            }
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
                            TextBoxNum.Enabled = false;
                            Cinvoice cinvoice = new Cinvoice(constr);
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxOrder, "SELECT `purchorder_master_id` AS `ID`, `purchorder_master_num` AS `Code` FROM `purchorder_master`  WHERE `purchorder_master_allocated`=0", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxCreditor, "SELECT `creditor_master_id` AS `ID`, `creditor_master_code` AS `Code` FROM `creditor_master`", "ID", "Code", constr, false);
                            //AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` where unit_master_property_id = " + Request.Cookies["bodycorpid"].Value + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Code", constr, true);
                            // Update 15/04/2016
                            AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master`, `property_master` where unit_master_property_id = property_master_id AND property_master_bodycorp_id = " + Request.Cookies["bodycorpid"].Value + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Code", constr, true);
                            if (null != Request.QueryString["creditorid"])
                            {
                                ComboBoxCreditor.SelectedValue = Request.QueryString["creditorid"];
                                ComboBoxCreditor.Enabled = false;
                            }
                            #endregion
                            #region Initial Web Controls
                            cinvoice.LoadData(Convert.ToInt32(cinvoice_id));
                            if (cinvoice.CinvoicePaid > 0)
                            {
                                // Change to limited edit. 31/03/2016
                                //throw new Exception("CInvoice has been paid");
                                isLimitedEdit.Value = "true";
                                TextBoxNum.Enabled = false;
                                ComboBoxOrder.Enabled = false;
                                ComboBoxCreditor.Enabled = false;
                                ComboBoxBodycorp.Enabled = false;
                                ComboBoxUnit.Enabled = false;
                                CheckBoxUnitAdminFee.Enabled = false;
                                TextBoxAdminFee.Enabled = false;
                                TextBoxDate.Enabled = false;
                                TextBoxDue.Enabled = false;
                                TextBoxApply.Enabled = false;

                                // hide "Add" and "Delete" button
                                ImageButtonDelete.Visible = false;
                                jqGridTrans.addVisible = "false";
                            }
                            cinvoice.LoadTransactions();
                            TextBoxDescription.Text = cinvoice.CinvoiceDescription;
                            TextBoxNum.Text = cinvoice.CinvoiceNum;
                            if (cinvoice.CinvoiceDue.HasValue)
                                TextBoxDue.Text = cinvoice.CinvoiceDue.Value.ToShortDateString();
                            if (cinvoice.CinvoiceApply.HasValue)
                                TextBoxApply.Text = cinvoice.CinvoiceApply.Value.ToShortDateString();
                            TextBoxDate.Text = cinvoice.CinvoiceDate.ToShortDateString();
                            TextBoxNum.Text = cinvoice.CinvoiceNum.ToString();
                            TextBoxDate.Text = cinvoice.CinvoiceDate.ToShortDateString();
                            LabelGross.Text = cinvoice.CinvoiceGross.ToString("0.00");
                            LabelNet.Text = cinvoice.CinvoiceNet.ToString("0.00");
                            LabelTax.Text = cinvoice.CinvoiceTax.ToString("0.00");
                            //TextBoxPaid.Text = cinvoice.CinvoicePaid.ToString();
                            //CheckBoxUnitAdminFee.Checked = cinvoice.CinvoiceUnitAdminFee ?? false;
                            AjaxControlUtils.ComboBoxSelection(ComboBoxOrder, cinvoice.CinvoiceOrderId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxUnit, cinvoice.CinvoiceUnitId, true);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxCreditor, cinvoice.CinvoiceCreditorId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxBodycorp, cinvoice.CinvoiceBodycorpId, false);
                            ComboBoxBodycorp.Enabled = false;   // Fix bug for ComboBoxUnit selected value. 17/03/2016
                            #endregion
                            if (!IsPostBack)
                            {
                                #region Initial Temp File In Mysql
                                string login = HttpContext.Current.User.Identity.Name;
                                Sapp.General.User user = new User(constr_general);
                                user.LoadData(login);
                                GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
                                gltranTemps.Add(cinvoice.GetGLTranJSON());
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion

                    }
                    if (mode == "paste")
                    {
                        #region Load Page

                        try
                        {
                            Cinvoice cinvoice = new Cinvoice(constr);
                            cinvoice.LoadData(Convert.ToInt32(cinvoice_id));
                            cinvoice.LoadTransactions();

                            #region Initial Web Controls
                            AjaxControlUtils.SetupComboBox(ComboBoxOrder, "SELECT `purchorder_master_id` AS `ID`, `purchorder_master_num` AS `Code` FROM `purchorder_master`  WHERE `purchorder_master_allocated`=0", "ID", "Code", constr, false);

                            AjaxControlUtils.SetupComboBox(ComboBoxCreditor, "SELECT `creditor_master_id` AS `ID`, `creditor_master_code` AS `Code` FROM `creditor_master`", "ID", "Code", constr, false);
                            if (null != Request.QueryString["creditorid"])
                            {
                                ComboBoxCreditor.SelectedValue = Request.QueryString["creditorid"];
                                ComboBoxCreditor.Enabled = false;
                            }
                            else
                            {
                                AjaxControlUtils.ComboBoxSelection(ComboBoxCreditor, cinvoice.CinvoiceCreditorId, false);
                            }

                            AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, false);
                            if (cinvoice.CinvoiceBodycorpId != 0)
                            {
                                AjaxControlUtils.ComboBoxSelection(ComboBoxBodycorp, cinvoice.CinvoiceBodycorpId, false);
                                ComboBoxBodycorp.Enabled = false;
                            }

                            AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master`, `property_master` where unit_master_property_id = property_master_id AND property_master_bodycorp_id = " + Request.Cookies["bodycorpid"].Value + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Code", constr, true);
                            if (cinvoice.CinvoiceUnitId != 0)
                            {
                                AjaxControlUtils.ComboBoxSelection(ComboBoxUnit, cinvoice.CinvoiceUnitId, true);
                            }
                            TextBoxDescription.Text = cinvoice.CinvoiceDescription;

                            LabelGross.Text = cinvoice.CinvoiceGross.ToString("0.00");
                            LabelNet.Text = cinvoice.CinvoiceNet.ToString("0.00");
                            LabelTax.Text = cinvoice.CinvoiceTax.ToString("0.00");
                            #endregion

                            if (!IsPostBack)
                            {
                                #region Initial Temp File In Mysql

                                Cinvoice cinvoice_new = new Cinvoice(constr);
                                string login = HttpContext.Current.User.Identity.Name;
                                Sapp.General.User user = new User(constr_general);
                                user.LoadData(login);
                                GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
                                gltranTemps.Add(cinvoice_new.GetGLTranJSON());


                                gltranTemps.LoadData();
                                #region INSERT
                                for (int i = 0; i < cinvoice.GltransactionList.Count(); i++)
                                {
                                    Hashtable line = new Hashtable();

                                    line.Add("gl_transaction_id", "i" + Guid.NewGuid().ToString("N"));
                                    line.Add("gl_transaction_type_id", cinvoice.GltransactionList[i].GLTransactionTypeId);
                                    line.Add("gl_transaction_ref", "");
                                    line.Add("gl_transaction_chart_id", cinvoice.GltransactionList[i].GlTransactionChartId);
                                    line.Add("gl_transaction_ref_type_id", cinvoice.GltransactionList[i].GLTransactionTypeId);
                                    line.Add("gl_transaction_bodycorp_id", cinvoice.GltransactionList[i].GlTransactionBodycorpId);
                                    line.Add("gl_transaction_unit_id", "");
                                    line.Add("gl_transaction_description", cinvoice.GltransactionList[i].GlTransactionDescription);
                                    line.Add("gl_transaction_net", - cinvoice.GltransactionList[i].GlTransactionNet);
                                    line.Add("gl_transaction_tax", - cinvoice.GltransactionList[i].GlTransactionTax);
                                    line.Add("gl_transaction_gross", - cinvoice.GltransactionList[i].GlTransactionGross);
                                    line.Add("gl_transaction_date", "");

                                    GlTransactionTemp gltran = new GlTransactionTemp(constr);
                                    gltran.LoadData(line);
                                    gltranTemps.GLTempList.Add(gltran);
                                }
                                #endregion

                                gltranTemps.UpdateTemp();
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion

                    }

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
                if (args[0] == "ImageButtonDelete")
                {
                    ImageButtonDelete_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #region WebControl Events
        protected string InvoiceImageButtonSave()
        {
            try
            {
                string invoice_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (!Page.IsValid) return null;
                if (Request.QueryString["invoicemasterid"] != null) invoice_id = Request.QueryString["invoicemasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                Hashtable items = new Hashtable();

                #region Retrieve Invoice Values
                items.Add("invoice_master_unit_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxUnit, true));
                string did = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "unit_master"), "unit_master_id", ComboBoxUnit.SelectedValue, "unit_master_debtor_id");
                items.Add("invoice_master_debtor_id", did);
                items.Add("invoice_master_bodycorp_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxBodycorp, false));
                items.Add("invoice_master_type_id", 1);
                InvoiceMaster im = new InvoiceMaster(constr);
                items.Add("invoice_master_num", DBSafeUtils.StrToQuoteSQL(im.GetNextNumber(-1, "O")));
                items.Add("invoice_master_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                items.Add("invoice_master_due", DBSafeUtils.DateToSQL(TextBoxDue.Text));
                items.Add("invoice_master_description", DBSafeUtils.StrToQuoteSQL(TextBoxDescription.Text));
                #endregion
                Odbc mydb = null;
                try
                {
                    mydb = new Odbc(constr);
                    mydb.StartTransaction();
                    if (mode == "add" || mode == "paste")
                    {
                        #region Add Cinvoice
                        InvoiceMaster invoice = new InvoiceMaster(constr);
                        invoice.SetOdbc(mydb);
                        Bodycorp b = new Bodycorp(AdFunction.conn);
                        b.LoadData(ComboBoxBodycorp.SelectedValue);
                        if (!b.CheckCloseOff(DateTime.Parse(TextBoxDate.Text)))
                        {
                            throw new Exception("CInvoice before close date");
                        }
                        invoice_id = invoice.Add(items, true).ToString();
                        #endregion
                    }
                    else if (mode == "edit")
                    {
                        #region Update Cinvoice
                        InvoiceMaster invoice = new InvoiceMaster(constr);
                        Cinvoice cinvoice = new Cinvoice(AdFunction.conn);
                        cinvoice.LoadData(int.Parse(invoice_id));
                        Bodycorp b = new Bodycorp(AdFunction.conn);
                        b.LoadData(cinvoice.CinvoiceBodycorpId);

                        if (!b.CheckCloseOff(cinvoice.CinvoiceDate))
                        {
                            throw new Exception("CInvoice before close date");
                        }
                        invoice.SetOdbc(mydb);
                        invoice.Update(items, Convert.ToInt32(invoice_id));

                        if (invoice.InvoiceMasterNum.Contains("REV"))
                            ImageButtonSave.Visible = false;


                        #endregion
                    }
                    #region Save Transaction
                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(constr_general);
                    user.LoadData(login);
                    GlTransactionTemps glTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
                    glTemps.LoadData();
                    InvoiceMaster related_invoice = new InvoiceMaster(constr);
                    related_invoice.SetOdbc(mydb);
                    related_invoice.LoadData(Convert.ToInt32(invoice_id));
                    glTemps.Submit<InvoiceMaster>(related_invoice);
                    #endregion
                    mydb.Commit();
                }
                catch (Exception ex)
                {
                    if (mydb != null) mydb.Rollback();
                    return "";
                    throw ex;
                }
                finally
                {
                    if (mydb != null) mydb.Close();
                }
                return invoice_id;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
                return "";
            }

        }
        protected void ImageButtonSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Cinvoice checkCinv = new Cinvoice(AdFunction.conn);
                checkCinv.LoadData(TextBoxNum.Text);
                if (checkCinv.CinvoiceId > 0 && TextBoxNum.Enabled)
                {
                    if (checkCinv.CinvoiceCreditorId.ToString().Equals(ComboBoxCreditor.SelectedValue))
                        throw new Exception("CInvoice Number Existed");
                }
                string iid = "";
                if (ComboBoxUnit.SelectedIndex > 0)
                    iid = InvoiceImageButtonSave();
                string cinvoice_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (!Page.IsValid) return;
                if (Request.QueryString["cinvoiceid"] != null) cinvoice_id = Request.QueryString["cinvoiceid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                Hashtable items = new Hashtable();
                #region Retrieve Invoice Values
                items.Add("cinvoice_order_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxOrder, false));
                items.Add("cinvoice_unit_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxUnit, true));
                items.Add("cinvoice_creditor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxCreditor, false));
                items.Add("cinvoice_bodycorp_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxBodycorp, false));
                items.Add("cinvoice_num", DBSafeUtils.StrToQuoteSQL(TextBoxNum.Text));
                items.Add("cinvoice_type_id", "1");
                items.Add("cinvoice_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                items.Add("cinvoice_due", DBSafeUtils.DateToSQL(TextBoxDue.Text));
                items.Add("cinvoice_apply", DBSafeUtils.DateToSQL(TextBoxApply.Text));
                items.Add("cinvoice_unit_admin_fee", DBSafeUtils.BoolToSQL(false)); //admin fee is not using anymore
                items.Add("cinvoice_description", DBSafeUtils.StrToQuoteSQL(TextBoxDescription.Text));
                #endregion
                Odbc mydb = null;
                try
                {
                    mydb = new Odbc(constr);
                    mydb.StartTransaction();
                    if (mode == "add" || mode == "paste")
                    {
                        #region Add Cinvoice
                        Cinvoice cinvoice = new Cinvoice(constr);
                        cinvoice.SetOdbc(mydb);
                        cinvoice_id = cinvoice.Add(items, true).ToString();
                        #endregion
                    }
                    else if (mode == "edit")
                    {
                        #region Update Cinvoice
                        Cinvoice cinvoice = new Cinvoice(constr);
                        cinvoice.LoadData(int.Parse(cinvoice_id));
                        if (cinvoice.CinvoicePaid > 0)
                        {
                            // Change to limited edit. 31/03/2016
                            //throw new Exception("Invoice has been paid");
                        }
                        Bodycorp b = new Bodycorp(AdFunction.conn);
                        b.LoadData(cinvoice.CinvoiceBodycorpId);
                        if (!b.CheckCloseOff(cinvoice.CinvoiceDate))
                        {
                            throw new Exception("CInvoice before close date");
                        }
                        if (cinvoice.CinvoicePaid > 0)
                        {
                            // Change to limited edit. 31/03/2016
                            //throw new Exception("CInvoice has been paid");
                        }
                        cinvoice.SetOdbc(mydb);
                        cinvoice.Update(items, Convert.ToInt32(cinvoice_id));
                        #endregion
                    }
                    #region Save Transaction

                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(constr_general);
                    user.LoadData(login);
                    GlTransactionTemps glTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
                    glTemps.LoadData();
                    Cinvoice related_invoice = new Cinvoice(constr);
                    related_invoice.SetOdbc(mydb);
                    related_invoice.LoadData(Convert.ToInt32(cinvoice_id));
                    glTemps.Submit<Cinvoice>(related_invoice);
                    mydb.Commit();
                }
                catch (Exception ex)
                {
                    if (mydb != null) mydb.Rollback();
                    throw ex;
                }
                finally
                {
                    if (mydb != null) mydb.Close();
                }
                    #endregion
                #region Redirect

                if (!ComboBoxUnit.SelectedValue.Equals("Null") && !ComboBoxUnit.SelectedValue.Equals(""))
                {
                    try
                    {
                        string invoice_id = "";
                        Hashtable invitems = new Hashtable();
                        #region Retrieve Invoice Values
                        invitems.Add("invoice_master_unit_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxUnit, true));
                        invitems.Add("invoice_master_debtor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxCreditor, false));
                        invitems.Add("invoice_master_bodycorp_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxBodycorp, false));
                        invitems.Add("invoice_master_type_id", 1);
                        invitems.Add("invoice_master_num", DBSafeUtils.StrToQuoteSQL(TextBoxNum.Text));
                        invitems.Add("invoice_master_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                        invitems.Add("invoice_master_due", DBSafeUtils.DateToSQL(TextBoxDue.Text));
                        invitems.Add("invoice_master_description", DBSafeUtils.StrToQuoteSQL(TextBoxDescription.Text));
                        #endregion
                        try
                        {
                            string g = GetGrossTotal();
                            if (!g.Equals("0.00"))
                            {
                                #region Add Cinvoice
                                InvoiceMaster invoice = new InvoiceMaster(constr);
                                Bodycorp b = new Bodycorp(AdFunction.conn);
                                b.LoadData(ComboBoxBodycorp.SelectedValue);
                                invoice.SetOdbc(mydb);
                                invoice_id = invoice.Add(invitems, true).ToString();
                                #endregion
                            }
                            #region Save Transaction
                            string login = HttpContext.Current.User.Identity.Name;
                            Sapp.General.User user = new User(constr_general);
                            user.LoadData(login);
                            GlTransactionTemps glTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
                            glTemps.LoadData();
                            InvoiceMaster related_invoice = new InvoiceMaster(constr);
                            related_invoice.SetOdbc(mydb);
                            related_invoice.LoadData(Convert.ToInt32(invoice_id));
                            glTemps.Submit<InvoiceMaster>(related_invoice);
                            #endregion

                        }
                        catch (Exception ex)
                        {
                            if (mydb != null) mydb.Rollback();
                            throw ex;
                        }
                        finally
                        {
                            if (mydb != null) mydb.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
                    }
                }

                if (iid.Equals(""))
                    if (Session["gobackstep"].ToString() == "2")
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("cinvoices.aspx", false);
                    }
                    else
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("cinvoices.aspx", false);
                    }
                else
                {
                    Response.Redirect("~/invoicemasteredit.aspx?mode=edit&invoicemasterid=" + iid + "&burl=no", false);
                }
                #endregion
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            //Javascript Function Only
        }
        private void InvoiceImageButtonDelete_Click(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string gl_transaction_id = args[1];

                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
                gltranTemps.LoadData();
                gltranTemps.DeleteGlTranTemp(gl_transaction_id);
                gltranTemps.UpdateTemp();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void ImageButtonDelete_Click(string[] args)
        {
            try
            {
                InvoiceImageButtonDelete_Click(args);
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string gl_transaction_id = args[1];

                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
                gltranTemps.LoadData();
                gltranTemps.DeleteGlTranTemp(gl_transaction_id);
                gltranTemps.UpdateTemp();
                Session["gobackstep"] = 2;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
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
        protected void CustomValidatorBodycorp_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxBodycorp.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
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
            dt.Columns.Add("Chart");
            dt.Columns.Add("Description");
            dt.Columns.Add("Net");
            dt.Columns.Add("Tax");
            dt.Columns.Add("Gross");
            dt.Columns.Add("GST");
            decimal net = 0;
            decimal tax = 0;
            decimal gross = 0;
            foreach (GlTransactionTemp gltran in gltranTemps.GLTempList)
            {
                Hashtable items = gltran.GetData();
                DataRow dr = dt.NewRow();
                if (items["gl_transaction_id"].ToString()[0] != 'd')
                {
                    dr["ID"] = items["gl_transaction_id"].ToString();
                    if (!String.IsNullOrEmpty(items["gl_transaction_chart_id"].ToString()))
                    {
                        ChartMaster chartMaster = new ChartMaster(constr);
                        string ch = items["gl_transaction_chart_id"].ToString();
                        if (ch.Contains("|"))
                        {

                            chartMaster.LoadData(ch.Substring(0, ch.IndexOf("|") - 1));
                        }
                        else
                        {
                            chartMaster.LoadData(int.Parse(ch));
                        }
                        dr["Chart"] = chartMaster.ChartMasterCode + " | " + chartMaster.ChartMasterName;
                    }
                    else
                        dr["Chart"] = "";
                    //dr["Chart"] = items["gl_transaction_chart_id"].ToString();
                    dr["Description"] = items["gl_transaction_description"].ToString();
                    dr["Net"] = items["gl_transaction_net"].ToString();
                    dr["Tax"] = items["gl_transaction_tax"].ToString();
                    dr["Gross"] = items["gl_transaction_gross"].ToString();
                    dt.Rows.Add(dr);
                    net += Convert.ToDecimal(items["gl_transaction_net"]);
                    tax += Convert.ToDecimal(items["gl_transaction_tax"]);
                    gross += Convert.ToDecimal(items["gl_transaction_gross"]);


                    decimal ltax = decimal.Parse(items["gl_transaction_tax"].ToString());
                    if (ltax != 0)
                    {
                        dr["GST"] = "Yes";
                    }
                    else
                    {
                        dr["GST"] = "No";
                    }
                }
            }
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`, `Chart`, `Description`, `Net`, `Tax`,`Gross`,`GST` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            Hashtable userdata = new Hashtable();
            userdata.Add("Description", "Total:");
            userdata.Add("Net", net.ToString("0.00"));
            userdata.Add("Tax", tax.ToString("0.00"));
            userdata.Add("Gross", gross.ToString("0.00"));
            jqgridObj.SetUserData(userdata);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }
        [System.Web.Services.WebMethod]
        public static string BindChartSelector()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                InvoiceMaster invoice = new InvoiceMaster(constr);

                foreach (ChartMaster chartmaster in invoice.GetChartMasters())
                {
                    html += "<option value='" + chartmaster.ChartMasterName + "'>" + chartmaster.ChartMasterCode + " | " + chartmaster.ChartMasterName + "</option>";
                }
                html += "</select>";
                return html;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        public static string InvoiceSaveDataFromGrid(string rowValue, string id)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);
            Object c = JSON.JsonDecode(rowValue);
            Hashtable hdata = (Hashtable)c;
            string line_id = hdata["ID"].ToString();
            GlTransactionTemps glTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
            glTemps.LoadData();
            Hashtable line = new Hashtable();


            if (hdata["GST"].ToString().Equals("Yes"))
            {
                if (hdata["Tax"].ToString().Equals(""))
                {
                    Sapp.SMS.System s = new Sapp.SMS.System(constr);
                    s.LoadData("GST");
                    double gst = double.Parse(s.SystemValue);
                    decimal g = AdFunction.Rounded(hdata["Gross"].ToString(), 2);
                    decimal t = decimal.Parse((double.Parse(hdata["Gross"].ToString()) / (1 + gst) * gst).ToString("f2"));
                    decimal n = g - t;
                    hdata["Net"] = n;
                    hdata["Tax"] = t;
                }
            }
            else
            {
                if (hdata["Net"].ToString().Equals(""))
                {
                    decimal g = AdFunction.Rounded(hdata["Gross"].ToString(), 2);
                    hdata["Net"] = g;
                    hdata["Tax"] = 0;
                }
            }

            if (line_id == "")
            {
                #region INSERT
                line.Add("gl_transaction_id", "i" + id); // Create a unique id for insert unit
                line.Add("gl_transaction_type_id", 1);
                line.Add("gl_transaction_ref", "");

                string ch = hdata["Chart"].ToString();
                if (ch.Contains("|"))
                {
                    ChartMaster chart = new ChartMaster(constr);
                    ch = ch.Substring(0, ch.IndexOf("|") - 1);
                    chart.LoadData(ch);
                    ch = chart.ChartMasterId.ToString();
                }

                line.Add("gl_transaction_chart_id", ch);
                line.Add("gl_transaction_bodycorp_id", "");
                line.Add("gl_transaction_unit_id", "");
                line.Add("gl_transaction_description", hdata["Description"].ToString());
                decimal n = AdFunction.Rounded(hdata["Net"].ToString(), 2);
                decimal t = AdFunction.Rounded(hdata["Tax"].ToString(), 2);
                decimal g = AdFunction.Rounded(hdata["Gross"].ToString(), 2);
                if (g != n + t)
                    throw new Exception("Net + Tax not Equals Gross");
                line.Add("gl_transaction_net", n.ToString("f2"));
                line.Add("gl_transaction_tax", t.ToString("f2"));
                line.Add("gl_transaction_gross", g.ToString("f2"));
                line.Add("gl_transaction_date", "");
                GlTransactionTemp gltran = new GlTransactionTemp(constr);
                gltran.LoadData(line);
                glTemps.GLTempList.Add(gltran);
                #endregion
            }
            else
            {
                decimal n = decimal.Parse(hdata["Net"].ToString());
                decimal t = decimal.Parse(hdata["Tax"].ToString());
                decimal g = decimal.Parse(hdata["Gross"].ToString());
                if (g != n + t)
                    throw new Exception("Net + Tax not Equals Gross");
                #region UPDATE
                for (int i = 0; i < glTemps.GLTempList.Count; i++)
                {
                    if (glTemps.GLTempList[i].GlTransactionId == hdata["ID"].ToString())
                    {
                        //ChartMaster chart = new ChartMaster(constr);
                        //string ch = hdata["Chart"].ToString();
                        //ch = ch.Substring(0, ch.IndexOf("|") - 1);
                        //chart.LoadData(ch);
                        if ((hdata["ID"].ToString()[0] != 'i') && (hdata["ID"].ToString()[0] != 'u')) glTemps.GLTempList[i].GlTransactionId = "u" + hdata["ID"].ToString();
                        string ch = hdata["Chart"].ToString();
                        if (ch.Contains("|"))
                        {
                            ChartMaster chart = new ChartMaster(constr);
                            ch = ch.Substring(0, ch.IndexOf("|") - 1);
                            chart.LoadData(ch);
                            ch = chart.ChartMasterId.ToString();
                        }
                        glTemps.GLTempList[i].GlTransactionChartId = ch;
                        glTemps.GLTempList[i].GlTransactionDescription = hdata["Description"].ToString();
                        glTemps.GLTempList[i].GlTransactionGross = AdFunction.Rounded(hdata["Gross"].ToString(), 2).ToString();
                        glTemps.GLTempList[i].GlTransactionNet = AdFunction.Rounded(hdata["Net"].ToString(), 2).ToString();
                        glTemps.GLTempList[i].GlTransactionTax = AdFunction.Rounded(hdata["Tax"].ToString(), 2).ToString();
                    }
                }
                #endregion
            }
            glTemps.UpdateTemp();
            return "";
        }
        [System.Web.Services.WebMethod]
        public static string SaveDataFromGrid(string rowValue)
        {
            string id = Guid.NewGuid().ToString("N");
            InvoiceSaveDataFromGrid(rowValue, id);

            return "";
        }
        [System.Web.Services.WebMethod]
        public static string GetNetTotal()
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            decimal total = 0;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);

            GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
            gltranTemps.LoadData();

            foreach (GlTransactionTemp gltran in gltranTemps.GLTempList)
            {
                total += Convert.ToDecimal(gltran.GlTransactionNet);
            }
            return total.ToString("0.00");
        }
        [System.Web.Services.WebMethod]
        public static string GetTaxTotal()
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            decimal total = 0;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);

            GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
            gltranTemps.LoadData();

            foreach (GlTransactionTemp gltran in gltranTemps.GLTempList)
            {
                total += Convert.ToDecimal(gltran.GlTransactionTax);
            }
            return total.ToString("0.00");
        }
        [System.Web.Services.WebMethod]
        public static string GetGrossTotal()
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            decimal total = 0;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);

            GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
            gltranTemps.LoadData();

            foreach (GlTransactionTemp gltran in gltranTemps.GLTempList)
            {
                total += Convert.ToDecimal(gltran.GlTransactionGross);
            }
            return total.ToString("0.00");
        }
        #endregion

        protected void ComboBoxBodycorp_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBoxUnit.Items.Clear();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string pid = "";
                if (ComboBoxBodycorp.Items.Count > 0 && !ComboBoxBodycorp.SelectedValue.Equals(""))
                {
                    pid = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "property_master"), "property_master_bodycorp_id", ComboBoxBodycorp.SelectedValue, "property_master_id");
                }
                string sql = "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` ";
                if (!pid.Equals(""))
                    sql += " where unit_master_property_id=" + pid;
                AjaxControlUtils.SetupComboBox(ComboBoxUnit, sql + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Code", constr, true);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
        }

        protected void TextBoxDate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBoxDue.Text = (DateTime.Parse(TextBoxDate.Text).AddMonths(1)).ToString("dd/MM/yyyy");
                TextBoxApply.Text = TextBoxDate.Text;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
        }


        /// <summary>
        /// Prepare JSon string for chart master search
        /// </summary>
        private void SetChartCodeNameJSonstring()
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            Odbc mydb = null;

            try
            {
                mydb = new Odbc(constr);

                string sql = "SELECT * FROM chart_master ";
                OdbcDataReader dr = mydb.Reader(sql);
                List<Hashtable> chartList = new List<Hashtable>();
                while (dr.Read())
                {
                    Hashtable items = new Hashtable();
                    items.Add("chart_master_code", DBSafeUtils.DBStrToStr(dr["chart_master_code"]));
                    items.Add("chart_master_name", DBSafeUtils.DBStrToStr(dr["chart_master_name"]));
                    chartList.Add(items);
                }

                JavaScriptSerializer helper = new JavaScriptSerializer();
                var st = helper.Serialize(chartList);
                hidenChartCodeName.Value = helper.Serialize(chartList).ToString();
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


        /// <summary>
        /// Check and set GST checkbox for transaction list
        /// </summary>
        private void SetGSTRegister()
        {
            if (Request.Cookies["bodycorpid"] != null)
            {
                Bodycorp body = new Bodycorp(AdFunction.conn);
                body.LoadData(Convert.ToInt32(Request.Cookies["bodycorpid"].Value));

                if (body.BodycorpNoGST == false)
                {
                    isGSTChecked.Value = "Yes";
                }
                else
                {
                    isGSTChecked.Value = "No";
                }
            }
        }



    }
}