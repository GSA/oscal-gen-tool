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

namespace OSCALGenerator.PagesSAR.AssessmentSubjects
{
    public partial class ExcludeSubject : BasePage
    {
        List<Prop> Props;
        List<Annotation> Annotations;
        List<Link> IDRefs;
        public List<string> RoleIds;

        List<Subject> Subjects;

        string RoleID;

        protected new void Page_Load(object sender, EventArgs e)
        {
            FaultyCacheBackHomeSAR();
            UserName = Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = int.Parse(Cache["SystemId"].ToString());

            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);

            Subjects = GetSubjects("exclude");

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
            AddRolePanel.Visible = false;
            MainDiv.Visible = false;
            AddPropPanel.Visible = false;
            MainDiv.Visible = false;
            var bindingRoles = new BindingList<Subject>(Subjects);
            this.RolesGridView.DataSource = bindingRoles;
            RolesGridView.DataBind();

            if (!IsPostBack)
            {
                var names = Subjects.Select(x => x.Name).ToList();
                var fullnames = new List<string> { "" };
                fullnames.AddRange(names);
                RoleDropDownList.DataSource = fullnames;
                RoleDropDownList.DataBind();
                PopulateEditPage();

            }
        }

        protected void LinkButton_Click(object sender, EventArgs e)
        {
            SetLinkPanel();
        }

        void SetLinkPanel()
        {
            AddRolePanel.Visible = true;
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

            var rawTag = string.Format("<assessment-subject><{1}=\"{0}\">", subjectid, "exclude-subject");

             IDRefs = GetLinks(subjectid, "exclude-subject", rawTag);
            var names = IDRefs.Select(x => x.HRef).ToList();
            var fullnames = new List<string> { "" };

            fullnames.AddRange(names);
            PropDropDownList.DataSource = fullnames;
            PropDropDownList.DataBind();

            PopulateLinkEditPage();

            LinkDropDownList.DataSource = names;
            LinkDropDownList.DataBind();
        }

        private void PopulateLinkEditPage()
        {
           // var roleid = RoleDropDownList.SelectedValue;
           // var links = GetLinks(roleid, "role id");
            var propId = PropDropDownList.SelectedValue;
            var link = new Link();
            foreach (var elt in IDRefs)
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
            Response.Redirect(@"~/PagesSAR/AssessmentSubject/IncludeSubjects.aspx", false);
        }


        private void PopulateAnnotationEditPage()
        {
           // var rawTag = string.Format("<objectives><{1}=\"{0}\">", "", "controls");
           // var anns = GetAnnotations("", "controls", rawTag);
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
            // ClassTextBox.Text = prop.Class;
            ValueTextBox.Text = ann.Value;
            RemarksTextArea.InnerText = ann.Remarks;


        }
        protected void AddRoleButton_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
            GridviewPanel.Visible = false;

            RoleDropDownList.Visible = true;
            RoleDropDownList.Width = 0;

