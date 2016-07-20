using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sapp.Common;
using System.Collections;
using Sapp.SMS;
using System.Configuration;
using Sapp.JQuery;
using Sapp.Data;
using System.Text.RegularExpressions;

namespace sapp_sms
{
    public partial class contactmasters : System.Web.UI.Page,IPostBackEventHandler
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
                #region Javascript setup
                Control[] wc = { jqGridCC1 };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                if ("XMLHttpRequest" != Request.Headers["X-Requested-With"])
                {
                    Session["bodycorpid"] = CheckedQueryString();
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
                else if (args[0] == "ButtonDeleteComm")
                {
                    ButtonDeleteComm_Click(args);
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
                string sqlfromstr = "FROM (`contact_master` left join `contact_types` on `contact_master_type_id`=`contact_type_id`)";
                if (null != HttpContext.Current.Session["bodycorpid"] && !String.IsNullOrEmpty(HttpContext.Current.Session["bodycorpid"].ToString()))
                {
                    sqlfromstr += "where contact_master_id in (select property_contact_contact_id from property_master,property_contacts where property_master_id=property_contact_property_id and  property_master_bodycorp_id=" + HttpContext.Current.Session["bodycorpid"].ToString() + ")";
                }
                string sqlselectstr = "SELECT `ID`,`Type`, `Name`,`Notes` FROM (SELECT `contact_master_id` AS `ID`,`contact_type_code` as `Type`, `contact_master_name` AS `Name`, `contact_master_notes` AS `Notes` ";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
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
        public static string SaveDataFromGrid(string rowValue)
        {
            Object c = JSON.JsonDecode(rowValue);
            Hashtable hdata = (Hashtable)c;
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string line_id = hdata["ID"].ToString();
            ContactType contacttype = new ContactType(constr);
            contacttype.LoadData(hdata["Type"].ToString());
            ContactMaster contactmaster = new ContactMaster(constr);
            Hashtable items = new Hashtable();
            items.Add("contact_master_type_id", DBSafeUtils.IntToSQL(contacttype.ContactTypeId));
            items.Add("contact_master_name", DBSafeUtils.StrToQuoteSQL(hdata["Name"].ToString()));
            items.Add("contact_master_notes", DBSafeUtils.StrToQuoteSQL(hdata["Notes"].ToString()));
            if (line_id == "")
            {
                contactmaster.Add(items);
            }
            else
            {
                contactmaster.Update(items, Convert.ToInt32(line_id));
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
        #endregion

        #region WebMethods For Sub DataGrid
        [System.Web.Services.WebMethod]
        public static string DataGridSubDataBind(string postdata, string selectedRow)
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
        public static void SaveDataFromCommGrid(string dataFromTheRow, string dataFromMasterRow)
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
                items.Add("comm_master_primary", Convert.ToInt32(rowObj["Primary"]));
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
        #endregion

        #region WebControl Events

        private void ImageButtonDelete_Click(string[] args)
        {
            string contact_master_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                contact_master_id = args[1];
                ContactMaster contact_master = new ContactMaster(constr);
                contact_master.Delete(Convert.ToInt32(contact_master_id));
                Response.BufferOutput = true;
                Response.Redirect("~/contactmasters.aspx" + NewQueryString("?"),false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ButtonDeleteComm_Click(string[] args)
        {
            try
            {
                string comm_master_id = args[1];
                string contactmaster_id = args[2];
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                ContactMaster contactmaster = new ContactMaster(constr);
                contactmaster.LoadData(Convert.ToInt32(contactmaster_id));
                Comm<ContactMaster> comm = new Comm<ContactMaster>(constr);
                comm.Delete(Convert.ToInt32(comm_master_id), contactmaster);
                Response.BufferOutput = true;
                Response.Redirect("~/contactmasters.aspx" + NewQueryString("?"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion
    }
}