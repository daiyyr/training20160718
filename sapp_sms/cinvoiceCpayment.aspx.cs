using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Text;
using Sapp.Common;
using Sapp.SMS;
using Sapp.JQuery;
using System.Data;
using Sapp.Data;

namespace sapp_sms
{
    public partial class cinvoiceCpayment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    AjaxControlUtils.SetupComboBox(TypeDL, "SELECT `payment_type_id` AS `ID`, `payment_type_code` AS `Code` FROM `payment_types`", "ID", "Code", AdFunction.conn, false);
                    TypeDL.SelectedIndex = 0;
                    string cinvid = "";
                    if (Request.QueryString["cinvid"] != null) cinvid = Request.QueryString["cinvid"];
                    Cinvoice c = new Cinvoice(AdFunction.conn);
                    c.LoadData(int.Parse(cinvid));

                    AmountT.Text = c.CinvoiceGross.ToString();

                    TextBoxDate.Text = c.CinvoiceDate.ToString("dd/MM/yyyy");

                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }


        protected void ButtonInsert_Click(object sender, EventArgs e)
        {
            try
            {
                string cinvid = "";
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                #region Load System Chart ID

                Sapp.SMS.System s = new Sapp.SMS.System(AdFunction.conn);
                ChartMaster ch = new ChartMaster(AdFunction.conn);
                s.LoadData("GST Input");
                ch.LoadData(s.SystemValue);
                string InputGstID = ch.ChartMasterId.ToString();
                s.LoadData("GST Output");
                ch.LoadData(s.SystemValue);
                string OutputGstID = ch.ChartMasterId.ToString();
                s.LoadData("GENERALTAX");
                ch.LoadData(s.SystemValue);
                string gstid = ch.ChartMasterId.ToString();
                s.LoadData("GENERALDEBTOR");
                ch.LoadData(s.SystemValue);
                ch.LoadData(s.SystemValue);
                string proprietorID = ch.ChartMasterId.ToString();
                s.LoadData("GENERALCREDITOR");
                ch.LoadData(s.SystemValue);
                string creditorID = ch.ChartMasterId.ToString();
                s.LoadData("DISCOUNTCHARCODE");
                ch.LoadData(s.SystemValue);
                string discountID = ch.ChartMasterId.ToString();

                #endregion

                if (Request.QueryString["cinvid"] != null) cinvid = Request.QueryString["cinvid"];
                if (!Page.IsValid) return;

                Cinvoice c = new Cinvoice(constr);
                c.LoadData(int.Parse(cinvid));

                Hashtable items = new Hashtable();

                items.Add("cpayment_bodycorp_id", c.CinvoiceBodycorpId);
                items.Add("cpayment_creditor_id", c.CinvoiceCreditorId);
                items.Add("cpayment_reference", "'" + ReferenceT.Text + "'");

                items.Add("cpayment_type_id", TypeDL.SelectedValue);
                items.Add("cpayment_gross", AmountT.Text);

                items.Add("cpayment_date", DBSafeUtils.DateToSQL(TextBoxDate.Text));


                CPayment cpayment = new CPayment(constr);
                cpayment.Add(items);


                Hashtable allowitems = new Hashtable();
                allowitems.Add("cinvoice_id", c.CinvoiceId);
                allowitems.Add("gl_transaction_description", "CPayment");
                allowitems.Add("gl_transaction_tax", "0");
                allowitems.Add("gl_transaction_date", "'" + DateTime.Parse(TextBoxDate.Text).ToString("yyyy-MM-dd") + "'");
                allowitems.Add("gl_transaction_type_id", "4");
                allowitems.Add("gl_transaction_gross", "0");
                allowitems.Add("gl_transaction_chart_id", creditorID);
                allowitems.Add("gl_transaction_ref", "'" + c.CinvoiceNum + "'");
                allowitems.Add("gl_transaction_bodycorp_id", c.CinvoiceBodycorpId);
                allowitems.Add("gl_transaction_net", c.CinvoiceGross);
                cpayment.AddGlTran(allowitems, true);



                c.UpdatePaid();
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.returnValue = 'refresh'; window.close();", true);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
    }
}