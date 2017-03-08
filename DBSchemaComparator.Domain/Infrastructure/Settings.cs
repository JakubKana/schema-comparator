using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
                Logger.Debug($"Loading settings from file {configFilePath}");
                string applicationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFilePath);
                using (StreamReader reader = new StreamReader(applicationPath))
                {
                    string json = reader.ReadToEnd();
                    Logger.Debug($"Config file content:\n {json}");
                    Logger.Debug($"Deserializing json to object");
                    DatabaseConnections = JsonConvert.DeserializeObject<DatabaseConnectionList>(json);
                    Logger.Debug($"Deserialized object:\n", DatabaseConnections);
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
                                  $"connection timeout={connection.Timeout}");
                Logger.Trace($"Retrieving database connection string = {connectionString}");
                return connectionString;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Cannot retrieve connection string.");
                return string.Empty;
            }
        }

        public static List<string> GetDatabaseConnectionStrings(DatabaseConnectionList databaseConnection)
        {
            return databaseConnection.DatabaseConnections.Select(GetConnectionString).Take(3).ToList();
        }

    }
}
