
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Data;
using System.Threading;
using System.Web.Services;
using SqlDataProvider;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Configuration;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using OSCALGenerator.Pages;
using System.Collections.Generic;
using OSCALHelperClasses;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using OConvert;
using System.Net;


namespace OSCALGenerator.PagesSAR
{
    public partial class HomeSAR  : BasePage
    {
        string FileName;
        int DocType = 4;
        string message = "";
        int percent = 0;
        int wordPercent = 0;


        string ProcessingPage;
        protected string OscalSchemaPath { get; set; }

        Guid OrgId;

        string SystemIdentifier;
        int UserId;
        List<string> UserDocuments;
        List<List<string>> DocInfos;
        protected Guid Id;

        private string FilePath;
 
        XmlWriterSettings XmlWriterSettings;
      
        string AppPath;
        bool OK;
        public int Stage { get; set; }

        public string Message { get; set; }

        
        protected new void Page_Load(object sender, EventArgs e)
        {
            try
            {

                DocInfos = new List<List<string>>();

                if (IsPostBack)
                {

                }
                OpenFileButton.Visible = false;

                string appPath = Request.PhysicalApplicationPath;
                OscalSchemaPath = string.Format(@"{0}\Templates\{1}", appPath, SARschema);
                ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
                DAL = new CSharpDAL(ConnString);

                if (Cache["username"] != null)
                {
                    UserName = Cache["username"].ToString();
                    OrgName = GetUserOrgName(UserName);
                    OrgId = GetUserOrgId(UserName);
                    this.OrgNameLabel.Text = OrgName;
                    UserId = GetUserID(UserName);

                    Cache.Insert("orgName", OrgName);
                    Cache.Insert("orgId", OrgId);

                    if (Cache["SystemId"] != null)
                    {
                        SystemID = int.Parse(Cache["SystemId"].ToString());
                        UserDocuments = GetSystemDocFullName(SystemID, 4);
                        DocInfos = GetSystemDocInfo(SystemID, 4);
                    }

                    if (!IsPostBack)
                    {

                        SystemDropDownList.DataSource = GetUserSystem(UserName);
                        SystemDropDownList.DataBind();


                        DocumentDropDownList.DataSource = UserDocuments;
                        DocumentDropDownList.DataBind();

                        if (Cache["doid"] != null)
                        {
                            DOID = int.Parse(Cache["doid"].ToString());
                            FileName = GetDocFullName(DOID);
                            DocumentDropDownList.SelectedValue = FileName;
                        }

                        if (Cache["systemName"] != null)
                        {
                            SystemDropDownList.SelectedValue = Cache["systemName"].ToString();
                            DocDiv.Visible = true;
                        }
                    }
                }
                if (!IsPostBack)
                {

                    SetDOID();
                    FileName = GetDocFullName(DOID);
                    Cache.Insert("doid", DOID);
                    Cache.Insert("userId", UserName);
                }

                if (Cache["selectedFile"] != null && !FileUpload1.HasFile && DocFullNameTextBox.Text.Length == 0)
                {
                    //FileName = Cache["selectedFile"].ToString();

                    //DocumentDropDownList.SelectedValue = FileName;
                }

                AppPath = Request.PhysicalApplicationPath;
            }catch(Exception ex)
            {
                StatusLabel.Text = ex.Message;
                StatusLabel.ForeColor = Color.Red;

            }
        }

        public void SetDOID()
        {

            int rank = -1;
            for (int i = 0; i < DocInfos.Count; i++)
            {
                if (DocInfos[i][2] == FileName)
                {
                    rank = i;
                    break;
                }
            }

            if (rank >= 0)
                DOID = int.Parse(DocInfos[rank][0]);



        }

        void CorrectXMLGenerateXMLDoc()
        {
            FileName = FileName.Replace(" ", "");
            FilePath = string.Format(@"{0}Uploads\{1}.xml", AppPath, FileName);
            XmlWriterSettings = new XmlWriterSettings();
            XmlWriterSettings.Indent = true;
            XmlWriterSettings.IndentChars = "\t";

            Xwriter = XmlWriter.Create(FilePath, XmlWriterSettings);

            X = Xwriter;
            // Xwriter.WriteRaw("<?xml-stylesheet type=\"text/xsl\" href=\"FedRAMP-compliance-worksheet.xsl\"?>\n");

            Xwriter.WriteStartElement("assessment-results", @"http://csrc.nist.gov/ns/oscal/1.0");
            //  Xwriter.WriteAttributeString("xsi", @"http://www.w3.org/2001/XMLSchema-instance");
            Xwriter.WriteAttributeString("uuid", Guid.NewGuid().ToString());
            //  Xwriter.WriteAttributeString("schemaLocation", "xsi", @"http://csrc.nist.gov/ns/oscal/1.0/oscal_ssp_schema.xsd");

            message = string.Format("Start to build the OSCAL Document {0}", FileName);
            percent = 2;
            PrintProgressBar(message, percent, true);

            XMLMetadata();

            message = string.Format("Rendering the OSCAL Document {0}:  sucessfully completed Medata ... ", FileName);
            percent = 20;
            PrintProgressBar(message, percent - wordPercent);
            XMLImportAP();

            message = string.Format("Rendering the OSCAL Document {0}:  sucessfully completed Import ... ", FileName);
            percent = 23;
            PrintProgressBar(message, percent - wordPercent);

            XMLObjectives();

            message = string.Format("Rendering the OSCAL Document {0}:  sucessfully completed Objectives ... ", FileName);
            percent = 40;
            PrintProgressBar(message, percent - wordPercent);

            XMLAssessmentSubject();

            message = string.Format("Rendering the OSCAL Document {0}:  sucessfully completed Assessment Subjects ... ", FileName);
            percent = 60;
            PrintProgressBar(message, percent - wordPercent);

            XMLAssets();

            message = string.Format("Rendering the OSCAL Document {0}:  sucessfully completed Assets ... ", FileName);
            percent = 70;
            PrintProgressBar(message, percent - wordPercent);

            XMLAssessmentActivities();

            message = string.Format("Rendering the OSCAL Document {0}:  sucessfully completed  Assessment Activities ... ", FileName);
            percent = 90;
            PrintProgressBar(message, percent - wordPercent);

            XMLResults();

            message = string.Format("Rendering the OSCAL Document {0}:  sucessfully completed Results ... ", FileName);
            percent = 99;
            PrintProgressBar(message, percent - wordPercent);

            XMLBackMatter();



            Xwriter.WriteEndElement();
            Xwriter.WriteEndDocument();
            Xwriter.Flush();
            Xwriter.Close();

        }

        private void XMLResults()
        {
            var results = GetFullResults();

            foreach (var result in results)
            {
                X.WriteStartElement("results");

                X.WriteAttributeString("uuid", result.UUID);
                X.WriteElementString("title", result.Title);

                X.WriteStartElement("description");
                X.WriteElementString("p", result.Description);
                X.WriteEndElement();

                WriteProps(result.Props);
                WriteAnns(result.Annotations);

                X.WriteElementString("start", result.Start);
                X.WriteElementString("end", result.End);

                WriteFindings(result.Findings);

                X.WriteStartElement("remarks");
                X.WriteElementString("p", result.Remarks);
                X.WriteEndElement();

                X.WriteEndElement();
            }
        }
        
        private void WriteObservations(List<Observation> observations)
        {
            if (observations == null)
                return;
          foreach(var obs in observations)
          {
                X.WriteStartElement("observation");
                X.WriteAttributeString("uuid", obs.UUID);
                X.WriteElementString("title", obs.Title);

                X.WriteStartElement("description");
                X.WriteElementString("p", obs.Description);
                X.WriteEndElement();

                WriteProps(obs.Props);
                WriteAnns(obs.Annotations);

                foreach(var meth in obs.ObservationMethod)
                {
                    X.WriteElementString("observation-method", meth);
                }

                foreach (var meth in obs.ObservationType)
                {
                    X.WriteElementString("observation-type", meth);
                }

                foreach (var meth in obs.Assessors)
                {
                    X.WriteStartElement("assessor");
                    X.WriteAttributeString("party-uuid", meth.XPath);
                    X.WriteString(meth.Value);
                    X.WriteEndElement();
                }

                foreach (var meth in obs.SubjectReferences)
                {
                    X.WriteStartElement("subject-reference");
                    X.WriteAttributeString("uuid-ref", meth.UUIDRef);
                    X.WriteAttributeString("type", meth.Type);
                    X.WriteElementString("title", meth.Value);
                    X.WriteEndElement();
                }

                foreach (var meth in obs.Origins)
                {
                    X.WriteStartElement("origin");
                    X.WriteAttributeString("uuid-ref", meth.UUIDRef);
                    X.WriteAttributeString("type", meth.Type);
                    X.WriteString(meth.Value);
                    X.WriteEndElement();
                }

                WriteRelevantEvidences(obs.RelevantEvidences);

                X.WriteStartElement("remarks");
                X.WriteElementString("p", obs.Remarks);
                X.WriteEndElement();


                X.WriteEndElement();
          }  
        }

