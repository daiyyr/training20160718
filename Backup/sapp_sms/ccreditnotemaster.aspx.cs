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
using Sapp.Data;
namespace sapp_sms
{
    public partial class ccreditnotemaster : System.Web.UI.Page, IPostBackEventHandler
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
            Odbc mydb = new Odbc(AdFunction.conn);
            try
            {

                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string sql = "SELECT `ID`,`Num`,`Creditor`,`Bodycorp`,`Date`,`Gross`,`Paid`,`Balance` from (select  `cinvoice_id` as `ID`,`cinvoice_num` as `Num`, `creditor_master_code` AS `Creditor`, `bodycorp_code` AS `Bodycorp`,DATE_FORMAT(`cinvoice_date`, '%d/%m/%Y') as `Date`,`cinvoice_gross` AS `Gross`,`cinvoice_paid` AS `Paid`, `cinvoice_gross`-`cinvoice_paid` AS `Balance` FROM (`Cinvoices` left join `purchorder_master`  on `cinvoice_order_id`=`purchorder_master_id` left join `creditor_master`  on `cinvoice_creditor_id`=`creditor_master_id` left join `bodycorps`  on `cinvoice_bodycorp_id`=`bodycorp_id` left join `unit_master`  on `cinvoice_unit_id`=`unit_master_id`) where `cinvoice_type_id` =2 and bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value + ") AS t1";
                DataTable dt = mydb.ReturnTable(sql, "temp");

                string sqlfromstr = "FROM `" + dt.TableName + "`";
           
                string sqlselectstr = "SELECT `ID`,`Num`,`Creditor`,`Bodycorp`,`Date`,-`Gross`,`Paid`,-`Balance` from (SELECT * ";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
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
                Response.Redirect("~/ccinvoiceedit.aspx?mode=add" + NewQueryString("&"), false);
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
                Response.Redirect("~/ccinvoiceedit.aspx?mode=edit&cinvoiceid=" + cinvoice_id + NewQueryString("&"), false);
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
                Response.Redirect("~/ccinvoicedetails.aspx?cinvoiceid=" + cinvoice_id + NewQueryString("&"), false);
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


    }
}