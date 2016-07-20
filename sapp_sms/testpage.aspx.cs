using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using Sapp.Common;

namespace sapp_sms
{
    public partial class testpage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //TreeNode tn = new TreeNode("Root");
            //tn.ChildNodes.Add(new TreeNode("Employee1"));
            //tn.ChildNodes.Add(new TreeNode("Employee2"));
            //TreeView1.Nodes.Add(tn);
        }



        [System.Web.Services.WebMethod]
        public static string DataTreeDataBind(string postdata)
        {
            try
            {
                string jsonStr = "{\"data\":[{\"data\":\"node 1\"},{\"data\":\"node 2\"}]}";
                return jsonStr;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
            return null;
        }

        protected void ButtonUpload_Click(object sender, EventArgs e)
        {
            try
            {
                //if (FileUploadDataFile.HasFile)
                //{
                //    string dirpath = Server.MapPath("~/uploads");
                //    FileUploadDataFile.SaveAs(dirpath + "\\" + FileUploadDataFile.FileName);
                //}
                //else
                //{
                //    throw new Exception("Error: please select a data file!");
                //}
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        protected void ButtonCSV_Click(object sender, EventArgs e)
        {
            CSVIO csvio = new CSVIO();
            string dirpath = Server.MapPath("~/uploads");
            DirectoryInfo di = new DirectoryInfo(dirpath);
            FileInfo[] files = di.GetFiles();
            foreach (FileInfo file in files)
            {
                csvio.CsvReader(file.FullName);
                DataTable dt = csvio.CsvImport();
                string s = "";
            }
        }

        protected void ButtonDown_Click(object sender, EventArgs e)
        {
            CSVIO csvio = new CSVIO();
            string dirpath = Server.MapPath("~/downloads");
            csvio.CsvWriter(dirpath + "/test.csv");
            DataTable dt = new DataTable();
            dt.Columns.Add("c1");
            dt.Columns.Add("c2");
            dt.Columns.Add("c3");
            DataRow dr = dt.NewRow();
            dr[0] = "1";
            dr[1] = "2";
            dr[2] = "3";
            dt.Rows.Add(dr);
            csvio.CsvExport(dt);
            string s = "";
        }

    }
}