using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace sapp_sms
{
    public partial class unitreports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #region WebControl Events
        protected void ImageButtonSubmit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string radioButtonValue = RadioButtonList01.SelectedValue;
                string arguments = "";
                if (Request.QueryString["propertyid"] != null)
                    arguments = Request.QueryString["propertyid"].ToString();
                if (radioButtonValue == "Unit List Report")
                {
                    Response.Write("<script type='text/javascript'> window.open('reportviewer.aspx?reportid=unitlist&args=" + arguments + "','_blank');  </script>");
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}