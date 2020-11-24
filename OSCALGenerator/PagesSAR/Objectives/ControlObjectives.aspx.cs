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

namespace OSCALGenerator.PagesSAR.Objectives
{
    public partial class ControlObjectives : BasePage
    {
        List<Prop> Props;
        List<Annotation> Annotations;
        List<string> ControlIDs;

        List<List<string>> IncludedControls;
        List<List<string>> ExcludedControls;


        private string SelectedBaseline;
        // private string ControlId;
        protected new void Page_Load(object sender, EventArgs e)
        {
            FaultyCacheBackHomeSAR();
            UserName = Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = int.Parse(Cache["SystemId"].ToString());

            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);

            AddDescriptionPanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = false;
            AddControlPanel.Visible = false;

            IncludedControls = GetControlList("include-objective objective-id");
            ExcludedControls = GetControlList("exclude-objective objective-id");
            IncludeControlsDropDownList.DataSource = IncludedControls;
            IncludeControlsDropDownList.DataBind();

            ExcludeControlsDropDownList.DataSource = ExcludedControls;
            ExcludeControlsDropDownList.DataBind();

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
                GetIds();
                PopulateEditPage();
                RadioButtonList1.SelectedValue = "Moderate";
                ControlDropDownList.DataSource = ControlIDs;
                ControlDropDownList.DataBind();
            }
        }


        protected void SaveButton_Click(object sender, EventArgs e)
        {

            var tagdesc = string.Format("<objectives><control-objectives><description>");

            var desc = DescriptionTextArea.InnerText;

            var tagAll = string.Format("<objectives><control-objectives><all>");
            var all = AllTextBox.Text;

            InsertElementToDataBase(DOID, SystemID, "description", desc.GetType(), tagdesc, "control-objectives description", desc, 1);
            InsertElementToDataBase(DOID, SystemID, "all", all.GetType(), tagAll, "control-objectives all", all, 1);

            MainDiv.Visible = false;
            Response.Redirect(@"~/PagesSAR/Objectives/Methods.aspx", false);


        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = false;


        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAR/Objectives/Controls.aspx", false);
        }


        private void PopulateAnnotationEditPage()
        {
            var rawTag = string.Format("<objectives><{1}=\"{0}\">", "", "control-objectives");
            var anns = GetAnnotations("", "control-objectives", rawTag);
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

        public List<ControlParamStatement> GetIds()
        {
            var result = new List<ControlParamStatement>();
            ControlIDs = new List<string>();
            string appPath = Request.PhysicalApplicationPath;
            var file = "";


            if (RadioButtonList1.SelectedItem != null)
                SelectedBaseline = RadioButtonList1.SelectedItem.Value;

            var tagbaseline = string.Format("<objectives><control-objectives><baseline>");
            string baseline = "";

            switch (SelectedBaseline)
            {
                case "Low":
                    {
                        file = string.Format(@"{0}\{1}", appPath, "ControlParamStatementsLow.txt");
                        baseline = "low";
                    }
                    break;
                case "Moderate":
                    {
                        file = string.Format(@"{0}\{1}", appPath, "ControlParamStatementsModerate.txt");
                        baseline = "moderate";
                    }
                    break;
                case "High":
                    {
                        file = string.Format(@"{0}\{1}", appPath, "ControlParamStatementsHigh.txt");
                        baseline = "high";
                    }
                    break;
                default:
                    {
                        file = string.Format(@"{0}\{1}", appPath, "ControlParamStatementsModerate.txt");
                        baseline = "moderate";
                    }
                    break;
            }
            InsertElementToDataBase(DOID, SystemID, "baseline", baseline.GetType(), tagbaseline, "control-objectives baseline", baseline, 1);
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
                    //  ControlIdToRank.Add(controlid, n);
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


                    result.Add(tepo);
                }

            }



            return result;
        }


        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetIds();
            ControlDropDownList.DataSource = ControlIDs;
            ControlDropDownList.DataBind();
        }

        private void PopulatePropertEditPage()
        {
            var rawTag = string.Format("<objectives><{1}=\"{0}\">", "", "controls");
            Props = GetProps("", "controls", rawTag);
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
            var desc = GetData("description", "<objectives><control-objectives><description>", UserName, DOID);
            DescriptionTextArea.InnerText = desc;

            var all = GetData("all", "<objectives><control-objectives><all>", UserName, DOID);
            AllTextBox.Text = all;
            var rawTag = string.Format("<objectives><{1}=\"{0}\">", "", "control-objectives");

            Props = GetProps("", "control-objectives", rawTag);
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            Annotations = GetAnnotations("", "control-objectives", rawTag);
            var annNames = Annotations.Select(x => x.Name).ToList();
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
            var rawTag = string.Format("<objectives><{1}=\"{0}\">", "", "control-objectives");
            Props = GetProps("", "control-objectives", rawTag);
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

            var rawTag = string.Format("<objectives><{1}=\"{0}\">", "", "control-objectives");
            var annotations = GetAnnotations("", "control-objectives", rawTag);
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

            var rawTag = string.Format("<objectives><{1}=\"{0}\">", entityId, entityName);

            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(entityId, entityName, rawTag);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<objectives><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<objectives><{2}=\"{0}\"><prop id=\"{1}\" name>", entityId, propid, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<objectives><{2}=\"{0}\"><prop id=\"{1}\" ns>", entityId, propid, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<objectives><{2}=\"{0}\"><prop id=\"{1}\" class>", entityId, propid, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<objectives><{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName);

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

                var tagAnnotationid = string.Format("<objectives><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var name = NameTextBox.Text;
                var tagname = string.Format("<objectives><{2}=\"{0}\"><annotation id=\"{1}\" name>", entityId, annotationId, entityName);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<objectives><{2}=\"{0}\"><annotation id=\"{1}\" ns>", entityId, annotationId, entityName);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<objectives><{2}=\"{0}\"><annotation id=\"{1}\" class>", entityId, annotationId, entityName);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<objectives><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);
                var remarks = RemarksTextArea.InnerText;
                var tagRemarks = string.Format("<objectives><{2}=\"{0}\"><annotation id=\"{1}\"><remarks>", entityId, annotationId, entityName);


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
                var taghref = string.Format("<objectives><{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

                var rel = NSTextBox.Text;
                var tagrel = string.Format("<objectives><{2}=\"{0}\"><link href=\"{1}\" rel>", entityId, linkHref, entityName, entityName);

                var media = ClassTextBox.Text;
                var tagmedia = string.Format("<objectives><{2}=\"{0}\"><link href=\"{1}\" media-type>", entityId, linkHref, entityName);


                var value = ValueTextBox.Text;
                var tagValue = string.Format("<objectives><{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

                InsertElementToDataBase(DOID, SystemID, "link", linkHref.GetType(), taghref, entityName + ", link href", linkHref, 1);
                InsertElementToDataBase(DOID, SystemID, "rel", rel.GetType(), tagrel, entityName + ", link rel", rel, 1);

                InsertElementToDataBase(DOID, SystemID, "media-type", media.GetType(), tagmedia, entityName + ", link media-type", media, 1);


                InsertElementToDataBase(DOID, SystemID, "value", value.GetType(), tagValue, entityName + ", link", value, 1);

            }
        }

        protected void AddEditButton_Click(object sender, EventArgs e)
        {
            ProcessEntity("", "control-objectives");
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

        protected void IncludeControlsButton_Click(object sender, EventArgs e)
        {
            AddControlPanel.Visible = true;
            ControlLabel.Text = "Select Included Controls";
        }

        protected void ExcludeControlsButton_Click(object sender, EventArgs e)
        {
            AddControlPanel.Visible = true;
            ControlLabel.Text = "Select Excluded Controls";
        }

        protected void ControlDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var val = ControlDropDownList.SelectedValue;
            var tag = string.Format("<objectives><controls><include-objective objective-id=\"{0}\">", val);
            if (ControlLabel.Text == "Select Included Controls")
            {
                StatementTextBox.Text = GetData("include-objective", tag, UserName, DOID);
            }

            if (ControlLabel.Text == "Select Excluded Controls")
            {
                StatementTextBox.Text = GetData("exclude-objective", tag, UserName, DOID);
            }

            this.AddControlPanel.Visible = true;
        }

        protected void AddButton_Click(object sender, EventArgs e)
        {
            if (ControlLabel.Text == "Select Included Controls")
            {
                var val = ControlDropDownList.SelectedValue;
                var statement = StatementTextBox.Text;
                var tag = string.Format("<objectives><controls><include-objective objective-id=\"{0}\">", val);
                InsertElementToDataBase(DOID, SystemID, val, val.GetType(), tag, "include-objective objective-id", statement, 1);
            }

            if (ControlLabel.Text == "Select Excluded Controls")
            {
                var val = ControlDropDownList.SelectedValue;
                var statement = StatementTextBox.Text;
                var tag = string.Format("<objectives><controls><exclude-objective objective-id=\"{0}\">", val);
                InsertElementToDataBase(DOID, SystemID, val, val.GetType(), tag, "exclude-objective objective-id", statement, 1);
            }

        }

     
    }
}