        private void WriteRelevantEvidences(List<RelevantEvidence> evidences)
        {
            foreach(var evi in evidences)
            {
                X.WriteStartElement("relevant-evidence");
                X.WriteAttributeString("href", evi.HREF);

                X.WriteStartElement("description");
                X.WriteElementString("p", evi.Description);
                X.WriteEndElement();

                WriteProps(evi.Props);
                WriteAnns(evi.Annotations);

                X.WriteStartElement("remarks");
                X.WriteElementString("p", evi.Remarks);
                X.WriteEndElement();


                X.WriteEndElement(); 
            }
        }
        private void WriteRisks(List<Risk> risks)
        {
            if (risks == null)
                return;
            foreach(var r in risks)
            {
                X.WriteStartElement("risk");
                X.WriteAttributeString("uuid", r.UUID);
                X.WriteElementString("title", r.Title);
               
                X.WriteStartElement("description");
                X.WriteElementString("p", r.Description);
                X.WriteEndElement();

                WriteProps(r.Props);
                WriteAnns(r.Annotations);

                if(r.RiskMetrics != null)
                    foreach(var m in r.RiskMetrics)
                    {
                        X.WriteStartElement("risk-metric");
                        X.WriteAttributeString("name", m.Name);
                        X.WriteAttributeString("class", m.Class);
                        X.WriteAttributeString("system", m.System);
                        X.WriteString(m.Value);
                        X.WriteEndElement();
                    }

                X.WriteStartElement("risk-statement");
                X.WriteElementString("p", r.Remarks);
                X.WriteEndElement();

                X.WriteElementString("risk-status", r.RiskStatus);

                


                X.WriteEndElement();
            }
        }
        private void WriteFindings(List<Finding> findings)
        {
            foreach(var F in findings)
            {
                X.WriteStartElement("finding");

                X.WriteAttributeString("uuid", F.UUID);
                X.WriteElementString("title", F.Title);

                X.WriteStartElement("description");
                X.WriteElementString("p", F.Description);
                X.WriteEndElement();

                WriteProps(F.Props);
                WriteAnns(F.Annotations);
                X.WriteElementString("date-time-stamp", F.DateTimeStamp);

                if (F.ObjectiveID != null && F.ObjectiveID.Length > 0)
                {
                    X.WriteStartElement("objective-status");
                    X.WriteAttributeString("objective-id", F.ObjectiveID);
                    X.WriteAttributeString("control-id", F.ControlID);
                    X.WriteElementString("title", F.ObjectiveTitle);

                    X.WriteStartElement("description");
                    X.WriteElementString("p", F.ObjectiveStatusDesc);
                    X.WriteEndElement();

                    X.WriteStartElement("result");
                    X.WriteAttributeString("system", F.ResultSystem);
                    X.WriteString(F.ResultValue);
                    X.WriteEndElement();

                    X.WriteStartElement("implementation-status");
                    X.WriteAttributeString("system", F.ImplementationStatusSystem);
                    X.WriteString(F.ImplementationStatusValue);
                    X.WriteEndElement();

                    X.WriteStartElement("remarks");
                    X.WriteElementString("p", F.ObjectiveStatusRemarks);
                    X.WriteEndElement();

                    X.WriteEndElement();
                }

                X.WriteElementString("implementation-statement-uuid", F.ImplementationStatementUUIDs);

                WriteObservations(F.Observations);

                foreach(var threat in F.ThreatIDs)
                {
                    X.WriteStartElement("threat-id");
                    X.WriteAttributeString("system", threat.System);
                    X.WriteAttributeString("uri", threat.URI);
                    X.WriteString(threat.Value);
                    X.WriteEndElement();

                }

                WriteRisks(F.Risks);

                foreach (var part in F.PartyUUIDS)
                    X.WriteElementString("party-uuid", part);

                X.WriteStartElement("remarks");
                X.WriteElementString("p", F.Remarks);
                X.WriteEndElement();

                X.WriteEndElement();
            }
        }

        private void XMLAssessmentActivities()
        {
            X.WriteStartElement("assessment-activities");
            var testMethods = GetTestMethods();
            foreach (var method in testMethods)
            {
                X.WriteStartElement("test-method");
                X.WriteAttributeString("uuid", method.UUID);
                X.WriteElementString("title", method.Title);

                X.WriteStartElement("description");
                X.WriteElementString("p", method.Description);
                X.WriteEndElement();

                WriteProps(method.Props);
                WriteAnns(method.Annotations);
                WriteLinks(method.Links);

                foreach (var step in method.TestSteps)
                {
                    X.WriteStartElement("test-step");
                    X.WriteAttributeString("uuid", step.UUID);
                    X.WriteElementString("sequence", step.Sequence.ToString());

                    X.WriteStartElement("description");
                    X.WriteElementString("p", step.Description);
                    X.WriteEndElement();

                    foreach (var r in step.RoleIds)
                        X.WriteElementString("role-id", r);
                    foreach (var p in step.PartyUUIDs)
                        X.WriteElementString("party-uuid", p);

                    X.WriteElementString("compare-to", step.CompareTo); ;

                    X.WriteStartElement("remarks");
                    X.WriteElementString("p", step.Remarks);
                    X.WriteEndElement();

                    X.WriteEndElement();
                }

                X.WriteElementString("compare-to", method.CompareTo); ;

                X.WriteStartElement("remarks");
                X.WriteElementString("p", method.Remarks);
                X.WriteEndElement();





                X.WriteEndElement();
            }

            var schedules = GetSchedules();
            for (int i = 0; i < Math.Min(schedules.Count, 1); i++)
            {
                var schedule = schedules[0];
                X.WriteStartElement("schedule");
                X.WriteAttributeString("uuid", schedule.UUID);
                foreach (var task in schedule.Tasks)
                {
                    X.WriteStartElement("task");
                    X.WriteAttributeString("uuid", task.UUID);
                    X.WriteElementString("title", task.Title);

                    X.WriteStartElement("description");
                    X.WriteElementString("p", task.Description);
                    X.WriteEndElement();

                    WriteProps(task.Props);
                    WriteAnns(task.Annotations);

                    X.WriteElementString("start", task.Start);
                    X.WriteElementString("end", task.End);

                    foreach (var act in task.ActivityUUIDs)
                    {
                        X.WriteElementString("activity-uuid", act);
                    }

                    foreach (var role in task.RoleIds)
                    {
                        X.WriteElementString("role-id", role);
                    }

                    foreach (var part in task.PartyUUIDs)
                    {
                        X.WriteElementString("party-uuid", part);
                    }

                    foreach (var loc in task.LocationUUIDs)
                    {
                        X.WriteElementString("location-uuid", loc);
                    }

                    X.WriteElementString("compare-to", task.CompareTo);

                    X.WriteStartElement("remarks");
                    X.WriteElementString("p", task.Remarks);
                    X.WriteEndElement();

                    X.WriteEndElement();
                }
            }

            X.WriteEndElement();


            X.WriteEndElement();


        }


        private void WriteComponents(List<Component> Subjects)
        {
            foreach (var x in Subjects)
            {
                Xwriter.WriteStartElement("component");
                Xwriter.WriteAttributeString("uuid", x.UUID);
                Xwriter.WriteAttributeString("component-type", x.ComponentType);
                Xwriter.WriteElementString("title", x.Title);
                Xwriter.WriteStartElement("description");
                Xwriter.WriteElementString("p", x.Description);
                Xwriter.WriteEndElement();

                if (x.Props != null)
                    foreach (var prop in x.Props)
                    {
                        Xwriter.WriteStartElement("prop");
                        Xwriter.WriteAttributeString("name", prop.Name);
                        Xwriter.WriteAttributeString("id", prop.ID);
                        Xwriter.WriteAttributeString("ns", prop.NS);
                        Xwriter.WriteAttributeString("class", prop.Class);
                        Xwriter.WriteValue(prop.Value);
                        Xwriter.WriteEndElement();

                    }

                if (x.Annotations != null)
                    foreach (var ann in x.Annotations)
                    {
                        Xwriter.WriteStartElement("annotation");
                        Xwriter.WriteAttributeString("name", ann.Name);
                        Xwriter.WriteAttributeString("id", ann.ID);
                        Xwriter.WriteAttributeString("ns", ann.NS);
                        Xwriter.WriteAttributeString("value", ann.Value);

                        Xwriter.WriteStartElement("remarks");
                        Xwriter.WriteElementString("p", ann.Remarks);
                        Xwriter.WriteEndElement();

                        Xwriter.WriteEndElement();
                    }
                if (x.Links != null)
                    foreach (var link in x.Links)
                    {
                        Xwriter.WriteStartElement("link");
                        Xwriter.WriteAttributeString("href", link.HRef);
                        Xwriter.WriteAttributeString("rel", link.Rel);
                        Xwriter.WriteAttributeString("media-type", link.MediaType);
                        Xwriter.WriteValue(link.MarkUpLine);
                        Xwriter.WriteEndElement();
                    }

                Xwriter.WriteStartElement("status");
                Xwriter.WriteAttributeString("state", x.State);

                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", x.StateRemarks);
                Xwriter.WriteEndElement();

                Xwriter.WriteEndElement();

                foreach (var y in x.ResponsibleRoles)
                {
                    Xwriter.WriteStartElement("responsible-role");
                    Xwriter.WriteAttributeString("role-id", y.RoleID);
                    if (y.Props != null)
                        foreach (var prop in y.Props)
                        {
                            Xwriter.WriteStartElement("prop");
                            Xwriter.WriteAttributeString("name", prop.Name);
                            Xwriter.WriteAttributeString("id", prop.ID);
                            Xwriter.WriteAttributeString("ns", prop.NS);
                            Xwriter.WriteAttributeString("class", prop.Class);
                            Xwriter.WriteValue(prop.Value);
                            Xwriter.WriteEndElement();

                        }

                    if (y.Annotations != null)
                        foreach (var ann in y.Annotations)
                        {
                            Xwriter.WriteStartElement("annotation");
                            Xwriter.WriteAttributeString("name", ann.Name);
                            Xwriter.WriteAttributeString("id", ann.ID);
                            Xwriter.WriteAttributeString("ns", ann.NS);
                            Xwriter.WriteAttributeString("value", ann.Value);

                            Xwriter.WriteStartElement("remarks");
                            Xwriter.WriteElementString("p", ann.Remarks);
                            Xwriter.WriteEndElement();


                            Xwriter.WriteEndElement();
                        }
                    if (y.Links != null)
                        foreach (var link in y.Links)
                        {
                            Xwriter.WriteStartElement("link");
                            Xwriter.WriteAttributeString("href", link.HRef);
                            Xwriter.WriteAttributeString("rel", link.Rel);
                            Xwriter.WriteAttributeString("media-type", link.MediaType);
                            Xwriter.WriteValue(link.MarkUpLine);
                            Xwriter.WriteEndElement();
                        }

                    Xwriter.WriteElementString("party-uuid", y.PartyUUID);


                    Xwriter.WriteStartElement("remarks");
                    Xwriter.WriteElementString("p", y.Remarks);
                    Xwriter.WriteEndElement();


                    Xwriter.WriteEndElement();
                }


                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", x.Remarks);
                Xwriter.WriteEndElement();


                Xwriter.WriteEndElement();
            }
        }

