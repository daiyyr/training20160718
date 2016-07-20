using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.SMS;
using System.Configuration;
using System.Collections;
using Sapp.Data;
using Sapp.Common;
using Sapp.JQuery;
using System.Text.RegularExpressions;
using System.Data;
namespace sapp_sms
{
    public partial class debtormasterdetails : System.Web.UI.Page, IPostBackEventHandler
    {
        private string CheckedQueryString()
        {
            if (null != Request.Cookies["bodycorpid"].Value && Regex.IsMatch(Request.Cookies["bodycorpid"].Value, "^[0-9]*$"))
            {
                return Request.Cookies["bodycorpid"].Value;
            }
            return "";
        }
        private string NewQueryString(string connector)
        {
            string result = CheckedQueryString();
            return string.IsNullOrEmpty(result) ? "" : connector + "bodycorpid=" + result;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { jqGridComms };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    #region Load webForm
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        string debtor_master_id = "";
                        if (Request.QueryString["debtorid"] != null) debtor_master_id = Request.QueryString["debtorid"];
                        DebtorMaster debtormaster = new DebtorMaster(constr);
                        debtormaster.LoadData(Convert.ToInt32(debtor_master_id));
                        LabelID.Text = debtormaster.DebtorMasterId.ToString();

                        Odbc o = new Odbc(AdFunction.conn);

                        DataTable codeDT = o.ReturnTable("select * from unit_master where unit_master_debtor_id =" + debtormaster.DebtorMasterId, "tt");
                        foreach (DataRow codeDR in codeDT.Rows)
                        {
                            LabelCode.Text = LabelCode.Text + codeDR["unit_master_debtor_code"].ToString() + ",";
                        }
                        if (LabelCode.Text.Contains(","))
                            LabelCode.Text = LabelCode.Text.Substring(0, LabelCode.Text.Length - 1);
                        if (LabelCode.Text.Length > 20)
                        {
                            LabelCode.ToolTip = LabelCode.Text;
                            LabelCode.Text = LabelCode.Text.Substring(0, 20) + "...";
                        }

                        LabelName.Text = debtormaster.DebtorMasterName;
                        LabelNotes.Text = debtormaster.DebtorMasterNotes;
                        LabelSalutation.Text = debtormaster.DebtorMasterSalutation;

                        DebtorType debtype = new DebtorType(constr);
                        debtype.LoadData(debtormaster.DebtorMasterTypeId);
                        LabelType.Text = debtype.DebtorTypeName;
                        if (debtormaster.DebtorMasterPaymentTermId.HasValue)
                        {
                            PaymentTerm payterm = new PaymentTerm(constr);
                            payterm.LoadData(debtormaster.DebtorMasterPaymentTermId.Value);
                            LabelPaymentTerm.Text = payterm.PaymentTermName;
                        }
                        if (debtormaster.DebtorMasterPaymentTypeId.HasValue)
                        {
                            PaymentType paytype = new PaymentType(constr);
                            paytype.LoadData(debtormaster.DebtorMasterPaymentTypeId.Value);
                            LabelPaymentType.Text = paytype.PaymentTypeName;
                        }
                        if (debtormaster.DebtorMasterPrint.HasValue)
                            LabelPrint.Text = debtormaster.DebtorMasterPrint.Value ? "Yes" : "No";
                        if (debtormaster.DebtorMasterEmail.HasValue)
                            LabelEmail.Text = debtormaster.DebtorMasterEmail.Value ? "Yes" : "No";
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    #endregion
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
                if (args[0] == "ButtonDeleteComm")
                {
                    ButtonDeleteComm_Click(args);
                }
                else
                    throw new Exception("Error: unknown command!");
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridCommsDataBind(string postdata, string debtorid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                DebtorMaster debtormaster = new DebtorMaster(constr);
                debtormaster.LoadData(Convert.ToInt32(debtorid));
                Comms<DebtorMaster> comms = new Comms<DebtorMaster>(constr);
                comms.LoadData(debtormaster);
                ArrayList commList = comms.Communications;
                ArrayList rowData = new ArrayList();
                int id = 1;
                foreach (Comm<DebtorMaster> comm in commList)
                {
                    Hashtable items = new Hashtable();
                    items.Add("id", id);
                    ArrayList cells = new ArrayList();
                    cells.Add(comm.CommMasterId.ToString());
                    CommType commType = new CommType(constr);
                    commType.LoadData(comm.CommMasterTypeId);
                    cells.Add(commType.CommTypeCode);
                    cells.Add(comm.CommMasterData);
                    cells.Add(DBSafeUtils.DBBoolToStr(comm.CommMasterPrimary));
                    cells.Add(id.ToString());
                    items.Add("cell", cells);
                    rowData.Add(items);
                    id++;
                }
                JQGrid jqgridObj = new JQGrid(postdata, constr, rowData);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }

        [System.Web.Services.WebMethod]
        public static string DataGridCommsTypeSelect()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                CommTypes commTypes = new CommTypes(constr);
                commTypes.LoadData();
                foreach (CommType commType in commTypes.CommTypeList)
                {
                    html += "<option value='" + commType.CommTypeId.ToString() + "'>" + commType.CommTypeCode + "</option>";
                }
                html += "</select>";
                return html;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }

