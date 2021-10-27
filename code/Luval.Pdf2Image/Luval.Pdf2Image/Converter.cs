using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Pdf2Image
{
    public class Converter
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

        public static IEnumerable<string> Convert(string inputFile, string outputDir)
        {
            if (string.IsNullOrWhiteSpace(inputFile)) throw new ArgumentNullException("inputFile", "value cannot be null or empty");
            if (string.IsNullOrWhiteSpace(outputDir)) throw new ArgumentNullException("outputDir", "value cannot be null or empty");
            var res = Convert(new FileInfo(inputFile), new DirectoryInfo(outputDir));
            return res.Select(i => i.FullName);
        }

        public static DataTable ConvertIntoDatatable(string inputFile, string outputDir)
        {
            var dt = new DataTable("PDFImages");
            dt.Columns.Add("SortOrder", typeof(int));
            dt.Columns.Add("ImageFileName", typeof(string));
            dt.Columns.Add("PDFFileName", typeof(string));
            var res = Convert(inputFile, outputDir);
            var sort = 1;
            foreach (var img in res)
            {
                var row = dt.NewRow();
                row["SortOrder"] = sort;
                row["ImageFileName"] = img;
                row["PDFFileName"] = inputFile;
                dt.Rows.Add(row);
                sort++;
            }
            return dt;
        }
    }
}
