using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Odbc;
using Sapp.Common;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.SMS;
using System.IO;
namespace sapp_sms
{
    public partial class invsendresult : System.Web.UI.Page
    {
        public string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        public static DataTable InvSendDT;
        protected void Page_Load(object sender, EventArgs e)
        {
            Timer1_Tick(null, null);
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)Session["SendEmail"];
            GridView1.DataSource = dt;
            GridView1.DataBind();
            foreach (GridViewRow gr in GridView1.Rows)
            {
                Image i =(Image) gr.Cells[0].FindControl("Image1");
                string send = gr.Cells[2].Text;
                if (send.Equals("Yes"))
                {
                    i.ImageUrl = "~/images/submit.gif";
                }
            }
        }

    }
}