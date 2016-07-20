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
using Sapp.SMS;
using Sapp.Common;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.General;
using System.Web.Script.Serialization;

namespace sapp_sms
{
    public partial class budgetmaster : System.Web.UI.Page, IPostBackEventHandler
    {
        private const string temp_type = "tempbudgetmaster";
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Javascript setup
            Control[] wc = { jqGridBudget };
            JSUtils.RenderJSArrayWithCliendIds(Page, wc);


            #endregion
            try
            {
                if (!IsPostBack)
                {
                    Session["startdate"] = Request.QueryString["startdate"].ToString();
                    Session["bodycorpid"] = Request.Cookies["bodycorpid"].Value;

                    /*
                    // Add duplication check for import 19/04/2016
                    if (HttpContext.Current.Session["imbudgetmaster"] != null)
                    {
                        DataTable impdt = ((DataTable)HttpContext.Current.Session["imbudgetmaster"]).Copy();
                        Hashtable allChartMasterID = new Hashtable();
                        foreach (DataRow dr in impdt.Rows)
                        {
                            string ID = dr["ID"].ToString();
                            if (allChartMasterID.ContainsKey(ID))
                            {
                                HttpContext.Current.Session["imbudgetmaster"] = null;
                                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString();
                                HttpContext.Current.Session["Error"] = new Exception("There are duplicated chart master ID [" + ID + "], budget import cann't be finished.");
                                HttpContext.Current.Response.Redirect("~/error.aspx", false);
                            }
                            else
                            {
                                allChartMasterID.Add(ID, ID);
                            }
                        }
                    }
                    // Add End 19/04/2016
                    */

                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                    string bodycorp_id = Request.Cookies["bodycorpid"].Value;
                    DateTime startdate = Convert.ToDateTime(Request.QueryString["startdate"]);
                    startdate = Convert.ToDateTime("01/" + startdate.Month + "/" + startdate.Year);
                    ClientScriptManager cs = Page.ClientScript;
                    string colNames = "";
                    for (int i = 1; i < 13; i++)
                    {
                        colNames += "'" + startdate.AddMonths(i - 1).ToString("MMM/yy") + "',";
                    }
                    colNames = colNames.Substring(0, colNames.Length - 1);
                    cs.RegisterArrayDeclaration("M", colNames);
                    DateTime enddate = startdate.AddMonths(11);
                    enddate = Convert.ToDateTime(DateTime.DaysInMonth(enddate.Year, enddate.Month) + "/" + enddate.Month + "/" + enddate.Year);
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                    #region Create Temp File In Mysql
                    Temp temp = new Temp(constr_general);

                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(constr_general);
                    user.LoadData(login);
                    Hashtable items = new Hashtable();

                    items.Add("temp_user_id", user.UserId);
                    items.Add("temp_type", DBSafeUtils.StrToQuoteSQL(temp_type));
                    DataTable dt = bodycorp.GetBudgetTable(startdate, enddate);
                    Hashtable temp_data = new Hashtable();
                    temp_data.Add("bodycorp_id", bodycorp_id);
                    ArrayList budgetList = new ArrayList();

                    foreach (DataRow dr in dt.Rows)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            string v = dr[dc.ColumnName].ToString();
                            if (v.Equals(""))
                            {
                                dr[dc.ColumnName] = "0";
                            }
                        }
                    }

                    Hashtable allUsedFlgs = new Hashtable();                    // Add 04/04/2016
                    Hashtable allCMId = new Hashtable();                        // Add 15/04/2016
                    foreach (DataRow dr in dt.Rows)
                    {
                        Hashtable usedFlgs = new Hashtable();                   // Add 04/04/2016
                        Hashtable bg_items = new Hashtable();
                        bg_items.Add("ID", dr["ID"].ToString());
                        bg_items.Add("Field", dr["Field"].ToString());
                        bg_items.Add("Scale", dr["Scale"].ToString());

                        for (int i = 1; i < 13; i++)
                        {
                            bg_items.Add("M" + i, dr["M" + i].ToString());

                            // Add Start 04/04/2016
                            string key = "M" + i + "_Used";
                            if (dr[key] != null)
                            {
                                usedFlgs.Add(key, dr[key].ToString());
                            }
                            else
                            {
                                usedFlgs.Add(key, "0");
                            }
                            // Add End 04/04/2016
                        }
                        budgetList.Add(bg_items);
                        allUsedFlgs.Add(dr["ID"].ToString(), usedFlgs);                 // Add 04/04/2016
                        allCMId.Add(dr["ID"].ToString(), "1");                          // Add 15/04/2016
                    }

