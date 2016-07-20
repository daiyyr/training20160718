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
    public partial class unitsetupwizard : System.Web.UI.Page, IPostBackEventHandler
    {
        private const string temp_type = "unitsetupwizard";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridUnitPlan, LabelTotalOI, LabelTotalUI };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                
                #endregion
                if (!IsPostBack)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                    string propertyid = Request.QueryString["propertyid"];
                    //Session["propertyid"] = Request.QueryString["propertyid"].ToString();
                    #region Create Temp File In Mysql
                    Temp temp = new Temp(constr_general);
                    Hashtable items = new Hashtable();
                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(constr_general);
                    user.LoadData(login);
                    items.Add("temp_user_id", user.UserId);
                    items.Add("temp_type", DBSafeUtils.StrToQuoteSQL(temp_type));
                    PropertyMaster property = new PropertyMaster(constr);
                    property.LoadData(Convert.ToInt32(propertyid));
                    items.Add("temp_data", DBSafeUtils.StrToQuoteSQL(property.GetJSONUnits("")));
                    items.Add("temp_date", DBSafeUtils.DateToSQL(DateTime.Today));
                    items.Add("temp_expire", DBSafeUtils.DateToSQL(DateTime.Today.AddDays(1)));
                    if (!temp.TempExist(Convert.ToInt32(items["temp_user_id"]), temp_type))
                    {
                        temp.Add(items);
                    }
                    else
                    {
                        temp.LoadData(Convert.ToInt32(items["temp_user_id"]), temp_type);
                        string json = temp.TempData;
                        Hashtable temp_data = (Hashtable)JSON.JsonDecode(json);
                        if (temp_data["property_id"].ToString() != propertyid)
                        {
                            temp.Delete();
                            temp.Add(items);
                        }
                    }
                    #endregion
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

        public void RaisePostBackEvent(string eventArgument)
        {
            try
            {
                string[] args = eventArgument.Split('|');
                if (args[0] == "ImageButtonDelete")
                {
                    ImageButtonDelete_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebControl Events
        private void ImageButtonDelete_Click(string[] args)
        {
            try
            {
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string unit_id = args[1];
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                UnitTemps unitTemps = new UnitTemps(user.UserId, temp_type, constr_general, constr);
                unitTemps.LoadData();
                unitTemps.DeleteUnitTemp(unit_id);
                unitTemps.UpdateTemp();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ImageButtonNext_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string propertyid = Request.QueryString["propertyid"].ToString();
                Response.Redirect("~/unitsetupwizard2.aspx?propertyid=" + propertyid,false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string UnitPlanDataBind(string postdata)
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
                dt.Columns.Add("Principle");
                dt.Columns.Add("Description");
                dt.Columns.Add("Proprietor");
                dt.Columns.Add("AreaSqm");
                dt.Columns.Add("AreaType");
                dt.Columns.Add("OI");
                dt.Columns.Add("UI");
                dt.Columns.Add("SpecialScale");
                dt.Columns.Add("Committee");

                foreach (Hashtable unit in units)
                {
                    DataRow dr = dt.NewRow();
                    if (unit["unit_master_id"].ToString()[0] != 'd')
                    {
                        dr["ID"] = unit["unit_master_id"].ToString();
                        dr["Code"] = unit["unit_master_code"].ToString();
                        Sapp.SMS.UnitType unittype = new Sapp.SMS.UnitType(constr);
                        unittype.LoadData(Convert.ToInt32(unit["unit_master_type_id"]));
                        dr["Type"] = unittype.UnitTypeCode;
                        string principal_id = unit["unit_master_principal_id"].ToString();
                        if (!string.IsNullOrEmpty(principal_id))
                        {
                            foreach (Hashtable _unit in units)
                            {
                                if (_unit["unit_master_id"].ToString() == principal_id)
                                    dr["Principle"] = _unit["unit_master_code"].ToString();
                            }
                        }
                        else
                            dr["Principle"] = "";
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
                        string unit_master_special_scale = unit["unit_master_special_scale"].ToString();
                        if (unit_master_special_scale != "")
                            dr["SpecialScale"] = unit["unit_master_special_scale"].ToString();
                        else
                            dr["SpecialScale"] = "";
                        string unit_master_committee = unit["unit_master_committee"].ToString();
                        if (unit_master_committee != "")
                        {
                            if (unit["unit_master_committee"].ToString() == "1")
                                dr["Committee"] = "true";
                            else if (unit["unit_master_committee"].ToString() == "0")
                                dr["Committee"] = "false";
                        }
                        else
                            dr["Committee"] = "";
                        dt.Rows.Add(dr);
                    }
                }
                string sqlfromstr = "FROM `" + dt.TableName + "`";
                string sqlselectstr = "SELECT `ID`, `Code`, `Type`, `Principle`, `Description`,`Proprietor`, `AreaSqm`, `AreaType`, `OI`, `UI`, `SpecialScale`, `Committee` FROM (SELECT *";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            return "{}";
        }
        [System.Web.Services.WebMethod]
        public static string UnitPlanSaveData(string rowValue)
        {
            try
            {
                #region Connection Str
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                #endregion
                #region Define Variables
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);

                Object c = JSON.JsonDecode(rowValue);
                Hashtable hdata = (Hashtable)c;

                string line_id = hdata["ID"].ToString();
                #endregion
                Temp temp = new Temp(constr_general);
                if (temp.TempExist(user.UserId, temp_type))
                {
                    #region Define Variables
                    temp.LoadData(user.UserId, temp_type);
                    string json = temp.TempData;
                    Hashtable items = (Hashtable)JSON.JsonDecode(json);
                    ArrayList units = (ArrayList)items["units"];

                    Sapp.SMS.UnitType unittype = new Sapp.SMS.UnitType(constr);
                    string unittypeid = unittype.GetIDByCode(hdata["Type"].ToString()).ToString();
                    string principleid = "";
                    if (hdata["Principle"].ToString() != "Null")
                    {
                        foreach (Hashtable unit in units)
                        {
                            if (unit["unit_master_code"].ToString() == hdata["Principle"].ToString())
                            {
                                principleid = unit["unit_master_id"].ToString();
                                break;
                            }
                        }
                    }
                    else
                        principleid = "";
                    UnitAreaType areatype = new UnitAreaType(constr);
                    string areatypeid = areatype.GetIDByCode(hdata["AreaType"].ToString()).ToString();
                    #endregion
                    Hashtable line = new Hashtable();
                    #region Insert or Update Line
                    if (line_id == "")
                    {
                        // if insert
                        line.Add("unit_master_id", "i" + Guid.NewGuid().ToString("N")); // Create a unique id for insert unit
                        line.Add("unit_master_code", hdata["Code"].ToString());
                        line.Add("unit_master_type_id", unittypeid);
                        line.Add("unit_master_principal_id", principleid);
                        line.Add("unit_master_property_id", HttpContext.Current.Session["propertyid"].ToString());
                        DebtorMaster debtor = new DebtorMaster(constr);
                        line.Add("unit_master_debtor_id", debtor.GetIDByCode(hdata["Proprietor"].ToString()).ToString());
                        line.Add("unit_master_area", hdata["AreaSqm"].ToString());
                        line.Add("unit_master_areatype_id", areatypeid);
                        line.Add("unit_master_ownership_interest", hdata["OI"].ToString());
                        line.Add("unit_master_utility_interest", hdata["UI"].ToString());
                        line.Add("unit_master_special_scale", hdata["SpecialScale"].ToString());
                        if(string.IsNullOrEmpty(hdata["Committee"].ToString()))
                            line.Add("unit_master_committee", "");
                        else if(hdata["Committee"].ToString() == "true")
                            line.Add("unit_master_committee", "1");
                        else if (hdata["Committee"].ToString() == "false")
                            line.Add("unit_master_committee", "0");
                        line.Add("unit_master_notes", JSON.StrToDQuoteJson(hdata["Description"].ToString()));
                        units.Add(line);
                    }
                    else
                    {
                        for(int i = 0; i < units.Count; i ++)
                        {
                            if (((Hashtable)units[i])["unit_master_id"].ToString() == hdata["ID"].ToString())
                            {
                                if ((hdata["ID"].ToString()[0] != 'i') && (hdata["ID"].ToString()[0] != 'u')) ((Hashtable)units[i])["unit_master_id"] = "u" + hdata["ID"].ToString();
                                ((Hashtable)units[i])["unit_master_code"] = hdata["Code"].ToString();
                                ((Hashtable)units[i])["unit_master_type_id"] = unittypeid;
                                ((Hashtable)units[i])["unit_master_principal_id"] = principleid;
                                ((Hashtable)units[i])["unit_master_property_id"] = HttpContext.Current.Session["propertyid"].ToString();
                                DebtorMaster debtor = new DebtorMaster(constr);
                                ((Hashtable)units[i])["unit_master_debtor_id"] = debtor.GetIDByCode(hdata["Proprietor"].ToString()).ToString();
                                ((Hashtable)units[i])["unit_master_area"] = hdata["AreaSqm"].ToString();
                                ((Hashtable)units[i])["unit_master_areatype_id"] = areatypeid;
                                ((Hashtable)units[i])["unit_master_ownership_interest"] = hdata["OI"].ToString();
                                ((Hashtable)units[i])["unit_master_utility_interest"] = hdata["UI"].ToString();
                                ((Hashtable)units[i])["unit_master_special_scale"] = hdata["SpecialScale"].ToString();
                                if (string.IsNullOrEmpty(hdata["Committee"].ToString()))
                                    ((Hashtable)units[i])["unit_master_committee"] = "";
                                else if (hdata["Committee"].ToString() == "true")
                                    ((Hashtable)units[i])["unit_master_committee"] = "1";
                                else if (hdata["Committee"].ToString() == "false")
                                    ((Hashtable)units[i])["unit_master_committee"] = "0";
                                ((Hashtable)units[i])["unit_master_notes"] = hdata["Description"].ToString();
                            }
                        }
                    }
                    #endregion
                    Hashtable updates = new Hashtable();
                    updates.Add("temp_data", DBSafeUtils.StrToQuoteSQL( JSON.JsonEncode(items)));
                    temp.Update(updates);
                }

                return "dd";
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return "dd";
        }
        [System.Web.Services.WebMethod]
        public static string BindDebtorSelector()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                DebtorMaster deb = new DebtorMaster(constr);
                List<DebtorMaster> deblist = deb.GetDebtorList();
                foreach (DebtorMaster debtor in deblist)
                {
                    html += "<option value='" + debtor.DebtorMasterId.ToString() + "'>" + debtor.DebtorMasterName + "</option>";
                }
                html += "</select>";
                return html;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        [System.Web.Services.WebMethod]
        public static string BindUnitTypeSelector()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                Sapp.SMS.UnitType unittype = new Sapp.SMS.UnitType(constr);
                List<Sapp.SMS.UnitType> typelist = unittype.GetAreaTypeList();
                foreach (Sapp.SMS.UnitType type in typelist)
                {
                    html += "<option value='" + type.UnitTypeId.ToString() + "'>" + type.UnitTypeCode + "</option>";
                }
                html += "</select>";
                return html;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        [System.Web.Services.WebMethod]
        public static string BindPrincipalSelector()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);

                string html = "<select>";



                html += "<option value='0'>Null</option>";

                Temp temp = new Temp(constr_general);
                if (temp.TempExist(user.UserId, temp_type))
                {
                    temp.LoadData(user.UserId, temp_type);
                    Hashtable items = (Hashtable)JSON.JsonDecode(temp.TempData);
                    ArrayList units = (ArrayList)items["units"];
                    Sapp.SMS.UnitType unittype = new Sapp.SMS.UnitType(constr);
                    foreach (Hashtable line in units)
                    {
                        if (line["unit_master_type_id"].ToString() == unittype.GetIDByCode("PRINCIPAL").ToString() && (line["unit_master_id"].ToString()[0] != 'd'))
                        {
                            html += "<option value='-" + line["unit_master_id"] + "'>" + line["unit_master_code"] + "</option>";
                        }
                    }

                }
                html += "</select>";
                return html;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        [System.Web.Services.WebMethod]
        public static string BindAreaTypeSelector()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                UnitAreaType unitareatype = new UnitAreaType(constr);
                List<UnitAreaTypeInfo> areatypelist = unitareatype.GetAreaTypeList();
                foreach (UnitAreaTypeInfo ui in areatypelist)
                {
                    html += "<option value='" + ui.UnitAreaTypeID.ToString() + "'>" + ui.UnitAreaTypeCode + "</option>";
                }
                html += "</select>";
                return html;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        [System.Web.Services.WebMethod]
        public static string ValidateCode(string postdata)
        {
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            Hashtable postObj = (Hashtable)JSON.JsonDecode(postdata);
            string unit_code = postObj["code"].ToString();
            string unit_id = postObj["id"].ToString();
            string login = HttpContext.Current.User.Identity.Name;
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
                    if (unit_id != "")
                    {
                        if ((unit["unit_master_code"].ToString() == unit_code) && (unit["unit_master_id"].ToString() != unit_id))
                            return " Duplicated Unit Number";
                    }
                    else
                    {
                        if (unit["unit_master_code"].ToString() == unit_code)
                            return " Duplicated Unit Number";
                    }
                }
            }
            return "true";
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

    }
}