        private void XMLAssessmentSubject()
        {
            Xwriter.WriteStartElement("assessment-subjects");

            var Subjects = GetSubjects("include");
            foreach (var x in Subjects)
            {
                Xwriter.WriteStartElement("include-subject");
                Xwriter.WriteAttributeString("name", x.Name);
                Xwriter.WriteAttributeString("class", x.Class);

                Xwriter.WriteStartElement("description");
                Xwriter.WriteElementString("p", x.Description);
                Xwriter.WriteEndElement();

                if (x.Props != null)
                    foreach (var prop in x.Props)
                    {
                        Xwriter.WriteStartElement("prop");
                        Xwriter.WriteAttributeString("name", prop.Name);
                        Xwriter.WriteAttributeString("uuid", prop.ID);
                        Xwriter.WriteAttributeString("ns", prop.NS);
                        Xwriter.WriteAttributeString("class", prop.Class);
                        Xwriter.WriteValue(prop.Value);
                        Xwriter.WriteEndElement();

                    }

                if (x.Annotations != null)
                    foreach (var ann in x.Annotations)
                    {
                        Xwriter.WriteStartElement("annotation");
                        Xwriter.WriteAttributeString("name", ann.Name);
                        Xwriter.WriteAttributeString("id", ann.ID);
                        Xwriter.WriteAttributeString("ns", ann.NS);
                        Xwriter.WriteAttributeString("value", ann.Value);


                        Xwriter.WriteStartElement("remarks");
                        Xwriter.WriteElementString("p", ann.Remarks);
                        Xwriter.WriteEndElement();



                        Xwriter.WriteEndElement();
                    }
                if (x.Links != null)
                    foreach (var link in x.Links)
                    {
                        Xwriter.WriteStartElement("link");
                        Xwriter.WriteAttributeString("href", link.HRef);
                        Xwriter.WriteAttributeString("rel", link.Rel);
                        Xwriter.WriteAttributeString("media-type", link.MediaType);
                        Xwriter.WriteValue(link.MarkUpLine);
                        Xwriter.WriteEndElement();
                    }

                Xwriter.WriteElementString("all", x.All);

                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", x.Remarks);
                Xwriter.WriteEndElement();


                Xwriter.WriteEndElement();
            }

            var Subjects2 = GetSubjects("exclude");
            foreach (var x in Subjects2)
            {
                Xwriter.WriteStartElement("exclude-subject");
                Xwriter.WriteAttributeString("name", x.Name);
                Xwriter.WriteAttributeString("class", x.Class);

                Xwriter.WriteStartElement("description");
                Xwriter.WriteElementString("p", x.Description);
                Xwriter.WriteEndElement();


                if (x.Props != null)
                    foreach (var prop in x.Props)
                    {
                        Xwriter.WriteStartElement("prop");
                        Xwriter.WriteAttributeString("name", prop.Name);
                        Xwriter.WriteAttributeString("id", prop.ID);
                        Xwriter.WriteAttributeString("ns", prop.NS);
                        Xwriter.WriteAttributeString("class", prop.Class);
                        Xwriter.WriteValue(prop.Value);
                        Xwriter.WriteEndElement();

                    }

                if (x.Annotations != null)
                    foreach (var ann in x.Annotations)
                    {
                        Xwriter.WriteStartElement("annotation");
                        Xwriter.WriteAttributeString("name", ann.Name);
                        Xwriter.WriteAttributeString("id", ann.ID);
                        Xwriter.WriteAttributeString("ns", ann.NS);
                        Xwriter.WriteAttributeString("value", ann.Value);

                        Xwriter.WriteStartElement("remarks");
                        Xwriter.WriteElementString("p", ann.Remarks);
                        Xwriter.WriteEndElement();

                        Xwriter.WriteEndElement();
                    }
                //if (x.Links != null)
                //    foreach (var link in x.Links)
                //    {
                //        Xwriter.WriteStartElement("link");
                //        Xwriter.WriteAttributeString("href", link.HRef);
                //        Xwriter.WriteAttributeString("rel", link.Rel);
                //        Xwriter.WriteAttributeString("media-type", link.MediaType);
                //        Xwriter.WriteValue(link.MarkUpLine);
                //        Xwriter.WriteEndElement();
                //    }

                Xwriter.WriteElementString("all", x.All);


                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", x.Remarks);
                Xwriter.WriteEndElement();

                Xwriter.WriteEndElement();
            }


            Xwriter.WriteStartElement("local-definitions");

            var Subjects3 = GetComponents();
            foreach (var x in Subjects3)
            {
                Xwriter.WriteStartElement("component");
                Xwriter.WriteAttributeString("uuid", x.UUID);
                Xwriter.WriteAttributeString("component-type", x.ComponentType);
                Xwriter.WriteElementString("title", x.Title);

                Xwriter.WriteStartElement("description");
                Xwriter.WriteElementString("p", x.Description);
                Xwriter.WriteEndElement();

                if (x.Props != null)
                    foreach (var prop in x.Props)
                    {
                        Xwriter.WriteStartElement("prop");
                        Xwriter.WriteAttributeString("name", prop.Name);
                        Xwriter.WriteAttributeString("id", prop.ID);
                        Xwriter.WriteAttributeString("ns", prop.NS);
                        Xwriter.WriteAttributeString("class", prop.Class);
                        Xwriter.WriteValue(prop.Value);
                        Xwriter.WriteEndElement();

                    }

                if (x.Annotations != null)
                    foreach (var ann in x.Annotations)
                    {
                        Xwriter.WriteStartElement("annotation");
                        Xwriter.WriteAttributeString("name", ann.Name);
                        Xwriter.WriteAttributeString("id", ann.ID);
                        Xwriter.WriteAttributeString("ns", ann.NS);
                        Xwriter.WriteAttributeString("value", ann.Value);

                        Xwriter.WriteStartElement("remarks");
                        Xwriter.WriteElementString("p", ann.Remarks);
                        Xwriter.WriteEndElement();


                        Xwriter.WriteEndElement();
                    }
                if (x.Links != null)
                    foreach (var link in x.Links)
                    {
                        Xwriter.WriteStartElement("link");
                        Xwriter.WriteAttributeString("href", link.HRef);
                        Xwriter.WriteAttributeString("rel", link.Rel);
                        Xwriter.WriteAttributeString("media-type", link.MediaType);
                        Xwriter.WriteValue(link.MarkUpLine);
                        Xwriter.WriteEndElement();
                    }

                Xwriter.WriteStartElement("status");
                Xwriter.WriteAttributeString("state", x.State);

                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", x.StateRemarks);
                Xwriter.WriteEndElement();


                Xwriter.WriteEndElement();

                foreach (var y in x.ResponsibleRoles)
                {
                    Xwriter.WriteStartElement("responsible-role");
                    Xwriter.WriteAttributeString("role-id", y.RoleID);
                    if (y.Props != null)
                        foreach (var prop in y.Props)
                        {
                            Xwriter.WriteStartElement("prop");
                            Xwriter.WriteAttributeString("name", prop.Name);
                            Xwriter.WriteAttributeString("uuid", prop.ID);
                            Xwriter.WriteAttributeString("ns", prop.NS);
                            Xwriter.WriteAttributeString("class", prop.Class);
                            Xwriter.WriteValue(prop.Value);
                            Xwriter.WriteEndElement();

                        }

                    if (y.Annotations != null)
                        foreach (var ann in y.Annotations)
                        {
                            Xwriter.WriteStartElement("annotation");
                            Xwriter.WriteAttributeString("name", ann.Name);
                            Xwriter.WriteAttributeString("id", ann.ID);
                            Xwriter.WriteAttributeString("ns", ann.NS);
                            Xwriter.WriteAttributeString("value", ann.Value);

                            Xwriter.WriteStartElement("remarks");
                            Xwriter.WriteElementString("p", ann.Remarks);
                            Xwriter.WriteEndElement();


                            Xwriter.WriteEndElement();
                        }
                    if (y.Links != null)
                        foreach (var link in y.Links)
                        {
                            Xwriter.WriteStartElement("link");
                            Xwriter.WriteAttributeString("href", link.HRef);
                            Xwriter.WriteAttributeString("rel", link.Rel);
                            Xwriter.WriteAttributeString("media-type", link.MediaType);
                            Xwriter.WriteValue(link.MarkUpLine);
                            Xwriter.WriteEndElement();
                        }

                    Xwriter.WriteElementString("party-uuid", y.PartyUUID);

                    Xwriter.WriteStartElement("remarks");
                    Xwriter.WriteElementString("p", y.Remarks);
                    Xwriter.WriteEndElement();

                    Xwriter.WriteEndElement();
                }


                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", x.Remarks);
                Xwriter.WriteEndElement();

                Xwriter.WriteEndElement();
            }

            var Subjects4 = GetInventoryItems();
            var X = Xwriter;
            foreach (var e in Subjects4)
            {
                X.WriteStartElement("inventory-item");
                X.WriteAttributeString("uuid", e.UUID);
                X.WriteAttributeString("asset-id", e.AssetID);
                Xwriter.WriteStartElement("description");
                Xwriter.WriteElementString("p", e.Description);
                Xwriter.WriteEndElement();


                if (e.Props != null)
                    foreach (var prop in e.Props)
                    {
                        Xwriter.WriteStartElement("prop");
                        Xwriter.WriteAttributeString("name", prop.Name);
                        Xwriter.WriteAttributeString("id", prop.ID);
                        Xwriter.WriteAttributeString("ns", prop.NS);
                        Xwriter.WriteAttributeString("class", prop.Class);
                        Xwriter.WriteValue(prop.Value);
                        Xwriter.WriteEndElement();

                    }

                if (e.Annotations != null)
                    foreach (var ann in e.Annotations)
                    {
                        Xwriter.WriteStartElement("annotation");
                        Xwriter.WriteAttributeString("name", ann.Name);
                        Xwriter.WriteAttributeString("id", ann.ID);
                        Xwriter.WriteAttributeString("ns", ann.NS);
                        Xwriter.WriteAttributeString("value", ann.Value);

                        Xwriter.WriteStartElement("remarks");
                        Xwriter.WriteElementString("p", ann.Remarks);
                        Xwriter.WriteEndElement();



                        Xwriter.WriteEndElement();
                    }
                if (e.Links != null)
                    foreach (var link in e.Links)
                    {
                        Xwriter.WriteStartElement("link");
                        Xwriter.WriteAttributeString("href", link.HRef);
                        Xwriter.WriteAttributeString("rel", link.Rel);
                        Xwriter.WriteAttributeString("media-type", link.MediaType);
                        Xwriter.WriteValue(link.MarkUpLine);
                        Xwriter.WriteEndElement();
                    }

                WriteResponsibleParties(e.ResponsibleRoles);

                foreach (var c in e.implementedComponents)
                {
                    X.WriteStartElement("implemented-component");
                    X.WriteAttributeString("component-id", c.UUID);
                    X.WriteAttributeString("use", c.Use);
                    WriteProps(c.Props);
                    WriteAnns(c.Annotations);
                    WriteLinks(c.Links);
                    WriteResponsibleParties(c.ResponsibleRoles);

                    X.WriteStartElement("remarks");
                    X.WriteElementString("p", c.Remarks);
                    X.WriteEndElement();

                    X.WriteEndElement();
                }

                X.WriteEndElement();
            }

            var Subjects5 = GetSAPUsers();
            foreach (var u in Subjects5)
            {
                X.WriteStartElement("user");
                X.WriteAttributeString("uuid", u.UUID.ToString());
                X.WriteElementString("title", u.Title);
                X.WriteElementString("short-name", u.ShortName);

                X.WriteStartElement("description");
                X.WriteElementString("p", u.Description);
                X.WriteEndElement();


                WriteProps(u.Props);
                WriteAnns(u.Annotations);
                WriteLinks(u.Links);
                foreach (var r in u.Roles)
                {
                    X.WriteElementString("role-id", r.RoleID);
                }


                foreach (var w in u.AuthorizedPrivileges)
                {
                    X.WriteStartElement("authorized-privilege");
                    X.WriteElementString("title", w.Title);

                    Xwriter.WriteStartElement("description");
                    Xwriter.WriteElementString("p", w.Description);
                    Xwriter.WriteEndElement();


                    X.WriteElementString("function-performed", w.FunctionPerformed);
                    X.WriteEndElement();
                }
                X.WriteStartElement("remarks");
                X.WriteElementString("p", u.Remarks);
                X.WriteEndElement();





                X.WriteEndElement();

            }

            var Remark = GetData("remarks", "<assessment-subject><local-definitions><remarks>", UserName, DOID);

            X.WriteStartElement("remarks");
            X.WriteElementString("p", Remark);
            X.WriteEndElement();

            Xwriter.WriteEndElement();



            Xwriter.WriteEndElement();

        }
        private void XMLAssets()
        {
            var X = Xwriter;
            X.WriteStartElement("assets");
            X.WriteStartElement("tools");

            var tagCore = "<assets><tools>";
            var Subjects = GetComponents(tagCore);
            WriteComponents(Subjects);

            X.WriteEndElement();

            var tagTitle = string.Format("<assets><origination><title>");


            var tagDesc = string.Format("<assets><origination><description>");

            var tit = GetData("title", tagTitle, UserName, DOID);


            var desc = GetData("desc", tagDesc, UserName, DOID);



            X.WriteStartElement("origination");
            X.WriteElementString("title", tit);

            X.WriteStartElement("description");
            X.WriteElementString("p", desc);
            X.WriteEndElement();

            var Props = GetProps("", "origination");
            WriteProps(Props);






            X.WriteEndElement();
            X.WriteEndElement();

        }

