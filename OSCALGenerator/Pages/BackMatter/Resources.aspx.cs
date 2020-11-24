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
using System.IO;

namespace OSCALGenerator.Pages.BackMatter
{
    public partial class Resources : BasePage
    {
        List<Resource> Resource;
        protected new void Page_Load(object sender, EventArgs e)
        {
            try
            {
                UserName = Cache["username"].ToString();
                DOID = (int)Cache["doid"];
                SystemID = int.Parse(Cache["SystemId"].ToString());
                ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
                DAL = new CSharpDAL(ConnString);

                ResourcePanel.Visible = false;
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

                Resource = GetResourceFiles();

                var bindingResources = new BindingList<Resource>(Resource);
                this.ResourceGridView.DataSource = bindingResources;
                ResourceGridView.DataBind();
            }catch(Exception ex)
            {
                StatusLabel.Text = ex.Message;
                StatusLabel.BackColor = Color.Red;
            }
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/Home.aspx");
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/ControlImplementation/Controls.aspx");
        }

        protected void SaveResourceButton_Click(object sender, EventArgs e)
        {
            var desc = this.DescTextarea.InnerText;
            var filename = "";
            if (FileUpload1.HasFile)
            {
                filename = FileUpload1.FileName;
            }
            var resourceId = ResourceIDTextBox.Text;
            var descDesc = "resource id, desc";
            var eltName = "desc";
            var tag = string.Format("<back-matter><resource  id=\"{0}\"><desc>", resourceId);
            var tagD = string.Format("<back-matter><resource  id=\"{0}\"><base64  filename=\"{1}\">", resourceId, filename);
            var tagR = string.Format("<back-matter><resource  id=\"{0}\">", resourceId);

            InsertElementToDataBase(DOID, SystemID, eltName, desc.GetType(), tag, descDesc, desc, 1);
            InsertElementToDataBase(DOID, SystemID, filename, filename.GetType(), tagD, "resource id, base64 filename", "Filestream", 1);
            InsertElementToDataBase(DOID, SystemID, resourceId, resourceId.GetType(), tagR, "resource id, resource id", filename, 1);

            ResourcePanel.Visible = false;
            GridviewPanel.Visible = true;
        }

        protected void CancelResourceButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/BackMatter/Resources.aspx");
        }

        protected void AddResourceButton_Click(object sender, EventArgs e)
        {
            ResourcePanel.Visible = true;
            GridviewPanel.Visible = false;
        }
    }
}