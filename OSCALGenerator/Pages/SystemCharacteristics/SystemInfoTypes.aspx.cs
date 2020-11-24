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

namespace OSCALGenerator.Pages.SystemCharacteristics
{
    public partial class SystemInfoTypes : BasePage
    {
        List<DocInfoType> DocInfoTypes; 
        protected new void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ErrorCache();

                UserName = Cache["username"].ToString();
                DOID = (int)Cache["doid"];
                SystemID = int.Parse(Cache["SystemId"].ToString());

                this.FormviewPanel.Visible = true;
                this.Panel1.Visible = false;
                ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
                DAL = new CSharpDAL(ConnString);

                DocInfoTypes = GetSystemInfoType();


                this.FormView1.DataSource = DocInfoTypes;

                FormView1.DataBind();


                if (Cache["orgName"] != null)
                {
                    OrgName = Cache["orgName"].ToString();
                    CorpNameLabel.Text = OrgName;
                }
                if (Cache["systemName"] != null)
                {
                    SystemName = Cache["systemName"].ToString();
                    SystemNameLabel.Text = SystemName;
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


        protected void SaveButton_Click(object sender, EventArgs e)
        {
            var sysName = "nist";
            var infoName = InfoTypeNameTextBox.Text;
            var name = infoName.Replace(" ", "");
            name = name.Replace(":", "");
            name = name.Replace("?", "");
            var infoId = name +"-"+ DocInfoTypes.Count;
            InfoTypeIDValueLabel.Text = name;

            var tagInfoTypeName = string.Format("<system-characteristics><system-information><information-type  name=\"{0}\" id=\"{1}\">", infoName, infoId);
            InsertElementToDataBase(DOID, SystemID, infoName, infoName.GetType(), tagInfoTypeName, "information-type name, information-type name", infoName, 1);
            InsertElementToDataBase(DOID, SystemID, infoId, infoId.GetType(), tagInfoTypeName, "information-type name, information-type id", infoId, 1);

            var infoSystId = SysTypeIDTextBox.Text;
            var tagInfoSysId = string.Format("<system-characteristics><system-information><information-type  name=\"{0}\" id=\"{1}\"><information-type-id  system=\"nist\">", infoName, infoId);
            InsertElementToDataBase(DOID, SystemID, sysName, sysName.GetType(), tagInfoSysId, "information-type name, information-type-id system", infoSystId, 1);

            var desc = DescTextarea.InnerHtml;
            var tagDesc = string.Format("<system-characteristics><system-information><information-type  name=\"{0}\" id=\"{1}\"><desc>", infoName, infoId);
            InsertElementToDataBase(DOID, SystemID, "desc", sysName.GetType(), tagDesc, "information-type name, information-type-id desc", desc, 1);

            var confImpactBase = ConfImpactBaseDropDownList.SelectedValue;
            var tagConfImpactBase = string.Format("<system-characteristics><system-information><information-type  name=\"{0}\" id=\"{1}\"><confidentiality-impact><base>", infoName, infoId);
            InsertElementToDataBase(DOID, SystemID, "base", sysName.GetType(), tagConfImpactBase, "information-type name, base", confImpactBase, 1);

            var confImpactSelected = ConfImpactSelectedDropDownList.SelectedValue;
            var tagConfImpactSelected = string.Format("<system-characteristics><system-information><information-type  name=\"{0}\" id=\"{1}\"><confidentiality-impact><selected>", infoName, infoId);
            InsertElementToDataBase(DOID, SystemID, "selected", sysName.GetType(), tagConfImpactSelected, "information-type name, selected", confImpactSelected, 1);

            var integrityImpactBase = IntegrityImpactBaseDropDownList.SelectedValue;
            var tagIntegrityImpactBase = string.Format("<system-characteristics><system-information><information-type  name=\"{0}\" id=\"{1}\"><integrity-impact><base>", infoName, infoId);
            InsertElementToDataBase(DOID, SystemID, "base", sysName.GetType(), tagIntegrityImpactBase, "information-type name, base", integrityImpactBase, 1);

            var integrityImpactSelected = IntegrityImpactSelectedDropDownList.SelectedValue;
            var tagIntegrityImpactSelected = string.Format("<system-characteristics><system-information><information-type  name=\"{0}\" id=\"{1}\"><integrity-impact><selected>", infoName, infoId);
            InsertElementToDataBase(DOID, SystemID, "selected", sysName.GetType(), tagIntegrityImpactSelected, "information-type name, selected", integrityImpactSelected, 1);

            var availabilityImpactBase = AvailabilityImpactBaseDropDownList.SelectedValue;
            var tagAvailabilityImpactBase = string.Format("<system-characteristics><system-information><information-type  name=\"{0}\" id=\"{1}\"><availability-impact><base>", infoName, infoId);
            InsertElementToDataBase(DOID, SystemID, "base", sysName.GetType(), tagAvailabilityImpactBase, "information-type name, base", availabilityImpactBase, 1);

            var availabilityImpactSelected = IntegrityImpactSelectedDropDownList.SelectedValue;
            var tagAvailabilityImpactSelected = string.Format("<system-characteristics><system-information><information-type  name=\"{0}\" id=\"{1}\"><availability-impact><selected>", infoName, infoId);
            InsertElementToDataBase(DOID, SystemID, "selected", sysName.GetType(), tagAvailabilityImpactSelected, "information-type name, selected", availabilityImpactSelected, 1);

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {

        }

        protected void AddSysInfoTypeButton_Click(object sender, EventArgs e)
        {
            this.Panel1.Visible = true;
            this.FormviewPanel.Visible = false;
        }

        protected void FormView1_PageIndexChanging(object sender, FormViewPageEventArgs e)
        {
            FormView1.PageIndex = e.NewPageIndex;
            FormView1.DataBind();
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/SystemCharacteristics/SecurityImpactLevel.aspx");
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/SystemCharacteristics/SystemIdentification.aspx");
        }
    }
}