        protected void WriteResponsibleParties(List<ResponsibleParty> P)
        {
            foreach (var y in P)
            {
                Xwriter.WriteStartElement("responsible-party");
                Xwriter.WriteAttributeString("role-id", y.RoleID);
                if (y.Props != null)
                    foreach (var prop in y.Props)
                    {
                        Xwriter.WriteStartElement("prop");
                        Xwriter.WriteAttributeString("name", prop.Name);
                        Xwriter.WriteAttributeString("id", prop.ID);
                        Xwriter.WriteAttributeString("ns", prop.NS);
                        Xwriter.WriteAttributeString("class", prop.Class);
                        Xwriter.WriteValue(prop.Value);
                        Xwriter.WriteEndElement();

                    }

                if (y.Annotations != null)
                    foreach (var ann in y.Annotations)
                    {
                        Xwriter.WriteStartElement("annotation");
                        Xwriter.WriteAttributeString("name", ann.Name);
                        Xwriter.WriteAttributeString("id", ann.ID);
                        Xwriter.WriteAttributeString("ns", ann.NS);
                        Xwriter.WriteAttributeString("value", ann.Value);

                        Xwriter.WriteStartElement("remarks");
                        Xwriter.WriteElementString("p", ann.Remarks);
                        Xwriter.WriteEndElement();

                        Xwriter.WriteEndElement();
                    }
                if (y.Links != null)
                    foreach (var link in y.Links)
                    {
                        Xwriter.WriteStartElement("link");
                        Xwriter.WriteAttributeString("href", link.HRef);
                        Xwriter.WriteAttributeString("rel", link.Rel);
                        Xwriter.WriteAttributeString("media-type", link.MediaType);
                        Xwriter.WriteValue(link.MarkUpLine);
                        Xwriter.WriteEndElement();
                    }

                Xwriter.WriteElementString("party-uuid", y.PartyUUID);

                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", y.Remarks);
                Xwriter.WriteEndElement();


                Xwriter.WriteEndElement();
            }
        }


        protected void WriteProps(List<Prop> props)
        {
            if (props != null)
                foreach (var prop in props)
                {
                    Xwriter.WriteStartElement("prop");
                    Xwriter.WriteAttributeString("name", prop.Name);
                    Xwriter.WriteAttributeString("id", prop.ID);
                    Xwriter.WriteAttributeString("ns", prop.NS);
                    Xwriter.WriteAttributeString("class", prop.Class);
                    Xwriter.WriteValue(prop.Value);
                    Xwriter.WriteEndElement();

                }
        }

