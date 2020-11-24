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
using OSCALHelperClasses;
using OSCALGenerator.Pages;

namespace OSCALGenerator.Pages.SystemCharacteristics
{
    public partial class SystemIdentification : BasePage
    {
        public string SysName { get; set; }
        public string SystemIdentifier { get; set; }
        public string SystemNameShort { get; set; }
        public string SecuritySensitivityLevel { get; set; }
        protected new void Page_Load(object sender, EventArgs e)
        {
            try
            {


                ErrorCache();
                UserName = Cache["username"].ToString();

                SystemID = int.Parse(Cache["SystemId"].ToString());

                ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
                DAL = new CSharpDAL(ConnString);

                Panel1.Visible = true;
                MainDiv.Visible = true;
                AddPropPanel.Visible = false;
                AddAuxPanel.Visible = false;
                if (!IsPostBack)
                {
                    Restore();
                }

                if (Cache["orgName"] != null)
                {
                    OrgName = Cache["orgName"].ToString();
                    CorpNameLabel.Text = OrgName;
                }
                if (Cache["systemName"] != null)
                {
                    SystemName = Cache["systemName"].ToString();
                    SytemInfoNameLabel.Text = SystemName;
                }

                if (Cache["doid"] != null)
                {
                    DOID = (int)Cache["doid"];
                    var doid = int.Parse(Cache["doid"].ToString());
                    DocName = GetDocFullName(doid);
                    DocNameLabel.Text = DocName;

                }
            }
            catch (Exception ex)
            {
                Panel1.Visible = true;
                MainDiv.Visible = true;
                AddPropPanel.Visible = false;
                AddAuxPanel.Visible = false;
                StatusLabel.BackColor = Color.Red;
                Panel1.Height = 400;
                StatusLabel.Text = ex.Message;               

            }
        }

      

        protected void AddAuxButton_Click(object sender, EventArgs e)
        {
            AddAuxPanel.Visible = true;
            Panel1.Visible = true;
            MainDiv.Visible = true;

            var number = SecondTextBox.Text;

            var tagType = string.Format("<system-characteristics><system id=\"{0}\"><type>", number);
            var type = TypeDropDownList.SelectedValue;
            var tagSysID = string.Format("<system-characteristics><system id=\"{0}\">", number);

            Errors += InsertElementToDataBase(DOID, SystemID, "type", type.GetType(), tagType, "system id, type", type, 1);
            Errors += InsertElementToDataBase(DOID, SystemID, "id", number.GetType(), tagSysID, "system id, system id", number, 1);

            var sysIds = GetSystemIDs();
            SystemIDDropDownList.DataSource = sysIds;
            SystemIDDropDownList.DataTextField = "Identification";
            SystemIDDropDownList.DataValueField = "Identification";
            SystemIDDropDownList.DataBind();
        }

        void SetPropertyPanel()
        {
            Panel1.Visible = true;
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

            var roleid = "";
            
            var props = GetProps(roleid, "system-id");
            var names = props.Select(x => x.Name).ToList();
            var fullnames = new List<string> { "" };

            var ids = props.Select(x => x.ID).ToList();
            var fullIDs = new List<string> { "" };
            fullIDs.AddRange(ids);

            fullnames.AddRange(names);
            PropDropDownList.DataSource = fullIDs;
            PropDropDownList.DataBind();

            PopulatePropertEditPage();

            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();
        }

        protected void PropButton_Click(object sender, EventArgs e)
        {
            SetPropertyPanel();
        }
        protected void PropertyDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        protected void AnnotationButton_Click(object sender, EventArgs e)
        {
            SetAnnotationPanel();
        }

