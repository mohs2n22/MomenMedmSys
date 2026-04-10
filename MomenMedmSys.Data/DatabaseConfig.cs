using System;
using System.IO;

namespace MomenMedmSys.Data
{
    public class DatabaseConfig
    {
        public string DatabasePath { get; set; } = "MomenMedmSys.db";
        
        public string ConnectionString
        {
            get
            {
                // Use absolute path if relative
                if (!Path.IsPathRooted(DatabasePath))
                {
                    var basePath = AppDomain.CurrentDomain.BaseDirectory;
                    return $"Data Source={Path.Combine(basePath, DatabasePath)}";
                }
                return $"Data Source={DatabasePath}";
            }
        }
    }

    public class AppConfig
    {
        public DatabaseConfig Database { get; set; } = new DatabaseConfig();
        public string ApplicationName { get; set; } = "MomenMedmSys - Medical Equipment Management System";
        public string Version { get; set; } = "1.0.0";
    }
}
