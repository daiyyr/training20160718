//070713
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Sapp.Common;
using Sapp.SMS;
using Sapp.JQuery;
using Sapp.Data;
using System.Data;

namespace sapp_sms
{
    public partial class receiptedit : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Javascript setup
            Control[] wc = { };
            JSUtils.RenderJSArrayWithCliendIds(Page, wc);
            #endregion
            string receipt_id = "";
            string mode = "";
            string multi = "";
            string receiptbatchid = "";
            string debtorid = "";
            string bodycorpid = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Odbc odbc = new Odbc(AdFunction.conn);
                if (Request.QueryString["receiptid"] != null) receipt_id = Request.QueryString["receiptid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                if (Request.QueryString["multi"] != null) multi = Request.QueryString["multi"];
                if (Request.QueryString["receiptbatchid"] != null) receiptbatchid = Request.QueryString["receiptbatchid"];
                if (Request.QueryString["debtorid"] != null) debtorid = Request.QueryString["debtorid"];
                if (Request.Cookies["bodycorpid"].Value != null) bodycorpid = Request.Cookies["bodycorpid"].Value;
                if (!IsPostBack)
                {
                    Session["gobackstep"] = 1;
                    if (receiptbatchid == "")
                        Session["receiptbatchid"] = null;
                    AjaxControlUtils.SetupComboBox(ComboBoxBodycorp, "SELECT `bodycorp_id` AS `ID`, `bodycorp_code` AS `Code` FROM `bodycorps`", "ID", "Code", constr, false);

                    if (mode == "add")
                    {
                        #region Load Page
                        try
                        {
                            #region Initial ComboBox


                            if (debtorid == "")
                            {
                               
                                AdFunction.Debtor_ComboBox(ComboBoxDebtor,"All");
                                AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` LEFT JOIN `property_master` ON property_master_id=unit_master_property_id where property_master_bodycorp_id = " + Request.Cookies["bodycorpid"].Value + " and unit_master_type_id <>5 and unit_master_inactive_date is null order by unit_master_code", "ID", "Code", constr, true);
                                AjaxControlUtils.SetupComboBox(ComboBoxPaymentType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", constr, false);

                            }
                            else
                            {
                                AjaxControlUtils.SetupComboBox(ComboBoxDebtor, "SELECT `debtor_master_id` AS `ID`, `debtor_master_ .4-++++++++++name` AS `Code` FROM `debtor_master` WHERE `debtor_master_id`=" + debtorid, "ID", "Code", constr, false);
                                AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` WHERE `unit_master_debtor_id`=" + debtorid + " and unit_master_type_id <>5 and unit_master_inactive_date is null order by unit_master_code", "ID", "Code", constr, true);
                                AjaxControlUtils.SetupComboBox(ComboBoxPaymentType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", constr, false);
                                AjaxControlUtils.ComboBoxSelection(ComboBoxDebtor, debtorid, false);
                            }
                            if (!bodycorpid.Equals(""))
                            {
                                AjaxControlUtils.ComboBoxSelection(ComboBoxBodycorp, bodycorpid, false);
                                ComboBoxBodycorp.Enabled = false;
                            }

                            #endregion
                            #region Initial Web Controls
                            TextBoxRef.Text = "";
                            Receipt receipt = new Receipt(constr);
                            TextBoxRef.Text = receipt.GetNextReceiptNumber();
                            TextBoxGross.Text = "";
                            TextBoxDate.Text = "";
                            if (Request.QueryString["unitid"] != null)
                                ComboBoxUnit.SelectedValue = Request.QueryString["unitid"].ToString();
                            if (multi == "true")
                            {
                                LiteralNext.Visible = true;
                                ImageButtonNext.Visible = true;
                                Page.Title = "Sapp SMS - Receipt Edit Multi Unit";
                            }
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
                            Receipt receipt = new Receipt(constr);
                            receipt.LoadData(Convert.ToInt32(receipt_id));
                            if (receipt.receipt_reconciled)
                            {
                                ImageButtonSave.Visible = false;
                                SaveL.Text = "Reconciled Receipt";
                            }

                            #region Check
                            {
                                Bodycorp b = new Bodycorp(constr);
                                b.LoadData(receipt.ReceiptBodycorpId);
                                if (!b.CheckCloseOff(receipt.ReceiptDate))
                                {
                                    SaveL.Text = "Receipt before close date";
                                    ImageButtonSave.Visible = false;
                                    throw new Exception("Receipt before close date");

                                }
                            }

                            string cksql = "Select * FROM `receipts` WHERE `receipt_id`=" + receipt_id;

                            DataTable dt = odbc.ReturnTable(cksql, "check");
                            if (dt.Rows.Count > 0)
                            {
                                if (dt.Rows[0]["receipt_reconciled"].ToString().Equals("1"))
                                {
                                    ImageButtonSave.Visible = false;
                                }
                            }


                            #endregion

                            #region Initial ComboBox
                            AjaxControlUtils.SetupComboBox(ComboBoxDebtor, "SELECT `debtor_master_id` AS `ID`, `debtor_master_name` AS `Code` FROM `debtor_master`", "ID", "Code", constr, false);
                            AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master`  where unit_master_inactive_date is null and unit_master_type_id <>5 order by unit_master_code", "ID", "Code", constr, true);
                            AjaxControlUtils.SetupComboBox(ComboBoxPaymentType, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", constr, false);
                            #endregion
                            #region Load Web Controls
                            TextBoxRef.Text = receipt.ReceiptRef;
                            TextBoxNotes.Text = receipt.ReceiptNotes;
                            TextBoxGross.Text = receipt.ReceiptGross.ToString("0.00");
                            TextBoxDate.Text = DBSafeUtils.DBDateToStr(receipt.ReceiptDate);
                            LiteralNext.Visible = false;
                            ImageButtonNext.Visible = false;

                            AjaxControlUtils.ComboBoxSelection(ComboBoxDebtor, receipt.ReceiptDebtorId, false);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxUnit, receipt.ReceiptUnitId, true);
                            AjaxControlUtils.ComboBoxSelection(ComboBoxPaymentType, receipt.ReceiptPaymentTypeId, false);


                            if (receipt.ReceiptRef.Contains("REV"))
                                ImageButtonSave.Visible = false;
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion
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

        #region WebControl Events
        protected void ImageButtonNext_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (!Page.IsValid) return;
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string multi = "";
                string patchid = "";
                if (Request.QueryString["multi"] != null) multi = Request.QueryString["multi"];
                if (Request.QueryString["patchid"] != null) patchid = Request.QueryString["patchid"];
                if (multi == "true")
                {
                    if (patchid == "")
                    {
                        //First BC
                        Receipt receipt = new Receipt(constr);
                        int patch_id = receipt.GetNextPatchId();
                        Hashtable items = new Hashtable();
                        #region Retireve Values
                        items.Add("receipt_ref", DBSafeUtils.StrToQuoteSQL(TextBoxRef.Text));
                        items.Add("receipt_debtor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxDebtor, false));

                        int bodycorp_id = 0;
                        int? unit_id = AjaxControlUtils.ComboBoxSelectedValue(ComboBoxUnit, true);
                        Odbc mydb = null;
                        try
                        {
                            mydb = new Odbc(constr);
                            string sql = "SELECT `property_master_bodycorp_id` FROM `property_master` WHERE `property_master_id` IN (SELECT `unit_master_property_id` FROM `unit_master` WHERE `unit_master_id`=" + unit_id.Value + ")";
                            object id = mydb.ExecuteScalar(sql);
                            bodycorp_id = Convert.ToInt32(id);
                        }
                        finally
                        {
                            if (mydb != null) mydb.Close();
                        }

                        items.Add("receipt_bodycorp_id", bodycorp_id);
                        items.Add("receipt_unit_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxUnit, true));
                        items.Add("receipt_payment_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxPaymentType, false));
                        Bodycorp bodycorp = new Bodycorp(constr);
                        bodycorp.LoadData(bodycorp_id);
                        items.Add("receipt_account_id", bodycorp.BodycorpAccountId);
                        items.Add("receipt_batch_id", patch_id);
                        items.Add("receipt_gross", Convert.ToDecimal(TextBoxGross.Text));
                        items.Add("receipt_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                        #endregion
                        #region Add
                        receipt.Add(items);
                        #endregion
                        #region Redirect
                        Response.BufferOutput = true;
                        Response.Redirect("receiptedit.aspx?mode=add&multi=true&patchid=" + patch_id, false);
                        #endregion
                    }
                    else
                    {
                        Receipt receipt = new Receipt(constr);
                        int patch_id = Convert.ToInt32(patchid);
                        Hashtable items = new Hashtable();
                        #region Retireve Values
                        items.Add("receipt_ref", DBSafeUtils.StrToQuoteSQL(TextBoxRef.Text));
                        items.Add("receipt_debtor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxDebtor, false));

                        int bodycorp_id = 0;
                        int? unit_id = AjaxControlUtils.ComboBoxSelectedValue(ComboBoxUnit, true);
                        Odbc mydb = null;
                        try
                        {
                            mydb = new Odbc(constr);
                            string sql = "SELECT `property_master_bodycorp_id` FROM `property_master` WHERE `property_master_id` IN (SELECT `unit_master_property_id` FROM `unit_master` WHERE `unit_master_id`=" + unit_id.Value + ")";
                            object id = mydb.ExecuteScalar(sql);
                            bodycorp_id = Convert.ToInt32(id);
                        }
                        finally
                        {
                            if (mydb != null) mydb.Close();
                        }

                        items.Add("receipt_bodycorp_id", bodycorp_id);
                        items.Add("receipt_unit_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxUnit, true));
                        items.Add("receipt_payment_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxPaymentType, false));
                        Bodycorp bodycorp = new Bodycorp(constr);
                        bodycorp.LoadData(bodycorp_id);
                        items.Add("receipt_account_id", bodycorp.BodycorpAccountId);
                        items.Add("receipt_batch_id", patch_id);
                        items.Add("receipt_gross", Convert.ToDecimal(TextBoxGross.Text));
                        items.Add("receipt_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                        #endregion
                        #region Add
                        receipt.Add(items);
                        #endregion
                        #region Redirect
                        Response.BufferOutput = true;
                        Response.Redirect("receiptedit.aspx?mode=add&multi=true&patchid=" + patch_id, false);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void ImageButtonSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (!Page.IsValid) return;
                string receipt_id = "";
                string mode = "";
                string patch_id = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                if (Request.QueryString["receiptid"] != null) receipt_id = Request.QueryString["receiptid"];
                if (Request.QueryString["mode"] != null) mode = Request.QueryString["mode"];
                if (Request.QueryString["patchid"] != null) patch_id = Request.QueryString["patchid"];

                //check close off
                Bodycorp b = new Bodycorp(constr);
                b.LoadData(int.Parse(ComboBoxBodycorp.SelectedValue));
                if (!b.CheckCloseOff(DateTime.Parse(TextBoxDate.Text)))
                {
                    SaveL.Text = "Receipt before close date";
                    ImageButtonSave.Visible = false;
                    throw new Exception("Receipt before close date");

                }
                Hashtable items = new Hashtable();
                #region Retireve Values
                items.Add("receipt_ref", DBSafeUtils.StrToQuoteSQL(TextBoxRef.Text));
                items.Add("receipt_debtor_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxDebtor, false));

                int bodycorp_id = 0;
                int? unit_id = AjaxControlUtils.ComboBoxSelectedValue(ComboBoxUnit, true);
                Odbc mydb = null;
                try
                {
                    //mydb = new Odbc(constr);
                    //string sql = "SELECT `property_master_bodycorp_id` FROM `property_master` WHERE `property_master_id` IN (SELECT `unit_master_property_id` FROM `unit_master` WHERE `unit_master_id`=" + unit_id.Value + ")";
                    //object id = mydb.ExecuteScalar(sql);
                    //bodycorp_id = Convert.ToInt32(id);
                    bodycorp_id = int.Parse(ComboBoxBodycorp.SelectedValue);
                }
                finally
                {
                    if (mydb != null) mydb.Close();
                }

                items.Add("receipt_bodycorp_id", bodycorp_id);
                items.Add("receipt_unit_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxUnit, true));
                items.Add("receipt_payment_type_id", AjaxControlUtils.ComboBoxSelectedValueToSQL(ComboBoxPaymentType, false));
                Bodycorp bodycorp = new Bodycorp(constr);
                bodycorp.LoadData(bodycorp_id);
                items.Add("receipt_account_id", bodycorp.BodycorpAccountId);
                if (patch_id != "")
                    items.Add("receipt_batch_id", Convert.ToInt32(patch_id));
                items.Add("receipt_gross", Convert.ToDecimal(TextBoxGross.Text));
                items.Add("receipt_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));
                items.Add("receipt_notes", DBSafeUtils.StrToQuoteSQL(TextBoxNotes.Text));
                #endregion
                if (mode == "add")
                {
                    #region Add

                    Receipt receipt = new Receipt(constr);
                    receipt.Add(items);
                    #endregion
                    #region Redirect
                    Response.BufferOutput = true;
                    Response.Redirect("~/receiptallocate.aspx?receiptid=" + receipt.ReceiptId, false);
                    #endregion
                }
                else if (mode == "edit")
                {
                    #region Edit
                    Receipt receipt = new Receipt(constr);
                    receipt.Update(items, Convert.ToInt32(receipt_id));
                    #endregion
                    #region Redirect
                    Response.BufferOutput = true;
                    Response.Redirect("goback.aspx", false);
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
        }
        protected void ComboBoxPaymentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxPaymentType.SelectedItem.Text == "CHEQUE")
            {
                LiteralChequeNum.Visible = true;
                TextBoxChequeNum.Visible = true;
                LiteralBank.Visible = true;
                TextBoxBank.Visible = true;
                LiteralBranch.Visible = true;
                TextBoxBranch.Visible = true;
            }
            else
            {
                LiteralChequeNum.Visible = false;
                TextBoxChequeNum.Visible = false;
                LiteralBank.Visible = false;
                TextBoxBank.Visible = false;
                LiteralBranch.Visible = false;
                TextBoxBranch.Visible = false;
            }
        }
        #endregion
        #region Validations
        protected void CustomValidatorDebtor_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxDebtor.SelectedIndex > -1) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void CustomValidatorPaymentType_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxPaymentType.SelectedIndex > -1) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        protected void CustomValidatorUnit_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (ComboBoxDebtor.SelectedIndex > -1) args.IsValid = true; // -1 unselect, 0 null selected
                else args.IsValid = false;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
        protected void ComboBoxDebtor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ComboBoxUnit.SelectedValue.Equals("null"))
            {
                AjaxControlUtils.SetupComboBox(ComboBoxUnit, "SELECT `unit_master_id` AS `ID`, `unit_master_code` AS `Code` FROM `unit_master` where unit_master_debtor_id=" + ComboBoxDebtor.SelectedValue + " and unit_master_type_id <>5 and unit_master_inactive_date is null order by unit_master_code", "ID", "Code", AdFunction.conn, true);
                ComboBoxUnit.Enabled = true;
            }

        }

        protected void ComboBoxUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ComboBoxUnit.SelectedValue.Equals("null"))
            {
                Odbc o = new Odbc(AdFunction.conn);
                DataTable dt = o.ReturnTable("SELECT * FROM `unit_master` where unit_master_id=" + ComboBoxUnit.SelectedValue, "y1");
                string did = dt.Rows[0]["unit_master_debtor_id"].ToString();
                AdFunction.Debtor_ComboBox(ComboBoxDebtor);
                ComboBoxDebtor.SelectedValue = did;
                ComboBoxDebtor.Enabled = false;
            }
            else
            {
                AdFunction.Debtor_ComboBox(ComboBoxDebtor, "2");
                ComboBoxDebtor.Enabled = true;
            }
        }



    }







}