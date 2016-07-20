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
using System.Web.Script.Serialization;
using System.Data.Odbc;

namespace sapp_sms
{
    public partial class invoicemasteredit : System.Web.UI.Page, IPostBackEventHandler
    {
        private const string TEMP_TYPE = "invoicemasteredit";
        private const string TYPE_ID = "1";
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Javascript setup
            Control[] wc = { jqGridTrans, LabelNet, LabelTax, LabelGross };
            JSUtils.RenderJSArrayWithCliendIds(Page, wc);
            #endregion

            string invoice_id = "";
            string mode = "";
            try
            {

                if (Request.QueryString["invoicemasterid"] != null) invoice_id = Request.QueryString["invoicemasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];

                if (!IsPostBack)
                {
                    Session["gobackstep"] = 1;
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                    AdFunction.Debtor_ComboBox(ComboBoxDebtor, "All");
                    //AjaxControlUtils.SetupComboBox(ComboBoxDebtor, "SELECT `debtor_master_id` AS `ID`, `debtor_master_name` AS `Code` FROM `debtor_master`", "ID", "Code", constr, false);
                    AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, false);
                    AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` LEFT JOIN `property_master` ON property_master_id=unit_master_property_id where property_master_bodycorp_id = " + Request.Cookies["bodycorpid"].Value + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Code", constr, true);

                    // Get chart master data as JSon string. 01/04/2016
                    SetChartCodeNameJSonstring();

                    // Link to GST registered. 01/04/2016
                    SetGSTRegister();

                    if (mode == "add")
                    {
                        #region Load Page
                        try
                        {

                            #region Initial ComboBox
                            #endregion
                            #region Initial Web Controls
                            if (Request.QueryString["debtorid"] != null)
                            {
                                AjaxControlUtils.ComboBoxSelection(ComboBoxDebtor, Request.QueryString["debtorid"].ToString(), false);
                                AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` WHERE `unit_master_debtor_id`=" + Request.QueryString["debtorid"].ToString() + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Code", constr, false);
                                ComboBoxDebtor.Enabled = false;
                            }
                            if (Request.Cookies["bodycorpid"].Value != null)
                            {
                                AjaxControlUtils.ComboBoxSelection(ComboBoxBodycorp, Request.Cookies["bodycorpid"].Value, false);
                                ComboBoxBodycorp.Enabled = false;
                            }
                            if (Request.QueryString["unitid"] != null)
                            {
                                AjaxControlUtils.ComboBoxSelection(ComboBoxUnit, Request.QueryString["unitid"].ToString(), false);
                                ComboBoxUnit.Enabled = false;
                            }
                            TextBoxDescription.Text = "";
                            InvoiceMaster i = new InvoiceMaster(constr);
                            TextBoxNum.Text = i.GetNextNumber(-1);
                            TextBoxDue.Text = "";
                            TextBoxApply.Text = "";     // Add 06/05/2016
                            TextBoxDate.Text = "";
                            LabelNet.Text = "0.00";
                            LabelTax.Text = "0.00";
                            LabelGross.Text = "0.00";
                            #endregion
                            #region Initial Temp File In Mysql
                            InvoiceMaster invoice = new InvoiceMaster(constr);
                            string login = HttpContext.Current.User.Identity.Name;
                            Sapp.General.User user = new User(constr_general);
                            user.LoadData(login);
                            GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
                            gltranTemps.Add(invoice.GetGLTranJSON());
                            #endregion

                            if (Request.Cookies["bodycorpid"] != null)
                            {
                                ComboBoxBodycorp.SelectedValue = Request.Cookies["bodycorpid"].Value;
                                ComboBoxBodycorp.Enabled = false;
                            }
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
                            #region Initial Web Controls
                            InvoiceMaster invoice = new InvoiceMaster(constr);
                            invoice.LoadData(Convert.ToInt32(invoice_id));
                            if (invoice.InvoiceMasterPaid > 0)
                            {
                                // Change to limited edit. 31/03/2016
                                //throw new Exception("Invoice has been paid");
                                isLimitedEdit.Value = "true";
                                TextBoxNum.Enabled = false;
                                ComboBoxBodycorp.Enabled = false;
                                ComboBoxUnit.Enabled = false;
                                ComboBoxDebtor.Enabled = false;
                                TextBoxDate.Enabled = false;
                                TextBoxDue.Enabled = false;
                                TextBoxApply.Enabled = false;

                                // hide "Add" and "Delete" button
                                ImageButtonDelete.Visible = false;
                                jqGridTrans.addVisible = "false";
                            }
                            invoice.LoadTransactions();
                            TextBoxDescription.Text = invoice.InvoiceMasterDescription;
                            TextBoxNum.Text = invoice.InvoiceMasterNum;
                            if (invoice.InvoiceMasterDue.HasValue)
                                TextBoxDue.Text = invoice.InvoiceMasterDue.Value.ToShortDateString();
                            TextBoxDate.Text = invoice.InvoiceMasterDate.ToShortDateString();
                            //TextBoxDate.Text = invoice.InvoiceMasterDate.ToShortDateString();
                            // Add 06/05/2016
                            if (invoice.InvoiceMasterApply.HasValue)
                            {
                                TextBoxApply.Text = invoice.InvoiceMasterApply.Value.ToShortDateString();
                            }
                            LabelNet.Text = invoice.InvoiceMasterNet.ToString("0.00");
                            LabelTax.Text = invoice.InvoiceMasterTax.ToString("0.00");
                            LabelGross.Text = invoice.InvoiceMasterGross.ToString("0.00");

