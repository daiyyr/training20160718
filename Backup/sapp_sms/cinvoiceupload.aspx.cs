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
using System.Data.Odbc;

namespace sapp_sms
{
    public partial class cinvoiceupload : System.Web.UI.Page
    {
        static string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonImport_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = new Guid().ToString() + ".csv";
                FileUpload1.SaveAs(Server.MapPath("~") + "temp\\" + filename);
                UIImport(SetImpotDT(filename));
                FileInfo f = new FileInfo(Server.MapPath("~") + "temp\\" + filename);
                f.Delete();
                Response.Redirect("cinvoices.aspx", false);
            }
            catch (Exception ex)
            {

                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        public DataTable SetImpotDT(string csvfilename)
        {
            try
            {
                CSVIO csvio = new CSVIO();
                csvio.CsvReader(Server.MapPath("~") + "temp\\" + csvfilename);
                DataTable dt = csvio.CsvImport();
                csvio.CsvClose();
                return dt;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }

        public void UIImport(DataTable dt)
        {
            Odbc mydb = null;
            try
            {
                mydb = new Odbc(constr);
                mydb.StartTransaction();
                
                // Add 15/05/2016
                Sapp.SMS.System sys = new Sapp.SMS.System(constr);
                sys.LoadData("INVAPPLYDATEDEFAULT");
                string dateKbn = sys.SystemValue;

                ArrayList invNumList = new ArrayList();
                foreach (DataRow dr in dt.Rows)
                {
                    string chartcode = dr["ChartCode"].ToString();
                    if (chartcode.Equals(""))
                    {
                        throw new Exception("ChartCode is empty!");
                    }

                    ChartMaster chart = new ChartMaster(constr);
                    chart.SetOdbc(mydb);
                    chart.LoadData(chartcode);

                    if (chart.ChartMasterCode == null )
                    {
                        throw new Exception("Invalid ChartCode!");
                    }
                    string chartdescription = chart.ChartMasterName;
                    string invNum = dr["InvNum"].ToString();
                    string bodyID = Request.Cookies["bodycorpid"].Value;
                    string creditorid = "";
                    CreditorMaster creditor = new CreditorMaster(constr);
                    creditor.SetOdbc(mydb);
                    creditor.LoadData(dr["CreditorCode"].ToString());
                    if(creditor.CreditorMasterCode != null)
                        creditorid = creditor.CreditorMasterId.ToString();
                    else
                        throw new Exception("Invalid CreditorCode!");

                    Cinvoice cinv = new Cinvoice(constr);
                    cinv.SetOdbc(mydb);
                    if (invNum == null)
                        throw new Exception("InvNum is empty!");

                    if (!invNumList.Contains(invNum))
                    {
                        Hashtable invitems = new Hashtable();
                        invitems.Add("cinvoice_num", DBSafeUtils.StrToQuoteSQL(invNum));
                        invitems.Add("cinvoice_creditor_id", creditorid);
                        invitems.Add("cinvoice_type_id", 1);
                        invitems.Add("cinvoice_bodycorp_id", bodyID);
                        invitems.Add("cinvoice_date", DBSafeUtils.DateToSQL(dr["InvDate"]));
                        invitems.Add("cinvoice_due", DBSafeUtils.DateToSQL(dr["InvDue"]));

                        // Add 15/05/2016
                        //invitems.Add("cinvoice_apply", DBSafeUtils.DateToSQL(dr["InvDate"]));
                        if (dr["InvApply"].ToString().Equals(""))
                        {
                            if ("InvoiceDate".Equals(dateKbn))
                            {
                                invitems.Add("cinvoice_apply", DBSafeUtils.DateToSQL(dr["InvDate"]));
                            }
                            else
                            {
                                invitems.Add("cinvoice_apply", DBSafeUtils.DateToSQL(dr["InvDue"]));
                            }
                        }
                        else
                        {
                            invitems.Add("cinvoice_apply", DBSafeUtils.DateToSQL(dr["InvApply"]));
                        }

                        invitems.Add("cinvoice_description", DBSafeUtils.StrToQuoteSQL(dr["InvDescription"].ToString()));
                        invitems.Add("cinvoice_net", 0);
                        invitems.Add("cinvoice_tax", 0);
                        invitems.Add("cinvoice_gross", 0);
                        invitems.Add("cinvoice_paid", 0);


                        DateTime invdate = Convert.ToDateTime(dr["InvDate"].ToString());
                        DateTime duedate = Convert.ToDateTime(dr["InvDue"].ToString());

                        if (duedate < invdate)
                        {
                            throw new Exception("Invalid InvDue!");
                        }

                        int cinv_id = cinv.Add(invitems, true);

                        DataRow[] result = dt.Select("InvNum='" + invNum + "'");

                        Hashtable line_items = new Hashtable();
                        foreach (DataRow dr1 in result)
                        {

                            line_items.Add("gl_transaction_type_id", 2);
                            line_items.Add("gl_transaction_ref", DBSafeUtils.StrToQuoteSQL(cinv_id.ToString()));
                            line_items.Add("gl_transaction_chart_id", chart.ChartMasterId);
                            line_items.Add("gl_transaction_unit_id", null);
                            line_items.Add("gl_transaction_description", DBSafeUtils.StrToQuoteSQL(dr["LineDesc"].ToString()));
                            decimal line_gross = 0;
                            decimal.TryParse(dr["LineGross"].ToString().Replace("$", ""), out line_gross);
                            decimal line_net = 0;
                            decimal.TryParse(dr["LineNet"].ToString().Replace("$", ""), out line_net);
                            decimal line_tax = 0;
                            decimal.TryParse(dr["LineTax"].ToString().Replace("$", ""), out line_tax);
                            if (line_gross - line_net - line_tax != 0)
                            {
                                throw new Exception("Gross not equal Net + Tax!");
                            }
                            line_items.Add("gl_transaction_net", line_net);
                            line_items.Add("gl_transaction_tax", line_tax);
                            line_items.Add("gl_transaction_gross", line_gross);
                            line_items.Add("gl_transaction_date", DBSafeUtils.DateToSQL(Convert.ToDateTime(dr["InvDate"].ToString())));
                        }

                        cinv.AddGlTran(line_items, true);

                        invNumList.Add(invNum);
                    }
                }
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
        }

        protected void ImageButtonTemplate_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("InvNum");
                dt.Columns.Add("CreditorCode");
                dt.Columns.Add("ChartCode");
                dt.Columns.Add("InvDate");
                dt.Columns.Add("InvDue");
                dt.Columns.Add("InvApply");     // 15/05/2016
                dt.Columns.Add("InvDescription");
                dt.Columns.Add("LineGross");
                dt.Columns.Add("LineNet");
                dt.Columns.Add("LineTax");
                dt.Columns.Add("LineDesc");
                
                CSVIO csvio = new CSVIO();

                string filefolder = Server.MapPath("~/temp");
                string filename = Guid.NewGuid().ToString();
                csvio.CsvWriter(filefolder + "\\" + filename + ".csv");
                csvio.CsvExport(dt);
                csvio.CsvClose();
                FileInfo fo = new FileInfo(filefolder + "\\" + filename + ".csv");
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fo.Name);
                HttpContext.Current.Response.AddHeader("Content-Length", fo.Length.ToString());
                HttpContext.Current.Response.ContentType = "text/plain";
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.TransmitFile(fo.FullName);
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
    }
}