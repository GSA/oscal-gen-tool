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

namespace OSCALGenerator.PagesSAR.Metadata
{
    public partial class ResponsibleParties : BasePage
    {
        protected List<ResponsibleParty> RespParties;
        protected List<DocParty> Parties;
        List<string> PartyRoleIds;
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
                RespParties = GetResponsibleParties();
               
                AddRolePanel.Visible = false;
                AddPropPanel.Visible = false;
                var bindingParties = new BindingList<ResponsibleParty>(RespParties);
                this.RolesGridView.DataSource = bindingParties;
                RolesGridView.DataBind();

                Parties = GetAllPartyInfo();
                var PartyIds = Parties.Select(x => x.PartyID).ToList();

                if (!IsPostBack)
                {
                    PartyIdDropDownList.DataSource = PartyIds;
                    PartyIdDropDownList.DataBind();
                }

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
            }
            catch (Exception ex)
            {
                StatusLabel.Visible = true;
                StatusLabel.ForeColor = Color.Red;
                StatusLabel.Text = ex.Message;
            }
        }



        protected void SaveButton_Click(object sender, EventArgs e)
        {
            Errors = "";
            StatusLabel.Text = "";
            var roleid = "";
            if (RoleDropDownList.SelectedIndex > 0)
                roleid = RoleDropDownList.SelectedValue;
            else
                roleid = GenerateRoleID();
           
            var tagroleid = string.Format("<metadata><responsible-party role-id=\"{0}\">", roleid);

            var remark = this.RoleRemarksTextarea.InnerText;
            var tagRemark= string.Format("<metadata><responsible-party role-id=\"{0}\"><remarks>", roleid);

            var partyIds = this.PartyIdDropDownList.SelectedValue;
            var vn = Parties.Where(x => x.PartyID == partyIds).FirstOrDefault();
            var uuid = vn.UUID;   
            var tagpartyId = string.Format("<metadata><responsible-party role-id=\"{0}\"><party-id>", roleid);
            var tagpartyUUId = string.Format("<metadata><responsible-party role-id=\"{0}\"><party-uuid>", roleid);

            Errors += InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "responsible-party role-id, responsible-party role-id", roleid, 1);
            Errors += InsertElementToDataBase(DOID, SystemID, "party-id", partyIds.GetType(), tagpartyId, "responsible-party role-id, party-id", partyIds, 1);
            Errors += InsertElementToDataBase(DOID, SystemID, "remarks", remark.GetType(), tagRemark, "responsible-party role-id, remarks", remark, 1);
            Errors += InsertElementToDataBase(DOID, SystemID, "uuid", partyIds.GetType(), tagpartyId, "responsible-party role-id, party-uuid", uuid, 1);

            if(Errors.Length>0)
            {
                StatusLabel.Text = Errors;
                AddRolePanel.Visible = true;
                    
            }

            AddRolePanel.Visible = false;
            GridviewPanel.Visible = true;
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            AddRolePanel.Visible = false;
            GridviewPanel.Visible = true;
        }

        protected void AddRoleButton_Click(object sender, EventArgs e)
        {
            PartyRoleIds = new List<string> { "" };
            var rowIds = RespParties.Select(x => x.RoleID).ToList();
            PartyRoleIds.AddRange(rowIds);
            RoleDropDownList.DataSource = PartyRoleIds;
            RoleDropDownList.DataBind();

            AddRolePanel.Visible = true;
            GridviewPanel.Visible = false;
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAR/Metadata/Parties.aspx");

        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAR/Import-AP/ImportAP.aspx");

        }

        private string GenerateRoleID()
        {
            var count = GetAllHeaders("responsible-party role-id, responsible-party role-id", UserName, DOID).Count;
            var id = string.Format("responsible-party-{0}", count);
            return id;
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

            var roleid = "";
            if (RoleDropDownList.SelectedIndex > 0)
                roleid = RoleDropDownList.SelectedValue;
            else
                roleid = GenerateRoleID();

            var props = GetProps(roleid, "responsible-party role-id");
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

        protected void LocationDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateEditPage();

            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            
           // DeleteButton.Visible = true;
        }

        protected void PropertyDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void AnnotationButton_Click(object sender, EventArgs e)
        {
            SetAnnotationPanel();
        }

        protected void AnnotationDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void SetAnnotationPanel()
        {
            AddRolePanel.Visible = true;
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

            var roleid = "";
            if (RoleDropDownList.SelectedIndex > 0)
                roleid = RoleDropDownList.SelectedValue;
            else
                roleid = GenerateRoleID();

            var annotations = GetAnnotations(roleid, "responsible-party role-id");
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

        protected void LinkDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void SetLinkPanel()
        {
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = true;
            GridviewPanel.Visible = false;
            NameLabel.Text = "href";
            ClassLabel.Text = "rel";
            NSLabel.Text = "media-type";
            ValueLabel.Text = "Value";
            RemarksLabel.Text = "";
            RemarksTextArea.Visible = false;
            ClassLabel.Visible = true;
            ClassTextBox.Visible = true;
            PropLabel.Text = "Link";
            PropLabel.Enabled = false;

            var roleid = "";
            if (RoleDropDownList.SelectedIndex > 0)
                roleid = RoleDropDownList.SelectedValue;
            else
                roleid = GenerateRoleID();


            var links = GetLinks(roleid, "responsible-party role-id");
            var names = links.Select(x => x.HRef).ToList();
            var ids = links.Select(x => x.ParentID).ToList();
            var fullnames = new List<string> { "" };



            fullnames.AddRange(ids);
            PropDropDownList.DataSource = fullnames;
            PropDropDownList.DataBind();

            PopulateLinkEditPage();

            LinkDropDownList.DataSource = names;
            LinkDropDownList.DataBind();
        }

        protected void LinkButton_Click(object sender, EventArgs e)
        {
            SetLinkPanel();
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
            if (PropLabel.Text == "Link")
            {
                PopulateLinkEditPage();
            }

            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
           
           // DeleteButton.Visible = true;

            AddPropPanel.Visible = true;

        }

        private void PopulatePropertEditPage()
        {

            var roleid = RoleDropDownList.SelectedValue;

            var props = GetProps(roleid, "responsible-party role-id");
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
        private void PopulateAnnotationEditPage()
        {

            var roleid = RoleDropDownList.SelectedValue;

            var anns = GetAnnotations(roleid, "responsible-party role-id");
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

            var roleid = RoleDropDownList.SelectedValue;

            var links = GetLinks(roleid, "responsible-party role-id");
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
            NSTextBox.Text = link.MediaType;
            ClassTextBox.Text =  link.Rel;
            ValueTextBox.Text = link.MarkUpLine;
        }


        private void PopulateEditPage()
        {
            var roleId = RoleDropDownList.SelectedValue;
            var resRole = new ResponsibleParty();
            foreach (var elt in RespParties)
            {
                if (elt.RoleID == roleId)
                {
                    resRole = elt;
                    break;
                }

            }
            PartyIdDropDownList.SelectedValue = resRole.PartyID;
            RoleRemarksTextarea.InnerText = resRole.Remarks;

            var Props = GetProps(roleId, "responsible-party role-id");
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            var Anns = GetAnnotations(roleId, "responsible-party role-id");
            var annNames = Anns.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();

            var Links = GetLinks(roleId, "responsible-party role-id");
            var hrefs = Links.Select(x => x.HRef).ToList();
            LinkDropDownList.DataSource = hrefs;
            LinkDropDownList.DataBind();
        }
        protected void AddEditButton_Click(object sender, EventArgs e)
        {
            string entityId;
            if (RoleDropDownList.SelectedIndex > 0)
                entityId = RoleDropDownList.SelectedValue;
            else
            {
                entityId = GenerateRoleID();
            }

            ProcessEntity(entityId, "responsible-party role-id");
            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
            GridviewPanel.Visible = false;
        }

        public void ProcessEntity(string entityId, string entityName)
        {

            Errors = "";
            PropStatusLabel.Text = "";
            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(entityId, entityName);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<metadata><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<metadata><{2}=\"{0}\"><prop id=\"{1}\" name>", entityId, propid, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<metadata><{2}=\"{0}\"><prop id=\"{1}\" ns>", entityId, propid, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<metadata><{2}=\"{0}\"><prop id=\"{1}\" class>", entityId, propid, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);

                Errors += InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, entityName+", prop id", propid, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "name", propid.GetType(), tagname, entityName+", prop name", name, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "ns", propid.GetType(), tagns, entityName+", prop ns", ns, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "class", propid.GetType(), tagClass, entityName+", prop class", classn, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "value", propid.GetType(), tagValue, entityName+", prop", value, 1);

                var props = GetProps(entityId, entityName);
                var names = props.Select(x => x.Name).ToList();
                var fullnames = new List<string> { "" };
                fullnames.AddRange(names);
                PropDropDownList.DataSource = fullnames;
                PropDropDownList.DataBind();

            }

            if (PropLabel.Text == "Annotation")
            {
                var annotationId = GenerateAnnotationID(entityId, entityName);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    annotationId = PropDropDownList.SelectedValue;
                }

                var tagAnnotationid = string.Format("<metadata><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<metadata><{2}=\"{0}\"><annotation id=\"{1}\" name>", entityId, annotationId, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<metadata><{2}=\"{0}\"><annotation id=\"{1}\" ns>", entityId, annotationId, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<metadata><{2}=\"{0}\"><annotation id=\"{1}\" class>", entityId, annotationId, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var remarks = RemarksTextArea.InnerText;
                var tagRemarks = string.Format("<metadata><{2}=\"{0}\"><annotation id=\"{1}\"><remarks>", entityId, annotationId, entityName);


                Errors += InsertElementToDataBase(DOID, SystemID, annotationId, annotationId.GetType(), tagAnnotationid, entityName+", annotation id", annotationId, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "name", annotationId.GetType(), tagname, entityName+", annotation name", name, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "ns", annotationId.GetType(), tagns, entityName+", annotation ns", ns, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "class", annotationId.GetType(), tagClass, entityName+", annotation class", classn, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "value", annotationId.GetType(), tagValue, entityName+", annotation", value, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "remarks", annotationId.GetType(), tagRemarks, entityName+", annotation, remarks", remarks, 1);

            }

            if (PropLabel.Text == "Link")
            {
                var linkid = "";

                if (PropDropDownList.SelectedIndex > 0)
                {
                    linkid = PropDropDownList.SelectedValue;
                }else
                   linkid = GenerateLinkID(entityId, entityName);

                var linkHref = NameTextBox.Text;
                 var taghref = string.Format("<metadata><{2}=\"{0}\"><link id=\"{1}\" href>", entityId, linkid, entityName);
                var tagid = string.Format("<metadata><{2}=\"{0}\"><link id=\"{1}\">", entityId, linkid, entityName);
                var  media = NSTextBox.Text;
                var tagrel = string.Format("<metadata><{2}=\"{0}\"><link id=\"{1}\" rel>", entityId, linkid, entityName, entityName);

                var rel = ClassTextBox.Text;
                var tagmedia = string.Format("<metadata><{2}=\"{0}\"><link id=\"{1}\" media-type>", entityId, linkid, entityName);


                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><{2}=\"{0}\"><link id=\"{1}\">", entityId, linkid, entityName);

                Errors += InsertElementToDataBase(DOID, SystemID, linkid, linkHref.GetType(), tagid, entityName + ", link id", linkid, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "link", linkHref.GetType(), taghref, entityName+", link href", linkHref, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "rel", rel.GetType(), tagrel, entityName+", link rel", rel, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "media-type", media.GetType(), tagmedia, entityName+", link media-type", media, 1);


                Errors += InsertElementToDataBase(DOID, SystemID, "value", value.GetType(), tagValue, entityName+", link", value, 1);

            }

            if(Errors.Length>0)
            {
                PropStatusLabel.Text = Errors;
                PropStatusLabel.ForeColor = Color.Red;
                AddPropPanel.Visible = true;
            }
        }

        protected void RoleDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateEditPage();

            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
        }

        protected void Delete_Click(object sender, EventArgs e)
        {
            var roleid = RoleDropDownList.SelectedValue;
            var tagroleid = string.Format("<metadata><responsible-party role-id=\"{0}\">", roleid);          
           
            Errors += InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "responsible-party role-id, responsible-party role-id", roleid, 0);
        }

        protected void RemoveButton_Click(object sender, EventArgs e)
        {
            string revisionId;
            if (RoleDropDownList.SelectedIndex > 0)
                revisionId = RoleDropDownList.SelectedValue;
            else
            {
                revisionId = GenerateRoleID();
            }
            if (PropLabel.Text == "Property")
            {
                var propid = PropDropDownList.SelectedValue;
                var tagpropid = string.Format("<metadata><responsible-party role-id=\"{0}\"><prop id=\"{1}\">", revisionId, propid);
                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "doc id, prop id", propid, 0);
            }
            if (PropLabel.Text == "Annotation")
            {
                var propid = PropDropDownList.SelectedValue;
                var tagpropid = string.Format("<metadata><responsible-party role-id=\"{0}\"><annotation id=\"{1}\">", revisionId, propid);
                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "doc id, annotation id", propid, 0);
            }
            if (PropLabel.Text == "Link")
            {
                var propid = PropDropDownList.SelectedValue;
                var tagpropid = string.Format("<metadata><responsible-party role-id=\"{0}\"><link id=\"{1}\">", revisionId, propid);
                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "doc id, link id", propid, 0);
            }

            AddRolePanel.Visible = true;
            GridviewPanel.Visible = false;

            AddPropPanel.Visible = true;
        }
    }
}