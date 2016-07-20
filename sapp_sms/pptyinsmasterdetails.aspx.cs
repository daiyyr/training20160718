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
    public partial class pptyinsmasterdetails : System.Web.UI.Page
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
                        string pptyinsmaster_id = "";
                        if (Request.QueryString["pptyinsmasterid"] != null) pptyinsmaster_id = Request.QueryString["pptyinsmasterid"];
                        PptyinsMaster pptyinsmaster = new PptyinsMaster(constr);
                        pptyinsmaster.LoadData(Convert.ToInt32(pptyinsmaster_id));

                        LabelElementID.Text = pptyinsmaster.PptyinsMasterId.ToString();

                        ContactMaster cmaster=new ContactMaster(constr);
                        cmaster.LoadData(pptyinsmaster.PptyinsMasterBrokerId);
                        LabelBroker.Text = cmaster.ContactMasterName;

                        CreditorMaster creditormaster = new CreditorMaster(constr);
                        creditormaster.LoadData(pptyinsmaster.PptyinsMasterUnderwriterId);
                        LabelUnderWriter.Text = creditormaster.CreditorMasterCode;

                        PropertyMaster pmaster = new PropertyMaster(constr);
                        pmaster.LoadData(pptyinsmaster.PptyinsMasterPropertyId);
                        LabelProperty.Text = pmaster.PropertyMasterCode;

                        PptyinsType ptype = new PptyinsType(constr);
                        ptype.LoadData(pptyinsmaster.PptyinsMasterTypeId);
                        LabelType.Text = ptype.PptyinsTypeCode;

                        LabelCover.Text = pptyinsmaster.PptyinsMasterCover;
                        if(pptyinsmaster.PptyinsMasterEnd.HasValue)
                            LabelEnd.Text = pptyinsmaster.PptyinsMasterEnd.Value.ToShortDateString();
                        LabelExcess.Text = pptyinsmaster.PptyinsMasterExcess.ToString();
                        LabelGST.Text = pptyinsmaster.PptyinsMasterGst;
                        LabelInsvt.Text = pptyinsmaster.PptyinsMasterInsvt.ToString();
                        LabelNotes.Text = pptyinsmaster.PptyinsMasterNotes;
                        LabelPlacement.Text = pptyinsmaster.PptyinsMasterPlacement;
                        LabelPolicyNum.Text = pptyinsmaster.PptyinsMasterPolicyNum;
                        LabelPremium.Text = pptyinsmaster.PptyinsMasterPremium.ToString();
                        if(pptyinsmaster.PptyinsMasterStart.HasValue)
                            LabelStart.Text = pptyinsmaster.PptyinsMasterStart.Value.ToShortDateString();
                        LabelNotes.Text = pptyinsmaster.PptyinsMasterNotes;
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
                string pptyinsmaster_id = "";
                pptyinsmaster_id = Request.QueryString["pptyinsmasterid"];
                Response.BufferOutput = true;
                Response.Redirect("~/pptyinsmasteredit.aspx?pptyinsmasterid=" + Server.UrlEncode(pptyinsmaster_id) + "&mode=edit" + NewQueryString("&"),false);
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
                string pptyinsmaster_id = "";
                if (Request.QueryString["pptyinsmasterid"] != null) pptyinsmaster_id = Request.QueryString["pptyinsmasterid"];
                if (pptyinsmaster_id != "")
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    PptyinsMaster pptyinsmaster = new PptyinsMaster(constr);
                    pptyinsmaster.LoadData(Convert.ToInt32(pptyinsmaster_id));
                    pptyinsmaster.Delete();
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
            Response.Redirect("~/pptyinsmaster.aspx" + NewQueryString("?"),false);
        }
    }
}