                            AjaxControlUtils.ComboBoxSelection(ComboBoxDebtor, invoice.InvoiceMasterDebtorId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxBodycorp, invoice.InvoiceMasterBodycorpId, true);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxUnit, invoice.InvoiceMasterUnitId, true);

                            if (invoice.InvoiceMasterNum.Contains("REV"))
                                ImageButtonSave.Visible = false;

                            #endregion

                            #region Initial Temp File In Mysql
                            string login = HttpContext.Current.User.Identity.Name;
                            Sapp.General.User user = new User(constr_general);
                            user.LoadData(login);
                            GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
                            gltranTemps.Add(invoice.GetGLTranJSON());
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
                    // Add 15/04/2016
                    if (mode == "paste")
                    {
                        #region Load Page
                        try
                        {
                            #region Initial Web Controls
                            InvoiceMaster invoice = new InvoiceMaster(constr);
                            invoice.LoadData(Convert.ToInt32(invoice_id));
                            invoice.LoadTransactions();

                            TextBoxNum.Text = invoice.GetNextNumber(-1);

                            AjaxControlUtils.ComboBoxSelection(ComboBoxBodycorp, invoice.InvoiceMasterBodycorpId, true);
                            if (Request.Cookies["bodycorpid"] != null)
                            {
                                ComboBoxBodycorp.SelectedValue = Request.Cookies["bodycorpid"].Value;
                                ComboBoxBodycorp.Enabled = false;
                            }

                            AjaxControlUtils.ComboBoxSelection(ComboBoxUnit, invoice.InvoiceMasterUnitId, true);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxDebtor, invoice.InvoiceMasterDebtorId, false);

                            TextBoxDescription.Text = invoice.InvoiceMasterDescription;

                            LabelNet.Text = invoice.InvoiceMasterNet.ToString("0.00");
                            LabelTax.Text = invoice.InvoiceMasterTax.ToString("0.00");
                            LabelGross.Text = invoice.InvoiceMasterGross.ToString("0.00");
                            #endregion

                            #region Initial Temp File In Mysql

                            InvoiceMaster invoice_new = new InvoiceMaster(constr);
                            string login = HttpContext.Current.User.Identity.Name;
                            Sapp.General.User user = new User(constr_general);
                            user.LoadData(login);
                            GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
                            gltranTemps.Add(invoice_new.GetGLTranJSON());

                            gltranTemps.LoadData();
                            for (int i = 0; i < invoice.GltransactionList.Count(); i++)
                            {
                                Hashtable line = new Hashtable();

                                line.Add("gl_transaction_id", "i" + Guid.NewGuid().ToString("N"));
                                line.Add("gl_transaction_type_id", invoice.GltransactionList[i].GLTransactionTypeId);
                                line.Add("gl_transaction_ref", "");
                                line.Add("gl_transaction_chart_id", invoice.GltransactionList[i].GlTransactionChartId);
                                line.Add("gl_transaction_ref_type_id", invoice.GltransactionList[i].GLTransactionTypeId);
                                line.Add("gl_transaction_bodycorp_id", invoice.GltransactionList[i].GlTransactionBodycorpId);
                                line.Add("gl_transaction_unit_id", "");
                                line.Add("gl_transaction_description", invoice.GltransactionList[i].GlTransactionDescription);
                                line.Add("gl_transaction_net", invoice.GltransactionList[i].GlTransactionNet);
                                line.Add("gl_transaction_tax", invoice.GltransactionList[i].GlTransactionTax);
                                line.Add("gl_transaction_gross", invoice.GltransactionList[i].GlTransactionGross);
                                line.Add("gl_transaction_date", "");

                                GlTransactionTemp gltran = new GlTransactionTemp(constr);
                                gltran.LoadData(line);
                                gltranTemps.GLTempList.Add(gltran);
                            }

                            gltranTemps.UpdateTemp();
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }

