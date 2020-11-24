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

namespace OSCALGenerator.Pages.SystemImplementation
{
    public partial class Users : BasePage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            UserName= Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = int.Parse(Cache["SystemId"].ToString());

            AddUserPanel.Visible = false;
            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);
            var users = GetUsers();
           
            FormView1.DataSource = users;
            FormView1.DataBind();

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

        protected void SaveButton_Click(object sender, EventArgs e)
        {
           
           
            var userid = this.UserIDTextBox.Text;
            var taguserid = string.Format("<system-implementation><user  id=\"{0}\">", userid);

            if (userid.Length == 0)
                return;

            InsertElementToDataBase(DOID, SystemID, userid, userid.GetType(), taguserid, "user id, user id", userid, 1);


            var roleid = RoleIDTextBox.Text;
            var tagroleid = string.Format("<system-implementation><user  id=\"{0}\"><role-id>", userid);
            InsertElementToDataBase(DOID, SystemID, "role-id", roleid.GetType(), tagroleid, "user id, role-id", roleid, 1);

            var title = TitleTextBox.Text;
            var tagTitle = string.Format("<system-implementation><user  id=\"{0}\"><title>", userid);
            InsertElementToDataBase(DOID, SystemID, "title", userid.GetType(), tagTitle, "user id, title", title, 1);

            var ns = NSTextBox.Text;
            var external = ExternalTextBox.Text;
            var tagNSExternal = string.Format("<system-implementation><user  id=\"{0}\"><prop  name=\"external\" ns=\"{1}\">", userid, ns);
            InsertElementToDataBase(DOID, SystemID, "external", external.GetType(), tagNSExternal, "user id, prop name", external, 1);
            InsertElementToDataBase(DOID, SystemID, ns, ns.GetType(), tagNSExternal, "user id, prop ns", ns, 1);


            var access = AccessTextBox.Text;
            var tagNSAccess = string.Format("<system-implementation><user  id=\"{0}\"><prop  name=\"access\" ns=\"{1}\">", userid, ns);
            InsertElementToDataBase(DOID, SystemID, "access", access.GetType(), tagNSAccess, "user id, prop name", access, 1);
            InsertElementToDataBase(DOID, SystemID, ns, ns.GetType(), tagNSAccess, "user id, prop ns", ns, 1);

            var sensitivity = SensitivityTextBox.Text;
            var tagNSSensitivity = string.Format("<system-implementation><user  id=\"{0}\"><prop  name=\"sensitivity-level\" ns=\"{1}\">", userid, ns);
            InsertElementToDataBase(DOID, SystemID, "sensitivity-level", sensitivity.GetType(), tagNSSensitivity, "user id, prop name", sensitivity, 1);
            InsertElementToDataBase(DOID, SystemID, ns, ns.GetType(), tagNSSensitivity, "user id, prop ns", ns, 1);

            var privilege = PrivilegeTextBox.Text;
            var tagPrivilege = string.Format("<system-implementation><user  id=\"{0}\"><authorized-privilege  name=\"{1}\">", userid, privilege);
            InsertElementToDataBase(DOID, SystemID, privilege, privilege.GetType(), tagPrivilege, "user id, authorized-privilege name", privilege, 1);

            var function = FunctionPerformedTextArea.InnerText;
            var tagFunction = string.Format("<system-implementation><user  id=\"{0}\"><authorized-privilege  name=\"{1}\"><function-performed>", userid, privilege);
            InsertElementToDataBase(DOID, SystemID, "function-performed", function.GetType(), tagFunction, "user id, function-performed", function, 1);



            



            FormView1.DataBind();
            FormviewPanel.Visible = true;
        }

        protected void AddUserButton_Click(object sender, EventArgs e)
        {
            AddUserPanel.Visible = true;
            FormviewPanel.Visible = false;
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            AddUserPanel.Visible = false;
            FormviewPanel.Visible = true;
        }

        protected void FormView1_PageIndexChanging(object sender, FormViewPageEventArgs e)
        {
            FormView1.PageIndex = e.NewPageIndex;
            FormView1.DataBind();
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/SystemCharacteristics/AuthorizationBoundary.aspx");
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/ControlImplementation/Controls.aspx");
        }
    }
}