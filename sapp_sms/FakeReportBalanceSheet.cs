using System;
using SystemAlias = System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using Microsoft.Reporting.WebForms;
using Sapp.Common;
using Sapp.Data;
using System.Globalization;

namespace Sapp.SMS
{
    public class FakeReportBalanceSheet : Report
    {
        #region Variables
        private DataTable generalDT = new DataTable();

        public DataTable FakebalanceDT = new DataTable();

        public DataTable SumDT = new DataTable();
        private Odbc o;

        #endregion

        public FakeReportBalanceSheet(string constr, string template_fileaddr, ReportViewer reportViewer)
            : base(constr, template_fileaddr, reportViewer)
        {
            o = new Odbc(constr);

            generalDT.Columns.Add("company_name");
            generalDT.Columns.Add("bodycorp_code");
            generalDT.Columns.Add("bodycorp_name");
            generalDT.Columns.Add("start_date");
            generalDT.Columns.Add("end_date");

            FakebalanceDT.Columns.Add("type");
            FakebalanceDT.Columns.Add("typeForShow");
            FakebalanceDT.Columns.Add("chart_master_name");
            FakebalanceDT.Columns.Add("Debit", SystemAlias.Type.GetType("System.Decimal"));
            FakebalanceDT.Columns.Add("Credit", SystemAlias.Type.GetType("System.Decimal"));
            FakebalanceDT.Columns.Add("Net", SystemAlias.Type.GetType("System.Decimal"));
            
            SumDT.Columns.Add("Debit");
            SumDT.Columns.Add("Credit");
            SumDT.Columns.Add("Net");
        }



