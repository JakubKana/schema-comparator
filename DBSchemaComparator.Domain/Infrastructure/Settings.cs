using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using DBSchemaComparator.Domain.Models.General;
using MySql.Data.MySqlClient;
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

        public SettingsObject SettingsObject { get; set; }

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
                    SettingsObject = JsonConvert.DeserializeObject<SettingsObject>(json);
                    Logger.Debug($"Deserialized object:\n", SettingsObject);
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
                switch (connection.DbType.ToLower())
                {
                    case "mssql":
                        SqlConnectionStringBuilder msSqlBuilder = GetMsSqlStringBuilder(connection);

                        Logger.Trace($"Retrieving MS SQL database connection string = {msSqlBuilder.ConnectionString}");
                        return msSqlBuilder.ConnectionString;
                    case "mysql":
                        MySqlConnectionStringBuilder mySqlBuilder = GetMySqlStringBuilder(connection);
                        Logger.Trace($"Retrieving MySQL database connection string = {mySqlBuilder.ConnectionString}");
                        return mySqlBuilder.ConnectionString;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Cannot retrieve connection string.");
                return string.Empty;
            }
        }

        public static MySqlConnectionStringBuilder GetMySqlStringBuilder(DatabaseConnection connection)
        {
            return new MySqlConnectionStringBuilder
            {
                UserID = connection.Username,
                Password = connection.Pass,
                Server = connection.IpAddress,
                Database = connection.DbName,
                ConnectionTimeout = connection.Timeout
            };
        }
        public static MySqlConnectionStringBuilder GetMySqlStringBuilder(string connectionString)
        {
            return new MySqlConnectionStringBuilder(connectionString);
           
        }
        public static SqlConnectionStringBuilder GetMsSqlStringBuilder(DatabaseConnection connection)
        {
            return new SqlConnectionStringBuilder
            {
                UserID = connection.Username,
                Password = connection.Pass,
                InitialCatalog = connection.DbName,
                DataSource = connection.IpAddress,
                ConnectTimeout = (int)connection.Timeout
            };
        }
        public static SqlConnectionStringBuilder GetMsSqlStringBuilder(string connectionString)
        {
            return new SqlConnectionStringBuilder(connectionString);

        }

        public static List<string> GetDatabaseConnectionStrings(SettingsObject databaseConnection)
        {
            return databaseConnection.DatabaseConnections.Select(GetConnectionString).Take(2).ToList();
        }

       public bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

    }
}
