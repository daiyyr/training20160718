using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.SMS;
using Sapp.General;

namespace sapp_sms
{
    public partial class bodycorps : System.Web.UI.Page, IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Request.Cookies["bodycorpid"] != null)
                {
                    Response.Redirect("bodycorpdetails.aspx?bodycorpid=" + Request.Cookies["bodycorpid"].Value, false);
                }
                #region Javascript setup
                Control[] wc = { jqGridBodycorps };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            try
            {
                string[] args = eventArgument.Split('|');
                if (args[0] == "ImageButtonEdit")
                {
                    ImageButtonEdit_Click(args);
                }
                else if (args[0] == "ImageButtonDelete")
                {
                    ImageButtonDelete_Click(args);
                }
                else if (args[0] == "ImageButtonDetails")
                {
                    ImageButtonDetails_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridBodycorpsDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                DataTable dt = new DataTable("bodycorps");
                dt.Columns.Add("ID");
                dt.Columns.Add("Code");
                dt.Columns.Add("Name");
                dt.Columns.Add("Address");
                Odbc mydb = null;
                try
                {
                    mydb = new Odbc(constr);
                    string sql = "SELECT * FROM `bodycorps`";
                    OdbcDataReader dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            string name = dr[i].ToString();
                        }
                        nr["ID"] = dr["bodycorp_id"];
                        nr["Code"] = dr["bodycorp_code"];
                        nr["Name"] = dr["bodycorp_name"];
                        sql = "SELECT `comm_master_data` FROM (`bodycorp_comms` JOIN `bodycorps` ON `bodycorp_id`=`bodycorp_comm_bodycorp_id`)"
                            + " LEFT JOIN `comm_master` ON `bodycorp_comm_comm_id`=`comm_master_id` WHERE `bodycorp_id`=" + Convert.ToInt32(dr["bodycorp_id"])
                            + " AND `comm_master_type_id`=1 ORDER BY `comm_master_order`";
                        string address = "";
                        OdbcDataReader dr2 = mydb.Reader(sql);
                        while (dr2.Read())
                        {
                            address += dr2["comm_master_data"] + ", ";
                        }
                        if (address != "") address = address.Substring(0, address.Length - 2);
                        nr["Address"] = address;
                        dt.Rows.Add(nr);
                    }
                }
                finally
                {
                    if (mydb != null)
                        mydb.Close();
                }

                string sqlfromstr = "FROM `" + dt.TableName + "`";
                string sqlselectstr = "SELECT `ID`, `Code`, `Name`, `Address` FROM (SELECT * ";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        #endregion

        #region WebControl Events
        protected void ImageButtonAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/bodycorpedit.aspx?mode=add", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            string bodycorp_id = "";
            try
            {
                bodycorp_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/bodycorpedit.aspx?mode=edit&bodycorpid=" + bodycorp_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonDelete_Click(string[] args)
        {
            string bodycorp_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                bodycorp_id = args[1];
                Bodycorp bodycorp = new Bodycorp(constr);
                bodycorp.SetLog(user.UserId);
                bodycorp.Delete(Convert.ToInt32(bodycorp_id));
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonDetails_Click(string[] args)
        {
            try
            {
                Session["bodycorpid"] = Request.Cookies["bodycorpid"].Value;
                Response.BufferOutput = true;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}