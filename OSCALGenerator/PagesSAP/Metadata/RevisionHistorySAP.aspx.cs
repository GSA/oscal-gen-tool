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
using OSCALHelperClasses;
using OSCALGenerator.Pages;

namespace OSCALGenerator.PagesSAP.Metadata
{
    public partial class RevisionHistorySAP : BasePage
    {
        int Width = 198;
        int Height = 400;
        int AddOn = 125;
        List<DocRevision> Revisions;
      
        protected new void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ErrorCache();

                UserName = Cache["username"].ToString();
                DOID = (int)Cache["doid"];
                SystemID = int.Parse(Cache["SystemId"].ToString());

                ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
                DAL = new CSharpDAL(ConnString);
                Revisions = GetRevisions();
                AddRevisionPanel.Visible = false;
                MainDiv.Visible = false;
                AddPropPanel.Visible = false;
                MainDiv.Visible = false;
                var bindingRevision = new BindingList<DocRevision>(Revisions);
                RevisionGridView.DataSource = bindingRevision;
                RevisionGridView.DataBind();

                GridviewPanel.Visible = true;

                AddOn = (LastModifiedDateCalendar.Visible && PublishedDateCalendar.Visible) ? 285 : 125;

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
                if (!IsPostBack)
                {
                    Errors = "";
                }

            }
            catch (Exception ex)
            {
                StatusLabel.BackColor = Color.Red;
                StatusLabel.Text = ex.Message;
            }
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAP/Metadata/Title.aspx", false);
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {

            Response.Redirect(@"~/PagesSAP/Metadata/DocID.aspx", false);
        }

        private string GenerateRevisionID()
        {
            var count = GetAllHeaders("revision id, revision id", UserName, DOID).Count;
           
            var id = string.Format("revision-{0}", count);
            return id;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            Errors = "";
            StatusLabel.Text = Errors;
            string revisionId;
            if (RevisionDropDownList.SelectedIndex > 0)
                revisionId = RevisionDropDownList.SelectedValue;
            else
            {
                revisionId = GenerateRevisionID();
            }

            var tagRevisionId = string.Format("<metadata><revision id=\"{0}\">", revisionId);

            var title = this.TitleTextBox.Text;
            var tagTitle = string.Format("<metadata><revision id=\"{0}\"><title>", revisionId);


            var published = this.PublishedTextBox.Text;
            var tagPublished = string.Format("<metadata><revision id=\"{0}\"><published>", revisionId);

            var lastModified = this.LastModifiedTextBox.Text;
            var tagLastModified = string.Format("<metadata><revision id=\"{0}\"><last-modified>", revisionId);

            var version = VersionTextBox.Text;
            var tagVersion = string.Format("<metadata><revision id=\"{0}\"><version>", revisionId);

            var oscalversion = OSCALVersionTextBox.Text;
            var tagOscalVersion = string.Format("<metadata><revision id=\"{0}\"><oscal-version>", revisionId);

            var remarks = RoleRemarksTextarea.InnerHtml;
            var tagRemarks = string.Format("<metadata><revision id=\"{0}\"><remarks>", revisionId);

            Errors += InsertElementToDataBase(DOID, SystemID, revisionId, revisionId.GetType(), tagRevisionId, "revision id, revision id", revisionId, 1);
            Errors += InsertElementToDataBase(DOID, SystemID, "title", title.GetType(), tagTitle, "revision id, title", title, 1);

            Errors += InsertElementToDataBase(DOID, SystemID, "published", PublishedTextBox.Text.GetType(), tagPublished, "revision id, published", PublishedTextBox.Text, 1);

            Errors += InsertElementToDataBase(DOID, SystemID, "last-modified", LastModifiedTextBox.Text.GetType(), tagLastModified, "revision id, last-modified", LastModifiedTextBox.Text, 1);

            Errors += InsertElementToDataBase(DOID, SystemID, "version", version.GetType(), tagVersion, "revision id, version", version, 1);

            Errors += InsertElementToDataBase(DOID, SystemID, "oscal-version", oscalversion.GetType(), tagOscalVersion, "revision id, oscal-version", oscalversion, 1);

            Errors += InsertElementToDataBase(DOID, SystemID, "remarks", remarks.GetType(), tagRemarks, "revision id, remarks", remarks, 1);

            if (Errors.Length > 0)
            {
                StatusLabel.Text = Errors;
                StatusLabel.BackColor = Color.Red;
            }

            MainDiv.Visible = false;
            AddRevisionPanel.Visible = false;
            GridviewPanel.Visible = true;
        }
        void SetPropertyPanel()
        {
            AddRevisionPanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = true;
            GridviewPanel.Visible = false;
            NameLabel.Text = "Name";
            ClassLabel.Text = "Class";
            NSLabel.Text = "NS";
            ValueLabel.Text = "Value";
            PropLabel.Text = "Property";
            PropLabel.Enabled = false;
            RemarksLabel.Text = "";
            RemarksTextArea.Visible = false;

            var revisionid = "";
            if (RevisionDropDownList.SelectedIndex > 0)
                revisionid = RevisionDropDownList.SelectedValue;
            else
                revisionid = GenerateRevisionID();

            var props = GetProps(revisionid, "revision id");
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

        protected void RevisionDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateEditPage();

            AddRevisionPanel.Visible = true;
            MainDiv.Visible = true;
            GridviewPanel.Visible = false;
            RevisionDropDownList.Width = Width;
            RevisionDropDownList.Visible = true;
            DeleteButton.Visible = true;
        }

