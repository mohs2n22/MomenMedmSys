using System;

namespace MomenMedmSys.WPF.Services
{
    public interface INavigationService
    {
        void NavigateTo<TViewModel>() where TViewModel : ViewModels.Base.ViewModelBase;
    }

    public class NavigationService : INavigationService
    {
        public void NavigateTo<TViewModel>() where TViewModel : ViewModels.Base.ViewModelBase
        {
            // Navigation logic handled by MainViewModel
        }
    }
}
