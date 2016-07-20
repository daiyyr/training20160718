using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using Sapp.Data;
using System.Configuration;
using Sapp.SMS;
using Sapp.JQuery;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;

namespace sapp_sms
{
    public partial class cpaymentdetails : System.Web.UI.Page
    {
        private const string TEMP_TYPE_RELATED = "cpaymentallocate1";
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
                #region Javascript Setup
                Control[] wc = { jqGridRelated };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    #region Load webForm
                    Odbc mydb = null;
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                        string cpayment_id = "";
                        if (Request.QueryString["cpaymentid"] != null) cpayment_id = Request.QueryString["cpaymentid"];
                        CPayment cpayment = new CPayment(constr);
                        cpayment.LoadData(Convert.ToInt32(cpayment_id));

                        LabelCpaymentID.Text = cpayment_id.ToString();

                        Bodycorp bodycorp = new Bodycorp(constr);
                        bodycorp.LoadData(cpayment.Cpayment_bodycorp_id);
                        LabelBodycorp.Text = bodycorp.BodycorpCode;
                        PaymentType paytype = new PaymentType(constr);
                        paytype.LoadData(cpayment.Cpayment_type_id);
                        LabelType.Text = paytype.PaymentTypeCode;
                        CreditorMaster creditor = new CreditorMaster(constr);
                        creditor.LoadData(cpayment.Cpayment_creditor_id);
                        LabelCreditor.Text = creditor.CreditorMasterCode;

                        LabelGross.Text = cpayment.Cpayment_gross.ToString();
                        LabelDate.Text = cpayment.Cpayment_date.ToShortDateString();
                        LabelReference.Text = cpayment.Cpayment_reference;

                        string login = HttpContext.Current.User.Identity.Name;
                        Sapp.General.User user = new Sapp.General.User(constr_general);
                        user.LoadData(login);

                        GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
                        //gltranTemps.Add(cpayment.GetRelatedCInvGLJSON());
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (mydb != null) mydb.Close();
                    }
                    #endregion
                }
                if ("XMLHttpRequest" != Request.Headers["X-Requested-With"])
                {
                    Session["cpaymentid"] = Request.QueryString["cpaymentid"];
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebControl Events
        protected void ImageButtonEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string cpayment_id = "";
                cpayment_id = Request.QueryString["cpaymentid"];
                Response.BufferOutput = true;
                Response.Redirect("~/cpaymentedit.aspx?cpaymentid=" + Server.UrlEncode(cpayment_id) + "&mode=edit" + NewQueryString("&"),false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ImageButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string cpayment_id = "";
                if (Request.QueryString["cpaymentid"] != null) cpayment_id = Request.QueryString["cpaymentid"];
                if (cpayment_id != "")
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    CPayment cpayment = new CPayment(constr);
                    cpayment.LoadData(Convert.ToInt32(cpayment_id));
                    cpayment.Delete();
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata)
        {
            //try
            //{
                //string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                //string sqlfromstr = "FROM( `gl_transactions` inner join `cpayment_gls` on `gl_transaction_id`=`cpayment_gl_gl_id` left join `cinvoice_gls` on `gl_transaction_id`=`cinvoice_gl_gl_id` left join `cinvoices` on `cinvoice_id`=`cinvoice_gl_cinvoice_id` left join `chart_master` on `chart_master_id`=`gl_transaction_chart_id`) where cpayment_gl_cpayment_id=" + HttpContext.Current.Session["cpaymentid"].ToString();
                //string sqlselectstr = "SELECT `ID`,`CinvNumber`,`Chart`,`Description`,`Net`,`Tax`,`Gross`,`Paid` FROM (SELECT `gl_transaction_id` AS `ID`,`cinvoice_num` as `CinvNumber`, `Chart_master_code` AS `Chart`, `gl_transaction_description` AS `Description`,`gl_transaction_net` AS `Net`,`gl_transaction_tax` AS `Tax`,`gl_transaction_gross` AS `Gross`,`cpayment_gl_paid` AS `Paid`";
                //JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
                //string jsonStr = jqgridObj.GetJSONStr();
                //return jsonStr;
                  string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            Sapp.General.User user = new Sapp.General.User(constr_general);
            user.LoadData(login);
            GlTransactionTemps gltranTemps = new GlTransactionTemps(user.UserId, TEMP_TYPE_RELATED, constr_general, constr);
            gltranTemps.LoadData();

            DataTable dt = new DataTable("gl_temp_transactions");
            dt.Columns.Add("ID");
            dt.Columns.Add("CinvoiceNum");
            dt.Columns.Add("Description");
            dt.Columns.Add("Date");
            dt.Columns.Add("DueDate");
            dt.Columns.Add("Net");
            dt.Columns.Add("Tax");
            dt.Columns.Add("Gross");
            dt.Columns.Add("Due");
            dt.Columns.Add("Paid");
            foreach (GlTransactionTemp gltran in gltranTemps.GLTempList)
            {
                Hashtable items = gltran.GetData();
                DataRow dr = dt.NewRow();
                if (items["gl_transaction_id"].ToString()[0] != 'd')
                {
                    dr["ID"] = items["gl_transaction_id"].ToString();
                    dr["CinvoiceNum"] = items["gl_transaction_ref"].ToString();
                    dr["Description"] = items["gl_transaction_description"].ToString();
                    dr["Date"] = items["gl_transaction_date"].ToString();
                    dr["DueDate"] = items["gl_transaction_duedate"].ToString();
                    dr["Net"] = items["gl_transaction_net"].ToString();
                    dr["Tax"] = items["gl_transaction_tax"].ToString();
                    dr["Gross"] = items["gl_transaction_gross"].ToString();
                    dr["Due"] = items["gl_transaction_due"].ToString();
                    dr["Paid"] = items["gl_transaction_paid"].ToString();
                    dt.Rows.Add(dr);
                }
            }
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`, `CinvoiceNum`, `Description`, `Date`, `DueDate`, `Net`, `Tax`,`Gross`, `Due`, `Paid` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }
        #endregion
        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/cpayment.aspx" + NewQueryString("?"),false);
        }
    }
}