        public void SetReportInfo(int bodycorp_id, DateTime start, DateTime end)
        {
            generalDT = ReportDT.BulidGeneralDT(constr, bodycorp_id, start, end);
            string typeSql = "SELECT  chart_master.chart_master_type_id, chart_types.chart_type_name "+
                                "FROM sapp_sms.gl_transactions, chart_master, chart_types, bodycorps " +
                                "where gl_transaction_type_id<>3 and gl_transaction_type_id<>4 "+
                                "and gl_transaction_chart_id = chart_master.chart_master_id "+
                                "and chart_types.chart_type_id = chart_master.chart_master_type_id "+
                                "AND `gl_transaction_date`<='2016-07-01' "+
                                "AND `gl_transaction_date`>='2015-07-01' "+
                                "and gl_transactions.gl_transaction_bodycorp_id = bodycorps.bodycorp_id " +
                                "and bodycorps.bodycorp_id =" + bodycorp_id +
                                " group by chart_master.chart_master_type_id "+
                                " order by chart_master.chart_master_type_id ";
            DataTable typeT = o.ReturnTable(typeSql, "incomeDebit");
            foreach (DataRow dr in typeT.Rows)
            {
                DataRow newRow = FakebalanceDT.NewRow();
                newRow["typeForShow"] = dr["chart_type_name"];
                FakebalanceDT.Rows.Add(newRow);

                string debitSql = "SELECT chart_master.chart_master_name, sum(gl_transactions.gl_transaction_net) as Debit " +
                                    "FROM sapp_sms.gl_transactions, chart_master, bodycorps  " +
                                    "where gl_transaction_type_id<>3 and gl_transaction_type_id<>4 " +
                                    "and gl_transaction_chart_id = chart_master.chart_master_id " +
                                    "AND `gl_transaction_date`<='" + end.ToString("yyyy-MM-dd") + "' " +
                                    "AND `gl_transaction_date`>='" + start.ToString("yyyy-MM-dd") + "' " +
                                    "and chart_master.chart_master_type_id=" + dr["chart_master_type_id"] + " " +
                                    "and gl_transactions.gl_transaction_net > 0 " +
                                    "and gl_transactions.gl_transaction_bodycorp_id = bodycorps.bodycorp_id " +
                                    " and bodycorps.bodycorp_id =" + bodycorp_id +
                                    " group by(chart_master.chart_master_id)";
                DataTable debitDT = o.ReturnTable(debitSql, "debit");
                foreach (DataRow debitDR in debitDT.Rows)
                {
                    DataRow newRowDebit = FakebalanceDT.NewRow();
                    newRowDebit["chart_master_name"] = debitDR["chart_master_name"];
                    newRowDebit["Debit"] = debitDR["Debit"];
                    newRowDebit["type"] = dr["chart_type_name"];
                    FakebalanceDT.Rows.Add(newRowDebit);
                }
                string creditSql = "SELECT chart_master.chart_master_name, sum(gl_transactions.gl_transaction_net) as Credit " +
                                    "FROM sapp_sms.gl_transactions, chart_master, bodycorps  " +
                                    "where gl_transaction_type_id<>3 and gl_transaction_type_id<>4 " +
                                    "and gl_transaction_chart_id = chart_master.chart_master_id " +
                                    "AND `gl_transaction_date`<='" + end.ToString("yyyy-MM-dd") + "' " +
                                    "AND `gl_transaction_date`>='" + start.ToString("yyyy-MM-dd") + "' " +
                                    "and chart_master.chart_master_type_id =" + dr["chart_master_type_id"] + " " +
                                    "and gl_transactions.gl_transaction_net < 0 " +
                                    "and gl_transactions.gl_transaction_bodycorp_id = bodycorps.bodycorp_id " +
                                    " and bodycorps.bodycorp_id =" + bodycorp_id +
                                    " group by(chart_master.chart_master_id)";
                DataTable creditDT = o.ReturnTable(creditSql, "credit");
                foreach (DataRow creditDR in creditDT.Rows)
                {
                    bool haveDebitRecord = false;
                    foreach (DataRow mainR in FakebalanceDT.Rows)
                    {
                        if (mainR["type"].ToString() == dr["chart_type_name"].ToString())
                        {
                            if (creditDR["chart_master_name"].ToString() == mainR["chart_master_name"].ToString())
                            {
                                mainR["Credit"] = -decimal.Parse(creditDR["Credit"].ToString());
                                haveDebitRecord = true;
                                break;
                            }
                        }
                    }
                    if (!haveDebitRecord)
                    {
                        DataRow newCreditRow = FakebalanceDT.NewRow();
                        newCreditRow["type"] = dr["chart_type_name"];
                        newCreditRow["chart_master_name"] = creditDR["chart_master_name"];
                        newCreditRow["Credit"] = -decimal.Parse(creditDR["Credit"].ToString());
                        FakebalanceDT.Rows.Add(newCreditRow);
                    }
                }
                foreach (DataRow mainDR2 in FakebalanceDT.Rows)
                {
                    if (mainDR2["type"].ToString() == dr["chart_type_name"].ToString())
                    {
                        if ((mainDR2["Debit"] == null || mainDR2["Debit"].ToString() == "")
                            &&
                            (mainDR2["Credit"] == null || mainDR2["Credit"].ToString() == "")
                            )
                        {
                            continue;
                        }
                        if (mainDR2["Debit"] == null || mainDR2["Debit"].ToString() == "")
                        {
                            mainDR2["Net"] = -decimal.Parse(mainDR2["Credit"].ToString());
                        }
                        else if (mainDR2["Credit"] == null || mainDR2["Credit"].ToString() == "")
                        {
                            mainDR2["Net"] = decimal.Parse(mainDR2["Debit"].ToString());
                        }
                        else
                        {
                            mainDR2["Net"] = decimal.Parse(mainDR2["Debit"].ToString()) - decimal.Parse(mainDR2["Credit"].ToString());
                        }
                    }
                }
                DataRow SumRow = FakebalanceDT.NewRow();
                SumRow["typeForShow"] = "Sub Total: ";
                SumRow["Debit"] = ReportDT.SumIf(FakebalanceDT, "Debit", "type", dr["chart_type_name"].ToString());
                SumRow["Credit"] = ReportDT.SumIf(FakebalanceDT, "Credit", "type", dr["chart_type_name"].ToString());
                SumRow["Net"] = ReportDT.SumIf(FakebalanceDT, "Net", "type", dr["chart_type_name"].ToString());
                FakebalanceDT.Rows.Add(SumRow);
                DataRow blankRow = FakebalanceDT.NewRow();
                blankRow["type"] = "";
                FakebalanceDT.Rows.Add(blankRow);
            }

            SumDT.Rows.Add(SumDT.NewRow());
            SumDT.Rows[0]["Debit"] = SumIfNot(FakebalanceDT, "Debit", "type", "Sub Total: ");
            SumDT.Rows[0]["Credit"] =SumIfNot(FakebalanceDT, "Credit", "type", "Sub Total: ");
            SumDT.Rows[0]["Net"] = SumIfNot(FakebalanceDT, "Net", "type", "Sub Total: ");

        }

        public static string SumIfNot(DataTable dt, string VColumn, string CColumn, string CValue)
        {
            decimal sum = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (!dt.Rows[i][VColumn].ToString().Equals(""))
                    if (!(dt.Rows[i][CColumn].ToString().Equals(CValue)))
                        sum += decimal.Parse(dt.Rows[i][VColumn].ToString());
            }
            return sum.ToString();
        }

        public override void Print()
        {
            try
            {
                #region Set DataSource

                ReportDataSource rpDataSourcegeneralDT = new ReportDataSource("general", generalDT);
                ReportDataSource rpDataSourcefakeBalance = new ReportDataSource("fakeBalance", FakebalanceDT);

                ReportDataSource rpDataSourceSumDT = new ReportDataSource("SumDT", SumDT);
                #endregion

                base.LoadDataSource(rpDataSourcegeneralDT);
                base.LoadDataSource(rpDataSourcefakeBalance);
                base.LoadDataSource(rpDataSourceSumDT);

                base.reportViewer.LocalReport.Refresh();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
