using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using System.Configuration;
using Sapp.SMS;
using Sapp.Data;
using Sapp.JQuery;
using System.Text.RegularExpressions;

namespace sapp_sms
{
    public partial class purchordermasterdetails : System.Web.UI.Page
    {
        private string CheckedQueryString()
        {
            if (null != Request.QueryString["creditorid"] && Regex.IsMatch(Request.QueryString["creditorid"], "^[0-9]*$"))
            {
                return Request.QueryString["creditorid"];
            }
            return "";
        }
        private string NewQueryString(string connector)
        {
            string result = CheckedQueryString();
            return string.IsNullOrEmpty(result) ? "" : connector + "creditorid=" + result;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { jqGridTrans};
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                Session["purchordermasterid"] = Request.QueryString["purchordermasterid"];
                #endregion
                if (!IsPostBack)
                {
                    #region Load webForm
                    Odbc mydb = null;
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        string purchordermaster_id = "";
                        if (Request.QueryString["purchordermasterid"] != null) purchordermaster_id = Request.QueryString["purchordermasterid"];
                        PurchorderMaster purchordermaster = new PurchorderMaster(constr);
                        purchordermaster.LoadData(Convert.ToInt32(purchordermaster_id));

                        if (purchordermaster.PurchorderMasterUnitId != null)
                        {
                            UnitMaster unitmaster = new UnitMaster(constr);
                            unitmaster.LoadData(Convert.ToInt32(purchordermaster.PurchorderMasterUnitId));
                            LabelUnit.Text = unitmaster.UnitMasterCode;
                        }
                        //if (purchordermaster.PurchorderMasterBodycorpId != null)
                        //{
                            Bodycorp bodycorp = new Bodycorp(constr);
                            bodycorp.LoadData(Convert.ToInt32(purchordermaster.PurchorderMasterBodycorpId));
                            LabelBodycorp.Text = bodycorp.BodycorpCode;
                        //}
                        PurchorderType ptype = new PurchorderType(constr);
                        ptype.LoadData(Convert.ToInt32(purchordermaster.PurchorderMasterTypeId));
                        LabelType.Text = ptype.PurchorderTypeCode;

                        CreditorMaster creditor = new CreditorMaster(constr);
                        creditor.LoadData(Convert.ToInt32(purchordermaster.PurchorderMasterCreditorId));
                        LabelCreditor.Text = creditor.CreditorMasterCode;

                        LabelAllocated.Text = purchordermaster.PurchorderMasterAllocated?"Yes":"No";
                        LabelApproval.Text = purchordermaster.PurchorderMasterApproval.ToString();
                        LabelDate.Text = purchordermaster.PurchorderMasterDate.ToShortDateString();
                        LabelElementID.Text = purchordermaster.PurchorderMasterId.ToString();
                        LabelGross.Text = purchordermaster.PurchorderMasterGross.ToString();
                        LabelNet.Text = purchordermaster.PurchodrerMasterNet.ToString();
                        LabelNum.Text = purchordermaster.PurchorderMasterNum.ToString();
                        LabelTax.Text = purchordermaster.PurchorderMasterTax.ToString();
                        LabelNote.Text = purchordermaster.PurchorderMasterDescription;
                        if (purchordermaster.PurchorderMasterAllocated)
                            ImageButtonInvoice.Enabled = true;
                        else
                            ImageButtonInvoice.Enabled = false;
                        #region Calculate Net Tax Gross
                        purchordermaster.LoadTransactions();
                        decimal sumNet = 0;
                        foreach (PurchorderTrans ptrans in purchordermaster.PurchorderTranList)
                        {
                            sumNet += ptrans.PurchorderTranNet;
                        }
                        LabelNet.Text = sumNet.ToString();
                        LabelTax.Text = ((Decimal)((double)sumNet * 0.15)).ToString();
                        LabelGross.Text = ((Decimal)((double)sumNet * 1.15)).ToString();
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (mydb != null) mydb.Close();
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
                string purchordermaster_id = "";
                purchordermaster_id = Request.QueryString["purchordermasterid"];
                Response.BufferOutput = true;
                Response.Redirect("~/purchordermasteredit.aspx?purchordermasterid=" + Server.UrlEncode(purchordermaster_id) + "&mode=edit" + NewQueryString("&"),false);
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
                string purchordermaster_id = "";
                if (Request.QueryString["purchordermasterid"] != null) purchordermaster_id = Request.QueryString["purchordermasterid"];
                if (purchordermaster_id != "")
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    PurchorderMaster purchordermaster = new PurchorderMaster(constr);
                    purchordermaster.LoadData(Convert.ToInt32(purchordermaster_id));
                    purchordermaster.Delete();
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string sqlfromstr = "FROM (`purchorder_trans` left join `chart_master` on `chart_master_id`=`purchorder_tran_chart_id`) WHERE `purchorder_tran_purchorder_id`=" + HttpContext.Current.Session["purchordermasterid"].ToString();
                string sqlselectstr = "SELECT `ID`,`Chart`,`Description`,`Net`,`Tax`,`Gross` FROM (SELECT `purchorder_tran_id` AS `ID`, `chart_master_code` AS `Chart`,`purchorder_tran_description` AS `Description` ,`purchorder_tran_net` AS `Net`,`purchorder_tran_tax` AS `Tax`,`purchorder_tran_gross` AS `Gross`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
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

        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/purchordermaster.aspx" + NewQueryString("?"),false);
        }

        protected void ImageButtonInvoice_Click(object sender, ImageClickEventArgs e)
        {
            //string purchordermaster_id = "";
            //if (Request.QueryString["purchordermasterid"] != null) purchordermaster_id = Request.QueryString["purchordermasterid"];
            //if (purchordermaster_id != "")
            //{
            //    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            //    Cinvoice cinv = new Cinvoice(constr);
            //    cinv.LoadDataByOrderID(Convert.ToInt32(purchordermaster_id));
            //    Response.Redirect("cinvoicedetails.aspx?cinvoiceid=" + cinv.CinvoiceId.ToString() + NewQueryString("&"));
            //}
        }
    }
}