        protected void PublishedDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            PublishedTextBox.Text = PublishedDateCalendar.SelectedDate.ToString("yyyy-MM-ddThh:mm:ssZ");
            PublishedDateCalendar.Visible = false;
            AddRevisionPanel.Visible = true;
            MainDiv.Visible = true;
            AddOn = HSize();
            AddRevisionPanel.Height = Height + AddOn;
        }

        protected void LastModifiedDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            LastModifiedTextBox.Text = LastModifiedDateCalendar.SelectedDate.ToString("yyyy-MM-ddThh:mm:ssZ");
            LastModifiedDateCalendar.Visible = false;

            AddRevisionPanel.Visible = true;
            MainDiv.Visible = true;
            AddOn = HSize();
            AddRevisionPanel.Height = Height + AddOn;
        }

        protected void PropertyDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void SetAnnotationPanel()
        {
            AddRevisionPanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = true;
            GridviewPanel.Visible = false;

            NameLabel.Text = "Name";
            ClassLabel.Visible = false;
            ClassTextBox.Visible = false;

            NSLabel.Text = "NS";
            ValueLabel.Text = "Value";
            RemarksLabel.Text = "Remarks";
            RemarksTextArea.Visible = true;
            PropLabel.Text = "Annotation";
            PropLabel.Enabled = false;

            var revisionid = "";
            if (RevisionDropDownList.SelectedIndex > 0)
                revisionid = RevisionDropDownList.SelectedValue;
            else
                revisionid = GenerateRevisionID();

            var annotations = GetAnnotations(revisionid, "revision id");
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

        protected void AnnotationButton_Click(object sender, EventArgs e)
        {
            SetAnnotationPanel();

        }



        void SetLinkPanel()
        {
            AddRevisionPanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = true;
            GridviewPanel.Visible = false;
            NameLabel.Text = "href";
            ClassLabel.Text = "rel";
            NSLabel.Text = "media-type";
            ValueLabel.Text = "Value";
            RemarksLabel.Text = "";
            RemarksTextArea.Visible = false;
            PropLabel.Text = "Link";
            PropLabel.Enabled = false;
            ClassLabel.Visible = true;
            ClassTextBox.Visible = true;

            var revisionid = "";
            if (RevisionDropDownList.SelectedIndex > 0)
                revisionid = RevisionDropDownList.SelectedValue;
            else
                revisionid = GenerateRevisionID();


            var links = GetLinks(revisionid, "revision id");
           

            var allLinks = new List<Link> { new Link() };
            allLinks.AddRange(links);
            
            PropDropDownList.DataSource = allLinks;
            PropDropDownList.DataValueField = "ParentID";
            PropDropDownList.DataTextField = "ParentID";
            PropDropDownList.DataBind();

            PopulateLinkEditPage();

            LinkDropDownList.DataSource = links;
            LinkDropDownList.DataValueField = "HRef";
            LinkDropDownList.DataTextField = "HRef";
            LinkDropDownList.DataBind();
        }
        protected void LinkButton_Click(object sender, EventArgs e)
        {
            SetLinkPanel();
        }

        protected void PropDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Errors = "";
            PropStatusLabel.Text = "";
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

            AddRevisionPanel.Visible = true;
            MainDiv.Visible = true;
            RevisionDropDownList.Width = Width;
            RevisionDropDownList.Visible = true;
            DeleteButton.Visible = true;
            GridviewPanel.Visible = false;
            AddPropPanel.Visible = true;

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            AddPropPanel.Visible = false;
            AddRevisionPanel.Visible = false;
            GridviewPanel.Visible = true;
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            string revisionId;
            if (RevisionDropDownList.SelectedIndex > 0)
                revisionId = RevisionDropDownList.SelectedValue;
            else
            {
                revisionId = GenerateRevisionID();
            }

            var tagRevisionId = string.Format("<metadata><revision id=\"{0}\">", revisionId);
            InsertElementToDataBase(DOID, SystemID, revisionId, revisionId.GetType(), tagRevisionId, "revision id, revision id", revisionId, 0);
            AddPropPanel.Visible = false;
            AddRevisionPanel.Visible = false;
            GridviewPanel.Visible = true;
        }



        protected void AddEditButton_Click(object sender, EventArgs e)
        {
            Errors = "";
            PropStatusLabel.Text = Errors;
            string revisionId;
            if (RevisionDropDownList.SelectedIndex > 0)
                revisionId = RevisionDropDownList.SelectedValue;
            else
            {
                revisionId = GenerateRevisionID();
            }



            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(revisionId, "revision id");

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<metadata><revision id=\"{0}\"><prop id=\"{1}\">", revisionId, propid);
                var name = RemoveSpace(NameTextBox.Text);
                var tagname = string.Format("<metadata><revision id=\"{0}\"><prop id=\"{1}\" name>", revisionId, propid);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<metadata><revision id=\"{0}\"><prop id=\"{1}\" ns>", revisionId, propid);
                var classn = RemoveSpace(ClassTextBox.Text);
                var tagClass = string.Format("<metadata><revision id=\"{0}\"><prop id=\"{1}\" class>", revisionId, propid);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><revision id=\"{0}\"><prop id=\"{1}\">", revisionId, propid);

                Errors += InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "revision id, prop id", propid, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "name", propid.GetType(), tagname, "revision id, prop name", name, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "ns", propid.GetType(), tagns, "revision id, prop ns", ns, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "class", propid.GetType(), tagClass, "revision id, prop class", classn, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "value", propid.GetType(), tagValue, "revision id, prop", value, 1);

                var props = GetProps(revisionId, "revision id");
                var names = props.Select(x => x.Name).ToList();
                var fullnames = new List<string> { "" };
                fullnames.AddRange(names);
                PropDropDownList.DataSource = fullnames;
                PropDropDownList.DataBind();

            }

            if (PropLabel.Text == "Annotation")
            {
                var annotationId = GenerateAnnotationID(revisionId, "revision id");

                if (PropDropDownList.SelectedIndex > 0)
                {
                    annotationId = PropDropDownList.SelectedValue;
                }

                var tagAnnotationid = string.Format("<metadata><revision id=\"{0}\"><annotation id=\"{1}\">", revisionId, annotationId);
                var name = RemoveSpace(NameTextBox.Text);
                var tagname = string.Format("<metadata><revision id=\"{0}\"><annotation id=\"{1}\" name>", revisionId, annotationId);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<metadata><revision id=\"{0}\"><annotation id=\"{1}\" ns>", revisionId, annotationId);
                var classn = RemoveSpace(ClassTextBox.Text);
                var tagClass = string.Format("<metadata><revision id=\"{0}\"><annotation id=\"{1}\" class>", revisionId, annotationId);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><revision id=\"{0}\"><annotation id=\"{1}\">", revisionId, annotationId);
                var remarks = RemarksTextArea.InnerHtml;
                var tagRemarks = string.Format("<metadata><revision id=\"{0}\"><annotation id=\"{1}\"><remarks>", revisionId, annotationId);


                Errors += InsertElementToDataBase(DOID, SystemID, annotationId, annotationId.GetType(), tagAnnotationid, "revision id, annotation id", annotationId, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "name", annotationId.GetType(), tagname, "revision id, annotation name", name, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "ns", annotationId.GetType(), tagns, "revision id, annotation ns", ns, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "class", annotationId.GetType(), tagClass, "revision id, annotation class", classn, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "value", annotationId.GetType(), tagValue, "revision id, annotation", value, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "remarks", annotationId.GetType(), tagRemarks, "revision id, annotation, remarks", remarks, 1);

                SaveRemarksDesc(tagRemarks, "revision id, annotation, remarks", remarks);
            }

            if (PropLabel.Text == "Link")
            {
                var linkHref = "";

                var linkId = GenerateLinkID(revisionId, "revision id");

                if (PropDropDownList.SelectedIndex > 0)
                {
                    linkId = PropDropDownList.SelectedValue;
                }
                var tagid = string.Format("<metadata><revision id=\"{0}\"><link id=\"{1}\">", revisionId, linkId);

                linkHref = NameTextBox.Text;
                var taghref = string.Format("<metadata><revision id=\"{0}\"><link id=\"{1}\" href>", revisionId, linkId);

                var rel = NSTextBox.Text;
                var tagrel = string.Format("<metadata><revision id=\"{0}\"><link href=\"{1}\" rel>", revisionId, linkId);

                var media = ClassTextBox.Text;
                var tagmedia = string.Format("<metadata><revision id=\"{0}\"><link href=\"{1}\" media-type>", revisionId, linkId);


                var value = RemoveSpace(ValueTextBox.Text);
                var tagValue = string.Format("<metadata><revision id=\"{0}\"><link href=\"{1}\">", revisionId, linkId);

                Errors += InsertElementToDataBase(DOID, SystemID, linkId, linkHref.GetType(), taghref, "revision id, link id", linkId, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "link", linkHref.GetType(), taghref, "revision id, link href", linkHref, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "rel", rel.GetType(), tagrel, "revision id, link rel", rel, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "media-type", media.GetType(), tagmedia, "revision id, link media-type", media, 1);


                Errors += InsertElementToDataBase(DOID, SystemID, "value", value.GetType(), tagValue, "revision id, link", value, 1);


            }

            if (Errors.Length > 0)
            {
                AddPropPanel.Visible = true;
                PropStatusLabel.Text = Errors;
                PropStatusLabel.BackColor = Color.Red;
            }

            MainDiv.Visible = true;
            AddRevisionPanel.Visible = true;
            GridviewPanel.Visible = false;
        }


        protected void RemoveButton_Click(object sender, EventArgs e)
        {

        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            AddRevisionPanel.Visible = true;
            MainDiv.Visible = true;
            GridviewPanel.Visible = false;

            RevisionDropDownList.Width = Width;
            RevisionDropDownList.Visible = true;
            DeleteButton.Visible = true;

            var RevisionIds = Revisions.Select(x => x.RevisionID).ToList(); ;
            var fullList = new List<string> { "" };
            fullList.AddRange(RevisionIds);
            RevisionDropDownList.DataSource = fullList;
            RevisionDropDownList.DataBind();

            PopulateEditPage();
            GridviewPanel.Visible = false;
        }


        private void PopulateAnnotationEditPage()
        {

            var revisionid = RevisionDropDownList.SelectedValue;

            var anns = GetAnnotations(revisionid, "revision id");
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

            var revisionid = RevisionDropDownList.SelectedValue;

            var links = GetLinks(revisionid, "revision id");
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
            NSTextBox.Text = link.Rel;
            ClassTextBox.Text = link.MediaType;
            ValueTextBox.Text = link.MarkUpLine;
        }

        private void PopulatePropertEditPage()
        {

            var revisionid = RevisionDropDownList.SelectedValue;

            var props = GetProps(revisionid, "revision id");
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

        private void PopulateEditPage()
        {
            var revisionId = RevisionDropDownList.SelectedValue;
            var revision = new DocRevision();
            foreach (var elt in Revisions)
            {
                if (elt.RevisionID == revisionId)
                {
                    revision = elt;
                    break;
                }

            }
            TitleTextBox.Text = revision.Title;
            PublishedTextBox.Text = revision.Published;
            LastModifiedTextBox.Text = revision.LastModified;
            VersionTextBox.Text = revision.Version;
            OSCALVersionTextBox.Text = revision.OSCALVersion;

            RoleRemarksTextarea.InnerText = revision.Remarks;

            var Props = GetProps(revisionId, "revision id");
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            var Anns = GetAnnotations(revisionId, "revision id");
            var annNames = Anns.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();

            var Links = GetLinks(revisionId, "revision id");
            var hrefs = Links.Select(x => x.HRef).ToList();
            LinkDropDownList.DataSource = hrefs;
            LinkDropDownList.DataBind();
        }

        protected void Date1Button_Click(object sender, EventArgs e)
        {

            PublishedDateCalendar.Visible = true;
            AddRevisionPanel.Visible = true;
            MainDiv.Visible = true;
            AddOn = HSize();
            AddRevisionPanel.Height = Height + AddOn;
        }

        protected void Date2Button_Click(object sender, EventArgs e)
        {

            LastModifiedDateCalendar.Visible = true;
            AddRevisionPanel.Visible = true;
            MainDiv.Visible = true;
            AddOn = HSize();
            AddRevisionPanel.Height = Height + AddOn;
        }

        int HSize()
        {
            if (LastModifiedDateCalendar.Visible && PublishedDateCalendar.Visible)
                return 285;
            if (LastModifiedDateCalendar.Visible || PublishedDateCalendar.Visible)
                return 125;
            else
                return 0;
        }

        protected void RemoveButton_Click1(object sender, EventArgs e)
        {
            string revisionId;
            if (RevisionDropDownList.SelectedIndex > 0)
                revisionId = RevisionDropDownList.SelectedValue;
            else
            {
                revisionId = GenerateRevisionID();
            }
            if (PropLabel.Text == "Property")
            {
                var propid = PropDropDownList.SelectedValue;
                var tagpropid = string.Format("<metadata><revision id=\"{0}\"><prop id=\"{1}\">", revisionId, propid);
                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "revision id, prop id", propid, 0);
            }
            if (PropLabel.Text == "Annotation")
            {
                var propid = PropDropDownList.SelectedValue;
                var tagpropid = string.Format("<metadata><revision id=\"{0}\"><annotation id=\"{1}\">", revisionId, propid);
                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "revision id, annotation id", propid, 0);
            }
            if (PropLabel.Text == "Link")
            {
                var propid = PropDropDownList.SelectedValue;
                var tagpropid = string.Format("<metadata><revision id=\"{0}\"><link id=\"{1}\">", revisionId, propid);
                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "revision id, link id", propid, 0);
            }

        }
    }
}