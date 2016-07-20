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
using System.IO;
namespace sapp_sms
{
    public partial class invoicemaster : System.Web.UI.Page, IPostBackEventHandler
    {
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
                else if (args[0] == "Batch")
                {
                    Batch(args);
                }
                else if (args[0] == "Delete")
                {
                    DeleteBatchInv(args);
                }
                else if (args[0] == "Export")
                {
                    Export(args);
                }

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }

        }

        public void Export(string[] args)
        {
            try
            {
                Odbc o = new Odbc(AdFunction.conn);
                InvoiceMaster im = new InvoiceMaster(AdFunction.conn);
                im.SetOdbc(o);
                im.LoadData(int.Parse(args[1]));

                DataTable dt;
                if (im.InvoiceMasterBatchId != null)
                    dt = o.ReturnTable("select * from invoice_master where invoice_master_batch_id=" + im.InvoiceMasterBatchId, "i");
                else
                    dt = o.ReturnTable("select * from invoice_master where invoice_master_batch_id is null", "i");

                foreach (DataRow dr in dt.Rows)
                {
                    string InvID = dr["invoice_master_id"].ToString();
                    InvoiceMaster invoice = new InvoiceMaster(AdFunction.conn);
                    invoice.LoadData(Convert.ToInt32(InvID));
                    Sapp.SMS.System system = new Sapp.SMS.System(AdFunction.conn);
                    system.LoadData("FILEFOLDER");
                    string filefolder = system.SystemValue;
                    Bodycorp bodycorp = new Bodycorp(AdFunction.conn);
                    bodycorp.LoadData(invoice.InvoiceMasterBodycorpId);
                    string bodycorp_code = bodycorp.BodycorpCode;
                    filefolder += bodycorp_code + "\\INVOICE";
                    if (!Directory.Exists(filefolder))
                    {
                        Directory.CreateDirectory(filefolder);
                    }
                    DirectoryInfo dirInfo = new DirectoryInfo(filefolder);
                    FileInfo[] fileInfos = dirInfo.GetFiles();
                    FileInfo f = new FileInfo((filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf"));
                    if (!f.Exists)
                    {
                        string template_dir = Server.MapPath("~/templates");

                        template_dir += "\\" + bodycorp.BodycorpInvTpl;
                        rpInvoice rp_invoice = new rpInvoice(AdFunction.conn, template_dir);
                        rp_invoice.SetReportInfo(invoice.InvoiceMasterId);
                        rp_invoice.ExportPDF(filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf");
                        FileInfo fo = new FileInfo(filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf");
                    }
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
                string sqlselectstr = "SELECT `ID`,`Num`,`Bodycorp`,`InvCount`, `Date`,  `Gross`, `Paid`,`Balance`,`Type` FROM (SELECT *";

                string sql = "SELECT `ID`,`Num`,`Bodycorp`, `Date`, `Gross`, `Paid`,invoice_master_batch_id  FROM (SELECT `invoice_master_batch_id`, `invoice_master_id` as `ID`,`invoice_master_num` as `Num`,`debtor_master_name` AS `Debtor`,`bodycorp_code` AS `Bodycorp`,`unit_master_code` AS `Unit`, `invoice_master_date` AS `Date`, `invoice_master_due` AS `Due`, `invoice_master_gross` AS `Gross`, `invoice_master_paid` AS `Paid`  FROM ((((`invoice_master` LEFT JOIN `invoice_types`  ON `invoice_master_type_id`=`invoice_type_id`) LEFT JOIN `debtor_master` ON `debtor_master_id`=`invoice_master_debtor_id`) LEFT JOIN `bodycorps` ON `invoice_master_bodycorp_id`=`bodycorp_id`) LEFT JOIN `unit_master` ON `unit_master_id`=`invoice_master_unit_id`) WHERE `invoice_master_type_id`=1 and bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value + ") as T1 group by invoice_master_batch_id order by `ID`";
                Odbc o = new Odbc(constr);
                DataTable dt = o.ReturnTable(sql, "NewTable");
                dt.Columns.Add("InvCount");
                dt.Columns.Add("Type");
                dt.Columns.Add("Balance");
                foreach (DataRow dr in dt.Rows)
                {
                    string batch = dr["invoice_master_batch_id"].ToString();

                    //int index = 3;
                    //if (dr["Num"].ToString().Contains("-"))
                    //    index = dr["Num"].ToString().IndexOf("-");
                    //if (dr["Num"].ToString().Contains("/"))
                    //    index = dr["Num"].ToString().IndexOf("/");
                    //dr["Num"] = dr["Num"].ToString().Substring(0, index);
                    dr["Num"] = dr["Num"].ToString();
                    if (!batch.Equals(""))
                    {
                        dr["Type"] = "Batch";
                        string csql = "select count(invoice_master_id) from invoice_master where invoice_master_bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value + " and invoice_master_batch_id=" + batch;
                        dr["InvCount"] = o.ReturnTable(csql, "temp").Rows[0][0].ToString();
                        dr["Gross"] = o.ReturnTable("select sum(invoice_master_gross) from invoice_master where invoice_master_bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value + " and invoice_master_batch_id=" + batch, "temp").Rows[0][0].ToString();
                        dr["Paid"] = o.ReturnTable("select sum(invoice_master_paid) from invoice_master where invoice_master_bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value + " and invoice_master_batch_id=" + batch, "temp").Rows[0][0].ToString();
                    }
                    else
                    {
                        string csql = "select count(invoice_master_id) from invoice_master where invoice_master_bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value + " and invoice_master_batch_id is null";

                        dr["InvCount"] = o.ReturnTable(csql, "temp").Rows[0][0].ToString();
                        dr["Date"] = DBNull.Value;
                        dr["Type"] = "Standard";
                        dr["Gross"] = DBNull.Value;
                        dr["Paid"] = DBNull.Value;
                    }
                    if (!dr["Gross"].ToString().Equals(""))
                    {
                        if (decimal.Parse(dr["Gross"].ToString()) > 0)
                            dr["Balance"] = decimal.Parse(dr["Gross"].ToString()) - decimal.Parse(dr["Paid"].ToString());
                        else
                            dr["Balance"] = decimal.Parse(dr["Gross"].ToString()) + decimal.Parse(dr["Paid"].ToString());
                    }
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

        private void DeleteBatchInv(string[] args)
        {
            Odbc o = new Odbc(AdFunction.conn);
            o.StartTransaction();
            string invID = "";
            try
            {
                InvoiceMaster im = new InvoiceMaster(AdFunction.conn);
                im.SetOdbc(o);
                im.LoadData(int.Parse(args[1]));

                DataTable dt = o.ReturnTable("select * from invoice_master where invoice_master_batch_id=" + im.InvoiceMasterBatchId, "i");

                foreach (DataRow dr in dt.Rows)
                {
                    invID = dr["invoice_master_id"].ToString();
                    im.LoadData(int.Parse(invID));
                    im.Delete();
                }
                o.Commit();
            }
            catch (Exception ex)
            {
                o.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }

        }

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
                else
                {
                    Session["batch"] = "null";
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

        protected void ImageButton3_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("invoicemasterOLD.aspx");

        }

        protected void ImageButton4_Click(object sender, ImageClickEventArgs e)
        {
            DataTable dt = new DataTable();
            //dt.Columns.Add("BodyCorpCode");
            dt.Columns.Add("InvNum");
            dt.Columns.Add("UnitNum");
            dt.Columns.Add("ChartCode");
            dt.Columns.Add("InvDate");
            dt.Columns.Add("InvDue");
            dt.Columns.Add("InvApply");     // Add 06/05/2016
            dt.Columns.Add("InvDescription");
            dt.Columns.Add("LineGross");
            dt.Columns.Add("LineNet");
            dt.Columns.Add("LineTax");
            dt.Columns.Add("LineDesc");

            CsvDT.DataTableToCsv(dt, Server.MapPath("~/Temp/InvTemplate.csv"));
            Response.Redirect("~/Temp/InvTemplate.csv");

        }
    }
}