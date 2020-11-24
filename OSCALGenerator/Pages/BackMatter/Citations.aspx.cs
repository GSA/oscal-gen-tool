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

namespace OSCALGenerator.Pages.BackMatter
{
    public partial class Citations : BasePage
    {
      
        List<Citation> Citation;
        protected new void Page_Load(object sender, EventArgs e)
        {
            try
            {
                UserName = Cache["userId"].ToString();
                DOID = (int)Cache["doid"];
                SystemID = int.Parse(Cache["SystemId"].ToString());
                ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
                DAL = new CSharpDAL(ConnString);

                Citation = GetCitations();



                var bindingCitations = new BindingList<Citation>(Citation);
                this.CitationsGridView.DataSource = bindingCitations;
                CitationsGridView.DataBind();


                CitationPanel.Visible = false;
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

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/BackMatter/Resources.aspx");
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/ControlImplementation/Controls.aspx");
        }

        protected void SaveCitButton_Click(object sender, EventArgs e)
        {
            var citation = CitationIDTextBox.Text;
            var target = TargetTextArea.InnerText;
            var title = TitleTextBox.Text;
           
            var tag = string.Format("<back-matter><citation  id=\"{0}\">", citation);
            var tagD = string.Format("<back-matter><citation  id=\"{0}\"><target>", citation);
            var tagR = string.Format("<back-matter><citation  id=\"{0}\"><title>", citation);

            InsertElementToDataBase(DOID, SystemID, citation, target.GetType(), tag, "citation id, citation id", target + title, 1);
            InsertElementToDataBase(DOID, SystemID, "target", target.GetType(), tagD, "citation id, target", target, 1);
            InsertElementToDataBase(DOID, SystemID, "title", title.GetType(), tagR, "citation id, title", title, 1);

            CitationPanel.Visible = false;
            GidviewPanel.Visible = true;
        }

        protected void CancelCitButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/BackMatter/Citations.aspx");
        }

        protected void AddCitationButton_Click(object sender, EventArgs e)
        {
            CitationPanel.Visible = true;
            GidviewPanel.Visible = false;
        }
    }
}