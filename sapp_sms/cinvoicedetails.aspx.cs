using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using System.Configuration;
using System.Collections;
using Sapp.SMS;
using Sapp.JQuery;
using Sapp.Data;
using System.Text.RegularExpressions;

namespace sapp_sms
{
    public partial class cinvoicedetails : System.Web.UI.Page
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
                        string cinvoice_id = "";
                        if (Request.QueryString["cinvoiceid"] != null)
                        {
                            cinvoice_id = Request.QueryString["cinvoiceid"];
                        }
                        Cinvoice cinvoice = new Cinvoice(constr);
                        cinvoice.LoadData(Convert.ToInt32(cinvoice_id));

                        if (cinvoice.CinvoiceUnitId != null)
                        {
                            UnitMaster unitmaster = new UnitMaster(constr);
                            unitmaster.LoadData(Convert.ToInt32(cinvoice.CinvoiceUnitId));
                            LabelUnit.Text = unitmaster.UnitMasterCode;
                        }
                        Bodycorp bodycorp = new Bodycorp(constr);
                        bodycorp.LoadData(Convert.ToInt32(cinvoice.CinvoiceBodycorpId));
                        LabelBodycorp.Text = bodycorp.BodycorpCode;

                        CreditorMaster creditor = new CreditorMaster(constr);
                        creditor.LoadData(Convert.ToInt32(cinvoice.CinvoiceCreditorId));
                        LabelCreditor.Text = creditor.CreditorMasterCode;

                        if (cinvoice.CinvoiceOrderId.HasValue)
                        {
                            PurchorderMaster order = new PurchorderMaster(constr);
                            order.LoadData(Convert.ToInt32(cinvoice.CinvoiceOrderId));
                            LabelOrder.Text = order.PurchorderMasterNum;
                        }
                        else
                        {
                            LabelOrder.Text = "";
                            ImageButtonOrder.Enabled = false;
                        }

                        LabelDescription.Text = cinvoice.CinvoiceDescription;
                        LabelNum.Text = cinvoice.CinvoiceNum;
                        if (cinvoice.CinvoiceDue.HasValue)
                            LabelDue.Text = cinvoice.CinvoiceDue.Value.ToShortDateString();
                        if (cinvoice.CinvoiceApply.HasValue)
                            LabelApply.Text = cinvoice.CinvoiceApply.Value.ToShortDateString();
                        LabelPaid.Text = cinvoice.CinvoicePaid.ToString("0.00");
                        if (cinvoice.CinvoiceUnitAdminFee.HasValue)
                            LabelAdminFee.Text = cinvoice.CinvoiceUnitAdminFee.Value ? "Yes" : "No";
                        LabelDate.Text = cinvoice.CinvoiceDate.ToShortDateString();
                        LiteralClientID.Text = cinvoice.CinvoiceId.ToString();
                        LabelNum.Text = cinvoice.CinvoiceNum.ToString();
                        LabelNet.Text = cinvoice.CinvoiceNet.ToString("0.00");
                        LabelGross.Text = cinvoice.CinvoiceGross.ToString("0.00");
                        LabelTax.Text = cinvoice.CinvoiceTax.ToString("0.00");
                        DetailI.Attributes.Add("TID", cinvoice_id);
                        //string url = " window.showModalDialog(\"activityunitdetail.aspx?id=" + cinvoice_id + "&type=Cinvoice\", \"#1\", \"dialogHeight: 700px; dialogWidth: 850px; edge: Raised; center: Yes; help: No; resizable: No; status: No; scroll: No;\")";
                        //DetailHL.NavigateUrl = url;
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

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata, string cinvoiceid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Cinvoice cinvoice = new Cinvoice(constr);
                cinvoice.LoadData(Convert.ToInt32(cinvoiceid));
                string sqlfromstr = "FROM (`gl_transactions` left join `cinvoice_gls` on `gl_transaction_id`=`cinvoice_gl_gl_id` left join `chart_master` on `gl_transaction_chart_id`=`chart_master_id`) WHERE `cinvoice_gl_cinvoice_id`=" + cinvoiceid + " AND `gl_transaction_type_id`=2";
                string sqlselectstr = "SELECT `ID`,`Chart`,`Description`,`Net`,`Tax`,`Gross` FROM (SELECT `gl_transaction_id` AS `ID`, `chart_master_code` AS `Chart`,`gl_transaction_description` AS `Description`,-`gl_transaction_net` AS `Net`,-`gl_transaction_tax` AS `Tax`,-`gl_transaction_gross` AS `Gross` ";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
                Hashtable userdata = new Hashtable();
                userdata.Add("Description", "Total");
                userdata.Add("Net", cinvoice.CinvoiceNet.ToString("0.00"));
                userdata.Add("Tax", cinvoice.CinvoiceTax.ToString("0.00"));
                userdata.Add("Gross", cinvoice.CinvoiceGross.ToString("0.00"));
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
                string cinvoice_id = "";
                cinvoice_id = Request.QueryString["cinvoiceid"];
                Response.BufferOutput = true;
                Response.Redirect("~/cinvoiceedit.aspx?mode=edit&cinvoiceid=" + cinvoice_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        // Add 15/04/2016
        protected void ImageButtonCopyAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string cinvoicemaster_id = Request.QueryString["cinvoiceid"];

                if (cinvoicemaster_id != null && cinvoicemaster_id.Equals(string.Empty) == false)
                {
                    Response.BufferOutput = true;
                    Response.Redirect("~/cinvoiceedit.aspx?mode=paste&cinvoiceid=" + cinvoicemaster_id, false);
                }
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
                string cinvoice_id = Request.QueryString["cinvoiceid"]; ;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Cinvoice cinvoice = new Cinvoice(constr);
                cinvoice.Delete(Convert.ToInt32(cinvoice_id));
                Response.BufferOutput = true;
                Response.Redirect("goback.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
        }

        protected void ImageButtonOrder_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string cinvoice_id = "";
                cinvoice_id = Request.QueryString["cinvoiceid"];
                Cinvoice cinvoice = new Cinvoice(constr);
                cinvoice.LoadData(Convert.ToInt32(cinvoice_id));
                if (cinvoice.CinvoiceOrderId.HasValue)
                    Response.Redirect("purchordermasterdetails.aspx?purchordermasterid=" + cinvoice.CinvoiceOrderId,false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion


    }
}