using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using Sapp.Common;
using Sapp.JQuery;
using Sapp.SMS;
using Sapp.Data;
using Sapp.General;

namespace sapp_sms
{
    public partial class levies : System.Web.UI.Page, IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridLevies };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                if (Request.Cookies["bodycorpid"].Value != null)
                    Session["levies_bodycorpid"] = Request.Cookies["bodycorpid"].Value;
                #endregion
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            Odbc o = new Odbc(AdFunction.conn);
            try
            {

                string[] args = eventArgument.Split('|');
                if (args[0] == "ImageButtonInvoice")
                {
                    ImageButtonInvoice_Click(args);
                }
                if (args[0] == "ImageButtonDelete")
                {

                    o.StartTransaction();
                    DataTable dt = o.ReturnTable("select * from levies where levy_batch_id=" + args[1], "t1");
                    {

          


                        foreach (DataRow ldr in dt.Rows)
                        {
                            string lid = ldr["levy_id"].ToString();
                            DataTable dt2 = o.ReturnTable("select * from invoice_master where invoice_master_batch_id=" + lid, "t1");
                            o.ExecuteScalar("delete from levy_lines where levy_line_levy_id=" + lid);
                            foreach (DataRow dr in dt2.Rows)
                            {
                                string invid = dr["invoice_master_id"].ToString();
                                InvoiceMaster im = new InvoiceMaster(AdFunction.conn);
                                im.SetOdbc(o);
                                im.Delete(int.Parse(invid));
                            }
                        }
                    }
                    o.ExecuteScalar("delete from levies where levy_batch_id=" + args[1]);

                    o.Commit();
                }
            }
            catch (Exception ex)
            {
                o.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebMethods For DataGrid
        [System.Web.Services.WebMethod]
        public static string jqGridLeviesDataBind(string postdata)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                Odbc mydb = new Odbc(constr);
                string bodycorp_id = "";
                if (HttpContext.Current.Session["levies_bodycorpid"] != null) bodycorp_id = HttpContext.Current.Session["levies_bodycorpid"].ToString();
                string sql = "SELECT `levy_batch_id` AS `ID` , `bodycorp_code` AS `Bodycorp` , DATE_FORMAT( `levy_date` , '%d/%m/%Y' ) AS `Date` , `levy_description` AS `Description` FROM (`levies` LEFT JOIN `bodycorps` ON `levy_bodycorp_id` = `bodycorp_id`) where levy_bodycorp_id= " + bodycorp_id + " group by ID";
                DataTable dt = mydb.ReturnTable(sql, "tempdt");
                dt.Columns.Add("Total");
                foreach (DataRow dr in dt.Rows)
                {
                    string id = dr["ID"].ToString();
                    string totalsql = "SELECT Sum(`levy_net`) FROM (`levies` LEFT JOIN `bodycorps` ON `levy_bodycorp_id` = `bodycorp_id`) where levy_batch_id=" + id + " and levy_bodycorp_id= " + bodycorp_id;
                    dr["Total"] = mydb.ReturnTable(totalsql, "total").Rows[0][0].ToString();
                }
                string sqlselectstr = "SELECT `ID`, `Bodycorp`, `Date`, `Description`,`Total` FROM (SELECT * ";
                string sqlfromstr = "FROM `" + dt.TableName + "`";
                //JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr);
                JQGrid jqgridObj = new JQGrid(postdata, constr, sqlfromstr, sqlselectstr, dt);
                string jsonStr = jqgridObj.GetJSONStr();
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return "{}";
        }


        #endregion

        #region WebControl Events
        private void ImageButtonInvoice_Click(string[] args)
        {
            try
            {
                string levy_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/invoicemaster.aspx?levyid=" + levy_id, false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        //protected void Timer1_Tick(object sender, EventArgs e)
        //{
        //    if (Session["LevyFinish"] != null)
        //    {
        //        if (!(bool)Session["LevyFinish"])
        //        {
        //            Timer1.Enabled = true;
        //            Image1.Visible = true;
        //        }
        //        else
        //        {
        //            Timer1.Enabled = false;
        //            Image1.Visible = false;
        //        }
        //    }
        //    else
        //    {
        //        Timer1.Enabled = false;
        //        Image1.Visible = false;
        //    }
        //}

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {

        }
    }
}