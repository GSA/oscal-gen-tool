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

namespace OSCALGenerator.Pages.Metadata
{
    public partial class Parties : BasePage
    {
        List<DocParty> Fullparties;

        protected new void Page_Load(object sender, EventArgs e)
        {
            UserName = Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = (int)Cache["SystemId"];

            AddPartyPanel.Visible = false;
            
            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);
            var part = GetParties();

             Fullparties = FillCompanyInfo(part);
            this.FormView1.DataSource = Fullparties; 
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
        }

        
       private void PopulateEditPage()
       {
            var partyId = PartyIdDropDownList.SelectedValue;
            DocParty party =new DocParty();
            foreach(var elt in Fullparties)
            {
                if (elt.PartyID == partyId)
                {
                    party = elt;
                    break;
                }

            }
            OrgNameTextBox.Text = party.OrgName;
            ShortNameTextBox.Text = party.ShortName;
            OrgIDTextBox.Text = party.OrgID;
            PersonNameTextBox.Text = party.PersonName;
            AddressLine1TextBox.Text = party.AddressLine1;
            AddressLine2TextBox.Text = party.AddressLine2;
            CityTextBox.Text = party.City;
            StateTextBox.Text = party.State;
            PostalCodeTextBox.Text = party.PostalCode;
            CountryTextBox.Text = party.Country;
            PhoneTextBox.Text = party.Phone;
            EmailTextBox.Text = party.Email;
        }
       

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            string entity;
            if (DropDownList1.SelectedValue == "Person")
            {
                entity = "person";
               
            }
            else
            {
                entity = "org";
              
            }

            string partyid;
            if (PartyIDTextBox.Text.Length ==0)
             partyid = PartyIdDropDownList.SelectedValue;
            else
              partyid = this.PartyIDTextBox.Text;

            var tagpartyid = string.Format("<metadata><party  id=\"{0}\">", partyid);

            if (partyid.Length == 0)
                return;

            InsertElementToDataBase(DOID, SystemID, partyid, partyid.GetType(), tagpartyid, "party id, party id", partyid , 1);


            var orgName = OrgNameTextBox.Text;
            var tagOrgName = string.Format("<metadata><party  id=\"{0}\"><org><org-name>", partyid);
            InsertElementToDataBase(DOID, SystemID, "org-name", partyid.GetType(), tagOrgName, "party id, org-name", orgName, 1);

            var shortName = ShortNameTextBox.Text;
            var tagShortName = string.Format("<metadata><party  id=\"{0}\"><org><short-name>", partyid);
            InsertElementToDataBase(DOID, SystemID, "short-name", partyid.GetType(), tagShortName, "party id, short-name", shortName, 1);

            string OrgID;
            if (orgName.Length == 0)
                OrgID = OrgIDTextBox.Text;
            else
            {
                OrgID = partyid;
                OrgIDTextBox.Text = partyid;
            }
            var tagOrgID = string.Format("<metadata><party  id=\"{0}\"><org><org-id>", partyid);
            InsertElementToDataBase(DOID, SystemID, "org-id", partyid.GetType(), tagOrgID, "party id, org-id", OrgID, 1);


            var personName = PersonNameTextBox.Text;
            var tagPersonName = string.Format("<metadata><party  id=\"{0}\"><person><person-name>", partyid);
            InsertElementToDataBase(DOID, SystemID, "person-name", partyid.GetType(), tagPersonName, "party id, person-name", personName, 1);

            var addressLine1 = AddressLine1TextBox.Text;
            var tagAddressLine1 = string.Format("<metadata><party  id=\"{0}\"><{1}><address><addr-line>", partyid, entity);
            InsertElementToDataBase(DOID, SystemID, "addr-line", partyid.GetType(), tagAddressLine1, "party id, addr-line", addressLine1, 1);

            var addressLine2 = AddressLine2TextBox.Text;
            var tagAddressLine2 = string.Format("<metadata><party  id=\"{0}\"><{1}><address><addr-line>", partyid, entity);
            InsertElementToDataBase(DOID, SystemID, "addr-line2", partyid.GetType(), tagAddressLine2, "party id, addr-line", addressLine2, 1);


            var city = CityTextBox.Text;
            var tagCity = string.Format("<metadata><party  id=\"{0}\"><{1}><address><city>", partyid, entity);
            InsertElementToDataBase(DOID, SystemID, "city", partyid.GetType(), tagCity, "party id, city", city, 1);