        [System.Web.Services.WebMethod]
        public static void DataGridCommsSave(string rowValue, string debtorid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable rowObj = (Hashtable)JSON.JsonDecode(rowValue);
                Hashtable items = new Hashtable();
                #region Retireve Values
                CommType commType = new CommType(constr);
                commType.LoadData(rowObj["Type"].ToString());
                items.Add("comm_master_type_id", commType.CommTypeId);
                items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(rowObj["Details"].ToString()));

                // Update 28/04/2016
                //items.Add("comm_master_primary", Convert.ToInt32(rowObj["Primary"]));
                if (rowObj["Primary"].ToString().Equals("Yes"))
                {
                    items.Add("comm_master_primary", 1);
                }
                else
                {
                    items.Add("comm_master_primary", 0);
                }
                #endregion

                if (rowObj["ID"].ToString() != "")
                {
                    DebtorMaster debtormaster = new DebtorMaster(constr);
                    debtormaster.LoadData(Convert.ToInt32(debtorid));
                    Comm<DebtorMaster> comm = new Comm<DebtorMaster>(constr);
                    comm.Update(items, Convert.ToInt32(rowObj["ID"]));
                }
                else
                {
                    DebtorMaster debtormaster = new DebtorMaster(constr);
                    debtormaster.LoadData(Convert.ToInt32(debtorid));
                    Comm<DebtorMaster> comm = new Comm<DebtorMaster>(constr);
                    comm.Add(items, debtormaster);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        #region WebControl Events

        private void ButtonDeleteComm_Click(string[] args)
        {
            try
            {
                string comm_master_id = "";
                comm_master_id = args[1];
                string debtor_master_id = "";
                if (Request.QueryString["debtorid"] != null) debtor_master_id = Request.QueryString["debtorid"];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                DebtorMaster debtormaster = new DebtorMaster(constr);
                debtormaster.LoadData(Convert.ToInt32(debtor_master_id));
                Comm<DebtorMaster> comm = new Comm<DebtorMaster>(constr);
                comm.Delete(Convert.ToInt32(comm_master_id), debtormaster);
                Response.BufferOutput = true;
                Response.Redirect("~/debtormasterdetails.aspx?debtorid=" + debtor_master_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string debtor_master_id = "";
                if (Request.QueryString["debtorid"] != null) debtor_master_id = Request.QueryString["debtorid"];
                Response.BufferOutput = true;
                Response.Redirect("~/debtormasteredit.aspx?mode=edit&debtorid=" + debtor_master_id + NewQueryString("&"), false);
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
                string debtor_master_id = Request.QueryString["debtorid"];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                DebtorMaster debtormaster = new DebtorMaster(constr);
                debtormaster.Delete(Convert.ToInt32(debtor_master_id));
                Response.BufferOutput = true;
                Response.Redirect("goback.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/debtormaster.aspx" + NewQueryString("?"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonInvoice_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string debtorid = "";
                string bodycorpid = "";
                string unitid = "";
                string querystring = "";
                if (Request.QueryString["debtorid"] != null)
                    debtorid = Request.QueryString["debtorid"].ToString();
                if (Request.Cookies["bodycorpid"].Value != null)
                    bodycorpid = Request.Cookies["bodycorpid"].Value;
                if (Request.QueryString["unitid"] != null)
                    unitid = Request.QueryString["unitid"].ToString();
                if (debtorid != "")
                    querystring += "debtorid=" + debtorid;
                if (bodycorpid != "")
                    querystring += "&bodycorpid=" + bodycorpid;
                if (unitid != "")
                    querystring += "&unitid=" + unitid;
                Response.Redirect("invoicemasteredit.aspx?mode=add&" + querystring, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonReceipt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string debtorid = "";
                string bodycorpid = "";
                string querystring = "";
                string unitid = "";
                if (Request.QueryString["debtorid"] != null)
                    debtorid = Request.QueryString["debtorid"].ToString();
                if (Request.Cookies["bodycorpid"].Value != null)
                    bodycorpid = Request.Cookies["bodycorpid"].Value;
                if (Request.QueryString["unitid"] != null)
                    unitid = Request.QueryString["unitid"].ToString();
                if (debtorid != "")
                    querystring += "debtorid=" + debtorid;
                if (unitid != "")
                    querystring += "&unitid=" + unitid;
                if (bodycorpid != "")
                    querystring += "&bodycorpid=" + bodycorpid;
                Response.Redirect("receiptedit.aspx?mode=add&" + querystring, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #endregion

        protected void ImageButton3_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string debtorid = "";
                string bodycorpid = "";
                string querystring = "";
                string unitid = "";
                if (Request.QueryString["debtorid"] != null)
                    debtorid = Request.QueryString["debtorid"].ToString();
                if (Request.Cookies["bodycorpid"].Value != null)
                    bodycorpid = Request.Cookies["bodycorpid"].Value;
                if (Request.QueryString["unitid"] != null)
                    unitid = Request.QueryString["unitid"].ToString();
                if (debtorid != "")
                    querystring += "debtorid=" + debtorid;
                if (unitid != "")
                    querystring += "&unitid=" + unitid;
                if (bodycorpid != "")
                    querystring += "&bodycorpid=" + bodycorpid;
                Response.Redirect("refundedit.aspx?mode=add&" + querystring, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
    }
}