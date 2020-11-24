using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Data;
using System.Threading;
using System.Web.Services;
using SqlDataProvider;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Configuration;
using System.Xml;
using System.Xml.Schema;
using OSCALHelperClasses;


namespace OSCALGenerator.Pages
{
    public partial class BasePage : System.Web.UI.Page
    {
        protected string ConnString;
        protected string ASAPConnString;
        protected CSharpDAL DAL;
        protected CSharpDAL ASAPDAL;
        protected List<Role> SavedRoles;
        public string UserName;
        public int SystemID;
        public int DOID;
        protected string OrgName;
        protected string SystemName;
        protected string DocName;
        protected List<DocParty> Fullparties;
        protected List<DocParty> AllPartyInfo;
        protected string Errors;
        protected XmlWriter Xwriter;
        protected XmlWriter X;
       
        protected string Status;
        public const string XMLNamespace = @"http://csrc.nist.gov/ns/oscal/1.0";
        public const string SSPschema = "oscal_ssp_schema.xsd";
        public const string SSP2schema = "Milestone2SSP_Schema.xsd";
        public const string SSP3schema = "SSP_Schema3.xsd";
        public const string SAPschema = "SAP_Schema.xsd";
        public const string SARschema = "SAR_Schema.xsd";
        protected Dictionary<string, int> ImplementationStatusDict;
        protected Dictionary<string, int> OriginationStatusDict;
        protected Dictionary<string, int> OriginationStatusShortDict;


        public string WordTempFilePath;
        public string TemplateFile = "FedRAMP-SSP-Moderate-Baseline-Template.docx";
        public string BaselinePropCountFile = "ModerateBaselineControlsToPropCount.txt";
        protected private bool OverwriteXMLMapping;

        protected void Page_Load(object sender, EventArgs e)
        {


            ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            DAL = new CSharpDAL(ConnString);

            ASAPConnString = ConfigurationManager.ConnectionStrings["ASAPConnectionString"].ConnectionString;
            ASAPDAL = new CSharpDAL(ASAPConnString);


        }


        public string GetXMLElement(string XMLFileName, string ElementName)
        {
            string xmlDataFile = HttpContext.Current.Server.MapPath(string.Format(@"~\Uploads\{0}", XMLFileName));
            string bytes = File.ReadAllText(xmlDataFile);
            System.IO.MemoryStream myStream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(bytes));
            string myElementValue = "";
            System.Xml.XmlReader xr = System.Xml.XmlReader.Create(myStream);
            while (xr.Read())
            {
                if (xr.NodeType == System.Xml.XmlNodeType.Element)
                    if (xr.Name == ElementName.ToString())
                    {
                        myElementValue = xr.ReadElementContentAsString();
                        break;
                    }
            }

            return (myElementValue);
        }

        



        public List<SecurityControl> GetSecurityControls(string xmlFile, string xmlSchema, string XMLNamespace)
        {


            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add(XMLNamespace, xmlSchema);
            schema.Compile();

            DataSet dataSet = new DataSet();
            dataSet.ReadXml(xmlFile, XmlReadMode.ReadSchema);
            XmlDocument doc = new XmlDocument();
            doc.Schemas = schema;
            doc.Load(xmlFile);

            var securityControls = new List<SecurityControl>();

            foreach (XmlNode node in doc.DocumentElement)  //XmlNode  doc.DocumentElement  Office.CustomXMLNode
            {
                if (node.NodeType == XmlNodeType.Text || node.NodeType == XmlNodeType.Comment)
                    continue;
                if (node.Name == "control-implementation")
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if (child.NodeType == XmlNodeType.Text || child.NodeType == XmlNodeType.Comment)
                            continue;

                        if (child.Attributes != null && child.Attributes.Count > 0 && (child.Attributes[0].Name == "control-id"))  ///|| child.Attributes[0].Name == "statement-id"
                        {
                            var control = new SecurityControl(child, 0, XMLNamespace);
                            if (control.ControlId != null)
                                securityControls.Add(control);
                        }

                    }
            }

