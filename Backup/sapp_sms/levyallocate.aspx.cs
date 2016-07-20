using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using Sapp.Common;
using Sapp.JQuery;
using Sapp.SMS;
using Sapp.Data;
using Sapp.General;
using System.Threading;
namespace sapp_sms
{
    public partial class levyallocate : System.Web.UI.Page, IPostBackEventHandler
    {
        private Odbc mydb = new Odbc(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
        private const string temp_list_type = "levylist";
        private const string temp_allocate_type = "levyallocate";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridLevy };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (Request.QueryString["installment"] != null) Session["levyallocate_installment"] = Request.QueryString["installment"].ToString();
                if (!IsPostBack)
                {

                    #region Create Page Session
                    Session["startdate"] = Request.QueryString["startdate"].ToString();
                    Session["bodycorpid"] = Request.Cookies["bodycorpid"].Value;
                    #endregion
                    #region Initial Variables
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                    string bodycorp_id = Request.Cookies["bodycorpid"].Value;
                    string start_date = Request.QueryString["startdate"].ToString();
                    DateTime startDate = Convert.ToDateTime(Request.QueryString["startdate"]);
                    startDate = Convert.ToDateTime("01/" + startDate.Month + "/" + startDate.Year);
                    ClientScriptManager cs = Page.ClientScript;
                    string colNames = "";
                    for (int i = 1; i < 25; i++)
                    {
                        colNames += "'" + startDate.AddMonths(i - 1).Month + "/" + startDate.AddMonths(i - 1).Year.ToString() + "',";
                    }
                    colNames = colNames.Substring(0, colNames.Length - 1);
                    cs.RegisterArrayDeclaration("M", colNames);
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                    LabelBodycorp.Text = bodycorp.BodycorpCode + " " + bodycorp.BodycorpName;
                    TextBoxDescription.Text = "";

                    // Add 15/05/2016
                    Sapp.SMS.System sys = new Sapp.SMS.System(constr);
                    sys.LoadData("INVAPPLYDATEDEFAULT");
                    HiddenFieldBaseDateBkn.Value = sys.SystemValue;

                    if ("InvoiceDate".Equals(sys.SystemValue))
                    {
                        TextBoxApplyDateM.Text = "0";
                        TextBoxApplyDateD.Text = "01";
                    }
                    else
                    {
                        TextBoxApplyDateM.Text = "0";
                        TextBoxApplyDateD.Text = "20";
                    }
                    #endregion
                    #region Load Levy Info From Temp File
                    Temp list_temp = new Temp(constr_general);
                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(constr_general);
                    user.LoadData(login);
                    if (list_temp.TempExist(user.UserId, temp_list_type))
                    {
                        int installment = Convert.ToInt32(Request.QueryString["installment"]);
                        Temp allocate_temp = new Temp(constr_general);
                        Hashtable temp_items = new Hashtable();
                        temp_items.Add("temp_user_id", user.UserId);
                        temp_items.Add("temp_type", DBSafeUtils.StrToQuoteSQL(temp_allocate_type));
                        temp_items.Add("temp_date", DBSafeUtils.DateToSQL(DateTime.Today));
                        temp_items.Add("temp_expire", DBSafeUtils.DateToSQL(DateTime.Today.AddDays(1)));
                        Hashtable td_items = new Hashtable();
                        td_items.Add("bodycorp_id", bodycorp_id);
                        list_temp.LoadData(user.UserId, temp_list_type);
                        string json = list_temp.TempData;
                        Hashtable jsonObj = (Hashtable)JSON.JsonDecode(json);
                        ArrayList levyList = (ArrayList)jsonObj["levies"];
                        ArrayList allocateList = new ArrayList();
                        foreach (Hashtable levy_items in levyList)
                        {
                            Hashtable allocate_items = new Hashtable();
                            allocate_items.Add("ID", levy_items["ID"].ToString());
                            allocate_items.Add("Chart", levy_items["Chart"].ToString());
                            allocate_items.Add("Field", levy_items["Description"].ToString());
                            allocate_items.Add("Total", levy_items["Net"].ToString());
                            allocate_items.Add("Scale", levy_items["Scale"].ToString());
                            for (int i = 1; i < (installment + 1); i++)
                            {
                                allocate_items.Add("M" + i, "0.00");
                            }
                            allocateList.Add(allocate_items);
                        }
                        td_items.Add("allocates", allocateList);
                        temp_items.Add("temp_data", DBSafeUtils.StrToQuoteSQL(JSON.JsonEncode(td_items)));
                        if (!allocate_temp.TempExist(user.UserId, temp_allocate_type))
                        {
                            allocate_temp.Add(temp_items);
                        }
                        else
                        {
                            allocate_temp.LoadData(user.UserId, temp_allocate_type);
                            allocate_temp.Delete();
                            allocate_temp.Add(temp_items);
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
                if (args[0] == "ImageButtonSubmit")
                {
                    ImageButtonSubmit_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;

            int installment = Convert.ToInt32(HttpContext.Current.Session["levyallocate_installment"]);
            string login = HttpContext.Current.User.Identity.Name;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);
            Temp temp_allocate = new Temp(constr_general);
            if (temp_allocate.TempExist(user.UserId, temp_allocate_type))
            {

                temp_allocate.LoadData(user.UserId, temp_allocate_type);
                string json = temp_allocate.TempData;
                Hashtable items = (Hashtable)JSON.JsonDecode(json);
                ArrayList allocateList = (ArrayList)items["allocates"];
                DataTable dt = new DataTable("levies");
                dt.Columns.Add("ID");
                dt.Columns.Add("Chart");
                dt.Columns.Add("Field");
                dt.Columns.Add("Total");
                dt.Columns.Add("Scale");
                for (int i = 1; i < (installment + 1); i++)
                {
                    dt.Columns.Add("M" + i);
                }

                foreach (Hashtable allocate_items in allocateList)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = allocate_items["ID"].ToString();
                    ChartMaster chart = new ChartMaster(constr);
                    chart.LoadData(Convert.ToInt32(allocate_items["Chart"]));
                    dr["Chart"] = chart.ChartMasterCode;
                    dr["Field"] = allocate_items["Field"].ToString().Replace('|', '-');
                    dr["Total"] = allocate_items["Total"].ToString();
                    ScaleType scaletype = new ScaleType(constr);
                    scaletype.LoadData(Convert.ToInt32(allocate_items["Scale"]));
                    dr["Scale"] = scaletype.ScaleTypeCode;
                    for (int i = 1; i < (installment + 1); i++)
                    {
                        dr["M" + i] = allocate_items["M" + i].ToString();
                    }
                    dt.Rows.Add(dr);
                }
                string sqlfromstr = "FROM `" + dt.TableName + "`";
                string sqlselectstr = "SELECT `ID`, `Chart`, `Field`, `Total`, `Scale`, ";
                for (int i = 1; i < (installment + 1); i++)
                {
                    sqlselectstr += "`M" + i + "`,";
                }
                sqlselectstr = sqlselectstr.Substring(0, sqlselectstr.Length - 1) + " FROM (SELECT *";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            return "{}";
        }
        #endregion
        #region Web Controls Methods
        public static string[] arg;
        public static int installment = 0;
        public static string bodycorp_id;
        public static string start_date;
        public static string login;
        protected void ImageButtonSubmit_Click(string[] args)
        {
            try
            {
                start_date = Request.QueryString["startdate"].ToString();
                installment = Convert.ToInt32(Request.QueryString["installment"]);
                bodycorp_id = Request.Cookies["bodycorpid"].Value;
                login = HttpContext.Current.User.Identity.Name;
                arg = args;
                //Thread t = new Thread(new ThreadStart(Go));
                //t.Start();
                Go();
                Response.Redirect("levies.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
        }
        #endregion
        #region User Functions
        private Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl.ID == controlID) return rootControl;

            foreach (Control controlToSearch in rootControl.Controls)
            {
                Control controlToReturn =
                    FindControlRecursive(controlToSearch, controlID);
                if (controlToReturn != null) return controlToReturn;
            }
            return null;
        }
        #endregion

        public void Go()
        {
            try
            {
                #region Define Variables
                Session["LevyFinish"] = false;

                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                DataTable leviesDT = ReportDT.getTable(constr, "levies");
                leviesDT = ReportDT.FilterDT(leviesDT, "", "levy_batch_id desc");
                int maxb = 1;
                if (leviesDT.Rows.Count > 0)
                {
                    int.TryParse(leviesDT.Rows[0]["levy_batch_id"].ToString(), out maxb);
                }
                maxb += 1;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string json = arg[1];
                DateTime startDate = Convert.ToDateTime(start_date);
                startDate = Convert.ToDateTime("01/" + startDate.Month + "/" + startDate.Year);
                Sapp.General.User user = new User(constr_general);
                #endregion
                user.LoadData(login);
                Temp temp = new Temp(constr_general);
                if (temp.TempExist(user.UserId, temp_allocate_type))
                {
                    ArrayList allocateList = (ArrayList)JSON.JsonDecode(json);
                    try
                    {
                        mydb.StartTransaction();

                        #region Go Through Each Unit
                        Bodycorp bodycorp = new Bodycorp(constr);
                        bodycorp.SetOdbc(mydb);
                        bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                        List<PropertyMaster> propertyList = bodycorp.GetPropertyList();

                        foreach (PropertyMaster property in propertyList)
                        {
                            #region Every Month
                            property.SetOdbc(mydb);
                            List<UnitMaster> unitList = property.GetUnitList("");
                            for (int i = 1; i < (installment + 1); i++)
                            {
                                #region Create Levy
                                Hashtable levy_items = new Hashtable();
                                levy_items.Add("levy_bodycorp_id", Convert.ToInt32(bodycorp_id));
                                levy_items.Add("levy_date", DBSafeUtils.DateToSQL("01/" + startDate.AddMonths(i - 1).Month + "/" + startDate.AddMonths(i - 1).Year));

                                levy_items.Add("levy_description", DBSafeUtils.StrToQuoteSQL(TextBoxDescription.Text + " " + startDate.AddMonths(i - 1).Month + "/" + startDate.AddMonths(i - 1).Year));
                                levy_items.Add("levy_net", 0);
                                levy_items.Add("levy_batch_id", maxb);
                                levy_items.Add("levy_tax", 0);
                                levy_items.Add("levy_gross", 0);
                                Levy levy = new Levy(constr);
                                levy.SetOdbc(mydb);
                                int levy_id = levy.Add(levy_items, true);
                                levy.LoadData(levy_id);
                                #endregion
                                #region Add Levy Lines

                                foreach (Hashtable line_items in allocateList)
                                {
                                    Hashtable _items = new Hashtable();
                                    ChartMaster chart = new ChartMaster(constr);
                                    chart.SetOdbc(mydb);
                                    chart.LoadData(line_items["Chart"].ToString());
                                    _items.Add("levy_line_chart_id", chart.ChartMasterId);
                                    _items.Add("levy_line_description", DBSafeUtils.StrToQuoteSQL(line_items["Field"].ToString()));
                                    decimal amount = 0;
                                    decimal.TryParse(line_items["M" + i].ToString(), out amount);
                                    _items.Add("levy_line_amount", amount);
                                    ScaleType scaletype = new ScaleType(constr);
                                    scaletype.SetOdbc(mydb);
                                    scaletype.LoadData(line_items["Scale"].ToString());
                                    _items.Add("levy_line_scale_id", scaletype.ScaleTypeId);
                                    levy.AddLine(_items);
                                }
                                #endregion
                                Hashtable unit_invs = new Hashtable();
                                foreach (UnitMaster unit in unitList)
                                {
                                    if (CheckBoxConsolidate.Checked)
                                    {
                                        //invs invoice lines
                                        Hashtable inv_line = new Hashtable();
                                        inv_line.Add("unit_master_id", unit.UnitMasterId.ToString());
                                        inv_line.Add("invoice_description", levy.LevyDescription);
                                        Sapp.SMS.System system = new Sapp.SMS.System(constr);
                                        system.SetOdbc(mydb);
                                        system.LoadData("GENERALLEVY");
                                        ChartMaster chart = new ChartMaster(constr);
                                        chart.SetOdbc(mydb);
                                        chart.LoadData(system.SystemValue);
                                        inv_line.Add("chart_master_id", chart.ChartMasterId);
                                        decimal invoice_net = 0;
                                        foreach (Hashtable line_items in allocateList)
                                        {
                                            ScaleType scaletype = new ScaleType(constr);
                                            scaletype.SetOdbc(mydb);
                                            scaletype.LoadData(line_items["Scale"].ToString());
                                            decimal r = unit.GetScale(scaletype.ScaleTypeId);
                                            decimal n = Convert.ToDecimal(line_items["M" + i]);
                                            invoice_net += n * r;
                                        }
                                        inv_line.Add("invoice_net", invoice_net.ToString("0.00"));
                                        if (invoice_net != 0)
                                        {
                                            #region Create Invoice
                                            InvoiceMaster invoice = new InvoiceMaster(constr);
                                            invoice.SetOdbc(mydb);
                                            Hashtable inv_items = new Hashtable();
                                            inv_items.Add("invoice_master_num", DBSafeUtils.StrToQuoteSQL(invoice.GetNextInvoiceNumber()));
                                            inv_items.Add("invoice_master_type_id", 1);
                                            inv_items.Add("invoice_master_debtor_id", unit.UnitMasterDebtorId);
                                            inv_items.Add("invoice_master_bodycorp_id", levy.LevyBodycorpId);
                                            inv_items.Add("invoice_master_unit_id", unit.UnitMasterId);
                                            DateTime billingDate = Convert.ToDateTime(TextBoxBillingDate.Text + "/" + startDate.AddMonths(i - 1).Month + "/" + startDate.AddMonths(i - 1).Year);
                                            DateTime billingDue = Convert.ToDateTime(TextBoxDueD.Text + "/" + startDate.AddMonths(i - 1 + int.Parse(TextBoxDueM.Text)).Month + "/" + startDate.AddMonths(i - 1 + int.Parse(TextBoxDueM.Text)).Year);

                                            // Add 15/05/2016
                                            DateTime applyDate;
                                            if (TextBoxApplyDateM.Text.Equals("") || TextBoxApplyDateD.Equals(""))
                                            {
                                                Sapp.SMS.System sys = new Sapp.SMS.System(constr);
                                                sys.LoadData("INVAPPLYDATEDEFAULT");

                                                if ("InvoiceDate".Equals(sys.SystemValue))
                                                {
                                                    applyDate = billingDate;
                                                }
                                                else
                                                {
                                                    applyDate = billingDue;
                                                }
                                            } else {
                                                applyDate = Convert.ToDateTime(TextBoxApplyDateD.Text + "/" + startDate.AddMonths(i - 1 + int.Parse(TextBoxApplyDateM.Text)).Month + "/" + startDate.AddMonths(i - 1 + int.Parse(TextBoxApplyDateM.Text)).Year);
                                            }

                                            inv_items.Add("invoice_master_date", DBSafeUtils.DateToSQL(billingDate));
                                            inv_items.Add("invoice_master_due", DBSafeUtils.DateToSQL(billingDue));

                                            inv_items.Add("invoice_master_apply", DBSafeUtils.DateToSQL(applyDate));        // Add 15/05/2016

                                            inv_items.Add("invoice_master_description", DBSafeUtils.StrToQuoteSQL(levy.LevyDescription));
                                            inv_items.Add("invoice_master_batch_id", levy.LevyId);
                                            inv_items.Add("invoice_master_net", 0);
                                            inv_items.Add("invoice_master_tax", 0);
                                            inv_items.Add("invoice_master_gross", 0);
                                            inv_items.Add("invoice_master_paid", 0);
                                            int invoice_id = invoice.Add(inv_items, true);
                                            invoice.LoadData(invoice_id);
                                            #endregion
                                            #region Create Invoice Line
                                            Hashtable line_items = new Hashtable();
                                            line_items.Add("gl_transaction_type_id", 1);
                                            line_items.Add("gl_transaction_ref", DBSafeUtils.StrToQuoteSQL(invoice_id.ToString()));
                                            chart.LoadData(Convert.ToInt32(inv_line["chart_master_id"]));
                                            if (chart.ChartMasterRechargeToId == null)
                                            {
                                                line_items.Add("gl_transaction_chart_id", chart.ChartMasterId);
                                            }
                                            else
                                            {
                                                line_items.Add("gl_transaction_chart_id", chart.ChartMasterRechargeToId);
                                            }
                                            line_items.Add("gl_transaction_unit_id", Convert.ToInt32(inv_line["unit_master_id"]));
                                            line_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(inv_line["invoice_description"].ToString()));
                                            line_items.Add("gl_transaction_batch_id", levy.LevyId);
                                            decimal line_net = Convert.ToDecimal(inv_line["invoice_net"]);
                                            system.LoadData("GST");
                                            decimal gst = Convert.ToDecimal(system.SystemValue);
                                            decimal line_tax = line_net * gst;
                                            decimal line_gross = line_net + line_tax;
                                            line_items.Add("gl_transaction_net", line_net);
                                            line_items.Add("gl_transaction_tax", line_tax);
                                            line_items.Add("gl_transaction_gross", line_gross);
                                            line_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(invoice.InvoiceMasterDate));
                                            invoice.AddGlTran(line_items, true);
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        ArrayList lineList = new ArrayList();
                                        foreach (Hashtable line_items in allocateList)
                                        {
                                            Hashtable inv_line = new Hashtable();
                                            inv_line.Add("unit_master_id", unit.UnitMasterId.ToString());
                                            inv_line.Add("invoice_description", line_items["Field"].ToString());
                                            ChartMaster chart = new ChartMaster(constr);
                                            chart.SetOdbc(mydb);
                                            chart.LoadData(line_items["Chart"].ToString());
                                            inv_line.Add("chart_master_id", chart.ChartMasterId);
                                            decimal invoice_net = 0;
                                            ScaleType scaletype = new ScaleType(constr);
                                            scaletype.SetOdbc(mydb);
                                            scaletype.LoadData(line_items["Scale"].ToString());
                                            invoice_net = Convert.ToDecimal(line_items["M" + i]) * unit.GetScale(scaletype.ScaleTypeId);
                                            inv_line.Add("invoice_net", invoice_net.ToString("0.00"));
                                            if (invoice_net != 0)
                                            {
                                                lineList.Add(inv_line);
                                            }
                                        }
                                        if (lineList.Count > 0)
                                        {
                                            #region Create Invoice
                                            InvoiceMaster invoice = new InvoiceMaster(constr);
                                            invoice.SetOdbc(mydb);
                                            Hashtable inv_items = new Hashtable();
                                            inv_items.Add("invoice_master_num", DBSafeUtils.StrToQuoteSQL(invoice.GetNextInvoiceNumber()));
                                            inv_items.Add("invoice_master_type_id", 1);
                                            inv_items.Add("invoice_master_debtor_id", unit.UnitMasterDebtorId);
                                            inv_items.Add("invoice_master_bodycorp_id", levy.LevyBodycorpId);
                                            inv_items.Add("invoice_master_unit_id", unit.UnitMasterId);
                                            DateTime billingDate = Convert.ToDateTime(TextBoxBillingDate.Text + "/" + startDate.AddMonths(i - 1).Month + "/" + startDate.AddMonths(i - 1).Year);
                                            inv_items.Add("invoice_master_date", DBSafeUtils.DateToSQL(billingDate));

                                            // Add 15/05/2016
                                            if (TextBoxApplyDateM.Text.Equals("") || TextBoxApplyDateD.Equals(""))
                                            {
                                                Sapp.SMS.System sys = new Sapp.SMS.System(constr);
                                                sys.LoadData("INVAPPLYDATEDEFAULT");

                                                if ("InvoiceDate".Equals(sys.SystemValue))
                                                {
                                                    inv_items.Add("invoice_master_apply", DBSafeUtils.DateToSQL(billingDate)); 
                                                }
                                            }
                                            else
                                            {
                                                DateTime applyDate = Convert.ToDateTime(TextBoxApplyDateD.Text + "/" + startDate.AddMonths(i - 1 + int.Parse(TextBoxApplyDateM.Text)).Month + "/" + startDate.AddMonths(i - 1 + int.Parse(TextBoxApplyDateM.Text)).Year);
                                                inv_items.Add("invoice_master_apply", DBSafeUtils.DateToSQL(billingDate)); 
                                            }

                                            inv_items.Add("invoice_master_description", DBSafeUtils.StrToQuoteSQL(levy.LevyDescription));
                                            inv_items.Add("invoice_master_batch_id", levy.LevyId);
                                            inv_items.Add("invoice_master_net", 0);
                                            inv_items.Add("invoice_master_tax", 0);
                                            inv_items.Add("invoice_master_gross", 0);
                                            inv_items.Add("invoice_master_paid", 0);
                                            int invoice_id = invoice.Add(inv_items, true);
                                            invoice.LoadData(invoice_id);
                                            #endregion
                                            #region Create Invoice Line
                                            foreach (Hashtable inv_line in lineList)
                                            {
                                                Hashtable line_items = new Hashtable();
                                                Sapp.SMS.System system = new Sapp.SMS.System(constr);
                                                system.SetOdbc(mydb);
                                                line_items.Add("gl_transaction_type_id", 1);
                                                line_items.Add("gl_transaction_ref", DBSafeUtils.StrToQuoteSQL(invoice_id.ToString()));
                                                ChartMaster chart = new ChartMaster(constr);
                                                chart.SetOdbc(mydb);
                                                chart.LoadData(Convert.ToInt32(inv_line["chart_master_id"]));
                                                if (chart.ChartMasterRechargeToId == null)
                                                {
                                                    line_items.Add("gl_transaction_chart_id", chart.ChartMasterId);
                                                }
                                                else
                                                {
                                                    line_items.Add("gl_transaction_chart_id", chart.ChartMasterRechargeToId);
                                                }
                                                line_items.Add("gl_transaction_unit_id", Convert.ToInt32(inv_line["unit_master_id"]));
                                                line_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(inv_line["invoice_description"].ToString()));
                                                decimal line_net = Convert.ToDecimal(inv_line["invoice_net"]);
                                                system.LoadData("GST");
                                                decimal gst = Convert.ToDecimal(system.SystemValue);
                                                decimal line_tax = line_net * gst;
                                                decimal line_gross = line_net + line_tax;
                                                line_items.Add("gl_transaction_net", line_net);
                                                line_items.Add("gl_transaction_tax", line_tax);
                                                line_items.Add("gl_transaction_gross", line_gross);
                                                line_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(invoice.InvoiceMasterDate));
                                                invoice.AddGlTran(line_items, true);
                                            }

                                            #endregion
                                        }
                                    }
                                }
                            }
                            #endregion


                        }
                        #endregion

                        if (Session["levylist_budget_master_ids"] != null)
                        {
                            ArrayList bmList = (ArrayList)JSON.JsonDecode(Session["levylist_budget_master_ids"].ToString());
                            foreach (string bm_id in bmList)
                            {
                                BudgetMaster bm = new BudgetMaster(constr);
                                bm.SetOdbc(mydb);
                                bm.LoadData(Convert.ToInt32(bm_id));
                                Hashtable items = new Hashtable();
                                items.Add("budget_master_used", 1);
                                bm.Update(items);
                            }
                        }
                        mydb.Commit();
                        Session["LevyFinish"] = true;
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}