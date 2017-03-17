using System;
using System.Data.SqlClient;
using NLog;
using PetaPoco;

namespace DBSchemaComparator.Domain.Database
{
    public class BaseDatabase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private DatabaseType DbType { get; set; }

        public PetaPoco.Database Database
        {
            get; private set;
        }

       
        protected void CreateDatabaseConnection(string connectionString, DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    DbType = databaseType;
                    Database = new PetaPoco.Database(connectionString, new SqlServerDatabaseProvider());
                    break;
                case DatabaseType.MySql:
                    DbType = databaseType;
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
