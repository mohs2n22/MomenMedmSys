using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Core.Enums;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    /// <summary>
    /// ViewModel for the notification panel (slide-out from top-right)
    /// </summary>
    public partial class NotificationPanelViewModel : ViewModelBase
    {
        private readonly INotificationService _notificationService;
        private readonly CurrentUserService _currentUserService;
        private Action? _onNavigate;

        [ObservableProperty]
        private bool _isOpen;

        [ObservableProperty]
        private int _unreadCount;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private NotificationType? _filterType;

        [ObservableProperty]
        private NotificationPriority? _filterPriority;

        public ObservableCollection<Notification> Notifications { get; } = new();

        public NotificationPanelViewModel(INotificationService notificationService, CurrentUserService currentUserService)
        {
            _notificationService = notificationService;
            _currentUserService = currentUserService;
            Title = "Notifications";
        }

        public void SetOnNavigate(Action onNavigate)
        {
            _onNavigate = onNavigate;
        }

        [RelayCommand]
        public async Task OpenPanelAsync()
        {
            IsOpen = true;
            await LoadNotificationsAsync();
        }

        [RelayCommand]
        private void ClosePanel()
        {
            IsOpen = false;
        }

        [RelayCommand]
        private async Task MarkAllAsReadAsync()
        {
            await _notificationService.MarkAllAsReadAsync(_currentUserService.CurrentUser?.Id);
            UnreadCount = 0;
            foreach (var n in Notifications)
            {
                n.IsRead = true;
                n.ReadAt = DateTime.Now;
            }
        }

        [RelayCommand]
        private async Task MarkAsReadAsync(Notification notification)
        {
            if (!notification.IsRead)
            {
                await _notificationService.MarkAsReadAsync(notification.Id);
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
                UnreadCount = Math.Max(0, UnreadCount - 1);
            }
        }

        [RelayCommand]
        private async Task DeleteNotificationAsync(Notification notification)
        {
            await _notificationService.DeleteNotificationAsync(notification.Id);
            Notifications.Remove(notification);
            if (!notification.IsRead)
            {
                UnreadCount = Math.Max(0, UnreadCount - 1);
            }
        }

        [RelayCommand]
        private void NavigateToEntity(Notification notification)
        {
            // Mark as read
            if (!notification.IsRead)
            {
                _ = _notificationService.MarkAsReadAsync(notification.Id);
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
                UnreadCount = Math.Max(0, UnreadCount - 1);
            }

            ClosePanel();
            _onNavigate?.Invoke();
        }

        [RelayCommand]
        private async Task ApplyFiltersAsync()
        {
            await LoadNotificationsAsync();
        }

        [RelayCommand]
        private void ClearFilters()
        {
            FilterType = null;
            FilterPriority = null;
            _ = LoadNotificationsAsync();
        }

        public async Task RefreshAsync()
        {
            UnreadCount = await _notificationService.GetUnreadCountAsync(_currentUserService.CurrentUser?.Id);
        }

        private async Task LoadNotificationsAsync()
        {
            IsLoading = true;
            try
            {
                var notifications = await _notificationService.GetNotificationsAsync(
                    _currentUserService.CurrentUser?.Id,
                    count: 100);

                // Apply filters
                if (FilterType.HasValue)
                    notifications = notifications.Where(n => n.Type == FilterType.Value);

                if (FilterPriority.HasValue)
                    notifications = notifications.Where(n => n.Priority == FilterPriority.Value);

                Notifications.Clear();
                foreach (var n in notifications)
                {
                    Notifications.Add(n);
                }

                UnreadCount = await _notificationService.GetUnreadCountAsync(_currentUserService.CurrentUser?.Id);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
