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
using System.Collections;
using Sapp.Data;
using System.IO;
namespace sapp_sms
{
    public partial class invoicemasterbatch : System.Web.UI.Page, IPostBackEventHandler
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
                if (Request.QueryString["invid"] != null)
                {
                    string id = Request.QueryString["invid"].ToString();
                    InvoiceMaster im = new InvoiceMaster(AdFunction.conn);
                    im.LoadData(int.Parse(id));
                    Session["batch"] = im.InvoiceMasterBatchId;
                }
                if (Session["batch"] == null)
                {
                    throw new Exception("Invoice Batch Is Not Available");
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
                else if (args[0] == "ImageButtonCopyAdd")
                {
                    ImageButtonCopyAdd_Click(args);     //Add18/04/2016
                }
                else if (args[0] == "ImageButtonEdit")
                {
                    ImageButtonEdit_Click(args);
                }
                else if (args[0] == "ImageButtonAllocate")
                {
                    string id = args[1];
                    Response.Redirect("~/invoiceallocate.aspx?id=" + id, false);
                }
                else if (args[0] == "Export")
                {
                    string id = args[1];
                    Export(id);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        public void Export(string invoice_id)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
  
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
                    FileInfo f = new FileInfo((filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf"));
                    if (!f.Exists)
                    {
                        string template_dir = Server.MapPath("~/templates");

                        template_dir += "\\" + bodycorp.BodycorpInvTpl;
                        rpInvoice rp_invoice = new rpInvoice(constr, template_dir);
                        rp_invoice.SetReportInfo(invoice.InvoiceMasterId);
                        rp_invoice.ExportPDF(filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf");
                        FileInfo fo = new FileInfo(filefolder + "\\" + invoice.InvoiceMasterNum + ".pdf");
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.ClearHeaders();
                        HttpContext.Current.Response.ClearContent();
                        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fo.Name);
                        HttpContext.Current.Response.AddHeader("Content-Length", fo.Length.ToString());
                        HttpContext.Current.Response.ContentType = "text/plain";
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.TransmitFile(fo.FullName);
                        HttpContext.Current.Response.End();


                    }
                    else
                    {
                        Response.Write("<script type='text/javascript'> window.open('invoiceExoverwrite.aspx?invoicemasterid=" + invoice_id + "','_blank'); </script>");
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
                string sqlselectstr = "SELECT `ID`,`Num`,`Debtor`,`Bodycorp`,`Unit`, `Date`, `Due`, `Gross`, `Paid`,`Balance` FROM (SELECT *";
                string batch = HttpContext.Current.Session["batch"].ToString();

                // Update Start 15/03/2016 Update 'Paid' column with 'Discount'
                /*
                string sql = "SELECT `ID`,`Num`,`Debtor`,`Bodycorp`,`Unit`, `Date`, `Due`, `Gross`, `Paid`,invoice_master_batch_id  FROM (SELECT `invoice_master_batch_id`, `invoice_master_id` as `ID`,`invoice_master_num` as `Num`,`debtor_master_name` AS `Debtor`,`bodycorp_code` AS `Bodycorp`,`unit_master_code` AS `Unit`, `invoice_master_date` AS `Date`, `invoice_master_due` AS `Due`, `invoice_master_gross` AS `Gross`, `invoice_master_paid` AS `Paid`  FROM ((((`invoice_master` LEFT JOIN `invoice_types`  ON `invoice_master_type_id`=`invoice_type_id`) LEFT JOIN `debtor_master` ON `debtor_master_id`=`invoice_master_debtor_id`) LEFT JOIN `bodycorps` ON `invoice_master_bodycorp_id`=`bodycorp_id`) LEFT JOIN `unit_master` ON `unit_master_id`=`invoice_master_unit_id`) WHERE `invoice_master_type_id`=1 and invoice_master_batch_id =" + batch + " and bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value + " ) as T1 order by `Date`";
                if (batch.Equals("null"))
                       sql = "SELECT `ID`,`Num`,`Debtor`,`Bodycorp`,`Unit`, `Date`, `Due`, `Gross`, `Paid`,invoice_master_batch_id  FROM (SELECT `invoice_master_batch_id`, `invoice_master_id` as `ID`,`invoice_master_num` as `Num`,`debtor_master_name` AS `Debtor`,`bodycorp_code` AS `Bodycorp`,`unit_master_code` AS `Unit`, `invoice_master_date` AS `Date`, `invoice_master_due` AS `Due`, `invoice_master_gross` AS `Gross`, `invoice_master_paid` AS `Paid`  FROM ((((`invoice_master` LEFT JOIN `invoice_types`  ON `invoice_master_type_id`=`invoice_type_id`) LEFT JOIN `debtor_master` ON `debtor_master_id`=`invoice_master_debtor_id`) LEFT JOIN `bodycorps` ON `invoice_master_bodycorp_id`=`bodycorp_id`) LEFT JOIN `unit_master` ON `unit_master_id`=`invoice_master_unit_id`) WHERE `invoice_master_type_id`=1 and invoice_master_batch_id is null and bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value + " ) as T1 order by `Date`";
                */
                string sql = " SELECT `ID`,`Num`,`Debtor`,`Bodycorp`,`Unit`, `Date`, `Due`, `Gross`, (Net_Paid + IFNULL(T5.Discount, 0)) as 'Paid', invoice_master_batch_id ";
                sql = sql + " FROM ";
                sql = sql + "  ( SELECT `ID`,`Num`,`Debtor`,`Bodycorp`,`Unit`, `Date`, `Due`, `Gross`, `Paid` as 'Net_Paid', invoice_master_batch_id ";
                sql = sql + "    FROM ( SELECT `invoice_master_batch_id`, `invoice_master_id` as `ID`,`invoice_master_num` as `Num`,`debtor_master_name` AS `Debtor`, ";
                sql = sql + "                  `bodycorp_code` AS `Bodycorp`,`unit_master_code` AS `Unit`, `invoice_master_date` AS `Date`, `invoice_master_due` AS `Due`, ";
                sql = sql + "                  `invoice_master_gross` AS `Gross`, `invoice_master_paid` AS `Paid` ";
                sql = sql + "           FROM ( ( ( ( `invoice_master` ";
                sql = sql + "                        LEFT JOIN `invoice_types`  ON `invoice_master_type_id`=`invoice_type_id`) ";
                sql = sql + "                      LEFT JOIN `debtor_master` ON `debtor_master_id`=`invoice_master_debtor_id`) ";
                sql = sql + "                    LEFT JOIN `bodycorps` ON `invoice_master_bodycorp_id`=`bodycorp_id`) ";
                sql = sql + "                  LEFT JOIN `unit_master` ON `unit_master_id`=`invoice_master_unit_id`) ";
                sql = sql + "           WHERE `invoice_master_type_id`=1 ";

                if (batch.Equals("null")) {
                    sql = sql + "          AND invoice_master_batch_id is null ";
                }
                else
                {
                    sql = sql + "          AND invoice_master_batch_id = " + batch;
                }

                sql = sql + "              AND bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value;
                sql = sql + "         ) as T1 ";
                sql = sql + "  ) as T2 ";
                sql = sql + "  LEFT JOIN (";
                sql = sql + "             select  ";
                sql = sql + "                  T3.invoice_gl_invoice_id,  ";
                sql = sql + "                  T4.gl_transaction_net as Discount ";
                sql = sql + "               from  ";
                sql = sql + "                   invoice_gls as T3,  ";
                sql = sql + "                   gl_transactions as T4  ";
                sql = sql + "               where ";
                sql = sql + "                       T3.invoice_gl_gl_id = T4.gl_transaction_id  ";
                sql = sql + "                   and T4.gl_transaction_type_id = 6 ";
                sql = sql + "           ) AS T5 ";
                sql = sql + "  ON T5.invoice_gl_invoice_id = T2.ID ";
                sql = sql + " order by `Date`";
                // Update End 15/03/2016 Update 'Paid' column with 'Discount'

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
                        if (decimal.Parse(dr["Gross"].ToString()) > 0)
                            dr["Balance"] = decimal.Parse(dr["Gross"].ToString()) - decimal.Parse(dr["Paid"].ToString());
                        else
                            dr["Balance"] = decimal.Parse(dr["Gross"].ToString()) + decimal.Parse(dr["Paid"].ToString());
                    }
                }

                string sqlfromstr = "FROM `" + dt.TableName + "`";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                Hashtable userdata = new Hashtable();
                userdata.Add("Unit", "Total");
                userdata.Add("Gross", ReportDT.SumTotal(dt, "Gross").ToString("0.00"));
                userdata.Add("Paid", ReportDT.SumTotal(dt, "Paid").ToString("0.00"));
                jqgridObj.SetUserData(userdata);
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

        // Add 18/04/2016
        private void ImageButtonCopyAdd_Click(string[] args)
        {
            try
            {
                string invoicemaster_id = args[1];

                if (invoicemaster_id != null && invoicemaster_id.Equals(string.Empty) == false)
                {
                    Response.BufferOutput = true;
                    Response.Redirect("~/invoicemasteredit.aspx?mode=paste&invoicemasterid=" + invoicemaster_id, false);
                }
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