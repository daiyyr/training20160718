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

namespace sapp_sms
{
    public partial class creditnotedetails : System.Web.UI.Page, IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { jqGridTrans, LabelNum, LabelElementID };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    #region Load webForm
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        string invoice_id = "";
                        if (Request.QueryString["invoicemasterid"] != null)
                        {
                            invoice_id = Request.QueryString["invoicemasterid"];
                            Session["invoicemasterid"] = invoice_id;
                        }
                        InvoiceMaster invoice = new InvoiceMaster(constr);
                        invoice.LoadData(Convert.ToInt32(invoice_id));

                        if (invoice.InvoiceMasterUnitId != null)
                        {
                            UnitMaster unitmaster = new UnitMaster(constr);
                            unitmaster.LoadData(Convert.ToInt32(invoice.InvoiceMasterUnitId));
                            LabelUnit.Text = unitmaster.UnitMasterCode;
                        }
                        if (invoice.InvoiceMasterBodycorpId != null)
                        {
                            Bodycorp bodycorp = new Bodycorp(constr);
                            bodycorp.LoadData(Convert.ToInt32(invoice.InvoiceMasterBodycorpId));
                            LabelBodycorp.Text = bodycorp.BodycorpCode;
                        }

                        DebtorMaster debtor = new DebtorMaster(constr);
                        debtor.LoadData(Convert.ToInt32(invoice.InvoiceMasterDebtorId));
                        LabelDebtor.Text = debtor.DebtorMasterName; ;

                        InvoiceType type = new InvoiceType(constr);
                        type.LoadData(Convert.ToInt32(invoice.InvoiceMasterTypeId));

                        LabelDescription.Text = invoice.InvoiceMasterDescription;
                        LabelNum.Text = invoice.InvoiceMasterNum;
                        //if (invoice.InvoiceMasterDue.HasValue)
                        //    LabelDue.Text = invoice.InvoiceMasterDue.Value.ToShortDateString();
                        LabelDate.Text = invoice.InvoiceMasterDate.ToShortDateString();
                        LabelElementID.Text = invoice.InvoiceMasterId.ToString();
                        LabelGross.Text = invoice.InvoiceMasterGross.ToString();
                        LabelNet.Text = invoice.InvoiceMasterNet.ToString();
                        LabelTax.Text = invoice.InvoiceMasterTax.ToString();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            try
            {
                string[] args = eventArgument.Split('|');
                if (args[0] == "ButtonOK")
                {
                    ButtonOK_Click(args);
                }
                else
                    throw new Exception("Error: unknown command!");
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata, string invoicemasterid)
        {
            try
            {

                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                InvoiceMaster invoice = new InvoiceMaster(constr);
                invoice.LoadData(Convert.ToInt32(invoicemasterid));
                string sqlfromstr = "FROM (`gl_transactions` left join `invoice_gls` on `gl_transaction_id`=`invoice_gl_gl_id` left join `chart_master` on `gl_transaction_chart_id`=`chart_master_id`) "
                    + "WHERE `invoice_gl_invoice_id`=" + HttpContext.Current.Session["invoicemasterid"].ToString() + " AND `gl_transaction_type_id`=1";
                string sqlselectstr = "SELECT `ID`,`Chart`,`Description`, `Net`,`Tax`,`Gross` FROM (SELECT `gl_transaction_id` AS `ID`, `chart_master_code` AS `Chart`, -1*`gl_transaction_net` AS `Net`,-1*`gl_transaction_tax` AS `Tax`,-1*`gl_transaction_gross` AS `Gross`,`gl_transaction_description` AS `Description` ";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
                Hashtable userdata = new Hashtable();
                userdata.Add("Description", "Total");
                userdata.Add("Net", (-invoice.InvoiceMasterNet).ToString("0.00"));
                userdata.Add("Tax", (-invoice.InvoiceMasterTax).ToString("0.00"));
                userdata.Add("Gross", (-invoice.InvoiceMasterGross).ToString("0.00") + "/Paid " + (-invoice.InvoiceMasterPaid).ToString("0.00"));
                jqgridObj.SetUserData(userdata);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return "{}";
        }
        #endregion

        #region WebControl Events


        protected void ImageButtonEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string invoice_id = "";
                if (Request.QueryString["invoicemasterid"] != null) invoice_id = Request.QueryString["invoicemasterid"];
                Response.BufferOutput = true;
                Response.Redirect("~/creditnoteedit.aspx?mode=edit&invoicemasterid=" + invoice_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //string invoice_id = Request.QueryString["invoicemasterid"];
                //string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                //InvoiceMaster invoice = new InvoiceMaster(constr);
                //invoice.Delete(true, Convert.ToInt32(invoice_id));
                //Response.BufferOutput = true;
                //Response.Redirect("goback.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/creditnotemaster.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ButtonOK_Click(string[] args)
        {


        }

        protected void ImageButtonExport_Click(object sender, ImageClickEventArgs e)
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
                            return;
                        }
                    }

                    string template_dir = Server.MapPath("~/templates");

                    template_dir += "\\" + bodycorp.BodycorpInvTpl;
                    rpInvoice rp_invoice = new rpInvoice(constr, template_dir);
                    rp_invoice.SetReportInfo(invoice.InvoiceMasterId);
                    rp_invoice.ExportPDF(filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf");
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
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion


    }
}