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

namespace OSCALGenerator.PagesSAR.AssessmentActivities
{
    public partial class Schedules : BasePage
    {
        List<Schedule> Subjects;
        List<Task> Tasks;
        List<DocParty> Parties;
        List<Role> Roles;
        List<DocLocation> Locations;
        
        List<Prop> Props;
        List<Annotation> Annotations;
        int Height = 580;
        int AddOn = 125;
        protected new void Page_Load(object sender, EventArgs e)
        {
            FaultyCacheBackHomeSAR();
            UserName = Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = int.Parse(Cache["SystemId"].ToString());

            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);

            Subjects = GetSchedules();

            AddOn = (EndDateCalendar.Visible && StartDateCalendar.Visible) ? 285 : 125;

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
            AddTaskPanel.Visible = false;
            AddRolePanel.Visible = false;
            MainDiv.Visible = false;
            AddPropPanel.Visible = false;
            AddResponsibleRolePanel.Visible = false;
            MainDiv.Visible = false;
            var bindingRoles = new BindingList<Schedule>(Subjects);
            this.RolesGridView.DataSource = bindingRoles;
            RolesGridView.DataBind();

            if (!IsPostBack)
            {

                var names = Subjects.Select(x => x.ID).ToList();
                var fullnames = new List<string> { "" };
                fullnames.AddRange(names);
                RoleDropDownList.DataSource = fullnames;
                RoleDropDownList.DataBind();
                PopulateEditPage();
                StartDateCalendar.Visible = false;
                EndDateCalendar.Visible = false;
                AddTaskPanel.Height = Height; 

            }
        }





        private void PopulateTaskEditPage()
        {
            var methodid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                methodid = GenerateSubjectID();
            }
            else
                methodid = RoleDropDownList.SelectedValue;

            Tasks = GetTasks(methodid);

            var taskId = AddTaskDropDownList.SelectedValue;
            var bon = new Task();
            foreach (var x in Tasks)
            {
                if (x.ID == taskId)
                {
                    bon = x;
                    break;
                }
            }



            CompareTextBox.Text = bon.CompareTo;
            TitleTextBox.Text = bon.Title;
            StartTextBox.Text = bon.Start;
            EndTextBox.Text = bon.End;
            DescTextArea.InnerHtml = bon.Description;
            TaskRemarksTextArea.InnerHtml = bon.Remarks;
            RoleStepDropDownList.DataSource = bon.RoleIds;
            RoleStepDropDownList.DataBind();
            PartyStepDropDownList.DataSource = bon.PartyUUIDs;
            PartyStepDropDownList.DataBind();
            LocationDropDownList.DataSource = bon.LocationUUIDs;
            LocationDropDownList.DataBind();
            ActivityDropDownList.DataSource = bon.ActivityUUIDs;
            ActivityDropDownList.DataBind();

