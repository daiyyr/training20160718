using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections;
using System.Data.Odbc;
using Sapp.SMS;
using Sapp.Data;
using Sapp.Common;
using System.Data;
namespace sapp_sms
{
    public partial class ownershiptransfer : System.Web.UI.Page, IPostBackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    #region Initial Web Controls
                    AdFunction.Debtor_ComboBox(ComboBoxDebtor, "1", Request.Cookies["bodycorpid"].Value, true);   // Update 28/04/2016
                    //AjaxControlUtils.SetupComboBox(ComboBoxDebtor, "SELECT `debtor_master_id` AS `ID`, `debtor_master_name` AS `Code` FROM `debtor_master` Where `debtor_master_type_id`=1 order by Code", "ID", "Code", constr, false);
                    //if (Request.QueryString["s"] != null)
                    if (Request.QueryString["debtormasterid"] != null)          // Update 28/04/2016
                    {
                        DebtorMaster dm = new DebtorMaster(constr);
                        dm.LoadData(Request.QueryString["debtormasterid"]);
                        ComboBoxDebtor.SelectedValue = dm.DebtorMasterId.ToString();
                        ShowProprietorDetail(dm.DebtorMasterId.ToString());     // Add 28/04/2016
                    }
                    else
                    {
                        ComboBoxDebtor.SelectedValue = "null";
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
                }
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            try
            {
                string[] args = eventArgument.Split('|');
                if (args[0] == "ImageButtonSubmit")
                {
                    ImageButtonSubmit_Click(args);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);
            }
        }

        #region WebControl Events
        // Update 28/04/2016
        //protected void ImageButtonSubmit_Click(object sender, ImageClickEventArgs e)
        protected void ImageButtonSubmit_Click(string[] args)
        {
            Odbc mydb = null;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                mydb = new Odbc(constr);
                mydb.StartTransaction();
                string unit_id = Request.QueryString["unitmasterid"].ToString();
                UnitMaster unit = new UnitMaster(constr);
                unit.SetOdbc(mydb);
                unit.LoadData(Convert.ToInt32(unit_id));


                int debtor_id = AjaxControlUtils.ComboBoxSelectedValue(ComboBoxDebtor, false).Value;

                string odmID = unit.UnitMasterDebtorId.ToString();
                DebtorMaster olddm = new DebtorMaster(constr);
                olddm.LoadData(unit.UnitMasterDebtorId);
                DebtorMaster newdm = new DebtorMaster(constr);
                newdm.LoadData(debtor_id);
                string nowCODE = newdm.DebtorMasterCode;
                string oldCODE = olddm.DebtorMasterCode;
                newdm.DebtorMasterCode = oldCODE;
                olddm.DebtorMasterCode = nowCODE;

                string tempCODE = Guid.NewGuid().ToString();

                Hashtable ditems = new Hashtable();
                ditems.Add("debtor_master_code", "'" + oldCODE + "'");
                Hashtable titems = new Hashtable();
                titems.Add("debtor_master_code", "'" + tempCODE + "'");
                Hashtable oditems = new Hashtable();
                oditems.Add("debtor_master_code", "'" + nowCODE + "'");


                newdm.Update(titems);
                olddm.Update(oditems);
                newdm.Update(ditems);
                DateTime start_date = Convert.ToDateTime(TextBoxStart.Text);
                DateTime last_end = new DateTime();

                #region Get Last OS Last End Date

                string sql = "SELECT MAX(`ownership_end`) FROM `ownerships` WHERE `ownership_unit_id`=" + unit_id;
                object id = mydb.ExecuteScalar(sql);
                if (id != DBNull.Value)
                    last_end = Convert.ToDateTime(id);
                else
                    last_end = unit.UnitMasterBeginDate;
                #endregion
                #region Create New OS for Current
                Hashtable items = new Hashtable();
                items.Add("ownership_unit_id", unit_id);
                items.Add("ownership_debtor_id", unit.UnitMasterDebtorId);
                items.Add("ownership_start", DBSafeUtils.DateToSQL(last_end.AddDays(1)));
                items.Add("ownership_end", DBSafeUtils.DateToSQL(start_date.AddDays(-1)));
                Ownership os = new Ownership(constr);
                os.SetOdbc(mydb);
                os.Add(items);
                #endregion
                #region Change OS
                items = new Hashtable();
                items.Add("unit_master_debtor_id", debtor_id);
                items.Add("unit_master_begin_date", DBSafeUtils.DateToSQL(start_date));
                unit.Update(items);
                #endregion
                mydb.Commit();
                Response.Write("<script type='text/javascript'> window.close(); </script>");
            }
            catch (Exception ex)
            {
                if (mydb != null) mydb.Rollback();
                HttpContext.Current.Session["ErrorUrl"] = HttpContext.Current.Request.Url.ToString(); HttpContext.Current.Session["Error"] = ex; HttpContext.Current.Response.Redirect("~/error.aspx", false);

            }
            finally
            {
                if (mydb != null) mydb.Close();
            }
        }
        #endregion

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("proprietornew.aspx?mode=add&unitmasterid=" + Request.QueryString["unitmasterid"].ToString(), false);
        }

        protected void ComboBoxDebtor_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update 28/04/2016
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string debtor_master_id = "";
            debtor_master_id = ComboBoxDebtor.SelectedValue;

            if ("null".Equals(debtor_master_id))
            {
                Panel1.Visible = false;
            }
            else
            {
                ShowProprietorDetail(debtor_master_id);
            }
        }

        private void ShowProprietorDetail(string debtor_master_id)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            DebtorMaster debtormaster = new DebtorMaster(constr);
            debtormaster.LoadData(Convert.ToInt32(debtor_master_id));

            LabelCode.Text = "";
            Odbc o = new Odbc(AdFunction.conn);
            DataTable codeDT = o.ReturnTable("select * from unit_master where unit_master_debtor_id =" + debtormaster.DebtorMasterId, "tt");
            foreach (DataRow codeDR in codeDT.Rows)
            {
                LabelCode.Text = LabelCode.Text + codeDR["unit_master_debtor_code"].ToString() + ",";
            }
            if (LabelCode.Text.Contains(","))
                LabelCode.Text = LabelCode.Text.Substring(0, LabelCode.Text.Length - 1);
            if (LabelCode.Text.Length > 20)
            {
                LabelCode.ToolTip = LabelCode.Text;
                LabelCode.Text = LabelCode.Text.Substring(0, 20) + "...";
            }

            LabelName.Text = debtormaster.DebtorMasterName;
            LabelNotes.Text = debtormaster.DebtorMasterNotes;
            LabelSalutation.Text = debtormaster.DebtorMasterSalutation;


            DebtorType debtype = new DebtorType(constr);
            debtype.LoadData(debtormaster.DebtorMasterTypeId);
            LabelType.Text = debtype.DebtorTypeName;
            if (debtormaster.DebtorMasterPaymentTermId.HasValue)
            {
                PaymentTerm payterm = new PaymentTerm(constr);
                payterm.LoadData(debtormaster.DebtorMasterPaymentTermId.Value);
                LabelPaymentTerm.Text = payterm.PaymentTermName;
            }
            if (debtormaster.DebtorMasterPaymentTypeId.HasValue)
            {
                PaymentType paytype = new PaymentType(constr);
                paytype.LoadData(debtormaster.DebtorMasterPaymentTypeId.Value);
                LabelPaymentType.Text = paytype.PaymentTypeName;
            }
            if (debtormaster.DebtorMasterPrint.HasValue)
            {
                LabelPrint.Text = debtormaster.DebtorMasterPrint.Value ? "Yes" : "No";
            }
            if (debtormaster.DebtorMasterEmail.HasValue)
            {
                LabelEmail.Text = debtormaster.DebtorMasterEmail.Value ? "Yes" : "No";
            }

            Panel1.Visible = true;
        }
    }
}