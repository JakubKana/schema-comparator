using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

using DBSchemaComparator.Domain.Models.General;
using Newtonsoft.Json;

using NLog;



namespace DBSchemaComparator.App.Infrastructure
{
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
            using (StreamReader reader = new StreamReader(configFilePath))
            {
                string json = reader.ReadToEnd();
                DatabaseConnections = JsonConvert.DeserializeObject<DatabaseConnectionList>(json);
            }
           
        }

        private Settings() : this(ConfigPath)
        {
        }

        //public static string GetEntityConnectionString()
        //{
        //    string connectionString = new EntityConnectionStringBuilder
        //    {
        //        Metadata = "res://*/DataModel.MailingWebModel.csdl|res://*/DataModel.MailingWebModel.ssdl|res://*/DataModel.MailingWebModel.msl",
        //        Provider = "System.Data.SqlClient",
        //        ProviderConnectionString = new System.Data.SqlClient.SqlConnectionStringBuilder
        //        {
        //            InitialCatalog = _instance.DbName,
        //            DataSource = _instance.IpAddress,
        //            IntegratedSecurity = false,
        //            UserID = _instance.DbUser,
        //            Password = _instance.DbPass,
        //        }.ConnectionString
        //    }.ConnectionString;

        //    return connectionString;
        //}

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
                Logger.Trace($"Retrieving connectionString = {connectionString}");
                return connectionString;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Cannot retrieve connection string.");
                return String.Empty;
            }

        }


    }
}
