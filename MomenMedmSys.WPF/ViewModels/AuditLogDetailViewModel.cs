using System;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    /// <summary>
    /// Detail view for a single audit log entry, showing old vs new values.
    /// </summary>
    public partial class AuditLogDetailViewModel : ViewModelBase
    {
        private readonly AuditLog _auditLog;

        public AuditLogDetailViewModel(AuditLog auditLog)
        {
            _auditLog = auditLog;
            Title = $"Audit Log Detail - {auditLog.Action} {auditLog.EntityType}";

            // Format JSON for display
            _oldValuesFormatted = FormatJson(auditLog.OldValues);
            _newValuesFormatted = FormatJson(auditLog.NewValues);
        }

        public int Id => _auditLog.Id;
        public string EntityType => _auditLog.EntityType;
        public int EntityId => _auditLog.EntityId;
        public string Action => _auditLog.Action;
        public int? UserId => _auditLog.UserId;
        public string UserName => _auditLog.UserName;
        public string? AffectedRecords => _auditLog.AffectedRecords;
        public string IpAddress => _auditLog.IpAddress;
        public DateTime Timestamp => _auditLog.Timestamp;
        public string? OldValuesRaw => _auditLog.OldValues;
        public string? NewValuesRaw => _auditLog.NewValues;

        private string _oldValuesFormatted = string.Empty;
        public string OldValuesFormatted
        {
            get => _oldValuesFormatted;
            private set => SetProperty(ref _oldValuesFormatted, value);
        }

        private string _newValuesFormatted = string.Empty;
        public string NewValuesFormatted
        {
            get => _newValuesFormatted;
            private set => SetProperty(ref _newValuesFormatted, value);
        }

        public bool HasOldValues => !string.IsNullOrWhiteSpace(_auditLog.OldValues);

        public bool HasNewValues => !string.IsNullOrWhiteSpace(_auditLog.NewValues);

        private static string FormatJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "(No data)";

            try
            {
                using var doc = JsonDocument.Parse(json);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                return JsonSerializer.Serialize(doc.RootElement, options);
            }
            catch
            {
                return json; // Return raw if not valid JSON
            }
        }

        [RelayCommand]
        private void GoBack()
        {
            // Detail view is read-only; just signal completion
        }
    }
}