            var state = StateTextBox.Text;
            var tagState = string.Format("<metadata><party  id=\"{0}\"><{1}><address><state>", partyid, entity);
            InsertElementToDataBase(DOID, SystemID, "state", partyid.GetType(), tagState, "party id, state", state, 1);

            var postalCode = PostalCodeTextBox.Text;
            var tagPostalCode = string.Format("<metadata><party  id=\"{0}\"><{1}><address><postal-code>", partyid, entity);
            InsertElementToDataBase(DOID, SystemID, "postal-code", partyid.GetType(), tagPostalCode, "party id, postal-code", postalCode, 1);

            var country = CountryTextBox.Text;
            var tagCountry = string.Format("<metadata><party  id=\"{0}\"><{1}><address><country>", partyid, entity);
            InsertElementToDataBase(DOID, SystemID, "country", partyid.GetType(), tagCountry, "party id, country", country, 1);


            var phone = PhoneTextBox.Text;
            var tagPhone = string.Format("<metadata><party  id=\"{0}\"><{1}><address><phone>", partyid, entity);
            InsertElementToDataBase(DOID, SystemID, "phone", partyid.GetType(), tagPhone, "party id, phone", phone, 1);



            var email = EmailTextBox.Text;
            var tagEmail = string.Format("<metadata><party  id=\"{0}\"><{1}><address><email>", partyid, entity);
            InsertElementToDataBase(DOID, SystemID, "email", partyid.GetType(), tagEmail, "party id, email", email, 1);



            FormView1.DataBind();
            FormviewPanel.Visible = true;

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            AddPartyPanel.Visible = false;
            FormviewPanel.Visible = true;
        }

        protected void AddPartyButton_Click(object sender, EventArgs e)
        {
            AddPartyPanel.Visible = true;
            PartyIdDropDownList.Visible = false;
            PartyIdDropDownList.Width = 0;
            PartyIDTextBox.Width = 350;
            PartyIDTextBox.Visible = true;

            DeleteButton.Visible = false;
            FormviewPanel.Visible = false;
        }

        protected void FormView1_PageIndexChanging(object sender, FormViewPageEventArgs e)
        {
            FormView1.PageIndex = e.NewPageIndex;
            FormView1.DataBind();
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/Metadata/Roles.aspx");
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/Pages/Metadata/ResponsibleParties.aspx");
        }

        protected void FormView1_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            var k0 = e.Keys[0];
            var old0 = e.OldValues[k0];

            FormView1.DataBind();
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            AddPartyPanel.Visible = true;
            PartyIDTextBox.Visible = false;
            PartyIDTextBox.Width = 0;
            PartyIdDropDownList.Width = 350;
            PartyIdDropDownList.Visible = true;
            DeleteButton.Visible = true;

            var PartyIds = GetPartyIds();
            PartyIdDropDownList.DataSource = PartyIds;
            PartyIdDropDownList.DataBind();

            PopulateEditPage();
            
            FormviewPanel.Visible = false;
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            var partyId = PartyIdDropDownList.SelectedValue;
      
          
            var tagpartyid = string.Format("<metadata><party  id=\"{0}\">", partyId);

            if (partyId.Length == 0)
                return;

            InsertElementToDataBase(DOID, SystemID, partyId, partyId.GetType(), tagpartyid, "party id, party id", partyId, 0);

            FormView1.DataBind();
            FormviewPanel.Visible = true;
        }

        protected void PartyIdDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateEditPage();

            AddPartyPanel.Visible = true;
            PartyIDTextBox.Visible = false;
            PartyIDTextBox.Width = 0;
            PartyIdDropDownList.Width = 350;
            PartyIdDropDownList.Visible = true;
            DeleteButton.Visible = true;
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList1.SelectedValue == "Person")
            {
                OrgNameTextBox.Text = "";
                ShortNameTextBox.Text = "";
                OrgNameTextBox.Enabled = false;
                ShortNameTextBox.Enabled = false;
                PersonNameTextBox.Enabled = true;
            }
            else
            {

                OrgNameTextBox.Enabled = true;
                ShortNameTextBox.Enabled = true;
                PersonNameTextBox.Text = "";
                PersonNameTextBox.Enabled = false;
            }

            AddPartyPanel.Visible = true;
            PartyIDTextBox.Visible = false;
            PartyIDTextBox.Width = 0;
            PartyIdDropDownList.Width = 350;
            PartyIdDropDownList.Visible = true;
            DeleteButton.Visible = true;
        }
    }
}