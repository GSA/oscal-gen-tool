using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using System.IO; 
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;


namespace OConvert
{
    public class XMLValidator
    {
        protected const string XMLNamespace = @"http://csrc.nist.gov/ns/oscal/1.0";
        public static bool SuccessfulValidation;
        private static string ErrorFile;
        private static bool HasErrors;
        static StreamWriter sw;
        public static void RunValidator(string XmlDocumentPath, string XsdSchemaPath, string ErrorFilePath = null)
        {
            
            ErrorFile = ErrorFilePath;
          
            try
            {
                using (sw = new StreamWriter(ErrorFile))
                {
                    var path = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
                    XmlSchemaSet schema = new XmlSchemaSet();
                    schema.Add(XMLNamespace, XsdSchemaPath);
                    schema.Compile();

                    XmlReader rd = XmlReader.Create(XmlDocumentPath);
                    XDocument doc = XDocument.Load(rd);
                    doc.Validate(schema, ValidationEventHandler);

                    sw.Close();

                    if (!HasErrors)
                        SuccessfulValidation = true;
                    else
                    {
                        sw.Close();
                        throw new Exception(" OSCAL Validation Errors. \n");
                    }
                }

            }
            catch (Exception exs)
            {
                SuccessfulValidation = false;
                throw exs;
            }
         
        }
        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
           
            //XmlSeverityType type = XmlSeverityType.Warning;
            //if (Enum.TryParse<XmlSeverityType>("Error", out type))
            //{
            //    if (type == XmlSeverityType.Error) throw new Exception(e.Message);
            //}

            XmlSeverityType type = XmlSeverityType.Warning;
            if (Enum.TryParse<XmlSeverityType>("Error", out type))
            {
                if (type == XmlSeverityType.Error)
                {
                    HasErrors = true;
                    sw.WriteLine(e.Message);
                }

            }

        }

        public static void PseudoValidator(string XmlDocumentPath, string XsdSchemaPath)
        {
            try
            {

                XmlReaderSettings OscalSettings = new XmlReaderSettings();
                OscalSettings.Schemas.Add(XMLNamespace, XsdSchemaPath);
                OscalSettings.ValidationType = ValidationType.Schema;
                OscalSettings.ValidationEventHandler += new ValidationEventHandler(booksSettingsValidationEventHandler);
                XmlReader OscalDoc = XmlReader.Create(XmlDocumentPath, OscalSettings);

                while (OscalDoc.Read()) { }

                OscalDoc.Close();
                SuccessfulValidation = true;
            }
            catch(Exception ex)
            {
                SuccessfulValidation = false;
                throw ex;
            }
        }

        static void booksSettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                Console.Write("WARNING: ");
                Console.WriteLine(e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                Console.Write("ERROR: ");
                Console.WriteLine(e.Message);
            }
        }

    }
    
}
