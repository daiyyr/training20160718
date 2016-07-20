using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Sapp.Common;
using Sapp.Data;
using System.Configuration;
using Sapp.SMS;

namespace sapp_sms
{
    public partial class pptycntrmasterdetails : System.Web.UI.Page
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
            try
            {
                #region Javascript Setup
                Control[] wc = {  };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    #region Load webForm
                    Odbc mydb = null;
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        string pptycntrmaster_id = "";
                        if (Request.QueryString["pptycntrmasterid"] != null) pptycntrmaster_id = Request.QueryString["pptycntrmasterid"];
                        PptycntrMaster pptycntrmaster = new PptycntrMaster(constr);
                        pptycntrmaster.LoadData(Convert.ToInt32(pptycntrmaster_id));

                        LabelElementID.Text = pptycntrmaster.PptycntrMasterId.ToString();
                        
                        if(pptycntrmaster.PptycntrExpiry.HasValue)
                            LabelExpiry.Text = pptycntrmaster.PptycntrExpiry.Value.ToShortDateString();
                        
                        PropertyMaster propertymaster = new PropertyMaster(constr);
                        propertymaster.LoadData(pptycntrmaster.PptycntrMasterPropertyId);
                        LabelProperty.Text = propertymaster.PropertyMasterCode;
                        
                        PptycntrService pservice = new PptycntrService(constr);
                        pservice.LoadData(pptycntrmaster.PptycntrMasterServiceId);
                        LabelService.Text = pservice.PptycntrServiceCode;

                        CreditorMaster creditormaster = new CreditorMaster(constr);
                        creditormaster.LoadData(pptycntrmaster.PptycntrMasterCreditorId);
                        LabelCreditor.Text = creditormaster.CreditorMasterName;

                        LabelInactive.Text = pptycntrmaster.PptycntrInactive ? "True" : "False";
                        LabelNotes.Text = pptycntrmaster.PptycntrNotes;
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
                string pptymaintmaster_id = "";
                pptymaintmaster_id = Request.QueryString["pptycntrmasterid"];
                Response.BufferOutput = true;
                Response.Redirect("~/pptycntrmasteredit.aspx?pptycntrmasterid=" + Server.UrlEncode(pptymaintmaster_id) + "&mode=edit" + NewQueryString("&"),false);
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
                string pptycntrmaster_id = "";
                if (Request.QueryString["pptycntrmasterid"] != null) pptycntrmaster_id = Request.QueryString["pptycntrmasterid"];
                if (pptycntrmaster_id != "")
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    PptycntrMaster pptycntrmaster = new PptycntrMaster(constr);
                    pptycntrmaster.LoadData(Convert.ToInt32(pptycntrmaster_id));
                    pptycntrmaster.Delete();
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
            Response.Redirect("~/pptycntrmaster.aspx" + NewQueryString("?"),false);
        }

        protected void ImageButtonCreditor_Click(object sender, ImageClickEventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string pptycntrmaster_id = "";
            if (Request.QueryString["pptycntrmasterid"] != null) pptycntrmaster_id = Request.QueryString["pptycntrmasterid"];
            PptycntrMaster pptycntrmaster = new PptycntrMaster(constr);
            pptycntrmaster.LoadData(Convert.ToInt32(pptycntrmaster_id));
            string creditormaster_id = pptycntrmaster.PptycntrMasterCreditorId.ToString();
            Response.BufferOutput = true;
            Response.Redirect("~/creditormasterdetails.aspx?creditorid=" + creditormaster_id, false);
        }
    }
}