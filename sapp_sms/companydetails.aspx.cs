using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using System.Configuration;
using Sapp.SMS;
using System.Collections;
using Sapp.Data;
using Sapp.JQuery;

namespace sapp_sms
{
    public partial class companydetails : System.Web.UI.Page, IPostBackEventHandler
    {

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
                        string company_id = "";
                        if (Request.QueryString["companyid"] != null) company_id = Request.QueryString["companyid"];
                        Company company = new Company(constr);
                        company.LoadData(Convert.ToInt32(company_id));
                        LabelBankNum.Text = company.CompanyBankNum;
                        if(company.CompanyFinancialYrBegin.HasValue)
                            LabelFinYrBegin.Text = company.CompanyFinancialYrBegin.Value.ToShortDateString();
                        LabelGST.Text = company.CompanyGst;
                        LabelName.Text = company.CompanyName;
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
        public static string DataGridCommsDataBind(string postdata, string companyid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Company company = new Company(constr);
                company.LoadData(Convert.ToInt32(companyid));
                Comms<Company> comms = new Comms<Company>(constr);
                comms.LoadData(company);
                ArrayList commList = comms.Communications;
                ArrayList rowData = new ArrayList();
                int id = 1;
                foreach (Comm<Company> comm in commList)
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
                CommType ct = new CommType(constr);
                List<CommType> commTypeList = ct.GetCommTypeList();
                foreach (CommType commType in commTypeList)
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
        public static void DataGridCommsSave(string rowValue, string companyid)
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
                items.Add("comm_master_primary", Convert.ToInt32(rowObj["Primary"]));
                #endregion
                if (rowObj["ID"].ToString() != "")
                {
                    Company company = new Company(constr);
                    company.LoadData(Convert.ToInt32(companyid));
                    Comm<Company> comm = new Comm<Company>(constr);
                    comm.Update(items, Convert.ToInt32(rowObj["ID"]));
                }
                else
                {
                    Company company = new Company(constr);
                    company.LoadData(Convert.ToInt32(companyid));
                    Comm<Company> comm = new Comm<Company>(constr);
                    comm.Add(items, company);
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
                string company_id = "";
                if (Request.QueryString["companyid"] != null) company_id = Request.QueryString["companyid"];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Company company = new Company(constr);
                company.LoadData(Convert.ToInt32(company_id));
                Comm<Company> comm = new Comm<Company>(constr);
                comm.Delete(Convert.ToInt32(comm_master_id), company);
                Response.BufferOutput = true;
                Response.Redirect("~/companydetails.aspx?companyid=" + company_id , false);
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
                string company_id = "";
                if (Request.QueryString["companyid"] != null) company_id = Request.QueryString["companyid"];
                Response.BufferOutput = true;
                Response.Redirect("~/companyedit.aspx?mode=edit&companyid=" + company_id , false);
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
                string company_id = Request.QueryString["companyid"];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Company company = new Company(constr);
                company.Delete(Convert.ToInt32(company_id));
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
                Response.Redirect("~/companydetails.aspx?companyid=" + Request.QueryString["companyid"], false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #endregion
    }
}