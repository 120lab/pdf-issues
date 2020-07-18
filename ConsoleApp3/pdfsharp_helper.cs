using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp3
{
    class pdfsharp_helper
    {
        public static void GetElements(string fileName)
        {
            try
            {
                // Read document into memory for modification
                PdfDocument document = PdfReader.Open(fileName);

                var page = document.Pages[0];
                CObject content = ContentReader.ReadContent(page);
                var extractedText = ExtractText(content);
                FileStream fs = new FileStream(Path.GetDirectoryName(fileName) + "\\result_sharp.txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(extractedText);
                sw.Flush();
                sw.Close();

                // Finally we must add the action dictionary to the /OpenAction key of
                // the document's catalog as an indirect value.
                //document.Internals.Catalog.Elements["/OpenAction"] =
                //PdfInternals.GetReference(dict);

                // Using PDFsharp we never deal with object numbers. We simply put the
                // objects together and the PDFsharp framework does the rest.
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static IEnumerable<string> ExtractText(CObject cObject)
        {
            var textList = new List<string>();
            if (cObject is COperator)
            {
                var cOperator = cObject as COperator;
                if (cOperator.OpCode.Name == OpCodeName.Tj.ToString() ||
                    cOperator.OpCode.Name == OpCodeName.TJ.ToString())
                {
                    foreach (var cOperand in cOperator.Operands)
                    {
                        textList.AddRange(ExtractText(cOperand));
                    }
                }
            }
            else if (cObject is CSequence)
            {
                var cSequence = cObject as CSequence;
                foreach (var element in cSequence)
                {
                    textList.AddRange(ExtractText(element));
                }
            }
            else if (cObject is CString)
            {
                var cString = cObject as CString;
                textList.Add(cString.Value);
            }
            return textList;
        }
    }

}

