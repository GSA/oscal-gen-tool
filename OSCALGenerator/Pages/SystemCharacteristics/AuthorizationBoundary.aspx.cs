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
    public partial class AuthorizationBoundary : BasePage
    {
        string AuthDesc;
        string NetDesc;
        string DataDesc;

        string AuthDiagramID;
        string NetDiagramID;
        string DataDiagramID;


        string AuthLink;
        string NetLink;
        string DataLink;

        protected new void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ErrorCache();

                UserName = Cache["username"].ToString();

                SystemID = int.Parse(Cache["SystemId"].ToString());

                ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
                DAL = new CSharpDAL(ConnString);

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
                    DOID = (int)Cache["doid"];
                    var doid = int.Parse(Cache["doid"].ToString());
                    DocName = GetDocFullName(doid);
                    DocNameLabel.Text = DocName;

                }


                if (!IsPostBack)
                {

                    Restore();
                }
            }
            catch (Exception ex)
            {
                StatusLabel.Text = ex.Message;
                StatusLabel.BackColor = Color.Red;
            }
        }



        void Restore()
        {
            AuthDesc = GetData("description", "<system-characteristics><authorization-boundary><description>", UserName, DOID);
            AuthDescTextArea.InnerText = AuthDesc;

            AuthLink = GetData("href", "<system-characteristics><authorization-boundary><href>", UserName, DOID);
            AuthLinkTextArea.InnerText = AuthLink;  

            NetDesc = GetData("description", "<system-characteristics><network-architecture><description>", UserName, DOID);
            NetDescTextArea.InnerText = NetDesc;

            NetLink = GetData("href", "<system-characteristics><network-architecture><href>", UserName, DOID);
            NetLinkTextArea.InnerText = NetLink;


            DataDesc = GetData("description", "<system-characteristics><data-flow><description>", UserName, DOID);
            DataDescTextArea.InnerText = DataDesc;

            DataLink = GetData("href", "<system-characteristics><data-flow><href>", UserName, DOID);
            DataLinkTextArea.InnerText = DataLink;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            var tagAutDes = "<system-characteristics><authorization-boundary><description>";
            AuthDesc = AuthDescTextArea.InnerText;
            InsertElementToDataBase(DOID, SystemID, "description", tagAutDes.GetType(), tagAutDes, "description", AuthDesc, 1);

            var tagAutHref = "<system-characteristics><authorization-boundary><href>";
            var autHref = AuthLinkTextArea.InnerHtml;
            InsertElementToDataBase(DOID, SystemID, "href", tagAutHref.GetType(), tagAutHref, "href", autHref, 1);

            AuthDiagramID = Guid.NewGuid().ToString();// AuthoDiagDropDownList.SelectedValue;
            var tagAuthDiag = string.Format("<system-characteristics><authorization-boundary><diagram id>");

            InsertElementToDataBase(DOID, SystemID, "diagram id", AuthDiagramID.GetType(), tagAuthDiag, "diagram id, diagram id", AuthDiagramID, 1);


            var tagNetDes = "<system-characteristics><network-architecture><description>";

            NetDesc = NetDescTextArea.InnerText;
            InsertElementToDataBase(DOID, SystemID, "description", tagNetDes.GetType(), tagNetDes, "description", NetDesc, 1);


            NetDiagramID = Guid.NewGuid().ToString();
            var tagNetDiag = string.Format("<system-characteristics><network-architecture><diagram id>", NetDiagramID);

            InsertElementToDataBase(DOID, SystemID, "diagram id", NetDiagramID.GetType(), tagNetDiag, "diagram id, diagram id", NetDiagramID, 1);

            NetLink = NetLinkTextArea.InnerText;
            var tagNetLink = string.Format("<system-characteristics><network-architecture><href>");
            InsertElementToDataBase(DOID, SystemID, "href", NetLink.GetType(), tagNetLink, "diagram id, link href", NetLink, 1);


            var tagDataDes = "<system-characteristics><data-flow><description>";

            DataDesc = DataDescTextArea.InnerText;
            InsertElementToDataBase(DOID, SystemID, "description", tagDataDes.GetType(), tagDataDes, "description", DataDesc, 1);


            DataDiagramID = Guid.NewGuid().ToString();
            var tagDataDiag = string.Format("<system-characteristics><data-flow><diagram id>");

            InsertElementToDataBase(DOID, SystemID, "diagram id", DataDiagramID.GetType(), tagDataDiag, "diagram id, diagram id", DataDiagramID, 1);

            DataLink = DataLinkTextArea.InnerText;

            var tagDataLink = string.Format("<system-characteristics><data-flow><href>");
            InsertElementToDataBase(DOID, SystemID, "href", DataLink.GetType(), tagDataLink, "diagram id, link href", DataLink, 1);



        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/SystemCharacteristics/SecurityImpactLevel.aspx");
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PageSSP/SystemImplementation/Users.aspx");
        }

        protected void AuthoDiaButton_Click(object sender, EventArgs e)
        {


        }
    }
}