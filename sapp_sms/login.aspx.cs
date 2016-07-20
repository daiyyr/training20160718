using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.Odbc;
using System.Web.Security;
using Sapp.Data;
using Sapp.Common;
using Sapp.General;
using Sapp.SMS;
using System.Data;
namespace sapp_sms
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Odbc mydb = new Odbc(AdFunction.conn);
                DataTable dt = mydb.ReturnTable("SELECT * FROM `bodycorps` ORDER BY bodycorp_code", "bb");
                dt.Columns.Add("Code");
                foreach (DataRow dr in dt.Rows)
                {
                    dr["Code"] = dr["bodycorp_code"].ToString() + " - " + dr["bodycorp_name"].ToString();
                }
                DropDownList1.DataSource = dt;
                DropDownList1.DataTextField = "Code";
                DropDownList1.DataValueField = "bodycorp_id";
                DropDownList1.DataBind();
            }
        }

        protected void ButtonLogin_Click(object sender, EventArgs e)
        {
            Odbc mydb = null;
            try
            {

                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                User user = new Sapp.General.User(constr);
                if (user.Authenticated(TextBoxLogin.Text, TextBoxPassword.Text))
                {
                    FormsAuthentication.RedirectFromLoginPage(TextBoxLogin.Text, true);
                    HttpCookie c = new HttpCookie("bodycorpid", DropDownList1.SelectedValue);
                    Response.Cookies.Add(c);
                    HttpCookie c1 = new HttpCookie("bodycorpcode", DropDownList1.SelectedItem.Text);
                    Response.Cookies.Add(c1);
                    Response.Redirect("bodycorpdetails.aspx?bodycorpid=" + DropDownList1.SelectedValue, false);
                }
                else
                    HttpContext.Current.Response.Write("Invalid Login");
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }
    }
}