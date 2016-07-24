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

namespace sapp_sms
{
    public class ReportFakeBalanceSheet : Report
    {

        #region Variables
        private DataTable generalDT = new DataTable();
        public DataTable Assets = new DataTable();
        public DataTable Liabilities = new DataTable();
        public DataTable Represented = new DataTable();
        public DataTable SumDT = new DataTable();
        private Odbc o;
        #endregion
        public ReportFakeBalanceSheet(string constr, string template_fileaddr, ReportViewer reportViewer)
            : base(constr, template_fileaddr, reportViewer)
        {
            o = new Odbc(constr);

            generalDT.Columns.Add("company_name");
            generalDT.Columns.Add("bodycorp_code");
            generalDT.Columns.Add("bodycorp_name");
            generalDT.Columns.Add("start_date");
            generalDT.Columns.Add("end_date");
            Assets.Columns.Add("Chart_Maseter_ID");
            Assets.Columns.Add("Code");
            Assets.Columns.Add("Name");
            Assets.Columns.Add("Actual", SystemAlias.Type.GetType("System.Decimal"));
            Assets.Columns.Add("Budget", SystemAlias.Type.GetType("System.Decimal"));
            Assets.Columns.Add("Variance", SystemAlias.Type.GetType("System.Decimal"));
            Liabilities.Columns.Add("Chart_Maseter_ID");
            Liabilities.Columns.Add("Code");
            Liabilities.Columns.Add("Name");
            Liabilities.Columns.Add("Actual", SystemAlias.Type.GetType("System.Decimal"));
            Liabilities.Columns.Add("Budget", SystemAlias.Type.GetType("System.Decimal"));
            Liabilities.Columns.Add("Variance", SystemAlias.Type.GetType("System.Decimal"));
            Represented.Columns.Add("Chart_Maseter_ID");
            Represented.Columns.Add("Code");
            Represented.Columns.Add("Name");
            Represented.Columns.Add("Actual", SystemAlias.Type.GetType("System.Decimal"));
            Represented.Columns.Add("Budget", SystemAlias.Type.GetType("System.Decimal"));
            Represented.Columns.Add("Variance", SystemAlias.Type.GetType("System.Decimal"));
            SumDT.Columns.Add("CapitalT");
            SumDT.Columns.Add("NetSurplusPeriod");
            SumDT.Columns.Add("RetainedSurplus");
            SumDT.Columns.Add("AssetsT");
            SumDT.Columns.Add("LiabilitiesT");
            SumDT.Columns.Add("WorkingCapitalT");
            SumDT.Rows.Add(SumDT.NewRow());
        }
        public decimal IncomeInAdvance(int bodycorp_id, DateTime start, DateTime end, string pid = "ALL")
        {
            //ReportProprietorAgedDetail pa = new ReportProprietorAgedDetail(constr, "", new ReportViewer());
            //pa.SetReportInfo(bodycorp_id, start, end, pid);
            //DataTable dt = pa.Creditor;
            //decimal a = ReportDT.SumTotal(dt, "Invoice") - ReportDT.SumTotal(dt, "Receipt");
            //return a;
            ReportProprietorAgedDetail pa = new ReportProprietorAgedDetail(constr, "", new ReportViewer());
            string unitsql = "SELECT        unit_master.unit_master_code, bodycorps.bodycorp_id FROM            bodycorps, property_master, unit_master WHERE        bodycorps.bodycorp_id = property_master.property_master_bodycorp_id AND property_master.property_master_id = unit_master.unit_master_property_id AND                           (bodycorps.bodycorp_id = " + bodycorp_id + ")  ";
            pa.SetReportInfo(bodycorp_id, start.AddYears(-10), end, pid, "0", "ApplyDate");     // Update 14/05/2016
            DataTable dt = pa.Creditor;
            DataTable unitDT = o.ReturnTable(unitsql, "unit");
            decimal unitadvance = 0;
            foreach (DataRow dr in unitDT.Rows)
            {
                DataTable temp = ReportDT.FilterDT(dt, "Unit='" + dr["unit_master_code"].ToString() + "'");
                decimal a = ReportDT.SumTotal(temp, "Invoice") - ReportDT.SumTotal(temp, "Receipt");
                if (a < 0)
                {
                    unitadvance += a;
                }
            }
            return unitadvance;
        }
        #region Functions
        public void SetReportInfo(int bodycorp_id, DateTime start, DateTime end)
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
            string[] gcodes = s.SystemValue.Split('|');
            string[] proprietorID = new string[gcodes.Length];
            for (int i = 0; i < gcodes.Length; i++)
            {
                ch.LoadData(gcodes[i]);
                proprietorID[i] = ch.ChartMasterId.ToString();
            }
            s.LoadData("GENERALCREDITOR");
            ch.LoadData(s.SystemValue);
            string creditorID = ch.ChartMasterId.ToString();
            s.LoadData("DISCOUNTCHARCODE");
            ch.LoadData(s.SystemValue);
            string discountID = ch.ChartMasterId.ToString();
            s.LoadData("AccumulatedFund");
            ch.LoadData(s.SystemValue);
            string AccumulatedFundID = ch.ChartMasterId.ToString();
            #endregion

