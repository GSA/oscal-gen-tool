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
using System.IO;

namespace OSCALGenerator.Pages.ControlImplementation
{
    public partial class Controls : BasePage
    {
        Dictionary<string, int> ControlIdToRank;
        List<string> ControlIDs;
        List<List<string>> StatementIDs;
        List<List<string>> ParameterIDs;
       
        private string SelectedBaseline;
        private string ControlId;
        
        string CurrentParameter;
        string CurrentStatement;
        Dictionary<string, User> UsersDict;
        protected new  void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ErrorCache();
                UserName = Cache["username"].ToString();
                DOID = (int)Cache["doid"];
                SystemID = int.Parse(Cache["SystemId"].ToString());

                ControlIdToRank = new Dictionary<string, int>();
                ControlIDs = new List<string>();
                StatementIDs = new List<List<string>>();
                ParameterIDs = new List<List<string>>();

                AddControlPanel.Visible = true;
                ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
                DAL = new CSharpDAL(ConnString);
                UsersDict = new Dictionary<string, User>();

                var level = getSensitivityLevel();
                RadioButtonList1.SelectedValue = level;
                var sensitivity = string.Format("Security Sensitivity Level: {0}", level);
                SensitivityLevelLabel.Text = sensitivity;

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


                var ids = GetIds();



