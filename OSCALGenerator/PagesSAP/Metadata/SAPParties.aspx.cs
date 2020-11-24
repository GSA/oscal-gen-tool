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

namespace OSCALGenerator.PagesSAP.Metadata
{
    public partial class SAPParties : BasePage
    {
        
      
        List<Prop> Props;
        List<Link> Links;
        List<Annotation> Annotations;
        List<DocAddress> Addresses;
        List<Phone> Phones;
        List<string> Emails;
        List<string> LocationUUIDs;
        List<string> MemberOfOrg;
      
        string PartyID;

        protected new void Page_Load(object sender, EventArgs e)
        {
            UserName = Cache["username"].ToString();
            DOID = (int)Cache["doid"];
            SystemID = (int)Cache["SystemId"];

         
            MainPartyPanel.Visible = false;
            AddPropPanel.Visible = false;
            AddAddressPanel.Visible = false;
            AddAuxPanel.Visible = false;

            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);


            var part = GetFormviewParties();
            Fullparties = part;
            AllPartyInfo = part;
            this.FormView1.DataSource = AllPartyInfo;
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

      
        private void PopulateEditMainPartyPage()
        {
            var partyId = PartyDropDownList.SelectedValue;
            DocParty party = new DocParty();
            foreach (var elt in Fullparties)
            {
                if (elt.PartyID == partyId)
                {
                    party = elt;
                    break;
                }

            }
            PartyNameTextBox.Text = party.Name;
            PartyShortNameTextBox.Text = party.ShortName;
            ExternalIDTextBox.Text = party.ExternalID;
            ExternalTypeTextBox.Text = party.ExternalType;
            PartyTypeDropDownList.SelectedValue = party.PartyType;
            Props = GetProps(partyId, "party id");
            Annotations = GetAnnotations(partyId, "party id");
            Links = GetLinks(partyId, "party id");
            LocationUUIDs = GetAuxillaries(partyId, "party id", "location uuid");
            MemberOfOrg = GetAuxillaries(partyId, "party id", "member-of-organization");
            Emails = GetAuxillaries(partyId, "party id", "email");
            Phones = GetPhones(partyId, "party id");
            Addresses = GetAddresses(partyId);

            AddressDropDownList.DataSource = Addresses.Select(x => x.AddressLine1).ToList();
            AddressDropDownList.DataBind();

            PhoneDropDownList.DataSource = Phones.Select(x => x.Number).ToList();
            PhoneDropDownList.DataBind();

            EmailDropDownList.DataSource = Emails;
            EmailDropDownList.DataBind();

            LocationUUIDDropDownList.DataSource = LocationUUIDs;
            LocationUUIDDropDownList.DataBind();

            MemberOrgDropDownList1.DataSource = MemberOfOrg;
            MemberOrgDropDownList1.DataBind();

            PropertyDropDownList.DataSource = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataBind();

            AnnotationDropDownList.DataSource = Annotations.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataBind();

            LinkDropDownList.DataSource = Links.Select(x => x.HRef).ToList();
            LinkDropDownList.DataBind();

            MainPartyTextarea.InnerText = party.Remarks;
            
        }

        private void PopulateEditPage()
        {
            var partyId = PartyDropDownList.SelectedValue;
            DocParty party = new DocParty();
            foreach (var elt in AllPartyInfo)
            {
                if (elt.PartyID == partyId)
                {
                    party = elt;
                    break;
                }

            }

            AddressLine1TextBox.Text = party.AddressLine1;
            AddressLine2TextBox.Text = party.AddressLine2;
            CityTextBox.Text = party.City;
            StateTextBox.Text = party.State;
            PostalCodeTextBox.Text = party.PostalCode;
            CountryTextBox.Text = party.Country;

        }

        private void PopulateEditAddressPage()
        {
            var partyId = PartyDropDownList.SelectedValue;
            Addresses = GetAddresses(partyId);
            var addressId = AddAddressDropDownList.SelectedValue;
           
           
            DocAddress party = new DocAddress();
            foreach (var elt in Addresses)
            {
                if (elt.AddressID == addressId)
                {
                    party = elt;
                    break;
                }

            }
            AddressTypeTextBox.Text = party.AddressType;
            AddressLine1TextBox.Text = party.AddressLine1;
            AddressLine2TextBox.Text = party.AddressLine2;
            CityTextBox.Text = party.City;
            StateTextBox.Text = party.State;
            PostalCodeTextBox.Text = party.PostalCode;
            CountryTextBox.Text = party.Country;
        }