            o = new Odbc(constr);
            generalDT = ReportDT.BulidGeneralDT(constr, bodycorp_id, start, end);

            // Update 13/05/2016 (invoice_master_due -> invoice_master_apply)
            //string gl_sql = "SELECT * FROM (((((`gl_transactions` LEFT JOIN `invoice_gls` ON `gl_transaction_id`=`invoice_gl_gl_id`)Left join `chart_master` on `gl_transaction_chart_id`=`chart_master_id`) LEFT JOIN `invoice_master` ON `invoice_gl_invoice_id`=`invoice_master_id`) LEFT JOIN `cinvoice_gls` ON `gl_transaction_id`=`cinvoice_gl_gl_id`) LEFT JOIN `cinvoices` ON `cinvoice_gl_cinvoice_id`=`cinvoice_id`)  WHERE `gl_transaction_type_id`<>3 AND `gl_transaction_type_id`<>4  AND ((`invoice_master_due` IS NULL AND `cinvoice_apply` IS NULL AND `gl_transaction_date`<='" + end.ToString("yyyy-MM-dd") + "') OR ((`invoice_master_due` IS NOT NULL AND `gl_transaction_type_id`<>6 AND `invoice_master_due`<='" + end.ToString("yyyy-MM-dd") + "') OR (`invoice_master_due` IS NOT NULL AND `gl_transaction_type_id`=6 AND `gl_transaction_date`<='" + end.ToString("yyyy-MM-dd") + "')) OR (`cinvoice_apply` IS NOT NULL AND `cinvoice_apply`<='" + end.ToString("yyyy-MM-dd") + "' )) and gl_transaction_bodycorp_id=  " + bodycorp_id;
            string gl_sql = "SELECT * FROM (((((`gl_transactions` "
                          + "    LEFT JOIN `invoice_gls` ON `gl_transaction_id`=`invoice_gl_gl_id`) "
                          + "    LEFT JOIN `chart_master` on `gl_transaction_chart_id`=`chart_master_id`) "
                          + "    LEFT JOIN `invoice_master` ON `invoice_gl_invoice_id`=`invoice_master_id`) "
                          + "    LEFT JOIN `cinvoice_gls` ON `gl_transaction_id`=`cinvoice_gl_gl_id`) "
                          + "    LEFT JOIN `cinvoices` ON `cinvoice_gl_cinvoice_id`=`cinvoice_id`) "
                          + "  WHERE `gl_transaction_type_id`<>3 AND `gl_transaction_type_id`<>4 "
                          + "    AND ( ( `invoice_master_apply` IS NULL AND `cinvoice_apply` IS NULL "
                          + "            AND `gl_transaction_date`<='" + end.ToString("yyyy-MM-dd") + "') "
                          + "          OR ( ( `invoice_master_apply` IS NOT NULL "
                          + "                 AND `gl_transaction_type_id`<>6 "
                          + "                 AND `invoice_master_apply`<='" + end.ToString("yyyy-MM-dd") + "') "
                          + "               OR ( `invoice_master_apply` IS NOT NULL "
                          + "                    AND `gl_transaction_type_id`=6 "
                          + "                    AND `gl_transaction_date`<='" + end.ToString("yyyy-MM-dd") + "')) "
                          + "          OR ( `cinvoice_apply` IS NOT NULL "
                          + "               AND `cinvoice_apply`<='" + end.ToString("yyyy-MM-dd") + "' )) "
                          + "    AND gl_transaction_bodycorp_id=  " + bodycorp_id;

