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
    public partial class cpayments : System.Web.UI.Page, IPostBackEventHandler
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
                Session["SelectedCpaymentAllactedType"] = ComboBoxAllocated.SelectedValue;
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
                Odbc odbc = new Odbc(AdFunction.conn);
                //string sqlfromstr = "FROM (`Cpayments` left join `bodycorps`  on `bodycorp_id`=`cpayment_bodycorp_id` left join `Creditor_master` on `Creditor_master_id` = `cpayment_creditor_id` left join `payment_types` on `payment_type_id`=`cpayment_type_id`) ";

                //string sqlselectstr = "SELECT `ID`,`Date`, `BodyCorp`, `Creditor`,`Type`,`Reference`,`Gross`, `Allocated` "
                //+ "from (select `cpayment_date` AS `Date`, `cpayment_id` as `ID`,`bodycorp_code` as `BodyCorp`,`creditor_master_code` as `Creditor`,`cpayment_reference` as`Reference`,`payment_type_code` as `Type`,"
                //+ "`cpayment_gross` as `gross`, `cpayment_allocated` AS `Allocated`";

                string dtsql = "SELECT `ID`,`Date`, `BodyCorp`, `Creditor`,`Type`,`Reference`,`Gross`, `Allocated` from (select `cpayment_date` AS `Date`, `cpayment_id` as `ID`,`bodycorp_code` as `BodyCorp`,`creditor_master_code` as `Creditor`,`cpayment_reference` as`Reference`,`payment_type_code` as `Type`,`cpayment_gross` as `gross`, `cpayment_allocated` AS `Allocated` FROM (`Cpayments` left join `bodycorps`  on `bodycorp_id`=`cpayment_bodycorp_id` left join `Creditor_master` on `Creditor_master_id` = `cpayment_creditor_id` left join `payment_types` on `payment_type_id`=`cpayment_type_id`) where cpayment_ctype_id=1 and bodycorp_id=" + HttpContext.Current.Request.Cookies["bodycorpid"].Value + ") AS t1";

                DataTable dt = odbc.ReturnTable(dtsql, "temp");

                // 15/03/2016 Start Add balance column
                dt.Columns.Add("Balance");

                foreach (DataRow dr in dt.Rows)
                {
                    if (!dr["Gross"].ToString().Equals(""))
                    {
                        dr["Balance"] = Math.Abs(decimal.Parse(dr["Gross"].ToString())) - decimal.Parse(dr["Allocated"].ToString());
                    }
                    else
                    {
                        dr["Balance"] = "0.00";
                    }
                }

                string sqlselectstr = "SELECT `ID`,`Date`, `BodyCorp`, `Creditor`,`Type`,`Reference`,`Gross`, `Allocated`, Balance FROM (SELECT * ";
                // 15/03/2016 End Add balance column

                string sqlfromstr = "FROM `" + dt.TableName + "`";
                if (null != HttpContext.Current.Session["SelectedCpaymentAllactedType"])
                {
                    string selectedValue = HttpContext.Current.Session["SelectedCpaymentAllactedType"].ToString();
                    if (selectedValue == "1" || selectedValue == "0")
                    {
                        if (selectedValue == "1")
                            sqlfromstr += " where `cpayment_gross`=`cpayment_allocated`";
                        else
                            sqlfromstr += " where `cpayment_gross`>`cpayment_allocated`";
                    }
                }

                if (null != HttpContext.Current.Session["creditorid"] && !String.IsNullOrEmpty(HttpContext.Current.Session["creditorid"].ToString()))
                {
                    string sql = " where cpayment_creditor_id=" + HttpContext.Current.Session["creditorid"].ToString();
                    sqlfromstr += sql;
                    HttpContext.Current.Session["creditorid"] = "";
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
                CPayment cpayment = new CPayment(constr);
                cpayment.LoadData(Convert.ToInt32(cpayment_id));
                Bodycorp b = new Bodycorp(constr);
                b.LoadData(cpayment.Cpayment_bodycorp_id);
                if (!b.CheckCloseOff(cpayment.Cpayment_date))
                {
                    throw new Exception("Payement before close date");

                }
                else if (cpayment.CpaymentReconciled)
                {
                    throw new Exception("Payement Reconciled");
                }

                cpayment.Delete(Convert.ToInt32(cpayment_id));
                Response.BufferOutput = true;
                Response.Redirect("~/cpayments.aspx" + NewQueryString("?"), false);
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
                Response.Redirect("~/cpaymentedit.aspx?mode=add" + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            string cpayment_id = "";
            try
            {
                cpayment_id = args[1];
                Bodycorp b = new Bodycorp(AdFunction.conn);
                CPayment cpayment = new CPayment(AdFunction.conn);
                cpayment.LoadData(Convert.ToInt32(cpayment_id));
                b.LoadData(cpayment.Cpayment_bodycorp_id);
                if (!b.CheckCloseOff(cpayment.Cpayment_date))
                {
                    throw new Exception("Payement before close date");

                }
                else if (cpayment.CpaymentReconciled)
                {
                    throw new Exception("Payement Reconciled");
                }

                Response.BufferOutput = true;
                Response.Redirect("~/cpaymentedit.aspx?mode=edit&cpaymentid=" + cpayment_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonDetails_Click(string[] args)
        {
            string cpayment_id = "";
            try
            {
                cpayment_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/cpaymentdetails.aspx?cpaymentid=" + cpayment_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonAllocate_Click(string[] args)
        {
            string cpayment_id = "";
            try
            {
                cpayment_id = args[1];
                Response.BufferOutput = true;
                Bodycorp b = new Bodycorp(AdFunction.conn);
                CPayment cpayment = new CPayment(AdFunction.conn);
                cpayment.LoadData(Convert.ToInt32(cpayment_id));
                b.LoadData(cpayment.Cpayment_bodycorp_id);
                if (!b.CheckCloseOff(cpayment.Cpayment_date))
                {
                    throw new Exception("Payement before close date");

                }

                Response.Redirect("~/cpaymentallocate.aspx?cpaymentid=" + cpayment_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ComboBoxAllocated_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/cpaymentupload.aspx", false);
        }
    }
}