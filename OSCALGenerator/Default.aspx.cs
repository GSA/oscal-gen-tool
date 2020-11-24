
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
using OConvert;
using System.Net;
using DocumentFormat.OpenXml.Packaging;

namespace OSCALGenerator
{
    public partial class _Default : BasePage
    {
       
        protected private const string WordTemplateFile = "FedRAMP-SSP-Moderate-Baseline-OSCAL";
        protected private const string ModerateWordTemplate = "FedRAMP-SSP-Moderate-Baseline-OSCAL";
        protected private const string LowWordTemplate = "FedRAMP-SSP-Low-Baseline-Template";
        protected private const string HighWordTemplate = "FedRAMP-SSP-High-Baseline-Template";
        
        protected string OscalSchemaPath { get; set; }
        protected string FileToConvertPath { get; set; }
  
        protected Guid Id;
      
        string AppPath;
        public int Stage { get; set; }

        public string Message { get; set; }

        private string SecuritySensitivityLevel;
        string FileName;

        string message = "";
        int percent = 0;
        int wordPercent = 0;


        string ProcessingPage;

     
        Dictionary<string, int> ControlIdToRank;
        List<string> ControlIDs;
        List<List<string>> StatementIDs;
        List<List<string>> ParameterIDs;
 

        Guid OrgId;

        string SystemIdentifier;
        int UserId;
        List<string> UserDocuments;
        List<List<string>> DocInfos;
        

        private string FilePath;
        StreamWriter sw;

        XmlWriterSettings XmlWriterSettings;
        StringBuilder sb;
       
        bool OK;

        private void InitDefaultUserSetting()
        {
            var defaultuserName = "wizardtest";
            Cache.Remove("username");
            Cache.Insert("username", defaultuserName);
        }
       
        protected new void Page_Load(object sender, EventArgs e)
        {
            try
            {
                InitDefaultUserSetting(); ///Tom you may modify this method accordingly.

                DocInfos = new List<List<string>>();

                if (IsPostBack)
                {

                }
                OpenFileButton.Visible = false;

                string appPath = Request.PhysicalApplicationPath;
                OscalSchemaPath = string.Format(@"{0}\Templates\{1}", appPath, SSP3schema);
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
                        UserDocuments = GetSystemDocFullName(SystemID, 3);
                        DocInfos = GetSystemDocInfo(SystemID, 3);
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
                
                    Cache.Insert("userId", UserName);
                }

                if (Cache["selectedFile"] != null && !FileUpload1.HasFile && DocFullNameTextBox.Text.Length == 0)
                {
                    //FileName = Cache["selectedFile"].ToString();

                    //DocumentDropDownList.SelectedValue = FileName;
                }

                AppPath = Request.PhysicalApplicationPath;


            }
            catch (Exception ex)
            {
                StatusLabel.Text = ex.Message;
                StatusLabel.BackColor = Color.Red;
            }
        }

