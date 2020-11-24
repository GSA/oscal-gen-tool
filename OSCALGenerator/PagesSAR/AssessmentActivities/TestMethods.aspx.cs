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

namespace OSCALGenerator.PagesSAR.AssessmentActivities
{
    public partial class TestMethods : BasePage
    {
        List<Prop> Props;
        List<Annotation> Annotations;
        List<Link> Links;
        public List<Role> Roles;
        public List<string> RoleIds;
        public List<DocParty> Parties;
        public List<string> PartyUUIDs;

        List<TestMethod> Subjects;
        List<TestStep> TestSteps;

    

        protected new void Page_Load(object sender, EventArgs e)
        {
            FaultyCacheBackHomeSAR();
            UserName = Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = int.Parse(Cache["SystemId"].ToString());

            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);

            Subjects = GetTestMethods();
            

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
            AddAuthorizedFunctPanel.Visible = false;
            AddRolePanel.Visible = false;
            MainDiv.Visible = false;
            AddPropPanel.Visible = false;
            AddResponsibleRolePanel.Visible = false;
            MainDiv.Visible = false;
            var bindingRoles = new BindingList<TestMethod>(Subjects);
            this.RolesGridView.DataSource = bindingRoles;
            RolesGridView.DataBind();

            if (!IsPostBack)
            {

                var names = Subjects.Select(x => x.ID).ToList();
                var fullnames = new List<string> { "" };
                fullnames.AddRange(names);
                RoleDropDownList.DataSource = fullnames;
                RoleDropDownList.DataBind();
               // PopulateEditPage();

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

            var rawTag = string.Format("<assessment-activities><{1}=\"{0}\">", subjectid, "test-method id");

            Links = GetLinks(subjectid, "test-method id", rawTag);
            var names = Links.Select(x => x.HRef).ToList();
            var fullnames = new List<string> { "" };

            fullnames.AddRange(names);
            PropDropDownList.DataSource = fullnames;
            PropDropDownList.DataBind();

            PopulateLinkEditPage();

            LinkDropDownList.DataSource = names;
            LinkDropDownList.DataBind();
        }

        private void PopulateTestStepEditPage()
        {
            //var methodid = "";
            //if (RoleDropDownList.SelectedIndex == 0)
            //{
            //    methodid = GenerateSubjectID();
            //}
            //else
            //    methodid = RoleDropDownList.SelectedValue;

            var stepId = StepDropDownList.SelectedValue;
            var bon = new TestStep();
            foreach(var x in TestSteps)
            {
                if(x.ID == stepId)
                {
                    bon = x;
                    break;
                }
            }
            SequenceTextBox.Text = bon.Sequence.ToString();
            CompareStepTextBox.Text = bon.CompareTo;
            PrivilegeDescTextArea.InnerHtml = bon.Description;
            RemarkStepTextArea.InnerHtml = bon.Remarks;
            RoleStepDropDownList.DataSource = bon.RoleIds;
            RoleStepDropDownList.DataBind();
            PartyStepDropDownList.DataSource = bon.PartyUUIDs;
            PartyStepDropDownList.DataBind();
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
            Response.Redirect(@"~/PagesSAR/Assets/Origination.aspx", false);
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
            AddRolePanel.Visible = true;
            GridviewPanel.Visible = false;

            RoleDropDownList.Visible = true;
            RoleDropDownList.Width = 0;

            DeleteButton.Visible = false;

        }

        private string GenerateTestStepID()
        {
            var nbrRole = TestSteps.Count();
            var methodid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                methodid = GenerateSubjectID();
            }
            else
                methodid = RoleDropDownList.SelectedValue;
            var id = string.Format("{1}-TestStep-{0}", nbrRole, methodid); 
            return id;
        }

        private string GenerateSubjectID()
        {
            var nbrRole = Subjects.Count();
            var id = string.Format("TestMethod-{0}", nbrRole);
            return id;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            var roleid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                roleid = GenerateSubjectID();
            }
            else
                roleid = RoleDropDownList.SelectedValue;
           

