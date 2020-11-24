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
    public partial class LocationsSAP : BasePage
    {
        int Width = 198;
     
        List<DocLocation>  SAPLocations;
        List<string> PhoneNumbers;
        List<string> Emails;

        List<string> Urls;

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
                SAPLocations = GetLocations();


                AddPropPanel.Visible = false;
                AddLocationPanel.Visible = false;
                MainDiv.Visible = false;
                AddAuxPanel.Visible = false;
                var bindingLocation = new BindingList<DocLocation>(SAPLocations);
                LocationGridView.DataSource = bindingLocation;
                LocationGridView.DataBind();

                GridviewPanel.Visible = true;



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
                StatusLabel.BackColor = Color.Red;
                StatusLabel.Text = ex.Message;
            }
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"~/PagesSAP/Metadata/Roles.aspx", false);
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {

            Response.Redirect(@"~/PagesSAP/Metadata/Parties.aspx", false);
        }

        private string GenerateLocationID()
        {
            var nbrRole = SAPLocations.Count();
            var id = string.Format("location-{0}", nbrRole);
            return id;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            string locationId;
            if (LocationDropDownList.SelectedIndex > 0)
                locationId = LocationDropDownList.SelectedValue;
            else
            {
                locationId = GenerateLocationID();
            }

            var taglocationId = string.Format("<metadata><location id=\"{0}\">", locationId);

            var taglocationUUId = string.Format("<metadata><location id=\"{0}\"><uuid>", locationId);

            var guid = Guid.NewGuid().ToString();

            var addressType = AddressTypeDropDownList.SelectedValue;
            var tagaddresstype = string.Format("<metadata><location id=\"{0}\"><address type>", locationId);


            var line1 = this.AddressLine1TextBox.Text;
            var tagline1 = string.Format("<metadata><location id=\"{0}\"><address><addr-line>", locationId);

            var line2 = this.AddressLine2TextBox.Text;
            var tagline2 = string.Format("<metadata><location id=\"{0}\"><address><addr-line>", locationId);

            var city = this.CityTextBox.Text;
            var tagcity = string.Format("<metadata><location id=\"{0}\"><address><city>", locationId);

            var postalcode = PostalCodeTextBox.Text;
            var tagpostalcode = string.Format("<metadata><location id=\"{0}\"><address><postal-code>", locationId);

            var state = StateTextBox.Text;
            var tagstate = string.Format("<metadata><location id=\"{0}\"><address><state>", locationId);

            var country = CountryTextBox.Text;
            var tagcountry = string.Format("<metadata><location id=\"{0}\"><address><country>", locationId);



            var remarks = RoleRemarksTextarea.InnerText;
            var tagRemarks = string.Format("<metadata><location id=\"{0}\"><address><remarks>", locationId);

            InsertElementToDataBase(DOID, SystemID, locationId, locationId.GetType(), taglocationId, "location id, location id", locationId, 1);
            InsertElementToDataBase(DOID, SystemID, "uuid", locationId.GetType(), taglocationUUId, "location id, uuid", guid, 1);

            InsertElementToDataBase(DOID, SystemID, "address", addressType.GetType(), tagaddresstype, "location id, address type", addressType, 1);

            InsertElementToDataBase(DOID, SystemID, "addr-line", AddressLine1TextBox.Text.GetType(), tagline1, "location id, addr-line", AddressLine1TextBox.Text, 1);
            InsertElementToDataBase(DOID, SystemID, "addr-line2", AddressLine2TextBox.Text.GetType(), tagline2, "location id, addr-line", AddressLine2TextBox.Text, 1);


            InsertElementToDataBase(DOID, SystemID, "city", CityTextBox.Text.GetType(), tagcity, "location id, city", CityTextBox.Text, 1);

            InsertElementToDataBase(DOID, SystemID, "state", StateTextBox.Text.GetType(), tagstate, "location id, state", StateTextBox.Text, 1);

            InsertElementToDataBase(DOID, SystemID, "country", CountryTextBox.Text.GetType(), tagcountry, "location id, country", CountryTextBox.Text, 1);

            InsertElementToDataBase(DOID, SystemID, "postal-code", PostalCodeTextBox.Text.GetType(), tagpostalcode, "location id, postal-code", PostalCodeTextBox.Text, 1);


           
            InsertElementToDataBase(DOID, SystemID, "remarks", remarks.GetType(), tagRemarks, "location id, remarks", remarks, 1);


            MainDiv.Visible = false;
            AddLocationPanel.Visible = false;
            GridviewPanel.Visible = true;
        }
        void SetPropertyPanel()
        {
            AddLocationPanel.Visible = true;
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

            var locationid = "";
            if (LocationDropDownList.SelectedIndex > 0)
                locationid = LocationDropDownList.SelectedValue;
            else
                locationid = GenerateLocationID();

            var props = GetProps(locationid, "location id");
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

        protected void PropButton_Click(object sender, EventArgs e)
        {
            SetPropertyPanel();
        }

        protected void LocationDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateEditPage();

            AddLocationPanel.Visible = true;
            MainDiv.Visible = true;
            LocationDropDownList.Width = Width;
            LocationDropDownList.Visible = true;
            DeleteButton.Visible = true;
        }

      

        protected void AnnotationButton_Click(object sender, EventArgs e)
        {
            SetAnnotationPanel();
        }

        protected void AnnotationDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void LinkDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void SetLinkPanel()
        {
            AddLocationPanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = true;
            GridviewPanel.Visible = false;
            NameLabel.Text = "href";
            ClassLabel.Text = "rel";
            NSLabel.Text = "media-type";
            ValueLabel.Text = "Value";
            RemarksLabel.Text = "";
            RemarksTextArea.Visible = false;
            PropLabel.Text = "Link";
            PropLabel.Enabled = false;

            var locationid = "";
            if (LocationDropDownList.SelectedIndex > 0)
                locationid = LocationDropDownList.SelectedValue;
            else
                locationid = GenerateLocationID();


            var links = GetLinks(locationid, "location id");
            var names = links.Select(x => x.HRef).ToList();
            var fullnames = new List<string> { "" };



            fullnames.AddRange(names);
            PropDropDownList.DataSource = fullnames;
            PropDropDownList.DataBind();

            PopulateLinkEditPage();

            LinkDropDownList.DataSource = names;
            LinkDropDownList.DataBind();
        }

        protected void LinkButton_Click(object sender, EventArgs e)
        {
            SetLinkPanel();
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
            if (PropLabel.Text == "Link")
            {
                PopulateLinkEditPage();
            }

            AddLocationPanel.Visible = true;
            MainDiv.Visible = true;
            LocationDropDownList.Width = Width;
            LocationDropDownList.Visible = true;
            DeleteButton.Visible = true;

            AddPropPanel.Visible = true;

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {

        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {

        }

        void SetAnnotationPanel()
        {
            AddLocationPanel.Visible = true;
            MainDiv.Visible = true;
            AddPropPanel.Visible = true;
            GridviewPanel.Visible = false;

            NameLabel.Text = "Name";
            ClassLabel.Visible = false;
            ClassTextBox.Visible = false;

            NSLabel.Text = "NS";
            ValueLabel.Text = "Value";
            RemarksLabel.Text = "Remarks";
            RemarksTextArea.Visible = true;
            PropLabel.Text = "Annotation";
            PropLabel.Enabled = false;

            var locationid = "";
            if (LocationDropDownList.SelectedIndex > 0)
                locationid = LocationDropDownList.SelectedValue;
            else
                locationid = GenerateLocationID();

            var annotations = GetAnnotations(locationid, "location id");
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

        protected void AddEditButton_Click(object sender, EventArgs e)
        {
            string locationId;
            if (LocationDropDownList.SelectedIndex > 0)
                locationId = LocationDropDownList.SelectedValue;
            else
            {
                locationId = GenerateLocationID();
            }



            if (PropLabel.Text == "Property")
            {
                var propid = GeneratePropID(locationId, "location id");

                if (PropDropDownList.SelectedIndex > 0)
                {
                    propid = PropDropDownList.SelectedValue;
                }

                var tagpropid = string.Format("<metadata><location id=\"{0}\"><prop id=\"{1}\">", locationId, propid);
                var name = NameTextBox.Text;
                var tagname = string.Format("<metadata><location id=\"{0}\"><prop id=\"{1}\" name>", locationId, propid);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<metadata><location id=\"{0}\"><prop id=\"{1}\" ns>", locationId, propid);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<metadata><location id=\"{0}\"><prop id=\"{1}\" class>", locationId, propid);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><location id=\"{0}\"><prop id=\"{1}\">", locationId, propid);

                InsertElementToDataBase(DOID, SystemID, propid, propid.GetType(), tagpropid, "location id, prop id", propid, 1);
                InsertElementToDataBase(DOID, SystemID, "name", propid.GetType(), tagname, "location id, prop name", name, 1);

                InsertElementToDataBase(DOID, SystemID, "ns", propid.GetType(), tagns, "location id, prop ns", ns, 1);

                InsertElementToDataBase(DOID, SystemID, "class", propid.GetType(), tagClass, "location id, prop class", classn, 1);
                InsertElementToDataBase(DOID, SystemID, "value", propid.GetType(), tagValue, "location id, prop", value, 1);

                var props = GetProps(locationId, "location id");
                var names = props.Select(x => x.Name).ToList();
                var fullnames = new List<string> { "" };
                fullnames.AddRange(names);
                PropDropDownList.DataSource = fullnames;
                PropDropDownList.DataBind();

            }

            if (PropLabel.Text == "Annotation")
            {
                var annotationId = GenerateAnnotationID(locationId, "location id");

                if (PropDropDownList.SelectedIndex > 0)
                {
                    annotationId = PropDropDownList.SelectedValue;
                }

                var tagAnnotationid = string.Format("<metadata><location id=\"{0}\"><annotation id=\"{1}\">", locationId, annotationId);
                var name = NameTextBox.Text;
                var tagname = string.Format("<metadata><location id=\"{0}\"><annotation id=\"{1}\" name>", locationId, annotationId);
                var ns = NSTextBox.Text;
                var tagns = string.Format("<metadata><location id=\"{0}\"><annotation id=\"{1}\" ns>", locationId, annotationId);
                var classn = ClassTextBox.Text;
                var tagClass = string.Format("<metadata><location id=\"{0}\"><annotation id=\"{1}\" class>", locationId, annotationId);
                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><location id=\"{0}\"><annotation id=\"{1}\">", locationId, annotationId);
                var remarks = RemarksTextArea.InnerText;
                var tagRemarks = string.Format("<metadata><location id=\"{0}\"><annotation id=\"{1}\"><remarks>", locationId, annotationId);


                InsertElementToDataBase(DOID, SystemID, annotationId, annotationId.GetType(), tagAnnotationid, "location id, annotation id", annotationId, 1);
                InsertElementToDataBase(DOID, SystemID, "name", annotationId.GetType(), tagname, "location id, annotation name", name, 1);

                InsertElementToDataBase(DOID, SystemID, "ns", annotationId.GetType(), tagns, "location id, annotation ns", ns, 1);

                InsertElementToDataBase(DOID, SystemID, "class", annotationId.GetType(), tagClass, "location id, annotation class", classn, 1);
                InsertElementToDataBase(DOID, SystemID, "value", annotationId.GetType(), tagValue, "location id, annotation", value, 1);

                InsertElementToDataBase(DOID, SystemID, "remarks", annotationId.GetType(), tagRemarks, "location id, annotation, remarks", remarks, 1);

            }

            if (PropLabel.Text == "Link")
            {
                var linkHref = "";

                if (PropDropDownList.SelectedIndex > 0)
                {
                    linkHref = PropDropDownList.SelectedValue;
                }
                linkHref = NameTextBox.Text;
                var taghref = string.Format("<metadata><location id=\"{0}\"><link href=\"{1}\">", locationId, linkHref);

                var rel = NSTextBox.Text;
                var tagrel = string.Format("<metadata><location id=\"{0}\"><link href=\"{1}\" rel>", locationId, linkHref);

                var media = ClassTextBox.Text;
                var tagmedia = string.Format("<metadata><location id=\"{0}\"><link href=\"{1}\" media-type>", locationId, linkHref);


                var value = ValueTextBox.Text;
                var tagValue = string.Format("<metadata><location id=\"{0}\"><link href=\"{1}\">", locationId, linkHref);

                InsertElementToDataBase(DOID, SystemID, "link", linkHref.GetType(), taghref, "location id, link href", linkHref, 1);
                InsertElementToDataBase(DOID, SystemID, "rel", rel.GetType(), tagrel, "location id, link rel", rel, 1);

                InsertElementToDataBase(DOID, SystemID, "media-type", media.GetType(), tagmedia, "location id, link media-type", media, 1);


                InsertElementToDataBase(DOID, SystemID, "value", value.GetType(), tagValue, "location id, link", value, 1);


            }

            
            MainDiv.Visible = true;
            AddLocationPanel.Visible = true;
            GridviewPanel.Visible = false;
        }

        protected void RemoveButton_Click(object sender, EventArgs e)
        {

        }

        protected void EditButton_Click(object sender, EventArgs e)
        {
            AddLocationPanel.Visible = true;
            MainDiv.Visible = true;

            LocationDropDownList.Width = Width;
            LocationDropDownList.Visible = true;
            DeleteButton.Visible = true;

            var locationIds = SAPLocations.Select(x => x.LocationID).ToList(); ;
            var fullList = new List<string> { "" };
            fullList.AddRange(locationIds);
            LocationDropDownList.DataSource = fullList;
            LocationDropDownList.DataBind();

            PopulateEditPage();
            GridviewPanel.Visible = false;
        }


        private void PopulateAnnotationEditPage()
        {

            var locationid = LocationDropDownList.SelectedValue;

            var anns = GetAnnotations(locationid, "location id");
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

        private void PopulateLinkEditPage()
        {

            var locationid = LocationDropDownList.SelectedValue;

            var links = GetLinks(locationid, "location id");
            var propId = PropDropDownList.SelectedValue;
            var link = new Link();
            foreach (var elt in links)
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

        private void PopulatePropertEditPage()
        {

            var locationid = LocationDropDownList.SelectedValue;

            var props = GetProps(locationid, "location id");
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
            var locationId = LocationDropDownList.SelectedValue;
            var location = new DocLocation();
            foreach (var elt in SAPLocations)
            {
                if (elt.LocationID == locationId)
                {
                    location = elt;
                    break;
                }

            }
            var loc = SAPLocations.Where(x => x.LocationID==locationId).FirstOrDefault();
            Emails = loc.Emails == null ? new List<string>() : loc.Emails;
            EmailDropDownList.DataSource = Emails;
            EmailDropDownList.DataBind();

            Urls = loc.Url == null ? new List<string>() : loc.Url;
            UrlDropDownList.DataSource = Urls;
            UrlDropDownList.DataBind();

            var Phones = loc.Phones == null ? new List<Phone>() : loc.Phones;
            var numbers = Phones.Select(x => x.Number).ToList();
            PhoneDropDownList.DataSource = numbers;
            PhoneDropDownList.DataBind();

            AddressTypeDropDownList.SelectedValue = location.AddressType;
            AddressLine1TextBox.Text = location.AddressLine1;
            AddressLine2TextBox.Text = location.AddressLine2;
            CityTextBox.Text = location.City;
            PostalCodeTextBox.Text = location.PostalCode;
            CountryTextBox.Text = location.Country;

            RoleRemarksTextarea.InnerText = location.Remarks;

            var Props = GetProps(locationId, "location id");
            var names = Props.Select(x => x.Name).ToList();
            PropertyDropDownList.DataSource = names;
            PropertyDropDownList.DataBind();

            var Anns = GetAnnotations(locationId, "location id");
            var annNames = Anns.Select(x => x.Name).ToList();
            AnnotationDropDownList.DataSource = annNames;
            AnnotationDropDownList.DataBind();

            var Links = GetLinks(locationId, "location id");
            var hrefs = Links.Select(x => x.HRef).ToList();
            LinkDropDownList.DataSource = hrefs;
            LinkDropDownList.DataBind();
        }

        protected void AddPhonesButton_Click(object sender, EventArgs e)
        {
            string locationId;
            if (LocationDropDownList.SelectedIndex > 0)
                locationId = LocationDropDownList.SelectedValue;
            else
            {
                locationId = GenerateLocationID();
            }
            var loc = SAPLocations.Where(x => x.LocationID == locationId).FirstOrDefault();
            var phones =loc.Phones == null? new List<Phone>(): loc.Phones;
            PhoneNumbers = phones.Select(x => x.Number).ToList();
            FirstLabel.Text = "Phone Type";
            SecondLabel.Text = "Phone Number";
            SecondLabel.Visible = true;
            SecondTextBox.Visible = true;
            AddAuxPanel.Visible = true;
            AddLocationPanel.Visible = true;
            MainDiv.Visible = true;
            var allnumbers = new List<string> { "" };
            if(PhoneNumbers!=null)
               allnumbers.AddRange(PhoneNumbers);
            AuxDropDownList.DataSource = allnumbers;
            AuxDropDownList.DataBind();

        }

        protected void AddEmailsButton_Click(object sender, EventArgs e)
        {
            FirstLabel.Text = "Email";
            SecondLabel.Visible = false;
            SecondTextBox.Visible = false;
            AddAuxPanel.Visible = true;
            AddLocationPanel.Visible = true;
            MainDiv.Visible = true;

            var allmails = new List<string> { "" };
            if( Emails != null) 
                allmails.AddRange(Emails);
            AuxDropDownList.DataSource = allmails;
            AuxDropDownList.DataBind();
        }

        protected void AddUrlButton_Click(object sender, EventArgs e)
        {
            FirstLabel.Text = "Url";
            SecondLabel.Visible = false;
            SecondTextBox.Visible = false;
            AddAuxPanel.Visible = true;
            AddLocationPanel.Visible = true;
            MainDiv.Visible = true;

            var allUrls = new List<string> { "" };
            if(Urls !=null)
                allUrls.AddRange(Urls);
            AuxDropDownList.DataSource = allUrls;
            AuxDropDownList.DataBind();
        }

        protected void AddAuxButton_Click(object sender, EventArgs e)
        {
            AddAuxPanel.Visible = true;
            AddLocationPanel.Visible = true;
            MainDiv.Visible = true;
            string locationId;
            if (LocationDropDownList.SelectedIndex > 0)
                locationId = LocationDropDownList.SelectedValue;
            else
            {
                locationId = GenerateLocationID();
            }

            if (FirstLabel.Text == "Phone Type")
            {

                 var number = SecondTextBox.Text;

                var tagPhoneType = string.Format("<metadata><location id=\"{0}\"><phone number={1}><type>", locationId, number );
                var type = FirstTextBox.Text;
                var tagPhoneNumber = string.Format("<metadata><location id=\"{0}\"><phone number={1}>", locationId, number);
               
                InsertElementToDataBase(DOID, SystemID, "type", type.GetType(), tagPhoneType, "location id, phone type", type, 1);
                InsertElementToDataBase(DOID, SystemID, "phone", number.GetType(), tagPhoneNumber, "location id, phone", number, 1);

            }
            if (FirstLabel.Text == "Email")
            {

                var tag = string.Format("<metadata><location id=\"{0}\"><email>", locationId);
                var newEmail = FirstTextBox.Text;
               
                InsertElementToDataBase(DOID, SystemID, newEmail, newEmail.GetType(), tag, "location id, email", newEmail, 1);
              
            }

            if (FirstLabel.Text == "Url")
            {

                var tag = string.Format("<metadata><location id=\"{0}\"><url>", locationId);
                var url = FirstTextBox.Text;

                InsertElementToDataBase(DOID, SystemID, url, url.GetType(), tag, "location id, url", url, 1); ;

            }


        }

        protected void PropertyDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}