        private void M3BackMatter()
        {


            Xwriter.WriteStartElement("back-matter");

            var cits = GetCitations();
            foreach (var cit in cits)
            {
                Xwriter.WriteStartElement("resource");
                Xwriter.WriteAttributeString("uuid", Guid.NewGuid().ToString());

                Xwriter.WriteStartElement("citation");
                // Xwriter.WriteAttributeString("id", cit.ID);
                Xwriter.WriteElementString("text", cit.Target);
                //Xwriter.WriteElementString("title", cit.Title);
                Xwriter.WriteEndElement();

                X.WriteEndElement();

            }

            var resS = GetResources();
            foreach (var res in resS)
            {
                Xwriter.WriteStartElement("resource");
                Xwriter.WriteAttributeString("uuid", Guid.NewGuid().ToString());
                Xwriter.WriteElementString("desc", res.Desc);
                Xwriter.WriteStartElement("base64");
                Xwriter.WriteAttributeString("filename", res.FileName);
                Xwriter.WriteValue(res.DataStream);
                Xwriter.WriteEndElement();
                Xwriter.WriteEndElement();

            }


            Xwriter.WriteEndElement();


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

        private void GenerateBackMatter()
        {
            var tag = "<back-matter>";
            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(1), tag);
            var cits = GetCitations();
            foreach (var cit in cits)
            {
                sw.WriteLine("{0}<citation id=\"{1}\">", GetEmptyTabSpaces(2), cit.ID);
                sw.WriteLine("{0}<target>{1}</target>", GetEmptyTabSpaces(3), cit.Target);
                sw.WriteLine("{0}<title>{1}</title>", GetEmptyTabSpaces(3), cit.Title);
                sw.WriteLine("{0}</citation>", GetEmptyTabSpaces(2));
            }

            var resS = GetResources();
            foreach (var res in resS)
            {
                sw.WriteLine("{0}<resource id=\"{0}\">", GetEmptyTabSpaces(2), res.ID);
                sw.WriteLine("{0}<desc>{1}</desc>", GetEmptyTabSpaces(3), res.Desc);
                sw.WriteLine("{0}<base64 filename=\"{1}\">{2}</base64>", GetEmptyTabSpaces(3), res.FileName, res.DataStream);
                sw.WriteLine("{0}</resource>", GetEmptyTabSpaces(2));
            }

            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(1), "</back-matter>");

        }

        private void XMLControlImplementation()
        {

            Xwriter.WriteStartElement("control-implementation");

            Xwriter.WriteStartElement("description");
            Xwriter.WriteElementString("p", "TBD");
            Xwriter.WriteEndElement();


            SecuritySensitivityLevel = GetData("security-sensitivity-level", "<system-characteristics><security-sensitivity-level>", UserName, DOID);
            var ids = GetIds();
            decimal n = 0;
            foreach (var controlid in ControlIDs)
            {
                message = string.Format("Rendering the OSCAL Document {0}: Control Implementation, filling {1}... ", FileName, controlid);
                var dec = (n / ControlIDs.Count) * 40;
                percent = 55 + (int)dec;
                PrintProgressBar(message, percent - wordPercent);
                // FillControl(controlid);
                M3FillControl(controlid);
                n++;
            }


            Xwriter.WriteEndElement();
            // sw.WriteLine("{0}{1}", GetEmptyTabSpaces(1), "</control-implementation>");
        }


        private void GenerateControlImplementation()
        {
            var tag = "<control-implementation>";
            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(1), tag);
            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(2), "<description>");
            sw.WriteLine("{0}<p>{1}</p>", GetEmptyTabSpaces(3), "TBD");
            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(2), "</description>");

            SecuritySensitivityLevel = GetData("security-sensitivity-level", "<system-characteristics><security-sensitivity-level>", UserName, DOID);
            var ids = GetIds();

            foreach (var controlid in ControlIDs)
            {
                //FillControl(controlid);
                M3FillControl(controlid);
            }


            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(1), "</control-implementation>");
        }

        private void M3SystemImplementation()
        {

            Xwriter.WriteStartElement("system-implementation");

            var Subjects5 = GetSAPUsers("<system-implementation>");
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

            X.WriteStartElement("system-inventory");

            var Subjects4 = GetInventoryItems();

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

            X.WriteEndElement();

            var Remark = GetData("remarks", "<system-implementation><remarks>", UserName, DOID);

            X.WriteStartElement("remarks");
            X.WriteElementString("p", Remark);
            X.WriteEndElement();

            Xwriter.WriteEndElement();
        }


        private void XMLSystemImplementation()
        {
           


            Xwriter.WriteStartElement("system-implementation");

            var users = GetUsers();
            foreach (var user in users)
            {
                Xwriter.WriteStartElement("user");
                Xwriter.WriteAttributeString("id", user.ID);
                Xwriter.WriteElementString("title", user.Title);

                Xwriter.WriteStartElement("prop");
                Xwriter.WriteAttributeString("name", "external");
                Xwriter.WriteAttributeString("ns", user.NS);
                Xwriter.WriteValue(user.External);
                Xwriter.WriteEndElement();

                Xwriter.WriteStartElement("prop");
                Xwriter.WriteAttributeString("name", "access");
                Xwriter.WriteAttributeString("ns", user.NS);
                Xwriter.WriteValue(user.Access);
                Xwriter.WriteEndElement();


                Xwriter.WriteStartElement("prop");
                Xwriter.WriteAttributeString("name", "sensitivity-level");
                Xwriter.WriteAttributeString("ns", user.NS);
                Xwriter.WriteValue(user.SensitivityLevel);
                Xwriter.WriteEndElement();

                Xwriter.WriteElementString("role-id", user.RoleID);

                Xwriter.WriteStartElement("authorized-privilege");
                Xwriter.WriteAttributeString("name", user.AuthorizedPrivilegeName);
                Xwriter.WriteElementString("function-performed", user.FunctionPerformed);
                Xwriter.WriteEndElement();



                Xwriter.WriteEndElement();
            }

            Xwriter.WriteEndElement();

        }

        private void GenerateSystemImplementation()
        {
            var tag = "<system-implementation>";
            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(1), tag);

            var users = GetUsers();
            foreach (var user in users)
            {
                sw.WriteLine("{0}<user id=\"{1}\">", GetEmptyTabSpaces(2), user.ID);
                sw.WriteLine(string.Format("{0}<title>{1}</title>", GetEmptyTabSpaces(3), user.Title));
                sw.WriteLine(string.Format("{0}<prop name=\"external\" ns=\"{1}\">{2}</prop>", GetEmptyTabSpaces(3), user.NS, user.External));
                sw.WriteLine(string.Format("{0}<prop name=\"access\" ns=\"{1}\">{2}</prop>", GetEmptyTabSpaces(3), user.NS, user.Access));
                sw.WriteLine(string.Format("{0}<prop name=\"sensitivity-level\" ns=\"{1}\">{2}</prop>", GetEmptyTabSpaces(3), user.NS, user.SensitivityLevel));
                sw.WriteLine(string.Format("{0}<role-id>{1}</role-id>", GetEmptyTabSpaces(3), user.RoleID));
                sw.WriteLine(string.Format("{0}<authorized-privilege name=\"{1}\">", GetEmptyTabSpaces(3), user.AuthorizedPrivilegeName));
                sw.WriteLine(string.Format("{0}<function-performed>{1}</function-performed>", GetEmptyTabSpaces(4), user.FunctionPerformed));
                sw.WriteLine(string.Format("{0}</authorized-privilege>", GetEmptyTabSpaces(3)));
                sw.WriteLine("{0}</user>", GetEmptyTabSpaces(2));

            }


            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(1), "</system-implementation>");
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

        private void M3Metadata()
        {
            Xwriter.WriteStartElement("metadata");

            Xwriter.WriteStartElement("title");
            var title = GetData("title", "<metadata><title>", UserName, DOID);
            Xwriter.WriteValue(title);
            Xwriter.WriteEndElement();

            Xwriter.WriteStartElement("published");
            string published = GetData("published", "<metadata><published>", UserName, DOID);
            Xwriter.WriteValue(published);
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

        private void M3ImportProfile()
        {
            Xwriter.WriteStartElement("import-profile");
            var href = GetData("href", "<import-profile href>", UserName, DOID);
            Xwriter.WriteAttributeString("href", href);
            var remarks = GetData("remarks", "<import-profile href><remarks>", UserName, DOID);
            Xwriter.WriteStartElement("remarks");
            Xwriter.WriteElementString("p", remarks);
            Xwriter.WriteEndElement();
            Xwriter.WriteEndElement();

        }

        private void XMLMetadata()
        {
            Xwriter.WriteStartElement("metadata");

            Xwriter.WriteStartElement("title");
            var title = GetData("title", "<metadata><title>", UserName, DOID);
            Xwriter.WriteValue(title);
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

            Xwriter.WriteStartElement("prop");
            Xwriter.WriteAttributeString("name", "sensitivity-labe");
            Xwriter.WriteValue("Company Sensitive and Proprietary");
            Xwriter.WriteEndElement();

            var roles = GetRoles();

            foreach (var role in roles)
            {
                Xwriter.WriteStartElement("role");
                Xwriter.WriteAttributeString("id", role.RoleID);
                Xwriter.WriteElementString("title", role.Title.Value);
                Xwriter.WriteElementString("desc", role.Description);
                Xwriter.WriteEndElement();

            }

            var part = GetParties();

            // var fullparties = FillCompanyInfo(part);

            foreach (var party in part)
            {


                Xwriter.WriteStartElement("party");
                Xwriter.WriteAttributeString("id", party.PartyID);

                if (party.IsAPerson)
                {
                    Xwriter.WriteStartElement("person");
                    Xwriter.WriteElementString("person-name", party.PersonName);
                }
                else
                {
                    Xwriter.WriteStartElement("org");
                    Xwriter.WriteElementString("org-name", party.OrgName);
                    Xwriter.WriteElementString("short-name", party.ShortName);
                }
                Xwriter.WriteElementString("org-id", party.OrgID);
                Xwriter.WriteStartElement("address");
                Xwriter.WriteElementString("addr-line", party.AddressLine1);
                Xwriter.WriteElementString("addr-line", party.AddressLine2);
                Xwriter.WriteElementString("city", party.City);
                Xwriter.WriteElementString("state", party.State);
                Xwriter.WriteElementString("postal-code", party.PostalCode);
                Xwriter.WriteElementString("country", party.Country);
                Xwriter.WriteEndElement();


                if (party.IsAPerson)
                {
                    Xwriter.WriteElementString("email", party.Email);
                    Xwriter.WriteElementString("phone", party.Phone);

                }
                else
                {
                    Xwriter.WriteStartElement("remarks");
                    Xwriter.WriteElementString("p", party.Remarks);
                    Xwriter.WriteEndElement();
                }
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
                        Xwriter.WriteElementString("party-id", resparty.PartyIDs[i].Value);

                    }
                else
                    Xwriter.WriteElementString("party-id", resparty.PartyID);

                Xwriter.WriteEndElement();



            }

            Xwriter.WriteEndElement();
        }

        private void M3SystemCharacteristics()
        {

            Xwriter.WriteStartElement("system-characteristics");

            var sysIds = GetSystemIDs();

            foreach (var id in sysIds)
            {
                X.WriteStartElement("system-id");
                X.WriteAttributeString("identifier-type", id.Type);
                X.WriteString(id.Identification);
                X.WriteEndElement();

            }
            //  var SystemID = GetData("system-id", "<system-characteristics><system-id>", UserName, DOID);

            var SysName = GetData("system-name", "<system-characteristics><system-name>", UserName, DOID);
            var SystemNameShort = GetData("system-name-short", "<system-characteristics><system-name-short>", UserName, DOID);

            SecuritySensitivityLevel = GetData("security-sensitivity-level", "<system-characteristics><security-sensitivity-level>", UserName, DOID);

            // Xwriter.WriteElementString("system-id", SystemID);
            Xwriter.WriteElementString("system-name", SysName);
            Xwriter.WriteElementString("system-name-short", SystemNameShort);

            var desc = GetData("desc", "<system-characteristics><description>", UserName, DOID);
            Xwriter.WriteStartElement("description");
            Xwriter.WriteElementString("p", desc);
            Xwriter.WriteEndElement();

            var props = GetProps("", "system identification");
            WriteProps(props);

            var anns = GetAnnotations("", "system identification");
            WriteAnns(anns);

            var links = GetLinks("", "system identification");
            WriteLinks(links);

            var date = GetData("date-authorized", "<system-characteristics><date-authorized>", UserName, DOID);
            //   X.WriteElementString("date-authorized", date);

            Xwriter.WriteElementString("security-sensitivity-level", SecuritySensitivityLevel);



            Xwriter.WriteStartElement("system-information");

            var parts = GetSystemInfoType();
            foreach (var infotype in parts)
            {
                Xwriter.WriteStartElement("information-type");

                Xwriter.WriteAttributeString("id", infotype.InfoId);

                Xwriter.WriteElementString("title", infotype.Name);
                Xwriter.WriteStartElement("description");
                Xwriter.WriteElementString("p", infotype.Description);
                Xwriter.WriteEndElement();

                Xwriter.WriteStartElement("information-type-id");
                Xwriter.WriteAttributeString("system", "nist");
                Xwriter.WriteValue(infotype.InfoTypeSytemId);
                Xwriter.WriteEndElement();


                Xwriter.WriteStartElement("confidentiality-impact");
                Xwriter.WriteElementString("base", infotype.ConfidentialityImpactBase);
                Xwriter.WriteElementString("selected", infotype.ConfidentialityImpactSelected);
                X.WriteElementString("adjustment-justification", "");
                Xwriter.WriteEndElement();


                Xwriter.WriteStartElement("integrity-impact");
                Xwriter.WriteElementString("base", infotype.IntegrityImpactBase);
                Xwriter.WriteElementString("selected", infotype.IntegrityImpactSelected);
                X.WriteElementString("adjustment-justification", "");
                Xwriter.WriteEndElement();



                Xwriter.WriteStartElement("availability-impact");
                Xwriter.WriteElementString("base", infotype.AvailabilityImpactBase);
                Xwriter.WriteElementString("selected", infotype.AvailabilityImpactSelected);
                X.WriteElementString("adjustment-justification", "");
                Xwriter.WriteEndElement();


                Xwriter.WriteEndElement();

            }
            Xwriter.WriteEndElement();

            Xwriter.WriteStartElement("security-impact-level");

            var Confidentiality = GetData("security-objective-confidentiality", "<system-characteristics><security-impact-level><security-objective-confidentiality>", UserName, DOID);
            Xwriter.WriteElementString("security-objective-confidentiality", Confidentiality);

            var Integrity = GetData("security-objective-integrity", "<system-characteristics><security-impact-level><security-objective-integrity>", UserName, DOID);
            Xwriter.WriteElementString("security-objective-integrity", Integrity);

            var Availability = GetData("security-objective-availability", "<system-characteristics><security-impact-level><security-objective-availability>", UserName, DOID);
            Xwriter.WriteElementString("security-objective-availability", Availability);

            Xwriter.WriteEndElement();

            var State = GetData("status state", UserName, DOID);
            Xwriter.WriteStartElement("status");
            Xwriter.WriteAttributeString("state", State);
            Xwriter.WriteEndElement();

            Xwriter.WriteStartElement("authorization-boundary");
            var AuthDesc = GetData("description", "<system-characteristics><authorization-boundary><description>", UserName, DOID);

            Xwriter.WriteStartElement("description");
            Xwriter.WriteElementString("p", AuthDesc);
            Xwriter.WriteEndElement();

            var Authuuid  = GetData("diagram id", "<system-characteristics><authorization-boundary><diagram id>", UserName, DOID);
            X.WriteStartElement("diagram");
            X.WriteAttributeString("uuid", Authuuid);
            X.WriteElementString("description", "");
            X.WriteElementString("caption", "");
            X.WriteEndElement();

            Xwriter.WriteEndElement();

            var Netuuid = GetData("diagram id", "<system-characteristics><network-architecture><diagram id>", UserName, DOID);
            if (Netuuid.Length > 0)
            {
                Xwriter.WriteStartElement("network-architecture");
                var NetDesc = GetData("description", "<system-characteristics><network-architecture><description>", UserName, DOID);

                Xwriter.WriteStartElement("description");
                Xwriter.WriteElementString("p", NetDesc);
                Xwriter.WriteEndElement();

                X.WriteStartElement("diagram");
                X.WriteAttributeString("uuid", Netuuid);
                X.WriteElementString("description", "");
                X.WriteElementString("caption", "");
                X.WriteEndElement();

                Xwriter.WriteEndElement();
            }


            var Datauuid = GetData("diagram id", "<system-characteristics><data-flow><diagram id>", UserName, DOID);
            if (Datauuid.Length > 0)
            {
                Xwriter.WriteStartElement("data-flow");
                var DataDesc = GetData("description", "<system-characteristics><data-flow><description>", UserName, DOID);

                Xwriter.WriteStartElement("description");
                Xwriter.WriteElementString("p", DataDesc);
                Xwriter.WriteEndElement();

                X.WriteStartElement("diagram");
                X.WriteAttributeString("uuid", Datauuid);
                X.WriteElementString("description", "");
                X.WriteElementString("caption", "");
                X.WriteEndElement();

                Xwriter.WriteEndElement();
            }


            Xwriter.WriteEndElement();
        }

        private void XMLSystemCharacteristics()
        {

            Xwriter.WriteStartElement("system-characteristics");


            var SystemID = GetData("system-id", "<system-characteristics><system-id>", UserName, DOID);

            var SysName = GetData("system-name", "<system-characteristics><system-name>", UserName, DOID);
            var SystemNameShort = GetData("system-name-short", "<system-characteristics><system-name-short>", UserName, DOID);


            SecuritySensitivityLevel = GetData("security-sensitivity-level", "<system-characteristics><security-sensitivity-level>", UserName, DOID);

            Xwriter.WriteElementString("system-id", SystemID);
            Xwriter.WriteElementString("system-name", SysName);
            Xwriter.WriteElementString("system-name-short", SystemNameShort);

            Xwriter.WriteStartElement("description");
            Xwriter.WriteElementString("p", "");
            Xwriter.WriteEndElement();

            Xwriter.WriteElementString("security-sensitivity-level", SecuritySensitivityLevel);




            Xwriter.WriteStartElement("system-information");


            var parts = GetSystemInfoType();
            foreach (var infotype in parts)
            {
                Xwriter.WriteStartElement("information-type");
                Xwriter.WriteAttributeString("name", infotype.Name);
                Xwriter.WriteAttributeString("id", infotype.InfoId);

                Xwriter.WriteStartElement("description");
                Xwriter.WriteElementString("p", infotype.Name);
                Xwriter.WriteEndElement();

                Xwriter.WriteStartElement("information-type-id");
                Xwriter.WriteAttributeString("system", "nist");
                Xwriter.WriteValue(infotype.InfoTypeSytemId);
                Xwriter.WriteEndElement();


                Xwriter.WriteStartElement("confidentiality-impact");
                Xwriter.WriteElementString("base", infotype.ConfidentialityImpactBase);
                Xwriter.WriteElementString("selected", infotype.ConfidentialityImpactSelected);
                Xwriter.WriteEndElement();


                Xwriter.WriteStartElement("integrity-impact");
                Xwriter.WriteElementString("base", infotype.IntegrityImpactBase);
                Xwriter.WriteElementString("selected", infotype.IntegrityImpactSelected);
                Xwriter.WriteEndElement();



                Xwriter.WriteStartElement("availability-impact");
                Xwriter.WriteElementString("base", infotype.AvailabilityImpactBase);
                Xwriter.WriteElementString("selected", infotype.AvailabilityImpactSelected);
                Xwriter.WriteEndElement();


                Xwriter.WriteEndElement();

            }
            Xwriter.WriteEndElement();

            Xwriter.WriteStartElement("security-impact-level");




            var Confidentiality = GetData("security-objective-confidentiality", "<system-characteristics><security-impact-level><security-objective-confidentiality>", UserName, DOID);
            Xwriter.WriteElementString("security-objective-confidentiality", Confidentiality);

            var Integrity = GetData("security-objective-integrity", "<system-characteristics><security-impact-level><security-objective-integrity>", UserName, DOID);
            Xwriter.WriteElementString("security-objective-integrity", Integrity);

            var Availability = GetData("security-objective-availability", "<system-characteristics><security-impact-level><security-objective-availability>", UserName, DOID);
            Xwriter.WriteElementString("security-objective-availability", Availability);

            Xwriter.WriteEndElement();

            var State = GetData("status state", UserName, DOID);
            Xwriter.WriteStartElement("status");
            Xwriter.WriteAttributeString("state", State);
            Xwriter.WriteEndElement();

            //var SecurityAuthIal = GetData("security-auth-ial", "<system-characteristics><security-eauth><security-auth-ial>", UserName, DOID);
            //var SecurityAuthAal = GetData("security-auth-aal", "<system-characteristics><security-eauth><security-auth-aal>", UserName, DOID);
            //var SecurityAuthFal = GetData("security-auth-fal", "<system-characteristics><security-eauth><security-auth-fal>", UserName, DOID);
            //var SecurityEauthLevel = GetData("security-eauth-level", "<system-characteristics><security-eauth><security-eauth-level>", UserName, DOID);

            //Xwriter.WriteStartElement("security-eauth");
            //Xwriter.WriteElementString("security-auth-ial", SecurityAuthIal);
            //Xwriter.WriteElementString("security-auth-aal", SecurityAuthAal);
            //Xwriter.WriteElementString("security-auth-fal", SecurityAuthFal);
            //Xwriter.WriteElementString("security-eauth-level", SecurityEauthLevel);
            //Xwriter.WriteEndElement();





            //var DeploymentModelType = GetData("deployment-model type", UserName, DOID);


            //var ServiceModelType = GetData("service-model type", UserName, DOID);



            //Xwriter.WriteStartElement("deployment-model");
            //Xwriter.WriteAttributeString("type", DeploymentModelType);
            //Xwriter.WriteEndElement();

            //Xwriter.WriteStartElement("service-model");
            //Xwriter.WriteAttributeString("type", ServiceModelType);
            //Xwriter.WriteEndElement();

            Xwriter.WriteStartElement("authorization-boundary");

            Xwriter.WriteStartElement("description");
            Xwriter.WriteElementString("p", "");
            Xwriter.WriteEndElement();

            Xwriter.WriteEndElement();

            Xwriter.WriteEndElement();
        }


        private void GenerateSystemCharacteristics()
        {
            var tag = "<system-characteristics>";
            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(1), tag);
            var SystemID = GetData("system-id", "<system-characteristics><system-id>", UserName, DOID);
            var SysName = GetData("system-name", "<system-characteristics><system-name>", UserName, DOID);
            var SystemNameShort = GetData("system-name-short", "<system-characteristics><system-name-short>", UserName, DOID);
            SecuritySensitivityLevel = GetData("security-sensitivity-level", "<system-characteristics><security-sensitivity-level>", UserName, DOID);

            sw.WriteLine(string.Format("{0}<system-id>{1}</system-id>", GetEmptyTabSpaces(2), SystemID));
            sw.WriteLine(string.Format("{0}<system-name>{1}</system-name>", GetEmptyTabSpaces(2), SysName));
            sw.WriteLine(string.Format("{0}<system-name-short>{1}</system-name-short>", GetEmptyTabSpaces(2), SystemNameShort));
            sw.WriteLine(string.Format("{0}<security-sensitivity-level>{1}</security-sensitivity-level>", GetEmptyTabSpaces(2), SecuritySensitivityLevel));

            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(1), "</system-characteristics>");

            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(2), "<system-information>");
            var parts = GetSystemInfoType();
            foreach (var infotype in parts)
            {
                sw.WriteLine(string.Format("{0}<information-type name=\"{1}\" id=\"{2}\">", GetEmptyTabSpaces(3), infotype.Name, infotype.InfoId));
                sw.WriteLine(string.Format("{0}<description>", GetEmptyTabSpaces(4)));
                sw.WriteLine(string.Format("{0}<p>{1}</p>", GetEmptyTabSpaces(5), infotype.Name));
                sw.WriteLine(string.Format("{0}</description>", GetEmptyTabSpaces(4)));
                sw.WriteLine(string.Format("{0}<information-type-id system=\"nist\">{1}</information-type-id >", GetEmptyTabSpaces(4), infotype.InfoTypeSytemId));
                sw.WriteLine(string.Format("{0}<confidentiality-impact>", GetEmptyTabSpaces(4)));
                sw.WriteLine(string.Format("{0}<base>{1}</base>", GetEmptyTabSpaces(5), infotype.ConfidentialityImpactBase));
                sw.WriteLine(string.Format("{0}<selected>{1}</selected>", GetEmptyTabSpaces(5), infotype.ConfidentialityImpactSelected));
                sw.WriteLine(string.Format("{0}</confidentiality-impact>", GetEmptyTabSpaces(4)));

                sw.WriteLine(string.Format("{0}<integrity-impact>", GetEmptyTabSpaces(4)));
                sw.WriteLine(string.Format("{0}<base>{1}</base>", GetEmptyTabSpaces(5), infotype.IntegrityImpactBase));
                sw.WriteLine(string.Format("{0}<selected>{1}</selected>", GetEmptyTabSpaces(5), infotype.IntegrityImpactSelected));
                sw.WriteLine(string.Format("{0}</integrity-impact>", GetEmptyTabSpaces(4)));

                sw.WriteLine(string.Format("{0}<availability-impact>", GetEmptyTabSpaces(4)));
                sw.WriteLine(string.Format("{0}<base>{1}</base>", GetEmptyTabSpaces(5), infotype.AvailabilityImpactBase));
                sw.WriteLine(string.Format("{0}<selected>{1}</selected>", GetEmptyTabSpaces(5), infotype.AvailabilityImpactSelected));
                sw.WriteLine(string.Format("{0}</availability-impact>", GetEmptyTabSpaces(4)));

                sw.WriteLine(string.Format("{0}</information-type>", GetEmptyTabSpaces(3), infotype.InfoTypeSytemName, infotype.InfoId));



            }
            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(2), "</system-information>");

            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(2), "<security-impact-level>");
            var Confidentiality = GetData("security-objective-confidentiality", "<system-characteristics><security-impact-level><security-objective-confidentiality>", UserName, DOID);
            sw.WriteLine(string.Format("{0}<security-objective-confidentiality>{1}</security-objective-confidentiality>", GetEmptyTabSpaces(3), Confidentiality));
            var Integrity = GetData("security-objective-integrity", "<system-characteristics><security-impact-level><security-objective-integrity>", UserName, DOID);
            sw.WriteLine(string.Format("{0}<security-objective-integrity>{1}</security-objective-integrity>", GetEmptyTabSpaces(3), Integrity));
            var Availability = GetData("security-objective-availability", "<system-characteristics><security-impact-level><security-objective-availability>", UserName, DOID);
            sw.WriteLine(string.Format("{0}<security-objective-availability>{1}</security-objective-availability>", GetEmptyTabSpaces(3), Availability));
            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(2), "</security-impact-level>");

            var SecurityAuthIal = GetData("security-auth-ial", "<system-characteristics><security-eauth><security-auth-ial>", UserName, DOID);
            var SecurityAuthAal = GetData("security-auth-aal", "<system-characteristics><security-eauth><security-auth-aal>", UserName, DOID);
            var SecurityAuthFal = GetData("security-auth-fal", "<system-characteristics><security-eauth><security-auth-fal>", UserName, DOID);
            var SecurityEauthLevel = GetData("security-eauth-level", "<system-characteristics><security-eauth><security-eauth-level>", UserName, DOID);

            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(2), "<security-eauth>");
            sw.WriteLine(string.Format("{0}<security-auth-ial>{1}</security-auth-ial>", GetEmptyTabSpaces(3), SecurityAuthIal));
            sw.WriteLine(string.Format("{0}<security-auth-aal>{1}</security-auth-aal>", GetEmptyTabSpaces(3), SecurityAuthAal));
            sw.WriteLine(string.Format("{0}<security-auth-fal>{1}</security-auth-fal>", GetEmptyTabSpaces(3), SecurityAuthFal));
            sw.WriteLine(string.Format("{0}<security-eauth-level>{1}</security-eauth-level>", GetEmptyTabSpaces(3), SecurityEauthLevel));
            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(2), "</security-eauth>");

            var State = GetData("status state", UserName, DOID);


            var DeploymentModelType = GetData("deployment-model type", UserName, DOID);


            var ServiceModelType = GetData("service-model type", UserName, DOID);


            sw.WriteLine("{0}<status state=\"{1}\"/>", GetEmptyTabSpaces(2), State);
            sw.WriteLine("{0}<deployment-model type=\"{1}\"/>", GetEmptyTabSpaces(2), DeploymentModelType);
            sw.WriteLine("{0}<service-model type=\"{1}\"/>", GetEmptyTabSpaces(2), ServiceModelType);
        }



        private void GenerateMetadata()
        {
            var tag = "<metadata>";
            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(1), tag);

            var title = GetData("title", "<metadata><title>", UserName, DOID);
            sw.WriteLine("{0}<title>{1}</title>", GetEmptyTabSpaces(2), title);

            string VersionDateString = GetData("last-modified", "<metadata><last-modified>", UserName, DOID);
            sw.WriteLine("{0}<last-modified>{1}</last-modified>", GetEmptyTabSpaces(2), VersionDateString);

            string Version = GetData("version", "<metadata><version>", UserName, DOID);
            sw.WriteLine("{0}<version>{1}</version>", GetEmptyTabSpaces(2), Version);

            string OSCALVersion = GetData("oscal-version", "<metadata><oscal-version>", UserName, DOID);
            sw.WriteLine("{0}<oscal-version>{1}</oscal-version>", GetEmptyTabSpaces(2), OSCALVersion);

            sw.WriteLine(string.Format("{0}<prop name=\"sensitivity-label\">Company Sensitive and Proprietary</prop>", GetEmptyTabSpaces(2)));

            var roles = GetRoles();

            foreach (var role in roles)
            {
                var tagRole = string.Format("<role id=\"{0}\">", role.RoleID);
                sw.WriteLine(string.Format("{0}{1}", GetEmptyTabSpaces(2), tagRole));
                sw.WriteLine(string.Format("{0}<title>{1}</title>", GetEmptyTabSpaces(3), role.Title.Value));
                sw.WriteLine(string.Format("{0}<desc>{1}</desc>", GetEmptyTabSpaces(3), role.Description));
                sw.WriteLine(string.Format("{0}{1}", GetEmptyTabSpaces(2), "</role>"));
            }


            var part = GetParties();

            //   var fullparties = FillCompanyInfo(part);

            foreach (var party in part)
            {
                var tagParty = string.Format("<party id=\"{0}\">", party.PartyID);
                sw.WriteLine(string.Format("{0}{1}", GetEmptyTabSpaces(2), tagParty));
                if (party.IsAPerson)
                {
                    sw.WriteLine(string.Format("{0}{1}", GetEmptyTabSpaces(3), "<person>"));
                    sw.WriteLine(string.Format("{0}<person-name>{1}</person-name>", GetEmptyTabSpaces(4), party.PersonName));
                }
                else
                {
                    sw.WriteLine(string.Format("{0}{1}", GetEmptyTabSpaces(3), "<org>"));
                    sw.WriteLine(string.Format("{0}<org-name>{1}</org-name>", GetEmptyTabSpaces(4), party.OrgName));
                    sw.WriteLine(string.Format("{0}<short-name>{1}</short-name>", GetEmptyTabSpaces(4), party.ShortName));

                }

                sw.WriteLine(string.Format("{0}<org-id>{1}</org-id>", GetEmptyTabSpaces(4), party.OrgID));
                sw.WriteLine("{0}{1}", GetEmptyTabSpaces(4), "<address>");
                sw.WriteLine(string.Format("{0}<addr-line>{1}</addr-line>", GetEmptyTabSpaces(5), party.AddressLine1));
                sw.WriteLine(string.Format("{0}<addr-line>{1}</addr-line>", GetEmptyTabSpaces(5), party.AddressLine2));
                sw.WriteLine(string.Format("{0}<city>{1}</city>", GetEmptyTabSpaces(5), party.City));
                sw.WriteLine(string.Format("{0}<state>{1}</state>", GetEmptyTabSpaces(5), party.State));
                sw.WriteLine(string.Format("{0}<postal-code>{1}</postal-code>", GetEmptyTabSpaces(5), party.PostalCode));
                sw.WriteLine(string.Format("{0}<country>{1}</country>", GetEmptyTabSpaces(5), party.Country));
                sw.WriteLine("{0}{1}", GetEmptyTabSpaces(4), "</address>");

                if (party.IsAPerson)
                {
                    sw.WriteLine(string.Format("{0}<email>{1}</email>", GetEmptyTabSpaces(4), party.Email));
                    sw.WriteLine(string.Format("{0}<email>{1}</email>", GetEmptyTabSpaces(4), party.Phone));
                    sw.WriteLine(string.Format("{0}{1}", GetEmptyTabSpaces(3), "</person>"));

                }
                else
                {
                    sw.WriteLine(string.Format("{0}<remarks>", GetEmptyTabSpaces(4)));
                    sw.WriteLine(string.Format("{0}<p>{1}</p>", GetEmptyTabSpaces(5), "None"));
                    sw.WriteLine(string.Format("{0}</remarks>", GetEmptyTabSpaces(4)));
                    sw.WriteLine(string.Format("{0}{1}", GetEmptyTabSpaces(3), "</org>"));
                }

                sw.WriteLine(string.Format("{0}{1}", GetEmptyTabSpaces(2), "</party>"));


            }

            var responParties = GetResponsibleParties();

            foreach (var resparty in responParties)
            {
                var tagResPart = string.Format("<responsible-party role-id=\"{0}\">", resparty.RoleID);
                sw.WriteLine(string.Format("{0}{1}", GetEmptyTabSpaces(2), tagResPart));
                if (resparty.PartyIDs != null)
                    for (int i = 0; i < resparty.PartyIDs.Count; i++)
                    {
                        sw.WriteLine(string.Format("{0}<party-id>{1}</party-id>", GetEmptyTabSpaces(3), resparty.PartyIDs[i]));
                    }
                else
                    sw.WriteLine(string.Format("{0}<party-id>{1}</party-id>", GetEmptyTabSpaces(3), resparty.PartyID));


                sw.WriteLine(string.Format("{0}{1}", GetEmptyTabSpaces(2), "</responsible-party>"));


            }

            sw.WriteLine("{0}{1}", GetEmptyTabSpaces(1), "</metadata>");
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

                    var shortName = string.Format("{0} {1}", nameOnly, "SSP");
                    var fullName = string.Format("{0} {1}", nameOnly, "Security System Plan");

                    var userDocs = GetSystemDocInfo(SystemID);
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
                        DOID = AddDoc(shortName, fullName, SystemID, 3, 1, SystemID);

                    Cache.Insert("doid", DOID);
                    Cache.Insert("userId", UserName);
                    Cache.Insert("selectedFile", fileName);
                    ImportToDB(XMLNamespace, OscalSchemaPath, FileToConvertPath, DOID, SystemID);
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

                    Response.Redirect(@"~/PageSSP/Metadata/Title.aspx", false);
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
                        DOID = AddDoc(shortName, fullName, SystemID, 3, 1, SystemID);



                    Cache.Insert("doid", DOID);
                    Cache.Insert("userId", UserName);
                    Cache.Insert("selectedFile", FileName);
                    Response.Redirect(@"~/PageSSP/Metadata/Title.aspx", false);

                    return;
                }

            }
            catch (Exception Ex)
            {
                StatusLabel.ForeColor = Color.Red;
                StatusLabel.Text = Ex.Message;
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

        private void XMLImportProfile()
        {
            Xwriter.WriteStartElement("import-profile");
            //var href = GetData("href", "<import-ssp href>", UserName, DOID);
            Xwriter.WriteAttributeString("href", "https://pages.nist.gov/OSCAL/documentation/schema/profile-layer/profile/");
            // var remarks = GetData("remarks", "<import-ssp href><remarks>", UserName, DOID);
            Xwriter.WriteStartElement("remarks");
            Xwriter.WriteElementString("p", "This is where to get OSCAL Profile.");
            Xwriter.WriteEndElement();
            Xwriter.WriteEndElement();

        }

        void Milestone3GenerateXMLDoc()
        {
            FileName = FileName.Replace(" ", "");
            FilePath = string.Format(@"{0}Uploads\{1}.xml", AppPath, FileName);
            XmlWriterSettings = new XmlWriterSettings();
            XmlWriterSettings.Indent = true;
            XmlWriterSettings.IndentChars = "\t";

            Xwriter = XmlWriter.Create(FilePath, XmlWriterSettings);

            X = Xwriter;

            // Xwriter.WriteRaw("<?xml-stylesheet type=\"text/xsl\" href=\"FedRAMP-compliance-worksheet.xsl\"?>\n");

            Xwriter.WriteStartElement("system-security-plan", @"http://csrc.nist.gov/ns/oscal/1.0");
            Xwriter.WriteAttributeString("uuid", Guid.NewGuid().ToString());
            //Xwriter.WriteAttributeString("xsi", @"http://www.w3.org/2001/XMLSchema-instance");
            //Xwriter.WriteAttributeString("id", "SSP-sample");
            //Xwriter.WriteAttributeString("schemaLocation", "xsi" , @"http://csrc.nist.gov/ns/oscal/1.0/oscal_ssp_schema.xsd");

            message = string.Format("Start to build the OSCAL Document {0}", FileName);
            percent = 2;
            PrintProgressBar(message, percent, true);

            M3Metadata();

            M3ImportProfile();

            message = string.Format("Rendering the OSCAL Document {0}:  successfully completed Medata ... ", FileName);
            percent = 20;
            PrintProgressBar(message, percent - wordPercent);


            M3SystemCharacteristics();

            message = string.Format("Rendering the OSCAL Document {0}:  successfully completed System Characteristics... ", FileName);
            percent = 35;
            PrintProgressBar(message, percent - wordPercent);

            M3SystemImplementation();
            message = string.Format("Rendering the OSCAL Document {0}:  successfully completed System Implementation, starting Control Implementation... ", FileName);
            percent = 55;
            PrintProgressBar(message, percent - wordPercent);

            XMLControlImplementation();

            message = string.Format("Rendering the OSCAL Document {0}:  successfully completed  Control Implementation... ", FileName);
            percent = 95;
            PrintProgressBar(message, percent - wordPercent);

            M3BackMatter();

            Xwriter.WriteEndElement();
            Xwriter.WriteEndDocument();
            Xwriter.Flush();
            Xwriter.Close();

        }

        void CorrectXMLGenerateXMLDoc()
        {
            FileName = FileName.Replace(" ", "");
            FilePath = string.Format(@"{0}Uploads\{1}.xml", AppPath, FileName);
            XmlWriterSettings = new XmlWriterSettings();
            XmlWriterSettings.Indent = true;
            XmlWriterSettings.IndentChars = "\t";

            Xwriter = XmlWriter.Create(FilePath, XmlWriterSettings);


            // Xwriter.WriteRaw("<?xml-stylesheet type=\"text/xsl\" href=\"FedRAMP-compliance-worksheet.xsl\"?>\n");

            Xwriter.WriteStartElement("system-security-plan", @"http://csrc.nist.gov/ns/oscal/1.0");
            Xwriter.WriteAttributeString("id", Guid.NewGuid().ToString());
            //Xwriter.WriteAttributeString("xsi", @"http://www.w3.org/2001/XMLSchema-instance");
            //Xwriter.WriteAttributeString("id", "SSP-sample");
            //Xwriter.WriteAttributeString("schemaLocation", "xsi" , @"http://csrc.nist.gov/ns/oscal/1.0/oscal_ssp_schema.xsd");

            message = string.Format("Start to build the OSCAL Document {0}", FileName);
            percent = 2;
            PrintProgressBar(message, percent, true);

            XMLMetadata();

            XMLImportProfile();

            message = string.Format("Rendering the OSCAL Document {0}:  successfully completed Medata ... ", FileName);
            percent = 20;
            PrintProgressBar(message, percent - wordPercent);


            XMLSystemCharacteristics();

            message = string.Format("Rendering the OSCAL Document {0}:  successfully completed System Characteristics... ", FileName);
            percent = 35;
            PrintProgressBar(message, percent - wordPercent);


            XMLSystemImplementation();


            message = string.Format("Rendering the OSCAL Document {0}:  successfully completed System Implementation, starting Control Implementation... ", FileName);
            percent = 55;
            PrintProgressBar(message, percent - wordPercent);


            XMLControlImplementation();

            message = string.Format("Rendering the OSCAL Document {0}:  successfully completed  Control Implementation... ", FileName);
            percent = 95;
            PrintProgressBar(message, percent - wordPercent);


            XMLBackMatter();



            Xwriter.WriteEndElement();
            Xwriter.WriteEndDocument();
            Xwriter.Flush();
            Xwriter.Close();

        }

        void GenerateXMLDoc()
        {
            FileName = FileName.Replace(" ", "");
            FilePath = string.Format(@"{0}\Uploads\{1}.xml", AppPath, FileName);
            sw = new StreamWriter(FilePath);
            sb = new StringBuilder();



            sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sw.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"FedRAMP-compliance-worksheet.xsl\"?>");
            sw.WriteLine("<system-security-plan xmlns=\"http://csrc.nist.gov/ns/oscal/1.0\"");
            sw.WriteLine(string.Format("{0}{1}", GetEmptyTabSpaces(5), "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\""));
            sw.WriteLine(string.Format("{0}{1}", GetEmptyTabSpaces(5), "id=\"SSP-sample\""));
            sw.WriteLine(string.Format("{0}{1}", GetEmptyTabSpaces(5), "xsi:schemaLocation=\"http://csrc.nist.gov/ns/oscal/1.0/oscal_ssp_schema.xsd\">"));
            message = string.Format("Start to build the OSCAL Document {0}", FileName);
            percent = 2;
            PrintProgressBar(message, percent, true);

            GenerateMetadata();

            message = string.Format("Rendering the OSCAL Document {0}:  sucessfully completed Medata ... ", FileName);
            percent = 20;
            PrintProgressBar(message, percent - wordPercent);


            GenerateSystemCharacteristics();

            message = string.Format("Rendering the OSCAL Document {0}:  sucessfully completed System Characteristics... ", FileName);
            percent = 35;
            PrintProgressBar(message, percent - wordPercent);


            GenerateSystemImplementation();

            message = string.Format("Rendering the OSCAL Document {0}:  sucessfully completed System Implementation, starting Control Implementation... ", FileName);
            percent = 55;
            PrintProgressBar(message, percent - wordPercent);


            GenerateControlImplementation();

            message = string.Format("Rendering the OSCAL Document {0}:  sucessfully completed  Control Implementation... ", FileName);
            percent = 95;
            PrintProgressBar(message, percent - wordPercent);


            GenerateBackMatter();
            sw.WriteLine("</system-security-plan>");

            sw.Close();
            // FinalProcessing();
        }

        protected void OSCALButton_Click(object sender, EventArgs e)
        {
            try
            {

                Cache.Remove("XMLPath");
                FileName = DocumentDropDownList.SelectedValue;
                SetDOID();

                OpenFileButton.Visible = false;

                GetHttpStream();

                // CorrectXMLGenerateXMLDoc();
                Milestone3GenerateXMLDoc();

                FileName = FileName.Replace(" ", "");
                string filename = FileName + ".xml";
                var ErrorFilePath = string.Format(@"{0}Uploads\{1}ValidationErrors.txt", AppPath, "SSP");

                //  GenerateXMLDoc();
                string xmlSchemaPath = HttpContext.Current.Server.MapPath(string.Format(@"~\Templates\{0}", SSP3schema));
                string XmlDocumentPath = HttpContext.Current.Server.MapPath(string.Format(@"~\Uploads\{0}", filename));

                XMLValidator.RunValidator(XmlDocumentPath, xmlSchemaPath, ErrorFilePath);

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
                var path = string.Format(@"{0}Uploads\{1}ValidationErrors.txt", AppPath, "SSP"); ;
                var err = GetError(path);
                StatusLabel.Text = ex.Message + "\n" + err;
            }

        }

        void FinalProcessing()
        {
            var sr = new StreamReader(FilePath);
            var map = sr.ReadToEnd();
            map = map.Replace("&", "&amp;");
            sr.Close();

            sw = new StreamWriter(FilePath);
            sw.Write(map);
            sw.Close();


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

        void M3FillControl(string controlId)
        {

            var ctrId = controlId.Replace(" ", "");

            Xwriter.WriteStartElement("implemented-requirement");


            var tagCtruuid = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\">", ctrId);
            var preguid = GetData("uuid", tagCtruuid, UserName, DOID);
            var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;
            X.WriteAttributeString("uuid", guid);
            Xwriter.WriteAttributeString("control-id", ctrId);

            var tag = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><prop  name=\"implementation-status\">", ctrId);

            var impleStatus = GetDBDataGivenTag(tag, UserName, DOID);

            for (int i = 0; i < impleStatus.Count; i++)
            {
                foreach (var x in impleStatus[i])
                {
                    var ex = x;
                    ex = ex.Trim();
                    if (ex.Length == 0)
                        continue;
                    Xwriter.WriteStartElement("prop");
                    Xwriter.WriteAttributeString("name", "implementation-status");
                    Xwriter.WriteValue(ex);
                    Xwriter.WriteEndElement();
                }
            }
            var oriTag = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><prop  name=\"control-origination\">", ctrId);

            var origination = GetDBDataGivenTag(oriTag, UserName, DOID);
            for (int j = 0; j < origination.Count; j++)
            {
                foreach (var y in origination[j])
                {
                    var ey = y;
                    ey = ey.Trim();
                    if (ey.Length == 0)
                        continue;

                    Xwriter.WriteStartElement("prop");
                    Xwriter.WriteAttributeString("name", "control-origination");
                    Xwriter.WriteValue(ey);
                    Xwriter.WriteEndElement();
                }
            }

            var resRoles = ControlResponsibleRole(ctrId);

            for (int i = 0; i < resRoles.Count; i++)
            {
                Xwriter.WriteStartElement("responsible-role");
                Xwriter.WriteAttributeString("role-id", resRoles[i].XPath);
                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", resRoles[i].Value);
                Xwriter.WriteEndElement();
                Xwriter.WriteEndElement();

            }

            var index = ControlIdToRank[controlId];
            foreach (var para in ParameterIDs[index])
            {

                var UpperControlId = ctrId.ToUpper();
                var paraC = para.Replace("Parameter ", "");
                var paraD = paraC.Replace(UpperControlId, "");
                var goodpara = string.Format("{0}_prm{1}", ctrId, paraD);
                goodpara = goodpara.Replace("(", "_");
                goodpara = goodpara.Replace(")", "");
                goodpara = goodpara.Replace(":", "");
                goodpara = goodpara.Replace(" ", "");
                var tagpara = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><set-param  param-id=\"{1}\"><value>", ctrId, goodpara);

                var ParameterValue = GetData("value", tagpara, UserName, DOID);

                Xwriter.WriteStartElement("set-parameter");
                Xwriter.WriteAttributeString("param-id", goodpara);
                Xwriter.WriteElementString("value", ParameterValue);
                Xwriter.WriteEndElement();

            }

            var w = StatementIDs[index].Count <= 1 ? 0 : 1;

            for (int i = w; i < StatementIDs[index].Count; i++)
            {

                var statC = StatementIDs[index][i].Replace("Part ", "");
                var goodstat = string.Format("{0}_stmt.{1}", ctrId, statC);
                goodstat = goodstat.Replace(" ", "");
                goodstat = goodstat.Replace("?", "");
                goodstat = goodstat.Replace("(", ".");
                goodstat = goodstat.Replace(")", "");

                var tagstat = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><statement  statement-id=\"{1}\">", ctrId, goodstat);
                var StatementV = GetData(goodstat, tagstat, UserName, DOID);
                var tagstatuuid = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><statement  statement-id=\"{1}\"><uuid>", ctrId, goodstat);
                var prestatguid = GetData("uuid", tagstatuuid, UserName, DOID);
                var statguid = prestatguid.Length == 0 ? Guid.NewGuid().ToString() : prestatguid;


                Xwriter.WriteStartElement("statement");
                Xwriter.WriteAttributeString("statement-id", goodstat);
                X.WriteAttributeString("uuid", statguid);
                Xwriter.WriteStartElement("description");
                Xwriter.WriteElementString("p", StatementV);
                Xwriter.WriteEndElement();
                Xwriter.WriteEndElement();

            }

            Xwriter.WriteEndElement();
        }


        void FillControl(string controlId)
        {

            var ctrId = controlId.Replace(" ", "");

            Xwriter.WriteStartElement("implemented-requirement");
            Xwriter.WriteAttributeString("control-id", ctrId);

            var tag = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><prop  name=\"implementation-status\">", ctrId);

            var impleStatus = GetDBDataGivenTag(tag, UserName, DOID);

            for (int i = 0; i < impleStatus.Count; i++)
            {
                foreach (var x in impleStatus[i])
                {
                    var ex = x;
                    ex = ex.Trim();
                    if (ex.Length == 0)
                        continue;
                    Xwriter.WriteStartElement("prop");
                    Xwriter.WriteAttributeString("name", "implementation-status");
                    Xwriter.WriteValue(ex);
                    Xwriter.WriteEndElement();
                }
            }
            var oriTag = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><prop  name=\"control-origination\">", ctrId);

            var origination = GetDBDataGivenTag(oriTag, UserName, DOID);
            for (int j = 0; j < origination.Count; j++)
            {
                foreach (var y in origination[j])
                {
                    var ey = y;
                    ey = ey.Trim();
                    if (ey.Length == 0)
                        continue;

                    Xwriter.WriteStartElement("prop");
                    Xwriter.WriteAttributeString("name", "control-origination");
                    Xwriter.WriteValue(ey);
                    Xwriter.WriteEndElement();
                }
            }

            var resRoles = ControlResponsibleRole(ctrId);

            for (int i = 0; i < resRoles.Count; i++)
            {
                Xwriter.WriteStartElement("responsible-role");
                Xwriter.WriteAttributeString("role-id", resRoles[i].XPath);
                Xwriter.WriteStartElement("remarks");
                Xwriter.WriteElementString("p", resRoles[i].Value);
                Xwriter.WriteEndElement();
                Xwriter.WriteEndElement();

            }

            var index = ControlIdToRank[controlId];
            foreach (var para in ParameterIDs[index])
            {

                var UpperControlId = ctrId.ToUpper();
                var paraC = para.Replace("Parameter ", "");
                var paraD = paraC.Replace(UpperControlId, "");
                var goodpara = string.Format("{0}_prm{1}", ctrId, paraD);
                goodpara = goodpara.Replace("(", "_");
                goodpara = goodpara.Replace(")", "");
                goodpara = goodpara.Replace(":", "");
                goodpara = goodpara.Replace(" ", "");
                var tagpara = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><set-param  param-id=\"{1}\"><value>", ctrId, goodpara);

                var ParameterValue = GetData("value", tagpara, UserName, DOID);

                Xwriter.WriteStartElement("set-param");
                Xwriter.WriteAttributeString("param-id", goodpara);
                Xwriter.WriteElementString("value", ParameterValue);
                Xwriter.WriteEndElement();

            }

            var w = StatementIDs[index].Count <= 1 ? 0 : 1;

            for (int i = w; i < StatementIDs[index].Count; i++)
            {

                var statC = StatementIDs[index][i].Replace("Part ", "");
                var goodstat = string.Format("{0}_stmt.{1}", ctrId, statC);
                goodstat = goodstat.Replace(" ", "");
                goodstat = goodstat.Replace("?", "");
                goodstat = goodstat.Replace("(", ".");
                goodstat = goodstat.Replace(")", "");

                var tagstat = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><statement  statement-id=\"{1}\">", ctrId, goodstat);
                var StatementV = GetData(goodstat, tagstat, UserName, DOID);

                Xwriter.WriteStartElement("statement");
                Xwriter.WriteAttributeString("statement-id", goodstat);
                Xwriter.WriteStartElement("description");
                Xwriter.WriteElementString("p", StatementV);
                Xwriter.WriteEndElement();
                Xwriter.WriteEndElement();

            }

            Xwriter.WriteEndElement();
        }


        private List<ControlParamStatement> GetIds()
        {
            var result = new List<ControlParamStatement>();
            ControlIDs = new List<string>();
            ControlIdToRank = new Dictionary<string, int>();
            ParameterIDs = new List<List<string>>();
            StatementIDs = new List<List<string>>();


            string appPath = Request.PhysicalApplicationPath;
            var file = "";



            switch (SecuritySensitivityLevel)
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
                    var parames = new List<string>();
                    var statements = new List<string>();
                    var tepo = new ControlParamStatement();
                    var line = sr.ReadLine();
                    var index = line.IndexOf(",");
                    var controlid = line.Substring(0, index);

                    tepo.ControlID = controlid;
                    ControlIDs.Add(controlid);
                    ControlIdToRank.Add(controlid, n);
                    n++;
                    line = line.Remove(0, index + 3);
                    int t = line.IndexOf('@');
                    var par = line.Substring(0, t);
                    var end = t > 0 ? line.Replace(par, "") : "";

                    end = end.Length > 0 ? end.Replace("@ #", "") : "";
                    end = end.Length > 0 ? end.Replace("@", "") : "";


                    while (par.IndexOf('*') > 0)
                    {
                        var into = par.IndexOf('*');
                        var para = par.Substring(0, into);
                        par = par.Remove(0, into + 1);

                        parames.Add(para);
                    }
                    if (par.Length > 0)
                    {
                        var ee = par.Replace("*", "");
                        parames.Add(ee);
                    }
                    tepo.ParamIDs = parames;
                    ParameterIDs.Add(parames);

                    while (end.IndexOf('#') > 0)
                    {
                        var intot = end.IndexOf('#');
                        var stat = end.Substring(0, intot);
                        end = end.Remove(0, intot + 1);
                        statements.Add(stat);
                    }
                    if (end.Length > 0)
                    {
                        var xx = end.Replace("#", "");
                        statements.Add(xx);
                    }
                    tepo.StatementIDs = statements;
                    StatementIDs.Add(statements);

                    result.Add(tepo);
                }

            }



            return result;
        }

        protected void InitFileConversion()
        {

            if (FileUpload1.HasFile)
            {
                Id = Guid.NewGuid();
                var FileData = FileUpload1.PostedFile;
                var fileName = FileUpload1.FileName;

                var extension = Path.GetExtension(fileName);

                var nameOnly = fileName.Replace(extension, "");
                //FileToConvertPath = HttpContext.Current.Server.MapPath(string.Format(@"~\Uploads\{0}{1}.xml", Id, nameOnly));
                //FileData.SaveAs(FileToConvertPath);
            }

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

            UserDocuments = GetSystemDocFullName(SystemID, 3);
            DocInfos = GetSystemDocInfo(SystemID, 3);

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

        protected void WordDocButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cache.Remove("OSCAL");

                wordPercent = 10;
                FilePath = Cache["XMLPath"] != null ? Cache["XMLPath"].ToString() : "";
                //if (FilePath.Length == 0 && Cache["selectedFile"] == null)
                //{
                //    OSCALButton_Click(sender, e);

                //}
                //else
                //{
                //    GetHttpStream();
                //    percent = 80;
                //    PrintProgressBar(message, percent, true);
                //}


                FileName = Cache["selectedFile"].ToString();
                FileName = FileName.Replace(".xml", "");
                SetDOID();

                FileName = FileName.Replace(" ", "");
                string myXMLElement = "";


                OpenFileButton.Visible = false;

                GetHttpStream();

                CorrectXMLGenerateXMLDoc();



                string filename = FileName + ".xml";
                //  PrintProgressBar("", 0);

                message = string.Format("Starting the conversion to Word of the OSCAL SSP {0}", filename);
                percent = 85;
                PrintProgressBar(message, percent);



                myXMLElement = GetXMLElement(filename, "security-sensitivity-level");
                if (myXMLElement == "low")
                {
                    TemplateFile = "FedRAMP-SSP-Low-Baseline-Template.docx";
                    BaselinePropCountFile = "LowBaselineControlsToPropCount.txt";
                }
                else if (myXMLElement == "high")
                {
                    TemplateFile = "FedRAMP-SSP-High-Baseline-Template.docx";
                    BaselinePropCountFile = "HighBaselineControlsToPropCount.txt";
                }

                string wordDocumentPath;
                string xmlSchemaPath = HttpContext.Current.Server.MapPath(string.Format(@"~\Templates\{0}", SSPschema));
                string XmlDocumentPath = HttpContext.Current.Server.MapPath(string.Format(@"~\Uploads\{0}", filename));

                XMLValidator.PseudoValidator(XmlDocumentPath, xmlSchemaPath);
                message = string.Format("Conversion of {0}: Successful validation of the xml file  {0} against OSCAL schema with namespace {1}", FileName, XMLNamespace);
                percent = 90;
                PrintProgressBar(message, percent);


                ProcessData(filename, TemplateFile, out wordDocumentPath);

                message = string.Format("Successfully mapped the Metadata, System Characteristics and System Implementation Data using basic XML Mappings");
                percent = 99;
                PrintProgressBar(message, percent);


                if (File.Exists(WordTempFilePath))
                {
                    File.Delete(WordTempFilePath);
                }

                //  StatusLabel.Text = "Processing Complete.. Click below to open file.";
                mainbarServer.Style["width"] = "100%";

                CollapseDiv("Form");

                OpenFileButton.Visible = true;
                Cache["outputFile"] = TemplateFile.Replace("Template", "OSCAL");

                //Response.Redirect(string.Format(@"/Downloads/{0}", TemplateFile.Replace("Template", "OSCAL")),false);

            }
            catch (Exception ex)
            {
                CollapseDiv("Form");

                StatusLabel.ForeColor = System.Drawing.Color.Red;
                StatusLabel.Text = "OSCAL Vailidation Error. The following error occured: " + ex.Message + " " + string.Format("XML File Path: {0}", FilePath);



                if (File.Exists(WordTempFilePath))
                {
                    File.Delete(WordTempFilePath);
                }

            }




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

    }
}