        protected void SavePersonOrgButton_Click(object sender, EventArgs e)
        {


        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
           
            FormviewPanel.Visible = true;
        }

        protected void AddPartyButton_Click(object sender, EventArgs e)
        {
           
            MainPartyPanel.Visible = true;

            var PartyIds = GetPartyIds();
            var fullList = new List<string> { "" };
            fullList.AddRange(PartyIds);
            PartyDropDownList.DataSource = fullList;
            PartyDropDownList.DataBind();

          
            FormviewPanel.Visible = false;
        }

        protected void FormView1_PageIndexChanging(object sender, FormViewPageEventArgs e)
        {
            FormView1.PageIndex = e.NewPageIndex;
            FormView1.DataBind();
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAP/Metadata/Locations.aspx");
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAP/Metadata/SAPResponsibleParties.aspx");
        }

        protected void FormView1_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            var k0 = e.Keys[0];
            var old0 = e.OldValues[k0];

            FormView1.DataBind();
        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
           // AddPartyPanel.Visible = true;


            MainPartyPanel.Visible = true;



            var PartyIds = GetPartyIds();
            var fullList = new List<string> { "" };
            fullList.AddRange(PartyIds);
            PartyDropDownList.DataSource = fullList;
            PartyDropDownList.DataBind();

 
            FormviewPanel.Visible = false;
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            var partyId = PartyDropDownList.SelectedValue;


            var tagpartyid = string.Format("<metadata><party id=\"{0}\">", partyId);

            if (partyId.Length == 0)
                return;

            InsertElementToDataBase(DOID, SystemID, partyId, partyId.GetType(), tagpartyid, "party id, party id", partyId, 0);

            FormView1.DataBind();
            FormviewPanel.Visible = true;
        }

        protected void PartyIdDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateEditPage();


