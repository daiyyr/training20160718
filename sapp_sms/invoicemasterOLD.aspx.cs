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
namespace sapp_sms
{
    public partial class invoicemasterOLD : System.Web.UI.Page, IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Session["InvoiceSelectedType"] = ComboBoxAllocated.SelectedValue;
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
                else if (args[0] == "Batch")
                {
                    Batch(args);
                }
                else if (args[0] == "ImageButtonAllocate")
                {
                    string id = args[1];
                    Response.Redirect("~/invoiceallocate.aspx?id=" + id, false);
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

                //string where = "";

                //{
                //    sqlfromstr = "FROM ((((`invoice_master` LEFT JOIN `invoice_types`  ON `invoice_master_type_id`=`invoice_type_id`)"
                //        + " LEFT JOIN `debtor_master` ON `debtor_master_id`=`invoice_master_debtor_id`) LEFT JOIN `bodycorps` ON `invoice_master_bodycorp_id`=`bodycorp_id`)"
                //        + " LEFT JOIN `unit_master` ON `unit_master_id`=`invoice_master_unit_id`) WHERE `invoice_master_type_id`=1 AND `invoice_master_batch_id`="
                //        + HttpContext.Current.Session["invoicemaster_levyid"].ToString();
                //}
                //else
                //    sqlfromstr = "FROM ((((`invoice_master` LEFT JOIN `invoice_types`  ON `invoice_master_type_id`=`invoice_type_id`) LEFT JOIN `debtor_master` ON `debtor_master_id`=`invoice_master_debtor_id`) LEFT JOIN `bodycorps` ON `invoice_master_bodycorp_id`=`bodycorp_id`) LEFT JOIN `unit_master` ON `unit_master_id`=`invoice_master_unit_id`) WHERE `invoice_master_type_id`=1 ";
                string sqlselectstr = "SELECT `ID`,`Num`,`Debtor`,`Bodycorp`,`Unit`, `Date`, `Due`, `Gross`, `Paid`,`Balance` FROM (SELECT *";

                string sql = "SELECT `ID`,`Num`,`Debtor`,`Bodycorp`,`Unit`, `Date`, `Due`, `Gross`, `Paid`,invoice_master_batch_id  FROM (SELECT `invoice_master_batch_id`, `invoice_master_id` as `ID`,`invoice_master_num` as `Num`,`debtor_master_name` AS `Debtor`,`bodycorp_code` AS `Bodycorp`,`unit_master_code` AS `Unit`, `invoice_master_date` AS `Date`, `invoice_master_due` AS `Due`, `invoice_master_gross` AS `Gross`, `invoice_master_paid` AS `Paid`  FROM ((((`invoice_master` LEFT JOIN `invoice_types`  ON `invoice_master_type_id`=`invoice_type_id`) LEFT JOIN `debtor_master` ON `debtor_master_id`=`invoice_master_debtor_id`) LEFT JOIN `bodycorps` ON `invoice_master_bodycorp_id`=`bodycorp_id`) LEFT JOIN `unit_master` ON `unit_master_id`=`invoice_master_unit_id`) WHERE `invoice_master_type_id`=1 and bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value + " ) as T1 order by `Date`";
                Odbc o = new Odbc(constr);
                DataTable dt = o.ReturnTable(sql, "NewTable");
                if (HttpContext.Current.Session["invoicemaster_levyid"] != null)
                {
                    string lid = HttpContext.Current.Session["invoicemaster_levyid"].ToString();
                    dt = ReportDT.FilterDT(dt, "invoice_master_batch_id=" + lid);
                    //where += " AND `invoice_master_batch_id`=" + HttpContext.Current.Session["invoicemaster_levyid"].ToString();
                }
                if (null != HttpContext.Current.Session["InvoiceSelectedType"])
                {
                    string selectedValue = HttpContext.Current.Session["InvoiceSelectedType"].ToString();
                    if (selectedValue.Equals("0") || selectedValue.Equals("1"))
                    {
                        string s = "";
                        if (selectedValue.Equals("0"))
                            s = "<";
                        else
                            s = ">=";
                        dt.DefaultView.RowFilter = " Gross " + s + " Paid ";
                        dt = dt.DefaultView.ToTable();
                    }
                }
                dt.Columns.Add("Balance");
                foreach (DataRow dr in dt.Rows)
                {
                    if (!dr["Gross"].ToString().Equals(""))
                    {
                        dr["Balance"] = System.Math.Abs(decimal.Parse(dr["Gross"].ToString())) - decimal.Parse(dr["Paid"].ToString());
                    }
                }

                string sqlfromstr = "FROM `" + dt.TableName + "`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                string jsonStr = jqgridObj.GetJSONStr();
                HttpContext.Current.Session["ExInvDT"] = jqgridObj.outDT;
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

        private void Batch(string[] args)
        {
            try
            {
                string invoicemaster_id = args[1];

                InvoiceMaster im = new InvoiceMaster(AdFunction.conn);
                im.LoadData(int.Parse(invoicemaster_id));
                if (im.InvoiceMasterBatchId != null)
                {
                    Session["batch"] = im.InvoiceMasterBatchId;
                    Response.Redirect("invoicemasterbatch.aspx", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        private void ImageButtonDelete_Click(string[] args)
        {
            string invoicemaster_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                invoicemaster_id = args[1];

                InvoiceMaster invoicemaster = new InvoiceMaster(constr);
                invoicemaster.LoadData(Convert.ToInt32(invoicemaster_id));


                Bodycorp b = new Bodycorp(AdFunction.conn);
                b.LoadData(invoicemaster.InvoiceMasterBodycorpId);
                if (!b.CheckCloseOff(invoicemaster.InvoiceMasterDate))
                {
                    throw new Exception("Invoice before close date");
                }


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
                Response.Redirect("~/invoicemasteredit.aspx?mode=add", false);
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
                InvoiceMaster invoicemaster = new InvoiceMaster(AdFunction.conn);
                invoicemaster.LoadData(Convert.ToInt32(invoicemaster_id));

                Bodycorp b = new Bodycorp(AdFunction.conn);
                b.LoadData(invoicemaster.InvoiceMasterBodycorpId);
                if (!b.CheckCloseOff(invoicemaster.InvoiceMasterDate))
                {
                    throw new Exception("Invoice before close date");
                }
                Response.BufferOutput = true;
                Response.Redirect("~/invoicemasteredit.aspx?mode=edit&invoicemasterid=" + invoicemaster_id, false);
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
                Response.Redirect("~/invoicemasterdetails.aspx?invoicemasterid=" + invoicemaster_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("invoiceupload.aspx");
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("invoiceExoverwrite.aspx");
        }
    }
}