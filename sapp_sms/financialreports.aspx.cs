using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.SMS;
using System.Data;
using Sapp.Data;
namespace sapp_sms
{
    public partial class financialreports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    Odbc odbc = new Odbc(AdFunction.conn);
                    TextBoxDateEnd.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    TextBoxDateStart.Text = new DateTime(DateTime.Today.AddYears(-1).Year, 12, 1).ToString("dd/MM/yyyy");
                    TextBoxYearBegins.Text = TextBoxDateStart.Text;
                    InvoiceMaster invoice = new InvoiceMaster(AdFunction.conn);
                    foreach (ChartMaster chartmaster in invoice.GetChartMasters())
                    {
                        ListItem li = new ListItem(chartmaster.ChartMasterCode + " | " + chartmaster.ChartMasterName, chartmaster.ChartMasterId.ToString());
                        ACChartDL.Items.Add(li);
                    }
                    ListItem l = new ListItem("ALL", "ALL");
                    ACChartDL.Items.Insert(0, l);
                    // Add 3 new reports. 17/03/2016 Start  
                    ListItem allExpenses = new ListItem("All Expenses", "All_Expenses");
                    ACChartDL.Items.Insert(1, allExpenses);
                    ListItem allIncomes = new ListItem("All Incomes", "All_Incomes");
                    ACChartDL.Items.Insert(2, allIncomes);
                    ListItem balanceSheet = new ListItem("Balance Sheet", "Balance_Sheet");
                    ACChartDL.Items.Insert(3, balanceSheet);
                    // Add 3 new reports. 17/03/2016 End 

                    DataTable dt = odbc.ReturnTable("SELECT `chart_master_id` AS `ID`, `chart_master_name` AS `Code` FROM `chart_master` where chart_master_type_id = 7", "temp");

                    SM_DL.DataSource = dt;
                    SM_DL.DataTextField = "Code";
                    SM_DL.DataValueField = "ID";
                    SM_DL.DataBind();
                    ListItem l2 = new ListItem("ALL", "ALL");
                    SM_DL.Items.Insert(0, l2);

                    Sapp.SMS.System s = new Sapp.SMS.System(AdFunction.conn);
                    ChartMaster ch = new ChartMaster(AdFunction.conn);
                    s.LoadData("GENERALDEBTOR");
                    string[] proprietorID;
                    string[] gcodes = s.SystemValue.Split('|');
                    proprietorID = new string[gcodes.Length];
                    for (int i = 0; i < gcodes.Length; i++)
                    {
                        ch.LoadData(gcodes[i]);
                        proprietorID[i] = ch.ChartMasterId.ToString();
                        AgedProprietorPIDDL.Items.Add(new ListItem(gcodes[i] + "|" + ch.ChartMasterName, proprietorID[i]));
                        AgedProprietorPIDDL0.Items.Add(new ListItem(gcodes[i] + "|" + ch.ChartMasterName, proprietorID[i]));
                    }



                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebControl Events
        protected void ImageButtonSubmit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string bodycorp_id = "";
                string start_date = "";
                string end_date = "";
                string begin_date = "";
                if (Request.Cookies["bodycorpid"].Value != null) bodycorp_id = Request.Cookies["bodycorpid"].Value;
                start_date = Convert.ToDateTime(TextBoxDateStart.Text).ToString("dd/MM/yyyy");
                end_date = Convert.ToDateTime(TextBoxDateEnd.Text).ToString("dd/MM/yyyy");
                begin_date = Convert.ToDateTime(TextBoxYearBegins.Text).ToString("dd/MM/yyyy");
                if (Convert.ToDateTime(TextBoxDateStart.Text) > Convert.ToDateTime(TextBoxDateEnd.Text))
                {
                    throw new Exception("Start Date Over End Date.");
                }
                string args = bodycorp_id + "|" + start_date + "|" + end_date + "|" + begin_date;
                if (CheckBoxActivity.Checked)
                {
                    Response.Write(SetUrl("activityreport", args + "&chartid=" + ACChartDL.SelectedValue));
                }
                if (CheckBoxProfitLoss.Checked) { Response.Write(SetUrl("profitloss", args)); }
                if (CheckBoxBalanceSheet.Checked) { Response.Write(SetUrl("balancesheet", args)); }
                if (CheckBoxFakeBalanceSheet.Checked) { Response.Write(SetUrl("fakebalancesheet", args)); }
                if (CheckBoxTrialBalance.Checked) { Response.Write(SetUrl("trialbalance", args + "|" + TB_DL.SelectedValue)); }
                if (CheckBoxGST.Checked) { Response.Write(SetUrl("gstreport", args)); }
                if (CheckBoxCashPosition.Checked) { Response.Write(SetUrl("cashposition", args)); }
                //if (CheckBoxCashPositionDetailed.Checked) { Response.Write(SetUrl("activityreport", args)); }
                if (CheckBoxAgedProprietors.Checked)
                { Response.Write(SetUrl("proprietoraged", args + "&type=" + AgePDL.SelectedValue + "&chartid=" + AgedProprietorPIDDL.SelectedValue + "&showname=" + CheckBoxShowName.Checked)); }       // Update 20/05/2016
                if (CheckBoxAgedCreditors.Checked) { Response.Write(SetUrl("creditoraged", args + "&type=" + AgeCDL.SelectedValue)); }
                if (CheckBoxCreditorActivity.Checked) { Response.Write(SetUrl("CreditorActivity", args)); }
                if (CheckBoxGSTRe.Checked) { Response.Write(SetUrl("gstrereport", args)); }
                if (CheckBoxAgedPDetail.Checked)
                {

                    Response.Write(SetUrl("proprietorageddetail", args + "&chartid=" + AgedProprietorPIDDL0.SelectedValue + "&outstandingflg=" + AgedProprietorOutstanding.SelectedValue));     // Update 06/04/2016
                }
                if (JournalReportCK.Checked) { Response.Write(SetUrl("JournalReport", args + "|" + JournalT.Text)); }
                if (BMPF_CK.Checked)
                {
                    Sapp.SMS.System s = new Sapp.SMS.System(AdFunction.conn);
                    s.LoadData("AccumulatedFund");
                    ChartMaster c = new ChartMaster(AdFunction.conn);
                    c.LoadData(s.SystemValue);

                    if (!SM_DL.SelectedValue.Equals("ALL"))
                    {
                        if (!SM_DL.SelectedValue.Equals(c.ChartMasterId.ToString()))
                            Response.Write(SetUrl("BMPF", args + "&chartid=" + SM_DL.SelectedValue));
                        else
                            Response.Write(SetUrl("AF", args));
                    }
                    else
                    {
                        foreach (ListItem li in SM_DL.Items)
                        {
                            string value = li.Value;
                            if (!value.Equals("ALL"))
                                //if (!value.Equals("285"))
                                if (!value.Equals(c.ChartMasterId.ToString()))      // Update 1/05/2016
                                    Response.Write(SetUrl("BMPF", args + "&chartid=" + value));
                                else
                                    Response.Write(SetUrl("AF", args));
                        }
                    }
                }
                if (CheckBoxGSTRe0.Checked) { Response.Write(SetUrl("GST", args)); }
                if (CheckBoxGSTReDetail.Checked) { Response.Write(SetUrl("GSTReDetail", args)); }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #endregion
        public string SetUrl(string reportid, string args)
        {
            string url = "<script type='text/javascript'> window.open('reportviewer.aspx?reportid=" + reportid + "&args=" + args + "','_blank'); </script>";
            return url;
        }
    }
}