using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Configuration;
using Sapp.Common;
using Sapp.SMS;
using Sapp.Data;
using Sapp.JQuery;
using System.Data.Odbc;

namespace sapp_sms
{
    public partial class creditormasterdetails : System.Web.UI.Page,IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { jqGridComms };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    #region Load webForm
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        string creditor_master_id = "";
                        if (Request.QueryString["creditorid"] != null) creditor_master_id = Request.QueryString["creditorid"];
                        CreditorMaster creditormaster = new CreditorMaster(constr);
                        creditormaster.LoadData(Convert.ToInt32(creditor_master_id));
                        LabelCreditorID.Text = creditormaster.CreditorMasterId.ToString();
                        LabelBankAC.Text = creditormaster.CreditorMasterBankAc;
                        LabelCode.Text = creditormaster.CreditorMasterCode;
                        LabelGST.Text = creditormaster.CreditorMasterGst;
                        LabelName.Text = creditormaster.CreditorMasterName;
                        LabelNotes.Text = creditormaster.CreditorMasterNotes;
                        LabelPayee.Text = creditormaster.CreditorMasterPayee;
                        Labelref.Text = creditormaster.CreditorMasterRef;
                        LabelSalutation.Text = creditormaster.CreditorMasterSalutation;
                        LabelNotax.Text = creditormaster.CreditorMasterNotax?"Yes":"No";
                        LabelCntrType.Text = creditormaster.CreditorMasterCntrtype;
                        LabelSrvArea.Text = creditormaster.CreditorMasterSrvarea;
                        if (creditormaster.CreditorMasterPaymentTermId.HasValue)
                        {
                            PaymentTerm payterm = new PaymentTerm(constr);
                            payterm.LoadData(creditormaster.CreditorMasterPaymentTermId.Value);
                            LabelPaymentTerm.Text = payterm.PaymentTermName;
                        }
                        if (creditormaster.CreditorMasterPaymentTypeId.HasValue)
                        {
                            PaymentType paytype = new PaymentType(constr);
                            paytype.LoadData(creditormaster.CreditorMasterPaymentTypeId.Value);
                            LabelPaymentType.Text = paytype.PaymentTypeName;
                        }
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
                if (args[0] == "ButtonDeleteComm")
                {
                    ButtonDeleteComm_Click(args);
                }
                else
                    throw new Exception("Error: unknown command!");
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridCommsDataBind(string postdata, string creditorid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                CreditorMaster creditormaster = new CreditorMaster(constr);
                creditormaster.LoadData(Convert.ToInt32(creditorid));
                Comms<CreditorMaster> comms = new Comms<CreditorMaster>(constr);
                comms.LoadData(creditormaster);
                ArrayList commList = comms.Communications;
                ArrayList rowData = new ArrayList();
                int id = 1;
                foreach (Comm<CreditorMaster> comm in commList)
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
        public static void DataGridCommsSave(string rowValue, string creditorid)
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

                // Update 05/05/2016
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
                    CreditorMaster creditormaster = new CreditorMaster(constr);
                    creditormaster.LoadData(Convert.ToInt32(creditorid));
                    Comm<CreditorMaster> comm = new Comm<CreditorMaster>(constr);
                    comm.Update(items, Convert.ToInt32(rowObj["ID"]));
                }
                else
                {
                    CreditorMaster creditormaster = new CreditorMaster(constr);
                    creditormaster.LoadData(Convert.ToInt32(creditorid));
                    Comm<CreditorMaster> comm = new Comm<CreditorMaster>(constr);
                    comm.Add(items, creditormaster);
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
                string creditor_master_id = "";
                if (Request.QueryString["creditorid"] != null) creditor_master_id = Request.QueryString["creditorid"];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                CreditorMaster creditormaster = new CreditorMaster(constr);
                creditormaster.LoadData(Convert.ToInt32(creditor_master_id));
                Comm<CreditorMaster> comm = new Comm<CreditorMaster>(constr);
                comm.Delete(Convert.ToInt32(comm_master_id), creditormaster);
                Response.BufferOutput = true;
                Response.Redirect("~/creditormasterdetails.aspx?creditorid=" + creditor_master_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonEdit_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string creditor_master_id = "";
                if (Request.QueryString["creditorid"] != null) creditor_master_id = Request.QueryString["creditorid"];
                Response.BufferOutput = true;
                Response.Redirect("~/creditormasteredit.aspx?mode=edit&creditorid=" + creditor_master_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string creditor_master_id = Request.QueryString["creditorid"];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                CreditorMaster creditormaster = new CreditorMaster(constr);
                creditormaster.Delete(Convert.ToInt32(creditor_master_id));
                Response.BufferOutput = true;
                Response.Redirect("goback.aspx", false);
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
                Response.Redirect("~/creditormaster.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonPurchOrder_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("purchordermaster.aspx?creditorid=" + Request.QueryString["creditorid"],false);
        }

        protected void ImageButtonInvoice_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("cinvoices.aspx?creditorid=" + Request.QueryString["creditorid"],false);
        }

        protected void ImageButtonPayment_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("cpaymentedit.aspx?mode=add&creditorid=" + Request.QueryString["creditorid"],false);
        }   

        #endregion

        
    }
}