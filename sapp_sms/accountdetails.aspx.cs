using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data.Odbc;
using Sapp.Common;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.SMS;
using System.Text.RegularExpressions;

namespace sapp_sms
{
    public partial class accountdetails : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
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
                        string account_id = "";
                        if (Request.QueryString["accountid"] != null) account_id = Request.QueryString["accountid"];
                        Account account = new Account(constr);
                        account.LoadData(Convert.ToInt32(account_id));
                        LabelAccountID.Text = account.AccountId.ToString();
                        LabelNumber.Text = account.AccountNum;
                        LabelCode.Text = account.AccountCode;
                        ChartMaster chart = new ChartMaster(constr);
                        chart.LoadData(account.AccountId);
                        LabelChart.Text = chart.ChartMasterCode;
                        LabelName.Text = account.AccountName;
                        LabelBank.Text = account.AccountBank;
                        LabelBranch.Text = account.AccountBranch;
                        LabelSwift.Text = account.AccountSwift;
                        TextBoxDescription.Text = account.AccountDescription;
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
        public static string DataGridCommsDataBind(string postdata, string accountid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Account account = new Account(constr);
                account.LoadData(Convert.ToInt32(accountid));
                Comms<Account> comms = new Comms<Account>(constr);
                comms.LoadData(account);
                ArrayList commList = comms.Communications;
                ArrayList rowData = new ArrayList();
                int id = 1;
                foreach (Comm<Account> comm in commList)
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
        public static void DataGridCommsSave(string rowValue, string accountid)
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
                    Account account = new Account(constr);
                    account.LoadData(Convert.ToInt32(accountid));
                    Comm<Account> comm = new Comm<Account>(constr);
                    comm.Update(items, Convert.ToInt32(rowObj["ID"]));
                }
                else
                {
                    Account account = new Account(constr);
                    account.LoadData(Convert.ToInt32(accountid));
                    Comm<Account> comm = new Comm<Account>(constr);
                    comm.Add(items, account);
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
                string account_id = "";
                if (Request.QueryString["accountid"] != null) account_id = Request.QueryString["accountid"];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Account account = new Account(constr);
                account.LoadData(Convert.ToInt32(account_id));
                Comm<Account> comm = new Comm<Account>(constr);
                comm.Delete(Convert.ToInt32(comm_master_id), account);
                Response.BufferOutput = true;
                Response.Redirect("~/accountdetails.aspx?accountid=" + account_id + NewQueryString("&"), false);
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
                string account_id = "";
                if (Request.QueryString["accountid"] != null) account_id = Request.QueryString["accountid"];
                Response.BufferOutput = true;
                Response.Redirect("~/accountedit.aspx?mode=edit&accountid=" + account_id + NewQueryString("&"), false);
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
                string account_id = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Account account = new Account(constr);
                account.Delete(Convert.ToInt32(account_id));
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
                Response.Redirect("~/accounts.aspx" + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        

        


        
    }
}