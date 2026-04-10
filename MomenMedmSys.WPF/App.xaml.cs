using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MomenMedmSys.Data;
using MomenMedmSys.WPF.ViewModels;

namespace MomenMedmSys.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }
        public static MainViewModel? MainViewModelInstance { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Register global exception handlers
            DispatcherUnhandledException += (s, args) =>
            {
                System.Diagnostics.Debug.WriteLine($"[UI Exception] {args.Exception}");
                MessageBox.Show($"An error occurred: {args.Exception.Message}",
                    "MomenMedmSys", MessageBoxButton.OK, MessageBoxImage.Warning);
                args.Handled = true;
            };
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                    System.Diagnostics.Debug.WriteLine($"[Fatal] {ex}");
            };
            TaskScheduler.UnobservedTaskException += (s, args) =>
            {
                System.Diagnostics.Debug.WriteLine($"[Task Error] {args.Exception}");
                args.SetObserved();
            };

            base.OnStartup(e);

            try
            {
                var config = new AppConfig
                {
                    Database = new DatabaseConfig { DatabasePath = "medmsys.db" }
                };

                ServiceProvider = AppStartup.ConfigureServices(config);

                // Show main window
                var mainWindow = new MainWindow();
                mainWindow.Show();

                // Store MainViewModel reference for form ViewModels to use
                if (mainWindow.DataContext is MainViewModel vm)
                {
                    MainViewModelInstance = vm;
                }

                // Seed database on background thread (non-blocking)
                _ = System.Threading.Tasks.Task.Run(async () =>
                {
                    try
                    {
                        var seeded = await DatabaseSeeder.SeedAsync(ServiceProvider);
                        if (seeded)
                            System.Diagnostics.Debug.WriteLine("[Seeder] Database seeded successfully");
                        else
                            System.Diagnostics.Debug.WriteLine("[Seeder] Data already exists, skipping");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Seeder] Error: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Startup failed:\n{ex.Message}\n\n{ex.StackTrace}",
                    "MomenMedmSys Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }
    }
}