        protected void AnnotationDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void SetAnnotationPanel()
        {
            Panel1.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = true;
          

            NameLabel.Text = "Name";
            ClassLabel.Visible = false;
            ClassTextBox.Visible = false;

            NSLabel.Text = "NS";
            ValueLabel.Text = "Value";
            RemarksLabel.Text = "Remarks";
            RemarksTextArea.Visible = true;
            PropLabel.Text = "Annotation";
            PropLabel.Enabled = false;

            var roleid = "";
            

            var annotations = GetAnnotations(roleid, "system-id");
            var names = annotations.Select(x => x.Name).ToList();
            var fullnames = new List<string> { "" };

            var ids = annotations.Select(x => x.ID).ToList();
            var fullIDs = new List<string> { "" };
            fullIDs.AddRange(ids);

            fullnames.AddRange(names);
            PropDropDownList.DataSource = fullIDs;
            PropDropDownList.DataBind();

            PopulateAnnotationEditPage();

            AnnotationDropDownList.DataSource = names;
            AnnotationDropDownList.DataBind();
        }

        protected void LinkDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void SetLinkPanel()
        {
            Panel1.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = true;
          
            NameLabel.Text = "href";
            ClassLabel.Text = "rel";
            NSLabel.Text = "media-type";
            ValueLabel.Text = "Value";
            RemarksLabel.Text = "";
            RemarksTextArea.Visible = false;
            ClassLabel.Visible = true;
            ClassTextBox.Visible = true;
            PropLabel.Text = "Link";
            PropLabel.Enabled = false;

            var roleid = "";
            


            var links = GetLinks(roleid, "system-id");
            var names = links.Select(x => x.HRef).ToList();
            var ids = links.Select(x => x.ParentID).ToList();
            var fullnames = new List<string> { "" };



            fullnames.AddRange(ids);
            PropDropDownList.DataSource = fullnames;
            PropDropDownList.DataBind();

            PopulateLinkEditPage();

            LinkDropDownList.DataSource = names;
            LinkDropDownList.DataBind();
        }

        protected void LinkButton_Click(object sender, EventArgs e)
        {
            SetLinkPanel();
        }

        private void PopulatePropertEditPage()
        {

            var roleid ="";

            var props = GetProps(roleid, "system-id");
            var propId = PropDropDownList.SelectedValue;
            var prop = new Prop();
            foreach (var elt in props)
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
        private void PopulateAnnotationEditPage()
        {

            var roleid ="";

            var anns = GetAnnotations(roleid, "system-id");
            var propId = PropDropDownList.SelectedValue;
            var ann = new Annotation();
            foreach (var elt in anns)
            {
                if (elt.ID == propId)
                {
                    ann = elt;
                    break;
                }

            }
            NameTextBox.Text = ann.Name;
            NSTextBox.Text = ann.NS;
            // ClassTextBox.Text = prop.Class;
            ValueTextBox.Text = ann.Value;
            RemarksTextArea.InnerText = ann.Remarks;


        }

        private void PopulateLinkEditPage()
        {

            var roleid ="";

            var links = GetLinks(roleid, "system-id");
            var propId = PropDropDownList.SelectedValue;
            var link = new Link();
            foreach (var elt in links)
            {
                if (elt.ParentID == propId)
                {
                    link = elt;
                    break;
                }

            }
            NameTextBox.Text = link.HRef;
            NSTextBox.Text = link.MediaType;
            ClassTextBox.Text =  link.Rel;
            ValueTextBox.Text = link.MarkUpLine;
        }


        protected void PropDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (PropLabel.Text == "Property")
            {
                PopulatePropertEditPage();
            }
            if (PropLabel.Text == "Annotation")
            {
                PopulateAnnotationEditPage();
            }
            if (PropLabel.Text == "Link")
            {
                PopulateLinkEditPage();
            }

            Panel1.Visible = true;
            MainDiv.Visible = true;

            // DeleteButton.Visible = true;

            AddPropPanel.Visible = true;

        }
        public void ProcessEntity(string entityId, string entityName)
        {

            Errors = "";
            PropStatusLabel.Text = "";
            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(entityId, entityName);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<system-characteristics><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<system-characteristics><{2}=\"{0}\"><prop id=\"{1}\" name>", entityId, propid, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<system-characteristics><{2}=\"{0}\"><prop id=\"{1}\" ns>", entityId, propid, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<system-characteristics><{2}=\"{0}\"><prop id=\"{1}\" class>", entityId, propid, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<system-characteristics><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);

