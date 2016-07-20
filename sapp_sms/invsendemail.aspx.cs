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
using System;
using System.Collections.Generic;
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
    public partial class invsendemail : System.Web.UI.Page
    {
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        private static DataTable emaildt;
        private static DataTable InvoiceDT = new DataTable();
        private static DataTable StatementDT = new DataTable();
        private static DataTable UploadDT = new DataTable();
        private static DateTime sStartDate = new DateTime();
        private static DateTime sEndDate = new DateTime();
        private static DateTime iStartDate = new DateTime();
        private static DateTime iEndDate = new DateTime();
        public void SendEmail()
        {
            try
            {
                if (InvCheckBox.Checked)
                {
                    iStartDate = DateTime.Parse(IStartT.Text);
                    iEndDate = DateTime.Parse(IEndT.Text);
                }
                if (StatementCheckBox.Checked)
                {
                    sEndDate = DateTime.Parse(SEDateT.Text);
                    sStartDate = sEndDate.AddMonths(-1);
                }
                foreach (DataRow dr in emaildt.Rows)
                {
                    string email = dr["comm_master_data"].ToString();
                    string uid = dr["unit_master_id"].ToString();
                    string name = dr["debtor_master_name"].ToString();
                    if (InvCheckBox.Checked)
                    {
                        GetInvID(uid, iStartDate, iEndDate);
                    }
                    if (StatementCheckBox.Checked)
                    {

                        MakeStatement("1", sStartDate, sEndDate, uid);
                    }
                    DataTable dt = AttachmentDT();
                    SendEmail(SubT.Text, email, BodyT.Text, name, dt);
                    CleanStatementDT();
                    CleanInvDT();
                    dr["Send"] = "Yes";
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        public void SetDT()
        {
            if (!InvoiceDT.Columns.Contains("FilePath"))
            {
                InvoiceDT.Columns.Add("FilePath");
                InvoiceDT.Columns.Add("FileName");
                InvoiceDT.Columns.Add("Extension");
                UploadDT = InvoiceDT.Clone();
                StatementDT = InvoiceDT.Clone();
            }
        }
        public void CleanInvDT()
        {
            foreach (DataRow dr in InvoiceDT.Rows)
            {
                FileInfo f = new System.IO.FileInfo(dr["FilePath"].ToString());
                f.Delete();
            }
            InvoiceDT.Clear();
        }
        public void CleanStatementDT()
        {
            foreach (DataRow dr in StatementDT.Rows)
            {
                FileInfo f = new System.IO.FileInfo(dr["FilePath"].ToString());
                f.Delete();
            }
            StatementDT.Clear();
        }

        public void GetInvID(string unitID, DateTime start, DateTime end)
        {
            string sql = "SELECT        invoice_master_unit_id, invoice_master_date, invoice_master_id FROM invoice_master where invoice_master_unit_id =" + unitID + " and invoice_master_date between '" + start.ToString("yyyy-MM-dd") + "' and '" + end.ToString("yyyy-MM-dd") + "'";
            Odbc o = new Odbc(constr);
            DataTable dt = o.ReturnTable(sql, "InvIDs");
            foreach (DataRow dr in dt.Rows)
            {
                MakeReport(dr["invoice_master_id"].ToString());
            }
        }
        public DataTable MakeReport(string InvID)
        {
            try
            {
                string invoice_id = "";
                invoice_id = InvID;
                if (invoice_id != "")
                {
                    InvoiceMaster invoice = new InvoiceMaster(constr);
                    invoice.LoadData(Convert.ToInt32(invoice_id));
                    Sapp.SMS.System system = new Sapp.SMS.System(constr);
                    system.LoadData("FILEFOLDER");
                    string filefolder = system.SystemValue;
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(invoice.InvoiceMasterBodycorpId);
                    string bodycorp_code = bodycorp.BodycorpCode;
                    filefolder += bodycorp_code + "\\INVOICE\\EmailTemp\\";
                    if (!Directory.Exists(filefolder))
                    {
                        Directory.CreateDirectory(filefolder);
                    }
                    DirectoryInfo dirInfo = new DirectoryInfo(filefolder);
                    FileInfo[] fileInfos = dirInfo.GetFiles();
                    string template_dir = Server.MapPath("~/templates");
                    template_dir += "\\" + bodycorp.BodycorpInvTpl;
                    rpInvoice rp_invoice = new rpInvoice(constr, template_dir);
                    rp_invoice.SetReportInfo(invoice.InvoiceMasterId);
                    rp_invoice.ExportPDF(filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf");
                    DataRow dr = InvoiceDT.NewRow();
                    dr["FilePath"] = filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf";
                    dr["FileName"] = invoice.InvoiceMasterNum + ".pdf";
                    dr["Extension"] = "pdf";
                    InvoiceDT.Rows.Add(dr);

                }
                return InvoiceDT;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
                return null;
            }
        }

        public void MakeStatement(string bodycorp_id, DateTime start_date, DateTime end_date, string unit_id)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            ReportUnitStatement Report = new ReportUnitStatement(constr, Server.MapPath("templates/statement_template.rdlc"), new Microsoft.Reporting.WebForms.ReportViewer());
            Report.SetReportInfo(Convert.ToInt32(bodycorp_id), Convert.ToDateTime(start_date), Convert.ToDateTime(end_date), unit_id);
            Sapp.SMS.System system = new Sapp.SMS.System(constr);
            system.LoadData("FILEFOLDER");
            string filefolder = system.SystemValue;
            DataTable bDT = ReportDT.getTable(constr, "bodycorps");
            string bodycorp_code = ReportDT.GetDataByColumn(bDT, "bodycorp_id", bodycorp_id, "bodycorp_code");
            string StateNo = Report.invoiceTable.Rows[0]["invoice_number"].ToString();
            filefolder += bodycorp_code + "\\Statement\\EmailTemp\\";
            if (!Directory.Exists(filefolder))
            {
                Directory.CreateDirectory(filefolder);
            }
            Report.ExportPDF(filefolder + StateNo + ".pdf");
            DataRow dr = StatementDT.NewRow();
            dr["FilePath"] = filefolder + StateNo + ".pdf";
            dr["FileName"] = StateNo + ".pdf";
            dr["Extension"] = "pdf";
            StatementDT.Rows.Add(dr);
        }
        public DataTable AttachmentDT()
        {
            DataTable dt = InvoiceDT.Clone();
            foreach (DataRow dr in InvoiceDT.Rows)
            {
                DataRow newRow = dt.NewRow();
                newRow.ItemArray = dr.ItemArray;
                dt.Rows.Add(newRow);
            }
            foreach (DataRow dr in StatementDT.Rows)
            {
                DataRow newRow = dt.NewRow();
                newRow.ItemArray = dr.ItemArray;
                dt.Rows.Add(newRow);
            }
            if (NotesCheckBox.Checked)
                foreach (DataRow dr in UploadDT.Rows)
                {
                    DataRow newRow = dt.NewRow();
                    newRow.ItemArray = dr.ItemArray;
                    dt.Rows.Add(newRow);
                }
            return dt;
        }

        public void SendEmail(string title, string email, string content, string name, DataTable attachment)
        {
            try
            {
                DataTable sys = ReportDT.getTable(constr, "system");
                string from = ReportDT.GetDataByColumn(sys, "system_code", "EMAILFROM", "system_value");
                string strHost = ReportDT.GetDataByColumn(sys, "system_code", "EMAILSERVER", "system_value");  //STMP服务器地址
                string strAccount = ReportDT.GetDataByColumn(sys, "system_code", "EMAILUSER", "system_value");       //SMTP服务帐号
                string strPwd = ReportDT.GetDataByColumn(sys, "system_code", "EMAILPWD", "system_value");     //SMTP服务密码
                string PORT = ReportDT.GetDataByColumn(sys, "system_code", "EMAILPORT", "system_value");
                var fromAddress = new MailAddress(from, "Test");
                var toAddress = new MailAddress(email, name);
                string fromPassword = strPwd;
                string subject = title;
                string body = content;

                var smtp = new SmtpClient
                {
                    Host = strHost,
                    Port = int.Parse(PORT),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (
                    var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body
                    })
                {
                    if (attachment.Rows.Count > 0)
                        foreach (DataRow dr in attachment.Rows)
                        {
                            Attachment a = new Attachment(dr[0].ToString());
                            message.Attachments.Add(a);
                        }
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["SendEmail"] != null)
                {
                    emaildt = (DataTable)Session["SendEmail"];
                    if (!emaildt.Columns.Contains("Send"))
                    {
                        emaildt.Columns.Add("Send");
                    }
                }
                else
                {
                    Response.Redirect("~/invsend.aspx", false);
                }
                SetDT();
            }
        }


        protected void UploadB_Click(object sender, System.EventArgs e)
        {
            string path = HttpContext.Current.Server.MapPath("~/temp") + "\\" + DateTime.Now.ToString("ddMMyyyy") + "\\";
            DirectoryInfo d = new DirectoryInfo(path);
            if (!d.Exists)
            {
                d.Create();
            }
            FileUpload1.SaveAs(path + FileUpload1.FileName);
            DataRow dr = UploadDT.NewRow();
            dr["FilePath"] = path + FileUpload1.FileName;
            dr["FileName"] = FileUpload1.FileName;
            UploadDT.Rows.Add(dr);
            GridView1.DataSource = UploadDT;
            GridView1.DataBind();
        }

        protected void Button1_Click(object sender, System.EventArgs e)
        {
            //Thread t = new Thread(new ThreadStart(SendEmail));
            //t.Start();
            SendEmail();
            Response.Redirect("~/invsendresult.aspx", false);
        }

        protected void DeleteB_Click(object sender, System.EventArgs e)
        {
            Button b = (Button)sender;
            string filename = b.CommandName;
            ReportDT.GetDataRowByColumn(UploadDT, "FileName", filename).Delete();
            UploadDT.AcceptChanges();
            GridView1.DataSource = UploadDT;
            GridView1.DataBind();
        }
    }
}