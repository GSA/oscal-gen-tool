using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlDataProvider;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Data.Sql;
using System.Data.SqlClient;
using OSCALGenerator.Pages;

namespace OSCALGenerator.PageSSP.SystemImplementation
{
    public partial class Remarks : BasePage
    {
        string Remark;
        protected new void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ErrorCache();
                UserName = Cache["username"].ToString();
                DOID = (int)Cache["doid"];
                SystemID = int.Parse(Cache["SystemId"].ToString());


                ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
                DAL = new CSharpDAL(ConnString);
                if (!IsPostBack)
                {
                    Restore();
                }

                if (Cache["orgName"] != null)
                {
                    OrgName = Cache["orgName"].ToString();
                    OrgNameLabel.Text = OrgName;

                }
                if (Cache["systemName"] != null)
                {
                    SystemName = Cache["systemName"].ToString();
                    SysNameLabel.Text = SystemName;
                }

                if (Cache["doid"] != null)
                {
                    var doid = int.Parse(Cache["doid"].ToString());
                    DocName = GetDocFullName(doid);
                    DocNameLabel.Text = DocName;

                }
            }catch(Exception ex)
            {
                StatusLabel.Text = ex.Message;
                StatusLabel.BackColor = Color.Red;
            }
        }

        void Restore()
        {
            Remark = GetData("remarks", "<system-implementation><remarks>", UserName, DOID);
            RemarksTextarea.InnerText = Remark;         
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Remark = RemarksTextarea.InnerText;
            InsertElementToDataBase(DOID, SystemID, "remarks", Remark.GetType(), "<system-implementation><remarks>", "local-definitions, remarks", Remark, 1);

            Response.Redirect(@"~/Pages/ControlImplementation/Controls.aspx", false);
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PageSSP/SystemImplementation/InventoryItems.aspx", false);
        }
    }
}