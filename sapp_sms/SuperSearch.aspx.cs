using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using Sapp.Common;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.SMS;
using System.Text.RegularExpressions;
using System.Data;
namespace sapp_sms
{
    public partial class SuperSearch : System.Web.UI.Page
    {
        static string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        Odbc o = new Odbc(constr);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataTable bodycorpDT = ReportDT.getTable(constr, "bodycorps");
                DropDownList1.DataSource = bodycorpDT;
                DropDownList1.DataTextField = "bodycorp_name";
                DropDownList1.DataValueField = "bodycorp_id";
                DropDownList1.DataBind();
                if (Request.Cookies["bodycorpid"].Value != null)
                    DropDownList1.SelectedValue = Request.Cookies["bodycorpid"].Value;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string gl_sql = "select * from(( SELECT *,   concat(gl_transaction_ref ,gl_transaction_description, CAST(gl_transaction_net as char) , CAST(gl_transaction_date as char)) as `skey`FROM      gl_transactions) AS T1) where skey like '%" + SearchT.Text + "%'";
                string invoice_sql = "select * from ( (SELECT  *,  concat( unit_master.unit_master_code  , CAST( debtor_master.debtor_master_name as char) , CAST( debtor_master.debtor_master_name as char) , CAST( invoice_master.invoice_master_num as char) , CAST( invoice_master.invoice_master_date as char) , CAST( invoice_master.invoice_master_due as char) , CAST( invoice_master.invoice_master_description as char) , CAST( invoice_master.invoice_master_net as char) , CAST(                  invoice_master.invoice_master_tax as char) , CAST( invoice_master.invoice_master_gross as char) , CAST( invoice_master.invoice_master_paid as char )) as skey FROM      invoice_master, debtor_master, unit_master WHERE   invoice_master.invoice_master_debtor_id = debtor_master.debtor_master_id AND                  invoice_master.invoice_master_unit_id = unit_master.unit_master_id) as t1) where skey like '%" + SearchT.Text + "%' ";
                //string receipt_sql = "select * from( ( SELECT *, concat(   unit_master.unit_master_code  , CAST( debtor_master.debtor_master_code as char) , CAST( debtor_master.debtor_master_name as char) , CAST( receipts.receipt_ref as char) , CAST( receipts.receipt_gross as char) ,CAST( receipts.receipt_date as char) , CAST( receipts.receipt_notes as char) , CAST(                  receipts.receipt_allocated as char)) as skey FROM      receipts, unit_master, debtor_master WHERE   receipts.receipt_unit_id = unit_master.unit_master_id AND receipts.receipt_debtor_id = debtor_master.debtor_master_id) as t1) where skey like '%" + SearchT.Text + "%' ";
                //string cinvoice_sql = "select * from (( SELECT *,  concat( CAST( creditor_master.creditor_master_code as char) , CAST( creditor_master.creditor_master_name as char) , CAST( unit_master.unit_master_code as char) , CAST(                   cinvoices.cinvoice_num as char) , CAST( cinvoices.cinvoice_date as char) ,  CAST( cinvoices.cinvoice_net as char) ,   CAST(   cinvoices.cinvoice_tax as char)) as skey  FROM      cinvoices, unit_master, creditor_master  WHERE   cinvoices.cinvoice_unit_id = unit_master.unit_master_id AND                   cinvoices.cinvoice_creditor_id = creditor_master.creditor_master_id) as t1) where skey like '%" + SearchT.Text + "%'  ";
                string receipt_sql = "select * from( ( SELECT *, concat(   unit_master.unit_master_code  , CAST( debtor_master.debtor_master_code as char) , CAST( debtor_master.debtor_master_name as char) , CAST( receipts.receipt_ref as char) , CAST( receipts.receipt_gross as char) ,CAST( receipts.receipt_date as char) , CAST( receipts.receipt_notes as char) , CAST(                  receipts.receipt_allocated as char)) as skey FROM      receipts, unit_master, debtor_master WHERE   receipts.receipt_unit_id = unit_master.unit_master_id AND receipts.receipt_debtor_id = debtor_master.debtor_master_id) as t1) where skey like '%" + SearchT.Text + "%' ";
                string cinvoice_sql = "select * from (( SELECT *,  concat( CAST( creditor_master.creditor_master_code as char) , CAST( creditor_master.creditor_master_name as char) , CAST( unit_master.unit_master_code as char) , CAST(                   cinvoices.cinvoice_num as char) , CAST( cinvoices.cinvoice_date as char) ,  CAST( cinvoices.cinvoice_net as char) ,   CAST(   cinvoices.cinvoice_tax as char)) as skey  FROM      cinvoices, unit_master, creditor_master  WHERE   cinvoices.cinvoice_unit_id = unit_master.unit_master_id AND                   cinvoices.cinvoice_creditor_id = creditor_master.creditor_master_id) as t1) where skey like '%" + SearchT.Text + "%'  ";
          
                
                string cpay_sql = "select * from (( SELECT  *,  concat( creditor_master.creditor_master_code, CAST( creditor_master.creditor_master_name as char) , CAST( cpayments.cpayment_reference as char) , CAST(                  cpayments.cpayment_gross as char) , CAST( cpayments.cpayment_date as char) , CAST( cpayments.cpayment_allocated as char)) as skey FROM      cpayments, creditor_master WHERE   cpayments.cpayment_creditor_id = creditor_master.creditor_master_id) as t1) where skey like '%" + SearchT.Text + "%'  ";


