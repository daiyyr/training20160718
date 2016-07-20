using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using System.Configuration;
using Sapp.JQuery;
using Sapp.SMS;
using System.Text.RegularExpressions;
using System.Data;

namespace sapp_sms
{
    public partial class cinvoices : System.Web.UI.Page, IPostBackEventHandler
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
                #region Javascript setup
                Control[] wc = { jqGridTable };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                Session["SelectedCinvoicePaidType"] = ComboBoxPaid.SelectedValue;
                string qscreditorid = CheckedQueryString();
                Session["creditorid"] = qscreditorid;

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
                if (args[0] == "ImageButtonDelete")
                {
                    ImageButtonDelete_Click(args);
                }
                else if (args[0] == "ImageButtonCopyAdd")
                {
                    ImageButtonCopyAdd_Click(args);
                }
                else if (args[0] == "ImageButtonDetails")
                {
                    ImageButtonDetails_Click(args);
                }
                else if (args[0] == "ImageButtonEdit")
                {
                    ImageButtonEdit_Click(args);
                }
                else if (args[0] == "ImageButtonCPayment")
                {
                    ImageButtonPayment_ClientClick(args);

                }
                else if (args[0] == "ImageButtonAllocate")
                {
                    Response.Redirect("cinvoiceallocate.aspx?id=" + args[1], false);

                }


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
                string sqlfromstr = "FROM (`Cinvoices` left join `purchorder_master`  on `cinvoice_order_id`=`purchorder_master_id` left join `creditor_master`  on `cinvoice_creditor_id`=`creditor_master_id` left join `bodycorps`  on `cinvoice_bodycorp_id`=`bodycorp_id` left join `unit_master`  on `cinvoice_unit_id`=`unit_master_id`) where cinvoice_type_id=1 and bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value;
                if (null != HttpContext.Current.Session["SelectedCinvoicePaidType"])
                {
                    string selectedValue = HttpContext.Current.Session["SelectedCinvoicePaidType"].ToString();
                    if (selectedValue == "1" || selectedValue == "0")
                    {
                        if (selectedValue == "1")
                            sqlfromstr += " and cinvoice_gross<=cinvoice_paid";
                        else
                            sqlfromstr += " and cinvoice_gross>cinvoice_paid";
                    }
                }
                if (null != HttpContext.Current.Session["creditorid"] && !String.IsNullOrEmpty(HttpContext.Current.Session["creditorid"].ToString()))
                {
                    string sql = " where ";
                    if (sqlfromstr.Contains("where")) sql = " and ";
                    sql += " cinvoice_creditor_id=" + HttpContext.Current.Session["creditorid"].ToString();
                    sqlfromstr += sql;
                    HttpContext.Current.Session["creditorid"] = "";
                }
                string sqlselectstr = "SELECT `ID`,`Num`,`Order`,`Creditor`,`Bodycorp`,`Unit`,`Date`,`Due`,`Gross`,`Paid`,`Balance` from (select  `cinvoice_id` as `ID`,`cinvoice_num` as `Num`,`purchorder_master_num` as `Order`, `creditor_master_code` AS `Creditor`, `bodycorp_code` AS `Bodycorp`,`unit_master_code` as `Unit`,`cinvoice_date` as `Date`,`cinvoice_due` as `Due`,`cinvoice_gross` AS `Gross`,`cinvoice_paid` AS `Paid`, ABS(`cinvoice_gross`)-`cinvoice_paid` AS `Balance`";

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

        #region WebControl Events

        private void ImageButtonDelete_Click(string[] args)
        {
            string cinvoice_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                cinvoice_id = args[1];


                Cinvoice cinvoice = new Cinvoice(constr);
                cinvoice.LoadData(int.Parse(cinvoice_id));
                Bodycorp b = new Bodycorp(AdFunction.conn);
                b.LoadData(cinvoice.CinvoiceBodycorpId);
                if (!b.CheckCloseOff(cinvoice.CinvoiceDate))
                {
                    throw new Exception("CInvoice before close date");
                }
                cinvoice.Delete(Convert.ToInt32(cinvoice_id));
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/cinvoiceedit.aspx?mode=add" + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        // Add 15/04/2016
        protected void ImageButtonCopyAdd_Click(string[] args)
        {
            try
            {
                string cinvoicemaster_id = args[1];

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

        private void ImageButtonEdit_Click(string[] args)
        {
            string cinvoice_id = "";
            try
            {
                cinvoice_id = args[1];


                Cinvoice cinvoice = new Cinvoice(AdFunction.conn);
                cinvoice.LoadData(int.Parse(cinvoice_id));
                Bodycorp b = new Bodycorp(AdFunction.conn);
                b.LoadData(cinvoice.CinvoiceBodycorpId);
                if (!b.CheckCloseOff(cinvoice.CinvoiceDate))
                {
                    throw new Exception("CInvoice before close date");
                }
                Response.BufferOutput = true;
                Response.Redirect("~/cinvoiceedit.aspx?mode=edit&cinvoiceid=" + cinvoice_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonPayment_ClientClick(string[] args)
        {
            string cinvoice_id = "";
            try
            {
                cinvoice_id = args[1];
                Cinvoice cinvoice = new Cinvoice(AdFunction.conn);
                cinvoice.LoadData(int.Parse(cinvoice_id));
                Bodycorp b = new Bodycorp(AdFunction.conn);
                b.LoadData(cinvoice.CinvoiceBodycorpId);
                if (!b.CheckCloseOff(cinvoice.CinvoiceDate))
                {
                    throw new Exception("CInvoice before close date");
                }
                Response.BufferOutput = true;
                Response.Redirect("~/cpaymentedit.aspx?mode=add&cinvoiceid=" + cinvoice_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonDetails_Click(string[] args)
        {
            string cinvoice_id = "";
            try
            {
                cinvoice_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/cinvoicedetails.aspx?cinvoiceid=" + cinvoice_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ComboBoxPaid_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/cinvoicebatch.aspx", false);
        }

        protected void ImageButtonImport_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/cinvoiceupload.aspx", false);
        }


    }
}