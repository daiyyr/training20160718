using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Data.Odbc;
using Sapp.Common;
using Sapp.JQuery;
using Sapp.SMS;
using Sapp.Data;
using Sapp.General;

namespace sapp_sms
{
    public partial class levylist : System.Web.UI.Page, IPostBackEventHandler
    {
        private const string temp_type = "levylist";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridLevyList, LabelTotalAmount };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                    DataTable sysDT = ReportDT.getTable(constr, "system");
                    InterestRateL.Text = ReportDT.GetDataByColumn(sysDT, "system_code", "LEVYINT", "system_value");
                    string bodycorp_id = Request.Cookies["bodycorpid"].Value;
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                    LabelBodycorp.Text = bodycorp.BodycorpCode + " " + bodycorp.BodycorpName;
                    TextBoxDate.Text = DBSafeUtils.DBDateToStr(DateTime.Today);
                    TextBoxEnd.Text = DBSafeUtils.DBDateToStr(DateTime.Today.AddYears(1));
                    for (int i = 1; i < 25; i++)
                    {
                        DropDownListInstallment.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }
                    DropDownListInstallment.SelectedValue = "12";
                    #region Create Temp File In Mysql
                    Temp temp = new Temp(constr_general);
                    Hashtable items = new Hashtable();
                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(constr_general);
                    user.LoadData(login);
                    items.Add("temp_user_id", user.UserId);
                    items.Add("temp_type", DBSafeUtils.StrToQuoteSQL(temp_type));