            string budget_master_sql = "select * from budget_master where budget_master_bodycorp_id=" + bodycorp_id + " and budget_master_month <= " + DBSafeUtils.DateTimeToSQL(end);
            string sql = "SELECT   chart_types.chart_type_id, chart_types.chart_type_name, chart_types.chart_type_class_id, chart_master.* FROM      chart_types, chart_classes, chart_master WHERE   chart_types.chart_type_class_id = chart_classes.chart_class_id AND                 chart_types.chart_type_id = chart_master.chart_master_type_id AND (chart_classes.chart_class_balance_sheet = 1)";

            DataTable chart_masterDT = o.ReturnTable(sql, "chart_master");
            DataTable Gl_DT = o.ReturnTable(gl_sql, "gl");
            //CsvDT.DataTableToCsv(Gl_DT, "C:\\temp\\bc.csv");
            //DataTable Gl_DT = ReportDT.FilterDT(ReportDT.GetGLDT(), "FixDate <= #" + end.ToString("yyyy-MM-dd") + "#");
            DataTable chart_typeDT = ReportDT.getTable(constr, "chart_types");
            DataTable budget_master_DT = o.ReturnTable(budget_master_sql, "budget_master");
            decimal unitadvance = 0;
            {
                DataRow newRow = Liabilities.NewRow();
                newRow["Chart_Maseter_ID"] = "0";
                newRow["Name"] = "Income in Advance";
                decimal a = IncomeInAdvance(bodycorp_id, start.AddYears(-10), end);
                unitadvance = a;

                if (unitadvance != 0)
                {
                    newRow["Actual"] = -unitadvance;
                    Liabilities.Rows.Add(newRow);
                }
            }
            #region Asset
            {
                string tid = ReportDT.GetDataByColumn(chart_typeDT, "chart_type_name", "Assets", "chart_type_id");
                if (!tid.Equals(""))
                {
                    DataTable temp = ReportDT.FilterDT(chart_masterDT, " chart_master_type_id = " + tid);
                    foreach (DataRow dr in temp.Rows)
                    {
                        DataRow newRow = Assets.NewRow();
                        newRow["Chart_Maseter_ID"] = dr["chart_master_id"];
                        newRow["Code"] = dr["chart_master_code"];
                        newRow["Name"] = dr["chart_master_name"].ToString();
                        if (dr["chart_master_id"].ToString().Equals("865"))
                        {
                        }
                        DataTable tempGL = ReportDT.FilterDT(Gl_DT, "chart_master_id = " + dr["chart_master_id"].ToString());
                        DataTable tempbuget = ReportDT.FilterDT(budget_master_DT, "budget_master_chart_id = " + dr["chart_master_id"].ToString());
                        decimal a = ReportDT.SumTotal(tempGL, "gl_transaction_net");
                        decimal b = ReportDT.SumTotal(tempbuget, "budget_master_amound");
                        if (a != 0)
                            newRow["Actual"] = -a;
                        if (b != 0)
                            newRow["Budget"] = b;
                        if (a != 0 && b != 0)
                            newRow["Variance"] = a - b;
                        if (proprietorID.Contains<string>(dr["chart_master_id"].ToString()))
                        {
                            if (!newRow["Actual"].ToString().Equals(""))
                            {
                                ReportProprietorAged pa = new ReportProprietorAged(constr, "", new ReportViewer());
                                pa.SetReportInfo(bodycorp_id, end.AddYears(-100), end, dr["chart_master_id"].ToString(), "ApplyDate");   // Update 14/05/2016
                                decimal t1 = AdFunction.Rounded(pa.SumDT.Rows[0]["Total"].ToString());
                                //if (unitadvance != 0)
                                //{
                                t1 = t1 - IncomeInAdvance(bodycorp_id, start.AddYears(-10), end, dr["chart_master_id"].ToString());

                                //}

                                newRow["Actual"] = t1;
                            }
                        }
                        Assets.Rows.Add(newRow);
                    }
                }
                //DataRow pdr=ReportDT.GetDataRowByColumn(Assets.Rows.)
            }
            #endregion
            #region Liabilities
            {
                string tid = ReportDT.GetDataByColumn(chart_typeDT, "chart_type_name", "Liabilities", "chart_type_id");
                if (!tid.Equals(""))
                {
                    DataTable temp = ReportDT.FilterDT(chart_masterDT, " chart_master_type_id = " + tid);
                    foreach (DataRow dr in temp.Rows)
                    {
                        DataRow newRow = Liabilities.NewRow();
                        newRow["Chart_Maseter_ID"] = dr["chart_master_id"];
                        newRow["Code"] = dr["chart_master_code"];
                        newRow["Name"] = dr["chart_master_name"].ToString();
                        DataTable tempGL = ReportDT.FilterDT(Gl_DT, "chart_master_id = " + dr["chart_master_id"].ToString());
                        DataTable tempbuget = ReportDT.FilterDT(budget_master_DT, "budget_master_chart_id = " + dr["chart_master_id"].ToString());
                        decimal a = ReportDT.SumTotal(tempGL, "gl_transaction_net");
                        decimal b = ReportDT.SumTotal(tempbuget, "budget_master_amound");
                        if (a != 0)
                            newRow["Actual"] = a;
                        if (b != 0)
                            newRow["Budget"] = b;
                        if (a != 0 && b != 0)
                            newRow["Variance"] = a - b;

                        Liabilities.Rows.Add(newRow);
                    }
                }
            }

