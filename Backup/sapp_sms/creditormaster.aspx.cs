using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.SMS;
using Sapp.JQuery;
using Sapp.Common;
using Sapp.Data;
using System.Data;
namespace sapp_sms
{
    public partial class creditormaster : System.Web.UI.Page, IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridCreditors };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
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
                if (args[0] == "ImageButtonEdit")
                {
                    ImageButtonEdit_Click(args);
                }
                else if (args[0] == "ImageButtonDelete")
                {
                    ImageButtonDelete_Click(args);
                }
                else if (args[0] == "ImageButtonDetails")
                {
                    ImageButtonDetails_Click(args);
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
                #region get Address
                Odbc o = new Odbc(constr);
                Odbc o2 = new Odbc(constr);

                string sql ="SELECT creditor_master.creditor_master_id AS ID, comm_master.comm_master_data, comm_master.comm_master_type_id, comm_types.comm_type_name, ";
sql+=" comm_types.comm_type_code";
sql += " FROM creditor_master, creditor_comms, comm_master, comm_types";
sql+=" WHERE creditor_master.creditor_master_id = creditor_comms.creditor_comm_creditor_id AND creditor_comms.creditor_comm_comm_id = comm_master.comm_master_id AND comm_master.comm_master_type_id = comm_types.comm_type_id";
                string sql2 = "select * from creditor_master";
                DataTable debotorID = o2.ReturnTable(sql2, "TableID");
                DataTable dt = o.ReturnTable(sql, "ContactDT");
                DataTable dt2 = new DataTable("");
                dt2.Columns.Add("Did");
                dt2.Columns.Add("Address");
                dt2.Columns.Add("Contact");
                foreach (DataRow dr in debotorID.Rows)
                {
                    dt.DefaultView.RowFilter = "ID = " + dr[0].ToString();
                    DataTable temp = dt.DefaultView.ToTable();
                    string address = "";
                    string contact = "";
                    foreach (DataRow dr2 in temp.Rows)
                    {
                        int t = int.Parse(dr2["comm_master_type_id"].ToString());

                        if (t == 1)
                        {
                            address += dr2["comm_master_data"].ToString() + ",";
                        }

                        if (t > 2)
                        {
                            contact +=dr2["comm_type_code"]+":"+  dr2["comm_master_data"].ToString() + ",";
                        }
                    }
                    DataRow newRow = dt2.NewRow();
                    newRow["Did"] = dr[0];
                    newRow["Address"] = address;
                    newRow["Contact"] = contact;
                    dt2.Rows.Add(newRow);
                }
                #endregion
                string tablesql = "SELECT `ID`, `Code`, `Name` FROM (SELECT `creditor_master_id` AS `ID`, `creditor_master_code` AS `Code`, `creditor_master_name` AS `Name` FROM (`creditor_master`)) AS t1";
                Odbc o3 = new Odbc(constr);
                DataTable dt3 = o3.ReturnTable(tablesql, "NewTable");
                dt3.Columns.Add("Address");
                dt3.Columns.Add("Contact");
                foreach (DataRow dr in dt3.Rows)
                {
                    string id = dr["ID"].ToString();
                    dt2.DefaultView.RowFilter = "Did=" + id;
                    foreach (DataRow dr2 in dt2.DefaultView.ToTable().Rows)
                    {
                        string a = dr2["Address"].ToString();
                        string c = dr2["Contact"].ToString();
                        dr["Contact"] = c;
                        dr["Address"] = a;
                    }
                }
                string sqlfromstr = "FROM `" + dt3.TableName + "`";
                string sqlselectstr = "SELECT `ID`, `Code`, `Name`, Address, Contact FROM (SELECT *";
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt3);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }
        #endregion

        #region WebControl Events
        protected void ImageButtonAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/creditormasteredit.aspx?mode=add", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            string creditormaster_id = "";
            try
            {
                creditormaster_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/creditormasteredit.aspx?mode=edit&creditorid=" + creditormaster_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonDelete_Click(string[] args)
        {
            string creditormaster_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                creditormaster_id = args[1];
                CreditorMaster creditor = new CreditorMaster(constr);
                creditor.Delete(Convert.ToInt32(creditormaster_id));
                Response.BufferOutput = true;
                Response.Redirect("~/creditormaster.aspx",false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonDetails_Click(string[] args)
        {
            string creditormaster_id = "";
            try
            {
                creditormaster_id = args[1];
                Session["creditormasterid"] = "1";
                Response.BufferOutput = true;
                Response.Redirect("~/creditormasterdetails.aspx?creditorid=" + creditormaster_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonImport_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/creditorupload.aspx", false);
        }
        #endregion

        
    }
}