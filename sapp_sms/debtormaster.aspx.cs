using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Sapp.JQuery;
using System.Data;
using Sapp.Common;
using Sapp.SMS;
using System.Text.RegularExpressions;
using Sapp.Data;
using Sapp.SMS;
using Sapp.General;
using Sapp.JQuery;
using System.Data.Odbc;
using System.Collections;
using System.IO;
namespace sapp_sms
{
    public partial class debtormaster : System.Web.UI.Page, IPostBackEventHandler
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridDebtors };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                Session["SelectedType"] = ComboBoxAllocated.SelectedValue;
                if (!IsPostBack)
                {
                    if (Request.Cookies["bodycorpid"] != null)
                        Session["debtormaster_bodycorpid"] = Request.Cookies["bodycorpid"].Value;
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
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            #region get Address
            Odbc o = new Odbc(constr);
            Odbc o2 = new Odbc(constr);
            string sql = "SELECT debtor_master.debtor_master_id, comm_master.comm_master_data, comm_master.comm_master_type_id, comm_types.comm_type_code, ";
            sql += " comm_types.comm_type_name ";
            sql += " FROM debtor_master, debtor_comms, comm_master, comm_types";
            sql += " WHERE debtor_master.debtor_master_id = debtor_comms.debtor_comm_debtor_id AND debtor_comms.debtor_comm_comm_id = comm_master.comm_master_id AND ";
            sql += " comm_master.comm_master_type_id = comm_types.comm_type_id";
            string sql2 = "select * from debtor_master";
            DataTable debotorID = o2.ReturnTable(sql2, "debotorID");
            DataTable dt = o.ReturnTable(sql, "ContactDT");
            DataTable dt2 = new DataTable("");
            dt2.Columns.Add("Did");
            dt2.Columns.Add("Address");
            dt2.Columns.Add("Contact");
            foreach (DataRow dr in debotorID.Rows)
            {
                dt.DefaultView.RowFilter = "debtor_master_id = " + dr[0].ToString();
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
                        contact += dr2["comm_type_code"] + ":" + dr2["comm_master_data"].ToString() + ",";
                    }
                }
                DataRow newRow = dt2.NewRow();
                newRow["Did"] = dr[0];
                newRow["Address"] = address;
                newRow["Contact"] = contact;
                dt2.Rows.Add(newRow);
            }
            #endregion

            string dsql = "SELECT debtor_master.debtor_master_id as ID ,debtor_master.debtor_master_id,debtor_master_type_id, debtor_master.debtor_master_code AS Code, debtor_master.debtor_master_name AS Name, debtor_types.debtor_type_name AS Type FROM `debtor_master` LEFT JOIN `debtor_types` ON `debtor_master_type_id` = `debtor_type_id`";

            if (HttpContext.Current.Session["BID"] != null)
            {
                dsql += " WHERE `debtor_master_id` IN (SELECT `unit_master_debtor_id` FROM `unit_master` LEFT JOIN `property_master` ON `unit_master_property_id`=`property_master_id` WHERE `property_master_bodycorp_id`=" + HttpContext.Current.Session["BID"].ToString() + ")";
            }


            Odbc o3 = new Odbc(constr);
            DataTable debtor = o3.ReturnTable(dsql, "debtor_temp");
            if (null != HttpContext.Current.Session["SelectedType"])
            {
                string selectedValue = HttpContext.Current.Session["SelectedType"].ToString();
                if (selectedValue == "1" || selectedValue == "2")
                {
                    debtor.DefaultView.RowFilter = "debtor_master_type_id = " + selectedValue;
                    debtor = debtor.DefaultView.ToTable();
                }
            }
            debtor.Columns.Add("Contact");
            debtor.Columns.Add("Address");
            foreach (DataRow dr in debtor.Rows)
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
            string sqlfromstr = "FROM " + debtor.TableName;
            if (HttpContext.Current.Session["debtormaster_bodycorpid"] != null)
            {
                sqlfromstr += " WHERE `debtor_master_id` IN (SELECT `unit_master_debtor_id` FROM `unit_master` LEFT JOIN `property_master` ON `unit_master_property_id`=`property_master_id` WHERE `property_master_bodycorp_id`=" + HttpContext.Current.Session["debtormaster_bodycorpid"].ToString() + ")";
                HttpContext.Current.Session["SelectedType"] = null;
            }
            string sqlselectstr = "SELECT `ID`, `Code`, `Name`,`Type`,`Address`, `Contact`  FROM (SELECT * ";
            JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, debtor.DefaultView.ToTable());
            string jsonStr = jqgridObj.GetJSONStr();
            return jsonStr;
        }
        #endregion

        #region WebControl Events
        protected void ImageButtonAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/debtormasteredit.aspx?mode=add", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            string debtormaster_id = "";
            try
            {
                debtormaster_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/debtormasteredit.aspx?mode=edit&debtorid=" + debtormaster_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonDelete_Click(string[] args)
        {
            string debtormaster_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                debtormaster_id = args[1];
                DebtorMaster bodycorp = new DebtorMaster(constr);
                bodycorp.Delete(Convert.ToInt32(debtormaster_id));
                Response.BufferOutput = true;
                Response.Redirect("~/debtormaster.aspx", false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonDetails_Click(string[] args)
        {
            string debtormaster_id = "";
            try
            {
                debtormaster_id = args[1];
                Session["debtormasterid"] = "1";
                Response.BufferOutput = true;
                string bid = "";
                if (Request.Cookies["bodycorpid"].Value != null)
                    bid = "&bodycorpid=" + Request.Cookies["bodycorpid"].Value;
                Response.Redirect("~/debtormasterdetails.aspx?debtorid=" + debtormaster_id + bid, false);

            }

            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonReports_Click(object sender, ImageClickEventArgs e)
        {
            if (Request.Cookies["bodycorpid"].Value != null)
            {
                Response.Write("<script type='text/javascript'> window.open('debtorreports.aspx','_blank'); </script>");
            }
            else
            {
                Response.Write("<script type='text/javascript'> window.open('debtorreports.aspx?','_blank'); </script>");
            }
        }
        #endregion

        protected void ComboBoxAllocated_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["SelectedType"] = ComboBoxAllocated.SelectedValue;
        }

        protected void ImageButtonExport_Click(object sender, ImageClickEventArgs e)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            Odbc o = new Odbc(constr);
            Odbc o2 = new Odbc(constr);

            string dbsql = "SELECT   bodycorps.bodycorp_id, property_master.property_master_id, debtor_master.debtor_master_id AS Expr1,                  debtor_master.* FROM      debtor_master, property_master, bodycorps, unit_master WHERE   debtor_master.debtor_master_id = unit_master.unit_master_debtor_id AND                  property_master.property_master_bodycorp_id = bodycorps.bodycorp_id AND                  property_master.property_master_id = unit_master.unit_master_property_id and bodycorp_id=" + Session["debtormaster_bodycorpid"].ToString();
            DataTable debtordt = o.ReturnTable(dbsql, "dbDT");
            //DataTable debtordt = ReportDT.getTable(constr, "debtor_master");
            debtordt.Columns.Add("Address1");
            debtordt.Columns.Add("Address2");
            debtordt.Columns.Add("Address3");
            debtordt.Columns.Add("Post1");
            debtordt.Columns.Add("Post2");
            debtordt.Columns.Add("Post3");
            debtordt.Columns.Add("PH_BUS");
            debtordt.Columns.Add("PH_RES");
            debtordt.Columns.Add("MOB");
            debtordt.Columns.Add("FAX");
            debtordt.Columns.Add("EMAIL");
            //Get address
            #region get Address

            string sql = "SELECT debtor_master.debtor_master_id, comm_master.comm_master_data, comm_master.comm_master_type_id, comm_types.comm_type_code, ";
            sql += " comm_types.comm_type_name ";
            sql += " FROM debtor_master, debtor_comms, comm_master, comm_types";
            sql += " WHERE debtor_master.debtor_master_id = debtor_comms.debtor_comm_debtor_id AND debtor_comms.debtor_comm_comm_id = comm_master.comm_master_id AND ";
            sql += " comm_master.comm_master_type_id = comm_types.comm_type_id";
            DataTable dt = o.ReturnTable(sql, "ContactDT");
            DataTable dt2 = new DataTable("");
            dt2.Columns.Add("Did");
            dt2.Columns.Add("Address");
            dt2.Columns.Add("Contact");
            foreach (DataRow dr in debtordt.Rows)
            {
                dt.DefaultView.RowFilter = "debtor_master_id = " + dr["debtor_master_id"].ToString();
                DataTable temp = dt.DefaultView.ToTable();
                string address1 = "";
                string address2 = "";
                string address3 = "";
                string post1 = "";
                string post2 = "";
                string post3 = "";
                string PH_BUS = "";
                string PH_RES = "";
                string MOB = "";
                string FAX = "";
                string EMAIL = "";
                foreach (DataRow dr2 in temp.Rows)
                {
                    int t = int.Parse(dr2["comm_master_type_id"].ToString());
                    if (t == 1)
                    {
                        if (address1.Equals(""))
                            address1 = dr2["comm_master_data"].ToString();
                        else if (address2.Equals(""))
                            address2 = dr2["comm_master_data"].ToString();
                        else if (address3.Equals(""))
                            address3 = dr2["comm_master_data"].ToString();
                    }
                    if (t == 2)
                    {
                        if (post1.Equals(""))
                            post1 = dr2["comm_master_data"].ToString();
                        else if (post2.Equals(""))
                            post2 = dr2["comm_master_data"].ToString();
                        else if (post3.Equals(""))
                            post3 = dr2["comm_master_data"].ToString();
                    }
                    if (t == 3)
                    {
                        PH_BUS = dr2["comm_master_data"].ToString();
                    }
                    if (t == 4)
                    {
                        PH_RES = dr2["comm_master_data"].ToString();
                    }
                    if (t == 5)
                    {
                        FAX = dr2["comm_master_data"].ToString();
                    }
                    if (t == 6)
                    {
                        EMAIL = dr2["comm_master_data"].ToString();
                    }
                    dr["Address1"] = address1;
                    dr["Address2"] = address2;
                    dr["Address3"] = address3;
                    dr["Post1"] = post1;
                    dr["Post2"] = post2;
                    dr["Post3"] = post3;
                    dr["PH_BUS"] = PH_BUS;
                    dr["PH_RES"] = PH_RES;
                    dr["MOB"] = MOB;
                    dr["FAX"] = FAX;
                    dr["EMAIL"] = EMAIL;

                }
            }
            debtordt.Columns.Remove("bodycorp_id");
            debtordt.Columns.Remove("debtor_master_bodycorp_id");
            debtordt.Columns.Remove("debtor_master_type_id");
            debtordt.Columns.Remove("property_master_id");
            debtordt.Columns.Remove("Expr1");
            debtordt.Columns.Remove("debtor_master_salutation");
            debtordt.Columns.Remove("debtor_master_payment_term_id");
            debtordt.Columns.Remove("debtor_master_payment_type_id");
            debtordt.Columns.Remove("debtor_master_print");
            debtordt.Columns.Remove("debtor_master_email");
            string path = "C:\\SMS\\Debtor" + DateTime.Now.ToString("ddmmyyyy") + ".csv";
            CsvDT.DataTableToCsv(debtordt, path);
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
            #endregion
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/debtorupload.aspx", false);
        }




    }
}