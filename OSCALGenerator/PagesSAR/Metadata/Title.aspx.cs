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

namespace OSCALGenerator.PagesSAR.Metadata
{
    public partial class Title : BasePage
    {

        public string DocumentTitle { get; set; }
   
        public string Version { get; set; }
        public string OSCALVersion { get; set; }
        public DateTime VersionDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public string Remarks; 
        protected new void   Page_Load(object sender, EventArgs e)
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
                StatusLabel.BackColor = Color.Red;
                StatusLabel.Text = ex.Message;
            }
        }

        void Restore()
        {
            DocumentTitle = GetData("title", "<metadata><title>", UserName, DOID);
            TitleTextArea.InnerText = DocumentTitle;

            Version = GetData("version", "<metadata><version>", UserName, DOID);
            VersionTextBox.Text = Version;

            OSCALVersion = GetData("oscal-version", "<metadata><oscal-version>", UserName, DOID);
            OSCALVersionTextBox.Text = OSCALVersion;

            var PublishedDateString = GetData("published", "<metadata><published>", VersionDateTextBox.Text.GetType(), UserName, DOID);
            VersionDateTextBox.Text = PublishedDateString;

            var LastModifiedDateString = GetData("last-modified", "<metadata><last-modified>", LastModifiedTextBox.Text.GetType(), UserName, DOID);
            LastModifiedTextBox.Text = LastModifiedDateString;

            Remarks = GetData("remarks", "<metadata><remarks>", UserName, DOID);
            RemarksTextarea.InnerHtml = Remarks;
        }

        protected void VersionDateButton_Click(object sender, EventArgs e)
        {
            VersionDateCalendar.Visible = true;
        }


        protected void LastModifiedCalendar_SelectionChanged(object sender, EventArgs e)
        {
            LastModifiedTextBox.Text = LastModifiedCalendar.SelectedDate.ToString("yyyy-MM-ddThh:mm:ssZ");
            LastModifiedCalendar.Visible = false;
        }

        protected void LastModifiedButton_Click(object sender, EventArgs e)
        {
          LastModifiedCalendar.Visible = true;
        }
        protected void VersionDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
      

            VersionDateTextBox.Text = VersionDateCalendar.SelectedDate.ToString("yyyy-MM-ddThh:mm:ssZ");
            VersionDateCalendar.Visible = false;
        }


        protected void SaveButton_Click(object sender, EventArgs e)
        {
            DocumentTitle = this.TitleTextArea.InnerText;
            InsertElementToDataBase(DOID, SystemID, "title", DocumentTitle.GetType(), "<metadata><title>", "Title of the SSP", DocumentTitle, 1);         


            Version = VersionTextBox.Text;
            InsertElementToDataBase(DOID, SystemID, "version", Version.GetType(), "<metadata><version>", "Version", Version, 1);


            OSCALVersion = OSCALVersionTextBox.Text;
            InsertElementToDataBase(DOID, SystemID, "oscal-version", OSCALVersion.GetType(), "<metadata><oscal-version>", "OSCAL Version", OSCALVersion, 1);

            VersionDate = new DateTime(); // VersionDateTextBox
            VersionDate = VersionDateCalendar.SelectedDate;
            InsertElementToDataBase(DOID, SystemID, "published", VersionDateTextBox.Text.GetType(), "<metadata><published>", "version release date", VersionDateTextBox.Text, 1);

            LastModifiedDate = new DateTime(); // VersionDateTextBox
            LastModifiedDate = LastModifiedCalendar.SelectedDate;
            InsertElementToDataBase(DOID, SystemID, "last-modified", LastModifiedTextBox.Text.GetType(), "<metadata><last-modified>", "version last-modified date", LastModifiedTextBox.Text, 1);

            Remarks = RemarksTextarea.InnerHtml;
            InsertElementToDataBase(DOID, SystemID, "remarks", Remarks.GetType(), "<metadata><remarks>", "remarks", Remarks, 1);
            
            Response.Redirect(@"~/PagesSAR/Metadata/RevisionHistory.aspx", false);
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAR/HomeSAR.aspx", false);
        }
    }
}