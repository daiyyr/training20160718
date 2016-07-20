using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.Data;
using System.Data.Odbc;
using Sapp.SMS;
using System.IO;
using System.Collections;
using Microsoft.Reporting.WebForms;
using System.Data;

namespace sapp_sms
{
    public partial class debtorreports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                /*
                string bodycorp_id = Request.Cookies["bodycorpid"].Value;

                TextBoxDateEnd.Text = DateTime.Today.ToString("dd/MM/yyyy");
                TextBoxDateStart.Text = new DateTime(DateTime.Today.AddYears(-1).Year, 12, 1).ToString("dd/MM/yyyy");

                ListItem all = new ListItem("ALL", "ALL");
                DropDownProprietorStatement.Items.Add(all);

                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Odbc mydb = new Odbc(constr);

                string sql = "SELECT debtor_master_id, debtor_master_code, debtor_master_name FROM debtor_master ";
                sql = sql + " WHERE debtor_master_type_id = 1 AND debtor_master_bodycorp_id = " + bodycorp_id + " ORDER BY debtor_master_code ";
                OdbcDataReader dr = mydb.Reader(sql);

                while (dr.Read())
                {
                    ListItem option = new ListItem(dr["debtor_master_code"].ToString() + " | " + dr["debtor_master_name"].ToString(), dr["debtor_master_id"].ToString());
                    DropDownProprietorStatement.Items.Add(option);
                }
                */ 
            }
        }
        #region WebControl Events
        protected void ImageButtonSubmit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string bodycorp_id = Request.Cookies["bodycorpid"].Value;
                //string start_date = Convert.ToDateTime(TextBoxDateStart.Text).ToString("dd/MM/yyyy");
                //string end_date = Convert.ToDateTime(TextBoxDateEnd.Text).ToString("dd/MM/yyyy");

                // Proprietor List Report
                if (CheckBoxProprietorList.Checked)
                {
                    Response.Write("<script type='text/javascript'> window.open('reportviewer.aspx?reportid=proprietorlist&args=" + bodycorp_id + "','_blank');  </script>");
                }

                /*
                // Proprietor Statement Report
                if (CheckBoxProprietorStatement.Checked)
                {
                    string ProprietorID = DropDownProprietorStatement.SelectedValue;
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    Odbc odbc = new Odbc(constr);
                    string sql = null;

                    // Summary report
                    DataTable reportResult = new DataTable();
                    reportResult.Columns.Add("proprietor_code");
                    reportResult.Columns.Add("proprietor_name");
                    reportResult.Columns.Add("proprietor_unit_id");
                    reportResult.Columns.Add("proprietor_unit");
                    reportResult.Columns.Add("report_path");
                    reportResult.Columns.Add("report_name");

                    // All proprietors
                    if (ProprietorID.Equals("ALL"))
                    {
                        sql = "SELECT unit_master_id, unit_master_code, unit_master_debtor_id, debtor_master_code, debtor_master_name FROM unit_master, debtor_master "
                            + "  WHERE unit_master_property_id IN (SELECT property_master_id FROM property_master WHERE property_master_bodycorp_id = " + bodycorp_id + " ) "
                            + "    AND unit_master_debtor_id IN (SELECT debtor_master_id FROM debtor_master WHERE debtor_master_type_id = 1 AND debtor_master_bodycorp_id = " + bodycorp_id + " ) "
                            + "    AND unit_master_debtor_id = debtor_master_id "
                            + "  ORDER BY debtor_master_code, "
                            + "           IF(LEFT(unit_master_code, 1) REGEXP '[0-9]', CONVERT(unit_master_code, UNSIGNED), 2147483648), "
                            + "           RIGHT(unit_master_code, LENGTH(unit_master_code) - LENGTH(CONVERT(unit_master_code, UNSIGNED))) ";
                    }
                    else
                    {
                        sql = "SELECT unit_master_id, unit_master_code, unit_master_debtor_id, debtor_master_code, debtor_master_name FROM unit_master, debtor_master "
                            + "  WHERE unit_master_property_id IN (SELECT property_master_id FROM property_master WHERE property_master_bodycorp_id = " + bodycorp_id + " ) "
                            + "    AND unit_master_debtor_id = " + ProprietorID
                            + "    AND unit_master_debtor_id = debtor_master_id "
                            + "  ORDER BY debtor_master_code, "
                            + "           IF(LEFT(unit_master_code, 1) REGEXP '[0-9]', CONVERT(unit_master_code, UNSIGNED), 2147483648), "
                            + "           RIGHT(unit_master_code, LENGTH(unit_master_code) - LENGTH(CONVERT(unit_master_code, UNSIGNED))) ";
                    }

                    OdbcDataReader dr = odbc.Reader(sql);
                    while (dr.Read())
                    {
                        DataRow row = reportResult.NewRow();
                        row["proprietor_code"] = dr["debtor_master_code"].ToString();
                        row["proprietor_name"] = dr["debtor_master_name"].ToString();
                        row["proprietor_unit_id"] = dr["unit_master_id"].ToString();
                        row["proprietor_unit"] = dr["unit_master_code"].ToString();
                        reportResult.Rows.Add(row);
                    }

                    if (reportResult.Rows.Count == 1)
                    {
                        // one pdf
                        DataRow row = reportResult.Rows[0];
                        Response.Write("<script type='text/javascript'> window.open('reportviewer.aspx?reportid=unitstatement&args=" + bodycorp_id + "|" + start_date + "|" + end_date + "|" + row["proprietor_unit_id"].ToString() + "','_blank');  </script>");
                    }
                    else if (reportResult.Rows.Count > 1)
                    {
                        // multi-pdf
                        Sapp.SMS.System system = new Sapp.SMS.System(constr);
                        ReportViewer ReportViewer1 = new ReportViewer();

                        // template base on bc
                        Bodycorp bodycorp = new Bodycorp(constr);
                        bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                        string tempplate = bodycorp.BodycorpStmtTpl;
                        if (tempplate == null || tempplate.Equals(""))
                        {
                            throw new Exception("Please set statement template for this bodycorp.");
                        }

                        system.LoadData("FILEFOLDER");
                        string filefolder = system.SystemValue + bodycorp.BodycorpCode + "\\Statement\\";
                        if (!Directory.Exists(filefolder))
                        {
                            Directory.CreateDirectory(filefolder);
                        }

                        // Unit base reports
                        foreach (DataRow row in reportResult.Rows) {
                            //string fileName = row["proprietor_code"].ToString() + "_" + row["proprietor_unit"].ToString() + ".pdf";
                            ReportUnitStatement Report = new ReportUnitStatement(constr, Server.MapPath("templates/" + tempplate), ReportViewer1);
                            Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), row["proprietor_unit_id"].ToString());
                            string fileName = Report.invoiceTable.Rows[0]["invoice_number"].ToString() + ".pdf";
                            Report.ExportPDF(filefolder + fileName);

                            row["report_path"] = filefolder;
                            row["report_name"] = fileName;
                        }

                        Session["PROPRIETOR_STATEMENT_SUMMARY"] = reportResult;
                        Response.Write("<script type='text/javascript'> window.open('reportviewer.aspx?reportid=ProprietorStatementSummary&args=" + bodycorp_id + "', '_blank');  </script>");
                    }
                
                }
                */ 
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}