using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MomenMedmSys.WPF.ViewModels.Base
{
    public abstract class ViewModelBase : ObservableObject
    {
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
    }

    public class NavigationItem
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public Type ViewModelType { get; set; } = typeof(ViewModelBase);
    }
}