        protected void WriteAnns(List<Annotation> anns)
        {
            if (anns != null)
                foreach (var ann in anns)
                {
                    Xwriter.WriteStartElement("annotation");
                    Xwriter.WriteAttributeString("name", ann.Name);
                    Xwriter.WriteAttributeString("id", ann.ID);
                    Xwriter.WriteAttributeString("ns", ann.NS);
                    Xwriter.WriteAttributeString("value", ann.Value);

                    Xwriter.WriteStartElement("remarks");
                    Xwriter.WriteElementString("p", ann.Remarks);
                    Xwriter.WriteEndElement();

                    Xwriter.WriteEndElement();
                }
        }

        protected void WriteLinks(List<Link> links)
        {
            if (links != null)
                foreach (var link in links)
                {
                    Xwriter.WriteStartElement("link");
                    Xwriter.WriteAttributeString("href", link.HRef);
                    Xwriter.WriteAttributeString("rel", link.Rel);
                    Xwriter.WriteAttributeString("media-type", link.MediaType);
                    Xwriter.WriteValue(link.MarkUpLine);
                    Xwriter.WriteEndElement();
                }
        }

        protected void XwriteAddress(DocAddress party)
        {
            Xwriter.WriteStartElement("address");
            Xwriter.WriteAttributeString("type", party.AddressType);
            Xwriter.WriteElementString("addr-line", party.AddressLine1);
            Xwriter.WriteElementString("addr-line", party.AddressLine2);
            Xwriter.WriteElementString("city", party.City);
            Xwriter.WriteElementString("state", party.State);
            Xwriter.WriteElementString("postal-code", party.PostalCode);
            Xwriter.WriteElementString("country", party.Country);
            Xwriter.WriteEndElement();
        }


        private void XMLImportAP()
        {
            Xwriter.WriteStartElement("import-ap");
            var href = GetData("href", "<import-ap href>", UserName, DOID);
            Xwriter.WriteAttributeString("href", href);
            var remarks = GetData("remarks", "<import-ap href><remarks>", UserName, DOID);
            Xwriter.WriteStartElement("remarks");
            Xwriter.WriteElementString("p", remarks);
            Xwriter.WriteEndElement();
            Xwriter.WriteEndElement();

        }


        private void XMLObjectives()
        {
            Xwriter.WriteStartElement("objectives");

            var desc = GetData("description", "<objectives><description>", UserName, DOID);
            Xwriter.WriteStartElement("description");
            Xwriter.WriteElementString("p", desc);
            Xwriter.WriteEndElement();

            var Props = GetProps("", "objectives description");
            foreach (var prop in Props)
            {

                Xwriter.WriteStartElement("prop");
                Xwriter.WriteAttributeString("name", prop.Name);
                Xwriter.WriteAttributeString("id", prop.ID);
                Xwriter.WriteAttributeString("ns", prop.NS);
                Xwriter.WriteAttributeString("class", prop.Class);
                Xwriter.WriteValue(prop.Value);
                Xwriter.WriteEndElement();

            }
            var Anns = GetAnnotations("", "objectives description");

            foreach (var ann in Anns)
            {
                Xwriter.WriteStartElement("annotation");
                Xwriter.WriteAttributeString("name", ann.Name);
                Xwriter.WriteAttributeString("id", ann.ID);
                Xwriter.WriteAttributeString("ns", ann.NS);
                Xwriter.WriteAttributeString("value", ann.Value);
                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", ann.Remarks);
                Xwriter.WriteEndElement();
                Xwriter.WriteEndElement();
            }
            ///
            Xwriter.WriteStartElement("controls");

            var descC = GetData("description", "<objectives><controls><description>", UserName, DOID);
            Xwriter.WriteStartElement("description");
            Xwriter.WriteElementString("p", descC);
            Xwriter.WriteEndElement();

            var all = GetData("all", "<objectives><controls><all>", UserName, DOID);

            var rawTag = string.Format("<objectives><{1}=\"{0}\">", "", "controls");

            var Props2 = GetProps("", "controls", rawTag);
            foreach (var prop in Props2)
            {

                Xwriter.WriteStartElement("prop");
                Xwriter.WriteAttributeString("name", prop.Name);
                Xwriter.WriteAttributeString("id", prop.ID);
                Xwriter.WriteAttributeString("ns", prop.NS);
                Xwriter.WriteAttributeString("class", prop.Class);
                Xwriter.WriteValue(prop.Value);
                Xwriter.WriteEndElement();

            }

            var Anns2 = GetAnnotations("", "controls", rawTag);
            foreach (var ann in Anns2)
            {
                Xwriter.WriteStartElement("annotation");
                Xwriter.WriteAttributeString("name", ann.Name);
                Xwriter.WriteAttributeString("id", ann.ID);
                Xwriter.WriteAttributeString("ns", ann.NS);
                Xwriter.WriteAttributeString("value", ann.Value);
                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", ann.Remarks);
                Xwriter.WriteEndElement();
                Xwriter.WriteEndElement();
            }

            Xwriter.WriteElementString("all", all);


            var IncludedControls = GetControlList("include-control control-id");
            var ExcludedControls = GetControlList("exclude-control control-id");
            foreach (var x in IncludedControls)
            {
                Xwriter.WriteStartElement("include-control");
                Xwriter.WriteAttributeString("control-id", x[0].Replace(" ", "").Replace(":", ""));

                Xwriter.WriteEndElement();
            }

            foreach (var x in ExcludedControls)
            {
                Xwriter.WriteStartElement("exclude-control");
                Xwriter.WriteAttributeString("control-id", x[0].Replace(" ", "").Replace(":", ""));

                Xwriter.WriteEndElement();
            }
            Xwriter.WriteEndElement();

            ///

            var descEC = GetData("description", "<objectives><control-objectives><description>", UserName, DOID);

            var allOC = GetData("all", "<objectives><control-objectives><all>", UserName, DOID);

            var rawTagOC = string.Format("<objectives><{1}=\"{0}\">", "", "control-objectives");

            var Props3 = GetProps("", "control-objectives", rawTagOC);


            var Anns3 = GetAnnotations("", "control-objectives", rawTagOC);

            Xwriter.WriteStartElement("control-objectives");

            Xwriter.WriteStartElement("description");
            Xwriter.WriteElementString("p", descEC);
            Xwriter.WriteEndElement();


            foreach (var prop in Props3)
            {
                Xwriter.WriteStartElement("prop");
                Xwriter.WriteAttributeString("name", prop.Name);
                Xwriter.WriteAttributeString("id", prop.ID);
                Xwriter.WriteAttributeString("ns", prop.NS);
                Xwriter.WriteAttributeString("class", prop.Class);
                Xwriter.WriteValue(prop.Value);
                Xwriter.WriteEndElement();
            }
            foreach (var ann in Anns3)
            {
                Xwriter.WriteStartElement("annotation");
                Xwriter.WriteAttributeString("name", ann.Name);
                Xwriter.WriteAttributeString("id", ann.ID);
                Xwriter.WriteAttributeString("ns", ann.NS);
                Xwriter.WriteAttributeString("value", ann.Value);
                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", ann.Remarks);
                Xwriter.WriteEndElement();
                Xwriter.WriteEndElement();
            }
            Xwriter.WriteElementString("all", allOC);
            var IncludedControls2 = GetControlList("include-objective objective-id");
            var ExcludedControls2 = GetControlList("exclude-objective objective-id");

            foreach (var x in IncludedControls2)
            {
                Xwriter.WriteStartElement("include-objective");
                Xwriter.WriteAttributeString("objective-id", x[2]);
                Xwriter.WriteValue(x[0]);
                Xwriter.WriteEndElement();
            }

            foreach (var x in ExcludedControls2)
            {
                Xwriter.WriteStartElement("exclude-objective");
                Xwriter.WriteAttributeString("objective-id", x[2]);
                Xwriter.WriteValue(x[0]);
                Xwriter.WriteEndElement();
            }
            Xwriter.WriteEndElement();
            ///

            WriteObjective();

            var meths = GetMethods();

            WriteMethods(meths);

            Xwriter.WriteEndElement();
        }

        private void WriteMethods(List<Method> Methods)
        {
            foreach (var meth in Methods)
            {
                X.WriteStartElement("method");
                X.WriteAttributeString("uuid", meth.UUID);

                X.WriteStartElement("description");
                X.WriteElementString("p", meth.Description);
                X.WriteEndElement();

                if (meth.Props != null)
                    WriteProps(meth.Props);
                if (meth.Annotations != null)
                    WriteAnns(meth.Annotations);

                X.WriteStartElement("part");
                X.WriteAttributeString("id", "part-id");


                X.WriteAttributeString("name", meth.PartName);
                X.WriteAttributeString("ns", meth.PartNS);
                X.WriteAttributeString("class", meth.PartClass);
                X.WriteElementString("title", meth.PartTitle);

                var rawPartTag = string.Format("<objectives><part=\"{0}\">", meth.ID);
                var PartAnns = GetAnnotations(meth.ID, "part", rawPartTag);
                var PartProps = GetProps(meth.ID, "part", rawPartTag);
                WriteProps(PartProps);
                WriteAnns(PartAnns);

                X.WriteElementString("p", meth.PartDescription);


                var parts = GetMethodParts(meth.ID);
                int i = 0;
                foreach (var p in parts)
                {
                    X.WriteStartElement("part");
                    X.WriteAttributeString("name", string.Format("subpart-{0}", i));
                    X.WriteElementString("title", p);
                    X.WriteEndElement();
                    i++;
                }

                X.WriteEndElement();


                X.WriteStartElement("remarks");
                X.WriteElementString("p", meth.Remarks);
                X.WriteEndElement();


                X.WriteEndElement();
            }
        }