                    items.Add("temp_data", DBSafeUtils.StrToQuoteSQL("{\"bodycorp_id\":\"" + bodycorp_id + "\", \"levies\":[]}"));
                    items.Add("temp_date", DBSafeUtils.DateToSQL(DateTime.Today));
                    items.Add("temp_expire", DBSafeUtils.DateToSQL(DateTime.Today.AddDays(1)));
                    if (!temp.TempExist(Convert.ToInt32(items["temp_user_id"]), temp_type))
                    {
                        temp.Add(items);
                    }
                    else
                    {
                        temp.LoadData(Convert.ToInt32(items["temp_user_id"]), temp_type);
                        temp.Delete();
                        temp.Add(items);
                    }
                    #endregion
                    #region Update Total OI and UI
                    LabelTotalAmount.Text = "0.00";
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
                else if (args[0] == "ButtonOK")
                {
                    ButtonOK_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string LevyListDataBind(string postdata)
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
                ArrayList levyList = (ArrayList)items["levies"];
                DataTable dt = new DataTable("levies");
                dt.Columns.Add("ID");
                dt.Columns.Add("Chart");
                dt.Columns.Add("Description");
                dt.Columns.Add("Net");
                dt.Columns.Add("Scale");
                decimal net = 0;
                foreach (Hashtable levy in levyList)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = levy["ID"].ToString();
                    ChartMaster chart = new ChartMaster(constr);
                    chart.LoadData(Convert.ToInt32(levy["Chart"]));
                    dr["Chart"] = chart.ChartMasterCode + " | " + chart.ChartMasterName;
                    dr["Description"] = levy["Description"].ToString();
                    dr["Net"] = levy["Net"].ToString();
                    net += Convert.ToDecimal(levy["Net"]);
                    ScaleType scaletype = new ScaleType(constr);
                    scaletype.LoadData(Convert.ToInt32(levy["Scale"]));
                    dr["Scale"] = scaletype.ScaleTypeCode;
                    dt.Rows.Add(dr);
                }
                string sqlfromstr = "FROM `" + dt.TableName + "`";
                string sqlselectstr = "SELECT `ID`, `Chart`, `Description`, `Net`, `Scale` FROM (SELECT *";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                Hashtable userdata = new Hashtable();
                userdata.Add("Description", "Total:");
                userdata.Add("Net", net.ToString("0.00"));
                jqgridObj.SetUserData(userdata);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            return "{}";
        }
        [System.Web.Services.WebMethod]
        public static string SaveDataFromGrid(string rowValue)
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
                    ArrayList levies = new ArrayList();
                    if (items.ContainsKey("levies"))
                    {
                        levies = (ArrayList)items["levies"];
                    }

                    ChartMaster chart = new ChartMaster(constr);
                    string ch = hdata["Chart"].ToString();
                    ch = ch.Substring(0, ch.IndexOf("|") - 1);
                    chart.LoadData(ch);
                    #endregion
                    Hashtable line = new Hashtable();
                    #region Insert or Update Line
                    if (line_id == "")
                    {
                        // if insert
                        line.Add("ID", Guid.NewGuid().ToString("N"));
                        line.Add("Chart", chart.ChartMasterId.ToString());
                        line.Add("Description", hdata["Description"].ToString());
                        line.Add("Net", hdata["Net"].ToString());
                        ScaleType scaletype = new ScaleType(constr);
                        scaletype.LoadData(hdata["Scale"].ToString());
                        line.Add("Scale", scaletype.ScaleTypeId.ToString());
                        levies.Add(line);
                    }
                    else
                    {
                        for (int i = 0; i < levies.Count; i++)
                        {
                            if (((Hashtable)levies[i])["ID"].ToString() == hdata["ID"].ToString())
                            {
                                ((Hashtable)levies[i])["Chart"] = chart.ChartMasterId.ToString();
                                ((Hashtable)levies[i])["Description"] = hdata["Description"].ToString();
                                ((Hashtable)levies[i])["Net"] = hdata["Net"].ToString();
                                ScaleType scaletypes = new ScaleType(constr);
                                scaletypes.LoadData(hdata["Scale"].ToString());
                                ((Hashtable)levies[i])["Scale"] = scaletypes.ScaleTypeId;
                            }
                        }
                    }
                    if (!items.ContainsKey("levies"))
                        items.Add("levies", levies);
                    #endregion
                    Hashtable updates = new Hashtable();
                    updates.Add("temp_data", DBSafeUtils.StrToQuoteSQL(JSON.JsonEncode(items)));
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
        public static string BindChartSelector()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                InvoiceMaster invoice = new InvoiceMaster(constr);

                foreach (ChartMaster chartmaster in invoice.GetChartMasters())
                {
                    html += "<option value='" + chartmaster.ChartMasterName + "'>" + chartmaster.ChartMasterCode + " | " + chartmaster.ChartMasterName + "</option>";
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
        public static string BindScaleSelector()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                ScaleType scaletypes = new ScaleType(constr);
                List<ScaleType> list = scaletypes.GetScaleTypeList();
                foreach (ScaleType scaletype in list)
                {
                    html += "<option value='" + scaletype.ScaleTypeId + "'>" + scaletype.ScaleTypeCode + "</option>";
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
        public static string GetTotal()
        {
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string login = HttpContext.Current.User.Identity.Name;
            decimal total = 0;
            Sapp.General.User user = new User(constr_general);
            user.LoadData(login);

            Temp temp = new Temp(constr_general);
            if (temp.TempExist(user.UserId, temp_type))
            {
                temp.LoadData(user.UserId, temp_type);
                string json = temp.TempData;
                Hashtable items = (Hashtable)JSON.JsonDecode(json);
                ArrayList levies = (ArrayList)items["levies"];
                foreach (Hashtable levy in levies)
                {
                    if (!string.IsNullOrEmpty(levy["Net"].ToString()))
                    {
                        total += Convert.ToDecimal(levy["Net"]);
                    }
                }
            }
            return total.ToString("0.00");
        }
        #endregion

        #region WebControl Events
        private void ImageButtonDelete_Click(string[] args)
        {
            try
            {
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string levy_id = args[1];
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                Temp temp = new Temp(constr_general);
                if (temp.TempExist(user.UserId, temp_type))
                {
                    temp.LoadData(user.UserId, temp_type);
                    string json = temp.TempData;
                    Hashtable items = (Hashtable)JSON.JsonDecode(json);
                    ArrayList levies = (ArrayList)items["levies"];
                    foreach (Hashtable levy in levies)
                    {
                        if (levy["ID"].ToString() == levy_id)
                        {
                            levies.Remove(levy);
                            break;
                        }
                    }
                    Hashtable updates = new Hashtable();
                    updates.Add("temp_data", DBSafeUtils.StrToQuoteSQL(JSON.JsonEncode(items)));
                    temp.Update(updates);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ImageButtonBudget_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string bodycorp_id = Request.Cookies["bodycorpid"].Value;
                DateTime startDate = Convert.ToDateTime(TextBoxDate.Text);
                startDate = Convert.ToDateTime("01/" + startDate.Month + "/" + startDate.Year);
                DateTime endDate = Convert.ToDateTime(TextBoxEnd.Text);
                endDate = Convert.ToDateTime(DateTime.DaysInMonth(endDate.Year, endDate.Month) + "/" + endDate.Month + "/" + endDate.Year);
                Bodycorp bodycorp = new Bodycorp(constr);
                bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                DataTable dt = bodycorp.GetBudgetTable(startDate, endDate);  
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                Temp temp = new Temp(constr_general);
                if (temp.TempExist(user.UserId, temp_type))
                {
                    #region Update Temp DATA
                    temp.LoadData(user.UserId, temp_type);
                    string json = temp.TempData;
                    Hashtable items = (Hashtable)JSON.JsonDecode(json);
                    ArrayList levies = new ArrayList();
                    if (items.ContainsKey("levies"))
                    {
                        levies = (ArrayList)items["levies"];
                    }
                    foreach (DataRow dr in dt.Rows)
                    {
                        Hashtable line = new Hashtable();
                        line.Add("ID", dr["ID"].ToString());

                        line.Add("Chart", dr["ID"].ToString());
                        line.Add("Description", dr["Field"].ToString());
                        decimal line_total = 0;
                        for (int i = 1; i < 13; i++)
                        {
                            if (!dr["M" + i].ToString().Equals(""))
                                line_total += Convert.ToDecimal(dr["M" + i]);
                        }
                        if (CheckBox1.Checked)
                        {
                            int days = (endDate - startDate).Days;
                            decimal taxtrate = decimal.Parse(InterestRateL.Text);
                            line_total += line_total * taxtrate / 365 * (days + 1);
                        }
                        line.Add("Net", line_total.ToString("0.00"));
                        ScaleType scaletype = new ScaleType(constr);
                        scaletype.LoadData(dr["Scale"].ToString());
                        line.Add("Scale", scaletype.ScaleTypeId);
                        levies.Add(line);
                    }
                    if (!items.ContainsKey("levies"))
                        items.Add("levies", levies);
                    Hashtable updates = new Hashtable();
                    updates.Add("temp_data", DBSafeUtils.StrToQuoteSQL(JSON.JsonEncode(items)));
                    #endregion
                    #region Update Total Amount Label
                    temp.Update(updates);
                    temp.LoadData(user.UserId, temp_type);
                    json = temp.TempData;
                    items = (Hashtable)JSON.JsonDecode(json);
                    levies = (ArrayList)items["levies"];
                    decimal total_amount = 0;
                    foreach (Hashtable levy in levies)
                    {
                        total_amount += Convert.ToDecimal(levy["Net"]);
                    }
                    LabelTotalAmount.Text = total_amount.ToString("0.00");
                    #endregion
                    #region Enable Clear Button
                    ImageButtonClear.Enabled = true;
                    #endregion
                }
                Odbc mydb = null;
                try
                {
                    mydb = new Odbc(constr);
                    string sql = "SELECT * FROM `budget_master` WHERE (`budget_master_bodycorp_id`=" + bodycorp_id + ") AND (`budget_master_month` BETWEEN " + DBSafeUtils.DateToSQL(startDate)
                        + " AND " + DBSafeUtils.DateToSQL(endDate) + ")";
                    OdbcDataReader dr = mydb.Reader(sql);
                    ArrayList bmList = new ArrayList();
                    while (dr.Read())
                    {
                        bmList.Add(dr["budget_master_id"].ToString());
                    }
                    string json = JSON.JsonEncode(bmList);
                    Session["levylist_budget_master_ids"] = json;
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
        }
        protected void ImageButtonClear_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string bodycorp_id = Request.Cookies["bodycorpid"].Value;
                Bodycorp bodycorp = new Bodycorp(constr);
                bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                Temp temp = new Temp(constr_general);
                if (temp.TempExist(user.UserId, temp_type))
                {
                    Hashtable items = new Hashtable();
                    items.Add("temp_user_id", user.UserId);
                    items.Add("temp_type", DBSafeUtils.StrToQuoteSQL(temp_type));
                    items.Add("temp_data", DBSafeUtils.StrToQuoteSQL("{\"bodycorp_id\":\"" + bodycorp_id + "\", \"levies\":[]}"));
                    items.Add("temp_date", DBSafeUtils.DateToSQL(DateTime.Today));
                    items.Add("temp_expire", DBSafeUtils.DateToSQL(DateTime.Today.AddDays(1)));
                    temp.LoadData(user.UserId, temp_type);
                    temp.Delete();
                    temp.Add(items);


                    #region Update Total Amount Label
                    LabelTotalAmount.Text = "0.00";
                    #endregion

                    #region Disable Clear Button
                    ImageButtonClear.Enabled = false;
                    #endregion

                    if (Session["levylist_budget_master_ids"] != null)
                        Session["levylist_budget_master_ids"] = null;
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ButtonOK_Click(string[] args)
        {
            string bodycorp_id = Request.Cookies["bodycorpid"].Value;
            string startdate = TextBoxDate.Text;
            string installment = DropDownListInstallment.SelectedValue;
            Response.BufferOutput = true;
            Response.Redirect("levyallocate.aspx?bodycorpid=" + bodycorp_id + "&startdate=" + startdate + "&installment=" + installment, false);
        }
        protected void ImageButtonLevies_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string bodycorp_id = Request.Cookies["bodycorpid"].Value;
                Response.BufferOutput = true;
                Response.Redirect("levies.aspx?bodycorpid=" + bodycorp_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}