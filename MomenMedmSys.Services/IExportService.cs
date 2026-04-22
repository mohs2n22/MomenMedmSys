using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Interface for unified export operations (Excel, CSV, PDF).
    /// </summary>
    public interface IExportService
    {
        // Excel
        Task ExportToExcelAsync(string filePath, string sheetName, string[] headers,
            IEnumerable<object[]> rows, Action<IXLWorksheet>? styleAction = null);

        Task ExportToExcelAsync(string filePath, Dictionary<string, (string[] Headers, IEnumerable<object[]> Rows)> sheets);

        // CSV
        Task ExportToCsvAsync(string filePath, string[] headers, IEnumerable<object[]> rows);

        // PDF
        Task ExportToPdfAsync(string filePath, string title, string subtitle,
            string[] headers, IEnumerable<object[]> rows,
            (string Label, string Value)[]? summaryFields = null);
    }
}
