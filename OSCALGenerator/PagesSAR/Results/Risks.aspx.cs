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
    public partial class Risks : BasePage
    {
        List<Prop> Props;
        List<Annotation> Annotations;

        public List<Role> Roles;
        public List<string> RoleIds;
        public List<DocParty> Parties;
        public List<string> PartyUUIDs;

        List<Risk> Subjects;
       

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

            AddRemediationPanel.Visible = false;
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = false;
            AddAuxPanel.Visible = false;
            FactorPanel.Visible = false;
            Subjects = GetRisks(ResultID, FindingID);

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

        private string GenerateMitigatingFactorID(string riskid)
        {
            var factors = Subjects.Where(x=>x.ID == riskid).FirstOrDefault().MitigatingFactors;
            factors = factors == null ? new List<MitigatingFactor>() : factors;
            var id = string.Format("{1}MitigatingFactor-{0}", factors.Count, riskid);
            return id;
        }

        private string GenerateRemediationID(string riskid)
        {
            var remediations = Subjects.Where(x => x.ID == riskid).FirstOrDefault().Remediations;
            remediations = remediations == null ? new List<Remediation>() : remediations;
            var id = string.Format("{1}remediation-{0}", remediations.Count, riskid);
            return id;
        }

        private string GenerateSubjectID()
        {
            var nbrRole = Subjects.Count();
            var id = string.Format("{1}Risk-{0}", nbrRole, FindingID);
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

            var tagroleid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\">", ResultID, FindingID, roleid);
            var taguuid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><uuid>", ResultID, FindingID, roleid);

            var preguid = GetData("observation uuid", taguuid, UserName, DOID);
            var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;
            var title = TitleTextBox.Text;
            var tagTitle = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><title>", ResultID, FindingID, roleid);

            var desc = this.DescriptionTextArea.InnerText;
            var tagDesc = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><description>", ResultID, FindingID, roleid);

            var remarks = RoleRemarksTextarea.InnerText;
            var tagRemarks = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><remarks>", ResultID, FindingID, roleid);

            var status = RiskStatusDropDownList.SelectedValue;
            var tagStatus = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><risk-status>", ResultID, FindingID, roleid);

            InsertElementToDataBase(DOID, SystemID, "risk id", roleid.GetType(), tagroleid, "risk id, risk id", roleid, 1);
            InsertElementToDataBase(DOID, SystemID, "risk uuid", roleid.GetType(), taguuid, "risk id, uuid", guid, 1);

            InsertElementToDataBase(DOID, SystemID, "title", title.GetType(), tagTitle, "risk id, title", title, 1);
            InsertElementToDataBase(DOID, SystemID, "status", status.GetType(), tagStatus, "risk id, risk-status", status, 1);

            InsertElementToDataBase(DOID, SystemID, "desc", roleid.GetType(), tagDesc, "risk id, description", desc, 1);

            InsertElementToDataBase(DOID, SystemID, "remarks", roleid.GetType(), tagRemarks, "risk id, remarks", remarks, 1);


            Response.Redirect(@"~/PagesSAR/Results/Findings.aspx", false);
            if (Cache["Risk"] == null)
            {
                Cache.Insert("Risk", "true");
            }
            else
            {
                Cache["Risk"] = "true";
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

        private void PopulateEditPage()
        {
            var roleid = RoleDropDownList.SelectedValue;
            var tag = string.Format("<results id=\"{2}\"><finding id=\"{1}\"><risk id=\"{0}\">", roleid, FindingID, ResultID);

            var tagName = string.Format("<results id=\"{2}\"><finding id=\"{1}\"><risk id=\"{0}\"><compare-to>", roleid, FindingID, ResultID);

            var tagTitle = string.Format("<results id=\"{2}\"><finding id=\"{1}\"><risk id=\"{0}\"><title>", roleid, FindingID, ResultID);
            TitleTextBox.Text = GetData("title", tagTitle, UserName, DOID);


            var tagDesc = string.Format("<results id=\"{2}\"><finding id=\"{1}\"><risk id=\"{0}\"><description>", roleid, FindingID, ResultID);
            DescriptionTextArea.InnerText = GetData("desc", tagDesc, UserName, DOID);

            var tagRemarks = string.Format("<results id=\"{2}\"><finding id=\"{1}\"><risk id=\"{0}\"><remarks>", roleid, FindingID, ResultID);
            RoleRemarksTextarea.InnerText = GetData("remarks", tagRemarks, UserName, DOID);

            var rawTag = tag;

            var risk = Subjects.Where(x => x.ID == roleid).FirstOrDefault();
            risk = risk.Title == null ? new Risk() : risk;
            RiskStatusDropDownList.SelectedValue = risk.RiskStatus;

            //  Props = GetProps(roleid, "observation id", tag);
            Props = risk.Props;
            Props = Props == null ? new List<Prop>() : Props;
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            // Annotations = GetAnnotations(roleid, "observation id", tag);
            Annotations = risk.Annotations;
            Annotations = Annotations == null ? new List<Annotation>() : Annotations;
            var annNames = Annotations.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();

            var riskmetrics = risk.RiskMetrics;
            riskmetrics = riskmetrics == null ? new List<RiskMetric>() : riskmetrics;
            RiskMetricsDropDownList.DataSource = riskmetrics;
            RiskMetricsDropDownList.DataTextField = "Name";
            RiskMetricsDropDownList.DataValueField = "Name";
            RiskMetricsDropDownList.DataBind();

            var factors = risk.MitigatingFactors;
            factors = factors == null ? new List<MitigatingFactor>() : factors;
            MitigatingFactorsDropDownList.DataSource = factors;
            MitigatingFactorsDropDownList.DataTextField = "ID";
            MitigatingFactorsDropDownList.DataValueField = "ID";
            MitigatingFactorsDropDownList.DataBind();

            var remeds = risk.Remediations;
            remeds = remeds == null ? new List<Remediation>() : remeds;
            RemediationDropDownList.DataSource = remeds;
            RemediationDropDownList.DataTextField = "Title";
            RemediationDropDownList.DataValueField = "ID";
            RemediationDropDownList.DataBind();
        }

        void SetPropertyPanel( string tail=null)
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

            var rawTag = string.Format("<results id=\"{3}\"><finding id=\"{2}\"><{1}=\"{0}\">", subjectid, "risk id", FindingID, ResultID);
            Props = GetProps(subjectid, "risk id", rawTag);

            if (tail != null)
            {
                rawTag = string.Format("<results id=\"{3}\"><finding id=\"{2}\"><{1}=\"{0}\">{4}", subjectid, "risk id", FindingID, ResultID, tail);
                if (!tail.Contains("required"))
                {
                    PropLabel.Text = "Remediation Property";
                    Props = GetProps(subjectid, "risk id", rawTag);
                    AddRemediationPanel.Visible = true;
                }
                else
                {
                    var id = "";
                    Props = GetProps(id, "required id", rawTag);
                }
               
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

        void SetAnnotationPanel( string tail = null)
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

            var rawTag = string.Format("<results id=\"{3}\"><finding id=\"{2}\"><{1}=\"{0}\">", subjectid, "risk id", FindingID, ResultID);
            Annotations = GetAnnotations(subjectid, "risk id", rawTag);

            if (tail != null)
            {
                rawTag = string.Format("<results id=\"{3}\"><finding id=\"{2}\"><{1}=\"{0}\">{3}", subjectid, "risk id", FindingID, ResultID, tail);
                if (!tail.Contains("required"))
                {
                    PropLabel.Text = "Remediation Annotation";
                    Annotations = GetAnnotations(subjectid, "risk id", rawTag);
                    AddRemediationPanel.Visible = true;
                }
                else
                {
                    var id = "";
                    Annotations = GetAnnotations(id, "required id", rawTag);
                }

               
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

            var rawTag = string.Format("<results id=\"{3}\"><finding id=\"{2}\"><{1}=\"{0}\">" + label, entityId, entityName, FindingID, ResultID);

            if (PropLabel.Text.Contains("Property"))
            {
                var propid = GeneratePropID(entityId, entityName, rawTag);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + "<prop id=\"{1}\">", entityId, propid, entityName, FindingID, ResultID);
                var name = NameTextBox.Text;
                var tagname = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + "<prop id=\"{1}\" name>", entityId, propid, entityName, FindingID, ResultID);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + "<prop id=\"{1}\" ns>", entityId, propid, entityName, FindingID, ResultID);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + "<prop id=\"{1}\" class>", entityId, propid, entityName, FindingID, ResultID);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + "<prop id=\"{1}\">", entityId, propid, entityName, FindingID, ResultID);

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

            if (PropLabel.Text.Contains("Annotation"))
            {
                var annotationId = GenerateAnnotationID(entityId, entityName, rawTag);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    annotationId = PropDropDownList.SelectedValue;
                }

                var tagAnnotationid = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + " <annotation id=\"{1}\">", entityId, annotationId, entityName, FindingID, ResultID);
                var name = NameTextBox.Text;
                var tagname = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + "<annotation id=\"{1}\" name>", entityId, annotationId, entityName, FindingID, ResultID);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + "<annotation id=\"{1}\" ns>", entityId, annotationId, entityName, FindingID, ResultID);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + "<annotation id=\"{1}\" class>", entityId, annotationId, entityName, FindingID, ResultID);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<results id=\"{4}\"><finding id=\"{3}\"><{2}=\"{0}\">" + label + " <annotation id=\"{1}\">", entityId, annotationId, entityName, FindingID, ResultID);
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
            if(PropLabel.Text == "Property" || PropLabel.Text == "Annotation")
                 ProcessEntity(roleid, "risk id");

            if (PropLabel.Text == "Remediation Annotation" || PropLabel.Text == "Remediation Property")
            {
                var id = AddRemediationDropDownList.SelectedValue;
                var label = string.Format("<remediation id=\"{0}\">", id);
                ProcessEntity(roleid, "risk id", label);
                AddRemediationPanel.Visible = true;

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
                var text = FirstTextBox.Text;
                if (roleid.Length == 0)
                    roleid = text;

                var tagroleid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><observation-method>", ResultID, FindingID, obsid);
                InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "observation id, observation method", roleid, 1);
            }
            if (KeyLabel.Text == "Types")
            {
                var roleid = AuxDropDownList.SelectedValue;
                var text = FirstTextBox.Text;
                if (roleid.Length == 0)
                    roleid = text;

                var tagroleid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><observation-type>", ResultID, FindingID, obsid);
                InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "observation id, observation type", roleid, 1);

            }
            if (KeyLabel.Text == "Party Name")
            {
                var partyuuid = AuxDropDownList.SelectedValue;
                var assessor = FirstTextBox.Text;
                var tagassessor = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><assessor>", ResultID, FindingID, obsid);
                var tagpartuuid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\"><assessor><party uuid>", ResultID, FindingID, obsid);
                InsertElementToDataBase(DOID, SystemID, partyuuid, partyuuid.GetType(), tagpartuuid, "observation, assessor party uuid", partyuuid, 1);
                InsertElementToDataBase(DOID, SystemID, assessor, assessor.GetType(), tagassessor, "observation, assessor", assessor, 1);

            }
            if (KeyLabel.Text == "Risk Metric")
            {
                var roleid = RoleDropDownList.SelectedValue;
               
              
                var name = FirstTextBox.Text;

                var tagname = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><risk-metric name>", ResultID, FindingID, roleid);
                var tagclass = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><risk-metric name><class>", ResultID, FindingID, roleid);
                var tagsystem = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><risk-metric name><system>", ResultID, FindingID, roleid);
                var tag = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><risk-metric name>", ResultID, FindingID, roleid);

                var classo = SecondTextBox.Text;
                var system = ThirdTextBox.Text;
                var value = FourTextBox.Text;

                InsertElementToDataBase(DOID, SystemID, "name="+name, roleid.GetType(),tagname, "risk id, risk-metric name", name, 1);
                InsertElementToDataBase(DOID, SystemID, "class", roleid.GetType(), tagclass, "risk id, risk-metric class", classo, 1);
                InsertElementToDataBase(DOID, SystemID, "system", roleid.GetType(), tagsystem, "risk id, risk-metric system", system, 1);
                InsertElementToDataBase(DOID, SystemID, "value", roleid.GetType(), tag, "risk id, risk-metric", value, 1);

            }

            if(KeyLabel.Text == "mitigating-factor")
            {
                var roleid = AuxDropDownList.SelectedValue;
                var factorid = AddFactorDropDownList.SelectedValue;
                var text = FirstTextBox.Text;
                if (roleid.Length == 0)
                    roleid = text;


                var tagroleid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><mitigating-factor id=\"{3}\"><subject-reference>", ResultID, FindingID, obsid, factorid);
                var taguuid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><mitigating-factor id=\"{3}\"><subject-reference><uuid-ref>", ResultID, FindingID, obsid, factorid);
                var tagtype = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><mitigating-factor id=\"{3}\"><subject-reference><type>", ResultID, FindingID, obsid, factorid);
                var type = TypeDropDownList.SelectedValue;


                var preguid = GetData("subject-reference uuid", taguuid, UserName, DOID);
                var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;

                InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "mitigating-factor id, subject-reference", roleid, 1);
                InsertElementToDataBase(DOID, SystemID, "subject-reference uuid", roleid.GetType(), taguuid, "mitigating-factor id, subject-reference uuid-ref", guid, 1);
                InsertElementToDataBase(DOID, SystemID, "subject-reference type", roleid.GetType(), tagtype, "mitigating-factor id, subject-reference type", type, 1);

            }


            if (KeyLabel.Text == "Origin")
            {
                var remedid = AddRemediationDropDownList.SelectedValue;
                var add = string.Format("<remediation id=\"{0}\">", remedid);
                var label = "origin";
                var roleid = AuxDropDownList.SelectedValue;
                var text = FirstTextBox.Text;
                if (roleid.Length == 0)
                    roleid = text;

                var tagroleid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\">{4}<{3}>", ResultID, FindingID, obsid, label,add);
                var taguuid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\">{4}<{3}><uuid-ref>", ResultID, FindingID, obsid, label, add);
                var tagtype = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\">{4}<{3}><type>", ResultID, FindingID, obsid, label, add);
                var type = OTypeDropDownList.SelectedValue;


                var preguid = GetData(string.Format("{0} uuid", label), taguuid, UserName, DOID);
                var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;

                InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "remediation id, " + label, roleid, 1);
                InsertElementToDataBase(DOID, SystemID, string.Format("{0} uuid", label), roleid.GetType(), taguuid, "remediation id, " + string.Format("{0} uuid-ref", label), guid, 1);
                InsertElementToDataBase(DOID, SystemID, string.Format("{0} type", label), roleid.GetType(), tagtype, "remediation id, " + string.Format("{0} type", label), type, 1);

            }

            MainDiv.Visible = true;
            AddRolePanel.Visible = true;

        }

        protected void SaveFunctionButton_Click(object sender, EventArgs e)
        {

        }


       
        protected void RiskMetricsButton_Click(object sender, EventArgs e)
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
            AuxLabel.Text = "Risk Metric";
            KeyLabel.Text = "Risk Metric";
            AddAuxPanel.Height = 200;
            FirstLabel.Height = 20;
            FirstLabel.Text = "Name";
            FirstTextBox.Height = 20;
            SecondTextBox.Visible = true;
            SecondLabel.Visible = true;
            SecondLabel.Text = "Class";

            TypeDropDownList.Height = 0;
            TypeDropDownList.Visible = false;

            ThirdLabel.Height = 20;
            ThirdLabel.Visible = true;
            ThirdLabel.Text = "System";
            ThirdTextBox.Height = 20;
            ThirdTextBox.Visible = true;
            FourLabel.Height = 20;
            FourLabel.Visible = true;
            FourLabel.Text = "Value";
            FourTextBox.Height = 20;
            FourTextBox.Visible = true;
            List<RiskMetric> Parties = GetRiskMetrics(obsid, "risk id");
            var allparties = new List<RiskMetric> { new RiskMetric() };
            allparties.AddRange(Parties);
            AuxDropDownList.DataSource = allparties;
            AuxDropDownList.DataTextField = "Name";
            AuxDropDownList.DataValueField = "Name";
            AuxDropDownList.DataBind();
        }

     



        protected void SusbjectReferenceButton_Click(object sender, EventArgs e)
        {
            var riskid = "";
            var factorid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                riskid = GenerateSubjectID();
            }
            else
                riskid = RoleDropDownList.SelectedValue;

            if (AddFactorDropDownList.SelectedIndex == 0)
            {
                factorid = GenerateMitigatingFactorID(riskid);
            }
            else
                factorid = AddFactorDropDownList.SelectedValue;

            AddAuxPanel.Visible = true;
            AddAuxPanel.Height = 150;
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            AuxLabel.Text = "Subject Reference";
            KeyLabel.Text = "Factor Subject";
            AddAuxPanel.Height = 150;
            FirstLabel.Height = 20;
            FirstTextBox.Height = 20;
            FirstTextBox.Visible = true;
            SecondLabel.Visible = true;
            SecondLabel.Height = 20;

            TypeDropDownList.Height = 20;
            TypeDropDownList.Visible = true;
            SecondTextBox.Height = 0;
            SecondTextBox.Visible = false;
            ThirdLabel.Height = 0;
            ThirdLabel.Visible = false;
            ThirdTextBox.Height = 0;
            ThirdTextBox.Visible = false;

            FourLabel.Height = 0;
            FourLabel.Visible = false;
            FourTextBox.Height = 0;
            FourTextBox.Visible = false;
            List<SubjectReference> Parties = GetSubjectReferences(factorid, "mitigating-factor id");
            var all = new List<SubjectReference> { new SubjectReference() };
            all.AddRange(Parties);
            AuxDropDownList.DataSource = all;
            AuxDropDownList.DataTextField = "Value";
            AuxDropDownList.DataValueField = "Value";
            AuxDropDownList.DataBind();
        }

        protected void OriginButton_Click(object sender, EventArgs e)
        {
            var riskid = "";
            var remedid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                riskid = GenerateSubjectID();
            }
            else
                riskid = RoleDropDownList.SelectedValue;

            if (AddRemediationDropDownList.SelectedIndex == 0)
            {
                remedid = GenerateRemediationID(riskid);
            }
            else
                remedid = AddFactorDropDownList.SelectedValue;

            AddAuxPanel.Visible = true;
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            AuxLabel.Text = "Origin";
            KeyLabel.Text = "Origin";
            AddAuxPanel.Height = 140;
           
            TypeDropDownList.Height = 0;
            TypeDropDownList.Visible = false;
            OTypeDropDownList.Height = 20;
            OTypeDropDownList.Visible = true;

            List<RemediationOrigin> origins = GetOrigins(remedid, "remediation id", "origin");
            var or = new RemediationOrigin();
            var allor = new List<RemediationOrigin> { or };
            allor.AddRange(origins);
            AuxDropDownList.DataSource = allor;
            AuxDropDownList.DataTextField = "Value";
            AuxDropDownList.DataValueField = "Value";
            AuxDropDownList.DataBind();
            ThirdLabel.Height = 0;
            ThirdLabel.Visible = false;
            ThirdTextBox.Height = 0;
            ThirdTextBox.Visible = false;
            FourLabel.Height = 0;
            FourLabel.Visible = false;
            FourTextBox.Height = 0;
            FourTextBox.Visible = false;
            SecondLabel.Height = 0;
            SecondLabel.Visible = false;
            SecondTextBox.Height = 0;
            SecondTextBox.Visible = false;
            AddRemediationPanel.Visible = true;
        }

        protected void Button3_Click(object sender, EventArgs e)
        {

        }

        protected void AuxDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var roleid = RoleDropDownList.SelectedValue;
            var threatid = AuxDropDownList.SelectedValue;

            //var threats = Subjects.Where(x => x.ID == roleid).FirstOrDefault().ThreatIDs;
            //var threat = threats.Where(x => x.ID == threatid).FirstOrDefault();
            //threat = threat.ID.Length == 0 ? new ThreatID() : threat;
            //SystemTextBox.Text = threat.System;
            //UriTextBox.Text = threat.URI;
            //ThreatTextBox.Text = threat.Value;

            AddAuxPanel.Visible = true;
        }

        protected void MitigatingFactorButton_Click(object sender, EventArgs e)
        {
            var obsid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                obsid = GenerateSubjectID();
            }
            else
                obsid = RoleDropDownList.SelectedValue;

            var factors = GetMitigatingFactors(obsid);
            var all = new List<MitigatingFactor> { new MitigatingFactor() };
            all.AddRange(factors);
            AddFactorDropDownList.DataSource = all;
            AddFactorDropDownList.DataValueField = "ID";
            AddFactorDropDownList.DataTextField = "ID";
            AddFactorDropDownList.DataBind();
            FactorPanel.Visible = true;
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
        }

        protected void SaveFactorButton_Click(object sender, EventArgs e)
        {
            var riskid = "";
            var factorid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                riskid = GenerateSubjectID();
            }
            else
                riskid = RoleDropDownList.SelectedValue;

            if (AddFactorDropDownList.SelectedIndex == 0)
            {
                factorid = GenerateMitigatingFactorID(riskid);
            }
            else
                factorid = AddFactorDropDownList.SelectedValue;

            var tagFactorId = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><mitigating-factor id=\"{3}\">", ResultID, FindingID, riskid, factorid);
            var tagFactorUUID = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><mitigating-factor id=\"{3}\"><uuid>", ResultID, FindingID, riskid, factorid);
            var tagImpleUUID = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><mitigating-factor id=\"{3}\"><implementation-uuid>", ResultID, FindingID, riskid, factorid);
            var tagDesc = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><mitigating-factor id=\"{3}\"><description>", ResultID, FindingID, riskid, factorid);

            var preguid = GetData("mitigating-factor uuid", tagFactorUUID, UserName, DOID);
            var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;

            var preImplguid = GetData("mitigating-factor implementation-uuid", tagImpleUUID, UserName, DOID);
            var guidImpl = preImplguid.Length == 0 ? Guid.NewGuid().ToString() : preImplguid;

            var desc = FactoDescTextarea.InnerHtml;



            InsertElementToDataBase(DOID, SystemID, factorid, factorid.GetType(), tagFactorId, "risk id, mitigating-factor id", factorid, 1);
            InsertElementToDataBase(DOID, SystemID, "mitigating-factor uuid", guid.GetType(), tagFactorUUID, "risk id, mitigating-factor uuid", guid, 1);
            InsertElementToDataBase(DOID, SystemID, "mitigating-factor implementation-uuid", guid.GetType(), tagImpleUUID, "risk id, mitigating-factor implementation-uuid", guidImpl, 1);
            InsertElementToDataBase(DOID, SystemID, "desc", desc.GetType(), tagDesc, "risk id, mitigating-factor desc", desc, 1);
            FactorPanel.Visible = false;
        }

        protected void AddFactorDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var riskid = RoleDropDownList.SelectedValue;
            var factorid = AddFactorDropDownList.SelectedValue;
            var factors = Subjects.Where(x => x.ID == riskid).FirstOrDefault().MitigatingFactors;
            factors = factors == null ? new List<MitigatingFactor>() : factors;
            var fac =factors.Where(x => x.ID == factorid).FirstOrDefault();
            FactoDescTextarea.InnerHtml = fac.Description;
            RefDropDownList.DataSource = fac.SubjectReferences;
            RefDropDownList.DataTextField = "Value";
            RefDropDownList.DataValueField = "Value";
            RefDropDownList.DataBind();

            FactorPanel.Visible = true;
        }

        protected void RemediationButton_Click(object sender, EventArgs e)
        {
            var riskid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                riskid = GenerateSubjectID();
            }
            else
                riskid = RoleDropDownList.SelectedValue;

            var risk = Subjects.Where(x => x.ID == riskid).FirstOrDefault();
            risk = risk.ID == null ? new Risk() : risk;

            var remeds = risk.Remediations;
            remeds = remeds == null ? new List<Remediation>() : remeds;
            var all = new List<Remediation> { new Remediation() };
            all.AddRange(remeds);
            AddRemediationDropDownList.DataSource = all;
            AddRemediationDropDownList.DataValueField = "ID";
            AddRemediationDropDownList.DataTextField = "ID";
            AddRemediationDropDownList.DataBind();
            AddRemediationPanel.Visible = true;
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
        }

        protected void SaveTestStepButton_Click(object sender, EventArgs e)
        {
            var riskid = "";
            var remedid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                riskid = GenerateSubjectID();
            }
            else
                riskid = RoleDropDownList.SelectedValue;
            if (AddRemediationDropDownList.SelectedIndex == 0)
            {
                remedid = GenerateRemediationID(riskid);
            }
            else
                remedid = AddRemediationDropDownList.SelectedValue;


            var tagRemedId = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><remediation id=\"{3}\">", ResultID, FindingID, riskid, remedid);
            var tagRemedUUID = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><remediation id=\"{3}\"><uuid>", ResultID, FindingID, riskid, remedid);
            var tagRemedType = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><remediation id=\"{3}\"><type>", ResultID, FindingID, riskid, remedid);
            var tagDesc = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><remediation id=\"{3}\"><description>", ResultID, FindingID, riskid, remedid);
            var tagRem = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><remediation id=\"{3}\"><remarks>", ResultID, FindingID, riskid, remedid);
            var tagTitle = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><remediation id=\"{3}\"><title>", ResultID, FindingID, riskid, remedid);
           

            var preguid = GetData("remediation uuid", tagRemedUUID, UserName, DOID);
            var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;
            var type = RemedTypeDropDownList.SelectedValue;
            var desc = RemedDescTextArea.InnerHtml;
            var rem = RemedRemarkTextArea.InnerHtml;
            var title = TitleRemediationTextBox.Text;

            InsertElementToDataBase(DOID, SystemID, remedid, remedid.GetType(), tagRemedId, "risk id, remediation id", remedid, 1);
            InsertElementToDataBase(DOID, SystemID, "remediation uuid", guid.GetType(), tagRemedUUID, "risk id, remediation uuid", guid, 1);
            InsertElementToDataBase(DOID, SystemID, "type", type.GetType(), tagRemedType, "risk id, remediation type", type, 1);
            InsertElementToDataBase(DOID, SystemID, "desc", desc.GetType(), tagDesc, "risk id, remediation desc", desc, 1);
            InsertElementToDataBase(DOID, SystemID, "remarks", rem.GetType(), tagRem, "risk id, remediation remarks", rem, 1);
            InsertElementToDataBase(DOID, SystemID, "title", rem.GetType(), tagTitle, "risk id, remediation title", title, 1);

            AddRemediationPanel.Visible = false;

        }

        protected void AddRemediationDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddRemediationPanel.Visible = true;
            var riskid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                riskid = GenerateSubjectID();
            }
            else
                riskid = RoleDropDownList.SelectedValue;

            var risk = Subjects.Where(x => x.ID == riskid).FirstOrDefault();
            risk = risk.ID == null ? new Risk() : risk;
            var remeds = risk.Remediations;
            remeds = remeds == null ? new List<Remediation>() : remeds;
            var medid = AddRemediationDropDownList.SelectedValue;

            var obj = remeds.Where(x => x.ID == medid).FirstOrDefault();
            TitleRemediationTextBox.Text = obj.Title;
            RemedTypeDropDownList.SelectedValue = obj.Type;
            RemedDescTextArea.InnerHtml = obj.Description;
            RemedRemarkTextArea.InnerHtml = obj.Remarks;
            RemedTypeDropDownList.SelectedValue = obj.Type;
            var props = obj.Props;
            RemedPropDropDownList.DataSource = props;
            RemedPropDropDownList.DataValueField = "Name";
            RemedPropDropDownList.DataTextField = "Name";
            RemedPropDropDownList.DataBind();
            var anns = obj.Annotations;
            RemedAnnDropDownList.DataSource = anns;
            RemedAnnDropDownList.DataValueField = "Name";
            RemedAnnDropDownList.DataTextField = "Name";
            RemedAnnDropDownList.DataBind();
            var origins = obj.RemediationOrigins;
            OriginDropDownList.DataSource = origins;
            OriginDropDownList.DataValueField = "Value";
            OriginDropDownList.DataTextField = "Value";
            OriginDropDownList.DataBind();
        }

        protected void RemedPropButton_Click(object sender, EventArgs e)
        {
            var remedid = AddRemediationDropDownList.SelectedValue; ;
            var label = string.Format("<remediation id=\"{0}\">", remedid);
            SetPropertyPanel(label);

        }

        protected void RemedAnnButton_Click(object sender, EventArgs e)
        {
            var remedid = AddRemediationDropDownList.SelectedValue; ;
            var label = string.Format("<remediation id=\"{0}\">", remedid);
            SetAnnotationPanel(label);
        }
    }
}