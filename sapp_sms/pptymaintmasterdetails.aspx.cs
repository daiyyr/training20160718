using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using System.Configuration;
using Sapp.Data;
using Sapp.SMS;
using System.Text.RegularExpressions;

namespace sapp_sms
{
    public partial class pptymaintmasterdetails : System.Web.UI.Page
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
                        string pptymaintmaster_id = "";
                        if (Request.QueryString["pptymaintmasterid"] != null) pptymaintmaster_id = Request.QueryString["pptymaintmasterid"];
                        PptymaintMaster pptymaintmaster = new PptymaintMaster(constr);
                        pptymaintmaster.LoadData(Convert.ToInt32(pptymaintmaster_id));

                        LabelElementID.Text = pptymaintmaster.PptymaintMasterId.ToString();
                        LabelCompliance.Text = pptymaintmaster.PptymaintMasterCompliance.ToString();
                        
                        CreditorMaster creditormaster=new CreditorMaster(constr);
                        creditormaster.LoadData(pptymaintmaster.PptymaintMasterCreditorId);
                        LabelCredit.Text = creditormaster.CreditorMasterName;
                        
                        FreqType freqtype = new FreqType(constr);
                        freqtype.LoadData(pptymaintmaster.PptymaintMasterFreqtypeId);
                        LabelFreqType.Text = freqtype.FreqTypeCode;
                        
                        PropertyMaster propertymaster = new PropertyMaster(constr);
                        propertymaster.LoadData(pptymaintmaster.PptymaintMasterPropertyId);
                        LabelProperty.Text = propertymaster.PropertyMasterCode;
                        
                        PptymaintType pptype=new PptymaintType(constr);
                        pptype.LoadData(pptymaintmaster.PptymaintMasterTypeId);
                        LabelType.Text = pptype.PptymaintTypeName;

                        LabelDue.Text = pptymaintmaster.PptymaintMasterDue.ToShortDateString();
                        LabelNextDue.Text = pptymaintmaster.PptymaintMasterNextDue.ToShortDateString();
                        LabelFreq.Text = pptymaintmaster.PptymaintMasterFreq.ToString();
                        LabelNotes.Text = pptymaintmaster.PptymaintMasterNotes;

                        if (pptymaintmaster.PptymaintMasterUnitId != null)
                        {
                            UnitMaster umaster = new UnitMaster(constr);
                            umaster.LoadData(pptymaintmaster.PptymaintMasterUnitId??0);
                            LabelUnit.Text = umaster.UnitMasterCode;
                        }
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
                pptymaintmaster_id = Request.QueryString["pptymaintmasterid"];
                Response.BufferOutput = true;
                Response.Redirect("~/pptymaintmasteredit.aspx?pptymaintmasterid=" + Server.UrlEncode(pptymaintmaster_id) + "&mode=edit"+NewQueryString("&"),false);
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
                string pptymaintmaster_id = "";
                if (Request.QueryString["pptymaintmasterid"] != null) pptymaintmaster_id = Request.QueryString["pptymaintmasterid"];
                if (pptymaintmaster_id != "")
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    PptymaintMaster pptymaintmaster = new PptymaintMaster(constr);
                    pptymaintmaster.LoadData(Convert.ToInt32(pptymaintmaster_id));
                    pptymaintmaster.Delete();
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
            Response.Redirect("~/pptymaintmaster.aspx" + NewQueryString("?"),false);
        }
    }
}