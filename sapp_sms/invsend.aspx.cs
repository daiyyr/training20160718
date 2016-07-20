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
    public partial class invsend : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        public string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridTable };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                string property_id = "";
                if (Request.QueryString["propertyid"] != null) property_id = Request.QueryString["propertyid"];
                PropertyMaster property = new PropertyMaster(constr);
                property.LoadData(Convert.ToInt32(property_id));
                LabelTotalOI.Text = property.GetOI().ToString("0.00000");
                LabelTotalUI.Text = property.GetUI().ToString("0.00000");
                decimal oi = decimal.Parse(LabelTotalOI.Text);
                decimal ui = decimal.Parse(LabelTotalUI.Text);
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
                if (args[0] == "Send")
                {
                    Send(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        public void Send(string[] args)
        {
            try
            {
                string jsonStr = args[1];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable jsonObj = (Hashtable)JSON.JsonDecode(jsonStr);
                ArrayList ids = (ArrayList)jsonObj["IDs"];
                string sql = "SELECT   comm_master.comm_master_data, unit_master.unit_master_id, debtor_master.debtor_master_id,                  comm_master.comm_master_type_id, comm_types.comm_type_code, debtor_master.debtor_master_email,                  debtor_master.debtor_master_name, debtor_master.debtor_master_name, unit_master.unit_master_code,                  comm_master.comm_master_primary FROM      comm_master, debtor_comms, debtor_master, unit_master, comm_types WHERE   comm_master.comm_master_id = debtor_comms.debtor_comm_comm_id AND                  debtor_comms.debtor_comm_debtor_id = debtor_master.debtor_master_id AND                  debtor_master.debtor_master_id = unit_master.unit_master_debtor_id AND                  comm_master.comm_master_type_id = comm_types.comm_type_id AND (comm_types.comm_type_code = 'EMAIL') AND                  (comm_master.comm_master_primary = 1) AND (debtor_master.debtor_master_email = 1) ORDER BY comm_master.comm_master_primary DESC, unit_master.unit_master_id DESC ";
                Odbc o = new Odbc(constr);
                DataTable dt = o.ReturnTable(sql, "EmailDT");
                DataTable emailDT = dt.Clone();
                for (int i = 0; i < ids.Count; i++)
                {
                    DataRow dr = ReportDT.GetDataRowByColumn(dt, "unit_master_id", ids[i].ToString());
                    if (dr != null)
                    {
                        string mid=dr["unit_master_id"].ToString();
                        if (ReportDT.GetDataByColumn(emailDT, "unit_master_id", mid, "unit_master_id").Equals(""))
                        {
                            DataRow dr2 = emailDT.NewRow();
                            dr2.ItemArray = dr.ItemArray;
                            emailDT.Rows.Add(dr2);
                        }
                    }
                }
                emailDT = ReportDT.DeleteNullRow(emailDT, "comm_master_data");
                Session.Add("SendEmail", emailDT);
                Response.Redirect("~/invsendemail.aspx",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        [System.Web.Services.WebMethod]
        public static string Select()
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string sql = "SELECT   comm_master.comm_master_data, unit_master.unit_master_id, debtor_master.debtor_master_id,                  comm_master.comm_master_type_id, comm_types.comm_type_code, debtor_master.debtor_master_email,                  debtor_master.debtor_master_name, debtor_master.debtor_master_code, unit_master.unit_master_code,                  comm_master.comm_master_primary FROM      comm_master, debtor_comms, debtor_master, unit_master, comm_types WHERE   comm_master.comm_master_id = debtor_comms.debtor_comm_comm_id AND                  debtor_comms.debtor_comm_debtor_id = debtor_master.debtor_master_id AND                  debtor_master.debtor_master_id = unit_master.unit_master_debtor_id AND                  comm_master.comm_master_type_id = comm_types.comm_type_id AND (comm_types.comm_type_code = 'EMAIL') AND                  (comm_master.comm_master_primary = 1) AND (debtor_master.debtor_master_email = 1) ORDER BY comm_master.comm_master_primary DESC, unit_master.unit_master_id DESC  ";
            Odbc o = new Odbc(constr);
            string ids = "";
            DataTable dt = o.ReturnTable(sql, "Select");
            foreach (DataRow dr in dt.Rows)
            {
                ids += dr["unit_master_code"].ToString() + "|";
            }
            return ids;
        }
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata, string propertyid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                decimal oi = 0;
                decimal ui = 0;
                Odbc mydb = null;
                try
                {
                    mydb = new Odbc(constr);
                    string sql = "SELECT * FROM ((`unit_master` LEFT JOIN `unit_types` ON `unit_master_type_id`=`unit_type_id`)"
                        + " LEFT JOIN `debtor_master` ON `unit_master_debtor_id`=`debtor_master_id`) LEFT JOIN `unit_areatypes` ON `unit_master_areatype_id`=`unit_areatype_id`"
                        + " WHERE `unit_master_property_id`=" + propertyid + " AND `unit_type_code` != 'ACCESSORIES'";
                    DataTable dt = new DataTable("units");
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Code");
                    dt.Columns.Add("DebtorCode");
                    dt.Columns.Add("Type");
                    dt.Columns.Add("Description");
                    dt.Columns.Add("Proprietor");
                    dt.Columns.Add("AreaSqm");
                    dt.Columns.Add("AreaType");
                    dt.Columns.Add("OI");
                    dt.Columns.Add("UI");
                    dt.Columns.Add("DID");
                    OdbcDataReader dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        nr["ID"] = dr["unit_master_id"];
                        nr["Code"] = dr["unit_master_code"];
                        nr["DebtorCode"] = dr["debtor_master_code"];
                        nr["Type"] = dr["unit_type_code"];
                        nr["Description"] = dr["unit_master_notes"];
                        nr["Proprietor"] = dr["debtor_master_name"];
                        nr["AreaSqm"] = dr["unit_master_area"];
                        nr["AreaType"] = dr["unit_areatype_code"];
                        nr["OI"] = dr["unit_master_ownership_interest"];
                        nr["UI"] = dr["unit_master_utility_interest"];
                        nr["DID"] = dr["unit_master_debtor_id"];
                        if (dr["unit_master_ownership_interest"] != DBNull.Value) oi += Convert.ToDecimal(dr["unit_master_ownership_interest"]);
                        if (dr["unit_master_utility_interest"] != DBNull.Value) ui += Convert.ToDecimal(dr["unit_master_utility_interest"]);

                        dt.Rows.Add(nr);
                    }
                    string sqlfromstr = "FROM `" + dt.TableName + "`";
                    string sqlselectstr = "SELECT `ID`, `Code`, `Type`, `Description`,`Proprietor`, `AreaSqm`, `AreaType`, `OI`, `UI` FROM (SELECT *";
                    JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                    HttpContext.Current.Session["UnitMasterDT"] = dt;
                    Hashtable userdata = new Hashtable();
                    userdata.Add("AreaType", "Total:");
                    userdata.Add("OI", oi.ToString("0.00000"));
                    userdata.Add("UI", ui.ToString("0.00000"));
                    jqgridObj.SetUserData(userdata);
                    string jsonStr = jqgridObj.GetJSONStr();
                    return jsonStr;
                }
                finally
                {
                    if (mydb != null) mydb.Close();
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }

    }
}