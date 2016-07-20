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
    public partial class receipts : System.Web.UI.Page, IPostBackEventHandler
    {
        private string CheckedQueryString()
        {
            if (null != Request.QueryString["debtorid"] && Regex.IsMatch(Request.QueryString["debtorid"], "^[0-9]*$"))
            {
                return Request.QueryString["debtorid"];
            }
            return "";
        }
        private string NewQueryString(string connector)
        {
            string result = CheckedQueryString();
            return string.IsNullOrEmpty(result) ? "" : connector + "debtorid=" + result;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Session["SelectType"] = ComboBoxAllocated.SelectedValue;
                #region Javascript setup
                Control[] wc = { jqGridTable };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                string qscreditorid = CheckedQueryString();
                Session["debtorid"] = qscreditorid;
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
                    ImageButtonAllocate_Click(args);
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


                string sql = "SELECT   * FROM      bodycorps, debtor_master, { oj receipts LEFT OUTER JOIN                 unit_master ON receipts.receipt_unit_id = unit_master.unit_master_id } WHERE   receipts.receipt_bodycorp_id = bodycorps.bodycorp_id AND                  receipts.receipt_debtor_id = debtor_master.debtor_master_id AND (receipts.receipt_type_id = 1) and bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value;

                if (null != HttpContext.Current.Session["debtorid"] && !String.IsNullOrEmpty(HttpContext.Current.Session["debtorid"].ToString()))
                {
                    sql += " where receipt_debtor_id=" + HttpContext.Current.Session["debtorid"].ToString();
                    HttpContext.Current.Session["debtorid"] = "";
                }
                Odbc o = new Odbc(constr);
                DataTable dt = o.ReturnTable(sql, "NewTable");
                string sqlselectstr = "SELECT `receipt_id`,`unit_master_code`,`bodycorp_code`,`debtor_master_name`,`receipt_ref`,`receipt_gross`,`receipt_allocated`,`Balance` ,`receipt_date`FROM (SELECT * ";
                string sqlfromstr = "FROM `" + dt.TableName + "`";
                if (HttpContext.Current.Session["SelectType"] != null)
                {
                    if (!HttpContext.Current.Session["SelectType"].Equals("*"))
                    {
                        string t = HttpContext.Current.Session["SelectType"].ToString();
                        if (t.Equals("1"))
                        {
                            t = "<=";
                        }
                        if (t.Equals("0"))
                        {
                            t = ">";
                        }
                        dt.DefaultView.RowFilter = " Gross" + t + " receipt_allocated ";
                        dt = dt.DefaultView.ToTable();

                    }
                }
                dt.Columns.Add("Balance");
                foreach (DataRow dr in dt.Rows)
                {
                    decimal allow = decimal.Parse(dr["receipt_allocated"].ToString());
                    decimal gross = decimal.Parse(dr["receipt_gross"].ToString());
                    decimal balance = Math.Abs(gross) - allow;
                    dr["Balance"] = balance;
                }
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
            string cpayment_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;


                cpayment_id = args[1];

                Receipt receipt = new Receipt(constr);

                receipt.LoadData(int.Parse(cpayment_id));

                Bodycorp b = new Bodycorp(constr);
                b.LoadData(receipt.ReceiptBodycorpId);
                if (!b.CheckCloseOff(receipt.ReceiptDate))
                {
                    throw new Exception("Receipt before close date");

                }
                else if (receipt.receipt_reconciled)
                {
                    throw new Exception("Receipt Reconciled");
                }

                else
                {
                    receipt.Delete(Convert.ToInt32(cpayment_id));
                    Response.BufferOutput = true;
                    Response.Redirect("~/receipts.aspx" + NewQueryString("?"), false);
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                Response.Redirect("error.aspx", false);
            }
        }

        protected void ImageButtonAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/receiptedit.aspx?mode=add&multi=false", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonMutliUnit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/receipteditM.aspx", false);
            } 
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            string receipt_id = "";
            try
            {

                receipt_id = args[1];

                Receipt r = new Receipt(AdFunction.conn);
                r.LoadData(int.Parse(receipt_id));
                Bodycorp b = new Bodycorp(AdFunction.conn);
                b.LoadData(r.ReceiptBodycorpId);
                if (!b.CheckCloseOff(r.ReceiptDate))
                {
                    throw new Exception("Receipt under close off date");
                }
                else if (r.receipt_reconciled)
                {
                    throw new Exception("Receipt Reconciled");

                }
                else
                {
                    Response.BufferOutput = true;
                    Response.Redirect("~/receiptedit.aspx?mode=edit&receiptid=" + receipt_id + NewQueryString("&"), false);
                }
            }
            catch (Exception ex)
            {
                //Session["Error"] = ex.Message; Response.Redirect("error.aspx");
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonDetails_Click(string[] args)
        {
            string receipt_id = "";
            try
            {
                receipt_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/receiptdetails.aspx?receiptid=" + receipt_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonAllocate_Click(string[] args)
        {
            string receipt_id = "";
            try
            {
                receipt_id = args[1];
                Response.Redirect("~/receiptallocate.aspx?receiptid=" + receipt_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.Redirect("~/receipteditMR.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


    }
}