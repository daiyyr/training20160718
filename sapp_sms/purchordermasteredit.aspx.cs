using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.Common;
using Sapp.SMS;
using System.Collections;
using Sapp.Data;
using Sapp.JQuery;
using System.IO;
using System.Text.RegularExpressions;

namespace sapp_sms
{
    public partial class purchordermasteredit : System.Web.UI.Page, IPostBackEventHandler
    {
        private string CheckedQueryString()
        {
            if (null != Request.QueryString["creditorid"] && Regex.IsMatch(Request.QueryString["creditorid"], "^[0-9]*$"))
            {
                return Request.QueryString["creditorid"];
            }
            return "";
        }
        private string NewQueryString(string connector)
        {
            string result = CheckedQueryString();
            return string.IsNullOrEmpty(result) ? "" : connector + "creditorid=" + result;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Javascript setup
            Control[] wc = { jqGridTrans, LabelNet, LabelTax, LabelGross };
            JSUtils.RenderJSArrayWithCliendIds(Page, wc);
            #endregion
            string purchordermaster_id = "";
            string mode = "";
            try
            {
                if (Request.QueryString["purchordermasterid"] != null) purchordermaster_id = Request.QueryString["purchordermasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                #region Javascript Setup
                #endregion
                if (!IsPostBack)
                {
                    if (mode == "add")
                    {
                        #region Load Page
                        try
                        {
                            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `purchorder_type_id` AS `ID`, `purchorder_type_code` AS `Code` FROM `purchorder_types`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxCreditor, "SELECT `creditor_master_id` AS `ID`, `creditor_master_code` AS `Code` FROM `creditor_master`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` LEFT JOIN `property_master` ON property_master_id=unit_master_property_id where property_master_bodycorp_id =" + Request.Cookies["bodycorpid"].Value + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by LENGTH(Code), Code", "ID", "Code", constr, true);
                            #endregion

                            #region Initial Web Controls
                            TextBoxApproval.Text = "";
                            LabelGross.Text = "";
                            LabelNet.Text = "";
                            LabelTax.Text = "";
                            PurchorderMaster purchorder = new PurchorderMaster(constr);
                            TextBoxNum.Text = purchorder.GetNextPONumber();
                            TextBoxDate.Text = "";
                            #endregion

                            #region Create Temp File
                            string session_id = HttpContext.Current.Session.SessionID;
                            string filename = HttpContext.Current.Server.MapPath("~/json/PurchOrder/" + session_id);
                            if (File.Exists(filename))
                                File.Delete(filename);
                            PurchorderMaster purObj = new PurchorderMaster(constr);
                            purObj.PurchorderTranList = new ArrayList();
                            purObj.ExportJSON(session_id);
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
                        #region Load Page
                        try
                        {
                            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxType, "SELECT `purchorder_type_id` AS `ID`, `purchorder_type_code` AS `Code` FROM `purchorder_types`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxCreditor, "SELECT `creditor_master_id` AS `ID`, `creditor_master_code` AS `Code` FROM `creditor_master`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` LEFT JOIN `property_master` ON property_master_id=unit_master_property_id where property_master_bodycorp_id =" + Request.Cookies["bodycorpid"].Value + "  and unit_master_inactive_date is null and unit_master_type_id <>5 order by LENGTH(Code), Code", "ID", "Code", constr, true);
                            #endregion

                            #region Initial Web Controls
                            PurchorderMaster purchordermaster = new PurchorderMaster(constr);
                            purchordermaster.LoadData(Convert.ToInt32(purchordermaster_id));
                            TextBoxDate.Text = purchordermaster.PurchorderMasterDate.ToShortDateString();
                            TextBoxApproval.Text = purchordermaster.PurchorderMasterApproval.ToString();
                            LabelGross.Text = purchordermaster.PurchorderMasterGross.ToString();
                            LabelNet.Text = purchordermaster.PurchodrerMasterNet.ToString();
                            TextBoxNum.Text = purchordermaster.PurchorderMasterNum.ToString();
                            LabelTax.Text = purchordermaster.PurchorderMasterTax.ToString();
                            TextBoxDate.Text = purchordermaster.PurchorderMasterDate.ToShortDateString();
                            TextBoxNote.Text = purchordermaster.PurchorderMasterDescription;
                            AjaxControlUtils.ComboBoxSelection(ComboBoxType, purchordermaster.PurchorderMasterTypeId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxUnit, purchordermaster.PurchorderMasterUnitId, true);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxCreditor, purchordermaster.PurchorderMasterCreditorId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxBodycorp, purchordermaster.PurchorderMasterBodycorpId, false);
                            #endregion
                            #region Create Temp File
                            string session_id = HttpContext.Current.Session.SessionID;
                            string filename = HttpContext.Current.Server.MapPath("~/json/PurchOrder/" + session_id);
                            if (File.Exists(filename))
                                File.Delete(filename);
                            purchordermaster.LoadTransactions();
                            purchordermaster.ExportJSON(session_id);
                            #endregion
                            #region Calculate Net Tax Gross
                            decimal sumNet = 0;
                            foreach (PurchorderTrans ptrans in purchordermaster.PurchorderTranList)
                            {
                                sumNet += ptrans.PurchorderTranNet;
                            }
                            LabelNet.Text = sumNet.ToString();
                            LabelTax.Text = ((Decimal)((double)sumNet * 0.15)).ToString();
                            LabelGross.Text = ((Decimal)((double)sumNet * 1.15)).ToString();
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
                    }
                    if (null != Request.QueryString["creditorid"])
                    {
                        ComboBoxCreditor.SelectedValue = Request.QueryString["creditorid"];
                        ComboBoxCreditor.Enabled = false;
                    }
                    if (Request.Cookies["bodycorpid"] != null)
                    {
                        ComboBoxBodycorp.SelectedValue = Request.Cookies["bodycorpid"].Value;
                        ComboBoxBodycorp.Enabled = false;
                    }
                }
                if ("XMLHttpRequest" != Request.Headers["X-Requested-With"])
                {
                    Session["purchordermasterid"] = Request.QueryString["purchordermasterid"];
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
                string purchordermaster_id = "";
                string mode = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable items = new Hashtable();
                if (!Page.IsValid) return;
                if (Request.QueryString["purchordermasterid"] != null) purchordermaster_id = Request.QueryString["purchordermasterid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                else throw new Exception("Mode is missing!");

                #region Preparations
                PurchorderMaster pmaster = new PurchorderMaster(constr);
                string session_id = HttpContext.Current.Session.SessionID;
                pmaster.ImportJSON(session_id);
                decimal sumNet = 0;
                foreach (PurchorderTrans ptrans in pmaster.PurchorderTranList)
                {
                    ChartMaster chart = new ChartMaster(constr);
                    chart.LoadData(ptrans.PurchorderTranChartCode);
                    ptrans.PurchorderTranChartId = chart.ChartMasterId;
                    ptrans.PurchorderTranDate = Convert.ToDateTime(TextBoxDate.Text);
                    sumNet += ptrans.PurchorderTranNet;
                }
                #endregion

                #region Retireve Values

                items.Add("purchorder_master_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxType, false));
                items.Add("purchorder_master_unit_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxUnit, true));
                items.Add("purchorder_master_creditor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxCreditor, false));
                items.Add("purchorder_master_bodycorp_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxBodycorp, false));
                items.Add("purchorder_master_num", DBSafeUtils.StrToQuoteSQL(TextBoxNum.Text));
                items.Add("purchorder_master_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                items.Add("purchorder_master_approval", DBSafeUtils.IntToSQL(TextBoxApproval.Text));
                items.Add("purchodrer_master_net", DBSafeUtils.DecimalToSQL(sumNet));
                items.Add("purchorder_master_tax", DBSafeUtils.DecimalToSQL(sumNet * (decimal)0.15));
                items.Add("purchorder_master_gross", DBSafeUtils.DecimalToSQL(sumNet * (decimal)1.15));
                items.Add("purchorder_master_description", DBSafeUtils.StrToQuoteSQL(TextBoxNote.Text));
                #endregion
                #region Save
                if (mode == "add")
                {
                    #region Save
                    items.Add("purchorder_master_allocated", DBSafeUtils.BoolToSQL(false));
                    PurchorderMaster purchmaster = new PurchorderMaster(constr);
                    purchmaster.Add(items);// Save Purchase Order Master data to database
                    #endregion

                }
                else if (mode == "edit")
                {
                    #region Save
                    PurchorderMaster purchmaster = new PurchorderMaster(constr);
                    purchmaster.Update(items, Convert.ToInt32(purchordermaster_id));
                    purchmaster.DeleteRelatedTransactions(Convert.ToInt32(purchordermaster_id));
                    #endregion

                }
                pmaster.SaveTransactions(DBSafeUtils.StrToQuoteSQL(TextBoxNum.Text));//Save Purchase Order Transactions
                pmaster.RemoveJSON(HttpContext.Current.Session.SessionID);
                #endregion
                #region Redirect
                Response.BufferOutput = true;
                Response.Redirect("goback.aspx", false);
                #endregion
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonClose_Click(object sender, ImageClickEventArgs e)
        {
            #region Redirect
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                PurchorderMaster purorder = new PurchorderMaster(constr);
                purorder.RemoveJSON(HttpContext.Current.Session.SessionID);
                Response.BufferOutput = true;
                Response.Redirect("~/purchordermaster.aspx" + NewQueryString("?"),false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            #endregion
        }
        #endregion

        #region Validation
        protected void CustomValidatorType_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxType.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
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
        protected void CustomValidatorCreditor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxCreditor.SelectedIndex >= 0) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion


        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridDataBind(string postdata)
        {
            try
            {
                string session_id = HttpContext.Current.Session.SessionID;
                string filename = HttpContext.Current.Server.MapPath("~/json/PurchOrder/" + session_id);
                if (File.Exists(filename))
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    PurchorderMaster purObj = new PurchorderMaster(constr);
                    purObj.ImportJSON(session_id);
                    ArrayList purTranList = purObj.PurchorderTranList;
                    ArrayList rowsObj = new ArrayList();
                    int id = 0;
                    foreach (PurchorderTrans tran in purTranList)
                    {
                        id++;
                        Hashtable row = new Hashtable();
                        row.Add("id", id);
                        ArrayList cell = new ArrayList();
                        cell.Add(id.ToString());
                        cell.Add(tran.PurchorderTranChartCode);
                        cell.Add(tran.PurchorderTranDescription);
                        cell.Add(tran.PurchorderTranNet);
                        cell.Add(tran.PurchorderTranTax);
                        cell.Add(tran.PurchorderTranGross);
                        row.Add("cell", cell);
                        rowsObj.Add(row);
                    }
                    JQGrid jqgridObj = new JQGrid(postdata, constr, rowsObj);
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
        public static string BindChartSelector()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                InvoiceMaster invoice = new InvoiceMaster(constr);

                // Update 06/06/2016
                List<ChartMaster> chartlist = invoice.GetChartMasters(true);
                foreach (ChartMaster chartmaster in chartlist)
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
        public static string SaveDataFromGrid(string rowValue)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            Object c = JSON.JsonDecode(rowValue);
            Hashtable hdata = (Hashtable)c;
            string session_id = HttpContext.Current.Session.SessionID;
            PurchorderMaster purObj = new PurchorderMaster(constr);
            purObj.ImportJSON(session_id);

            PurchorderTrans ptran = new PurchorderTrans(constr);
            string tran_id = hdata["ID"].ToString();
            ChartMaster chart = new ChartMaster(constr);
            string ch = hdata["Chart"].ToString();
            ch = ch.Substring(0, ch.IndexOf("|") - 1);
            chart.LoadData(ch);
            if (tran_id != "") //if user is updating a jqgrid row
            {
                int tran_index = purObj.FindPurTransByID(tran_id);
                if (tran_index != -1)
                {
                    ptran = (PurchorderTrans)(purObj.PurchorderTranList[tran_index]);
                    ptran.PurchorderTranChartCode = chart.ChartMasterCode;//--
                    ptran.PurchorderTranDescription = hdata["Description"].ToString();
                    ptran.PurchorderTranGross = Convert.ToDecimal(hdata["Gross"].ToString());
                    ptran.PurchorderTranNet = Convert.ToDecimal(hdata["Net"].ToString());
                    ptran.PurchorderTranTax = Convert.ToDecimal(hdata["Tax"].ToString());
                    purObj.ExportJSON(session_id);
                }
            }
            else // if user is adding a new line
            {
                ptran = new PurchorderTrans(constr);

                ptran.PurchorderTranChartCode = chart.ChartMasterCode;//--
                ptran.PurchorderTranDescription = hdata["Description"].ToString();
                ptran.PurchorderTranGross = Convert.ToDecimal(hdata["Gross"].ToString());
                ptran.PurchorderTranNet = Convert.ToDecimal(hdata["Net"].ToString());
                ptran.PurchorderTranTax = Convert.ToDecimal(hdata["Tax"].ToString());

                if (purObj.PurchorderTranList == null) purObj.PurchorderTranList = new ArrayList();
                purObj.PurchorderTranList.Add(ptran);
                purObj.ExportJSON(session_id);
            }
            return "dd";
        }
        #endregion

        #region WebControl Events
        private void ImageButtonDelete_Click(string[] args)
        {
            int tran_id = 0;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                tran_id = int.Parse(args[1]);

                string session_id = HttpContext.Current.Session.SessionID;
                PurchorderMaster purObj = new PurchorderMaster(constr);
                purObj.ImportJSON(session_id);
                purObj.PurchorderTranList.RemoveAt(tran_id - 1);
                decimal sumNet = 0;
                foreach (PurchorderTrans ptrans in purObj.PurchorderTranList)
                {
                    sumNet += ptrans.PurchorderTranNet;
                }
                LabelNet.Text = sumNet.ToString();
                LabelTax.Text = ((Decimal)((double)sumNet * 0.15)).ToString();
                LabelGross.Text = ((Decimal)((double)sumNet * 1.15)).ToString();
                purObj.ExportJSON(session_id);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion


    }
}
