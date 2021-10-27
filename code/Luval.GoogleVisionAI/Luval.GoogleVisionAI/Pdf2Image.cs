using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoogleVisionAI
{
    public class Pdf2Image
    {
        public static IEnumerable<FileInfo> Convert(FileInfo inputFile, DirectoryInfo outputDir)
        {
            var result = new List<FileInfo>();
            using (var doc = PdfDocument.Load(inputFile.FullName)) // C# Read PDF Document
            {
                var pageNum = 1;
                foreach (var page in doc.Pages)
                {
                    int width = (int)(page.Width / 72.0 * 96);
                    int height = (int)(page.Height / 72.0 * 96);
                    using (var bitmap = new PdfBitmap(width, height, true))
                    {
                        bitmap.FillRect(0, 0, width, height, FS_COLOR.White);
                        page.Render(bitmap, 0, 0, width, height, PageRotate.Normal, RenderFlags.FPDF_LCD_TEXT);
                        var fileName = Path.Combine(outputDir.FullName, string.Format("{0}-{1}.png", pageNum.ToString().PadLeft(4, '0'), inputFile.Name));
                        bitmap.Image.Save(fileName, ImageFormat.Png);
                        result.Add(new FileInfo(fileName));
                    }
                    pageNum++;
                }
            }
            return result;
        }
    }
}
