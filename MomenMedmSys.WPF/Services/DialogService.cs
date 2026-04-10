using System.Threading.Tasks;
using System.Windows;

namespace MomenMedmSys.WPF.Services
{
    public interface IDialogService
    {
        Task ShowMessageAsync(string message, string title = "Information");
        Task<bool> ShowConfirmAsync(string message, string title = "Confirmation");
    }

    public class DialogService : IDialogService
    {
        public Task ShowMessageAsync(string message, string title = "Information")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            return Task.CompletedTask;
        }

        public Task<bool> ShowConfirmAsync(string message, string title = "Confirmation")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return Task.FromResult(result == MessageBoxResult.Yes);
        }
    }
}