            PartyDropDownList.Width = 350;
            PartyDropDownList.Visible = true;
           
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
        void SetPartyPropertyPanel()
        {

           
            AddPropPanel.Visible = true;
            NameLabel.Text = "Name";
            ClassLabel.Text = "Class";
            NSLabel.Text = "NS";
            ValueLabel.Text = "Value";
            PropLabel.Text = "Property";
            PropLabel.Enabled = false;
            RemarksLabel.Text = "";
            RemarksTextArea.Visible = false;
            var id = "";
          
            if (PartyDropDownList.SelectedIndex == 0)
            {
                id = GeneratePartyID();
            }
            else
            {              
                id = PartyDropDownList.SelectedValue;                      
            }
            Props = GetProps(id, "party id");
            PartyID = id;
           
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

       

      

        protected void PropertyDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


       
        protected void AnnotationDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void LinkDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       

        private void PopulateLinkEditPage()
        {

            
            var propId = PropDropDownList.SelectedValue;
            var link = new Link();
            foreach (var elt in Links)
            {
                if (elt.HRef == propId)
                {
                    link = elt;
                    break;
                }

            }
            NameTextBox.Text = link.HRef;
            NSTextBox.Text = link.Rel;
            ClassTextBox.Text = link.MediaType;
            ValueTextBox.Text = link.MarkUpLine;
        }

        void SetPartyLinkPanel()
        {
           
            
            AddPropPanel.Visible = true;
            FormviewPanel.Visible = false;
            NameLabel.Text = "href";
            ClassLabel.Text = "rel";
            NSLabel.Text = "media-type";
            ValueLabel.Text = "Value";
            RemarksLabel.Text = "";
            RemarksTextArea.Visible = false;
            PropLabel.Text = "Link";
            PropLabel.Enabled = false;

            var id = "";


            if (PartyDropDownList.SelectedIndex == 0)
            {
                id = GeneratePartyID();
                PartyID = id;
            }
            else
            {

                id = PartyDropDownList.SelectedValue;
                Links = GetLinks(id, "party id");
                PartyID = id;

            }
      
            var names = Links.Select(x => x.HRef).ToList();
            var fullnames = new List<string> { "" };

            fullnames.AddRange(names);
            PropDropDownList.DataSource = fullnames;
            PropDropDownList.DataBind();

            PopulateLinkEditPage();

            LinkDropDownList.DataSource = names;
            LinkDropDownList.DataBind();
        }

        private string GeneratePartyID()
        {
            var nbrRole = Fullparties.Count();
            var id = string.Format("party-{0}", nbrRole);
            return id;
        }

        private string GenerateAddressID(string partyId)
        {
           
            Addresses = GetAddresses(partyId);
            var nbrRole = Addresses.Count();
            var id = string.Format("{0}-address-{1}",partyId, nbrRole);
            return id;
        }

        void SetPartyAnnotationPanel()
        {
            MainPartyPanel.Visible = false;
          
            AddPropPanel.Visible = true;
            FormviewPanel.Visible = false;

            NameLabel.Text = "Name";
            ClassLabel.Visible = false;
            ClassTextBox.Visible = false;

            NSLabel.Text = "NS";
            ValueLabel.Text = "Value";
            RemarksLabel.Text = "Remarks";
            RemarksTextArea.Visible = true;
            PropLabel.Text = "Annotation";
            PropLabel.Enabled = false;

            var id = "";
            if (PartyDropDownList.SelectedIndex == 0)
            {
                id = GeneratePartyID();
            }
            else
            {
                id = PartyDropDownList.SelectedValue;
            }
            PartyID = id;
            
            Annotations = GetAnnotations(id, "party id");
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
            // ClassTextBox.Text = prop.Class;
            ValueTextBox.Text = ann.Value;
            RemarksTextArea.InnerText = ann.Remarks;
        }

        protected void AddEditMainButton_Click(object sender, EventArgs e)
        {  
            var Id = "";
            if (PartyDropDownList.SelectedIndex == 0)
            {
                Id = GeneratePartyID();
            }
            else
            {
                Id = PartyDropDownList.SelectedValue;
            }
            PartyID = Id;
            var entity = "";

            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(Id, "party id");

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<metadata><party id=\"{0}\">{2}<prop id=\"{1}\">", Id, propid, entity);
                var name = NameTextBox.Text;
                var tagname = string.Format("<metadata><party id=\"{0}\">{2}<prop id=\"{1}\" name>", Id, propid, entity);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<metadata><party id=\"{0}\">{2}<prop id=\"{1}\" ns>", Id, propid, entity);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<metadata><party id=\"{0}\">{2}<prop id=\"{1}\" class>", Id, propid, entity);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><party id=\"{0}\">{2}<prop id=\"{1}\">", Id, propid, entity);

                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "party id, prop id", propid, 1);
                InsertElementToDataBase(DOID, SystemID, "name", propid.GetType(), tagname, "party id, prop name", name, 1);

                InsertElementToDataBase(DOID, SystemID, "ns", propid.GetType(), tagns, "party id, prop ns", ns, 1);

                InsertElementToDataBase(DOID, SystemID, "class", propid.GetType(), tagClass, "party id, prop class", classn, 1);
                InsertElementToDataBase(DOID, SystemID, "value", propid.GetType(), tagValue, "party id, prop", value, 1);

                var props = GetProps(Id, "party id");
                var names = props.Select(x => x.Name).ToList();
                var fullnames = new List<string> { "" };
                fullnames.AddRange(names);
                PropDropDownList.DataSource = fullnames;
                PropDropDownList.DataBind();

            }

            if (PropLabel.Text == "Annotation")
            {
                var annotationId = GenerateAnnotationID(Id, "party id");

                if (PropDropDownList.SelectedIndex > 0)
                {
                    annotationId = PropDropDownList.SelectedValue;
                }

                var tagAnnotationid = string.Format("<metadata><party id=\"{0}\">{2}<annotation id=\"{1}\">", Id, annotationId, entity);
                var name = NameTextBox.Text;
                var tagname = string.Format("<metadata><party id=\"{0}\">{2}<annotation id=\"{1}\" name>", Id, annotationId, entity);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<metadata><party id=\"{0}\">{2}<annotation id=\"{1}\" ns>", Id, annotationId, entity);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<metadata><party id=\"{0}\">{2}<annotation id=\"{1}\" class>", Id, annotationId, entity);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><party id=\"{0}\">{2}<annotation id=\"{1}\">", Id, annotationId, entity);
                var remarks = RemarksTextArea.InnerText;
                var tagRemarks = string.Format("<metadata><party id=\"{0}\">{2}<annotation id=\"{1}\"><remarks>", Id, annotationId, entity);


                InsertElementToDataBase(DOID, SystemID, annotationId, annotationId.GetType(), tagAnnotationid, "party id, annotation id", annotationId, 1);
                InsertElementToDataBase(DOID, SystemID, "name", annotationId.GetType(), tagname, "party id, annotation name", name, 1);

                InsertElementToDataBase(DOID, SystemID, "ns", annotationId.GetType(), tagns, "party id, annotation ns", ns, 1);

                InsertElementToDataBase(DOID, SystemID, "class", annotationId.GetType(), tagClass, "party id, annotation class", classn, 1);
                InsertElementToDataBase(DOID, SystemID, "value", annotationId.GetType(), tagValue, "party id, annotation", value, 1);

                InsertElementToDataBase(DOID, SystemID, "remarks", annotationId.GetType(), tagRemarks, "party id, annotation, remarks", remarks, 1);

            }

            if (PropLabel.Text == "Link")
            {
                var linkHref = "";

                if (PropDropDownList.SelectedIndex > 0)
                {
                    linkHref = PropDropDownList.SelectedValue;
                }
                linkHref = NameTextBox.Text;
                var taghref = string.Format("<metadata><party id=\"{0}\">{2}<link href=\"{1}\">", Id, linkHref, entity);

                var rel = NSTextBox.Text;
                var tagrel = string.Format("<metadata><party id=\"{0}\">{2}<link href=\"{1}\" rel>", Id, linkHref, entity);

                var media = ClassTextBox.Text;
                var tagmedia = string.Format("<metadata><party id=\"{0}\">{2}<link href=\"{1}\" media-type>", Id, linkHref, entity);

                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><party id=\"{0}\">{2}<link href=\"{1}\">", Id, linkHref, entity);

                InsertElementToDataBase(DOID, SystemID, "link", linkHref.GetType(), taghref, "party id, link href", linkHref, 1);
                InsertElementToDataBase(DOID, SystemID, "rel", rel.GetType(), tagrel, "party id, link rel", rel, 1);

                InsertElementToDataBase(DOID, SystemID, "media-type", media.GetType(), tagmedia, "party id, link media-type", media, 1);


                InsertElementToDataBase(DOID, SystemID, "value", value.GetType(), tagValue, "party id, link", value, 1);
            }


            
            FormviewPanel.Visible = true;
            MainPartyPanel.Visible = true;
        }

       

        protected void RemoveButton_Click(object sender, EventArgs e)
        {

        }

        protected void SavePartyButton_Click(object sender, EventArgs e)
        {
       
            string partyid;

            if (PartyDropDownList.SelectedIndex == 0)
            {
                partyid = GeneratePartyID();
            }
            else
            {
                partyid = PartyDropDownList.SelectedValue;
            }
          
            PartyID = partyid;
            var guid = Guid.NewGuid().ToString();
            var tagpartyid = string.Format("<metadata><party id=\"{0}\">", partyid);

            var tagpartyuuid = string.Format("<metadata><party id=\"{0}\"><uuid>", partyid);
            var tagpartytype = string.Format("<metadata><party id=\"{0}\"><type>", partyid);

            InsertElementToDataBase(DOID, SystemID, partyid, partyid.GetType(), tagpartyid, "party id, party id", partyid, 1);

            InsertElementToDataBase(DOID, SystemID, "party type", partyid.GetType(), tagpartytype, "party id, type", PartyTypeDropDownList.SelectedValue, 1);

            InsertElementToDataBase(DOID, SystemID, "uuid", partyid.GetType(), tagpartyuuid, "party id, uuid", guid, 1);

            var partyName = PartyNameTextBox.Text;
            var tagName = string.Format("<metadata><party id=\"{0}\"><name>", partyid);
            InsertElementToDataBase(DOID, SystemID, "name", partyid.GetType(), tagName, "party id, name", partyName, 1);

            var partyShortName = PartyShortNameTextBox.Text;
            var tagShortName = string.Format("<metadata><party id=\"{0}\"><short-name>", partyid);
            InsertElementToDataBase(DOID, SystemID, "short-name", partyid.GetType(), tagShortName, "party id, short-name", partyShortName, 1);

            var externalType = ExternalTypeTextBox.Text;
            var tagExternalType = string.Format("<metadata><party id=\"{0}\"><external-id type>", partyid);
            InsertElementToDataBase(DOID, SystemID, "external-id type", partyid.GetType(), tagExternalType, "party id, external-id type", externalType, 1);

            var externalID = ExternalIDTextBox.Text;
            var tagExternalID = string.Format("<metadata><party id=\"{0}\"><external-id>", partyid);
            InsertElementToDataBase(DOID, SystemID, "external-id ", partyid.GetType(), tagExternalID, "party id, external-id", externalID, 1);

            var Remark = MainPartyTextarea.InnerText;
            var tagRemark = string.Format("<metadata><party id=\"{0}\"><remarks>", partyid);
            InsertElementToDataBase(DOID, SystemID, "remarks", Remark.GetType(), tagRemark, "party id, remarks", Remark, 1);

            FormviewPanel.Visible = true;
            MainPartyPanel.Visible = false;
          
      
        }

        protected void PartyMainDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void DeletePartyButton_Click(object sender, EventArgs e)
        {

        }


       

        protected void RemoveButton_Click1(object sender, EventArgs e)
        {

        }

        protected void MainPropButton_Click(object sender, EventArgs e)
        {
            SetPartyPropertyPanel();
        }

        protected void MainAnnotationButton_Click(object sender, EventArgs e)
        {
            SetPartyAnnotationPanel();
        }

        protected void MainLinkButton_Click(object sender, EventArgs e)
        {
            SetPartyLinkPanel();
        }

 

       
        protected void PartyDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateEditMainPartyPage();
            MainPartyPanel.Visible = true;
        }

        protected void SaveAddressButton_Click(object sender, EventArgs e)
        {

            var partyid = "";
            var addressID = "";

            if (PartyDropDownList.SelectedIndex == 0)
            {
                partyid = GeneratePartyID();
            }
            else
            {
                partyid = PartyDropDownList.SelectedValue;
            }
            if (AddAddressDropDownList.SelectedIndex == 0)
                addressID = GenerateAddressID(partyid);
            else
                addressID = AddAddressDropDownList.SelectedValue;

            if (partyid.Length == 0)
                return;

            var tagAddressId = string.Format("<metadata><party id=\"{0}\"><address id=\"{1}\">", partyid, addressID);
            InsertElementToDataBase(DOID, SystemID, addressID, partyid.GetType(), tagAddressId, "party id, address id", addressID, 1);

            var tagType = string.Format("<metadata><party id=\"{0}\"><address id=\"{1}\"><type>", partyid, addressID);
            InsertElementToDataBase(DOID, SystemID, "address type", partyid.GetType(), tagType, "party id, address type", AddressTypeTextBox.Text, 1);


            var addressLine1 = AddressLine1TextBox.Text;
            var tagAddressLine1 = string.Format("<metadata><party id=\"{0}\"><address id=\"{1}\"><addr-line>", partyid, addressID);
            InsertElementToDataBase(DOID, SystemID, "addr-line", partyid.GetType(), tagAddressLine1, "party id, addr-line", addressLine1, 1);

            var addressLine2 = AddressLine2TextBox.Text;
            var tagAddressLine2 = string.Format("<metadata><party id=\"{0}\"><address id=\"{1}\"><addr-line>", partyid, addressID);
            InsertElementToDataBase(DOID, SystemID, "addr-line2", partyid.GetType(), tagAddressLine2, "party id, addr-line", addressLine2, 1);


            var city = CityTextBox.Text;
            var tagCity = string.Format("<metadata><party id=\"{0}\"><address id=\"{1}\"><city>", partyid, addressID);
            InsertElementToDataBase(DOID, SystemID, "city", partyid.GetType(), tagCity, "party id, city", city, 1);

            var state = StateTextBox.Text;
            var tagState = string.Format("<metadata><party id=\"{0}\"><address id=\"{1}\"><state>", partyid, addressID);
            InsertElementToDataBase(DOID, SystemID, "state", partyid.GetType(), tagState, "party id, state", state, 1);

            var postalCode = PostalCodeTextBox.Text;
            var tagPostalCode = string.Format("<metadata><party id=\"{0}\"><address id=\"{1}\"><postal-code>", partyid, addressID);
            InsertElementToDataBase(DOID, SystemID, "postal-code", partyid.GetType(), tagPostalCode, "party id, postal-code", postalCode, 1);

            var country = CountryTextBox.Text;
            var tagCountry = string.Format("<metadata><party id=\"{0}\"><address id=\"{1}\"><country>", partyid, addressID);
            InsertElementToDataBase(DOID, SystemID, "country", partyid.GetType(), tagCountry, "party id, country", country, 1);



            AddAuxPanel.Visible = false;
            AddAddressPanel.Visible = false;
            FormviewPanel.Visible = false;
            MainPartyPanel.Visible = true;
        }

        protected void AddAuxButton_Click(object sender, EventArgs e)
        {
            AddAuxPanel.Visible = true;
            MainPartyPanel.Visible = true;
           
            string partyId;
            if (PartyDropDownList.SelectedIndex > 0)
                partyId = PartyDropDownList.SelectedValue;
            else
            {
                partyId = GeneratePartyID();
            }

            if (FirstLabel.Text == "Phone Type")
            {

                var number = SecondTextBox.Text;

                var tagPhoneType = string.Format("<metadata><party id=\"{0}\"><phone number={1}><type>", partyId, number);
                var type = FirstTextBox.Text;
                var tagPhoneNumber = string.Format("<metadata><party id=\"{0}\"><phone number={1}>", partyId, number);

                InsertElementToDataBase(DOID, SystemID, "type", type.GetType(), tagPhoneType, "party id, phone type", type, 1);
                InsertElementToDataBase(DOID, SystemID, "phone", number.GetType(), tagPhoneNumber, "party id, phone", number, 1);

            }
            if (FirstLabel.Text == "Email")
            {

                var tag = string.Format("<metadata><party id=\"{0}\"><email>", partyId);
                var newEmail = FirstTextBox.Text;

                InsertElementToDataBase(DOID, SystemID, newEmail, newEmail.GetType(), tag, "party id, email", newEmail, 1);

            }

            if (FirstLabel.Text == "Member of Org")
            {

                var tag = string.Format("<metadata><party id=\"{0}\"><member-of-organization>", partyId);
                var url = FirstTextBox.Text;

                InsertElementToDataBase(DOID, SystemID, url, url.GetType(), tag, "party id, member-of-organization", url, 1); ;

            }
            if (FirstLabel.Text == "Location UUID")
            {

                var tag = string.Format("<metadata><party id=\"{0}\"><location-uuid>", partyId);
                var url = FirstTextBox.Text;

                InsertElementToDataBase(DOID, SystemID, url, url.GetType(), tag, "party id, location-uuid", url, 1); ;

            }

            AddAuxPanel.Visible = false;
        }

   

        protected void AddAddressButton_Click(object sender, EventArgs e)
        {
            AddAddressPanel.Visible = true;
            MainPartyPanel.Visible = true;
            AddAuxPanel.Visible = false;
            FormviewPanel.Visible = false;

            AddAddressPanel.Visible = true;
            MainPartyPanel.Visible = true;
            AddAuxPanel.Visible = false;
            FormviewPanel.Visible = false;
     

            string partyId;
            if (PartyDropDownList.SelectedIndex > 0)
                partyId = PartyDropDownList.SelectedValue;
            else
            {
                partyId = GeneratePartyID();
            }
            Addresses = GetAddresses(partyId);
            var ids = Addresses.Select(x => x.AddressID).ToList();
            var allids = new List<string> { "" };
            allids.AddRange(ids);
            AddAddressDropDownList.DataSource = allids;
            AddAddressDropDownList.DataBind();
        }

        protected void AddPhoneButton_Click(object sender, EventArgs e)
        {
            AddAddressPanel.Visible = false;
            MainPartyPanel.Visible = true;
            
            FormviewPanel.Visible = false;

            FirstLabel.Text = "Phone Type";
            SecondLabel.Text = "Phone Number";
            SecondLabel.Visible = true;
            SecondTextBox.Visible = true;
            AddAuxPanel.Visible = true;

            string partyId;
            if (PartyDropDownList.SelectedIndex > 0)
                partyId = PartyDropDownList.SelectedValue;
            else
            {
                partyId = GeneratePartyID();
            }

            var PhoneNumbers = GetPhones(partyId, "party id").Select(x=>x.Number).ToList();


            var allnumbers = new List<string> { "" };
            if (PhoneNumbers != null)
                allnumbers.AddRange(PhoneNumbers);
            AuxDropDownList.DataSource = allnumbers;
            AuxDropDownList.DataBind();
        }

        protected void AddEmailButton_Click(object sender, EventArgs e)
        {

            AddAddressPanel.Visible = false;
            MainPartyPanel.Visible = true;

            FormviewPanel.Visible = false;

            FirstLabel.Text = "Email";
           
            SecondLabel.Visible = false;
            SecondTextBox.Visible = false;
            AddAuxPanel.Visible = true;

            string partyId;
            if (PartyDropDownList.SelectedIndex > 0)
                partyId = PartyDropDownList.SelectedValue;
            else
            {
                partyId = GeneratePartyID();
            }

            var emails = GetAuxillaries(partyId, "party id", "email");


            var allnumbers = new List<string> { "" };
            if (emails != null)
                allnumbers.AddRange(emails);
            AuxDropDownList.DataSource = allnumbers;
            AuxDropDownList.DataBind();
        }

        protected void AddMemberOrgButton_Click(object sender, EventArgs e)
        {
            AddAddressPanel.Visible = false;
            MainPartyPanel.Visible = true;

            FormviewPanel.Visible = false;

            FirstLabel.Text = "Member of Org";

            SecondLabel.Visible = false;
            SecondTextBox.Visible = false;
            AddAuxPanel.Visible = true;

            string partyId;
            if (PartyDropDownList.SelectedIndex > 0)
                partyId = PartyDropDownList.SelectedValue;
            else
            {
                partyId = GeneratePartyID();
            }
         
            var orgs = GetAuxillaries(partyId, "party id", "member-of-organization");


            var allnumbers = new List<string> { "" };
            if (orgs != null)
                allnumbers.AddRange(orgs);
            AuxDropDownList.DataSource = allnumbers;
            AuxDropDownList.DataBind();
        }

        protected void LocationIDButton_Click(object sender, EventArgs e)
        {
            AddAddressPanel.Visible = false;
            MainPartyPanel.Visible = true;

            FormviewPanel.Visible = false;

            FirstLabel.Text = "Location UUID";

            SecondLabel.Visible = false;
            SecondTextBox.Visible = false;
            AddAuxPanel.Visible = true;

            string partyId;
            if (PartyDropDownList.SelectedIndex > 0)
                partyId = PartyDropDownList.SelectedValue;
            else
            {
                partyId = GeneratePartyID();
            }
            
            var locations = GetAuxillaries(partyId, "party id", "location-uuid");

            var allnumbers = new List<string> { "" };
            if (locations != null)
                allnumbers.AddRange(locations);
            AuxDropDownList.DataSource = allnumbers;
            AuxDropDownList.DataBind();
        }

        protected void PropDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = "";
            if (PartyDropDownList.SelectedIndex == 0)
            {
                id = GeneratePartyID();
            }
            else
            {
                id = PartyDropDownList.SelectedValue;
            }

            if (PropLabel.Text == "Property")
            {
                Props = GetProps(id, "party id");
                PopulatePropertEditPage();
            }
            if (PropLabel.Text == "Annotation")
            {
                Annotations = GetAnnotations(id, "party id");
                PopulateAnnotationEditPage();
            }
            if (PropLabel.Text == "Link")
            {
                Links = GetLinks(id, "party id");
                PopulateLinkEditPage();
            }

            MainPartyPanel.Visible = true;



            AddPropPanel.Visible = true;
        }

        protected void AddAddressDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddAddressPanel.Visible = true;
            MainPartyPanel.Visible = true;
            AddAuxPanel.Visible = false;
            FormviewPanel.Visible = false;
            PopulateEditAddressPage();


            //string partyId;
            //if (PartyDropDownList.SelectedIndex > 0)
            //    partyId = PartyDropDownList.SelectedValue;
            //else
            //{
            //    partyId = GeneratePartyID();
            //}
           
            //var ids =Addresses.Select(x => x.AddressID).ToList();

            //AddAddressDropDownList.DataSource = ids;
            //AddAddressDropDownList.DataBind();

        }

        protected void CancelPartyButton_Click(object sender, EventArgs e)
        {
            FormviewPanel.Visible = true;
            MainPartyPanel.Visible = false;
        }

        protected void AddressTypeTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}