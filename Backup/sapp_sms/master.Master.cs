using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Configuration;
using Sapp.Common;
using Sapp.General;
using Sapp.Data;
using Sapp.SMS;
using System.Data;
namespace sapp_sms
{
    public partial class master : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
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
                    DropDownList1.SelectedValue = Request.Cookies["bodycorpid"].Value;
                }
                if (Request.IsAuthenticated)
                {

                    #region Javascript setup
                    Control[] wc = { message_board, WelcomeBackMessage };
                    JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                    WelcomeBackMessage.Text = "Welcome back!" + HttpContext.Current.User.Identity.Name;
                    AuthenticatedMessagePanel.Visible = true;
                    AnonymousMessagePanel.Visible = false;
                    CreateMenu();
                    FormAuthentication();
                    CreateMessageBoard();
                    #endregion
                    #region Load Task Message

                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(constr_general);
                    user.LoadData(login);
                    Odbc mydb = null;
                    try
                    {
                        mydb = new Odbc(constr);
                        string sql = "SELECT COUNT(*) FROM `bodycorp_managers` LEFT JOIN `invoice_master` ON `bodycorp_manager_bodycorp_id`=`invoice_master_bodycorp_id`"
                            + " WHERE `bodycorp_manager_user_id`=" + user.UserId + " AND `invoice_master_sent`=0 AND `invoice_master_date`<=" + DBSafeUtils.DateToSQL(DateTime.Today);
                        object amount = mydb.ExecuteScalar(sql);
                        if (Convert.ToInt32(amount) > 0)
                        {
                            LiteralTask.Text = "<b>Unsend Invoice: </b>" + amount.ToString();
                        }
                        else
                            LiteralTask.Text = "";
                    }
                    finally
                    {
                        if (mydb != null) mydb.Close();
                    }
                    #endregion
                }
                else
                {
                    AuthenticatedMessagePanel.Visible = false;
                    AnonymousMessagePanel.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("~/login.aspx");
            }
        }

        #region User Functions
        protected void logout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("~/login.aspx", false);
        }

        protected void CreateMenu()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string login = HttpContext.Current.User.Identity.Name;
                Menus menus = new Menus(constr);
                menus.LoadData();
                string html = menus.GetHtmlStr(login);
                menu.InnerHtml = html;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void FormAuthentication()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                Forms forms = new Forms(constr);
                forms.LoadData();
                int? form_id = forms.FindFormId(Request.Url.ToString());
                if (form_id == null)
                    throw new Exception("Error: unautherised url!");
                string login = HttpContext.Current.User.Identity.Name;
                User user = new User(constr);
                if (!user.FormAuthentication(login, form_id.Value))
                {
                    //Response.BufferOutput = true;
                    //Response.Redirect("~/warning.aspx", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void CreateMessageBoard()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                Messages messages = new Messages(constr);
                string login = HttpContext.Current.User.Identity.Name;
                User user = new User(constr);
                user.LoadData(login);
                messages.LoadUserMessage(user.UserId);
                message_board.InnerHtml = messages.GetHtmlStr();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpCookie c = new HttpCookie("bodycorpid", DropDownList1.SelectedValue);
            Response.Cookies.Add(c);
            HttpCookie c1 = new HttpCookie("bodycorpcode", DropDownList1.SelectedItem.Text);
            Response.Cookies.Add(c1);
            Response.Redirect("bodycorpdetails.aspx?bodycorpid=" + DropDownList1.SelectedValue, false);
        }
    }
}