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


        public string ConfigPath { get; }
        public string ConnString1 { get; }
        public string ConnString2 { get; }

        // private static Settings _instance;

        public static SettingsObject SettingsObject { get; set; }

        //public static Settings Instance
        //{
        //    get
        //    {
        //        if (_instance != null)
        //            return _instance;

        //        Logger.Info("Creating settings object");
        //        _instance = new Settings();
        //        return _instance;
        //    }
        //}

        public Settings(string configFilePath)
        {
            Logger.Info("Creating settings object");

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

        public Settings(string connString1, string connString2, string resultPath, string dbType)
        {
            Logger.Info("Creating settings object");

            ConnString1 = connString1;
            ConnString2 = connString2;
            ConfigPath = resultPath;
            var listStrings = new List<DatabaseConnection>();
            switch (dbType.ToLower())
            {
                case "mssql":
                    listStrings.Add(GetMsDatabaseConnection(connString1, dbType));
                    listStrings.Add(GetMsDatabaseConnection(connString2, dbType));
                    SettingsObject = new SettingsObject
                    {
                        DatabaseConnections = listStrings,
                        ResultPath = dbType
                    };
                    break;
                case "mysql":
                  
                    listStrings.Add(GetMyDatabaseConnection(connString1, dbType));
                    listStrings.Add(GetMyDatabaseConnection(connString2, dbType));
                    SettingsObject = new SettingsObject
                    {
                        DatabaseConnections = listStrings,
                        ResultPath = dbType
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    


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
            MySqlConnectionStringBuilder connectionBuilder;
            if (string.IsNullOrWhiteSpace(connection.Port))
            {
                connectionBuilder = new MySqlConnectionStringBuilder
                {
                    UserID = connection.Username,
                    Password = connection.Pass,
                    Server = connection.IpAddress,
                    Database = connection.DbName,
                    ConnectionTimeout = connection.Timeout,
                    Port = uint.Parse(connection.Port)
                };
            }
            else
            {
                connectionBuilder = new MySqlConnectionStringBuilder
                {
                    UserID = connection.Username,
                    Password = connection.Pass,
                    Server = connection.IpAddress,
                    Database = connection.DbName,
                    ConnectionTimeout = connection.Timeout
                };
            }
            return connectionBuilder;
        }

        public static MySqlConnectionStringBuilder GetMySqlStringBuilder(string connectionString)
        {
            return new MySqlConnectionStringBuilder(connectionString);
        }

        public static SqlConnectionStringBuilder GetMsSqlStringBuilder(DatabaseConnection connection)
        {
            SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder
            {
                UserID = connection.Username,
                Password = connection.Pass,
                InitialCatalog = connection.DbName,
                DataSource = connection.IpAddress,
                ConnectTimeout = (int) connection.Timeout
            };
            return connectionBuilder;
        }

        public static SqlConnectionStringBuilder GetMsSqlStringBuilder(string connectionString)
        {
            return new SqlConnectionStringBuilder(connectionString);
        }

        public static List<string> GetDatabaseConnectionStrings(SettingsObject databaseConnection)
        {
            return databaseConnection.DatabaseConnections.Select(GetConnectionString).Take(2).ToList();
        }

        public static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }



        private static DatabaseConnection GetMsDatabaseConnection(string connString1, string dbType)
        {
            SqlConnectionStringBuilder msSqlBuilder1 = new SqlConnectionStringBuilder(connString1);

            var dbConnection = new DatabaseConnection
            {
                DbName = msSqlBuilder1.InitialCatalog,
                DbType = dbType,
                IpAddress = msSqlBuilder1.DataSource,
                Pass = msSqlBuilder1.Password,
                Timeout = (uint) msSqlBuilder1.ConnectTimeout,
                Username = msSqlBuilder1.UserID
            };

            return dbConnection;
        }

        private DatabaseConnection GetMyDatabaseConnection(string connString1, string dbType)
        {
            MySqlConnectionStringBuilder mySqlBuilder = new MySqlConnectionStringBuilder(connString1);
            DatabaseConnection dbConnection;
            if (string.IsNullOrWhiteSpace(mySqlBuilder.Port.ToString()))
            {
                dbConnection = new DatabaseConnection
                {
                    DbName = mySqlBuilder.Database,
                    DbType = dbType,
                    IpAddress = mySqlBuilder.Server,
                    Pass = mySqlBuilder.Password,
                    Timeout = mySqlBuilder.ConnectionTimeout,
                    Username = mySqlBuilder.UserID
                };
            }
            else
            {
                dbConnection = new DatabaseConnection
                {
                    DbName = mySqlBuilder.Database,
                    DbType = dbType,
                    IpAddress = mySqlBuilder.Server,
                    Pass = mySqlBuilder.Password,
                    Timeout = mySqlBuilder.ConnectionTimeout,
                    Username = mySqlBuilder.UserID,
                    Port = mySqlBuilder.Port.ToString()
                };
            }
            return dbConnection;
        }
    }
}
