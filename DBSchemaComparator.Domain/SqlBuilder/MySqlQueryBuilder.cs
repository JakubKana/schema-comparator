using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace DBSchemaComparator.Domain.SqlBuilder
{
    public class MySqlQueryBuilder : SqlBaseBuilder
    {
        public string DbName { get; set; }

        public MySqlQueryBuilder(string dbName)
        {
            DbName = dbName;
        }

        protected override string GetTableQuery() => $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA=DATABASE();";

        protected override string GetColumnsQuery() => $"SELECT TABLE_NAME, COLUMN_NAME, IS_NULLABLE, DATA_TYPE, COLLATION_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE();";

        protected override string GetIdentityColumnsQuery() => $"SELECT TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND EXTRA like '%auto_increment%';";

        protected override string GetIndexesQuery() => $"SELECT  stat.TABLE_NAME as TableName," +
                                                       $"stat.INDEX_NAME as IndexName," +
                                                       $"stat.INDEX_TYPE as IndexType, " +
                                                       $"CASE stat.INDEX_NAME WHEN 'PRIMARY' THEN 1 ELSE 0 END as IsPrimaryKey," +
                                                       $"CASE stat.NON_UNIQUE WHEN 0 THEN 1 WHEN 1 THEN 0 END as IsUnique, " +
                                                       $"stat.COLUMN_NAME as ColumnName " +
                                                       $"FROM information_schema.STATISTICS stat " +
                                                       $"where stat.INDEX_SCHEMA=DATABASE();";

        protected override string GetStoredProcedureQuery() => $"SELECT sp.ROUTINE_NAME as PROCEDURE_NAME, sp.ROUTINE_DEFINITION as PROCEDURE_BODY FROM INFORMATION_SCHEMA.ROUTINES sp where routine_type='PROCEDURE' AND ROUTINE_SCHEMA=DATABASE();";

        protected override string GetFunctionQuery() => $"SELECT sp.ROUTINE_NAME as FUNCTION_NAME, sp.ROUTINE_DEFINITION as FUNCTION_BODY FROM INFORMATION_SCHEMA.ROUTINES sp where routine_type = 'FUNCTION' AND ROUTINE_SCHEMA =DATABASE();";

        protected override string GetViewQuery() => $"SELECT TABLE_NAME as VIEW_NAME, VIEW_DEFINITION as VIEW_BODY FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA=DATABASE();";

        protected override string GetChecksQuery()
        {
            throw new NotSupportedException();
        }

        protected override string GetPrimaryKeysQuery() => $"SELECT Col.Column_Name AS COLUMN_NAME, Tab.TABLE_NAME as TABLE_NAME,Col.CONSTRAINT_NAME as CONSTRAINT_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab, INFORMATION_SCHEMA.KEY_COLUMN_USAGE Col WHERE Col.Constraint_Name=Tab.Constraint_Name AND Col.Table_Name=Tab.Table_Name AND CONSTRAINT_TYPE='PRIMARY KEY' AND Col.CONSTRAINT_SCHEMA=DATABASE();";

        protected override string GetForeignKeysQuery() => $"SELECT CONSTRAINT_NAME as FOREIGN_KEY_NAME, TABLE_NAME, REFERENCED_TABLE_NAME as REFERENCED_TABLE, REFERENCED_COLUMN_NAME as REFERENCED_COLUMN FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE REFERENCED_TABLE_NAME IS NOT NULL AND REFERENCED_COLUMN_NAME IS NOT NULL AND TABLE_SCHEMA = DATABASE();";

        protected override string GetDatabaseCollationQuery() => $"SELECT default_character_set_name as COLLATION_TYPE FROM information_schema.SCHEMATA WHERE schema_name =DATABASE();";
    }
}
