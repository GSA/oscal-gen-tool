using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DocumentFormat.OpenXml.Packaging;

namespace OConvert
{
    public class Converter
    {

        public static void ConvertXml(string xml_file, string word_template_file)
        {
         
            string xmlDataFile =  xml_file;
            string templateDocument =  word_template_file;


          

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
