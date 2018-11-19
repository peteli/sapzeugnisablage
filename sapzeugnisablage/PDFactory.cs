using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;

namespace sapzeugnisablage
{
    static class PDFactory
    {
        public static void LabelonAllPages(FileInfo file, PDFMetaData prop)
        {
            Console.WriteLine("PDF processing {0}", file.FullName);
            //const string LABELCERTFOLDER = "certificate-folder";

            try
            {
                PdfDocument document = PdfReader.Open(file.FullName);

                // write metadata
                foreach(var customProp in prop.CustomProperties)
                {
                    if (document.Info.Elements.ContainsKey(@"/" + customProp.Key))
                    {
                        document.Info.Elements.SetValue(@"/" + customProp.Key, new PdfString(customProp.Value));
                    }
                    else
                    {
                        document.Info.Elements.Add(@"/" + customProp.Key, new PdfString(customProp.Value));
                    }
                }

                string inPrint = prop.Domain + @" Certificate Number: " + prop.CertNum;
                foreach (PdfPage p in document.Pages)
                {

                    XGraphics gfx = XGraphics.FromPdfPage(p);
                    XFont font = new XFont("Arial", 6, XFontStyle.Regular);
                    XSize stringBlock = gfx.MeasureString(inPrint, font);
                    XRect rect = new XRect(0, 0, stringBlock.Width + 2, stringBlock.Height + 2);
                    XBrush brush = XBrushes.Black;
                    gfx.DrawRectangle(brush, rect);
                    gfx.DrawString(inPrint, font, XBrushes.White, new XRect(1, 1, stringBlock.Width, stringBlock.Height), XStringFormats.Center);
                }

                document.Save(file.FullName);
                
            }
            catch (PdfSharp.Pdf.IO.PdfReaderException e)
            {
                Console.WriteLine("{1} has {0}", e.Message, file.FullName);
            }
        }
    }
}
