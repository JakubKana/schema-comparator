using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace DBSchemaComparator.Domain.SqlBuilder
{
    public class MsSqlQueryBuilder : SqlBaseBuilder
    {
        public override Sql GetSqlQueryString(InformationType infoType)
        {
            
            var sqlQuery = Sql.Builder;



            return sqlQuery;
        }
    }
}
