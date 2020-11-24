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

namespace OSCALGenerator.PageSSP.Metadata
{
    public partial class DocID : BasePage
    {
        List<DocumentIdentifier> DocumentIdentifiers;
        int Width = 198;
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
                DocumentIdentifiers = GetDocIDs();
                AddRevisionPanel.Visible = false;
                MainDiv.Visible = false;
                AddPropPanel.Visible = false;
                MainDiv.Visible = false;
                var bindingRevision = new BindingList<DocumentIdentifier>(DocumentIdentifiers);
                RevisionGridView.DataSource = bindingRevision;
                RevisionGridView.DataBind();

                GridviewPanel.Visible = true;

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
            }catch(Exception ex)
            {
                StatusLabel.BackColor = Color.Red;
                StatusLabel.Text = ex.Message;
            }
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PageSSP/Metadata/RevisionHistory.aspx", false);
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {

            Response.Redirect(@"~/PageSSP/Metadata/Roles.aspx", false);
        }

        private string GenerateRevisionID()
        {
            var count = GetAllHeaders("doc-id, doc-id", UserName, DOID).Count;
            var id = string.Format("docId-{0}", count);
            return id;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            Errors = "";
            StatusLabel.Text = "";
            string docId;
            if (DocIDDropDownList.SelectedIndex > 0)
                docId = DocIDDropDownList.SelectedValue;
            else
            {
                docId = GenerateRevisionID();
            }

            var tagdocId = string.Format("<metadata><doc-id=\"{0}\">", docId);

            var id = this.IDTextBox.Text;
            var tagID = string.Format("<metadata><doc-id=\"{0}\"><identifier>", docId);

            var type = TypeDropDownList.SelectedValue;

  
            var tagType = string.Format("<metadata><doc-id=\"{0}\"><type>", docId);

            Errors +=InsertElementToDataBase(DOID, SystemID, docId, docId.GetType(), tagdocId, "doc-id, doc-id", docId, 1);
            Errors +=InsertElementToDataBase(DOID, SystemID, "identifier", id.GetType(), tagID, "doc-id, identifier", id, 1);

            Errors +=InsertElementToDataBase(DOID, SystemID, "type", type.GetType(), tagType, "doc-id, type", type, 1);


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

            var id = "";
            if (DocIDDropDownList.SelectedIndex > 0)
                 id = DocIDDropDownList.SelectedValue;
            else
                id = GenerateRevisionID();

            var props = GetProps(id, "doc-id");
            var names = props.Select(x => x.Name).ToList();
            var fullnames = new List<string> { "" };

            var all = new List<Prop> { new Prop() };
            all.AddRange(props);
 
            PropDropDownList.DataSource = all;
            PropDropDownList.DataValueField = "ID";
            PropDropDownList.DataTextField = "ID";
            PropDropDownList.DataBind();

            PopulatePropertEditPage();

            PropertyDropDownList.DataSource = props;
            PropDropDownList.DataValueField = "Name";
            PropDropDownList.DataTextField = "Name";
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
            DocIDDropDownList.Width = Width;
            DocIDDropDownList.Visible = true;
            DeleteButton.Visible = true;
        }

 


        protected void PropertyDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

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

            var  id = "";
            if (DocIDDropDownList.SelectedIndex > 0)
                 id = DocIDDropDownList.SelectedValue;
            else
                id = GenerateRevisionID();


            var links = GetLinks(id, "doc-id");
            var all = new List<Link> { new Link() };
            all.AddRange(links);

            PropDropDownList.DataSource = all;
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
           
            if (PropLabel.Text == "Link")
            {
                PopulateLinkEditPage();
            }

