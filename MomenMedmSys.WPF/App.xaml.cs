using System;
using System.IO;
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
        private static readonly string LogFile = "app_error.log";

        private static void Log(string msg)
        {
            try { File.AppendAllText(LogFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {msg}\n"); }
            catch { }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Clear previous log
            if (File.Exists(LogFile)) File.Delete(LogFile);
            Log("App starting");

            // Register global exception handlers
            DispatcherUnhandledException += (s, args) =>
            {
                Log($"[UI Exception] {args.Exception}");
                System.Diagnostics.Debug.WriteLine($"[UI Exception] {args.Exception}");
                MessageBox.Show($"An error occurred: {args.Exception.Message}",
                    "MomenMedmSys", MessageBoxButton.OK, MessageBoxImage.Warning);
                args.Handled = true;
            };
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                {
                    Log($"[Fatal] {ex}");
                    System.Diagnostics.Debug.WriteLine($"[Fatal] {ex}");
                }
            };
            TaskScheduler.UnobservedTaskException += (s, args) =>
            {
                Log($"[Task Error] {args.Exception}");
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

                Log("App started successfully");

                // Seed database on background thread (non-blocking)
                _ = System.Threading.Tasks.Task.Run(async () =>
                {
                    try
                    {
                        var seeded = await DatabaseSeeder.SeedAsync(ServiceProvider);
                        if (seeded)
                        {
                            Log("[Seeder] Database seeded successfully");
                            System.Diagnostics.Debug.WriteLine("[Seeder] Database seeded successfully");
                        }
                        else
                        {
                            Log("[Seeder] Data already exists, skipping");
                            System.Diagnostics.Debug.WriteLine("[Seeder] Data already exists, skipping");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"[Seeder] Error: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"[Seeder] Error: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                Log($"Startup failed: {ex.Message}\n{ex.StackTrace}");
                System.IO.File.WriteAllText("startup_error.log", $"Startup failed:\n{ex.Message}\n\n{ex.StackTrace}");
                MessageBox.Show($"Startup failed:\n{ex.Message}\n\n{ex.StackTrace}",
                    "MomenMedmSys Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }
    }
}
