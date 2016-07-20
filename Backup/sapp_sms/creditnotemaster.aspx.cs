using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.JQuery;
using System.Configuration;
using Sapp.Common;
using Sapp.SMS;
using System.Text.RegularExpressions;
using System.Data;
using Sapp.Data;
using System.Collections;
namespace sapp_sms
{
    public partial class creditnotemaster : System.Web.UI.Page, IPostBackEventHandler
    {
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridTable };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                if (Request.QueryString["levyid"] != null)
                {
                    Session["invoicemaster_levyid"] = Request.QueryString["levyid"].ToString();
                }
                #endregion
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
                else if (args[0] == "ImageButtonAllocate")
                {
                    string id = args[1];
                    Response.Redirect("~/creditnoteallocate.aspx?id=" + id, false);
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

                string where = "";
                if (HttpContext.Current.Session["invoicemaster_levyid"] != null)
                {
                    where += " AND `invoice_master_batch_id`=" + HttpContext.Current.Session["invoicemaster_levyid"].ToString();
                }
                string sqlselectstr = "SELECT `ID`,`Num`,`Debtor`,`Bodycorp`,`Unit`, `Date`, `Due`, `Gross`, `Paid`, `Balance` FROM (SELECT *";

                string sql = "SELECT `ID`,`Num`,`Debtor`,`Bodycorp`,`Unit`, `Date`, `Due`, `Gross`, `Paid` FROM (SELECT  `invoice_master_id` as `ID`,`invoice_master_num` as `Num`,`debtor_master_name` AS `Debtor`,`bodycorp_code` AS `Bodycorp`,`unit_master_code` AS `Unit`, `invoice_master_date` AS `Date`, `invoice_master_due` AS `Due`, -1*`invoice_master_gross` AS `Gross`, `invoice_master_paid` AS `Paid`  FROM ((((`invoice_master` LEFT JOIN `invoice_types`  ON `invoice_master_type_id`=`invoice_type_id`) LEFT JOIN `debtor_master` ON `debtor_master_id`=`invoice_master_debtor_id`) LEFT JOIN `bodycorps` ON `invoice_master_bodycorp_id`=`bodycorp_id`) LEFT JOIN `unit_master` ON `unit_master_id`=`invoice_master_unit_id`) WHERE `invoice_master_type_id`=2 and bodycorp_id= " + HttpContext.Current.Request.Cookies["bodycorpid"].Value + " " + where + ") as T1";
                Odbc o = new Odbc(constr);
                DataTable dt = o.ReturnTable(sql, "NewTable");

                dt.Columns.Add("Balance");
                foreach (DataRow dr in dt.Rows)
                {
                    dr["Balance"] = decimal.Parse(dr["Gross"].ToString()) - decimal.Parse(dr["Paid"].ToString());
                }

                string sqlfromstr = "FROM `" + dt.TableName + "`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
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
            string invoicemaster_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                invoicemaster_id = args[1];
                InvoiceMaster invoicemaster = new InvoiceMaster(constr);
                invoicemaster.LoadData(Convert.ToInt32(invoicemaster_id));
                invoicemaster.Delete();
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
                Response.Redirect("~/creditnoteedit.aspx?mode=add", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            string invoicemaster_id = "";
            try
            {
                invoicemaster_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/creditnoteedit.aspx?mode=edit&invoicemasterid=" + invoicemaster_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonDetails_Click(string[] args)
        {
            string invoicemaster_id = "";
            try
            {
                invoicemaster_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/creditnotedetails.aspx?invoicemasterid=" + invoicemaster_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void ImageButtonAllocate_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("creditnoteallocate.aspx");
        }


    }
}