        private void WriteObjective()
        {
            //Write Objective
            X.WriteStartElement("objective");
            X.WriteAttributeString("id", "objective");

            var desc = GetData("description", "<objectives><objective><description>", UserName, DOID);
            var ctrId = GetData("control-id", "<objectives><objective><control-id>", UserName, DOID);
            X.WriteAttributeString("control-id", ctrId);

            X.WriteStartElement("description");
            X.WriteElementString("p", desc);
            X.WriteEndElement();
            var rawTag = string.Format("<objectives><objective=\"\">");
            var Props = GetProps("", "objective", rawTag);
            var Anns = GetAnnotations("", "objective", rawTag);
            WriteProps(Props);
            WriteAnns(Anns);


            X.WriteStartElement("part");
            X.WriteAttributeString("id", "part-id");

            var name = GetData("name", "<objectives><objective><part><name>", UserName, DOID);
            var cal = GetData("class", "<objectives><objective><part><class>", UserName, DOID);
            var ns = GetData("ns", "<objectives><objective><part><ns>", UserName, DOID);
            var partDesc = GetData("desc", "<objectives><objective><part><description>", UserName, DOID);
            var title = GetData("title", "<objectives><objective><part><title>", UserName, DOID);

            X.WriteAttributeString("name", name);
            X.WriteAttributeString("ns", ns);
            X.WriteAttributeString("class", cal);
            X.WriteElementString("title", title);

            var rawPartTag = string.Format("<objectives><objective=\"\"><part=\"\">");
            var PartLinks = GetLinks("", "part", rawPartTag);
            var PartProps = GetProps("", "part", rawPartTag);
            WriteProps(PartProps);

            //         X.WriteStartElement("description");
            X.WriteElementString("p", partDesc);
            //        X.WriteEndElement();

            var parts = GetParts();
            int i = 0;
            foreach (var p in parts)
            {
                X.WriteStartElement("part");
                X.WriteAttributeString("name", string.Format("subpart-{0}", i));
                X.WriteElementString("title", p);
                X.WriteEndElement();
                i++;
            }
            WriteLinks(PartLinks);
            X.WriteEndElement();

            var rem = GetData("remarks", "<objectives><objective><remarks>", UserName, DOID);

            var uuids = GetDBData("objective, method uuid", UserName, DOID);

            foreach (var x in uuids)
            {
                X.WriteStartElement("assessment-method");
                X.WriteAttributeString("method-uuid", x[0]);
                X.WriteString(x[2]);
                X.WriteEndElement();
            }


            X.WriteStartElement("remarks");
            X.WriteElementString("p", rem);
            X.WriteEndElement();


            X.WriteEndElement();
        }
        private void XMLBackMatter()
        {


            Xwriter.WriteStartElement("back-matter");

            var cits = GetCitations();
            foreach (var cit in cits)
            {
                Xwriter.WriteStartElement("citation");
                Xwriter.WriteAttributeString("id", cit.ID);
                Xwriter.WriteElementString("target", cit.Target);
                Xwriter.WriteElementString("title", cit.Title);
                Xwriter.WriteEndElement();

            }

            var resS = GetResources();
            foreach (var res in resS)
            {
                Xwriter.WriteStartElement("resource");
                Xwriter.WriteAttributeString("id", res.ID);
                Xwriter.WriteElementString("desc", res.Desc);
                Xwriter.WriteStartElement("base64");
                Xwriter.WriteAttributeString("filename", res.FileName);
                Xwriter.WriteValue(res.DataStream);
                Xwriter.WriteEndElement();
                Xwriter.WriteEndElement();

            }


            Xwriter.WriteEndElement();


        }

        protected void StartButton_Click(object sender, EventArgs e)
        {
            Cache.Remove("doid");
            try
            {
                if (FileUpload1.HasFile)
                {
                    var FileData = FileUpload1.PostedFile;
                    var fileName = FileUpload1.FileName;

                    var nameOnly = fileName.Replace(".xml", "");

                    var Id = Guid.NewGuid();

                    var FileToConvertPath = HttpContext.Current.Server.MapPath(string.Format(@"~\Uploads\{0}{1}", Id, fileName));
                    FileData.SaveAs(FileToConvertPath);

                    var shortName = string.Format("{0} {1}", nameOnly, "SAR");
                    var fullName = string.Format("{0} {1}", nameOnly, "Security Assessment Report");

                    var userDocs = GetSystemDocInfo(SystemID, DocType);
                    int rank = -1;
                    for (int i = 0; i < userDocs.Count; i++)
                    {
                        if (userDocs[i][1] == shortName && userDocs[i][2] == fullName)
                        {
                            rank = i;
                            break;
                        }
                    }

                    if (rank >= 0)
                        DOID = int.Parse(userDocs[rank][0]);
                    else
                        DOID = AddDoc(shortName, fullName, SystemID, DocType, 1, SystemID);

                    Cache.Insert("doid", DOID);
                    Cache.Insert("userId", UserName);
                    Cache.Insert("selectedFile", fileName);
                    ImportToDBSAR(XMLNamespace, OscalSchemaPath, FileToConvertPath, DOID, SystemID);
                    var InfoSysName = GetData("system-name", "<system-characteristics><system-name>", UserName, DOID);
                    var SystemIDcode = GetData("system-id", "<system-characteristics><system-id>", UserName, DOID);

                    SystemID = AddSystem(OrgId, InfoSysName, 1, SystemIDcode, 1, UserId);

                    Cache.Insert("SystemId", SystemID);

                    return;

                }
                if (DocFullNameTextBox.Text.Length == 0)
                {

                    FileName = DocumentDropDownList.SelectedValue;

                    SystemName = SystemDropDownList.SelectedValue;

                    SystemID = GetUserSystemId(UserName, SystemName);

                    Cache.Insert("SystemId", SystemID);

                    if (Cache["selectedFile"] == null)
                        Cache.Insert("selectedFile", FileName);

                    for (int i = 0; i < DocInfos.Count(); i++)
                    {
                        if (DocInfos[i][2] == FileName)
                        {
                            DOID = int.Parse(DocInfos[i][0]);
                            break;
                        }
                    }


                    Cache.Insert("doid", DOID);
                    Cache.Insert("userId", UserName);

                    Response.Redirect(@"~/PagesSAR/Metadata/Title.aspx", false);
                    return;
                }
                else
                {
                    FileName = DocFullNameTextBox.Text;
                    var fullName = FileName;
                    var shortName = DocShortNameTextBox.Text;

                    SystemName = SystemDropDownList.SelectedValue;

                    SystemID = GetUserSystemId(UserName, SystemName);

                    Cache.Insert("SystemId", SystemID);


                    int rank = -1;
                    for (int i = 0; i < DocInfos.Count; i++)
                    {
                        if (DocInfos[i][1] == shortName && DocInfos[i][2] == fullName)
                        {
                            rank = i;
                            break;
                        }
                    }

                    if (rank >= 0)
                        DOID = int.Parse(DocInfos[rank][0]);
                    else
                        DOID = AddDoc(shortName, fullName, SystemID, DocType, 1, SystemID);



                    Cache.Insert("doid", DOID);
                    Cache.Insert("userId", UserName);
                    Cache.Insert("selectedFile", FileName);
                    Response.Redirect(@"~/PagesSAR/Metadata/Title.aspx", false);

                    return;
                }

            }
            catch (Exception Ex)
            {
                StatusLabel.ForeColor = Color.Red;
                StatusLabel.Text = Ex.Message;
            }


        }

        void GetHttpStream()
        {

            // Creates an HttpWebRequest with the specified URL.
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(Page.Request.Url);
            // Sends the HttpWebRequest and waits for the response.			
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            // Gets the stream associated with the response.
            Stream receiveStream = myHttpWebResponse.GetResponseStream();  // Response.OutputStream;//
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, encode);

            ProcessingPage = readStream.ReadToEnd();

            receiveStream.Close();
            readStream.Close();
        }

