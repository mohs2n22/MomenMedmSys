using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.ViewModels.Base;
using Microsoft.Win32;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class AdvancedDashboardViewModel : ViewModelBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AdvancedDashboardViewModel(IAnalyticsService analyticsService)
        {
            Title = "Advanced Analytics Dashboard";
            _analyticsService = analyticsService;

            DateRangeOptions = new ObservableCollection<string>
            {
                "Last 30 Days",
                "Last 90 Days",
                "Last 6 Months",
                "Last 1 Year"
            };
            SelectedDateRange = "Last 1 Year";

            // Pre-create axis instances
            MaintenanceTrendXAxis = new Axis { LabelsRotation = -45, TextSize = 10 };
            MaintenanceTrendYAxis = new Axis { TextSize = 10 };
            DepartmentCostXAxis = new Axis { LabelsRotation = -30, TextSize = 10 };
            DepartmentCostYAxis = new Axis { TextSize = 10 };
            RiskIncidentXAxis = new Axis { LabelsRotation = -45, TextSize = 10 };
            RiskIncidentYAxis = new Axis { TextSize = 10 };

            LoadAllDataCommand.Execute(null);
        }

        // Date Range Filter
        [ObservableProperty] private ObservableCollection<string> _dateRangeOptions = new();
        [ObservableProperty] private string _selectedDateRange = string.Empty;

        // KPI Metrics
        [ObservableProperty] private double _equipmentAvailability;
        [ObservableProperty] private double _mtbf;
        [ObservableProperty] private double _mttr;
        [ObservableProperty] private double _maintenanceCompletionRate;
        [ObservableProperty] private double _calibrationCompliance;
        [ObservableProperty] private double _costPerDevice;
        [ObservableProperty] private int _openWorkOrders;
        [ObservableProperty] private int _overdueItems;

        // Chart Data - Work Order Status (Pie)
        [ObservableProperty] private ISeries[] _workOrderStatusSeries = Array.Empty<ISeries>();

        // Chart Data - Maintenance Trend (Line)
        [ObservableProperty] private ISeries[] _maintenanceTrendSeries = Array.Empty<ISeries>();
        [ObservableProperty] private string[] _maintenanceTrendLabels = Array.Empty<string>();
        [ObservableProperty] private Axis _maintenanceTrendXAxis;
        [ObservableProperty] private Axis _maintenanceTrendYAxis;

        // Chart Data - Device Status (Donut)
        [ObservableProperty] private ISeries[] _deviceStatusSeries = Array.Empty<ISeries>();

        // Chart Data - Department Costs (Bar)
        [ObservableProperty] private ISeries[] _departmentCostSeries = Array.Empty<ISeries>();
        [ObservableProperty] private string[] _departmentCostLabels = Array.Empty<string>();
        [ObservableProperty] private Axis _departmentCostXAxis;
        [ObservableProperty] private Axis _departmentCostYAxis;

        // Chart Data - Risk Incident Trend (Line)
        [ObservableProperty] private ISeries[] _riskIncidentSeries = Array.Empty<ISeries>();
        [ObservableProperty] private string[] _riskIncidentLabels = Array.Empty<string>();
        [ObservableProperty] private Axis _riskIncidentXAxis;
        [ObservableProperty] private Axis _riskIncidentYAxis;

        // Top Failing Equipment
        [ObservableProperty] private ObservableCollection<FailingEquipmentRow> _topFailingEquipment = new();

        // Warranty Expiry Timeline
        [ObservableProperty] private ObservableCollection<WarrantyExpiryRow> _warrantyExpiryTimeline = new();

        // Color palette
        private static readonly SKColor[] StatusColors = new SKColor[]
        {
            new SKColor(59, 130, 246),   // Blue - Open
            new SKColor(139, 92, 246),   // Purple - Assigned
            new SKColor(245, 158, 11),   // Orange - InProgress
            new SKColor(236, 72, 153),   // Pink - PendingParts
            new SKColor(16, 185, 129),   // Green - Completed
            new SKColor(107, 114, 128),  // Gray - Cancelled
            new SKColor(239, 68, 68),    // Red - OnHold
        };

        private static readonly SKColor[] DeviceStatusColors = new SKColor[]
        {
            new SKColor(16, 185, 129),   // Green - Active
            new SKColor(245, 158, 11),   // Orange - UnderMaintenance
            new SKColor(239, 68, 68),    // Red - OutOfService
            new SKColor(139, 92, 246),   // Purple - PendingCalibration
            new SKColor(107, 114, 128),  // Gray - Decommissioned
            new SKColor(75, 85, 99),     // DarkGray - Disposed
        };

        [RelayCommand]
        private async Task LoadAllData()
        {
            IsLoading = true;
            StatusMessage = "Loading analytics...";

            try
            {
                var (startDate, endDate) = GetDateRange();

                await Task.WhenAll(
                    LoadKpiMetricsAsync(startDate, endDate),
                    LoadWorkOrderStatusChartAsync(startDate, endDate),
                    LoadMaintenanceTrendChartAsync(),
                    LoadDeviceStatusChartAsync(),
                    LoadDepartmentCostChartAsync(startDate, endDate),
                    LoadRiskIncidentTrendAsync(),
                    LoadTopFailingEquipmentAsync(startDate, endDate),
                    LoadWarrantyExpiryTimelineAsync()
                );

                StatusMessage = $"Dashboard refreshed - {SelectedDateRange}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading analytics: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ExportToExcel()
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                    FileName = $"AnalyticsDashboard_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    DefaultExt = "xlsx"
                };

                if (saveDialog.ShowDialog() != true) return;

                using var workbook = new XLWorkbook();

                // KPI Summary Sheet
                var kpiSheet = workbook.Worksheets.Add("KPI Summary");
                kpiSheet.Cell(1, 1).Value = "KPI Metric";
                kpiSheet.Cell(1, 2).Value = "Value";
                kpiSheet.Range(1, 1, 1, 2).Style.Font.Bold = true;
                kpiSheet.Range(1, 1, 1, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(30, 41, 59);
                kpiSheet.Range(1, 1, 1, 2).Style.Font.FontColor = XLColor.White;

                var kpiData = new (string, string)[]
                {
                    ("Equipment Availability", $"{EquipmentAvailability}%"),
                    ("MTBF (days)", $"{Mtbf}"),
                    ("MTTR (hours)", $"{Mttr}"),
                    ("Maintenance Completion Rate", $"{MaintenanceCompletionRate}%"),
                    ("Calibration Compliance", $"{CalibrationCompliance}%"),
                    ("Avg Cost per Device", $"${CostPerDevice:N2}"),
                    ("Open Work Orders", $"{OpenWorkOrders}"),
                    ("Overdue Items", $"{OverdueItems}"),
                };

                for (int i = 0; i < kpiData.Length; i++)
                {
                    kpiSheet.Cell(i + 2, 1).Value = kpiData[i].Item1;
                    kpiSheet.Cell(i + 2, 2).Value = kpiData[i].Item2;
                }
                kpiSheet.Columns().AdjustToContents();

                // Work Order Status Sheet
                var woSheet = workbook.Worksheets.Add("Work Order Status");
                woSheet.Cell(1, 1).Value = "Status";
                woSheet.Cell(1, 2).Value = "Count";
                woSheet.Range(1, 1, 1, 2).Style.Font.Bold = true;
                woSheet.Range(1, 1, 1, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(30, 41, 59);
                woSheet.Range(1, 1, 1, 2).Style.Font.FontColor = XLColor.White;
                woSheet.Columns().AdjustToContents();

                // Maintenance Trend Sheet
                var maintSheet = workbook.Worksheets.Add("Maintenance Trend");
                maintSheet.Cell(1, 1).Value = "Month";
                maintSheet.Cell(1, 2).Value = "Preventive";
                maintSheet.Cell(1, 3).Value = "Corrective";
                maintSheet.Cell(1, 4).Value = "Emergency";
                maintSheet.Cell(1, 5).Value = "Total Cost";
                maintSheet.Range(1, 1, 1, 5).Style.Font.Bold = true;
                maintSheet.Range(1, 1, 1, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(30, 41, 59);
                maintSheet.Range(1, 1, 1, 5).Style.Font.FontColor = XLColor.White;
                maintSheet.Columns().AdjustToContents();

                // Device Status Sheet
                var devSheet = workbook.Worksheets.Add("Device Status");
                devSheet.Cell(1, 1).Value = "Status";
                devSheet.Cell(1, 2).Value = "Count";
                devSheet.Range(1, 1, 1, 2).Style.Font.Bold = true;
                devSheet.Range(1, 1, 1, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(30, 41, 59);
                devSheet.Range(1, 1, 1, 2).Style.Font.FontColor = XLColor.White;
                devSheet.Columns().AdjustToContents();

                // Department Costs Sheet
                var deptSheet = workbook.Worksheets.Add("Department Costs");
                deptSheet.Cell(1, 1).Value = "Department";
                deptSheet.Cell(1, 2).Value = "Labor Cost";
                deptSheet.Cell(1, 3).Value = "Parts Cost";
                deptSheet.Cell(1, 4).Value = "Total Cost";
                deptSheet.Cell(1, 5).Value = "Device Count";
                deptSheet.Range(1, 1, 1, 5).Style.Font.Bold = true;
                deptSheet.Range(1, 1, 1, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(30, 41, 59);
                deptSheet.Range(1, 1, 1, 5).Style.Font.FontColor = XLColor.White;
                deptSheet.Columns().AdjustToContents();

                // Top Failing Equipment Sheet
                var failSheet = workbook.Worksheets.Add("Top Failing Equipment");
                failSheet.Cell(1, 1).Value = "Device Name";
                failSheet.Cell(1, 2).Value = "Device Code";
                failSheet.Cell(1, 3).Value = "Category";
                failSheet.Cell(1, 4).Value = "Department";
                failSheet.Cell(1, 5).Value = "Maint Count";
                failSheet.Cell(1, 6).Value = "Total Cost";
                failSheet.Cell(1, 7).Value = "Downtime (Hrs)";
                failSheet.Range(1, 1, 1, 7).Style.Font.Bold = true;
                failSheet.Range(1, 1, 1, 7).Style.Fill.BackgroundColor = XLColor.FromArgb(30, 41, 59);
                failSheet.Range(1, 1, 1, 7).Style.Font.FontColor = XLColor.White;

                for (int i = 0; i < TopFailingEquipment.Count; i++)
                {
                    var item = TopFailingEquipment[i];
                    failSheet.Cell(i + 2, 1).Value = item.DeviceName;
                    failSheet.Cell(i + 2, 2).Value = item.DeviceCode;
                    failSheet.Cell(i + 2, 3).Value = item.Category;
                    failSheet.Cell(i + 2, 4).Value = item.Department;
                    failSheet.Cell(i + 2, 5).Value = item.MaintenanceCount;
                    failSheet.Cell(i + 2, 6).Value = (double)item.TotalCost;
                    failSheet.Cell(i + 2, 7).Value = item.DowntimeHours;
                }
                failSheet.Columns().AdjustToContents();

                // Warranty Expiry Sheet
                var warrantySheet = workbook.Worksheets.Add("Warranty Expiry");
                warrantySheet.Cell(1, 1).Value = "Quarter";
                warrantySheet.Cell(1, 2).Value = "Device Count";
                warrantySheet.Range(1, 1, 1, 2).Style.Font.Bold = true;
                warrantySheet.Range(1, 1, 1, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(30, 41, 59);
                warrantySheet.Range(1, 1, 1, 2).Style.Font.FontColor = XLColor.White;

                for (int i = 0; i < WarrantyExpiryTimeline.Count; i++)
                {
                    var item = WarrantyExpiryTimeline[i];
                    warrantySheet.Cell(i + 2, 1).Value = item.Quarter;
                    warrantySheet.Cell(i + 2, 2).Value = item.DeviceCount;
                }
                warrantySheet.Columns().AdjustToContents();

                workbook.SaveAs(saveDialog.FileName);
                StatusMessage = $"Exported to {Path.GetFileName(saveDialog.FileName)}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Export failed: {ex.Message}";
            }
        }

        private (DateTime? startDate, DateTime? endDate) GetDateRange()
        {
            var now = DateTime.Now;
            return SelectedDateRange switch
            {
                "Last 30 Days" => (now.AddDays(-30), now),
                "Last 90 Days" => (now.AddDays(-90), now),
                "Last 6 Months" => (now.AddMonths(-6), now),
                "Last 1 Year" => (now.AddYears(-1), now),
                _ => (null, null)
            };
        }

        private async Task LoadKpiMetricsAsync(DateTime? startDate, DateTime? endDate)
        {
            EquipmentAvailability = await _analyticsService.GetEquipmentAvailabilityAsync(startDate, endDate);
            Mtbf = await _analyticsService.GetMTBFAsync(startDate, endDate);
            Mttr = await _analyticsService.GetMTTRAsync(startDate, endDate);
            MaintenanceCompletionRate = await _analyticsService.GetMaintenanceCompletionRateAsync(startDate, endDate);
            CalibrationCompliance = await _analyticsService.GetCalibrationComplianceAsync(startDate, endDate);
            CostPerDevice = await _analyticsService.GetCostPerDeviceAsync(startDate, endDate);

            var woStatus = await _analyticsService.GetWorkOrderStatusDistributionAsync(startDate, endDate);
            OpenWorkOrders = woStatus.GetValueOrDefault("Open", 0) +
                             woStatus.GetValueOrDefault("Assigned", 0) +
                             woStatus.GetValueOrDefault("InProgress", 0);
            OverdueItems = woStatus.GetValueOrDefault("OnHold", 0);
        }

        private async Task LoadWorkOrderStatusChartAsync(DateTime? startDate, DateTime? endDate)
        {
            var distribution = await _analyticsService.GetWorkOrderStatusDistributionAsync(startDate, endDate);

            if (distribution.Count == 0)
            {
                WorkOrderStatusSeries = Array.Empty<ISeries>();
                return;
            }

            var seriesList = new List<ISeries>();
            int colorIndex = 0;

            foreach (var kvp in distribution)
            {
                seriesList.Add(new PieSeries<int>
                {
                    Values = new int[] { kvp.Value },
                    Name = kvp.Key,
                    Fill = new SolidColorPaint(StatusColors[colorIndex % StatusColors.Length]),
                    Stroke = new SolidColorPaint(SKColors.White, 2),
                    Pushout = 4,
                });
                colorIndex++;
            }

            WorkOrderStatusSeries = seriesList.ToArray();
        }

        private async Task LoadMaintenanceTrendChartAsync()
        {
            var trendData = await _analyticsService.GetMaintenanceByMonthAsync(12);

            if (trendData.Count == 0)
            {
                MaintenanceTrendSeries = Array.Empty<ISeries>();
                MaintenanceTrendLabels = Array.Empty<string>();
                return;
            }

            MaintenanceTrendLabels = trendData.Select(t => t.Month).ToArray();
            MaintenanceTrendXAxis.Labels = MaintenanceTrendLabels;

            var preventiveValues = trendData.Select(t => (int)t.PreventiveCount).ToArray();
            var correctiveValues = trendData.Select(t => (int)t.CorrectiveCount).ToArray();
            var emergencyValues = trendData.Select(t => (int)t.EmergencyCount).ToArray();

            MaintenanceTrendSeries = new ISeries[]
            {
                new LineSeries<int>
                {
                    Name = "Preventive",
                    Values = preventiveValues,
                    Stroke = new SolidColorPaint(new SKColor(59, 130, 246), 3),
                    Fill = new SolidColorPaint(new SKColor(59, 130, 246, 20)),
                    GeometrySize = 12,
                    GeometryStroke = new SolidColorPaint(new SKColor(59, 130, 246), 2),
                    GeometryFill = new SolidColorPaint(new SKColor(59, 130, 246, 30)),
                },
                new LineSeries<int>
                {
                    Name = "Corrective",
                    Values = correctiveValues,
                    Stroke = new SolidColorPaint(new SKColor(245, 158, 11), 3),
                    Fill = new SolidColorPaint(new SKColor(245, 158, 11, 20)),
                    GeometrySize = 12,
                    GeometryStroke = new SolidColorPaint(new SKColor(245, 158, 11), 2),
                    GeometryFill = new SolidColorPaint(new SKColor(245, 158, 11, 30)),
                },
                new LineSeries<int>
                {
                    Name = "Emergency",
                    Values = emergencyValues,
                    Stroke = new SolidColorPaint(new SKColor(239, 68, 68), 3),
                    Fill = new SolidColorPaint(new SKColor(239, 68, 68, 20)),
                    GeometrySize = 12,
                    GeometryStroke = new SolidColorPaint(new SKColor(239, 68, 68), 2),
                    GeometryFill = new SolidColorPaint(new SKColor(239, 68, 68, 30)),
                },
            };
        }

        private async Task LoadDeviceStatusChartAsync()
        {
            var distribution = await _analyticsService.GetDeviceStatusDistributionAsync();

            if (distribution.Count == 0)
            {
                DeviceStatusSeries = Array.Empty<ISeries>();
                return;
            }

            var seriesList = new List<ISeries>();
            int colorIndex = 0;

            foreach (var kvp in distribution)
            {
                seriesList.Add(new PieSeries<int>
                {
                    Values = new int[] { kvp.Value },
                    Name = kvp.Key,
                    Fill = new SolidColorPaint(DeviceStatusColors[colorIndex % DeviceStatusColors.Length]),
                    Stroke = new SolidColorPaint(SKColors.White, 2),
                    Pushout = 4,
                    IsHoverable = true,
                });
                colorIndex++;
            }

            DeviceStatusSeries = seriesList.ToArray();
        }

        private async Task LoadDepartmentCostChartAsync(DateTime? startDate, DateTime? endDate)
        {
            var deptCosts = await _analyticsService.GetDepartmentCostComparisonAsync(startDate, endDate);
            var topDepts = deptCosts.Take(8).ToList();

            if (topDepts.Count == 0)
            {
                DepartmentCostSeries = Array.Empty<ISeries>();
                DepartmentCostLabels = Array.Empty<string>();
                return;
            }

            DepartmentCostLabels = topDepts.Select(d => d.DepartmentName).ToArray();
            DepartmentCostXAxis.Labels = DepartmentCostLabels;

            var maintCostValues = topDepts.Select(d => (double)d.MaintenanceCost).ToArray();
            var partsCostValues = topDepts.Select(d => (double)d.PartsCost).ToArray();

            DepartmentCostSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Labor Cost",
                    Values = maintCostValues,
                    Fill = new SolidColorPaint(new SKColor(59, 130, 246)),
                    Stroke = new SolidColorPaint(new SKColor(30, 64, 175), 1),
                    Rx = 4,
                    Ry = 4,
                },
                new ColumnSeries<double>
                {
                    Name = "Parts Cost",
                    Values = partsCostValues,
                    Fill = new SolidColorPaint(new SKColor(16, 185, 129)),
                    Stroke = new SolidColorPaint(new SKColor(5, 150, 105), 1),
                    Rx = 4,
                    Ry = 4,
                },
            };
        }

        private async Task LoadRiskIncidentTrendAsync()
        {
            var trendData = await _analyticsService.GetRiskIncidentTrendAsync(12);

            if (trendData.Count == 0)
            {
                RiskIncidentSeries = Array.Empty<ISeries>();
                RiskIncidentLabels = Array.Empty<string>();
                return;
            }

            RiskIncidentLabels = trendData.Select(t => t.Month).ToArray();
            RiskIncidentXAxis.Labels = RiskIncidentLabels;

            var incidentValues = trendData.Select(t => (int)t.IncidentCount).ToArray();

            RiskIncidentSeries = new ISeries[]
            {
                new LineSeries<int>
                {
                    Name = "Incidents",
                    Values = incidentValues,
                    Stroke = new SolidColorPaint(new SKColor(239, 68, 68), 3),
                    Fill = new SolidColorPaint(new SKColor(239, 68, 68, 20)),
                    GeometrySize = 12,
                    GeometryStroke = new SolidColorPaint(new SKColor(239, 68, 68), 2),
                    GeometryFill = new SolidColorPaint(new SKColor(239, 68, 68, 40)),
                },
            };
        }

        private async Task LoadTopFailingEquipmentAsync(DateTime? startDate, DateTime? endDate)
        {
            var equipment = await _analyticsService.GetTopFailingEquipmentAsync(10, startDate, endDate);

            TopFailingEquipment.Clear();
            foreach (var item in equipment)
            {
                TopFailingEquipment.Add(new FailingEquipmentRow
                {
                    DeviceName = item.DeviceName,
                    DeviceCode = item.DeviceCode,
                    Category = item.Category,
                    Department = item.Department,
                    MaintenanceCount = item.MaintenanceCount,
                    TotalCost = item.TotalCost,
                    DowntimeHours = Math.Round(item.DowntimeHours, 1)
                });
            }
        }

        private async Task LoadWarrantyExpiryTimelineAsync()
        {
            var timeline = await _analyticsService.GetWarrantyExpiryTimelineAsync();

            WarrantyExpiryTimeline.Clear();
            foreach (var item in timeline)
            {
                WarrantyExpiryTimeline.Add(new WarrantyExpiryRow
                {
                    Quarter = item.Quarter,
                    DeviceCount = item.DeviceCount,
                    DeviceList = string.Join(", ", item.Devices.Take(3).Select(d => d.DeviceName)) +
                                 (item.Devices.Count > 3 ? $" +{item.Devices.Count - 3} more" : "")
                });
            }
        }
    }

    public partial class FailingEquipmentRow : ObservableObject
    {
        [ObservableProperty] private string _deviceName = string.Empty;
        [ObservableProperty] private string _deviceCode = string.Empty;
        [ObservableProperty] private string _category = string.Empty;
        [ObservableProperty] private string _department = string.Empty;
        [ObservableProperty] private int _maintenanceCount;
        [ObservableProperty] private decimal _totalCost;
        [ObservableProperty] private double _downtimeHours;
    }

    public partial class WarrantyExpiryRow : ObservableObject
    {
        [ObservableProperty] private string _quarter = string.Empty;
        [ObservableProperty] private int _deviceCount;
        [ObservableProperty] private string _deviceList = string.Empty;
    }
}
