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
using System.Data.Odbc;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Net.Mail;

namespace sapp_sms
{
    public partial class invoicesend : System.Web.UI.Page, IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridInvoice };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion

                if (!IsPostBack)
                {
                    #region Load Page
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        #region Initial ComboBox
                        AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`,  CONCAT(`bodycorp_code`,' | ',`bodycorp_name`) AS `Code` FROM `bodycorps`", "ID", "Code", constr, false);
                        #endregion

                        TextBoxStart.Text = "";
                        TextBoxEnd.Text = "";

                        CheckBoxStmt.Checked = false;
                        CheckBoxNotes.Checked = false;
                        TextBoxStmtStartDate.Text = "";
                        TextBoxStmtEndDate.Text = "";
                        TextBoxSubject.Text = "";
                        TextBoxBody.Text = "";

                        Session["INVOICE_SEND_SUMMARY"] = null;
                        ImageButtonSendEmail.Enabled = false;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    #endregion

                    if (Request.Cookies["bodycorpid"] != null)
                    {
                        ComboBoxBodycorp.SelectedValue = Request.Cookies["bodycorpid"].Value;
                        ComboBoxBodycorp.Enabled = false;
                    }
                }

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        protected void ImageButtonLoad_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string bodycorpid = "";
                string start = "";
                string end = "";
                bodycorpid = AjaxControlUtils.ComboBoxSelectedValue(ComboBoxBodycorp, false).Value.ToString();
                start = TextBoxStart.Text;
                end = TextBoxEnd.Text;
                if (bodycorpid != "") Session["invoicesend_bodycorpid"] = bodycorpid;
                if (start != "") Session["invoicesend_start"] = start;
                if (end != "") Session["invoicesend_end"] = end;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        [System.Web.Services.WebMethod]
        public static string jqGridInvoiceDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string bodycorpid = "";
            string start = "";
            string end = "";

            if (HttpContext.Current.Session["invoicesend_bodycorpid"] != null)
                bodycorpid = HttpContext.Current.Session["invoicesend_bodycorpid"].ToString();
            if (HttpContext.Current.Session["invoicesend_start"] != null)
                start = HttpContext.Current.Session["invoicesend_start"].ToString();
            if (HttpContext.Current.Session["invoicesend_end"] != null)
                end = HttpContext.Current.Session["invoicesend_end"].ToString();

            if (bodycorpid == "" || start == "" || end == "")
            {
                return "{}";
            }
            else
            {
                // Update 26/04/2016
                #region Old sql
                /*
                string sqlfromstr = "FROM (`invoice_master` LEFT JOIN `debtor_master` ON `invoice_master_debtor_id`=`debtor_master_id`) "
                    + "WHERE `invoice_master_bodycorp_id`=" + bodycorpid + " AND (`invoice_master_date` BETWEEN " + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") "
                    + "AND `invoice_master_sent`=0";
                 * 
                string sqlfromstr = "FROM (`invoice_master` LEFT JOIN `debtor_master` ON `invoice_master_debtor_id`=`debtor_master_id`), debtor_comms, comm_master "
                    + "WHERE `invoice_master_bodycorp_id`=" + bodycorpid + " AND (`invoice_master_date` BETWEEN " + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") "
                    + "  AND invoice_master_sent=0"
                    + "  AND invoice_master_debtor_id = debtor_comm_debtor_id "
                    + "  AND debtor_comm_comm_id =  comm_master_id "
                    + "  AND comm_master_type_id = 7 "
                    + "  AND comm_master_primary = 1 "
                    + "  AND comm_master_data IS NOT NULL ";

                string sqlselectstr = "SELECT `ID`,`Num`,`Debtor`,`Date`,`Due`, `Total`, Invoice, Statement, Notes "
                    + "FROM (SELECT  `invoice_master_id` as `ID`,`invoice_master_num` as `Num`,`debtor_master_name` AS `Debtor`, DATE_FORMAT(`invoice_master_date`, '%d/%m/%Y') AS `Date`, "
                    + "DATE_FORMAT(`invoice_master_due`, '%d/%m/%Y') AS `Due`, `invoice_master_gross` AS `Total` "
                    + ", '-' AS Invoice, '-' AS Statement, '-' AS Notes ";
                */
                #endregion

                #region get DB data
                
                Odbc odbc = null;
                DataTable dt = null;
                DataTable mailSummary = null;
                string sql = null;

                try
                {
                    sql = " SELECT IM.invoice_master_id as ID, "
                        + "        IM.invoice_master_num as Num, "
                        + "        DM.debtor_master_name AS Debtor, "
                        + "        DATE_FORMAT(IM.invoice_master_date, '%d/%m/%Y') AS Date, "
                        + "        DATE_FORMAT(IM.invoice_master_due, '%d/%m/%Y') AS Due, "
                        + "        IM.invoice_master_gross AS Total,  "
                        + "        '-' AS Invoice, "
                        + "        '-' AS Statement, "
                        + "        '-' AS Notes "
                        + " FROM invoice_master AS IM, debtor_master AS DM, debtor_comms AS DC, comm_master AS CM "
                        + " WHERE IM.invoice_master_bodycorp_id = " + bodycorpid + " AND (IM.invoice_master_date BETWEEN " + DBSafeUtils.DateToSQL(start) + " AND " + DBSafeUtils.DateToSQL(end) + ") "
                        + "   AND IM.invoice_master_sent = 0 "
                        + "   AND IM.invoice_master_debtor_id = DM.debtor_master_id "
                        + "   AND IM.invoice_master_debtor_id = DC.debtor_comm_debtor_id "
                        + "   AND DC.debtor_comm_comm_id = CM.comm_master_id "
                        + "   AND CM.comm_master_type_id = 7 "
                        + "   AND CM.comm_master_primary = 1 "
                        + "   AND CM.comm_master_data IS NOT NULL ";

                    odbc = new Odbc(constr);
                    dt = odbc.ReturnTable(sql, "InvoiceMailTemp");
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
                }
                finally
                {
                    if (odbc != null)
                    {
                        odbc.Close();
                    }
                }
                #endregion

                #region update status of attachments
                if (HttpContext.Current.Session["INVOICE_SEND_SUMMARY"] != null)
                {
                    mailSummary = ((DataTable)HttpContext.Current.Session["INVOICE_SEND_SUMMARY"]).Copy();

                    foreach (DataRow mailrow in mailSummary.Rows)
                    {
                        DataRow[] rows = dt.Select("ID = " + mailrow["invoice_master_id"].ToString());

                        if (rows != null)
                        {
                            DataRow row = rows[0];
                            if (mailrow["invoice_path"] != null)
                            {
                                row["Invoice"] = "Ready";
                            }
                            if (string.IsNullOrEmpty(mailrow["statement_path"].ToString()) == false)
                            {
                                row["Statement"] = "Ready";
                            }
                            if (string.IsNullOrEmpty(mailrow["notes_path"].ToString()) == false)
                            {
                                row["Notes"] = "Ready";
                            }
                        }
                    }
                }
                #endregion

                string sqlfromstr = " FROM " + dt.TableName;
                string sqlselectstr = " SELECT ID, Num, Debtor, Date, Due, Total, Invoice, Statement, Notes FROM (SELECT * ";

                //JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
        }


        public void RaisePostBackEvent(string eventArgument)
        {
            try
            {
                string[] args = eventArgument.Split('|');
                if (args[0] == "ImageButtonNewReport")
                {
                    ImageButtonNewReport_Click(args);
                }
                if (args[0] == "ImageButtonSendEmail")
                {
                    ImageButtonSendEmail_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        protected void ImageButtonNewReport_Click(string[] args)
        {
            #region variables
            Odbc odbc = null;
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string bodycorp_id = HttpContext.Current.Request.Cookies["bodycorpid"].Value;
            string tempplate = null;

            // Summary report
            DataTable mailSummary = new DataTable();
            mailSummary.Columns.Add("invoice_master_id");
            mailSummary.Columns.Add("invoice_master_num");
            mailSummary.Columns.Add("invoice_master_debtor_id");
            mailSummary.Columns.Add("invoice_master_unit_id");
            mailSummary.Columns.Add("debtor_master_code");
            mailSummary.Columns.Add("debtor_master_name");
            mailSummary.Columns.Add("unit_master_code");
            mailSummary.Columns.Add("invoice_path");
            mailSummary.Columns.Add("statement_path");
            mailSummary.Columns.Add("notes_path");
            mailSummary.Columns.Add("mail_address");
            mailSummary.Columns.Add("mail_datetime");
            mailSummary.Columns.Add("mail_result");
            #endregion

            Session["INVOICE_SEND_SUMMARY"] = null;

            #region Get invoice list & address
            string json = "";

            for (int i = 1; i < args.Length; i++)
            {
                json += args[i];
            }
            ArrayList invoiceIdList = (ArrayList)JSON.JsonDecode(json);

            try
            {
                odbc = new Odbc(constr);

                for (int i = 0; i < invoiceIdList.Count; i++)
                {
                    DataRow mailrow = mailSummary.NewRow(); 
                    
                    string invoice_id = invoiceIdList[i].ToString();
                    string sql = "SELECT * FROM invoice_master, debtor_comms, comm_master, debtor_master, unit_master "
                                + "  WHERE invoice_master_id = " + invoice_id
                                + "    AND invoice_master_debtor_id = debtor_comm_debtor_id "
                                + "    AND invoice_master_debtor_id = debtor_master_id "
                                + "    AND invoice_master_unit_id = unit_master_id "
                                + "    AND debtor_comm_comm_id =  comm_master_id "
                                + "    AND comm_master_type_id = 7 "
                                + "    AND comm_master_primary = 1 ";

                    OdbcDataReader reader = odbc.Reader(sql);
                    if (reader.Read())
                    {
                        mailrow["invoice_master_id"] = reader["invoice_master_id"].ToString();
                        mailrow["invoice_master_num"] = reader["invoice_master_num"].ToString();
                        mailrow["invoice_master_debtor_id"] = reader["invoice_master_debtor_id"].ToString();
                        mailrow["invoice_master_unit_id"] = reader["invoice_master_unit_id"].ToString();
                        mailrow["debtor_master_code"] = reader["debtor_master_code"].ToString();
                        mailrow["debtor_master_name"] = reader["debtor_master_name"].ToString();
                        mailrow["unit_master_code"] = reader["unit_master_code"].ToString();
                        mailrow["mail_address"] = reader["comm_master_data"].ToString();

                        mailSummary.Rows.Add(mailrow);
                    }

                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            #endregion

            #region Get inoive report template
            // template base on bc
            Bodycorp bodycorp = new Bodycorp(constr);
            bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
            tempplate = bodycorp.BodycorpInvTpl;

            if (tempplate == null || tempplate.Equals(""))
            {
                throw new Exception("Please set invoice template for this bodycorp.");
            }
            #endregion

            #region Save notes file
            string notesPath = "";
            if (CheckBoxNotes.Checked == true)
            {
                // Check notes file (pdf)
                if (FileUploadNotes.PostedFile.FileName.EndsWith(".pdf"))
                {
                    notesPath = Server.MapPath("~") + "temp\\" + FileUploadNotes.PostedFile.FileName;
                    FileUploadNotes.SaveAs(notesPath);
                }
                else
                {
                    throw new Exception("Please select pdf file as notes.");
                }
            }
            #endregion

            try
            {
                Sapp.SMS.System system = new Sapp.SMS.System(constr);
                system.LoadData("FILEFOLDER");

                foreach (DataRow row in mailSummary.Rows)
                {
                    #region Create invoice report
                    string filefolder = system.SystemValue + bodycorp.BodycorpCode + "\\INVOICE\\";
                    if (!Directory.Exists(filefolder))
                    {
                        Directory.CreateDirectory(filefolder);
                    }

                    string invoicePath = filefolder + row["invoice_master_num"].ToString() + ".pdf";
                    if (File.Exists(invoicePath) == false)
                    {
                        //ReportInvoice report = new ReportInvoice(constr, Server.MapPath("templates/" + tempplate));
                        rpInvoice report = new rpInvoice(constr, Server.MapPath("templates/" + tempplate));
                        report.SetReportInfo(Convert.ToInt32(row["invoice_master_id"].ToString()));
                        report.ExportPDF(invoicePath);
                    }

                    row["invoice_path"] = invoicePath;
                    #endregion

                    #region Create unit statement
                    string stmtStartDate = TextBoxStmtStartDate.Text;
                    string stmtEndDate = TextBoxStmtEndDate.Text;
                    string statementPath = "";
                    if (CheckBoxStmt.Checked == true && stmtStartDate.Equals("") == false && stmtEndDate.Equals("") == false && row["invoice_master_unit_id"] != null)
                    {
                        string stmtFolder = system.SystemValue + bodycorp.BodycorpCode + "\\Statement\\";
                        if (!Directory.Exists(stmtFolder))
                        {
                            Directory.CreateDirectory(stmtFolder);
                        }

                        ReportViewer viewer = new ReportViewer();
                        ReportUnitStatement stmtReport = new ReportUnitStatement(constr, Server.MapPath("templates/statement_template.rdlc"), viewer);
                        stmtReport.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(stmtStartDate), Convert.ToDateTime(stmtEndDate), row["invoice_master_unit_id"].ToString());
                        statementPath = stmtFolder + stmtReport.invoiceTable.Rows[0]["invoice_number"].ToString() + ".pdf";
                        stmtReport.ExportPDF(statementPath);
                    }

                    row["statement_path"] = statementPath;
                    #endregion

                    row["notes_path"] = notesPath;
                }

                Session["INVOICE_SEND_SUMMARY"] = mailSummary;
                ImageButtonSendEmail.Enabled = true;
            }
            catch (Exception ex)
            {
                odbc.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            finally
            {
                if (odbc != null)
                {
                    odbc.Close();
                }
            }

        }


        protected void ImageButtonSendEmail_Click(string[] args)
        {
            #region variables
            Odbc odbc = null;
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string bodycorp_id = HttpContext.Current.Request.Cookies["bodycorpid"].Value;

            // Summary report
            DataTable mailSummary = null;
            if (Session["INVOICE_SEND_SUMMARY"] != null)
            {
                mailSummary = (DataTable)Session["INVOICE_SEND_SUMMARY"];
            }
            else
            {
                return;
            }
            #endregion

            try
            {
                odbc = new Odbc(AdFunction.conn);
                string invoice_master_id = null;

                #region Load & initialize Email Server
                Sapp.SMS.System system = new Sapp.SMS.System(constr);

                system.LoadData("EMAILFROM");
                string from_address = system.SystemValue;

                system.LoadData("EMAILSERVER");
                string server_address = system.SystemValue;

                system.LoadData("EMAILPORT");
                string server_port = system.SystemValue;

                system.LoadData("EMAILUSER");
                string server_username = system.SystemValue;

                system.LoadData("EMAILPWD");
                string server_password = system.SystemValue;

                SmtpClient smtpSrv = new SmtpClient();
                smtpSrv.Host = server_address;
                smtpSrv.Port = Convert.ToInt32(server_port);
                smtpSrv.DeliveryMethod = SmtpDeliveryMethod.Network;
                //smtpSrv.EnableSsl = true;         // In case of SSL
                smtpSrv.UseDefaultCredentials = false;                                                      // Order: First step
                smtpSrv.Credentials = new System.Net.NetworkCredential(server_username, server_password);   // Order: Next step

                #endregion

                #region Send mails
                for (int i = 1; i < args.Count(); i++)
                {
                    invoice_master_id = args[i];
                    DataRow mailrow = null;

                    DataRow[] rows = mailSummary.Select("invoice_master_id = " + args[i]);

                    if (rows != null)
                    {
                        mailrow = rows[0];
                        MailMessage mailMsg = new MailMessage();

                        #region Check attachments
                        bool can_send_mail = true;

                        if (string.IsNullOrEmpty(mailrow["invoice_path"].ToString()) == false)
                        {
                            if (System.IO.File.Exists(mailrow["invoice_path"].ToString()))
                            {
                                Attachment attachment = new Attachment(mailrow["invoice_path"].ToString());
                                mailMsg.Attachments.Add(attachment);
                            }
                            else
                            {
                                mailrow["mail_datetime"] = DateTime.Now.ToLocalTime();
                                mailrow["mail_result"] = "Missing invoice [" + mailrow["invoice_path"].ToString() + "].";
                                can_send_mail = false;
                            }
                        }
                        else
                        {
                            mailrow["mail_datetime"] = DateTime.Now.ToLocalTime();
                            mailrow["mail_result"] = "Missing invoice [" + mailrow["invoice_path"].ToString() + "].";
                            can_send_mail = false;
                        }

                        if (string.IsNullOrEmpty(mailrow["statement_path"].ToString()) == false)
                        {
                            if (System.IO.File.Exists(mailrow["statement_path"].ToString()))
                            {
                                Attachment attachment = new Attachment(mailrow["statement_path"].ToString());
                                mailMsg.Attachments.Add(attachment);
                            }
                            else
                            {
                                mailrow["mail_datetime"] = DateTime.Now.ToLocalTime();
                                mailrow["mail_result"] = "Missing unit statement [" + mailrow["statement_path"].ToString() + "].";
                                can_send_mail = false;
                            }
                        }

                        if (string.IsNullOrEmpty(mailrow["notes_path"].ToString()) == false)
                        {
                            if (System.IO.File.Exists(mailrow["notes_path"].ToString()))
                            {
                                Attachment attachment = new Attachment(mailrow["notes_path"].ToString());
                                mailMsg.Attachments.Add(attachment);
                            }
                            else
                            {
                                mailrow["mail_datetime"] = DateTime.Now.ToLocalTime();
                                mailrow["mail_result"] = "Missing notes file [" + mailrow["notes_path"].ToString() + "].";
                                can_send_mail = false;
                            }
                        }
                        #endregion

                        #region Sent Email & Update DB
                        if (can_send_mail == true)
                        {
                            // DEBUG STRART <=================================================================
                            string mail_address = mailrow["mail_address"].ToString();
                            mail_address = "Kaii.xie@advanceit.co.nz";
                            // DEBUG END    <=================================================================

                            mailMsg.From = new MailAddress(from_address);
                            mailMsg.Subject = DBSafeUtils.DBStrToStr(TextBoxSubject.Text);
                            mailMsg.Body = DBSafeUtils.DBStrToStr(TextBoxBody.Text);

                            if (mail_address.Contains(";"))
                            {
                                string[] emails = mail_address.Split(';');
                                foreach (string email in emails)
                                {
                                    if (email != "")
                                    {
                                        mailMsg.To.Add(email);
                                    }
                                }
                            }
                            else
                            {
                                mailMsg.To.Add(mail_address);
                            }

                            try
                            {
                                smtpSrv.Send(mailMsg);

                                DateTime time = DateTime.Now.ToLocalTime();
                                mailrow["mail_datetime"] = time;
                                mailrow["mail_result"] = "Successful";

                                #region Update DB (invoice_master_sent)

                                InvoiceMaster invoice = new InvoiceMaster(constr);
                                invoice.LoadData(Convert.ToInt32(mailrow["invoice_master_id"].ToString()));
                                Hashtable sent_flg = new Hashtable();
                                sent_flg.Add("invoice_master_sent", 1);
                                invoice.Update(sent_flg);
                                
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                mailrow["mail_datetime"] = DateTime.Now.ToLocalTime();
                                mailrow["mail_result"] = "Failed";
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                #region Summary report
                Session["INVOICE_SEND_SUMMARY"] = null;
                ImageButtonSendEmail.Enabled = false;
                Session["INVOICE_SEND_REPORT"] = mailSummary;

                string url = "<script type='text/javascript'> window.open('reportviewer.aspx?reportid=InvoiceSendSummary&args=" + bodycorp_id + "', '_blank');  </script>";
                ClientScript.RegisterStartupScript(this.GetType(), "script", url);

                #endregion
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            finally
            {
                if (odbc != null)
                {
                    odbc.Close();
                }
            }
        }


    }
}