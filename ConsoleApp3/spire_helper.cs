using System;
using System.IO;
using Spire.Pdf;
using Spire.Pdf.Exporting.Text;

namespace ConsoleApp3
{
    class spire_helper
    {
        public static void GetElements(string fileName)
        {
            try
            {
                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(fileName);
                PdfPageBase page = doc.Pages[0];

                SimpleTextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                string text = page.ExtractText(strategy);
                FileStream fs = new FileStream(Path.GetDirectoryName(fileName) + "\\result_spire.txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(text);
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
       
    }

}

