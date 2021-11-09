using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabula;
using Tabula.Extractors;
using UglyToad.PdfPig;

namespace Luval.PDFTableReader
{
    public class CleanTableExtrator
    {
        private FileInfo _fileInfo;
        public CleanTableExtrator(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException("fileName");
            _fileInfo = new FileInfo(fileName);
            if (!_fileInfo.Exists) throw new ArgumentException(string.Format("File {0} does not exists", _fileInfo), "fileName");
        }

        public IEnumerable<DataTable> GetTables()
        {
            return GetTablesInPage(4);
        }

        public IEnumerable<DataTable> GetTablesInPage(int pageNum)
        {
            var result = new List<DataTable>();
            using (PdfDocument document = PdfDocument.Open(_fileInfo.FullName, new ParsingOptions() { ClipPaths = true }))
            {
                result.AddRange(GetTablesInPage(pageNum, document));
            }
            return result;
        }

        public IEnumerable<DataTable> GetTablesInPage(int pageNum, PdfDocument document)
        {
            var result = new List<DataTable>();
            ObjectExtractor oe = new ObjectExtractor(document);
            PageArea page = oe.Extract(pageNum);
            var ea = new SpreadsheetExtractionAlgorithm();
            foreach (var table in ea.Extract(page))
            {
                if (table.Rows.Count > 0)
                {
                    var cellA = table.Rows[0][0].ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(cellA) &&
                        cellA.StartsWith("DocuSign"))
                        continue;
                    if (!string.IsNullOrWhiteSpace(cellA) &&
                        cellA.StartsWith("See additional"))
                        continue;
                }
                result.AddRange(CreateTables(table));
            }
            return result;
        }

        private IEnumerable<DataTable> CreateTables(Table table)
        {
            var dt = new List<DataTable>();
            if (table == null || table.Rows == null || !table.Rows.Any()) return dt;
            bool isFirst = true;
            bool isSummary = false;
            
            var dt1 = new DataTable("EquipmentToInstall");
            var dt2 = new DataTable("Summary");
            foreach (var row in table.Rows)
            {
                var cellA = row[0].ToString();
                if (string.IsNullOrWhiteSpace(cellA))
                    continue;
                if (isFirst)
                {
                    if (!string.IsNullOrWhiteSpace(cellA) && !cellA.StartsWith("Quantity"))
                        continue;
                    CreateTab1Cols(dt1, row);
                    isFirst = false;
                }
                else if (cellA.StartsWith("Subtotal"))
                {
                    isSummary = true;
                    if (dt2.Columns.Count <= 0)
                        CreateTab2Cols(dt2);
                }
                else if(!isSummary)
                    AddRow(dt1, row);
                if (isSummary)
                {
                    if (cellA.StartsWith("See additional")) continue;
                    var dt2Row = dt2.NewRow();
                    dt2Row[0] = row[0].ToString();
                    dt2Row[1] = row[1].ToString();
                    dt2.Rows.Add(dt2Row);
                }
            }
            dt.AddRange(new[] { dt1, dt2 });
            return dt;
        }

        private void AddRow(DataTable dt, IReadOnlyList<Cell> row)
        {
            var dtRow = dt.NewRow();
            for (int i = 0; i < row.Count; i++)
            {
                dtRow[i] = row[i].ToString();
            }
            dt.Rows.Add(dtRow);
        }

        private void CreateTab1Cols(DataTable table, IReadOnlyList<Cell> row)
        {
            foreach (var cell in row)
            {
                table.Columns.Add(cell.ToString(), typeof(string));
            }
        }

        private void CreateTab2Cols(DataTable table)
        {
            table.Columns.Add("Item", typeof(string));
            table.Columns.Add("Value", typeof(string));
        }
    }
}
