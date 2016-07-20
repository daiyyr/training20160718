using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.IO;
using Sapp.SMS;
using System.Collections;
using Sapp.Common;
using Sapp.Data;
using Sapp.General;

namespace sapp_sms
{
    public partial class bodycorpedit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string bodycorp_id = "";
            string mode = "";
            try
            {
                if (Request.Cookies["bodycorpid"].Value != null) bodycorp_id = Request.Cookies["bodycorpid"].Value;
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (!IsPostBack)
                {

                    if (mode == "add")
                    {
                        #region Load Page
                        try
                        {
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxAccount, "SELECT `chart_master_id` AS `ID`, `chart_master_code` AS `Code` FROM `chart_master` WHERE `chart_master_type_id`=3", "ID", "Code", constr, true);
                            DataTable dt = new DataTable();
                            dt.Columns.Add("ID");
                            dt.Columns.Add("Code");
                            DataTable dt2 = new DataTable();
                            dt2.Columns.Add("ID");
                            dt2.Columns.Add("Code");
                            string template_dir = Server.MapPath("~/templates");
                            DirectoryInfo dirInfo = new DirectoryInfo(template_dir);
                            FileInfo[] fileInfos = dirInfo.GetFiles();
                            foreach (FileInfo fi in fileInfos)
                            {
                                if (fi.Name.Contains("invoice"))
                                {
                                    DataRow nr = dt.NewRow();
                                    nr["ID"] = fi.Name;
                                    nr["Code"] = fi.Name;
                                    dt.Rows.Add(nr);
                                }
                                else if (fi.Name.Contains("statement"))
                                {
                                    DataRow nr = dt2.NewRow();
                                    nr["ID"] = fi.Name;
                                    nr["Code"] = fi.Name;
                                    dt2.Rows.Add(nr);
                                }
                            }
                            AjaxControlUtils.SetupComboBox(ComboBoxInvTpl, dt, "ID", "Code", false);
                            AjaxControlUtils.SetupComboBox(ComboBoxStmtTpl, dt2, "ID", "Code", false);
                            #endregion
                            #region Initial Web Controls
                            TextBoxAgmDate.Text = "";
                            TextBoxAgmTime.Text = "";
                            TextBoxEgmDate.Text = "";
                            TextBoxEgmTime.Text = "";
                            TextBoxCode.Text = "";
                            TextBoxCommitteeDate.Text = "";
                            TextBoxGST.Text = "";
                            TextBoxInactiveDate.Text = "";
                            TextBoxName.Text = "";
                            TextBoxNotes.Text = "";

                            //TextBoxBeginDate.Text = "01/01/0002";
                            TextBoxCloseDate.Text = "01/01/0001";
                            TextBoxCloseDate.Enabled = false;
                            CheckBoxGST.Checked = true;
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
                    if (mode == "edit")
                    {
                        #region Security Check
                        string login = HttpContext.Current.User.Identity.Name;
                        Sapp.General.User user = new User(constr_general);
                        user.LoadData(login);
                        BodycorpManager bm = new BodycorpManager(constr);
                        string security_type = bm.GetSecurityType(Convert.ToInt32(bodycorp_id), user.UserId, constr_general);
                        if (security_type != "WRITE")
                        {
                            Response.BufferOutput = true;
                            Response.Redirect("warning.aspx", false);
                        }
                        #endregion
                        #region Load Page
                        try
                        {
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxAccount, "SELECT `chart_master_id` AS `ID`, `chart_master_code` AS `Code` FROM `chart_master` WHERE `chart_master_type_id`=3", "ID", "Code", constr, true);
                            DataTable dt = new DataTable();
                            dt.Columns.Add("ID");
                            dt.Columns.Add("Code");
                            DataTable dt2 = new DataTable();
                            dt2.Columns.Add("ID");
                            dt2.Columns.Add("Code");
                            string template_dir = Server.MapPath("~/templates");
                            DirectoryInfo dirInfo = new DirectoryInfo(template_dir);
                            FileInfo[] fileInfos = dirInfo.GetFiles();
                            foreach (FileInfo fi in fileInfos)
                            {
                                if (fi.Name.Contains("invoice"))
                                {
                                    DataRow nr = dt.NewRow();
                                    nr["ID"] = fi.Name;
                                    nr["Code"] = fi.Name;
                                    dt.Rows.Add(nr);
                                }
                                else if (fi.Name.Contains("statement"))
                                {
                                    DataRow nr = dt2.NewRow();
                                    nr["ID"] = fi.Name;
                                    nr["Code"] = fi.Name;
                                    dt2.Rows.Add(nr);
                                }
                            }
                            AjaxControlUtils.SetupComboBox(ComboBoxInvTpl, dt, "ID", "Code", false);
                            AjaxControlUtils.SetupComboBox(ComboBoxStmtTpl, dt2, "ID", "Code", false);
                            #endregion
                            #region Load Web Controls
                            Bodycorp bodycorp = new Bodycorp(constr);
                            bodycorp.LoadData(Convert.ToInt32(bodycorp_id));
                            AjaxControlUtils.ComboBoxSelection(ComboBoxAccount, bodycorp.BodycorpAccountId, true);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxInvTpl, bodycorp.BodycorpInvTpl, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxStmtTpl, bodycorp.BodycorpStmtTpl, false);
                            TextBoxCloseDate.Text = bodycorp.Bodycorp_Close_Off.ToString("dd/MM/yyyy");
                            if (bodycorp.BodycorpAgmDate.HasValue)
                                TextBoxAgmDate.Text = bodycorp.BodycorpAgmDate.Value.ToShortDateString();
                            if (bodycorp.BodycorpAgmTime.HasValue)
                            {
                                TextBoxAgmTime.Text = bodycorp.BodycorpAgmTime.Value.ToString(@"hh\:mm");
                            }
                            if (bodycorp.BodycorpEgmDate.HasValue)
                                TextBoxEgmDate.Text = bodycorp.BodycorpEgmDate.Value.ToShortDateString();
                            if (bodycorp.BodycorpEgmTime.HasValue)
                            {
                                TextBoxEgmTime.Text = bodycorp.BodycorpEgmTime.Value.ToString(@"hh\:mm");
                            }
                            TextBoxCode.Text = bodycorp.BodycorpCode;
                            if (bodycorp.BodycorpCommitteeDate.HasValue)
                                TextBoxCommitteeDate.Text = bodycorp.BodycorpCommitteeDate.Value.ToShortDateString();
                            if (bodycorp.BodycorpCommitteeTime.HasValue)
                            {
                                TextBoxCommitteeTime.Text = bodycorp.BodycorpCommitteeTime.Value.ToString(@"hh\:mm");
                            }
                            TextBoxClosePeriodDate.Text = bodycorp.BodycorpClosePeriodDate.ToString("dd/MM/yyyy");
                            TextBoxGST.Text = bodycorp.BodycorpGST.ToString();
                            if (bodycorp.BodycorpInactiveDate.HasValue)
                                TextBoxInactiveDate.Text = bodycorp.BodycorpInactiveDate.Value.ToShortDateString();
                            TextBoxName.Text = bodycorp.BodycorpName.ToString();
                            TextBoxNotes.Text = bodycorp.BodycorpNotes.ToString();
                            TextBoxBeginDate.Text = DBSafeUtils.DBDateToStr(bodycorp.BodycorpBeginDate);
                            CheckBoxGST.Checked = !bodycorp.BodycorpNoGST;
                            CheckBoxDiscount.Checked = bodycorp.BodycorpDiscount;   // Add 20/05/2016
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
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
                string bodycorp_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string constr_general = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
                if (!Page.IsValid) return;
                if (Request.Cookies["bodycorpid"].Value != null) bodycorp_id = Request.Cookies["bodycorpid"].Value;
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                string login = HttpContext.Current.User.Identity.Name;
                Sapp.General.User user = new User(constr_general);
                user.LoadData(login);
                #region Retireve Values

                Hashtable items = new Hashtable();

                items.Add("bodycorp_code", DBSafeUtils.StrToQuoteSQL(TextBoxCode.Text));
                items.Add("bodycorp_name", DBSafeUtils.StrToQuoteSQL(TextBoxName.Text));
                items.Add("bodycorp_nogst", DBSafeUtils.BoolToSQL(!CheckBoxGST.Checked));
                items.Add("bodycorp_gst", DBSafeUtils.StrToQuoteSQL(TextBoxGST.Text));
                items.Add("bodycorp_discount", DBSafeUtils.BoolToSQL(CheckBoxDiscount.Checked));   // Add 20/05/2016
                items.Add("bodycorp_account_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxAccount, true));
                items.Add("bodycorp_inv_tpl", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxInvTpl, false));
                items.Add("bodycorp_stmt_tpl", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxStmtTpl, false));
                items.Add("bodycorp_agm_date", DBSafeUtils.DateToSQL(TextBoxAgmDate.Text));
                items.Add("bodycorp_agm_time", DBSafeUtils.TimeToSQL(TextBoxAgmTime.Text));
                items.Add("bodycorp_egm_date", DBSafeUtils.DateToSQL(TextBoxEgmDate.Text));
                items.Add("bodycorp_egm_time", DBSafeUtils.TimeToSQL(TextBoxEgmTime.Text));
                items.Add("bodycorp_committee_date", DBSafeUtils.DateToSQL(TextBoxCommitteeDate.Text));
                items.Add("bodycorp_committee_time", DBSafeUtils.TimeToSQL(TextBoxCommitteeTime.Text));
                items.Add("bodycorp_notes", DBSafeUtils.StrToQuoteSQL(TextBoxNotes.Text));
                items.Add("bodycorp_begin_date", DBSafeUtils.DateToSQL(TextBoxBeginDate.Text));

                items.Add("bodycorp_close_period_date", DBSafeUtils.DateToSQL(TextBoxClosePeriodDate.Text));
                items.Add("bodycorp_inactive", DBSafeUtils.BoolToSQL(CheckBoxInactive.Checked));
                items.Add("bodycorp_inactive_date", DBSafeUtils.DateToSQL(TextBoxInactiveDate.Text));
                items.Add("bodycorp_close_off", DBSafeUtils.DateToSQL(TextBoxCloseDate.Text));

                #endregion
                if (mode == "add")
                {
                    #region Save
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.SetLog(user.UserId);
                    items["bodycorp_close_off"] = "'0001-01-01'";
                    items["bodycorp_begin_date"] = DBSafeUtils.DateTimeToSQL(TextBoxBeginDate.Text);
                    bodycorp.Add(items);
                    Response.BufferOutput = true;
                    //Response.Redirect("goback.aspx", false);
                    Response.Redirect("~/bodycorps.aspx?bodycorpid=" + bodycorp_id, false);    //Update 07/06/2016
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Save
                    Bodycorp bodycorp = new Bodycorp(constr);
                    bodycorp.SetLog(user.UserId);
                    bodycorp.Update(items, Convert.ToInt32(bodycorp_id));
                    Response.BufferOutput = true;
                    //Response.Redirect("goback.aspx", false);
                    Response.Redirect("~/bodycorps.aspx?bodycorpid=" + bodycorp_id, false);    //Update 07/06/2016
                    #endregion
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/bodycorps.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void CustomValidatorAccount_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;
                if (ComboBoxAccount.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected


            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonSave0_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void CustomValidatorInvTpl_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;
                if (ComboBoxInvTpl.SelectedIndex >= 0) args.IsValid = true;

            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void CustomValidatorStmtTpl_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;
                if (ComboBoxStmtTpl.SelectedIndex >= 0) args.IsValid = true;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }



        }
    }
}