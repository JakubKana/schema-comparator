using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace DBSchemaComparator.ScriptRunner.Deployment
{
    public class MySqlDeploy : IDeployment
    {
        public bool CheckDatabaseExists(Database db, string databaseName)
        {
            throw new NotImplementedException();
        }

        public void CreateDatabase(Database db, string dbName)
        {
            throw new NotImplementedException();
        }

        public void DeleteDatabase(Database db, string dbName)
        {
            throw new NotImplementedException();
        }
    }
}