                DataTable glDT = o.ReturnTable(gl_sql, "gl");
                glDT = ReportDT.FilterDT(glDT, "gl_transaction_bodycorp_id=" + DropDownList1.SelectedValue);
                DataTable invoiceDT = o.ReturnTable(invoice_sql, "inv");
                invoiceDT = ReportDT.FilterDT(invoiceDT, "invoice_master_bodycorp_id=" + DropDownList1.SelectedValue);
                DataTable recDT = o.ReturnTable(receipt_sql, "rec");
                recDT = ReportDT.FilterDT(recDT, "receipt_bodycorp_id=" + DropDownList1.SelectedValue);
                DataTable cinDT = o.ReturnTable(cinvoice_sql, "cin");
                cinDT = ReportDT.FilterDT(cinDT, "cinvoice_bodycorp_id=" + DropDownList1.SelectedValue);
                DataTable cpay = o.ReturnTable(cpay_sql, "cpay");
                cpay = ReportDT.FilterDT(cpay, "cpayment_bodycorp_id=" + DropDownList1.SelectedValue);

                GridView1.DataSource = glDT;
                GridView2.DataSource = invoiceDT;
                GridView3.DataSource = recDT;
                GridView4.DataSource = cinDT;
                GridView5.DataSource = cpay;

                GridView1.DataBind();
                GridView2.DataBind();
                GridView3.DataBind();
                GridView4.DataBind();
                GridView5.DataBind();

                foreach (GridViewRow gr in GridView2.Rows)
                {
                    HyperLink hl = (HyperLink)gr.Cells[0].FindControl("HyperLink1");
                    hl.NavigateUrl = "~/invoicemasterdetails.aspx?invoicemasterid=" + hl.NavigateUrl.ToString();
                }
                foreach (GridViewRow gr in GridView4.Rows)
                {
                    HyperLink hl = (HyperLink)gr.Cells[0].FindControl("HyperLink1");
                    hl.NavigateUrl = "~/cinvoicedetails.aspx?cinvoiceid=" + hl.NavigateUrl.ToString();
                }
                foreach (GridViewRow gr in GridView3.Rows)
                {
                    HyperLink hl = (HyperLink)gr.Cells[0].FindControl("HyperLink1");
                    hl.NavigateUrl = "~/receiptdetails.aspx?receiptid=" + hl.NavigateUrl.ToString();
                }
                foreach (GridViewRow gr in GridView5.Rows)
                {
                    HyperLink hl = (HyperLink)gr.Cells[0].FindControl("HyperLink1");
                    hl.NavigateUrl = "~/cpaymentdetails.aspx?cpaymentid=" + hl.NavigateUrl.ToString();
                }
                if (GridView1.Rows.Count > 0)
                {
                    GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
                }

                if (GridView2.Rows.Count > 0)
                {
                    GridView2.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
                if (GridView3.Rows.Count > 0)
                {
                    GridView3.HeaderRow.TableSection = TableRowSection.TableHeader;
                }

                if (GridView4.Rows.Count > 0)
                {
                    GridView4.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
                if (GridView5.Rows.Count > 0)
                {
                    GridView5.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            catch (Exception ex)
            {
            }
        }

    }
}