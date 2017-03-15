using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace DBSchemaComparator.Domain.Database
{
    public class BaseDatabase
    {
        private DatabaseType DbType { get; set; }

        public PetaPoco.Database Database
        {
            get; private set;
        }


        protected void CreateDatabaseConnection(string connectionString, DatabaseType databaseType)
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

    }
}
