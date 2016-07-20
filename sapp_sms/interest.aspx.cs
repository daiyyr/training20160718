using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using Sapp.SMS;
using Sapp.Data;
using Sapp.Common;
using Sapp.JQuery;
using Sapp.SMS;
using System.Collections;
using Sapp.Common;
using Sapp.General;
using System.Data;
using System.Threading;
namespace sapp_sms
{
    public partial class interest : System.Web.UI.Page
    {
        public static DataTable dt = new DataTable();
        public static bool end = false;
        public static string tempbid;
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
        public void SetDT()
        {
            if (!dt.Columns.Contains("Unit"))
            {
                dt.Columns.Add("Unit");
                dt.Columns.Add("UnitID");
                dt.Columns.Add("State");
                dt.Columns.Add("Amount");
                dt.Columns.Add("Interest");
            }
        }
        public void Formatbodycory(string bodycorp_id)
        {
            string login = HttpContext.Current.User.Identity.Name;
            Sapp.General.User user = new Sapp.General.User(constr_general);
            user.LoadData(login);
            DataTable bDT = ReportDT.getTable(constr, "bodycorps");
            DateTime cDate = new DateTime(2000, 10, 10);
            cDate = DateTime.Parse(ReportDT.GetDataByColumn(bDT, "bodycorp_id", bodycorp_id, "bodycorp_close_period_date"));
            if (cDate.Year == 9999)
            {
                Bodycorp bodycorp = new Bodycorp(constr);
                Hashtable items = new Hashtable();
                items.Add("bodycorp_close_period_date", DBSafeUtils.DateToSQL("1/1/1000"));
                bodycorp.SetLog(user.UserId);
                bodycorp.Update(items, Convert.ToInt32(bodycorp_id));
            }
            else
            {
                CalendarExtenderActivityStart.StartDate = cDate.AddMonths(1);
                CalendarExtenderActivityStart.SelectedDate = cDate.AddMonths(1);
            }
            StartDateT.Text = cDate.ToString("dd/MM/yyyy");
        }
        public void GetData()
        {
            string bid = tempbid;

            DataTable sysDT = ReportDT.getTable(constr, "system");
            DataTable bodycorpsDT = ReportDT.getTable(constr, "bodycorps");
            DataTable unitDT = ReportDT.getTable(constr, "unit_master");
            DataTable invDT = ReportDT.getTable(constr, "invoice_master");
            DataTable reDT = ReportDT.getTable(constr, "receipts");
            DataTable jDT = ReportDT.getTable(constr, "gl_transactions");
            jDT = ReportDT.FilterDT(jDT, "gl_transaction_type_id = 6");
            decimal taxtrate = decimal.Parse(ReportDT.GetDataByColumn(sysDT, "system_code", "PENALTYINT", "system_value"));

            string tempstartDate = ReportDT.GetDataByColumn(bodycorpsDT, "bodycorp_id", bid, "bodycorp_close_period_date");
            DateTime endDate = DateTime.Parse(EndDateT.Text);
            endDate = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(1).AddDays(-1);
            DateTime startDate = DateTime.Parse(tempstartDate);


            foreach (DataRow unitDR in unitDT.Rows)
            {
                string unit_id = unitDR["unit_master_id"].ToString();
                DataTable unitInvDT = ReportDT.FilterDT(invDT, "invoice_master_unit_id=" + unit_id);
                DataTable unitReDT = ReportDT.FilterDT(reDT, "receipt_unit_id=" + unit_id);
                DataTable unitJDT = ReportDT.FilterDT(jDT, "gl_transaction_unit_id=" + unit_id);
                unitReDT = ReportDT.FilterDT(unitReDT, " receipt_date<=#" + endDate.ToString("yyyy-MM-dd") + "#");
                unitInvDT = ReportDT.FilterDT(unitInvDT, " invoice_master_date<=#" + endDate.ToString("yyyy-MM-dd") + "#");
                unitJDT = ReportDT.FilterDT(unitJDT, "gl_transaction_date<=#" + endDate.ToString("yyyy-MM-dd") + "#");
                decimal check = ReportDT.SumTotal(unitInvDT, "invoice_master_gross") - ReportDT.SumTotal(unitInvDT, "invoice_master_paid") - ReportDT.SumTotal(unitReDT, "receipt_gross") - ReportDT.SumTotal(unitJDT, "gl_transaction_net") + ReportDT.SumTotal(unitReDT, "receipt_allocated");
                decimal totalpaid = ReportDT.SumTotal(unitReDT, "receipt_gross") + ReportDT.SumTotal(unitJDT, "gl_transaction_net") - ReportDT.SumTotal(unitReDT, "receipt_allocated");
                decimal interest = 0;
                if (check > 0)
                {
                    unitInvDT = ReportDT.FilterDT(unitInvDT, "", "invoice_master_due desc");
                    foreach (DataRow unitInvDR in unitInvDT.Rows)
                    {
                        decimal gross = decimal.Parse(unitInvDR["invoice_master_gross"].ToString()) - decimal.Parse(unitInvDR["invoice_master_paid"].ToString());
                        DateTime due = DateTime.Parse(unitInvDR["invoice_master_date"].ToString());
                        if (!unitInvDR["invoice_master_due"].ToString().Equals(""))
                            due = DateTime.Parse(unitInvDR["invoice_master_due"].ToString());
                        int invDue = 0;
                        if (startDate < due)
                        {
                            TimeSpan t = endDate - due;
                            invDue = t.Days;
                        }
                        if (startDate >= due)
                        {
                            TimeSpan t2 = endDate - startDate;
                            invDue = t2.Days;
                        }
                        if (invDue > 0)
                        {
                            if (totalpaid > 0)
                            {
                                if (totalpaid >= gross)
                                {
                                    totalpaid -= gross;
                                }
                                else
                                {
                                    gross -= totalpaid;
                                    totalpaid = 0;
                                    interest += (gross / 365) * invDue * taxtrate;
                                }
                            }
                            else
                            {
                                interest += (gross / 365) * invDue * taxtrate;
                            }
                        }
                    }
                }
                if (interest > 0)
                {
                    SetDT();
                    DataRow dr = dt.NewRow();
                    dr["UnitID"] = unit_id;
                    dr["Interest"] = interest.ToString("f2");
                    dr["Unit"] = unitDR["unit_master_code"].ToString();
                    dr["Amount"] = check;
                    dt.Rows.Add(dr);
                }
            }

            end = true;

        }

