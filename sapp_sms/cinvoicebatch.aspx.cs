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
using System.Collections;
using Sapp.Data;
namespace sapp_sms
{
    public partial class cinvoicebatch : System.Web.UI.Page, IPostBackEventHandler
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
                if (!IsPostBack)
                {
                    AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", AdFunction.conn, false);
                }
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

                if (args[0] == "ImageButtonEdit")
                {
                    //ImageButtonEdit_Click(args);
                }



            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string SaveDataFromGrid(string rowValue)
        {
            Object c = JSON.JsonDecode(rowValue);
            Hashtable hdata = (Hashtable)c;
            decimal ba = AdFunction.Rounded(hdata["Balance"].ToString());
            decimal pay = AdFunction.Rounded(hdata["Pay"].ToString());

            if (pay <= ba)
            {
                ArrayList a = new ArrayList();
                if (HttpContext.Current.Session["Pay"] != null)
                {
                    a = (ArrayList)HttpContext.Current.Session["Pay"];
                }

                a.Add(hdata);
                HttpContext.Current.Session["Pay"] = a;
            }
            else
            {
                throw new Exception("Pay over balance");
            }
            return "";
        }
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Odbc odbc = new Odbc(AdFunction.conn);
                string sql = "SELECT `ID`,`Num`,`Order`,`Creditor`,`Bodycorp`,`Unit`,`Date`,`Due`,`Gross`,`Paid`,`Balance` from (select  `cinvoice_id` as `ID`,`cinvoice_num` as `Num`,`purchorder_master_num` as `Order`, `creditor_master_code` AS `Creditor`, `bodycorp_code` AS `Bodycorp`,`unit_master_code` as `Unit`,DATE_FORMAT(`cinvoice_date`, '%d/%m/%Y') as `Date`,DATE_FORMAT(`cinvoice_due`, '%d/%m/%Y') as `Due`,`cinvoice_gross` AS `Gross`,`cinvoice_paid` AS `Paid`, `cinvoice_gross`-`cinvoice_paid` AS `Balance` FROM (`Cinvoices` left join `purchorder_master`  on `cinvoice_order_id`=`purchorder_master_id` left join `creditor_master`  on `cinvoice_creditor_id`=`creditor_master_id` left join `bodycorps`  on `cinvoice_bodycorp_id`=`bodycorp_id` left join `unit_master`  on `cinvoice_unit_id`=`unit_master_id`) where cinvoice_type_id=1 and  `cinvoice_gross` - `cinvoice_paid` <>0) as T1";

                DataTable dt = odbc.ReturnTable(sql, "paytemp");
                dt.Columns.Add("Pay");
                dt = ReportDT.FilterDT(dt, "Paid =0");
                if (HttpContext.Current.Session["Pay"] != null)
                {
                    ArrayList a = (ArrayList)HttpContext.Current.Session["Pay"];
                    for (int i = 0; i < a.Count; i++)
                    {
                        Hashtable hdata = (Hashtable)a[i];
                        foreach (DataRow dr in dt.Rows)
                        {
                            string id = hdata["ID"].ToString();
                            string pay = hdata["Pay"].ToString();
                            if (dr["ID"].ToString().Equals(id))
                            {
                                dr["Pay"] = pay;
                            }
                        }
                    }
                }
                HttpContext.Current.Session["PaySubmit"] = dt;
                string sqlfromstr = "FROM `" + dt.TableName + "`";
                //string sqlfromstr = "FROM (`Cinvoices` left join `purchorder_master`  on `cinvoice_order_id`=`purchorder_master_id` left join `creditor_master`  on `cinvoice_creditor_id`=`creditor_master_id` left join `bodycorps`  on `cinvoice_bodycorp_id`=`bodycorp_id` left join `unit_master`  on `cinvoice_unit_id`=`unit_master_id`) ";
                string sqlselectstr = "SELECT `ID`,`Num`,`Order`,`Creditor`,`Bodycorp`,`Unit`,`Date`,`Due`,`Gross`,`Paid`,`Balance`,`Pay` FROM (SELECT * ";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                Hashtable userdata = new Hashtable();
                userdata.Add("Paid", "Total:");
                userdata.Add("Pay", ReportDT.SumTotal(dt, "Pay").ToString());
                jqgridObj.SetUserData(userdata);
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


