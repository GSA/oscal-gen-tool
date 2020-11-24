using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DocumentFormat.OpenXml.Packaging;


namespace OConvert
{

    class Program
    {
        static void ProcessData(string ssp_file, string word_template_file)
        {
            Console.WriteLine("SSP GENERATOR                                    Version: (Prototype)");
            Console.WriteLine("OSCAL File: {0}", ssp_file);
            Console.WriteLine("WORD Template: {0}", word_template_file);
            Console.WriteLine("Generating...");

            string rootPath = @".";
            string xmlDataFile = rootPath + @"\" + ssp_file;
            string templateDocument = rootPath + @"\" + word_template_file;
            string outputDocument = rootPath + @"\MyGeneratedDocument.docx";

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
            Console.Write("Generation Complete.");


        }

        static void Main(string[] args)
        {
            string OSCALSSP_file = "";
            string WordTemplate_file = "";

            if(args.Length == 2)
            {
                OSCALSSP_file = args[0];
                WordTemplate_file = args[1];
                ProcessData(OSCALSSP_file, WordTemplate_file); 
            }
            else
                Console.WriteLine("Error - Insufficient Command Line Arguments.");
                
        }
       
    }
}
