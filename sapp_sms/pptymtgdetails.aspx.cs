using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.SMS;

namespace sapp_sms
{
    public partial class pptymtgdetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string pptymtg_id = "";
            try
            {
                if (Request.QueryString["pptymtgid"] != null) pptymtg_id = Request.QueryString["pptymtgid"];
                #region Javascript Setup
                #endregion
                if (!IsPostBack)
                {
                        #region Load Page
                        try
                        {
                            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                            if (Request.QueryString["pptymtgid"] != null) pptymtg_id = Request.QueryString["pptymtgid"];
                            Pptymtg pptymtg = new Pptymtg(constr);
                            pptymtg.LoadData(Convert.ToInt32(pptymtg_id));
                            #region Initial Web Controls
                            LabelElementID.Text = pptymtg.PptymtgId.ToString();
                            if(pptymtg.PptymtgDate.HasValue)
                                LabelDate.Text = pptymtg.PptymtgDate.Value.ToShortDateString();
                            if(pptymtg.PptymtgExpiry.HasValue)
                                LabelExpiry.Text = pptymtg.PptymtgExpiry.Value.ToShortDateString();
                            LabelMortgagor.Text = pptymtg.PptymtgMortgagor;
                            LabelPayment.Text = pptymtg.PptymtgPayment.ToString();
                            PropertyMaster pmaster=new PropertyMaster(constr);
                            pmaster.LoadData(pptymtg.PptymtgPropertyId);
                            LabelProperty.Text = pmaster.PropertyMasterCode;
                            LabelPrinciple.Text = pptymtg.PptymtgPrincipal.ToString();
                            LabelRate.Text = pptymtg.PptymtgRate.ToString();
                            LabelTerm.Text = pptymtg.PptymtgTerms;
                            
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
            string pptymtg_id = "";
            pptymtg_id = Request.QueryString["pptymtgid"];
            Response.BufferOutput = true;
            Response.Redirect("~/pptymtgedit.aspx?pptymtgid=" + Server.UrlEncode(pptymtg_id) + "&mode=edit",false);
        }
        protected void ImageButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string pptymtg_id = "";
                if (Request.QueryString["pptymtgid"] != null) pptymtg_id = Request.QueryString["pptymtgid"];
                if (pptymtg_id != "")
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    Pptymtg pptymtg = new Pptymtg(constr);
                    pptymtg.LoadData(Convert.ToInt32(pptymtg_id));
                    pptymtg.Delete();
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
            Response.Redirect("~/pptymtgs.aspx",false);
        }
    }
}