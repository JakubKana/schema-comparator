using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.Domain.Database;

namespace DBSchemaComparator.Domain.SqlBuilder
{
    public class SqlBuilderFactory
    {
        public static SqlBaseBuilder Create(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                   return new MsSqlQueryBuilder();
                case DatabaseType.MySql:
                   return new MySqlQueryBuilder();
                case DatabaseType.Unsupported:
                    throw new NotSupportedException();
                default:
                    return new MsSqlQueryBuilder();
            }
            

        }
    }
}
