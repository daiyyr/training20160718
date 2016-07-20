using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Text;
using Sapp.Common;
using Sapp.SMS;
using Sapp.JQuery;
using System.Data;
using Sapp.Data;

namespace sapp_sms
{
    public partial class bodyInterestCloseDate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void ButtonInsert_Click(object sender, EventArgs e)
        {
            try
            {
     

                string bid = AdFunction.GetQueryString("bid");
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new Sapp.General.User(AdFunction.constr_general);
                user.LoadData(login);
                Bodycorp bodycorp = new Bodycorp(AdFunction.conn);
                bodycorp.SetLog(user.UserId);
                Hashtable items = new Hashtable();
                items.Add("bodycorp_close_period_date", DBSafeUtils.DateToSQL(Date_T.Text));
                bodycorp.Update(items, Convert.ToInt32(bid));
                Response.BufferOutput = true;
                //items.Add("bodycorp_close_period_date", DBSafeUtils.DateToSQL(TextBoxClosePeriodDate.Text));
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.returnValue = 'refresh'; window.close();", true);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }









    }
}