            AddRevisionPanel.Visible = true;
            MainDiv.Visible = true;
            DocIDDropDownList.Width = Width;
            DocIDDropDownList.Visible = true;
            DeleteButton.Visible = true;
            GridviewPanel.Visible = false;
            AddPropPanel.Visible = true;

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            AddRevisionPanel.Visible = false;
            AddPropPanel.Visible = false;
            GridviewPanel.Visible = true;
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            var docId = DocIDDropDownList.SelectedValue;
            var tagdocId = string.Format("<metadata><doc-id=\"{0}\">", docId);
            InsertElementToDataBase(DOID, SystemID, docId, docId.GetType(), tagdocId, "doc-id, doc-id", docId, 0);
            AddRevisionPanel.Visible = false;
            AddPropPanel.Visible = false;
            GridviewPanel.Visible = true;
        }



        protected void AddEditButton_Click(object sender, EventArgs e)
        {
            string revisionId;
            if (DocIDDropDownList.SelectedIndex > 0)
                revisionId = DocIDDropDownList.SelectedValue;
            else
            {
                revisionId = GenerateRevisionID();
            }

            Errors = "";
            PropStatusLabel.Text = "";

            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(revisionId, "doc-id");

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<metadata><doc-id=\"{0}\"><prop id=\"{1}\">", revisionId, propid);
                var name = RemoveSpace(NameTextBox.Text);
                var tagname = string.Format("<metadata><doc-id=\"{0}\"><prop id=\"{1}\" name>", revisionId, propid);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<metadata><doc-id=\"{0}\"><prop id=\"{1}\" ns>", revisionId, propid);
                var classn = RemoveSpace(ClassTextBox.Text);
                var tagClass = string.Format("<metadata><doc-id=\"{0}\"><prop id=\"{1}\" class>", revisionId, propid);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><doc-id=\"{0}\"><prop id=\"{1}\">", revisionId, propid);

                Errors +=InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "doc-id, prop id", propid, 1);
                Errors +=InsertElementToDataBase(DOID, SystemID, "name", propid.GetType(), tagname, "doc-id, prop name", name, 1);

                Errors +=InsertElementToDataBase(DOID, SystemID, "ns", propid.GetType(), tagns, "doc-id, prop ns", ns, 1);

                Errors +=InsertElementToDataBase(DOID, SystemID, "class", propid.GetType(), tagClass, "doc-id, prop class", classn, 1);
                Errors +=InsertElementToDataBase(DOID, SystemID, "value", propid.GetType(), tagValue, "doc-id, prop", value, 1);
               

            }

            if (PropLabel.Text == "Annotation")
            {
                var annotationId = GenerateAnnotationID(revisionId, "doc id");

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


                Errors +=InsertElementToDataBase(DOID, SystemID, annotationId, annotationId.GetType(), tagAnnotationid, "revision id, annotation id", annotationId, 1);
                Errors +=InsertElementToDataBase(DOID, SystemID, "name", annotationId.GetType(), tagname, "revision id, annotation name", name, 1);

                Errors +=InsertElementToDataBase(DOID, SystemID, "ns", annotationId.GetType(), tagns, "revision id, annotation ns", ns, 1);

                Errors +=InsertElementToDataBase(DOID, SystemID, "class", annotationId.GetType(), tagClass, "revision id, annotation class", classn, 1);
                Errors +=InsertElementToDataBase(DOID, SystemID, "value", annotationId.GetType(), tagValue, "revision id, annotation", value, 1);

                Errors +=InsertElementToDataBase(DOID, SystemID, "remarks", annotationId.GetType(), tagRemarks, "revision id, annotation, remarks", remarks, 1);

                SaveRemarksDesc(tagRemarks, "revision id, annotation, remarks", remarks);
            }

            if (PropLabel.Text == "Link")
            {
                var linkid = GenerateLinkID(revisionId, "doc id");

                if (PropDropDownList.SelectedIndex > 0)
                {
                    linkid = PropDropDownList.SelectedValue;
                }


                var linkHref = NameTextBox.Text;
                var tagid = string.Format("<metadata><doc-id=\"{0}\"><link id=\"{1}\">", revisionId, linkid);
                var taghref = string.Format("<metadata><doc-id=\"{0}\"><link id=\"{1}\" href>", revisionId, linkid);

                var media = NSTextBox.Text;
                var tagrel = string.Format("<metadata><doc-id=\"{0}\"><link id=\"{1}\" rel>", revisionId, linkid);

                var rel = ClassTextBox.Text;
                var tagmedia = string.Format("<metadata><doc-id=\"{0}\"><link id=\"{1}\" media-type>", revisionId, linkid);


                var value = RemoveSpace(ValueTextBox.Text);
                var tagValue = string.Format("<metadata><doc-id=\"{0}\"><link id=\"{1}\">", revisionId, linkid);
                Errors += InsertElementToDataBase(DOID, SystemID, linkid, linkHref.GetType(), tagid, "doc-id, link id", linkid, 1);
                Errors +=InsertElementToDataBase(DOID, SystemID, "link", linkHref.GetType(), taghref, "doc-id, link href", linkHref, 1);
                Errors +=InsertElementToDataBase(DOID, SystemID, "rel", rel.GetType(), tagrel, "doc-id, link rel", rel, 1);

                Errors +=InsertElementToDataBase(DOID, SystemID, "media-type", media.GetType(), tagmedia, "doc-id, link media-type", media, 1);


                Errors +=InsertElementToDataBase(DOID, SystemID, "value", value.GetType(), tagValue, "doc-id, link", value, 1);


            }

            if(Errors.Length>0)
            {
                PropStatusLabel.Text = Errors;
                PropStatusLabel.BackColor = Color.Red;
                AddPropPanel.Visible = true;
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

            DocIDDropDownList.Width = Width;
            DocIDDropDownList.Visible = true;
            DeleteButton.Visible = true;

            var RevisionIds =DocumentIdentifiers.Select(x => x.DocID).ToList(); ;
            var fullList = new List<string> { "" };
            fullList.AddRange(RevisionIds);
            DocIDDropDownList.DataSource = fullList;
            DocIDDropDownList.DataBind();

            PopulateEditPage();
            GridviewPanel.Visible = false;
        }




        private void PopulateLinkEditPage()
        {

            var revisionid = DocIDDropDownList.SelectedValue;

            var links = GetLinks(revisionid, "doc-id");
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

            var revisionid = DocIDDropDownList.SelectedValue;

            var props = GetProps(revisionid, "doc-id");
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
            var revisionId = DocIDDropDownList.SelectedValue;
            var doc = new DocumentIdentifier();
            foreach (var elt in DocumentIdentifiers)
            {
                if (elt.DocID == revisionId)
                {
                    doc = elt;
                    break;
                }
            }
            IDTextBox.Text = doc.Value;
            TypeDropDownList.SelectedValue = doc.Type;

            var Props = GetProps(revisionId, "doc-id");
          
            PropertyDropDownList.DataSource = Props;
            PropertyDropDownList.DataValueField = "Name";
            PropertyDropDownList.DataTextField = "Name";
            PropertyDropDownList.DataBind();

            var Links = GetLinks(revisionId, "doc-id");

            LinkDropDownList.DataSource = Links;
            LinkDropDownList.DataValueField = "HRef";
            LinkDropDownList.DataTextField = "HRef";
            LinkDropDownList.DataBind();
        }

        protected void RemoveButton_Click1(object sender, EventArgs e)
        {
            string revisionId;
            if (DocIDDropDownList.SelectedIndex > 0)
                revisionId = DocIDDropDownList.SelectedValue;
            else
            {
                revisionId = GenerateRevisionID();
            }
            if (PropLabel.Text == "Property")
            {
                var propid = PropDropDownList.SelectedValue;
                var tagpropid = string.Format("<metadata><doc id=\"{0}\"><prop id=\"{1}\">", revisionId, propid);
                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "doc id, prop id", propid, 0);
            }
            if (PropLabel.Text == "Annotation")
            {
                var propid = PropDropDownList.SelectedValue;
                var tagpropid = string.Format("<metadata><doc id=\"{0}\"><annotation id=\"{1}\">", revisionId, propid);
                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "doc id, annotation id", propid, 0);
            }
            if (PropLabel.Text == "Link")
            {
                var propid = PropDropDownList.SelectedValue;
                var tagpropid = string.Format("<metadata><doc id=\"{0}\"><link id=\"{1}\">", revisionId, propid);
                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "doc id, link id", propid, 0);
            }

            AddRevisionPanel.Visible = true;
            MainDiv.Visible = true;
            
            DeleteButton.Visible = true;

            AddPropPanel.Visible = true;
        }

        protected void DocIDDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddRevisionPanel.Visible = true;
            MainDiv.Visible = true;
            GridviewPanel.Visible = false;

            DocIDDropDownList.Width = Width;
            DocIDDropDownList.Visible = true;
            DeleteButton.Visible = true;
            PopulateEditPage();
        }
    }
}