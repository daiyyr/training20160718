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
    public partial class pptytitledetails : System.Web.UI.Page
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
            string pptytitle_id = "";
            try
            {
                if (Request.QueryString["pptytitleid"] != null) pptytitle_id = Request.QueryString["pptytitleid"];
                #region Javascript Setup
                #endregion
                if (!IsPostBack)
                {
                    #region Load Page
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        if (Request.QueryString["pptytitleid"] != null) pptytitle_id = Request.QueryString["pptytitleid"];
                        Pptytitle pptytitle = new Pptytitle(constr);
                        pptytitle.LoadData(Convert.ToInt32(pptytitle_id));
                        #region Initial Web Controls
                        LabelElementID.Text = pptytitle.PptytitleId.ToString();
                        LabelArea.Text = pptytitle.PptytitleArea.ToString();
                        Authority authority=new Authority(constr);
                        authority.LoadData(pptytitle.PptytitleAuthorityId);
                        LabelAuthority.Text = authority.AuthorityCode;
                        LabelCtreference.Text = pptytitle.PptytitleCtreference;
                        LabelDplan.Text = pptytitle.PptytitleDplan;
                        Labellot.Text = pptytitle.PptytitleLot;
                        LabelNote.Text = pptytitle.PptytitleNotes;
                        PropertyMaster pmaster=new PropertyMaster(constr);
                        pmaster.LoadData(pptytitle.PptytitlePropertyId);
                        LabelProperty.Text = pmaster.PropertyMasterCode;
                        LabelZone.Text = pptytitle.PptytitleZoneId.ToString();
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
            string pptytitle_id = "";
            pptytitle_id = Request.QueryString["pptytitleid"];
            Response.BufferOutput = true;
            Response.Redirect("~/pptytitleedit.aspx?pptytitleid=" + Server.UrlEncode(pptytitle_id) + "&mode=edit" + NewQueryString("&"),false);
        }
        protected void ImageButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string pptytitle_id = "";
                if (Request.QueryString["pptytitleid"] != null) pptytitle_id = Request.QueryString["pptytitleid"];
                if (pptytitle_id != "")
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    Pptytitle pptytitle = new Pptytitle(constr);
                    pptytitle.LoadData(Convert.ToInt32(pptytitle_id));
                    pptytitle.Delete();
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
            Response.Redirect("~/pptytitles.aspx" + NewQueryString("?"),false);
        }
    }
}