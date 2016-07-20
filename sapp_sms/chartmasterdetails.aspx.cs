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

namespace sapp_sms
{
    public partial class chartmasterdetails : System.Web.UI.Page
    {
        public void CheckBoxFunction(CheckBox c, string t)
        {
            if (!t.Equals("False"))
                c.Checked = true;
            c.Enabled = false;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    #region Load webForm
                    Odbc mydb = null;
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        string chartmaster_id = "";
                        if (Request.QueryString["chartmasterid"] != null) chartmaster_id = Request.QueryString["chartmasterid"];
                        ChartMaster chartmaster = new ChartMaster(constr);
                        chartmaster.LoadData(Convert.ToInt32(chartmaster_id));
                        ChartMaster pchart = new ChartMaster(constr);
                        if (chartmaster.ChartMasterParentID != null)
                        {

                            int id = chartmaster.ChartMasterParentID.Value;
                            pchart.LoadData(id);
                            LabelParent.Text = pchart.ChartMasterName;
                        }
                        if (chartmaster.ChartMasterAccountId != null)
                        {
                            Account account = new Account(constr);
                            account.LoadData(Convert.ToInt32(chartmaster.ChartMasterAccountId));
                            LabelAccount.Text = account.AccountCode;
                        }
                        LiteralClientID.Text = chartmaster.ChartMasterId.ToString();
                        LabelBank.Text = chartmaster.ChartMasterBankAccount.ToString();
                        LabelCode.Text = chartmaster.ChartMasterCode;
                        CheckBoxFunction(CheckBox4, chartmaster.ChartMasterInactive.ToString());
                        LabelName.Text = chartmaster.ChartMasterName;

                        CheckBoxFunction(CheckBox1, chartmaster.ChartMasterNotax.ToString());
                        if (chartmaster.ChartMasterRechargeToId != null)
                        {
                            ChartMaster rechargeto = new ChartMaster(constr);
                            rechargeto.LoadData(chartmaster.ChartMasterRechargeToId.Value);
                            LabelRecharge.Text = rechargeto.ChartMasterCode + " | " + rechargeto.ChartMasterName;
                        }
                        else
                            LabelRecharge.Text = "";
                        CheckBoxFunction(CheckBox3, chartmaster.ChartMasterTrustAccount.ToString());
                        CheckBoxFunction(CheckBox2, chartmaster.ChartMasterLevyBase.ToString());
                        CheckBoxBankAccount.Checked = chartmaster.ChartMasterBankAccount;

                        ChartType charttype = new ChartType(constr);
                        charttype.LoadData(chartmaster.ChartMasterTypeId);
                        LabelType.Text = charttype.ChartTypeName;
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
                string chartmaster_id = "";
                chartmaster_id = Request.QueryString["chartmasterid"];
                Response.BufferOutput = true;
                Response.Redirect("~/chartmasteredit.aspx?chartmasterid=" + Server.UrlEncode(chartmaster_id) + "&mode=edit", false);
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
                string chartmaster_id = "";
                if (Request.QueryString["chartmasterid"] != null) chartmaster_id = Request.QueryString["chartmasterid"];
                if (chartmaster_id != "")
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    ChartMaster chartmaster = new ChartMaster(constr);
                    chartmaster.LoadData(Convert.ToInt32(chartmaster_id));
                    chartmaster.Delete();
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

        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/chartmaster.aspx", false);
        }
    }
}