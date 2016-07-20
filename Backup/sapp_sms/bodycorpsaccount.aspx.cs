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
    public partial class bodycorpsaccount : System.Web.UI.Page
    {
        public string bid = "";
        string sms = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string portal = ConfigurationManager.ConnectionStrings["constr3"].ConnectionString;
        public DataTable GetUnit(string pid)
        {
            DataTable dt = new DataTable();
            string sql = "select * from unit_master  where unit_master_property_id = " + pid;
            Odbc o = new Odbc(sms);
            dt = o.ReturnTable(sql, "Unit");
            return dt;
        }
        public DataTable GetExAccount()
        {
            DataTable dt = new DataTable();
            string sql = "select * from logins";
            Odbc o = new Odbc(portal);
            dt = o.ReturnTable(sql, "ExAccount");
            return dt;
        }
        public DataTable GetBodycorp()
        {
            DataTable dt = new DataTable();
            string sql = "select * from property_master where property_master_bodycorp_id  = " + bid;
            Odbc o = new Odbc(sms);
            dt = o.ReturnTable(sql, "Unit");
            return dt;
        }
        public DataTable MakeList()
        {
            DataTable result = new DataTable("ResultDT");
            result.Columns.Add("Num");
            result.Columns.Add("UnitNum");
            result.Columns.Add("BcNum");
            result.Columns.Add("Password");
            result.Columns.Add("EnPassword");
            DataTable b = GetBodycorp();
            DataTable account = GetExAccount();
            foreach (DataRow dr in b.Rows)
            {
                string id = dr["property_master_id"].ToString();
                DataTable unit = GetUnit(id);
                foreach (DataRow dr2 in unit.Rows)
                {
                    int i = 1;
                    account.DefaultView.RowFilter = " login_bodycorp_code = '" + dr["property_master_code"].ToString() + "' and login_unit_code = '" + dr2["unit_master_code"].ToString() + "'";
                    bool check = false;
                    string password = "";
                    foreach (DataRow a in account.Rows)
                    {

                        string bcnum = a["login_bodycorp_code"].ToString();
                        string unitnum = a["login_unit_code"].ToString();
                        if (bcnum.Equals(dr["property_master_code"].ToString()) && unitnum.Equals(dr2["unit_master_code"].ToString()))
                        {
                            password = Crypto.DecryptStringAES(a["login_password"].ToString(), "user");
                            check = true;
                        }
                    }
                    if (check)
                    {
                        DataRow newRow = result.NewRow();
                        newRow["Num"] = i.ToString();
                        newRow["BcNum"] = dr["property_master_code"].ToString();
                        newRow["UnitNum"] = dr2["unit_master_code"].ToString();
                        newRow["Password"] = password;
                        //newRow["EnPassword"] = Crypto.EncryptStringAES(p, "user");
                        result.Rows.Add(newRow);
                        i++;
                    }
                    else
                    {
                        DataRow newRow = result.NewRow();
                        string p = GetRandomPassword(8);
                        newRow["Num"] = i.ToString();
                        newRow["BcNum"] = dr["property_master_code"].ToString();
                        newRow["UnitNum"] = dr2["unit_master_code"].ToString();
                        newRow["Password"] = p;
                        newRow["EnPassword"] = Crypto.EncryptStringAES(p, "user");
                        result.Rows.Add(newRow);
                        InsertRows(newRow["BcNum"].ToString(), newRow["UnitNum"].ToString(), newRow["EnPassword"].ToString());
                        i++;
                    }
                }
            }
            return result;
        }
        public void InsertRows(string bc, string unit, string password)
        {
            Odbc o = new Odbc(portal);
            Hashtable items = new Hashtable();
            items.Add("login_bodycorp_code", "'" + bc + "'");
            items.Add("login_unit_code", "'" + unit + "'");
            items.Add("login_password", "'" + password + "'");
            o.ExecuteInsert("logins", items);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bid = Request.Cookies["bodycorpid"].Value;
                Session["AccountList"] = MakeList();
            }
        }


        public string GetRandomPassword(int passwordLen)
        {
            string randomChars = "AaBvCcDdFfGgHhJjKkMmLPpQqRrTtVWXY23467819";
            string password = string.Empty;
            int randomNum;
            Random random = new Random();
            for (int i = 0; i < passwordLen; i++)
            {
                randomNum = random.Next(randomChars.Length);
                password += randomChars[randomNum];
            }
            return password;
        }
        [System.Web.Services.WebMethod]
        public static string DataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            DataTable dt = (DataTable)HttpContext.Current.Session["AccountList"];
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `Num`, `BcNum`, `UnitNum`, `Password` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }

        protected void ImageButtonLevies_Click(object sender, ImageClickEventArgs e)
        {
            if (Session["AccountList"] == null)
                Session["AccountList"] = MakeList();
            string bodycorp_id = Request.Cookies["bodycorpid"].Value;
            Response.Redirect("~/reportviewer.aspx?reportid=unitaccount&args=" + Server.UrlEncode(bodycorp_id),false);
        }
    }
}