                    // Add 15/05/2016
                    Sapp.SMS.System sys = new Sapp.SMS.System(constr);
                    sys.LoadData("INVAPPLYDATEDEFAULT");
                    HiddenFieldBaseDateBkn.Value = sys.SystemValue;
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
        protected void ImageButtonSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string invoice_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (!Page.IsValid) return;
                if (Request.QueryString["invoicemasterid"] != null) invoice_id = Request.QueryString["invoicemasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");
                Hashtable items = new Hashtable();

                #region Retrieve Invoice Values
                items.Add("invoice_master_unit_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxUnit, true));
                items.Add("invoice_master_debtor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxDebtor, false));
                items.Add("invoice_master_bodycorp_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxBodycorp, false));
                items.Add("invoice_master_type_id", 1);
                items.Add("invoice_master_num", DBSafeUtils.StrToQuoteSQL(TextBoxNum.Text));
                items.Add("invoice_master_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                items.Add("invoice_master_due", DBSafeUtils.DateToSQL(TextBoxDue.Text));
                items.Add("invoice_master_apply", DBSafeUtils.DateToSQL(TextBoxApply.Text));        // Add 05/062016
                items.Add("invoice_master_description", DBSafeUtils.StrToQuoteSQL(TextBoxDescription.Text));
                #endregion
                Odbc mydb = null;
                try
                {
                    mydb = new Odbc(constr);
                    mydb.StartTransaction();
                    if (mode == "add" || mode == "paste")
                    {
                        string g = GetGrossTotal();
                        if (!g.Equals("0.00"))
                        {
                            #region Add Cinvoice
                            InvoiceMaster invoice = new InvoiceMaster(constr);
                            Bodycorp b = new Bodycorp(AdFunction.conn);
                            b.LoadData(ComboBoxBodycorp.SelectedValue);
                            if (!b.CheckCloseOff(DateTime.Parse(TextBoxDate.Text)))
                            {
                                throw new Exception("Invoice before close date");
                            }
                            invoice.SetOdbc(mydb);
                            invoice_id = invoice.Add(items, true).ToString();
                            #endregion
                        }
                    }
                    else if (mode == "edit")
                    {
                        #region Update Cinvoice
                        InvoiceMaster invoice = new InvoiceMaster(constr);
                        InvoiceMaster invoicemaster = new InvoiceMaster(constr);
                        invoicemaster.LoadData(Convert.ToInt32(invoice_id));


                        Bodycorp b = new Bodycorp(AdFunction.conn);
                        b.LoadData(invoicemaster.InvoiceMasterBodycorpId);
                        if (!b.CheckCloseOff(invoicemaster.InvoiceMasterDate))
                        {
                            throw new Exception("Invoice before close date");
                        }
                        if (invoicemaster.InvoiceMasterPaid > 0)
                        {
                            //throw new Exception("Invoice has been paid");     // Change to limited edit. 31/03/2016
                        }
                        invoice.SetOdbc(mydb);

                        invoice.Update(items, Convert.ToInt32(invoice_id));
                        #endregion
                    }
                    if (invoice_id.Equals(""))
                        Response.Redirect(Request.Url.ToString(), false);
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
                    throw ex;
                }
                finally
                {
                    if (mydb != null) mydb.Close();
                }
                #region Redirect
                if (Request.QueryString["burl"] == null)
                    if (Session["gobackstep"].ToString() == "2")
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("invoicemaster.aspx", false);
                    }
                    else
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("invoicemaster.aspx", false);
                    }
                else
                {
                    Response.Redirect("~/cinvoices.aspx", false);
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
        private void ImageButtonDelete_Click(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string gl_transaction_id = args[1];
                //InvoiceMaster invoicemaster = new InvoiceMaster(constr);
                //invoicemaster.LoadData(Convert.ToInt32(int.Parse(Request.QueryString["invoicemasterid"])));


                //Bodycorp b = new Bodycorp(AdFunction.conn);
                //b.LoadData(invoicemaster.InvoiceMasterBodycorpId);
                //if (!b.CheckCloseOff(invoicemaster.InvoiceMasterDate))
                //{
                //    throw new Exception("Invoice before close date");
                //}
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
        protected void CustomValidatorDebtor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxDebtor.SelectedIndex > -1) args.IsValid = true; // -1 unselect, 0 null selected
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
                if (ComboBoxDebtor.SelectedIndex > -1) args.IsValid = true; // -1 unselect, 0 null selected
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
            GlTransactionTemps glTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE, constr_general, constr);
            glTemps.LoadData();
            Hashtable line = new Hashtable();

            if (hdata["GST"].ToString().Equals("Yes"))
            {
                Sapp.SMS.System s = new Sapp.SMS.System(constr);
                s.LoadData("GST");
                double gst = double.Parse(s.SystemValue);
                if (hdata["Tax"].ToString().Equals("") && !hdata["Gross"].ToString().Equals(""))
                {

                    decimal g = AdFunction.Rounded(hdata["Gross"].ToString(), 2);
                    decimal t = decimal.Parse((double.Parse(hdata["Gross"].ToString()) / (1 + gst) * gst).ToString("f2"));
                    decimal n = g - t;
                    hdata["Net"] = n;
                    hdata["Tax"] = t;
                }
                if (!hdata["Net"].ToString().Equals("") && hdata["Gross"].ToString().Equals("") && hdata["Tax"].ToString().Equals(""))
                {
                    double n = double.Parse(hdata["Net"].ToString());
                    hdata["Net"] = n;
                    hdata["Tax"] = (n * gst);
                    hdata["Gross"] = n + (n * gst);
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
                if (!hdata["Net"].ToString().Equals("") && hdata["Gross"].ToString().Equals("") && hdata["Tax"].ToString().Equals(""))
                {
                    double n = double.Parse(hdata["Net"].ToString());
                    hdata["Net"] = n;
                    hdata["Tax"] = 0;
                    hdata["Gross"] = n;
                }

            }


            if (line_id == "")
            {
                #region INSERT
                line.Add("gl_transaction_id", "i" + Guid.NewGuid().ToString("N")); // Create a unique id for insert unit
                line.Add("gl_transaction_type_id", TYPE_ID);
                line.Add("gl_transaction_ref", "");
                ChartMaster chart = new ChartMaster(constr);
                string ch = hdata["Chart"].ToString();
                ch = ch.Substring(0, ch.IndexOf("|") - 1);
                chart.LoadData(ch);
                line.Add("gl_transaction_chart_id", chart.ChartMasterId.ToString());
                line.Add("gl_transaction_ref_type_id", "1");
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
                decimal n = AdFunction.Rounded(hdata["Net"].ToString(), 2);
                decimal t = AdFunction.Rounded(hdata["Tax"].ToString(), 2);
                decimal g = AdFunction.Rounded(hdata["Gross"].ToString(), 2);
                if (g != n + t)
                    throw new Exception("Net + Tax not Equals Gross");
                #region UPDATE
                for (int i = 0; i < glTemps.GLTempList.Count; i++)
                {
                    if (glTemps.GLTempList[i].GlTransactionId == hdata["ID"].ToString())
                    {
                        string ch = hdata["Chart"].ToString();
                        if (ch.Contains("|"))
                        {
                            ChartMaster chart = new ChartMaster(constr);
                            ch = ch.Substring(0, ch.IndexOf("|") - 1);
                            chart.LoadData(ch);
                            ch = chart.ChartMasterId.ToString();
                        }
                        if ((hdata["ID"].ToString()[0] != 'i') && (hdata["ID"].ToString()[0] != 'u')) glTemps.GLTempList[i].GlTransactionId = "u" + hdata["ID"].ToString();
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

        protected void ComboBoxDebtor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ComboBoxUnit.SelectedValue.Equals("null"))
            {
                AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` where unit_master_debtor_id=" + ComboBoxDebtor.SelectedValue + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Code", AdFunction.conn, true);
                ComboBoxUnit.Enabled = true;
            }

        }

        protected void ComboBoxUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ComboBoxUnit.SelectedValue.Equals("null"))
            {
                Odbc o = new Odbc(AdFunction.conn);
                DataTable dt = o.ReturnTable("SELECT * FROM `unit_master` where unit_master_id=" + ComboBoxUnit.SelectedValue, "y1");
                string did = dt.Rows[0]["unit_master_debtor_id"].ToString();
                AdFunction.Debtor_ComboBox(ComboBoxDebtor);
                ComboBoxDebtor.SelectedValue = did;
                ComboBoxDebtor.Enabled = false;
            }
            else
            {
                AdFunction.Debtor_ComboBox(ComboBoxDebtor, "2");
                ComboBoxDebtor.Enabled = true;
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