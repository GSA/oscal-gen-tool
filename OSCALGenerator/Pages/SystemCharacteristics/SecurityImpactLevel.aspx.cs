using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlDataProvider;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Data.Sql;
using System.Data.SqlClient;

namespace OSCALGenerator.Pages.SystemCharacteristics
{
    public partial class SecurityImpactLevel : BasePage
    {
        string Confidentiality;
        string Availability;
        string Integrity;
        string SecurityAuthIal;
        string SecurityAuthAal;
        string SecurityAuthFal;
        string SecurityEauthLevel;
        string State;
        string DeploymentModelType;
        string ServiceModelType;
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
                if (!IsPostBack)
                {
                    Restore();
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
            }catch(Exception ex)
            {
                Response.Redirect(@"~/Pages/Home.aspx");
                throw ex;
            }

        }
       
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            Confidentiality = ConfidentialityDropDownList.SelectedValue;
            InsertElementToDataBase(DOID, SystemID, "security-objective-confidentiality", Confidentiality.GetType(), "<system-characteristics><security-impact-level><security-objective-confidentiality>", "security-objective-confidentiality",Confidentiality, 1);

            Integrity = IntegrityDropDownList.SelectedValue;
            InsertElementToDataBase(DOID, SystemID, "security-objective-integrity", Integrity.GetType(), "<system-characteristics><security-impact-level><security-objective-integrity>", "security-objective-integrity", Integrity, 1);

            Availability = AvailabilityDropDownList.SelectedValue;
            InsertElementToDataBase(DOID, SystemID, "security-objective-availability", Availability.GetType(), "<system-characteristics><security-impact-level><security-objective-availability>", "security-objective-availability", Availability, 1);

            SecurityAuthIal=SecurityAuthIalDropDownList.SelectedValue;
            InsertElementToDataBase(DOID, SystemID, "security-auth-ial", SecurityAuthIal.GetType(), "<system-characteristics><security-eauth><security-auth-ial>", "security-auth-ial", SecurityAuthIal, 1);

            SecurityAuthAal = SecurityAuthAalDropDownList.SelectedValue;
            InsertElementToDataBase(DOID, SystemID, "security-auth-aal", SecurityAuthAal.GetType(), "<system-characteristics><security-eauth><security-auth-aal>", "security-auth-aal", SecurityAuthAal, 1);
             

            SecurityAuthFal=SecurityAuthFalDropDownList.SelectedValue;
            InsertElementToDataBase(DOID, SystemID, "security-auth-fal", SecurityAuthFal.GetType(), "<system-characteristics><security-eauth><security-auth-fal>", "security-auth-fal", SecurityAuthFal, 1);


            SecurityEauthLevel=SecurityEauthLevelDropDownList.SelectedValue;
            InsertElementToDataBase(DOID, SystemID, "security-eauth-level", SecurityEauthLevel.GetType(), "<system-characteristics><security-eauth><security-eauth-level>", "security-eauth-level", SecurityEauthLevel, 1);

            State = StateDropDownList.SelectedValue;
            var tagState = string.Format("<system-characteristics><status state>");
            InsertElementToDataBase(DOID, SystemID, "status state", SecurityEauthLevel.GetType(), tagState, "status state, status state", State, 1);

            var rem = RemarkTextArea.InnerHtml;
            var tagStateRemark = string.Format("<system-characteristics><status state><remarks>");
            InsertElementToDataBase(DOID, SystemID, "status state remarks", SecurityEauthLevel.GetType(), tagStateRemark, "status state, remarks", rem, 1);

            DeploymentModelType = DeploymentModelTypeTextBox.Text;
            var tagDeploy = string.Format("<system-characteristics><deployment-model type=\"{0}\">", DeploymentModelType);
            InsertElementToDataBase(DOID, SystemID, "deployment-model type", DeploymentModelType.GetType(), tagDeploy, "deployment-model type, deployment-model type", DeploymentModelType, 1);

            ServiceModelType = ServiceModelTypeTextBox.Text;
            var tagService = string.Format("<system-characteristics><service-model type=\"{0}\">", ServiceModelType);
            InsertElementToDataBase(DOID, SystemID, "service-model type", ServiceModelType.GetType(), tagService, "service-model type, service-model type", ServiceModelType, 1);


        }

        void Restore()
        {
            Confidentiality = GetData("security-objective-confidentiality", "<system-characteristics><security-impact-level><security-objective-confidentiality>", UserName, DOID);
            ConfidentialityDropDownList.SelectedValue = Confidentiality;

            Integrity = GetData("security-objective-integrity", "<system-characteristics><security-impact-level><security-objective-integrity>", UserName, DOID);
            IntegrityDropDownList.SelectedValue = Integrity;

            Availability = GetData("security-objective-availability", "<system-characteristics><security-impact-level><security-objective-availability>", UserName, DOID);
            AvailabilityDropDownList.SelectedValue = Availability;

            SecurityAuthIal = GetData("security-auth-ial", "<system-characteristics><security-eauth><security-auth-ial>", UserName, DOID);
            SecurityAuthIalDropDownList.SelectedValue = SecurityAuthIal;

            SecurityAuthAal = GetData("security-auth-aal", "<system-characteristics><security-eauth><security-auth-aal>", UserName, DOID);
            SecurityAuthAalDropDownList.SelectedValue = SecurityAuthAal;

            SecurityAuthFal = GetData("security-auth-fal", "<system-characteristics><security-eauth><security-auth-fal>", UserName, DOID);
            SecurityAuthFalDropDownList.SelectedValue = SecurityAuthFal;

            SecurityEauthLevel = GetData("security-eauth-level", "<system-characteristics><security-eauth><security-eauth-level>", UserName, DOID);
            SecurityEauthLevelDropDownList.SelectedValue = SecurityEauthLevel;



            State = GetData("status state", "<system-characteristics><status state>", UserName, DOID);
            StateDropDownList.SelectedValue = State;
            var rem = GetData("status state remarks", "<system-characteristics><status state><remarks>", UserName, DOID);
            RemarkTextArea.InnerHtml = rem;

            DeploymentModelType = GetData("deployment-model type", UserName, DOID);
            DeploymentModelTypeTextBox.Text = DeploymentModelType;

            
            ServiceModelType = GetData("service-model type", UserName, DOID);
            ServiceModelTypeTextBox.Text = ServiceModelType;

        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/SystemCharacteristics/SystemInfoTypes.aspx");
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/SystemCharacteristics/AuthorizationBoundary.aspx");
        }
    }
}