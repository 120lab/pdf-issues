using System;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                const string dir = @"C:\Users\p0011111\Desktop\NSG MEITAV\pdf-issues\ConsoleApp3";
                const string fileName = dir + @"\מיקוד פועלים 05.07.20.pdf";
                const string signedFileName = dir + @"\signed-result.pdf";
                const string privateKeyFileName = dir + @"\crypto-private-key.txt";
                const string certFileName = dir + @"\crypto-x509-cert.txt";
                //const string fileName = @"C:\Users\p0011111\Downloads\splitPdf_3.pdf";
                //const string fileName = @"C:\Users\p0011111\Downloads\WorkOnPdfObjects_output.pdf";
                itext_helper.Sign(fileName, signedFileName, "Signed Hadas Customer's Invoices", "Israel, Haifa",
                    privateKeyFileName, certFileName, "qwe123!@#QWE");

                pdfsharp_helper.GetElements(fileName);
                itext_helper.GetElements(fileName);

                spire_helper.GetElements(fileName);
                pig_helper.GetElements(fileName);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }   
    }
}
