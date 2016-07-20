using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.JQuery;
using Sapp.SMS;
using Sapp.Data;
using System.Collections;
using Sapp.Common;

namespace sapp_sms
{
    public partial class creditornotedetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { jqGridTrans };
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
                        if (invoice.InvoiceMasterDue.HasValue)
                            LabelDue.Text = invoice.InvoiceMasterDue.Value.ToShortDateString();
                        LabelBatchID.Text = invoice.InvoiceMasterBatchId.ToString();
                        LabelPaid.Text = invoice.InvoiceMasterPaid.ToString();
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

        #region WebControl Events
        protected void ImageButtonEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string cpayment_id = "";
                cpayment_id = Request.QueryString["invoicemasterid"];
                Response.BufferOutput = true;
                Response.Redirect("~/creditornotesedit.aspx?invoicemasterid=" + Server.UrlEncode(cpayment_id) + "&mode=edit",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string sqlfromstr = "FROM( `gl_transactions` left join `invoice_gls` on `gl_transaction_id`=`invoice_gl_gl_id` left join `chart_master` on `chart_master_id`=`gl_transaction_chart_id`) where `gl_transaction_gross`>`invoice_gl_paid` or `invoice_gl_paid` is null";
                string sqlselectstr = "SELECT `ID`,`Chart`,`Description`,`Net`,`Tax`,`Gross` FROM (SELECT `gl_transaction_id` AS `ID`, `Chart_master_code` AS `Chart`, `gl_transaction_description` AS `Description`,`gl_transaction_net` AS `Net`,`gl_transaction_tax` AS `Tax`,`gl_transaction_gross` AS `Gross`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        #endregion
        protected void ImageButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            string invoicemaster_id = "";
            try
            {
                //string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                //invoicemaster_id = Request.QueryString["invoicemasterid"];
                //InvoiceMaster invoicemaster = new InvoiceMaster(constr);
                //invoicemaster.Delete(true, Convert.ToInt32(invoicemaster_id));
                //Response.BufferOutput = true;
                //Response.Redirect("goback.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/creditornotes.aspx",false);
        }
    }
}