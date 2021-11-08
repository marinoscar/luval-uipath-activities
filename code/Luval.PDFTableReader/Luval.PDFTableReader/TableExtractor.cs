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
    public class TableExtractor
    {
        private FileInfo _fileInfo;

        public TableExtractor(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException("fileName");
            _fileInfo = new FileInfo(fileName);
            if (!_fileInfo.Exists) throw new ArgumentException(string.Format("File {0} does not exists", _fileInfo), "fileName");
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
                result.Add(CreateTable(table));
            }
            return result;
        }

        private DataTable CreateTable(Table table)
        {
            var dt = new DataTable();
            if (table == null || table.Rows == null || !table.Rows.Any()) return dt;
            bool isFirst = true;
            foreach (var row in table.Rows)
            {
                if (isFirst)
                {
                    CreateColumns(dt, row);
                    isFirst = false;
                }
                else
                    AddRow(dt, row);
            }
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

        private void CreateColumns(DataTable table, IReadOnlyList<Cell> row)
        {
            foreach (var cell in row)
            {
                table.Columns.Add(cell.ToString(), typeof(string));
            }
        }
    }
}
