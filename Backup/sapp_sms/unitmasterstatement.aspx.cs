using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using Sapp.SMS;
using Sapp.Data;
using Sapp.Common;
using Sapp.JQuery;


namespace sapp_sms
{
    public partial class unitmasterstatement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string bodycorp_id = Request.Cookies["bodycorpid"].Value;
            string start_date = Convert.ToDateTime(TextBoxStart.Text).ToString("dd/MM/yyyy");
            string end_date = Convert.ToDateTime(TextBoxEnd.Text).ToString("dd/MM/yyyy");
            string unit_id = Request.QueryString["unitid"].ToString();
            string args = bodycorp_id + "|" + start_date + "|" + end_date + "|" + unit_id;
            Response.Write(SetUrl("unitstatement", args));
        }
        public string SetUrl(string reportid, string args)
        {
            string url = "<script type='text/javascript'> window.open('reportviewer.aspx?reportid=" + reportid + "&args=" + args + "','_blank'); </script>";
            return url;
        }
    }
}