        protected void SaveSystemButton_Click(object sender, EventArgs e)
        {
            if (SystemTextBox.Text.Length == 0 || SysIDTextBox.Text.Length == 0)
            {
                SystemName = SystemDropDownList.SelectedValue;
            }
            else
            {
                SystemName = SystemTextBox.Text;
                SystemIdentifier = SysIDTextBox.Text;
                AddSystem(OrgId, SystemName, 1, SystemIdentifier, 1, UserId);
            }

            SystemID = GetUserSystemId(UserName, SystemName);
            Cache.Insert("SystemId", SystemID);
            Cache.Insert("systemName", SystemName);

            UserDocuments = GetSystemDocFullName(SystemID, DocType);
            DocInfos = GetSystemDocInfo(SystemID, DocType);

            DocumentDropDownList.DataSource = UserDocuments;
            DocumentDropDownList.DataBind();
            this.DocDiv.Visible = true;
        }

        protected void DocumentDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var fileName = DocumentDropDownList.SelectedValue;
            Cache.Insert("selectedFile", fileName);
            OK = true;
            this.DocDiv.Visible = OK;
        }

        public void PrintProgressBar(string Message, int PercentComplete, bool first = false)
        {
            var sb = new StringBuilder();
            sb.Append("<script>");
            var iis = string.Format("\"{0}%\"", PercentComplete);
            sb.AppendLine(string.Format("myFunction(\"{0}\",{1})", Message, iis));
            sb.AppendLine(string.Format("document.getElementById(\"mainbar\").innerText=\"{0}%\"", PercentComplete));

            sb.Append("</script>");
            //mainbar.InnerText = string.Format("{0}%", PercentComplete); ;

            var file = "";
            var update = sb.ToString();
            if (first)
            {
                HttpContext.Current.Response.Write(ProcessingPage);

                HttpContext.Current.Response.Flush();


                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearContent();
                file = update;  ////  ProcessingPage + 
                HttpContext.Current.Response.Write(file);

                HttpContext.Current.Response.Flush();
            }
            else
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearContent();
                file = update;

                HttpContext.Current.Response.Write(file);

                HttpContext.Current.Response.Flush();
            }

        }

        public void ProcessData(string ssp_file, string word_template_file, out string wordDocumentPath)
        {
            var message = string.Format("Starting mappings XML tags");
            var percent = 55;
            PrintProgressBar(message, percent);
            string xmlDataFile = HttpContext.Current.Server.MapPath(string.Format(@"~\Uploads\{0}", ssp_file));
            string templateDocument = HttpContext.Current.Server.MapPath(string.Format(@"~/Templates/{0}", word_template_file));
            string tempDocument = HttpContext.Current.Server.MapPath(string.Format(@"~/Downloads/{0}", "MyGeneratedDocument.docx"));
            string outputDocument = HttpContext.Current.Server.MapPath(string.Format(@"~/Downloads/{0}", word_template_file.Replace("Template", "OSCAL")));

            if (File.Exists(tempDocument))
            {
                File.Delete(tempDocument);
            }
            File.Copy(templateDocument, tempDocument);

            if (File.Exists(outputDocument))
            {
                File.Delete(outputDocument);
            }


            string xmlDataFileWithoutTable = HttpContext.Current.Server.MapPath(string.Format(@"~\Uploads\{0}", "workingFile.xml"));
            OSCALBaseClass.CollapseDescriptionAndRemoveTable(xmlDataFile, xmlDataFileWithoutTable);

            message = string.Format("Wrapping XML mappings");
            percent = 75;
            PrintProgressBar(message, percent);



            //   using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateDocument, true))
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(tempDocument, true))
            {
                //get the main part of the document which contains CustomXMLParts
                MainDocumentPart mainPart = wordDoc.MainDocumentPart;

                //delete all CustomXMLParts in the document. If needed only specific CustomXMLParts can be deleted using the CustomXmlParts IEnumerable
                mainPart.DeleteParts<CustomXmlPart>(mainPart.CustomXmlParts);

                //add new CustomXMLPart with data from new XML file
                CustomXmlPart myXmlPart = mainPart.AddCustomXmlPart(CustomXmlPartType.CustomXml);
                using (FileStream stream = new FileStream(xmlDataFileWithoutTable, FileMode.Open))
                {
                    myXmlPart.FeedData(stream);

                }
            }

            File.Copy(tempDocument, outputDocument);
            File.Delete(tempDocument);
            wordDocumentPath = outputDocument;

        }
        public Guid GetGuid(string id)
        {
            var len = id.Length;
            int d = 32 - len;
            if (d < 0)
                return new Guid(id);
            else
            {
                var tan = id;
                for (int i = 0; i < d; i++)
                {
                    tan += "0";
                }
                var hs = tan.Length;
                return new Guid(tan);
            }
        }

       

        private void XMLMetadata()
        {
            Xwriter.WriteStartElement("metadata");

            Xwriter.WriteStartElement("title");
            var title = GetData("title", "<metadata><title>", UserName, DOID);
            Xwriter.WriteValue(title);
            Xwriter.WriteEndElement();

            Xwriter.WriteStartElement("published");
            var PublishedDateString = GetData("published", "<metadata><published>", UserName, DOID);
            Xwriter.WriteValue(PublishedDateString);
            Xwriter.WriteEndElement();

            Xwriter.WriteStartElement("last-modified");
            string VersionDateString = GetData("last-modified", "<metadata><last-modified>", UserName, DOID);
            Xwriter.WriteValue(VersionDateString);
            Xwriter.WriteEndElement();

            Xwriter.WriteStartElement("version");
            string Version = GetData("version", "<metadata><version>", UserName, DOID);
            Xwriter.WriteValue(Version);
            Xwriter.WriteEndElement();


            Xwriter.WriteStartElement("oscal-version");
            string OSCALVersion = GetData("oscal-version", "<metadata><oscal-version>", UserName, DOID);
            Xwriter.WriteValue(OSCALVersion);
            Xwriter.WriteEndElement();

            //Xwriter.WriteStartElement("prop");
            //Xwriter.WriteAttributeString("name", "sensitivity-labe");
            //Xwriter.WriteValue("Controlled Unclassified Information");
            //Xwriter.WriteEndElement();

            var revisions = GetRevisions();
            Xwriter.WriteStartElement("revision-history");

            foreach (var rev in revisions)
            {
                Xwriter.WriteStartElement("revision");

                Xwriter.WriteElementString("title", rev.Title);
                Xwriter.WriteElementString("published", rev.Published);
                Xwriter.WriteElementString("last-modified", rev.LastModified);
                Xwriter.WriteElementString("version", rev.Version);
                Xwriter.WriteElementString("oscal-version", rev.OSCALVersion);
                if (rev.Props != null)
                    foreach (var prop in rev.Props)
                    {
                        Xwriter.WriteStartElement("prop");
                        Xwriter.WriteAttributeString("name", prop.Name);
                        Xwriter.WriteAttributeString("id", prop.ID);
                        Xwriter.WriteAttributeString("ns", prop.NS);
                        Xwriter.WriteAttributeString("class", prop.Class);
                        Xwriter.WriteValue(prop.Value);
                        Xwriter.WriteEndElement();

                    }
                if (rev.Links != null)
                    foreach (var link in rev.Links)
                    {
                        Xwriter.WriteStartElement("link");
                        Xwriter.WriteAttributeString("href", link.HRef);
                        Xwriter.WriteAttributeString("rel", link.Rel);
                        Xwriter.WriteAttributeString("media-type", link.MediaType);
                        Xwriter.WriteValue(link.MarkUpLine);
                        Xwriter.WriteEndElement();
                    }
                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", rev.Remarks);
                Xwriter.WriteEndElement();
                Xwriter.WriteEndElement();

            }

            Xwriter.WriteEndElement();

            var roles = GetRoles();

            foreach (var role in roles)
            {
                Xwriter.WriteStartElement("role");
                Xwriter.WriteAttributeString("id", role.RoleID);
                Xwriter.WriteElementString("title", role.Title.Value);
                Xwriter.WriteElementString("short-name", role.ShortName);
                Xwriter.WriteElementString("desc", role.Description);
                if (role.Props != null)
                    WriteProps(role.Props);


                if (role.Annotations != null)
                    WriteAnns(role.Annotations);

                if (role.Links != null)
                    WriteLinks(role.Links);
                    
                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", role.Remarks);
                Xwriter.WriteEndElement();
                Xwriter.WriteEndElement();

            }

            var locations = GetLocations();
            foreach (var loc in locations)
            {
                Xwriter.WriteStartElement("location");
                Xwriter.WriteAttributeString("uuid", loc.UUID);
                Xwriter.WriteStartElement("address");
                Xwriter.WriteAttributeString("type", loc.AddressType);
                Xwriter.WriteElementString("addr-line", loc.AddressLine1);
                Xwriter.WriteElementString("addr-line", loc.AddressLine2);
                Xwriter.WriteElementString("city", loc.City);
                Xwriter.WriteElementString("state", loc.State);
                Xwriter.WriteElementString("postal-code", loc.PostalCode);
                Xwriter.WriteElementString("country", loc.Country);
                Xwriter.WriteEndElement();
                foreach (var mail in loc.Emails)
                    Xwriter.WriteElementString("email", mail);
                foreach (var phone in loc.Phones)
                {
                    Xwriter.WriteStartElement("phone");
                    Xwriter.WriteAttributeString("type", phone.Type);
                    Xwriter.WriteValue(phone.Number);
                    Xwriter.WriteEndElement();
                }
                //  Xwriter.WriteElementString("url", loc.Url.ToString());

                if (loc.Props != null)
                    foreach (var prop in loc.Props)
                    {
                        Xwriter.WriteStartElement("prop");
                        Xwriter.WriteAttributeString("name", prop.Name);
                        Xwriter.WriteAttributeString("id", prop.ID);
                        Xwriter.WriteAttributeString("ns", prop.NS);
                        Xwriter.WriteAttributeString("class", prop.Class);
                        Xwriter.WriteValue(prop.Value);
                        Xwriter.WriteEndElement();

                    }

                if (loc.Annotations != null)
                    foreach (var ann in loc.Annotations)
                    {
                        Xwriter.WriteStartElement("annotation");
                        Xwriter.WriteAttributeString("name", ann.Name);
                        Xwriter.WriteAttributeString("id", ann.ID);
                        Xwriter.WriteAttributeString("ns", ann.NS);
                        Xwriter.WriteAttributeString("value", ann.Value);
                        Xwriter.WriteStartElement("remarks");
                        Xwriter.WriteElementString("p", ann.Remarks);
                        Xwriter.WriteEndElement();
                        Xwriter.WriteEndElement();
                    }
                if (loc.Links != null)
                    foreach (var link in loc.Links)
                    {
                        Xwriter.WriteStartElement("link");
                        Xwriter.WriteAttributeString("href", link.HRef);
                        Xwriter.WriteAttributeString("rel", link.Rel);
                        Xwriter.WriteAttributeString("media-type", link.MediaType);
                        Xwriter.WriteValue(link.MarkUpLine);
                        Xwriter.WriteEndElement();
                    }
                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", loc.Remarks);
                Xwriter.WriteEndElement();

                Xwriter.WriteEndElement();

            }

            var fullparties = GetMainParties();

            for (int i = 0; i < fullparties.Count(); i++)
            {
                var party = fullparties[i];
                Xwriter.WriteStartElement("party");

                Xwriter.WriteAttributeString("uuid", party.UUID);
                Xwriter.WriteAttributeString("type", party.PartyType);

                Xwriter.WriteElementString("party-name", party.Name);
                Xwriter.WriteElementString("short-name", party.ShortName);
                Xwriter.WriteStartElement("external-id");
                Xwriter.WriteAttributeString("type", party.ExternalType);
                Xwriter.WriteString(party.ExternalID);
                Xwriter.WriteEndElement();

                if (party.Props != null)
                    foreach (var prop in party.Props)
                    {
                        Xwriter.WriteStartElement("prop");
                        Xwriter.WriteAttributeString("name", prop.Name);
                        Xwriter.WriteAttributeString("id", prop.ID);
                        Xwriter.WriteAttributeString("ns", prop.NS);
                        Xwriter.WriteAttributeString("class", prop.Class);
                        Xwriter.WriteValue(prop.Value);
                        Xwriter.WriteEndElement();

                    }

                if (party.Annotations != null)
                    foreach (var ann in party.Annotations)
                    {
                        Xwriter.WriteStartElement("annotation");
                        Xwriter.WriteAttributeString("name", ann.Name);
                        Xwriter.WriteAttributeString("id", ann.ID);
                        Xwriter.WriteAttributeString("ns", ann.NS);
                        Xwriter.WriteAttributeString("value", ann.Value);
                        Xwriter.WriteStartElement("remarks");
                        Xwriter.WriteElementString("p", ann.Remarks);
                        Xwriter.WriteEndElement();

                        Xwriter.WriteEndElement();
                    }
                if (party.Links != null)
                    foreach (var link in party.Links)
                    {
                        Xwriter.WriteStartElement("link");
                        Xwriter.WriteAttributeString("href", link.HRef);
                        Xwriter.WriteAttributeString("rel", link.Rel);
                        Xwriter.WriteAttributeString("media-type", link.MediaType);
                        Xwriter.WriteValue(link.MarkUpLine);
                        Xwriter.WriteEndElement();
                    }

                if (party.Addresses != null)
                    foreach (var address in party.Addresses)
                    {
                        XwriteAddress(address);
                    }
                if (party.Emails != null)
                    foreach (var x in party.Emails)
                    {
                        Xwriter.WriteElementString("email", x);
                    }

                if (party.Phones != null)
                    foreach (var x in party.Phones)
                    {
                        Xwriter.WriteStartElement("phone");
                        Xwriter.WriteAttributeString("type", x.Type);
                        Xwriter.WriteValue(x.Number);
                        Xwriter.WriteEndElement();
                    }

                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", party.Remarks);
                Xwriter.WriteEndElement();


                Xwriter.WriteEndElement();
            }

            var responParties = GetResponsibleParties();

            foreach (var resparty in responParties)
            {

                Xwriter.WriteStartElement("responsible-party");
                Xwriter.WriteAttributeString("role-id", resparty.RoleID);


                if (resparty.PartyIDs != null)
                    for (int i = 0; i < resparty.PartyIDs.Count; i++)
                    {
                        Xwriter.WriteElementString("party-uuid", resparty.PartyIDs[i].Value);

                    }
                else
                    Xwriter.WriteElementString("party-uuid", resparty.PartyUUID);

                if (resparty.Props != null)
                    foreach (var prop in resparty.Props)
                    {
                        Xwriter.WriteStartElement("prop");
                        Xwriter.WriteAttributeString("name", prop.Name);
                        Xwriter.WriteAttributeString("id", prop.ID);
                        Xwriter.WriteAttributeString("ns", prop.NS);
                        Xwriter.WriteAttributeString("class", prop.Class);
                        Xwriter.WriteValue(prop.Value);
                        Xwriter.WriteEndElement();

                    }

                if (resparty.Annotations != null)
                    foreach (var ann in resparty.Annotations)
                    {
                        Xwriter.WriteStartElement("annotation");
                        Xwriter.WriteAttributeString("name", ann.Name);
                        Xwriter.WriteAttributeString("id", ann.ID);
                        Xwriter.WriteAttributeString("ns", ann.NS);
                        Xwriter.WriteAttributeString("value", ann.Value);
                        Xwriter.WriteStartElement("remarks");
                        Xwriter.WriteElementString("p", ann.Remarks);
                        Xwriter.WriteEndElement();
                        Xwriter.WriteEndElement();
                    }
                if (resparty.Links != null)
                    foreach (var link in resparty.Links)
                    {
                        Xwriter.WriteStartElement("link");
                        Xwriter.WriteAttributeString("href", link.HRef);
                        Xwriter.WriteAttributeString("rel", link.Rel);
                        Xwriter.WriteAttributeString("media-type", link.MediaType);
                        Xwriter.WriteValue(link.MarkUpLine);
                        Xwriter.WriteEndElement();
                    }

                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", resparty.Remarks);
                Xwriter.WriteEndElement();

                Xwriter.WriteEndElement();
            }

            Xwriter.WriteEndElement();
        }

        protected void OpenFileButton_Click(object sender, EventArgs e)
        {
            FileName = Cache["selectedFile"].ToString();
            FileName = FileName.Replace(" ", "");
            if (Cache["OSCAL"] != null && Cache["OSCAL"].ToString() == "true")
                Response.Redirect(string.Format(@"~/Uploads/{0}.xml", FileName));
            else
            {
                var file = Cache["outputFile"].ToString();
                Response.Redirect(string.Format(@"~/Downloads/{0}", file));
            }
        }

        protected void OSCALButton_Click(object sender, EventArgs e)
        {
            try
            {
                // var guid = Guid.NewGuid().ToString(); 
                var ErrorFilePath = string.Format(@"{0}Uploads\{1}ValidationErrors.txt", AppPath, "SAR");
                Cache.Remove("XMLPath");
                FileName = DocumentDropDownList.SelectedValue;
                SetDOID();

                OpenFileButton.Visible = false;

                GetHttpStream();

                CorrectXMLGenerateXMLDoc();
                string filename = FileName + ".xml";
                string xmlSchemaPath = HttpContext.Current.Server.MapPath(string.Format(@"~\Templates\{0}", SARschema));
                string XmlDocumentPath = HttpContext.Current.Server.MapPath(string.Format(@"~\Uploads\{0}", filename));

                //  XMLValidator.PseudoValidator(XmlDocumentPath, xmlSchemaPath);

                XMLValidator.RunValidator(XmlDocumentPath, xmlSchemaPath, ErrorFilePath);
                //  GenerateXMLDoc();

                CollapseDiv("Form");

                mainbarServer.Style["width"] = "100%";

                OpenFileButton.Visible = true;


                Cache.Insert("OSCAL", "true");
                Cache.Insert("XMLPath", FilePath);
                StatusLabel.Text = string.Format("File Path: {0}", FilePath);
            }
            catch (Exception ex)
            {
                CollapseDiv("Form");
                StatusLabel.ForeColor = System.Drawing.Color.Red;
                var path = string.Format(@"{0}Uploads\{1}ValidationErrors.txt", AppPath, "SAR"); ;
                var err = GetError(path);
                StatusLabel.Text = ex.Message + "\n" + err;
            }

        }
    }
}