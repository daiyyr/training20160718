using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Configuration;
using System.IO;
using Sapp.Common;
using Sapp.SMS;
using Sapp.JQuery;
using Sapp.Data;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Globalization;
using System.Threading;

using System.Data.OleDb;
using System.Data.Odbc;
using System.Net.Mail;
using System.Net;

namespace sapp_sms
{
    public partial class journalupload : System.Web.UI.Page
    {
        public int irow = 1;    // Update 19/06/2016
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        protected void Button1_Click1(object sender, System.EventArgs e)
        {
            Odbc o = new Odbc(AdFunction.conn);
            o.StartTransaction();
            try
            {

                FileUpload1.SaveAs(Server.MapPath("~") + "\\temp\\JNL.csv");
                string path = Server.MapPath("~") + "\\temp\\";

                string jnlNum = Journal.GetNextNumber();
                DataTable dt = CsvDT.CsvToDataTable(path, "JNL.csv");
                foreach (DataRow dr in dt.Rows)
                {
                    irow++;
                    string bcode = dr["BodycorpCode"].ToString();
                    if (bcode.Equals(""))
                    {
                        // Update 19/06/2016
                        //jnlNum = Journal.GetNextNumber();
                        throw new Exception(CsvDT.editErrorMessage(irow, "BodycorpCode", "bodycorp code is empty"));
                    }
                    else
                    {
                        Bodycorp bc = new Bodycorp(AdFunction.conn);
                        bc.SetOdbc(o);
                        bc.LoadData(bcode);

                        // Add 19/06/2016
                        if (bc.BodycorpId == 0)
                        {
                            throw new Exception(CsvDT.editErrorMessage(irow, "BodycorpCode", "bodycorp code [" + bcode + "] could not be found in DB"));
                        }

                        string unitcode = dr["UnitNum"].ToString();
                        string uid = "null";
                        if (!unitcode.Equals(""))
                        {
                            DataTable uDT = o.ReturnTable("SELECT   unit_master.* FROM      bodycorps, property_master, unit_master WHERE   bodycorps.bodycorp_id = property_master.property_master_bodycorp_id AND  property_master.property_master_id = unit_master.unit_master_property_id and unit_master_code='" + unitcode + "'  AND bodycorps.bodycorp_id = " + AdFunction.BodyCorpID, "t1");
                            if (uDT.Rows.Count > 0)
                            {
                                uid = uDT.Rows[0]["unit_master_id"].ToString();
                            }
                            else
                            {
                                // Add 19/06/2016
                                throw new Exception(CsvDT.editErrorMessage(irow, "UnitNum", "unit master code [" + unitcode + "] could not be found in DB"));
                            }
                        }
                        else
                        {
                            // Add 19/06/2016
                            throw new Exception(CsvDT.editErrorMessage(irow, "UnitNum", "unit master code is empty"));
                        }

                        string chartcode = dr["ChartCode"].ToString();
                        // Add 19/06/2016
                        if ("".Equals(chartcode))
                        {
                            throw new Exception(CsvDT.editErrorMessage(irow, "ChartCode", "chart code is empty"));
                        }

                        ChartMaster c = new ChartMaster(AdFunction.conn);
                        c.SetOdbc(o);
                        c.LoadData(chartcode);
                        // Add 19/06/2016
                        if (c.ChartMasterId == 0)
                        {
                            throw new Exception(CsvDT.editErrorMessage(irow, "ChartCode", "chart code [" + chartcode + "] could not be found in DB"));
                        }

                        // Add 19/06/2016
                        if (dr["DR"].ToString().Equals("") && dr["CR"].ToString().Equals(""))
                        {
                            throw new Exception(CsvDT.editErrorMessage(irow, "DR or CR", "both DR and CR are empty"));
                        }

                        decimal net = 0;
                        if (dr["DR"].ToString().Equals(""))
                            net = decimal.Parse(dr["CR"].ToString());
                        if (dr["CR"].ToString().Equals(""))
                            net = -decimal.Parse(dr["DR"].ToString());

                        // Add 19/06/2016
                        if ("".Equals(dr["Date"].ToString()))
                        {
                            throw new Exception(CsvDT.editErrorMessage(irow, "Date", "journal date is empty"));
                        }
                        string date = DBSafeUtils.DateToSQL(dr["Date"].ToString());
                        string description = dr["Description"].ToString();
                        GLInsert(o, bc.BodycorpId.ToString(), "", "", "6", "6", jnlNum, c.ChartMasterId.ToString(), uid, description, net.ToString(), date, date, "0", "0");
                    }
                }

                FileInfo f = new FileInfo(path + "JNL.csv");
                f.Delete();
                o.Commit();
                Response.Redirect("journalList.aspx", false);
            }
            catch (Exception ex)
            {
                o.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString() + "<br><br>Error At Import File Row Index:" + irow.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }
        public string QuString(string text)
        {
            return "'" + text + "'";
        }
        public string GLInsert(Odbc o, string bid, string oldrefernce, string oldid, string typeid, string reftyprid, string reference, string chartid, string unitid, string description, string net, string date, string createdate, string rev, string rec, string tax = "0", string gross = "0", string creditorID = "null")
        {
            string gl_insert = "insert into gl_transactions (`gl_transaction_oldref`,`gl_trasaction_oldid`,`gl_transaction_type_id`,`gl_transaction_ref_type_id`,`gl_transaction_ref`,`gl_transaction_chart_id`,`gl_transaction_bodycorp_id`,`gl_transaction_unit_id`,`gl_transaction_description`,`gl_transaction_net`,`gl_transaction_tax`,`gl_transaction_gross`,`gl_transaction_date`,`gl_transaction_createdate`,`gl_transaction_rev`,`gl_transaction_rec`,`gl_transaction_creditor_id`)";
            gl_insert += " values (" + QuString(oldrefernce) + "," + QuString(oldid) + "," + typeid + "," + reftyprid + ",'" + reference + "'," + chartid + "," + bid + "," + unitid + ",'" + description + "'," + net + "," + tax + "," + gross + "," + date + "," + createdate + "," + rev + "," + rec + "," + creditorID + ")";
            o.ExecuteScalar(gl_insert);
            return o.ExecuteScalar("SELECT LAST_INSERT_ID()").ToString();
        }


    }
}