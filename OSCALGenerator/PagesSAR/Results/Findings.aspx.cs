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
    public partial class Findings : BasePage
    {
        List<Prop> Props;
        List<Annotation> Annotations;
   
        public List<Role> Roles;
        public List<string> RoleIds;
        public List<DocParty> Parties;
        public List<string> PartyUUIDs;

        string ResultID;
       

        List<Finding> Subjects;


        int Height = 475;
        int AddOn = 125;



        protected new void Page_Load(object sender, EventArgs e)
        {
            FaultyCacheBackHomeSAR();
            UserName = Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = int.Parse(Cache["SystemId"].ToString());

            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);

         

            AddOn = ( StartDateCalendar.Visible) ? 125 : 0;

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

            

            Subjects = GetFindings(ResultID);

            AddAuthorizedFunctPanel.Visible = false;
            AddFindingPanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = false;
            AddResponsibleRolePanel.Visible = false;
            AddAuxPanel.Visible = false;
            // GridviewPanel.Visible = false;
            //var bindingRoles = new BindingList<Finding>(Subjects);
            //this.RolesGridView.DataSource = bindingRoles;
            //RolesGridView.DataBind();
            if (Cache["Observation"] == null || Cache["Observation"].ToString() != "true")
            {
              
                //var bindingRoles = new BindingList<Finding>(Subjects);
                //this.RolesGridView.DataSource = bindingRoles;
                //RolesGridView.DataBind();
            }
            else
            {
                if (Cache["FindingID"] != null)
                {
                    var names = Subjects.Select(x => x.ID).ToList();
                    var fullnames = new List<string> { "" };
                    fullnames.AddRange(names);
                    RoleDropDownList.DataSource = fullnames;
                    RoleDropDownList.DataBind();

                    var magic = Cache["FindingID"].ToString();
                    RoleDropDownList.SelectedValue = magic;
                    PopulateEditPage();
                    Cache.Remove("FindingID");
                }
            }

            if (Cache["Risk"] == null || Cache["Risk"].ToString() != "true")
            {

            }
            else
            {
                if (Cache["FindingID"] != null)
                {
                    var names = Subjects.Select(x => x.ID).ToList();
                    var fullnames = new List<string> { "" };
                    fullnames.AddRange(names);
                    RoleDropDownList.DataSource = fullnames;
                    RoleDropDownList.DataBind();

                    var magic = Cache["FindingID"].ToString();
                    RoleDropDownList.SelectedValue = magic;
                    PopulateEditPage();
                    Cache.Remove("FindingID");
                }
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
               
                AddFindingPanel.Height = Height;


            }
        }


        public List<string> GetIds()
        {
           
           var ControlIDs = new List<string>();
            string appPath = Request.PhysicalApplicationPath;
            var file = "";
            var tagbaseline = string.Format("<objectives><control-objectives><baseline>");
            var baseline = GetData("baseline", tagbaseline, UserName, DOID);
                       

            switch (baseline)
            {
                case "low":
                    {
                        file = string.Format(@"{0}\{1}", appPath, "ControlParamStatementsLow.txt");
                       
                    }
                    break;
                case "moderate":
                    {
                        file = string.Format(@"{0}\{1}", appPath, "ControlParamStatementsModerate.txt");                   
                    }
                    break;
                case "high":
                    {
                        file = string.Format(@"{0}\{1}", appPath, "ControlParamStatementsHigh.txt");
                    }
                    break;
                default:
                    {
                        file = string.Format(@"{0}\{1}", appPath, "ControlParamStatementsModerate.txt");
                    }
                    break;
            }
           
            using (var sr = new StreamReader(file))
            {
                int n = 0;


                while (!sr.EndOfStream)
                {


                    var line = sr.ReadLine();
                    var index = line.IndexOf(",");
                    var controlid = line.Substring(0, index);


                    ControlIDs.Add(controlid);
                    //  ControlIdToRank.Add(controlid, n);
                    n++;



                }

            }

            return ControlIDs;
        }

        private void PopulateObjectiveStateEditPage()
        {
           
            var stepId = ControlIDDropDownList.SelectedValue;
            var bon = new TestStep();
          
           
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
            AddFindingPanel.Visible = true;
           // GridviewPanel.Visible = false;

            RoleDropDownList.Visible = true;
            RoleDropDownList.Width = 0;

            DeleteButton.Visible = false;

        }

      

        private string GenerateSubjectID()
        {
            var nbrRole = Subjects.Count();
            var id = string.Format("{1}-finding-{0}", nbrRole,ResultID);
            return id;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            var findingid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                findingid = GenerateSubjectID();
            }
            else
                findingid = RoleDropDownList.SelectedValue;


            var tagroleid = string.Format("<results id={0}><finding id=\"{1}\">", ResultID, findingid);
            var taguuid = string.Format("<results id={0}><finding id=\"{1}\"><uuid>", ResultID, findingid);

            var preguid = GetData("finding uuid", taguuid, UserName, DOID);
            var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;

         

            var title = TitleTextBox.Text;
            var tagTitle = string.Format("<results id={0}><finding id=\"{1}\"><title>", ResultID, findingid);

            var desc = this.DescriptionTextArea.InnerText;
            var tagDesc = string.Format("<results id={0}><finding id=\"{1}\"><description>", ResultID, findingid);

            var remarks = RoleRemarksTextarea.InnerText;
            var tagRemarks = string.Format("<results id={0}><finding id=\"{1}\"><remarks>", ResultID, findingid);

            var date = StartTextBox.Text;
            var tagDate = string.Format("<results id={0}><finding id=\"{1}\"><date-time-stamp>", ResultID, findingid);

            InsertElementToDataBase(DOID, SystemID, "finding id", findingid.GetType(), tagroleid, "finding id, finding id", findingid, 1);
            InsertElementToDataBase(DOID, SystemID, "finding uuid", guid.GetType(), taguuid, "finding id, uuid", guid, 1);

       
            InsertElementToDataBase(DOID, SystemID, "title", title.GetType(), tagTitle, "finding id, title", title, 1);

            InsertElementToDataBase(DOID, SystemID, "desc", desc.GetType(), tagDesc, "finding id, description", desc, 1);

            InsertElementToDataBase(DOID, SystemID, "date-time-stamp", date.GetType(), tagDate, "finding id, date-time-stamp", date, 1);


            InsertElementToDataBase(DOID, SystemID, "remarks", remarks.GetType(), tagRemarks, "finding id, remarks", remarks, 1);

            var tagISuuid = string.Format("<results id={0}><finding id=\"{1}\"><implementation-statement-uuid>", ResultID, findingid);

            var preISguid = GetData("implementation-statement-uuid", taguuid, UserName, DOID);
            var ISguid = preISguid.Length == 0 ? Guid.NewGuid().ToString() : preISguid;


            InsertElementToDataBase(DOID, SystemID, "implementation-statement-uuid", ISguid.GetType(), tagISuuid, "finding id, implementation-statement-uuid", ISguid, 1);


            MainDiv.Visible = false;
            AddFindingPanel.Visible = false;
            // GridviewPanel.Visible = true;
            Response.Redirect(@"~/PagesSAR/Results/Results.aspx", false);
            if(Cache["Finding"]==null)
            {
                Cache.Insert("Finding", "true");
            }
            else
            {
                Cache["Finding"] = "true";
            }

           
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = false;
            AddFindingPanel.Visible = false;
           // GridviewPanel.Visible = true;
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
            var tag = string.Format("<results id={0}><finding id=\"{1}\">", ResultID, roleid);

            var tagTitle = string.Format("<results id={0}><finding id=\"{1}\"><title>", ResultID, roleid);
            TitleTextBox.Text = GetData("title", tagTitle, UserName, DOID);

            var tagDesc = string.Format("<results id={0}><finding id=\"{1}\"><description>", ResultID, roleid);
            DescriptionTextArea.InnerText = GetData("desc", tagDesc, UserName, DOID);

            var tagRemarks = string.Format("<results id={0}><finding id=\"{1}\"><remarks>", ResultID, roleid);
            RoleRemarksTextarea.InnerText = GetData("remarks", tagRemarks, UserName, DOID);
            
            var tagDate = string.Format("<results id={0}><finding id=\"{1}\"><date-time-stamp>", ResultID, roleid);
            StartTextBox.Text = GetData("date-time-stamp", tagDate, UserName, DOID);


            var rawTag = tag;
            var fd = Subjects.Where(x => x.ID == roleid).FirstOrDefault();
            fd = fd.ID == null ? new Finding() : fd;
            Props = fd.Props;
            Props = Props == null ? new List<Prop>() : Props;

            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            Annotations = fd.Annotations;
            Annotations = Annotations == null ? new List<Annotation>() : Annotations;
            var annNames = Annotations.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();

            RestoreObjectiveStatus();

            var obs = GetObservations(ResultID, roleid);

            ObservationDropDownList.DataSource = obs;
            ObservationDropDownList.DataTextField = "Title";
            ObservationDropDownList.DataValueField = "ID";
            ObservationDropDownList.DataBind();

            var risk = GetRisks(ResultID, roleid);
            RiskDropDownList.DataSource = risk;
            RiskDropDownList.DataTextField = "Title";
            RiskDropDownList.DataValueField = "ID";
            RiskDropDownList.DataBind();

            var parties  = Subjects.Where(x => x.ID == roleid).FirstOrDefault().PartyUUIDS;
            PartyDropDownList.DataSource = parties;
            PartyDropDownList.DataBind();

            var threats = Subjects.Where(x => x.ID == roleid).FirstOrDefault().ThreatIDs;
            ThreatIDDropDownList.DataSource = threats;
            ThreatIDDropDownList.DataTextField = "Value";
            ThreatIDDropDownList.DataValueField = "Value";
            ThreatIDDropDownList.DataBind();

        }

        void SetPropertyPanel()
        {
            AddFindingPanel.Visible = true;
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
            AddFindingPanel.Visible = true;
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
            var rawTag = string.Format("<results id={2}><{1}=\"{0}\">", entityId, entityName, ResultID);

            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(entityId, entityName, rawTag);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<results id=\"{3}\"><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName, ResultID);
                var name = NameTextBox.Text;
                var tagname = string.Format("<results id=\"{3}\"><{2}=\"{0}\"><prop id=\"{1}\" name>", entityId, propid, entityName, ResultID);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<assessment-activities><{2}=\"{0}\"><prop id=\"{1}\" ns>", entityId, propid, entityName, ResultID);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<results id=\"{3}\"><{2}=\"{0}\"><prop id=\"{1}\" class>", entityId, propid, entityName, ResultID);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<results id=\"{3}\"><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName, ResultID);

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

                var tagAnnotationid = string.Format("<results id=\"{3}\"><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName, ResultID);
                var name = NameTextBox.Text;
                var tagname = string.Format("<results id=\"{3}\"><{2}=\"{0}\"><annotation id=\"{1}\" name>", entityId, annotationId, entityName, ResultID);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<results id=\"{3}\"><{2}=\"{0}\"><annotation id=\"{1}\" ns>", entityId, annotationId, entityName, ResultID);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<results id=\"{3}\"><{2}=\"{0}\"><annotation id=\"{1}\" class>", entityId, annotationId, entityName, ResultID);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<results id=\"{3}\"><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName, ResultID);
                var remarks = RemarksTextArea.InnerText;
                var tagRemarks = string.Format("<results id=\"{3}\"><{2}=\"{0}\"><annotation id=\"{1}\"><remarks>", entityId, annotationId, entityName, ResultID);


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
            AddFindingPanel.Visible = true;

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

            AddFindingPanel.Visible = true;
            MainDiv.Visible = true;

            AddPropPanel.Visible = true;
        }

        protected void RoleDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

            AddFindingPanel.Visible = true;
            MainDiv.Visible = true;
            PopulateEditPage();

        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {

        }

        

        protected void EditButton_Click(object sender, EventArgs e)
        {
            AddFindingPanel.Visible = true;
            MainDiv.Visible = true;
           // GridviewPanel.Visible = false;
        }

        protected void AddResponsibleRoleButton_Click(object sender, EventArgs e)
        {
            KeyLabel.Text = "Roles";
            MainDiv.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            AddFindingPanel.Visible = true;
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
            AddFindingPanel.Visible = true;
            Roles = GetRoles();
            AddAuthorizedFunctPanel.Visible = true;
            var partyIds = Roles.Select(x => x.RoleID);
            RespRoleDropDownList.DataSource = partyIds;
            RespRoleDropDownList.DataBind();
        }    
        protected void SaveResponsibleRoleButton_Click(object sender, EventArgs e)
        {
            var findingid = RoleDropDownList.SelectedValue;

            MainDiv.Visible = true;
            AddFindingPanel.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            if (KeyLabel.Text == "Party Name")
            {
                var partyuuid = RespRoleDropDownList.SelectedValue;
                var tagpartuuid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><party uuid>", ResultID, findingid);
                InsertElementToDataBase(DOID, SystemID, partyuuid, partyuuid.GetType(), tagpartuuid, "finding id, party uuid", partyuuid, 1);
                
            }

            AddResponsibleRolePanel.Visible = false;
        }

        protected void SaveFunctionButton_Click(object sender, EventArgs e)
        {

        }


        protected void AddPartyButton_Click(object sender, EventArgs e)
        {
            KeyLabel.Text = "Party Name";
            MainDiv.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            AddAuthorizedFunctPanel.Visible = true;
            AddFindingPanel.Visible = true;
            Parties = GetFormviewParties();

            //var partyIds = Parties.Select(x => x.UUID);
            RespRoleDropDownList.DataSource = Parties;
            RespRoleDropDownList.DataTextField = "PartyID";
            RespRoleDropDownList.DataValueField = "UUID";
            RespRoleDropDownList.DataBind();
        }



   

        protected void ObjectiveButton_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = true;

            AddFindingPanel.Visible = true;
            AddAuthorizedFunctPanel.Visible = true;
            var controlIds = GetIds();
            ControlIDDropDownList.DataSource = controlIds;
            ControlIDDropDownList.DataBind();
            AuthLabel.Text = " Control ID";
            SecondAuthLabel.Text = " Objective Id";
            ThirdAuthLabel.Text = "Title";
        }
        int HSize()
        {
            if ( StartDateCalendar.Visible)
                return 125;
            else
                return 0;
        }


        protected void Date1Button_Click(object sender, EventArgs e)
        {
            StartDateCalendar.Visible = true;
            AddFindingPanel.Visible = true;
            MainDiv.Visible = true;

            AddOn = HSize();
            AddFindingPanel.Height = Height + AddOn;
        }

        protected void StartDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            StartTextBox.Text = StartDateCalendar.SelectedDate.ToString("yyyy-MM-ddThh:mm:ssZ");
            StartDateCalendar.Visible = false;
            AddFindingPanel.Visible = true;
            MainDiv.Visible = true;

            AddOn = HSize();
            AddFindingPanel.Height = Height + AddOn;
        }

        protected void AddObservationButton_Click(object sender, EventArgs e)
        {
            var findingid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                findingid = GenerateSubjectID();
            }
            else
                findingid = RoleDropDownList.SelectedValue;
            if (Cache["FindingID"] == null)
            {
                Cache.Insert("FindingID", findingid);
            }
            else
            {
                Cache["FindingID"] = findingid;
            }
            Response.Redirect(@"~/PagesSAR/Results/Observations.aspx", false);
        }

        protected void SaveObjStatusButton_Click(object sender, EventArgs e)
        {
            var findingid = "";
       
            if (RoleDropDownList.SelectedIndex == 0)
            {
                findingid = GenerateSubjectID();
            }
            else
                findingid = RoleDropDownList.SelectedValue;

            var controlId = ControlIDDropDownList.SelectedValue;
            SecondAuthTextBox.Text = "objective-" + controlId.Trim();
            SecondAuthLabel.Enabled = false;
            var title = ThirdAuthTextBox.Text;
            var objectId = SecondAuthTextBox.Text;

            var tagControlId = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id>", ResultID, findingid);

            var tagObjectId = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id>", ResultID, findingid);
            InsertElementToDataBase(DOID, SystemID, "control id", controlId.GetType(), tagControlId, "finding id, objective id control id", controlId.Trim(), 1);
            InsertElementToDataBase(DOID, SystemID, "Objective id", controlId.GetType(), tagObjectId, "finding id, objective id", SecondAuthTextBox.Text, 1);

            var tagTitle = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><title>", ResultID, findingid);
            InsertElementToDataBase(DOID, SystemID, "title", title.GetType(), tagTitle, "finding id, objective id control id title", title, 1);

            var desc = AuthDescTextArea.InnerText;
            var tagDesc = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><description>", ResultID, findingid);
            InsertElementToDataBase(DOID, SystemID, "desc", desc.GetType(), tagDesc, "finding id, objective id control id  description", desc, 1);


            var resultSystem = FourthAuthTextBox.Text;
            var tagResultSystem = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><result system>", ResultID, findingid);
            InsertElementToDataBase(DOID, SystemID, "result system", resultSystem.GetType(), tagResultSystem, "finding id, objective id control id result system", resultSystem, 1);

            var resultValue = FifthAuthTextBox.Text;
            var tagResultValue = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><result>", ResultID, findingid);
            InsertElementToDataBase(DOID, SystemID, "result", resultValue.GetType(), tagResultValue, "finding id, objective id control id result", resultValue, 1);


            var implementationSystem = SixthAuthTextBox.Text;
            var tagimplSystem = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><implementation-status system>", ResultID, findingid);
            InsertElementToDataBase(DOID, SystemID, "implementation-status sytem", resultSystem.GetType(), tagimplSystem, "finding id, objective id control id implementation-status system", implementationSystem, 1);

            var implValue = SeventhAuthTextBox.Text;
            var tagImplValue = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><implementation-status>", ResultID, findingid);
            InsertElementToDataBase(DOID, SystemID, "implementation-status", resultValue.GetType(), tagImplValue, "finding id, objective id control id implementation-status", implValue, 1);


            var rem = RemarkStepTextArea.InnerText;
            var tagRem = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><remarks>", ResultID, findingid);
            InsertElementToDataBase(DOID, SystemID, "remarks", desc.GetType(), tagRem, "finding id, objective id control id remarks", rem, 1);

            MainDiv.Visible = true;
            AddFindingPanel.Visible = true;
            AddAuthorizedFunctPanel.Visible = false;
        }

        void RestoreObjectiveStatus()
        {
            var findingid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                findingid = GenerateSubjectID();
            }
            else
                findingid = RoleDropDownList.SelectedValue;

            var controlId = ControlIDDropDownList.SelectedValue;
            SecondAuthTextBox.Text = "objective-" + controlId.Trim();
            SecondAuthLabel.Enabled = false;
         

            var tagControlId = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id>", ResultID, findingid);

            var tagObjectId = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id>", ResultID, findingid);
            ControlIDDropDownList.SelectedValue = GetData("control id", tagControlId,UserName, DOID);
            SecondAuthTextBox.Text = GetData("Objective id", tagObjectId, UserName, DOID);

            var tagTitle = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><title>", ResultID, findingid);
            ThirdAuthTextBox.Text = GetData("title", tagTitle, UserName, DOID);

            
            var tagDesc = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><description>", ResultID, findingid);
            AuthDescTextArea.InnerText = GetData("desc", tagDesc, UserName, DOID);
           
            var tagResultSystem = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><result system>", ResultID, findingid);
            FourthAuthTextBox.Text = GetData("result system", tagResultSystem, UserName, DOID);
            
            
            var tagResultValue = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><result>", ResultID, findingid);
            FifthAuthTextBox.Text = GetData("result", tagResultValue, UserName, DOID);
   
            var tagimplSystem = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><implementation-status system>", ResultID, findingid);
            SixthAuthTextBox.Text = GetData("implementation-status sytem", tagimplSystem, UserName, DOID);

             
            var tagImplValue = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><implementation-status>", ResultID, findingid);
            SeventhAuthTextBox.Text = GetData("implementation-status", tagImplValue, UserName, DOID);


            
            var tagRem = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><remarks>", ResultID, findingid);
            RemarkStepTextArea.InnerText = GetData("remarks", tagRem, UserName, DOID);

            MainDiv.Visible = true;
            AddFindingPanel.Visible = true;
            AddAuthorizedFunctPanel.Visible = false;
        }

        protected void ControlIDDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainDiv.Visible = true;
            AddFindingPanel.Visible = true;
            AddAuthorizedFunctPanel.Visible = true;
            var controlId = ControlIDDropDownList.SelectedValue;
            var temp= "objective-" + controlId;
            SecondAuthTextBox.Text = temp.Replace(" ", "");
            SecondAuthLabel.Enabled = false;

        }

        protected void AddRiskButton_Click(object sender, EventArgs e)
        {
            var findingid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                findingid = GenerateSubjectID();
            }
            else
                findingid = RoleDropDownList.SelectedValue;
            if (Cache["FindingID"] == null)
            {
                Cache.Insert("FindingID", findingid);
            }
            else
            {
                Cache["FindingID"] = findingid;
            }
            Response.Redirect(@"~/PagesSAR/Results/Risks.aspx", false);
        }

        protected void AddThreatIDButton_Click(object sender, EventArgs e)
        {
            var findingid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                findingid = GenerateSubjectID();
            }
            else
                findingid = RoleDropDownList.SelectedValue;
            AddAuxPanel.Visible = true;
            AuxLabel.Text = "Thread IDs";
            var threats = Subjects.Where(x => x.ID == findingid).FirstOrDefault().ThreatIDs;
            threats = threats == null ? new List<ThreatID>() : threats;
            var all = new List<ThreatID> { new ThreatID() };
            all.AddRange(threats);
            AuxDropDownList.DataSource = all;
            AuxDropDownList.DataTextField = "ID";
            AuxDropDownList.DataValueField = "ID";
            AuxDropDownList.DataBind();
        }

        protected void AddPartyButton_Click1(object sender, EventArgs e)
        {
            var obsid = "";
            KeyLabel.Text = "Party Name";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                obsid = GenerateSubjectID();
            }
            else
                obsid = RoleDropDownList.SelectedValue;
            var parties = GetMainParties();
            AddResponsibleRolePanel.Visible = true;
            RespRoleDropDownList.DataSource = parties;
            RespRoleDropDownList.DataTextField = "Name";
            RespRoleDropDownList.DataValueField = "UUID";
            RespRoleDropDownList.DataBind();

            AddFindingPanel.Visible = true;
            MainDiv.Visible = true;
        }

        string GetThreatID(string findingid)
        {
           
            var ids = Subjects.Where(x => x.ID == findingid).FirstOrDefault().ThreatIDs;
            ids = ids == null ? new List<ThreatID>() : ids;

            var id = string.Format("{1}-theat-{0}", ids.Count, findingid);
            return id;
        }

        protected void SaveAuxButton_Click(object sender, EventArgs e)
        {
            var findingid = RoleDropDownList.SelectedValue;
            var threatid = "";
            if (AuxDropDownList.SelectedIndex == 0)
            {
                threatid = GetThreatID(findingid);
            }
            else
                threatid = AuxDropDownList.SelectedValue;

            var tagthreatid = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><threat-id=\"{2}\">", ResultID, findingid, threatid);
            var value = ThreatTextBox.Text;

            var tagsystem = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><threat-id=\"{2}\"><system>", ResultID, findingid, threatid);
            var sys = SystemTextBox.Text;


            var taguri = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><threat-id=\"{2}\"><uri>", ResultID, findingid, threatid);
            var uri = UriTextBox.Text;

            InsertElementToDataBase(DOID, SystemID, threatid, value.GetType(), tagthreatid, "finding id, threat-id", threatid, 1);
            InsertElementToDataBase(DOID, SystemID, "system uri", sys.GetType(), tagsystem, "finding id, threat-id system", sys, 1);
            InsertElementToDataBase(DOID, SystemID, "uri", uri.GetType(), taguri, "finding id, threat-id uri", uri, 1);

            InsertElementToDataBase(DOID, SystemID, "threat", value.GetType(), tagthreatid, "finding id, threat-id threat", value, 1);

        }

        protected void Aux2DropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var roleid = RoleDropDownList.SelectedValue;
            var threatid = AuxDropDownList.SelectedValue;

            var threats = Subjects.Where(x => x.ID == roleid).FirstOrDefault().ThreatIDs;
            var threat = threats.Where(x => x.ID == threatid).FirstOrDefault();
            threat = threat.ID.Length == 0 ? new ThreatID() : threat;
            SystemTextBox.Text = threat.System;
            UriTextBox.Text = threat.URI;
            ThreatTextBox.Text = threat.Value;

            AddAuxPanel.Visible = true;
        }
    }
}