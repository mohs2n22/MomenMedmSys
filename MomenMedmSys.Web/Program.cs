using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MomenMedmSys.Data;
using MomenMedmSys.Data.Repositories;
using MomenMedmSys.Services;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Core.Enums;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Index";
        options.AccessDeniedPath = "/Index";
        options.Cookie.Name = "MomenMedmSys.Auth";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<ILicenseService, LicenseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDatabaseBackupService, DatabaseBackupService>();
builder.Services.AddScoped<IHardwareInfoService, HardwareInfoService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INetworkDiscoveryService, NetworkDiscoveryService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IStaffManagementService, StaffManagementService>();
builder.Services.AddScoped<IElectricalSafetyService, ElectricalSafetyService>();
builder.Services.AddScoped<IProcurementService, ProcurementService>();
builder.Services.AddScoped<IServiceContractService, ServiceContractService>();
builder.Services.AddScoped<ISparePartService, SparePartService>();
builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();
builder.Services.AddScoped<IRiskService, RiskService>();
builder.Services.AddScoped<ICalibrationService, CalibrationService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IHospitalSettingsService, HospitalSettingsService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var appConfig = new AppConfig();
builder.Configuration.GetSection("Database").Bind(appConfig.Database);
appConfig.Database.ContentRootPath = builder.Environment.ContentRootPath;
builder.Services.AddSingleton(appConfig);

var connectionString = appConfig.Database.ConnectionString;
Console.WriteLine($"Database: {connectionString}");

builder.Services.AddDbContext<MedMsysDbContext>(options =>
    options.UseSqlite(connectionString)
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MedMsysDbContext>();
        var authService = services.GetRequiredService<IAuthService>();
        var deviceRepo = services.GetRequiredService<IRepository<MedicalDevice>>();

        await context.Database.EnsureCreatedAsync();

        if (!await context.Users.AnyAsync(u => u.Username == "admin"))
        {
            var adminUser = new User
            {
                Username = "admin",
                FullName = "System Administrator",
                Email = "admin@medmsys.local",
                Role = UserRole.Admin,
                IsActive = true
            };
            await authService.CreateUserAsync(adminUser, "Admin@123");
            Console.WriteLine("Seeded admin user (username: admin, password: Admin@123)");
        }

        if (!await context.MedicalDevices.AnyAsync())
        {
            var sampleDevices = new[]
            {
                new MedicalDevice
                {
                    DeviceCode = "DEV-001",
                    DeviceName = "Infusion Pump",
                    Manufacturer = "Baxter",
                    Model = "Colleague CX",
                    SerialNumber = "BP-2024-001",
                    Category = "IV Pumps",
                    SubCategory = "Infusion Pumps",
                    PurchaseDate = DateTime.Now.AddDays(-365),
                    PurchasePrice = 3500,
                    WarrantyExpiryDate = DateTime.Now.AddDays(365),
                    Status = DeviceStatus.Active,
                    RiskClassification = RiskClass.High,
                    RequiresCalibration = true,
                    RequiresPreventiveMaintenance = true,
                    Room = "ICU-101"
                },
                new MedicalDevice
                {
                    DeviceCode = "DEV-002",
                    DeviceName = "Patient Monitor",
                    Manufacturer = "Philips",
                    Model = "IntelliVue MX400",
                    SerialNumber = "PH-2024-002",
                    Category = "Patient Monitoring",
                    SubCategory = "Multi-Parameter Monitor",
                    PurchaseDate = DateTime.Now.AddDays(-180),
                    PurchasePrice = 15000,
                    WarrantyExpiryDate = DateTime.Now.AddDays(545),
                    Status = DeviceStatus.Active,
                    RiskClassification = RiskClass.Critical,
                    RequiresCalibration = true,
                    RequiresElectricalSafetyTesting = true,
                    Room = "ICU-102"
                },
                new MedicalDevice
                {
                    DeviceCode = "DEV-003",
                    DeviceName = "Defibrillator",
                    Manufacturer = "Zoll",
                    Model = "X Series",
                    SerialNumber = "ZL-2024-003",
                    Category = "Defibrillators",
                    SubCategory = "External Defibrillator",
                    PurchaseDate = DateTime.Now.AddDays(-200),
                    PurchasePrice = 18000,
                    WarrantyExpiryDate = DateTime.Now.AddDays(535),
                    Status = DeviceStatus.Active,
                    RiskClassification = RiskClass.Critical,
                    RequiresCalibration = true,
                    RequiresElectricalSafetyTesting = true,
                    Room = "ER-201"
                },
                new MedicalDevice
                {
                    DeviceCode = "DEV-004",
                    DeviceName = "Ventilator",
                    Manufacturer = "Hamilton",
                    Model = "HAMILTON-C6",
                    SerialNumber = "HM-2024-004",
                    Category = "Ventilators",
                    SubCategory = "Critical Care Ventilator",
                    PurchaseDate = DateTime.Now.AddDays(-400),
                    PurchasePrice = 45000,
                    WarrantyExpiryDate = DateTime.Now.AddDays(335),
                    Status = DeviceStatus.Active,
                    RiskClassification = RiskClass.Critical,
                    RequiresCalibration = true,
                    RequiresPreventiveMaintenance = true,
                    RequiresElectricalSafetyTesting = true,
                    Room = "ICU-103"
                },
                new MedicalDevice
                {
                    DeviceCode = "DEV-005",
                    DeviceName = "Suction Unit",
                    Manufacturer = "Stryker",
                    Model = "1065",
                    SerialNumber = "SY-2024-005",
                    Category = "Surgical",
                    SubCategory = "Suction Units",
                    PurchaseDate = DateTime.Now.AddDays(-150),
                    PurchasePrice = 2200,
                    WarrantyExpiryDate = DateTime.Now.AddDays(575),
                    Status = DeviceStatus.Active,
                    RiskClassification = RiskClass.Medium,
                    RequiresPreventiveMaintenance = true,
                    Room = "OR-301"
                }
            };

            await context.MedicalDevices.AddRangeAsync(sampleDevices);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {sampleDevices.Length} sample devices");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Seeding error: {ex.Message}");
    }
}

app.UseExceptionHandler("/Error");
app.UseStatusCodePagesWithReExecute("/Error");

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
