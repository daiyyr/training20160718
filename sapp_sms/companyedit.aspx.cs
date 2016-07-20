using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.SMS;
using System.Collections;
using Sapp.Data;

namespace sapp_sms
{
    public partial class companyedit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string company_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["companyid"] != null) company_id = Request.QueryString["companyid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                if (!IsPostBack)
                {
                    if (mode == "edit")
                    {
                        #region Load Page
                        try
                        {
                            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                            #region Load Web Controls
                            Company company = new Company(constr);
                            company.LoadData(Convert.ToInt32(company_id));

                            if (company.CompanyFinancialYrBegin.HasValue)
                                TextBoxFinancialYrBegin.Text = company.CompanyFinancialYrBegin.Value.ToShortDateString();
                            TextBoxBankNum.Text = company.CompanyBankNum;
                            TextBoxGST.Text = company.CompanyGst;
                            TextBoxName.Text = company.CompanyName;
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #region WebControl Events
        protected void ImageButtonSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string company_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (!Page.IsValid) return;
                if (Request.QueryString["companyid"] != null) company_id = Request.QueryString["companyid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];

                #region Retireve Values

                Hashtable items = new Hashtable();

                items.Add("company_name", DBSafeUtils.StrToQuoteSQL(TextBoxName.Text));
                items.Add("company_gst", DBSafeUtils.StrToQuoteSQL(TextBoxGST.Text));
                items.Add("company_bank_num", DBSafeUtils.StrToQuoteSQL(TextBoxBankNum.Text));
                items.Add("company_financial_yr_begin", DBSafeUtils.DateTimeToSQL(TextBoxFinancialYrBegin.Text));

                #endregion
                if (mode == "edit")
                {
                    #region Save
                    Company company = new Company(constr);
                    company.Update(items, Convert.ToInt32(company_id));
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                    #endregion
                }
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
                Response.Redirect("~/home.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

    }
}