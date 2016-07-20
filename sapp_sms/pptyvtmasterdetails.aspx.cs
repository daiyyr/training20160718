using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.SMS;
using System.Text.RegularExpressions;

namespace sapp_sms
{
    public partial class pptyvtmasterdetails : System.Web.UI.Page
    {
        private string CheckedQueryString()
        {
            if (null != Request.QueryString["propertyid"] && Regex.IsMatch(Request.QueryString["propertyid"], "^[0-9]*$"))
            {
                return Request.QueryString["propertyid"];
            }
            return "";
        }
        private string NewQueryString(string connector)
        {
            string result = CheckedQueryString();
            return string.IsNullOrEmpty(result) ? "" : connector + "propertyid=" + result;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string pptyvtmaster_id = "";
            try
            {
                if (Request.QueryString["pptyvtmasterid"] != null) pptyvtmaster_id = Request.QueryString["pptyvtmasterid"];
                #region Javascript Setup
                #endregion
                if (!IsPostBack)
                {
                    #region Load Page
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        if (Request.QueryString["pptyvtmasterid"] != null) pptyvtmaster_id = Request.QueryString["pptyvtmasterid"];
                        PptyvtMaster pptyvtmaster = new PptyvtMaster(constr);
                        pptyvtmaster.LoadData(Convert.ToInt32(pptyvtmaster_id));
                        #region Initial Web Controls
                        if (pptyvtmaster.PptyvtMasterDate.HasValue)
                        {
                            LabelDate.Text = pptyvtmaster.PptyvtMasterDate.Value.ToShortDateString();
                        }
                        LabelElementID.Text = pptyvtmaster.PptyvtMasterId.ToString();
                        LabelDemolition.Text = pptyvtmaster.PptyvtMasterDemolition.ToString();
                        LabelFee.Text = pptyvtmaster.PptyvtMasterFee.ToString();
                        LabelGST.Text = pptyvtmaster.PptyvtMasterGst.ToString();
                        LabelInflation.Text = pptyvtmaster.PptyvtMasterInflation.ToString();
                        LabelNotes.Text = pptyvtmaster.PptyvtMasterNotes;
                        PropertyMaster pmaster=new PropertyMaster(constr);
                        pmaster.LoadData(pptyvtmaster.PptyvtMasterPropertyId);
                        LabelProperty.Text = pmaster.PropertyMasterCode;
                        LabelRef.Text = pptyvtmaster.PptyvtMasterRef.ToString();
                        LabelReinstatement.Text = pptyvtmaster.PptyvtMasterReinstatement.ToString();
                        if(pptyvtmaster.PptyvtMasterReplacement.HasValue)
                            LabelReplacement.Text = pptyvtmaster.PptyvtMasterReplacement.Value.ToString();
                        if(pptyvtmaster.PptyvtMasterTypeId!=null)
                        {
                            PptyvtType ptype=new PptyvtType(constr);
                            ptype.LoadData(pptyvtmaster.PptyvtMasterTypeId.Value);
                            LabelType.Text = ptype.PptyvtTypeCode;
                        }
                        ContactMaster cmaster=new ContactMaster(constr);
                        cmaster.LoadData(pptyvtmaster.PptyvtMasterValuerId);
                        Labelvaluer.Text = cmaster.ContactMasterName;
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
            string pptyvtmaster_id = "";
            pptyvtmaster_id = Request.QueryString["pptyvtmasterid"];
            Response.BufferOutput = true;
            Response.Redirect("~/pptyvtmasteredit.aspx?pptyvtmasterid=" + Server.UrlEncode(pptyvtmaster_id) + "&mode=edit"+NewQueryString("&"),false);
        }
        protected void ImageButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string pptyvtmaster_id = "";
                if (Request.QueryString["pptyvtmasterid"] != null) pptyvtmaster_id = Request.QueryString["pptyvtmasterid"];
                if (pptyvtmaster_id != "")
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    PptyvtMaster pptyvtmaster = new PptyvtMaster(constr);
                    pptyvtmaster.LoadData(Convert.ToInt32(pptyvtmaster_id));
                    pptyvtmaster.Delete();
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
            Response.Redirect("~/pptyvtmaster.aspx" + NewQueryString("?"),false);
        }
    }
}