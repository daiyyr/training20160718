using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Data;
using sapp_sms;
using Sapp.SMS;
using System.Data;
namespace sapp_sms.UseControl
{
    public partial class ChartCodeSearch : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //ChartMaster c = new ChartMaster(AdFunction.conn);
            Odbc o = new Odbc(AdFunction.conn); 
            string sql = "select * from chart_master where chart_master_name like '%"+TextBox1.Text+"%'";
            DataTable dt = o.ReturnTable(sql, "0");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Label1.Text += dr["chart_master_code"].ToString()+"<br>";
                }
                //Label1.Text = Label1.Text.Substring(0, Label1.Text.Length - 1);
            }
           
        }
    }
}