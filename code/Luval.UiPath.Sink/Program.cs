using Luval.GoogleVisionAI;
using Luval.PDFTableReader;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Luval.UiPath.Sink
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = new DirectoryInfo(@"C:\Users\CH489GT\Downloads\Test");
            foreach (var file in dir.GetFiles("*.pdf", SearchOption.AllDirectories))
            {
                var tableExtractor = new CleanTableExtrator(file.FullName);
                var dts = tableExtractor.GetTables();
                SaveFile(dir, file, dts.ToList());
            }
        }

        private static void SaveFile(DirectoryInfo dir, FileInfo file, List<DataTable> tables)
        {
            var excelFile = new FileInfo(Path.Combine(dir.FullName, string.Format("RESULT-{0}.xlsx", file.Name.Replace(file.Extension, ""))));
            if (excelFile.Exists) excelFile.Delete();
            using (var package = new ExcelPackage(excelFile))
            {
                for (int t = 0; t < tables.Count(); t++)
                {
                    var table = tables[t];
                    var sheet = package.Workbook.Worksheets.Add(string.Format("Data{0}", t.ToString().PadLeft(3, '0')));
                    GetHeadears(table, sheet);
                    var rowCout = 2;
                    foreach (var row in table.Rows.Cast<DataRow>())
                    {
                        for (int c = 0; c < table.Columns.Count; c++)
                        {
                            sheet.Cells[rowCout, c + 1].Value = row[c];
                        }
                        rowCout++;
                    }
                    var range = sheet.Cells[rowCout - 1, table.Columns.Count];
                    sheet.Tables.Add(range, string.Format("Table{0}", t.ToString().PadLeft(3, '0')));
                    range.AutoFitColumns();
                }
                package.Save();
            }
        }

        private static void GetHeadears(DataTable table, ExcelWorksheet sheet)
        {
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sheet.Cells[1, i + 1].Value = table.Columns[i].ColumnName;
            }
        }
    }
}
