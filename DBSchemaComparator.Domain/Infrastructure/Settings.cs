using System;
using System.Configuration;
using System.IO;
using DBSchemaComparator.Domain.Models.General;
using Newtonsoft.Json;
using NLog;

namespace DBSchemaComparator.Domain.Infrastructure
{
    /// <summary>
    /// Settings class provides model and methods for loading configuration file from JSON data format.
    /// </summary>
    public class Settings
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly string ConfigPath = ConfigurationManager.AppSettings["ConfigPath"];

        private static Settings _instance;

        public DatabaseConnectionList DatabaseConnections { get; set; }

        public static Settings Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                Logger.Info("Creating settings object");
                _instance = new Settings();
                return _instance;
            }
        }

        public Settings(string configFilePath)
        {
            try
            {
                string applicationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFilePath);
                using (StreamReader reader = new StreamReader(applicationPath))
                {
                    string json = reader.ReadToEnd();
                    DatabaseConnections = JsonConvert.DeserializeObject<DatabaseConnectionList>(json);
                }
            }
            catch (IOException exception)
            {
                Logger.Error(exception, "Unable to load configuration file.");
            }
        }

        private Settings() : this(ConfigPath) { }



        public static string GetConnectionString(DatabaseConnection connection)
        {
            try
            {
                var connectionString =
                    string.Format($"Data Source={connection.IpAddress}; " +
                                  $"Initial Catalog={connection.DbName}; " +
                                  $"user ID ={connection.Username}; " +
                                  $"password={connection.Pass};" +
                                  "connection timeout=30");
                Logger.Trace($"Retrieving database connection string = {connectionString}");
                return connectionString;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Cannot retrieve connection string.");
                return string.Empty;
            }

        }


    }
}
