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

namespace OSCALGenerator.Pages.Metadata
{
    public partial class Title : BasePage
    {
       
        public string DocumentTitle { get; set; }
        public string CSPName { get; set; }
        public string InfoSysName { get; set; }
        public string Version { get; set; }
        public string OSCALVersion { get; set; }
        public DateTime VersionDate { get; set; }

    

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
                StatusLabel.BackColor = Color.Red;
                StatusLabel.Text = ex.Message;
            }
        }

        void Restore()
        {
            DocumentTitle = GetData("title", "<metadata><title>", UserName, DOID);
            TitleTextArea.InnerText = DocumentTitle;

          
            CSPName = GetData("org-name", "<metadata><party  id=\"csp\"><org><org-name>", UserName, DOID);
            CSPNameTextBox.Text = CSPName;

            InfoSysName = GetData("system-name", "<system-characteristics><system-name>", UserName, DOID);
            SysInfoNameTextBox.Text = InfoSysName;

            Version = GetData("version", "<metadata><version>", UserName, DOID); 
            VersionTextBox.Text = Version;

            OSCALVersion = GetData("oscal-version", "<metadata><oscal-version>", UserName, DOID);
            OSCALVersionTextBox.Text = OSCALVersion;

            string VersionDateString = GetData("last-modified", "<metadata><last-modified>", VersionDate.GetType(), UserName, DOID);
            VersionDateTextBox.Text = VersionDateString;
        }
        
        protected void VersionDateButton_Click(object sender, EventArgs e)
        {
            VersionDateCalendar.Visible = true;
        }

        protected void VersionDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            VersionDateTextBox.Text = VersionDateCalendar.SelectedDate.ToString("yyyy-MM-ddThh:mm:ssZ");
            VersionDateCalendar.Visible = false;
           
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            DocumentTitle = this.TitleTextArea.InnerText;
            InsertElementToDataBase(DOID, SystemID, "title", DocumentTitle.GetType(), "<metadata><title>", "title", DocumentTitle, 1);

            CSPName = CSPNameTextBox.Text;
            InsertElementToDataBase(DOID, SystemID, "org-name", CSPName.GetType(), "<metadata><party  id=\"csp\"><org><org-name>", "org-name", CSPName, 1);


            InfoSysName = SysInfoNameTextBox.Text;
            InsertElementToDataBase(DOID, SystemID, "system-name", InfoSysName.GetType(), "<system-characteristics><system-name>", "system-name", InfoSysName, 1);


            Version = VersionTextBox.Text;
            InsertElementToDataBase(DOID, SystemID, "version", Version.GetType(), "<metadata><version>", "version", Version, 1);


            OSCALVersion = OSCALVersionTextBox.Text;
            InsertElementToDataBase(DOID, SystemID, "oscal-version", OSCALVersion.GetType(), "<metadata><oscal-version>", "oscal-version", OSCALVersion, 1);

            VersionDate = new DateTime(); // VersionDateTextBox
            VersionDate = VersionDateCalendar.SelectedDate;
           
            var versionDateText = VersionDateTextBox.Text;
            InsertElementToDataBase(DOID, SystemID, "last-modified", versionDateText.GetType(), "<metadata><last-modified>", "last-modified", versionDateText, 1);

            Response.Redirect(@"~/Pages/Metadata/Roles.aspx", false);
        }

        void ImportToDB()
        {
            
           DataSet dataSet = new DataSet();
            
         // InsertElementToDataBase("MetaData:Title",ElementTypeId.String,"<metadata><title>","title","",1)


        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/Home.aspx", false);
        }
    }
}