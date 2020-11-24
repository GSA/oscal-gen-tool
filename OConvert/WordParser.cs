using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Office.Interop.Word;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;

namespace OConvert
{
    public class WordParser
    {
        protected const string XMLNamespace = @"http://csrc.nist.gov/ns/oscal/1.0";
        protected private const string WordTemplateFile = "FedRAMP-SSP-Moderate-Baseline-OSCAL";
        protected private const string ModerateWordTemplate = "FedRAMP-SSP-Moderate-Baseline-OSCAL";
        protected private const string LowWordTemplate = "FedRAMP-SSP-Low-Baseline-Template";
        protected private const string HighWordTemplate = "FedRAMP-SSP-High-Baseline-Template";
        protected private const string OSCALSchema = "oscal_catalog_schema.xsd";
        protected private const string xml_file = "OSCALTemplate_1_0.xml";
       public  void Parse()
        {
            Application ap = new Application();
            // var outputFilePdfPath = outputFile.Replace(".docx", ".pdf");

            string appPath = @"C:\Temp";
            var wordTemplatePath = string.Format(@"{0}\{1}.docx", appPath, HighWordTemplate);

            Document document = ap.Documents.Open(wordTemplatePath);
           // document.ExportAsFixedFormat(outputFilePdfPath, WdExportFormat.wdExportFormatPDF, true);
            ap.Visible = true;

            var field = document.Fields;
           
                string xmlDataFile = string.Format(@"{0}\{1}.docx", appPath, xml_file);
                string templateDocument = HighWordTemplate;




                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateDocument, true))
                {
                    //get the main part of the document which contains CustomXMLParts
                    MainDocumentPart mainPart = wordDoc.MainDocumentPart;

                    //delete all CustomXMLParts in the document. If needed only specific CustomXMLParts can be deleted using the CustomXmlParts IEnumerable
                    mainPart.DeleteParts<CustomXmlPart>(mainPart.CustomXmlParts);

                    //add new CustomXMLPart with data from new XML file
                    CustomXmlPart myXmlPart = mainPart.AddCustomXmlPart(CustomXmlPartType.CustomXml);
                    using (FileStream stream = new FileStream(xmlDataFile, FileMode.Open))
                    {
                        myXmlPart.FeedData(stream);
                    }
                }
            }
    }
}
