using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.SMS;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Data.Odbc;
using System.IO;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.Common;
using Sapp.General;

namespace sapp_sms
{
    public partial class bodycorpdetails : System.Web.UI.Page, IPostBackEventHandler
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { jqGridComms, jqGridFiles };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                string bodycorp_id = "";
                bodycorp_id = Request.Cookies["bodycorpid"].Value;
                #endregion
                if (!IsPostBack)
                {

                    DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)); 
                    DateTime d2 = new DateTime(DateTime.Now.AddYears(-1).Year, 12, 1);
                    TextBoxActivityStart.Text = d2.ToString("dd/MM/yyyy");
                    TextBoxActivityEnd.Text = d.ToString("dd/MM/yyyy");
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                    Session["bodycorpid"] = Request.Cookies["bodycorpid"].Value;
                    #region Security Check
                    string login = HttpContext.Current.User.Identity.Name;
                    Sapp.General.User user = new User(constr_general);
                    user.LoadData(login);
                    BodycorpManager bm = new BodycorpManager(constr);
                    string security_type = bm.GetSecurityType(Convert.ToInt32(bodycorp_id), user.UserId, constr_general);
                    if (security_type == "DENY")
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("warning.aspx", false);
                    }
                    #endregion
                    #region Load webForm
                    try
                    {



                        Bodycorp bodycorp = new Bodycorp(constr);
                        bodycorp.LoadData(Convert.ToInt32(bodycorp_id));

                        LabelBodycorpID.Text = bodycorp.BodycorpId.ToString();
                        LabelCode.Text = bodycorp.BodycorpCode.ToString();
                        LabelName.Text = bodycorp.BodycorpName;
                        LabelGST.Text = bodycorp.BodycorpGST.ToString();
                        LabelCloseDate.Text = bodycorp.Bodycorp_Close_Off.ToString("dd/MM/yyyy");
                        if (bodycorp.BodycorpAgmDate.HasValue)
                            LabelAgmDate.Text = bodycorp.BodycorpAgmDate.Value.ToShortDateString();//
                        if (bodycorp.BodycorpAgmTime.HasValue)
                            LabelAgmTime.Text = bodycorp.BodycorpAgmTime.Value.ToString(@"hh\:mm");//
                        LabelBeginDate.Text = bodycorp.BodycorpBeginDate.ToShortDateString();//
                        LabelClosePeriodDate.Text = bodycorp.BodycorpClosePeriodDate.ToShortDateString();//
                        if (bodycorp.BodycorpCommitteeDate.HasValue)
                            LabelCommitteeDate.Text = bodycorp.BodycorpCommitteeDate.Value.ToShortDateString();//
                        if (bodycorp.BodycorpCommitteeTime.HasValue)
                            LabelCommitteeTime.Text = bodycorp.BodycorpCommitteeTime.Value.ToString(@"hh\:mm");//
                        if (bodycorp.BodycorpInactiveDate.HasValue)
                            LabelInactiveDate.Text = bodycorp.BodycorpInactiveDate.Value.ToShortDateString();//
                        if (bodycorp.BodycorpEgmDate.HasValue)
                            LabelEgmDate.Text = bodycorp.BodycorpEgmDate.Value.ToShortDateString();//
                        if (bodycorp.BodycorpEgmTime.HasValue)
                            LabelEgmTime.Text = bodycorp.BodycorpEgmTime.Value.ToString(@"hh\:mm");//
                        LabelInactive.Text = bodycorp.BodycorpInactive ? "Yes" : "No";
                        ChartMaster chart = new ChartMaster(constr);
                        chart.LoadData(bodycorp.BodycorpAccountId);
                        LabelAccount.Text = chart.ChartMasterCode;
                        CheckBoxGST.Checked = !bodycorp.BodycorpNoGST;

                        TextBoxNotes.Text = bodycorp.BodycorpNotes;
                        CheckBoxDiscount.Checked = bodycorp.BodycorpDiscount;   // Add 07/06/2016
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    #endregion
                }
                HiddenBCID.Value = Request.Cookies["bodycorpid"].Value;
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            try
            {
                string[] args = eventArgument.Split('|');
                if (args[0] == "ButtonDeleteComm")
                {
                    ButtonDeleteComm_Click(args);
                }
                else if (args[0] == "ButtonDownload")
                {
                    ButtonDownload_Click(args);
                }
                else if (args[0] == "ButtonDelete")
                {
                    ButtonDelete_Click(args);
                }
                else if (args[0] == "ButtonEnter")
                {
                    ButtonEnter_Click(args);
                }
                else
                    throw new Exception("Error: unknown command!");
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridCommsDataBind(string postdata, string bodycorpid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Bodycorp bodycorp = new Bodycorp(constr);
                bodycorp.LoadData(Convert.ToInt32(bodycorpid));
                Comms<Bodycorp> comms = new Comms<Bodycorp>(constr);
                comms.LoadData(bodycorp);
                ArrayList commList = comms.Communications;
                ArrayList rowData = new ArrayList();
                int id = 1;
                foreach (Comm<Bodycorp> comm in commList)
                {
                    Hashtable items = new Hashtable();
                    items.Add("id", id);
                    ArrayList cells = new ArrayList();
                    cells.Add(comm.CommMasterId.ToString());
                    CommType commType = new CommType(constr);
                    commType.LoadData(comm.CommMasterTypeId);
                    cells.Add(commType.CommTypeCode);
                    cells.Add(comm.CommMasterData);
                    cells.Add(DBSafeUtils.DBBoolToStr(comm.CommMasterPrimary));
                    cells.Add(id.ToString());
                    items.Add("cell", cells);
                    rowData.Add(items);
                    id++;
                }
                JQGrid jqgridObj = new JQGrid(postdata, constr, rowData);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }

        [System.Web.Services.WebMethod]
        public static string DataGridCommsTypeSelect()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";

                CommTypes commTypes = new CommTypes(constr);
                //CommTypes commTypes = new CommTypes("dsn=sapp_sms2;UID=root;PWD=1password!;  OPTION=1; Pooled=True;");
                commTypes.LoadData();
                foreach (CommType commType in commTypes.CommTypeList)
                {
                    html += "<option value='" + commType.CommTypeId.ToString() + "'>" + commType.CommTypeCode + "</option>";
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
        public static string jqGridLogsDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string bodycorp_id = HttpContext.Current.Request.Cookies["bodycorpid"].Value;
            DataTable dt = new DataTable("logs");
            dt.Columns.Add("ID");
            dt.Columns.Add("User");
            dt.Columns.Add("Action");
            dt.Columns.Add("DateTime");
            dt.Columns.Add("Details");
            Odbc mydb = null;
            try
            {
                mydb = new Odbc(constr);
                string sql = "SELECT * FROM `log_revs` LEFT JOIN `sapp_general`.`users` ON `log_rev_user_id`=`user_id` WHERE `log_rev_table`='bodycorps' AND `log_rev_table_id`=" + bodycorp_id;
                OdbcDataReader dr = mydb.Reader(sql);
                while (dr.Read())
                {
                    DataRow nr = dt.NewRow();
                    nr["ID"] = dr["log_rev_id"].ToString();
                    nr["User"] = dr["user_login"].ToString();
                    nr["Action"] = dr["log_rev_type"].ToString();
                    string datetime = Convert.ToDateTime(dr["log_rev_time"]).ToString("d/m/yyy HH:mm:ss");
                    nr["DateTime"] = datetime;
                    string details = "";
                    sql = "SELECT * FROM `log_master` WHERE `log_master_rev_id`=" + dr["log_rev_id"].ToString();
                    OdbcDataReader dr2 = mydb.Reader(sql);
                    while (dr2.Read())
                    {
                        details += dr2["log_master_column"].ToString() + ":";
                        details += dr2["log_master_data"].ToString() + ",";
                    }
                    if (details != "") details = details.Substring(0, details.Length - 1);
                    nr["Details"] = details;
                    dt.Rows.Add(nr);
                }

                string sqlfromstr = "FROM `" + dt.TableName + "`";
                string sqlselectstr = "SELECT `ID`, `User`, `Action`, `DateTime`, `Details` FROM (SELECT *";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
            return "{}";
        }

        [System.Web.Services.WebMethod]
        public static string jqGridFilesDataBind(string postdata)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string bodycorp_id = HttpContext.Current.Request.Cookies["bodycorpid"].Value;
            DataTable dt = new DataTable("files");
            dt.Columns.Add("ID");
            dt.Columns.Add("FileName");
            dt.Columns.Add("Date");
            dt.Columns.Add("Type");
            dt.Columns.Add("Size");

            Sapp.SMS.System system = new Sapp.SMS.System(constr);
            system.LoadData("FILEFOLDER");
            string filefolder = system.SystemValue;
            Bodycorp bodycorp = new Bodycorp(constr);
            bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
            string bodycorp_code = bodycorp.BodycorpCode;
            filefolder += bodycorp_code + "\\";

            if (!Directory.Exists(filefolder))
            {
                Directory.CreateDirectory(filefolder);
            }

            if (HttpContext.Current.Session["bodycorpdetails_file_dir"] != null)
            {
                filefolder += HttpContext.Current.Session["bodycorpdetails_file_dir"].ToString();
            }

            int id = 0;
            DirectoryInfo dirInfo = new DirectoryInfo(filefolder);
            foreach (DirectoryInfo di in dirInfo.GetDirectories())
            {
                string at = di.Attributes.ToString();
                if (!at.Contains("Hidden"))
                {
                    DataRow nr = dt.NewRow();
                    nr["ID"] = id;
                    nr["FileName"] = di.Name;
                    nr["Date"] = di.CreationTime.ToString("dd/MM/yyyy");
                    nr["Type"] = "DIR";
                    nr["Size"] = "";
                    dt.Rows.Add(nr);
                    id++;
                }
            }

            FileInfo[] fileInfos = dirInfo.GetFiles();

            foreach (FileInfo fi in fileInfos)
            {
                string at = fi.Attributes.ToString();
                if (!at.Contains("Hidden"))
                {
                    DataRow nr = dt.NewRow();
                    nr["ID"] = id;
                    nr["FileName"] = fi.Name;
                    nr["Date"] = fi.CreationTime.ToString("dd/MM/yyyy");
                    nr["Type"] = fi.Extension.Substring(1).ToUpper();
                    nr["Size"] = Convert.ToInt32(fi.Length / 1000 + 1).ToString() + "KB";
                    dt.Rows.Add(nr);
                    id++;
                }
            }

            string sqlfromstr = "FROM `" + dt.TableName + "`";
            string sqlselectstr = "SELECT `ID`, `FileName`, `Date`, `Type`, `Size` FROM (SELECT *";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }

        [System.Web.Services.WebMethod]
        public static void DataGridCommsSave(string rowValue, string bodycorpid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                Hashtable rowObj = (Hashtable)JSON.JsonDecode(rowValue);
                Hashtable items = new Hashtable();
                #region Retireve Values
                CommType commType = new CommType(constr);
                commType.LoadData(rowObj["Type"].ToString());
                items.Add("comm_master_type_id", commType.CommTypeId);
                items.Add("comm_master_data", DBSafeUtils.StrToQuoteSQL(rowObj["Details"].ToString()));

                // Update 28/04/2016
                //items.Add("comm_master_primary", Convert.ToInt32(rowObj["Primary"]));
                if (rowObj["Primary"].ToString().Equals("Yes"))
                {
                    items.Add("comm_master_primary", 1);
                }
                else
                {
                    items.Add("comm_master_primary", 0);
                }
                #endregion
                if (rowObj["ID"].ToString() != "")
                {
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(Convert.ToInt32(bodycorpid));
                    Comm<Bodycorp> comm = new Comm<Bodycorp>(constr);
                    comm.Update(items, Convert.ToInt32(rowObj["ID"]));
                }
                else
                {
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(Convert.ToInt32(bodycorpid));
                    Comm<Bodycorp> comm = new Comm<Bodycorp>(constr);
                    comm.Add(items, bodycorp);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        #region WebControl Events

        private void ButtonDeleteComm_Click(string[] args)
        {

            try
            {
                string comm_master_id = "";
                comm_master_id = args[1];
                string bodycorp_id = "";
                bodycorp_id = Request.Cookies["bodycorpid"].Value;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Bodycorp bodycorp = new Bodycorp(constr);
                bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                Comm<Bodycorp> comm = new Comm<Bodycorp>(constr);
                comm.Delete(Convert.ToInt32(comm_master_id), bodycorp);
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        protected void ImageButtonEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string bodycorp_id = "";
                bodycorp_id = Request.Cookies["bodycorpid"].Value;
                Response.BufferOutput = true;
                Response.Redirect("~/bodycorpedit.aspx?mode=edit&bodycorpid=" + bodycorp_id, false);
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        protected void ImageButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string bodycorp_id = Request.Cookies["bodycorpid"].Value;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Bodycorp bodycorp = new Bodycorp(constr);
                bodycorp.Delete(Convert.ToInt32(bodycorp_id));
                Response.BufferOutput = true;
                Response.Redirect("goback.aspx", false);
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        protected void ImageButtonProperty_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("propertymaster.aspx?bodycorpid=" + Request.Cookies["bodycorpid"].Value, false);
        }

        protected void ImageButtonDebtor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Session["BID"] = Request.Cookies["bodycorpid"].Value;
                Response.Redirect("debtormaster.aspx?bodycorpid=" + Request.Cookies["bodycorpid"].Value, false);
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        protected void ImageButtonContacts_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("contactmasters.aspx?bodycorpid=" + Request.Cookies["bodycorpid"].Value, false);
        }

        protected void ImageButtonBudget_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void ImageButtonManager_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.Redirect("bodycorpmanagers.aspx?bodycorpid=" + Request.Cookies["bodycorpid"].Value, false);
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        protected void ButtonUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (FileUpload1.HasFile)
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    string bodycorp_id = HttpContext.Current.Session["bodycorpid"].ToString();
                    Sapp.SMS.System system = new Sapp.SMS.System(constr);
                    system.LoadData("FILEFOLDER");
                    string filefolder = system.SystemValue;
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                    string bodycorp_code = bodycorp.BodycorpCode;
                    filefolder += bodycorp_code;

                    if (!Directory.Exists(filefolder))
                    {
                        Directory.CreateDirectory(filefolder);
                    }

                    DirectoryInfo dirInfo = new DirectoryInfo(filefolder);
                    FileInfo[] fileInfos = dirInfo.GetFiles();

                    foreach (FileInfo fi in fileInfos)
                    {
                        if ((fi.Name + "." + fi.Extension) == FileUpload1.FileName)
                        {
                            throw new Exception("Error: file already exsit!");
                        }
                    }
                    FileUpload1.SaveAs(filefolder + "\\" + FileUpload1.FileName);
                }
                else
                {
                    throw new Exception("Error: please select a data file!");
                }
            }
            catch (Exception ex)
            {
                //ClientScriptManager c = Page.ClientScript;
                //ErrorMessage1.Message = ex.Message;
                //c.RegisterStartupScript(this.GetType(), "", "<script>$('#ErrorDIVB').show();$('#ErrorDIV').show();</script>");
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ButtonDownload_Click(string[] args)
        {
            try
            {
                string filename = "";
                filename = args[1];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = HttpContext.Current.Session["bodycorpid"].ToString();
                Sapp.SMS.System system = new Sapp.SMS.System(constr);
                system.LoadData("FILEFOLDER");
                string filefolder = system.SystemValue;
                Bodycorp bodycorp = new Bodycorp(constr);
                bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                string bodycorp_code = bodycorp.BodycorpCode;
                filefolder += bodycorp_code;
                filename = filefolder + "\\" + filename;
                FileInfo file = new FileInfo(filename);
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
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        protected void ButtonDelete_Click(string[] args)
        {
            try
            {
                string filename = "";
                string type = "";
                filename = args[1];
                type = args[2];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string bodycorp_id = HttpContext.Current.Session["bodycorpid"].ToString();
                Sapp.SMS.System system = new Sapp.SMS.System(constr);
                system.LoadData("FILEFOLDER");
                string filefolder = system.SystemValue;
                Bodycorp bodycorp = new Bodycorp(constr);
                bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                string bodycorp_code = bodycorp.BodycorpCode;
                filefolder += bodycorp_code;
                if (type == "DIR")
                {
                    filename = filefolder + "\\" + filename;
                    System.IO.Directory.Delete(filename, true);
                }
                else
                {
                    filename = filefolder + "\\" + filename;
                    File.Delete(filename);
                }
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        protected void ButtonHome_Click(object sender, EventArgs e)
        {
            try
            {
                Session["bodycorpdetails_file_dir"] = "";
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        protected void ButtonEnter_Click(string[] args)
        {
            try
            {
                if (Session["bodycorpdetails_file_dir"] != null)
                {
                    Session["bodycorpdetails_file_dir"] = Session["bodycorpdetails_file_dir"] + "\\" + args[1];
                }
                else
                {
                    Session["bodycorpdetails_file_dir"] = args[1];
                }
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        protected void ButtonReturn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["bodycorpdetails_file_dir"] != null)
                {
                    string file_dir = Session["bodycorpdetails_file_dir"].ToString();
                    string[] dirs = file_dir.Split('\\');
                    file_dir = "";
                    if (dirs.Length > 1)
                    {
                        for (int i = 0; i < (dirs.Length - 1); i++)
                        {
                            file_dir += dirs[i] + "\\";
                        }
                        file_dir = file_dir.Substring(0, file_dir.Length - 1);
                    }
                    Session["bodycorpdetails_file_dir"] = file_dir;
                }
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        protected void ImageButtonUnitplan_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string bodycorp_id = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (Request.Cookies["bodycorpid"].Value != null)
                {
                    bodycorp_id = Request.Cookies["bodycorpid"].Value;
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                    List<PropertyMaster> propertyList = bodycorp.GetPropertyList();
                    foreach (PropertyMaster property in propertyList)
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("unitmaster.aspx?propertyid=" + property.PropertyMasterId + "&bodycorpid=" + bodycorp_id, false);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }

        }

        protected void ImageButtonMaintenance_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string bodycorp_id = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (Request.Cookies["bodycorpid"].Value != null)
                {
                    bodycorp_id = Request.Cookies["bodycorpid"].Value;
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                    List<PropertyMaster> propertyList = bodycorp.GetPropertyList();
                    foreach (PropertyMaster property in propertyList)
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("pptymaintmaster.aspx?propertyid=" + property.PropertyMasterId, false);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }


        protected void ImageButtonValuations_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string bodycorp_id = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (Request.Cookies["bodycorpid"].Value != null)
                {
                    bodycorp_id = Request.Cookies["bodycorpid"].Value;
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                    List<PropertyMaster> propertyList = bodycorp.GetPropertyList();
                    foreach (PropertyMaster property in propertyList)
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("pptyvtmaster.aspx?propertyid=" + property.PropertyMasterId, false);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }


        protected void ImageButtonTitles_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string bodycorp_id = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (Request.Cookies["bodycorpid"].Value != null)
                {
                    bodycorp_id = Request.Cookies["bodycorpid"].Value;
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                    List<PropertyMaster> propertyList = bodycorp.GetPropertyList();
                    foreach (PropertyMaster property in propertyList)
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("pptytitles.aspx?propertyid=" + property.PropertyMasterId, false);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        protected void ImageButtonMortgages_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string bodycorp_id = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (Request.Cookies["bodycorpid"].Value != null)
                {
                    bodycorp_id = Request.Cookies["bodycorpid"].Value;
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                    List<PropertyMaster> propertyList = bodycorp.GetPropertyList();
                    foreach (PropertyMaster property in propertyList)
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("pptymtgs.aspx?propertyid=" + property.PropertyMasterId, false);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        protected void ImageButtonContractor_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string bodycorp_id = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (Request.Cookies["bodycorpid"].Value != null)
                {
                    bodycorp_id = Request.Cookies["bodycorpid"].Value;
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                    List<PropertyMaster> propertyList = bodycorp.GetPropertyList();
                    foreach (PropertyMaster property in propertyList)
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("pptycntrmaster.aspx?propertyid=" + property.PropertyMasterId, false);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }

        protected void ImageButtonInsurance_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string bodycorp_id = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (Request.Cookies["bodycorpid"].Value != null)
                {
                    bodycorp_id = Request.Cookies["bodycorpid"].Value;
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                    List<PropertyMaster> propertyList = bodycorp.GetPropertyList();
                    foreach (PropertyMaster property in propertyList)
                    {
                        Response.BufferOutput = true;
                        Response.Redirect("pptyinsmaster.aspx?propertyid=" + property.PropertyMasterId, false);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScriptManager c = Page.ClientScript;
                c.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('" + ex.ToString() + "');</script>");
                //HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
            }
        }


        #endregion

        protected void AccountB_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/bodycorpsaccount.aspx?bodycorpid=" + Request.Cookies["bodycorpid"].Value, false);
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/interest.aspx?bodycorpid=" + Request.Cookies["bodycorpid"].Value, false);
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/SuperSearch.aspx?bodycorpid=" + Request.Cookies["bodycorpid"].Value, false);
        }


    }
}
