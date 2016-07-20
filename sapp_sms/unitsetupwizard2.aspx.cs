using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using Sapp.Common;
using Sapp.JQuery;
using Sapp.SMS;
using Sapp.Data;
using Sapp.General;

namespace sapp_sms
{
    public partial class unitsetupwizard2 : System.Web.UI.Page
    {
        private const string temp_type = "unitsetupwizard";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridUnitMaster };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    string propertyid = Request.QueryString["propertyid"];

                    #region Update Total OI and UI
                    LabelTotalOI.Text = GetTotalOI();
                    LabelTotalUI.Text = GetTotalUI();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string jqGridUnitMasterDataBind(string postdata, string propertyid)
        {
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);
            Temp temp = new Temp(constr_general);
            if (temp.TempExist(user.UserId, temp_type))
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                temp.LoadData(user.UserId, temp_type);
                string json = temp.TempData;
                Hashtable items = (Hashtable)JSON.JsonDecode(json);
                ArrayList units = (ArrayList)items["units"];
                DataTable dt = new DataTable("units");
                dt.Columns.Add("ID");
                dt.Columns.Add("Code");
                dt.Columns.Add("Type");
                dt.Columns.Add("Description");
                dt.Columns.Add("Proprietor");
                dt.Columns.Add("AreaSqm");
                dt.Columns.Add("AreaType");
                dt.Columns.Add("OI");
                dt.Columns.Add("UI");

