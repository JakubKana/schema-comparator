using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Infrastructure;

namespace DBSchemaComparator.Domain.SqlBuilder
{
    public class SqlBuilderFactory
    {
        public static SqlBaseBuilder Create(DatabaseType dbType, string connString)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                   return new MsSqlQueryBuilder();
                case DatabaseType.MySql:
                    var dbName = Settings.GetMySqlStringBuilder(connString).Database;
                    return new MySqlQueryBuilder(dbName);
                case DatabaseType.Unsupported:
                    throw new NotSupportedException();
                default:
                    return new MsSqlQueryBuilder();
            }
        }
    }
}
