using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IViewFactory _viewFactory;

        // Navigation stack for back navigation (e.g., form -> list)
        private readonly System.Collections.Generic.Stack<ViewModelBase> _navStack = new();

        public MainViewModel(IServiceProvider serviceProvider, IViewFactory viewFactory)
        {
            Title = "MomenMedmSys - Medical Equipment Management System";
            _serviceProvider = serviceProvider;
            _viewFactory = viewFactory;

            BuildNavigation();
        }

        public ObservableCollection<NavigationItem> NavigationItems { get; } = new();

        private UserControl? _currentView;
        public UserControl? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        private bool _canGoBack;
        public bool CanGoBack
        {
            get => _canGoBack;
            set => SetProperty(ref _canGoBack, value);
        }

        private void BuildNavigation()
        {
            NavigationItems.Add(new NavigationItem { Name = "Dashboard", Icon = "📊", ViewModelType = typeof(DashboardViewModel) });
            NavigationItems.Add(new NavigationItem { Name = "Device Register", Icon = "🏥", ViewModelType = typeof(DeviceListViewModel) });
            NavigationItems.Add(new NavigationItem { Name = "Maintenance", Icon = "🔧", ViewModelType = typeof(MaintenanceViewModel) });
            NavigationItems.Add(new NavigationItem { Name = "Calibration", Icon = "📏", ViewModelType = typeof(CalibrationViewModel) });
            NavigationItems.Add(new NavigationItem { Name = "Spare Parts", Icon = "📦", ViewModelType = typeof(SparePartsViewModel) });
            NavigationItems.Add(new NavigationItem { Name = "Risk Management", Icon = "⚠️", ViewModelType = typeof(RiskManagementViewModel) });
            NavigationItems.Add(new NavigationItem { Name = "Work Orders", Icon = "📋", ViewModelType = typeof(WorkOrdersViewModel) });
            NavigationItems.Add(new NavigationItem { Name = "Network Devices", Icon = "🌐", ViewModelType = typeof(NetworkDevicesViewModel) });
            NavigationItems.Add(new NavigationItem { Name = "Admin Panel", Icon = "⚙️", ViewModelType = typeof(AdminControlPanelViewModel) });
            NavigationItems.Add(new NavigationItem { Name = "Staff & Training", Icon = "👥", ViewModelType = typeof(StaffViewModel) });
            NavigationItems.Add(new NavigationItem { Name = "Safety Tests", Icon = "⚡", ViewModelType = typeof(ElectricalSafetyViewModel) });
            NavigationItems.Add(new NavigationItem { Name = "Reports", Icon = "📈", ViewModelType = typeof(ReportsViewModel) });
            NavigationItems.Add(new NavigationItem { Name = "About Us", Icon = "ℹ️", ViewModelType = typeof(AboutUsViewModel) });
        }

        [RelayCommand]
        private void Navigate(int index)
        {
            if (index < 0 || index >= NavigationItems.Count) return;

            // Clear nav stack when clicking sidebar
            _navStack.Clear();
            CanGoBack = false;
            SelectedIndex = index;

            LoadViewModel(NavigationItems[index].ViewModelType);
        }

        /// <summary>
        /// Navigate to a specific ViewModel (e.g., form view)
        /// </summary>
        public void NavigateTo(ViewModelBase viewModel)
        {
            // Push current to stack
            var current = GetViewModelFromCurrentView();
            if (current != null)
            {
                _navStack.Push(current);
                CanGoBack = true;
            }

            CurrentView = _viewFactory.CreateViewFor(viewModel) as UserControl;
            StatusMessage = viewModel.StatusMessage;
        }

        [RelayCommand]
        private void GoBack()
        {
            if (_navStack.Count == 0) return;

            var previous = _navStack.Pop();
            CanGoBack = _navStack.Count > 0;
            CurrentView = _viewFactory.CreateViewFor(previous) as UserControl;
            StatusMessage = previous.StatusMessage;
        }

        private void LoadViewModel(Type viewModelType)
        {
            var viewModel = (ViewModelBase)_serviceProvider.GetRequiredService(viewModelType);
            CurrentView = _viewFactory.CreateViewFor(viewModel) as UserControl;
            StatusMessage = viewModel.StatusMessage;
        }

        private ViewModelBase? GetViewModelFromCurrentView()
        {
            return CurrentView?.DataContext as ViewModelBase;
        }
    }
}
