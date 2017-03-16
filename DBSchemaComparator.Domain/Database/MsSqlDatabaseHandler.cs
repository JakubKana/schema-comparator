﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DBSchemaComparator.Domain.Models.SQLServer;
using NLog;
using PetaPoco;

namespace DBSchemaComparator.Domain.Database
{
    public class MsSqlDatabaseHandler : BaseDatabase, IDatabaseHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MsSqlDatabaseHandler(string connectionString, DatabaseType databaseType)
        {
            CreateDatabaseConnection(connectionString, databaseType);
        }

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

            Sql sqlQuery = GetSqlQueryString(infoType);

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

        public Sql GetSqlQueryString(InformationType infoType)
        {

            var sqlQuery = Sql.Builder;
            switch (infoType)
            {
                case InformationType.Tables:
                    sqlQuery.Append(@"SELECT TABLE_NAME FROM [INFORMATION_SCHEMA].[TABLES] WHERE TABLE_TYPE = 'BASE TABLE';");
                    break;
                case InformationType.Columns:
                    sqlQuery.Append(@"SELECT TABLE_NAME, COLUMN_NAME, IS_NULLABLE, DATA_TYPE, COLLATION_NAME FROM [INFORMATION_SCHEMA].[COLUMNS]");
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
                case InformationType.StoredProcedure:
                    sqlQuery.Append(@"SELECT sp.ROUTINE_NAME as PROCEDURE_NAME, sp.ROUTINE_DEFINITION as PROCEDURE_BODY FROM [INFORMATION_SCHEMA].[ROUTINES] sp where routine_type = 'PROCEDURE'");
                    break;
                case InformationType.Function:
                    sqlQuery.Append(@"SELECT sp.ROUTINE_NAME as FUNCTION_NAME, sp.ROUTINE_DEFINITION as FUNCTION_BODY FROM [INFORMATION_SCHEMA].[ROUTINES] sp where routine_type = 'FUNCTION'");
                    break;
                case InformationType.View:
                    sqlQuery.Append(@"SELECT TABLE_NAME as VIEW_NAME, VIEW_DEFINITION as VIEW_BODY FROM INFORMATION_SCHEMA.VIEWS");
                    break;
                case InformationType.Checks:
                    sqlQuery.Append(@"SELECT cc.CONSTRAINT_NAME,
                                            TABLE_NAME, 
                                            COLUMN_NAME, 
                                            CHECK_CLAUSE
                                            FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS cc 
                                            INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE c 
                                            ON cc.CONSTRAINT_NAME = c.CONSTRAINT_NAME");
                    break;
                case InformationType.PrimaryKeys:
                    sqlQuery.Append(@"SELECT Col.Column_Name AS COLUMN_NAME, Tab.TABLE_NAME as TABLE_NAME, Col.CONSTRAINT_NAME as CONSTRAINT_NAME from 
                                        INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab, 
                                        INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col 
                                        WHERE 
                                        Col.Constraint_Name = Tab.Constraint_Name
                                        AND Col.Table_Name = Tab.Table_Name
                                        AND Constraint_Type = 'PRIMARY KEY'
                                    ");
                    break;
                case InformationType.ForeignKeys:
                    sqlQuery.Append(@"SELECT obj.name AS FOREIGN_KEY_NAME, tab1.name AS TABLE_NAME,
    col1.name AS COLUMN_NAME, tab2.name AS REFERENCED_TABLE,
    col2.name AS REFERENCED_COLUMN
FROM sys.foreign_key_columns fkc
INNER JOIN sys.objects obj
    ON obj.object_id = fkc.constraint_object_id
INNER JOIN sys.tables tab1
    ON tab1.object_id = fkc.parent_object_id
INNER JOIN sys.schemas sch
    ON tab1.schema_id = sch.schema_id
INNER JOIN sys.columns col1
    ON col1.column_id = parent_column_id AND col1.object_id = tab1.object_id
INNER JOIN sys.tables tab2
    ON tab2.object_id = fkc.referenced_object_id
INNER JOIN sys.columns col2
    ON col2.column_id = referenced_column_id AND col2.object_id = tab2.object_id");
                    break;
                case InformationType.DatabaseCollation:
                    sqlQuery.Append(@"SELECT CONVERT (varchar, SERVERPROPERTY('collation')) as COLLATION_TYPE");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(infoType), infoType, "Unable to retrieve query string.");
            }

            Logger.Debug($"Returning SQL Query string {sqlQuery} of type {infoType}");

            return sqlQuery;
        }

        
    }
}