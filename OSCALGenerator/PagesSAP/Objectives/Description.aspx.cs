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

namespace OSCALGenerator.PagesSAP.Objectives
{
    public partial class Description : BasePage
    {
       
        List<Prop> Props;

        protected new void Page_Load(object sender, EventArgs e)
        {
            FaultyCacheBackHomeSAP();
            UserName = Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = int.Parse(Cache["SystemId"].ToString());

            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);
            
            AddDescriptionPanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = false;
  

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
                PopulateEditPage();
        }


        protected void SaveButton_Click(object sender, EventArgs e)
        {
           
            var tagdesc = string.Format("<objectives><description>");

            var desc = DescriptionTextArea.InnerText;

            InsertElementToDataBase(DOID, SystemID, "description", desc.GetType(), tagdesc, "objectives, description", desc, 1);
           
            MainDiv.Visible = false;
            Response.Redirect(@"~/PagesSAP/Objectives/Controls.aspx", false);


        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = false;
           
           
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAP/Import-SSP/ImportSSP.aspx", false);
        }

    
        private void PopulateAnnotationEditPage()
        {           

            var anns = GetAnnotations("", "objectives description");
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

      
        private void PopulatePropertEditPage()
        {

            var props = GetProps("", "objectives description");
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
            var desc = GetData("description", "<objectives><description>", UserName, DOID);
            DescriptionTextArea.InnerText = desc;

            Props = GetProps("", "objectives description");
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            var Anns = GetAnnotations("", "objectives description");
            var annNames = Anns.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();
        }

     

        void SetPropertyPanel()
        {
            AddDescriptionPanel.Visible = true;
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

            var props = GetProps("", "objectives description");
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
            AddDescriptionPanel.Visible = true;
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

           
            var annotations = GetAnnotations("", "objectives description");
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
                var taghref = string.Format("<metadata><{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

                var rel = NSTextBox.Text;
                var tagrel = string.Format("<metadata><{2}=\"{0}\"><link href=\"{1}\" rel>", entityId, linkHref, entityName, entityName);

                var media = ClassTextBox.Text;
                var tagmedia = string.Format("<metadata><{2}=\"{0}\"><link href=\"{1}\" media-type>", entityId, linkHref, entityName);


                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

                InsertElementToDataBase(DOID, SystemID, "link", linkHref.GetType(), taghref, entityName + ", link href", linkHref, 1);
                InsertElementToDataBase(DOID, SystemID, "rel", rel.GetType(), tagrel, entityName + ", link rel", rel, 1);

                InsertElementToDataBase(DOID, SystemID, "media-type", media.GetType(), tagmedia, entityName + ", link media-type", media, 1);


                InsertElementToDataBase(DOID, SystemID, "value", value.GetType(), tagValue, entityName + ", link", value, 1);

            }
        }

        protected void AddEditButton_Click(object sender, EventArgs e)
        {
            ProcessEntity("", "objectives description");
            MainDiv.Visible = true;
            AddDescriptionPanel.Visible = true;
           
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
           
            AddDescriptionPanel.Visible = true;
            MainDiv.Visible = true;
          
            AddPropPanel.Visible = true;
        }
    }
}