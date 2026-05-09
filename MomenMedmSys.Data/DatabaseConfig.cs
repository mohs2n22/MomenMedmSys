namespace MomenMedmSys.Data
{
    /// <summary>
    /// MySQL database configuration.
    /// Connection string should be sourced from appsettings.json or environment variables.
    /// </summary>
    public class DatabaseConfig
    {
        public string Server { get; set; } = "localhost";
        public string Database { get; set; } = "medmsys";
        public string User { get; set; } = "root";
        public string Password { get; set; } = "";
        public bool SslMode { get; set; } = false;
        public bool AllowPublicKeyRetrieval { get; set; } = true;
        public uint Port { get; set; } = 3306;

        public string ConnectionString
        {
            get
            {
                var ssl = SslMode ? "Required" : "None";
                return $"Server={Server};Port={Port};Database={Database};User={User};Password={Password};SslMode={ssl};AllowPublicKeyRetrieval={AllowPublicKeyRetrieval};";
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