                if (!IsPostBack)
                {
                    ControlDropDownList.DataSource = ControlIDs;

                    ParameterDropDownList.DataSource = ParameterIDs[0];
                    ParameterDropDownList.DataBind();

                    StatementDropDownList.DataSource = StatementIDs[0];
                    StatementDropDownList.DataBind();
                    if (StatementIDs.Count > 0)
                    {
                        StatementLabel.Text = StatementIDs[0][0];
                        StatementDropDownList.SelectedIndex = 1;
                    }

                    ControlDropDownList.DataBind();
                    Restore();
                }
                ControlDropDownList.DataBind();
            }catch(Exception ex)
            {
                StatusLabel.Text = ex.Message;
                StatusLabel.BackColor = Color.Red;
            }

        }

        void Restore()
        {
            UncheckAllBoxes();
            //var level = getSensitivityLevel();
            //RadioButtonList1.SelectedValue = level;
            //var sensitivity = string.Format("Security Sensitivity Level: {0}", level);
            //SensitivityLevelLabel.Text = sensitivity;
            ControlId = ControlDropDownList.SelectedValue;
            ControlId = ControlId.Replace(" ", "");
            var para = ParameterDropDownList.SelectedValue;
             var UpperControlId = ControlId.ToUpper();
              para = para.Replace("Parameter ", "");
            para = para.Replace(UpperControlId, "");
            var goodpara = string.Format("{0}_prm{1}", ControlId, para);
            goodpara = goodpara.Replace("(", "_");
            goodpara = goodpara.Replace(")", "");
            goodpara = goodpara.Replace(":", "");
            goodpara = goodpara.Replace(" ", "");
            var tagpara = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><set-param  param-id=\"{1}\"><value>", ControlId, goodpara);

            CurrentParameter = GetData("value", tagpara, UserName, DOID);
            ParameterTextArea.InnerText = CurrentParameter;

            var stat = StatementDropDownList.SelectedValue;
            stat = stat.Replace("Part ", "");
            var goodstat = string.Format("{0}_stmt.{1}", ControlId, stat);
            goodstat = goodstat.Replace(" ", "");

            var tagstat = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><statement  statement-id=\"{1}\">", ControlId, goodstat);
            CurrentStatement = GetData(goodstat, tagstat, UserName, DOID);
            StatementTextArea.InnerText = CurrentStatement;
           

            var tag = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><prop  name=\"implementation-status\">", ControlId);
            var impleStatus = GetDBDataGivenTag(tag, UserName, DOID);
            
            for (int i = 0; i < impleStatus.Count; i++)
            {
                foreach (var x in impleStatus[i])
                {
                    FillImplementationCheckBox(x);
                }
            }

            var oriTag = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><prop  name=\"control-origination\">", ControlId);
            var origination = GetDBDataGivenTag(oriTag, UserName, DOID);
            for (int i = 0; i < origination.Count; i++)
            {
                foreach (var x in origination[i])
                {
                    FillOriginationCheckBox(x);
                }
            }

            RestoreReponsibleTextBox();
        }

        public void RestoreReponsibleTextBox()
        {
            var res = ControlResponsibleRole(ControlId).Distinct().ToList();
            var test = "";

             for (int i = 0; i < res.Count() ; i++)
             {
                    test = test + res[i].Value + ", ";
             }
            var m = test.LastIndexOf(",");
            test = m>=0? test.Remove(m): test;
            ResponsibleRoleTextBox.Text = test;
        }

        

        void UncheckAllBoxes()
        {
            implemented.Checked =false;
            partiallyimplemented.Checked = false;
            alternativeimplementation.Checked = false;
            planned.Checked = false;
            notapplicable.Checked = false;
            shared.Checked = false;
            inherited.Checked = false;
            serviceprovidercorporate.Checked = false;
            serviceproviderhybrid.Checked = false;
            serviceprovidersystemspecific.Checked = false;
            providedbycustomer.Checked = false;
            configuredbycustomer.Checked = false;
        }
        void FillImplementationCheckBox(string item)
        {
            switch(item)
            {
                case "implemented":
                    {
                        implemented.Checked = true ;
                    }
                    break;
                case "partially-implemented":
                    {
                        partiallyimplemented.Checked = true;
                    }
                    break;
                case "alternative-implementation":
                    {
                        alternativeimplementation.Checked = true;
                    }
                    break;
                case "planned":
                    {
                        planned.Checked = true;
                    }
                    break;
                case "not-applicable":
                    {
                        notapplicable.Checked = true;
                    }
                    break;
                default:
                    return;
            }

        }

        void FillOriginationCheckBox(string item)
        {
            switch (item)
            {
                case "service-provider-system-specific":
                    {
                        serviceprovidersystemspecific.Checked = true;
                    }
                    break;
                case "service-provider-corporate":
                    {
                        serviceprovidercorporate.Checked = true;
                    }
                    break;
                case "service-provider-hybrid":
                    {
                        serviceproviderhybrid.Checked = true;
                    }
                    break;
                case "configured-by-customer":
                    {
                        configuredbycustomer.Checked = true;
                    }
                    break;
                case "provided-by-customer":
                    {
                        providedbycustomer.Checked = true;
                    }
                    break;
                case "shared":
                    {
                        shared.Checked = true;
                    }
                    break;
                case "inherited":
                    {
                        inherited.Checked = true;
                    }
                    break;
                default:
                    return;
            }

        }


        
        protected void CancelButton_Click(object sender, EventArgs e)
        {

        }

        protected void UpdateControlButton_Click(object sender, EventArgs e)
        {
            ControlId = ControlDropDownList.SelectedValue;
            ControlId = ControlId.Replace(" ", "");
            var tagImple = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><prop  name=\"implementation-status\">", ControlId);
            var tagOrigination = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><prop  name=\"control-origination\">", ControlId);
           
            var list = new List<string>();
            var listOri = new List<string>();

            if (implemented.Checked)
                list.Add("implemented");
            if (partiallyimplemented.Checked)
                list.Add("partially-implemented");

            if (alternativeimplementation.Checked)
                list.Add("alternative-implementation");
            if (planned.Checked)
                list.Add("planned");

            if (notapplicable.Checked)
                list.Add("not-applicable");

            if (serviceprovidercorporate.Checked)
                listOri.Add("service-provider-corporate");
            if (serviceprovidersystemspecific.Checked)
                listOri.Add("service-provider-system-specific");
            if (serviceproviderhybrid.Checked)
                listOri.Add("service-provider-hybrid");
            if (configuredbycustomer.Checked)
                listOri.Add("configured-by-customer");
            if (providedbycustomer.Checked)
                listOri.Add("provided-by-customer");
            if (shared.Checked)
                listOri.Add("shared");
            if (inherited.Checked)
                listOri.Add("inherited");


            InsertElementToDataBase(DOID, SystemID, "implementation-status", ControlId.GetType(), tagImple, "implemented-requirement control-id, prop name",list, 1);
            InsertElementToDataBase(DOID, SystemID, "control-origination", ControlId.GetType(), tagOrigination, "implemented-requirement control-id, prop name", listOri, 1);

        }

        string getSensitivityLevel()
        {
            var level = GetData("security-sensitivity-level", "<system-characteristics><security-sensitivity-level>", UserName, DOID);
            var res = "Moderate";
            switch(level)
            {
                case "low":
                    res = "Low";
                    break;
                case "moderate":
                    res = "Moderate";
                    break;
                case "high":
                    res = "High";
                    break;
              
            }

            return res;
        }

        public List<ControlParamStatement>GetIds()
        {
            var result = new List<ControlParamStatement>();


            string appPath = Request.PhysicalApplicationPath;
            var file = "";


            if (RadioButtonList1.SelectedItem != null)
                SelectedBaseline = RadioButtonList1.SelectedItem.Value;

            else
                RadioButtonList1.SelectedValue = getSensitivityLevel();

            switch (SelectedBaseline)
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
               
               
                while(!sr.EndOfStream)
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
                    line = line.Remove(0, index+3);
                    int t = line.IndexOf('@');
                    var par = line.Substring(0, t);
                    var end = t>0 ? line.Replace(par, ""): "";

                    end = end.Length>0? end.Replace("@ #", ""): "";
                    end = end.Length > 0 ? end.Replace("@", "") : "";


                    while (par.IndexOf('*')>0)
                    {
                        var into = par.IndexOf('*');
                        var para = par.Substring(0, into);
                        par = par.Remove(0, into+1);
                       
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
                        end = end.Remove(0, intot+1);
                        stat = stat.Replace("?", "");
                        statements.Add(stat);
                    }
                    if (end.Length > 0)
                    {
                        var xx = end.Replace("#", "");
                         xx = xx.Replace("?", "");  ///new line
                        statements.Add(xx);
                    }
                    
                    tepo.StatementIDs = statements;
                    StatementIDs.Add(statements);

                    result.Add(tepo);
                }

            }



                return result;
        }

   

        protected void ControlDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = ControlDropDownList.SelectedIndex;
            ParameterDropDownList.DataSource = ParameterIDs[index];
            ParameterDropDownList.DataBind();

           
            StatementDropDownList.DataSource = StatementIDs[index];
            if (StatementIDs[index].Count > 1)
            {
                StatementDropDownList.DataBind();
                StatementDropDownList.SelectedIndex = 1;
                StatementLabel.Text = StatementIDs[index][0];
                var selected = StatementDropDownList.SelectedValue;

                SaveStatementButton.Text = string.Format("{0} {1}", "Save", selected);
            }
            if(StatementIDs[index].Count ==1)
            {
                StatementDropDownList.DataBind();
                StatementDropDownList.SelectedIndex = 0;
                StatementLabel.Text = "";
            }
            if (StatementIDs[index].Count > 0)
                StatementDropDownList.DataBind();
            else
            {
             
                StatementDropDownList.DataBind();
                StatementLabel.Text = "";
            }

            Restore();

        }


        protected void StatementDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = StatementDropDownList.SelectedValue;

            SaveStatementButton.Text = string.Format("{0} {1}", "Save", selected);
            Restore();
        }

        protected void ParameterDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            var selected = ParameterDropDownList.SelectedValue;
            SaveParameterButton.Text = string.Format("{0} {1}", "Save", selected);
            Restore();
        }

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ControlDropDownList.DataSource = ControlIDs;

            ParameterDropDownList.DataSource = ParameterIDs[0];
            ParameterDropDownList.DataBind();

            StatementDropDownList.DataSource = StatementIDs[0];
            StatementDropDownList.DataBind();

            StatementLabel.Text = StatementIDs[0][0];
            StatementDropDownList.SelectedIndex = 1;
           

            ControlDropDownList.DataBind();
        }

        protected void SaveParameterButton_Click(object sender, EventArgs e)
        {
            ControlId = ControlDropDownList.SelectedValue;
            ControlId = ControlId.Replace(" ", "");
            var para = ParameterDropDownList.SelectedValue;
            var UpperControlId = ControlId.ToUpper();
            para = para.Replace("Parameter ", "");
            para = para.Replace(UpperControlId, "");
            var goodpara = string.Format("{0}_prm{1}", ControlId, para);
            goodpara = goodpara.Replace("(", "_");
            goodpara = goodpara.Replace(")", "");
            goodpara = goodpara.Replace(":", "");
            goodpara = goodpara.Replace(" ", "");
            var tagpara = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><set-param  param-id=\"{1}\"><value>", ControlId, goodpara);

            var text = ParameterTextArea.InnerText;
             
            InsertElementToDataBase(DOID, SystemID, "value", text.GetType(), tagpara, "implemented-requirement control-id, value", text, 1);


        }

        protected void SaveStatementButton_Click(object sender, EventArgs e)
        {
            ControlId = ControlDropDownList.SelectedValue;
            ControlId = ControlId.Replace(" ", "");
            var stat = StatementDropDownList.SelectedValue;
            stat = stat.Replace("Part ", "");
            var goodstat = string.Format("{0}_stmt.{1}", ControlId, stat);
            goodstat = goodstat.Replace(" ", "");

            var tagstat = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><statement  statement-id=\"{1}\">", ControlId, goodstat);
          
            CurrentStatement = StatementTextArea.InnerText;
            InsertElementToDataBase(DOID, SystemID, goodstat, CurrentStatement.GetType(), tagstat, "implemented-requirement control-id, statement statement-id", CurrentStatement, 1);


        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PageSSP/SystemImplementation/Users.aspx");
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/BackMatter/Citations.aspx");
        }

        protected void RespRoleButton_Click(object sender, EventArgs e)
        {
            this.RoleDropDownList.Visible = true;
            var users = GetUsers();
            var titles = new List<string>();

            titles.Add("Select A Role");
            for (int i=0; i<users.Count(); i++)
            {
                if (!UsersDict.ContainsKey(users[i].Title))
                {
                    titles.Add(users[i].Title);

                    UsersDict.Add(users[i].Title, users[i]);
                }

            }
             RoleDropDownList.DataSource = titles;
             RoleDropDownList.DataBind();
        }

        protected void RoleDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ControlId = ControlDropDownList.SelectedValue;
            ControlId = ControlId.Replace(" ", "");
            var done = RoleDropDownList.SelectedValue;
            var users = GetUsers();
            for (int i = 0; i < users.Count(); i++)
            {    
                if(!UsersDict.ContainsKey(users[i].Title))
                   UsersDict.Add(users[i].Title, users[i]);
            }
            var user = UsersDict[done];
            var tag = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\"><responsible-role  role-id=\"{1}\">", ControlId, user.ID);

            InsertElementToDataBase(DOID, SystemID, user.ID, user.ID.GetType(), tag, "implemented-requirement control-id, responsible-role role-id", user.Title, 1);
            this.RoleDropDownList.Visible = false;
            RestoreReponsibleTextBox();
        }
    }

   
}