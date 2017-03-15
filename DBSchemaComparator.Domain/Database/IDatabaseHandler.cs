using System.Collections;
using System.Collections.Generic;
using DBSchemaComparator.Domain.Models.SQLServer;
using PetaPoco;


namespace DBSchemaComparator.Domain.Database
{
    public interface IDatabaseHandler
    {
        IList<PrimaryKey> GetPrimaryKeysInfo();
        IList<Table> GetTablesSchemaInfo();
        IList<StoredProcedure> GetStoredProceduresInfo();
        IList<Collation> GetCollationInfo();
        IList<CheckConstraint> GetCheckConstraintsInfo();
        IList<ForeignKey> GetForeignKeysInfo();
        IList<Index> GetIndexesInfo();
        IList<Function> GetFunctionsInfo();
        IList<View> GetViewsInfo();
        IList<Table> SelectTablesSchemaInfo();
        IList<Column> SelectColumnsSchemaInfo();
        IList<IdentityColumn> SelectColumnsWithIdentity();
        IList<T> SelectSchemaInfo<T>(InformationType infoType);
        Sql GetSqlQueryString(InformationType infoType);
    }
}