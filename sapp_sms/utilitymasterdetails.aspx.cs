using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.SMS;
using Sapp.Data;

namespace sapp_sms
{
    public partial class utilitymasterdetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string utilitymaster_id = "";
            try
            {
                if (Request.QueryString["utilitymasterid"] != null) utilitymaster_id = Request.QueryString["utilitymasterid"];
                #region Javascript Setup
                #endregion
                if (!IsPostBack)
                {
                    #region Load Page
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        if (Request.QueryString["utilitymasterid"] != null) utilitymaster_id = Request.QueryString["utilitymasterid"];
                        UtilityMaster utilitymaster = new UtilityMaster(constr);
                        utilitymaster.LoadData(Convert.ToInt32(utilitymaster_id));
                        #region Initial Web Controls
                        LabelElementID.Text = utilitymaster.UtilityMasterId.ToString();
                        LabelBatch.Text = utilitymaster.UtilityMasterBatchId.ToString();
                        LabelDate.Text = utilitymaster.UtilityMasterDate.ToShortDateString();
                        LabelNotes.Text = utilitymaster.UtilityMasterNotes.ToString();
                        LabelReading.Text = utilitymaster.UtilityMasterReading.ToString();
                        UtilityType utype=new UtilityType(constr);
                        utype.LoadData(utilitymaster.UtilityMasterTypeId);
                        LabelType.Text = utype.UtilityTypeCode;
                        UnitMaster umaster = new UnitMaster(constr);
                        umaster.LoadData(utilitymaster.UtilityMasterUnitId);
                        LabelUnit.Text = utype.UtilityTypeCode;
                        LabelUnitPrice.Text = utilitymaster.UtilityMasterUnitPrice.ToString();
                        #endregion
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

        protected void ImageButtonEdit_Click(object sender, ImageClickEventArgs e)
        {
            string utilitymaster_id = "";
            utilitymaster_id = Request.QueryString["utilitymasterid"];
            Response.BufferOutput = true;
            Response.Redirect("~/utilitymasteredit.aspx?utilitymasterid=" + Server.UrlEncode(utilitymaster_id) + "&mode=edit",false);
        }
        protected void ImageButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string utilitymaster_id = "";
                if (Request.QueryString["utilitymasterid"] != null) utilitymaster_id = Request.QueryString["utilitymasterid"];
                if (utilitymaster_id != "")
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    UtilityMaster utilitymaster = new UtilityMaster(constr);
                    utilitymaster.LoadData(Convert.ToInt32(utilitymaster_id));
                    utilitymaster.Delete();
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/utilitymaster.aspx",false);
        }
    }
}