        public void Insert(string bodycorpid, string unitid, string description, string charid, decimal net, DateTime date)
        {
            string snet = net.ToString("f2");
            Odbc odbc = new Odbc(constr);
            InvoiceMaster i = new InvoiceMaster(constr);
            string invNum = i.GetNextNumber(-1);
            string detorid = ReportDT.GetDataByColumn(ReportDT.getTable(constr, "unit_master"), "unit_master_id", unitid, "unit_master_debtor_id");
            string insertSQL = "INSERT INTO invoice_master                 (invoice_master_num, invoice_master_type_id, invoice_master_debtor_id, invoice_master_bodycorp_id,                  invoice_master_unit_id, invoice_master_date, invoice_master_description,                   invoice_master_net, invoice_master_tax, invoice_master_gross, invoice_master_paid, invoice_master_sent) VALUES   ('" + invNum + "',1," + detorid + "," + bodycorpid + "," + unitid + "," + DBSafeUtils.DateToSQL(date) + ",'" + description + "'," + snet + ",0," + snet + ",0,0) ";
            string idsql = "SELECT LAST_INSERT_ID()";
            odbc.ExecuteScalar(insertSQL);
            string invID = Convert.ToInt32(odbc.ExecuteScalar(idsql)).ToString();
            string sql = "INSERT INTO gl_transactions                 (gl_transaction_type_id, gl_transaction_ref, gl_transaction_chart_id, gl_transaction_bodycorp_id, gl_transaction_unit_id,                  gl_transaction_description, gl_transaction_net, gl_transaction_tax, gl_transaction_gross,                  gl_transaction_date) VALUES   (1,'" + invID + "','" + charid + "'," + bodycorpid + "," + unitid + ",'" + description + "'," + snet + ",0," + snet + "," + DBSafeUtils.DateToSQL(date) + ") ";
            odbc.ReturnTable(sql, "temp");
            string gid = Convert.ToInt32(odbc.ExecuteScalar(idsql)).ToString();
            string invGLsql = "INSERT INTO invoice_gls (invoice_gl_invoice_id, invoice_gl_gl_id, invoice_gl_paid) VALUES   (" + invID + "," + gid + ",0)";
            odbc.ExecuteScalar(invGLsql);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Formatbodycory(Request.Cookies["bodycorpid"].Value);
                tempbid = Request.Cookies["bodycorpid"].Value;

            }
            if (GridView1.Rows.Count > 1)
            {
                GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }


