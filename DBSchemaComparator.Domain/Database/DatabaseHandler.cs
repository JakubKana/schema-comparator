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

        public IList<Table> GetTablesSchemaInfo()
        {
            var tables = SelectTablesSchemaInfo();
            var columns = SelectColumnsSchemaInfo();
            var identity = SelectColumnsWithIdentity();
            var columnsWithIdentity = JoinIdentityInfoToColumns(columns, identity);
            return JoinColumnsToTables(tables, columnsWithIdentity);
        }

        public IList<Index> GetIndexesInfo()
        {
            return SelectSchemaInfo<Index>(InformationType.Indexes);
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


        private IList<Table> SelectTablesSchemaInfo()
        {
           return SelectSchemaInfo<Table>(InformationType.Tables);
        }

        private IList<Column> SelectColumnsSchemaInfo()
        {
            return SelectSchemaInfo<Column>(InformationType.Columns);
        }

        private IList<IdentityColumn> SelectColumnsWithIdentity()
        {

            return SelectSchemaInfo<IdentityColumn>(InformationType.IdentityColumns);
        }

        private static IList<Column> JoinIdentityInfoToColumns(IList<Column> columns, IList<IdentityColumn> identityColumns)
        {
            foreach (var ideCol in identityColumns)
            {
                foreach (var column in columns)
                {
                    if (ideCol.TableName == column.TableName && ideCol.ColumnName == column.ColumnName)
                    {
                        column.IsIdentification = true;
                    }
                }
            }
            return columns;
        }

        private static IList<Table> JoinColumnsToTables(IList<Table> tables, IList<Column> columns)
        {
            foreach (var table in tables)
            {
                var columnsOfTable = columns.Where(col => col.TableName == table.TableName);
                table.Columns = columnsOfTable.ToList();
            }
            return tables;
        }

        private IList<T> SelectSchemaInfo<T>(InformationType infoType)
        {
            Logger.Info($"Selecting basic schema information about tables from database");

            Sql sqlQuery = GetSqlQuery(infoType);

            try
            {
                using (var db = Database)
                {
                    Logger.Info($"Querying database with {sqlQuery}");
                    var queryResult = db.Query<T>(sqlQuery).ToList();
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
                    sqlQuery.Append(@"SELECT TABLE_NAME, COLUMN_NAME, IS_NULLABLE, DATA_TYPE FROM [INFORMATION_SCHEMA].[COLUMNS]");
                    break;
                    case InformationType.IdentityColumns:
                    sqlQuery.Append(@"SELECT TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS where COLUMNPROPERTY(object_id(TABLE_SCHEMA+'.'+TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1");
                    break;
                    case InformationType.Indexes:
                    //U => Only get indexes for User Created Tables
                    sqlQuery.Append(@"SELECT so.name AS TableName, si.name AS IndexName, si.type_desc AS IndexType, si.is_primary_key AS IsPrimaryKey, si.is_unique as IsUnique, c.name AS ColumnName FROM 
                                        sys.indexes si
                                        INNER JOIN sys.objects so ON si.[object_id] = so.[object_id]
			                            INNER JOIN sys.index_columns ic ON si.object_id = ic.object_id AND ic.index_id = si.index_id
			                            INNER JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
                                        WHERE
			                            so.type = 'U'    
                                        AND 
                                        si.name IS NOT NULL");
                    break;
            }
            Logger.Debug($"Returning SQL Query string {sqlQuery} of type {infoType}");
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
