using System;
using System.IO;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Signatures;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;
using static iText.Signatures.PdfSigner;
using Org.BouncyCastle.Crypto.Parameters;
namespace ConsoleApp3
{
    public class itext_helper
    {
    
        public static void GetElements(string fileName)
        {
            FileInfo file = new FileInfo(System.IO.Path.GetDirectoryName(fileName) + "\\result_itext.txt");
            file.Directory.Create();

            new itext_helper().ManipulatePdf(System.IO.Path.GetDirectoryName(fileName) + "\\result_itext.txt", fileName);
        }

        public virtual void ManipulatePdf(String dest, string fileName)
        {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(fileName));

            Rectangle rect = new Rectangle(36, 750, 523, 56);
            CustomFontFilter fontFilter = new CustomFontFilter(rect);
            FilteredEventListener listener = new FilteredEventListener();

            // Create a text extraction renderer
            LocationTextExtractionStrategy extractionStrategy = listener
                .AttachEventListener(new LocationTextExtractionStrategy(), fontFilter);

            // Note: If you want to re-use the PdfCanvasProcessor, you must call PdfCanvasProcessor.reset()
            new PdfCanvasProcessor(listener).ProcessPageContent(pdfDoc.GetFirstPage());

            // Get the resultant text after applying the custom filter
            String actualText = extractionStrategy.GetResultantText();

            pdfDoc.Close();

            // See the resultant text in the console
            Console.Out.WriteLine(actualText);

            using (StreamWriter writer = new StreamWriter(dest))
            {
                writer.Write(actualText);
            }
        }

        // The custom filter filters only the text of which the font name ends with Bold or Oblique.
        protected class CustomFontFilter : TextRegionEventFilter
        {
            public CustomFontFilter(Rectangle filterRect)
                : base(filterRect)
            {
            }

            public override bool Accept(IEventData data, EventType type)
            {
                if (type.Equals(EventType.RENDER_TEXT))
                {
                    TextRenderInfo renderInfo = (TextRenderInfo)data;
                    PdfFont font = renderInfo.GetFont();
                    if (null != font)
                    {
                        String fontName = font.GetFontProgram().GetFontNames().GetFontName();
                        return fontName.EndsWith("Bold") || fontName.EndsWith("Oblique");
                    }
                }

                return false;
            }
        }

        public static void Sign(string fileName, string signedFileName, string reason, string location,
                                string privateKeyFileName,  string certFileName, string password)
        {
            PdfReader reader = new PdfReader(fileName);
            PdfWriter write = new PdfWriter(signedFileName);
            PdfSigner signer = new PdfSigner(reader, write, false);

            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetReason(reason);
            appearance.SetLocation(location);
            appearance.SetReuseAppearance(false);

            Rectangle rect = new Rectangle(36, 648, 200, 100);
            appearance.SetPageRect(rect);
            appearance.SetPageNumber(1);

            signer.SetFieldName("sig");

            IExternalSignature pks = new PrivateKeySignature(ReadPrivateKey(privateKeyFileName, password), GetEncryptionAlgorithm());

            X509CertificateParser parser = new X509CertificateParser();
            X509Certificate cert = LoadCertificate(certFileName);
            X509Certificate[] chain = new X509Certificate[1];
            chain[0] = cert;
            PdfSigner.CryptoStandard subfilter = GetSubFilter();

            signer.SignDetached(pks, chain, null, null, null, 0, subfilter);

        }

        private static string GetEncryptionAlgorithm()
        {
            return "SHA512";
        }

        private static PdfSigner.CryptoStandard GetSubFilter()
        {
            return CryptoStandard.CADES;
        }

        private static RsaPrivateCrtKeyParameters ReadPrivateKey(string privateKeyFileName, string password)
        {
            
            AsymmetricCipherKeyPair keyPair;

            using (var reader = File.OpenText(privateKeyFileName))
                  keyPair = (RSAKeyPair)new PemReader(reader, new PasswordFinder(password)).ReadObject();

            return keyPair.Private;
        }

        private class PasswordFinder : IPasswordFinder
        {
            private string password;

            public PasswordFinder(string password)
            {
                this.password = password;
            }


            public char[] GetPassword()
            {
                return password.ToCharArray();
            }
        }
        private static X509Certificate LoadCertificate(string filename)
        {
            X509CertificateParser certParser = new X509CertificateParser();
            FileStream fs = new FileStream(filename, FileMode.Open);
            X509Certificate cert = certParser.ReadCertificate(fs);
            fs.Close();

            return cert;
        }
    }
}