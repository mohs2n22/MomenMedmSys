using System;
using Microsoft.Extensions.DependencyInjection;
using MomenMedmSys.Data;
using MomenMedmSys.Data.Repositories;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels;

namespace MomenMedmSys.WPF
{
    public static class AppStartup
    {
        public static IServiceProvider ConfigureServices(AppConfig config)
        {
            var services = new ServiceCollection();

            // Database
            services.AddSingleton(config);
            services.AddMedMsysDbContext(config);

            // Data layer
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Business services
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<ICalibrationService, CalibrationService>();
            services.AddScoped<IRiskService, RiskService>();
            services.AddScoped<IWorkOrderService, WorkOrderService>();
            services.AddScoped<ISparePartService, SparePartService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<ITrainingService, TrainingService>();
            services.AddScoped<IElectricalSafetyService, ElectricalSafetyService>();
            services.AddScoped<INetworkDiscoveryService, NetworkDiscoveryService>();
            services.AddScoped<IStaffManagementService, StaffManagementService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IHardwareInfoService, HardwareInfoService>();
            services.AddScoped<ILicenseService, LicenseService>();
            services.AddScoped<IDatabaseBackupService, DatabaseBackupService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IExportService, ExportService>();

            // WPF Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IViewFactory, ViewFactory>();
            services.AddSingleton<CurrentUserService>();

            // Form factories
            services.AddTransient<Func<DeviceFormViewModel>>(sp => () => sp.GetRequiredService<DeviceFormViewModel>());
            services.AddTransient<Func<MaintenanceFormViewModel>>(sp => () => sp.GetRequiredService<MaintenanceFormViewModel>());
            services.AddTransient<Func<CalibrationFormViewModel>>(sp => () => sp.GetRequiredService<CalibrationFormViewModel>());
            services.AddTransient<Func<SparePartFormViewModel>>(sp => () => sp.GetRequiredService<SparePartFormViewModel>());
            services.AddTransient<Func<RiskIncidentFormViewModel>>(sp => () => sp.GetRequiredService<RiskIncidentFormViewModel>());
            services.AddTransient<Func<WorkOrderFormViewModel>>(sp => () => sp.GetRequiredService<WorkOrderFormViewModel>());

            // MainViewModel - Singleton so all ViewModels share the SAME instance
            services.AddSingleton<MainViewModel>();
            services.AddTransient<DashboardViewModel>();
            // Form ViewModels (no MainVM needed - use App.MainViewModelInstance)
            services.AddTransient<DeviceFormViewModel>();
            services.AddTransient<MaintenanceFormViewModel>();
            services.AddTransient<CalibrationFormViewModel>();
            services.AddTransient<SparePartFormViewModel>();
            services.AddTransient<RiskIncidentFormViewModel>();
            services.AddTransient<WorkOrderFormViewModel>();
            services.AddTransient<ReportsViewModel>();
            services.AddTransient<NetworkDevicesViewModel>();
            services.AddTransient<AdminControlPanelViewModel>(sp =>
            {
                return new AdminControlPanelViewModel(
                    sp.GetRequiredService<IStaffManagementService>(),
                    sp.GetRequiredService<ILicenseService>(),
                    sp.GetRequiredService<IDatabaseBackupService>(),
                    sp.GetRequiredService<IAuditService>(),
                    sp.GetRequiredService<IDeviceService>(),
                    sp.GetRequiredService<IUnitOfWork>(),
                    sp.GetRequiredService<CurrentUserService>(),
                    sp.GetRequiredService<IDialogService>());
            });

            // DeviceListViewModel
            services.AddTransient<DeviceListViewModel>(sp =>
            {
                var vm = new DeviceListViewModel(
                    sp.GetRequiredService<IDeviceService>(),
                    sp.GetRequiredService<IDialogService>(),
                    sp.GetRequiredService<IExportService>(),
                    sp.GetRequiredService<Func<DeviceFormViewModel>>());
                vm.SetMainViewModel(sp.GetRequiredService<MainViewModel>());
                return vm;
            });

            // MaintenanceViewModel
            services.AddTransient<MaintenanceViewModel>(sp =>
            {
                var vm = new MaintenanceViewModel(
                    sp.GetRequiredService<IMaintenanceService>(),
                    sp.GetRequiredService<IDeviceService>(),
                    sp.GetRequiredService<IDialogService>(),
                    sp.GetRequiredService<IExportService>(),
                    sp.GetRequiredService<Func<MaintenanceFormViewModel>>());
                vm.SetMainViewModel(sp.GetRequiredService<MainViewModel>());
                return vm;
            });

            // CalibrationViewModel
            services.AddTransient<CalibrationViewModel>(sp =>
            {
                var vm = new CalibrationViewModel(
                    sp.GetRequiredService<ICalibrationService>(),
                    sp.GetRequiredService<IDeviceService>(),
                    sp.GetRequiredService<IDialogService>(),
                    sp.GetRequiredService<IExportService>(),
                    sp.GetRequiredService<Func<CalibrationFormViewModel>>());
                vm.SetMainViewModel(sp.GetRequiredService<MainViewModel>());
                return vm;
            });

            // SparePartsViewModel
            services.AddTransient<SparePartsViewModel>(sp =>
            {
                var vm = new SparePartsViewModel(
                    sp.GetRequiredService<ISparePartService>(),
                    sp.GetRequiredService<IDeviceService>(),
                    sp.GetRequiredService<IDialogService>(),
                    sp.GetRequiredService<IExportService>(),
                    sp.GetRequiredService<Func<SparePartFormViewModel>>());
                vm.SetMainViewModel(sp.GetRequiredService<MainViewModel>());
                return vm;
            });

            // RiskManagementViewModel
            services.AddTransient<RiskManagementViewModel>(sp =>
            {
                var vm = new RiskManagementViewModel(
                    sp.GetRequiredService<IRiskService>(),
                    sp.GetRequiredService<IDeviceService>(),
                    sp.GetRequiredService<IDialogService>(),
                    sp.GetRequiredService<IExportService>(),
                    sp.GetRequiredService<Func<RiskIncidentFormViewModel>>());
                vm.SetMainViewModel(sp.GetRequiredService<MainViewModel>());
                return vm;
            });

            // WorkOrdersViewModel
            services.AddTransient<WorkOrdersViewModel>(sp =>
            {
                var vm = new WorkOrdersViewModel(
                    sp.GetRequiredService<IWorkOrderService>(),
                    sp.GetRequiredService<IDeviceService>(),
                    sp.GetRequiredService<IDialogService>(),
                    sp.GetRequiredService<IExportService>(),
                    sp.GetRequiredService<Func<WorkOrderFormViewModel>>());
                vm.SetMainViewModel(sp.GetRequiredService<MainViewModel>());
                return vm;
            });

            // StaffViewModel
            services.AddTransient<StaffViewModel>(sp =>
            {
                var vm = new StaffViewModel(
                    sp.GetRequiredService<IStaffService>(),
                    sp.GetRequiredService<ITrainingService>(),
                    sp.GetRequiredService<IDialogService>());
                vm.SetMainViewModel(sp.GetRequiredService<MainViewModel>());
                return vm;
            });

            // ElectricalSafetyViewModel
            services.AddTransient<ElectricalSafetyViewModel>(sp =>
            {
                var vm = new ElectricalSafetyViewModel(
                    sp.GetRequiredService<IElectricalSafetyService>(),
                    sp.GetRequiredService<IDeviceService>(),
                    sp.GetRequiredService<IDialogService>());
                vm.SetMainViewModel(sp.GetRequiredService<MainViewModel>());
                return vm;
            });

            // AboutUsViewModel
            services.AddTransient<AboutUsViewModel>(sp =>
            {
                var vm = new AboutUsViewModel(
                    sp,
                    sp.GetRequiredService<IDatabaseBackupService>(),
                    sp.GetRequiredService<IAuditService>());
                vm.SetMainViewModel(sp.GetRequiredService<MainViewModel>());

                // Populate runtime counts
                try
                {
                    var deviceService = sp.GetRequiredService<IDeviceService>();
                    var staffService = sp.GetRequiredService<IStaffManagementService>();
                    var backupService = sp.GetRequiredService<IDatabaseBackupService>();
                    var devices = deviceService.GetAllDevicesAsync().Result;
                    var staff = staffService.GetAllStaffAsync().Result;
                    var backupDir = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Backups");
                    var backups = backupService.GetBackupHistoryAsync(backupDir).Result;
                    vm.SetDeviceCount(devices?.Count() ?? 0);
                    vm.SetStaffCount(staff?.Count() ?? 0);
                    vm.SetBackupCount(backups?.Count ?? 0);
                }
                catch { }

                return vm;
            });

            // ServicesDocViewModel
            services.AddTransient<ServicesDocViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
