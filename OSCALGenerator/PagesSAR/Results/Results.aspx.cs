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

namespace OSCALGenerator.PagesSAR.Results
{
    public partial class Results : BasePage
    {
        List<Prop> Props;
        List<Annotation> Annotations;
        List<Link> Links;
        public List<Role> Roles;
        public List<string> RoleIds;
        public List<DocParty> Parties;
        public List<string> PartyUUIDs;
        
        List<Result> Subjects;


        int Height = 425;
        int AddOn = 125;

        protected new void Page_Load(object sender, EventArgs e)
        {
            FaultyCacheBackHomeSAR();
            UserName = Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = int.Parse(Cache["SystemId"].ToString());

            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);

            Subjects = GetResults();

            AddOn = (EndDateCalendar.Visible && StartDateCalendar.Visible) ? 285 : 125;

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

            if (Cache["Finding"] == null || Cache["Finding"].ToString() != "true")
            {
                AddResultPanel.Visible = false;
                MainDiv.Visible = false;
                AddPropPanel.Visible = false;
                MainDiv.Visible = false;
                var bindingRoles = new BindingList<Result>(Subjects);
                this.RolesGridView.DataSource = bindingRoles;
                RolesGridView.DataBind();
            }
            else
            {
                if (Cache["ResultsID"] != null)
                {
                    var names = Subjects.Select(x => x.ID).ToList();
                    var fullnames = new List<string> { "" };
                    fullnames.AddRange(names);
                    RoleDropDownList.DataSource = fullnames;
                    RoleDropDownList.DataBind();

                    var magic = Cache["ResultsID"].ToString();
                    RoleDropDownList.SelectedValue = magic;
                    
                    Cache.Remove("ResultsID");
                }
                AddResultPanel.Visible = true;
                AddPropPanel.Visible = false;
                MainDiv.Visible = true;
                PopulateEditPage();
                GridviewPanel.Visible = false;
            }
           

            if (!IsPostBack)
            {

                var names = Subjects.Select(x => x.ID).ToList();
                var fullnames = new List<string> { "" };
                fullnames.AddRange(names);
                RoleDropDownList.DataSource = fullnames;
                RoleDropDownList.DataBind();
                // PopulateEditPage();
                StartDateCalendar.Visible = false;
                EndDateCalendar.Visible = false;
                AddResultPanel.Height = Height;


            }
        }

        protected void LinkButton_Click(object sender, EventArgs e)
        {
            SetLinkPanel();
        }

        void SetLinkPanel()
        {
            AddResultPanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = true;
            NameLabel.Text = "href";
            ClassLabel.Text = "rel";
            NSLabel.Text = "media-type";
            ValueLabel.Text = "Value";
            RemarksLabel.Text = "";
            RemarksTextArea.Visible = false;
            PropLabel.Text = "Link";
            PropLabel.Enabled = false;

            var subjectid = "";
            if (RoleDropDownList.SelectedIndex > 0)
                subjectid = RoleDropDownList.SelectedValue;
            else
                subjectid = GenerateSubjectID();

            var rawTag = string.Format("<assessment-activities><{1}=\"{0}\">", subjectid, "test-method id");

            Links = GetLinks(subjectid, "test-method id", rawTag);
            var names = Links.Select(x => x.HRef).ToList();
            var fullnames = new List<string> { "" };

            fullnames.AddRange(names);
            PropDropDownList.DataSource = fullnames;
            PropDropDownList.DataBind();

            PopulateLinkEditPage();


        }

      

