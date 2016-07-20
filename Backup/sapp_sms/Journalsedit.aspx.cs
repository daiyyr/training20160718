using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.General;
using Sapp.Common;
using Sapp.SMS;

namespace sapp_sms
{
    public partial class Journalsedit1 : System.Web.UI.Page, IPostBackEventHandler
    {
        private static string TYPE_ID = "6";
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Javascript setup
            Control[] wc = { jqGridTrans, LabelNet, LabelTax, LabelGross, TextBoxDate };
            JSUtils.RenderJSArrayWithCliendIds(Page, wc);
            #endregion
            try
            {
                if (!IsPostBack)
                {
                    TextBoxNum.Enabled = false;
                    TextBoxDescription.Text = "";
                    TextBoxDate.Text = "";

                    AdFunction.Unit_ComboBox(ComboBoxUnit, true);
                    AdFunction.Bodycorp_ComboBox(ComboBoxBodycorp);
                    string mode = "";
                    if (Request.QueryString["mode"] != null)
                    {
                        mode = Request["mode"].ToString();
                        if (mode.Equals("unit"))
                        {
                            TextBoxNum.Text = Journal.GetNextNumber();
                            ComboBoxUnit.SelectedValue = AdFunction.GetQueryString("unitid"); ;
                            ComboBoxUnit.Enabled = true;
                            ComboBoxBodycorp.SelectedValue = AdFunction.GetQueryString("bodycorpid"); ;
                            ComboBoxBodycorp.Enabled = true;
                        }
                        else if (mode.Equals("bc"))
                        {
                            UnitL.Visible = false;
                            ComboBoxUnit.Visible = false;
                            TextBoxNum.Text = Journal.GetNextNumber();
                        }
                    }
                    if (Request.QueryString["unitid"] != null)
                    {
                        ComboBoxUnit.SelectedValue = Request.QueryString["unitid"];
                        ComboBoxUnit.Enabled = false;
                    }

                    string jref = AdFunction.GetQueryString("jref");
                    if (!jref.Equals(""))
                    {
                        LoadJun(jref);
                    }
                    else
                    {
                        if (Request.QueryString["mode"] != null)
                            Session["gl_transactions"] = null;//clean datatable
                    }
                    if (Request.Cookies["bodycorpid"] != null)
                    {
                        ComboBoxBodycorp.SelectedValue = Request.Cookies["bodycorpid"].Value;
                        ComboBoxBodycorp.Enabled = false;
                    }

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
        protected void ImageButtonSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Bodycorp b = new Bodycorp(AdFunction.conn);
                b.LoadData(int.Parse(ComboBoxBodycorp.SelectedValue));
                if (!b.CheckCloseOff(DateTime.Parse(TextBoxDate.Text)))
                {
                    throw new Exception("Journal in close date");
                }
                string unitid = "0";
                if (!ComboBoxUnit.SelectedValue.Equals(""))
                {
                    unitid = ComboBoxUnit.SelectedValue;
                }
                if (HttpContext.Current.Session["gl_transactions"] == null)
                {
                    throw new Exception("Page Expired!");
                }
                Journal.Save(TextBoxNum.Text, ComboBoxBodycorp.SelectedValue, TextBoxDate.Text, unitid, (DataTable)HttpContext.Current.Session["gl_transactions"]);
                Session["gl_transactions"] = null;//clean datatable

                Response.Redirect(Request.Url.ToString(), false);

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            //Javascript Function Only
        }
        private void ImageButtonDelete_Click(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string id = args[1];
                DataTable dt = new DataTable();
                if (Session["gl_transactions"] != null)
                    dt = ((DataTable)HttpContext.Current.Session["gl_transactions"]);
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["ID"].Equals(id))
                    {
                        dr.Delete();
                        dt.AcceptChanges();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        #region Validations
        protected void CustomValidatorBodycorp_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxBodycorp.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        #region WebMethods For DataGrid
        public static DataTable SetDataTable()
        {
            if (HttpContext.Current.Session["gl_transactions"] != null)
            {
                DataTable dt = (DataTable)HttpContext.Current.Session["gl_transactions"];
                return dt;
            }
            else return Journal.Format();

        }


        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            DataTable dt = SetDataTable();
            dt.TableName = "temp";
            Decimal c = 0;
            Decimal d = 0;
            foreach (DataRow dr in dt.Rows)
            {
                c += Decimal.Parse(dr["Credit"].ToString());
                d += Decimal.Parse(dr["Debit"].ToString());
            }
            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`, `Chart`, `Description`, `Debit`,`Credit` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            Hashtable userdata = new Hashtable();
            userdata.Add("Description", "Total:");
            userdata.Add("Credit", c);
            userdata.Add("Debit", d);
            jqgridObj.SetUserData(userdata);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }
        [System.Web.Services.WebMethod]
        public static string BindChartSelector()
        {
            try
            {
                return AdFunction.BindChartSelector();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        [System.Web.Services.WebMethod]
        public static void SaveDataFromGrid(string rowValue)
        {
            DataTable dt = SetDataTable();
            Object c = JSON.JsonDecode(rowValue);
            Hashtable hdata = (Hashtable)c;
            string id = hdata["ID"].ToString();
            if (id.Equals(""))//id judge the update or insert function
                MadeDataRow(hdata);
            else
                UpdateRow(hdata);
        }
        [System.Web.Services.WebMethod]
        public static string EditRow(string rowValue)
        {
            DataTable dt = SetDataTable();
            Object c = JSON.JsonDecode(rowValue);
            Hashtable hdata = (Hashtable)c;
            MadeDataRow(hdata);
            return "";
        }
        [System.Web.Services.WebMethod]
        public static void DeleteRow(string rowID)
        {
            DataTable dt = new DataTable();
            if (HttpContext.Current.Session["gl_transactions"] != null)
                dt = ((DataTable)HttpContext.Current.Session["gl_transactions"]);
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["ID"].Equals(rowID))
                {
                    dr.Delete();
                    dt.AcceptChanges();
                    return;
                }
            }
        }
        #endregion



        public void LoadJun(string jref)
        {
            if (!jref.Equals(""))
            {
                Odbc odbc = new Odbc(AdFunction.conn);
                string sql = "select * from gl_transactions where gl_transaction_ref=" + DBSafeUtils.StrToQuoteSQL(jref);
                DataTable dt = odbc.ReturnTable(sql, "gl");
                dt.Columns.Add("ID");
                dt.Columns.Add("Chart");
                dt.Columns.Add("Description");
                dt.Columns.Add("Debit");
                dt.Columns.Add("Credit");
                if (dt.Rows.Count > 0)
                {
                    ComboBoxBodycorp.SelectedValue = dt.Rows[0]["gl_transaction_bodycorp_id"].ToString();
                    string unitid = dt.Rows[0]["gl_transaction_unit_id"].ToString();
                    if (!unitid.Equals(""))
                        ComboBoxUnit.SelectedValue = unitid;
                    string date = DateTime.Parse(dt.Rows[0]["gl_transaction_date"].ToString()).ToString("dd/MM/yyyy");
                    TextBoxDate.Text = date;
                    TextBoxNum.Text = dt.Rows[0]["gl_transaction_ref"].ToString();
                }
                foreach (DataRow dr in dt.Rows)
                {
                    ChartMaster c = new ChartMaster(AdFunction.conn);
                    string cid = dr["gl_transaction_chart_id"].ToString();
                    c.LoadData(int.Parse(cid));
                    dr["ID"] = dr["gl_transaction_id"].ToString();
                    dr["Description"] = dr["gl_transaction_description"].ToString();
                    dr["Chart"] = c.ChartMasterCode;
                    decimal n = AdFunction.Rounded(dr["gl_transaction_net"].ToString(), 2);
                    if (n < 0)// for jqgrid disply col
                    {
                        dr["Debit"] = -n;
                        dr["Credit"] = "0";
                    }
                    else
                    {
                        dr["Debit"] = "0";
                        dr["Credit"] = n;
                    }
                }
                HttpContext.Current.Session["gl_transactions"] = dt;
            }
        }

        public void InsertRow(decimal n, string chartcode, string Description)
        {
            //ChartMaster c = new ChartMaster(AdFunction.conn);
            //c.LoadData(chartcode);
            //DataTable dt = (DataTable)HttpContext.Current.Session["gl_transactions"];
            //DataRow dr = dt.NewRow();
            //dr["ID"] = dt.Rows.Count;
            //dr["gl_transaction_type_id"] = TYPE_ID;
            //dr["gl_transaction_ref_type_id"] = TYPE_ID;
            //dr["gl_transaction_chart_id"] = c.ChartMasterId;
            //dr["Description"] = Description;
            //dr["gl_transaction_description"] = Description;

            //dr["Chart"] = c.ChartMasterCode;
            //dr["gl_transaction_net"] = n;
            //if (n < 0)// for jqgrid disply col
            //{
            //    dr["Debit"] = -n;
            //    dr["Credit"] = "0";
            //}
            //else
            //{
            //    dr["Debit"] = "0";
            //    dr["Credit"] = n;
            //}
            //dt.Rows.Add(dr);
            //HttpContext.Current.Session["gl_transactions"] = dt;
        }

        public static void UpdateRow(Hashtable hs)
        {
            Decimal credit = 0;
            Decimal debit = 0;
            if (!hs["Credit"].ToString().Equals("") && !hs["Credit"].Equals("0"))
            {
                credit = Decimal.Parse(hs["Credit"].ToString());
                UpdateRow2(hs, credit);
            }
            if (!hs["Debit"].ToString().Equals("") && !hs["Debit"].Equals("0"))
            {
                debit = Decimal.Parse(hs["Debit"].ToString());
                UpdateRow2(hs, -debit);
            }
        }

        public static void UpdateRow2(Hashtable hs, decimal n)
        {
            DataTable dt = new DataTable();
            string rowid = hs["ID"].ToString();
            if (HttpContext.Current.Session["gl_transactions"] != null)
                dt = (DataTable)HttpContext.Current.Session["gl_transactions"];
            DataRow dr = ReportDT.GetDataRowByColumn(dt, "ID", rowid);
            dr["gl_transaction_type_id"] = TYPE_ID;
            ChartMaster c = new ChartMaster(AdFunction.conn);
            string ch = hs["Chart"].ToString();
            ch = ch.Substring(0, ch.IndexOf("|") - 1);
            c.LoadData(ch);
            dr["gl_transaction_chart_id"] = c.ChartMasterId;
            if (hs["Description"].ToString().Equals(""))
            {
                dr["Description"] = c.ChartMasterName;
                dr["gl_transaction_description"] = c.ChartMasterName;
            }
            else
            {
                dr["Description"] = hs["Description"].ToString();
                dr["gl_transaction_description"] = hs["Description"].ToString();
            }
            dr["Chart"] = c.ChartMasterCode;
            dr["gl_transaction_net"] = n;
            if (n < 0)// for jqgrid disply col
            {
                dr["Debit"] = -n;
                dr["Credit"] = "0";
            }
            else
            {
                dr["Debit"] = "0";
                dr["Credit"] = n;
            }
            HttpContext.Current.Session["gl_transactions"] = dt;
        }

        public static void MadeDataRow(Hashtable hs)
        {
            Decimal credit = 0;
            Decimal debit = 0;
            if (!hs["Credit"].ToString().Equals("") && !hs["Credit"].Equals("0"))
            {
                credit = Decimal.Parse(hs["Credit"].ToString());
                MadeDataRow2(hs, credit);
            }
            if (!hs["Debit"].ToString().Equals("") && !hs["Debit"].Equals("0"))
            {
                debit = Decimal.Parse(hs["Debit"].ToString());
                MadeDataRow2(hs, -debit);
            }
        }// for insert 2 rows if credit and debit fills at same time
        public static void MadeDataRow2(Hashtable hs, Decimal n)
        {
            if (HttpContext.Current.Session["gl_transactions"] != null)
            {
                string temp = n.ToString("f2");
                n = decimal.Parse(temp);
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                DataTable dt = (DataTable)HttpContext.Current.Session["gl_transactions"];
                DataRow dr = dt.NewRow();
                dr["ID"] = dt.Rows.Count;
                dr["gl_transaction_type_id"] = TYPE_ID;
                ChartMaster c = new ChartMaster(constr);
                string ch = hs["Chart"].ToString();
                ch = ch.Substring(0, ch.IndexOf("|") - 1);
                c.LoadData(ch);
                dr["gl_transaction_chart_id"] = c.ChartMasterId;
                if (hs["Description"].ToString().Equals(""))
                {
                    dr["Description"] = c.ChartMasterName;
                    dr["gl_transaction_description"] = c.ChartMasterName;
                }
                else
                {
                    dr["Description"] = hs["Description"].ToString();
                    dr["gl_transaction_description"] = hs["Description"].ToString();
                }
                dr["Chart"] = c.ChartMasterCode + " | " + c.ChartMasterName;
                dr["gl_transaction_net"] = n;
                if (n < 0)// for jqgrid disply col
                {
                    dr["Debit"] = -n;
                    dr["Credit"] = "0";
                }
                else
                {
                    dr["Debit"] = "0";
                    dr["Credit"] = n;
                }
                dt.Rows.Add(dr);
                HttpContext.Current.Session["gl_transactions"] = dt;
            }
        }
    }
}