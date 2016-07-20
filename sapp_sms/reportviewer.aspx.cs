//070713
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using Sapp.SMS;
using System.Diagnostics;
namespace sapp_sms
{
    public partial class reportviewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    string report_id = "";
                    string args = "";
                    if (Request.QueryString["reportid"] != null) report_id = Request.QueryString["reportid"].ToString();
                    if (Request.QueryString["args"] != null) args = Request.QueryString["args"].ToString();
                    if (report_id == "activityreport")
                    {
                        ActivityReport(args.Split('|'));
                        Page.Title = "Activity Report";
                    }
                    else if (report_id == "proprietorlist")
                    {
                        DebtorListReport(args.Split('|'));
                        Page.Title = "Debtor List Report";
                    }
                    else if (report_id == "unitlist")
                    {
                        UnitListReport(args.Split('|'));
                        Page.Title = "Unit List Report";
                    }
                    else if (report_id == "profitloss")
                    {
                        ProfixLoss(args.Split('|'));
                        Page.Title = "Profit and Loss Report";
                    }
                    else if (report_id == "balancesheet")
                    {
                        BalanceSheet(args.Split('|'));
                        Page.Title = "Balance Sheet Report";
                    }
                    else if (report_id == "chartofaccount")
                    {
                        COAReport(args.Split('|'));
                        Page.Title = "COA Report";
                    }
                    else if (report_id == "unitaccount")
                    {
                        UnitAccount(args.Split('|'));
                        Page.Title = "Unit Account Report";
                    }
                    else if (report_id == "trialbalance")
                    {
                        Trialbalance(args.Split('|'));
                        Page.Title = "Trial Balance Report";
                    }
                    else if (report_id == "creditoraged")
                    {
                        CreditorAged(args.Split('|'));
                        //Page.Title = "Creditor Aged Report";
                    }
                    else if (report_id == "proprietoraged")
                    {
                        ProprietorAged(args.Split('|'));
                        //Page.Title = "Proprietor Aged Report";
                    }
                    else if (report_id == "cashposition")
                    {
                        CashPosition(args.Split('|'));
                        Page.Title = "Cash Position Report";
                    }
                    else if (report_id == "gstreport")
                    {
                        Report_GST(args.Split('|'));
                        Page.Title = "GST Report Invoice Base Detail";
                    }
                    else if (report_id == "gstrereport")
                    {
                        Report_GSTRe(args.Split('|'));
                        Page.Title = "GST Return Report Detail";
                    }
                    else if (report_id == "proprietorageddetail")
                    {
                        Report_PDetailAged(args.Split('|'));
                        Page.Title = "Proprietor Detail Aged Report";
                    }
                    else if (report_id == "unitstatement")
                    {
                        UnitStatement(args.Split('|'));
                        Page.Title = "Unit Statement Report";
                    }
                    else if (report_id == "CreditorActivity")
                    {
                        CreditorActivity(args.Split('|'));
                        Page.Title = "Creditor Activity Report";
                    }
                    else if (report_id == "BMPF")
                    {
                        BMPF(args.Split('|'), Request.QueryString["chartid"]);
                        //Page.Title = "Statement of Movement in Equity Building Maintenance Project Fund";
                    }
                    else if (report_id == "AF")
                    {
                        AF(args.Split('|'));
                        Page.Title = "Statement of Movement in Equity Accumulated Fund";
                    }
                    else if (report_id == "bankreconciliation")
                    {
                        bankreconciliation(args.Split('|'));
                        Page.Title = "BANK RECONCILIATION";
                    }
                    else if (report_id == "GSTReDetail")
                    {
                        GSTReDetail(args.Split('|'));
                        Page.Title = "GST Report Cash Base Detail";
                    }
                    else if (report_id == "BankReconciliationDetail")
                    {
                        BankReconciliationDetail(args.Split('|'));
                        Page.Title = "Bank Reconciliation Detail";
                    }
                    else if (report_id == "GST")
                    {
                        GST(args.Split('|'));
                        Page.Title = "GST Reconciliation";
                    }
                    else if (report_id == "JournalReport")
                    {
                        JournalReport(args.Split('|'));
                        Page.Title = "Journal Report";
                    }
                    else if (report_id == "ProprietorStatementSummary")
                    {
                        // Add 20/04/2016
                        ProprietorStatementSummary(args.Split('|'));
                        Page.Title = "Proprietor Statement Summary";
                    }
                    else if (report_id == "InvoiceSendSummary")
                    {
                        // Add 24/04/2016
                        InvoiceSendSummary(args.Split('|'));
                        Page.Title = "Invoice Send Summary";
                    }
                    stopwatch.Stop();
                    TimeSpan timespan = stopwatch.Elapsed;
                    double seconds = timespan.TotalSeconds;
                    Label1.Text = seconds.ToString();
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region User Functions
        private void GST(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string account_id = args[0];
                string start_date = args[1];
                string end_date = args[2];

                GSTReconciliation2 Report = new GSTReconciliation2(constr, Server.MapPath("templates/GSTReconciliation2.rdlc"), ReportViewer1);
                Report.SetReportInfo(int.Parse(account_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void BankReconciliationDetail(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string account_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                string batch = args[3];
                BReconciliationHistory Report = new BReconciliationHistory(constr, Server.MapPath("templates/BRecoinciliationDetail.rdlc"), ReportViewer1);
                Report.SetReportInfo(int.Parse(account_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), batch);
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void GSTReDetail(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                GSTCashDetail Report = new GSTCashDetail(constr, Server.MapPath("templates/GSTCash.rdlc"), ReportViewer1);
                Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
                Report.Print();

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void bankreconciliation(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                BRecoinciliation Report = new BRecoinciliation(constr, Server.MapPath("templates/BRecoinciliation.rdlc"), ReportViewer1);
                Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void AF(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                AF Report = new AF(constr, Server.MapPath("templates/AF.rdlc"), ReportViewer1);
                Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void BMPF(string[] args, string chartid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                string pt = "Statement of Movement in ";
                ChartMaster cm = new ChartMaster(constr);
                cm.LoadData(int.Parse(chartid));
                Page.Title = pt + cm.ChartMasterName;
                if (chartid.Equals("285"))
                {
                    AF(args);
                }
                else
                {
                    BMPF Report = new BMPF(constr, Server.MapPath("templates/BMPF.rdlc"), ReportViewer1);
                    Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), chartid, "bcs_report");    // Update 11/05/2016
                    Report.Print();
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        //private void CF(string[] args)
        //{
        //    try
        //    {
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        string bodycorp_id = args[0];
        //        string start_date = args[1];
        //        string end_date = args[2];
        //        CF Report = new CF(constr, Server.MapPath("templates/CK.rdlc"), ReportViewer1);
        //        Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
        //        Report.Print();
        //    }
        //    catch (Exception ex)
        //    {
        //       HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
        //    }
        //}
        //private void LTMF(string[] args)
        //{
        //    try
        //    {
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        string bodycorp_id = args[0];
        //        string start_date = args[1];
        //        string end_date = args[2];
        //        LTMF Report = new LTMF(constr, Server.MapPath("templates/LTMF.rdlc"), ReportViewer1);
        //        Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
        //        Report.Print();
        //    }
        //    catch (Exception ex)
        //    {
        //       HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
        //    }
        //}
        //private void BS(string[] args)
        //{
        //    try
        //    {
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        string bodycorp_id = args[0];
        //        string start_date = args[1];
        //        string end_date = args[2];
        //        BS Report = new BS(constr, Server.MapPath("templates/BS.rdlc"), ReportViewer1);
        //        Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
        //        Report.Print();
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
        //    }
        //}




        private void CreditorActivity(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                ReportCreditorActivity Report = new ReportCreditorActivity(constr, Server.MapPath("templates/CreditorActivity.rdlc"), ReportViewer1);
                Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void UnitStatement(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                string unit_id = args[3];
                ReportUnitStatement Report = new ReportUnitStatement(constr, Server.MapPath("templates/statement_template.rdlc"), ReportViewer1);
                Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), unit_id);
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void Report_PDetailAged(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                string chartid = Request.QueryString["chartid"].ToString();
                string outstandingflg = Request.QueryString["outstandingflg"].ToString();   // Add 06/04/2016
                ReportProprietorAgedDetail Report = new ReportProprietorAgedDetail(constr, Server.MapPath("templates/ProprietorDetailedAged.rdlc"), ReportViewer1);
                Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), chartid, outstandingflg);
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void Report_GSTRe(string[] args)
        {
            try
            {


                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                ReportGSTRe Report = new ReportGSTRe(constr, Server.MapPath("templates/GSTReconciliations.rdlc"), ReportViewer1);
                Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void Report_GST(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                GSTInvoiceDetail Report = new GSTInvoiceDetail(constr, Server.MapPath("templates/GSTInvoice.rdlc"), ReportViewer1);
                Report.SetReportInfo(Convert.ToInt32(bodycorp_id), DateTime.Parse(start_date), DateTime.Parse(end_date));
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void CashPosition(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                ReportCashPosition Report = new ReportCashPosition(constr, Server.MapPath("templates/CashPosition.rdlc"), ReportViewer1);
                Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void ProprietorAged(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                string type = Request.QueryString["type"].ToString();
                string chartid = Request.QueryString["chartid"].ToString();

                string showname = Request.QueryString["showname"].ToString();       // Add 20/05/2016
                string template = null;
                if ("True".Equals(showname))
                {
                    if (type.Equals("1") || type.Equals("2"))
                    {
                        template = "templates/ProprietorAged.rdlc";
                    }
                    else
                    {
                        template = "templates/ProprietorAgedInv.rdlc";
                    }
                }
                else
                {
                    if (type.Equals("1") || type.Equals("2"))
                    {
                        template = "templates/ProprietorAged2.rdlc";
                    }
                    else
                    {
                        template = "templates/ProprietorAgedInv2.rdlc";
                    }
                }

                if (type.Equals("1"))
                {
                    Page.Title = "Proprietor Aged Due Date Base";
                    ReportProprietorAged Report = new ReportProprietorAged(constr, Server.MapPath(template), ReportViewer1);        // Update 20/05/2016
                    Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), chartid);
                    Report.Print();
                }
                else if (type.Equals("2"))
                {
                    // Add 14/05/2016
                    Page.Title = "Proprietor Aged Apply Date Base";
                    ReportProprietorAged Report = new ReportProprietorAged(constr, Server.MapPath(template), ReportViewer1);        // Update 20/05/2016
                    Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), chartid, "ApplyDate");
                    Report.Print();
                }
                else
                {
                    Page.Title = "Proprietor Aged Invoice Base";
                    ProprietorAgedInvBase Report = new ProprietorAgedInvBase(constr, Server.MapPath("templates/ProprietorAgedInv.rdlc"), ReportViewer1);
                    Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
                    Report.Print();
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void CreditorAged(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];

                string type = Request.QueryString["type"].ToString();
                if (type.Equals("1"))
                {
                    Page.Title = "Creditor Aged Due Date Base";
                    ReportCreditorAged Report = new ReportCreditorAged(constr, Server.MapPath("templates/CreditorAged.rdlc"), ReportViewer1);
                    Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
                    Report.Print();
                }
                else
                {
                    Page.Title = "Creditor Aged Invoice Base";
                    CreditorAgedInvBase Report = new CreditorAgedInvBase(constr, Server.MapPath("templates/CreditorAgedInv.rdlc"), ReportViewer1);
                    Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
                    Report.Print();
                }


            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void Trialbalance(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                string begin_date = args[3];
                string with0 = args[4];
                ReportTrialBalance Report = new ReportTrialBalance(constr, Server.MapPath("templates/TrialBalance.rdlc"), ReportViewer1);
                Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), Convert.ToDateTime(begin_date), with0);
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void UnitAccount(string[] args)
        {
            try
            {
                string bodycorp_id = args[0];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                ReportUnitAccount Report = new ReportUnitAccount(constr, Server.MapPath("templates/unitaccount.rdlc"), ReportViewer1);
                Report.SetReportInfo((DataTable)Session["AccountList"], int.Parse(bodycorp_id));
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void BalanceSheet(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                ReportBalanceSheet Report = new ReportBalanceSheet(constr, Server.MapPath("templates/balancesheet.rdlc"), ReportViewer1);
                Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void ProfixLoss(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                string begin_date = args[3];        // Add 12/04/2016
                ReportProfixLoss Report = new ReportProfixLoss(constr, Server.MapPath("templates/profitlossreport.rdlc"), ReportViewer1);
                Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), true);     // Update 14/04/2016
                Report.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void ActivityReport(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                ActivityReport activityReport;
                activityReport = new ActivityReport(constr, Server.MapPath("templates/activityreport.rdlc"), ReportViewer1);
                if (Request.QueryString["chartid"].Equals("ALL"))
                    activityReport.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date));
                // Add 3 new reports. 17/03/2016 Start  
                else if (Request.QueryString["chartid"].Equals("All_Expenses"))
                {
                    activityReport.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), "", "All_Expenses");
                }
                else if (Request.QueryString["chartid"].Equals("All_Incomes"))
                {
                    activityReport.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), "", "All_Incomes");
                }
                else if (Request.QueryString["chartid"].Equals("Balance_Sheet"))
                {
                    activityReport.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), "", "Balance_Sheet");
                }
                // Add 3 new reports. 17/03/2016 End
                else
                {
                    activityReport.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), Request.QueryString["chartid"].ToString());
                }
                activityReport.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void DebtorListReport(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];


