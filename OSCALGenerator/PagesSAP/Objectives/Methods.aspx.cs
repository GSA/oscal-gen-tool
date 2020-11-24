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

namespace OSCALGenerator.PagesSAP.Objectives
{
    public partial class Methods : BasePage
    {
        List<Prop> Props;
        List<Annotation> Annotations;
        List<Link> Links;
        List<string> ControlIDs;
        List<Method> Methodos;
        
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

                GridviewPanel.Visible = true;
                AddDescriptionPanel.Visible = false;
                MainDiv.Visible = false;
                AddPropPanel.Visible = false;
                AddControlPanel.Visible = false;
                Methodos = GetMethods();
                var bindingRevision = new BindingList<Method>(Methodos);
                MethodGridView.DataSource = bindingRevision;
                MethodGridView.DataBind();

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
                AddPartPanel.Visible = false;
                AddDescriptionPanel.Visible = true;
                if (!IsPostBack)
                {
                    GetIds();

                    PopulateEditPage();
                    ControlDropDownList.DataSource = ControlIDs;
                    ControlDropDownList.DataBind();
                }
            }
            catch (Exception ex)
            {
                StatusLabel.Text = ex.Message;
            }
        }

        string GenerateMethodId()
        {
            var count = GetAllHeaders("method id, method id", UserName, DOID).Count;
            var id = string.Format("method-{0}", count);
            return id;
        }
        public List<ControlParamStatement> GetIds()
        {
            var result = new List<ControlParamStatement>();
            ControlIDs = new List<string>();
            string appPath = Request.PhysicalApplicationPath;
            var file = "";

            var tagBaseline = string.Format("<objectives><control-objectives><baseline>");

            var selectedBaseline = GetData("baseline", tagBaseline, UserName, DOID);
            if (selectedBaseline.Length == 0)
                selectedBaseline = "Moderate";

            switch (selectedBaseline)
            {
                case "Low":
                    {
                        file = string.Format(@"{0}\{1}", appPath, "ControlParamStatementsLow.txt");
                    }
                    break;
                case "Moderate":
                    {
                        file = string.Format(@"{0}\{1}", appPath, "ControlParamStatementsModerate.txt");
                    }
                    break;
                case "High":
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

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            var id = "";
            if (ControlDropDownList.SelectedIndex == 0)
            {
                id = GenerateMethodId();
            }
            else id = ControlDropDownList.SelectedValue;

            var tagId = string.Format("<objectives><method id=\"{0}\">", id);
            var tagUUId = string.Format("<objectives><method id=\"{0}\"><uuid>", id);
            var tagdesc = string.Format("<objectives><method id=\"{0}\"><description>", id);
           
            var desc = DescriptionTextArea.InnerText;
            var remark = OBJRemarkTextarea.InnerText;
            var tagRemark = string.Format("<objectives><method id=\"{0}\"><remarks>", id);
            var guid = Guid.NewGuid().ToString();

            InsertElementToDataBase(DOID, SystemID, id, desc.GetType(), tagId, "method id, method id", id, 1);
            InsertElementToDataBase(DOID, SystemID, "uuid", desc.GetType(), tagUUId, "method id, uuid", guid, 1);

            InsertElementToDataBase(DOID, SystemID, "description", desc.GetType(), tagdesc, "method id, description", desc, 1);
            InsertElementToDataBase(DOID, SystemID, "remarks", remark.GetType(), tagRemark, "method id, remarks", remark, 1);

            MainDiv.Visible = false;
            GridviewPanel.Visible = true;

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = false;
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAP/Objectives/ControlObjectives.aspx", false);
        }


        private void PopulateAnnotationEditPage()
        {
            //var rawTag = string.Format("<objectives><{1}=\"{0}\">", "", "control-objectives");
            //var anns = GetAnnotations("", "control-objectives", rawTag);
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

        private void PopulateLinkEditPage()
        {

            var propId = PropDropDownList.SelectedValue;
            var prop = new Link();
            foreach (var elt in Links)
            {
                if (elt.ParentID == propId)
                {
                    prop = elt;
                    break;
                }
            }
            NameTextBox.Text = prop.HRef;
            NSTextBox.Text = prop.MediaType;
            ClassTextBox.Text = prop.Rel;
            ValueTextBox.Text = prop.MarkUpLine;
        }


        private void PopulateEditPage()
        {
            var id = ControlDropDownList.SelectedValue;
            var meth = Methodos.Where(x => x.ID == id).FirstOrDefault();
            meth = meth.ID == null ? new Method() : meth;
           
            DescriptionTextArea.InnerText = meth.Description;

            OBJRemarkTextarea.InnerHtml = meth.Remarks;
            var rawTag = string.Format("<objectives><method id=\"{0}\">", id);

            Props = GetProps(id, "method id", rawTag);
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            Annotations = GetAnnotations(id, "method id", rawTag);
            var annNames = Annotations.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();
        }

        void SetPropertyPanel()
        {
            var id = ControlDropDownList.SelectedValue;
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

            var rawTag = string.Format("<objectives><method id=\"{0}\">", id);
            Props = GetProps(id, "method id", rawTag);
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

            var id = ControlDropDownList.SelectedValue;
            var rawTag = string.Format("<objectives><method id=\"{0}\">", id);
            Annotations = GetAnnotations(id, "method id", rawTag);
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

        public void RemoveEntity(string entityId, string entityName)
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


                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, entityName + ", prop id", propid, 0);


            }

            if (PropLabel.Text == "Annotation")
            {
                var annotationId = GenerateAnnotationID(entityId, entityName, rawTag);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    annotationId = PropDropDownList.SelectedValue;
                }

                var tagAnnotationid = string.Format("<objectives><{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName);



                InsertElementToDataBase(DOID, SystemID, annotationId, annotationId.GetType(), tagAnnotationid, entityName + ", annotation id", annotationId, 0);

            }

            if (PropLabel.Text == "Link")
            {
                var linkid = "";

                if (PropDropDownList.SelectedIndex > 0)
                {
                    linkid = PropDropDownList.SelectedValue;
                }
                else
                    linkid = NameTextBox.Text;


                var taghref = string.Format("<objectives><{2}=\"{0}\"><link id=\"{1}\">", entityId, linkid, entityName);

                InsertElementToDataBase(DOID, SystemID,linkid , linkid.GetType(), taghref, entityName + ", link id", linkid, 0);

            }
        }

        public void ProcessEntity(string entityId, string entityName)
        {
            Errors = "";
            PropStatusLabel.Text = "";
            var rawTag = string.Format("<objectives><{1}=\"{0}\">", entityId, entityName);

            if (PropLabel.Text.Contains("Property"))
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

                Errors +=InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, entityName + ", prop id", propid, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "name", propid.GetType(), tagname, entityName + ", prop name", name, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "ns", propid.GetType(), tagns, entityName + ", prop ns", ns, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "class", propid.GetType(), tagClass, entityName + ", prop class", classn, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "value", propid.GetType(), tagValue, entityName + ", prop", value, 1);

                var props = GetProps(entityId, entityName);
                var names = props.Select(x => x.Name).ToList();
                var fullnames = new List<string> { "" };
                fullnames.AddRange(names);
                PropDropDownList.DataSource = fullnames;
                PropDropDownList.DataBind();

            }

            if (PropLabel.Text.Contains("Annotation"))
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


                Errors += InsertElementToDataBase(DOID, SystemID, annotationId, annotationId.GetType(), tagAnnotationid, entityName + ", annotation id", annotationId, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "name", annotationId.GetType(), tagname, entityName + ", annotation name", name, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "ns", annotationId.GetType(), tagns, entityName + ", annotation ns", ns, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "class", annotationId.GetType(), tagClass, entityName + ", annotation class", classn, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "value", annotationId.GetType(), tagValue, entityName + ", annotation", value, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "remarks", annotationId.GetType(), tagRemarks, entityName + ", annotation, remarks", remarks, 1);

            }

            if (PropLabel.Text.Contains("Link"))
            {
                var linkid = GenerateLinkID(entityId, entityName);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    linkid = PropDropDownList.SelectedValue;
                }
                var linkHref = NameTextBox.Text;
                var taghref = string.Format("<objectives><{2}=\"{0}\"><link id=\"{1}\" href>", entityId, linkid, entityName);
                var tagid = string.Format("<objectives><{2}=\"{0}\"><link id=\"{1}\">", entityId, linkid, entityName);
                var   media = NSTextBox.Text;
                var tagrel = string.Format("<objectives><{2}=\"{0}\"><link id=\"{1}\" rel>", entityId, linkid, entityName, entityName);

                var rel = ClassTextBox.Text;
                var tagmedia = string.Format("<objectives><{2}=\"{0}\"><link id=\"{1}\" media-type>", entityId, linkid, entityName);


                var value = ValueTextBox.Text;
                var tagValue = string.Format("<objectives><{2}=\"{0}\"><link id=\"{1}\">", entityId, linkid, entityName);

                Errors += InsertElementToDataBase(DOID, SystemID, linkid, linkHref.GetType(), taghref, entityName + ", link id", linkid, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "link", linkHref.GetType(), taghref, entityName + ", link href", linkHref, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "rel", rel.GetType(), tagrel, entityName + ", link rel", rel, 1);

                Errors += InsertElementToDataBase(DOID, SystemID, "media-type", media.GetType(), tagmedia, entityName + ", link media-type", media, 1);
                Errors += InsertElementToDataBase(DOID, SystemID, "value", value.GetType(), tagValue, entityName + ", link", value, 1);

            }

            if(Errors.Length>0)
            {
                AddPropPanel.Visible = true;
                PropStatusLabel.ForeColor = Color.Red;
                PropStatusLabel.Text = Errors;
            }
        }

        protected void AddEditButton_Click(object sender, EventArgs e)
        {
            var id = "";
            if (ControlDropDownList.SelectedIndex == 0)
            {
                id = GenerateMethodId();
            }
            else id = ControlDropDownList.SelectedValue;

            if (PropLabel.Text.Contains("Part"))
            {
                ProcessEntity(id, "part");
                AddPartPanel.Visible = true;

            }
            else
            {
                ProcessEntity(id, "method id");
            }

            MainDiv.Visible = true;
            AddDescriptionPanel.Visible = true;

        }

        protected void PropDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = "";
            if (ControlDropDownList.SelectedIndex == 0)
            {
                id = GenerateMethodId();
            }
            else id = ControlDropDownList.SelectedValue;
            var rawTag = "";
            if (PropLabel.Text.Contains("Property"))
            {
                if (!(PropLabel.Text == "Part Property"))
                {
                    rawTag = string.Format("<objectives><method id=\"{0}\">", id);
                    Props = GetProps(id, "method id", rawTag);
                    PopulatePropertEditPage();
                }
                else
                {
                    rawTag = string.Format("<objectives><part=\"{0}\">", id);
                    Props = GetProps(id, "part", rawTag);
                    PopulatePropertEditPage();
                }

            }
            if (PropLabel.Text.Contains("Annotation"))
            {
                if (!(PropLabel.Text == "Part Annotation"))
                {
                    rawTag = string.Format("<objectives><method id=\"{0}\">", id);
                    Annotations = GetAnnotations(id, "method id", rawTag);
                    PopulateAnnotationEditPage();
                }
                else
                {
                    rawTag = string.Format("<objectives><part=\"{0}\">", id);
                    Annotations = GetAnnotations(id, "part", rawTag);
                    PopulateAnnotationEditPage();
                }
            }
            if (PropLabel.Text == "Link")
            {
                rawTag = string.Format("<objectives><part=\"{0}\">", id);
                Links = GetLinks(id, "part", rawTag);
                PopulateLinkEditPage();
            }

            AddDescriptionPanel.Visible = true;
            MainDiv.Visible = true;

            AddPropPanel.Visible = true;
        }







        protected void AddPartButton_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = true;
            AddDescriptionPanel.Visible = true;
            AddPartPanel.Visible = true;
            GridviewPanel.Visible = false;
            PartStatusLabel.Text = "";
            RestorePart();
        }

        protected void PropPartButton_Click(object sender, EventArgs e)
        {
            AddDescriptionPanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = true;
            AddPartPanel.Visible = true;
            NameLabel.Text = "Name";
            ClassLabel.Text = "Class";
            NSLabel.Text = "NS";
            ValueLabel.Text = "Value";
            PropLabel.Text = "Part Property";
            PropLabel.Enabled = false;
            RemarksLabel.Text = "";
            RemarksTextArea.Visible = false;
            var rawTag = string.Format("<objectives><part=\"\">");
            Props = GetProps("", "part", rawTag);
            var names = Props.Select(x => x.Name).ToList();
            var fullnames = new List<string> { "" };

            var ids = Props.Select(x => x.ID).ToList();
            var fullIDs = new List<string> { "" };
            fullIDs.AddRange(ids);

            fullnames.AddRange(names);
            PropDropDownList.DataSource = fullIDs;
            PropDropDownList.DataBind();

            PopulatePropertEditPage();

            PropPartDropDownList.DataSource = names;
            PropPartDropDownList.DataBind();

        }

        protected void LinkPartButton_Click(object sender, EventArgs e)
        {
            AddDescriptionPanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = true;
            AddPartPanel.Visible = true;
            NameLabel.Text = "Href";
            ClassLabel.Text = "Rel";
            NSLabel.Text = "Media-type";
            ValueLabel.Text = "Value";
            PropLabel.Text = "Part Link";
            PropLabel.Enabled = false;
            RemarksLabel.Text = "";
            RemarksTextArea.Visible = false;
            var rawTag = string.Format("<objectives><part=\"\">");
            Links = GetLinks("", "part", rawTag);
            var names = Links.Select(x => x.HRef).ToList();
            var fullnames = new List<string> { "" };

            fullnames.AddRange(names);
            PropDropDownList.DataSource = fullnames;
            PropDropDownList.DataBind();

            PopulateLinkEditPage();

            PropPartDropDownList.DataSource = names;
            PropPartDropDownList.DataBind();

        }

        protected void RealPartButton_Click(object sender, EventArgs e)
        {
            AddDescriptionPanel.Visible = true;
            MainDiv.Visible = true;
            AddPartPanel.Visible = true;
            AddControlPanel.Visible = true;
            GridviewPanel.Visible = false;
            var parts = GetParts();
            var all = new List<string> { "" };
            all.AddRange(parts);

            PartsDropDownList.DataSource = all;
            PartsDropDownList.DataBind();
        }

        protected void AddButton_Click(object sender, EventArgs e)
        {
            var id = "";
            if (ControlDropDownList.SelectedIndex == 0)
            {
                id = GenerateMethodId();
            }
            else id = ControlDropDownList.SelectedValue;

            var text = StatementTextBox.Text;
            var tag = string.Format("<objectives><method id=\"{0}\"><part><part>", id);
            InsertElementToDataBase(DOID, SystemID, text, text.GetType(), tag, "method id, part part", text, 1);
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            var text = PartsDropDownList.SelectedValue;
            var tag = string.Format("<objectives><objective><part><part>");
            if (text.Length == 0)
                return;

            InsertElementToDataBase(DOID, SystemID, text, text.GetType(), tag, "method id, part part", text, 0);
        }

        protected void RemoveButton_Click(object sender, EventArgs e)
        {

        }

        protected void SaveTestStepButton_Click(object sender, EventArgs e)
        {
            var id = "";
            if (ControlDropDownList.SelectedIndex == 0)
            {
                id = GenerateMethodId();
            }
            else id = ControlDropDownList.SelectedValue;


            var tagName = string.Format("<objectives><method id=\"{0}\"><part><name>", id);
            var name = PartNameTextBox.Text;
            var tagClass = string.Format("<objectives><method id=\"{0}\"><part><class>", id);
            var classo = PartClassTextBox.Text;
            var tagNS = string.Format("<objectives><method id=\"{0}\"><part><ns>", id);
            var NS = PartNSTextBox.Text;
            var tagTitle = string.Format("<objectives><method id=\"{0}\"><part><title>", id);
            var desc = PrivilegeDescTextArea.InnerHtml;
            var tagDesc = string.Format("<objectives><method id=\"{0}\"><part><description>", id);
            var title = PartTitleTextBox.Text;
            Errors +=InsertElementToDataBase(DOID, SystemID, "name", name.GetType(), tagName, "method id, part name", name, 1);
            Errors +=InsertElementToDataBase(DOID, SystemID, "class", classo.GetType(), tagClass, "method id, part class", classo, 1);
            Errors += InsertElementToDataBase(DOID, SystemID, "ns", NS.GetType(), tagNS, "method id, part ns", NS, 1);
            Errors += InsertElementToDataBase(DOID, SystemID, "desc", desc.GetType(), tagDesc, "method id, part description", desc, 1);
            Errors += InsertElementToDataBase(DOID, SystemID, "title", name.GetType(), tagTitle, "method id, part title", title, 1);

            if(Errors.Length>0)
            {
                PartStatusLabel.Text = Errors;
                PartStatusLabel.ForeColor = Color.Red;
                AddPartPanel.Visible = true;
            }

            MainDiv.Visible = true;
            AddDescriptionPanel.Visible = true;
        }

        void RestorePart()
        {
            var id = ControlDropDownList.SelectedValue;
            var meth = Methodos.Where(x => x.ID == id).FirstOrDefault();
            meth = meth.ID == null ? new Method() : meth;
            PartNameTextBox.Text = meth.PartName;
            PartClassTextBox.Text = meth.PartClass;
            PartNSTextBox.Text = meth.PartNS;
            PrivilegeDescTextArea.InnerHtml = meth.PartDescription;
            PartTitleTextBox.Text = meth.PartTitle;

            var rawTag = string.Format("<objectives><objective=\"\"><part=\"\">");
            Links = GetLinks("", "part", rawTag);
            var hrefs = Links.Select(x => x.HRef).ToList();

            LinkPartDropDownList.DataSource = hrefs;
            LinkPartDropDownList.DataBind();

            Props = GetProps("", "part", rawTag);
            var names = Props.Select(x => x.Name).ToList();

            PropPartDropDownList.DataSource = names;
            PropPartDropDownList.DataBind();

            var parts = GetMethodParts(id);
            RealPartDropDownList.DataSource = parts;
            RealPartDropDownList.DataBind();
        }

        protected void PartsDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddPartPanel.Visible = true;
            AddControlPanel.Visible = true;
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAP/Objectives/Objectives.aspx", false);
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            AddDescriptionPanel.Visible = true;
            MainDiv.Visible = true;

         
            DeleteButton.Visible = true;

            var locationIds = Methodos.Select(x => x.ID).ToList(); ;
            var fullList = new List<string> { "" };
            fullList.AddRange(locationIds);
            ControlDropDownList.DataSource = fullList;
            ControlDropDownList.DataBind();

            PopulateEditPage();
            GridviewPanel.Visible = false;
        }

        protected void ControlDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateEditPage();
            AddDescriptionPanel.Visible = true;
            MainDiv.Visible = true;
            GridviewPanel.Visible = false;
        }

        protected void MainDeleteButton_Click(object sender, EventArgs e)
        {
            var id = ControlDropDownList.SelectedValue;

            var tagId = string.Format("<objectives><method id=\"{0}\">", id);
            

            InsertElementToDataBase(DOID, SystemID, id, id.GetType(), tagId, "method id, method id", id, 0);
        }
    }
}