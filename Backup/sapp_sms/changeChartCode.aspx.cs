using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data;
using Sapp.Data;
using Sapp.JQuery;
using Sapp.General;
using Sapp.Common;
using Sapp.SMS;
using Sapp.General;
namespace sapp_sms
{
    public partial class changeChartCode : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string path = Server.MapPath("~") + "temp";
            FileUpload1.SaveAs(path + "\\temp.csv");
            DataTable dt = CsvDT.CsvToDataTable(path, "\\temp.csv");
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            //Odbc o = new Odbc(AdFunction.conn);
            //o.StartTransaction();
            try
            {
                MessageL.Text = "";
                string path = Server.MapPath("~") + "temp";
                DataTable dt = CsvDT.CsvToDataTable(path, "\\temp.csv");
                DataTable chartDT = ReportDT.getTable(AdFunction.conn, "chart_master");
                foreach (DataRow dr in dt.Rows)
                {
                    string reference = dr["Reference"].ToString();
                    string description = dr["Inv"].ToString();
                    decimal net = decimal.Parse(dr["Amount"].ToString());
                    string cinvID = reference.Remove(0, 3);

                    string chartcode = dr["Code"].ToString();


                    //string chartsql = "select * from chart_master where chart_master_code='" + chartcode + "'";
                    //DataTable chartDT = AdFunction.odbc.ReturnTable(chartsql, "");
                    DataTable tempchartDT = ReportDT.FilterDT(chartDT, "chart_master_code = '" + chartcode + "'");

                    if (tempchartDT.Rows.Count > 0)
                    {
                        string chartid = tempchartDT.Rows[0]["chart_master_id"].ToString();
                        string sql = "SELECT   * FROM      cinvoices, cinvoice_gls, gl_transactions, chart_master WHERE   cinvoices.cinvoice_id = cinvoice_gls.cinvoice_gl_cinvoice_id AND                 cinvoice_gls.cinvoice_gl_gl_id = gl_transactions.gl_transaction_id AND                 gl_transactions.gl_transaction_chart_id = chart_master.chart_master_id";
                        sql = sql + " And (gl_transaction_net=" + net + " or gl_transaction_net=" + -net + ") and cinvoice_id = '" + cinvID + "'";
                        Odbc odbc = new Odbc(AdFunction.conn);
                        DataTable tempdt = odbc.ReturnTable(sql, "dt");
                        if (tempdt.Rows.Count > 0)
                        {
                            string glid = tempdt.Rows[0]["gl_transaction_id"].ToString();
                            string updatesql = "update  gl_transactions set gl_transaction_chart_id=" + chartid + " where gl_transaction_id =" + glid;
                            odbc.ExecuteScalar(updatesql);
                        }
                        else
                        {
                            MessageL.Text += "GL ID Error: Net: " + net + " Reference: " + reference + " Description: " + description + "<br>";
                        }
                    }
                    else

                        MessageL.Text += ("Chart Code Error:" + chartcode) + "<br>";

                }
                MessageL.Text += "Success";
            }
            catch (Exception ex)
            {
                //o.Rollback();
                MessageL.Text = ex.Message;
            }
        }
    }

}