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
using System.IO;
using OSCALHelperClasses;
using OSCALGenerator.Pages;
namespace OSCALGenerator.PagesSAR.Assets
{
    public partial class Origination : BasePage
    {
        List<Prop> Props;
        protected new void Page_Load(object sender, EventArgs e)
        {
            FaultyCacheBackHomeSAR();
            UserName = Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = int.Parse(Cache["SystemId"].ToString());

            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);


            Props= GetProps("", "origination");

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

           
            AddPropPanel.Visible = false;

            MainDiv.Visible = true;

            if (!IsPostBack)
            {


                PopulateEditPage();

            }
        }

        void PopulateEditPage()
        {
           
            var tagTitle = string.Format("<assets><origination><title>");

       
            var tagDesc = string.Format("<assets><origination><description>");

              TitleTextBox.Text = GetData("title", tagTitle, UserName, DOID);


               DescriptionTextArea.InnerText = GetData("desc", tagDesc, UserName, DOID);

        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {

            var title = TitleTextBox.Text;
            var tagTitle = string.Format("<assets><origination><title>");

            var desc = this.DescriptionTextArea.InnerText;
            var tagDesc = string.Format("<assets><origination><description>");

        
            InsertElementToDataBase(DOID, SystemID, "title", title.GetType(), tagTitle, "origination, title", title, 1);
            InsertElementToDataBase(DOID, SystemID, "desc", desc.GetType(), tagDesc, "origination, description", desc, 1);

            Response.Redirect(@"~/PagesSAR/AssessmentActivities/TestMethods.aspx", false);
        }

        protected void AddEditButton_Click(object sender, EventArgs e)
        {

            ProcessEntity("", "origination");
            MainDiv.Visible = true;
            AddRolePanel.Visible = true;

        }

        public void ProcessEntity(string entityId, string entityName)
        {

            var rawTag = string.Format("<assets><origination>");

            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(entityId, entityName, rawTag);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<assets><origination><prop id=\"{0}\">", propid);
                var name = NameTextBox.Text;
                var tagname = string.Format("<assets><origination><prop id=\"{0}\" name>",propid);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<assets><origination><prop id=\"{0}\" ns>",propid);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<assets><origination><prop id=\"{0}\" class>",propid);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<<assets><origination><prop id=\"{0}\">",propid);

                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, entityName + ", prop id", propid, 1);
                InsertElementToDataBase(DOID, SystemID, "name", propid.GetType(), tagname, entityName + ", prop name", name, 1);

                InsertElementToDataBase(DOID, SystemID, "ns", propid.GetType(), tagns, entityName + ", prop ns", ns, 1);

                InsertElementToDataBase(DOID, SystemID, "class", propid.GetType(), tagClass, entityName + ", prop class", classn, 1);
                InsertElementToDataBase(DOID, SystemID, "value", propid.GetType(), tagValue, entityName + ", prop", value, 1);

                var props = GetProps(entityId, entityName);
                var names = props.Select(x => x.Name).ToList();
                var fullnames = new List<string> { "" };
                fullnames.AddRange(names);
                PropDropDownList.DataSource = fullnames;
                PropDropDownList.DataBind();

            }

           
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAR/Assets/Tools.aspx", false);
        }

        protected void PropDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PropLabel.Text == "Property")
            {
                PopulatePropertEditPage();
            }
           
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;

            AddPropPanel.Visible = true;
        }

        private void PopulatePropertEditPage()
        {

            var propId = PropDropDownList.SelectedValue;
            var prop = new Prop();
            foreach (var elt in Props)
            {
                if (elt.ID == propId)
                {
                    prop = elt;
                    break;
                }

            }
            NameTextBox.Text = prop.Name;
            NSTextBox.Text = prop.NS;
            ClassTextBox.Text = prop.Class;
            ValueTextBox.Text = prop.Value;
        }

        protected void PropButton_Click(object sender, EventArgs e)
        {
            SetPropertyPanel();
        }

        void SetPropertyPanel()
        {
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = true;
            NameLabel.Text = "Name";
            ClassLabel.Text = "Class";
            NSLabel.Text = "NS";
            ValueLabel.Text = "Value";
            PropLabel.Text = "Property";
            PropLabel.Enabled = false;
            RemarksLabel.Text = "";
            RemarksTextArea.Visible = false;

            var subjectid = "";


            var rawTag = string.Format("<assessment-subject><local-definitions><{1}=\"{0}\">", subjectid, "component id");

            Props = GetProps(subjectid, "component id", rawTag);
            var names = Props.Select(x => x.Name).ToList();
            var fullnames = new List<string> { "" };

            var ids = Props.Select(x => x.ID).ToList();
            var fullIDs = new List<string> { "" };
            fullIDs.AddRange(ids);

            fullnames.AddRange(names);
            PropDropDownList.DataSource = fullIDs;
            PropDropDownList.DataBind();

            PopulatePropertEditPage();

            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();
        }
    }
}