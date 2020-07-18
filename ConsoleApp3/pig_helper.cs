using System;
using System.IO;

namespace ConsoleApp3
{
    class pig_helper
    {
        public static void GetElements(string fileFullName)
        {

            try
            {
                using (var stream = File.OpenRead(fileFullName))
                using (UglyToad.PdfPig.PdfDocument document = UglyToad.PdfPig.PdfDocument.Open(stream))
                {
                    FileStream fs = new FileStream(System.IO.Path.GetDirectoryName(fileFullName) + "\\result_pig.txt", FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);

                    foreach (var page in document.GetPages())
                    {
                        string txt = page.Text;

                        sw.Write(txt);
                    }

                    sw.Flush();
                    sw.Close();

                }

            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);                
            }
        }
        
    }
}
