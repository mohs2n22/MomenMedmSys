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
    public partial class RiskIncidentFormViewModel : ViewModelBase
    {
        private readonly IRiskService _riskService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;

        public string Mode { get; private set; } = "Add";
        public RiskIncident? EditingIncident { get; private set; }

        public RiskIncidentFormViewModel(IRiskService riskService, IDeviceService deviceService, IDialogService dialogService)
        {
            _riskService = riskService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            Title = "Add Risk Incident";
        }

        public void SetEditMode(RiskIncident incident)
        {
            Mode = "Edit";
            EditingIncident = incident;
            Title = "Edit Risk Incident";

            DeviceId = incident.DeviceId;
            IncidentCode = incident.IncidentCode;
            TitleText = incident.Title;
            Description = incident.Description;
            IncidentDate = incident.IncidentDate;
            ReportedBy = incident.ReportedBy;
            IncidentLocation = incident.IncidentLocation;
            IncidentType = incident.IncidentType;
            Severity = incident.Severity;
            Probability = incident.Probability;
            RootCause = incident.RootCause;
            InvestigationFindings = incident.InvestigationFindings;
            CorrectiveActions = incident.CorrectiveActions;
            PreventiveActions = incident.PreventiveActions;
            Status = incident.Status;
            IsRecall = incident.IsRecall;
            RecallNumber = incident.RecallNumber;
            RecallAuthority = incident.RecallAuthority;
            RecallDate = incident.RecallDate;
            RegulatoryReported = incident.RegulatoryReported;
            RegulatoryReportDate = incident.RegulatoryReportDate;
            RegulatoryReportReference = incident.RegulatoryReportReference;
            Resolution = incident.Resolution;
            PatientInjury = incident.PatientInjury;
            StaffInjury = incident.StaffInjury;
            AffectedPatients = incident.AffectedPatients;
            AffectedStaff = incident.AffectedStaff;
            StatusMessage = $"Editing: {incident.Title}";
        }

        public void SetAddMode(int? defaultDeviceId = null)
        {
            Mode = "Add";
            EditingIncident = null;
            Title = "Add Risk Incident";
            DeviceId = defaultDeviceId ?? 0;
            IncidentCode = string.Empty;
            TitleText = string.Empty;
            Description = string.Empty;
            IncidentDate = DateTime.Now;
            ReportedBy = string.Empty;
            IncidentLocation = string.Empty;
            IncidentType = string.Empty;
            Severity = SeverityLevel.Medium;
            Probability = ProbabilityLevel.Possible;
            RootCause = string.Empty;
            InvestigationFindings = string.Empty;
            CorrectiveActions = string.Empty;
            PreventiveActions = string.Empty;
            Status = IncidentStatus.Open;
            IsRecall = false;
            RecallNumber = string.Empty;
            RecallAuthority = string.Empty;
            RecallDate = null;
            RegulatoryReported = false;
            RegulatoryReportDate = null;
            RegulatoryReportReference = string.Empty;
            Resolution = string.Empty;
            PatientInjury = false;
            StaffInjury = false;
            AffectedPatients = 0;
            AffectedStaff = 0;
            StatusMessage = "Fill in incident details";
        }

        [ObservableProperty] private int _deviceId;
        [ObservableProperty] private string _incidentCode = string.Empty;
        [ObservableProperty] private string _titleText = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private DateTime _incidentDate = DateTime.Now;
        [ObservableProperty] private string _reportedBy = string.Empty;
        [ObservableProperty] private string _incidentLocation = string.Empty;
        [ObservableProperty] private string _incidentType = string.Empty;
        [ObservableProperty] private SeverityLevel _severity = SeverityLevel.Medium;
        [ObservableProperty] private ProbabilityLevel _probability = ProbabilityLevel.Possible;
        [ObservableProperty] private string _rootCause = string.Empty;
        [ObservableProperty] private string _investigationFindings = string.Empty;
        [ObservableProperty] private string _correctiveActions = string.Empty;
        [ObservableProperty] private string _preventiveActions = string.Empty;
        [ObservableProperty] private IncidentStatus _status = IncidentStatus.Open;
        [ObservableProperty] private bool _isRecall;
        [ObservableProperty] private string _recallNumber = string.Empty;
        [ObservableProperty] private string _recallAuthority = string.Empty;
        [ObservableProperty] private DateTime? _recallDate;
        [ObservableProperty] private bool _regulatoryReported;
        [ObservableProperty] private DateTime? _regulatoryReportDate;
        [ObservableProperty] private string _regulatoryReportReference = string.Empty;
        [ObservableProperty] private string _resolution = string.Empty;
        [ObservableProperty] private bool _patientInjury;
        [ObservableProperty] private bool _staffInjury;
        [ObservableProperty] private int _affectedPatients;
        [ObservableProperty] private int _affectedStaff;

        public SeverityLevel[] SeverityOptions => (SeverityLevel[])Enum.GetValues(typeof(SeverityLevel));
        public ProbabilityLevel[] ProbabilityOptions => (ProbabilityLevel[])Enum.GetValues(typeof(ProbabilityLevel));
        public IncidentStatus[] StatusOptions => (IncidentStatus[])Enum.GetValues(typeof(IncidentStatus));

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(TitleText))
            {
                await _dialogService.ShowMessageAsync("Title is required.", "Validation Error");
                return;
            }

            try
            {
                if (Mode == "Edit" && EditingIncident != null)
                {
                    EditingIncident.IncidentCode = IncidentCode;
                    EditingIncident.Title = TitleText;
                    EditingIncident.Description = Description;
                    EditingIncident.IncidentDate = IncidentDate;
                    EditingIncident.ReportedBy = ReportedBy;
                    EditingIncident.IncidentLocation = IncidentLocation;
                    EditingIncident.IncidentType = IncidentType;
                    EditingIncident.Severity = Severity;
                    EditingIncident.Probability = Probability;
                    EditingIncident.RootCause = RootCause;
                    EditingIncident.InvestigationFindings = InvestigationFindings;
                    EditingIncident.CorrectiveActions = CorrectiveActions;
                    EditingIncident.PreventiveActions = PreventiveActions;
                    EditingIncident.Status = Status;
                    EditingIncident.IsRecall = IsRecall;
                    EditingIncident.RecallNumber = RecallNumber;
                    EditingIncident.RecallAuthority = RecallAuthority;
                    EditingIncident.RecallDate = RecallDate;
                    EditingIncident.RegulatoryReported = RegulatoryReported;
                    EditingIncident.RegulatoryReportDate = RegulatoryReportDate;
                    EditingIncident.RegulatoryReportReference = RegulatoryReportReference;
                    EditingIncident.Resolution = Resolution;
                    EditingIncident.PatientInjury = PatientInjury;
                    EditingIncident.StaffInjury = StaffInjury;
                    EditingIncident.AffectedPatients = AffectedPatients;
                    EditingIncident.AffectedStaff = AffectedStaff;
                    EditingIncident.UpdatedAt = DateTime.Now;

                    await _riskService.UpdateIncidentAsync(EditingIncident);
                    StatusMessage = $"Updated: {TitleText}";
                }
                else
                {
                    var incident = new RiskIncident
                    {
                        DeviceId = DeviceId,
                        IncidentCode = string.IsNullOrWhiteSpace(IncidentCode) ? $"INC-{DateTime.Now:yyyy}-{DateTime.Now:yyyyMMdd}" : IncidentCode,
                        Title = TitleText,
                        Description = Description,
                        IncidentDate = IncidentDate,
                        ReportedBy = ReportedBy,
                        IncidentLocation = IncidentLocation,
                        IncidentType = IncidentType,
                        Severity = Severity,
                        Probability = Probability,
                        RootCause = RootCause,
                        InvestigationFindings = InvestigationFindings,
                        CorrectiveActions = CorrectiveActions,
                        PreventiveActions = PreventiveActions,
                        Status = Status,
                        IsRecall = IsRecall,
                        RecallNumber = RecallNumber,
                        RecallAuthority = RecallAuthority,
                        RecallDate = RecallDate,
                        RegulatoryReported = RegulatoryReported,
                        RegulatoryReportDate = RegulatoryReportDate,
                        RegulatoryReportReference = RegulatoryReportReference,
                        Resolution = Resolution,
                        PatientInjury = PatientInjury,
                        StaffInjury = StaffInjury,
                        AffectedPatients = AffectedPatients,
                        AffectedStaff = AffectedStaff,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    await _riskService.CreateIncidentAsync(incident);
                    StatusMessage = $"Created: {TitleText}";
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