                    JavaScriptSerializer helper = new JavaScriptSerializer();           // Add 04/04/2016
                    HiddenUsedFlgs.Value = helper.Serialize(allUsedFlgs).ToString();    // Add 04/04/2016

                    Session["USED_BUDGET"] = allUsedFlgs;                               // Add 15/04/2016
                    Session["ALL_CHART_MASTER_ID"] = allCMId;                           // Add 19/04/2016

                    temp_data.Add("budgets", budgetList);
                    items.Add("temp_data", DBSafeUtils.StrToQuoteSQL(JSON.JsonEncode(temp_data)));
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

                    jqGridBudget.addVisible = "false";
                    #endregion

                    string sql = "SELECT COUNT(*) FROM `chart_master` WHERE `chart_master_levy_base`=1 ORDER BY chart_master_code ";
                    Odbc odbc = new Odbc(constr);
                    object counter = odbc.ExecuteScalar(sql);
                    HiddenCMTotalAmount.Value = counter.ToString();
                    HiddenCMCurrentAmount.Value = budgetList.Count.ToString();
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
                if (args[0] == "ImageButtonSave")
                {
                    ImageButtonSave_Click(args);
                }
                if (args[0] == "ImageButtonDelete")
                {
                    //ImageButtonDelete_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                Temp temp = new Temp(constr_general);
                if (temp.TempExist(user.UserId, temp_type))
                {
                    temp.LoadData(user.UserId, temp_type);
                    string json = temp.TempData;
                    Hashtable items = (Hashtable)JSON.JsonDecode(json);
                    ArrayList budgetList = (ArrayList)items["budgets"];
                    DataTable dt = new DataTable("tempbudgets");
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Total");
                    dt.Columns.Add("Field");
                    dt.Columns.Add("Scale");
                    for (int i = 1; i < 13; i++)
                    {
                        dt.Columns.Add("M" + i);
                    }

                    foreach (Hashtable budget in budgetList)
                    {
                        DataRow dr = dt.NewRow();
                        dr["ID"] = budget["ID"].ToString();
                        dr["Field"] = budget["Field"].ToString();
                        dr["Scale"] = "UI";
                        for (int i = 1; i < 13; i++)
                        {
                            dr["M" + i] = budget["M" + i].ToString();
                        }

                        dt.Rows.Add(dr);
                    }

                    string sqlfromstr = "FROM `" + dt.TableName + "`";
                    string sqlselectstr = "SELECT `ID`, `Total`,`Field`, `Scale`";
                    for (int i = 1; i < 13; i++)
                    {
                        sqlselectstr += ", `M" + i + "` ";
                    }

                    sqlselectstr += " FROM (SELECT *";
                    if (HttpContext.Current.Session["imbudgetmaster"] != null)
                    {
                        dt = ((DataTable)HttpContext.Current.Session["imbudgetmaster"]).Copy();
                        HttpContext.Current.Session["imbudgetmaster"] = null;
                    }


                    HttpContext.Current.Session["budgetmaster"] = dt;
                    foreach (DataRow dr in dt.Rows)
                    {
                        decimal total = 0;
                        foreach (DataColumn dc in dt.Columns)
                        {

                            if (dc.ColumnName.Contains("M"))
                            {
                                decimal d = 0;
                                decimal.TryParse(dr[dc.ColumnName].ToString(), out d);
                                total += d;
                            }
                        }
                        dr["Total"] = total;

                    }
                    JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                    Hashtable userdata = new Hashtable();
                    userdata.Add("Field", "Total:");
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dc.ColumnName.Contains("M"))
                        {
                            userdata.Add(dc.ColumnName, ReportDT.SumTotal(dt, dc.ColumnName));
                        }

                    }


                    jqgridObj.SetUserData(userdata);
                    string jsonStr = jqgridObj.GetJSONStr();
                    return jsonStr;

                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return "{}";
        }

