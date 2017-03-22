using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DBSchemaComparator.Domain.Infrastructure;
using DBSchemaComparator.Domain.Models.SQLServer;
using DBSchemaComparator.Domain.SqlBuilder;
using NLog;
using PetaPoco;

namespace DBSchemaComparator.Domain.Database
{
    public class DatabaseHandler : BaseDatabase, IDatabaseHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DatabaseHandler(string connectionString, DatabaseType databaseType)
            : base(connectionString, databaseType) { }

        public IList<PrimaryKey> GetPrimaryKeysInfo()
        {
            return SelectSchemaInfo<PrimaryKey>(InformationType.PrimaryKeys);
        }

        public IList<Table> GetTablesSchemaInfo()
        {
            var tables = SelectTablesSchemaInfo();
            var columns = SelectColumnsSchemaInfo();
            var identity = SelectColumnsWithIdentity();
            var columnsWithIdentity = JoinIdentityInfoToColumns(columns, identity);
            return JoinColumnsToTables(tables, columnsWithIdentity);
        }

        public IList<StoredProcedure> GetStoredProceduresInfo()
        {
            return SelectSchemaInfo<StoredProcedure>(InformationType.StoredProcedure);
        }

        public IList<Collation> GetCollationInfo()
        {
            return SelectSchemaInfo<Collation>(InformationType.DatabaseCollation);
        }

        public IList<CheckConstraint> GetCheckConstraintsInfo()
        {
            return SelectSchemaInfo<CheckConstraint>(InformationType.Checks);
        }

        public IList<ForeignKey> GetForeignKeysInfo()
        {
            return SelectSchemaInfo<ForeignKey>(InformationType.ForeignKeys);
        }

        public IList<Index> GetIndexesInfo()
        {
            return SelectSchemaInfo<Index>(InformationType.Indexes);
        }

        public IList<Function> GetFunctionsInfo()
        {
            return SelectSchemaInfo<Function>(InformationType.Function);
        }

        public IList<View> GetViewsInfo()
        {
            return SelectSchemaInfo<View>(InformationType.View);
        }

        public IList<Table> SelectTablesSchemaInfo()
        {
            return SelectSchemaInfo<Table>(InformationType.Tables);
        }

        public IList<Column> SelectColumnsSchemaInfo()
        {
            return SelectSchemaInfo<Column>(InformationType.Columns);
        }

        public IList<IdentityColumn> SelectColumnsWithIdentity()
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

        public IList<T> SelectSchemaInfo<T>(InformationType infoType)
        {
            Logger.Info($"Selecting basic schema information of type: {infoType}");

            var queryBuilder = SqlBuilderFactory.Create(DbType, Database.ConnectionString);

            Sql sqlQuery = queryBuilder.GetSqlQueryString(infoType);

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
                Logger.Warn(exception, $"Unable to retrieve basic schema information of type {infoType} from database.");
                return null;
            }
            catch (Exception exception)
            {
                Logger.Warn(exception, "Unexpected error.");
                return null;
            }
        }

       
    }
}
