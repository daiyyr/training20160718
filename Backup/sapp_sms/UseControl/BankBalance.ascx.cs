using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Data;
using Sapp.SMS;
using System.Data;
namespace sapp_sms.UseControl
{
    public partial class BankBalance : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Odbc o = new Odbc(AdFunction.conn);
                DataTable dt = o.ReturnTable("select bodycorp_account_id from bodycorps where bodycorp_id=" + Request.Cookies["bodycorpid"].Value, "t1");
                string cid = dt.Rows[0][0].ToString();

                dt = o.ReturnTable("select sum(gl_transaction_net) from gl_transactions where gl_transaction_chart_id=" + cid, "t2");
                decimal result = 0;
                decimal.TryParse(dt.Rows[0][0].ToString(), out result);
                result = -result;
                BalanceL.Text = "$" + result.ToString("###,###");
            }
        }
    }
}