        protected void Button1_Click1(object sender, EventArgs e)
        {
            //dt.Clear();
            //Thread t = new Thread(new ThreadStart(GetData));
            //t.Start();
            GetData();
            //Button1.Visible = false;
            //Timer1.Enabled = true;
            //Image1.Visible = true;
            Session["IntDT"] = dt;
            GridView1.DataSource = dt;

            GridView1.DataBind();
            if (GridView1.Rows.Count > 1)
            {
                GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
            }

        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (!end)
            {
                GridView1.DataSource = dt;
                GridView1.DataBind();
                if (GridView1.Rows.Count > 1)
                {
                    GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            else
            {
                GridView1.DataSource = dt;
                GridView1.DataBind();
                if (GridView1.Rows.Count > 1)
                {
                    GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
                Image1.Visible = false;
                Timer1.Enabled = false;

            }
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            if (GridView1.Rows.Count > 0)
            {
                foreach (GridViewRow gr in GridView1.Rows)
                {
                    CheckBox cb = (CheckBox)gr.Cells[3].FindControl("CheckBox1");
                    decimal net = decimal.Parse(gr.Cells[2].Text);
                    string unitid = cb.ToolTip.ToString();
                    if (cb.Checked)
                    {
                        DataTable sysDT = ReportDT.getTable(constr, "system");
                        DataTable chartDT = ReportDT.getTable(constr, "chart_master");
                        string invoiceChartCode = ReportDT.GetDataByColumn(sysDT, "system_code", "PENALTYINTCODE", "system_value");
                        string invoiceChartID = ReportDT.GetDataByColumn(chartDT, "chart_master_code", invoiceChartCode, "chart_master_id");
                        Insert(Request.Cookies["bodycorpid"].Value, unitid, "Interest", invoiceChartID, net, DateTime.Now);
                    }
                }
                GridView1.Enabled = false;
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new Sapp.General.User(constr_general);
                user.LoadData(login);
                Bodycorp bodycorp = new Bodycorp(constr);
                Hashtable items = new Hashtable();
                items.Add("bodycorp_close_period_date", DBSafeUtils.DateToSQL(EndDateT.Text));
                bodycorp.SetLog(user.UserId);
                bodycorp.Update(items, Convert.ToInt32(Request.Cookies["bodycorpid"].Value));
                if (GridView1.Rows.Count > 1)
                {
                    GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
                Response.Redirect("bodycorps.aspx", false);
            }
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            DataTable dt = (DataTable)Session["IntDT"];

            DataTable exDT = new System.Data.DataTable();
            exDT.Columns.Add("BodyCorpCode");
            exDT.Columns.Add("UnitNum");
            exDT.Columns.Add("InvDate");
            exDT.Columns.Add("InvDue");
            exDT.Columns.Add("InvDescription");
            exDT.Columns.Add("ChartCode");
            exDT.Columns.Add("LineDesc");
            exDT.Columns.Add("LineNet");
            exDT.Columns.Add("LineTax");
            exDT.Columns.Add("LineGross");
            DataTable bodycDT = ReportDT.getTable(constr, "bodycorps");
            foreach (DataRow dr in dt.Rows)
            {
                DataRow newDR = exDT.NewRow();
                newDR["BodyCorpCode"] = ReportDT.GetDataByColumn(bodycDT, "bodycorp_id", Request.Cookies["bodycorpid"].Value, "bodycorp_code");
                newDR["LineNet"] = dr["Interest"].ToString();
                newDR["LineTax"] = "0";
                newDR["LineGross"] = "0";
                newDR["InvDate"] = EndDateT.Text;
                newDR["InvDue"] = EndDateT.Text;
                newDR["UnitNum"] = dr["Unit"].ToString();
                newDR["ChartCode"] = "60.0100";
                newDR["LineDesc"] = "Interest";
                exDT.Rows.Add(newDR);
            }
            CsvDT.DataTableToCsv(exDT, Server.MapPath("~/Temp") + "\\" + "interest.csv");
            Response.Redirect("~/Temp/interest.csv", false);
            if (GridView1.Rows.Count > 1)
            {
                GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow gr in GridView1.Rows)
            {
                CheckBox c = (CheckBox)gr.Cells[3].FindControl("CheckBox1");
                c.Checked = ((CheckBox)sender).Checked;
            }
            if (GridView1.Rows.Count > 1)
            {
                GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }
    }
}