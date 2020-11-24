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
    public partial class Roles : BasePage
    {
        public List<string> RoleIds;
        List<Prop> Props;
        int Width = 198;
        string RoleID;
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
                SavedRoles = GetRoles();
                AddRolePanel.Visible = false;
                MainDiv.Visible = false;
                AddPropPanel.Visible = false;
                MainDiv.Visible = false;
                var bindingRoles = new BindingList<Role>(SavedRoles);
                this.RolesGridView.DataSource = bindingRoles;
                RolesGridView.DataBind();

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
                StatusLabel.Text = ex.Message;
                StatusLabel.BackColor = Color.Red;
            }
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

       

        private string GenerateRoleID()
        {
            var count = GetAllHeaders("role id, role id", UserName, DOID).Count;
            var id = string.Format("role-{0}", count);
            return id;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            Errors = "";
            StatusLabel.Text = "";
            string roleid;
            if (ShortNameTextBox.Text.Length==0 || TitleTextBox.Text.Length==0)
            {
                StatusLabel.Visible = true;
                StatusLabel.ForeColor = Color.Red;
                StatusLabel.Text = " The role Title and Short Name are required entries they cannot be empty.";
                return;
            }
            else
            {
                    roleid = GenerateRoleID();
            }

           
            if (RoleDropDownList.SelectedIndex>0)
                roleid = RoleDropDownList.SelectedValue;

            RoleID = roleid;

            var tagroleid = string.Format("<metadata><role id=\"{0}\">", roleid);

            var roleTitle = this.TitleTextBox.Text;
            var tagTitle = string.Format("<metadata><role id=\"{0}\"><title>", roleid);


            var roleShortName = this.ShortNameTextBox.Text;
            var tagShortName = string.Format("<metadata><role id=\"{0}\"><short-name>", roleid);

            var desc = this.DescriptionTextArea.InnerText;
            var tagDesc = string.Format("<metadata><role id=\"{0}\"><desc>", roleid);

            var remarks = RoleRemarksTextarea.InnerText;
            var tagRemarks = string.Format("<metadata><role id=\"{0}\"><remarks>", roleid);

            Errors +=InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "role id, role id", roleid, 1);
            Errors +=InsertElementToDataBase(DOID, SystemID, "title", roleid.GetType(), tagTitle, "role id, title", roleTitle, 1);

            Errors +=InsertElementToDataBase(DOID, SystemID, "desc", roleid.GetType(), tagDesc, "role id, desc", desc, 1);

            Errors +=InsertElementToDataBase(DOID, SystemID, "short-name", roleid.GetType(), tagShortName, "role id, short-name", roleShortName, 1);

            Errors +=InsertElementToDataBase(DOID, SystemID, "remarks", roleid.GetType(), tagRemarks, "role id, remarks", remarks, 1);

            if(Errors.Length>0)
            {
                StatusLabel.Text = Errors;
                StatusLabel.BackColor = Color.Red;
            }

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

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAR/Metadata/RevisionHistory.aspx", false);
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAR/Metadata/Locations.aspx", false);
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;

            RoleDropDownList.Width = Width;
            RoleDropDownList.Visible = true;
            DeleteButton.Visible = true;

            var RoleIds = GetRoleIds();
            var fullList = new List<string> { "" };
            fullList.AddRange(RoleIds);
            RoleDropDownList.DataSource = fullList;
            RoleDropDownList.DataBind();

            PopulateEditPage();
            GridviewPanel.Visible = false;
        }

        private void PopulateAnnotationEditPage()
        {

            var roleid = RoleDropDownList.SelectedValue;

            var anns = GetAnnotations(roleid, "role id");
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

            var links = GetLinks(roleid, "role id");
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
            ClassTextBox.Text = link.Rel;
            ValueTextBox.Text = link.MarkUpLine;
        }

        private void PopulatePropertEditPage()
        {
          
            var roleid = RoleDropDownList.SelectedValue;
       
            var props = GetRoleProps(roleid);
            var propId = PropDropDownList.SelectedValue;
            var prop = new Prop();
            foreach (var elt in props)
            {
                if (elt.ID== propId)
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
            var roleId = RoleDropDownList.SelectedValue;
            var role = new Role();
            foreach (var elt in SavedRoles)
            {
                if (elt.RoleID == roleId)
                {
                    role = elt;
                    break;
                }

            }
            TitleTextBox.Text = role.RoleTitle;
            ShortNameTextBox.Text = role.ShortName;
            DescriptionTextArea.InnerText = role.Description;
            RoleRemarksTextarea.InnerText = role.Remarks;

             Props = GetRoleProps(roleId);
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            var Anns = GetAnnotations(roleId, "role id");
            var annNames = Anns.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();

            var Links = GetLinks(roleId, "role id");
            var hrefs = Links.Select(x => x.HRef).ToList();
            LinkDropDownList.DataSource = hrefs;
            LinkDropDownList.DataBind();
        }

        protected void RoleDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateEditPage();

            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            RoleDropDownList.Width = Width;
            RoleDropDownList.Visible = true;
            DeleteButton.Visible = true;
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            var roleId = RoleDropDownList.SelectedValue;


            var tagroleid = string.Format("<metadata><role id=\"{0}\">", roleId);

            if (roleId.Length == 0)
                return;

            Errors +=InsertElementToDataBase(DOID, SystemID, roleId, roleId.GetType(), tagroleid, "role id, role id", "", 0);

            RolesGridView.DataBind();
            GridviewPanel.Visible = true;
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
            if (RoleDropDownList.SelectedIndex >0)
                roleid = RoleDropDownList.SelectedValue;
            else
                roleid = GenerateRoleID();

            var props = GetRoleProps(roleid);
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

            var roleid = "";
            if (RoleDropDownList.SelectedIndex > 0)
                roleid = RoleDropDownList.SelectedValue;
            else
                roleid = GenerateRoleID();

            var annotations = GetAnnotations(roleid, "role id");
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

        void SetLinkPanel()
        {
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = true;
            NameLabel.Text = "href";
            ClassLabel.Text = "rel";
            ClassLabel.Visible = true;
            ClassTextBox.Visible = true;
            NSLabel.Text = "media-type";
            ValueLabel.Text = "Value";
            RemarksLabel.Text = "";
            RemarksTextArea.Visible = false;
            PropLabel.Text = "Link";
            PropLabel.Enabled = false;

            var roleid = "";
            if (RoleDropDownList.SelectedIndex > 0)
                roleid = RoleDropDownList.SelectedValue;
            else
                roleid = GenerateRoleID();


            var links = GetLinks(roleid, "role id");
            var ids = links.Select(x => x.ParentID).ToList();
            var fullnames = new List<string> { "" };

            var names = links.Select(x => x.HRef).ToList();

            fullnames.AddRange(ids);
            PropDropDownList.DataSource = fullnames;
            PropDropDownList.DataBind();

            PopulateLinkEditPage();

            LinkDropDownList.DataSource = names;
            LinkDropDownList.DataBind();
        }

        protected void PropButton_Click(object sender, EventArgs e)
        {
            SetPropertyPanel();
        }

        protected void AnnotationButton_Click(object sender, EventArgs e)
        {
            SetAnnotationPanel();
        }

        protected void LinkButton_Click(object sender, EventArgs e)
        {
            SetLinkPanel();
        }

        protected void AddEditButton_Click(object sender, EventArgs e)
        {
            string roleid;
            Errors = "";
            PropStatusLabel.Text = "";
            
           if(RoleDropDownList.SelectedIndex == 0)
                roleid = GenerateRoleID();
            else
               roleid = RoleDropDownList.SelectedValue;
            
         
            RoleID = roleid;

            if(PropLabel.Text=="Property")
            {
                var propid = GeneratePropID(roleid, "role id");

                if(PropDropDownList.SelectedIndex>0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<metadata><role id=\"{0}\"><prop id=\"{1}\">", roleid, propid);
                var name = NameTextBox.Text;
                var tagname = string.Format("<metadata><role id=\"{0}\"><prop id=\"{1}\" name>", roleid, propid);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<metadata><role id=\"{0}\"><prop id=\"{1}\" ns>", roleid, propid);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<metadata><role id=\"{0}\"><prop id=\"{1}\" class>", roleid, propid);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><role id=\"{0}\"><prop id=\"{1}\">", roleid, propid);

                Errors +=InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "role id, prop id", propid, 1);
                Errors +=InsertElementToDataBase(DOID, SystemID, "name", propid.GetType(), tagname, "role id, prop name", name, 1);

                Errors +=InsertElementToDataBase(DOID, SystemID, "ns", propid.GetType(), tagns, "role id, prop ns", ns, 1);

                Errors +=InsertElementToDataBase(DOID, SystemID, "class", propid.GetType(), tagClass, "role id, prop class", classn, 1);
                Errors +=InsertElementToDataBase(DOID, SystemID, "value", propid.GetType(), tagValue, "role id, prop", value, 1);

               

            }

            if (PropLabel.Text == "Annotation")
            {
                var annotationId = GenerateAnnotationID(roleid, "role id");

                if (PropDropDownList.SelectedIndex > 0)
                {
                    annotationId = PropDropDownList.SelectedValue;
                }

                var tagAnnotationid = string.Format("<metadata><role id=\"{0}\"><annotation id=\"{1}\">", roleid, annotationId);
                var name = NameTextBox.Text;
                var tagname = string.Format("<metadata><role id=\"{0}\"><annotation id=\"{1}\" name>", roleid, annotationId);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<metadata><role id=\"{0}\"><annotation id=\"{1}\" ns>", roleid, annotationId);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<metadata><role id=\"{0}\"><annotation id=\"{1}\" class>", roleid, annotationId);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><role id=\"{0}\"><annotation id=\"{1}\">", roleid, annotationId);
                var remarks = RemarksTextArea.InnerText;
                var tagRemarks = string.Format("<metadata><role id=\"{0}\"><annotation id=\"{1}\"><remarks>", roleid, annotationId);


                Errors +=InsertElementToDataBase(DOID, SystemID, annotationId, annotationId.GetType(), tagAnnotationid, "role id, annotation id", annotationId, 1);
                Errors +=InsertElementToDataBase(DOID, SystemID, "name", annotationId.GetType(), tagname, "role id, annotation name", name, 1);

                Errors +=InsertElementToDataBase(DOID, SystemID, "ns", annotationId.GetType(), tagns, "role id, annotation ns", ns, 1);

                Errors +=InsertElementToDataBase(DOID, SystemID, "class", annotationId.GetType(), tagClass, "role id, annotation class", classn, 1);
                Errors +=InsertElementToDataBase(DOID, SystemID, "value", annotationId.GetType(), tagValue, "role id, annotation", value, 1);

                Errors +=InsertElementToDataBase(DOID, SystemID, "remarks", annotationId.GetType(), tagRemarks, "role id, annotation, remarks", remarks, 1);

            }

            if (PropLabel.Text == "Link")
            {
                var linkid =GenerateLinkID(roleid, "role id");

                if (PropDropDownList.SelectedIndex > 0)
                {
                    linkid = PropDropDownList.SelectedValue;
                }
               var  linkHref = NameTextBox.Text;
                var taghref = string.Format("<metadata><role id=\"{0}\"><link id=\"{1}\" href>", roleid, linkid);
                var tagid = string.Format("<metadata><role id=\"{0}\"><link id=\"{1}\">", roleid, linkid);
                var  media = NSTextBox.Text;
                var tagrel = string.Format("<metadata><role id=\"{0}\"><link id=\"{1}\" rel>", roleid, linkid);

                var rel = ClassTextBox.Text;
                var tagmedia = string.Format("<metadata><role id=\"{0}\"><link id=\"{1}\" media-type>", roleid, linkid);


                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><role id=\"{0}\"><link id=\"{1}\">", roleid, linkid);

                Errors += InsertElementToDataBase(DOID, SystemID, linkid, linkHref.GetType(), taghref, "role id, link id", linkid, 1);

                Errors +=InsertElementToDataBase(DOID, SystemID, "link", linkHref.GetType(), taghref, "role id, link href", linkHref, 1);
                Errors +=InsertElementToDataBase(DOID, SystemID, "rel", rel.GetType(), tagrel, "role id, link rel", rel, 1);

                Errors +=InsertElementToDataBase(DOID, SystemID, "media-type", media.GetType(), tagmedia, "role id, link media-type", media, 1);              
                Errors +=InsertElementToDataBase(DOID, SystemID, "value", value.GetType(), tagValue, "role id, link", value, 1);


            }

            //var tagroleid = string.Format("<metadata><role id=\"{0}\">", roleid);

            //var roleTitle = this.TitleTextBox.Text;
            //var tagTitle = string.Format("<metadata><role id=\"{0}\"><title>", roleid);


            //var roleShortName = this.ShortNameTextBox.Text;
            //var tagShortName = string.Format("<metadata><role id=\"{0}\"><short-name>", roleid);

            //var desc = this.DescriptionTextArea.InnerText;
            //var tagDesc = string.Format("<metadata><role id=\"{0}\"><desc>", roleid);


            //Errors +=InsertElementToDataBase(DOID, SystemID, roleid, roleid.GetType(), tagroleid, "role id, role id", roleTitle + " " + desc, 1);
            //Errors +=InsertElementToDataBase(DOID, SystemID, "title", roleid.GetType(), tagTitle, "role id, title", roleTitle, 1);

            //Errors +=InsertElementToDataBase(DOID, SystemID, "desc", roleid.GetType(), tagDesc, "role id, desc", desc, 1);

            //Errors +=InsertElementToDataBase(DOID, SystemID, "short-name", roleid.GetType(), tagShortName, "role id, short-name", roleShortName, 1);

            if (Errors.Length > 0)
            {
                PropStatusLabel.Text = Errors;
                PropStatusLabel.BackColor = Color.Red;
                AddPropPanel.Visible = true;
            }

            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
            GridviewPanel.Visible = false;

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
            if(PropLabel.Text =="Link")
            {
                PopulateLinkEditPage();
            }

                AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            RoleDropDownList.Width = Width;
            RoleDropDownList.Visible = true;
            DeleteButton.Visible = true;

            AddPropPanel.Visible = true;
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
                var tagpropid = string.Format("<metadata><role id=\"{0}\"><prop id=\"{1}\">", revisionId, propid);
                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "doc id, prop id", propid, 0);
            }
            if (PropLabel.Text == "Annotation")
            {
                var propid = PropDropDownList.SelectedValue;
                var tagpropid = string.Format("<metadata><role id=\"{0}\"><annotation id=\"{1}\">", revisionId, propid);
                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "doc id, annotation id", propid, 0);
            }
            if (PropLabel.Text == "Link")
            {
                var propid = PropDropDownList.SelectedValue;
                var tagpropid = string.Format("<metadata><role id=\"{0}\"><link id=\"{1}\">", revisionId, propid);
                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "doc id, link id", propid, 0);
            }

            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            RoleDropDownList.Width = Width;
            RoleDropDownList.Visible = true;
            DeleteButton.Visible = true;

            AddPropPanel.Visible = true;
        }
    }
}