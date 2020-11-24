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

namespace OSCALGenerator.Pages.Metadata
{
    public partial class ResponsibleParties : BasePage
    {
        protected List<ResponsibleParty> RespParties;
        protected new void Page_Load(object sender, EventArgs e)
        {
            try
            {
                UserName = Cache["username"].ToString();
                DOID = (int)Cache["doid"];
                SystemID = int.Parse(Cache["SystemId"].ToString());
                ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
                DAL = new CSharpDAL(ConnString);
                RespParties = GetResponsibleParties();
                Panel1.Visible = false;
                var bindingParties = new BindingList<ResponsibleParty>(RespParties);
                this.RolesGridView.DataSource = bindingParties;
                RolesGridView.DataBind();

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
                StatusLabel.Visible = true;
                StatusLabel.ForeColor = Color.Red;
                StatusLabel.Text ="Please Select a System. Your System ID is null: "+  ex.Message;
            }
        }

        

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            var roleid = this.RoleIDTextBox.Text;
            var tagroleid = string.Format("<metadata><responsible-party role-id=\"{0}\">", roleid);

         

            var partyIds = this.TextArea1.InnerText;
            var tagpartyId = string.Format("<metadata><responsible-party role-id=\"{0}\"><party-id>", roleid);

            InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "responsible-party role-id, responsible-party role-id", roleid , 1);
            InsertElementToDataBase(DOID, SystemID, partyIds, partyIds.GetType(), tagpartyId, "responsible-party role-id, party-id", partyIds, 1);

       
            Panel1.Visible = false;
            GidviewPanel.Visible = true;
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            GidviewPanel.Visible = true;
        }

        protected void AddRoleButton_Click(object sender, EventArgs e)
        {
            Panel1.Visible = true;
            GidviewPanel.Visible = false;
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/Metadata/Parties.aspx");

        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/SystemCharacteristics/SystemIdentification.aspx");

        }
    }
}