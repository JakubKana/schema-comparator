using DBSchemaComparator.Domain.Database;
using PetaPoco;

namespace DBSchemaComparator.Domain.SqlBuilder
{
    interface ISqlQueryBuilder
    {
        Sql GetSqlQueryString(InformationType infoType);
    }

   
}
