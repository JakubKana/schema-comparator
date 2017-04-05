using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DBSchemaComparator.Domain.Infrastructure;
using NLog;
using PetaPoco;

namespace DBSchemaComparator.Domain.Database
{
    public class BaseDatabase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected DatabaseType DbType { get; set; }

        public string DatabaseName { get; set; }
        public PetaPoco.Database Database
        {
            get; private set;
        }
        public BaseDatabase(string connectionString, DatabaseType databaseType)
        {
            CreateDatabaseConnection(connectionString, databaseType);
        }

        protected void CreateDatabaseConnection(string connectionString, DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    DbType = databaseType;
                    var connStr = Settings.GetMsSqlStringBuilder(connectionString);
                    DatabaseName = connStr.InitialCatalog;
                    Database = new PetaPoco.Database(connectionString, new SqlServerDatabaseProvider());
                    break;
                case DatabaseType.MySql:
                    DbType = databaseType;
                    var connStg = Settings.GetMySqlStringBuilder(connectionString);
                    DatabaseName = connStg.Database;
                    Database = new PetaPoco.Database(connectionString, new MySqlDatabaseProvider());
                    break;
                default:
                    DbType = databaseType;
                    Database = new PetaPoco.Database(connectionString, new SqlServerDatabaseProvider());
                    break;
            }
        }

        public static DatabaseType GetDatabaseType(string dataType)
        {
            switch (dataType.ToLower())
            {
                case "mssql":
                    return DatabaseType.SqlServer;
                case "mysql":
                    return DatabaseType.MySql;
                default:
                    throw new ArgumentOutOfRangeException();

            }
        }
    }
}
