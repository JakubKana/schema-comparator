using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DBSchemaComparator.Domain.Infrastructure;
using DBSchemaComparator.Domain.Models.SQLServer;
using NLog;
using PetaPoco;

namespace DBSchemaComparator.Domain.Database
{
    public class DatabaseHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public PetaPoco.Database Database
        {
            get; private set;
        }

        private DatabaseType DbType { get; set; }


        public DatabaseHandler(string connectionString, DatabaseType databaseType)
        {
            CreateDatabaseConnection(connectionString, databaseType);
        }

        private void CreateDatabaseConnection(string connectionString, DatabaseType databaseType)
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


        public IList<Table> SelectTablesSchemaInfo(string tableName)
        {
           return SelectTablesInfo(tableName, InformationType.Tables);
        }

        public IList<Table> SelectTablesInfo(string tableName, InformationType infoType)
        {
            Logger.Info($"Selecting basic schema information about tables from database");

            Sql sqlQuery = GetSqlQuery(infoType);

            try
            {
                using (var db = Database)
                {
                    if (!string.IsNullOrWhiteSpace(tableName))
                    {
                        sqlQuery.Append("WHERE TABLE_NAME = @0", "%" + tableName + "%");
                    }
                    Logger.Info($"Querying database with {sqlQuery}");
                    var queryResult = db.Query<Table>(sqlQuery).ToList();

                    return queryResult;
                }
            }
            catch (SqlException exception)
            {
                Logger.Warn(exception, "Unable to retrieve basic tables schema information from database.");
                return null;
            }
            catch (Exception exception)
            {
                Logger.Warn(exception);
                return null;
            }
        }

        private static Sql GetSqlQuery(InformationType infoType)
        {
            var sqlQuery = Sql.Builder;
            
            switch (infoType)
            {
                    case InformationType.Tables:
                    sqlQuery.Append(@"SELECT TABLE_NAME FROM [INFORMATION_SCHEMA].[TABLES]");
                    break;
                    case InformationType.Columns:
                    sqlQuery.Append(@"SELECT TABLE_NAME FROM [INFORMATION_SCHEMA].[COLUMNS]");
                    break;
            }

            return sqlQuery;
        }

        //public bool IsAvailible()
        //{
        //    try
        //    {
        //        using (var db = new PetaPoco.Database(Database.ConnectionString)))
        //        {
        //            db.Open();
        //            db.Close();
        //        }
        //    }
        //    catch (SqlException exception)
        //    {
        //        Logger.Warn($"Database unavailible {Database.ConnectionString}", exception);
        //        return false;
        //    }
        //    return true;

        //}
    }
}