                ReportDebtorList debtorlistReport = new ReportDebtorList(constr, Server.MapPath("templates/debtorlist.rdlc"), ReportViewer1);
                debtorlistReport.SetReportInfo(Convert.ToInt32(bodycorp_id));
                debtorlistReport.Print();


            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void UnitListReport(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string property_id = args[0];
                PropertyMaster property = new PropertyMaster(constr);
                property.LoadData(Convert.ToInt32(property_id));

                ReportUnitList unitlistReport = new ReportUnitList(constr, Server.MapPath("templates/unitlist.rdlc"), ReportViewer1);
                unitlistReport.SetReportInfo(Convert.ToInt32(property_id), property.PropertyMasterBodycorpId);
                unitlistReport.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void COAReport(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                ReportChartOfAccount coaReport = new ReportChartOfAccount(constr, Server.MapPath("templates/chartofaccount.rdlc"), ReportViewer1);
                coaReport.SetReportInfo();
                coaReport.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void JournalReport(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                JournalReport coaReport = new JournalReport(constr, Server.MapPath("templates/JournalReport.rdlc"), ReportViewer1);
                string bodycorp_id = args[0];
                string start_date = args[1];
                string end_date = args[2];
                string begin_date = args[3];
                string journal = args[4];
                coaReport.SetReportInfo(int.Parse(bodycorp_id), DateTime.Parse(start_date), DateTime.Parse(end_date), journal);
                coaReport.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        /// <summary>
        /// Summary report for Proprietor Statement
        /// </summary>
        /// <param name="args">bodycorp_id</param>
        private void ProprietorStatementSummary(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                DataTable reportResult = (DataTable)Session["PROPRIETOR_STATEMENT_SUMMARY"];
                Session["PROPRIETOR_STATEMENT_SUMMARY"] = null;

                ProprietorReportSummary summary = new ProprietorReportSummary(constr, Server.MapPath("templates/ProprietorReportSummary.rdlc"), ReportViewer1);
                summary.SetReportInfo(Convert.ToInt32(bodycorp_id), reportResult);
                summary.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        /// <summary>
        /// Summary report for invoice send
        /// </summary>
        /// <param name="args">bodycorp_id</param>
        private void InvoiceSendSummary(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = args[0];
                DataTable mailSummary = (DataTable)Session["INVOICE_SEND_REPORT"];
                Session["INVOICE_SEND_REPORT"] = null;

                InvoiceSendSummary summary = new InvoiceSendSummary(constr, Server.MapPath("templates/InvoiceSendSummary.rdlc"), ReportViewer1);
                summary.SetReportInfo(Convert.ToInt32(bodycorp_id), mailSummary);
                summary.Print();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #endregion
    }
}