            #endregion

            #region Represented
            {
                string tid = ReportDT.GetDataByColumn(chart_typeDT, "chart_type_name", "Capital", "chart_type_id");
                if (!tid.Equals(""))
                {
                    DataTable temp = ReportDT.FilterDT(chart_masterDT, " chart_master_type_id = " + tid);
                    foreach (DataRow dr in temp.Rows)
                    {
                        DataRow newRow = Represented.NewRow();
                        newRow["Chart_Maseter_ID"] = dr["chart_master_id"];
                        newRow["Code"] = dr["chart_master_code"];
                        newRow["Name"] = dr["chart_master_name"].ToString();
                        DataTable tempGL = ReportDT.FilterDT(Gl_DT, "chart_master_id = " + dr["chart_master_id"].ToString());
                        DataTable tempbuget = ReportDT.FilterDT(budget_master_DT, "budget_master_chart_id = " + dr["chart_master_id"].ToString());
                        decimal a = ReportDT.SumTotal(tempGL, "gl_transaction_net");
                        decimal b = ReportDT.SumTotal(tempbuget, "budget_master_amound");
                        if (a != 0)
                            newRow["Actual"] = a;
                        if (b != 0)
                            newRow["Budget"] = b;
                        if (a != 0 && b != 0)
                            newRow["Variance"] = a - b;
                        Represented.Rows.Add(newRow);

                    }
                }
            }
            {
                string tid = ReportDT.GetDataByColumn(chart_typeDT, "chart_type_name", "Equity", "chart_type_id");

                if (!tid.Equals(""))
                {
                    DataTable temp = ReportDT.FilterDT(chart_masterDT, " chart_master_type_id = " + tid);
                    foreach (DataRow dr in temp.Rows)
                    {
                        DataRow newRow = Represented.NewRow();
                        newRow["Chart_Maseter_ID"] = dr["chart_master_id"];
                        newRow["Code"] = dr["chart_master_code"];
                        newRow["Name"] = dr["chart_master_name"].ToString();
                        DataTable tempGL = ReportDT.FilterDT(Gl_DT, "chart_master_id = " + dr["chart_master_id"].ToString());
                        DataTable tempbuget = ReportDT.FilterDT(budget_master_DT, "budget_master_chart_id = " + dr["chart_master_id"].ToString());
                        decimal a = ReportDT.SumTotal(tempGL, "gl_transaction_net");
                        decimal b = ReportDT.SumTotal(tempbuget, "budget_master_amound");
                        if (a != 0)
                            newRow["Actual"] = a;
                        if (b != 0)
                            newRow["Budget"] = b;
                        if (a != 0 && b != 0)
                            newRow["Variance"] = a - b;
                        Represented.Rows.Add(newRow);
                    }
                }
            }
            #endregion


