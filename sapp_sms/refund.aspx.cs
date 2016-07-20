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
using System.Globalization;
namespace sapp_sms
{
    public partial class refund : System.Web.UI.Page, IPostBackEventHandler
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
                //Session["SelectType"] = ComboBoxAllocated.SelectedValue;
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


                string sql = "SELECT   bodycorps.*, debtor_master.*, receipts.*, unit_master.* FROM      receipts, bodycorps, debtor_master, unit_master WHERE   receipts.receipt_bodycorp_id = bodycorps.bodycorp_id AND                  receipts.receipt_debtor_id = debtor_master.debtor_master_id AND receipts.receipt_unit_id = unit_master.unit_master_id and receipt_type_id=2 and bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value;
                if (null != HttpContext.Current.Session["debtorid"] && !String.IsNullOrEmpty(HttpContext.Current.Session["debtorid"].ToString()))
                {
                    sql += " where receipt_debtor_id=" + HttpContext.Current.Session["debtorid"].ToString();
                    HttpContext.Current.Session["debtorid"] = "";
                }
                Odbc o = new Odbc(constr);
                DataTable dt = o.ReturnTable(sql, "NewTable");

                // 15/03/2016 Start Add balance column
                dt.Columns.Add("Balance");

                foreach (DataRow dr in dt.Rows)
                {
                    if (!dr["receipt_gross"].ToString().Equals(""))
                    {
                        dr["Balance"] = Math.Abs(decimal.Parse(dr["receipt_gross"].ToString())) - decimal.Parse(dr["receipt_allocated"].ToString());
                    }
                    else
                    {
                        dr["Balance"] = "0.00";
                    }
                }

                string sqlselectstr = "SELECT `receipt_id`,`unit_master_code`,`bodycorp_code`,`debtor_master_name`,`receipt_ref`,-`receipt_gross`,`receipt_allocated`, Balance,`receipt_date` FROM (SELECT * ";
                // 15/03/2016 End Add balance column

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
                    throw new Exception("Refund before close date");

                }
                else if (receipt.receipt_reconciled)
                {
                    throw new Exception("Refund Reconciled");
                }

                else
                {
                    receipt.Delete(Convert.ToInt32(cpayment_id));
                    Response.BufferOutput = true;
                    Response.Redirect("~/refund.aspx" + NewQueryString("?"), false);
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
                Response.Redirect("~/refundedit.aspx?mode=add&multi=false", false);
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
                Response.Redirect("~/refundedit.aspx?mode=add&multi=true", false);
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
                    throw new Exception("Refund under close off date");
                }
                else if (r.receipt_reconciled)
                {
                    throw new Exception("Refund Reconciled");

                }
                else
                {
                    Response.BufferOutput = true;
                    Response.Redirect("~/refundedit.aspx?mode=edit&receiptid=" + receipt_id + NewQueryString("&"), false);
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
                Response.Redirect("~/refunddetails.aspx?receiptid=" + receipt_id + NewQueryString("&"), false);
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
                Response.BufferOutput = true;
                Response.Redirect("~/refundallocate.aspx?receiptid=" + receipt_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion


    }
}