        [System.Web.Services.WebMethod]
        public static string BindBudgetField()
        {
            try
            {
                // Add 06/04/2016
                string current_id = HttpContext.Current.Request.QueryString["id"];

                // Add 19/04/2016
                Hashtable chartIDs = (Hashtable)HttpContext.Current.Session["ALL_CHART_MASTER_ID"];
                List<string> chartIdList = new List<string>();
                foreach (string key in chartIDs.Keys)
                {
                    chartIdList.Add(key);
                }

                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                Odbc mydb = null;
                try
                {
                    mydb = new Odbc(constr);
                    string sql = "SELECT * FROM `chart_master` WHERE `chart_master_levy_base`=1 order by chart_master_code";
                    OdbcDataReader dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        // Update 15/04/2016
                        if (chartIdList.Contains(dr["chart_master_id"].ToString()) == false || (current_id != "" && current_id.Equals(dr["chart_master_id"].ToString())))
                        {
                            html += "<option value='" + dr["chart_master_id"].ToString() + "'>" + dr["chart_master_code"].ToString() + " | " + dr["chart_master_name"].ToString() + "</option>";
                        }
                    }
                }
                finally
                {
                    if (mydb != null) mydb.Close();
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
        public static string BindScale()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                Odbc mydb = null;
                try
                {
                    mydb = new Odbc(constr);
                    string sql = "SELECT * FROM `scale_types`";
                    OdbcDataReader dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        html += "<option value='" + dr["scale_type_id"].ToString() + "'>" + dr["scale_type_code"].ToString() + "</option>";
                    }
                }
                finally
                {
                    if (mydb != null) mydb.Close();
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
        public static string SaveDataFromGrid(string rowValue)
        {
            return "";
        }

        [System.Web.Services.WebMethod]
        public static string SaveDataGrid(object postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                string bodycorp_id = HttpContext.Current.Session["bodycorpid"].ToString();
                object[] objs = (object[])postdata;
                ArrayList budgetList = new ArrayList();
                Hashtable allCMId = new Hashtable();                                    // Add 19/04/2016
                ChartMaster chart = new ChartMaster(constr);                            // Add 19/04/2016

                foreach (object obj in objs)
                {
                    Hashtable record = new Hashtable((Dictionary<string, object>)obj);  // Add 19/04/2016
                    //budgetList.Add(new Hashtable((Dictionary<string, object>)obj));
                    budgetList.Add(record);

                    // Add Strat 19/04/2016
                    if (record["ID"].ToString().Equals(""))
                    {
                        string field = record["Field"].ToString();
                        string code = field.Substring(0, field.IndexOf(" "));
                        chart.LoadData(code);
                        allCMId.Add(chart.ChartMasterId.ToString(), "1");
                        record["ID"] = chart.ChartMasterId.ToString();                  // Add 03/05/2016
                    }
                    else
                    {
                        allCMId.Add(record["ID"].ToString(), "1");
                    }
                    // Add End 19/04/2016
                }
                HttpContext.Current.Session["ALL_CHART_MASTER_ID"] = allCMId;           // Add 19/04/2016

                Temp temp = new Temp(constr_general);
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                Hashtable temp_data = new Hashtable();
                temp_data.Add("bodycorp_id", bodycorp_id);
                temp_data.Add("budgets", budgetList);
                Hashtable items = new Hashtable();
                items.Add("temp_data", DBSafeUtils.StrToQuoteSQL(JSON.JsonEncode(temp_data)));
                if (temp.TempExist(user.UserId, temp_type))
                {
                    temp.LoadData(user.UserId, temp_type);
                    temp.Update(items);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return "";
        }
        #endregion
        #region Web Control Methods
        protected void ImageButtonSave_Click(string[] args)
        {
            try
            {
                string v = "";
                for (int i = 1; i < args.Length; i++)
                {
                    v += args[i];
                }
                this.SaveJQGrid(v);
                Response.BufferOutput = true;
                // Update after import display 17/03/2016 
                //Response.Redirect("goback.aspx", false);
                Response.Redirect("bodycorpdetails.aspx?bodycorpid=" + Request.Cookies["bodycorpid"].Value, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
        #region User Functions
        protected void SaveJQGrid(string json)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                ArrayList budgetList = (ArrayList)JSON.JsonDecode(json);

                // Add duplication check 19/04/2016
                Hashtable allChartMasterID = new Hashtable();
                foreach (Hashtable bg_items in budgetList)
                {
                    string ID = bg_items["ID"].ToString();
                    if (allChartMasterID.ContainsKey(ID))
                    {
                        HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); 
                        HttpContext.Current.Session["Error"] = new Exception("There are duplicated chart master ID [" + ID +"], update cann't be finished.");
                        HttpContext.Current.Response.Redirect("~/error.aspx", false);
                    }
                    else
                    {
                        allChartMasterID.Add(ID, ID);
                    }
                }

                string bodycorp_id = Request.Cookies["bodycorpid"].Value;
                DateTime startdate = Convert.ToDateTime(Request.QueryString["startdate"]);
                string checkline = "";
                Odbc mydb = null;
                try
                {
                    mydb = new Odbc(constr);
                    mydb.StartTransaction();
                    //string sql = "delete From budget_master where (budget_master_month between" + DBSafeUtils.DateTimeToSQL(startdate) + " and " + DBSafeUtils.DateTimeToSQL(startdate.AddMonths(12)) + ") and budget_master_bodycorp_id = " + bodycorp_id;
                    // Update 15/04/2016
                    string sql = "delete From budget_master where (budget_master_month between" + DBSafeUtils.DateTimeToSQL(startdate) + " and " + DBSafeUtils.DateTimeToSQL(startdate.AddMonths(12)) + ") and budget_master_bodycorp_id = " + bodycorp_id + " AND budget_master_used <> 1 ";
                    DataTable bugDT = mydb.ReturnTable(sql, "temp");

                    // Add 15/04/2016
                    Hashtable allUsedFlgs = (Hashtable)Session["USED_BUDGET"];

                    foreach (Hashtable bg_items in budgetList)
                    {
                        string ID = bg_items["ID"].ToString();
                        string Field = bg_items["Field"].ToString();
                        string Scale = bg_items["Scale"].ToString();
                        ScaleType scaletype = new ScaleType(constr);
                        scaletype.SetOdbc(mydb);
                        scaletype.LoadData(Scale);


                        bool is_empty = true;

                        for (int i = 1; i < 13; i++)
                        {
                            if (!bg_items["M" + i].ToString().Equals(""))
                            {
                                if (Convert.ToDecimal(bg_items["M" + i].ToString()) != 0)
                                {
                                    // Add "used" check 15/04/2016
                                    if (allUsedFlgs[ID] == null || ((Hashtable)allUsedFlgs[ID])["M" + i + "_Used"].Equals("1") == false)
                                    {
                                        is_empty = false;
                                    }
                                }
                            }
                        }
                        if (!is_empty)
                        {
                            for (int i = 1; i < 13; i++)
                            {
                                if (ID.Equals(""))
                                {
                                    string code = Field.Substring(0, Field.IndexOf(" "));
                                    ChartMaster chart = new ChartMaster(constr);
                                    chart.LoadData(code);
                                    ID = chart.ChartMasterId.ToString();
                                }

                                // Add "used" check 15/04/2016
                                if (allUsedFlgs[ID] == null || ((Hashtable)allUsedFlgs[ID])["M" + i + "_Used"].Equals("1") == false)
                                {
                                    BudgetMaster budget = new BudgetMaster(constr);
                                    budget.SetOdbc(mydb);
                                    bool isexist = budget.LoadData(Convert.ToInt32(ID), startdate.AddMonths(i - 1));
                                    Hashtable bg_items2 = new Hashtable();
                                    checkline = bg_items["M" + i].ToString();
                                    bg_items2.Add("budget_master_bodycorp_id", Convert.ToInt32(bodycorp_id));
                                    bg_items2.Add("budget_master_chart_id", Convert.ToInt32(ID));
                                    bg_items2.Add("budget_master_scale_id", scaletype.ScaleTypeId);
                                    bg_items2.Add("budget_master_amound", AdFunction.Rounded(bg_items["M" + i].ToString()));
                                    bg_items2.Add("budget_master_month", DBSafeUtils.DateToSQL(startdate.AddMonths(i - 1)));
                                    budget.Add(bg_items2);
                                }
                            }
                        }
                    }
                    mydb.Commit();
                }
                catch (Exception ex)
                {
                    if (mydb != null) mydb.Rollback();
                    HttpContext.Current.Session["Error"] = ex.Message + "  " + checkline; HttpContext.Current.Response.Redirect("~/error.aspx", false);

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
        #endregion

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            DataTable dt = new DataTable();
            string path = "~/temp/budget.csv";
            string fpath = Server.MapPath(path);
            if (Session["budgetmaster"] != null)
            {
                dt = (DataTable)Session["budgetmaster"];
            }
            else
            {
                return;
            }
            CsvDT.DataTableToCsv(dt, fpath);
            Response.Redirect(path, false);
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            // Update 22/04/2016
            //Response.Redirect("budgetupload.aspx?burl=" + Server.UrlEncode(Request.Url.ToString()), false);
            Response.Redirect("budgetupload.aspx?burl=" + Server.UrlEncode(Request.Url.ToString()) + "&bodycorpid=" + Request.QueryString["bodycorpid"] + "&startdate=" + Request.QueryString["startdate"], false);
        }


    }
}