            ReportProfixLoss p = new ReportProfixLoss(constr, template_fileaddr, reportViewer);
            p.SetReportInfo(bodycorp_id, start.AddYears(-1000), end);

            DataTable EquityDT = o.ReturnTable("SELECT * FROM chart_master, chart_types WHERE chart_master.chart_master_type_id = chart_types.chart_type_id AND (chart_type_name = 'Equity')", "t1");

            DataRow AccumulatedFund = ReportDT.GetDataRowByColumn(Represented, "Chart_Maseter_ID", AccumulatedFundID);
            if (AccumulatedFund != null)
            {
                decimal AFund = 0;
                if (AccumulatedFund != null)
                    AFund = AdFunction.Rounded(AccumulatedFund["Actual"].ToString());
                AFund = AFund + AdFunction.Rounded(p.SumDT.Rows[0]["IcomOExpenses"].ToString());
                if (AFund != 0)
                    AccumulatedFund["Actual"] = AFund;
                else
                    AccumulatedFund["Actual"] = DBNull.Value;
            }
            foreach (DataRow edr in EquityDT.Rows)
            {
                string ecode = edr["chart_master_code"].ToString();
                DataRow bmp = ReportDT.GetDataRowByColumn(Represented, "Code", ecode);
                if (bmp != null)
                {
                    BMPF bmpReport = new BMPF(constr, template_fileaddr, reportViewer);
                    bmpReport.SetReportInfo(bodycorp_id, start.AddYears(-1000), end, edr["chart_master_id"].ToString());
                    decimal temp = AdFunction.Rounded(bmp["Actual"].ToString()) + AdFunction.Rounded(bmpReport.SumDT.Rows[0]["IcomOExpenses"].ToString());
                    if (temp != 0)
                        bmp["Actual"] = temp;
                    else
                        bmp["Actual"] = DBNull.Value;
                }
            }


            decimal sumr = ReportDT.SumTotal(Represented, "Actual");
            decimal suma = ReportDT.SumTotal(Assets, "Actual");
            decimal suml = ReportDT.SumTotal(Liabilities, "Actual");
            SumDT.Rows[0]["CapitalT"] = sumr;
            SumDT.Rows[0]["AssetsT"] = suma;
            SumDT.Rows[0]["LiabilitiesT"] = suml;
            SumDT.Rows[0]["NetSurplusPeriod"] = p.SumDT.Rows[0]["IcomOExpenses"];
            SumDT.Rows[0]["RetainedSurplus"] = (suma - suml) - AdFunction.Rounded(SumDT.Rows[0]["NetSurplusPeriod"].ToString()) - sumr;
            SumDT.Rows[0]["WorkingCapitalT"] = suma - suml;

        }
        public DataTable DeleteRow(DataTable dt, string c)
        {
            dt.DefaultView.RowFilter = c + " is not null";
            return dt.DefaultView.ToTable();
        }
        public override void Print()
        {
            try
            {
                #region Set DataSource

                ReportDataSource rpDataSource = new ReportDataSource("general", generalDT);
                ReportDataSource rpDataSource2 = new ReportDataSource("Assets", DeleteRow(Assets, "Actual"));
                ReportDataSource rpDataSource3 = new ReportDataSource("Liabilities", DeleteRow(Liabilities, "Actual"));
                ReportDataSource rpDataSource4 = new ReportDataSource("SumDT", SumDT);
                ReportDataSource rpDataSource5 = new ReportDataSource("Represented", DeleteRow(Represented, "Actual"));
                #endregion
                base.LoadDataSource(rpDataSource);
                base.LoadDataSource(rpDataSource2);
                base.LoadDataSource(rpDataSource3);
                base.LoadDataSource(rpDataSource4);
                base.LoadDataSource(rpDataSource5);
                base.reportViewer.LocalReport.Refresh();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
