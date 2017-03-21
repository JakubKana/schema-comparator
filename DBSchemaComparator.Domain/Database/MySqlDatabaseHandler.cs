using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.Domain.Models.SQLServer;
using PetaPoco;

namespace DBSchemaComparator.Domain.Database
{
    public class MySqlDatabaseHandler : BaseDatabase, IDatabaseHandler
    {
        public IList<PrimaryKey> GetPrimaryKeysInfo()
        {
            throw new NotImplementedException();
        }

        public IList<Table> GetTablesSchemaInfo()
        {
            throw new NotImplementedException();
        }

        public IList<StoredProcedure> GetStoredProceduresInfo()
        {
            throw new NotImplementedException();
        }

        public IList<Collation> GetCollationInfo()
        {
            throw new NotImplementedException();
        }

        public IList<CheckConstraint> GetCheckConstraintsInfo()
        {
            throw new NotImplementedException();
        }

        public IList<ForeignKey> GetForeignKeysInfo()
        {
            throw new NotImplementedException();
        }

        public IList<Index> GetIndexesInfo()
        {
            throw new NotImplementedException();
        }

        public IList<Function> GetFunctionsInfo()
        {
            throw new NotImplementedException();
        }

        public IList<View> GetViewsInfo()
        {
            throw new NotImplementedException();
        }

        public IList<Table> SelectTablesSchemaInfo()
        {
            throw new NotImplementedException();
        }

        public IList<Column> SelectColumnsSchemaInfo()
        {
            throw new NotImplementedException();
        }

        public IList<IdentityColumn> SelectColumnsWithIdentity()
        {
            throw new NotImplementedException();
        }

        public IList<T> SelectSchemaInfo<T>(InformationType infoType)
        {
            throw new NotImplementedException();
        }

        public Sql GetSqlQueryString(InformationType infoType)
        {
            throw new NotImplementedException();
        }
    }
}
