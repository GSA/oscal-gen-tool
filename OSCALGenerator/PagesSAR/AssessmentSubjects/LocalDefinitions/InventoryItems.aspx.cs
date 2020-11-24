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

namespace OSCALGenerator.PagesSAR.AssessmentSubjects.LocalDefinitions
{
    public partial class InventoryItems : BasePage
    {
        List<Prop> Props;
        List<Annotation> Annotations;
        List<Link> IDRefs;
        public List<string> RoleIds;
        List<ResponsibleParty> ResponsibleParties;
        List<ImplementedComponent> Components;

        List<InventoryItem> Subjects;

        List<string> MyRoles;

        protected new void Page_Load(object sender, EventArgs e)
        {
            FaultyCacheBackHomeSAR();
            UserName = Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = int.Parse(Cache["SystemId"].ToString());

            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);


            Subjects = GetInventoryItems();


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
            AddResponsibleRolePanel.Visible = false;
            MainDiv.Visible = false;
            var bindingRoles = new BindingList<InventoryItem>(Subjects);
            this.RolesGridView.DataSource = bindingRoles;
            RolesGridView.DataBind();

            if (!IsPostBack)
            {
                MyRoles = new List<string>();
                var names = Subjects.Select(x => x.ID).ToList();
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

            var rawTag = string.Format("<assessment-subject><local-definitions><{1}=\"{0}\">", subjectid, "inventory-item id");

            IDRefs = GetLinks(subjectid, "component id", rawTag);
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
            Response.Redirect(@"~/PagesSAR/AssessmentSubjects/ImplementedComponents.aspx", false);
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
            var id = string.Format("inventoryItem-{0}", nbrRole);
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

            var tagroleid = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\">", roleid);

            var taguuid = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\"><uuid>", roleid);
            var guid = Guid.NewGuid().ToString();