            return securityControls;
        }



        public void InitPropertyStatusDictionaries()
        {
            ImplementationStatusDict = new Dictionary<string, int>();
            OriginationStatusDict = new Dictionary<string, int>();
            OriginationStatusShortDict = new Dictionary<string, int>();


            ImplementationStatusDict.Add("implemented", 0);
            ImplementationStatusDict.Add("partially-implemented", 1);
            ImplementationStatusDict.Add("planned", 2);
            ImplementationStatusDict.Add("alternative-implementation", 3);
            ImplementationStatusDict.Add("not-applicable", 4);

            OriginationStatusDict.Add("service-provider-corporate", 5);
            OriginationStatusDict.Add("service-provider-system-specific", 6);
            OriginationStatusDict.Add("service-provider-hybrid", 7);
            OriginationStatusDict.Add("configured-by-customer", 8);
            OriginationStatusDict.Add("provided-by-customer", 9);
            OriginationStatusDict.Add("shared", 10);
            OriginationStatusDict.Add("inherited", 11);

            OriginationStatusShortDict.Add("service-provider-corporate", 5);
            OriginationStatusShortDict.Add("service-provider-system-specific", 6);
            OriginationStatusShortDict.Add("service-provider-hybrid", 7);

            OriginationStatusShortDict.Add("provided-by-customer", 8);
            OriginationStatusShortDict.Add("shared", 9);
            OriginationStatusShortDict.Add("inherited", 10);

        }


        protected void ProcessChildrenNodeSAP(XmlNode node, StreamWriter sw3, int dOID, int SystemID)
        {
            if (node.NodeType == XmlNodeType.Text || node.NodeType == XmlNodeType.Comment || node.Name.Length == 0 || node.Name == "p" || node.Name == "table" || node.Name == "th" || node.Name == "td" || node.Name == "tr" || node.Name == "#text") //!node.HasChildNodes 
            {
                return;
            }
            var parentName = node.ParentNode.Name;
            var parentAttributes = node.ParentNode.Attributes;
            if (parentName == "system-security-plan")
                parentName = "";
            var eltDesc = "";
            var eltName = "";
            var eltDetail = "";
            Type eltTypeId = typeof(String);
            ElementTypeId realEltTypeId;
            ElementHeader eltHeader;
            var tag = "";
            var newtemp = "";

            var attributes = node.Attributes;
            var children = node.ChildNodes;

            var text = node.InnerText;
            var name = node.Name;
            eltName = name;
            eltDetail = node.InnerText;
            eltTypeId = node.InnerText.GetType();
            //tag = string.Format("<{0}>", name);

            string tagdesc;
            string realdesc;
            tag = GetTagSAP(node, out tagdesc);
            if (HasDescription(node, out realdesc))
                eltDesc = realdesc;
            else
                eltDesc = tagdesc;


            var line = string.Format("----------------- Node:{0}--------------", name);
            //  InsertElementToDataBase(eltName, eltTypeId, tag, eltDesc, eltDetail, 1);

            var gag = "";

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].NodeType == XmlNodeType.Text || children[i].NodeType == XmlNodeType.Comment || children[i].Name.Length == 0 || children[i].Name == "p" || children[i].Name == "table" || children[i].Name == "th" || children[i].Name == "td" || children[i].Name == "tr" || children[i].Name == "#text") //!node.HasChildNodes 
                {
                    continue;
                }
                var gog = "";
                eltDesc = "";
                //HasDescription(children[i], out eltDesc);
                if (children[i].Attributes == null || children[i].Attributes.Count == 0)
                {
                    tag = GetTagSAP(children[i], out eltDesc);                 
                    eltName = children[i].Name;
                    eltDetail = children[i].InnerText;
                    eltTypeId = children[i].InnerText.GetType();


                    realEltTypeId = GetElementTypeId(eltTypeId);

                    newtemp = string.Format("Name:{0} Type:{1} Tag:{2} Detail:{3} Desc:{4}  ##", eltName, eltTypeId.ToString(), tag, eltDetail, eltDesc);
                    sw3.WriteLine(newtemp);

                    InsertElementToDataBase(dOID, SystemID, eltName, eltTypeId, tag, eltDesc, eltDetail, 1);

                }
                else
                {
                    for (int p = 0; p < children[i].Attributes.Count; p++)
                    {
                        eltDesc = "";
                        HasDescription(children[i], out eltDesc);
                        gog += string.Format(" Name:{0}  Value:{1}  ", children[i].Attributes[p].Name, children[i].Attributes[p].Value);
                        tag = GetTagSAP(children[i], out eltDesc);
                       
                        eltDetail = children[i].InnerText;
                        eltName = children[i].Attributes[p].Value;
                        eltTypeId = children[i].Attributes[p].Value.GetType();
                        //eltDesc = children[i].Attributes[p].Name;

                        eltDesc = eltDesc + string.Format(" {0}", children[i].Attributes[p].Name);

                        realEltTypeId = GetElementTypeId(eltTypeId);
                        eltHeader = new ElementHeader(eltName, realEltTypeId, tag, "", eltDetail, 1);

                        InsertElementToDataBase(dOID, SystemID, eltName, eltTypeId, tag, eltDesc, eltDetail, 1);

                        newtemp = string.Format("Name:{0} Type:{1} Tag:{2} Desc:{3}  Detail:{4}   ##", eltName, eltTypeId.ToString(), tag, eltDesc, eltDetail);
                        sw3.WriteLine(newtemp);
                    }
                }

                ProcessChildrenNodeSAP(children[i], sw3, dOID, SystemID);
            }


        }

        protected void ProcessChildrenNodeSAR(XmlNode node, StreamWriter sw3, int dOID, int SystemID)
        {
            if (node.NodeType == XmlNodeType.Text || node.NodeType == XmlNodeType.Comment || node.Name.Length == 0 || node.Name == "p" || node.Name == "table" || node.Name == "th" || node.Name == "td" || node.Name == "tr" || node.Name == "#text") //!node.HasChildNodes 
            {
                return;
            }
            var parentName = node.ParentNode.Name;
            var parentAttributes = node.ParentNode.Attributes;
            if (parentName == "system-security-plan")
                parentName = "";
            var eltDesc = "";
            var eltName = "";
            var eltDetail = "";
            Type eltTypeId = typeof(String);
            ElementTypeId realEltTypeId;
            ElementHeader eltHeader;
            var tag = "";
            var newtemp = "";

            var attributes = node.Attributes;
            var children = node.ChildNodes;

            var text = node.InnerText;
            var name = node.Name;
            eltName = name;
            eltDetail = node.InnerText;
            eltTypeId = node.InnerText.GetType();
            //tag = string.Format("<{0}>", name);

            string tagdesc;
            string realdesc;
            tag = GetTagSAR(node, out tagdesc);
            if (HasDescription(node, out realdesc))
                eltDesc = realdesc;
            else
                eltDesc = tagdesc;


            var line = string.Format("----------------- Node:{0}--------------", name);
            //  InsertElementToDataBase(eltName, eltTypeId, tag, eltDesc, eltDetail, 1);

            var gag = "";

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].NodeType == XmlNodeType.Text || children[i].NodeType == XmlNodeType.Comment || children[i].Name.Length == 0 || children[i].Name == "p" || children[i].Name == "table" || children[i].Name == "th" || children[i].Name == "td" || children[i].Name == "tr" || children[i].Name == "#text") //!node.HasChildNodes 
                {
                    continue;
                }
                var gog = "";
                eltDesc = "";
                //HasDescription(children[i], out eltDesc);
                if (children[i].Attributes == null || children[i].Attributes.Count == 0)
                {
                    tag = GetTagSAR(children[i], out eltDesc);

                    
                    eltName = children[i].Name;
                    eltDetail = children[i].InnerText;
                    eltTypeId = children[i].InnerText.GetType();


                    realEltTypeId = GetElementTypeId(eltTypeId);

                    newtemp = string.Format("Name:{0} Type:{1} Tag:{2} Detail:{3} Desc:{4}  ##", eltName, eltTypeId.ToString(), tag, eltDetail, eltDesc);
                    sw3.WriteLine(newtemp);

                    InsertElementToDataBase(dOID, SystemID, eltName, eltTypeId, tag, eltDesc, eltDetail, 1);

                }
                else
                {
                    for (int p = 0; p < children[i].Attributes.Count; p++)
                    {
                        eltDesc = "";
                        HasDescription(children[i], out eltDesc);
                        gog += string.Format(" Name:{0}  Value:{1}  ", children[i].Attributes[p].Name, children[i].Attributes[p].Value);
                        tag = GetTagSAR(children[i], out eltDesc);
                       
                        eltDetail = children[i].InnerText;
                        eltName = children[i].Attributes[p].Value;
                        eltTypeId = children[i].Attributes[p].Value.GetType();
                        //eltDesc = children[i].Attributes[p].Name;

                        eltDesc = eltDesc + string.Format(" {0}", children[i].Attributes[p].Name);

                        realEltTypeId = GetElementTypeId(eltTypeId);
                        eltHeader = new ElementHeader(eltName, realEltTypeId, tag, "", eltDetail, 1);

                        InsertElementToDataBase(dOID, SystemID, eltName, eltTypeId, tag, eltDesc, eltDetail, 1);

                        newtemp = string.Format("Name:{0} Type:{1} Tag:{2} Desc:{3}  Detail:{4}   ##", eltName, eltTypeId.ToString(), tag, eltDesc, eltDetail);
                        sw3.WriteLine(newtemp);
                    }
                }

                ProcessChildrenNodeSAR(children[i], sw3, dOID, SystemID);
            }


        }
        protected void ProcessChildrenNode(XmlNode node, StreamWriter sw3, int dOID, int SystemID)
        {
            if (node.NodeType == XmlNodeType.Text || node.NodeType == XmlNodeType.Comment || node.Name.Length == 0 || node.Name == "p" || node.Name == "table" || node.Name == "th" || node.Name == "td" || node.Name == "tr" || node.Name == "#text") //!node.HasChildNodes 
            {
                return;
            }
            var parentName = node.ParentNode.Name;
            var parentAttributes = node.ParentNode.Attributes;
            if (parentName == "system-security-plan")
                parentName = "";
            var eltDesc = "";
            var eltName = "";
            var eltDetail = "";
            Type eltTypeId = typeof(String);
            ElementTypeId realEltTypeId;
            ElementHeader eltHeader;
            var tag = "";
            var newtemp = "";

            var attributes = node.Attributes;
            var children = node.ChildNodes;

            var text = node.InnerText;
            var name = node.Name;
            eltName = name;
            eltDetail = node.InnerText;
            eltTypeId = node.InnerText.GetType();
            //tag = string.Format("<{0}>", name);

            string tagdesc;
            string realdesc;
            tag = GetTag(node, out tagdesc);
            if (HasDescription(node, out realdesc))
                eltDesc = realdesc;
            else
                eltDesc = tagdesc;


            var line = string.Format("----------------- Node:{0}--------------", name);
            //  InsertElementToDataBase(eltName, eltTypeId, tag, eltDesc, eltDetail, 1);

            var gag = "";

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].NodeType == XmlNodeType.Text || children[i].NodeType == XmlNodeType.Comment || children[i].Name.Length == 0 || children[i].Name == "p" || children[i].Name == "table" || children[i].Name == "th" || children[i].Name == "td" || children[i].Name == "tr" || children[i].Name == "#text") //!node.HasChildNodes 
                {
                    continue;
                }
                var gog = "";
                eltDesc = "";
                //HasDescription(children[i], out eltDesc);
                if (children[i].Attributes == null || children[i].Attributes.Count == 0)
                {
                    tag = GetTag(children[i], out eltDesc);

                    //if (parentName.Length > 0)
                    //{
                    //    if (attributes.Count == 0)
                    //    {
                    //        tag = string.Format("<{2}><{0}><{1}>", name, children[i].Name, parentName);
                    //    }
                    //    else
                    //    { 
                    //        tag = string.Format("<{2}><{0} {3}=\"{4}\"><{1}>", name, children[i].Name, parentName, attributes[0].Name, attributes[0].Value);
                    //        eltDesc = string.Format("{0} {1}, {2} ", name, attributes[0].Name, children[i].Name);
                    //    }
                    //}
                    //else
                    //{
                    //    if (attributes.Count == 0)
                    //    {
                    //        tag = string.Format("<{0}><{1}>", name, children[i].Name);
                    //    }
                    //    else
                    //    {
                    //        tag = string.Format("<{0} {2}=\"{3}\"><{1}>", name, children[i].Name, attributes[0].Name, attributes[0].Value);

                    //        eltDesc = string.Format("{0} {1}, {2} ", name, attributes[0].Name, children[i].Name);
                    //    }
                    //}
                    eltName = children[i].Name;
                    eltDetail = children[i].InnerText;
                    eltTypeId = children[i].InnerText.GetType();


                    realEltTypeId = GetElementTypeId(eltTypeId);

                    newtemp = string.Format("Name:{0} Type:{1} Tag:{2} Detail:{3} Desc:{4}  ##", eltName, eltTypeId.ToString(), tag, eltDetail, eltDesc);
                    sw3.WriteLine(newtemp);

                    InsertElementToDataBase(dOID, SystemID, eltName, eltTypeId, tag, eltDesc, eltDetail, 1);

                }
                else
                {
                    for (int p = 0; p < children[i].Attributes.Count; p++)
                    {
                        eltDesc = "";
                        HasDescription(children[i], out eltDesc);
                        gog += string.Format(" Name:{0}  Value:{1}  ", children[i].Attributes[p].Name, children[i].Attributes[p].Value);
                        tag = GetTag(children[i], out eltDesc);
                        //if (parentName.Length > 0)
                        //{
                        //    tag = string.Format("<{4}><{0}><{1} {2}=\"{3}\">", name, children[i].Name, children[i].Attributes[p].Name, children[i].Attributes[p].Value, parentName);
                        //    eltDesc = string.Format("{0} {1} ", children[i].Name, children[i].Attributes[p].Name);
                        //}
                        //else
                        //{
                        //    tag = string.Format("<{0}><{1} {2}=\"{3}\">", name, children[i].Name, children[i].Attributes[p].Name, children[i].Attributes[p].Value);
                        //    eltDesc = string.Format("{0} {1} ", children[i].Name, children[i].Attributes[p].Name);

                        //}
                        eltDetail = children[i].InnerText;
                        eltName = children[i].Attributes[p].Value;
                        eltTypeId = children[i].Attributes[p].Value.GetType();
                        //eltDesc = children[i].Attributes[p].Name;

                        eltDesc = eltDesc + string.Format(" {0}", children[i].Attributes[p].Name);

                        realEltTypeId = GetElementTypeId(eltTypeId);
                        eltHeader = new ElementHeader(eltName, realEltTypeId, tag, "", eltDetail, 1);

                        InsertElementToDataBase(dOID, SystemID, eltName, eltTypeId, tag, eltDesc, eltDetail, 1);

                        newtemp = string.Format("Name:{0} Type:{1} Tag:{2} Desc:{3}  Detail:{4}   ##", eltName, eltTypeId.ToString(), tag, eltDesc, eltDetail);
                        sw3.WriteLine(newtemp);
                    }
                }

                ProcessChildrenNode(children[i], sw3, dOID, SystemID);
            }


        }

        protected void ProcessNode(XmlNode node, StreamWriter sw3, int dOID, int SystemID)
        {
            var eltDesc = "";
            var eltName = "";
            var eltDetail = "";
            Type eltTypeId = typeof(String);
            ElementTypeId realEltTypeId;
            ElementHeader eltHeader;
            var tag = "";
            var newtemp = "";

            var attributes = node.Attributes;
            var children = node.ChildNodes;

            var text = node.InnerText;
            var name = node.Name;
            eltDetail = node.InnerText;
            eltTypeId = node.InnerText.GetType();
            tag = string.Format("<{0}>", name);
            eltDesc = "";
            HasDescription(node, out eltDesc);
            var line = string.Format("----------------- Node:{0}--------------", name);
            InsertElementToDataBase(dOID, SystemID, eltName, eltTypeId, tag, eltDesc, eltDetail, 1);

            var gag = "";

            if (children == null || children.Count == 0)
            {

                if (attributes == null || attributes.Count == 0)
                {
                    tag = string.Format("<{0}>", eltName);
                    eltName = node.Name;
                    eltDetail = node.InnerText;
                    eltTypeId = node.InnerText.GetType();
                    eltDesc = "";
                    HasDescription(node, out eltDesc);
                    newtemp = string.Format("Name:{0} Type:{1} Tag:{2} Detail:{3}   ##", eltName, eltTypeId.ToString(), tag, eltDetail);
                    sw3.WriteLine(newtemp);

                    tag = GetTag(node, out eltDesc);

                    InsertElementToDataBase(dOID, SystemID, eltName, eltTypeId, tag, eltDesc, eltDetail, 1);

                }
                else
                {
                    for (int i = 0; i < attributes.Count; i++)
                    {
                        gag += string.Format("  Name:{0}  Value:{1}  ", attributes[i].Name, attributes[i].Value);

                        tag = string.Format("<{0} {1}=\"{2}\">", name, attributes[i].Name, attributes[i].Value);
                        tag = GetTag(node, out eltDesc);
                        eltDetail = attributes[i].InnerText;
                        eltName = attributes[i].Value;
                        eltTypeId = attributes[i].Value.GetType();
                        eltDesc = attributes[i].Name;
                        newtemp = string.Format("Name:{0} Type:{1} Tag:{2} Detail:{3} Desc:{4}  ##", eltName, eltTypeId.ToString(), tag, eltDetail, eltDesc);
                        sw3.WriteLine(newtemp);

                        InsertElementToDataBase(dOID, SystemID, eltName, eltTypeId, tag, eltDesc, eltDetail, 1);

                    }

                }

            }
            else
            {
                ProcessChildrenNode(node, sw3,dOID, SystemID);


            }


        }

        protected string GetTagSAR(XmlNode node, out string desc)
        {
            StringBuilder sb = new StringBuilder();
            string name = node.Name;
            string attr = "";
            var tem = "";

            while (node.Name != "assessment-results")
            {
                var attributes = node.Attributes;
                if (attributes != null && attributes.Count > 0)
                {
                    var sbb = new StringBuilder();
                    sbb.Append(string.Format("<{0} ", node.Name));
                    for (int k = 0; k < attributes.Count; k++)
                    {
                        sbb.Append(string.Format(" {0}=\"{1}\"", attributes[k].Name, attributes[k].Value));
                    }

                    sbb.Append(">");
                    tem = sbb.ToString();
                    // tem = string.Format("<{0} {1}=\"{2}\">", node.Name, attributes[0].Name, attributes[0].Value);
                    attr = string.Format("{0} {1}", node.Name, attributes[0].Name);
                    sb.Insert(0, tem);
                }
                else
                {
                    var tim = string.Format("<{0}>", node.Name);
                    sb.Insert(0, tim);
                }

                node = node.ParentNode;
            }
            if (attr.Length > 0)
                desc = string.Format("{0}, {1}", attr, name);
            else
                desc = name;
            return sb.ToString();

        }

      

        protected string GetTagSAP(XmlNode node, out string desc)
        {
            StringBuilder sb = new StringBuilder();
            string name = node.Name;
            string attr = "";
            var tem = "";

            while (node.Name != "assessment-plan")
            {
                var attributes = node.Attributes;
                if (attributes != null && attributes.Count > 0)
                {
                    var sbb = new StringBuilder();
                    sbb.Append(string.Format("<{0} ", node.Name));
                    for (int k = 0; k < attributes.Count; k++)
                    {
                        sbb.Append(string.Format(" {0}=\"{1}\"", attributes[k].Name, attributes[k].Value));
                    }

                    sbb.Append(">");
                    tem = sbb.ToString();
                    // tem = string.Format("<{0} {1}=\"{2}\">", node.Name, attributes[0].Name, attributes[0].Value);
                    attr = string.Format("{0} {1}", node.Name, attributes[0].Name);
                    sb.Insert(0, tem);
                }
                else
                {
                    var tim = string.Format("<{0}>", node.Name);
                    sb.Insert(0, tim);
                }

                node = node.ParentNode;
            }
            if (attr.Length > 0)
                desc = string.Format("{0}, {1}", attr, name);
            else
                desc = name;
            return sb.ToString();

        }

        protected string GetTag(XmlNode node, out string desc)
        {
            StringBuilder sb = new StringBuilder();
            string name = node.Name;
            string attr = "";
            var tem = "";

            while (node.Name != "system-security-plan")
            {
                var attributes = node.Attributes;
                if (attributes != null && attributes.Count > 0)
                {
                    var sbb = new StringBuilder();
                    sbb.Append(string.Format("<{0} ", node.Name));
                    for (int k = 0; k < attributes.Count; k++)
                    {
                        sbb.Append(string.Format(" {0}=\"{1}\"", attributes[k].Name, attributes[k].Value));
                    }

                    sbb.Append(">");
                    tem = sbb.ToString();
                    // tem = string.Format("<{0} {1}=\"{2}\">", node.Name, attributes[0].Name, attributes[0].Value);
                    attr = string.Format("{0} {1}", node.Name, attributes[0].Name);
                    sb.Insert(0, tem);
                }
                else
                {
                    var tim = string.Format("<{0}>", node.Name);
                    sb.Insert(0, tim);
                }

                node = node.ParentNode;
            }
            if (attr.Length > 0)
                desc = string.Format("{0}, {1}", attr, name);
            else
                desc = name;
            return sb.ToString();

        }

        protected bool HasDescription(XmlNode node, out string desc)
        {
            if (node.HasChildNodes)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.Name == "desc" || child.Name == "description")
                    {
                        desc = child.InnerText;
                        return true;
                    }
                }
            }

            desc = "";
            return false;
        }

        public void ImportToDBSAR(string XMLNamespace, string OscalSchemaPath, string FileToImportPath, int dOID, int SystemID)
        {
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add(XMLNamespace, OscalSchemaPath);
            schema.Compile();


            DataSet dataSet = new DataSet();
            dataSet.ReadXml(FileToImportPath, XmlReadMode.ReadSchema);
            XmlDocument doc = new XmlDocument();
            doc.Schemas = schema;
            doc.Load(FileToImportPath);
            var appPath = Request.PhysicalApplicationPath;
            string output = string.Format(@"{0}DBImportLog.txt", appPath);


            try
            {
                using (StreamWriter sw3 = new StreamWriter(output))
                {
                    foreach (XmlNode node in doc.DocumentElement)
                    {

                        ProcessChildrenNodeSAR(node, sw3, dOID, SystemID);
                    }

                    sw3.Close();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ImportToDBSAP(string XMLNamespace, string OscalSchemaPath, string FileToImportPath, int dOID, int SystemID)
        {
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add(XMLNamespace, OscalSchemaPath);
            schema.Compile();


            DataSet dataSet = new DataSet();
            dataSet.ReadXml(FileToImportPath, XmlReadMode.ReadSchema);
            XmlDocument doc = new XmlDocument();
            doc.Schemas = schema;
            doc.Load(FileToImportPath);
            var appPath = Request.PhysicalApplicationPath;
            string output = string.Format(@"{0}DBImportLog.txt", appPath);


            try
            {
                using (StreamWriter sw3 = new StreamWriter(output))
                {
                    foreach (XmlNode node in doc.DocumentElement)
                    {

                        ProcessChildrenNodeSAP(node, sw3, dOID, SystemID);
                    }

                    sw3.Close();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public void ImportToDB( string XMLNamespace, string OscalSchemaPath, string FileToImportPath, int dOID, int SystemID)
        {
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add(XMLNamespace, OscalSchemaPath);
            schema.Compile();


            DataSet dataSet = new DataSet();
            dataSet.ReadXml(FileToImportPath, XmlReadMode.ReadSchema);
            XmlDocument doc = new XmlDocument();
            doc.Schemas = schema;
            doc.Load(FileToImportPath);
           var appPath = Request.PhysicalApplicationPath;
            string output = string.Format(@"{0}DBImportLog.txt", appPath);


            try
            {
                using (StreamWriter sw3 = new StreamWriter(output))
                {
                    foreach (XmlNode node in doc.DocumentElement)
                    {

                        ProcessChildrenNode(node, sw3, dOID, SystemID);
                    }

                    sw3.Close();
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public static string Utf16ToUtf8(string utf16String)
        {
            /**************************************************************
             * Every .NET string will store text with the UTF16 encoding, *
             * known as Encoding.Unicode. Other encodings may exist as    *
             * Byte-Array or incorrectly stored with the UTF16 encoding.  *
             *                                                            *
             * UTF8 = 1 bytes per char                                    *
             *    ["100" for the ansi 'd']                                *
             *    ["206" and "186" for the russian 'κ']                   *
             *                                                            *
             * UTF16 = 2 bytes per char                                   *
             *    ["100, 0" for the ansi 'd']                             *
             *    ["186, 3" for the russian 'κ']                          *
             *                                                            *
             * UTF8 inside UTF16                                          *
             *    ["100, 0" for the ansi 'd']                             *
             *    ["206, 0" and "186, 0" for the russian 'κ']             *
             *                                                            *
             * We can use the convert encoding function to convert an     *
             * UTF16 Byte-Array to an UTF8 Byte-Array. When we use UTF8   *
             * encoding to string method now, we will get a UTF16 string. *
             *                                                            *
             * So we imitate UTF16 by filling the second byte of a char   *
             * with a 0 byte (binary 0) while creating the string.        *
             **************************************************************/

            // Storage for the UTF8 string
            string utf8String = String.Empty;

            // Get UTF16 bytes and convert UTF16 bytes to UTF8 bytes
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(utf16String);
            byte[] utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);

            // Fill UTF8 bytes inside UTF8 string
            for (int i = 0; i < utf8Bytes.Length; i++)
            {
                // Because char always saves 2 bytes, fill char with 0
                byte[] utf8Container = new byte[2] { utf8Bytes[i], 0 };
                utf8String += BitConverter.ToChar(utf8Container, 0);
            }

            // Return UTF8
            return utf8String;
        }

       

        protected DataSet AddHeaderElement(int deid, string eltName, int eltTypeId, string eltTag, string eltDescription, int active)
        {


            //Create Parameters
            SqlParameter[] oParams = new SqlParameter[8];
            oParams[0] = new SqlParameter("UID", SqlDbType.Int, 10);
            oParams[0].Value = 1;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = 1;
            oParams[2] = new SqlParameter("ElementName", SqlDbType.NVarChar, 255);
            oParams[2].Value = eltName;
            oParams[3] = new SqlParameter("ElementTypeId", SqlDbType.Int, 10);
            oParams[3].Value = eltTypeId;
            oParams[4] = new SqlParameter("ElementTag", SqlDbType.NVarChar, 255);
            oParams[4].Value = eltTag;
            oParams[5] = new SqlParameter("ElementDesc", SqlDbType.NVarChar, 1024);
            oParams[5].Value = eltDescription;
            oParams[6] = new SqlParameter("Active", SqlDbType.Int, 10);
            oParams[6].Value = active;
            oParams[7] = new SqlParameter("DEID", SqlDbType.Int, 10);
            oParams[7].Value = deid;

          
            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocHeader_Put]", CommandType.StoredProcedure, oParams);

            return _ds;

        }


        DataSet AddHeaderElement(string eltName, int eltTypeId, string eltTag, string eltDescription, int active)
        {


            //Create Parameters
            SqlParameter[] oParams = new SqlParameter[7];
            oParams[0] = new SqlParameter("UID", SqlDbType.Int, 10);
            oParams[0].Value = 1;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = 1;
            oParams[2] = new SqlParameter("ElementName", SqlDbType.NVarChar, 255);
            oParams[2].Value = eltName;
            oParams[3] = new SqlParameter("ElementTypeId", SqlDbType.Int, 10);
            oParams[3].Value = eltTypeId;
            oParams[4] = new SqlParameter("ElementTag", SqlDbType.NVarChar, 255);
            oParams[4].Value = eltTag;
            oParams[5] = new SqlParameter("ElementDesc", SqlDbType.NVarChar, 1024);
            oParams[5].Value = eltDescription;
            oParams[6] = new SqlParameter("Active", SqlDbType.Int, 10);
            oParams[6].Value = active;


            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocHeader_Put]", CommandType.StoredProcedure, oParams);

            return _ds;

        }


       



        public List<User> GetUsers()
        {
            var result = new List<User>();
            var userIds = new List<List<string>>();
            var roleIds = new List<List<string>>();
            var titles = new List<List<string>>();
            var NSs = new List<List<string>>();
            var Externals = new List<List<string>>();
            var Accesses = new List<List<string>>();
            var SensitivityLevels = new List<List<string>>();
            var AuthorizedPrivilegeNames = new List<List<string>>();
            var FunctionPerformeds = new List<List<string>>();

           

            userIds = GetHeaderAndTag("user id, user id", UserName, DOID);
            roleIds = GetDBData("user id, role-id", UserName, DOID);
            titles = GetDBData("user id, title", UserName, DOID);
            Externals = GetDBData("user id, prop name", UserName, DOID);
            Accesses = GetDBData("user id, prop name", UserName, DOID);
            SensitivityLevels = GetDBData("user id, prop name", UserName, DOID);
            AuthorizedPrivilegeNames = GetDBData("user id, authorized-privilege name", UserName, DOID);
            FunctionPerformeds = GetDBData("user id, function-performed", UserName, DOID);

            var tem = new List<User>();
            foreach (var user in userIds)
            {
                string roleid = "", title = "", ns = "fedramp", external = "", access = "", sensitivity = "", privilegeName = "", functionPerformed = "";

                for (int i = 0; i < roleIds.Count; i++)
                {
                    if (user[1] + "<role-id>" == roleIds[i][1])
                    {
                        roleid = roleIds[i][0];
                        break;
                    }
                }

                for (int i = 0; i < titles.Count; i++)
                {
                    if (user[1] + "<title>" == titles[i][1])
                    {
                        title = titles[i][0];
                        break;
                    }
                }

                for (int i = 0; i < Externals.Count; i++)
                {
                    if (user[1] + "<prop  name=\"external\" ns=\"fedramp\">" == Externals[i][1])
                    {
                        external = Externals[i][0];
                        break;
                    }
                }

                for (int i = 0; i < Accesses.Count; i++)
                {
                    if (user[1] + "<prop  name=\"access\" ns=\"fedramp\">" == Accesses[i][1])
                    {
                        access = Accesses[i][0];
                        break;
                    }
                }

                for (int i = 0; i < SensitivityLevels.Count; i++)
                {
                    if (user[1] + "<prop  name=\"sensitivity-level\" ns=\"fedramp\">" == SensitivityLevels[i][1])
                    {
                        sensitivity = SensitivityLevels[i][0];
                        break;
                    }
                }

                for (int i = 0; i < AuthorizedPrivilegeNames.Count; i++)
                {
                    var ths = string.Format("<authorized-privilege  name=\"{0}\">", AuthorizedPrivilegeNames[i][2]);
                    if (user[1] + ths == AuthorizedPrivilegeNames[i][1])
                    {
                        privilegeName = AuthorizedPrivilegeNames[i][2];
                        functionPerformed = FunctionPerformeds[i][0];
                        break;
                    }
                }



                var take = new User
                {
                    ID = user[0],
                    RoleID = roleid,
                    Title = title,
                    NS = ns,
                    External = external,
                    Access = access,
                    SensitivityLevel = sensitivity,
                    AuthorizedPrivilegeName = privilegeName,
                    FunctionPerformed = functionPerformed
                };
                tem.Add(take);
            }

            return tem;
        }

        protected List<DocParty> FillCompanyInfo(List<DocParty> docParties)
        {
            var result = new List<DocParty>();

            foreach (var e in docParties)
            {
                var y = e;
                if (e.IsAPerson)
                {

                    foreach (var x in docParties)
                    {
                        if (x.PartyID == e.OrgID)
                        {
                            y.OrgName = x.OrgName;
                            y.ShortName = x.ShortName;
                            break;
                        }
                    }
                }
                result.Add(y);
            }

            return result;
        }

        protected List<Resource> GetResourceFiles()
        {
            var result = new List<Resource>();
            var sources = GetHeaderAndTag("resource id, resource id", UserName, DOID);
            var desc = GetDBData("resource id, desc", UserName, DOID);
            var filenames = GetHeaderAndTag("resource id, base64 filename", UserName, DOID);
            for (int i = 0; i < sources.Count; i++)
            {
                var res = new Resource();
                res.ID = sources[i][0];
                var nt = desc[i][1].LastIndexOf("<");
                var man = desc[i][1].Remove(nt);
                var mt = filenames[i][1].LastIndexOf("<");
                var pan = filenames[i][1].Remove(mt);
                if (sources[i][1] == man)
                {
                    res.Desc = desc[i][0];
                }

                if (sources[i][1] == pan)
                {
                    res.FileName = filenames[i][0];
                }

                result.Add(res);

            }

            return result;
        }
        protected List<Resource> GetResources()
        {
            var result = new List<Resource>();
            var sources = GetHeaderAndTag("resource id, resource id", UserName, DOID);
            var desc = GetDBData("resource id, desc", UserName, DOID);
            var filenames = GetHeaderAndTag("resource id, base64 filename", UserName, DOID);
            var filedataStreams = GetDBData("resource id, base64 filename", UserName, DOID);
            for (int i = 0; i < sources.Count; i++)
            {
                var res = new Resource();
                res.ID = sources[i][0];
                var nt = desc[i][1].LastIndexOf("<");
                var man = desc[i][1].Remove(nt);
                
                var re = filedataStreams[i][1].LastIndexOf("<");
                var pe = filedataStreams[i][1].Remove(re);

                var mt = filenames[i][1].LastIndexOf("<");
                var pan = filenames[i][1].Remove(mt);
                if (sources[i][1] == man)
                {
                    res.Desc = desc[i][0];
                }

                if (sources[i][1] == pan)
                {
                    res.FileName = filenames[i][0];
                }


                if(sources[i][1] == pe)
                {
                    res.DataStream = filedataStreams[i][0];
                }

                result.Add(res);

            }

            return result;
        }

        protected List<Citation> GetCitations()
        {
            var result = new List<Citation>();
            var cit = GetHeaderAndTag("citation id, citation id", UserName, DOID);
            var targets = GetDBData("citation id, target", UserName, DOID);
            var titles = GetDBData("citation id, title", UserName, DOID);
            for (int i = 0; i < cit.Count; i++)
            {
                var res = new Citation();
                res.ID = cit[i][0];
                var nt = targets[i][1].LastIndexOf("<");
                var man = targets[i][1].Remove(nt);
                var mt = titles[i][1].LastIndexOf("<");
                var pan = titles[i][1].Remove(mt);
                if(cit[i][1] == man)
                {
                    res.Target = targets[i][0];
                }

                if(cit[i][1] == pan)
                {
                    res.Title = titles[i][0];
                }

                result.Add(res);

            }

                return result;
        }

        

            protected List<AuthorizedPrivilege> GetAuthorizedPrivileges(string userId, string label = "<assessment-subject><local-definitions>")
            {
            var result = new List<AuthorizedPrivilege>();
            var ids = GetDBData("user id, authorized-privilege id", UserName, DOID);
            var  titles = GetDBData("user id, authorized-privilege title", UserName, DOID);
            var Desc = GetDBData("user id, authorized-privilege description", UserName, DOID);
            var Functs = GetDBData("user id, authorized-privilege function-performed", UserName, DOID);

            var tag = string.Format("{1}<user id=\"{0}\">", userId, label);

            for (int j = 0; j < ids.Count; j++)
            {
                var title = "";
                var desc = "";
                var func = "";

                if (!ids[j][1].Contains(tag))
                    continue;

                var maintag = ids[j][1];

                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i][1].Contains(maintag))
                    {
                        title = titles[i][0];
                        break;
                    }
                }
                for(int i=0; i< Desc.Count; i++)
                {
                    if(Desc[i][1].Contains(maintag))
                    {
                        desc = Desc[i][0];
                        break;
                    }
                }

                for (int i = 0; i < Functs.Count; i++)
                {
                    if (Functs[i][1].Contains(maintag))
                    {
                        func = Functs[i][0];
                        break;
                    }
                }

                var pre = new AuthorizedPrivilege
                {
                    Title = title,
                    Description = desc,
                    FunctionPerformed = func

                };
                result.Add(pre);
            }

            return result;
        }
        protected List<ResponsibleParty> GetResponsibleParties()
        {
            var result = new List<ResponsibleParty>();
            var roles = new List<List<string>>();
            var parties = new List<List<string>>();
            var uuids = GetDBData("responsible-party role-id, party-uuid", UserName, DOID);
            roles = GetHeaderAndTag("responsible-party role-id, responsible-party role-id", UserName, DOID);
            parties = GetDBData("responsible-party role-id, party-id", UserName, DOID);
            var remarks = GetDBData("responsible-party role-id, remarks", UserName, DOID);


            for (int i = 0; i < roles.Count; i++)
            {
                var respparty = new ResponsibleParty();
                for (int j = 0; j < parties.Count; j++)
                {
                    if (roles[i][1] + "<party-id>" == parties[j][1])
                    {
                        respparty.RoleID = roles[i][0];
                       
                        respparty.PartyID = parties[j][0];
                    }
                }

                for (int j = 0; j < remarks.Count; j++)
                {
                    if (roles[i][1] + "<remarks>" == remarks[j][1])
                    {
                        respparty.Remarks = remarks[j][0];
                        break;
                    }
                }

                for(int j=0; j<uuids.Count(); j++)
                {
                    if(uuids[j][1].Contains(roles[i][1]))
                    {
                        respparty.PartyUUID = uuids[j][0];
                        break;
                    }
                }

                for (int j = 0; j < parties.Count(); j++)
                {
                    if (parties[j][1].Contains(roles[i][1]))
                    {
                        respparty.PartyID = parties[j][0];
                        break;
                    }
                }

                result.Add(respparty);
            }


            return result;
        }

        protected List<string> GetPartyIds()
        {
            var result = new List<string>();
            var partyIds = GetHeaderAndTag("party id, party id", UserName, DOID);
            foreach(var part in partyIds)
            {
                result.Add(part[0]);
            }

            return result;
        }

        protected List<string> GetMethodParts(string methodId)
        {
            var aux = GetDBData(string.Format("method id, part part"), UserName, DOID);
            var res = new List<string>();
            foreach (var x in aux)
            {
                if(x[1].Contains(methodId))
                    res.Add(x[0]);
            }

            return res;
        }

        protected List<string> GetParts()
        {
            var aux = GetDBData(string.Format("part, part"), UserName, DOID);
            var res = new List<string>();
            foreach (var x in aux)
            {
               
                    res.Add(x[0]);
            }

            return res;
        }

        protected List<string> GetAuxillaries(string entityId, string entityName, string auxName)
        {
            var aux = GetDBData(string.Format("{0}, {1}", entityName, auxName), UserName, DOID);
            var res = new List<string>();
            foreach(var x in aux)
            {
                if(x[1].Contains(entityId))
                res.Add(x[0]);
            }

            return res;
        }

        protected List<Item> GetAssessors(string entityId, string entityName)
        {
            var a = string.Format("{0}, assessor", entityName);
            var b = string.Format("{0}, assessor party uuid", entityName);
            var assessors = GetDBData(a, UserName, DOID);
            var partyuuids = GetDBData(b, UserName, DOID);

            var res = new List<Item>();
            foreach (var ass in assessors)
            {
                var tem = new Item();
                
                if (!ass[1].Contains(entityId))
                    continue;

                foreach (var uuid in partyuuids)
                {
                    if (uuid[1].Contains(ass[1]) && uuid[1].Contains(entityId))
                    {
                        tem.XPath = uuid[0];
                        tem.Value = ass[0] ;
                        break;
                    }
                }
                res.Add(tem);
            }
            return res;
        }

        protected List<Remediation> GetRemediations(string riskid)
        {
            var ids = GetDBData("risk id, remediation id", UserName, DOID);
            var uuids = GetDBData("risk id, remediation uuid", UserName, DOID);
            var types = GetDBData("risk id, remediation type", UserName, DOID);
            var descs = GetDBData("risk id, remediation desc", UserName, DOID);
            var rems = GetDBData("risk id, remediation remarks", UserName, DOID);
            var titles = GetDBData("risk id, remediation title", UserName, DOID);

            var res = new List<Remediation>();
            foreach (var subject in ids)
            {
                if (!subject[1].Contains(riskid))
                    continue;

                var tem = new Remediation();
                tem.ID = subject[0];
                tem.Props = GetProps(tem.ID, "remediation id", subject[1]);
                tem.Annotations= GetAnnotations(tem.ID, "remediation id", subject[1]);
                tem.RemediationOrigins= GetOrigins(tem.ID, "remediation id", "origin");
               
                foreach (var lass in uuids)
                {
                    if (lass[1].Contains(subject[1]) && lass[1].Contains(riskid))
                    {
                        tem.UUID = lass[0];
                        break;
                    }
                }

                foreach (var typ in types)
                {
                    if (typ[1].Contains(subject[1]) && typ[1].Contains(riskid))
                    {
                        tem.Type = typ[0];
                        break;
                    }
                }

                foreach (var value in descs)
                {
                    if (value[1].Contains(subject[1]) && value[1].Contains(riskid))
                    {
                        tem.Description = value[0];
                        break;
                    }
                }
                foreach (var rem in rems)
                {
                    if (rem[1].Contains(subject[1]) && rem[1].Contains(riskid))
                    {
                        tem.Remarks = rem[0];
                        break;
                    }
                }

                foreach (var title in titles)
                {
                    if (title[1].Contains(subject[1]) && title[1].Contains(riskid))
                    {
                        tem.Title = title[0];
                        break;
                    }
                }

                res.Add(tem);
            }
            return res;

        }


        protected List<MitigatingFactor> GetMitigatingFactors( string riskid)
        {
            var ids = GetDBData("risk id, mitigating-factor id", UserName, DOID);
            var uuids = GetDBData("risk id, mitigating-factor uuid", UserName, DOID);
            var impuuids = GetDBData("risk id, mitigating-factor implementation-uuid", UserName, DOID);
            var descs = GetDBData("risk id, mitigating-factor desc", UserName, DOID);

            var res = new List<MitigatingFactor>();
            foreach (var subject in ids)
            {
                if (!subject[1].Contains(riskid))
                    continue;

                var tem = new MitigatingFactor();
                tem.ID = subject[0];

                var refs = GetSubjectReferences(tem.ID, "mitigating-factor id");
                tem.SubjectReferences = refs;
                foreach (var lass in uuids)
                {
                    if (lass[1].Contains(subject[1]) && lass[1].Contains(riskid))
                    {
                        tem.UUID = lass[0];
                        break;
                    }
                }

                foreach (var sys in impuuids)
                {
                    if (sys[1].Contains(subject[1]) && sys[1].Contains(riskid))
                    {
                        tem.ImplementationUUID = sys[0];
                        break;
                    }
                }

                foreach (var value in descs)
                {
                    if (value[1].Contains(subject[1]) && value[1].Contains(riskid))
                    {
                        tem.Description = value[0];
                        break;
                    }
                }

                res.Add(tem);
            }
            return res;

        }

        protected List<SubjectReference> GetSubjectReferences(string entityId, string entityName)
        {         
            var subjects = GetDBData(string.Format("{0}, subject-reference", entityName), UserName, DOID);
            var guids = GetDBData(string.Format("{0}, subject-reference uuid-ref", entityName), UserName, DOID);
            var types = GetDBData(string.Format("{0}, subject-reference type", entityName), UserName, DOID);

            var res = new List<SubjectReference>();
            foreach (var subject in subjects)
            {
                if (!subject[1].Contains(entityId))
                    continue;

                var tem = new SubjectReference();
                tem.Value = subject[0];


                foreach (var type in types)
                {
                    if (type[1].Contains(subject[1]) && type[1].Contains(entityId))
                    {
                        tem.Type = type[0];
                        break;
                    }
                }

                foreach (var guid in guids)
                {
                    if (guid[1].Contains(subject[1]) && guid[1].Contains(entityId))
                    {
                        tem.UUIDRef = guid[0];
                        break;
                    }
                }

                res.Add(tem);
            }
            return res;
        }

        protected List<RiskMetric> GetRiskMetrics(string entityId, string entityName)
        {
       
            var names = GetDBData(string.Format("{0}, risk-metric name", entityName), UserName, DOID);
            var classes = GetDBData(string.Format("{0}, risk-metric class", entityName), UserName, DOID);
            var systems = GetDBData(string.Format("{0}, risk-metric system", entityName), UserName, DOID);
            var values = GetDBData(string.Format("{0}, risk-metric", entityName), UserName, DOID);

            var res = new List<RiskMetric>();
            foreach (var subject in names)
            {
                if (!subject[1].Contains(entityId))
                    continue;

                var tem = new RiskMetric();
                tem.Name = subject[0];


                foreach (var lass in classes)
                {
                    if (lass[1].Contains(subject[1]) && lass[1].Contains(entityId))
                    {
                        tem.Class = lass[0];
                        break;
                    }
                }

                foreach (var sys in systems)
                {
                    if (sys[1].Contains(subject[1]) && sys[1].Contains(entityId))
                    {
                        tem.System = sys[0];
                        break;
                    }
                }

                foreach (var value in values)
                {
                    if (value[1].Contains(subject[1]) && value[1].Contains(entityId))
                    {
                        tem.Value = value[0];
                        break;
                    }
                }

                res.Add(tem);
            }
            return res;
        }

        protected List<RelevantEvidence> GetEvidences(string entityId)
        {

            var subjects = GetDBData("observation id, relevant-evidence id", UserName, DOID);
            var descs = GetDBData("observation id, relevant-evidence desc", UserName, DOID);
            var hrefs = GetDBData("observation id, relevant-evidence href", UserName, DOID);
            var remarks = GetDBData("observation id, relevant-evidence remarks", UserName, DOID);

            var res = new List<RelevantEvidence>();
            foreach (var subject in subjects)
            {
                if (!subject[1].Contains(entityId))
                    continue;

                var props = GetProps(entityId, "observation id", subject[1]);
                var anns = GetAnnotations(entityId, "observation id", subject[1]);
                var tem = new RelevantEvidence();
                tem.ID = subject[0];
                tem.Props = props;
                tem.Annotations = anns;

                foreach (var desc in descs)
                {
                    if (desc[1].Contains(subject[1]) && desc[1].Contains(entityId))
                    {
                        tem.Description = desc[0];
                        break;
                    }
                }

                foreach (var rem in remarks)
                {
                    if (rem[1].Contains(subject[1]) && rem[1].Contains(entityId))
                    {
                        tem.Remarks = rem[0];
                        break;
                    }
                }

                foreach (var href in hrefs)
                {
                    if (href[1].Contains(subject[1]) && href[1].Contains(entityId))
                    {
                        tem.HREF = href[0];
                        break;
                    }
                }

                res.Add(tem);
            }
            return res;
        }


        protected List<RemediationOrigin> GetOrigins(string entityId, string entityName, string label)
        {

            var subjects = GetDBData(string.Format("{0}, {1}", entityName, label), UserName, DOID);
            var guids = GetDBData(string.Format("{0}, {1} uuid-ref", entityName, label), UserName, DOID);
            var types = GetDBData(string.Format("{0}, {1} type", entityName, label), UserName, DOID);

            var res = new List<RemediationOrigin>();
            foreach (var subject in subjects)
            {
                if (!subject[1].Contains(entityId))
                    continue;

                var tem = new RemediationOrigin();
                tem.Value = subject[0];


                foreach (var type in types)
                {
                    if (type[1].Contains(subject[1]) && type[1].Contains(entityId))
                    {
                        tem.Type = type[0];
                        break;
                    }
                }

                foreach (var guid in guids)
                {
                    if (guid[1].Contains(subject[1]) && guid[1].Contains(entityId))
                    {
                        tem.UUIDRef = guid[0];
                        break;
                    }
                }

                res.Add(tem);
            }
            return res;
        }

        protected List<SystemID> GetSystemIDs()
        {
          
            var ids = GetDBData("system id, system id", UserName, DOID);
            var types = GetDBData("system id, type", UserName, DOID);

            var res = new List<SystemID>();
            foreach (var id in ids)
            {
                var tem = new SystemID();
                tem.Identification = id[0];
                
                foreach (var type in types)
                {
                    if (type[1].Contains(id[1]))
                    {
                        tem.Type = type[0];
                        break;
                    }
                }
                res.Add(tem);
            }
            return res;
        }

        protected List<Phone> GetPhones(string entityId, string entityName)
        { 
            var a = string.Format("{0}, phone", entityName);
            var b = string.Format("{0}, phone type", entityName);
            var phones = GetDBData(a, UserName, DOID);
            var types = GetDBData(b, UserName, DOID);

            var res = new List<Phone>();
            foreach(var phone in phones)
            {
                var tem = new Phone();
                tem.Number = phone[0];
                if (!phone[1].Contains(entityId))
                    continue;

                foreach(var type in types)
                {
                    if(type[1].Contains(phone[1]) && type[1].Contains(entityId))
                    {
                        tem.Type = type[0];
                        break;
                    }
                }
                res.Add(tem);
            }
            return res;
        }

        protected List<DocAddress> GetAddresses(string partyId)
        {
            var result = new List<DocAddress>();
            var addressIds = GetHeaderAndTag("party id, address id", UserName, DOID);
            var addressTypes = GetDBData("party id, address type", UserName, DOID);
            var addrLines = GetDBData("party id, addr-line", UserName, DOID);
            var cities = GetDBData("party id, city", UserName, DOID);
            var states = GetDBData("party id, state", UserName, DOID);
            var postalCodes = GetDBData("party id, postal-code", UserName, DOID);
            var countries = GetDBData("party id, country", UserName, DOID);

         
            foreach (var add in addressIds)
            {
                string addId, addType = "", line1 = "", line2 = "", state ="", city="", postalcode="", country="";
                addId = add[0];
                var locTag = add[1];
                if (!locTag.Contains(partyId))
                    continue;

                foreach (var type in addressTypes)
                {
                    if(type[1].Contains(add[1]))
                    {
                        addType = type[0];
                        break;
                    }
                }
          
                for (int i = 0; i < addrLines.Count; i++)
                {
                    if (addrLines[i][1].Contains(locTag) && addrLines[i][2] == "addr-line")
                    {
                        line1 = addrLines[i][0];
                        break;
                    }
                }
                for (int i = 0; i < addrLines.Count; i++)
                {
                    if (addrLines[i][1].Contains(locTag) && addrLines[i][2] == "addr-line2")
                    {
                        line2 = addrLines[i][0];
                        break;
                    }
                }
                for (int i = 0; i < cities.Count; i++)
                {
                    if (cities[i][1].Contains(locTag))
                    {
                        city = cities[i][0];
                        break;
                    }
                }
                for (int i = 0; i < states.Count; i++)
                {
                    if (states[i][1].Contains(locTag))
                    {
                        state = states[i][0];
                        break;
                    }
                }

                for (int i = 0; i < postalCodes.Count; i++)
                {
                    if (postalCodes[i][1].Contains(locTag))
                    {
                        postalcode = postalCodes[i][0];
                        break;
                    }
                }

                for (int i = 0; i < countries.Count; i++)
                {
                    if (countries[i][1].Contains(locTag))
                    {
                        country = countries[i][0];
                        break;
                    }
                }

                var newAddress = new DocAddress
                { 
                    AddressID = addId,
                    AddressType = addType,
                    AddressLine1 = line1,
                    AddressLine2 = line2,
                    City = city,
                    State = state,
                    PostalCode = postalcode,
                    Country = country
                };

                result.Add(newAddress);
            }

            return result;
        }
        protected List<DocParty> GetMainParties()
        {
            var result = new List<DocParty>();

            var partyIds = GetHeaderAndTag("party id, party id", UserName, DOID);
         
            var names = GetDBData("party id, name", UserName, DOID);
            var shortNames = GetDBData("party id, short-name", UserName, DOID);
            var partyTypes = GetDBData("party id, type", UserName, DOID);
            var types = GetDBData("party id, external-id type", UserName, DOID);
            var externalIds = GetDBData("party id, external-id", UserName, DOID);
            var remarks = GetDBData("party id, remarks", UserName, DOID);
            var uuids = GetDBData("party id, uuid", UserName, DOID);

            var tem = new List<DocParty>();
            foreach (var party in partyIds)
            {
                string partyId, name = "", shortname ="", type = "", externalid = "", uuid="", remark = "", parttype="";
                partyId = party[0];
                var props = GetProps(partyId, "party id");
                var anns = GetAnnotations(partyId, "party id");
                var links = GetLinks(partyId, "party id");
                var locs = GetAuxillaries(partyId, "party id", "location uuid");
                var members = GetAuxillaries(partyId, "party id", "member-of-organization");
                var emails = GetAuxillaries(partyId, "party id", "email");
                var phones = GetPhones(partyId, "party id");
                var addresses = GetAddresses(partyId);

                var tag = party[1];
                for (int i = 0; i < names.Count; i++)
                {
                    if ( names[i][1].Contains(tag))
                    {
                        name = names[i][0];
                        break;
                    }
                }

        
                for (int i = 0; i < shortNames.Count; i++)
                {
                    if (shortNames[i][1].Contains(tag))
                    {
                        shortname = shortNames[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (party[1] + "<remarks>" == remarks[i][1])
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                for (int i = 0; i < uuids.Count; i++)
                {
                    if (uuids[i][1].Contains(tag))
                    {
                        uuid = uuids[i][0];
                        break;
                    }
                }

                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i][1].Contains(tag))
                    {
                        type = types[i][0];
                        break;
                    }
                }

                for (int i = 0; i < partyTypes.Count; i++)
                {
                    if (partyTypes[i][1].Contains(tag))
                    {
                        parttype = partyTypes[i][0];
                        break;
                    }
                }
                for (int i = 0; i < externalIds.Count; i++)
                {
                    if (externalIds[i][1].Contains(tag))
                    {
                        externalid = externalIds[i][0];
                        break;
                    }
                }
                var orm = partyId.Replace("party-", "");
                int rank = int.Parse(orm);
                var take = new DocParty
                {
                    Rank = rank,
                    PartyID = partyId,
                    Name = name,
                    ShortName = shortname,
                    UUID = uuid,
                    PartyType= parttype,
                    ExternalType = type,
                    ExternalID = externalid,
                    Remarks = remark,
                    Props = props,
                    Annotations = anns,
                    Links = links,
                    Phones = phones,
                    Emails = emails,
                    Addresses =  addresses,
                    MemberOfOrg =  members,
                    LocationUUIDs = locs
                };
                tem.Add(take);
            }

            var main = tem.OrderBy(x => x.Rank).ToList();
            return main;
        }

   
        protected List<DocParty> GetFormviewParties()
        {
            
            var partyIds = GetHeaderAndTag("party id, party id", UserName, DOID);
            var names = GetDBData("party id, name", UserName, DOID);
            var shortNames = GetDBData("party id, short-name", UserName, DOID);
            var partTypes = GetDBData("party id, type", UserName, DOID);
            var types = GetDBData("party id, external-id type", UserName, DOID);
            var externalIds = GetDBData("party id, external-id", UserName, DOID);
            var remarks = GetDBData("party id, remarks", UserName, DOID);
            var uuids = GetDBData("party id, uuid", UserName, DOID);

            var tem = new List<DocParty>();
            foreach (var party in partyIds)
            {
                string partyId, name = "", shortname = "", type = "", externalid = "", uuid = "", remark = "", partT="";
                partyId = party[0];
               

                var tag = party[1];
                for (int i = 0; i < names.Count; i++)
                {
                    if (names[i][1].Contains(tag))
                    {
                        name = names[i][0];
                        break;
                    }
                }


                for (int i = 0; i < shortNames.Count; i++)
                {
                    if (shortNames[i][1].Contains(tag))
                    {
                        shortname = shortNames[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (party[1] + "<remarks>" == remarks[i][1])
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                for (int i = 0; i < uuids.Count; i++)
                {
                    if (uuids[i][1].Contains(tag))
                    {
                        uuid = uuids[i][0];
                        break;
                    }
                }

                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i][1].Contains(tag))
                    {
                        type = types[i][0];
                        break;
                    }
                }

                for (int i = 0; i < partTypes.Count; i++)
                {
                    if (partTypes[i][1].Contains(tag))
                    {
                        partT = partTypes[i][0];
                        break;
                    }
                }

                for (int i = 0; i < externalIds.Count; i++)
                {
                    if (externalIds[i][1].Contains(tag))
                    {
                        externalid = externalIds[i][0];
                        break;
                    }
                }
                var orm = partyId.Replace("party-", "");
                int rank = int.Parse(orm);
                var take = new DocParty
                {
                    Rank =  rank,
                    PartyID = partyId,
                    Name = name,
                    ShortName = shortname,
                    UUID = uuid,
                    ExternalType = type,
                    ExternalID = externalid,
                    Remarks = remark,
                    PartyType=partT
                    
                };
                tem.Add(take);
            }
            var main = tem.OrderBy(x => x.Rank).ToList();
            return main;
        }

        protected List<DocParty> GetAllPartyInfo()
        {
            var result = new List<DocParty>();
            var partyIds = new List<List<string>>();
            var orgIds = new List<List<string>>();
            var personNames = new List<List<string>>();
            var orgNames = new List<List<string>>();
            var shortNames = new List<List<string>>();
            var addrLines = new List<List<string>>();
            var cities = new List<List<string>>();
            var states = new List<List<string>>();
            var postalCodes = new List<List<string>>();
            var countries = new List<List<string>>();
            var remarks = new List<List<string>>();
            var locations = new List<List<string>>();


            partyIds = GetHeaderAndTag("party id, party id", UserName, DOID);
            orgIds = GetDBData("party id, org-id", UserName, DOID);
            var uuids = GetDBData("party id, uuid", UserName, DOID);
            personNames = GetDBData("party id, person-name", UserName, DOID);
            orgNames = GetDBData("party id, org-name", UserName, DOID);
            shortNames = GetDBData("party id, short-name", UserName, DOID);
            addrLines = GetDBData("party id, addr-line", UserName, DOID);
          // locations = GetDBData("party id, location-id", UserName, DOID);
            cities = GetDBData("party id, city", UserName, DOID);
            states = GetDBData("party id, state", UserName, DOID);
            postalCodes = GetDBData("party id, postal-code", UserName, DOID);
            countries = GetDBData("party id, country", UserName, DOID);
            var phones = GetDBData("party id, phone", UserName, DOID);
            var emails = GetDBData("party id, email", UserName, DOID);
            remarks = GetDBData("party id, remarks", UserName, DOID);

            var tem = new List<DocParty>();
            foreach (var party in partyIds)
            {
                string  uuid = "", partyId ="", orgname = "", personname = "", shortname = "", orgid = "", addrline1 = "", addrline2 = "", city = "", state = "", postalcode = "", country = "", remark = "", phone = "", email = "";
                partyId = party[0];
                for (int i = 0; i < orgNames.Count; i++)
                {
                    if (orgNames[i][1].Contains(party[1]))
                    {
                        orgname = orgNames[i][0];
                        break;
                    }
                }

                for (int i = 0; i < orgIds.Count; i++)
                {
                    if (orgIds[i][1].Contains(party[1]) || orgIds[i][1].Contains(party[1]))
                    {
                        orgid = orgIds[i][0];
                        break;
                    }
                }

                for (int i = 0; i < shortNames.Count; i++)
                {
                    if (shortNames[i][1].Contains(party[1]))
                    {
                        shortname = shortNames[i][0];
                        break;
                    }
                }

                for (int i = 0; i < uuids.Count; i++)
                {
                    if (uuids[i][1].Contains(party[1]))
                    {
                        uuid = uuids[i][0];
                        break;
                    }
                }

                for (int i = 0; i < personNames.Count; i++)
                {
                    if (personNames[i][1].Contains(party[1]))
                    {
                        personname = personNames[i][0];
                        break;
                    }
                }

                for (int i = 0; i < addrLines.Count; i++)
                {
                    if ((addrLines[i][1].Contains(party[1])) || ( addrLines[i][1].Contains(party[1])))
                    {
                        addrline1 = addrLines[i][0];
                        break;
                    }
                }

                for (int i = 0; i < addrLines.Count; i++)
                {
                    if (addrLines[i][2] == "addr-line2" && (( addrLines[i][1].Contains(party[1])) || ( addrLines[i][1].Contains(party[1]))))
                    {
                        addrline2 = addrLines[i][0];
                        break;
                    }
                }

                for (int i = 0; i < cities.Count; i++)
                {
                    if ((cities[i][1].Contains(party[1])) || (cities[i][1].Contains(party[1])))
                    {
                        city = cities[i][0];
                        break;
                    }
                }

                for (int i = 0; i < states.Count; i++)
                {
                    if ((states[i][1].Contains(party[1])) || (states[i][1].Contains(party[1])))
                    {
                        state = states[i][0];
                        break;
                    }
                }

                for (int i = 0; i < postalCodes.Count; i++)
                {
                    if (( postalCodes[i][1].Contains(party[1])) || ( postalCodes[i][1].Contains(party[1])))
                    {
                        postalcode = postalCodes[i][0];
                        break;
                    }
                }
                for (int i = 0; i < countries.Count; i++)
                {
                    if ((countries[i][1].Contains(party[1])) || (countries[i][1].Contains(party[1])))
                    {
                        country = countries[i][0];
                        break;
                    }
                }

                for (int i = 0; i < phones.Count; i++)
                {
                    if (( phones[i][1].Contains(party[1])) || ( phones[i][1].Contains(party[1])))
                    {
                        phone = phones[i][0];
                        break;
                    }
                }

                for (int i = 0; i < emails.Count; i++)
                {
                    if (( emails[i][1].Contains(party[1])) || ( emails[i][1].Contains(party[1])))
                    {
                        email = emails[i][0];
                        break;
                    }
                }

               

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (party[1] + "<org><remarks>" == remarks[i][1])
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                var take = new DocParty
                {
                    PartyID = partyId,
                    OrgName = orgname,
                    OrgID = orgid,
                    ShortName = shortname,
                    PersonName = personname,
                    AddressLine1 = addrline1,
                    AddressLine2 = addrline2,
                    City = city,
                    State = state,
                    PostalCode = postalcode,
                    Country = country,
                    Phone = phone,
                    Email = email,
                    Remarks = remark,
                  
                    UUID = uuid
                };
                tem.Add(take);
            }

            return tem;
        }

        protected List<DocParty> GetParties()
        {
            var result = new List<DocParty>();
            var partyIds = new List<List<string>>();
            var orgIds = new List<List<string>>();
            var personNames = new List<List<string>>();
            var orgNames = new List<List<string>>();
            var shortNames = new List<List<string>>();
            var addrLines = new List<List<string>>();
            var cities = new List<List<string>>();
            var states = new List<List<string>>();
            var postalCodes = new List<List<string>>();
            var countries = new List<List<string>>();
            var remarks = new List<List<string>>();

           
            partyIds = GetHeaderAndTag("party id, party id", UserName, DOID);
            orgIds = GetDBData("party id, org-id", UserName, DOID);
            personNames = GetDBData("party id, person-name", UserName, DOID);
            orgNames = GetDBData("party id, org-name", UserName, DOID);
            shortNames = GetDBData("party id, short-name", UserName, DOID);
            addrLines = GetDBData("party id, addr-line", UserName, DOID);
            
            cities = GetDBData("party id, city", UserName, DOID);
            states = GetDBData("party id, state", UserName, DOID);
            postalCodes = GetDBData("party id, postal-code", UserName, DOID);
            countries = GetDBData("party id, country", UserName, DOID);
            var phones = GetDBData("party id, phone", UserName, DOID);
            var emails = GetDBData("party id, email", UserName, DOID);
            remarks = GetDBData("party id, remarks", UserName, DOID);

            var tem = new List<DocParty>();
            foreach (var party in partyIds)
            {
                string partyId, orgname = "", personname = "", shortname = "", orgid = "", addrline1 = "", addrline2 = "", city = "", state = "", postalcode = "", country = "", remark = "", phone = "", email = "";
                partyId = party[0];
                for (int i = 0; i < orgNames.Count; i++)
                {
                    if (party[1] + "<org><org-name>" == orgNames[i][1])
                    {
                        orgname = orgNames[i][0];
                        break;
                    }
                }

                for (int i = 0; i < orgIds.Count; i++)
                {
                    if (party[1] + "<org><org-id>" == orgIds[i][1] || party[1] + "<person><org-id>" == orgIds[i][1])
                    {
                        orgid = orgIds[i][0];
                        break;
                    }
                }

                for (int i = 0; i < shortNames.Count; i++)
                {
                    if (party[1] + "<org><short-name>" == shortNames[i][1])
                    {
                        shortname = shortNames[i][0];
                        break;
                    }
                }

                for (int i = 0; i < personNames.Count; i++)
                {
                    if (party[1] + "<person><person-name>" == personNames[i][1])
                    {
                        personname = personNames[i][0];
                        break;
                    }
                }

                for (int i = 0; i < addrLines.Count; i++)
                {
                    if ((party[1] + "<org><address><addr-line>" == addrLines[i][1]) || (party[1] + "<person><address><addr-line>" == addrLines[i][1]))
                    {
                        addrline1 = addrLines[i][0];
                        break;
                    }
                }

                for (int i = 0; i < addrLines.Count; i++)
                {
                    if (addrLines[i][2] =="addr-line2" &&  ((party[1] + "<org><address><addr-line>" == addrLines[i][1]) || (party[1] + "<person><address><addr-line>" == addrLines[i][1])))
                    {
                        addrline2 = addrLines[i][0];
                        break;
                    }
                }

                for (int i = 0; i < cities.Count; i++)
                {
                    if ((party[1] + "<org><address><city>" == cities[i][1]) || (party[1] + "<person><address><city>" == cities[i][1]))
                    {
                        city = cities[i][0];
                        break;
                    }
                }

                for (int i = 0; i < states.Count; i++)
                {
                    if ((party[1] + "<org><address><state>" == states[i][1]) || (party[1] + "<person><address><state>" == states[i][1]))
                    {
                        state = states[i][0];
                        break;
                    }
                }

                for (int i = 0; i < postalCodes.Count; i++)
                {
                    if ((party[1] + "<org><address><postal-code>" == postalCodes[i][1]) || (party[1] + "<person><address><postal-code>" == postalCodes[i][1]))
                    {
                        postalcode = postalCodes[i][0];
                        break;
                    }
                }
                for (int i = 0; i < countries.Count; i++)
                {
                    if ((party[1] + "<org><address><country>" == countries[i][1]) || (party[1] + "<person><address><country>" == countries[i][1]))
                    {
                        country = countries[i][0];
                        break;
                    }
                }

                for (int i = 0; i < phones.Count; i++)
                {
                    if ((party[1] + "<org><address><phone>" == phones[i][1]) || (party[1] + "<person><address><phone>" == phones[i][1]))
                    {
                        phone = phones[i][0];
                        break;
                    }
                }

                for (int i = 0; i < emails.Count; i++)
                {
                    if ((party[1] + "<org><address><email>" == emails[i][1]) || (party[1] + "<person><address><email>" == emails[i][1]))
                    {
                        email = emails[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (party[1] + "<org><remarks>" == remarks[i][1])
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                var take = new DocParty
                {
                    PartyID = partyId,
                    OrgName = orgname,
                    OrgID = orgid,
                    ShortName = shortname,
                    PersonName = personname,
                    AddressLine1 = addrline1,
                    AddressLine2 = addrline2,
                    City = city,
                    State = state,
                    PostalCode = postalcode,
                    Country = country,
                    Phone = phone,
                    Email = email,
                    Remarks = remark
                };
                tem.Add(take);
            }

            return tem;
        }

        public List<string> GetAllHeaders(string eltdesc, string userId, int doid)
        {
            var res = new List<string>();

            SqlParameter[] oParams = new SqlParameter[3];
            oParams[0] = new SqlParameter("UserName", SqlDbType.VarChar, 255);
            oParams[0].Value = userId;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = doid;
            oParams[2] = new SqlParameter("ElementDesc", SqlDbType.NVarChar, 255);
            oParams[2].Value = eltdesc;



            DataSet ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocHeader_GetAllHeaderGivenDesc]", CommandType.StoredProcedure, oParams);


            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                   
                    res.Add(ds.Tables[0].Rows[i].ItemArray[0].ToString());
                  
                }
            }


            return res;
        }


        public List<List<string>> GetHeaderAndTag(string eltdesc, string userId, int doid)
        {
            var res = new List<List<string>>();

            SqlParameter[] oParams = new SqlParameter[3];
            oParams[0] = new SqlParameter("UserName", SqlDbType.VarChar, 255);
            oParams[0].Value = userId;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = doid;
            oParams[2] = new SqlParameter("ElementDesc", SqlDbType.NVarChar, 255);
            oParams[2].Value = eltdesc;



            DataSet ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocHeader_GetHeaderGivenDesc]", CommandType.StoredProcedure, oParams);


            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var temp = new List<string>();
                    temp.Add(ds.Tables[0].Rows[i].ItemArray[3].ToString());
                    temp.Add(ds.Tables[0].Rows[i].ItemArray[4].ToString());
                    res.Add(temp);
                }
            }


            return res;
        }


        public List<List<string>> GetDBDataGivenTag(string eltTag, string username, int doid)
        {
            var res = new List<List<string>>();

            SqlParameter[] oParams = new SqlParameter[3];
            oParams[0] = new SqlParameter("UserName", SqlDbType.VarChar, 255);
            oParams[0].Value = username;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = doid;
            oParams[2] = new SqlParameter("ElementTag", SqlDbType.VarChar, 255);
            oParams[2].Value = eltTag;



            DataSet ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocElement_GetDetailGivenTag]", CommandType.StoredProcedure, oParams);


            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var temp = new List<string>();

                    temp.Add(ds.Tables[0].Rows[i].ItemArray[0].ToString());
                    
                    res.Add(temp);
                }
            }


            return res;
        }



        public List<List<string>> GetDBData(string eltdesc, string userId, int doid)
        {
            var res = new List<List<string>>();

            SqlParameter[] oParams = new SqlParameter[3];
            oParams[0] = new SqlParameter("UserName", SqlDbType.VarChar, 255);
            oParams[0].Value = userId;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = doid;
            oParams[2] = new SqlParameter("ElementDesc", SqlDbType.NVarChar, 255);
            oParams[2].Value = eltdesc;



            DataSet ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocElement_GetDetailAndTag]", CommandType.StoredProcedure, oParams);

          
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var temp = new List<string>();

                    temp.Add( ds.Tables[0].Rows[i].ItemArray[0].ToString());
                    temp.Add(ds.Tables[0].Rows[i].ItemArray[1].ToString());
                    temp.Add(ds.Tables[0].Rows[i].ItemArray[2].ToString());
                    res.Add(temp);
                }
            }


            return res;
        }


      


        protected string GetData(string eltName, string uid, int doid)
        {
            string result = "";
         

            var deidList = GetDEID(doid, uid, eltName, eltName.GetType(), 1);
            if (deidList.Count > 0)
            {
                var deid = deidList.FirstOrDefault();
                result = GetElementDetail(deid);
               
            }


            return result; ;
        }

        public List<List<string>> GetAllSystemInfo()
        {
            var res = new List<List<string>>();


            DataSet ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_System_Get_Details]", CommandType.StoredProcedure);

          
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var x = new List<string>();
                    for(int j=0; j<ds.Tables[0].Rows[i].ItemArray.Length; j++)
                    {
                        
                        x.Add(ds.Tables[0].Rows[i].ItemArray[j].ToString());
                    }
                    res.Add(x);
                }
            }
            return res;

        }
        public List<DocInfoType> GetSystemInfoType()
        {
            var result = new List<DocInfoType>();
            var InfoTypeIds = new List<List<string>>();
            var InfoTypeNames = new List<List<string>>();
            var ImpactBase = new List<List<string>>();
            var ImpactSelected = new List<List<string>>();
            var InfoTypeNistIds = new List<List<string>>();

            InfoTypeIds = GetHeaderAndTag("information-type name, information-type id", UserName, DOID);
            InfoTypeNames = GetHeaderAndTag("information-type name, information-type name", UserName, DOID);
            ImpactBase = GetDBData("information-type name, base", UserName, DOID);
            ImpactSelected = GetDBData("information-type name, selected", UserName, DOID);
            InfoTypeNistIds = GetDBData("information-type name, information-type-id system", UserName, DOID);
            var descs = GetDBData("information-type name, information-type-id desc", UserName, DOID);
            int rank = -1;
            var tem = new List<DocInfoType>();
            foreach (var info in InfoTypeNames)
            {
                string infoId = "", Desc="", NistId = "", confbase = "", confselected = "", intebase = "", inteselected = "", availbase = "", availselected = "";
                for (int i = 0; i < InfoTypeIds.Count; i++)
                {
                    if (info[1] == InfoTypeIds[i][1])
                    {
                        infoId = InfoTypeIds[i][0];
                        rank = i;
                        break;
                    }
                }
                if (rank >= 0)
                {
                    var me = InfoTypeIds[rank][1];
                    for (int i = 0; i < InfoTypeNistIds.Count; i++)
                    {
                        var man = me + "<information-type-id  system=\"nist\">";
                        if (man == InfoTypeNistIds[i][1])
                        {
                            NistId = InfoTypeNistIds[i][0];
                            break;
                        }
                    }

                    for (int i = 0; i < descs.Count; i++)
                    {
                        var man = me + "<desc>";
                        if (man == descs[i][1])
                        {
                            Desc = descs[i][0];
                            break;
                        }
                    }

                    for (int i = 0; i < ImpactBase.Count; i++)
                    {
                        if (InfoTypeIds[rank][1] + "<confidentiality-impact><base>" == ImpactBase[i][1])
                        {
                            confbase = ImpactBase[i][0];
                        }

                        if (InfoTypeIds[rank][1] + "<integrity-impact><base>" == ImpactBase[i][1])
                        {
                            intebase = ImpactBase[i][0];
                        }

                        if (InfoTypeIds[rank][1] + "<availability-impact><base>" == ImpactBase[i][1])
                        {
                            availbase = ImpactBase[i][0];
                        }

                    }

                    for (int i = 0; i < ImpactSelected.Count; i++)
                    {
                        if (InfoTypeIds[rank][1] + "<confidentiality-impact><selected>" == ImpactSelected[i][1])
                        {
                            confselected = ImpactSelected[i][0];
                        }

                        if (InfoTypeIds[rank][1] + "<integrity-impact><selected>" == ImpactSelected[i][1])
                        {
                            inteselected = ImpactSelected[i][0];
                        }

                        if (InfoTypeIds[rank][1] + "<availability-impact><selected>" == ImpactSelected[i][1])
                        {
                            availselected = ImpactSelected[i][0];
                        }

                    }

                    var take = new DocInfoType
                    {
                        Name = info[0],
                        InfoId = infoId,
                        InfoTypeSytemId = NistId,
                        Description = Desc,
                        InfoTypeSytemName = "nist",
                        ConfidentialityImpactBase = confbase,
                        ConfidentialityImpactSelected = confselected,
                        IntegrityImpactBase = intebase,
                        IntegrityImpactSelected = inteselected,
                        AvailabilityImpactBase = availbase,
                        AvailabilityImpactSelected = availselected


                    };
                    tem.Add(take);
                }
            }

            return tem;
        }

        protected List<string> GetRoleIds()
        {
            var result = new List<string>();
            var roles = GetHeaderAndTag("role id, role id", UserName, DOID);
            for (int i = 0; i < roles.Count; i++)
            {
                var roleid = roles[i][0];
                result.Add(roleid);
            }
             
            return result;
        }

        protected List<Prop> GetProps(string entityId, string entityName, string rawTag)
        {
            var roleTag = rawTag;          // string.Format("<metadata><{0}=\"{1}\">", entityName, entityId);
            var result = new List<Prop>();

            var props = GetDBData(string.Format("{0}, prop id", entityName), UserName, DOID);
            var names = GetDBData(string.Format("{0}, prop name", entityName), UserName, DOID);
            var ns = GetDBData(string.Format("{0}, prop ns", entityName), UserName, DOID);
            var classes = GetDBData(string.Format("{0}, prop class", entityName), UserName, DOID);
            var values = GetDBData(string.Format("{0}, prop", entityName), UserName, DOID);


            for (int i = 0; i < props.Count; i++)
            {
                string id = "";
                if (props[i][1].Contains(roleTag))
                {

                    id = props[i][0];
                    var pp = new Prop
                    {
                        ParentID = entityId,
                        ID = id
                    };

                    result.Add(pp);
                }

            }
            var res = new List<Prop>();
            foreach (var prop in result)
            {
                string clas = "", name = "", nss = "", value = "";
                for (int i = 0; i < classes.Count; i++)
                {
                    if (classes[i][1].Contains(roleTag) && classes[i][1].Contains(prop.ID))
                    {
                        clas = classes[i][0];
                        break;
                    }
                }
                for (int i = 0; i < names.Count; i++)
                {
                    if (names[i][1].Contains(roleTag) && names[i][1].Contains(prop.ID))
                    {
                        name = names[i][0];
                        break;
                    }
                }
                for (int i = 0; i < ns.Count; i++)
                {
                    if (ns[i][1].Contains(roleTag) && ns[i][1].Contains(prop.ID))
                    {
                        nss = ns[i][0];
                        break;
                    }
                }
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i][1].Contains(roleTag) && values[i][1].Contains(prop.ID))
                    {
                        value = values[i][0];
                        break;
                    }
                }

                var temp = new Prop
                {
                    ParentID = prop.ParentID,
                    Name = name,
                    ID = prop.ID,
                    Class = clas,
                    NS = nss,
                    Value = value
                };

                res.Add(temp);
            }

            return res;
        }

        protected string RemoveSpace(string text)
        {
            text = text.Replace(" ", "_");
            text = text.Replace("+", "");
            text = text.Replace("-", "");
            text = text.Replace(":", "");
            return text;
        }
        protected List<Prop> GetProps(string entityId, string entityName)
        {
            var roleTag = string.Format("<metadata><{0}=\"{1}\">", entityName, entityId);
            var result = new List<Prop>();

            var props = GetDBData(string.Format("{0}, prop id", entityName), UserName, DOID);
            var names = GetDBData(string.Format("{0}, prop name", entityName), UserName, DOID);
            var ns = GetDBData(string.Format("{0}, prop ns", entityName), UserName, DOID);
            var classes = GetDBData(string.Format("{0}, prop class",entityName), UserName, DOID);
            var values = GetDBData(string.Format("{0}, prop", entityName), UserName, DOID);
            

            for (int i = 0; i < props.Count; i++)
            {
                string id = "";
                if (props[i][1].Contains(roleTag))
                {

                    id = props[i][0];
                    var pp = new Prop
                    {
                        ParentID = entityId,
                        ID = id
                    };

                    result.Add(pp);
                }

            }
            var res = new List<Prop>();
            foreach (var prop in result)
            {
                string clas = "", name = "", nss = "", value = "";
                for (int i = 0; i < classes.Count; i++)
                {
                    if (classes[i][1].Contains(roleTag) && classes[i][1].Contains(prop.ID))
                    {
                        clas = classes[i][0];
                        break;
                    }
                }
                for (int i = 0; i < names.Count; i++)
                {
                    if (names[i][1].Contains(roleTag) && names[i][1].Contains(prop.ID))
                    {
                        name = names[i][0];
                        break;
                    }
                }
                for (int i = 0; i < ns.Count; i++)
                {
                    if (ns[i][1].Contains(roleTag) && ns[i][1].Contains(prop.ID))
                    {
                        nss = ns[i][0];
                        break;
                    }
                }
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i][1].Contains(roleTag) && values[i][1].Contains(prop.ID))
                    {
                        value = values[i][0];
                        break;
                    }
                }

                var temp = new Prop
                {
                    ParentID = prop.ParentID,
                    Name = name,
                    ID = prop.ID,
                    Class = clas,
                    NS = nss,
                    Value = value
                };

                res.Add(temp);
            }

            return res;
        }

        protected List<List<string>> GetControlList(string desc)
        {
          
            var raw = GetDBData(desc, UserName, DOID);
           
            return raw;
        }

        protected List<Prop> GetRoleProps(string roleid)
        {
            var roleTag = string.Format("<metadata><role id=\"{0}\">", roleid);
            var result = new List<Prop>();

            var props = GetDBData("role id, prop id", UserName, DOID);
            var names = GetDBData("role id, prop name", UserName, DOID);
            var ns = GetDBData("role id, prop ns", UserName, DOID);
            var classes = GetDBData("role id, prop class", UserName, DOID);
            var values = GetDBData("role id, prop", UserName, DOID);


            for (int i = 0; i < props.Count; i++)
            {
                string id = "";
                if (props[i][1].Contains(roleTag))
                {

                    id = props[i][0];
                    var pp = new Prop
                    {
                        ParentID = roleid,
                        ID = id
                    };

                    result.Add(pp);
                }

            }
            var res = new List<Prop>();
            foreach (var prop in result)
            {
                string clas = "", name = "", nss = "", value = "";
                for (int i = 0; i < classes.Count; i++)
                {
                    if (classes[i][1].Contains(roleTag) && classes[i][1].Contains(prop.ID))
                    {
                        clas = classes[i][0];
                        break;
                    }
                }
                for (int i = 0; i < names.Count; i++)
                {
                    if (names[i][1].Contains(roleTag) && names[i][1].Contains(prop.ID))
                    {
                        name = names[i][0];
                        break;
                    }
                }
                for (int i = 0; i < ns.Count; i++)
                {
                    if (ns[i][1].Contains(roleTag) && ns[i][1].Contains(prop.ID))
                    {
                        nss = ns[i][0];
                        break;
                    }
                }
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i][1].Contains(roleTag) && values[i][1].Contains(prop.ID))
                    {
                        value = values[i][0];
                        break;
                    }
                }

                var temp = new Prop
                {
                    ParentID = prop.ParentID,
                    Name = name,
                    ID = prop.ID,
                    Class = clas,
                    NS = nss,
                    Value = value
                };

                res.Add(temp);
            }

            return res;
        }

        protected List<Link> GetLinks(string entityId, string entityName, string rawTag)
        {
            var Tag = rawTag;                      //string.Format("<metadata><{1}=\"{0}\">", entityId, entityName);
            var result = new List<Link>();

            var links = GetDBData(string.Format("{0}, link href", entityName), UserName, DOID);
            var rels = GetDBData(string.Format("{0}, link rel", entityName), UserName, DOID);
            var mediatypes = GetDBData(string.Format("{0}, link media-type", entityName), UserName, DOID);

            var values = GetDBData(string.Format("{0}, link", entityName), UserName, DOID);


            for (int i = 0; i < links.Count; i++)
            {
                string id = "";
                if (links[i][1].Contains(Tag))
                {

                    id = links[i][0];
                    var pp = new Link
                    {
                        ParentID = entityId,
                        HRef = id
                    };

                    result.Add(pp);
                }

            }
            var res = new List<Link>();
            foreach (var prop in result)
            {
                string rel = "", mediatype = "", value = "";



                for (int i = 0; i < rels.Count; i++)
                {
                    if (rels[i][1].Contains(Tag) && rels[i][1].Contains(prop.HRef))
                    {
                        rel = rels[i][0];
                        break;
                    }
                }
                for (int i = 0; i < mediatypes.Count; i++)
                {
                    if (mediatypes[i][1].Contains(Tag) && mediatypes[i][1].Contains(prop.HRef))
                    {
                        mediatype = mediatypes[i][0];
                        break;
                    }
                }
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i][1].Contains(Tag) && values[i][1].Contains(prop.HRef))
                    {
                        value = values[i][0];
                        break;
                    }
                }

                var temp = new Link
                {
                    ParentID = prop.ParentID,
                    HRef = prop.HRef,
                    MarkUpLine = value,
                    Rel = rel,
                    MediaType = mediatype,
                };

                res.Add(temp);
            }

            return res;
        }

        protected List<Link> GetLinks(string entityId, string entityName)
        {
            var Tag = string.Format("<metadata><{1}=\"{0}\">", entityId, entityName);
            var result = new List<Link>();

            var ids = GetDBData(string.Format("{0}, link id", entityName), UserName, DOID);
            var links = GetDBData(string.Format("{0}, link href", entityName), UserName, DOID);
            var rels = GetDBData(string.Format("{0}, link rel", entityName), UserName, DOID);
            var mediatypes = GetDBData(string.Format("{0}, link media-type", entityName), UserName, DOID);
          
            var values = GetDBData(string.Format("{0}, link", entityName), UserName, DOID);


            for (int i = 0; i < ids.Count; i++)
            {
                string id = "";
                if (ids[i][1].Contains(Tag))
                {

                    id = ids[i][0];
                    var pp = new Link
                    {
                        ParentID = id,
                    
                    };

                    result.Add(pp);
                }

            }
            var res = new List<Link>();
            foreach (var prop in result)
            {
                string href ="", rel = "", mediatype = "", value = "";

                for (int i = 0; i < links.Count; i++)
                {
                    if (links[i][1].Contains(Tag) && links[i][1].Contains(prop.ParentID))
                    {
                        href =links[i][0];

                        break;
                    }
                }

                for (int i = 0; i < rels.Count; i++)
                {
                    if (rels[i][1].Contains(Tag) && rels[i][1].Contains(prop.ParentID))
                    {
                        rel = rels[i][0];
                        break;
                    }
                }
                for (int i = 0; i < mediatypes.Count; i++)
                {
                    if (mediatypes[i][1].Contains(Tag) && mediatypes[i][1].Contains(prop.ParentID))
                    {
                        mediatype = mediatypes[i][0];
                        break;
                    }
                }
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i][1].Contains(Tag) && values[i][1].Contains(prop.ParentID))
                    {
                        value = values[i][0];
                        break;
                    }
                }

                var temp = new Link
                {
                    ParentID = prop.ParentID,
                    HRef = href,
                     MarkUpLine=value,
                    Rel=rel,
                    MediaType = mediatype,
                };

                res.Add(temp);
            }

            return res;
        }

        protected string GenerateAnnotationID(string entityId, string entityName)
        {
            var Anns = GetAnnotations(entityId, entityName);
            var nbrRole = Anns.Count();
            var id = string.Format("ann-{0}-{1}", entityId, nbrRole);
            return id;
        }

        protected string GenerateLinkID(string entityId, string entityName)
        {
            var count = GetAllHeaders(string.Format("{0}, link id", entityName), UserName, DOID).Count;
            var id = string.Format("link-{0}-{1}", entityId, count);
            return id;
        }

        protected string GenerateAnnotationID(string entityId, string entityName, string rawTag)
        {

            var count = GetAllHeaders(string.Format("{0}, annotation id", entityName), UserName, DOID).Count;
            var id = string.Format("ann-{0}-{1}", entityId, count);
            return id;
        }

        protected string GeneratePropID(string entityId, string entityName)
        {
            var count = GetAllHeaders(string.Format("{0}, prop id", entityName), UserName, DOID).Count;         
            var id = string.Format("prop-{0}-{1}", entityId, count);
            return id;
        }

        protected string GeneratePropID(string entityId, string entityName, string rawTag)
        {
            var count = GetAllHeaders(string.Format("{0}, prop id", entityName), UserName, DOID).Count;
            var id = string.Format("prop-{0}-{1}", entityId, count);
            return id;
        }

        protected List<Annotation> GetAnnotations(string entityId, string entityName, string mainTag)
        {
            var Tag = mainTag;      //string.Format("<metadata><{1}=\"{0}\">", entityId, entityName);
            var result = new List<Annotation>();

            var annotations = GetDBData(string.Format("{0}, annotation id", entityName), UserName, DOID);
            var names = GetDBData(string.Format("{0}, annotation name", entityName), UserName, DOID);
            var ns = GetDBData(string.Format("{0}, annotation ns", entityName), UserName, DOID);
            var remarks = GetDBData(string.Format("{0}, annotation, remarks", entityName), UserName, DOID);
            var values = GetDBData(string.Format("{0}, annotation", entityName), UserName, DOID);


            for (int i = 0; i < annotations.Count; i++)
            {
                string id = "";
                if (annotations[i][1].Contains(Tag))
                {

                    id = annotations[i][0];
                    var pp = new Annotation
                    {
                        ParentID = entityId,
                        ID = id
                    };

                    result.Add(pp);
                }

            }
            var res = new List<Annotation>();
            foreach (var prop in result)
            {
                string remark = "", name = "", nss = "", value = "";

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(Tag) && remarks[i][1].Contains(prop.ID))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                for (int i = 0; i < names.Count; i++)
                {
                    if (names[i][1].Contains(Tag) && names[i][1].Contains(prop.ID))
                    {
                        name = names[i][0];
                        break;
                    }
                }
                for (int i = 0; i < ns.Count; i++)
                {
                    if (ns[i][1].Contains(Tag) && ns[i][1].Contains(prop.ID))
                    {
                        nss = ns[i][0];
                        break;
                    }
                }
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i][1].Contains(Tag) && values[i][1].Contains(prop.ID))
                    {
                        value = values[i][0];
                        break;
                    }
                }

                var temp = new Annotation
                {
                    ParentID = prop.ParentID,
                    Name = name,
                    ID = prop.ID,
                    Remarks = remark,
                    NS = nss,
                    Value = value
                };

                res.Add(temp);
            }

            return res;
        }

        protected void SaveRemarksDesc(string mainTag, string ElementDesc, string InnerHtml)
        {
            var temp = InnerHtml;
            var lines = new List<string>();
            while (temp.Length > 0)
            {
                int nbr = temp.IndexOf("\r\n");
                if (nbr > 0)
                {
                    var pan = temp.Substring(0, nbr);
                    temp = temp.Remove(0, nbr + 4);
                    lines.Add(pan);

                }
                else
                {
                    lines.Add(temp);
                    break;
                }
            }

            for (int i = 0; i < lines.Count; i++)
            {
                var eltName = string.Format("p{0}", i);
                var tag = string.Format("{0}<p>", mainTag);
                var eltDesc = string.Format("{0}, p", ElementDesc);
                InsertElementToDataBase(DOID, SystemID, eltName, eltName.GetType(), tag, eltDesc, lines[i], 1);

            }
        }


        protected List<Annotation> GetAnnotations(string entityId, string entityName)
        {
            var Tag = string.Format("<metadata><{1}=\"{0}\">", entityId, entityName);
            var result = new List<Annotation>();
           
            var annotations = GetDBData(string.Format("{0}, annotation id",entityName), UserName, DOID);
            var names = GetDBData(string.Format("{0}, annotation name", entityName), UserName, DOID);
            var ns = GetDBData(string.Format("{0}, annotation ns",entityName), UserName, DOID);
            var remarks = GetDBData(string.Format("{0}, annotation, remarks", entityName), UserName, DOID);
            var values =  GetDBData(string.Format("{0}, annotation", entityName), UserName, DOID);


            for (int i = 0; i < annotations.Count; i++)
            {
                string id = "";
                if (annotations[i][1].Contains(Tag))
                {

                    id = annotations[i][0];
                    var pp = new Annotation
                    {
                        ParentID = entityId,
                        ID = id
                    };

                    result.Add(pp);
                }
                
            }
            var res = new List<Annotation>();
            foreach( var prop in result)
            {
                string remark="",  name = "", nss = "", value = "";

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(Tag) && remarks[i][1].Contains(prop.ID))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                for (int i = 0; i < names.Count; i++)
                {
                    if (names[i][1].Contains(Tag) && names[i][1].Contains(prop.ID))
                    {
                        name = names[i][0];
                        break;
                    }
                }
                for (int i = 0; i < ns.Count; i++)
                {
                    if (ns[i][1].Contains(Tag) && ns[i][1].Contains(prop.ID))
                    {
                        nss = ns[i][0];
                        break;
                    }
                }
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i][1].Contains(Tag) && values[i][1].Contains(prop.ID))
                    {
                        value = values[i][0];
                        break;
                    }
                }

                var temp = new Annotation
                {
                    ParentID = prop.ParentID,
                    Name = name,
                    ID = prop.ID,
                    Remarks = remark,
                    NS = nss,
                    Value = value
                };

                res.Add(temp);
            }

            return res;
        }

        protected List<DocLocation> GetLocations()
        {
            var result = new List<DocLocation>();



           var locations = GetHeaderAndTag("location id, location id", UserName, DOID);
           var addressTypes = GetDBData("location id, address type", UserName, DOID);
            var addrLines = GetDBData("location id, addr-line", UserName, DOID);
            var uuids = GetDBData("location id, uuid", UserName, DOID);
            var cities = GetDBData("location id, city", UserName, DOID);
            var states = GetDBData("location id, state", UserName, DOID);
            var postalCodes = GetDBData("location id, postal-code", UserName, DOID);
            var countries = GetDBData("location id, country", UserName, DOID);
            var phones = GetDBData("location id, phone", UserName, DOID);
            var emails = GetDBData("location id, email", UserName, DOID);
            var  remarks = GetDBData("location id, remarks", UserName, DOID);

            for (int i = 0; i < locations.Count; i++)
            {
                var locid = locations[i][0];
                var tag = locations[i][1];
                var loc = new DocLocation
                {
                    LocationID = locid,
                };
                result.Add(loc);
            }

            var tem = new List<DocLocation>();
            foreach (var loc in result)
            {
                var locTag = string.Format("<metadata><location id=\"{0}\">", loc.LocationID);
                var addressType = "";
                var addrline1 = "";
                var addrline2 = "";
                var city = "";
                var state = "";
                var postalcode = "";
                var country = "";
                var phone = "";
                var email = "";
                var url = "";
                var remark = "";
                var uuid = "";
                var props = GetProps(loc.LocationID, "location id");
                var links = GetLinks(loc.LocationID, "location id");
                for (int i = 0; i < addressTypes.Count; i++)
                {
                    if (addressTypes[i][1].Contains(locTag))
                    {
                        addressType = addressTypes[i][0];
                        break;
                    }
                }

                for (int i = 0; i < uuids.Count; i++)
                {
                    if (uuids[i][1].Contains(locTag))
                    {
                        uuid = uuids[i][0];
                        break;
                    }
                }

                for (int i = 0; i < addrLines.Count; i++)
                {
                    if (addrLines[i][1].Contains(locTag) && addrLines[i][2]=="addr-line")
                    {
                        addrline1 = addrLines[i][0];
                        break;
                    }
                }
                for (int i = 0; i < addrLines.Count; i++)
                {
                    if (addrLines[i][1].Contains(locTag) && addrLines[i][2] == "addr-line2")
                    {
                        addrline2 = addrLines[i][0];
                        break;
                    }
                }
                for (int i = 0; i < cities.Count; i++)
                {
                    if (cities[i][1].Contains(locTag))
                    {
                        city = cities[i][0];
                        break;
                    }
                }
                for (int i = 0; i < states.Count; i++)
                {
                    if(states[i][1].Contains(locTag))
                    {
                        state = states[i][0];
                        break;
                    }
                }

                for (int i = 0; i < postalCodes.Count; i++)
                {
                    if (postalCodes[i][1].Contains(locTag))
                    {
                        postalcode = postalCodes[i][0];
                        break;
                    }
                }

                for (int i = 0; i < countries.Count; i++)
                {
                    if (countries[i][1].Contains(locTag))
                    {
                        country= countries[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(locTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }


                var Emails = GetAuxillaries(loc.LocationID, "location id", "email");
               

                var Urls = GetAuxillaries(loc.LocationID, "location id", "url");
               

                var Phones = GetPhones(loc.LocationID, "location id");

                var take = new DocLocation
                {
                    LocationID = loc.LocationID,
                    AddressType = addressType,
                    AddressLine1 = addrline1,
                    AddressLine2 = addrline2,
                    PostalCode = postalcode,
                    City = city,
                    State =  state,
                    Country = country,
                    Remarks = remark,
                    Props = props,
                    Links = links,
                    UUID = uuid,
                    Emails = Emails,
                    Url = Urls,
                    Phones = Phones
                };
                tem.Add(take);
            }

            return tem;
        }


        protected List<DocumentIdentifier> GetDocIDs()
        {
            var result = new List<DocumentIdentifier>();


            var docids = GetHeaderAndTag("doc-id, doc-id", UserName, DOID);
            var ids = GetDBData("doc-id, identifier", UserName, DOID);
            var types = GetDBData("doc-id, type", UserName, DOID);
            

            for (int i = 0; i < docids.Count; i++)
            {
                var docid = docids[i][0];
                var tag = docids[i][1];
                var rev = new DocumentIdentifier
                {
                    DocID = docid,
                };
                result.Add(rev);
            }

            var tem = new List<DocumentIdentifier>();
            foreach (var rev in result)
            {
                var revTag = string.Format("<metadata><doc-id=\"{0}\">", rev.DocID);
                var value = "";
                var type = "";

                var props = GetProps(rev.DocID, "doc-id");
                var links = GetLinks(rev.DocID, "doc-id");
                for (int i = 0; i < ids.Count; i++)
                {
                    if (ids[i][1].Contains(revTag))
                    {
                        value = ids[i][0];
                        break;
                    }
                }
                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i][1].Contains(revTag))
                    {
                        type = types[i][0];
                        break;
                    }
                }
              
                var take = new DocumentIdentifier
                {
                    DocID = rev.DocID,
                    Value= value,
                    Type = type,
                    Props = props,
                    Links = links
                };
                tem.Add(take);
            }

            return tem;
        }

        protected List<Method> GetMethods()
        {
            var result = new List<Method>();
            var ids = GetHeaderAndTag("method id, method id", UserName, DOID);
            var uuids = GetDBData("method id, uuid", UserName, DOID);
            var descs = GetDBData("method id, description", UserName, DOID);
            var remarks = GetDBData("method id, remarks", UserName, DOID);
            var names = GetDBData("method id, part name", UserName, DOID);
            var classes = GetDBData("method id, part class", UserName, DOID);
            var nss = GetDBData("method id, part ns", UserName, DOID);
            var partdess = GetDBData("method id, part description", UserName, DOID);
            var parttitles = GetDBData("method id, part title", UserName, DOID);
            var partParts = GetDBData("method id, part part", UserName, DOID);
            

            for (int i = 0; i < ids.Count; i++)
            {
                string uuid = "", desc = "", rem = "", name = "", ns = "", cal="", dess = "", tit = "";
                List<string> parts = new List<string>();
                var id = ids[i][0];
                var tag = ids[i][1];
                for (int j = 0; j < uuids.Count; j++)
                {
                    if (uuids[j][1].Contains(tag))
                    {
                        uuid = uuids[j][0];
                        break;
                    }
                }

                for (int j = 0; j < descs.Count; j++)
                {
                    if (descs[j][1].Contains(tag))
                    {
                        desc = descs[j][0];
                        break;
                    }
                }
                for (int j = 0; j < remarks.Count; j++)
                {
                    if (remarks[j][1].Contains(tag))
                    {
                        rem = remarks[j][0];
                        break;
                    }
                }
                foreach(var x in names)
                {
                    if(x[1].Contains(tag))
                    {
                        name = x[0];
                        break;
                    }
                }
                foreach (var x in nss)
                {
                    if (x[1].Contains(tag))
                    {
                        ns = x[0];
                        break;
                    }
                }

                foreach (var x in classes)
                {
                    if (x[1].Contains(tag))
                    {
                        cal = x[0];
                        break;
                    }
                }

                foreach (var x in partdess)
                {
                    if (x[1].Contains(tag))
                    {
                        dess = x[0];
                        break;
                    }
                }
                foreach (var x in parttitles)
                {
                    if (x[1].Contains(tag))
                    {
                        tit = x[0];
                        break;
                    }
                }
                foreach (var x in partParts)
                {
                    if (x[1].Contains(tag))
                    {
                        var pp = x[0];
                        parts.Add(pp);
                    }
                }
                var rawTag = string.Format("<objectives><method id=\"{0}\">", id);
                var props = GetProps(id, "method id", rawTag);
                var anns = GetAnnotations(id, "method id", rawTag);
                var rev = new Method
                {
                   ID = id,
                   UUID = uuid,
                   Description = desc,
                   Remarks = rem,
                   PartClass = cal,
                   PartID = "part-id",
                   PartName = name,
                   PartNS = ns,
                   PartTitle = tit,
                   Parts =parts,
                   PartDescription = dess,
                   Props= props,
                   Annotations = anns
                };
                result.Add(rev);
            }

            return result;
        }

        protected List<DocRevision> GetRevisions()
        {
            var result = new List<DocRevision>();
           

            var revisions = GetHeaderAndTag("revision id, revision id", UserName, DOID);
            var titles = GetDBData("revision id, title", UserName, DOID);
            var publications = GetDBData("revision id, published", UserName, DOID);
            var modifications = GetDBData("revision id, last-modified", UserName, DOID);
            var versions = GetDBData("revision id, version", UserName, DOID);
            var oscalversions = GetDBData("revision id, oscal-version", UserName, DOID);
            var remarks = GetDBData("revision id, remarks", UserName, DOID);

            for (int i = 0; i < revisions.Count; i++)
            {
                var revisionid = revisions[i][0];
                var tag = revisions[i][1];
                var rev = new DocRevision
                {
                    RevisionID = revisionid,                  
                };
                result.Add(rev);
            }
       
            var tem = new List<DocRevision>();
            foreach (var rev in result)
            {
                var revTag = string.Format("<metadata><revision id=\"{0}\">", rev.RevisionID);
                var title = "";
                var published = "";
                var lastmodified = "";
                var remark = "";
                var version = "";
                var oscalversion = "";
                var props = GetProps(rev.RevisionID, "revision id");
                var links = GetLinks(rev.RevisionID, "revision id");
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i][1].Contains(revTag))
                    {
                        title = titles[i][0];
                        break;
                    }
                }
                for (int i = 0; i < publications.Count; i++)
                {
                    if (publications[i][1].Contains(revTag))
                    {
                        published = publications[i][0];
                        break;
                    }
                }
                for (int i = 0; i < modifications.Count; i++)
                {
                    if (modifications[i][1].Contains(revTag))
                    {
                        lastmodified = modifications[i][0];
                        break;
                    }
                }

                for (int i = 0; i < versions.Count; i++)
                {
                    if (versions[i][1].Contains(revTag))
                    {
                        version = versions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < oscalversions.Count; i++)
                {
                    if (oscalversions[i][1].Contains(revTag))
                    {
                        oscalversion = oscalversions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }


                var take = new DocRevision
                {
                    RevisionID = rev.RevisionID,
                   Title = title,
                   Published = published,
                   LastModified = lastmodified,
                   Version = version,
                   OSCALVersion = oscalversion,
                    Remarks = remark,
                    Props = props,
                    Links =links
                };
                tem.Add(take);
            }

            return tem;
        }

        protected List<Task> GetTasks( string scheduleid)
        {

            var result = new List<Task>();
            var taskIds = GetDBData("schedule id, task id", UserName, DOID);
            var taskuuIds = GetDBData("schedule id, task id uuid", UserName, DOID);
            var Descs = GetDBData("schedule id, task id description", UserName, DOID);
            var starts = GetDBData("schedule id, task id start", UserName, DOID);
            var ends = GetDBData("schedule id, task id end", UserName, DOID);
            var titles = GetDBData("schedule id, task id title", UserName, DOID);
            var Remarks = GetDBData("schedule id, task id remarks", UserName, DOID);
            var Compares = GetDBData("schedule id, task id compare-to", UserName, DOID);
            var roleIds = GetDBData("schedule id, task id role id", UserName, DOID);
            var partyUUIDs = GetDBData("schedule id, task id party uuid", UserName, DOID);
            var locationUUIDs = GetDBData("schedule id, task id location uuid", UserName, DOID);
            var activityUUIDs = GetDBData("schedule id, task id activity uuid", UserName, DOID);

            foreach (var task in taskIds)
            {
                var taskid = task[0];
                string uuid = "", desc = "", rem = "", comp = "", title="", start="", end="";
                List<string> rIds = new List<string>(); ;
                List<string> partuuids = new List<string>() ;
                var locuuids = new List<string>();
                var actuuids = new List<string>();
                var rawTag = string.Format("<assessment-activities><{1}=\"{0}\"><{2}=\"{3}\">", scheduleid, "schedule id", "task id", taskid);

                if (!task[1].Contains(rawTag))
                    continue;
                
                var Props = GetProps(taskid, "task id", rawTag);
                var Anns = GetAnnotations(taskid, "task id", rawTag);
                foreach(var x in taskuuIds)
                {
                    if(x[1].Contains(rawTag))
                    {
                        uuid = x[0];
                        break;
                    }
                }

                foreach (var x in Descs)
                {
                    if (x[1].Contains(rawTag))
                    {
                        desc = x[0];
                        break;
                    }
                }

                foreach (var x in Remarks)
                {
                    if (x[1].Contains(rawTag))
                    {
                        rem = x[0];
                        break;
                    }
                }
                foreach (var x in titles)
                {
                    if (x[1].Contains(rawTag))
                    {
                        title = x[0];
                        break;
                    }
                }
                foreach (var x in starts)
                {
                    if (x[1].Contains(rawTag))
                    {
                        start = x[0];
                        break;
                    }
                }

                foreach (var x in ends)
                {
                    if (x[1].Contains(rawTag))
                    {
                        end = x[0];
                        break;
                    }
                }
                foreach (var x in Compares)
                {
                    if (x[1].Contains(rawTag))
                    {
                        comp = x[0];
                        break;
                    }
                }
                for (int i = 0; i < roleIds.Count; i++)
                {
                    if (roleIds[i][1].Contains(rawTag))
                    {
                        rIds.Add(roleIds[i][0]);
                        break;
                    }
                }
                for (int i = 0; i < partyUUIDs.Count; i++)
                {
                    if (partyUUIDs[i][1].Contains(rawTag))
                    {
                        partuuids.Add(partyUUIDs[i][0]);
                        break;
                    }
                }

                for (int i = 0; i < locationUUIDs.Count; i++)
                {
                    if (locationUUIDs[i][1].Contains(rawTag))
                    {
                        locuuids.Add(locationUUIDs[i][0]);
                        break;
                    }
                }

                for (int i = 0; i < activityUUIDs.Count; i++)
                {
                    if (activityUUIDs[i][1].Contains(rawTag))
                    {
                        actuuids.Add(activityUUIDs[i][0]);
                        break;
                    }
                }

                var temp = new Task
                {
                    ID = taskid,
                    UUID = uuid,
                    Description = desc,
                    Remarks = rem,
                    Props = Props,
                    Annotations = Anns,
                    LocationUUIDs = locuuids,
                    RoleIds = rIds,
                    PartyUUIDs =  partuuids,
                    ActivityUUIDs = actuuids,
                    CompareTo = comp,
                    Title = title,
                    Start = start,
                    End = end
                };

                result.Add(temp);

            }

            return result;
        }

        protected List<TestStep> GetTestSteps(string methodid)
        {
            var result = new List<TestStep>();

           
            var stepIds = GetDBData("test-method id, test-step id", UserName, DOID);
            var sequences = GetDBData("test-method id, test-step id sequence", UserName, DOID);
            var Desc = GetDBData("user test-method id, test-step id  description", UserName, DOID);
            var uuids = GetDBData("test-method id, test-step uuid", UserName, DOID);
            var remarks = GetDBData("user test-method id, test-step id  remarks", UserName, DOID);
            var roleIds = GetDBData("test-method id, test-step id role id", UserName, DOID);
            var partyUUIDs = GetDBData("test-method id, test-step id party uuid", UserName, DOID);

           
            for (int j = 0; j < stepIds.Count; j++)
            {

                var desc = "";
                var sequence = "";
                var remark = "";
                var uuid = "";
                var stepid = stepIds[j][0];
                var rIds = new List<string>();
                var puuids = new List<string>();
                var tag = string.Format("<assessment-activities><test-method id=\"{0}\"><test-step id=\"{1}\">", methodid, stepid);

                if (!stepid.Contains(methodid))
                    continue;

                for (int i = 0; i < sequences.Count; i++)
                {
                    if (sequences[i][1].Contains(tag))
                    {
                        sequence = sequences[i][0];
                        break;
                    }
                }
                for (int i = 0; i < Desc.Count; i++)
                {
                    if (Desc[i][1].Contains(tag))
                    {
                        desc = Desc[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(tag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                for (int i = 0; i < uuids.Count; i++)
                {
                    if (uuids[i][1].Contains(tag))
                    {
                        uuid = uuids[i][0];
                        break;
                    }
                }

                for (int i = 0; i < roleIds.Count; i++)
                {
                    if (roleIds[i][1].Contains(tag))
                    {
                        rIds.Add( roleIds[i][0]);
                        break;
                    }
                }
                for (int i = 0; i < partyUUIDs.Count; i++)
                {
                    if (partyUUIDs[i][1].Contains(tag))
                    {
                        puuids.Add(partyUUIDs[i][0]);
                        break;
                    }
                }

                var pre = new TestStep
                {
                    Sequence = int.Parse(sequence),
                    ID = stepid,
                    UUID = uuid,
                    Description = desc,
                    Remarks = remark,
                    RoleIds = rIds,
                    PartyUUIDs = puuids

                };
                result.Add(pre);
            }

            return result;
        }

        protected List<ThreatID>GetThreatIDs( string resultid, string findingid)
        {
            var results = new List<ThreatID>();
            var ids = GetDBData("finding id, threat-id", UserName, DOID);
            var systems = GetDBData("finding id, threat-id system", UserName, DOID);
            var uris = GetDBData("finding id, threat-id uri", UserName, DOID);
            var threats = GetDBData("finding id, threat-id threat", UserName, DOID);

            var last = new List<ThreatID>();
            for (int i = 0; i < ids.Count; i++)
            {
                var name = ids[i][0];
                var tag = ids[i][1];
                if (!tag.Contains(findingid))
                    continue;

                var rev = new ThreatID
                {
                    ID = name,
                };
                results.Add(rev);
            }

            foreach (var rev in results)
            {
                var revTag = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><threat-id=\"{2}\">", resultid, findingid, rev.ID); ;
                var system = "";

                var urit = "";
                var threa = "";
               
                foreach( var sys in systems)
                {
                    if(sys[1].Contains(revTag))
                    {
                        system = sys[0];
                        break;
                    }
                }

                foreach (var uri in uris)
                {
                    if (uri[1].Contains(revTag))
                    {
                        urit = uri[0];
                        break;
                    }
                }

                foreach (var th in threats)
                {
                    if (th[1].Contains(revTag))
                    {
                        threa = th[0];
                        break;
                    }
                }

                var tem = new ThreatID
                {
                    ID = rev.ID,
                    System = system,
                    URI = urit,
                    Value = threa
                };
                last.Add(tem);
            }


            return last;
        }

        

        protected List<Schedule> GetSchedules()
        {

            var results = new List<Schedule>();
            var label = "schedule";
            var ids = GetDBData(string.Format("{0} id, {0} id", label), UserName, DOID);
            var uuids = GetDBData(string.Format("{0} id, uuid", label), UserName, DOID);
           foreach(var id in ids)
            {
                var Id = id[0];
                var uuId = "";
                var tag  = string.Format("<assessment-activities><schedule id=\"{0}\">", Id);
                foreach (var uuid in uuids)
                {
                    if(uuid[1].Contains(""))
                    {
                        uuId = uuid[0];
                        break;
                    }
                }

                var tasks = GetTasks(Id);
                var temp = new Schedule
                {
                    ID = Id,
                    UUID = uuId,
                    Tasks = tasks
                };
                results.Add(temp);
            }


            return results;
        }
        protected List<Finding> GetFullFindings( string resultId)
        {
            var result = GetFindings(resultId);
            var fullresult = new List<Finding>();
            foreach( var find in result)
            {
                var id = find.ID;
                var obs = GetObservations(resultId, id);
                var temp = find;
                    temp.Observations = obs;
                var risks = GetRisks(resultId, id);
                temp.Risks = risks;
                fullresult.Add(temp);
            }

            return fullresult;
        }
        protected List<Finding> GetFindings( string resultId)
        {
            var result = new List<Finding>();
            var label = "finding";
            var ids = GetDBData(string.Format("{0} id, {0} id", label), UserName, DOID);
            var uuids = GetDBData(string.Format("{0} id, uuid", label), UserName, DOID);
            var titles = GetDBData(string.Format("{0} id, title", label), UserName, DOID);
            var descriptions = GetDBData(string.Format("{0} id, description", label), UserName, DOID);
            var remarks = GetDBData(string.Format("{0} id, remarks", label), UserName, DOID);

            var dates = GetDBData(string.Format("{0} id, date-time-stamp", label), UserName, DOID);
            

            for (int i = 0; i < ids.Count; i++)
            {
                var name = ids[i][0];
                var tag = ids[i][1];
                if (!tag.Contains(resultId))
                    continue;

                var rev = new Finding
                {
                    ID = name,
                };
                result.Add(rev);
            }

            var tem = new List<Finding>();
            foreach (var rev in result)
            {
                var revTag = string.Format("<results id={0}><finding id=\"{1}\">", resultId, rev.ID);
                var start = "";
   
                var title = "";
                var desc = "";
                var remark = "";
                var uuid = "";

                var statementuuid = GetData("implementation-statement-uuid", revTag + "<implementation-statement-uuid>", UserName, DOID);
                var tagControlId = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id>", resultId, rev.ID);

                var tagObjectId = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id>", resultId, rev.ID);

                var tagObjectTitle = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><title>", resultId, rev.ID);

                var tagObjectDesc = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><description>", resultId, rev.ID);

                var tagResultSystem = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><result system>", resultId, rev.ID);
                var tagResult = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><result>", resultId, rev.ID);
                var tagimplSystem = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><implementation-status system>", resultId, rev.ID);
                var tagImplValue = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><result>", resultId, rev.ID);
                var tagRem = string.Format("<results id={0}><finding id=\"{1}\"><objective-status objective id control id><remarks>", resultId, rev.ID);


                var controlId = GetData("control id", tagControlId, UserName, DOID);
                var objId = GetData("objective id", tagObjectId, UserName, DOID);
                var objTitle = GetData("title", tagObjectTitle, UserName, DOID);
                var objectDesc =  GetData("desc", tagObjectDesc, UserName, DOID);
                var resultSystem = GetData("result system", tagResultSystem, UserName, DOID);
                var resultValue = GetData("result", tagResult, UserName, DOID);
                var implSystem = GetData("implementation-status sytem", tagimplSystem, UserName, DOID);
                var implValue = GetData("implementation-status", tagImplValue, UserName, DOID);
                var ObjRem = GetData("remarks", tagRem, UserName, DOID);

                var props = GetProps(rev.ID, string.Format("{0} id", label), revTag);

                var anns = GetAnnotations(rev.ID, string.Format("{0} id", label), revTag);
                var parties = GetAuxillaries(rev.ID, "finding id", "party uuid");
                var threats = GetThreatIDs(resultId, rev.ID);
                for (int i = 0; i < dates.Count; i++)
                {
                    if (dates[i][1].Contains(revTag))
                    {
                        start = dates[i][0];
                        break;
                    }
                }
                
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i][1].Contains(revTag))
                    {
                        title = titles[i][0];
                        break;
                    }
                }
                for (int i = 0; i < uuids.Count; i++)
                {
                    if (uuids[i][1].Contains(revTag))
                    {
                        uuid = uuids[i][0];
                        break;
                    }
                }

                for (int i = 0; i < descriptions.Count; i++)
                {
                    if (descriptions[i][1].Contains(revTag))
                    {
                        desc = descriptions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }




                var take = new Finding
                {
                    UUID = uuid,
                    ID = rev.ID,
                    DateTimeStamp = start,
                    Title = title,
                    Description = desc,
                    Remarks = remark,
                    ControlID = controlId,
                    ObjectiveID = objId,
                    ObjectiveStatusDesc = objectDesc,
                    ObjectiveTitle = objTitle,
                    ObjectiveStatusRemarks = ObjRem,
                    ResultSystem = resultSystem,
                    ResultValue = resultValue,
                    ImplementationStatusSystem = implSystem,
                    ImplementationStatusValue = implValue,
                    PartyUUIDS = parties,
                    ThreatIDs = threats,
                    Props = props,
                    ImplementationStatementUUIDs = statementuuid,
                    Annotations = anns,

                };
                tem.Add(take);
            }

            return tem;
        }


        protected List<Result> GetFullResults()
        {
            var result = new List<Result>();
            var label = "results";
            var ids = GetDBData(string.Format("{0} id, {0} id", label), UserName, DOID);
            var uuids = GetDBData(string.Format("{0} id, uuid", label), UserName, DOID);
            var titles = GetDBData(string.Format("{0} id, title", label), UserName, DOID);
            var descriptions = GetDBData(string.Format("{0} id, description", label), UserName, DOID);
            var remarks = GetDBData(string.Format("{0} id, remarks", label), UserName, DOID);

            var starts = GetDBData(string.Format("{0} id, start", label), UserName, DOID);
            var ends = GetDBData(string.Format("{0} id, end", label), UserName, DOID);

            for (int i = 0; i < ids.Count; i++)
            {
                var name = ids[i][0];
                var tag = ids[i][1];
                var rev = new Result
                {
                    ID = name,
                };
                result.Add(rev);
            }

            var tem = new List<Result>();
            foreach (var rev in result)
            {
                var revTag = string.Format("<results id=\"{0}\">", rev.ID);
                var start = "";
                var end = "";
                var title = "";
                var desc = "";
                var remark = "";
                var uuid = "";


                var props = GetProps(rev.ID, string.Format("{0} id", label), revTag);

                var anns = GetAnnotations(rev.ID, string.Format("{0} id", label), revTag);
                var Finding = GetFullFindings(rev.ID);
                for (int i = 0; i < starts.Count; i++)
                {
                    if (starts[i][1].Contains(revTag))
                    {
                        start = starts[i][0];
                        break;
                    }
                }
                for (int i = 0; i < ends.Count; i++)
                {
                    if (ends[i][1].Contains(revTag))
                    {
                        end = ends[i][0];
                        break;
                    }
                }
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i][1].Contains(revTag))
                    {
                        title = titles[i][0];
                        break;
                    }
                }
                for (int i = 0; i < uuids.Count; i++)
                {
                    if (uuids[i][1].Contains(revTag))
                    {
                        uuid = uuids[i][0];
                        break;
                    }
                }

                for (int i = 0; i < descriptions.Count; i++)
                {
                    if (descriptions[i][1].Contains(revTag))
                    {
                        desc = descriptions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                var take = new Result
                {
                    UUID = uuid,
                    ID = rev.ID,
                    Start = start,
                    Title = title,
                    Description = desc,
                    Remarks = remark,
                    End = end,
                    Props = props,
                    Findings = Finding,
                    Annotations = anns,

                };
                tem.Add(take);
            }
            return tem;
        }
        protected List<Result> GetResults()
        {
            var result = new List<Result>();
            var label = "results";
            var ids = GetDBData(string.Format("{0} id, {0} id", label), UserName, DOID);
            var uuids = GetDBData(string.Format("{0} id, uuid", label), UserName, DOID);
            var titles = GetDBData(string.Format("{0} id, title", label), UserName, DOID);
            var descriptions = GetDBData(string.Format("{0} id, description", label), UserName, DOID);
            var remarks = GetDBData(string.Format("{0} id, remarks", label), UserName, DOID);
           
            var starts = GetDBData(string.Format("{0} id, start", label), UserName, DOID);
            var ends = GetDBData(string.Format("{0} id, end", label), UserName, DOID);

            for (int i = 0; i < ids.Count; i++)
            {
                var name = ids[i][0];
                var tag = ids[i][1];
                var rev = new Result
                {
                    ID = name,
                };
                result.Add(rev);
            }

            var tem = new List<Result>();
            foreach (var rev in result)
            {
                var revTag = string.Format("<results id=\"{0}\">", rev.ID);
                var start = "";
                var end = "";
                var title = "";
                var desc = "";
                var remark = "";
                var uuid = "";


                var props = GetProps(rev.ID, string.Format("{0} id", label), revTag);
               
                var anns = GetAnnotations(rev.ID, string.Format("{0} id", label), revTag);
               
                for (int i = 0; i < starts.Count; i++)
                {
                    if (starts[i][1].Contains(revTag))
                    {
                        start = starts[i][0];
                        break;
                    }
                }
                for (int i = 0; i < ends.Count; i++)
                {
                    if (ends[i][1].Contains(revTag))
                    {
                        end = ends[i][0];
                        break;
                    }
                }
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i][1].Contains(revTag))
                    {
                        title = titles[i][0];
                        break;
                    }
                }
                for (int i = 0; i < uuids.Count; i++)
                {
                    if (uuids[i][1].Contains(revTag))
                    {
                        uuid = uuids[i][0];
                        break;
                    }
                }

                for (int i = 0; i < descriptions.Count; i++)
                {
                    if (descriptions[i][1].Contains(revTag))
                    {
                        desc = descriptions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                var take = new Result
                {
                    UUID = uuid,
                    ID = rev.ID,
                    Start =  start,
                    Title = title,
                    Description = desc,
                    Remarks = remark,
                    End = end,
                    Props = props,
                  
                    Annotations = anns,

                };
                tem.Add(take);
            }

            return tem;
        }

        protected List<Risk> GetRisks(string ResultID, string FindingID)
        {
            var result = new List<Risk>();
            var label = "risk";
            var ids = GetDBData(string.Format("{0} id, {0} id", label), UserName, DOID);
            var uuids = GetDBData(string.Format("{0} id, uuid", label), UserName, DOID);
            var titles = GetDBData(string.Format("{0} id, title", label), UserName, DOID);
            var descriptions = GetDBData(string.Format("{0} id, description", label), UserName, DOID);
            var remarks = GetDBData(string.Format("{0} id, remarks", label), UserName, DOID);

            for (int i = 0; i < ids.Count; i++)
            {
                var name = ids[i][0];
                var tag = ids[i][1];
                if (!(tag.Contains(ResultID) && tag.Contains(FindingID)))
                    continue;

                var rev = new Risk
                {
                    ID = name,
                };

                result.Add(rev);
            }

            var tem = new List<Risk>();
            foreach (var rev in result)
            {
                var revTag = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\">", ResultID, FindingID, rev.ID);
                var tagStatus = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><risk id=\"{2}\"><risk-status>", ResultID, FindingID, rev.ID);

                var title = "";
                var desc = "";
                var remark = "";
                var uuid = "";

                
                var props = GetProps(rev.ID, string.Format("{0} id", label), revTag);
                var status = GetData("status", tagStatus, UserName, DOID);
                var anns = GetAnnotations(rev.ID, string.Format("{0} id", label), revTag);
                var riskmetrics = GetRiskMetrics(rev.ID, "risk id");
                var factors  = GetMitigatingFactors(rev.ID);
                var remeds = GetRemediations(rev.ID);
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i][1].Contains(revTag))
                    {
                        title = titles[i][0];
                        break;
                    }
                }
                for (int i = 0; i < uuids.Count; i++)
                {
                    if (uuids[i][1].Contains(revTag))
                    {
                        uuid = uuids[i][0];
                        break;
                    }
                }

                for (int i = 0; i < descriptions.Count; i++)
                {
                    if (descriptions[i][1].Contains(revTag))
                    {
                        desc = descriptions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                var take = new Risk
                {
                    UUID = uuid,
                    ID = rev.ID,                  
                    Title = title,
                    Description = desc,
                    Remarks = remark,
                    Props = props,                  
                    Annotations = anns,
                    RiskMetrics = riskmetrics,
                    MitigatingFactors = factors,
                    Remediations = remeds,
                    RiskStatus = status
                };
                tem.Add(take);
            }

            return tem;
        }

        protected List<Observation> GetObservations(string ResultID, string FindingID)
        {
            var result = new List<Observation>();
            var label = "observation";
            var ids = GetDBData(string.Format("{0} id, {0} id", label), UserName, DOID);
            var uuids = GetDBData(string.Format("{0} id, uuid", label), UserName, DOID);
            var titles = GetDBData(string.Format("{0} id, title", label), UserName, DOID);
            var descriptions = GetDBData(string.Format("{0} id, description", label), UserName, DOID);
            var remarks = GetDBData(string.Format("{0} id, remarks", label), UserName, DOID);


            for (int i = 0; i < ids.Count; i++)
            {
                var name = ids[i][0];
                var tag = ids[i][1];
                if (!(tag.Contains(ResultID) && tag.Contains(FindingID)))
                    continue;

                var rev = new Observation
                {
                    ID = name,
                };
                
                result.Add(rev);
            }

            var tem = new List<Observation>();
            foreach (var rev in result)
            {
                var revTag  = string.Format("<results id=\"{0}\"><finding id=\"{1}\"><observation id=\"{2}\">", ResultID, FindingID, rev.ID);
 
                var title = "";
                var desc = "";
                var remark = "";
                var uuid = "";

                var assessors = GetAssessors(rev.ID, "observation");
                var props = GetProps(rev.ID, string.Format("{0} id", label), revTag);
                 var methods = GetAuxillaries(rev.ID, "observation id", "observation method");
                var types = GetAuxillaries(rev.ID, "observation id", "observation type");
                var subjects = GetSubjectReferences(rev.ID, "observation id");
                var anns = GetAnnotations(rev.ID, string.Format("{0} id", label), revTag);
                var evidences = GetEvidences(rev.ID);
                var origins = GetOrigins(rev.ID, "observation id", "origin");
               
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i][1].Contains(revTag))
                    {
                        title = titles[i][0];
                        break;
                    }
                }
                for (int i = 0; i < uuids.Count; i++)
                {
                    if (uuids[i][1].Contains(revTag))
                    {
                        uuid = uuids[i][0];
                        break;
                    }
                }

                for (int i = 0; i < descriptions.Count; i++)
                {
                    if (descriptions[i][1].Contains(revTag))
                    {
                        desc = descriptions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                var take = new Observation
                {
                    UUID = uuid,
                    ID = rev.ID,
                    Assessors = assessors,
                    Title = title,
                    Description = desc,
                    Remarks = remark,
                    ObservationMethod = methods,
                    ObservationType = types,
                    Props = props,
                    SubjectReferences = subjects,
                    Annotations = anns,
                    RelevantEvidences = evidences,
                    Origins = origins
                };
                tem.Add(take);
            }

            return tem;
        }


        protected List<TestMethod> GetTestMethods()
        {
            var result = new List<TestMethod>();
            var label = "test-method";
            var ids = GetDBData(string.Format("{0} id, {0} id", label), UserName, DOID);
            var uuids =  GetDBData(string.Format("{0} id, uuid", label), UserName, DOID);
            var titles = GetDBData(string.Format("{0} id, title", label), UserName, DOID);
            var descriptions = GetDBData(string.Format("{0} id, description", label), UserName, DOID);
            var remarks = GetDBData(string.Format("{0} id, remarks", label), UserName, DOID);
            var compares = GetDBData(string.Format("{0} id, compare-to", label), UserName, DOID);

           

            for (int i = 0; i < ids.Count; i++)
            {
                var name = ids[i][0];
                var tag = ids[i][1];
                var rev = new TestMethod
                {
                    ID = name,
                };
                result.Add(rev);
            }

            var tem = new List<TestMethod>();
            foreach (var rev in result)
            {
                var revTag = string.Format("<assessment-activities><test-method id=\"{0}\">", rev.ID);
                var compare = "";
                var title = "";
                var desc = "";
                var remark = "";
                var uuid = "";


                var props = GetProps(rev.ID, string.Format("{0} id", label), revTag);
                var links = GetLinks(rev.ID, string.Format("{0} id", label), revTag);
                var anns = GetAnnotations(rev.ID, string.Format("{0} id", label), revTag);
                var TestSteps = GetTestSteps(rev.ID);
                for (int i = 0; i < compares.Count; i++)
                {
                    if (compares[i][1].Contains(revTag))
                    {
                        compare = compares[i][0];
                        break;
                    }
                }
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i][1].Contains(revTag))
                    {
                        title = titles[i][0];
                        break;
                    }
                }
                for (int i = 0; i < uuids.Count; i++)
                {
                    if (uuids[i][1].Contains(revTag))
                    {
                       uuid = uuids[i][0];
                        break;
                    }
                }

                for (int i = 0; i < descriptions.Count; i++)
                {
                    if (descriptions[i][1].Contains(revTag))
                    {
                        desc = descriptions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }


                
               
                var take = new TestMethod
                {
                    UUID = uuid,
                    ID = rev.ID,
                    TestSteps = TestSteps,
                    Title = title,
                    Description = desc,
                    Remarks = remark,
                    CompareTo = compare,
                    Props = props,
                    Links = links,
                    Annotations = anns,
                 
                };
                tem.Add(take);
            }

            return tem;
        }

       

        protected List<SAPUser> GetSAPUsers(string otherlabel= "<assessment-subject><local-definitions>")
        {
            var result = new List<SAPUser>();
            var label = "user";
            var uuids = GetDBData(string.Format("{0} id, uuid", label), UserName, DOID);
            var ids = GetHeaderAndTag(string.Format("{0} id, {0} id", label), UserName, DOID);
            var shortNames = GetDBData(string.Format("{0} id, short-name", label), UserName, DOID);
            var titles = GetDBData(string.Format("{0} id, title", label), UserName, DOID);
            var descriptions = GetDBData(string.Format("{0} id, description", label), UserName, DOID);        
            var remarks = GetDBData(string.Format("{0} id, remarks", label), UserName, DOID);

            var Roles = GetRoles();

            for (int i = 0; i < ids.Count; i++)
            {
                var name = ids[i][0];
                var tag = ids[i][1];
                var rev = new SAPUser
                {
                    ID = name,
                    MainTag = tag
                };
                result.Add(rev);
            }

            var tem = new List<SAPUser>();
            foreach (var rev in result)
            {
                var revTag = rev.MainTag; //string.Format("<assessment-subject><local-definitions><user id=\"{0}\">", rev.ID);
                var name = "";
                var title = "";
                var desc = "";
                var remark = "";
                var uuid = "";

                var props = GetProps(rev.ID, string.Format("{0} id", label), revTag);
                var links = GetLinks(rev.ID, string.Format("{0} id", label), revTag);
                var anns = GetAnnotations(rev.ID, string.Format("{0} id", label), revTag);

                for (int i = 0; i < shortNames.Count; i++)
                {
                    if (shortNames[i][1].Contains(revTag))
                    {
                        name = shortNames[i][0];
                        break;
                    }
                }
                for (int i = 0; i < uuids.Count; i++)
                {
                    if (uuids[i][1].Contains(revTag))
                    {
                        uuid = uuids[i][0];
                        break;
                    }
                }
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i][1].Contains(revTag))
                    {
                        title = titles[i][0];
                        break;
                    }
                }
                for (int i = 0; i < descriptions.Count; i++)
                {
                    if (descriptions[i][1].Contains(revTag))
                    {
                        desc = descriptions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

               
                var RespRoles = GetUserRoles(rev.ID, Roles, otherlabel);
                var Func = GetAuthorizedPrivileges(rev.ID, otherlabel);
                var take = new SAPUser
                {
                    UUID= uuid,
                    ID = rev.ID,
                    ShortName = name,
                    Title = title,
                    Description = desc,
                    Remarks = remark,
                    AuthorizedPrivileges= Func,
                    Props = props,
                    Links = links,
                    Annotations = anns,
                    Roles = RespRoles
                };
                tem.Add(take);
            }

            return tem;
        }

        protected List<SAPUser> GetLiteSAPUsers()
        {
            var result = new List<SAPUser>();
            var label = "user";
            var ids = GetHeaderAndTag(string.Format("{0} id, {0} id", label), UserName, DOID);
            var uuids = GetDBData(string.Format("{0} id, uuid", label), UserName, DOID);
            var shortNames = GetDBData(string.Format("{0} id, short-name", label), UserName, DOID);
            var titles = GetDBData(string.Format("{0} id, title", label), UserName, DOID);
            var descriptions = GetDBData(string.Format("{0} id, description", label), UserName, DOID);
            var remarks = GetDBData(string.Format("{0} id, remarks", label), UserName, DOID);

           // var Roles = GetRoles();

            for (int i = 0; i < ids.Count; i++)
            {
                var name = ids[i][0];
                var tag = ids[i][1];
                var rev = new SAPUser
                {
                    ID = name,
                    MainTag = tag
                };
                result.Add(rev);
            }

            var tem = new List<SAPUser>();
            foreach (var rev in result)
            {
                var revTag = rev.MainTag; //string.Format("<assessment-subject><local-definitions><user id=\"{0}\">", rev.ID);
                var name = "";
                var title = "";
                var desc = "";
                var remark = "";
                var uuid = "";


               // var props = GetProps(rev.ID, string.Format("{0} id", label), revTag);
               // var links = GetLinks(rev.ID, string.Format("{0} id", label), revTag);
               // var anns = GetAnnotations(rev.ID, string.Format("{0} id", label), revTag);

                for (int i = 0; i < shortNames.Count; i++)
                {
                    if (shortNames[i][1].Contains(revTag))
                    {
                        name = shortNames[i][0];
                        break;
                    }
                }
                for (int i = 0; i < uuids.Count; i++)
                {
                    if(uuids[i][1].Contains(revTag))
                    {
                        uuid = uuids[i][0];
                        break;
                    }
                }
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i][1].Contains(revTag))
                    {
                        title = titles[i][0];
                        break;
                    }
                }
                for (int i = 0; i < descriptions.Count; i++)
                {
                    if (descriptions[i][1].Contains(revTag))
                    {
                        desc = descriptions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }


              //  var RespRoles = GetUserRoles(rev.ID, Roles, "<assessment-subject><local-definitions>");
              //  var Func = GetAuthorizedPrivileges(rev.ID);
                var take = new SAPUser
                {
                    UUID = uuid,
                    ID = rev.ID,
                    ShortName = name,
                    Title = title,
                    Description = desc,
                    Remarks = remark,
                    
                };
                tem.Add(take);
            }

            return tem;
        }

        protected List<Component> GetComponents(string tagCore)
        {
            var result = new List<Component>();
            var label = "component";
            var ids = GetDBData(string.Format("{0} id, {0} id", label), UserName, DOID);
            var types = GetDBData(string.Format("{0} id, component-type", label), UserName, DOID);
            var titles = GetDBData(string.Format("{0} id, title", label), UserName, DOID);
            var uuIds = GetDBData(string.Format("{0} id, uuid", label), UserName, DOID);
            var descriptions = GetDBData(string.Format("{0} id, description", label), UserName, DOID);
            var states = GetDBData(string.Format("{0} id, status state", label), UserName, DOID);
            var statesRemarks = GetDBData(string.Format("{0} id, status state remarks", label), UserName, DOID);
            var remarks = GetDBData(string.Format("{0} id, remarks", label), UserName, DOID);

            var ResponsibleParties = GetResponsibleParties();


            for (int i = 0; i < ids.Count; i++)
            {
                var name = ids[i][0];
                var tag = ids[i][1];
                if (!tag.Contains(tagCore))
                    continue;

                var rev = new Component
                {
                    ID = name,
                };
                result.Add(rev);
            }

            var tem = new List<Component>();
            foreach (var rev in result)
            {
                var revTag = string.Format("{1}<component id=\"{0}\">", rev.ID, tagCore);
                var type = "";
                var title = "";
                var desc = "";
                var remark = "";
                var state = "";
                var stateRem = "";
                var uuid = "";

                var props = GetProps(rev.ID, string.Format("{0} id", label), revTag);
                var links = GetLinks(rev.ID, string.Format("{0} id", label), revTag);
                var anns = GetAnnotations(rev.ID, string.Format("{0} id", label), revTag);

                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i][1].Contains(rev.ID))
                    {
                        type = types[i][0];
                        break;
                    }
                }
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i][1].Contains(revTag))
                    {
                        title = titles[i][0];
                        break;
                    }
                }

                for (int i = 0; i < uuIds.Count; i++)
                {
                    if (uuIds[i][1].Contains(revTag))
                    {
                        uuid = uuIds[i][0];
                        break;
                    }
                }
                for (int i = 0; i < descriptions.Count; i++)
                {
                    if (descriptions[i][1].Contains(revTag))
                    {
                        desc = descriptions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                for (int i = 0; i < states.Count; i++)
                {
                    if (states[i][1].Contains(revTag))
                    {
                        state = states[i][0];
                        break;
                    }
                }

                for (int i = 0; i < statesRemarks.Count; i++)
                {
                    if (statesRemarks[i][1].Contains(revTag))
                    {
                        stateRem = statesRemarks[i][0];
                        break;
                    }
                }
                var RespRoles = GetComponentRespRoles(rev.ID, ResponsibleParties,tagCore);

                var take = new Component
                {
                    ID = rev.ID,
                    UUID = uuid,
                    ComponentType = type,
                    Title = title,
                    Description = desc,
                    Remarks = remark,
                    State = state,
                    StateRemarks = stateRem,
                    Props = props,
                    Links = links,
                    Annotations = anns,
                    ResponsibleRoles = RespRoles
                };
                tem.Add(take);
            }

            return tem;
        }

        protected List<Component> GetComponents()
        {
            var result = new List<Component>();
            var label = "component";
            var ids = GetDBData(string.Format("{0} id, {0} id", label), UserName, DOID);
            var types = GetDBData(string.Format("{0} id, component-type", label), UserName, DOID);
            var titles = GetDBData(string.Format("{0} id, title", label), UserName, DOID);
            var uuIds = GetDBData(string.Format("{0} id, uuid", label), UserName, DOID);
            var descriptions = GetDBData(string.Format("{0} id, description", label), UserName, DOID);
            var states = GetDBData(string.Format("{0} id, status state", label), UserName, DOID);
            var statesRemarks = GetDBData(string.Format("{0} id, status state remarks", label), UserName, DOID);
            var remarks = GetDBData(string.Format("{0} id, remarks", label), UserName, DOID);

            var ResponsibleParties = GetResponsibleParties();


            for (int i = 0; i < ids.Count; i++)
            {

                var name = ids[i][0];
                var tag = ids[i][1];
                if (!tag.Contains("<assessment-subject><local-definitions>"))
                    continue;

                var rev = new Component
                {
                    ID = name,
                };
                result.Add(rev);
            }

            var tem = new List<Component>();
            foreach (var rev in result)
            {
                var revTag = string.Format("<assessment-subject><local-definitions><component id=\"{0}\">", rev.ID);
                var type = "";
                var title = "";
                var desc = "";
                var remark = "";
                var state = "";
                var stateRem = "";
                var guid = "";

                var props = GetProps(rev.ID, string.Format("{0} id", label), revTag);
                var links = GetLinks(rev.ID, string.Format("{0} id", label), revTag);
                var anns = GetAnnotations(rev.ID, string.Format("{0} id", label), revTag);

                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i][1].Contains(rev.ID))
                    {
                        type = types[i][0];
                        break;
                    }
                }
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i][1].Contains(revTag))
                    {
                        title = titles[i][0];
                        break;
                    }
                }
                for (int i = 0; i < descriptions.Count; i++)
                {
                    if (descriptions[i][1].Contains(revTag))
                    {
                        desc = descriptions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < uuIds.Count; i++)
                {
                    if (uuIds[i][1].Contains(revTag))
                    {
                        guid = uuIds[i][0];
                        break;
                    }
                }


                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                for (int i = 0; i < states.Count; i++)
                {
                    if (states[i][1].Contains(revTag))
                    {
                        state = states[i][0];
                        break;
                    }
                }

                for (int i = 0; i < statesRemarks.Count; i++)
                {
                    if (statesRemarks[i][1].Contains(revTag))
                    {
                        stateRem = statesRemarks[i][0];
                        break;
                    }
                }
                var RespRoles = GetComponentRespRoles(rev.ID, ResponsibleParties, "<assessment-subject><local-definitions>");

                var take = new Component
                {
                    ID = rev.ID,
                    UUID = guid,
                    ComponentType = type,
                    Title = title,
                    Description = desc,
                    Remarks = remark,
                    State = state,
                    StateRemarks = stateRem,
                    Props = props,
                    Links = links,
                    Annotations = anns,
                    ResponsibleRoles = RespRoles
                };
                tem.Add(take);
            }

            return tem;
        }

        protected List<InventoryItem> GetInventoryItems()
        {
            var result = new List<InventoryItem>();
            var label = "inventory-item id";
            var ids = GetDBData(string.Format("{0}, {0}", label), UserName, DOID);
            var assetIds = GetDBData(string.Format("{0}, asset-id", label), UserName, DOID);
            var uuIds = GetDBData(string.Format("{0}, uuid", label), UserName, DOID);
            var descriptions = GetDBData(string.Format("{0}, description", label), UserName, DOID);
                      var remarks = GetDBData(string.Format("{0}, remarks", label), UserName, DOID);

            var ResponsibleParties = GetResponsibleParties();
            var ImplementedComponents = GetImplementedComponents();

            for (int i = 0; i < ids.Count; i++)
            {
                var name = ids[i][0];
                var tag = ids[i][1];
                var rev = new InventoryItem
                {
                    ID = name,
                };
                result.Add(rev);
            }

            var tem = new List<InventoryItem>();
            foreach (var rev in result)
            {
                var revTag = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\">", rev.ID);

                var desc = "";
                var remark = "";
                var asset = "";
                var uuid = "";

                var props = GetProps(rev.ID, string.Format("{0}", label), revTag);
                var links = GetLinks(rev.ID, string.Format("{0}", label), revTag);
                var anns = GetAnnotations(rev.ID, string.Format("{0}", label), revTag);

                for (int i = 0; i < descriptions.Count; i++)
                {
                    if (descriptions[i][1].Contains(revTag))
                    {
                        desc = descriptions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }

                for (int i = 0; i < assetIds.Count; i++)
                {
                    if (assetIds[i][1].Contains(rev.ID))
                    {
                        asset = assetIds[i][0];
                        break;
                    }
                }
                for (int i = 0; i < uuIds.Count; i++)
                {
                    if (uuIds[i][1].Contains(rev.ID))
                    {
                        uuid = uuIds[i][0];
                        break;
                    }
                }
                var RespRoles = GetInventoryItemRespRoles(rev.ID, ResponsibleParties);

                var ImplementedComps = GetInventoryItemComponents(rev.ID, ImplementedComponents);

                var take = new InventoryItem
                {
                    ID = rev.ID,
                    UUID = uuid,
                    AssetID = asset,
                    Description = desc,
                    Remarks = remark,
                    Props = props,
                    Links = links,
                    Annotations = anns,
                    ResponsibleRoles = RespRoles, 
                    implementedComponents = ImplementedComps
                };
                tem.Add(take);
            }

            return tem;
        }

        protected List<ImplementedComponent> GetImplementedComponents()
        {
            var result = new List<ImplementedComponent>();
            var label = "implemented-component component-id";
            var ids = GetDBData(string.Format("{0}, {0}", label), UserName, DOID);
            var remarks = GetDBData(string.Format("{0}, remarks", label), UserName, DOID);
            var uuids = GetDBData(string.Format("{0}, uuid", label), UserName, DOID);
            var uses = GetDBData(string.Format("{0}, use", label), UserName, DOID);

            var ResponsibleParties = GetResponsibleParties();
            for (int i = 0; i < ids.Count; i++)
            {
                var name = ids[i][0];
                
                var rev = new ImplementedComponent
                {
                    ComponentID = name,
                };
                result.Add(rev);
            }

            var tem = new List<ImplementedComponent>();
            foreach (var rev in result)
            {
                var revTag = string.Format("<assessment-subject><local-definitions><implemented-component component-id=\"{0}\">", rev.ComponentID);

                var remark = "";
                var use = "";
                var uuid = ""; 
                var props = GetProps(rev.ComponentID, string.Format("{0}", label), revTag);
                var links = GetLinks(rev.ComponentID, string.Format("{0}", label), revTag);
                var anns = GetAnnotations(rev.ComponentID, string.Format("{0}", label), revTag);

              
               
                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }
                for (int i = 0; i < uuids.Count; i++)
                {
                    if (uuids[i][1].Contains(revTag))
                    {
                        uuid = uuids[i][0];
                        break;
                    }
                }

                for (int i = 0; i < uses.Count; i++)
                {
                    if (uses[i][1].Contains(rev.ComponentID))
                    {
                        use = uses[i][0];
                        break;
                    }
                }

                var RespRoles = GetImplementedComponentRespRoles(rev.ComponentID, ResponsibleParties);

                var take = new ImplementedComponent
                {
                    ComponentID = rev.ComponentID,
                    UUID = uuid,
                    Use = use,
                    Remarks = remark,
                    ResponsibleRoles = RespRoles,
                    Props = props,
                    Links = links,
                    Annotations = anns
                };
                tem.Add(take);
            }

            return tem;
        }

        protected List<ResponsibleParty> GetImplementedComponentRespRoles(string componentid, List<ResponsibleParty> ResponsibleParties)
        {
            var result = new List<ResponsibleParty>();
            var tagroleid = string.Format("<assessment-subject><local-definitions><implemented-component component-id=\"{0}\">", componentid);
            var allIds = GetDBData("implemented-component component-id, responsible-role role-id", UserName, DOID);
           
            var goodIds = new List<string>();
            foreach (var x in allIds)
            {
                if (x[1].Contains(tagroleid))
                {
                    goodIds.Add(x[0]);
                }
            }

            foreach (var resrole in ResponsibleParties)
            {
                if (goodIds.Contains(resrole.RoleID))
                    result.Add(resrole);
            }

            return result;
        }

        protected List<ResponsibleParty> GetComponentRespRoles(string componentid, List<ResponsibleParty> ResponsibleParties, string rootTag)
        {
            var result = new List<ResponsibleParty>();
            var tagroleid = string.Format("{1}<component id=\"{0}\">", componentid, rootTag);
            var allIds = GetDBData("component id, responsible-role role-id", UserName, DOID);

            var goodIds = new List<string>();
            foreach (var x in allIds)
            {
                if (x[1].Contains(tagroleid))
                {
                    goodIds.Add(x[0]);
                }
            }

            foreach (var resrole in ResponsibleParties)
            {
                if (goodIds.Contains(resrole.RoleID))
                    result.Add(resrole);
            }

            return result;
        }

        protected List<Role> GetUserRoles(string componentid, List<Role> Roles, string rootTag)
        {
            var result = new List<Role>();
            var tagroleid = string.Format("{1}<user id=\"{0}\">", componentid, rootTag);
            var allIds = GetDBData("user id, role-id", UserName, DOID);

            var goodIds = new List<string>();
            foreach (var x in allIds)
            {
                if (x[1].Contains(tagroleid))
                {
                    goodIds.Add(x[0]);
                }
            }

            foreach (var resrole in Roles)
            {
                if (goodIds.Contains(resrole.RoleID))
                    result.Add(resrole);
            }

            return result;
        }

        protected List<ResponsibleParty> GetInventoryItemRespRoles(string componentid, List<ResponsibleParty> ResponsibleParties)
        {
            var result = new List<ResponsibleParty>();
            var tagroleid = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\">", componentid);
            var allIds = GetDBData("inventory-item id, responsible-role role-id", UserName, DOID);

            var goodIds = new List<string>();
            foreach (var x in allIds)
            {
                if (x[1].Contains(tagroleid))
                {
                    goodIds.Add(x[0]);
                }
            }

            foreach (var resrole in ResponsibleParties)
            {
                if (goodIds.Contains(resrole.RoleID))
                    result.Add(resrole);
            }

            return result;
        }

        protected List<ImplementedComponent> GetInventoryItemComponents(string itemId, List<ImplementedComponent> Components)
        {
            var result = new List<ImplementedComponent>();
            var tagroleid = string.Format("<assessment-subject><local-definitions><inventory-item id=\"{0}\">", itemId);
            var allIds = GetDBData("inventory-item id, implemented-component component-id", UserName, DOID);

            var goodIds = new List<string>();
            foreach (var x in allIds)
            {
                if (x[1].Contains(tagroleid))
                {
                    goodIds.Add(x[0]);
                }
            }

            foreach (var resrole in Components)
            {
                if (goodIds.Contains(resrole.ComponentID))
                    result.Add(resrole);
            }

            return result;
        }


        protected List<Subject> GetSubjects(string label)
        {
            var result = new List<Subject>();


            var names = GetDBData(string.Format("{0}-subject name, {0}-subject name", label), UserName, DOID);
            var classes = GetDBData(string.Format("{0}-subject name, class", label), UserName, DOID);
            var alls = GetDBData(string.Format("{0}-subject name, all", label), UserName, DOID);
            var descriptions = GetDBData(string.Format("{0}-subject name, description", label), UserName, DOID);
            
            var remarks = GetDBData(string.Format("{0}-subject name, remarks", label), UserName, DOID);

            for (int i = 0; i < names.Count; i++)
            {
                var name = names[i][0];
                var tag = names[i][1];
                var rev = new Subject
                {
                    Name = name,
                };
                result.Add(rev);
            }

            var tem = new List<Subject>();
            foreach (var rev in result)
            {
                var revTag = string.Format("<assessment-subject><{1}-subject name=\"{0}\">", rev.Name, label);
                var Class = "";
                var all = "";
                var desc = "";
                var remark = "";
               
                var props = GetProps(rev.Name, string.Format("{0}-subject name",label), revTag);
                var links = GetLinks(rev.Name, string.Format("{0}-subject name", label), revTag);
                var anns = GetAnnotations(rev.Name, string.Format("{0}-subject name", label), revTag);

                for (int i = 0; i < classes.Count; i++)
                {
                    if (names[i][1].Contains(rev.Name))
                    {
                        Class = classes[i][0];
                        break;
                    }
                }
                for (int i = 0; i < alls.Count; i++)
                {
                    if (alls[i][1].Contains(revTag))
                    {
                        all = alls[i][0];
                        break;
                    }
                }
                for (int i = 0; i < descriptions.Count; i++)
                {
                    if (descriptions[i][1].Contains(revTag))
                    {
                        desc = descriptions[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (remarks[i][1].Contains(revTag))
                    {
                         remark = remarks[i][0];
                        break;
                    }
                }

               


                var take = new Subject
                {
                    Name = rev.Name,
                    Class = Class,
                    All = all,
                    Description = desc,
                    Remarks = remark,
                    Props = props,
                    Links = links,
                    Annotations = anns
                };
                tem.Add(take);
            }

            return tem;
        }


        protected List<Role> GetRoles()
        {
            var result = new List<Role>();
            var titles = new List<Item>();
          

            var roles = GetHeaderAndTag("role id, role id", UserName, DOID);
            var titleSe = GetDBData("role id, title", UserName, DOID);
            var DescSe = GetDBData("role id, desc", UserName, DOID);
            var shortNames = GetDBData("role id, short-name", UserName, DOID);
            var remarks = GetDBData("role id, remarks", UserName, DOID);

            for (int i=0; i< roles.Count; i++)
            {
                var roleid = roles[i][0];
                var tag = roles[i][1];
                var role = new Role
                {
                    RoleID = roleid,
                    ElementTag = tag
                };
                result.Add(role);                
            }

            for(int j = 0; j < titleSe.Count; j++)
            {

                var roletitle = titleSe[j][0];
                var tag = titleSe[j][1];
                var roleItem = new Item
                {
                    Value = roletitle,
                    XPath = tag
                };
                titles.Add(roleItem);
               
            }

           
            var tem = new List<Role>();
            foreach (var role in result)
            {
                var title = "";
                var desc = "";
                var shortName = "";
                var remark = "";
                var props = GetRoleProps(role.RoleID);
                for (int i = 0; i < titles.Count; i++)
                {
                    if (role.ElementTag + "<title>" == titles[i].XPath)
                    {
                        title = titles[i].Value;
                        break;
                    }
                }
                for (int i = 0; i < DescSe.Count; i++)
                {
                    if (role.ElementTag + "<desc>" == DescSe[i][1])
                    {
                        desc = DescSe[i][0];
                        break;
                    }
                }
                for (int i = 0; i < shortNames.Count; i++)
                {
                    if (role.ElementTag + "<short-name>" == shortNames[i][1])
                    {
                        shortName = shortNames[i][0];
                        break;
                    }
                }

                for (int i = 0; i < remarks.Count; i++)
                {
                    if (role.ElementTag + "<remarks>" == remarks[i][1])
                    {
                        remark = remarks[i][0];
                        break;
                    }
                }


                var take = new Role
                {
                    Title = new Item { Value = title, XPath = role.ElementTag + "<title>" },
                    RoleID = role.RoleID,
                    Description = desc,
                    RoleTitle = title,
                    ShortName =  shortName,
                    Remarks = remark,
                    Props = props
                };
                tem.Add(take);
            }

            return tem;
        }

        protected string GetData(string eltName, string tag, string username, int doid)
        {
            string result = "";
           
            var deidList = GetDEID(doid, username, eltName, tag, 1);
           
            if (deidList.Count > 0)
            {
                var deid = deidList.FirstOrDefault();
                result = GetElementDetail(deid);
            }


            return result;
        }

        protected string GetData(string eltName, string tag, Type eltType, string uid, int doid)
        {
            string result = "";

            var deidList = GetDEID(doid, uid, eltName, eltType , tag, 1);

            if (deidList.Count > 0)
            {
                var deid = deidList.FirstOrDefault();
                result = GetElementDetail(deid);
            }


            return result;
        }
        protected string GetEmptyTabSpaces(int nber)
        {
            var res = "";
            for (int i = 0; i < nber; i++)
            {
                res += "   ";
            }
            return res;
        }

        protected string EndTag(string tag)
        {
            var ind = tag.LastIndexOf("<");
            var end = tag.Substring(ind, tag.Length - ind + 1);
            return end;
        }


        protected List<int> GetDEID(int doid, string uid, string eltName, Type eltTypeId, int active)
        {


            var realEltTypeId = GetElementTypeId(eltTypeId);
            var elementHeader = new ElementHeader(eltName, realEltTypeId, "", "", "", active);

            var res = GetDEID(doid, uid, eltName, (int)elementHeader.TypeId, active);

            return res;
        }

       

        protected List<int> GetDEID(int doid, string uid, string eltName, Type eltTypeId, string tag, int active)
        {
            

            var realEltTypeId = GetElementTypeId(eltTypeId);
            var elementHeader = new ElementHeader(eltName, realEltTypeId, tag, "", "", active);

            var iid = (int)elementHeader.TypeId;

            var   res = GetDEID(doid, uid, eltName, tag, active);

            return res;
        }

        protected List<int> GetDEID(int doid, string userName, string eltName, string eltTag, int active)
        {
            var res = new List<int>();

            SqlParameter[] oParams = new SqlParameter[5];
            oParams[0] = new SqlParameter("UserName", SqlDbType.VarChar, 255);
            oParams[0].Value = userName;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = doid;
            oParams[2] = new SqlParameter("ElementName", SqlDbType.NVarChar, 255);
            oParams[2].Value = eltName;


            oParams[3] = new SqlParameter("ElementTag", SqlDbType.NVarChar, 255);
            oParams[3].Value = eltTag;

            oParams[4] = new SqlParameter("Active", SqlDbType.Int, 10);
            oParams[4].Value = active;


            DataSet ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocHeader_GetDEID]", CommandType.StoredProcedure, oParams);

            int x = 0;
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    x = (int)ds.Tables[0].Rows[i].ItemArray[0];
                    res.Add(x);
                }
            }
            return res;
        }

        protected List<int> GetDEID(int doid, string username, string eltName, int eltTypeId, int active)
        {
            var res = new List<int>();

            SqlParameter[] oParams = new SqlParameter[5];
            oParams[0] = new SqlParameter("UserName", SqlDbType.VarChar, 255);
            oParams[0].Value = username;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = doid;
            oParams[2] = new SqlParameter("ElementName", SqlDbType.NVarChar, 255);
            oParams[2].Value = eltName;


            oParams[3] = new SqlParameter("ElementTypeID", SqlDbType.Int, 10);
            oParams[3].Value = eltTypeId;

            oParams[4] = new SqlParameter("Active", SqlDbType.Int, 10);
            oParams[4].Value = active;
          

            DataSet ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocHeader_GetDEID]", CommandType.StoredProcedure, oParams);

            int x = 0;
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    x = (int)ds.Tables[0].Rows[i].ItemArray[0];
                    res.Add(x);
                }
            }
            return res;
        }

        protected List<int> GetDEID(int doid, int systemId, string eltName, int eltTypeId, string tag, int active)
        {
            var res = new List<int>();

            var uid = GetUserIdFromSystemId(systemId);
            var username = GetUsernameFromID(uid);

            SqlParameter[] oParams = new SqlParameter[6];
            oParams[0] = new SqlParameter("UserName", SqlDbType.VarChar, 255);
            oParams[0].Value = username;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = doid;
            oParams[2] = new SqlParameter("ElementName", SqlDbType.NVarChar, 255);
            oParams[2].Value = eltName;


            oParams[3] = new SqlParameter("ElementTypeID", SqlDbType.Int, 10);
            oParams[3].Value = eltTypeId;

            oParams[4] = new SqlParameter("Active", SqlDbType.Int, 10);
            oParams[4].Value = active;
            oParams[5] = new SqlParameter("ElementTag", SqlDbType.NVarChar, 255);
            oParams[5].Value = tag;

            DataSet ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocHeader_GetDEID]", CommandType.StoredProcedure, oParams);

            int x = 0;
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    x = (int)ds.Tables[0].Rows[i].ItemArray[0];
                    res.Add(x);
                }
            }
            return res;
        }

  

        protected List<int> GetDEID(int doid, string username, string eltName, int eltTypeId, string tag, int active)
        {
            var res = new List<int>();

            SqlParameter[] oParams = new SqlParameter[6];
            oParams[0] = new SqlParameter("UserName", SqlDbType.VarChar, 255);
            oParams[0].Value = username;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = doid;
            oParams[2] = new SqlParameter("ElementName", SqlDbType.NVarChar, 255);
            oParams[2].Value = eltName;


            oParams[3] = new SqlParameter("ElementTypeID", SqlDbType.Int, 10);
            oParams[3].Value = eltTypeId;

            oParams[4] = new SqlParameter("Active", SqlDbType.Int, 10);
            oParams[4].Value = active;
            oParams[5] = new SqlParameter("ElementTag", SqlDbType.NVarChar, 255);
            oParams[5].Value = tag;

            DataSet ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocHeader_GetDEID]", CommandType.StoredProcedure, oParams);

            int x = 0;
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    x = (int)ds.Tables[0].Rows[i].ItemArray[0];
                    res.Add(x);
                }
            }
            return res;
        }
        protected bool ContainBadChar(string Text)
        {
            var bad = new List<string> { " ", "?", ":", ";", "!", "=" };
            var res= false;

            for(int i=0; i<bad.Count; i++)
            {
                res = res || Text.Contains(bad[i]); 
            }

            return res;
        }

        protected void FaultyCacheBackHomeSAR()
        {
            if (Cache["username"] == null)
            {

                Response.Redirect(@"~/PagesSAR/HomeSAR.aspx");
            }
            if (Cache["SystemId"] == null)
            {
                Response.Redirect(@"~/PagesSAR/HomeSAR.aspx");
            }
            if (Cache["doid"] == null)
            {
                Response.Redirect(@"~/PagesSAR/HomeSAR.aspx");
            }
        }


        protected void FaultyCacheBackHomeSAP()
        {
            if (Cache["username"] == null)
            {
              
                Response.Redirect(@"~/PagesSAP/SAPHome.aspx");
            }
            if (Cache["SystemId"] == null)
            {
                Response.Redirect(@"~/PagesSAP/SAPHome.aspx");
            }
            if (Cache["doid"] == null)
            {
                Response.Redirect(@"~/PagesSAP/SAPHome.aspx");
            }
        }

        protected void ErrorCache()
        {
            if (Cache["username"] == null)
            {
                var mes = string.Format("No UserName, please select a user.");
                throw new Exception(mes);
            }
            if (Cache["SystemId"] == null)
            {
                var mes = string.Format("No System is selected. Please select or define a system.");
                throw new Exception(mes);
            }
            if (Cache["doid"] == null)
            {
                var mes = string.Format("No Document is selected. Please select an existing document or create a new one.");
                throw new Exception(mes);
            }
        }

        protected string ErrorProcessing(string eltName, string value)
        {
            if(eltName=="name" || eltName =="class" || eltName == "rel")
            {
                if(ContainBadChar(value))
                {
                    var mes = string.Format("OSCAL Validation Error: The entry {0} is not a proper NCName type. \n", value);
                    return mes;
                }
                if(value == "")
                {
                    var mes = string.Format("OSCAL Validation Error: An entry of type name, class or rel cannot be empty. \n");
                    return mes;
                }
            }
            if(eltName=="ns" || eltName.Contains("uri"))
            {
                if (value.Contains("/") || value.Contains(@"\") || value.Contains("."))
                {
                }
                else
                {
                    var mes = string.Format("OSCAL Validation Error: The entry {0} is not a proper URI type. \n", eltName);
                    return mes;

                }
                if (value == "")
                {
                    var mes = string.Format("OSCAL Validation Error: An entry of type {0} cannot be empty. \n", eltName);
                    return mes;
                }

            }
            return "";
        }
        protected string InsertElementToDataBase(int doid, int uid, string eltName, Type eltTypeId, string tag, string desc, string eltDetail, int active)
        {
            string errorMessage="";
            errorMessage= ErrorProcessing(eltName, eltDetail);

            if (errorMessage.Length > 0)
                return errorMessage;

            var realEltTypeId = GetElementTypeId(eltTypeId);
            var elementHeader = new ElementHeader(eltName, realEltTypeId, tag, desc, eltDetail, active);

            var deidList = new List<int>();


            if(active ==0)
                deidList = GetDEID(doid, uid, eltName, (int)elementHeader.TypeId, tag, 1);
            else
               deidList = GetDEID(doid, uid, eltName, (int)elementHeader.TypeId, tag, active);

            if (deidList.Count > 0)
            {
                //var deid = deidList.FirstOrDefault();

                foreach (var deid in deidList)
                {


                    var eltheader = AddHeaderElement(doid, uid, deid, elementHeader.Name, (int)elementHeader.TypeId, elementHeader.Tag, elementHeader.Desc, elementHeader.Active);

                    var eltdetail = AddElementHeaderAndValue(deid, doid, elementHeader.Detail, uid, elementHeader.Active);
                }
            }
            else
            {
                var eltheader = AddHeaderElement(doid, uid, elementHeader.Name, (int)elementHeader.TypeId, elementHeader.Tag, elementHeader.Desc, elementHeader.Active);
                var deid = int.Parse(eltheader.Tables[0].Rows[0].ItemArray[0].ToString());
                var eltdetail = AddElementHeaderAndValue(deid, doid, elementHeader.Detail, uid, elementHeader.Active);
               
            }

            return errorMessage;

        }


        protected void InsertElementToDataBase(int doid, int uid, string eltName, Type eltTypeId, string tag, string desc, List<string> eltDetails, int active)
        {
            var realEltTypeId = GetElementTypeId(eltTypeId);

            var elementHeader = new ElementHeader(eltName, realEltTypeId, tag, desc, "", active);


            var deidList = new List<int>();




            deidList = GetDEID(doid, uid, eltName, (int)elementHeader.TypeId, tag, active);

            int p = 0;
            if (deidList.Count > 0)
            {   // erase existing entries

                for (int i = 0; i < deidList.Count; i++)
                {

                    AddHeaderElement(doid, uid, deidList[i], elementHeader.Name, (int)elementHeader.TypeId, elementHeader.Tag, elementHeader.Desc, elementHeader.Active);
                    AddElementHeaderAndValue(deidList[i], doid, "", uid, elementHeader.Active);
                    if (i < eltDetails.Count)
                    {
                        p++;
                        AddElementHeaderAndValue(deidList[i], doid, eltDetails[i], uid, elementHeader.Active);
                    }


                }
                for (int i = p; i < eltDetails.Count; i++)
                {
                    var elementHeader0 = new ElementHeader(eltName, realEltTypeId, tag, desc, eltDetails[i], active);
                    var eltheader = AddHeaderElement(doid, uid, elementHeader0.Name, (int)elementHeader0.TypeId, elementHeader0.Tag, elementHeader0.Desc, elementHeader0.Active);
                    var deid = int.Parse(eltheader.Tables[0].Rows[0].ItemArray[0].ToString());
                    var eltdetail = AddElementHeaderAndValue(deid, doid, elementHeader0.Detail, uid, elementHeader0.Active);
                }


            }
            else
            {


                for (int i = p; i < eltDetails.Count; i++)
                {
                    var elementHeader0 = new ElementHeader(eltName, realEltTypeId, tag, desc, eltDetails[i], active);
                    var eltheader = AddHeaderElement(doid, uid, elementHeader0.Name, (int)elementHeader0.TypeId, elementHeader0.Tag, elementHeader0.Desc, elementHeader0.Active);
                    var deid = int.Parse(eltheader.Tables[0].Rows[0].ItemArray[0].ToString());
                    var eltdetail = AddElementHeaderAndValue(deid, doid, elementHeader0.Detail, uid, elementHeader0.Active);
                }
            }



        }


        protected int AddSystem(Guid guid, string systemname, int systypeid,  string systemIdentifier, int active,  int userId)
        {
            var allSystems = GetAllSystemInfo();
            int rank = 6;
            var ID = 0;

            SqlParameter[] oParams = new SqlParameter[rank];

            for (int i=0; i<allSystems.Count; i++)
            {
                var maintest = allSystems[i][1] == guid.ToString() && allSystems[i][5] == userId.ToString();
                if ( (allSystems[i][2]==systemname || allSystems[i][4]==systemIdentifier) && maintest )
                {
                    ID = int.Parse(allSystems[i][0]);
                    rank = 7;
                    oParams = new SqlParameter[rank];
                    oParams[rank-1] = new SqlParameter("SystemId", SqlDbType.Int, 10);
                    oParams[rank-1].Value = ID;
                    break;
                }
            }
          

            oParams[0] = new SqlParameter("SysTypeId", SqlDbType.Int, 10);
            oParams[0].Value = systypeid;
            oParams[1] = new SqlParameter("SysName", SqlDbType.VarChar, 255);
            oParams[1].Value = systemname; 
            oParams[2] = new SqlParameter("SystemIdentifier", SqlDbType.VarChar, 50);
            oParams[2].Value = systemIdentifier;

            oParams[3] = new SqlParameter("UserId", SqlDbType.Int, 10);
            oParams[3].Value = userId;

            oParams[4] = new SqlParameter("OrgId", SqlDbType.UniqueIdentifier);
            oParams[4].Value = guid;

            oParams[5] = new SqlParameter("isActive", SqlDbType.Int, 10);
            oParams[5].Value = active;



            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_System_Put]", CommandType.StoredProcedure, oParams);


            int res = 0;
            res = int.Parse(_ds.Tables[0].Rows[0].ItemArray[0].ToString());
            return res;

        }

        protected string GetDocFullName(int doid)
        {
            var res = "";
            SqlParameter[] param = new SqlParameter[1];

            param[0] = new SqlParameter("DOID", SqlDbType.Int, 10);
            param[0].Value = doid;

            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocObject_Get]", CommandType.StoredProcedure, param);

            if (_ds.Tables[0].Rows.Count > 0)
                res = _ds.Tables[0].Rows[0].ItemArray[2].ToString();

             return res;
        }

        public List<Item> ControlResponsibleRole(string controlId)
        {
            var roles = GetDBData("implemented-requirement control-id, responsible-role role-id", UserName, DOID);
            var tagC = string.Format("<control-implementation><implemented-requirement  control-id=\"{0}\">", controlId);
            var ControlRespRole = new List<Item>();

            for (int i = 0; i < roles.Count(); i++)
            {
                var tagA = roles[i][1];
                int ine = tagA.LastIndexOf("<");
                var ntag = tagA.Remove(ine);
                if (tagC == ntag)
                {
                    var tem = new Item
                    {
                        Value = roles[i][0],
                        XPath = roles[i][2]
                    };
                    ControlRespRole.Add(tem);
                }
            }

            return ControlRespRole;
        }

        protected List<string> GetWizardUsers()
        {
          var res = new List<string>();

            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_User_Get]", CommandType.StoredProcedure);

            for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
            {
                var x = _ds.Tables[0].Rows[i].ItemArray[1].ToString();
                res.Add(x);

            }

            return res;
        }

        protected string GetUserPassword(string username)
        {
            SqlParameter[] param = new SqlParameter[1];

            param[0] = new SqlParameter("Uname", SqlDbType.VarChar, 255);
            param[0].Value = username;

            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_User_Get]", CommandType.StoredProcedure, param);

          var res = _ds.Tables[0].Rows[0].ItemArray[2].ToString();

            return res;
        }

        protected List<List<string>> GetSystemDocInfo(int systemId)
        {
            var res = new List<List<string>>();

           
            var ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocObject_Get]", CommandType.StoredProcedure);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var orid = int.Parse(ds.Tables[0].Rows[i].ItemArray[4].ToString());
                if (systemId == orid)
                {
                    var tem = new List<string>();
                    for(int j=0; j< ds.Tables[0].Rows[i].ItemArray.Length;j++)
                    {
                        tem.Add(ds.Tables[0].Rows[i].ItemArray[j].ToString());
                    }
                    res.Add(tem);
                }

            }

            return res;
        }

        protected List<List<string>> GetSystemDocInfo(int systemId, int docType)
        {
            var res = new List<List<string>>();


            var ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocObject_Get]", CommandType.StoredProcedure);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var orid = int.Parse(ds.Tables[0].Rows[i].ItemArray[4].ToString());
                var type = int.Parse(ds.Tables[0].Rows[i].ItemArray[3].ToString());
                if (systemId == orid && type == docType)
                {
                    var tem = new List<string>();
                    for (int j = 0; j < ds.Tables[0].Rows[i].ItemArray.Length; j++)
                    {
                        tem.Add(ds.Tables[0].Rows[i].ItemArray[j].ToString());
                    }
                    res.Add(tem);
                }

            }

            return res;
        }

        protected List<string> GetSystemDocFullName(int systemId, int docType)
        {
            var res = new List<string>();


            var ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocObject_Get]", CommandType.StoredProcedure);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var orid = int.Parse(ds.Tables[0].Rows[i].ItemArray[4].ToString());
                var type = int.Parse(ds.Tables[0].Rows[i].ItemArray[3].ToString());
                if (systemId == orid && type == docType)
                {
                    res.Add(ds.Tables[0].Rows[i].ItemArray[2].ToString());
                }

            }

            return res;
        }


        protected List<string> GetSystemDocFullName(int systemId)
        {
            var res = new List<string>();

            
            var ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocObject_Get]", CommandType.StoredProcedure);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var orid = int.Parse(ds.Tables[0].Rows[i].ItemArray[4].ToString());
                if (systemId == orid)
                {
                    res.Add(ds.Tables[0].Rows[i].ItemArray[2].ToString());
                }

            }

            return res;
        }
        protected string GetUserOrgName(string username)
        {
            var orgid = GetUserOrgId(username);
            SqlParameter[] param = new SqlParameter[1];

            param[0] = new SqlParameter("OrgId", SqlDbType.UniqueIdentifier);
            param[0].Value = orgid;

            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_Organization_Get]", CommandType.StoredProcedure, param);

            var res = _ds.Tables[0].Rows[0].ItemArray[1].ToString();

            return res;
        }

        protected Guid GetUserOrgId(string username)
        {
            SqlParameter[] param = new SqlParameter[1];

            param[0] = new SqlParameter("Uname", SqlDbType.VarChar, 255);
            param[0].Value = username;

            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_User_Get]", CommandType.StoredProcedure, param);

            var res = new Guid(_ds.Tables[0].Rows[0].ItemArray[9].ToString());

            return res;
        }

        protected string GetError(string FileName)
        {
            var sr = new StreamReader(FileName);
            var err = sr.ReadToEnd();
            return err;
        }
        protected int GetUserIdFromSystemId(int SystemId)
        {
            SqlParameter[] param = new SqlParameter[1];

            param[0] = new SqlParameter("SystemId", SqlDbType.Int, 10);
            param[0].Value = SystemId;
            var res = -1;
            var ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_System_Get_Details]", CommandType.StoredProcedure, param);
            
            if(ds.Tables.Count > 0 && ds.Tables[0].Rows.Count>0)
            {
                res = int.Parse(ds.Tables[0].Rows[0].ItemArray[5].ToString());
            }

            return res;
        }

        protected  int GetUserSystemId(string username, string sysName)
        {
            var id = GetUserID(username);
            var res = -1;
            var ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_System_Get_Details]", CommandType.StoredProcedure);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var orid = int.Parse(ds.Tables[0].Rows[i].ItemArray[5].ToString());
                var sysRealName = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                var sysIdent = ds.Tables[0].Rows[i].ItemArray[4].ToString();
                if (id == orid && sysName == sysRealName)
                {
                    res=int.Parse(ds.Tables[0].Rows[i].ItemArray[0].ToString());
                    break;
                }

            }

            return res;
        }

        protected List<string> GetUserSystem( string username)
        {
            var res = new List<string>();

            var id = GetUserID(username);
            var ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_System_Get_Details]", CommandType.StoredProcedure);
            for(int i=0; i<ds.Tables[0].Rows.Count; i++)
            {
                var orid = int.Parse(ds.Tables[0].Rows[i].ItemArray[5].ToString());
              if(id ==orid)
              {
                    res.Add(ds.Tables[0].Rows[i].ItemArray[2].ToString());
              }

            }

            return res;
        }

        protected string GetElementDetail(int deid)
        {
            SqlParameter[] param = new SqlParameter[1];

            param[0] = new SqlParameter("DEID", SqlDbType.Int, 10);
            param[0].Value = deid;

            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocElement_Get]", CommandType.StoredProcedure, param);
            var res = "";
            if (_ds.Tables[0].Rows.Count>0)
                res = _ds.Tables[0].Rows[0].ItemArray[3].ToString();
  
            return res;
        }

        protected string GetUsernameFromID(int uid)
        {
            SqlParameter[] param = new SqlParameter[1];

            param[0] = new SqlParameter("UID", SqlDbType.Int, 10);
            param[0].Value = uid;

            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_User_Get]", CommandType.StoredProcedure, param);

            var res = _ds.Tables[0].Rows[0].ItemArray[1].ToString();

            return res;
        }

        protected int GetUserID(string username)
        {
            SqlParameter[] param = new SqlParameter[1];

            param[0] = new SqlParameter("Uname", SqlDbType.VarChar, 255);
            param[0].Value = username;

            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_User_Get]", CommandType.StoredProcedure, param);

           var res = int.Parse(_ds.Tables[0].Rows[0].ItemArray[0].ToString());

            return res;
        }

        protected int AddDoc(string docShortName, string docFullName, int docOwnerId, int docTypeId, int active, int modifiedby) 
        {
           
            SqlParameter[] oParams = new SqlParameter[7];

            oParams[0] = new SqlParameter("SystemId", SqlDbType.Int, 10);
            oParams[0].Value = docOwnerId;
            oParams[1] = new SqlParameter("DocShortName", SqlDbType.VarChar, 50);
            oParams[1].Value = docShortName; 
            oParams[2] = new SqlParameter("DocFullName", SqlDbType.VarChar, 255);
            oParams[2].Value = docFullName;


            oParams[3] = new SqlParameter("DocTypeId", SqlDbType.Int, 10);
            oParams[3].Value = docTypeId;

            oParams[4] = new SqlParameter("Active", SqlDbType.Int, 10);
            oParams[4].Value = active;

            oParams[5] = new SqlParameter("ModifiedBy", SqlDbType.Int, 10);
            oParams[5].Value = modifiedby;

            oParams[6] = new SqlParameter("CreatedBy", SqlDbType.Int, 10);
            oParams[6].Value = modifiedby;

            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocObject_Put]", CommandType.StoredProcedure, oParams);

            int res = 0;
            
              res = int.Parse(_ds.Tables[0].Rows[0].ItemArray[0].ToString());
                       
            return res;

        }

        protected int AddUser(string username, string password, int owner, int roleid, Guid guid, int active)
        {
            SqlParameter[] oParams = new SqlParameter[6];

            oParams[0] = new SqlParameter("roleid", SqlDbType.Int, 10);
            oParams[0].Value = roleid;
            oParams[1] = new SqlParameter("UName", SqlDbType.VarChar, 255);
            oParams[1].Value = username; //OSCAL-SSP
            oParams[2] = new SqlParameter("UPass", SqlDbType.VarChar, 512);
            oParams[2].Value = password;


            oParams[3] = new SqlParameter("Owner", SqlDbType.Int, 10);
            oParams[3].Value = owner;

            oParams[4] = new SqlParameter("OrganizationId", SqlDbType.UniqueIdentifier);
            oParams[4].Value = guid;

            oParams[5] = new SqlParameter("isActive", SqlDbType.Int, 10);
            oParams[5].Value = active;

            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_User_Put]", CommandType.StoredProcedure, oParams);

            int res = 0;
            var lang = _ds.Tables[0].Rows[0].ItemArray[0].ToString();
            if (lang.Length != 0)
            {
                res = int.Parse(_ds.Tables[0].Rows[0].ItemArray[0].ToString());
            }
            else
            {
                res = GetUserID(username);
            }
            return res;

        }

        protected Guid AddOrganization(string orgname, int orgtype,  string orgdesc, int active, int owner)
        {
            SqlParameter[] oParams = new SqlParameter[5];

            oParams[0] = new SqlParameter("OrgTypeID", SqlDbType.Int, 10);
            oParams[0].Value = orgtype;
            oParams[1] = new SqlParameter("OrgDesc", SqlDbType.VarChar, 255);
            oParams[1].Value = orgdesc; //OSCAL-SSP
            oParams[2] = new SqlParameter("OrgName", SqlDbType.VarChar, 255);
            oParams[2].Value = orgname;


            oParams[3] = new SqlParameter("UserId", SqlDbType.Int, 10);
            oParams[3].Value = owner;

            oParams[4] = new SqlParameter("isActive", SqlDbType.Int, 10);
            oParams[4].Value = active;

            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_Organization_Put]", CommandType.StoredProcedure, oParams);

          
           var res = new Guid(_ds.Tables[0].Rows[0].ItemArray[0].ToString());
            return res;
        }


        protected void PutPemyOld(int id, string xml, string stringValue)
        {
            SqlParameter[] oParams = new SqlParameter[3];
            oParams[0] = new SqlParameter("DEID", SqlDbType.Int, 10);
            oParams[0].Value = id;
            oParams[1] = new SqlParameter("ElementValue", SqlDbType.Xml);
            oParams[1].Value = xml;

            oParams[2] = new SqlParameter("StringValue", SqlDbType.NVarChar); // SqlDbType.NText, 1024*1024);
            oParams[2].Value = stringValue;


            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_PemyTest_Put]", CommandType.StoredProcedure, oParams);

        }

        protected DataSet AddElementHeaderAndValue(int deid, int doid, string eltValue, int uid, int active)
        {




            //Create Parameters
            SqlParameter[] oParams = new SqlParameter[5];
            oParams[0] = new SqlParameter("USERID", SqlDbType.Int, 10);
            oParams[0].Value = uid;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = doid; //OSCAL-SSP
            oParams[2] = new SqlParameter("ElementValue", SqlDbType.NVarChar);
            oParams[2].Value = eltValue;


            oParams[3] = new SqlParameter("DEID", SqlDbType.Int, 10);
            oParams[3].Value = deid;

            oParams[4] = new SqlParameter("Active", SqlDbType.Int, 10);
            oParams[4].Value = active;

            //Execute Procedure With Parameters
            //Fill DataSet
            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocElement_Put]", CommandType.StoredProcedure, oParams);


            return _ds;
        }

        protected DataSet AddHeaderElement(int doid, int uid, string eltName, int eltTypeId, string eltTag, string eltDescription, int active)
        {


            //Create Parameters
            SqlParameter[] oParams = new SqlParameter[7];
            oParams[0] = new SqlParameter("UID", SqlDbType.Int, 10);
            oParams[0].Value = uid;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = doid;
            oParams[2] = new SqlParameter("ElementName", SqlDbType.NVarChar, 255);
            oParams[2].Value = eltName;
            oParams[3] = new SqlParameter("ElementTypeId", SqlDbType.Int, 10);
            oParams[3].Value = eltTypeId;
            oParams[4] = new SqlParameter("ElementTag", SqlDbType.NVarChar, 255);
            oParams[4].Value = eltTag;
            oParams[5] = new SqlParameter("ElementDesc", SqlDbType.NVarChar, 1024);
            oParams[5].Value = eltDescription;
            oParams[6] = new SqlParameter("Active", SqlDbType.Int, 10);
            oParams[6].Value = active;



            //Execute Procedure With Parameters
            //Fill DataSet
            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocHeader_Put]", CommandType.StoredProcedure, oParams);

            return _ds;

        }

        protected DataSet AddHeaderElement(int doid, int uid,  int deid, string eltName, int eltTypeId, string eltTag, string eltDescription, int active)
        {


            //Create Parameters
            SqlParameter[] oParams = new SqlParameter[8];
            oParams[0] = new SqlParameter("UID", SqlDbType.Int, 10);
            oParams[0].Value = uid;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = doid;
            oParams[2] = new SqlParameter("ElementName", SqlDbType.NVarChar, 255);
            oParams[2].Value = eltName;
            oParams[3] = new SqlParameter("ElementTypeId", SqlDbType.Int, 10);
            oParams[3].Value = eltTypeId;
            oParams[4] = new SqlParameter("ElementTag", SqlDbType.NVarChar, 255);
            oParams[4].Value = eltTag;
            oParams[5] = new SqlParameter("ElementDesc", SqlDbType.NVarChar, 1024);
            oParams[5].Value = eltDescription;
            oParams[6] = new SqlParameter("Active", SqlDbType.Int, 10);
            oParams[6].Value = active;
            oParams[7] = new SqlParameter("DEID", SqlDbType.Int, 10);
            oParams[7].Value = deid;




            //Execute Procedure With Parameters
            //Fill DataSet
            DataSet _ds = DAL.FillDataset("[OWT_DEV].[dbo].[OWT_DocHeader_Put]", CommandType.StoredProcedure, oParams);

            return _ds;

        }

        protected void CollapseDiv(string entity)
        {
            var sb = new StringBuilder();
            sb.Append("<script>");  ////type=\"text/javascript\">
            sb.Append(string.Format("document.getElementById(\"{0}\").style.display='none';", entity));
            sb.Append("</script>");

            var update = sb.ToString();
            HttpContext.Current.Response.Write(update);

            HttpContext.Current.Response.Flush();

        }

       

        protected ElementTypeId GetElementTypeId(Type eltTypeId)
        {
            ElementTypeId realEltTypeId;

            if (eltTypeId == typeof(String))
                realEltTypeId = ElementTypeId.String;
            else
            {
                if (eltTypeId == typeof(Int32))
                    realEltTypeId = ElementTypeId.Int;
                else
                    realEltTypeId = ElementTypeId.XML;
            }

            return realEltTypeId;
        }
    }
}