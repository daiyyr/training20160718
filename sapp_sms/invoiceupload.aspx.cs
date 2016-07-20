using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Configuration;
using System.IO;
using Sapp.Common;
using Sapp.SMS;
using Sapp.JQuery;
using Sapp.Data;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Globalization;
using System.Threading;

using System.Data.OleDb;
using System.Data.Odbc;
using System.Net.Mail;
using System.Net;

namespace sapp_sms
{
    public partial class invoiceupload : System.Web.UI.Page
    {
        public int irow = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        protected void Button1_Click1(object sender, System.EventArgs e)
        {
            try
            {
                string filename = Guid.NewGuid().ToString().Replace("-", "");

                string path = Server.MapPath("~") + "\\temp\\";
                FileUpload1.SaveAs(path + filename);
                UIImport(SetImpotDT(path, filename));
                FileInfo f = new FileInfo(path + filename);
                f.Delete();
                Response.Redirect("invoicemaster.aspx", false);
            }
            catch (Exception ex)
            {

                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString() + "<br><br>Error At Import File Row Index:" + irow.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        public DataTable SetImpotDT(string path, string filename)
        {
            DataTable dt = CsvDT.CsvToDataTable(path, filename);

            return dt;
        }
        public string GetIDbyeName(string name, string col, string Table)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string result = "";
            Odbc o = new Odbc(constr);
            string sql = "select * from `" + Table + "` where `" + col + "` = '" + name.Replace("'", "''") + "'";
            DataTable dt = o.ReturnTable(sql, "type");
            if (dt.Rows.Count > 0)
                result = dt.Rows[0][0].ToString();
            return result;
        }
        public void UIImport(DataTable dt)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            Odbc o = new Odbc(constr);

            //DataTable bodycorpsDT = ReportDT.getTable(constr, "bodycorps");
            //DataTable propertyDT = ReportDT.getTable(constr, "property_master");

            // Update 30/05/2016
            //DataTable chartDT = ReportDT.getTable(constr, "chart_master");
            string sql = "SELECT * FROM chart_master WHERE chart_master_levy_base = 1 ";
            DataTable chartDT = o.ReturnTable(sql, "chart_master");

            DataTable invAL = new DataTable();
            invAL.Columns.Add("Key");
            invAL.Columns.Add("invNum");

            //DataTable invBatch = o.ReturnTable("select * from invoice_master order by  invoice_master_batch_id desc", "g2");
            //int batch = 0;
            //int.TryParse(invBatch.Rows[0]["invoice_master_batch_id"].ToString(), out batch);
            //batch++;
            // Get and update last lavy_id & gl_transaction_batch_id 31/03/2016
            Sapp.SMS.System sys = new Sapp.SMS.System(constr);
            sys.LoadData("INVOICEBATCHPILOT");
            int batch = int.Parse(sys.SystemValue) + 1;
            Hashtable item = new Hashtable();
            item.Add("system_value", batch.ToString());
            sys.Update(item);

            // Add 15/05/2016
            sys.LoadData("INVAPPLYDATEDEFAULT");
            string dateKbn = sys.SystemValue;

            o.StartTransaction();
            try
            {

                foreach (DataRow newdr in dt.Rows)
                {

                    irow++;
                    string ccode = newdr["ChartCode"].ToString();
                    if (ccode.Equals(""))
                    {
                        //throw new Exception("chart code " + ccode + " not found ");
                        throw new Exception(CsvDT.editErrorMessage(irow + 1, "ChartCode", "chart code is empty"));
                    }

                    string chartid = "";
                    if (dt.Columns.Contains("ChartID"))
                        chartid = newdr["ChartID"].ToString();
                    else
                        chartid = ReportDT.GetDataByColumn(chartDT, "chart_master_code", ccode, "chart_master_id");
                    if (chartid.Equals(""))
                    {
                        //throw new Exception("chart code " + ccode + " not found or levy base ");    // 30/05/2016
                        throw new Exception(CsvDT.editErrorMessage(irow + 1, "ChartCode", "chart code '" + ccode + "' can not be not found in DB or it is levy base"));
                    }
                    string chartdescription = ReportDT.GetDataByColumn(chartDT, "chart_master_id", chartid, "chart_master_name");
                    string invNum = newdr["InvNum"].ToString();
                    string bodyID = Request.Cookies["bodycorpid"].Value;
                    DataTable unitDT = o.ReturnTable("SELECT        * FROM            unit_master INNER JOIN                          property_master ON unit_master.unit_master_property_id = property_master.property_master_id INNER JOIN                          bodycorps ON  property_master.property_master_bodycorp_id = bodycorps.bodycorp_id where  bodycorps.bodycorp_id=" + bodyID, "t1");
                    string unit_master_id = ReportDT.GetDataByColumn(unitDT, "unit_master_code", newdr["UnitNum"].ToString(), "unit_master_id");

                    InvoiceMaster invoice = new InvoiceMaster(constr);
                    invoice.SetOdbc(o);
                    string num = "";
                    num = ReportDT.GetDataByColumn(invAL, "Key", bodyID + unit_master_id, "invNum");
                    if (invNum.Equals("") && num.Equals(""))
                    {
                        invNum = invoice.GetNextNumber(-1);
                    }
                    else if (!invNum.Equals("") && !num.Equals(""))
                    {
                        //throw new Exception("Not Match Invoice Num Rule");
                        throw new Exception(CsvDT.editErrorMessage(irow + 1, "invNum", "invoice number should be empty here for the same unit id [" + newdr["UnitNum"].ToString() + "]"));
                    }
                    if (num.Equals(""))
                    {

                        Hashtable inv_items = new Hashtable();
                        UnitMaster unit = new UnitMaster(constr);
                        if (unit_master_id.Equals(""))
                        {
                            //throw new Exception("unit " + newdr["UnitNum"].ToString() + " not found ");
                            throw new Exception(CsvDT.editErrorMessage(irow + 1, "UnitNum", "unit master code [" + newdr["UnitNum"].ToString() + "] can not be found in DB"));
                        }
                        unit.LoadData(int.Parse(unit_master_id));
                        inv_items.Add("invoice_master_num", DBSafeUtils.StrToQuoteSQL(invNum));
                        inv_items.Add("invoice_master_type_id", 1);
                        inv_items.Add("invoice_master_debtor_id", unit.UnitMasterDebtorId);
                        inv_items.Add("invoice_master_bodycorp_id", bodyID);
                        inv_items.Add("invoice_master_unit_id", unit.UnitMasterId);
                        inv_items.Add("invoice_master_batch_id", batch);
                        DateTime invdate = Convert.ToDateTime(newdr["InvDate"].ToString());
                        DateTime duedate = Convert.ToDateTime(newdr["InvDue"].ToString());

                        // Add 15/05/2016
                        DateTime applydate;
                        if (newdr["InvApply"].ToString().Equals(""))
                        {
                            if ("InvoiceDate".Equals(dateKbn))
                            {
                                applydate = invdate;
                            }
                            else
                            {
                                applydate = duedate;
                            }
                        }
                        else
                        {
                            applydate = Convert.ToDateTime(newdr["InvApply"].ToString());  
                        }

                        if (duedate < invdate)
                        {
                            //throw new Exception("Due Date error");
                            throw new Exception(CsvDT.editErrorMessage(irow + 1, "InvDue", "due date [" + newdr["InvDue"].ToString() + "] should not be earlier than invoice date [" + newdr["InvDate"].ToString() + "]"));
                        }
                        inv_items.Add("invoice_master_date", DBSafeUtils.DateToSQL(invdate));
                        inv_items.Add("invoice_master_due", DBSafeUtils.DateToSQL(duedate));
                        inv_items.Add("invoice_master_apply", DBSafeUtils.DateToSQL(applydate));    // Add 06/05/2016
                        inv_items.Add("invoice_master_description", DBSafeUtils.StrToQuoteSQL(newdr["InvDescription"].ToString()));
                        inv_items.Add("invoice_master_net", 0);
                        inv_items.Add("invoice_master_tax", 0);
                        inv_items.Add("invoice_master_gross", 0);
                        inv_items.Add("invoice_master_paid", 0);
                        inv_items.Add("invoice_master_sent", 1);
                        int invoice_id = invoice.Add(inv_items, true);
                        DataRow ndr = invAL.NewRow();
                        ndr["Key"] = bodyID + unit_master_id;
                        ndr["invNum"] = invNum;
                        invAL.Rows.Add(ndr);
                    }
                    else
                    {
                        invoice.LoadData(DBSafeUtils.StrToQuoteSQL(num));
                    }
                    decimal balance = 0;
                    decimal.TryParse(newdr["LineGross"].ToString().Replace("$", ""), out balance);
                    Hashtable line_items = new Hashtable();
                    line_items.Add("gl_transaction_type_id", 1);
                    line_items.Add("gl_transaction_ref", DBSafeUtils.StrToQuoteSQL(num.ToString()));
                    line_items.Add("gl_transaction_chart_id", chartid);
                    line_items.Add("gl_transaction_unit_id", unit_master_id);
                    line_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(newdr["LineDesc"].ToString()));
                    decimal line_gross = 0;
                    decimal.TryParse(newdr["LineGross"].ToString().Replace("$", ""), out line_gross);
                    decimal line_net = 0;
                    decimal.TryParse(newdr["LineNet"].ToString().Replace("$", ""), out line_net);
                    decimal line_tax = 0;
                    decimal.TryParse(newdr["LineTax"].ToString().Replace("$", ""), out line_tax);
                    if (line_gross - line_net - line_tax != 0)
                    {
                        //throw new Exception("Gross not equal net + tax !");
                        throw new Exception(CsvDT.editErrorMessage(irow + 1, "LineGross", "the gross [" + line_gross + "] is not equal to the summary of net [" + line_net + "] and tax [" + line_tax + "]"));
                    }
                    //if (line_net + line_tax != line_gross)
                    //{
                    //    throw new Exception("Gross not match");
                    //}
                    line_items.Add("gl_transaction_net", line_net);
                    line_items.Add("gl_transaction_tax", line_tax);
                    line_items.Add("gl_transaction_gross", line_gross);
                    line_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(Convert.ToDateTime(newdr["InvDate"].ToString())));
                    invoice.AddGlTran(line_items, true);

                }
                o.Commit();
            }
            catch (Exception ex)
            {
                if (o != null) o.Rollback();
                throw ex;
            }
            finally
            {
                if (o != null) o.Close();
            }
        }
        public Hashtable MakeHsahTable(DataRow dr)
        {
            Hashtable ht = new Hashtable();
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                string cname = dr.Table.Columns[i].ColumnName;
                string v = dr[cname].ToString();
                if (!v.Equals("Null"))
                    ht.Add(cname, "'" + v + "'");
            }
            return ht;
        }
    }
}