            var tagAssetId = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\" asset-id>", roleid);
            var asset = AssetIDTextBox.Text;

            var desc = this.DescriptionTextArea.InnerText;
            var tagDesc = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\"><description>", roleid);

            var remarks = RoleRemarksTextarea.InnerText;
            var tagRemarks = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\"><remarks>", roleid);

            InsertElementToDataBase(DOID, SystemID, "inventory-item id", roleid.GetType(), tagroleid, "inventory-item id, inventory-item id", roleid, 1);
            InsertElementToDataBase(DOID, SystemID, "inventory-item uuid", roleid.GetType(), taguuid, "inventory-item id, uuid", guid, 1);


            InsertElementToDataBase(DOID, SystemID, "asset-id", roleid.GetType(), tagAssetId, "inventory-item id, asset-id", asset, 1);

            InsertElementToDataBase(DOID, SystemID, "desc", roleid.GetType(), tagDesc, "inventory-item id, description", desc, 1);


            InsertElementToDataBase(DOID, SystemID, "remarks", roleid.GetType(), tagRemarks, "inventory-item id, remarks", remarks, 1);


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
            var tag = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\">", roleid);

            var tagroleid = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\">", roleid);

            var tagAssetID = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\" asset-id>", roleid);

            var type = GetData("asset-id", tagAssetID, UserName, DOID);
            AssetIDTextBox.Text = type;
         
            var tagDesc = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\"><description>", roleid);
            DescriptionTextArea.InnerText = GetData("desc", tagDesc, UserName, DOID);

            var tagRemarks = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\"><remarks>", roleid);
            RoleRemarksTextarea.InnerText = GetData("remarks", tagRemarks, UserName, DOID);

            var rawTag = tag;

            Props = GetProps(roleid, "inventory-item id", tag);
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            Annotations = GetAnnotations(roleid, "inventory-item id", tag);
            var annNames = Annotations.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();

            IDRefs = GetLinks(roleid, "inventory-item id", tag);
            var linkNames = IDRefs.Select(x => x.HRef).ToList();
            LinkDropDownList.DataSource = linkNames;
            LinkDropDownList.DataBind();

            var component = Subjects.Where(x => x.ID == roleid).FirstOrDefault();
            var RespRoles = component.ResponsibleRoles == null ? new List<ResponsibleParty>() : component.ResponsibleRoles;
            var RoleIds = RespRoles.Select(x => x.RoleID).ToList();
            ResponsibleRoleDropDownList.DataSource = RoleIds;
            ResponsibleRoleDropDownList.DataBind();

            var Comps = component.implementedComponents == null ? new List<ImplementedComponent>() : component.implementedComponents;
            var ids = Comps.Select(x => x.ComponentID).ToList();
            ImplementedCompDropDownList.DataSource = ids;
            ImplementedCompDropDownList.DataBind();
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

            var rawTag = string.Format("<assessment-subject><local-definitions><{1}=\"{0}\">", subjectid, "inventory-item id");

            Props = GetProps(subjectid, "inventory-item id", rawTag);
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

            var rawTag = string.Format("<assessment-subject><local-definitions><{1}=\"{0}\">", subjectid, "inventory-item id");

            Annotations = GetAnnotations(subjectid, "inventory-item id", rawTag);
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

            var rawTag = string.Format("<assessment-subject><local-definitions><{1}=\"{0}\">", entityId, entityName);

            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(entityId, entityName, rawTag);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><prop id=\"{1}\" name>", entityId, propid, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><prop id=\"{1}\" ns>", entityId, propid, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><prop id=\"{1}\" class>", entityId, propid, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);

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

                var tagAnnotationid = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><annotation id=\"{1}\" name>", entityId, annotationId, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><annotation id=\"{1}\" ns>", entityId, annotationId, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><annotation id=\"{1}\" class>", entityId, annotationId, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var remarks = RemarksTextArea.InnerText;
                var tagRemarks = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><annotation id=\"{1}\"><remarks>", entityId, annotationId, entityName);


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
                var taghref = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

                var rel = NSTextBox.Text;
                var tagrel = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><link href=\"{1}\" rel>", entityId, linkHref, entityName, entityName);

                var media = ClassTextBox.Text;
                var tagmedia = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><link href=\"{1}\" media-type>", entityId, linkHref, entityName);


                var value = ValueTextBox.Text;
                var tagValue = string.Format("<assessment-subject><local-definitions><{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

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
            ProcessEntity(roleid, "inventory-item id");
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
            Response.Redirect(@"~/PagesSAR/AssessmentSubjects/LocalDefinitions/SARUsers.aspx", false);
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            GridviewPanel.Visible = false;
        }

        protected void AddResponsibleRoleButton_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            AddRolePanel.Visible = true;
            ResponsibleParties = GetResponsibleParties();

            var partyIds = ResponsibleParties.Select(x => x.RoleID);
            RespRoleDropDownList.DataSource = partyIds;
            RespRoleDropDownList.DataBind();
        }

        protected void AddRoleButton_Click1(object sender, EventArgs e)
        {
            KeyLabel.Text = "Responsible Role ID";

            MainDiv.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            AddRolePanel.Visible = true;
            ResponsibleParties = GetResponsibleParties();

            var partyIds = ResponsibleParties.Select(x => x.RoleID);
            RespRoleDropDownList.DataSource = partyIds;
            RespRoleDropDownList.DataBind();
        }

        protected void AddImplementedCompButton_Click(object sender, EventArgs e)
        {
            KeyLabel.Text = "Implemented Component ID";

            MainDiv.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            AddRolePanel.Visible = true;
            Components = GetImplementedComponents();

            var partyIds = Components.Select(x => x.ComponentID);
            RespRoleDropDownList.DataSource = partyIds;
            RespRoleDropDownList.DataBind();
        }

        protected void AddAuxButton_Click(object sender, EventArgs e)
        {
          
            var id = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                id = GenerateSubjectID();
            }
            else
                id = RoleDropDownList.SelectedValue;

            if (KeyLabel.Text == "Responsible Role ID")
            {
                var Resproleid = RespRoleDropDownList.SelectedValue;
                var tagroleid = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\"><responsible-role role-id=\"{1}\">", id, Resproleid);
                InsertElementToDataBase(DOID, SystemID, "inventory-item responsible role id", Resproleid.GetType(), tagroleid, "inventory-item id, responsible-role role-id", Resproleid, 1);
            }
            if(KeyLabel.Text == "Implemented Component ID")
            {
                var Resproleid = RespRoleDropDownList.SelectedValue;
                var tagroleid = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\"><implemented-component component-id=\"{1}\">", id, Resproleid);
                InsertElementToDataBase(DOID, SystemID, "inventory-item implemented-component component-id", Resproleid.GetType(), tagroleid, "inventory-item id, implemented-component component-id", Resproleid, 1);

            }
            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
        }
    }
}