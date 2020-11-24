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
    public partial class Roles : BasePage
    {
        public List<string> RoleIds;

        protected new void Page_Load(object sender, EventArgs e)
        {
            UserName = Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = int.Parse(Cache["SystemId"].ToString());

            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);
            SavedRoles = GetRoles();
            AddRolePanel.Visible = false;
            var bindingRoles = new BindingList<Role>(SavedRoles);
            this.RolesGridView.DataSource = bindingRoles;
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
        }

      

        protected void AddRoleButton_Click(object sender, EventArgs e)
        {
            AddRolePanel.Visible = true;
            GridviewPanel.Visible = false;

            RoleDropDownList.Visible = false;
            RoleDropDownList.Width = 0;
            RoleIDTextBox.Width = 295;
            RoleIDTextBox.Visible = true;

            DeleteButton.Visible = false;

        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
       
            string roleid;
            if (RoleIDTextBox.Text.Length == 0)
                roleid = RoleDropDownList.SelectedValue;
            else
                roleid = RoleIDTextBox.Text;

            var tagroleid = string.Format("<metadata><role id=\"{0}\">", roleid);
            
            var roleTitle = this.TitleTextBox.Text;
            var tagTitle = string.Format("<metadata><role id=\"{0}\"><title>", roleid);


            var desc = this.TextArea1.InnerText;
            var tagDesc = string.Format("<metadata><role id=\"{0}\"><desc>", roleid);

            InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "role id, role id", roleTitle + " " + desc  , 1);
            InsertElementToDataBase(DOID, SystemID, "title", roleid.GetType(), tagTitle, "role id, title", roleTitle, 1);

            InsertElementToDataBase(DOID, SystemID, "desc", roleid.GetType(), tagDesc, "role id, desc", desc, 1);

            
            //SavedRoles = GetRoles();
            RolesGridView.DataBind();
            AddRolePanel.Visible = false;
            GridviewPanel.Visible = true;
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            AddRolePanel.Visible = false;
            GridviewPanel.Visible = true;
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/Metadata/Title.aspx",false);
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/Metadata/Parties.aspx", false);
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            AddRolePanel.Visible = true;
            RoleIDTextBox.Visible = false;
            RoleIDTextBox.Width = 0;
            RoleDropDownList.Width = 295;
            RoleDropDownList.Visible = true;
            DeleteButton.Visible = true;

            var RoleIds = GetRoleIds();
            RoleDropDownList.DataSource = RoleIds;
            RoleDropDownList.DataBind();

            PopulateEditPage();
            GridviewPanel.Visible = false;
        }

        private void PopulateEditPage()
        {
            var roleId = RoleDropDownList.SelectedValue;
            var role = new Role();
            foreach (var elt in SavedRoles)
            {
                if (elt.RoleID == roleId)
                {
                    role = elt;
                    break;
                }

            }
            TitleTextBox.Text = role.RoleTitle;
            TextArea1.InnerText = role.Description;
        }

        protected void RoleDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateEditPage();

            AddRolePanel.Visible = true;
            RoleIDTextBox.Visible = false;
            RoleIDTextBox.Width = 0;
            RoleDropDownList.Width = 295;
            RoleDropDownList.Visible = true;
            DeleteButton.Visible = true;
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            var roleId = RoleDropDownList.SelectedValue;


            var tagroleid = string.Format("<metadata><role id=\"{0}\">", roleId);

            if (roleId.Length == 0)
                return;

            InsertElementToDataBase(DOID, SystemID, roleId, roleId.GetType(), tagroleid, "role id, role id", "", 0);

            RolesGridView.DataBind();
            GridviewPanel.Visible = true;
        }
    }
}