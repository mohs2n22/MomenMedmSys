using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Unified export service supporting Excel (.xlsx), CSV, and PDF formats.
    /// </summary>
    public class ExportService : IExportService
    {
        static ExportService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        #region Excel Export

        public async Task ExportToExcelAsync(string filePath, string sheetName, string[] headers,
            IEnumerable<object[]> rows, Action<IXLWorksheet>? styleAction = null)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            // Header row
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E293B");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Data rows
            int row = 2;
            foreach (var rowData in rows)
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cell(row, i + 1);
                    var val = i < rowData.Length ? rowData[i] : null;
                    SetCellValue(cell, val);
                }

                // Alternate row shading
                if (row % 2 == 0)
                {
                    for (int i = 1; i <= headers.Length; i++)
                        worksheet.Cell(row, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                }
                row++;
            }

            styleAction?.Invoke(worksheet);

            // Auto-fit and freeze
            worksheet.Columns().AdjustToContents();
            worksheet.SheetView.FreezeRows(1);
            if (row > 1)
                worksheet.Range(1, 1, row - 1, headers.Length).SetAutoFilter();

            workbook.SaveAs(filePath);
        }

        public async Task ExportToExcelAsync(string filePath, Dictionary<string, (string[] Headers, IEnumerable<object[]> Rows)> sheets)
        {
            using var workbook = new XLWorkbook();

            foreach (var kvp in sheets)
            {
                var worksheet = workbook.Worksheets.Add(kvp.Key);
                var (headers, rows) = kvp.Value;

                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cell(1, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E293B");
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                int row = 2;
                foreach (var rowData in rows)
                {
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cell(row, i + 1);
                        var val = i < rowData.Length ? rowData[i] : null;
                        SetCellValue(cell, val);
                    }
                    if (row % 2 == 0)
                        for (int i = 1; i <= headers.Length; i++)
                            worksheet.Cell(row, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                    row++;
                }

                worksheet.Columns().AdjustToContents();
                worksheet.SheetView.FreezeRows(1);
                if (row > 1)
                    worksheet.Range(1, 1, row - 1, headers.Length).SetAutoFilter();
            }

            workbook.SaveAs(filePath);
        }

        private static void SetCellValue(IXLCell cell, object? value)
        {
            if (value == null)
            {
                cell.Value = string.Empty;
            }
            else if (value is DateTime dt)
            {
                cell.Value = dt;
                cell.Style.NumberFormat.Format = "yyyy-MM-dd";
            }
            else if (value is decimal d)
            {
                cell.Value = d;
                cell.Style.NumberFormat.Format = "#,##0.00";
            }
            else if (value is double db)
            {
                cell.Value = db;
                cell.Style.NumberFormat.Format = "#,##0.00";
            }
            else if (value is int iv)
            {
                cell.Value = iv;
            }
            else if (value is bool b)
            {
                cell.Value = b ? "Yes" : "No";
            }
            else
            {
                cell.Value = value.ToString() ?? string.Empty;
            }
        }

        #endregion

        #region CSV Export

        public async Task ExportToCsvAsync(string filePath, string[] headers, IEnumerable<object[]> rows)
        {
            var sb = new StringBuilder();

            // Header
            sb.AppendLine(string.Join(",", headers.Select(EscapeCsv)));

            // Data
            foreach (var rowData in rows)
            {
                var line = new string[headers.Length];
                for (int i = 0; i < headers.Length; i++)
                {
                    var val = i < rowData.Length ? rowData[i] : null;
                    line[i] = EscapeCsv(val);
                }
                sb.AppendLine(string.Join(",", line));
            }

            await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
        }

        private static string EscapeCsv(object? value)
        {
            if (value == null) return string.Empty;
            var s = value.ToString() ?? string.Empty;
            // Escape commas, quotes, and newlines
            if (s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r'))
            {
                return $"\"{s.Replace("\"", "\"\"")}\"";
            }
            return s;
        }

        #endregion

        #region PDF Export

        public async Task ExportToPdfAsync(string filePath, string title, string subtitle,
            string[] headers, IEnumerable<object[]> rows,
            (string Label, string Value)[]? summaryFields = null)
        {
            var rowDataList = rows.ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Column(col =>
                    {
                        col.Item().Text(title).FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                        col.Item().Text(subtitle).FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").FontSize(8).FontColor(Colors.Grey.Medium);

                        if (summaryFields != null && summaryFields.Length > 0)
                        {
                            col.Item().PaddingTop(8).Row(row =>
                            {
                                foreach (var (label, value) in summaryFields)
                                {
                                    row.RelativeItem().Column(c =>
                                    {
                                        c.Item().Text(label).FontSize(7).Bold().FontColor(Colors.Grey.Medium);
                                        c.Item().Text(value).FontSize(8);
                                    });
                                }
                            });
                        }

                        col.Item().PaddingTop(8).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    });

                    page.Content().PaddingVertical(8).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                for (int i = 0; i < headers.Length; i++)
                                    columns.RelativeColumn();
                            });

                            // Header
                            table.Header(header =>
                            {
                                foreach (var h in headers)
                                {
                                    header.Cell().Background(Colors.Blue.Darken2).Padding(4)
                                        .Text(h).Bold().FontColor(Colors.White).FontSize(8);
                                }
                            });

                            // Rows
                            int rowIndex = 0;
                            foreach (var rowData in rowDataList)
                            {
                                var bgColor = rowIndex % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                                foreach (int i in Enumerable.Range(0, headers.Length))
                                {
                                    var val = i < rowData.Length ? rowData[i]?.ToString() ?? string.Empty : string.Empty;
                                    table.Cell().Background(bgColor).Padding(4).Text(val).FontSize(7);
                                }
                                rowIndex++;
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            document.GeneratePdf(filePath);
        }

        #endregion
    }
}
