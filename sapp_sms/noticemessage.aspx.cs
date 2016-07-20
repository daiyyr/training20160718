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
using System;
using System.Collections.Generic;
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
    public partial class noticemessage : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["title"] != null)
                    TextBox1.Text = Server.UrlDecode(Request.QueryString["title"].ToString());
                Date_HF.Value = Server.UrlDecode(Request.QueryString["date"]);
                PID_HF.Value = Server.UrlDecode(Request.QueryString["propertyid"]);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(Send));
            t.Start();
            Response.Redirect("bodycorps.aspx", false);
        }
        public void Send()
        {
            string constr = ConfigurationManager.ConnectionStrings["constr2"].ConnectionString;
            string constr2 = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            DataTable dt = ReportDT.getTable(constr, "users");
            DateTime sdate = DateTime.Parse(Date_HF.Value);
            string bid = ReportDT.GetDataByColumn(ReportDT.getTable(constr2, "property_master"), "property_master_id", PID_HF.Value, "property_master_bodycorp_id");
            string subject = TextBox1.Text;
            string body = TextBox2.Text;
            DataTable managerDT = ReportDT.getTable(constr2, "bodycorp_managers");
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["user_privilege_id"].ToString().Equals("1"))
                {
                    string email = dr["user_email"].ToString();
                    string name = dr["user_name"].ToString();
                    if (!email.Equals(""))
                    {


                        DataTable checkDT = ReportDT.FilterDT(managerDT, "bodycorp_manager_bodycorp_id=" + bid + " and bodycorp_manager_user_id=" + dr["user_id"].ToString());
                        if (checkDT.Rows.Count > 0)
                            CreateAppointment(email, name, sdate, sdate, subject, body);
                    }
                }
            }
        }
        public void CreateAppointment(string email, string username, DateTime satrtDateTime, DateTime endDateTime, string subject, string body)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            DataTable sys = ReportDT.getTable(constr, "system");
            string strHost = ReportDT.GetDataByColumn(sys, "system_code", "EMAILSERVER", "system_value");  //STMP服务器地址
            string strAccount = ReportDT.GetDataByColumn(sys, "system_code", "EMAILUSER", "system_value");       //SMTP服务帐号
            string strPwd = ReportDT.GetDataByColumn(sys, "system_code", "EMAILPWD", "system_value");     //SMTP服务密码
            var fromAddress = new MailAddress(strAccount, "Test");
            var toAddress = new MailAddress(email, username);
            SmtpClient sc = sc = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, strPwd)
            };
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(strAccount, "Notice");
            msg.To.Add(new MailAddress(email, username));
            msg.Subject = TextBox1.Text;
            msg.Body = TextBox2.Text;


            StringBuilder str = new StringBuilder();
            str.AppendLine("BEGIN:VCALENDAR");
            str.AppendLine("PRODID:-//Niti Jotani");
            str.AppendLine("VERSION:2.0");
            str.AppendLine("METHOD:REQUEST");
            str.AppendLine("BEGIN:VEVENT");

            str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", satrtDateTime)); //Here startDuration is the startDate-Time of the meeting. It should be in DateTime Format.
            str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", endDateTime)); //Here endDuration is the endDate-Time of the meeting. It should be in DateTime Format. Here yyyyMMddTHHmmssZ means yyyy-year, MM-month, dd-day, T-is just a seperator between Date And Time, HH-hour, mm-min, ss-sec, Z-indicator of the end

            str.AppendLine("LOCATION: Gandhinagar");
            str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
            str.AppendLine(string.Format("DESCRIPTION:{0}", msg.Body));
            str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", msg.Body));
            str.AppendLine(string.Format("SUMMARY:{0}", msg.Subject));
            str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", msg.From.Address));

            str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", msg.To[0].DisplayName, msg.To[0].Address));

            str.AppendLine("BEGIN:VALARM");
            str.AppendLine("TRIGGER:-PT15M");
            str.AppendLine("ACTION:DISPLAY");
            str.AppendLine("DESCRIPTION:Reminder");
            str.AppendLine("END:VALARM");
            str.AppendLine("END:VEVENT");
            str.AppendLine("END:VCALENDAR");


            System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("text/calendar");

            ct.Parameters.Add("method", "REQUEST");
            AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), ct);
            msg.AlternateViews.Add(avCal);

            // If we are using the IIS SMTP Service, we can write the message directly to the //PickupDirectory and bypass the Network Layer
            //sc.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
            sc.Send(msg);
        }

        protected void Button2_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("bodycorps.aspx",false);
        }
    }
}