            var tagroleid = string.Format("<assessment-activities><test-method id=\"{0}\">", roleid);
            var taguuid = string.Format("<assessment-activities><test-method id=\"{0}\"><uuid>", roleid);

            var preguid = GetData("test-method uuid", taguuid, UserName, DOID);
            var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;

            var compare = CompareTextBox.Text;
            var tagComp = string.Format("<assessment-activities><test-method id=\"{0}\"><compare-to>", roleid);

            var title = TitleTextBox.Text;
            var tagTitle = string.Format("<assessment-activities><test-method id=\"{0}\"><title>", roleid);

            var desc = this.DescriptionTextArea.InnerText;
            var tagDesc = string.Format("<assessment-activities><test-method id=\"{0}\"><description>", roleid);

            var remarks = RoleRemarksTextarea.InnerText;
            var tagRemarks = string.Format("<assessment-activities><test-method id=\"{0}\"><remarks>", roleid);

            InsertElementToDataBase(DOID, SystemID, "test-method id", roleid.GetType(), tagroleid, "test-method id, test-method id", roleid, 1);
            InsertElementToDataBase(DOID, SystemID, "test-method uuid", roleid.GetType(), taguuid, "test-method id, uuid", guid, 1);

            InsertElementToDataBase(DOID, SystemID, "compare-to", compare.GetType(), tagComp, "test-method id, compare-to", compare, 1);

            InsertElementToDataBase(DOID, SystemID, "title", title.GetType(), tagTitle, "test-method id, title", title, 1);

            InsertElementToDataBase(DOID, SystemID, "desc", roleid.GetType(), tagDesc, "test-method id, description", desc, 1);

            InsertElementToDataBase(DOID, SystemID, "remarks", roleid.GetType(), tagRemarks, "test-method id, remarks", remarks, 1);


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
            var tag = string.Format("<assessment-activities><test-method id=\"{0}\">", roleid);

            var tagName = string.Format("<assessment-activities><test-method id=\"{0}\"><compare-to>", roleid);

            var type = GetData("compare-to", tagName, UserName, DOID);
            CompareTextBox.Text = type;


            var tagTitle = string.Format("<assessment-activities><test-method id=\"{0}\"><title>", roleid);
            TitleTextBox.Text = GetData("title", tagTitle, UserName, DOID);


            var tagDesc = string.Format("<assessment-activities><test-method id=\"{0}\"><description>", roleid);
            DescriptionTextArea.InnerText = GetData("desc", tagDesc, UserName, DOID);

            var tagRemarks = string.Format("<assessment-activities><test-method id=\"{0}\"><remarks>", roleid);
            RoleRemarksTextarea.InnerText = GetData("remarks", tagRemarks, UserName, DOID);

            var rawTag = tag;

            Props = GetProps(roleid, "test-method id", tag);
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            Annotations = GetAnnotations(roleid, "test-method id", tag);
            var annNames = Annotations.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();

            Links = GetLinks(roleid, "test-method id", tag);
            var linkNames = Links.Select(x => x.HRef).ToList();
            LinkDropDownList.DataSource = linkNames;
            LinkDropDownList.DataBind();

            var component = Subjects.Where(x => x.ID == roleid).FirstOrDefault();
            var RespRoles = component.TestSteps == null ? new List<TestStep>() : component.TestSteps;
            var RoleIds = RespRoles.Select(x => x.ID).ToList();
            TestStepDropDownList.DataSource = RoleIds;
            TestStepDropDownList.DataBind();

           


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

            var rawTag = string.Format("<assessment-activities><{1}=\"{0}\">", subjectid, "test-method id");

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

            var rawTag = string.Format("<assessment-activities><{1}=\"{0}\">", subjectid, "test-method id");

            Annotations = GetAnnotations(subjectid, "component id", rawTag);
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

