using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Sapp.Common;
using System.Configuration;
using Sapp.JQuery;
using Sapp.SMS;

namespace sapp_sms
{
    public partial class purchordermaster : System.Web.UI.Page, System.Web.UI.IPostBackEventHandler
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
            try
            {
                #region Javascript setup
                Control[] wc = { jqGridTable };
                JSUtils.RenderJSArrayWithCliendIds(Page, wc);
                #endregion
                Session["SelectedPurchOrderAllocatedType"] = ComboBoxAllocated.SelectedValue;
                string qscreditorid = CheckedQueryString();
                Session["creditorid"] = qscreditorid;
                
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
                else if (args[0] == "ImageButtonDetails")
                {
                    ImageButtonDetails_Click(args);
                }
                else if (args[0] == "ImageButtonEdit")
                {
                    ImageButtonEdit_Click(args);
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
                string sqlfromstr = "FROM (`purchorder_master` left join `purchorder_types`  on `purchorder_master_type_id`=`purchorder_type_id` left join `creditor_master`  on `purchorder_master_creditor_id`=`creditor_master_id` left join `unit_master` on `purchorder_master_unit_id`=`unit_master_id`  left join `bodycorps`  on `bodycorp_id`=`purchorder_master_bodycorp_id`) ";
                if (null != HttpContext.Current.Session["SelectedPurchOrderAllocatedType"])
                {
                    string selectedValue = HttpContext.Current.Session["SelectedPurchOrderAllocatedType"].ToString();
                    if (selectedValue == "1" || selectedValue=="0")
                    {
                        sqlfromstr += " where `purchorder_master_allocated`="+selectedValue;
                    }
                }
                if (null != HttpContext.Current.Session["creditorid"] && !String.IsNullOrEmpty(HttpContext.Current.Session["creditorid"].ToString()))
                {
                    string sql = " where ";
                    if (sqlfromstr.Contains("where")) sql = " and ";
                    sql += " purchorder_master_creditor_id=" + HttpContext.Current.Session["creditorid"].ToString();
                    sqlfromstr += sql;
                    HttpContext.Current.Session["creditorid"] = "";
                }
                string sqlselectstr = "SELECT `ID`,`Num`,`Type`,`Creditor`,`Bodycorp`,`Unit`,`Date`,`Allocated` from (select  `purchorder_master_id` as `ID`,`purchorder_master_num` as `Num`,`purchorder_type_code` as `Type`, DATE_FORMAT(`purchorder_master_date`, '%d/%m/%Y') as `Date`,`creditor_master_code` as `Creditor`, `unit_master_code` as `Unit`, `bodycorp_code` as `Bodycorp`, REPLACE(REPLACE(`purchorder_master_allocated`, '0', 'False'), '1', 'True') as `Allocated` ";

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
        #endregion

        #region WebControl Events

        private void ImageButtonDelete_Click(string[] args)
        {
            string purchordermaster_id = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                purchordermaster_id = args[1];
                PurchorderMaster purchordermaster = new PurchorderMaster(constr);
                purchordermaster.Delete(Convert.ToInt32(purchordermaster_id));
                Response.BufferOutput = true;
                Response.Redirect("~/purchordermaster.aspx" + NewQueryString("?"),false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ImageButtonAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.BufferOutput = true;
                Response.Redirect("~/purchordermasteredit.aspx?mode=add" + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        private void ImageButtonEdit_Click(string[] args)
        {
            string purchordermaster_id = "";
            try
            {
                purchordermaster_id = args[1];

                Response.BufferOutput = true;
                Response.Redirect("~/purchordermasteredit.aspx?mode=edit&purchordermasterid=" + purchordermaster_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        private void ImageButtonDetails_Click(string[] args)
        {
            string purchordermaster_id = "";
            try
            {
                purchordermaster_id = args[1];
                Response.BufferOutput = true;
                Response.Redirect("~/purchordermasterdetails.aspx?purchordermasterid=" + purchordermaster_id + NewQueryString("&"), false);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        #endregion

        protected void ComboBoxAllocated_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    
}