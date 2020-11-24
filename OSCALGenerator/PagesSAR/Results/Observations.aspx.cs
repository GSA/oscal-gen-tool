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
    public partial class Observations : BasePage
    {
        List<Prop> Props;
        List<Annotation> Annotations;
     
        public List<Role> Roles;
        public List<string> RoleIds;
        public List<DocParty> Parties;
        public List<string> PartyUUIDs;

        List<Observation> Subjects;
        List<TestStep> TestSteps;

        string ResultID;
        string FindingID; 

        protected new void Page_Load(object sender, EventArgs e)
        {
            FaultyCacheBackHomeSAR();
            UserName = Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = int.Parse(Cache["SystemId"].ToString());

            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);

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

            if (Cache["ResultsID"] != null)
            {
                ResultID = Cache["ResultsID"].ToString();
            }
            else
                return;

            if (Cache["FindingID"] != null)
            {
                FindingID = Cache["FindingID"].ToString();
            }
            else
                return;

            AddAuthorizedFunctPanel.Visible = false;
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = false;
            AddAuxPanel.Visible = false;

            Subjects = GetObservations(ResultID, FindingID);

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

      

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAP/AssessmentSubject/LocalDefinitions/InventoryItems.aspx", false);
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
           

            RoleDropDownList.Visible = true;
            RoleDropDownList.Width = 0;

            DeleteButton.Visible = false;

        }

        private string GenerateEvidenceID(string obsid)
        {
            var obs = Subjects.Where(x => x.ID == obsid).FirstOrDefault();
            obs = obs.ID == null ? new Observation() : obs;
            var nbrRole = obs.RelevantEvidences ==null? 0 : obs.RelevantEvidences.Count;
            var id = string.Format("{0}Evidence-{1}", obsid, nbrRole);
            return id;
        }
        
        private string GenerateSubjectID()
        {
            var nbrRole = Subjects.Count();
            var id = string.Format("{1}Obs-{0}", nbrRole, FindingID);
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

            var tagroleid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\">",  ResultID, FindingID, roleid);
            var taguuid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><uuid>", ResultID, FindingID, roleid);

            var preguid = GetData("observation uuid", taguuid, UserName, DOID);
            var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;

          
            var title = TitleTextBox.Text;
            var tagTitle = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><title>", ResultID, FindingID, roleid);

            var desc = this.DescriptionTextArea.InnerText;
            var tagDesc = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><description>", ResultID, FindingID, roleid);

            var remarks = RoleRemarksTextarea.InnerText;
            var tagRemarks = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><remarks>", ResultID, FindingID, roleid);

            InsertElementToDataBase(DOID, SystemID, "observation id", roleid.GetType(), tagroleid, "observation id, observation id", roleid, 1);
            InsertElementToDataBase(DOID, SystemID, "observation uuid", roleid.GetType(), taguuid, "observation id, uuid", guid, 1);
       
            InsertElementToDataBase(DOID, SystemID, "title", title.GetType(), tagTitle, "observation id, title", title, 1);

            InsertElementToDataBase(DOID, SystemID, "desc", roleid.GetType(), tagDesc, "observation id, description", desc, 1);

            InsertElementToDataBase(DOID, SystemID, "remarks", roleid.GetType(), tagRemarks, "observation id, remarks", remarks, 1);


            Response.Redirect(@"~/PagesSAR/Results/Findings.aspx", false);
            if (Cache["Observation"] == null)
            {
                Cache.Insert("Observation", "true");
            }
            else
            {
                Cache["Observation"] = "true";
            }


        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = false;
            AddRolePanel.Visible = false;
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

        private void PopulateEvidenceEditPage()
        {
            var obsid = RoleDropDownList.SelectedValue;
            var obs = Subjects.Where(x => x.ID == obsid).FirstOrDefault();
            var evid = EvidenceDropDownList.SelectedValue;
            var relevant = obs.RelevantEvidences.Where(x => x.ID == evid).FirstOrDefault();
            HrefTextBox.Text = relevant.HREF;
            EvidenceDescTextArea.InnerHtml = relevant.Description;
            EvidenceRemarkTextArea.InnerHtml = relevant.Description;
            EviPropDropDownList.DataSource = relevant.Props;
            EviPropDropDownList.DataBind();
            EviAnnDropDownList.DataSource = relevant.Annotations;
            EviAnnDropDownList.DataBind();
        }

        private void PopulateEditPage()
        {
            var roleid = RoleDropDownList.SelectedValue;
            var tag = string.Format("<results id=\"{2}\"><finding id=\"{1}\"><observation id=\"{0}\">", roleid, FindingID, ResultID);

            var tagName = string.Format("<results id=\"{2}\"><finding id=\"{1}\"><observation id=\"{0}\"><compare-to>", roleid, FindingID, ResultID);

            var tagTitle = string.Format("<results id=\"{2}\"><finding id=\"{1}\"><observation id=\"{0}\"><title>", roleid, FindingID, ResultID);
            TitleTextBox.Text = GetData("title", tagTitle, UserName, DOID);

            var tagDesc = string.Format("<results id=\"{2}\"><finding id=\"{1}\"><observation id=\"{0}\"><description>", roleid, FindingID, ResultID);
            DescriptionTextArea.InnerText = GetData("desc", tagDesc, UserName, DOID);

            var tagRemarks = string.Format("<results id=\"{2}\"><finding id=\"{1}\"><observation id=\"{0}\"><remarks>", roleid, FindingID, ResultID);
            RoleRemarksTextarea.InnerText = GetData("remarks", tagRemarks, UserName, DOID);

            var rawTag = tag;

            var obs = Subjects.Where(x => x.ID == roleid).FirstOrDefault();
            //  Props = GetProps(roleid, "observation id", tag);
            Props = obs.Props;
            Props = Props == null ? new List<Prop>() : Props;
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            // Annotations = GetAnnotations(roleid, "observation id", tag);
            Annotations = obs.Annotations;
            Annotations = Annotations == null ? new List<Annotation>() : Annotations;
            var annNames = Annotations.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();

            var methods = obs.ObservationMethod;
            methods = methods == null ? new List<string>() : methods;
            ObsMethodDropDownList.DataSource = methods;
            ObsMethodDropDownList.DataBind();

            var types = obs.ObservationType;
            types = types == null ? new List<string>() : types;
            ObsTypeDropDownList.DataSource = types;
            ObsTypeDropDownList.DataBind();

            var assessors = obs.Assessors;
            assessors = assessors == null ? new List<Item>() : assessors;
            AssessorDropDownList.DataSource = assessors.Select(x => x.Value).ToList(); 
            AssessorDropDownList.DataBind();


            var subjects = obs.SubjectReferences;
            subjects = subjects == null ? new List<SubjectReference>() : subjects;
            SubjectReferenceDropDownList.DataSource = subjects;
            SubjectReferenceDropDownList.DataTextField = "Value";
            SubjectReferenceDropDownList.DataValueField = "UUIDRef";
            SubjectReferenceDropDownList.DataBind();

            var origins = obs.Origins;
            OriginDropDownList.DataSource = origins;
            OriginDropDownList.DataTextField = "Value";
            OriginDropDownList.DataValueField = "Value";
            OriginDropDownList.DataBind();

            var evidences = obs.RelevantEvidences;
            RelevantEvidenceDropDownList.DataSource = evidences;
            RelevantEvidenceDropDownList.DataTextField = "HREF";
            RelevantEvidenceDropDownList.DataValueField = "HREF";
            RelevantEvidenceDropDownList.DataBind();
        }

        void SetPropertyPanel( bool evidence=false)
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

            var rawTag = string.Format("<results id=\"{3}\"><finding id=\"{2}\"><{1}=\"{0}\">", subjectid, "observation id", FindingID, ResultID);
            Props = GetProps(subjectid, "observation id", rawTag);
            if (evidence)
            {
                AddAuthorizedFunctPanel.Visible = true;
                var evidenceId = RelevantEvidenceDropDownList.SelectedValue;
                rawTag = string.Format("<results id=\"{3}\"><finding id=\"{2}\"><{1}=\"{0}\"><relevant-evidence id=\"{4}\">", subjectid, "observation id", FindingID, ResultID, evidenceId);
                Props = GetProps(evidenceId, "relevant-evidence id", rawTag);
            }
          
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

        void SetAnnotationPanel(bool evidence = false)
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

            var rawTag = string.Format("<results id=\"{3}\"><finding id=\"{2}\"><{1}=\"{0}\">", subjectid, "observation id", FindingID, ResultID);
            Annotations = GetAnnotations(subjectid, "observation id", rawTag);

            if (evidence)
            {
                AddAuthorizedFunctPanel.Visible = true;
                var evidenceId = RelevantEvidenceDropDownList.SelectedValue;
                rawTag = string.Format("<results id=\"{3}\"><finding id=\"{2}\"><{1}=\"{0}\"><relevant-evidence id=\"{4}\">", subjectid, "observation id", FindingID, ResultID, evidenceId);
                Annotations = GetAnnotations(evidenceId, "relevant-evidence id", rawTag);
            }

           
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

        public void ProcessEntity(string entityId, string entityName, string label = null)
        {

            var rawTag = string.Format("<results id=\"{3}\"><finding id=\"{2}\"><{1}=\"{0}\">"+label, entityId, entityName, FindingID, ResultID);

            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(entityId, entityName, rawTag);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">"+ label+ "<prop id=\"{1}\">", entityId, propid, entityName, FindingID, ResultID);
                var name = NameTextBox.Text;
                var tagname = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">"+ label + "<prop id=\"{1}\" name>", entityId, propid, entityName, FindingID, ResultID);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label +"<prop id=\"{1}\" ns>", entityId, propid, entityName, FindingID, ResultID);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">"+ label+ "<prop id=\"{1}\" class>", entityId, propid, entityName, FindingID, ResultID);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label+ "<prop id=\"{1}\">", entityId, propid, entityName, FindingID, ResultID);

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

                var tagAnnotationid = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + "<annotation id=\"{1}\">", entityId, annotationId, entityName, FindingID, ResultID);
                var name = NameTextBox.Text;
                var tagname = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + "<annotation id=\"{1}\" name>", entityId, annotationId, entityName, FindingID, ResultID);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + "<annotation id=\"{1}\" ns>", entityId, annotationId, entityName, FindingID, ResultID);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + "<annotation id=\"{1}\" class>", entityId, annotationId, entityName, FindingID, ResultID);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label+ "<annotation id=\"{1}\">", entityId, annotationId, entityName, FindingID, ResultID);
                var remarks = RemarksTextArea.InnerText;
                var tagRemarks = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + "<annotation id=\"{1}\"><remarks>", entityId, annotationId, entityName, FindingID, ResultID);


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

            if(!AddAuthorizedFunctPanel.Visible)
               ProcessEntity(roleid, "observation id");
            else
            {
                var evidenceid = RelevantEvidenceDropDownList.SelectedValue;
                var label = string.Format("<relevant-evidence id=\"{0}\">", evidenceid);
                ProcessEntity(roleid, "observation id", label);
            }
            MainDiv.Visible = true;
            AddRolePanel.Visible = true;

        }

        protected void PropDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PropLabel.Text == "Property")
            {
                var roleid = RoleDropDownList.SelectedValue;
                var obs = Subjects.Where(x => x.ID == roleid).FirstOrDefault();
                //  Props = GetProps(roleid, "observation id", tag);
                Props = obs.Props;
                PopulatePropertEditPage();
            }
            if (PropLabel.Text == "Annotation")
            {
                var roleid = RoleDropDownList.SelectedValue;
                var obs = Subjects.Where(x => x.ID == roleid).FirstOrDefault();
                Annotations = obs.Annotations;
                Annotations = Annotations == null ? new List<Annotation>() : Annotations;
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
            Response.Redirect(@"~/PagesSAP/AssessmentSubject/LocalDefinitions/Inventories.aspx", false);
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;           
        }
        protected void AddAuxButton_Click(object sender, EventArgs e)
        {
            var obsid = "";
            
            if (RoleDropDownList.SelectedIndex == 0)
            {
                obsid = GenerateSubjectID();
            }
            else
                obsid = RoleDropDownList.SelectedValue;        

            if (KeyLabel.Text == "Methods")
            {
                var roleid = AuxDropDownList.SelectedValue;
                var text = AuxTextBox.Text;          
                if (roleid.Length == 0)
                    roleid = text;

                var tagroleid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><observation-method>", ResultID, FindingID, obsid);
                InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "observation id, observation method", roleid, 1);
            }
            if (KeyLabel.Text == "Types")
            {
                var roleid = AuxDropDownList.SelectedValue;
                var text = AuxTextBox.Text;
                if (roleid.Length == 0)
                    roleid = text;

                var tagroleid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><observation-type>", ResultID, FindingID, obsid);
                InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "observation id, observation type", roleid, 1);

            }
            if (KeyLabel.Text == "Party Name")
            {
                var partyuuid = AuxDropDownList.SelectedValue;
                var assessor = AuxTextBox.Text;
                var tagassessor = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><assessor>", ResultID, FindingID, obsid);
                var tagpartuuid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><assessor><party uuid>", ResultID, FindingID, obsid);
                InsertElementToDataBase(DOID, SystemID, partyuuid, partyuuid.GetType(), tagpartuuid, "observation, assessor party uuid", partyuuid, 1);
                InsertElementToDataBase(DOID, SystemID, assessor, assessor.GetType(), tagassessor, "observation, assessor", assessor, 1);

            }
            if(KeyLabel.Text == "Subject")
            {
                var roleid = AuxDropDownList.SelectedValue;
                var text = AuxTextBox.Text;
                if (roleid.Length == 0)
                    roleid = text;

                var tagroleid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><subject-reference>", ResultID, FindingID, obsid);
                var taguuid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><subject-reference><uuid-ref>", ResultID, FindingID, obsid);
                var tagtype = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><subject-reference><type>", ResultID, FindingID, obsid);
                var type = TypeDropDownList.SelectedValue;
                

                var preguid = GetData("subject-reference uuid", taguuid, UserName, DOID);
                var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;

                InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "observation id, subject-reference", roleid, 1);
                InsertElementToDataBase(DOID, SystemID, "subject-reference uuid", roleid.GetType(), taguuid, "observation id, subject-reference uuid-ref", guid, 1);
                InsertElementToDataBase(DOID, SystemID, "subject-reference type", roleid.GetType(), tagtype, "observation id, subject-reference type", type, 1);

            }

            if (KeyLabel.Text == "Origin")
            {
                var label = "origin";
                var roleid = AuxDropDownList.SelectedValue;
                var text = AuxTextBox.Text;
                if (roleid.Length == 0)
                    roleid = text;

                var tagroleid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><{3}>", ResultID, FindingID, obsid, label);
                var taguuid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><{3}><uuid-ref>", ResultID, FindingID, obsid, label);
                var tagtype = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><{3}><type>", ResultID, FindingID, obsid, label);
                var type = OTypeDropDownList.SelectedValue;


                var preguid = GetData(string.Format("{0} uuid", label), taguuid, UserName, DOID);
                var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;

                InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "observation id, "+label, roleid, 1);
                InsertElementToDataBase(DOID, SystemID, string.Format("{0} uuid", label), roleid.GetType(), taguuid, "observation id, "+ string.Format("{0} uuid-ref", label), guid, 1);
                InsertElementToDataBase(DOID, SystemID, string.Format("{0} type", label), roleid.GetType(), tagtype, "observation id, "+ string.Format("{0} type", label), type, 1);

            }

            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
            
        }

        protected void SaveFunctionButton_Click(object sender, EventArgs e)
        {

        }

        protected void ObsMethodButton_Click(object sender, EventArgs e)
        {
            var obsid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                obsid = GenerateSubjectID();
            }
            else
                obsid = RoleDropDownList.SelectedValue;
            AddAuxPanel.Visible = true;

            

            AddAuxPanel.Height = 115;
            OtherLabel.Height = 20;
            AuxTextBox.Height = 20;
            TypeLabel.Height = 0;
            TypeLabel.Visible = false;
            TypeDropDownList.Height = 0;
            TypeDropDownList.Visible = false;
            AuxTextBox.Visible = true;
            OtherLabel.Visible = true;

         
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            AuxLabel.Text = "Observation Methods";
            KeyLabel.Text = "Methods";
            List<string> methods = GetAuxillaries(obsid, "observation id", "observation method");
            var realmethods = new List<string> { "" };
            realmethods.AddRange(methods);
            AuxDropDownList.DataSource = realmethods;
            AuxDropDownList.DataBind();
        }

        protected void ObsTypeButton_Click(object sender, EventArgs e)
        {
            var obsid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                obsid = GenerateSubjectID();
            }
            else
                obsid = RoleDropDownList.SelectedValue;
            AddAuxPanel.Visible = true;
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;

  
            AddAuxPanel.Height = 115;
            OtherLabel.Height = 20;
            AuxTextBox.Height = 20;
            AuxTextBox.Visible = true;
            OtherLabel.Visible = true;
            TypeLabel.Height = 0;
            TypeLabel.Visible = false;
            TypeDropDownList.Height = 0;
            TypeDropDownList.Visible = false;

            AuxLabel.Text = "Observation Types";
            KeyLabel.Text = "Types";
            List<string> types = GetAuxillaries(obsid, "observation id", "observation type");
            var realtypes = new List<string> { "" };
            realtypes.AddRange(types);
            AuxDropDownList.DataSource = realtypes;
            AuxDropDownList.DataBind();
        }

        protected void AssessorButton_Click(object sender, EventArgs e)
        {

            var obsid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                obsid = GenerateSubjectID();
            }
            else
                obsid = RoleDropDownList.SelectedValue;
            AddAuxPanel.Visible = true;
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            AuxLabel.Text = "Assessors";
            KeyLabel.Text = "Party Name";
            AddAuxPanel.Height = 115;
            OtherLabel.Height = 20;
            AuxTextBox.Height = 20;
            AuxTextBox.Visible = true;
            OtherLabel.Visible = true;
            TypeLabel.Height = 0;
            TypeLabel.Visible = false;
            TypeDropDownList.Height = 0;
            TypeDropDownList.Visible = false;
            List<DocParty> Parties = GetMainParties();
            AuxDropDownList.DataSource = Parties;
            AuxDropDownList.DataTextField = "Name";
            AuxDropDownList.DataValueField = "UUID";
            AuxDropDownList.DataBind();
        }

        protected void StepDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void SusbjectReferenceButton_Click(object sender, EventArgs e)
        {

            var obsid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                obsid = GenerateSubjectID();
            }
            else
                obsid = RoleDropDownList.SelectedValue;
            AddAuxPanel.Visible = true;
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            AuxLabel.Text = "Subject Reference";
            KeyLabel.Text = "Subject";
            AddAuxPanel.Height = 150;
            OtherLabel.Height = 20;
            AuxTextBox.Height = 20;
            AuxTextBox.Visible = true;
            OtherLabel.Visible = true;
            TypeLabel.Height = 20;
            TypeLabel.Visible = true;
            TypeDropDownList.Height = 20;
            TypeDropDownList.Visible = true;
            OTypeDropDownList.Height = 0;
            OTypeDropDownList.Visible = false;
            List<SubjectReference> Parties = GetSubjectReferences(obsid, "observation id");
            AuxDropDownList.DataSource = Parties;
            AuxDropDownList.DataTextField = "Value";
            AuxDropDownList.DataValueField = "Value";
            AuxDropDownList.DataBind();
        }

        protected void OriginButton_Click(object sender, EventArgs e)
        {

            var obsid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                obsid = GenerateSubjectID();
            }
            else
                obsid = RoleDropDownList.SelectedValue;
            AddAuxPanel.Visible = true;
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            AuxLabel.Text = "Origin";
            KeyLabel.Text = "Origin";
            AddAuxPanel.Height = 150;
            OtherLabel.Height = 20;
            AuxTextBox.Height = 20;
            AuxTextBox.Visible = true;
            OtherLabel.Visible = true;
            TypeLabel.Height = 20;
            TypeLabel.Visible = true;
            TypeDropDownList.Height = 0;
            TypeDropDownList.Visible = false;
            OTypeDropDownList.Height = 20;
            OTypeDropDownList.Visible = true;
            List<RemediationOrigin> origins = GetOrigins(obsid, "observation id", "origin");
            var or = new RemediationOrigin();
            var allor = new List<RemediationOrigin> { or };
            allor.AddRange(origins);
            AuxDropDownList.DataSource = allor;
            AuxDropDownList.DataTextField = "Value";
            AuxDropDownList.DataValueField = "Value";
            AuxDropDownList.DataBind();
        }

       
        protected void EvidenceButton_Click(object sender, EventArgs e)
        {
            AddAuthorizedFunctPanel.Visible = true;
            AddAuxPanel.Visible = false;
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            string obsid;
            if (RoleDropDownList.SelectedIndex == 0)
            {
                obsid = GenerateSubjectID();
            }
            else
                obsid = RoleDropDownList.SelectedValue;

            var nan = new RelevantEvidence();
            var all = new List<RelevantEvidence> { nan };
            List<RelevantEvidence> evidences = GetEvidences(obsid);
            all.AddRange(evidences);
            EvidenceDropDownList.DataSource = all;
            EvidenceDropDownList.DataTextField = "ID";
            EvidenceDropDownList.DataValueField = "ID";
            EvidenceDropDownList.DataBind();
        }

        protected void EvidencePropButton_Click(object sender, EventArgs e)
        {
            SetPropertyPanel(true);
        }

        protected void EvidenceAnnButton_Click(object sender, EventArgs e)
        {
            SetAnnotationPanel(true);
        }

        protected void SaveRelevantEvidenceButton_Click(object sender, EventArgs e)
        {

            var obsid="";
            var evidenceid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                obsid = GenerateSubjectID();
            }
            else
                obsid = RoleDropDownList.SelectedValue;

            if (EvidenceDropDownList.SelectedIndex == 0)
            {
                evidenceid = GenerateEvidenceID( obsid);
            }
            else
                evidenceid = EvidenceDropDownList.SelectedValue;

            var href = HrefTextBox.Text;
            var desc = EvidenceDescTextArea.InnerHtml;
            var remarks = EvidenceRemarkTextArea.InnerHtml;
            var tagroleid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><relevant-evidence id=\"{3}\">", ResultID, FindingID, obsid, evidenceid);
            var tagHref = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><relevant-evidence id=\"{3}\"><href>", ResultID, FindingID, obsid, evidenceid);
            var tagDesc = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><relevant-evidence id=\"{3}\"><desc>", ResultID, FindingID, obsid, evidenceid);
            var tagRemarks = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><relevant-evidence id=\"{3}\"><remarks>", ResultID, FindingID, obsid, evidenceid);

            InsertElementToDataBase(DOID, SystemID, evidenceid, evidenceid.GetType(), tagroleid, "observation id, relevant-evidence id", evidenceid, 1);
            
            InsertElementToDataBase(DOID, SystemID, "description", desc.GetType(), tagDesc, "observation id, relevant-evidence desc", desc, 1);
            InsertElementToDataBase(DOID, SystemID, "href", desc.GetType(), tagHref, "observation id, relevant-evidence href", href, 1);
            InsertElementToDataBase(DOID, SystemID, "remarks", desc.GetType(), tagRemarks, "observation id, relevant-evidence remarks", remarks, 1);

            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
        }

        protected void EvidenceDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateEvidenceEditPage();
        }
    }
}