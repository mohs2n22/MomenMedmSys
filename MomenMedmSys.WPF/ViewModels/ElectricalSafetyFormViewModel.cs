using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class ElectricalSafetyFormViewModel : ViewModelBase
    {
        private readonly IElectricalSafetyService _safetyService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;

        public string Mode { get; private set; } = "Add";
        public ElectricalSafetyTest? EditingTest { get; private set; }

        public ElectricalSafetyFormViewModel(IElectricalSafetyService safetyService, IDeviceService deviceService, IDialogService dialogService)
        {
            _safetyService = safetyService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            Title = "Add Electrical Safety Test";
        }

        public void SetEditMode(ElectricalSafetyTest test)
        {
            Mode = "Edit";
            EditingTest = test;
            Title = "Edit Electrical Safety Test";

            DeviceId = test.DeviceId;
            TestDate = test.TestDate;
            NextDueDate = test.NextDueDate;
            TestType = test.TestType;
            TestStandard = test.TestStandard;
            TestEquipmentUsed = test.TestEquipmentUsed;
            TestEquipmentCalibration = test.TestEquipmentCalibration;
            EarthResistanceMeasured = test.EarthResistanceMeasured;
            EarthResistanceLimit = test.EarthResistanceLimit;
            LeakageCurrentMeasured = test.LeakageCurrentMeasured;
            LeakageCurrentLimit = test.LeakageCurrentLimit;
            InsulationResistanceMeasured = test.InsulationResistanceMeasured;
            InsulationResistanceLimit = test.InsulationResistanceLimit;
            TouchCurrentMeasured = test.TouchCurrentMeasured;
            TouchCurrentLimit = test.TouchCurrentLimit;
            VisualInspectionPass = test.VisualInspectionPass;
            OverallResult = test.OverallResult;
            Remarks = test.Remarks;
            CertificateNumber = test.CertificateNumber;
            PerformedBy = test.PerformedBy;
            TechnicianSignature = test.TechnicianSignature;
            IsExternalTester = test.IsExternalTester;
            TestingCompany = test.TestingCompany;
            StatusMessage = $"Editing: {test.CertificateNumber}";
        }

        public void SetAddMode(int? defaultDeviceId = null)
        {
            Mode = "Add";
            EditingTest = null;
            Title = "Add Electrical Safety Test";
            DeviceId = defaultDeviceId ?? 0;
            TestDate = DateTime.Now;
            NextDueDate = DateTime.Now.AddYears(1);
            TestType = SafetyTestType.FullSafetyTest;
            TestStandard = "IEC 60601-1";
            TestEquipmentUsed = string.Empty;
            TestEquipmentCalibration = string.Empty;
            EarthResistanceMeasured = null;
            EarthResistanceLimit = 0.2m;
            LeakageCurrentMeasured = null;
            LeakageCurrentLimit = 0.1m;
            InsulationResistanceMeasured = null;
            InsulationResistanceLimit = 2.0m;
            TouchCurrentMeasured = null;
            TouchCurrentLimit = 0.1m;
            VisualInspectionPass = true;
            OverallResult = SafetyTestResult.NotTested;
            Remarks = string.Empty;
            CertificateNumber = string.Empty;
            PerformedBy = string.Empty;
            TechnicianSignature = string.Empty;
            IsExternalTester = false;
            TestingCompany = string.Empty;
            StatusMessage = "Fill in test details";
        }

        [ObservableProperty] private int _deviceId;
        [ObservableProperty] private DateTime _testDate = DateTime.Now;
        [ObservableProperty] private DateTime _nextDueDate = DateTime.Now.AddYears(1);
        [ObservableProperty] private SafetyTestType _testType = SafetyTestType.FullSafetyTest;
        [ObservableProperty] private string _testStandard = "IEC 60601-1";
        [ObservableProperty] private string _testEquipmentUsed = string.Empty;
        [ObservableProperty] private string _testEquipmentCalibration = string.Empty;
        [ObservableProperty] private decimal? _earthResistanceMeasured;
        [ObservableProperty] private decimal _earthResistanceLimit = 0.2m;
        [ObservableProperty] private decimal? _leakageCurrentMeasured;
        [ObservableProperty] private decimal _leakageCurrentLimit = 0.1m;
        [ObservableProperty] private decimal? _insulationResistanceMeasured;
        [ObservableProperty] private decimal _insulationResistanceLimit = 2.0m;
        [ObservableProperty] private decimal? _touchCurrentMeasured;
        [ObservableProperty] private decimal _touchCurrentLimit = 0.1m;
        [ObservableProperty] private bool _visualInspectionPass = true;
        [ObservableProperty] private SafetyTestResult _overallResult = SafetyTestResult.NotTested;
        [ObservableProperty] private string _remarks = string.Empty;
        [ObservableProperty] private string _certificateNumber = string.Empty;
        [ObservableProperty] private string _performedBy = string.Empty;
        [ObservableProperty] private string _technicianSignature = string.Empty;
        [ObservableProperty] private bool _isExternalTester;
        [ObservableProperty] private string _testingCompany = string.Empty;

        public SafetyTestType[] TestTypeOptions => (SafetyTestType[])Enum.GetValues(typeof(SafetyTestType));
        public SafetyTestResult[] ResultOptions => (SafetyTestResult[])Enum.GetValues(typeof(SafetyTestResult));

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(PerformedBy))
            {
                await _dialogService.ShowMessageAsync("Performed By is required.", "Validation Error");
                return;
            }

            try
            {
                if (Mode == "Edit" && EditingTest != null)
                {
                    EditingTest.TestDate = TestDate;
                    EditingTest.NextDueDate = NextDueDate;
                    EditingTest.TestType = TestType;
                    EditingTest.TestStandard = TestStandard;
                    EditingTest.TestEquipmentUsed = TestEquipmentUsed;
                    EditingTest.TestEquipmentCalibration = TestEquipmentCalibration;
                    EditingTest.EarthResistanceMeasured = EarthResistanceMeasured;
                    EditingTest.EarthResistanceLimit = EarthResistanceLimit;
                    EditingTest.LeakageCurrentMeasured = LeakageCurrentMeasured;
                    EditingTest.LeakageCurrentLimit = LeakageCurrentLimit;
                    EditingTest.InsulationResistanceMeasured = InsulationResistanceMeasured;
                    EditingTest.InsulationResistanceLimit = InsulationResistanceLimit;
                    EditingTest.TouchCurrentMeasured = TouchCurrentMeasured;
                    EditingTest.TouchCurrentLimit = TouchCurrentLimit;
                    EditingTest.VisualInspectionPass = VisualInspectionPass;
                    EditingTest.OverallResult = OverallResult;
                    EditingTest.Remarks = Remarks;
                    EditingTest.CertificateNumber = CertificateNumber;
                    EditingTest.PerformedBy = PerformedBy;
                    EditingTest.TechnicianSignature = TechnicianSignature;
                    EditingTest.IsExternalTester = IsExternalTester;
                    EditingTest.TestingCompany = TestingCompany;
                    EditingTest.UpdatedAt = DateTime.Now;

                    await _safetyService.UpdateTestAsync(EditingTest);
                    StatusMessage = $"Updated: {CertificateNumber}";
                }
                else
                {
                    var test = new ElectricalSafetyTest
                    {
                        DeviceId = DeviceId,
                        TestDate = TestDate,
                        NextDueDate = NextDueDate,
                        TestType = TestType,
                        TestStandard = TestStandard,
                        TestEquipmentUsed = TestEquipmentUsed,
                        TestEquipmentCalibration = TestEquipmentCalibration,
                        EarthResistanceMeasured = EarthResistanceMeasured,
                        EarthResistanceLimit = EarthResistanceLimit,
                        LeakageCurrentMeasured = LeakageCurrentMeasured,
                        LeakageCurrentLimit = LeakageCurrentLimit,
                        InsulationResistanceMeasured = InsulationResistanceMeasured,
                        InsulationResistanceLimit = InsulationResistanceLimit,
                        TouchCurrentMeasured = TouchCurrentMeasured,
                        TouchCurrentLimit = TouchCurrentLimit,
                        VisualInspectionPass = VisualInspectionPass,
                        OverallResult = OverallResult,
                        Remarks = Remarks,
                        CertificateNumber = string.IsNullOrWhiteSpace(CertificateNumber) ? $"EST-{DateTime.Now:yyyy}-{DateTime.Now:yyyyMMdd}" : CertificateNumber,
                        PerformedBy = PerformedBy,
                        TechnicianSignature = TechnicianSignature,
                        IsExternalTester = IsExternalTester,
                        TestingCompany = TestingCompany,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    await _safetyService.CreateTestAsync(test);
                    StatusMessage = $"Created: {CertificateNumber}";
                }

                App.MainViewModelInstance?.GoBackCommand.Execute(null);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Save error: {ex.Message}";
                await _dialogService.ShowMessageAsync($"Failed to save: {ex.Message}", "Error");
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            StatusMessage = "Form cancelled";
            App.MainViewModelInstance?.GoBackCommand.Execute(null);
        }
    }
}
