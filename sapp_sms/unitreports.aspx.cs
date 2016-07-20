using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.Data;
using System.Data.Odbc;

namespace sapp_sms
{
    public partial class unitreports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string property_id = Request.QueryString["propertyid"].ToString();

                TextBoxDateEnd.Text = DateTime.Today.ToString("dd/MM/yyyy");
                TextBoxDateStart.Text = new DateTime(DateTime.Today.AddYears(-1).Year, 12, 1).ToString("dd/MM/yyyy");

                Odbc mydb = null;
                try
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string sql = " SELECT unit_master_id, unit_master_code, debtor_master_name FROM unit_master LEFT JOIN debtor_master ON debtor_master_id = unit_master_debtor_id WHERE unit_master_property_id = " + property_id;

                    mydb = new Odbc(constr);
                    OdbcDataReader dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        ListItem option = new ListItem(dr["unit_master_code"].ToString() + " | " + dr["debtor_master_name"].ToString(), dr["unit_master_id"].ToString());
                        DropDownUnitList.Items.Add(option);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (mydb != null)
                    {
                        mydb.Close();
                    }
                }
            }
        }


        #region WebControl Events
        protected void ImageButtonSubmit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string arguments = "";
                if (Request.QueryString["propertyid"] != null)
                {
                    arguments = Request.QueryString["propertyid"].ToString();
                }

                if (CheckBoxUnitList.Checked == true)
                {
                    Response.Write("<script type='text/javascript'> window.open('reportviewer.aspx?reportid=unitlist&args=" + arguments + "','_blank');  </script>");
                }

                if (CheckBoxProprietorStatement.Checked == true)
                {
                    string bodycorp_id = Request.Cookies["bodycorpid"].Value;
                    string start_date = Convert.ToDateTime(TextBoxDateStart.Text).ToString("dd/MM/yyyy");
                    string end_date = Convert.ToDateTime(TextBoxDateEnd.Text).ToString("dd/MM/yyyy");
                    string unit_id = DropDownUnitList.SelectedValue; ;
                    Response.Write("<script type='text/javascript'> window.open('reportviewer.aspx?reportid=unitstatement&args=" + bodycorp_id + "|" + start_date + "|" + end_date + "|" + unit_id + "','_blank');  </script>");
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