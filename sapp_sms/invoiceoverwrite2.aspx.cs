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
    public partial class invoiceoverwrite2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string invoice_id = Session["EXInvID"].ToString();
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

                    foreach (FileInfo fi in fileInfos)
                    {
                        if (fi.Name == (invoice.InvoiceMasterNum + ".pdf"))
                        {
                            HttpContext.Current.Response.Clear();
                            HttpContext.Current.Response.ClearHeaders();
                            HttpContext.Current.Response.ClearContent();
                            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fi.Name);
                            HttpContext.Current.Response.AddHeader("Content-Length", fi.Length.ToString());
                            HttpContext.Current.Response.ContentType = "text/plain";
                            HttpContext.Current.Response.Flush();
                            HttpContext.Current.Response.TransmitFile(fi.FullName);
                            HttpContext.Current.Response.End();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }



    }
}