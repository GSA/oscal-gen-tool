using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlDataProvider;
using System.Data;
using System.ComponentModel;
using System.Drawing;
using System.Configuration;
using System.Data.Sql;
using System.Data.SqlClient;
using OSCALHelperClasses;
using OSCALGenerator.Pages;


namespace OSCALGenerator.PageSSP.Import_Profile
{
    public partial class ImportProfile : BasePage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            try
            {
                UserName = Cache["username"].ToString();
                DOID = (int)Cache["doid"];
                SystemID = int.Parse(Cache["SystemId"].ToString());

                ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
                DAL = new CSharpDAL(ConnString);


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
                if (!IsPostBack)
                    Restore();
            }catch(Exception ex)
            {
                StatusLabel.Text = ex.Message;
                StatusLabel.BackColor = Color.Red;
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            var href = HrefTextBox.Text;
            var tag = string.Format("<import-profile href>", href);

            var remarks = RoleRemarksTextarea.InnerText;
            var tagRemarks = string.Format("<import-profile href><remarks>");

            InsertElementToDataBase(DOID, SystemID, "href", href.GetType(), tag, "import-profile href", href, 1);

            InsertElementToDataBase(DOID, SystemID, "remarks", remarks.GetType(), tagRemarks, "import-profile href, remarks", remarks, 1);
            
            Response.Redirect(@"~/Pages/SystemCharacteristics/SystemIdentification.aspx", false);
        }

        protected void Restore()
        {
            var href = GetData("href", "<import-profile href>", UserName, DOID);
            HrefTextBox.Text = href;

            var remarks = GetData("remarks", "<import-profile href><remarks>", UserName, DOID);
            RoleRemarksTextarea.InnerText = remarks;
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PageSSP/Metadata/ResponsibleParties.aspx", false);
        }
       
    }
}