        protected void ImageButtonEdit_Click1(object sender, ImageClickEventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            try
            {

                if (HttpContext.Current.Session["PaySubmit"] != null)
                {
                    #region Load System Chart ID

                    Sapp.SMS.System s = new Sapp.SMS.System(AdFunction.conn);
                    ChartMaster ch = new ChartMaster(AdFunction.conn);
                    s.LoadData("GST Input");
                    ch.LoadData(s.SystemValue);
                    string InputGstID = ch.ChartMasterId.ToString();
                    s.LoadData("GST Output");
                    ch.LoadData(s.SystemValue);
                    string OutputGstID = ch.ChartMasterId.ToString();
                    s.LoadData("GENERALTAX");
                    ch.LoadData(s.SystemValue);
                    string gstid = ch.ChartMasterId.ToString();
                    s.LoadData("GENERALDEBTOR");
                    ch.LoadData(s.SystemValue);
                    ch.LoadData(s.SystemValue);
                    string proprietorID = ch.ChartMasterId.ToString();
                    s.LoadData("GENERALCREDITOR");
                    ch.LoadData(s.SystemValue);
                    string creditorID = ch.ChartMasterId.ToString();
                    s.LoadData("DISCOUNTCHARCODE");
                    ch.LoadData(s.SystemValue);
                    string discountID = ch.ChartMasterId.ToString();

                    #endregion
                    DataTable dt = (DataTable)HttpContext.Current.Session["PaySubmit"];
                    dt = ReportDT.FilterDT(dt, "Pay is not null");
                    decimal tpay = ReportDT.SumTotal(dt, "Pay");

                    if (tpay != decimal.Parse(AmountL.Text))
                    {
                        throw new Exception("Payment not equals amount");
                    }
                    foreach (DataRow dr in dt.Rows)
                    {

                        string id = dr["ID"].ToString();
                        string pay = dr["Pay"].ToString();
                        decimal p = 0;
                        decimal.TryParse(pay, out p);
                        if (p > 0)
                        {
                            Cinvoice c = new Cinvoice(AdFunction.conn);
                            c.LoadData(int.Parse(id));




                            Hashtable items = new Hashtable();

                            items.Add("cpayment_bodycorp_id", c.CinvoiceBodycorpId);
                            items.Add("cpayment_creditor_id", c.CinvoiceCreditorId);
                            items.Add("cpayment_reference", c.CinvoiceId);
                            items.Add("cpayment_type_id", ComboBoxType.SelectedValue);
                            items.Add("cpayment_gross", AmountT.Text);
                            items.Add("cpayment_date", DBSafeUtils.DateToSQL(c.CinvoiceDate));



                            CPayment cpayment = new CPayment(AdFunction.conn);
                            cpayment.Add(items);


                            Hashtable allowitems = new Hashtable();
                            allowitems.Add("cinvoice_id", c.CinvoiceId);
                            allowitems.Add("gl_transaction_description", "CPayment");
                            allowitems.Add("gl_transaction_tax", "0");
                            allowitems.Add("gl_transaction_date", "'" + c.CinvoiceDate + "'");
                            allowitems.Add("gl_transaction_type_id", "4");
                            allowitems.Add("gl_transaction_gross", "0");
                            allowitems.Add("gl_transaction_chart_id", creditorID);
                            allowitems.Add("gl_transaction_ref", "'" + c.CinvoiceNum + "'");
                            allowitems.Add("gl_transaction_bodycorp_id", c.CinvoiceBodycorpId);
                            allowitems.Add("gl_transaction_net", c.CinvoiceGross);
                            cpayment.AddGlTran(allowitems, true);



                            c.UpdatePaid();
                        }
                    }
                }
                Response.Redirect("cinvoices.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            Panel2.Visible = true;
            AmountL.Text = AmountT.Text;
            TypeL.Text = ComboBoxType.SelectedItem.Text;
        }



    }
}