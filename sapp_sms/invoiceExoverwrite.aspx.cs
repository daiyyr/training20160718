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
using System.IO;
using System.Threading;
namespace sapp_sms
{
    public partial class invoiceExoverwrite : System.Web.UI.Page
    {
        public static bool over = false;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["ExInvDT"] == null)
            {
                Response.Redirect("invoicemaster.aspx", false);

            }
            t1.Visible = false;
            Export(Request.QueryString["invoicemasterid"].ToString());
        }
        public void Export(string invoice_id)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;


                InvoiceMaster invoice = new InvoiceMaster(constr);
                invoice.LoadData(Convert.ToInt32(invoice_id));
                Sapp.SMS.System system = new Sapp.SMS.System(constr);
                system.LoadData("FILEFOLDER");
                string filefolder = system.SystemValue;
                Bodycorp bodycorp = new Bodycorp(constr);
                bodycorp.LoadData(invoice.InvoiceMasterBodycorpId);
                string bodycorp_code = bodycorp.BodycorpCode;
                filefolder += bodycorp_code + "\\INVOICE";
                if (!Directory.Exists(filefolder))
                {
                    Directory.CreateDirectory(filefolder);
                }
                FileInfo f = new FileInfo(filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf");

                DirectoryInfo dirInfo = new DirectoryInfo(filefolder);
                FileInfo[] fileInfos = dirInfo.GetFiles();
                string template_dir = Server.MapPath("~/templates");
                template_dir += "\\" + bodycorp.BodycorpInvTpl;
                rpInvoice rp_invoice = new rpInvoice(constr, template_dir);
                rp_invoice.SetReportInfo(invoice.InvoiceMasterId);
                rp_invoice.ExportPDF(filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf");
                rp_invoice.ExportPDF(Server.MapPath("~/") + "temp\\" + invoice.InvoiceMasterNum + ".pdf");
                Response.Redirect("~/temp/" + invoice.InvoiceMasterNum + ".pdf", false);





            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        //public void Export()
        //{
        //    try
        //    {
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        string invoice_id = "";
        //        DataTable dt = (DataTable)Session["ExInvDT"];
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            invoice_id = dr["ID"].ToString();
        //            if (invoice_id != "")
        //            {
        //                InvoiceMaster invoice = new InvoiceMaster(constr);
        //                invoice.LoadData(Convert.ToInt32(invoice_id));
        //                Sapp.SMS.System system = new Sapp.SMS.System(constr);
        //                system.LoadData("FILEFOLDER");
        //                string filefolder = system.SystemValue;
        //                Bodycorp bodycorp = new Bodycorp(constr);
        //                bodycorp.LoadData(invoice.InvoiceMasterBodycorpId);
        //                string bodycorp_code = bodycorp.BodycorpCode;
        //                filefolder += bodycorp_code + "\\INVOICE";
        //                if (!Directory.Exists(filefolder))
        //                {
        //                    Directory.CreateDirectory(filefolder);
        //                }
        //                FileInfo f = new FileInfo(filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf");
        //                if (over)
        //                {
        //                    DirectoryInfo dirInfo = new DirectoryInfo(filefolder);
        //                    FileInfo[] fileInfos = dirInfo.GetFiles();
        //                    string template_dir = Server.MapPath("~/templates");
        //                    template_dir += "\\" + bodycorp.BodycorpInvTpl;
        //                    rpInvoice rp_invoice = new rpInvoice(constr, template_dir);
        //                    rp_invoice.SetReportInfo(invoice.InvoiceMasterId);
        //                    rp_invoice.ExportPDF(filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf");
        //                    rp_invoice.ExportPDF(Server.MapPath("~/") + "temp\\" + invoice.InvoiceMasterNum + ".pdf");
        //                }
        //                else
        //                {
        //                    if (!f.Exists)
        //                    {
        //                        DirectoryInfo dirInfo = new DirectoryInfo(filefolder);
        //                        FileInfo[] fileInfos = dirInfo.GetFiles();
        //                        string template_dir = Server.MapPath("~/templates");
        //                        template_dir += "\\" + bodycorp.BodycorpInvTpl;
        //                        rpInvoice rp_invoice = new rpInvoice(constr, template_dir);
        //                        rp_invoice.SetReportInfo(invoice.InvoiceMasterId);
        //                        rp_invoice.ExportPDF(filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf");
        //                        rp_invoice.ExportPDF(Server.MapPath("~/") + "temp\\" + invoice.InvoiceMasterNum + ".pdf");
        //                    }
        //                }
        //                Response.Redirect("~/temp/" + invoice.InvoiceMasterNum + ".pdf", false);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.Current.Session["ErrorUrl"] = Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
        //    }
        //}
        protected void Button1_Click(object sender, EventArgs e)
        {
            over = true;
            //Thread t = new Thread(new ThreadStart(Export));
            //t.Start();
            //Export();

            //Response.Redirect("invoicemaster.aspx", false);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            //    over = false;
            //    Export();
            //Thread t = new Thread(new ThreadStart(Export));
            //t.Start();
            //Response.Redirect("invoicemaster.aspx", false);
        }


    }
}