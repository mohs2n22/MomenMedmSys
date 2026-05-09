namespace MomenMedmSys.Data
{
    public class DatabaseConfig
    {
        public string FileName { get; set; } = "medmsys.db";
        public string ContentRootPath { get; set; } = "";

        public string ConnectionString
        {
            get
            {
                var dbPath = string.IsNullOrEmpty(ContentRootPath)
                    ? FileName
                    : Path.Combine(ContentRootPath, FileName);
                return $"Data Source={dbPath}";
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
