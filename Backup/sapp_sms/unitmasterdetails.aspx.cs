using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using Sapp.SMS;
using Sapp.Data;
using Sapp.Common;
using Sapp.JQuery;


namespace sapp_sms
{
    public partial class unitmasterdetails : System.Web.UI.Page, IPostBackEventHandler
    {
        private string CheckedQueryString()
        {
            if (null != Request.QueryString["propertyid"] && Regex.IsMatch(Request.QueryString["propertyid"], "^[0-9]*$"))
            {
                return Request.QueryString["propertyid"];
            }
            return "";
        }
        private string NewQueryString(string connector)
        {
            string result = CheckedQueryString();
            return string.IsNullOrEmpty(result) ? "" : connector + "propertyid=" + result;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string unitmaster_id = "";
            try
            {
                DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                DateTime d2 = new DateTime(DateTime.Now.AddYears(-1).Year, 12, 1);
                TextBoxActivityStart.Text = d2.ToString("dd/MM/yyyy");
                TextBoxActivityEnd.Text = d.ToString("dd/MM/yyyy");
                if (Request.QueryString["unitmasterid"] != null)
                {
                    Session["unitmasterdetails_unitmasterid"] = Request.QueryString["unitmasterid"].ToString();
                    unitmaster_id = Request.QueryString["unitmasterid"];
                }
                #region Javascript setup
                Control[] wc = { jqGridAccUnit };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    #region Load Page
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        if (Request.QueryString["unitmasterid"] != null) unitmaster_id = Request.QueryString["unitmasterid"];
                        UnitMaster unitmaster = new UnitMaster(constr);
                        unitmaster.LoadData(Convert.ToInt32(unitmaster_id));
                        #region Initial Web Controls
                        LabelElementID.Text = unitmaster.UnitMasterId.ToString();
                        HiddenUnitId.Value = unitmaster.UnitMasterId.ToString();
                        LabelCode.Text = DBSafeUtils.DBStrToStr(unitmaster.UnitMasterCode);
                        LabelKnowAs.Text = DBSafeUtils.DBStrToStr(unitmaster.UnitMasterKnowAs);
                        LabelArea.Text = unitmaster.UnitMasterArea.ToString();
                        LabelCommittee.Text = unitmaster.UnitMasterCommittee.ToString();
                        LabelSpecialScale.Text = unitmaster.UnitMasterSpecialScale.ToString();
                        LabelUtilityInterest.Text = unitmaster.UnitMasterUtilityInterest.ToString();
                        LabelOwnershipInterest.Text = unitmaster.UnitMasterOwnershipInterest.ToString();
                        LabelNotes.Text = DBSafeUtils.DBStrToStr(unitmaster.UnitMasterNotes);
                        BeginingDateT.Text = unitmaster.UnitMasterBeginDate.ToString("dd/MM/yyyy");
                        if (unitmaster.UnitMasterInactiveDate != null)
                            InactiveDateT.Text = DateTime.Parse(unitmaster.UnitMasterInactiveDate.ToString()).ToString("dd/MM/yyyy");
                        PropertyMaster property = new PropertyMaster(constr);
                        property.LoadData(unitmaster.UnitMasterPropertyId);
                        LabelBodycorp.Text = property.PropertyMasterCode;
                        Sapp.SMS.UnitType unittype = new Sapp.SMS.UnitType(constr);
                        unittype.LoadData(unitmaster.UnitMasterTypeId);
                        LabelType.Text = unittype.UnitTypeCode;
                        DebtorMaster debtor = new DebtorMaster(constr);
                        debtor.LoadData((int)unitmaster.UnitMasterDebtorId);
                        LabelDebtor.Text = debtor.DebtorMasterName;
                        if (unitmaster.UnitMasterPrincipalId != null)
                        {
                            UnitMaster umaster = new UnitMaster(constr);
                            umaster.LoadData((int)unitmaster.UnitMasterPrincipalId);
                            LabelPrincipal.Text = DBSafeUtils.DBStrToStr(umaster.UnitMasterCode);
                        }
                        if (unitmaster.UnitMasterAreatypeId.HasValue)
                        {
                            UnitAreaType areatype = new UnitAreaType(constr);
                            areatype.LoadData(unitmaster.UnitMasterAreatypeId.Value);
                            LabelAreaType.Text = DBSafeUtils.DBStrToStr(areatype.UnitAreaTypeCode);
                        }


                        #endregion
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
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
                if (args[0] == "ButtonDeleteAcc")
                {
                    ButtonDeleteAcc_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string AccUnitDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string unit_id = HttpContext.Current.Session["unitmasterdetails_unitmasterid"].ToString();
            Odbc mydb = null;
            try
            {
                mydb = new Odbc(constr);
                string sql = "SELECT * FROM (`unit_master` LEFT JOIN `unit_types` ON `unit_master_type_id`=`unit_type_id`)"
                        + " LEFT JOIN `unit_areatypes` ON `unit_master_areatype_id`=`unit_areatype_id`"
                        + " WHERE `unit_master_principal_id`=" + unit_id;
                DataTable dt = new DataTable("units");
                dt.Columns.Add("ID");
                dt.Columns.Add("Code");
                dt.Columns.Add("Description");
                dt.Columns.Add("AreaSqm");
                dt.Columns.Add("AreaType");
                dt.Columns.Add("OI");
                dt.Columns.Add("UI");
                dt.Columns.Add("SI");
                OdbcDataReader dr = mydb.Reader(sql);
                while (dr.Read())
                {
                    DataRow nr = dt.NewRow();
                    nr["ID"] = dr["unit_master_id"];
                    nr["Code"] = dr["unit_master_code"];
                    nr["Description"] = dr["unit_master_notes"];
                    nr["AreaSqm"] = dr["unit_master_area"];
                    nr["AreaType"] = dr["unit_areatype_code"];
                    nr["OI"] = dr["unit_master_ownership_interest"];
                    nr["UI"] = dr["unit_master_utility_interest"];
                    nr["SI"] = dr["unit_master_special_scale"];

                    dt.Rows.Add(nr);
                }

                string sqlfromstr = "FROM `" + dt.TableName + "`";
                string sqlselectstr = "SELECT `ID`, `Code`, `Description`, `AreaSqm`, `AreaType`, `OI`, `UI`, `SI` FROM (SELECT *";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }
        [System.Web.Services.WebMethod]
        public static string AccUnitSaveData(string rowValue)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string unit_id = HttpContext.Current.Session["unitmasterdetails_unitmasterid"].ToString();
            UnitMaster unit = new UnitMaster(constr);
            unit.LoadData(Convert.ToInt32(unit_id));
            Object c = JSON.JsonDecode(rowValue);
            Hashtable hdata = (Hashtable)c;
            string line_id = hdata["ID"].ToString();
            if (line_id == "")
            {
                #region Add
                Hashtable items = new Hashtable();
                items.Add("unit_master_code", DBSafeUtils.StrToQuoteSQL(hdata["Code"].ToString()));
                Sapp.SMS.UnitType unittype = new Sapp.SMS.UnitType(constr);
                unittype.LoadData("ACCESSORIES");
                items.Add("unit_master_type_id", unittype.UnitTypeId);
                items.Add("unit_master_principal_id", unit.UnitMasterId);
                items.Add("unit_master_property_id", unit.UnitMasterPropertyId);
                items.Add("unit_master_debtor_id", unit.UnitMasterDebtorId);
                items.Add("unit_master_area", DBSafeUtils.DecimalToSQL(hdata["AreaSqm"].ToString()));

                if (hdata["AreaType"].ToString() != "")
                {
                    UnitAreaType unitareatype = new UnitAreaType(constr);
                    unitareatype.LoadData(hdata["AreaType"].ToString());
                    items.Add("unit_master_areatype_id", unitareatype.UnitAreaTypeId);
                }
                items.Add("unit_master_ownership_interest", DBSafeUtils.DecimalToSQL(hdata["OI"].ToString()));
                items.Add("unit_master_utility_interest", DBSafeUtils.DecimalToSQL(hdata["UI"].ToString()));
                items.Add("unit_master_special_scale", DBSafeUtils.DecimalToSQL(hdata["SI"].ToString()));
                items.Add("unit_master_committee", 0);
                if (unit.UnitMasterBeginDate != DateTime.MinValue) items.Add("unit_master_begin_date", DBSafeUtils.DateToSQL(unit.UnitMasterBeginDate));
                unit.Add(items);
                #endregion
            }
            else
            {
                #region Update
                unit.LoadData(Convert.ToInt32(line_id));
                Hashtable items = new Hashtable();
                items.Add("unit_master_code", DBSafeUtils.StrToQuoteSQL(hdata["Code"].ToString()));
                items.Add("unit_master_area", DBSafeUtils.DecimalToSQL(hdata["AreaSqm"].ToString()));

                if (hdata["AreaType"].ToString() != "")
                {
                    UnitAreaType unitareatype = new UnitAreaType(constr);
                    unitareatype.LoadData(hdata["AreaType"].ToString());
                    items.Add("unit_master_areatype_id", unitareatype.UnitAreaTypeId);
                }
                items.Add("unit_master_ownership_interest", DBSafeUtils.DecimalToSQL(hdata["OI"].ToString()));
                items.Add("unit_master_utility_interest", DBSafeUtils.DecimalToSQL(hdata["UI"].ToString()));
                items.Add("unit_master_special_scale", DBSafeUtils.DecimalToSQL(hdata["SI"].ToString()));
                unit.Update(items);
                #endregion
            }
            return "";
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
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string unit_id = HttpContext.Current.Session["unitmasterdetails_unitmasterid"].ToString();
            UnitMaster unit = new UnitMaster(constr);
            unit.LoadData(Convert.ToInt32(unit_id));
            Hashtable postObj = (Hashtable)JSON.JsonDecode(postdata);
            string unit_code = postObj["code"].ToString();
            Odbc mydb = null;
            try
            {
                mydb = new Odbc(constr);
                string sql = "SELECT `unit_master_id` FROM `unit_master` WHERE (`unit_master_property_id`=" + unit.UnitMasterPropertyId
                    + ") AND (`unit_master_code`=" + DBSafeUtils.StrToQuoteSQL(unit_code) + ")";
                object id = mydb.ExecuteScalar(sql);
                if (id == DBNull.Value) return "true";
                else if (id == null || id.ToString() == "")
                {
                    return "true";
                }
                else
                    return " Duplicated Unit Number";
            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }

        #endregion
        #region WebControl Events
        protected void ImageButtonEdit_Click(object sender, ImageClickEventArgs e)
        {
            string unitmaster_id = "";
            unitmaster_id = Request.QueryString["unitmasterid"];
            Response.BufferOutput = true;
            Response.Redirect("~/unitmasteredit.aspx?unitmasterid=" + Server.UrlEncode(unitmaster_id) + "&mode=edit" + NewQueryString("&"), false);
        }
        protected void ImageButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string unitmaster_id = "";
                if (Request.QueryString["unitmasterid"] != null) unitmaster_id = Request.QueryString["unitmasterid"];
                if (unitmaster_id != "")
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    UnitMaster unitmaster = new UnitMaster(constr);
                    unitmaster.LoadData(Convert.ToInt32(unitmaster_id));
                    unitmaster.Delete();
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/unitmaster.aspx" + NewQueryString("?"), false);
        }
        protected void ImageButtonOwnership_Click(object sender, ImageClickEventArgs e)
        {
            string unitmaster_id = "";
            if (Request.QueryString["unitmasterid"] != null) unitmaster_id = Request.QueryString["unitmasterid"];
            if (unitmaster_id != "")
            {
                Response.Redirect("~/ownerships.aspx?unitid=" + unitmaster_id, false);
            }

        }
        protected void ImageButtonTranOS_Click(object sender, ImageClickEventArgs e)
        {
            string unitmaster_id = "";
            if (Request.QueryString["unitmasterid"] != null) unitmaster_id = Request.QueryString["unitmasterid"];
            if (unitmaster_id != "")
            {
                Response.Write("<script type='text/javascript'> window.open('ownershiptransfer.aspx?unitmasterid=" + unitmaster_id + "','_blank'); </script>");
            }
        }
        protected void ImageButtonDebtor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string unitid = Request.QueryString["unitmasterid"];
                UnitMaster unit = new UnitMaster(constr);
                unit.LoadData(Convert.ToInt32(unitid));

                Response.Redirect("debtormasterdetails.aspx?debtorid=" + unit.UnitMasterDebtorId + "&bodycorpid=" + unit.GetBodycorpId() + "&unitid=" + unit.UnitMasterId, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ButtonDeleteAcc_Click(string[] args)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string unit_id = "";
                unit_id = args[1];
                UnitMaster unit = new UnitMaster(constr);
                unit.Delete(Convert.ToInt32(unit_id));
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void ImageButtonUnitJournal_Click(object sender, ImageClickEventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            UnitMaster unit = new UnitMaster(constr);
            string uid = Request.QueryString["unitmasterid"];
            unit.LoadData(Convert.ToInt32(uid));

            Response.Redirect("~/journalsedit.aspx?mode=unit&unitid=" + HiddenUnitId.Value + "&bodycorpid=" + unit.GetBodycorpId().ToString(), false);
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr3"].ConnectionString;
            Odbc o = new Odbc(constr);
            ArrayList s = new ArrayList();
            string password = GetRandomPassword(6);
            string sql = "Update logins set login_password='" + Crypto.EncryptStringAES(password, "user") + "' where login_id = " + Request.QueryString["unitmasterid"].ToString();
            s.Add(sql);
            o.ExecuteSQL(s);
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(GetType(), ImageButton2.ID, "alert('New Password: " + password + "');", true);
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
            string constr = ConfigurationManager.ConnectionStrings["constr3"].ConnectionString;
            Odbc o = new Odbc(constr);
            ArrayList s = new ArrayList();
            string sql = "Update logins set login_password='" + Crypto.EncryptStringAES(password, "user") + "' where login_unit_code = '" + LabelCode.Text + "'";
            s.Add(sql);
            o.ExecuteSQL(s);
            return password;
        }

        //protected void ImageButtonMaintenance_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        string bodycorp_id = "";
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        if (Request.Cookies["bodycorpid"].Value != null)
        //        {
        //            bodycorp_id = Request.Cookies["bodycorpid"].Value;
        //            Bodycorp bodycorp = new Bodycorp(constr);
        //            bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
        //            List<PropertyMaster> propertyList = bodycorp.GetPropertyList();
        //            foreach (PropertyMaster property in propertyList)
        //            {
        //                Response.BufferOutput = true;
        //                Response.Redirect("pptymaintmaster.aspx?propertyid=" + property.PropertyMasterId);
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ClientScriptManager c = Page.ClientScript;
        //        c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
        //        //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
        //    }
        //}


        //protected void ImageButtonValuations_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        string bodycorp_id = "";
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        if (Request.Cookies["bodycorpid"].Value != null)
        //        {
        //            bodycorp_id = Request.Cookies["bodycorpid"].Value;
        //            Bodycorp bodycorp = new Bodycorp(constr);
        //            bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
        //            List<PropertyMaster> propertyList = bodycorp.GetPropertyList();
        //            foreach (PropertyMaster property in propertyList)
        //            {
        //                Response.BufferOutput = true;
        //                Response.Redirect("pptyvtmaster.aspx?propertyid=" + property.PropertyMasterId);
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ClientScriptManager c = Page.ClientScript;
        //        c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
        //        //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
        //    }
        //}


        //protected void ImageButtonTitles_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        string bodycorp_id = "";
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        if (Request.Cookies["bodycorpid"].Value != null)
        //        {
        //            bodycorp_id = Request.Cookies["bodycorpid"].Value;
        //            Bodycorp bodycorp = new Bodycorp(constr);
        //            bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
        //            List<PropertyMaster> propertyList = bodycorp.GetPropertyList();
        //            foreach (PropertyMaster property in propertyList)
        //            {
        //                Response.BufferOutput = true;
        //                Response.Redirect("pptytitles.aspx?propertyid=" + property.PropertyMasterId);
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ClientScriptManager c = Page.ClientScript;
        //        c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
        //        //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
        //    }
        //}

        //protected void ImageButtonMortgages_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        string bodycorp_id = "";
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        if (Request.Cookies["bodycorpid"].Value != null)
        //        {
        //            bodycorp_id = Request.Cookies["bodycorpid"].Value;
        //            Bodycorp bodycorp = new Bodycorp(constr);
        //            bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
        //            List<PropertyMaster> propertyList = bodycorp.GetPropertyList();
        //            foreach (PropertyMaster property in propertyList)
        //            {
        //                Response.BufferOutput = true;
        //                Response.Redirect("pptymtgs.aspx?propertyid=" + property.PropertyMasterId);
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ClientScriptManager c = Page.ClientScript;
        //        c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
        //        //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
        //    }
        //}

        //protected void ImageButtonContractor_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        string bodycorp_id = "";
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        if (Request.Cookies["bodycorpid"].Value != null)
        //        {
        //            bodycorp_id = Request.Cookies["bodycorpid"].Value;
        //            Bodycorp bodycorp = new Bodycorp(constr);
        //            bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
        //            List<PropertyMaster> propertyList = bodycorp.GetPropertyList();
        //            foreach (PropertyMaster property in propertyList)
        //            {
        //                Response.BufferOutput = true;
        //                Response.Redirect("pptycntrmaster.aspx?propertyid=" + property.PropertyMasterId);
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ClientScriptManager c = Page.ClientScript;
        //        c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
        //        //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
        //    }
        //}

        //protected void ImageButtonInsurance_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        string bodycorp_id = "";
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        if (Request.Cookies["bodycorpid"].Value != null)
        //        {
        //            bodycorp_id = Request.Cookies["bodycorpid"].Value;
        //            Bodycorp bodycorp = new Bodycorp(constr);
        //            bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
        //            List<PropertyMaster> propertyList = bodycorp.GetPropertyList();
        //            foreach (PropertyMaster property in propertyList)
        //            {
        //                Response.BufferOutput = true;
        //                Response.Redirect("pptyinsmaster.aspx?propertyid=" + property.PropertyMasterId);
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ClientScriptManager c = Page.ClientScript;
        //        c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
        //        //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
        //    }
        //}

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("unitmasterstatement.aspx?bodycorpid=" + Request.Cookies["bodycorpid"].Value + "&unitid=" + LabelElementID.Text);
        }

        protected void ImageButton3_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("unitmasterdiscount.aspx?bodycorpid=" + Request.Cookies["bodycorpid"].Value + "&unitid=" + LabelElementID.Text);
        }

        protected void ImageButton4_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("refundedit.aspx?mode=add&multi=false&bodycorpid=" + Request.Cookies["bodycorpid"].Value + "&unitid=" + LabelElementID.Text);
        }

        protected void ImageButton5_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("creditnoteedit.aspx?mode=add&bodycorpid=" + Request.Cookies["bodycorpid"].Value + "&unitid=" + LabelElementID.Text);


        }

    }
}