        private void PopulateLinkEditPage()
        {
            // var roleid = RoleDropDownList.SelectedValue;
            // var links = GetLinks(roleid, "role id");
            var propId = PropDropDownList.SelectedValue;
            var link = new Link();
            foreach (var elt in Links)
            {
                if (elt.HRef == propId)
                {
                    link = elt;
                    break;
                }

            }
            NameTextBox.Text = link.HRef;
            NSTextBox.Text = link.Rel;
            ClassTextBox.Text = link.MediaType;
            ValueTextBox.Text = link.MarkUpLine;
        }


        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAR/AssessmentActivities/Schedules.aspx", false);
        }


        private void PopulateAnnotationEditPage()
        {

            var propId = PropDropDownList.SelectedValue;
            var ann = new Annotation();
            foreach (var elt in Annotations)
            {
                if (elt.ID == propId)
                {
                    ann = elt;
                    break;
                }

            }
            NameTextBox.Text = ann.Name;
            NSTextBox.Text = ann.NS;
            ClassTextBox.Text = ann.ID;
            ValueTextBox.Text = ann.Value;
            RemarksTextArea.InnerText = ann.Remarks;


        }
        protected void AddRoleButton_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = true;
            AddResultPanel.Visible = true;
            GridviewPanel.Visible = false;

            RoleDropDownList.Visible = true;
            RoleDropDownList.Width = 0;

            DeleteButton.Visible = false;

        }



        private string GenerateSubjectID()
        {
            var nbrRole = Subjects.Count();
            var id = string.Format("result-{0}", nbrRole);
            return id;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            var resultid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                resultid = GenerateSubjectID();
            }
            else
                resultid = RoleDropDownList.SelectedValue;


            var tagroleid = string.Format("<results id=\"{0}\">", resultid);
            var taguuid = string.Format("<results id=\"{0}\"><uuid>", resultid);

            var preguid = GetData("results uuid", taguuid, UserName, DOID);
            var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;

            var title = TitleTextBox.Text;
            var tagTitle = string.Format("<results id=\"{0}\"><title>", resultid);

            var desc = this.DescriptionTextArea.InnerText;
            var tagDesc = string.Format("<results id=\"{0}\"><description>", resultid);

            var remarks = RoleRemarksTextarea.InnerText;
            var tagRemarks = string.Format("<results id=\"{0}\"><remarks>", resultid);

            InsertElementToDataBase(DOID, SystemID, "results id", resultid.GetType(), tagroleid, "results id, results id", resultid, 1);
            InsertElementToDataBase(DOID, SystemID, "results uuid", resultid.GetType(), taguuid, "results id, uuid", guid, 1);

            InsertElementToDataBase(DOID, SystemID, "title", title.GetType(), tagTitle, "results id, title", title, 1);

            InsertElementToDataBase(DOID, SystemID, "desc", desc.GetType(), tagDesc, "results id, description", desc, 1);

            InsertElementToDataBase(DOID, SystemID, "remarks", resultid.GetType(), tagRemarks, "results id, remarks", remarks, 1);

            var start = StartTextBox.Text;
            var tagStart = string.Format("<results id=\"{0}\"><start>", resultid);
            InsertElementToDataBase(DOID, SystemID, "start", start.GetType(), tagStart, "results id, start", start, 1);

            var end = EndTextBox.Text;
            var tagEnd = string.Format("<results id=\"{0}\"><end>", resultid);
            InsertElementToDataBase(DOID, SystemID, "end", end.GetType(), tagEnd, "results id, end", end, 1);

            MainDiv.Visible = false;
            AddResultPanel.Visible = false;
            GridviewPanel.Visible = true;

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = false;
            AddResultPanel.Visible = false;
            GridviewPanel.Visible = true;
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

        private void PopulateEditPage()
        {
            var roleid = RoleDropDownList.SelectedValue;
            var result = Subjects.Where(x => x.ID == roleid).FirstOrDefault();

            TitleTextBox.Text = result.Title;
            DescriptionTextArea.InnerText = result.Description;
            StartTextBox.Text = result.Start;
            EndTextBox.Text = result.End;
            RoleRemarksTextarea.InnerText = result.Remarks;

            Props = Subjects.Where(x => x.ID == roleid).FirstOrDefault().Props;
          
            Props = Props == null ? new List<Prop>() : Props;

           // Props = GetProps(roleid, "results id");
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            Annotations = Subjects.Where(x => x.ID == roleid).FirstOrDefault().Annotations;
            Annotations = Annotations == null ? new List<Annotation>() : Annotations;
           // Annotations = GetAnnotations(roleid, "results id");
            var annNames = Annotations.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();

            var Findings = GetFindings(roleid);

            result.Findings = Findings;
           
            FindingDropDownList.DataSource = Findings;
           
            FindingDropDownList.DataTextField = "Title";
            FindingDropDownList.DataValueField = "ID";
            FindingDropDownList.DataBind();

        }

        void SetPropertyPanel()
        {
            AddResultPanel.Visible = true;
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
            if (RoleDropDownList.SelectedIndex > 0)
                subjectid = RoleDropDownList.SelectedValue;
            else
                subjectid = GenerateSubjectID();

            var rawTag = string.Format("<{1}=\"{0}\">", subjectid, "results id");


            Props = Subjects.Where(x => x.ID == subjectid).FirstOrDefault().Props;
            // Props = GetProps(subjectid, "results id", rawTag);
            Props = Props == null ? new List<Prop>() : Props;
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

        void SetAnnotationPanel()
        {
            AddResultPanel.Visible = true;
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

            var subjectid = "";
            if (RoleDropDownList.SelectedIndex > 0)
                subjectid = RoleDropDownList.SelectedValue;
            else
                subjectid = GenerateSubjectID();

            var rawTag = string.Format("<{1}=\"{0}\">", subjectid, "results id");

            //Annotations = GetAnnotations(subjectid, "component id", rawTag);
            Annotations = Subjects.Where(x => x.ID == subjectid).FirstOrDefault().Annotations;
            Annotations = Annotations == null ? new List<Annotation>() : Annotations;
            var names = Annotations.Select(x => x.Name).ToList();
            var fullnames = new List<string> { "" };

            var ids = Annotations.Select(x => x.ID).ToList();
            var fullIDs = new List<string> { "" };
            fullIDs.AddRange(ids);

            fullnames.AddRange(names);
            PropDropDownList.DataSource = fullIDs;
            PropDropDownList.DataBind();

            PopulateAnnotationEditPage();

            AnnotationDropDownList.DataSource = names;
            AnnotationDropDownList.DataBind();
        }


        protected void PropButton_Click(object sender, EventArgs e)
        {
            SetPropertyPanel();
        }

        protected void AnnotationButton_Click(object sender, EventArgs e)
        {
            SetAnnotationPanel();
        }

        public void ProcessEntity(string entityId, string entityName)
        {

            var rawTag = string.Format("<{1}=\"{0}\">", entityId, entityName);

            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(entityId, entityName, rawTag);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<{2}=\"{0}\"><prop id=\"{1}\" name>", entityId, propid, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<{2}=\"{0}\"><prop id=\"{1}\" ns>", entityId, propid, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<{2}=\"{0}\"><prop id=\"{1}\" class>", entityId, propid, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);

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

            if (PropLabel.Text == "Annotation")
            {
                var annotationId = GenerateAnnotationID(entityId, entityName, rawTag);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    annotationId = PropDropDownList.SelectedValue;
                }

                var tagAnnotationid = string.Format("<{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<{2}=\"{0}\"><annotation id=\"{1}\" name>", entityId, annotationId, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<{2}=\"{0}\"><annotation id=\"{1}\" ns>", entityId, annotationId, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<{2}=\"{0}\"><annotation id=\"{1}\" class>", entityId, annotationId, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var remarks = RemarksTextArea.InnerText;
                var tagRemarks = string.Format("<{2}=\"{0}\"><annotation id=\"{1}\"><remarks>", entityId, annotationId, entityName);


                InsertElementToDataBase(DOID, SystemID, annotationId, annotationId.GetType(), tagAnnotationid, entityName + ", annotation id", annotationId, 1);
                InsertElementToDataBase(DOID, SystemID, "name", annotationId.GetType(), tagname, entityName + ", annotation name", name, 1);

                InsertElementToDataBase(DOID, SystemID, "ns", annotationId.GetType(), tagns, entityName + ", annotation ns", ns, 1);

                InsertElementToDataBase(DOID, SystemID, "class", annotationId.GetType(), tagClass, entityName + ", annotation class", classn, 1);
                InsertElementToDataBase(DOID, SystemID, "value", annotationId.GetType(), tagValue, entityName + ", annotation", value, 1);

                InsertElementToDataBase(DOID, SystemID, "remarks", annotationId.GetType(), tagRemarks, entityName + ", annotation, remarks", remarks, 1);

            }

            if (PropLabel.Text == "Link")
            {
                var linkHref = "";

                if (PropDropDownList.SelectedIndex > 0)
                {
                    linkHref = PropDropDownList.SelectedValue;
                }
                linkHref = NameTextBox.Text;
                var taghref = string.Format("<{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

                var rel = NSTextBox.Text;
                var tagrel = string.Format("<{2}=\"{0}\"><link href=\"{1}\" rel>", entityId, linkHref, entityName, entityName);

                var media = ClassTextBox.Text;
                var tagmedia = string.Format("<{2}=\"{0}\"><link href=\"{1}\" media-type>", entityId, linkHref, entityName);


                var value = ValueTextBox.Text;
                var tagValue = string.Format("<{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

                InsertElementToDataBase(DOID, SystemID, "link", linkHref.GetType(), taghref, entityName + ", link href", linkHref, 1);
                InsertElementToDataBase(DOID, SystemID, "rel", rel.GetType(), tagrel, entityName + ", link rel", rel, 1);

                InsertElementToDataBase(DOID, SystemID, "media-type", media.GetType(), tagmedia, entityName + ", link media-type", media, 1);


                InsertElementToDataBase(DOID, SystemID, "value", value.GetType(), tagValue, entityName + ", link", value, 1);

            }
        }

        protected void AddEditButton_Click(object sender, EventArgs e)
        {
            var roleid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                roleid = GenerateSubjectID();
            }
            else
                roleid = RoleDropDownList.SelectedValue;
            ProcessEntity(roleid, "results id");
            MainDiv.Visible = true;
            AddResultPanel.Visible = true;

        }

        protected void PropDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PropLabel.Text == "Property")
            {
                Props = Subjects.Where(x => x.ID == RoleDropDownList.SelectedValue).FirstOrDefault().Props;              
                Props = Props == null ? new List<Prop>() : Props;
                PopulatePropertEditPage();
            }
            if (PropLabel.Text == "Annotation")
            {
                Annotations = Subjects.Where(x => x.ID == RoleDropDownList.SelectedValue).FirstOrDefault().Annotations;
                Annotations = Annotations == null ? new List<Annotation>() : Annotations;
                PopulateAnnotationEditPage();
            }

            AddResultPanel.Visible = true;
            MainDiv.Visible = true;

            AddPropPanel.Visible = true;
        }

        protected void RoleDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            AddResultPanel.Visible = true;
            MainDiv.Visible = true;
            PopulateEditPage();

        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {

        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAR/HomeSAR.aspx", false);
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            AddResultPanel.Visible = true;
            MainDiv.Visible = true;
            GridviewPanel.Visible = false;
        }
      

        protected void StartDateCalendar_SelectionChanged(object sender, EventArgs e)
        {

            StartTextBox.Text = StartDateCalendar.SelectedDate.ToString("yyyy-MM-ddThh:mm:ssZ");
            StartDateCalendar.Visible = false;
            AddResultPanel.Visible = true;
            MainDiv.Visible = true;
            
            AddOn = HSize();
            AddResultPanel.Height = Height + AddOn;
        }

        protected void Date1Button_Click(object sender, EventArgs e)
        {

            StartDateCalendar.Visible = true;
            AddResultPanel.Visible = true;
            MainDiv.Visible = true;
            
            AddOn = HSize();
            AddResultPanel.Height = Height + AddOn;
        }

        protected void Date2Button_Click(object sender, EventArgs e)
        {

            EndDateCalendar.Visible = true;
            AddResultPanel.Visible = true;
            MainDiv.Visible = true;
            
            AddOn = HSize();
            AddResultPanel.Height = Height + AddOn;
        }
        protected void EndDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            EndTextBox.Text = EndDateCalendar.SelectedDate.ToString("yyyy-MM-ddThh:mm:ssZ");
            EndDateCalendar.Visible = false;
            
            AddResultPanel.Visible = true;
            MainDiv.Visible = true;
            AddOn = HSize();
            AddResultPanel.Height = Height + AddOn;
        }

        int HSize()
        {
            if (EndDateCalendar.Visible && StartDateCalendar.Visible)
                return 285;
            if (EndDateCalendar.Visible || StartDateCalendar.Visible)
                return 125;
            else
                return 0;
        }

        protected void AddFindingButton_Click(object sender, EventArgs e)
        {
            var resultid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                resultid = GenerateSubjectID();
            }
            else
                resultid = RoleDropDownList.SelectedValue;
            if (Cache["ResultsID"] == null)
            {
                Cache.Insert("ResultsID", resultid);
            }
            else
            {
                Cache["ResultsID"] = resultid;
            }
            Response.Redirect(@"~/PagesSAR/Results/Findings.aspx", false);
        }
    }
}