            var roleid = RoleDropDownList.SelectedValue;
            var tag = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\">", roleid, taskId);

            var tagName = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><compare-to>", roleid, taskId);

            var type = GetData("compare-to", tagName, UserName, DOID);
            CompareTextBox.Text = type;


            var tagTitle = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><title>", roleid, taskId);
            TitleTextBox.Text = GetData("title", tagTitle, UserName, DOID);


            var tagDesc = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><description>", roleid, taskId);
            DescTextArea.InnerText = GetData("desc", tagDesc, UserName, DOID);

            var tagRemarks = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><remarks>", roleid, taskId);
            TaskRemarksTextArea.InnerText = GetData("remarks", tagRemarks, UserName, DOID);

            var rawTag = tag;

            Props = GetProps(taskId, "task id", tag);
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            Annotations = GetAnnotations(taskId, "task id", tag);
            var annNames = Annotations.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();
        }




        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAR/AssessmentActivities/TestMethods.aspx", false);
        }


        private void PopulateAnnotationEditPage()
        {

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
            ClassTextBox.Text = ann.ID;
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

        private string GenerateTaskID()
        {
            var nbrRole = Tasks.Count();
            var methodid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                methodid = GenerateSubjectID();
            }
            else
                methodid = RoleDropDownList.SelectedValue;
            var id = string.Format("{1}-Task-{0}", nbrRole, methodid);
            return id;
        }

        private string GenerateSubjectID()
        {
            var nbrRole = Subjects.Count();
            var id = string.Format("Schedule-{0}", nbrRole);
            return id;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            var scheduleid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                scheduleid = GenerateSubjectID();
            }
            else
                scheduleid = RoleDropDownList.SelectedValue;


            var tagroleid = string.Format("<assessment-activities><schedule id=\"{0}\">", scheduleid);
            var taguuid = string.Format("<assessment-activities><schedule id=\"{0}\"><uuid>", scheduleid);

            var preguid = GetData("schedule uuid", taguuid, UserName, DOID);
            var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;


            InsertElementToDataBase(DOID, SystemID, scheduleid, scheduleid.GetType(), tagroleid, "schedule id, schedule id", scheduleid, 1);
            InsertElementToDataBase(DOID, SystemID, "schedule uuid", scheduleid.GetType(), taguuid, "schedule id, uuid", guid, 1);



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
            var tag = string.Format("<assessment-activities><test-method id=\"{0}\">", roleid);

            var tagName = string.Format("<assessment-activities><test-method id=\"{0}\"><compare-to>", roleid);

            var component = Subjects.Where(x => x.ID == roleid).FirstOrDefault();
            var RespRoles = component.Tasks == null ? new List<Task>() : component.Tasks;
            var RoleIds = RespRoles.Select(x => x.ID).ToList();
            TaskDropDownList.DataSource = RoleIds;
            TaskDropDownList.DataBind();




        }

        void SetPropertyPanel()
        {
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            AddTaskPanel.Visible = true;
            AddPropPanel.Visible = true;
            NameLabel.Text = "Name";
            ClassLabel.Text = "Class";
            NSLabel.Text = "NS";
            ValueLabel.Text = "Value";
            PropLabel.Text = "Property";
            PropLabel.Enabled = false;
            RemarksLabel.Text = "";
            RemarksTextArea.Visible = false;

            var scheduleid = "";
            var taskid = "";
            if (RoleDropDownList.SelectedIndex > 0)
                scheduleid = RoleDropDownList.SelectedValue;
            else
                scheduleid = GenerateSubjectID();

            Tasks = GetTasks(scheduleid);

            if (AddTaskDropDownList.SelectedIndex == 0)
            {
                taskid = GenerateTaskID();
            }
            else
                taskid = AddTaskDropDownList.SelectedValue;


            var rawTag = string.Format("<assessment-activities><{1}=\"{0}\"><{2}=\"{3}\">", scheduleid, "schedule id", "task id", taskid);

            Props = GetProps(taskid, "task id", rawTag);
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
            AddTaskPanel.Visible = true;
            NameLabel.Text = "Name";
            ClassLabel.Visible = false;
            ClassTextBox.Visible = false;

            NSLabel.Text = "NS";
            ValueLabel.Text = "Value";
            RemarksLabel.Text = "Remarks";
            RemarksTextArea.Visible = true;
            PropLabel.Text = "Annotation";
            PropLabel.Enabled = false;

            var scheduleid = "";
            var taskid = "";
            if (RoleDropDownList.SelectedIndex > 0)
                scheduleid = RoleDropDownList.SelectedValue;
            else
                scheduleid = GenerateSubjectID();

            Tasks = GetTasks(scheduleid);

            if (AddTaskDropDownList.SelectedIndex == 0)
            {
                taskid = GenerateTaskID();
            }
            else
                taskid = AddTaskDropDownList.SelectedValue;


            var rawTag = string.Format("<assessment-activities><{1}=\"{0}\"><{2}=\"{3}\">", scheduleid, "schedule id", "task id", taskid);

            Annotations = GetAnnotations(taskid, "task id", rawTag);
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

        public void ProcessEntity(string entityId, string entityName, string root)
        {

            var rawTag = string.Format("<assessment-activities>{2}<{1}=\"{0}\">", entityId, entityName, root);

            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(entityId, entityName, rawTag);

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<assessment-activities>{3}<{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName, root);
                var name = NameTextBox.Text;
                var tagname = string.Format("<assessment-activities>{3}<{2}=\"{0}\"><prop id=\"{1}\" name>", entityId, propid, entityName, root);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<assessment-activities>{3}<{2}=\"{0}\"><prop id=\"{1}\" ns>", entityId, propid, entityName, root);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<assessment-activities>{3}<{2}=\"{0}\"><prop id=\"{1}\" class>", entityId, propid, entityName, root);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<assessment-activities>{3}<{2}=\"{0}\"><prop id=\"{1}\">", entityId, propid, entityName, root);

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

                var tagAnnotationid = string.Format("<assessment-activities>{3}<{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName, root);
                var name = NameTextBox.Text;
                var tagname = string.Format("<assessment-activities>{3}<{2}=\"{0}\"><annotation id=\"{1}\" name>", entityId, annotationId, entityName, root);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<assessment-activities>{3}<{2}=\"{0}\"><annotation id=\"{1}\" ns>", entityId, annotationId, entityName, root);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<assessment-activities>{3}<{2}=\"{0}\"><annotation id=\"{1}\" class>", entityId, annotationId, entityName, root);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<assessment-activities>{3}<{2}=\"{0}\"><annotation id=\"{1}\">", entityId, annotationId, entityName, root);
                var remarks = RemarksTextArea.InnerText;
                var tagRemarks = string.Format("<assessment-activities>{3}<{2}=\"{0}\"><annotation id=\"{1}\"><remarks>", entityId, annotationId, entityName, root);


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
                var taghref = string.Format("<assessment-activities><{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

                var rel = NSTextBox.Text;
                var tagrel = string.Format("<assessment-activities><{2}=\"{0}\"><link href=\"{1}\" rel>", entityId, linkHref, entityName, entityName);

                var media = ClassTextBox.Text;
                var tagmedia = string.Format("<assessment-activities><{2}=\"{0}\"><link href=\"{1}\" media-type>", entityId, linkHref, entityName);


                var value = ValueTextBox.Text;
                var tagValue = string.Format("<assessment-activities><{2}=\"{0}\"><link href=\"{1}\">", entityId, linkHref, entityName);

                InsertElementToDataBase(DOID, SystemID, "link", linkHref.GetType(), taghref, entityName + ", link href", linkHref, 1);
                InsertElementToDataBase(DOID, SystemID, "rel", rel.GetType(), tagrel, entityName + ", link rel", rel, 1);

                InsertElementToDataBase(DOID, SystemID, "media-type", media.GetType(), tagmedia, entityName + ", link media-type", media, 1);


                InsertElementToDataBase(DOID, SystemID, "value", value.GetType(), tagValue, entityName + ", link", value, 1);

            }
        }

        protected void AddEditButton_Click(object sender, EventArgs e)
        {
            var taskid = "";
            var scheduleid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                scheduleid = GenerateSubjectID();
            }
            else
                scheduleid = RoleDropDownList.SelectedValue;

            Tasks = GetTasks(scheduleid);

            if (RoleDropDownList.SelectedIndex == 0)
            {
                taskid = GenerateTaskID();
            }
            else
                taskid = RoleDropDownList.SelectedValue;

            var root = string.Format("<schedule id={0}>", scheduleid);
            ProcessEntity(taskid, "task id", root);
            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
            AddTaskPanel.Visible = true;

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
            Response.Redirect(@"~/PagesSAR/Results/Results.aspx", false);
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            AddRolePanel.Visible = true;
            MainDiv.Visible = true;
            GridviewPanel.Visible = false;
        }

        protected void AddResponsibleRoleButton_Click(object sender, EventArgs e)
        {
            KeyLabel.Text = "Roles";
            MainDiv.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            AddRolePanel.Visible = true;
            AddTaskPanel.Visible = true;
            Roles = GetRoles();

            var roleIds = Roles.Select(x => x.RoleID);
            RespRoleDropDownList.DataSource = roleIds;
            RespRoleDropDownList.DataBind();
        }

        protected void AddRoleButton_Click1(object sender, EventArgs e)
        {
            KeyLabel.Text = "Roles";
            MainDiv.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            AddRolePanel.Visible = true;
            Roles = GetRoles();
            AddTaskPanel.Visible = true;
            var partyIds = Roles.Select(x => x.RoleID);
            RespRoleDropDownList.DataSource = partyIds;
            RespRoleDropDownList.DataBind();

        }

        protected void AddFunctionButton_Click(object sender, EventArgs e)
        {
            AddTaskPanel.Visible = true;
            MainDiv.Visible = true;

            AddRolePanel.Visible = true;
            var methodid = "";

            if (RoleDropDownList.SelectedIndex == 0)
            {
                methodid = GenerateSubjectID();
            }
            else
                methodid = RoleDropDownList.SelectedValue;

            Tasks = GetTasks(methodid);
            var stepIds = Tasks.Select(x => x.ID).ToList();
            var allIds = new List<string> { "" };
            allIds.AddRange(stepIds);
            AddTaskDropDownList.DataSource = allIds;
            AddTaskDropDownList.DataBind();
        }

        protected void AddAuxButton_Click(object sender, EventArgs e)
        {
            var methodid = "";
            var stepid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                methodid = GenerateSubjectID();
            }
            else
                methodid = RoleDropDownList.SelectedValue;

            Tasks = GetTasks(methodid);

            if (AddTaskDropDownList.SelectedIndex == 0)
            {
                stepid = GenerateTaskID();
            }
            else
                stepid = AddTaskDropDownList.SelectedValue;

            if (KeyLabel.Text == "Roles")
            {
                var roleid = RespRoleDropDownList.SelectedValue;

                var tagroleid = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><role id=\"{2}\">", methodid, stepid, roleid);
                InsertElementToDataBase(DOID, SystemID, "schedule task role id", roleid.GetType(), tagroleid, "schedule id, task id role id", roleid, 1);
            }
            if (KeyLabel.Text == "Parties")
            {
                var roleid = RespRoleDropDownList.SelectedValue;


                var tagroleid = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><party uuid=\"{2}\">", methodid, stepid, roleid);
                InsertElementToDataBase(DOID, SystemID, "schedule task party uuid", roleid.GetType(), tagroleid, "schedule id, task id party uuid", roleid, 1);
            }

            if (KeyLabel.Text == "Locations")
            {
                var roleid = RespRoleDropDownList.SelectedValue;


                var tagroleid = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><location uuid=\"{2}\">", methodid, stepid, roleid);
                InsertElementToDataBase(DOID, SystemID, "schedule task location uuid", roleid.GetType(), tagroleid, "schedule id, task id location uuid", roleid, 1);
            }

            if (KeyLabel.Text == "Activities")
            {
                var roleid = RespRoleDropDownList.SelectedValue;


                var tagroleid = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><activities uuid=\"{2}\">", methodid, stepid, roleid);
                InsertElementToDataBase(DOID, SystemID, "schedule task activities uuid", roleid.GetType(), tagroleid, "schedule id, task id activities uuid", roleid, 1);
            }


            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
            AddTaskPanel.Visible = true;
        }

        protected void SaveFunctionButton_Click(object sender, EventArgs e)
        {

        }


        protected void AddPartyButton_Click(object sender, EventArgs e)
        {
            KeyLabel.Text = "Parties";
            MainDiv.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            AddTaskPanel.Visible = true;
            AddRolePanel.Visible = true;
            Parties = GetFormviewParties();

            RespRoleDropDownList.DataSource =  Parties;
            RespRoleDropDownList.DataTextField = "PartyID";
            RespRoleDropDownList.DataValueField = "UUID";
            RespRoleDropDownList.DataBind();

        }

        protected void SaveTaskButton_Click(object sender, EventArgs e)
        {
            var scheduleid = "";
            var taskid = "";

          
            if (RoleDropDownList.SelectedIndex == 0)
            {
                scheduleid = GenerateSubjectID();
            }
            else
                scheduleid = RoleDropDownList.SelectedValue;

           Tasks = GetTasks(scheduleid);

            if (AddTaskDropDownList.SelectedIndex == 0)
            {
                taskid = GenerateTaskID();
            }
            else
                taskid = AddTaskDropDownList.SelectedValue;


            var tagroleid = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\">", scheduleid, taskid);

            var taguuid = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><uuid>", scheduleid, taskid);

            var preguid = GetData("task uuid", taguuid, UserName, DOID);
            var guid = preguid.Length == 0 ? Guid.NewGuid().ToString() : preguid;

            InsertElementToDataBase(DOID, SystemID, "task id", taskid.GetType(), tagroleid, "schedule id, task id", taskid, 1);
            InsertElementToDataBase(DOID, SystemID, "task uuid", guid.GetType(), taguuid, "schedule id, task id uuid", guid, 1);

            var title = TitleTextBox.Text;
            var tagTitle = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><title>", scheduleid, taskid);
            InsertElementToDataBase(DOID, SystemID, "title", title.GetType(), tagTitle, "schedule id, task id title", title, 1);


            var desc = DescTextArea.InnerHtml;
            var tagDesc = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><description>", scheduleid, taskid);
            InsertElementToDataBase(DOID, SystemID, "desc", desc.GetType(), tagDesc, "schedule id, task id description", desc, 1);

            var rem = TaskRemarksTextArea.InnerHtml;
            var tagRem = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><remarks>", scheduleid, taskid);
            InsertElementToDataBase(DOID, SystemID, "remarks", desc.GetType(), tagRem, "schedule id, task id remarks", rem, 1);

            var compare = CompareTextBox.Text;
            var tagCom = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><compare-to>", scheduleid, taskid);
            InsertElementToDataBase(DOID, SystemID, "compare-to", compare.GetType(), tagCom, "schedule id, task id compare-to", compare, 1);

            var start = StartTextBox.Text;
            var tagStart = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><start>", scheduleid, taskid);
            InsertElementToDataBase(DOID, SystemID, "start", start.GetType(), tagStart, "schedule id, task id start", start, 1);

            var end = EndTextBox.Text;
            var tagEnd = string.Format("<assessment-activities><schedule id=\"{0}\"><task id=\"{1}\"><end>", scheduleid, taskid);
            InsertElementToDataBase(DOID, SystemID, "end", end.GetType(), tagEnd, "schedule id, task id end", end, 1);


            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
            AddTaskPanel.Visible = false;
        }

  

        int HSize()
        {
            if (EndDateCalendar.Visible && StartDateCalendar.Visible)
                return 285;
            if (EndDateCalendar.Visible || StartDateCalendar.Visible)
                return 125;
            else
                return 0;
        }

        protected void StartDateCalendar_SelectionChanged(object sender, EventArgs e)
        {

            StartTextBox.Text = StartDateCalendar.SelectedDate.ToString("yyyy-MM-ddThh:mm:ssZ");
            StartDateCalendar.Visible = false;
            AddTaskPanel.Visible = true;
            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
            AddOn = HSize();
            AddTaskPanel.Height = Height + AddOn;
        }

        protected void Date1Button_Click(object sender, EventArgs e)
        {

            StartDateCalendar.Visible = true;
            AddTaskPanel.Visible = true;
            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
            AddOn = HSize();
            AddTaskPanel.Height = Height + AddOn;
        }

        protected void Date2Button_Click(object sender, EventArgs e)
        {

            EndDateCalendar.Visible = true;
            AddTaskPanel.Visible = true;
            MainDiv.Visible = true;
            AddRolePanel.Visible = true;
            AddOn = HSize();
            AddTaskPanel.Height = Height + AddOn;
        }
        protected void EndDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            EndTextBox.Text = EndDateCalendar.SelectedDate.ToString("yyyy-MM-ddThh:mm:ssZ");
            EndDateCalendar.Visible = false;
            AddRolePanel.Visible = true;
            AddTaskPanel.Visible = true;
            MainDiv.Visible = true;
            AddOn = HSize();
            AddTaskPanel.Height = Height + AddOn;
        }

        protected void TaskDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var methodid = "";
            if (RoleDropDownList.SelectedIndex == 0)
            {
                methodid = GenerateSubjectID();
            }
            else
                methodid = RoleDropDownList.SelectedValue;
            Tasks = GetTasks(methodid);
            PopulateTaskEditPage();
            MainDiv.Visible = true;

            AddRolePanel.Visible = true;
            AddTaskPanel.Visible = true;
        }

        protected void AddLoactionButton_Click(object sender, EventArgs e)
        {

        }

        protected void AddLocationButton_Click(object sender, EventArgs e)
        {
            KeyLabel.Text = "Locations";
            MainDiv.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            AddTaskPanel.Visible = true;
            AddRolePanel.Visible = true;
            Locations = GetLocations();

          
            RespRoleDropDownList.DataSource = Locations;
            RespRoleDropDownList.DataTextField = "LocationID";
            RespRoleDropDownList.DataValueField = "UUID";
            RespRoleDropDownList.DataBind();
        }

        protected void AddActivityButton_Click(object sender, EventArgs e)
        {
            KeyLabel.Text = "Activities";
            MainDiv.Visible = true;
            AddResponsibleRolePanel.Visible = true;
            AddTaskPanel.Visible = true;
            AddRolePanel.Visible = true;
            //Locations = GetLocations();


            //RespRoleDropDownList.DataSource = Locations;
            //RespRoleDropDownList.DataTextField = "LocationID";
            //RespRoleDropDownList.DataValueField = "UUID";
            RespRoleDropDownList.DataBind();
        }

        protected void AddTaskDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainDiv.Visible = true;
            
            AddTaskPanel.Visible = true;
            AddRolePanel.Visible = true;
            PopulateTaskEditPage();
        }

        protected void PropertyDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void AnnotationDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}