                foreach (Hashtable unit in units)
                {
                    
                    DataRow dr = dt.NewRow();
                    Sapp.SMS.UnitType unittype = new Sapp.SMS.UnitType(constr);
                    unittype.LoadData(Convert.ToInt32(unit["unit_master_type_id"]));
                    if (unittype.UnitTypeCode != "ACCESSORIES" && unit["unit_master_id"].ToString()[0] != 'd')
                    {
                        dr["ID"] = unit["unit_master_id"].ToString();
                        dr["Code"] = unit["unit_master_code"].ToString();
                        dr["Type"] = unittype.UnitTypeCode;
                        dr["Description"] = unit["unit_master_notes"].ToString();
                        string proprietor_id = unit["unit_master_debtor_id"].ToString();
                        if (proprietor_id != "")
                        {
                            DebtorMaster debtor = new DebtorMaster(constr);
                            debtor.LoadData(Convert.ToInt32(proprietor_id));
                            dr["Proprietor"] = debtor.DebtorMasterName;
                        }
                        else
                            dr["Proprietor"] = "";
                        string unit_master_area = unit["unit_master_area"].ToString();
                        if (unit_master_area != "")
                        {
                            dr["AreaSqm"] = unit_master_area;
                        }
                        else
                            dr["AreaSqm"] = "";
                        string unit_master_areatype_id = unit["unit_master_areatype_id"].ToString();
                        if (unit_master_areatype_id != "")
                        {
                            UnitAreaType areatype = new UnitAreaType(constr);
                            areatype.LoadData(Convert.ToInt32(unit_master_areatype_id));
                            dr["AreaType"] = areatype.UnitAreaTypeCode;
                        }
                        else
                            dr["AreaType"] = "";
                        string unit_master_ownership_interest = unit["unit_master_ownership_interest"].ToString();
                        if (unit_master_ownership_interest != "")
                            dr["OI"] = unit["unit_master_ownership_interest"].ToString();
                        else
                            dr["OI"] = "";
                        string unit_master_utility_interest = unit["unit_master_utility_interest"].ToString();
                        if (unit_master_utility_interest != "")
                            dr["UI"] = unit["unit_master_utility_interest"].ToString();
                        else
                            dr["UI"] = "";
                        dt.Rows.Add(dr);
                    }
                }
                string sqlfromstr = "FROM `" + dt.TableName + "`";
                string sqlselectstr = "SELECT `ID`, `Code`, `Type`, `Description`,`Proprietor`, `AreaSqm`, `AreaType`, `OI`, `UI` FROM (SELECT *";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            return "{}";
        }
        [System.Web.Services.WebMethod]
        public static string jqGridUnitMasterFillSubGrid(string postdata, string masterRow)
        {
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);
            Temp temp = new Temp(constr_general);
            if (temp.TempExist(user.UserId, temp_type))
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Object principleObj = JSON.JsonDecode(masterRow);
                Hashtable hdata = (Hashtable)principleObj;
                string principle_id = hdata["ID"].ToString();

                temp.LoadData(user.UserId, temp_type);
                string json = temp.TempData;
                Hashtable items = (Hashtable)JSON.JsonDecode(json);
                ArrayList units = (ArrayList)items["units"];

                ArrayList accessoryUnits = new ArrayList();
                foreach (Hashtable unit in units)
                {
                    if (principle_id[0] == 'u')
                    {
                        if ((unit["unit_master_principal_id"].ToString() == principle_id) || (unit["unit_master_principal_id"].ToString() == principle_id.Substring(1)))
                        {
                            accessoryUnits.Add(unit);
                        }
                    }
                    else if (unit["unit_master_principal_id"].ToString() == principle_id)
                    {
                        accessoryUnits.Add(unit);
                    }
                }
                DataTable dt = new DataTable("units");
                dt.Columns.Add("ID");
                dt.Columns.Add("Code");
                dt.Columns.Add("Type");
                dt.Columns.Add("Description");
                dt.Columns.Add("Proprietor");
                dt.Columns.Add("AreaSqm");
                dt.Columns.Add("AreaType");
                dt.Columns.Add("OI");
                dt.Columns.Add("UI");

                foreach (Hashtable unit in accessoryUnits)
                {
                    DataRow dr = dt.NewRow();
                    if (unit["unit_master_id"].ToString()[0] != 'd')
                    {
                        dr["ID"] = unit["unit_master_id"].ToString();
                        dr["Code"] = unit["unit_master_code"].ToString();
                        Sapp.SMS.UnitType unittype = new Sapp.SMS.UnitType(constr);
                        unittype.LoadData(Convert.ToInt32(unit["unit_master_type_id"]));
                        dr["Type"] = unittype.UnitTypeCode;
                        dr["Description"] = unit["unit_master_notes"].ToString();
                        string proprietor_id = unit["unit_master_debtor_id"].ToString();
                        if (proprietor_id != "")
                        {
                            DebtorMaster debtor = new DebtorMaster(constr);
                            debtor.LoadData(Convert.ToInt32(proprietor_id));
                            dr["Proprietor"] = debtor.DebtorMasterName;
                        }
                        else
                            dr["Proprietor"] = "";
                        string unit_master_area = unit["unit_master_area"].ToString();
                        if (unit_master_area != "")
                        {
                            dr["AreaSqm"] = unit_master_area;
                        }
                        else
                            dr["AreaSqm"] = "";
                        string unit_master_areatype_id = unit["unit_master_areatype_id"].ToString();
                        if (unit_master_areatype_id != "")
                        {
                            UnitAreaType areatype = new UnitAreaType(constr);
                            areatype.LoadData(Convert.ToInt32(unit_master_areatype_id));
                            dr["AreaType"] = areatype.UnitAreaTypeCode;
                        }
                        else
                            dr["AreaType"] = "";
                        string unit_master_ownership_interest = unit["unit_master_ownership_interest"].ToString();
                        if (unit_master_ownership_interest != "")
                            dr["OI"] = unit["unit_master_ownership_interest"].ToString();
                        else
                            dr["OI"] = "";
                        string unit_master_utility_interest = unit["unit_master_utility_interest"].ToString();
                        if (unit_master_utility_interest != "")
                            dr["UI"] = unit["unit_master_utility_interest"].ToString();
                        else
                            dr["UI"] = "";
                        dt.Rows.Add(dr);
                    }
                }
                string sqlfromstr = "FROM `" + dt.TableName + "`";
                string sqlselectstr = "SELECT `ID`, `Code`, `Type`, `Description`,`Proprietor`, `AreaSqm`, `AreaType`, `OI`, `UI` FROM (SELECT *";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            return "{}";
        }
        [System.Web.Services.WebMethod]
        public static string GetTotalOI()
        {
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            decimal total_oi = 0;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);

            Temp temp = new Temp(constr_general);
            if (temp.TempExist(user.UserId, temp_type))
            {
                temp.LoadData(user.UserId, temp_type);
                string json = temp.TempData;
                Hashtable items = (Hashtable)JSON.JsonDecode(json);
                ArrayList units = (ArrayList)items["units"];
                foreach (Hashtable unit in units)
                {
                    if (!string.IsNullOrEmpty(unit["unit_master_ownership_interest"].ToString()) && (unit["unit_master_id"].ToString()[0] != 'd'))
                    {
                        total_oi += Convert.ToDecimal(unit["unit_master_ownership_interest"]);
                    }
                }
            }
            return total_oi.ToString("0.00");
        }
        [System.Web.Services.WebMethod]
        public static string GetTotalUI()
        {
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            decimal total_ui = 0;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);

            Temp temp = new Temp(constr_general);
            if (temp.TempExist(user.UserId, temp_type))
            {
                temp.LoadData(user.UserId, temp_type);
                string json = temp.TempData;
                Hashtable items = (Hashtable)JSON.JsonDecode(json);
                ArrayList units = (ArrayList)items["units"];
                foreach (Hashtable unit in units)
                {
                    if (!string.IsNullOrEmpty(unit["unit_master_utility_interest"].ToString()) && (unit["unit_master_id"].ToString()[0] != 'd'))
                    {
                        total_ui += Convert.ToDecimal(unit["unit_master_utility_interest"]);
                    }
                }
            }
            return total_ui.ToString("0.00");
        }
        #endregion

        #region WebControl Events

        protected void ImageButtonSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                #region Connection Str
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                #endregion
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                UnitTemps unittemps = new UnitTemps(user.UserId, temp_type, constr_general);
                unittemps.LoadData();
                unittemps.Submit(constr);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #endregion

    }
}