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
namespace sapp_sms
{
    public partial class invoiceoverwrite : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["EXInvID"] = Request.QueryString["invoicemasterid"];
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string invoice_id = "";
                if (Request.QueryString["invoicemasterid"] != null) invoice_id = Request.QueryString["invoicemasterid"];
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
                    filefolder += bodycorp_code + "\\INVOICE";
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
                    Response.Redirect("invoicemasterdetails.aspx?invoicemasterid=" + invoice_id,false);
                }

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {

            string id = Request.QueryString["invoicemasterid"];

            Response.Redirect("invoicemasterdetails.aspx?invoicemasterid=" + id,false);
        }


    }
}