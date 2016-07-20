using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.SMS;
using System.Configuration;
using System.Collections;
using Sapp.Common;
using Sapp.Data;
using Sapp.JQuery;
using System.Text.RegularExpressions;

namespace sapp_sms
{
    public partial class propertymasterdetails : System.Web.UI.Page, IPostBackEventHandler
    {
        private string CheckedQueryString()
        {
            if (null != Request.Cookies["bodycorpid"].Value && Regex.IsMatch(Request.Cookies["bodycorpid"].Value, "^[0-9]*$"))
            {
                return Request.Cookies["bodycorpid"].Value;
            }
            return ""; 
        }
        private string NewQueryString(string connector)
        {
            string result = CheckedQueryString();
            return string.IsNullOrEmpty(result) ? "" : connector + "bodycorpid=" + result;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript Setup
                Control[] wc = { jqGridComms , jqGridContacts};
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if (!IsPostBack)
                {
                    #region Load webForm
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                        string property_id = "";
                        if (Request.QueryString["propertyid"] != null) property_id = Request.QueryString["propertyid"];
                        PropertyMaster propertymaster = new PropertyMaster(constr);
                        propertymaster.LoadData(Convert.ToInt32(property_id));

                        LabelID.Text = propertymaster.PropertyMasterId.ToString();
                        LabelCode.Text = propertymaster.PropertyMasterCode.ToString();
                        LabelName.Text = propertymaster.PropertyMasterName;
                        if (propertymaster.PropertyMasterTotalsqm.HasValue)
                            LabelTotalSqm.Text = propertymaster.PropertyMasterTotalsqm.Value.ToString();
                        LabelNumOfUnits.Text = propertymaster.PropertyMasterNumOfUnits.ToString();
                        LabelNotes.Text = propertymaster.PropertyMasterNotes;

                        PropertyType ptype = new PropertyType(constr);
                        ptype.LoadData(propertymaster.PropertyMasterTypeId);
                        LabelType.Text = ptype.PropertyTypeCode;

                        Bodycorp bodycorp = new Bodycorp(constr);
                        bodycorp.LoadData(propertymaster.PropertyMasterBodycorpId);
                        LabelBodycorp.Text = bodycorp.BodycorpCode;

                        LabelBeginDate.Text = propertymaster.PropertyMasterBeginDate.ToString("dd/MM/yyyy");

                        if ("XMLHttpRequest" != Request.Headers["X-Requested-With"])
                        {
                            Session["propertyid"] = Request.QueryString["propertyid"];
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
                else if (args[0] == "ButtonDeleteContact")
                {
                    ButtonDeleteContact_Click(args);
                }
                else if (args[0] == "ButtonDeleteContactComm")
                {
                    ButtonDeleteContactComm_Click(args);
                }
                else
                    throw new Exception("Error: unknown command!");
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For Comm DataGrid

        [System.Web.Services.WebMethod]
        public static string DataGridCommsDataBind(string postdata, string propertyid)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                PropertyMaster propertymaster = new PropertyMaster(constr);
                propertymaster.LoadData(Convert.ToInt32(propertyid));
                Comms<PropertyMaster> comms = new Comms<PropertyMaster>(constr);
                comms.LoadData(propertymaster);
                ArrayList commList = comms.Communications;
                ArrayList rowData = new ArrayList();
                int id = 1;
                foreach (Comm<PropertyMaster> comm in commList)
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
        public static void DataGridCommsSave(string rowValue, string propertyid)
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
                    PropertyMaster propertymaster = new PropertyMaster(constr);
                    propertymaster.LoadData(Convert.ToInt32(propertyid));
                    Comm<PropertyMaster> comm = new Comm<PropertyMaster>(constr);
                    comm.Update(items, Convert.ToInt32(rowObj["ID"]));
                }
                else
                {
                    PropertyMaster propertymaster = new PropertyMaster(constr);
                    propertymaster.LoadData(Convert.ToInt32(propertyid));
                    Comm<PropertyMaster> comm = new Comm<PropertyMaster>(constr);
                    comm.Add(items, propertymaster);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion



        #region WebMethods For DataGridContacts
        //[System.Web.Services.WebMethod]
        //public static string DataGridContactsDataBind(string postdata, string propertyid)
        //{
        //    try
        //    {
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        PropertyMaster propertymaster = new PropertyMaster(constr);
        //        propertymaster.LoadData(Convert.ToInt32(propertyid));
        //        Contacts<PropertyMaster> contacts = new Contacts<PropertyMaster>(constr);
        //        contacts.LoadData(propertymaster);
        //        ArrayList contactList = contacts.ContactMasters;
        //        ArrayList rowData = new ArrayList();
        //        int id = 1;
        //        foreach (Contact<PropertyMaster> contact in contactList)
        //        {
        //            Hashtable items = new Hashtable();
        //            items.Add("id", id);
        //            ArrayList cells = new ArrayList();
        //            cells.Add(contact.ContactMasterId.ToString());
        //            ContactType contactType = new ContactType(constr);
        //            contactType.LoadData(contact.ContactMasterTypeId);
        //            cells.Add(contactType.ContactTypeCode);
        //            cells.Add(contact.ContactMasterName);
        //            cells.Add(contact.ContactMasterNotes);
        //            cells.Add(id.ToString());
        //            items.Add("cell", cells);
        //            rowData.Add(items);
        //            id++;
        //        }
        //        JQGrid jqgridObj = new JQGrid(postdata, constr, rowData);
        //        string jsonStr = jqgridObj.GetJSONStr();
        //        return jsonStr;
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
        //    }
        //    return null;
        //}
        //[System.Web.Services.WebMethod]
        //public static string DataGridContactsTypeSelect()
        //{
        //    try
        //    {
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        string html = "<select>";
        //        ContactTypes contactTypes = new ContactTypes(constr);
        //        contactTypes.LoadData();
        //        foreach (ContactType conType in contactTypes.ContactTypeList)
        //        {
        //            html += "<option value='" + conType.ContactTypeId.ToString() + "'>" + conType.ContactTypeCode + "</option>";
        //        }
        //        html += "</select>";
        //        return html;
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
        //    }
        //    return null;
        //}
        //[System.Web.Services.WebMethod]
        //public static void DataGridContactsSave(string rowValue, string propertyid)
        //{
        //    try
        //    {
        //        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //        Hashtable rowObj = (Hashtable)JSON.JsonDecode(rowValue);
        //        Hashtable items = new Hashtable();
        //        #region Retireve Values
        //        ContactType conType = new ContactType(constr);
        //        conType.LoadData(rowObj["Type"].ToString());
        //        items.Add("contact_master_type_id", conType.ContactTypeId);
        //        items.Add("contact_master_name", DBSafeUtils.StrToQuoteSQL(rowObj["Name"].ToString()));
        //        items.Add("contact_master_notes", DBSafeUtils.StrToQuoteSQL(rowObj["Notes"].ToString()));
        //        #endregion
        //        if (rowObj["ID"].ToString() != "")
        //        {
        //            PropertyMaster propertymaster = new PropertyMaster(constr);
        //            propertymaster.LoadData(Convert.ToInt32(propertyid));
        //            Contact<PropertyMaster> contact = new Contact<PropertyMaster>(constr);
        //            contact.Update(items, Convert.ToInt32(rowObj["ID"]));
        //        }
        //        else
        //        {
        //            PropertyMaster propertymaster = new PropertyMaster(constr);
        //            propertymaster.LoadData(Convert.ToInt32(propertyid));
        //            Contact<PropertyMaster> contact = new Contact<PropertyMaster>(constr);
        //            contact.Add(items, propertymaster);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.Current.Session["Error"] = ex.Message; HttpContext.Current.Response.Redirect("~/error.aspx",false);
        //    }
        //}
        #endregion


        #region WebMethods For Contacts Master Details DataGrid
 

        #region WebMethods For Master DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridContactDataBind(string postdata)
        {
            try
            {
                string propertyid = HttpContext.Current.Session["propertyid"].ToString();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                PropertyMaster propertymaster = new PropertyMaster(constr);
                propertymaster.LoadData(Convert.ToInt32(propertyid));
                Contacts<PropertyMaster> contacts = new Contacts<PropertyMaster>(constr);
                contacts.LoadData(propertymaster);
                ArrayList contactList = contacts.ContactMasters;
                ArrayList rowData = new ArrayList();
                int id = 1;
                foreach (Contact<PropertyMaster> contact in contactList)
                {
                    Hashtable items = new Hashtable();
                    items.Add("id", id);
                    ArrayList cells = new ArrayList();
                    cells.Add(contact.ContactMasterId.ToString());
                    ContactType contactType = new ContactType(constr);
                    contactType.LoadData(contact.ContactMasterTypeId);
                    cells.Add(contactType.ContactTypeCode);
                    cells.Add(contact.ContactMasterName);
                    cells.Add(contact.ContactMasterNotes);
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
        public static string SaveDataFromContactGrid(string dataFromTheRow)
        {
            string propertyid = "";
            try
            {
                propertyid = HttpContext.Current.Session["propertyid"].ToString();
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable rowObj = (Hashtable)JSON.JsonDecode(dataFromTheRow);
                Hashtable items = new Hashtable();
                #region Retireve Values
                ContactType conType = new ContactType(constr);
                conType.LoadData(rowObj["Type"].ToString());
                items.Add("contact_master_type_id", conType.ContactTypeId);
                items.Add("contact_master_name", DBSafeUtils.StrToQuoteSQL(rowObj["Name"].ToString()));
                items.Add("contact_master_notes", DBSafeUtils.StrToQuoteSQL(rowObj["Notes"].ToString()));
                #endregion
                if (rowObj["ID"].ToString() != "")
                {
                    PropertyMaster propertymaster = new PropertyMaster(constr);
                    propertymaster.LoadData(Convert.ToInt32(propertyid));
                    Contact<PropertyMaster> contact = new Contact<PropertyMaster>(constr);
                    contact.Update(items, Convert.ToInt32(rowObj["ID"]));
                }
                else
                {
                    PropertyMaster propertymaster = new PropertyMaster(constr);
                    propertymaster.LoadData(Convert.ToInt32(propertyid));
                    Contact<PropertyMaster> contact = new Contact<PropertyMaster>(constr);
                    contact.Add(items, propertymaster);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return "dd";
        }
        [System.Web.Services.WebMethod]
        public static string DataGridContactTypeSelect()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                string html = "<select>";
                ContactTypes contactTypes = new ContactTypes(constr);
                contactTypes.LoadData();
                foreach (ContactType conType in contactTypes.ContactTypeList)
                {
                    html += "<option value='" + conType.ContactTypeId.ToString() + "'>" + conType.ContactTypeCode + "</option>";
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
        private void ButtonDeleteContact_Click(string[] args)
        {
            try
            {
                string property_id = "";
                string contact_master_id = "";
                contact_master_id = args[1];
                if (Request.QueryString["propertyid"] != null) property_id = Request.QueryString["propertyid"];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                PropertyMaster propertymaster = new PropertyMaster(constr);
                propertymaster.LoadData(Convert.ToInt32(property_id));
                Contact<PropertyMaster> contact = new Contact<PropertyMaster>(constr);
                contact.Delete(Convert.ToInt32(contact_master_id), propertymaster);
                Response.BufferOutput = true;
                Response.Redirect("goback.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        #region WebMethods For Sub DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridContactSubDataBind(string postdata, string selectedRow)
        {
            try
            {
                Object c = JSON.JsonDecode(selectedRow);
                Hashtable hdata = (Hashtable)c;
                string contactmasterid = hdata["ID"].ToString();

                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                ContactMaster contactmaster = new ContactMaster(constr);
                contactmaster.LoadData(Convert.ToInt32(contactmasterid));
                Comms<ContactMaster> comms = new Comms<ContactMaster>(constr);
                comms.LoadData(contactmaster);
                ArrayList commList = comms.Communications;
                ArrayList rowData = new ArrayList();
                int id = 1;
                foreach (Comm<ContactMaster> comm in commList)
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
        public static string DataGridContactCommsTypeSelect()
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
        public static void SaveDataFromContactCommGrid(string dataFromTheRow, string dataFromMasterRow)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Hashtable rowObj = (Hashtable)JSON.JsonDecode(dataFromTheRow);
                Hashtable masterRow = (Hashtable)JSON.JsonDecode(dataFromMasterRow);
                if (masterRow.Count <= 0) return;
                string contactmasterid = masterRow["ID"].ToString();
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
                    ContactMaster contactmaster = new ContactMaster(constr);
                    contactmaster.LoadData(Convert.ToInt32(contactmasterid));
                    Comm<ContactMaster> comm = new Comm<ContactMaster>(constr);
                    comm.Update(items, Convert.ToInt32(rowObj["ID"]));
                }
                else
                {
                    ContactMaster contactmaster = new ContactMaster(constr);
                    contactmaster.LoadData(Convert.ToInt32(contactmasterid));
                    Comm<ContactMaster> comm = new Comm<ContactMaster>(constr);
                    comm.Add(items, contactmaster);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        [System.Web.Services.WebMethod]
        private void ButtonDeleteContactComm_Click(string[] args)
        {
            try
            {
                string property_id="";
                string comm_master_id = args[1];
                string contactmaster_id = args[2];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                ContactMaster contactmaster = new ContactMaster(constr);
                contactmaster.LoadData(Convert.ToInt32(contactmaster_id));
                Comm<ContactMaster> comm = new Comm<ContactMaster>(constr);
                comm.Delete(Convert.ToInt32(comm_master_id), contactmaster);
                if (Request.QueryString["propertyid"] != null) 
                    property_id = Request.QueryString["propertyid"];
                Response.BufferOutput = true;
                Response.Redirect("~/propertymasterdetails.aspx?propertyid=" + property_id + NewQueryString("?"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion


        #endregion



        #region WebControl Events

        private void ButtonDeleteComm_Click(string[] args)
        {

            try
            {
                string comm_master_id = "";
                comm_master_id = args[1];
                string property_id = "";
                if (Request.QueryString["propertyid"] != null) property_id = Request.QueryString["propertyid"];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                PropertyMaster propertymaster = new PropertyMaster(constr);
                propertymaster.LoadData(Convert.ToInt32(property_id));
                Comm<PropertyMaster> comm = new Comm<PropertyMaster>(constr);
                comm.Delete(Convert.ToInt32(comm_master_id), propertymaster);
                Response.BufferOutput = true;
                Response.Redirect("~/propertymasterdetails.aspx?propertyid=" + property_id+ NewQueryString("&"), false);
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
                string property_id = "";
                if (Request.QueryString["propertyid"] != null) property_id = Request.QueryString["propertyid"];
                Response.BufferOutput = true;
                Response.Redirect("~/propertymasteredit.aspx?mode=edit&propertyid=" + property_id + NewQueryString("&"), false);
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
                string property_id = Request.QueryString["propertyid"];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                PropertyMaster propertymaster = new PropertyMaster(constr);
                propertymaster.Delete(Convert.ToInt32(property_id));
                Response.BufferOutput = true;
                Response.Redirect("~/propertymaster.aspx" + NewQueryString("?"), false);
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
                Response.Redirect("~/propertymaster.aspx" + NewQueryString("?"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        

        protected void ImageButtonUnitplan_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("unitmaster.aspx?propertyid=" + Request.QueryString["propertyid"],false);
        }

        protected void ImageButtonMaintenance_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("pptymaintmaster.aspx?propertyid=" + Request.QueryString["propertyid"],false);
        }

        protected void ImageButtonValuations_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("pptyvtmaster.aspx?propertyid=" + Request.QueryString["propertyid"],false);
        }

        protected void ImageButtonTitles_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("pptytitles.aspx?propertyid=" + Request.QueryString["propertyid"],false);
        }

        protected void ImageButtonMortgages_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("pptymtgs.aspx?propertyid=" + Request.QueryString["propertyid"],false);
        }

        protected void ImageButtonContractor_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("pptycntrmaster.aspx?propertyid=" + Request.QueryString["propertyid"],false);
        }

        protected void ImageButtonInsurance_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("pptyinsmaster.aspx?propertyid=" + Request.QueryString["propertyid"],false);
        }

        #endregion

    }
}