                Errors += InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, entityName + ", prop id", propid, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "name", propid.GetType(), tagname, entityName + ", prop name", name, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "ns", propid.GetType(), tagns, entityName + ", prop ns", ns, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "class", propid.GetType(), tagClass, entityName + ", prop class", classn, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "value", propid.GetType(), tagValue, entityName + ", prop", value, 1);

                var props = GetProps(entityId, entityName);
                var names = props.Select(x => x.Name).ToList();
                var fullnames = new List<string> { "" };
                fullnames.AddRange(names);
                PropDropDownList.DataSource = fullnames;
                PropDropDownList.DataBind();

            }

            if (PropLabel.Text == "Annotation")
            {
                var annotationId = GenerateAnnotationID(entityId, entityName);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    annotationId = PropDropDownList.SelectedValue;
                }

                var tagAnnotationid = string.Format("<system-characteristics><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<system-characteristics><{2}=\"{0}\"><annotation id=\"{1}\" name>", entityId, annotationId, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<system-characteristics><{2}=\"{0}\"><annotation id=\"{1}\" ns>", entityId, annotationId, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<system-characteristics><{2}=\"{0}\"><annotation id=\"{1}\" class>", entityId, annotationId, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<system-characteristics><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var remarks = RemarksTextArea.InnerText;
                var tagRemarks = string.Format("<system-characteristics><{2}=\"{0}\"><annotation id=\"{1}\"><remarks>", entityId, annotationId, entityName);


                Errors += InsertElementToDataBase(DOID, SystemID, annotationId, annotationId.GetType(), tagAnnotationid, entityName + ", annotation id", annotationId, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "name", annotationId.GetType(), tagname, entityName + ", annotation name", name, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "ns", annotationId.GetType(), tagns, entityName + ", annotation ns", ns, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "class", annotationId.GetType(), tagClass, entityName + ", annotation class", classn, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "value", annotationId.GetType(), tagValue, entityName + ", annotation", value, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "remarks", annotationId.GetType(), tagRemarks, entityName + ", annotation, remarks", remarks, 1);

            }

            if (PropLabel.Text == "Link")
            {
                var linkid = "";

                if (PropDropDownList.SelectedIndex > 0)
                {
                    linkid = PropDropDownList.SelectedValue;
                }
                else
                {
                    linkid = GenerateLinkID("", "system-characteristics");
                }
                var linkHref = NameTextBox.Text;
                var taghref = string.Format("<system-characteristics><{2}=\"{0}\"><link id=\"{1}\" href>", entityId, linkid, entityName);
                var tagid = string.Format("<system-characteristics><{2}=\"{0}\"><link id=\"{1}\">", entityId, linkid, entityName);

                var media = NSTextBox.Text;
                var tagrel = string.Format("<system-characteristics><{2}=\"{0}\"><link id=\"{1}\" rel>", entityId, linkid, entityName, entityName);

                var rel = ClassTextBox.Text;
                var tagmedia = string.Format("<system-characteristics><{2}=\"{0}\"><link href=\"{1}\" media-type>", entityId, linkHref, entityName);


                var value = ValueTextBox.Text;
                var tagValue = string.Format("<system-characteristics><{2}=\"{0}\"><link id=\"{1}\">", entityId, linkid, entityName);

                Errors += InsertElementToDataBase(DOID, SystemID, linkid, linkHref.GetType(), tagid, entityName + ", link href", linkid, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "link", linkHref.GetType(), taghref, entityName + ", link href", linkHref, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "rel", rel.GetType(), tagrel, entityName + ", link rel", rel, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "media-type", media.GetType(), tagmedia, entityName + ", link media-type", media, 1);


                Errors += InsertElementToDataBase(DOID, SystemID, "value", value.GetType(), tagValue, entityName + ", link", value, 1);

            }
            if (Errors.Length > 0)
            {
                PropStatusLabel.Text = Errors;
                PropStatusLabel.ForeColor = Color.Red;
                AddPropPanel.Visible = true;
            }
        }

        protected void AddEditButton_Click(object sender, EventArgs e)
        {

           

            ProcessEntity("", "system identification");
            MainDiv.Visible = true;
            Panel1.Visible = true;
           
        }

        void Restore()
        {
            Panel1.Height = 380;
            SysName = GetData("system-name", "<system-characteristics><system-name>", UserName, DOID);
            SystemNameTextBox.Text = SysName;

            SystemIdentifier = GetData("system-id", "<system-characteristics><system-id>", UserName, DOID);
          //  SystemIDTextBox.Text = SystemIdentifier;

            SystemNameShort = GetData("system-name-short", "<system-characteristics><system-name-short>", UserName, DOID);
            SystemNameShortTextBox.Text = SystemNameShort;

            DescTextarea.InnerHtml  = GetData("desc", "<system-characteristics><description>", UserName, DOID);

            SecuritySensitivityLevel = GetData("security-sensitivity-level", "<system-characteristics><security-sensitivity-level>", UserName, DOID);
            DropDownList1.SelectedValue = SecuritySensitivityLevel;

            DateTextBox.Text = GetData("date-authorized", "<system-characteristics><date-authorized>", UserName, DOID);

            var sysIds = GetSystemIDs();
            SystemIDDropDownList.DataSource = sysIds;
            SystemIDDropDownList.DataTextField = "Identification";
            SystemIDDropDownList.DataValueField = "Identification";
            SystemIDDropDownList.DataBind();
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            SysName = SystemNameTextBox.Text;
            Errors += InsertElementToDataBase(DOID, SystemID, "system-name", SysName.GetType(), "<system-characteristics><system-name>", "system-name", SysName, 1);

            var desc = DescTextarea.InnerHtml;
            Errors += InsertElementToDataBase(DOID, SystemID, "desc", SysName.GetType(), "<system-characteristics><description>", "system-id, description", desc, 1);

            SystemNameShort = SystemNameShortTextBox.Text;
            Errors += InsertElementToDataBase(DOID, SystemID, "system-name-short", SysName.GetType(), "<system-characteristics><system-name-short>", "system-name-short", SystemNameShort, 1);

            SecuritySensitivityLevel = DropDownList1.SelectedValue;
            Errors += InsertElementToDataBase(DOID, SystemID, "security-sensitivity-level", SysName.GetType(), "<system-characteristics><security-sensitivity-level>", "security-sensitivity-level", SecuritySensitivityLevel, 1);

            Errors += InsertElementToDataBase(DOID, SystemID, "date-authorized", SysName.GetType(), "<system-characteristics><date-authorized>", "date-authorized", DateTextBox.Text, 1);


            var guid = new Guid(Cache["orgId"].ToString());

            var userId = GetUserID(UserName);

            SystemIdentifier = SystemIDDropDownList.SelectedValue;

            AddSystem(guid, SysName, 1, SystemIdentifier, 1, userId);
            Cache["systemName"] = SysName;
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/Metadata/ResponsibleParties.aspx");
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/SystemCharacteristics/SystemInfoTypes.aspx");
        }

        protected void SysIDButton_Click(object sender, EventArgs e)
        {
            AddAuxPanel.Visible = true;
            MainDiv.Visible = true;
            Panel1.Visible = true;
            var sysIds = GetSystemIDs();
            AuxDropDownList.DataSource = sysIds;
            AuxDropDownList.DataTextField = "Identification";
            AuxDropDownList.DataValueField = "Identification";
            AuxDropDownList.DataBind();

        }

        protected void Calendar_SelectionChanged(object sender, EventArgs e)
        {

            DateTextBox.Text = Calendar.SelectedDate.ToString(); //"yyyy-MM-ddThh:mm:ssZ"
            Calendar.Visible = false;
            Panel1.Height = 380;
        }

        protected void DateButton_Click(object sender, EventArgs e)
        {
            Calendar.Visible = true;
            Panel1.Height = 500;
        }
    }
}