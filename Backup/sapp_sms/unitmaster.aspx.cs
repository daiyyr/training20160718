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
    public partial class unitmaster : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
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
                LabelBalance.Text = "123";
                decimal oi = decimal.Parse(LabelTotalOI.Text);
                decimal ui = decimal.Parse(LabelTotalUI.Text);
                ClientScriptManager cs = Page.ClientScript;
                if (oi > 100)
                    cs.RegisterStartupScript(GetType(), LabelTotalOI.ID, "alert('OI over 100%');", true);
                if (ui > 100)
                    cs.RegisterStartupScript(GetType(), LabelTotalOI.ID, "alert('UI over 100%');", true);
                if (Request.QueryString["inactive"] != null)
                    Session["inactive"] = " and unit_master_inactive_date is not null";
                else
                    Session["inactive"] = " and unit_master_inactive_date is null";
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
                if (args[0] == "ImageButtonDetails")
                {
                    ImageButtonDetails_Click(args);
                }
                else if (args[0] == "ImageButtonEdit")
                {
                    ImageButtonEdit_Click(args);
                }
                else if (args[0] == "ImageButtonDelete")
                {
                    ImageButtonDelete_Click(args);
                }
                else if (args[0] == "P")
                {
                    ImageButtonDelete_Click(args);
                    UnitMaster u = new UnitMaster(AdFunction.conn);
                    u.LoadData(int.Parse(args[1]));
                    Response.Redirect("debtormasterdetails.aspx?debtorid="+u.UnitMasterDebtorId,false);
                }

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
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
                        + " WHERE `unit_master_property_id`=" + propertyid + " AND `unit_type_code` != 'ACCESSORIES'" + HttpContext.Current.Session["inactive"].ToString();
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
                    dt.Columns.Add("Balance");
                    OdbcDataReader dr = mydb.Reader(sql);
                    while (dr.Read())
                    {
                        DataRow nr = dt.NewRow();
                        nr["ID"] = dr["unit_master_id"];
                        nr["Code"] = dr["unit_master_code"];
                        nr["DebtorCode"] = dr["debtor_master_name"];
                        nr["Type"] = dr["unit_type_code"];
                        nr["Description"] = dr["unit_master_notes"];
                        nr["Proprietor"] = dr["debtor_master_name"];
                        nr["AreaSqm"] = dr["unit_master_area"];
                        nr["AreaType"] = dr["unit_areatype_code"];
                        nr["OI"] = dr["unit_master_ownership_interest"];
                        nr["UI"] = dr["unit_master_utility_interest"];
                        //UnitMaster u = new UnitMaster(constr);
                        //u.SetOdbc(mydb);
                        //u.LoadData(int.Parse(dr["unit_master_id"].ToString()));
                        //DataTable balanceDT = u.GetActivity(DateTime.Now.AddYears(-1), DateTime.Now);
                        //nr["Balance"] = ReportDT.SumTotal(balanceDT, "Invoice") - ReportDT.SumTotal(balanceDT, "Receipt");
                        if (dr["unit_master_ownership_interest"] != DBNull.Value && dr["unit_master_inactive_date"] == DBNull.Value) oi += Convert.ToDecimal(dr["unit_master_ownership_interest"]);
                        if (dr["unit_master_utility_interest"] != DBNull.Value && dr["unit_master_inactive_date"] == DBNull.Value) ui += Convert.ToDecimal(dr["unit_master_utility_interest"]);

                        dt.Rows.Add(nr);
                    }
                    string sqlfromstr = "FROM `" + dt.TableName + "`";
                    string sqlselectstr = "SELECT `ID`, `Code`, `Type`, `Description`,`Proprietor`, `AreaSqm`, `AreaType`, `OI`, `UI`,`Balance` FROM (SELECT *";
                    JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                    HttpContext.Current.Session["UnitMasterDT"] = dt;
                    Hashtable userdata = new Hashtable();
                    userdata.Add("AreaType", "Total:");
                    userdata.Add("OI", oi.ToString("0.00000"));
                    userdata.Add("UI", ui.ToString("0.00000"));
                    userdata.Add("Balance", ReportDT.SumTotal(dt, "Balance"));
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
        //[System.Web.Services.WebMethod]
        //public static string FillSubGrid(string postdata, string masterRow)
        //{
        //    try
        //    {
        //        Object principleObj = JSON.JsonDecode(masterRow);
        //        Hashtable hdata = (Hashtable)principleObj;
        //        int principle_id = Convert.ToInt32(hdata["ID"]);
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        UnitMaster principle = new UnitMaster(constr);
        //        principle.LoadData(principle_id);
        //        ArrayList units = principle.GetAccessoryUnits();
        //        DataTable dt = new DataTable("units");
        //        dt.Columns.Add("ID");
        //        dt.Columns.Add("Code");
        //        dt.Columns.Add("Type");
        //        dt.Columns.Add("Description");
        //        dt.Columns.Add("Proprietor");
        //        dt.Columns.Add("AreaSqm");
        //        dt.Columns.Add("AreaType");
        //        dt.Columns.Add("OI");
        //        dt.Columns.Add("UI");

        //        foreach (UnitMaster unit in units)
        //        {
        //            DataRow dr = dt.NewRow();
        //            dr["ID"] = unit.UnitMasterId.ToString();
        //            dr["Code"] = unit.UnitMasterCode;
        //            Sapp.SMS.UnitType unittype = new Sapp.SMS.UnitType(constr);
        //            unittype.LoadData(unit.UnitMasterTypeId);
        //            dr["Type"] = unittype.UnitTypeCode;
        //            dr["Description"] = unit.UnitMasterNotes;
        //            if (unit.UnitMasterDebtorId != null)
        //            {
        //                DebtorMaster proprietor = new DebtorMaster(constr);
        //                proprietor.LoadData(unit.UnitMasterDebtorId);
        //                dr["Proprietor"] = proprietor.DebtorMasterCode;
        //            }
        //            else
        //                dr["Proprietor"] = "";
        //            if (unit.UnitMasterArea != null) dr["AreaSqm"] = unit.UnitMasterArea.Value.ToString("0.00");
        //            else
        //                dr["AreaSqm"] = "";



        //            if (unit.UnitMasterAreatypeId != null)
        //            {
        //                UnitAreaType areatype = new UnitAreaType(constr);
        //                areatype.LoadData(unit.UnitMasterAreatypeId.Value);
        //                dr["AreaType"] = areatype.UnitAreaTypeCode;
        //            }
        //            else
        //                dr["AreaType"] = "";
        //            if (unit.UnitMasterOwnershipInterest != null) dr["OI"] = unit.UnitMasterOwnershipInterest.Value.ToString("0.00000");
        //            else
        //                dr["OI"] = "";
        //            if (unit.UnitMasterUtilityInterest != null) dr["UI"] = unit.UnitMasterUtilityInterest.Value.ToString("0.00000");
        //            else
        //                dr["UI"] = "";

        //            dt.Rows.Add(dr);
        //        }
        //        string sqlfromstr = "FROM `" + dt.TableName + "`";
        //        string sqlselectstr = "SELECT `ID`, `Code`, `Type`, `Description`,`Proprietor`, `AreaSqm`, `AreaType`, `OI`, `UI` FROM (SELECT *";
        //        JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
        //        string jsonStr = jqgridObj.GetJSONStr();
        //        return jsonStr;
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
        //    }
        //    return null;
        //}
        #endregion

        #region WebControl Events

        protected void ImageButtonAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string property_id = "";
                if (Request.QueryString["propertyid"] != null) property_id = Request.QueryString["propertyid"];
                Response.BufferOutput = true;
                Response.Redirect("~/unitmasteredit.aspx?mode=add&propertyid=" + property_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            try
            {
                string unitmaster_id = args[1];

                Response.BufferOutput = true;
                Response.Redirect("~/unitmasteredit.aspx?mode=edit&unitmasterid=" + unitmaster_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        private void ImageButtonDetails_Click(string[] args)
        {
            string unitmaster_id = "";
            try
            {
                unitmaster_id = args[1];
                Response.BufferOutput = true;
                string bodycorp_id = "";
                if (Request.Cookies["bodycorpid"].Value != null)
                    bodycorp_id = "&bodycorpid=" + Request.Cookies["bodycorpid"].Value;
                Response.Redirect("~/unitmasterdetails.aspx?unitmasterid=" + unitmaster_id + bodycorp_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonDelete_Click(string[] args)
        {
            try
            {
                string unitmaster_id = args[1];

                unitmaster_id = args[1];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                UnitMaster unit = new UnitMaster(constr);
                unit.Delete(Convert.ToInt32(unitmaster_id));
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
            }
        }

        protected void ImageButtonSetupWizard_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string property_id = Request.QueryString["propertyid"];
                Response.Redirect("unitsetupwizard.aspx?propertyid=" + property_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonExport_Click(object sender, ImageClickEventArgs e)
        {
            if (Session["UnitMasterDT"] == null)
                DataGridDataBind("", Request.QueryString["propertyid"].ToString());
            DataTable dt = (DataTable)Session["UnitMasterDT"];
            string path = "C:\\SMS\\" + DateTime.Now.ToString("ddmmyyyy") + ".csv";
            CsvDT.DataTableToCsv(dt, path);
            FileInfo file = new FileInfo(path);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
            HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
            HttpContext.Current.Response.ContentType = "text/plain";
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.TransmitFile(file.FullName);
            HttpContext.Current.Response.End();
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("unitupload.aspx?propertyid=" + Request.QueryString["propertyid"].ToString());
            //UIImport(SetImpotDT());
        }
        public DataTable SetImpotDT()
        {
            DataTable dt = CsvDT.CsvToDataTable("C:\\SMS\\", "1.csv");
            dt.Columns.Add("unit_master_id");
            dt.Columns.Add("unit_master_code");
            dt.Columns.Add("unit_master_type_id");
            dt.Columns.Add("unit_master_principal_id");
            dt.Columns.Add("unit_master_property_id");
            dt.Columns.Add("unit_master_debtor_id");
            dt.Columns.Add("unit_master_area");
            dt.Columns.Add("unit_master_areatype_id");
            dt.Columns.Add("unit_master_ownership_interest");
            dt.Columns.Add("unit_master_utility_interest");
            dt.Columns.Add("unit_master_special_scale");
            dt.Columns.Add("unit_master_committee");
            dt.Columns.Add("unit_master_notes");
            dt.Columns.Add("unit_master_begin_date");
            dt.Columns.Add("unit_master_inactive_date");
            foreach (DataRow dr in dt.Rows)
            {
                dr["unit_master_id"] = dr["ID"];
                dr["unit_master_code"] = dr["Code"];
                dr["unit_master_type_id"] = GetIDbyeName(dr["Type"].ToString(), "unit_type_code", "unit_types");
                dr["unit_master_principal_id"] = "Null";
                dr["unit_master_property_id"] = Request.QueryString["propertyid"].ToString();
                dr["unit_master_debtor_id"] = GetIDbyeName(dr["DebtorCode"].ToString(), "debtor_master_name", "debtor_master");
                dr["unit_master_area"] = "Null";
                dr["unit_master_areatype_id"] = GetIDbyeName(dr["AreaType"].ToString(), "unit_areatype_name", "unit_areatypes"); ;
                dr["unit_master_ownership_interest"] = dr["OI"].ToString();
                dr["unit_master_utility_interest"] = dr["UI"].ToString();
                dr["unit_master_special_scale"] = "Null";
                dr["unit_master_committee"] = "Null";
                dr["unit_master_notes"] = "Null";
                dr["unit_master_begin_date"] = "0000-00-00";
                dr["unit_master_inactive_date"] = "Null";
            }
            dt.Columns.Remove("ID");
            dt.Columns.Remove("Code");
            dt.Columns.Remove("DebtorCode");
            dt.Columns.Remove("Type");
            dt.Columns.Remove("Description");
            dt.Columns.Remove("Proprietor");
            dt.Columns.Remove("AreaSqm");
            dt.Columns.Remove("AreaType");
            dt.Columns.Remove("OI");
            dt.Columns.Remove("UI");
            return dt;
        }
        public void UIImport(DataTable dt)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            Odbc o = new Odbc(constr);
            o.StartTransaction();
            try
            {
                string sql = "select * from `unit_master` where unit_master_property_id = " + Request.QueryString["propertyid"].ToString();
                DataTable old = o.ReturnTable(sql, "oldUnit");
                foreach (DataRow newdr in dt.Rows)
                {
                    string newid = newdr["unit_master_id"].ToString();
                    if (!newid.Equals(""))
                        old.DefaultView.RowFilter = "unit_master_id = " + newid;
                    else
                        old.DefaultView.RowFilter = "unit_master_id = -1";
                    DataTable temp = old.DefaultView.ToTable();
                    if (temp.Rows.Count > 0)
                        foreach (DataRow odr in temp.Rows)
                        {
                            string oid = odr["unit_master_id"].ToString();
                            Hashtable ht = MakeHsahTable(newdr);
                            o.ExecuteUpdate("unit_master", ht, " where `unit_master_id`=" + newid);
                        }
                    else
                    {
                        Hashtable ht = MakeHsahTable(newdr);
                        o.ExecuteInsert("unit_master", ht);
                    }
                }
                o.Commit();
            }
            catch (Exception ex)
            {
                if (o != null) o.Rollback();
                throw ex;
            }
            finally
            {
                if (o != null) o.Close();
            }
        }
        public Hashtable MakeHsahTable(DataRow dr)
        {
            Hashtable ht = new Hashtable();
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                string cname = dr.Table.Columns[i].ColumnName;
                string v = dr[cname].ToString();
                if (!v.Equals("Null"))
                    ht.Add(cname, "'" + v + "'");
            }
            return ht;
        }
        public string GetIDbyeName(string name, string col, string Table)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string result = "";
            Odbc o = new Odbc(constr);
            string sql = "select * from `" + Table + "` where `" + col + "` = '" + name.Replace("'", "''") + "'";
            DataTable dt = o.ReturnTable(sql, "type");
            if (dt.Rows.Count > 0)
                result = dt.Rows[0][0].ToString();
            return result;
        }
        #endregion

        protected void ImageButton6_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/owershipupload.aspx", false);
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect(Request.Url.ToString() + "&inactive=yes");

        }

        protected void ImageButton3_Click(object sender, ImageClickEventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            //o.StartTransaction();
            try
            {
                DataTable dt = o.ReturnTable("select * from unit_master LEFT JOIN property_master ON unit_master_property_id=property_master_id  where property_master_bodycorp_id=" + AdFunction.BodyCorpID, "t1");
                foreach (DataRow dr in dt.Rows)
                {
                    string uid = dr["unit_master_id"].ToString();
                    UnitMaster um = new UnitMaster(AdFunction.conn);
                    um.SetOdbc(o);
                    um.Delete(int.Parse(uid));
                }
                //o.Commit();
            }
            catch (Exception ex)
            {
                //o.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
        }


    }
}