            var rawTag = string.Format("<assessment-activities><{1}=\"{0}\">", entityId, entityName);

            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(entityId, entityName, rawTag);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<assessment-activities><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<assessment-activities><{2}=\"{0}\"><prop id=\"{1}\" name>", entityId, propid, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<assessment-activities><{2}=\"{0}\"><prop id=\"{1}\" ns>", entityId, propid, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<assessment-activities><{2}=\"{0}\"><prop id=\"{1}\" class>", entityId, propid, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<assessment-activities><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);

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

                var tagAnnotationid = string.Format("<assessment-activities><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<assessment-activities><{2}=\"{0}\"><annotation id=\"{1}\" name>", entityId, annotationId, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<assessment-activities><{2}=\"{0}\"><annotation id=\"{1}\" ns>", entityId, annotationId, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<assessment-activities><{2}=\"{0}\"><annotation id=\"{1}\" class>", entityId, annotationId, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<assessment-activities><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var remarks = RemarksTextArea.InnerText;
                var tagRemarks = string.Format("<assessment-activities><{2}=\"{0}\"><annotation id=\"{1}\"><remarks>", entityId, annotationId, entityName);


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
                var taghref = string.Format("<assessment-activities><{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

                var rel = NSTextBox.Text;
                var tagrel = string.Format("<assessment-activities><{2}=\"{0}\"><link href=\"{1}\" rel>", entityId, linkHref, entityName, entityName);

                var media = ClassTextBox.Text;
                var tagmedia = string.Format("<assessment-activities><{2}=\"{0}\"><link href=\"{1}\" media-type>", entityId, linkHref, entityName);


                var value = ValueTextBox.Text;
                var tagValue = string.Format("<assessment-activities><{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

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
            ProcessEntity(roleid, "test-method id");
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
            Response.Redirect(@"~/PagesSAR/AssessmentActivities/Schedules.aspx", false);
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            GridviewPanel.Visible = false;
        }

        protected void AddResponsibleRoleButton_Click(object sender, EventArgs e)
        {
            KeyLabel.Text = "Roles";
            MainDiv.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            AddRolePanel.Visible = true;
            AddAuthorizedFunctPanel.Visible = true;
            Roles = GetRoles();

            var roleIds = Roles.Select(x => x.RoleID);
            RespRoleDropDownList.DataSource = roleIds;
            RespRoleDropDownList.DataBind();
        }

        protected void AddRoleButton_Click1(object sender, EventArgs e)
        {
            KeyLabel.Text = "Roles";
            MainDiv.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            AddRolePanel.Visible = true;
            Roles = GetRoles();
            AddAuthorizedFunctPanel.Visible = true;
            var partyIds = Roles.Select(x => x.RoleID);
            RespRoleDropDownList.DataSource = partyIds;
            RespRoleDropDownList.DataBind();

        }

        protected void AddFunctionButton_Click(object sender, EventArgs e)
        {
            AddAuthorizedFunctPanel.Visible = true;
            MainDiv.Visible = true;

            AddRolePanel.Visible = true;
            var methodid = "";
        
            if (RoleDropDownList.SelectedIndex == 0)
            {
                methodid = GenerateSubjectID();
            }
            else
                methodid = RoleDropDownList.SelectedValue;

            TestSteps = GetTestSteps(methodid);
            var stepIds = TestSteps.Select(x => x.ID).ToList();
            var allIds = new List<string> { "" };
            allIds.AddRange(stepIds);
            StepDropDownList.DataSource = allIds;
            StepDropDownList.DataBind();
        }

        protected void AddAuxButton_Click(object sender, EventArgs e)
        {
            var methodid = "";
            var stepid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                methodid = GenerateSubjectID();
            }
            else
                methodid = RoleDropDownList.SelectedValue;

            TestSteps = GetTestSteps(methodid);

            if (StepDropDownList.SelectedIndex == 0)
            {
                stepid = GenerateTestStepID();
            }
            else
                stepid = StepDropDownList.SelectedValue;

            if (KeyLabel.Text == "Roles")
            {
                var roleid = RespRoleDropDownList.SelectedValue;

                var tagroleid = string.Format("<assessment-activities><test-method id=\"{0}\"><test-step id=\"{1}\"><role id=\"{2}\">", methodid, stepid, roleid);
                InsertElementToDataBase(DOID, SystemID, "test-method test-step role id", roleid.GetType(), tagroleid, "test-method id, test-step id role id", roleid, 1);
            }
            if (KeyLabel.Text == "Parties")
            {
                var roleid = RespRoleDropDownList.SelectedValue;
              

                var tagroleid = string.Format("<assessment-activities><test-method id=\"{0}\"><test-step id=\"{1}\"><party uuid=\"{2}\">", methodid, stepid, roleid);
                InsertElementToDataBase(DOID, SystemID, "test-method test-step party uuid", roleid.GetType(), tagroleid, "test-method id, test-step id party uuid", roleid, 1);
            }

            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
            AddAuthorizedFunctPanel.Visible = true;
        }

        protected void SaveFunctionButton_Click(object sender, EventArgs e)
        {
           
        }


        protected void AddPartyButton_Click(object sender, EventArgs e)
        {
            KeyLabel.Text = "Parties";
            MainDiv.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            AddAuthorizedFunctPanel.Visible = true;
            AddRolePanel.Visible = true;
            Parties = GetFormviewParties();

            //var partyIds = Parties.Select(x => x.UUID);
            RespRoleDropDownList.DataSource = Parties;
            RespRoleDropDownList.DataTextField = "PartyID";
            RespRoleDropDownList.DataValueField = "UUID";
            RespRoleDropDownList.DataBind();
        }

        protected void SaveTestStepButton_Click(object sender, EventArgs e)
        {
            var methodid = "";
            var stepid = "";
           
            var SequenceString = SequenceTextBox.Text;
            if (RoleDropDownList.SelectedIndex == 0)
            {
                methodid = GenerateSubjectID();
            }
            else
                methodid = RoleDropDownList.SelectedValue;

            TestSteps = GetTestSteps(methodid);

            if (StepDropDownList.SelectedIndex == 0)
            {
                stepid = GenerateTestStepID();
            }
            else
                stepid = StepDropDownList.SelectedValue;

            
            var tagroleid = string.Format("<assessment-activities><test-method id=\"{0}\"><test-step id=\"{1}\">", methodid, stepid);

            var taguuid = string.Format("<assessment-activities><test-method id=\"{0}\"><test-step id=\"{1}\"><uuid>", methodid, stepid);

            var preguid = GetData("test-step uuid", taguuid, UserName, DOID);
            var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;

            InsertElementToDataBase(DOID, SystemID, stepid, stepid.GetType(), tagroleid, "test-method id, test-step id", stepid, 1);
            InsertElementToDataBase(DOID, SystemID, "test-step uuid", guid.GetType(), taguuid, "test-method id, test-step uuid", guid, 1);



            var tagSequence = string.Format("<assessment-activities><test-method id=\"{0}\"><test-step id=\"{1}\"><sequence>", methodid, stepid);
            InsertElementToDataBase(DOID, SystemID, "sequence", SequenceString.GetType(), tagSequence, "test-method id, test-step id sequence", SequenceString, 1);

            var desc = PrivilegeDescTextArea.InnerText;
            var tagDesc = string.Format("<assessment-activities><test-method id=\"{0}\"><test-step id=\"{1}\"><description>", methodid, stepid);
            InsertElementToDataBase(DOID, SystemID, "desc", desc.GetType(), tagDesc, "test-method id, test-step id  description", desc, 1);

            var rem = RemarkStepTextArea.InnerText;
            var tagRem = string.Format("<assessment-activities><test-method id=\"{0}\"><test-step id=\"{1}\"><remarks>", methodid, stepid);
            InsertElementToDataBase(DOID, SystemID, "remarks", desc.GetType(), tagRem, "test-method id, test-step id remarks", rem, 1);

            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
            AddAuthorizedFunctPanel.Visible = false;
        }

        protected void StepDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var methodid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                methodid = GenerateSubjectID();
            }
            else
                methodid = RoleDropDownList.SelectedValue;
            TestSteps = GetTestSteps(methodid);
            PopulateTestStepEditPage();
            MainDiv.Visible = true;
           
            AddRolePanel.Visible = true;
            AddAuthorizedFunctPanel.Visible = true;
        }
    }
}