            DeleteButton.Visible = false;

        }

        private string GenerateSubjectID()
        {
            var nbrRole = Subjects.Count();
            var id = string.Format("excludesubject-{0}", nbrRole);
            return id;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            string roleid = SubjectNameTextBox.Text;
            if (roleid.Length == 0)
            {
                roleid = GenerateSubjectID();
            }

            RoleID = roleid;

            var tagroleid = string.Format("<assessment-subject><exclude-subject name=\"{0}\">", roleid);

            var classo = SubjectClassTextBox.Text;
            var tagClass = string.Format("<assessment-subject><exclude-subject name=\"{0}\" class>", roleid);


            var all = this.AllTextBox.Text;
            var tagAll = string.Format("<assessment-subject><exclude-subject name=\"{0}\"><all>", roleid);

            var desc = this.DescriptionTextArea.InnerText;
            var tagDesc = string.Format("<assessment-subject><exclude-subject name=\"{0}\"><description>", roleid);

            var remarks = RoleRemarksTextarea.InnerText;
            var tagRemarks = string.Format("<assessment-subject><exclude-subject name=\"{0}\"><remarks>", roleid);

            InsertElementToDataBase(DOID, SystemID, "name", roleid.GetType(), tagroleid, "exclude-subject name, exclude-subject name", roleid, 1);
            InsertElementToDataBase(DOID, SystemID, "class", roleid.GetType(), tagClass, "exclude-subject name, class", classo, 1);

            InsertElementToDataBase(DOID, SystemID, "desc", roleid.GetType(), tagDesc, "exclude-subject name, description", desc, 1);

            InsertElementToDataBase(DOID, SystemID, "all", roleid.GetType(), tagAll, "exclude-subject name, all", all, 1);

            InsertElementToDataBase(DOID, SystemID, "remarks", roleid.GetType(), tagRemarks, "exclude-subject name, remarks", remarks, 1);


            MainDiv.Visible = false;
            AddRolePanel.Visible = false;
            GridviewPanel.Visible = true;
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = false;
            AddRolePanel.Visible = false;
            GridviewPanel.Visible = true;
        }


        private void PopulatePropertEditPage()
        {
           // var rawTag = string.Format("<objectives><{1}=\"{0}\">", "", "controls");
            //  var props = GetProps("", "controls", rawTag);
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
            var tag = string.Format("<assessment-subject><exclude-subject name=\"{0}\">", roleid);
            var desc = GetData("desc", tag + "<description>", UserName, DOID);
            DescriptionTextArea.InnerText = desc;

            SubjectNameTextBox.Text = roleid;

            var all = GetData("all", tag + "<all>", UserName, DOID);
            AllTextBox.Text = all;

            var remark = GetData("remarks", tag + "<remarks>", UserName, DOID);
            RoleRemarksTextarea.InnerText = remark;

            var tagclass = string.Format("<assessment-subject><exclude-subject name=\"{0}\" class>", roleid);
            var classo = GetData("class", tagclass, UserName, DOID);
            SubjectClassTextBox.Text = classo;

            var rawTag = tag;

            Props = GetProps(roleid, "exclude-subject name", tag);
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            Annotations = GetAnnotations(roleid, "exclude-subject name", tag);
            var annNames = Annotations.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();

            IDRefs = GetLinks(roleid, "exclude-subject name", tag);
            var linkNames = IDRefs.Select(x => x.HRef).ToList();
            LinkDropDownList.DataSource = linkNames;
            LinkDropDownList.DataBind();
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
            if (RoleDropDownList.SelectedIndex > 0)
                subjectid = RoleDropDownList.SelectedValue;
            else
                subjectid = GenerateSubjectID();

            var rawTag = string.Format("<assessment-subject><{1}=\"{0}\">", subjectid, "exclude-subject name");

            Props = GetProps(subjectid, "exclude-subject name", rawTag);
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
            AddRolePanel.Visible = true;
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

            var rawTag = string.Format("<assessment-subject><{1}=\"{0}\">", subjectid, "exclude-subject name");

            Annotations = GetAnnotations(subjectid, "exclude-subject name", rawTag);
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

            var rawTag = string.Format("<assessment-subject><{1}=\"{0}\">", entityId, entityName);

            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(entityId, entityName, rawTag);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<assessment-subject><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<assessment-subject><{2}=\"{0}\"><prop id=\"{1}\" name>", entityId, propid, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<assessment-subject><{2}=\"{0}\"><prop id=\"{1}\" ns>", entityId, propid, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<assessment-subject><{2}=\"{0}\"><prop id=\"{1}\" class>", entityId, propid, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<assessment-subject><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);

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

                var tagAnnotationid = string.Format("<assessment-subject><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<assessment-subject><{2}=\"{0}\"><annotation id=\"{1}\" name>", entityId, annotationId, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<assessment-subject><{2}=\"{0}\"><annotation id=\"{1}\" ns>", entityId, annotationId, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<assessment-subject><{2}=\"{0}\"><annotation id=\"{1}\" class>", entityId, annotationId, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<assessment-subject><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var remarks = RemarksTextArea.InnerText;
                var tagRemarks = string.Format("<assessment-subject><{2}=\"{0}\"><annotation id=\"{1}\"><remarks>", entityId, annotationId, entityName);


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
                var taghref = string.Format("<assessment-subject><{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

                var rel = NSTextBox.Text;
                var tagrel = string.Format("<assessment-subject><{2}=\"{0}\"><link href=\"{1}\" rel>", entityId, linkHref, entityName, entityName);

                var media = ClassTextBox.Text;
                var tagmedia = string.Format("<assessment-subject><{2}=\"{0}\"><link href=\"{1}\" media-type>", entityId, linkHref, entityName);


                var value = ValueTextBox.Text;
                var tagValue = string.Format("<assessment-subject><{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

                InsertElementToDataBase(DOID, SystemID, "link", linkHref.GetType(), taghref, entityName + ", link href", linkHref, 1);
                InsertElementToDataBase(DOID, SystemID, "rel", rel.GetType(), tagrel, entityName + ", link rel", rel, 1);

                InsertElementToDataBase(DOID, SystemID, "media-type", media.GetType(), tagmedia, entityName + ", link media-type", media, 1);


                InsertElementToDataBase(DOID, SystemID, "value", value.GetType(), tagValue, entityName + ", link", value, 1);

            }
        }

        protected void AddEditButton_Click(object sender, EventArgs e)
        {
            string roleid = SubjectNameTextBox.Text;
            if (roleid.Length == 0)
            {
                roleid = GenerateSubjectID();
            }
            ProcessEntity(roleid, "exclude-subject name");
            MainDiv.Visible = true;
            AddRolePanel.Visible = true;

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

            AddRolePanel.Visible = true;
            MainDiv.Visible = true;

            AddPropPanel.Visible = true;
        }

        protected void RoleDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            PopulateEditPage();

        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {

        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAR/AssessmentSubjects/LocalDefinitions/Remarks.aspx", false);
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            GridviewPanel.Visible = false;
        }
    }
}