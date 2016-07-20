using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.General;
using Sapp.Common;
using Sapp.SMS;
using Sapp.General;
namespace sapp_sms
{
    public partial class error : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Error"] != null)
            {
                //Uri u = HttpContext.Current.Request.UrlReferrer;
                if (Session["ErrorUrl"] != null)
                    MessageL.Text += Session["ErrorUrl"].ToString() + "<br><br>";
                MessageL.Text += ((Exception)Session["Error"]).Message;
            }
        }


    }
}
